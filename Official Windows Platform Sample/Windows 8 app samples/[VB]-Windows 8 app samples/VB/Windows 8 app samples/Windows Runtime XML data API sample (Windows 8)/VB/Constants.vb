'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Generic
Imports System
Imports System.Threading.Tasks
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage
        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Xml Sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Public Shared _ScenarioList As New List(Of Scenario)() From {New Scenario With {.Title = "Build New RSS", .ClassType = GetType(Global.Xml.BuildNewRss)},
                                                               New Scenario With {.Title = "DOM Load/Save", .ClassType = GetType(Global.Xml.MarkHotProducts)},
                                                               New Scenario With {.Title = "Set Load Settings", .ClassType = GetType(Global.Xml.XmlLoading)},
                                                               New Scenario With {.Title = "XPath Query", .ClassType = GetType(Global.Xml.GiftDispatch)},
                                                               New Scenario With {.Title = "XSLT Transformation", .ClassType = GetType(Global.Xml.XSLTTransform)}
                                                              }
    End Class

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function

        Public Shared ReadOnly Property LoadFileErrorMsg() As String
            Get
                Return m_loadFileErrorMsg
            End Get
        End Property
        Private Shared m_loadFileErrorMsg As String = "Error: Unable to load XML file"

        Public Shared Async Function LoadXmlFile(folder As String, file As String) As Task(Of Windows.Data.Xml.Dom.XmlDocument)
            Dim storageFolder As Windows.Storage.StorageFolder = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folder)
            Dim storageFile As Windows.Storage.StorageFile = Await storageFolder.GetFileAsync(file)
            Dim loadSettings As New Windows.Data.Xml.Dom.XmlLoadSettings()
            loadSettings.ProhibitDtd = False
            loadSettings.ResolveExternals = False
            Return Await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(storageFile, loadSettings)
        End Function

        Public Shared Sub RichEditBoxSetText(richEditBox As RichEditBox, msg As String, color As Windows.UI.Color, fReadOnly As Boolean)
            richEditBox.IsReadOnly = False
            richEditBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, msg)
            richEditBox.Foreground = New SolidColorBrush(color)
            richEditBox.IsReadOnly = fReadOnly
        End Sub

        Public Shared Sub RichEditBoxSetError(richEditBox As RichEditBox, errorMsg As String)
            RichEditBoxSetText(richEditBox, errorMsg, Windows.UI.Colors.Red, True)
        End Sub

        Public Shared Sub RichEditBoxSetMsg(richEditBox As RichEditBox, msg As String, fReadOnly As Boolean)
            RichEditBoxSetText(richEditBox, msg, Windows.UI.Colors.Black, fReadOnly)
        End Sub
    End Class
End Namespace
