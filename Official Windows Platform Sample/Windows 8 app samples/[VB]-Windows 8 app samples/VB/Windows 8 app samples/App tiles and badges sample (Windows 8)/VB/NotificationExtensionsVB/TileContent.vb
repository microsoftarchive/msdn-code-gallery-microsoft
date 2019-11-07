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
    Friend MustInherit Class TileNotificationBase
        Inherits NotificationBase
        Public Sub New(templateName As String, imageCount As Integer, textCount As Integer)
            MyBase.New(templateName, imageCount, textCount)
        End Sub

        Public Property Branding() As TileBranding
            Get
                Return m_Branding
            End Get
            Set(value As TileBranding)
                If Not [Enum].IsDefined(GetType(TileBranding), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Branding = value
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
    End Class

    Friend Interface ISquareTileInternal
        Function SerializeBinding(globalLang As String, globalBaseUri As String, globalBranding As TileBranding) As String
    End Interface

    Friend Class TileSquareBase
        Inherits TileNotificationBase
        Implements ISquareTileInternal

        Public Sub New(templateName As String, imageCount As Integer, textCount As Integer)
            MyBase.New(templateName, imageCount, textCount)
        End Sub

        Public Overrides Function GetContent() As String
            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile><visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                builder.AppendFormat(" branding='{0}'", Branding.ToString.ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            builder.Append(">")
            builder.Append(SerializeBinding(Lang, BaseUri, Branding))
            builder.Append("</visual></tile>")
            Return builder.ToString
        End Function

        Public Function SerializeBinding(globalLang As String, globalBaseUri As String, globalBranding As TileBranding) As String Implements ISquareTileInternal.SerializeBinding
            Dim bindingNode As New StringBuilder(String.Empty)
            bindingNode.AppendFormat("<binding template='{0}'", TemplateName)
            If Not String.IsNullOrWhiteSpace(Lang) AndAlso Not Lang.Equals(globalLang) Then
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
                globalLang = Lang
            End If
            If Branding <> TileBranding.Logo AndAlso Branding <> globalBranding Then
                bindingNode.AppendFormat(" branding='{0}'", Branding.ToString.ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) AndAlso Not BaseUri.Equals(globalBaseUri) Then
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
                globalBaseUri = BaseUri
            End If
            bindingNode.AppendFormat(">{0}</binding>", SerializeProperties(globalLang, globalBaseUri))

            Return bindingNode.ToString
        End Function

        
    End Class

    Friend Class TileWideBase
        Inherits TileNotificationBase

        Public Sub New(templateName As String, imageCount As Integer, textCount As Integer)
            MyBase.New(templateName, imageCount, textCount)
        End Sub

        Public Property SquareContent() As ISquareTileNotificationContent
            Get
                Return m_SquareContent
            End Get
            Set(value As ISquareTileNotificationContent)
                m_SquareContent = value
            End Set
        End Property

        Public Property RequireSquareContent() As Boolean
            Get
                Return m_RequireSquareContent
            End Get
            Set(value As Boolean)
                m_RequireSquareContent = value
            End Set
        End Property

        Public Overrides Function GetContent() As String
            'If SquareContent Is Nothing And RequireSquareContent Then
            '    Throw New NotificationContentValidationException("Square tile content should be included with each wide tile. " & "If this behavior is undesired, use the RequireSquareContent property.")
            'End If

            Dim visualNode As New StringBuilder(String.Empty)
            visualNode.AppendFormat("<visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Branding <> TileBranding.Logo Then
                visualNode.AppendFormat(" branding='{0}'", Branding.ToString.ToLowerInvariant())
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            visualNode.Append(">")

            Dim builder As New StringBuilder(String.Empty)
            builder.AppendFormat("<tile>{0}<binding template='{1}'>{2}</binding>", visualNode, TemplateName, SerializeProperties(Lang, BaseUri))
            If SquareContent IsNot Nothing Then
                Dim squareBase As ISquareTileInternal = TryCast(SquareContent, ISquareTileInternal)
                If squareBase Is Nothing Then
                    Throw New NotificationContentValidationException("The provided square tile content class is unsupported.")
                End If
                builder.Append(squareBase.SerializeBinding(Lang, BaseUri, Branding))
            End If
            builder.Append("</visual></tile>")

            Return builder.ToString
        End Function

        Private m_SquareContent As ISquareTileNotificationContent = Nothing
        Private m_RequireSquareContent As Boolean = True
    End Class

    Friend Class TileSquareBlock
        Inherits TileSquareBase
        Implements ITileSquareBlock
        Public Sub New()
            MyBase.New(templateName:="TileSquareBlock", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileSquareBlock.TextBlock
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileSquareBlock.TextSubBlock
            Get
                Return TextFields(1)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquareImage
        Inherits TileSquareBase
        Implements ITileSquareImage
        Public Sub New()
            MyBase.New(templateName:="TileSquareImage", imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquareImage.Image
            Get
                Return Images(0)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquarePeekImageAndText01
        Inherits TileSquareBase
        Implements ITileSquarePeekImageAndText01
        Public Sub New()
            MyBase.New(templateName:="TileSquarePeekImageAndText01", imageCount:=1, textCount:=4)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquarePeekImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquarePeekImageAndText01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquarePeekImageAndText01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquarePeekImageAndText01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquarePeekImageAndText01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquarePeekImageAndText02
        Inherits TileSquareBase
        Implements ITileSquarePeekImageAndText02

        Public Sub New()
            MyBase.New(templateName:="TileSquarePeekImageAndText02", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquarePeekImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquarePeekImageAndText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquarePeekImageAndText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquarePeekImageAndText03
        Inherits TileSquareBase
        Implements ITileSquarePeekImageAndText03
        Public Sub New()
            MyBase.New(templateName:="TileSquarePeekImageAndText03", imageCount:=1, textCount:=4)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquarePeekImageAndText03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquarePeekImageAndText03.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquarePeekImageAndText03.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquarePeekImageAndText03.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquarePeekImageAndText03.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquarePeekImageAndText04
        Inherits TileSquareBase
        Implements ITileSquarePeekImageAndText04
        Public Sub New()
            MyBase.New(templateName:="TileSquarePeekImageAndText04", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileSquarePeekImageAndText04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquarePeekImageAndText04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquareText01
        Inherits TileSquareBase
        Implements ITileSquareText01
        Public Sub New()
            MyBase.New(templateName:="TileSquareText01", imageCount:=0, textCount:=4)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquareText01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquareText01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquareText01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquareText01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquareText02
        Inherits TileSquareBase
        Implements ITileSquareText02

        Public Sub New()
            MyBase.New(templateName:="TileSquareText02", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileSquareText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquareText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquareText03
        Inherits TileSquareBase
        Implements ITileSquareText03
        Public Sub New()
            MyBase.New(templateName:="TileSquareText03", imageCount:=0, textCount:=4)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileSquareText03.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileSquareText03.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileSquareText03.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileSquareText03.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileSquareText04
        Inherits TileSquareBase
        Implements ITileSquareText04
        Public Sub New()
            MyBase.New(templateName:="TileSquareText04", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileSquareText04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
    End Class

    Friend Class TileWideBlockAndText01
        Inherits TileWideBase
        Implements ITileWideBlockAndText01

        Public Sub New()
            MyBase.New(templateName:="TileWideBlockAndText01", imageCount:=0, textCount:=6)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWideBlockAndText01.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWideBlockAndText01.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWideBlockAndText01.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWideBlockAndText01.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileWideBlockAndText01.TextBlock
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileWideBlockAndText01.TextSubBlock
            Get
                Return TextFields(5)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideBlockAndText02
        Inherits TileWideBase
        Implements ITileWideBlockAndText02

        Public Sub New()
            MyBase.New(templateName:="TileWideBlockAndText02", imageCount:=0, textCount:=6)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWideBlockAndText02.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBlock() As INotificationContentText Implements ITileWideBlockAndText02.TextBlock
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextSubBlock() As INotificationContentText Implements ITileWideBlockAndText02.TextSubBlock
            Get
                Return TextFields(2)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideImage
        Inherits TileWideBase
        Implements ITileWideImage

        Public Sub New()
            MyBase.New(templateName:="TileWideImage", imageCount:=1, textCount:=0)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideImage.Image
            Get
                Return Images(0)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideImageAndText01
        Inherits TileWideBase
        Implements ITileWideImageAndText01
        Public Sub New()
            MyBase.New(templateName:="TileWideImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextCaptionWrap() As INotificationContentText Implements ITileWideImageAndText01.TextCaptionWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Property AddImageQuery As Boolean Implements ITileWideImageAndText01.AddImageQuery


        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideImageAndText02
        Inherits TileWideBase
        Implements ITileWideImageAndText02
        Public Sub New()
            MyBase.New(templateName:="TileWideImageAndText02", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextCaption1() As INotificationContentText Implements ITileWideImageAndText02.TextCaption1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextCaption2() As INotificationContentText Implements ITileWideImageAndText02.TextCaption2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideImageCollection
        Inherits TileWideBase
        Implements ITileWideImageCollection
        Public Sub New()
            MyBase.New(templateName:="TileWideImageCollection", imageCount:=5, textCount:=0)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWideImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImage01
        Inherits TileWideBase
        Implements ITileWidePeekImage01
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImage01", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWidePeekImage01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWidePeekImage01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImage01.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImage02
        Inherits TileWideBase
        Implements ITileWidePeekImage02
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImage02", imageCount:=1, textCount:=5)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWidePeekImage02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWidePeekImage02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWidePeekImage02.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWidePeekImage02.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWidePeekImage02.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWidePeekImage02.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImage03
        Inherits TileWideBase
        Implements ITileWidePeekImage03
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImage03", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWidePeekImage03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWidePeekImage03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImage04
        Inherits TileWideBase
        Implements ITileWidePeekImage04
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImage04", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWidePeekImage04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImage04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImage05
        Inherits TileWideBase
        Implements ITileWidePeekImage05
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImage05", imageCount:=2, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWidePeekImage05.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWidePeekImage05.ImageSecondary
            Get
                Return Images(1)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWidePeekImage05.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImage05.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImage06
        Inherits TileWideBase
        Implements ITileWidePeekImage06
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImage06", imageCount:=2, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWidePeekImage06.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWidePeekImage06.ImageSecondary
            Get
                Return Images(1)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWidePeekImage06.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImageAndText01
        Inherits TileWideBase
        Implements ITileWidePeekImageAndText01

        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWidePeekImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImageAndText01.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWidePeekImageAndText02
        Inherits TileWideBase
        Implements ITileWidePeekImageAndText02
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageAndText02", imageCount:=1, textCount:=5)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWidePeekImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWidePeekImageAndText02.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWidePeekImageAndText02.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWidePeekImageAndText02.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWidePeekImageAndText02.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileWidePeekImageAndText02.TextBody5
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWidePeekImageCollection01
        Inherits TileWideBase
        Implements ITileWidePeekImageCollection01
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageCollection01", imageCount:=5, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWidePeekImageCollection01.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWidePeekImageCollection01.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWidePeekImageCollection01.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWidePeekImageCollection01.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWidePeekImageCollection01.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWidePeekImageCollection01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImageCollection01.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWidePeekImageCollection02
        Inherits TileWideBase
        Implements ITileWidePeekImageCollection02
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageCollection02", imageCount:=5, textCount:=5)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWidePeekImageCollection02.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWidePeekImageCollection02.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWidePeekImageCollection02.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWidePeekImageCollection02.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWidePeekImageCollection02.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWidePeekImageCollection02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWidePeekImageCollection02.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWidePeekImageCollection02.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWidePeekImageCollection02.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWidePeekImageCollection02.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWidePeekImageCollection03
        Inherits TileWideBase
        Implements ITileWidePeekImageCollection03
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageCollection03", imageCount:=5, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWidePeekImageCollection03.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWidePeekImageCollection03.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWidePeekImageCollection03.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWidePeekImageCollection03.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWidePeekImageCollection03.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWidePeekImageCollection03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function
        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWidePeekImageCollection04
        Inherits TileWideBase
        Implements ITileWidePeekImageCollection04
        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageCollection04", imageCount:=5, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWidePeekImageCollection04.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWidePeekImageCollection04.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWidePeekImageCollection04.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWidePeekImageCollection04.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWidePeekImageCollection04.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImageCollection04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWidePeekImageCollection05
        Inherits TileWideBase
        Implements ITileWidePeekImageCollection05

        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageCollection05", imageCount:=6, textCount:=2)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWideImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWidePeekImageCollection05.ImageSecondary
            Get
                Return Images(5)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWidePeekImageCollection05.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWidePeekImageCollection05.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWidePeekImageCollection06
        Inherits TileWideBase
        Implements ITileWidePeekImageCollection06

        Public Sub New()
            MyBase.New(templateName:="TileWidePeekImageCollection06", imageCount:=6, textCount:=1)
        End Sub

        Public ReadOnly Property ImageMain() As INotificationContentImage Implements ITileWideImageCollection.ImageMain
            Get
                Return Images(0)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn1Row1
            Get
                Return Images(1)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn2Row1
            Get
                Return Images(2)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn1Row2
            Get
                Return Images(3)
            End Get
        End Property
        Public ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage Implements ITileWideImageCollection.ImageSmallColumn2Row2
            Get
                Return Images(4)
            End Get
        End Property
        Public ReadOnly Property ImageSecondary() As INotificationContentImage Implements ITileWidePeekImageCollection06.ImageSecondary
            Get
                Return Images(5)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWidePeekImageCollection06.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent

    End Class

    Friend Class TileWideSmallImageAndText01
        Inherits TileWideBase
        Implements ITileWideSmallImageAndText01
        Public Sub New()
            MyBase.New(templateName:="TileWideSmallImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideSmallImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWideSmallImageAndText01.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideSmallImageAndText02
        Inherits TileWideBase
        Implements ITileWideSmallImageAndText02
        Public Sub New()
            MyBase.New(templateName:="TileWideSmallImageAndText02", imageCount:=1, textCount:=5)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideSmallImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideSmallImageAndText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWideSmallImageAndText02.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWideSmallImageAndText02.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWideSmallImageAndText02.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWideSmallImageAndText02.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideSmallImageAndText03
        Inherits TileWideBase

        Implements ITileWideSmallImageAndText03
        Public Sub New()
            MyBase.New(templateName:="TileWideSmallImageAndText03", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideSmallImageAndText03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWideSmallImageAndText03.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements ITileWideSmallImageAndText03.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements ITileWideSmallImageAndText03.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileWideSmallImageAndText03.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideSmallImageAndText04
        Inherits TileWideBase
        Implements ITileWideSmallImageAndText04

        Public Sub New()
            MyBase.New(templateName:="TileWideSmallImageAndText04", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements ITileWideSmallImageAndText04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideSmallImageAndText04.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWideSmallImageAndText04.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText01
        Inherits TileWideBase
        Implements ITileWideText01

        Public Sub New()
            MyBase.New(templateName:="TileWideText01", imageCount:=0, textCount:=5)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideText01.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWideText01.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWideText01.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWideText01.TextBody3
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWideText01.TextBody4
            Get
                Return TextFields(4)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText02
        Inherits TileWideBase
        Implements ITileWideText02
        Public Sub New()
            MyBase.New(templateName:="TileWideText02", imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements ITileWideText02.TextColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWideText02.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements ITileWideText02.TextColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWideText02.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements ITileWideText02.TextColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWideText02.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements ITileWideText02.TextColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWideText02.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText03
        Inherits TileWideBase
        Implements ITileWideText03
        Public Sub New()
            MyBase.New(templateName:="TileWideText03", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements ITileWideText03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText04
        Inherits TileWideBase
        Implements ITileWideText04
        Public Sub New()
            MyBase.New(templateName:="TileWideText04", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWideText04.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class


    Friend Class TileWideText05
        Inherits TileWideBase
        Implements ITileWideText05
        Public Sub New()
            MyBase.New(templateName:="TileWideText05", imageCount:=0, textCount:=5)
        End Sub

        Public ReadOnly Property TextBody1() As INotificationContentText Implements ITileWideText05.TextBody1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements ITileWideText05.TextBody2
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody3() As INotificationContentText Implements ITileWideText05.TextBody3
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextBody4() As INotificationContentText Implements ITileWideText05.TextBody4
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextBody5() As INotificationContentText Implements ITileWideText05.TextBody5
            Get
                Return TextFields(4)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText06
        Inherits TileWideBase
        Implements ITileWideText06

        Public Sub New()
            MyBase.New(templateName:="TileWideText06", imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextColumn1Row1() As INotificationContentText Implements ITileWideText06.TextColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWideText06.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row2() As INotificationContentText Implements ITileWideText06.TextColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWideText06.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row3() As INotificationContentText Implements ITileWideText06.TextColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWideText06.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row4() As INotificationContentText Implements ITileWideText06.TextColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWideText06.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn1Row5() As INotificationContentText Implements ITileWideText06.TextColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileWideText06.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText07
        Inherits TileWideBase
        Implements ITileWideText07

        Public Sub New()
            MyBase.New(templateName:="TileWideText07", imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideText07.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row1() As INotificationContentText Implements ITileWideText07.TextShortColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWideText07.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row2() As INotificationContentText Implements ITileWideText07.TextShortColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWideText07.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row3() As INotificationContentText Implements ITileWideText07.TextShortColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWideText07.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row4() As INotificationContentText Implements ITileWideText07.TextShortColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWideText07.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText08
        Inherits TileWideBase
        Implements ITileWideText08

        Public Sub New()
            MyBase.New(templateName:="TileWideText08", imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextShortColumn1Row1() As INotificationContentText Implements ITileWideText08.TextShortColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWideText08.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row2() As INotificationContentText Implements ITileWideText08.TextShortColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWideText08.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row3() As INotificationContentText Implements ITileWideText08.TextShortColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWideText08.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row4() As INotificationContentText Implements ITileWideText08.TextShortColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWideText08.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextShortColumn1Row5() As INotificationContentText Implements ITileWideText08.TextShortColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileWideText08.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText09
        Inherits TileWideBase
        Implements ITileWideText09
        Public Sub New()
            MyBase.New(templateName:="TileWideText09", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideText09.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements ITileWideText09.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property
        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText10
        Inherits TileWideBase
        Implements ITileWideText10

        Public Sub New()
            MyBase.New(templateName:="TileWideText10", imageCount:=0, textCount:=9)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements ITileWideText10.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row1() As INotificationContentText Implements ITileWideText10.TextPrefixColumn1Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWideText10.TextColumn2Row1
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row2() As INotificationContentText Implements ITileWideText10.TextPrefixColumn1Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWideText10.TextColumn2Row2
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row3() As INotificationContentText Implements ITileWideText10.TextPrefixColumn1Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWideText10.TextColumn2Row3
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row4() As INotificationContentText Implements ITileWideText10.TextPrefixColumn1Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWideText10.TextColumn2Row4
            Get
                Return TextFields(8)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    Friend Class TileWideText11
        Inherits TileWideBase
        Implements ITileWideText11
        Public Sub New()
            MyBase.New(templateName:="TileWideText11", imageCount:=0, textCount:=10)
        End Sub

        Public ReadOnly Property TextPrefixColumn1Row1() As INotificationContentText Implements ITileWideText11.TextPrefixColumn1Row1
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row1() As INotificationContentText Implements ITileWideText11.TextColumn2Row1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row2() As INotificationContentText Implements ITileWideText11.TextPrefixColumn1Row2
            Get
                Return TextFields(2)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row2() As INotificationContentText Implements ITileWideText11.TextColumn2Row2
            Get
                Return TextFields(3)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row3() As INotificationContentText Implements ITileWideText11.TextPrefixColumn1Row3
            Get
                Return TextFields(4)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row3() As INotificationContentText Implements ITileWideText11.TextColumn2Row3
            Get
                Return TextFields(5)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row4() As INotificationContentText Implements ITileWideText11.TextPrefixColumn1Row4
            Get
                Return TextFields(6)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row4() As INotificationContentText Implements ITileWideText11.TextColumn2Row4
            Get
                Return TextFields(7)
            End Get
        End Property
        Public ReadOnly Property TextPrefixColumn1Row5() As INotificationContentText Implements ITileWideText11.TextPrefixColumn1Row5
            Get
                Return TextFields(8)
            End Get
        End Property
        Public ReadOnly Property TextColumn2Row5() As INotificationContentText Implements ITileWideText11.TextColumn2Row5
            Get
                Return TextFields(9)
            End Get
        End Property

        Public Overrides Function GetContent() As String Implements INotificationContent.GetContent
            Return MyBase.GetContent()
        End Function

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Overrides Function CreateNotification() As TileNotification Implements ITileNotificationContent.CreateNotification
            Return MyBase.CreateNotification()
        End Function

        Public Property BaseUri1 As String Implements ITileNotificationContent.BaseUri
        Public Property Branding1 As TileBranding Implements ITileNotificationContent.Branding
        Public Property Lang1 As String Implements ITileNotificationContent.Lang
        Public Property StrictValidation1 As Boolean Implements ITileNotificationContent.StrictValidation
        Public Property RequireSquareContent1 As Boolean Implements IWideTileNotificationContent.RequireSquareContent
        Public Property SquareContent1 As ISquareTileNotificationContent Implements IWideTileNotificationContent.SquareContent
    End Class

    ''' <summary>
    ''' A factory which creates tile content objects for all of the toast template types.
    ''' </summary>
    Public NotInheritable Class TileContentFactory
        ''' <summary>
        ''' Creates a tile content object for the TileSquareBlock template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquareBlock template.</returns>
        Public Shared Function CreateTileSquareBlock() As ITileSquareBlock
            Return New TileSquareBlock()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquareImage template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquareImage template.</returns>
        Public Shared Function CreateTileSquareImage() As ITileSquareImage
            Return New TileSquareImage()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquarePeekImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquarePeekImageAndText01 template.</returns>
        Public Shared Function CreateTileSquarePeekImageAndText01() As ITileSquarePeekImageAndText01
            Return New TileSquarePeekImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquarePeekImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquarePeekImageAndText02 template.</returns>
        Public Shared Function CreateTileSquarePeekImageAndText02() As ITileSquarePeekImageAndText02
            Return New TileSquarePeekImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquarePeekImageAndText03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquarePeekImageAndText03 template.</returns>
        Public Shared Function CreateTileSquarePeekImageAndText03() As ITileSquarePeekImageAndText03
            Return New TileSquarePeekImageAndText03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquarePeekImageAndText04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquarePeekImageAndText04 template.</returns>
        Public Shared Function CreateTileSquarePeekImageAndText04() As ITileSquarePeekImageAndText04
            Return New TileSquarePeekImageAndText04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquareText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquareText01 template.</returns>
        Public Shared Function CreateTileSquareText01() As ITileSquareText01
            Return New TileSquareText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquareText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquareText02 template.</returns>
        Public Shared Function CreateTileSquareText02() As ITileSquareText02
            Return New TileSquareText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquareText03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquareText03 template.</returns>
        Public Shared Function CreateTileSquareText03() As ITileSquareText03
            Return New TileSquareText03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileSquareText04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileSquareText04 template.</returns>
        Public Shared Function CreateTileSquareText04() As ITileSquareText04
            Return New TileSquareText04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideBlockAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideBlockAndText01 template.</returns>
        Public Shared Function CreateTileWideBlockAndText01() As ITileWideBlockAndText01
            Return New TileWideBlockAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideBlockAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideBlockAndText02 template.</returns>
        Public Shared Function CreateTileWideBlockAndText02() As ITileWideBlockAndText02
            Return New TileWideBlockAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideImage template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideImage template.</returns>
        Public Shared Function CreateTileWideImage() As ITileWideImage
            Return New TileWideImage()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideImageAndText01 template.</returns>
        Public Shared Function CreateTileWideImageAndText01() As ITileWideImageAndText01
            Return New TileWideImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideImageAndText02 template.</returns>
        Public Shared Function CreateTileWideImageAndText02() As ITileWideImageAndText02
            Return New TileWideImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideImageCollection template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideImageCollection template.</returns>
        Public Shared Function CreateTileWideImageCollection() As ITileWideImageCollection
            Return New TileWideImageCollection()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImage01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImage01 template.</returns>
        Public Shared Function CreateTileWidePeekImage01() As ITileWidePeekImage01
            Return New TileWidePeekImage01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImage02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImage02 template.</returns>
        Public Shared Function CreateTileWidePeekImage02() As ITileWidePeekImage02
            Return New TileWidePeekImage02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImage03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImage03 template.</returns>
        Public Shared Function CreateTileWidePeekImage03() As ITileWidePeekImage03
            Return New TileWidePeekImage03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImage04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImage04 template.</returns>
        Public Shared Function CreateTileWidePeekImage04() As ITileWidePeekImage04
            Return New TileWidePeekImage04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImage05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImage05 template.</returns>
        Public Shared Function CreateTileWidePeekImage05() As ITileWidePeekImage05
            Return New TileWidePeekImage05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImage06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImage06 template.</returns>
        Public Shared Function CreateTileWidePeekImage06() As ITileWidePeekImage06
            Return New TileWidePeekImage06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageAndText01 template.</returns>
        Public Shared Function CreateTileWidePeekImageAndText01() As ITileWidePeekImageAndText01
            Return New TileWidePeekImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageAndText02 template.</returns>
        Public Shared Function CreateTileWidePeekImageAndText02() As ITileWidePeekImageAndText02
            Return New TileWidePeekImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageCollection01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageCollection01 template.</returns>
        Public Shared Function CreateTileWidePeekImageCollection01() As ITileWidePeekImageCollection01
            Return New TileWidePeekImageCollection01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageCollection02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageCollection02 template.</returns>
        Public Shared Function CreateTileWidePeekImageCollection02() As ITileWidePeekImageCollection02
            Return New TileWidePeekImageCollection02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageCollection03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageCollection03 template.</returns>
        Public Shared Function CreateTileWidePeekImageCollection03() As ITileWidePeekImageCollection03
            Return New TileWidePeekImageCollection03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageCollection04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageCollection04 template.</returns>
        Public Shared Function CreateTileWidePeekImageCollection04() As ITileWidePeekImageCollection04
            Return New TileWidePeekImageCollection04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageCollection05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageCollection05 template.</returns>
        Public Shared Function CreateTileWidePeekImageCollection05() As ITileWidePeekImageCollection05
            Return New TileWidePeekImageCollection05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWidePeekImageCollection06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWidePeekImageCollection06 template.</returns>
        Public Shared Function CreateTileWidePeekImageCollection06() As ITileWidePeekImageCollection06
            Return New TileWidePeekImageCollection06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideSmallImageAndText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideSmallImageAndText01 template.</returns>
        Public Shared Function CreateTileWideSmallImageAndText01() As ITileWideSmallImageAndText01
            Return New TileWideSmallImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideSmallImageAndText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideSmallImageAndText02 template.</returns>
        Public Shared Function CreateTileWideSmallImageAndText02() As ITileWideSmallImageAndText02
            Return New TileWideSmallImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideSmallImageAndText03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideSmallImageAndText03 template.</returns>

        Public Shared Function CreateTileWideSmallImageAndText03() As ITileWideSmallImageAndText03
            Return New TileWideSmallImageAndText03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideSmallImageAndText04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideSmallImageAndText04 template.</returns>
        Public Shared Function CreateTileWideSmallImageAndText04() As ITileWideSmallImageAndText04
            Return New TileWideSmallImageAndText04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText01 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText01 template.</returns>
        Public Shared Function CreateTileWideText01() As ITileWideText01
            Return New TileWideText01()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText02 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText02 template.</returns>
        Public Shared Function CreateTileWideText02() As ITileWideText02
            Return New TileWideText02()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText03 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText03 template.</returns>
        Public Shared Function CreateTileWideText03() As ITileWideText03
            Return New TileWideText03()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText04 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText04 template.</returns>
        Public Shared Function CreateTileWideText04() As ITileWideText04
            Return New TileWideText04()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText05 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText05 template.</returns>
        Public Shared Function CreateTileWideText05() As ITileWideText05
            Return New TileWideText05()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText06 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText06 template.</returns>
        Public Shared Function CreateTileWideText06() As ITileWideText06
            Return New TileWideText06()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText07 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText07 template.</returns>
        Public Shared Function CreateTileWideText07() As ITileWideText07
            Return New TileWideText07()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText08 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText08 template.</returns>
        Public Shared Function CreateTileWideText08() As ITileWideText08
            Return New TileWideText08()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText09 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText09 template.</returns>
        Public Shared Function CreateTileWideText09() As ITileWideText09
            Return New TileWideText09()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText10 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText10 template.</returns>
        Public Shared Function CreateTileWideText10() As ITileWideText10
            Return New TileWideText10()
        End Function

        ''' <summary>
        ''' Creates a tile content object for the TileWideText11 template.
        ''' </summary>
        ''' <returns>A tile content object for the TileWideText11 template.</returns>
        Public Shared Function CreateTileWideText11() As ITileWideText11
            Return New TileWideText11()
        End Function
    End Class
End Namespace
