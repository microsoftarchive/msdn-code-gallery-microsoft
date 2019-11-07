Imports System
Imports System.Runtime.Serialization
Imports Windows.Storage
Imports Windows.Storage.Streams

' Copyright (c) Microsoft. All rights reserved.

Namespace Global.SDKTemplate
    ''' <summary>
    ''' SuspensionManager captures global session state to simplify process lifetime management
    ''' for an application.  Note that session state will be automatically cleared under a variety
    ''' of conditions and should only be used to store information that would be convenient to
    ''' carry across sessions, but that should be discarded when an application crashes or is
    ''' upgraded.
    ''' </summary>
    Friend NotInheritable Class SuspensionManager
        Private Shared _sessionState As New Dictionary(Of String, Object)()
        Private Shared _knownTypes As New List(Of Type)()
        Private Const sessionStateFilename As String = "_sessionState.xml"

        ''' <summary>
        ''' Provides access to global session state for the current session.  This state is
        ''' serialized by <see cref="SaveAsync"/> and restored by
        ''' <see cref="RestoreAsync"/>, so values must be serializable by
        ''' <see cref="DataContractSerializer"/> and should be as compact as possible.  Strings
        ''' and other self-contained data types are strongly recommended.
        ''' </summary>
        Public Shared ReadOnly Property SessionState() As Dictionary(Of String, Object)
            Get
                Return _sessionState
            End Get
        End Property

        ''' <summary>
        ''' List of custom types provided to the <see cref="DataContractSerializer"/> when
        ''' reading and writing session state.  Initially empty, additional types may be
        ''' added to customize the serialization process.
        ''' </summary>
        Public Shared ReadOnly Property KnownTypes() As List(Of Type)
            Get
                Return _knownTypes
            End Get
        End Property

        ''' <summary>
        ''' Save the current <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
        ''' registered with <see cref="RegisterFrame"/> will also preserve their current
        ''' navigation stack, which in turn gives their active <see cref="Page"/> an opportunity
        ''' to save its state.
        ''' </summary>
        ''' <returns>An asynchronous task that reflects when session state has been saved.</returns>
        Public Shared Async Function SaveAsync() As Task
            Try
                ' Save the navigation state for all registered frames
                For Each weakFrameReference In _registeredFrames
                    Dim frame As New Frame()
                    If weakFrameReference.TryGetTarget(frame) Then
                        SaveFrameNavigationState(frame)
                    End If
                Next weakFrameReference

                ' Serialize the session state synchronously to avoid asynchronous access to shared
                ' state
                Dim sessionData As New MemoryStream()
                Dim serializer As New DataContractSerializer(GetType(Dictionary(Of String, Object)), _knownTypes)
                serializer.WriteObject(sessionData, _sessionState)

                ' Get an output stream for the SessionState file and write the state asynchronously
                Dim file As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting)
                Using fileStream As Stream = Await file.OpenStreamForWriteAsync()
                    sessionData.Seek(0, SeekOrigin.Begin)
                    Await sessionData.CopyToAsync(fileStream)
                End Using
            Catch e As Exception
                Throw New SuspensionManagerException(e)
            End Try
        End Function

        ''' <summary>
        ''' Restores previously saved <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
        ''' registered with <see cref="RegisterFrame"/> will also restore their prior navigation
        ''' state, which in turn gives their active <see cref="Page"/> an opportunity restore its
        ''' state.
        ''' </summary>
        ''' <returns>An asynchronous task that reflects when session state has been read.  The
        ''' content of <see cref="SessionState"/> should not be relied upon until this task
        ''' completes.</returns>
        Public Shared Async Function RestoreAsync() As Task
            _sessionState = New Dictionary(Of String, Object)()

            Try
                ' Get the input stream for the SessionState file
                Dim file As StorageFile = Await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename)
                Using inStream As IInputStream = Await file.OpenSequentialReadAsync()
                    ' Deserialize the Session State
                    Dim serializer As New DataContractSerializer(GetType(Dictionary(Of String, Object)), _knownTypes)
                    _sessionState = CType(serializer.ReadObject(inStream.AsStreamForRead()), Dictionary(Of String, Object))
                End Using

                ' Restore any registered frames to their saved state
                For Each weakFrameReference In _registeredFrames
                    Dim frame As New Frame
                    If weakFrameReference.TryGetTarget(frame) Then
                        frame.ClearValue(FrameSessionStateProperty)
                        RestoreFrameNavigationState(frame)
                    End If
                Next weakFrameReference
            Catch e As Exception
                Throw New SuspensionManagerException(e)
            End Try
        End Function

        Private Shared FrameSessionStateKeyProperty As DependencyProperty = DependencyProperty.RegisterAttached("_FrameSessionStateKey", GetType(String), GetType(SuspensionManager), Nothing)
        Private Shared FrameSessionStateProperty As DependencyProperty = DependencyProperty.RegisterAttached("_FrameSessionState", GetType(Dictionary(Of String, Object)), GetType(SuspensionManager), Nothing)
        Private Shared _registeredFrames As New List(Of WeakReference(Of Frame))()

        ''' <summary>
        ''' Registers a <see cref="Frame"/> instance to allow its navigation history to be saved to
        ''' and restored from <see cref="SessionState"/>.  Frames should be registered once
        ''' immediately after creation if they will participate in session state management.  Upon
        ''' registration if state has already been restored for the specified key
        ''' the navigation history will immediately be restored.  Subsequent invocations of
        ''' <see cref="RestoreAsync"/> will also restore navigation history.
        ''' </summary>
        ''' <param name="frame">An instance whose navigation history should be managed by
        ''' <see cref="SuspensionManager"/></param>
        ''' <param name="sessionStateKey">A unique key into <see cref="SessionState"/> used to
        ''' store navigation-related information.</param>
        Public Shared Sub RegisterFrame(ByVal frame As Frame, ByVal sessionStateKey As String)
            If frame.GetValue(FrameSessionStateKeyProperty) IsNot Nothing Then
                Throw New InvalidOperationException("Frames can only be registered to one session state key")
            End If

            If frame.GetValue(FrameSessionStateProperty) IsNot Nothing Then
                Throw New InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all")
            End If

            ' Use a dependency property to associate the session key with a frame, and keep a list of frames whose
            ' navigation state should be managed
            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey)
            _registeredFrames.Add(New WeakReference(Of Frame)(frame))

            ' Check to see if navigation state can be restored
            RestoreFrameNavigationState(frame)
        End Sub

        ''' <summary>
        ''' Disassociates a <see cref="Frame"/> previously registered by <see cref="RegisterFrame"/>
        ''' from <see cref="SessionState"/>.  Any navigation state previously captured will be
        ''' removed.
        ''' </summary>
        ''' <param name="frame">An instance whose navigation history should no longer be
        ''' managed.</param>
        Public Shared Sub UnregisterFrame(ByVal frame As Frame)
            ' Remove session state and remove the frame from the list of frames whose navigation
            ' state will be saved (along with any weak references that are no longer reachable)
            SessionState.Remove(CStr(frame.GetValue(FrameSessionStateKeyProperty)))
            _registeredFrames.RemoveAll(Function(weakFrameReference)
                                            Dim testFrame As New Frame()
                                            Return (Not weakFrameReference.TryGetTarget(testFrame)) OrElse testFrame Is frame
                                        End Function)
        End Sub

        ''' <summary>
        ''' Provides storage for session state associated with the specified <see cref="Frame"/>.
        ''' Frames that have been previously registered with <see cref="RegisterFrame"/> have
        ''' their session state saved and restored automatically as a part of the global
        ''' <see cref="SessionState"/>.  Frames that are not registered have transient state
        ''' that can still be useful when restoring pages that have been discarded from the
        ''' navigation cache.
        ''' </summary>
        ''' <remarks>Apps may choose to rely on <see cref="LayoutAwarePage"/> to manage
        ''' page-specific state instead of working with frame session state directly.</remarks>
        ''' <param name="frame">The instance for which session state is desired.</param>
        ''' <returns>A collection of state subject to the same serialization mechanism as
        ''' <see cref="SessionState"/>.</returns>
        Public Shared Function SessionStateForFrame(ByVal frame As Frame) As Dictionary(Of String, Object)
            Dim frameState = CType(frame.GetValue(FrameSessionStateProperty), Dictionary(Of String, Object))

            If frameState Is Nothing Then
                Dim frameSessionKey = CStr(frame.GetValue(FrameSessionStateKeyProperty))
                If frameSessionKey IsNot Nothing Then
                    ' Registered frames reflect the corresponding session state
                    If Not _sessionState.ContainsKey(frameSessionKey) Then
                        _sessionState(frameSessionKey) = New Dictionary(Of String, Object)()
                    End If
                    frameState = CType(_sessionState(frameSessionKey), Dictionary(Of String, Object))
                Else
                    ' Frames that aren't registered have transient state
                    frameState = New Dictionary(Of String, Object)()
                End If
                frame.SetValue(FrameSessionStateProperty, frameState)
            End If
            Return frameState
        End Function

        Private Shared Sub RestoreFrameNavigationState(ByVal frame As Frame)
            Dim frameState = SessionStateForFrame(frame)
            If frameState.ContainsKey("Navigation") Then
                frame.SetNavigationState(CStr(frameState("Navigation")))
            End If
        End Sub

        Private Shared Sub SaveFrameNavigationState(ByVal frame As Frame)
            Dim frameState = SessionStateForFrame(frame)
            frameState("Navigation") = frame.GetNavigationState()
        End Sub
    End Class

    Public Class SuspensionManagerException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal e As Exception)
            MyBase.New("SuspensionManager failed", e)

        End Sub
    End Class
End Namespace
