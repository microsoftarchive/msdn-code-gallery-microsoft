' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Text

#If Not WINRT_NOT_PRESENT Then
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
#End If

Namespace TileContent
    Friend NotInheritable Class TileUtil

        Private Sub New()
        End Sub

        Public Const NOTIFICATION_CONTENT_VERSION As Integer = 2
    End Class

    Friend MustInherit Class TileNotificationBase
        Inherits NotificationBase

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, fallbackName, imageCount, textCount)
        End Sub

        Public Property Branding() As TileBranding
            Get
                Return m_Branding
            End Get
            Set(ByVal value As TileBranding)
                If Not System.Enum.IsDefined(GetType(TileBranding), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Branding = value
            End Set
        End Property

        Public Property ContentId() As String
            Get
                Return m_ContentId
            End Get
            Set(ByVal value As String)
                m_ContentId = value
            End Set
        End Property

#If Not WINRT_NOT_PRESENT Then
        Public Overridable Function CreateNotification() As TileNotification
            Dim xmlDoc As New XmlDocument()
            xmlDoc.LoadXml(GetContent())
            Return New TileNotification(xmlDoc)
        End Function
#End If

        Private m_Branding As TileBranding = TileBranding.Logo
        Private m_ContentId As String = Nothing
    End Class

    Friend Interface ISquare150x150TileInternal
        Function SerializeBinding(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String
    End Interface

    Friend Class TileSquare150x150Base
        Inherits TileNotificationBase
        Implements ISquare150x150TileInternal

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, fallbackName, imageCount, textCount)
        End Sub

        Public Overrides Function GetContent() As String
            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile><visual version='{0}'", TileUtil.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                builder.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                builder.AppendFormat(" addImageQuery='true'")
            End If
            builder.Append(">")
            builder.Append(SerializeBinding(Lang, BaseUri, Branding, AddImageQuery))
            builder.Append("</visual></tile>")
            Return builder.ToString()
        End Function

        Public Function SerializeBinding(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String Implements ISquare150x150TileInternal.SerializeBinding
            Dim bindingNode As New StringBuilder(String.Empty)
            bindingNode.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                bindingNode.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            If (Not String.IsNullOrWhiteSpace(Lang)) AndAlso (Not Lang.Equals(globalLang)) Then
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
                globalLang = Lang
            End If
            If Branding <> TileBranding.Logo AndAlso Branding <> globalBranding Then
                bindingNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If (Not String.IsNullOrWhiteSpace(BaseUri)) AndAlso (Not BaseUri.Equals(globalBaseUri)) Then
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
                globalBaseUri = BaseUri
            End If
            If AddImageQueryNullable IsNot Nothing AndAlso Not AddImageQueryNullable.Equals(globalAddImageQuery) Then
                bindingNode.AppendFormat(" addImageQuery='{0}'", AddImageQuery.ToString().ToLowerInvariant())
                globalAddImageQuery = AddImageQuery
            End If
            If Not String.IsNullOrWhiteSpace(ContentId) Then
                bindingNode.AppendFormat(" contentId='{0}'", ContentId.ToLowerInvariant())
            End If
            bindingNode.AppendFormat(">{0}</binding>", SerializeProperties(globalLang, globalBaseUri, globalAddImageQuery))

            Return bindingNode.ToString()
        End Function
    End Class

    Friend Interface IWide310x150TileInternal
        Function SerializeBindings(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String
    End Interface

    Friend Class TileWide310x150Base
        Inherits TileNotificationBase
        Implements IWide310x150TileInternal

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, fallbackName, imageCount, textCount)
        End Sub

        Public Property Square150x150Content() As ISquare150x150TileNotificationContent
            Get
                Return m_Square150x150Content
            End Get
            Set(ByVal value As ISquare150x150TileNotificationContent)
                m_Square150x150Content = value
            End Set
        End Property

        Public Property RequireSquare150x150Content() As Boolean
            Get
                Return m_RequireSquare150x150Content
            End Get
            Set(ByVal value As Boolean)
                m_RequireSquare150x150Content = value
            End Set
        End Property

        Public Overrides Function GetContent() As String
            If RequireSquare150x150Content AndAlso Square150x150Content Is Nothing Then
                Throw New NotificationContentValidationException("Square150x150 tile content should be included with each wide tile. " & "If this behavior is undesired, use the RequireSquare150x150Content property.")
            End If

            Dim visualNode As New StringBuilder(String.Empty)
            visualNode.AppendFormat("<visual version='{0}'", TileUtil.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                visualNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                visualNode.AppendFormat(" addImageQuery='true'")
            End If
            visualNode.Append(">")

            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile>{0}", visualNode)
            If Square150x150Content IsNot Nothing Then
                Dim squareBase As ISquare150x150TileInternal = TryCast(Square150x150Content, ISquare150x150TileInternal)
                If squareBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided square tile content class is unsupported.")
                End If
                builder.Append(squareBase.SerializeBinding(Lang, BaseUri, Branding, AddImageQuery))
            End If
            builder.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                builder.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            builder.AppendFormat(">{0}</binding></visual></tile>", SerializeProperties(Lang, BaseUri, AddImageQuery))
            Return builder.ToString()
        End Function

        Public Function SerializeBindings(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String Implements IWide310x150TileInternal.SerializeBindings
            Dim bindingNode As New StringBuilder(String.Empty)
            If Square150x150Content IsNot Nothing Then
                Dim squareBase As ISquare150x150TileInternal = TryCast(Square150x150Content, ISquare150x150TileInternal)
                If squareBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided square tile content class is unsupported.")
                End If
                bindingNode.Append(squareBase.SerializeBinding(Lang, BaseUri, Branding, AddImageQuery))
            End If

            bindingNode.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                bindingNode.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            If (Not String.IsNullOrWhiteSpace(Lang)) AndAlso (Not Lang.Equals(globalLang)) Then
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
                globalLang = Lang
            End If
            If Branding <> TileBranding.Logo AndAlso Branding <> globalBranding Then
                bindingNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If (Not String.IsNullOrWhiteSpace(BaseUri)) AndAlso (Not BaseUri.Equals(globalBaseUri)) Then
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
                globalBaseUri = BaseUri
            End If
            If AddImageQueryNullable IsNot Nothing AndAlso Not AddImageQueryNullable.Equals(globalAddImageQuery) Then
                bindingNode.AppendFormat(" addImageQuery='{0}'", AddImageQuery.ToString().ToLowerInvariant())
                globalAddImageQuery = AddImageQuery
            End If
            If Not String.IsNullOrWhiteSpace(ContentId) Then
                bindingNode.AppendFormat(" contentId='{0}'", ContentId.ToLowerInvariant())
            End If
            bindingNode.AppendFormat(">{0}</binding>", SerializeProperties(globalLang, globalBaseUri, globalAddImageQuery))

            Return bindingNode.ToString()
        End Function

        Private m_Square150x150Content As ISquare150x150TileNotificationContent = Nothing
        Private m_RequireSquare150x150Content As Boolean = True
    End Class

    Friend Class TileSquare310x310Base
        Inherits TileNotificationBase

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, Nothing, imageCount, textCount)
        End Sub

        Public Property Wide310x150Content() As IWide310x150TileNotificationContent
            Get
                Return m_Wide310x150Content
            End Get
            Set(ByVal value As IWide310x150TileNotificationContent)
                m_Wide310x150Content = value
            End Set
        End Property

        Public Property RequireWide310x150Content() As Boolean
            Get
                Return m_RequireWide310x150Content
            End Get
            Set(ByVal value As Boolean)
                m_RequireWide310x150Content = value
            End Set
        End Property

        Public Overrides Function GetContent() As String
            If RequireWide310x150Content AndAlso Wide310x150Content Is Nothing Then
                Throw New NotificationContentValidationException("Wide310x150 tile content should be included with each large tile. " & "If this behavior is undesired, use the RequireWide310x150Content property.")
            End If

            If Wide310x150Content IsNot Nothing AndAlso Wide310x150Content.RequireSquare150x150Content AndAlso Wide310x150Content.Square150x150Content Is Nothing Then
                Throw New NotificationContentValidationException("This tile's wide content requires square content. " & "If this behavior is undesired, use the Wide310x150Content.RequireSquare150x150Content property.")
            End If

            Dim visualNode As New StringBuilder(String.Empty)
            visualNode.AppendFormat("<visual version='{0}'", TileUtil.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                visualNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                visualNode.AppendFormat(" addImageQuery='true'")
            End If
            visualNode.Append(">")

            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile>{0}", visualNode)
            If Wide310x150Content IsNot Nothing Then
                Dim wideBase As IWide310x150TileInternal = TryCast(Wide310x150Content, IWide310x150TileInternal)
                If wideBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided wide tile content class is unsupported.")
                End If
                builder.Append(wideBase.SerializeBindings(Lang, BaseUri, Branding, AddImageQuery))
            End If
            builder.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                builder.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            If Not String.IsNullOrWhiteSpace(ContentId) Then
                builder.AppendFormat(" contentId='{0}'", ContentId.ToLowerInvariant())
            End If
            builder.AppendFormat(">{0}</binding></visual></tile>", SerializeProperties(Lang, BaseUri, AddImageQuery))

            Return builder.ToString()
        End Function

        Private m_Wide310x150Content As IWide310x150TileNotificationContent = Nothing
        Private m_RequireWide310x150Content As Boolean = True
    End Class

    Friend Interface ISquare99x99TileInternal
        Function SerializeBinding(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String
    End Interface

    Friend Class TileSquare99x99Base
        Inherits TileNotificationBase
        Implements ISquare99x99TileInternal

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, fallbackName, imageCount, textCount)
        End Sub

        Public Overrides Function GetContent() As String
            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile><visual version='{0}'", TileUtil.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                builder.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                builder.AppendFormat(" addImageQuery='true'")
            End If
            builder.Append(">")
            builder.Append(SerializeBinding(Lang, BaseUri, Branding, AddImageQuery))
            builder.Append("</visual></tile>")
            Return builder.ToString()
        End Function

        Public Function SerializeBinding(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String Implements ISquare99x99TileInternal.SerializeBinding
            Dim bindingNode As New StringBuilder(String.Empty)
            bindingNode.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                bindingNode.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            If (Not String.IsNullOrWhiteSpace(Lang)) AndAlso (Not Lang.Equals(globalLang)) Then
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
                globalLang = Lang
            End If
            If Branding <> TileBranding.Logo AndAlso Branding <> globalBranding Then
                bindingNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If (Not String.IsNullOrWhiteSpace(BaseUri)) AndAlso (Not BaseUri.Equals(globalBaseUri)) Then
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
                globalBaseUri = BaseUri
            End If
            If AddImageQueryNullable IsNot Nothing AndAlso Not AddImageQueryNullable.Equals(globalAddImageQuery) Then
                bindingNode.AppendFormat(" addImageQuery='{0}'", AddImageQuery.ToString().ToLowerInvariant())
                globalAddImageQuery = AddImageQuery
            End If
            bindingNode.AppendFormat(">{0}</binding>", SerializeProperties(globalLang, globalBaseUri, globalAddImageQuery))

            Return bindingNode.ToString()
        End Function
    End Class

    Friend Interface ISquare210x210TileInternal
        Function SerializeBindings(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String
    End Interface

    Friend Class TileSquare210x210Base
        Inherits TileNotificationBase
        Implements ISquare210x210TileInternal

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, fallbackName, imageCount, textCount)
        End Sub

        Public Property Square99x99Content() As ISquare99x99TileNotificationContent
            Get
                Return m_Square99x99Content
            End Get
            Set(ByVal value As ISquare99x99TileNotificationContent)
                m_Square99x99Content = value
            End Set
        End Property

        Public Property RequireSquare99x99Content() As Boolean
            Get
                Return m_RequireSquare99x99Content
            End Get
            Set(ByVal value As Boolean)
                m_RequireSquare99x99Content = value
            End Set
        End Property

        Public Overrides Function GetContent() As String
            If RequireSquare99x99Content AndAlso Square99x99Content Is Nothing Then
                Throw New NotificationContentValidationException("Square99x99 tile content should be included with each medium tile. " & "If this behavior is undesired, use the RequireSquare99x99Content property.")
            End If

            Dim visualNode As New StringBuilder(String.Empty)
            visualNode.AppendFormat("<visual version='{0}'", TileUtil.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                visualNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                visualNode.AppendFormat(" addImageQuery='true'")
            End If
            visualNode.Append(">")

            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile>{0}", visualNode)
            If Square99x99Content IsNot Nothing Then
                Dim smallTileBase As ISquare99x99TileInternal = TryCast(Square99x99Content, ISquare99x99TileInternal)
                If smallTileBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided small tile content class is unsupported.")
                End If
                builder.Append(smallTileBase.SerializeBinding(Lang, BaseUri, Branding, AddImageQuery))
            End If
            builder.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                builder.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            builder.AppendFormat(">{0}</binding></visual></tile>", SerializeProperties(Lang, BaseUri, AddImageQuery))
            Return builder.ToString()
        End Function

        Public Function SerializeBindings(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalBranding As TileBranding, ByVal globalAddImageQuery As Boolean) As String Implements ISquare210x210TileInternal.SerializeBindings
            Dim bindingNode As New StringBuilder(String.Empty)
            If Square99x99Content IsNot Nothing Then
                Dim smallTileBase As ISquare99x99TileInternal = TryCast(Square99x99Content, ISquare99x99TileInternal)
                If smallTileBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided small tile content class is unsupported.")
                End If
                bindingNode.Append(smallTileBase.SerializeBinding(Lang, BaseUri, Branding, AddImageQuery))
            End If

            bindingNode.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                bindingNode.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            If (Not String.IsNullOrWhiteSpace(Lang)) AndAlso (Not Lang.Equals(globalLang)) Then
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
                globalLang = Lang
            End If
            If Branding <> TileBranding.Logo AndAlso Branding <> globalBranding Then
                bindingNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If (Not String.IsNullOrWhiteSpace(BaseUri)) AndAlso (Not BaseUri.Equals(globalBaseUri)) Then
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
                globalBaseUri = BaseUri
            End If
            If AddImageQueryNullable IsNot Nothing AndAlso Not AddImageQueryNullable.Equals(globalAddImageQuery) Then
                bindingNode.AppendFormat(" addImageQuery='{0}'", AddImageQuery.ToString().ToLowerInvariant())
                globalAddImageQuery = AddImageQuery
            End If
            bindingNode.AppendFormat(">{0}</binding>", SerializeProperties(globalLang, globalBaseUri, globalAddImageQuery))

            Return bindingNode.ToString()
        End Function

        Private m_Square99x99Content As ISquare99x99TileNotificationContent = Nothing
        Private m_RequireSquare99x99Content As Boolean = True
    End Class

    Friend Class TileWide432x210Base
        Inherits TileNotificationBase

        Public Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName, Nothing, imageCount, textCount)
        End Sub

        Public Property Square210x210Content() As ISquare210x210TileNotificationContent
            Get
                Return m_Square210x210Content
            End Get
            Set(ByVal value As ISquare210x210TileNotificationContent)
                m_Square210x210Content = value
            End Set
        End Property

        Public Property RequireSquare210x210Content() As Boolean
            Get
                Return m_RequireSquare210x210Content
            End Get
            Set(ByVal value As Boolean)
                m_RequireSquare210x210Content = value
            End Set
        End Property

        Public Overrides Function GetContent() As String
            If RequireSquare210x210Content AndAlso Square210x210Content Is Nothing Then
                Throw New NotificationContentValidationException("Square210x210 tile content should be included with each large tile. " & "If this behavior is undesired, use the RequireSquare210x210Content property.")
            End If

            If Square210x210Content IsNot Nothing AndAlso Square210x210Content.RequireSquare99x99Content AndAlso Square210x210Content.Square99x99Content Is Nothing Then
                Throw New NotificationContentValidationException("This tile's medium tile content requires small tile content. " & "If this behavior is undesired, use the Square210x210Content.RequireSquare99x99Content property.")
            End If

            Dim visualNode As New StringBuilder(String.Empty)
            visualNode.AppendFormat("<visual version='{0}'", TileUtil.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                visualNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                visualNode.AppendFormat(" addImageQuery='true'")
            End If
            visualNode.Append(">")

            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile>{0}", visualNode)
            If Square210x210Content IsNot Nothing Then
                Dim mediumTileBase As ISquare210x210TileInternal = TryCast(Square210x210Content, ISquare210x210TileInternal)
                If mediumTileBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided medium tile content class is unsupported.")
                End If
                builder.Append(mediumTileBase.SerializeBindings(Lang, BaseUri, Branding, AddImageQuery))
            End If
            builder.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(FallbackName) Then
                builder.AppendFormat(" fallback='{0}'", FallbackName)
            End If
            builder.AppendFormat(">{0}</binding></visual></tile>", SerializeProperties(Lang, BaseUri, AddImageQuery))

            Return builder.ToString()
        End Function

        Private m_Square210x210Content As ISquare210x210TileNotificationContent = Nothing
        Private m_RequireSquare210x210Content As Boolean = True
    End Class

    Friend Class TileSquare150x150Block
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150Block

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150Block", fallbackName:="TileSquareBlock", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileSquare150x150Block.TextBlock
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileSquare150x150Block.TextSubBlock
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150Image
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150Image

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150Image", fallbackName:="TileSquareImage", imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare150x150Image.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150PeekImageAndText01
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150PeekImageAndText01

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150PeekImageAndText01", fallbackName:="TileSquarePeekImageAndText01", imageCount:=1, textCount:=4)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare150x150PeekImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare150x150PeekImageAndText01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare150x150PeekImageAndText01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare150x150PeekImageAndText01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare150x150PeekImageAndText01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150PeekImageAndText02
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150PeekImageAndText02

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150PeekImageAndText02", fallbackName:="TileSquarePeekImageAndText02", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare150x150PeekImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare150x150PeekImageAndText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquare150x150PeekImageAndText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150PeekImageAndText03
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150PeekImageAndText03

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150PeekImageAndText03", fallbackName:="TileSquarePeekImageAndText03", imageCount:=1, textCount:=4)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare150x150PeekImageAndText03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare150x150PeekImageAndText03.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare150x150PeekImageAndText03.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare150x150PeekImageAndText03.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquare150x150PeekImageAndText03.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150PeekImageAndText04
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150PeekImageAndText04

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150PeekImageAndText04", fallbackName:="TileSquarePeekImageAndText04", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare150x150PeekImageAndText04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquare150x150PeekImageAndText04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150Text01
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150Text01

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150Text01", fallbackName:="TileSquareText01", imageCount:=0, textCount:=4)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare150x150Text01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare150x150Text01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare150x150Text01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare150x150Text01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150Text02
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150Text02

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150Text02", fallbackName:="TileSquareText02", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare150x150Text02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquare150x150Text02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150Text03
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150Text03

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150Text03", fallbackName:="TileSquareText03", imageCount:=0, textCount:=4)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare150x150Text03.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare150x150Text03.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare150x150Text03.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquare150x150Text03.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare150x150Text04
        Inherits TileSquare150x150Base
        Implements ITileSquare150x150Text04

        Public Sub New()
            MyBase.New(templateName:="TileSquare150x150Text04", fallbackName:="TileSquareText04", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquare150x150Text04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150BlockAndText01
        Inherits TileWide310x150Base
        Implements ITileWide310x150BlockAndText01

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150BlockAndText01", fallbackName:="TileWideBlockAndText01", imageCount:=0, textCount:=6)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150BlockAndText01.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150BlockAndText01.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150BlockAndText01.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150BlockAndText01.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileWide310x150BlockAndText01.TextBlock
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileWide310x150BlockAndText01.TextSubBlock
            Get
                Return TextFields(5)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150BlockAndText02
        Inherits TileWide310x150Base
        Implements ITileWide310x150BlockAndText02

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150BlockAndText02", fallbackName:="TileWideBlockAndText02", imageCount:=0, textCount:=6)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150BlockAndText02.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileWide310x150BlockAndText02.TextBlock
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileWide310x150BlockAndText02.TextSubBlock
            Get
                Return TextFields(2)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Image
        Inherits TileWide310x150Base
        Implements ITileWide310x150Image

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Image", fallbackName:="TileWideImage", imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150Image.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150ImageAndText01
        Inherits TileWide310x150Base
        Implements ITileWide310x150ImageAndText01

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150ImageAndText01", fallbackName:="TileWideImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150ImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextCaptionWrap() As INotificationContentText Implements ITileWide310x150ImageAndText01.TextCaptionWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150ImageAndText02
        Inherits TileWide310x150Base
        Implements ITileWide310x150ImageAndText02

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150ImageAndText02", fallbackName:="TileWideImageAndText02", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150ImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextCaption1() As INotificationContentText Implements ITileWide310x150ImageAndText02.TextCaption1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextCaption2() As INotificationContentText Implements ITileWide310x150ImageAndText02.TextCaption2
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150ImageCollection
        Inherits TileWide310x150Base
        Implements ITileWide310x150ImageCollection

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150ImageCollection", fallbackName:="TileWideImageCollection", imageCount:=5, textCount:=0)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImage01
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImage01

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImage01", fallbackName:="TileWidePeekImage01", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150PeekImage01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150PeekImage01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImage01.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImage02
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImage02

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImage02", fallbackName:="TileWidePeekImage02", imageCount:=1, textCount:=5)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150PeekImage02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150PeekImage02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150PeekImage02.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150PeekImage02.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150PeekImage02.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150PeekImage02.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImage03
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImage03

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImage03", fallbackName:="TileWidePeekImage03", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150PeekImage03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWide310x150PeekImage03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImage04
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImage04

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImage04", fallbackName:="TileWidePeekImage04", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150PeekImage04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImage04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImage05
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImage05

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImage05", fallbackName:="TileWidePeekImage05", imageCount:=2, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWide310x150PeekImage05.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWide310x150PeekImage05.ImageSecondary
            Get
                Return Images(1)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150PeekImage05.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImage05.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImage06
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImage06

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImage06", fallbackName:="TileWidePeekImage06", imageCount:=2, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWide310x150PeekImage06.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWide310x150PeekImage06.ImageSecondary
            Get
                Return Images(1)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWide310x150PeekImage06.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImageAndText01
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageAndText01

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageAndText01", fallbackName:="TileWidePeekImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150PeekImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImageAndText01.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImageAndText02
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageAndText02

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageAndText02", fallbackName:="TileWidePeekImageAndText02", imageCount:=1, textCount:=5)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150PeekImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150PeekImageAndText02.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150PeekImageAndText02.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150PeekImageAndText02.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150PeekImageAndText02.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileWide310x150PeekImageAndText02.TextBody5
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150PeekImageCollection01
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageCollection01

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageCollection01", fallbackName:="TileWidePeekImageCollection01", imageCount:=5, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150PeekImageCollection01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImageCollection01.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireSquare150x150Content As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content

        Public Overloads Property Square150x150Content As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content

    End Class

    Friend Class TileWide310x150PeekImageCollection02
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageCollection02

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageCollection02", fallbackName:="TileWidePeekImageCollection02", imageCount:=5, textCount:=5)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150PeekImageCollection02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150PeekImageCollection02.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150PeekImageCollection02.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150PeekImageCollection02.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150PeekImageCollection02.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireSquare150x150Content As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content

        Public Overloads Property Square150x150Content As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content

    End Class

    Friend Class TileWide310x150PeekImageCollection03
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageCollection03

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageCollection03", fallbackName:="TileWidePeekImageCollection03", imageCount:=5, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWide310x150PeekImageCollection03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireSquare150x150Content As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content

        Public Overloads Property Square150x150Content As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content

    End Class

    Friend Class TileWide310x150PeekImageCollection04
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageCollection04

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageCollection04", fallbackName:="TileWidePeekImageCollection04", imageCount:=5, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImageCollection04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireSquare150x150Content As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content

        Public Overloads Property Square150x150Content As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content

    End Class

    Friend Class TileWide310x150PeekImageCollection05
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageCollection05

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageCollection05", fallbackName:="TileWidePeekImageCollection05", imageCount:=6, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWide310x150PeekImageCollection05.ImageSecondary
            Get
                Return Images(5)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150PeekImageCollection05.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150PeekImageCollection05.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireSquare150x150Content As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content

        Public Overloads Property Square150x150Content As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content

    End Class

    Friend Class TileWide310x150PeekImageCollection06
        Inherits TileWide310x150Base
        Implements ITileWide310x150PeekImageCollection06

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150PeekImageCollection06", fallbackName:="TileWidePeekImageCollection06", imageCount:=6, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements NotificationsExtensions.TileContent.ITileWide310x150ImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWide310x150PeekImageCollection06.ImageSecondary
            Get
                Return Images(5)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWide310x150PeekImageCollection06.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireSquare150x150Content As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content

        Public Overloads Property Square150x150Content As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content

    End Class

    Friend Class TileWide310x150SmallImageAndText01
        Inherits TileWide310x150Base
        Implements ITileWide310x150SmallImageAndText01

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150SmallImageAndText01", fallbackName:="TileWideSmallImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150SmallImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWide310x150SmallImageAndText01.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150SmallImageAndText02
        Inherits TileWide310x150Base
        Implements ITileWide310x150SmallImageAndText02

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150SmallImageAndText02", fallbackName:="TileWideSmallImageAndText02", imageCount:=1, textCount:=5)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150SmallImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150SmallImageAndText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150SmallImageAndText02.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150SmallImageAndText02.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150SmallImageAndText02.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150SmallImageAndText02.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150SmallImageAndText03
        Inherits TileWide310x150Base
        Implements ITileWide310x150SmallImageAndText03

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150SmallImageAndText03", fallbackName:="TileWideSmallImageAndText03", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150SmallImageAndText03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150SmallImageAndText03.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150SmallImageAndText04
        Inherits TileWide310x150Base
        Implements ITileWide310x150SmallImageAndText04

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150SmallImageAndText04", fallbackName:="TileWideSmallImageAndText04", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150SmallImageAndText04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150SmallImageAndText04.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150SmallImageAndText04.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150SmallImageAndText05
        Inherits TileWide310x150Base
        Implements ITileWide310x150SmallImageAndText05

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150SmallImageAndText05", fallbackName:="TileWideSmallImageAndText05", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWide310x150SmallImageAndText05.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150SmallImageAndText05.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150SmallImageAndText05.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text01
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text01

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text01", fallbackName:="TileWideText01", imageCount:=0, textCount:=5)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150Text01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150Text01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150Text01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150Text01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150Text01.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text02
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text02

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text02", fallbackName:="TileWideText02", imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150Text02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements ITileWide310x150Text02.TextColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWide310x150Text02.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements ITileWide310x150Text02.TextColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWide310x150Text02.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements ITileWide310x150Text02.TextColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWide310x150Text02.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements ITileWide310x150Text02.TextColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWide310x150Text02.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text03
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text03

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text03", fallbackName:="TileWideText03", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWide310x150Text03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text04
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text04

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text04", fallbackName:="TileWideText04", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150Text04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text05
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text05

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text05", fallbackName:="TileWideText05", imageCount:=0, textCount:=5)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide310x150Text05.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide310x150Text05.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWide310x150Text05.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWide310x150Text05.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileWide310x150Text05.TextBody5
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text06
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text06

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text06", fallbackName:="TileWideText06", imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements ITileWide310x150Text06.TextColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWide310x150Text06.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements ITileWide310x150Text06.TextColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWide310x150Text06.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements ITileWide310x150Text06.TextColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWide310x150Text06.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements ITileWide310x150Text06.TextColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWide310x150Text06.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements ITileWide310x150Text06.TextColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileWide310x150Text06.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text07
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text07

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text07", fallbackName:="TileWideText07", imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150Text07.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row1() As INotificationContentText Implements ITileWide310x150Text07.TextShortColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWide310x150Text07.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row2() As INotificationContentText Implements ITileWide310x150Text07.TextShortColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWide310x150Text07.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row3() As INotificationContentText Implements ITileWide310x150Text07.TextShortColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWide310x150Text07.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row4() As INotificationContentText Implements ITileWide310x150Text07.TextShortColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWide310x150Text07.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text08
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text08

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text08", fallbackName:="TileWideText08", imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextShortColumn1Row1() As INotificationContentText Implements ITileWide310x150Text08.TextShortColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWide310x150Text08.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row2() As INotificationContentText Implements ITileWide310x150Text08.TextShortColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWide310x150Text08.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row3() As INotificationContentText Implements ITileWide310x150Text08.TextShortColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWide310x150Text08.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row4() As INotificationContentText Implements ITileWide310x150Text08.TextShortColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWide310x150Text08.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row5() As INotificationContentText Implements ITileWide310x150Text08.TextShortColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileWide310x150Text08.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text09
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text09

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text09", fallbackName:="TileWideText09", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150Text09.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWide310x150Text09.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text10
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text10

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text10", fallbackName:="TileWideText10", imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide310x150Text10.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row1() As INotificationContentText Implements ITileWide310x150Text10.TextPrefixColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWide310x150Text10.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row2() As INotificationContentText Implements ITileWide310x150Text10.TextPrefixColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWide310x150Text10.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row3() As INotificationContentText Implements ITileWide310x150Text10.TextPrefixColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWide310x150Text10.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row4() As INotificationContentText Implements ITileWide310x150Text10.TextPrefixColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWide310x150Text10.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide310x150Text11
        Inherits TileWide310x150Base
        Implements ITileWide310x150Text11

        Private Property IWide310x150TileNotificationContent_Square150x150Content() As ISquare150x150TileNotificationContent Implements IWide310x150TileNotificationContent.Square150x150Content
            Get
                Return MyBase.Square150x150Content
            End Get
            Set(value As ISquare150x150TileNotificationContent)
                MyBase.Square150x150Content = Value
            End Set
        End Property

        Private Property IWide310x150TileNotificationContent_RequireSquare150x150Content() As Boolean Implements IWide310x150TileNotificationContent.RequireSquare150x150Content
            Get
                Return MyBase.RequireSquare150x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare150x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide310x150Text11", fallbackName:="TileWideText11", imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextPrefixColumn1Row1() As INotificationContentText Implements ITileWide310x150Text11.TextPrefixColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWide310x150Text11.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row2() As INotificationContentText Implements ITileWide310x150Text11.TextPrefixColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWide310x150Text11.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row3() As INotificationContentText Implements ITileWide310x150Text11.TextPrefixColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWide310x150Text11.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row4() As INotificationContentText Implements ITileWide310x150Text11.TextPrefixColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWide310x150Text11.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row5() As INotificationContentText Implements ITileWide310x150Text11.TextPrefixColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileWide310x150Text11.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310BlockAndText01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310BlockAndText01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310BlockAndText01", fallbackName:=Nothing, imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBody5
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextBody6() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBody6
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextBlock
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileSquare310x310BlockAndText01.TextSubBlock
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310BlockAndText02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310BlockAndText02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310BlockAndText02", fallbackName:=Nothing, imageCount:=1, textCount:=7)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310BlockAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextBlock
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextHeading1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextHeading2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextBody1
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextBody2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextBody3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquare310x310BlockAndText02.TextBody4
            Get
                Return TextFields(6)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310Image
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Image

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Image", fallbackName:=Nothing, imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310Image.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageAndText01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageAndText01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageAndText01", fallbackName:=Nothing, imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310ImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextCaptionWrap() As INotificationContentText Implements ITileSquare310x310ImageAndText01.TextCaptionWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageAndText02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageAndText02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageAndText02", fallbackName:=Nothing, imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310ImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextCaption1() As INotificationContentText Implements ITileSquare310x310ImageAndText02.TextCaption1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextCaption2() As INotificationContentText Implements ITileSquare310x310ImageAndText02.TextCaption2
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageAndTextOverlay01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageAndTextOverlay01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageAndTextOverlay01", fallbackName:=Nothing, imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310ImageAndTextOverlay01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay01.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageAndTextOverlay02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageAndTextOverlay02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageAndTextOverlay02", fallbackName:=Nothing, imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310ImageAndTextOverlay02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay02.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageAndTextOverlay03
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageAndTextOverlay03

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageAndTextOverlay03", fallbackName:=Nothing, imageCount:=1, textCount:=4)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310ImageAndTextOverlay03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay03.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay03.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare310x310ImageAndTextOverlay03.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageCollection
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageCollection

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageCollection", fallbackName:=Nothing, imageCount:=5, textCount:=0)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileSquare310x310ImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmall1() As INotificationContentImage Implements ITileSquare310x310ImageCollection.ImageSmall1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmall2() As INotificationContentImage Implements ITileSquare310x310ImageCollection.ImageSmall2
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmall3() As INotificationContentImage Implements ITileSquare310x310ImageCollection.ImageSmall3
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmall4() As INotificationContentImage Implements ITileSquare310x310ImageCollection.ImageSmall4
            Get
                Return Images(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageCollectionAndText01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageCollectionAndText01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageCollectionAndText01", fallbackName:=Nothing, imageCount:=5, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText01.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmall1() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText01.ImageSmall1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmall2() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText01.ImageSmall2
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmall3() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText01.ImageSmall3
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmall4() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText01.ImageSmall4
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextCaptionWrap() As INotificationContentText Implements ITileSquare310x310ImageCollectionAndText01.TextCaptionWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310ImageCollectionAndText02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310ImageCollectionAndText02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310ImageCollectionAndText02", fallbackName:=Nothing, imageCount:=5, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText02.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmall1() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText02.ImageSmall1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmall2() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText02.ImageSmall2
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmall3() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText02.ImageSmall3
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmall4() As INotificationContentImage Implements ITileSquare310x310ImageCollectionAndText02.ImageSmall4
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextCaption1() As INotificationContentText Implements ITileSquare310x310ImageCollectionAndText02.TextCaption1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextCaption2() As INotificationContentText Implements ITileSquare310x310ImageCollectionAndText02.TextCaption2
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310SmallImageAndText01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310SmallImageAndText01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310SmallImageAndText01", fallbackName:=Nothing, imageCount:=1, textCount:=3)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquare310x310SmallImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare310x310SmallImageAndText01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquare310x310SmallImageAndText01.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody() As INotificationContentText Implements ITileSquare310x310SmallImageAndText01.TextBody
            Get
                Return TextFields(2)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310SmallImagesAndTextList01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310SmallImagesAndTextList01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310SmallImagesAndTextList01", fallbackName:=Nothing, imageCount:=3, textCount:=9)
        End Sub

        Public ReadOnly Property Image1() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList01.Image1
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextHeading1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup1Field1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextBodyGroup1Field1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup1Field2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextBodyGroup1Field2
            Get
                Return TextFields(2)
            End Get
        End Property

        Public ReadOnly Property Image2() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList01.Image2
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextHeading2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup2Field1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextBodyGroup2Field1
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup2Field2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextBodyGroup2Field2
            Get
                Return TextFields(5)
            End Get
        End Property

        Public ReadOnly Property Image3() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList01.Image3
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property TextHeading3() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextHeading3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup3Field1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextBodyGroup3Field1
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup3Field2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList01.TextBodyGroup3Field2
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310SmallImagesAndTextList02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310SmallImagesAndTextList02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310SmallImagesAndTextList02", fallbackName:=Nothing, imageCount:=3, textCount:=3)
        End Sub

        Public ReadOnly Property Image1() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList02.Image1
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property TextWrap1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList02.TextWrap1
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property Image2() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList02.Image2
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property TextWrap2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList02.TextWrap2
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property Image3() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList02.Image3
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property TextWrap3() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList02.TextWrap3
            Get
                Return TextFields(2)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310SmallImagesAndTextList03
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310SmallImagesAndTextList03

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310SmallImagesAndTextList03", fallbackName:=Nothing, imageCount:=3, textCount:=6)
        End Sub

        Public ReadOnly Property Image1() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList03.Image1
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList03.TextHeading1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextWrap1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList03.TextWrap1
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property Image2() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList03.Image2
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList03.TextHeading2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextWrap2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList03.TextWrap2
            Get
                Return TextFields(3)
            End Get
        End Property

        Public ReadOnly Property Image3() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList03.Image3
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property TextHeading3() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList03.TextHeading3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextWrap3() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList03.TextWrap3
            Get
                Return TextFields(5)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310SmallImagesAndTextList04
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310SmallImagesAndTextList04

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310SmallImagesAndTextList04", fallbackName:=Nothing, imageCount:=3, textCount:=6)
        End Sub

        Public ReadOnly Property Image1() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList04.Image1
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList04.TextHeading1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextWrap1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList04.TextWrap1
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property Image2() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList04.Image2
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList04.TextHeading2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextWrap2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList04.TextWrap2
            Get
                Return TextFields(3)
            End Get
        End Property

        Public ReadOnly Property Image3() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList04.Image3
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property TextHeading3() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList04.TextHeading3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextWrap3() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList04.TextWrap3
            Get
                Return TextFields(5)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310SmallImagesAndTextList05
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310SmallImagesAndTextList05

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310SmallImagesAndTextList05", fallbackName:=Nothing, imageCount:=3, textCount:=7)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property Image1() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList05.Image1
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property TextGroup1Field1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextGroup1Field1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextGroup1Field2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextGroup1Field2
            Get
                Return TextFields(2)
            End Get
        End Property

        Public ReadOnly Property Image2() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList05.Image2
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property TextGroup2Field1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextGroup2Field1
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextGroup2Field2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextGroup2Field2
            Get
                Return TextFields(4)
            End Get
        End Property

        Public ReadOnly Property Image3() As INotificationContentImage Implements ITileSquare310x310SmallImagesAndTextList05.Image3
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property TextGroup3Field1() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextGroup3Field1
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextGroup3Field2() As INotificationContentText Implements ITileSquare310x310SmallImagesAndTextList05.TextGroup3Field2
            Get
                Return TextFields(6)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310Text01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text01", fallbackName:=Nothing, imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare310x310Text01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare310x310Text01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare310x310Text01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare310x310Text01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquare310x310Text01.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileSquare310x310Text01.TextBody5
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextBody6() As INotificationContentText Implements ITileSquare310x310Text01.TextBody6
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextBody7() As INotificationContentText Implements ITileSquare310x310Text01.TextBody7
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextBody8() As INotificationContentText Implements ITileSquare310x310Text01.TextBody8
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextBody9() As INotificationContentText Implements ITileSquare310x310Text01.TextBody9
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310Text02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text02", fallbackName:=Nothing, imageCount:=0, textCount:=19)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquare310x310Text02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row5
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row5
            Get
                Return TextFields(10)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row6() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row6
            Get
                Return TextFields(11)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row6() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row6
            Get
                Return TextFields(12)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row7() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row7
            Get
                Return TextFields(13)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row7() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row7
            Get
                Return TextFields(14)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row8() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row8
            Get
                Return TextFields(15)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row8() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row8
            Get
                Return TextFields(16)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row9() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn1Row9
            Get
                Return TextFields(17)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row9() As INotificationContentText Implements ITileSquare310x310Text02.TextColumn2Row9
            Get
                Return TextFields(18)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310Text03
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text03

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text03", fallbackName:=Nothing, imageCount:=0, textCount:=11)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare310x310Text03.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare310x310Text03.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquare310x310Text03.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquare310x310Text03.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileSquare310x310Text03.TextBody5
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextBody6() As INotificationContentText Implements ITileSquare310x310Text03.TextBody6
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextBody7() As INotificationContentText Implements ITileSquare310x310Text03.TextBody7
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextBody8() As INotificationContentText Implements ITileSquare310x310Text03.TextBody8
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextBody9() As INotificationContentText Implements ITileSquare310x310Text03.TextBody9
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextBody10() As INotificationContentText Implements ITileSquare310x310Text03.TextBody10
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextBody11() As INotificationContentText Implements ITileSquare310x310Text03.TextBody11
            Get
                Return TextFields(10)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310Text04
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text04

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text04", fallbackName:=Nothing, imageCount:=0, textCount:=22)
        End Sub

        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row6() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row6
            Get
                Return TextFields(10)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row6() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row6
            Get
                Return TextFields(11)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row7() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row7
            Get
                Return TextFields(12)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row7() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row7
            Get
                Return TextFields(13)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row8() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row8
            Get
                Return TextFields(14)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row8() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row8
            Get
                Return TextFields(15)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row9() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row9
            Get
                Return TextFields(16)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row9() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row9
            Get
                Return TextFields(17)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row10() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row10
            Get
                Return TextFields(18)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row10() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row10
            Get
                Return TextFields(19)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row11() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn1Row11
            Get
                Return TextFields(20)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row11() As INotificationContentText Implements ITileSquare310x310Text04.TextColumn2Row11
            Get
                Return TextFields(21)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310Text05
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text05

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text05", fallbackName:=Nothing, imageCount:=0, textCount:=19)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row5
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row5
            Get
                Return TextFields(10)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row6
            Get
                Return TextFields(11)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row6
            Get
                Return TextFields(12)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row7
            Get
                Return TextFields(13)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row7
            Get
                Return TextFields(14)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row8
            Get
                Return TextFields(15)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row8
            Get
                Return TextFields(16)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row9
            Get
                Return TextFields(17)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row9
            Get
                Return TextFields(18)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireWide310x150Content As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content

        Public Overloads Property Wide310x150Content As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content

    End Class

    Friend Class TileSquare310x310Text06
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text06

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text06", fallbackName:=Nothing, imageCount:=0, textCount:=22)
        End Sub

        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row6
            Get
                Return TextFields(10)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row6
            Get
                Return TextFields(11)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row7
            Get
                Return TextFields(12)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row7
            Get
                Return TextFields(13)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row8
            Get
                Return TextFields(14)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row8
            Get
                Return TextFields(15)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row9
            Get
                Return TextFields(16)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row9
            Get
                Return TextFields(17)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row10() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row10
            Get
                Return TextFields(18)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row10() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row10
            Get
                Return TextFields(19)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row11() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row11
            Get
                Return TextFields(20)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row11() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row11
            Get
                Return TextFields(21)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireWide310x150Content As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content

        Public Overloads Property Wide310x150Content As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content

    End Class

    Friend Class TileSquare310x310Text07
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text07

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text07", fallbackName:=Nothing, imageCount:=0, textCount:=19)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row5
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row5
            Get
                Return TextFields(10)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row6
            Get
                Return TextFields(11)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row6
            Get
                Return TextFields(12)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row7
            Get
                Return TextFields(13)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row7
            Get
                Return TextFields(14)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row8
            Get
                Return TextFields(15)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row8
            Get
                Return TextFields(16)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn1Row9
            Get
                Return TextFields(17)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text02.TextColumn2Row9
            Get
                Return TextFields(18)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireWide310x150Content As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content

        Public Overloads Property Wide310x150Content As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content

    End Class

    Friend Class TileSquare310x310Text08
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text08

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text08", fallbackName:=Nothing, imageCount:=0, textCount:=22)
        End Sub

        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row6
            Get
                Return TextFields(10)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row6() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row6
            Get
                Return TextFields(11)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row7
            Get
                Return TextFields(12)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row7() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row7
            Get
                Return TextFields(13)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row8
            Get
                Return TextFields(14)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row8() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row8
            Get
                Return TextFields(15)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row9
            Get
                Return TextFields(16)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row9() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row9
            Get
                Return TextFields(17)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row10() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row10
            Get
                Return TextFields(18)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row10() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row10
            Get
                Return TextFields(19)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row11() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn1Row11
            Get
                Return TextFields(20)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row11() As INotificationContentText Implements NotificationsExtensions.TileContent.ITileSquare310x310Text04.TextColumn2Row11
            Get
                Return TextFields(21)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

        Public Overloads Property RequireWide310x150Content As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content

        Public Overloads Property Wide310x150Content As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content

    End Class

    Friend Class TileSquare310x310Text09
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310Text09

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310Text09", fallbackName:=Nothing, imageCount:=0, textCount:=5)
        End Sub

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileSquare310x310Text09.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310Text09.TextHeading1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310Text09.TextHeading2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquare310x310Text09.TextBody1
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquare310x310Text09.TextBody2
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310TextList01
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310TextList01

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310TextList01", fallbackName:=Nothing, imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310TextList01.TextHeading1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup1Field1() As INotificationContentText Implements ITileSquare310x310TextList01.TextBodyGroup1Field1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup1Field2() As INotificationContentText Implements ITileSquare310x310TextList01.TextBodyGroup1Field2
            Get
                Return TextFields(2)
            End Get
        End Property

        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310TextList01.TextHeading2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup2Field1() As INotificationContentText Implements ITileSquare310x310TextList01.TextBodyGroup2Field1
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup2Field2() As INotificationContentText Implements ITileSquare310x310TextList01.TextBodyGroup2Field2
            Get
                Return TextFields(5)
            End Get
        End Property

        Public ReadOnly Property TextHeading3() As INotificationContentText Implements ITileSquare310x310TextList01.TextHeading3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup3Field1() As INotificationContentText Implements ITileSquare310x310TextList01.TextBodyGroup3Field1
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextBodyGroup3Field2() As INotificationContentText Implements ITileSquare310x310TextList01.TextBodyGroup3Field2
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310TextList02
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310TextList02

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310TextList02", fallbackName:=Nothing, imageCount:=0, textCount:=3)
        End Sub

        Public ReadOnly Property TextWrap1() As INotificationContentText Implements ITileSquare310x310TextList02.TextWrap1
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextWrap2() As INotificationContentText Implements ITileSquare310x310TextList02.TextWrap2
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property TextWrap3() As INotificationContentText Implements ITileSquare310x310TextList02.TextWrap3
            Get
                Return TextFields(2)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare310x310TextList03
        Inherits TileSquare310x310Base
        Implements ITileSquare310x310TextList03

        Private Property ISquare310x310TileNotificationContent_Wide310x150Content() As IWide310x150TileNotificationContent Implements ISquare310x310TileNotificationContent.Wide310x150Content
            Get
                Return MyBase.Wide310x150Content
            End Get
            Set(value As IWide310x150TileNotificationContent)
                MyBase.Wide310x150Content = Value
            End Set
        End Property

        Private Property ISquare310x310TileNotificationContent_RequireWide310x150Content() As Boolean Implements ISquare310x310TileNotificationContent.RequireWide310x150Content
            Get
                Return MyBase.RequireWide310x150Content
            End Get
            Set(value As Boolean)
                MyBase.RequireWide310x150Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare310x310TextList03", fallbackName:=Nothing, imageCount:=0, textCount:=6)
        End Sub

        Public ReadOnly Property TextHeading1() As INotificationContentText Implements ITileSquare310x310TextList03.TextHeading1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextWrap1() As INotificationContentText Implements ITileSquare310x310TextList03.TextWrap1
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property TextHeading2() As INotificationContentText Implements ITileSquare310x310TextList03.TextHeading2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextWrap2() As INotificationContentText Implements ITileSquare310x310TextList03.TextWrap2
            Get
                Return TextFields(3)
            End Get
        End Property

        Public ReadOnly Property TextHeading3() As INotificationContentText Implements ITileSquare310x310TextList03.TextHeading3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextWrap3() As INotificationContentText Implements ITileSquare310x310TextList03.TextWrap3
            Get
                Return TextFields(5)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId
    End Class

    Friend Class TileSquare99x99IconWithBadge
        Inherits TileSquare99x99Base
        Implements ITileSquare99x99IconWithBadge

        Public Sub New()
            MyBase.New(templateName:="TileSquare99x99IconWithBadge", fallbackName:=Nothing, imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property ImageIcon() As INotificationContentImage Implements ITileSquare99x99IconWithBadge.ImageIcon
            Get
                Return Images(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileSquare210x210IconWithBadge
        Inherits TileSquare210x210Base
        Implements ITileSquare210x210IconWithBadge

        Private Property ISquare210x210TileNotificationContent_Square99x99Content() As ISquare99x99TileNotificationContent Implements ISquare210x210TileNotificationContent.Square99x99Content
            Get
                Return MyBase.Square99x99Content
            End Get
            Set(value As ISquare99x99TileNotificationContent)
                MyBase.Square99x99Content = Value
            End Set
        End Property

        Private Property ISquare210x210TileNotificationContent_RequireSquare99x99Content() As Boolean Implements ISquare210x210TileNotificationContent.RequireSquare99x99Content
            Get
                Return MyBase.RequireSquare99x99Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare99x99Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileSquare210x210IconWithBadge", fallbackName:=Nothing, imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property ImageIcon() As INotificationContentImage Implements ITileSquare210x210IconWithBadge.ImageIcon
            Get
                Return Images(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    Friend Class TileWide432x210IconWithBadgeAndText
        Inherits TileWide432x210Base
        Implements ITileWide432x210IconWithBadgeAndText

        Private Property IWide432x210TileNotificationContent_Square210x210Content() As ISquare210x210TileNotificationContent Implements IWide432x210TileNotificationContent.Square210x210Content
            Get
                Return MyBase.Square210x210Content
            End Get
            Set(value As ISquare210x210TileNotificationContent)
                MyBase.Square210x210Content = Value
            End Set
        End Property

        Private Property IWide432x210TileNotificationContent_RequireSquare210x210Content() As Boolean Implements IWide432x210TileNotificationContent.RequireSquare210x210Content
            Get
                Return MyBase.RequireSquare210x210Content
            End Get
            Set(value As Boolean)
                MyBase.RequireSquare210x210Content = Value
            End Set
        End Property

        Public Sub New()
            MyBase.New(templateName:="TileWide432x210IconWithBadgeAndText", fallbackName:=Nothing, imageCount:=1, textCount:=3)
        End Sub

        Public ReadOnly Property ImageIcon() As INotificationContentImage Implements ITileWide432x210IconWithBadgeAndText.ImageIcon
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWide432x210IconWithBadgeAndText.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWide432x210IconWithBadgeAndText.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWide432x210IconWithBadgeAndText.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification
        End Function

        Public Overloads Property AddImageQuery As Boolean Implements ITileNotificationContent.AddImageQuery

        Public Overloads Property BaseUri As String Implements ITileNotificationContent.BaseUri

        Public Overloads Property Branding As TileBranding Implements ITileNotificationContent.Branding

        Public Overloads Property Lang As String Implements ITileNotificationContent.Lang

        Public Overloads Property StrictValidation As Boolean Implements ITileNotificationContent.StrictValidation

        Public Overloads Property ContentId As String Implements ITileNotificationContent.ContentId

    End Class

    ''' <summary>
    ''' A factory which creates tile content objects for all of the toast template types.
    ''' </summary>
    Public NotInheritable Class TileContentFactory
        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150Block template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150Block template.</returns>
        Public Shared Function CreateTileSquare150x150Block() As ITileSquare150x150Block
            Return New TileSquare150x150Block()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150Image template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150Image template.</returns>
        Public Shared Function CreateTileSquare150x150Image() As ITileSquare150x150Image
            Return New TileSquare150x150Image()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150PeekImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150PeekImageAndText01 template.</returns>
        Public Shared Function CreateTileSquare150x150PeekImageAndText01() As ITileSquare150x150PeekImageAndText01
            Return New TileSquare150x150PeekImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150PeekImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150PeekImageAndText02 template.</returns>
        Public Shared Function CreateTileSquare150x150PeekImageAndText02() As ITileSquare150x150PeekImageAndText02
            Return New TileSquare150x150PeekImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150PeekImageAndText03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150PeekImageAndText03 template.</returns>
        Public Shared Function CreateTileSquare150x150PeekImageAndText03() As ITileSquare150x150PeekImageAndText03
            Return New TileSquare150x150PeekImageAndText03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150PeekImageAndText04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150PeekImageAndText04 template.</returns>
        Public Shared Function CreateTileSquare150x150PeekImageAndText04() As ITileSquare150x150PeekImageAndText04
            Return New TileSquare150x150PeekImageAndText04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150Text01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150Text01 template.</returns>
        Public Shared Function CreateTileSquare150x150Text01() As ITileSquare150x150Text01
            Return New TileSquare150x150Text01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150Text02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150Text02 template.</returns>
        Public Shared Function CreateTileSquare150x150Text02() As ITileSquare150x150Text02
            Return New TileSquare150x150Text02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150Text03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150Text03 template.</returns>
        Public Shared Function CreateTileSquare150x150Text03() As ITileSquare150x150Text03
            Return New TileSquare150x150Text03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare150x150Text04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare150x150Text04 template.</returns>
        Public Shared Function CreateTileSquare150x150Text04() As ITileSquare150x150Text04
            Return New TileSquare150x150Text04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150BlockAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150BlockAndText01 template.</returns>
        Public Shared Function CreateTileWide310x150BlockAndText01() As ITileWide310x150BlockAndText01
            Return New TileWide310x150BlockAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150BlockAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150BlockAndText02 template.</returns>
        Public Shared Function CreateTileWide310x150BlockAndText02() As ITileWide310x150BlockAndText02
            Return New TileWide310x150BlockAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Image template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Image template.</returns>
        Public Shared Function CreateTileWide310x150Image() As ITileWide310x150Image
            Return New TileWide310x150Image()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150ImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150ImageAndText01 template.</returns>
        Public Shared Function CreateTileWide310x150ImageAndText01() As ITileWide310x150ImageAndText01
            Return New TileWide310x150ImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150ImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150ImageAndText02 template.</returns>
        Public Shared Function CreateTileWide310x150ImageAndText02() As ITileWide310x150ImageAndText02
            Return New TileWide310x150ImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150ImageCollection template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150ImageCollection template.</returns>
        Public Shared Function CreateTileWide310x150ImageCollection() As ITileWide310x150ImageCollection
            Return New TileWide310x150ImageCollection()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImage01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImage01 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImage01() As ITileWide310x150PeekImage01
            Return New TileWide310x150PeekImage01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImage02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImage02 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImage02() As ITileWide310x150PeekImage02
            Return New TileWide310x150PeekImage02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImage03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImage03 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImage03() As ITileWide310x150PeekImage03
            Return New TileWide310x150PeekImage03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImage04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImage04 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImage04() As ITileWide310x150PeekImage04
            Return New TileWide310x150PeekImage04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImage05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImage05 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImage05() As ITileWide310x150PeekImage05
            Return New TileWide310x150PeekImage05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImage06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImage06 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImage06() As ITileWide310x150PeekImage06
            Return New TileWide310x150PeekImage06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageAndText01 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageAndText01() As ITileWide310x150PeekImageAndText01
            Return New TileWide310x150PeekImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageAndText02 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageAndText02() As ITileWide310x150PeekImageAndText02
            Return New TileWide310x150PeekImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageCollection01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageCollection01 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageCollection01() As ITileWide310x150PeekImageCollection01
            Return New TileWide310x150PeekImageCollection01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageCollection02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageCollection02 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageCollection02() As ITileWide310x150PeekImageCollection02
            Return New TileWide310x150PeekImageCollection02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageCollection03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageCollection03 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageCollection03() As ITileWide310x150PeekImageCollection03
            Return New TileWide310x150PeekImageCollection03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageCollection04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageCollection04 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageCollection04() As ITileWide310x150PeekImageCollection04
            Return New TileWide310x150PeekImageCollection04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageCollection05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageCollection05 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageCollection05() As ITileWide310x150PeekImageCollection05
            Return New TileWide310x150PeekImageCollection05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150PeekImageCollection06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150PeekImageCollection06 template.</returns>
        Public Shared Function CreateTileWide310x150PeekImageCollection06() As ITileWide310x150PeekImageCollection06
            Return New TileWide310x150PeekImageCollection06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150SmallImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150SmallImageAndText01 template.</returns>
        Public Shared Function CreateTileWide310x150SmallImageAndText01() As ITileWide310x150SmallImageAndText01
            Return New TileWide310x150SmallImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150SmallImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150SmallImageAndText02 template.</returns>
        Public Shared Function CreateTileWide310x150SmallImageAndText02() As ITileWide310x150SmallImageAndText02
            Return New TileWide310x150SmallImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150SmallImageAndText03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150SmallImageAndText03 template.</returns>

        Public Shared Function CreateTileWide310x150SmallImageAndText03() As ITileWide310x150SmallImageAndText03
            Return New TileWide310x150SmallImageAndText03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150SmallImageAndText04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150SmallImageAndText04 template.</returns>
        Public Shared Function CreateTileWide310x150SmallImageAndText04() As ITileWide310x150SmallImageAndText04
            Return New TileWide310x150SmallImageAndText04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150SmallImageAndText05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150SmallImageAndText05 template.</returns>
        Public Shared Function CreateTileWide310x150SmallImageAndText05() As ITileWide310x150SmallImageAndText05
            Return New TileWide310x150SmallImageAndText05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text01 template.</returns>
        Public Shared Function CreateTileWide310x150Text01() As ITileWide310x150Text01
            Return New TileWide310x150Text01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text02 template.</returns>
        Public Shared Function CreateTileWide310x150Text02() As ITileWide310x150Text02
            Return New TileWide310x150Text02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text03 template.</returns>
        Public Shared Function CreateTileWide310x150Text03() As ITileWide310x150Text03
            Return New TileWide310x150Text03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text04 template.</returns>
        Public Shared Function CreateTileWide310x150Text04() As ITileWide310x150Text04
            Return New TileWide310x150Text04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text05 template.</returns>
        Public Shared Function CreateTileWide310x150Text05() As ITileWide310x150Text05
            Return New TileWide310x150Text05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text06 template.</returns>
        Public Shared Function CreateTileWide310x150Text06() As ITileWide310x150Text06
            Return New TileWide310x150Text06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text07 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text07 template.</returns>
        Public Shared Function CreateTileWide310x150Text07() As ITileWide310x150Text07
            Return New TileWide310x150Text07()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text08 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text08 template.</returns>
        Public Shared Function CreateTileWide310x150Text08() As ITileWide310x150Text08
            Return New TileWide310x150Text08()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text09 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text09 template.</returns>
        Public Shared Function CreateTileWide310x150Text09() As ITileWide310x150Text09
            Return New TileWide310x150Text09()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text10 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text10 template.</returns>
        Public Shared Function CreateTileWide310x150Text10() As ITileWide310x150Text10
            Return New TileWide310x150Text10()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide310x150Text11 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide310x150Text11 template.</returns>
        Public Shared Function CreateTileWide310x150Text11() As ITileWide310x150Text11
            Return New TileWide310x150Text11()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310BlockAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310BlockAndText01 template.</returns>
        Public Shared Function CreateTileSquare310x310BlockAndText01() As ITileSquare310x310BlockAndText01
            Return New TileSquare310x310BlockAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310BlockAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310BlockAndText02 template.</returns>
        Public Shared Function CreateTileSquare310x310BlockAndText02() As ITileSquare310x310BlockAndText02
            Return New TileSquare310x310BlockAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Image template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Image template.</returns>
        Public Shared Function CreateTileSquare310x310Image() As ITileSquare310x310Image
            Return New TileSquare310x310Image()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageAndText01 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageAndText01() As ITileSquare310x310ImageAndText01
            Return New TileSquare310x310ImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageAndText02 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageAndText02() As ITileSquare310x310ImageAndText02
            Return New TileSquare310x310ImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageAndTextOverlay01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageAndTextOverlay01 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageAndTextOverlay01() As ITileSquare310x310ImageAndTextOverlay01
            Return New TileSquare310x310ImageAndTextOverlay01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageAndTextOverlay02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageAndTextOverlay02 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageAndTextOverlay02() As ITileSquare310x310ImageAndTextOverlay02
            Return New TileSquare310x310ImageAndTextOverlay02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageAndTextOverlay03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageAndTextOverlay03 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageAndTextOverlay03() As ITileSquare310x310ImageAndTextOverlay03
            Return New TileSquare310x310ImageAndTextOverlay03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageCollection template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageCollection template.</returns>
        Public Shared Function CreateTileSquare310x310ImageCollection() As ITileSquare310x310ImageCollection
            Return New TileSquare310x310ImageCollection()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageCollectionAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageCollectionAndText01 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageCollectionAndText01() As ITileSquare310x310ImageCollectionAndText01
            Return New TileSquare310x310ImageCollectionAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310ImageCollectionAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310ImageCollectionAndText02 template.</returns>
        Public Shared Function CreateTileSquare310x310ImageCollectionAndText02() As ITileSquare310x310ImageCollectionAndText02
            Return New TileSquare310x310ImageCollectionAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310SmallImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310SmallImageAndText01 template.</returns>
        Public Shared Function CreateTileSquare310x310SmallImageAndText01() As ITileSquare310x310SmallImageAndText01
            Return New TileSquare310x310SmallImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310SmallImagesAndTextList01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310SmallImagesAndTextList01 template.</returns>
        Public Shared Function CreateTileSquare310x310SmallImagesAndTextList01() As ITileSquare310x310SmallImagesAndTextList01
            Return New TileSquare310x310SmallImagesAndTextList01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310SmallImagesAndTextList02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310SmallImagesAndTextList02 template.</returns>
        Public Shared Function CreateTileSquare310x310SmallImagesAndTextList02() As ITileSquare310x310SmallImagesAndTextList02
            Return New TileSquare310x310SmallImagesAndTextList02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310SmallImagesAndTextList03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310SmallImagesAndTextList03 template.</returns>
        Public Shared Function CreateTileSquare310x310SmallImagesAndTextList03() As ITileSquare310x310SmallImagesAndTextList03
            Return New TileSquare310x310SmallImagesAndTextList03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310SmallImagesAndTextList04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310SmallImagesAndTextList04 template.</returns>
        Public Shared Function CreateTileSquare310x310SmallImagesAndTextList04() As ITileSquare310x310SmallImagesAndTextList04
            Return New TileSquare310x310SmallImagesAndTextList04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310SmallImagesAndTextList05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310SmallImagesAndTextList05 template.</returns>
        Public Shared Function CreateTileSquare310x310SmallImagesAndTextList05() As ITileSquare310x310SmallImagesAndTextList05
            Return New TileSquare310x310SmallImagesAndTextList05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text01 template.</returns>
        Public Shared Function CreateTileSquare310x310Text01() As ITileSquare310x310Text01
            Return New TileSquare310x310Text01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text02 template.</returns>
        Public Shared Function CreateTileSquare310x310Text02() As ITileSquare310x310Text02
            Return New TileSquare310x310Text02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text03 template.</returns>
        Public Shared Function CreateTileSquare310x310Text03() As ITileSquare310x310Text03
            Return New TileSquare310x310Text03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text04 template.</returns>
        Public Shared Function CreateTileSquare310x310Text04() As ITileSquare310x310Text04
            Return New TileSquare310x310Text04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text05 template.</returns>
        Public Shared Function CreateTileSquare310x310Text05() As ITileSquare310x310Text05
            Return New TileSquare310x310Text05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text06 template.</returns>
        Public Shared Function CreateTileSquare310x310Text06() As ITileSquare310x310Text06
            Return New TileSquare310x310Text06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text07 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text07 template.</returns>
        Public Shared Function CreateTileSquare310x310Text07() As ITileSquare310x310Text07
            Return New TileSquare310x310Text07()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text08 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text08 template.</returns>
        Public Shared Function CreateTileSquare310x310Text08() As ITileSquare310x310Text08
            Return New TileSquare310x310Text08()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310Text09 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310Text09 template.</returns>
        Public Shared Function CreateTileSquare310x310Text09() As ITileSquare310x310Text09
            Return New TileSquare310x310Text09()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310TextList01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310TextList01 template.</returns>
        Public Shared Function CreateTileSquare310x310TextList01() As ITileSquare310x310TextList01
            Return New TileSquare310x310TextList01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310TextList02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310TextList02 template.</returns>
        Public Shared Function CreateTileSquare310x310TextList02() As ITileSquare310x310TextList02
            Return New TileSquare310x310TextList02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare310x310TextList03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare310x310TextList03 template.</returns>
        Public Shared Function CreateTileSquare310x310TextList03() As ITileSquare310x310TextList03
            Return New TileSquare310x310TextList03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare99x99IconWithBadge template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare99x99IconWithBadge template.</returns>
        Public Shared Function CreateTileSquare99x99IconWithBadge() As ITileSquare99x99IconWithBadge
            Return New TileSquare99x99IconWithBadge()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquare210x210IconWithBadge template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquare210x210IconWithBadge template.</returns>
        Public Shared Function CreateTileSquare210x210IconWithBadge() As ITileSquare210x210IconWithBadge
            Return New TileSquare210x210IconWithBadge()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWide432x210IconWithBadgeAndText template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWide432x210IconWithBadgeAndText template.</returns>
        Public Shared Function CreateTileWide432x210IconWithBadgeAndText() As ITileWide432x210IconWithBadgeAndText
            Return New TileWide432x210IconWithBadgeAndText()
        End Function
    End Class
End Namespace
