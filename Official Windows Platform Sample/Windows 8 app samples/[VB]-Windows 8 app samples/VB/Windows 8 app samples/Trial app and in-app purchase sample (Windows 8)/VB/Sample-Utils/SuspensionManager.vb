' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
' 
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.IO
Imports System.Runtime.Serialization
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports System.Collections.Generic
Imports System.Threading.Tasks


Namespace Global.SDKTemplate
    Module SuspensionManager
        Private sessionState_ As New Dictionary(Of String, Object)
        Private Const filename As String = "_sessionState.xml"

        Public ReadOnly Property SessionState As Dictionary(Of String, Object)
                Get
                        Return sessionState_
                End Get
        End Property

        ' Worker to workaround deadlocks.
        Public Async Function SaveAsync() As Task
                Await Windows.System.Threading.ThreadPool.RunAsync(Sub(wiSender) SuspensionManager.SaveImplAsync.Wait(), Windows.System.Threading.WorkItemPriority.Normal)
        End Function

        Private Async Function SaveImplAsync() As Task
            ' Get the output stream for the SessionState file.
            Dim file As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting)


            Using transaction As StorageStreamTransaction = Await file.OpenTransactedWriteAsync
                ' Serialize the Session State.
                Dim serializer As New DataContractSerializer(GetType(Dictionary(Of String, Object)))
                serializer.WriteObject(transaction.Stream.AsStreamForWrite, sessionState_)
                Await transaction.CommitAsync()
            End Using
        End Function

        ' Worker to workaround deadlocks.
        Public Async Function RestoreAsync() As Task
                Await Windows.System.Threading.ThreadPool.RunAsync(Sub(wiSender) SuspensionManager.RestoreImplAsync.Wait(), Windows.System.Threading.WorkItemPriority.Normal)
        End Function

        Private Async Function RestoreImplAsync() As Task
                ' Get the input stream for the SessionState file.
                Dim file As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists)
                If file Is Nothing Then
                        Exit Function
                End If
                Dim inStream As IInputStream = Await file.OpenSequentialReadAsync

                ' Deserialize the Session State.
           Dim serializer As New DataContractSerializer(GetType(Dictionary(Of String, Object)))
                sessionState_ = CType(serializer.ReadObject(inStream.AsStreamforRead), Dictionary(Of String, Object))
        End Function
    End Module
End Namespace