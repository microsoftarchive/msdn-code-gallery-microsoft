'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports System.Threading
Imports System.IO
Imports System.IO.IsolatedStorage


Partial Public Class App
    Inherits Application
    ''' <summary>
    ''' Provides easy access to the root frame of the Phone Application.
    ''' </summary>
    ''' <returns>The root frame of the Phone Application.</returns>
    Public Property RootFrame() As PhoneApplicationFrame


    ''' <summary>
    ''' Constructor for the Application object.
    ''' </summary>
    Public Sub New()
        ' Global handler for uncaught exceptions. 
        AddHandler UnhandledException, AddressOf Application_UnhandledException

        ' Standard Silverlight initialization
        InitializeComponent()

        ' Phone-specific initialization
        InitializePhoneApplication()

        ' Show graphics profiling information while debugging.
        If System.Diagnostics.Debugger.IsAttached Then

            ' Use #if DEBUG so that the below counters only show up on the
            ' screen in the Debug build.
#If DEBUG Then
            ' Display the current frame rate counters.
            Application.Current.Host.Settings.EnableFrameRateCounter = True

            ' Show the areas of the app that are being redrawn in each frame.
            'Application.Current.Host.Settings.EnableRedrawRegions = true

            ' Enable non-production analysis visualization mode, 
            ' which shows areas of a page that are handed off to GPU with a colored overlay.
            'Application.Current.Host.Settings.EnableCacheVisualization = true
#End If

            ' Disable the application idle detection by setting the UserIdleDetectionMode property of the
            ' application's PhoneApplicationService object to Disabled.
            ' Caution:- Use this under debug mode only. Application that disable user idle detection will continue to run
            ' and consume battery power when the user is not using the phone.
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled
        End If

    End Sub

    ' Code to execute when the application is launching (eg, from Start)
    ' This code will not execute when the application is reactivated
    Private Sub Application_Launching(ByVal sender As Object, ByVal e As LaunchingEventArgs)

    End Sub

    ' Code to execute when the application is activated (brought to foreground)
    ' This code will not execute when the application is first launched
    Private Sub Application_Activated(ByVal sender As Object, ByVal e As ActivatedEventArgs)
        If e.IsApplicationInstancePreserved Then
            Return
        End If

        ' Check to see if the key for the application state data is in the State dictionary.
        If PhoneApplicationService.Current.State.ContainsKey("ApplicationDataObject") Then
            ' If it exists, assign the data to the application member variable.
            Dim data As String = TryCast(PhoneApplicationService.Current.State("ApplicationDataObject"), String)
            DeSerializeSettings(data)
        End If

    End Sub

    ' Code to execute when the application is deactivated (sent to background)
    ' This code will not execute when the application is closing
    Private Sub Application_Deactivated(ByVal sender As Object, ByVal e As DeactivatedEventArgs)
        ' Serialize the settings into a string that can be easily stored
        Dim settings As String = SerializeSettings()

        ' Store it in the State dictionary.
        PhoneApplicationService.Current.State("ApplicationDataObject") = settings

        ' Also store it in Isolated Storage, in case the application is never reactivated.
        SaveDataToIsolatedStorage("myDataFile.txt", settings)

    End Sub

    ' Code to execute when the application is closing (eg, user hit Back)
    ' This code will not execute when the application is deactivated
    Private Sub Application_Closing(ByVal sender As Object, ByVal e As ClosingEventArgs)
        ' The application will not be tombstoned, so only save to Isolated Storage

        ' Serialize the settings into a string that can be easily stored
        Dim settings As String = SerializeSettings()

        ' Also store it in Isolated Storage, in case the application is never reactivated.
        SaveDataToIsolatedStorage("myDataFile.txt", settings)

    End Sub

    ' Code to execute if a navigation fails
    Private Sub RootFrame_NavigationFailed(ByVal sender As Object, ByVal e As NavigationFailedEventArgs)
        If System.Diagnostics.Debugger.IsAttached Then
            ' A navigation has failed; break into the debugger
            System.Diagnostics.Debugger.Break()
        End If
    End Sub

    ' Code to execute on Unhandled Exceptions
    Private Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs)
        If System.Diagnostics.Debugger.IsAttached Then
            ' An unhandled exception has occurred; break into the debugger
            System.Diagnostics.Debugger.Break()
        End If
    End Sub

#Region "Phone application initialization"

    ' Avoid double-initialization
    Private phoneApplicationInitialized As Boolean = False

    ' Do not add any additional code to this method
    Private Sub InitializePhoneApplication()
        If phoneApplicationInitialized Then
            Return
        End If

        ' Create the frame but don't set it as RootVisual yet; this allows the splash
        ' screen to remain active until the application is ready to render.
        RootFrame = New PhoneApplicationFrame()
        AddHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication

        ' Handle navigation failures
        AddHandler RootFrame.NavigationFailed, AddressOf RootFrame_NavigationFailed

        ' Ensure we don't initialize again
        phoneApplicationInitialized = True
    End Sub

    ' Do not add any additional code to this method
    Private Sub CompleteInitializePhoneApplication(ByVal sender As Object, ByVal e As NavigationEventArgs)
        ' Set the root visual to allow the application to render
        If RootVisual IsNot RootFrame Then
            RootVisual = RootFrame
        End If

        ' Remove this handler since it is no longer needed
        RemoveHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication
    End Sub

#End Region

#Region "Settings Management"
    ' Declare an event for when the application data changes.
    Public Event ApplicationDataObjectChanged As EventHandler

    ' Declare a private variable to store the host (server) name
    Private _hostName As String

    ' Declare a public property to access the application data variable.
    Public Property HostName() As String
        Get
            Return _hostName
        End Get
        Set(ByVal value As String)
            If value <> _hostName Then
                _hostName = value
                OnApplicationDataObjectChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    ' Declare a private variable to store the port number used by the application
    ' NOTE: The port number in this application and the port number in the server must match.
    ' Remember to open the port you choose on the computer running the server.
    Private _portNumber As Integer = 0

    ' Declare a public property to access the application data variable.
    Public Property PortNumber() As Integer
        Get
            Return _portNumber
        End Get
        Set(ByVal value As Integer)
            If value <> _portNumber Then
                _portNumber = value
                OnApplicationDataObjectChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    ' Declare a private variable to store whether the client is playing a
    ' 'X' or as 'O'
    Private _playAsX As Boolean = True

    ' Declare a public property to access the application data variable.
    Public Property PlayAsX() As Boolean
        Get
            Return _playAsX
        End Get
        Set(ByVal value As Boolean)
            If value <> _playAsX Then
                _playAsX = value
                OnApplicationDataObjectChanged(EventArgs.Empty)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Produce a string that contains all the settings used by the application
    ''' </summary>
    ''' <returns>The string containing the serialized data</returns>
    Private Function SerializeSettings() As String
        ' This string will be of the form
        ' PlayAsX|Hostname|PortNumber

        Dim result As String = String.Empty

        result &= _playAsX.ToString()
        result &= "|"
        result &= _hostName
        result &= "|"
        result &= _portNumber.ToString()

        Return result
    End Function

    ''' <summary>
    ''' Given a string of serialized settings, re-hydrate each settings variable
    ''' </summary>
    ''' <param name="data">The string of serialized settings</param>
    Private Sub DeSerializeSettings(ByVal data As String)
        ' Split the string using the '|' delimiter
        Dim values() As String = data.Split("|".ToCharArray(), StringSplitOptions.None)

        ' The string is of the form
        ' PlayAsX|Hostname|PortNumber

        PlayAsX = Convert.ToBoolean(values(0))
        HostName = values(1)
        PortNumber = Convert.ToInt32(values(2))
    End Sub

    ' Create a method to raise the ApplicationDataObjectChanged event.
    Protected Sub OnApplicationDataObjectChanged(ByVal e As EventArgs)
        Dim handler As EventHandler = ApplicationDataObjectChangedEvent
        If handler IsNot Nothing Then
            handler(Me, e)
        End If
    End Sub

    ''' <summary>
    ''' Save data to a file in IsolatedStorage
    ''' </summary>
    ''' <param name="isoFileName">The name of the file</param>
    ''' <param name="value">The string value to write to the given file</param>
    Private Sub SaveDataToIsolatedStorage(ByVal isoFileName As String, ByVal value As String)
        Using isoStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
            Dim sw As New StreamWriter(isoStore.OpenFile(isoFileName, FileMode.Create))
            sw.Write(value)
            sw.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Call GetData on a different thread, i.e, asynchronously
    ''' </summary>
    Public Sub GetDataAsync()
        ' Call the GetData method on a new thread.
        Dim t As New Thread(New ThreadStart(AddressOf GetData))
        t.Start()
    End Sub

    ''' <summary>
    ''' Retrieve data from IsolatedStorage
    ''' </summary>
    Private Sub GetData()
        ' Check to see if data exists in Isolated Storage 
        Dim isoStore As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
        If isoStore.FileExists("myDataFile.txt") Then
            ' This method loads the data from Isolated Storage, if it is available.
            Dim sr As New StreamReader(isoStore.OpenFile("myDataFile.txt", FileMode.Open))
            Dim data As String = sr.ReadToEnd()

            sr.Close()

            DeSerializeSettings(data)
        End If
    End Sub
#End Region
End Class
