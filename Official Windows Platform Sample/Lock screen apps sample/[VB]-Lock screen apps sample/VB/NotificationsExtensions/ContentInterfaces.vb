' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

#If Not WINRT_NOT_PRESENT Then
Imports Windows.UI.Notifications
Imports Windows.Data.Xml.Dom
#End If

''' <summary>
''' Base notification content interface to retrieve notification Xml as a string.
''' </summary>
Public Interface INotificationContent
    ''' <summary>
    ''' Retrieves the notification Xml content as a string.
    ''' </summary>
    ''' <returns>The notification Xml content as a string.</returns>
    Function GetContent() As String

#If Not WINRT_NOT_PRESENT Then
    ''' <summary>
    ''' Retrieves the notification Xml content as a WinRT Xml document.
    ''' </summary>
    ''' <returns>The notification Xml content as a WinRT Xml document.</returns>
    Function GetXml() As XmlDocument
#End If
End Interface

''' <summary>
''' A type contained by the tile and toast notification content objects that
''' represents a text field in the template.
''' </summary>
Public Interface INotificationContentText
    ''' <summary>
    ''' The text value that will be shown in the text field.
    ''' </summary>
    Property Text() As String

    ''' <summary>
    ''' The language of the text field.  This proprety overrides the language provided in the
    ''' containing notification object.  The language should be specified using the
    ''' abbreviated language code as defined by BCP 47.
    ''' </summary>
    Property Lang() As String
End Interface

''' <summary>
''' A type contained by the tile and toast notification content objects that
''' represents an image in a template.
''' </summary>
Public Interface INotificationContentImage
    ''' <summary>
    ''' The location of the image.  Relative image paths use the BaseUri provided in the containing
    ''' notification object.  If no BaseUri is provided, paths are relative to ms-appx:///.
    ''' Only png and jpg images are supported.  Images must be 1024x1024 pixels or less, and smaller than
    ''' 200 kB in size.
    ''' </summary>
    Property Src() As String

    ''' <summary>
    ''' Alt text that describes the image.
    ''' </summary>
    Property Alt() As String

    ''' <summary>
    ''' Controls if query strings that denote the client configuration of contrast, scale, and language setting should be appended to the Src
    ''' If true, Windows will append query strings onto the Src
    ''' If false, Windows will not append query strings onto the Src
    ''' Query string details:
    ''' Parameter: ms-contrast
    '''     Values: standard, black, white
    ''' Parameter: ms-scale
    '''     Values: 80, 100, 140, 180
    ''' Parameter: ms-lang
    '''     Values: The BCP 47 language tag set in the notification xml, or if omitted, the current preferred language of the user
    ''' </summary>
    Property AddImageQuery() As Boolean
End Interface

Namespace TileContent
    ''' <summary>
    ''' Base tile notification content interface.
    ''' </summary>
    Public Interface ITileNotificationContent
        Inherits INotificationContent

        ''' <summary>
        ''' Whether strict validation should be applied when the Xml or notification object is created,
        ''' and when some of the properties are assigned.
        ''' </summary>
        Property StrictValidation() As Boolean

        ''' <summary>
        ''' The language of the content being displayed.  The language should be specified using the
        ''' abbreviated language code as defined by BCP 47.
        ''' </summary>
        Property Lang() As String

        ''' <summary>
        ''' The BaseUri that should be used for image locations.  Relative image locations use this
        ''' field as their base Uri.  The BaseUri must begin with http://, https://, ms-appx:///, or
        ''' ms-appdata:///local/.
        ''' </summary>
        Property BaseUri() As String

        ''' <summary>
        ''' Determines the application branding when tile notification content is displayed on the tile.
        ''' </summary>
        Property Branding() As TileBranding

        ''' <summary>
        ''' Controls if query strings that denote the client configuration of contrast, scale, and language setting should be appended to the Src
        ''' If true, Windows will append query strings onto images that exist in this template
        ''' If false (the default), Windows will not append query strings onto images that exist in this template
        ''' Query string details:
        ''' Parameter: ms-contrast
        '''     Values: standard, black, white
        ''' Parameter: ms-scale
        '''     Values: 80, 100, 140, 180
        ''' Parameter: ms-lang
        '''     Values: The BCP 47 language tag set in the notification xml, or if omitted, the current preferred language of the user
        ''' </summary>
        Property AddImageQuery() As Boolean

        ''' <summary>
        ''' Used by the system to do semantic deduplication of content with the same contentId.
        ''' </summary>
        Property ContentId() As String

#If Not WINRT_NOT_PRESENT Then
        ''' <summary>
        ''' Creates a WinRT TileNotification object based on the content.
        ''' </summary>
        ''' <returns>The WinRT TileNotification object</returns>
        Function CreateNotification() As TileNotification
#End If
    End Interface

    ''' <summary>
    ''' Base square tile notification content interface.
    ''' </summary>
    Public Interface ISquare150x150TileNotificationContent
        Inherits ITileNotificationContent

    End Interface

    ''' <summary>
    ''' Base wide tile notification content interface.
    ''' </summary>
    Public Interface IWide310x150TileNotificationContent
        Inherits ITileNotificationContent

        ''' <summary>
        ''' Corresponding square tile notification content should be a part of every wide tile notification.
        ''' </summary>
        Property Square150x150Content() As ISquare150x150TileNotificationContent

        ''' <summary>
        ''' Whether square tile notification content needs to be added to pass
        ''' validation.  Square150x150 content is required by default.
        ''' </summary>
        Property RequireSquare150x150Content() As Boolean
    End Interface

    ''' <summary>
    ''' Base large tile notification content interface.
    ''' </summary>
    Public Interface ISquare310x310TileNotificationContent
        Inherits ITileNotificationContent

        ''' <summary>
        ''' Corresponding wide tile notification content should be a part of every large tile notification.
        ''' </summary>
        Property Wide310x150Content() As IWide310x150TileNotificationContent

        ''' <summary>
        ''' Whether wide tile notification content needs to be added to pass
        ''' validation.  Wide310x150 content is required by default.
        ''' </summary>
        Property RequireWide310x150Content() As Boolean
    End Interface

    ''' <summary>
    ''' Base Windows Phone small tile notification content interface.
    ''' </summary>
    Public Interface ISquare99x99TileNotificationContent
        Inherits ITileNotificationContent

    End Interface

    ''' <summary>
    ''' Base Windows Phone medium tile notification content interface.
    ''' </summary>
    Public Interface ISquare210x210TileNotificationContent
        Inherits ITileNotificationContent

        ''' <summary>
        ''' Corresponding small tile notification content should be a part of every medium tile notification.
        ''' </summary>
        Property Square99x99Content() As ISquare99x99TileNotificationContent

        ''' <summary>
        ''' Whether small tile notification content needs to be added to pass
        ''' validation.  Square99x99 content is required by default.
        ''' </summary>
        Property RequireSquare99x99Content() As Boolean
    End Interface

    ''' <summary>
    ''' Base Windows Phone large tile notification content interface.
    ''' </summary>
    Public Interface IWide432x210TileNotificationContent
        Inherits ITileNotificationContent

        ''' <summary>
        ''' Corresponding medium tile notification content should be a part of every large tile notification.
        ''' </summary>
        Property Square210x210Content() As ISquare210x210TileNotificationContent

        ''' <summary>
        ''' Whether medium tile notification content needs to be added to pass
        ''' validation.  Square210x210 content is required by default.
        ''' </summary>
        Property RequireSquare210x210Content() As Boolean
    End Interface

    ''' <summary>
    ''' A square tile template that displays two text captions.
    ''' </summary>
    Public Interface ITileSquare150x150Block
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock() As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image.
    ''' </summary>
    Public Interface ITileSquare150x150Image
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to show
    ''' four text fields.
    ''' </summary>
    Public Interface ITileSquare150x150PeekImageAndText01
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to show
    ''' two text fields.
    ''' </summary>
    Public Interface ITileSquare150x150PeekImageAndText02
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to show
    ''' four text fields.
    ''' </summary>
    Public Interface ITileSquare150x150PeekImageAndText03
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText
        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to
    ''' show a text field.
    ''' </summary>
    Public Interface ITileSquare150x150PeekImageAndText04
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays four text fields.
    ''' </summary>
    Public Interface ITileSquare150x150Text01
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays two text fields.
    ''' </summary>
    Public Interface ITileSquare150x150Text02
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays four text fields.
    ''' </summary>
    Public Interface ITileSquare150x150Text03
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays a text field.
    ''' </summary>
    Public Interface ITileSquare150x150Text04
        Inherits ISquare150x150TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays six text fields.
    ''' </summary>
    Public Interface ITileWide310x150BlockAndText01
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock() As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays three text fields.
    ''' </summary>
    Public Interface ITileWide310x150BlockAndText02
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock() As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image.
    ''' </summary>
    Public Interface ITileWide310x150Image
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text caption.
    ''' </summary>
    Public Interface ITileWide310x150ImageAndText01
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextCaptionWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and two text captions.
    ''' </summary>
    Public Interface ITileWide310x150ImageAndText02
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' The first caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption1() As INotificationContentText

        ''' <summary>
        ''' The second caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid.
    ''' </summary>
    Public Interface ITileWide310x150ImageCollection
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn1Row1() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn2Row1() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn1Row2() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn2Row2() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' two text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImage01
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' five text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImage02
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' a text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImage03
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' a text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImage04
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' another image and two text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImage05
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain() As INotificationContentImage

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' another image and a text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImage06
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain() As INotificationContentImage

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a portion of a text field,
    ''' then transitions to show all of the text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageAndText01
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text field,
    ''' then transitions to show the text field and four other text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageAndText02
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show two
    ''' text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageCollection01
        Inherits ITileWide310x150ImageCollection

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show five
    ''' text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageCollection02
        Inherits ITileWide310x150ImageCollection

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show a
    ''' text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageCollection03
        Inherits ITileWide310x150ImageCollection

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show a
    ''' text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageCollection04
        Inherits ITileWide310x150ImageCollection

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show an image
    ''' and two text fields.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageCollection05
        Inherits ITileWide310x150ImageCollection

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show an image
    ''' and a text field.
    ''' </summary>
    Public Interface ITileWide310x150PeekImageCollection06
        Inherits ITileWide310x150ImageCollection

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text field.
    ''' </summary>
    Public Interface ITileWide310x150SmallImageAndText01
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and 5 text fields.
    ''' </summary>
    Public Interface ITileWide310x150SmallImageAndText02
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text field.
    ''' </summary>
    Public Interface ITileWide310x150SmallImageAndText03
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and two text fields.
    ''' </summary>
    Public Interface ITileWide310x150SmallImageAndText04
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays a poster image and two text fields.
    ''' </summary>
    Public Interface ITileWide310x150SmallImageAndText05
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text01
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nine text fields - a heading and two columns
    ''' of four text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text02
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays a text field.
    ''' </summary>
    Public Interface ITileWide310x150Text03
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays a text field.
    ''' </summary>
    Public Interface ITileWide310x150Text04
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text05
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays ten text fields - two columns
    ''' of five text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text06
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nine text fields - a heading and two columns
    ''' of four text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text07
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays ten text fields - two columns
    ''' of five text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text08
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays two text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text09
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nine text fields - a heading and two columns
    ''' of four text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text10
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays ten text fields - two columns
    ''' of five text fields.
    ''' </summary>
    Public Interface ITileWide310x150Text11
        Inherits IWide310x150TileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large tile template that displays a heading,
    ''' six text fields grouped into three units,
    ''' and two more text fields.
    ''' </summary>
    Public Interface ITileSquare310x310BlockAndText01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody6() As INotificationContentText

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock() As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large tile template that displays seven text fields with one background image.
    ''' </summary>
    Public Interface ITileSquare310x310BlockAndText02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The background image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock() As INotificationContentText

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays an image.
    ''' </summary>
    Public Interface ITileSquare310x310Image
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A large square tile template that displays an image and a text caption.
    ''' </summary>
    Public Interface ITileSquare310x310ImageAndText01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextCaptionWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays an image and two text captions.
    ''' </summary>
    Public Interface ITileSquare310x310ImageAndText02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' The first caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption1() As INotificationContentText

        ''' <summary>
        ''' The second caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a heading with a background image.
    ''' </summary>
    Public Interface ITileSquare310x310ImageAndTextOverlay01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a heading and a text field with a background image.
    ''' </summary>
    Public Interface ITileSquare310x310ImageAndTextOverlay02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a heading and three text fields with a background image.
    ''' </summary>
    Public Interface ITileSquare310x310ImageAndTextOverlay03
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays five images - one main image,
    ''' and four smaller images in a row across the top.
    ''' </summary>
    Public Interface ITileSquare310x310ImageCollection
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain() As INotificationContentImage

        ''' <summary>
        ''' A small image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall1() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall2() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall3() As INotificationContentImage

        ''' <summary>
        ''' A small image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall4() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A large square tile template that displays five images - one main image,
    ''' four smaller images in a row across the top, and a text caption.
    ''' </summary>
    Public Interface ITileSquare310x310ImageCollectionAndText01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain() As INotificationContentImage

        ''' <summary>
        ''' A small image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall1() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall2() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall3() As INotificationContentImage

        ''' <summary>
        ''' A small image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall4() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextCaptionWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays five images - one main image,
    ''' four smaller images in a row across the top, and two text captions.
    ''' </summary>
    Public Interface ITileSquare310x310ImageCollectionAndText02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain() As INotificationContentImage

        ''' <summary>
        ''' A small image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall1() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall2() As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall3() As INotificationContentImage

        ''' <summary>
        ''' A small image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmall4() As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption1() As INotificationContentText

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays an image, a headline, a text field that can wrap to two lines,
    ''' and a text field.
    ''' </summary>
    Public Interface ITileSquare310x310SmallImageAndText01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The image at the top of the tile.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' The headline text.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' The middle text field that wraps to two lines.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText

        ''' <summary>
        ''' The lower text field.
        ''' </summary>
        ReadOnly Property TextBody() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three groups, each group having an image,
    ''' a heading, and two text fields.
    ''' </summary>
    Public Interface ITileSquare310x310SmallImagesAndTextList01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The image for the first group in the list.
        ''' </summary>
        ReadOnly Property Image1() As INotificationContentImage

        ''' <summary>
        ''' The heading for the first group in the list.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' The first text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup1Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup1Field2() As INotificationContentText

        ''' <summary>
        ''' The image for the second group in the list.
        ''' </summary>
        ReadOnly Property Image2() As INotificationContentImage

        ''' <summary>
        ''' The heading for the second group in the list.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' The first text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup2Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup2Field2() As INotificationContentText

        ''' <summary>
        ''' The image for the third group in the list.
        ''' </summary>
        ReadOnly Property Image3() As INotificationContentImage

        ''' <summary>
        ''' The heading for the third group in the list.
        ''' </summary>
        ReadOnly Property TextHeading3() As INotificationContentText

        ''' <summary>
        ''' The first text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup3Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup3Field2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three groups, each group having an image,
    ''' and a text field that can wrap to a total of three lines.
    ''' </summary>
    Public Interface ITileSquare310x310SmallImagesAndTextList02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The image for the first group in the list.
        ''' </summary>
        ReadOnly Property Image1() As INotificationContentImage

        ''' <summary>
        ''' The text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextWrap1() As INotificationContentText

        ''' <summary>
        ''' The image for the second group in the list.
        ''' </summary>
        ReadOnly Property Image2() As INotificationContentImage

        ''' <summary>
        ''' The text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextWrap2() As INotificationContentText

        ''' <summary>
        ''' The image for the third group in the list.
        ''' </summary>
        ReadOnly Property Image3() As INotificationContentImage

        ''' <summary>
        ''' The  text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextWrap3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three groups, each group having an image,
    ''' a heading, and one text field, which wraps to two lines.
    ''' </summary>
    Public Interface ITileSquare310x310SmallImagesAndTextList03
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The image for the first group in the list.
        ''' </summary>
        ReadOnly Property Image1() As INotificationContentImage

        ''' <summary>
        ''' The heading for the first group in the list.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' The first text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextWrap1() As INotificationContentText

        ''' <summary>
        ''' The image for the second group in the list.
        ''' </summary>
        ReadOnly Property Image2() As INotificationContentImage

        ''' <summary>
        ''' The heading for the second group in the list.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' The first text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextWrap2() As INotificationContentText

        ''' <summary>
        ''' The image for the third group in the list.
        ''' </summary>
        ReadOnly Property Image3() As INotificationContentImage

        ''' <summary>
        ''' The heading for the third group in the list.
        ''' </summary>
        ReadOnly Property TextHeading3() As INotificationContentText

        ''' <summary>
        ''' The first text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextWrap3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three groups, each group having
    ''' a heading, and one text field, which wraps to two lines followed by an image.
    ''' </summary>
    Public Interface ITileSquare310x310SmallImagesAndTextList04
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The image for the first group in the list.
        ''' </summary>
        ReadOnly Property Image1() As INotificationContentImage

        ''' <summary>
        ''' The heading for the first group in the list.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' The first text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextWrap1() As INotificationContentText

        ''' <summary>
        ''' The image for the second group in the list.
        ''' </summary>
        ReadOnly Property Image2() As INotificationContentImage

        ''' <summary>
        ''' The heading for the second group in the list.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' The first text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextWrap2() As INotificationContentText

        ''' <summary>
        ''' The image for the third group in the list.
        ''' </summary>
        ReadOnly Property Image3() As INotificationContentImage

        ''' <summary>
        ''' The heading for the third group in the list.
        ''' </summary>
        ReadOnly Property TextHeading3() As INotificationContentText

        ''' <summary>
        ''' The first text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextWrap3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a headline and a list of three groups, each group having
    ''' an image and two text fields.
    ''' </summary>
    Public Interface ITileSquare310x310SmallImagesAndTextList05
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The headline text.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' The image for the first group in the list.
        ''' </summary>
        ReadOnly Property Image1() As INotificationContentImage

        ''' <summary>
        ''' The first text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextGroup1Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextGroup1Field2() As INotificationContentText

        ''' <summary>
        ''' The image for the second group in the list.
        ''' </summary>
        ReadOnly Property Image2() As INotificationContentImage

        ''' <summary>
        ''' The first text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextGroup2Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextGroup2Field2() As INotificationContentText

        ''' <summary>
        ''' The image for the third group in the list.
        ''' </summary>
        ReadOnly Property Image3() As INotificationContentImage

        ''' <summary>
        ''' The first text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextGroup3Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextGroup3Field2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a heading and nine text fields.
    ''' </summary>
    Public Interface ITileSquare310x310Text01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody6() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody7() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody8() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody9() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nineteen text fields - a heading and two columns
    ''' of nine text fields.
    ''' </summary>
    Public Interface ITileSquare310x310Text02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row6() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row6() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row7() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row7() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row8() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row8() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row9() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row9() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays eleven text fields.
    ''' </summary>
    Public Interface ITileSquare310x310Text03
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody6() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody7() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody8() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody9() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody10() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody11() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays 22 text fields - two columns
    ''' of 11 text fields.
    ''' </summary>
    Public Interface ITileSquare310x310Text04
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row6() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row6() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row7() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row7() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row8() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row8() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row9() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row9() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row10() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row10() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row11() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row11() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nineteen text fields - a heading and two columns
    ''' of nine text fields. Column 1 is narrower than column 2.
    ''' </summary>
    Public Interface ITileSquare310x310Text05
        Inherits ITileSquare310x310Text02

        ' ITileSquare310x310Text05 has the same properties as ITileSquare310x310Text02
    End Interface

    ''' <summary>
    ''' A wide tile template that displays 22 text fields - two columns
    ''' of eleven text fields. Column 1 is narrower than column 2.
    ''' </summary>
    Public Interface ITileSquare310x310Text06
        Inherits ITileSquare310x310Text04

        ' ITileSquare310x310Text06 has the same properties as ITileSquare310x310Text04
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nineteen text fields - a heading and two columns
    ''' of nine text fields. Column 1 is very narrow.
    ''' </summary>
    Public Interface ITileSquare310x310Text07
        Inherits ITileSquare310x310Text02

        ' ITileSquare310x310Text07 has the same properties as ITileSquare310x310Text02
    End Interface

    ''' <summary>
    ''' A wide tile template that displays 22 text fields - two columns
    ''' of eleven text fields. Column 1 is very narrow.
    ''' </summary>
    Public Interface ITileSquare310x310Text08
        Inherits ITileSquare310x310Text04

        ' ITileSquare310x310Text08 has the same properties as ITileSquare310x310Text04
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a heading which wraps to two lines,
    ''' two more heading-sized text fields, and two text fields.
    ''' </summary>
    Public Interface ITileSquare310x310Text09
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three groups, each group having
    ''' a heading, and two text fields.
    ''' </summary>
    Public Interface ITileSquare310x310TextList01
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The heading for the first group in the list.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' The first text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup1Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup1Field2() As INotificationContentText

        ''' <summary>
        ''' The heading for the second group in the list.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' The first text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup2Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup2Field2() As INotificationContentText

        ''' <summary>
        ''' The heading for the third group in the list.
        ''' </summary>
        ReadOnly Property TextHeading3() As INotificationContentText

        ''' <summary>
        ''' The first text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup3Field1() As INotificationContentText

        ''' <summary>
        ''' The second text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextBodyGroup3Field2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three text fields that can wrap to a total of three lines.
    ''' </summary>
    Public Interface ITileSquare310x310TextList02
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextWrap1() As INotificationContentText

        ''' <summary>
        ''' The text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextWrap2() As INotificationContentText

        ''' <summary>
        ''' The  text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextWrap3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A large square tile template that displays a list of three groups, each group having
    ''' a heading, and one text field, which wraps to two lines.
    ''' </summary>
    Public Interface ITileSquare310x310TextList03
        Inherits ISquare310x310TileNotificationContent

        ''' <summary>
        ''' The heading for the first group in the list.
        ''' </summary>
        ReadOnly Property TextHeading1() As INotificationContentText

        ''' <summary>
        ''' The first text field for the first group in the list.
        ''' </summary>
        ReadOnly Property TextWrap1() As INotificationContentText

        ''' <summary>
        ''' The heading for the second group in the list.
        ''' </summary>
        ReadOnly Property TextHeading2() As INotificationContentText

        ''' <summary>
        ''' The first text field for the second group in the list.
        ''' </summary>
        ReadOnly Property TextWrap2() As INotificationContentText

        ''' <summary>
        ''' The heading for the third group in the list.
        ''' </summary>
        ReadOnly Property TextHeading3() As INotificationContentText

        ''' <summary>
        ''' The first text field for the third group in the list.
        ''' </summary>
        ReadOnly Property TextWrap3() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A Windows Phone small tile template that displays a monochrome icon with a badge.
    ''' </summary>
    Public Interface ITileSquare99x99IconWithBadge
        Inherits ISquare99x99TileNotificationContent

        ''' <summary>
        ''' The image for the icon.
        ''' </summary>
        ReadOnly Property ImageIcon() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A Windows Phone medium tile template that displays a monochrome icon with a badge.
    ''' </summary>
    Public Interface ITileSquare210x210IconWithBadge
        Inherits ISquare210x210TileNotificationContent

        ''' <summary>
        ''' The image for the icon.
        ''' </summary>
        ReadOnly Property ImageIcon() As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A Windows Phone large tile template that displays a monochrome icon with a badge and three lines of text.
    ''' </summary>
    Public Interface ITileWide432x210IconWithBadgeAndText
        Inherits IWide432x210TileNotificationContent

        ''' <summary>
        ''' The image for the icon.
        ''' </summary>
        ReadOnly Property ImageIcon() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' The types of behavior that can be used for application branding when
    ''' tile notification content is displayed on the tile.
    ''' </summary>
    Public Enum TileBranding
        ''' <summary>
        ''' No application branding will be displayed on the tile content.
        ''' </summary>
        None = 0
        ''' <summary>
        ''' The application logo will be displayed with the tile content.
        ''' </summary>
        Logo
        ''' <summary>
        ''' The application name will be displayed with the tile content.
        ''' </summary>
        Name
    End Enum
End Namespace

Namespace ToastContent
    ''' <summary>
    ''' Type representing the toast notification audio properties which is contained within
    ''' a toast notification content object.
    ''' </summary>
    Public Interface IToastAudio
        ''' <summary>
        ''' The audio content that should be played when the toast is shown.
        ''' </summary>
        Property Content() As ToastAudioContent

        ''' <summary>
        ''' Whether the audio should loop.  If this property is set to true, the toast audio content
        ''' must be a looping sound.
        ''' </summary>
        Property [Loop]() As Boolean
    End Interface

    ''' <summary>
    ''' Type representing the incoming call command properties which is contained within
    ''' a toast notification content object.
    ''' </summary>
    Public Interface IIncomingCallCommands
        ''' <summary>
        ''' Whether the toast shows the video button.
        ''' </summary>
        Property ShowVideoCommand() As Boolean

        ''' <summary>
        ''' The argument passed to the app when the video command is invoked.
        ''' </summary>
        Property VideoArgument() As String

        ''' <summary>
        ''' Whether the toast shows the voice button.
        ''' </summary>
        Property ShowVoiceCommand() As Boolean

        ''' <summary>
        ''' The argument passed to the app when the voice command is invoked.
        ''' </summary>
        Property VoiceArgument() As String

        ''' <summary>
        ''' Whether the toast shows the decline button.
        ''' </summary>
        Property ShowDeclineCommand() As Boolean

        ''' <summary>
        ''' The argument passed to the app when the decline command is invoked.
        ''' </summary>
        Property DeclineArgument() As String
    End Interface

    ''' <summary>
    ''' Type representing the alarm command properties which is contained within
    ''' a toast notification content object.
    ''' </summary>
    Public Interface IAlarmCommands
        ''' <summary>
        ''' Whether the toast shows the snooze button.
        ''' </summary>
        Property ShowSnoozeCommand() As Boolean

        ''' <summary>
        ''' The argument passed to the app when the snooze command is invoked.
        ''' </summary>
        Property SnoozeArgument() As String

        ''' <summary>
        ''' Whether the toast shows the dismiss button.
        ''' </summary>
        Property ShowDismissCommand() As Boolean

        ''' <summary>
        ''' The argument passed to the app when the dismiss command is invoked.
        ''' </summary>
        Property DismissArgument() As String
    End Interface

    ''' <summary>
    ''' Base toast notification content interface.
    ''' </summary>
    Public Interface IToastNotificationContent
        Inherits INotificationContent

        ''' <summary>
        ''' Whether strict validation should be applied when the Xml or notification object is created,
        ''' and when some of the properties are assigned.
        ''' </summary>
        Property StrictValidation() As Boolean

        ''' <summary>
        ''' The language of the content being displayed.  The language should be specified using the
        ''' abbreviated language code as defined by BCP 47.
        ''' </summary>
        Property Lang() As String

        ''' <summary>
        ''' The BaseUri that should be used for image locations.  Relative image locations use this
        ''' field as their base Uri.  The BaseUri must begin with http://, https://, ms-appx:///, or
        ''' ms-appdata:///local/.
        ''' </summary>
        Property BaseUri() As String

        ''' <summary>
        ''' Controls if query strings that denote the client configuration of contrast, scale, and language setting should be appended to the Src
        ''' If true, Windows will append query strings onto images that exist in this template
        ''' If false (the default, Windows will not append query strings onto images that exist in this template
        ''' Query string details:
        ''' Parameter: ms-contrast
        '''     Values: standard, black, white
        ''' Parameter: ms-scale
        '''     Values: 80, 100, 140, 180
        ''' Parameter: ms-lang
        '''     Values: The BCP 47 language tag set in the notification xml, or if omitted, the current preferred language of the user
        ''' </summary>
        Property AddImageQuery() As Boolean

        ''' <summary>
        ''' The launch parameter passed into the Windows Store app when the toast is activated.
        ''' </summary>
        Property Launch() As String

        ''' <summary>
        ''' The audio that should be played when the toast is displayed.
        ''' </summary>
        ReadOnly Property Audio() As IToastAudio

        ''' <summary>
        ''' The length that the toast should be displayed on screen.
        ''' </summary>
        Property Duration() As ToastDuration

        ''' <summary>
        ''' IncomingCall action buttons will be displayed on the toast if one of the option flags (ShowVideoCommand/ShowVoiceCommand/ShowDeclineCommand) is set to true.
        ''' To enable IncomingCall toasts for an app, ensure that the Lock Screen Call extension is enabled in the application manifest.
        ''' </summary>
        ReadOnly Property IncomingCallCommands() As IIncomingCallCommands

        ''' <summary>
        ''' Alarm action buttons will be displayed on the toast if one of the option flags (ShowSnoozeCommand/ShowDismissCommand) is set to true.
        ''' To enable Alarm toasts for an app, ensure the app declares itself as Alarm capable.
        ''' The app needs to be set as the Primary Alarm on the PC Settings page (only one app can exist as the alarm app at a given time).
        ''' </summary>
        ReadOnly Property AlarmCommands() As IAlarmCommands

#If Not WINRT_NOT_PRESENT Then
        ''' <summary>
        ''' Creates a WinRT ToastNotification object based on the content.
        ''' </summary>
        ''' <returns>A WinRT ToastNotification object based on the content.</returns>
        Function CreateNotification() As ToastNotification
#End If
    End Interface

    ''' <summary>
    ''' The audio options that can be played while the toast is on screen.
    ''' </summary>
    Public Enum ToastAudioContent
        ''' <summary>
        ''' The default toast audio sound.
        ''' </summary>
        [Default] = 0
        ''' <summary>
        ''' Audio that corresponds to new mail arriving.
        ''' </summary>
        Mail
        ''' <summary>
        ''' Audio that corresponds to a new SMS message arriving.
        ''' </summary>
        SMS
        ''' <summary>
        ''' Audio that corresponds to a new IM arriving.
        ''' </summary>
        IM
        ''' <summary>
        ''' Audio that corresponds to a reminder.
        ''' </summary>
        Reminder
        ''' <summary>
        ''' The default looping sound.  Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall2
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall3
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall4
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall5
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall6
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall7
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall8
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall9
        ''' <summary>
        ''' Audio that corresponds to a call.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingCall10
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm2
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm3
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm4
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm5
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm6
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm7
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm8
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm9
        ''' <summary>
        ''' Audio that corresponds to an alarm.
        ''' Only valid for toasts that are have the duration set to "Long".
        ''' </summary>
        LoopingAlarm10
        ''' <summary>
        ''' No audio should be played when the toast is displayed.
        ''' </summary>
        Silent
    End Enum

    ''' <summary>
    ''' The duration the toast should be displayed on screen.
    ''' </summary>
    Public Enum ToastDuration
        ''' <summary>
        ''' Default behavior.  The toast will be on screen for a short amount of time.
        ''' </summary>
        [Short] = 0
        ''' <summary>
        ''' The toast will be on screen for a longer amount of time.
        ''' </summary>
        [Long]
    End Enum

    ''' <summary>
    ''' A toast template that displays an image and a text field.
    ''' </summary>
    Public Interface IToastImageAndText01
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays an image and two text fields.
    ''' </summary>
    Public Interface IToastImageAndText02
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays an image and two text fields.
    ''' </summary>
    Public Interface IToastImageAndText03
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays an image and three text fields.
    ''' </summary>
    Public Interface IToastImageAndText04
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image() As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays a text fields.
    ''' </summary>
    Public Interface IToastText01
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays two text fields.
    ''' </summary>
    Public Interface IToastText02
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays two text fields.
    ''' </summary>
    Public Interface IToastText03
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody() As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays three text fields.
    ''' </summary>
    Public Interface IToastText04
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2() As INotificationContentText
    End Interface
End Namespace

Namespace BadgeContent
    ''' <summary>
    ''' Base badge notification content interface.
    ''' </summary>
    Public Interface IBadgeNotificationContent
        Inherits INotificationContent

#If Not WINRT_NOT_PRESENT Then
        ''' <summary>
        ''' Creates a WinRT BadgeNotification object based on the content.
        ''' </summary>
        ''' <returns>A WinRT BadgeNotification object based on the content.</returns>
        Function CreateNotification() As BadgeNotification
#End If
    End Interface

    ''' <summary>
    ''' The types of glyphs that can be placed on a badge.
    ''' </summary>
    Public Enum GlyphValue
        ''' <summary>
        ''' No glyph.  If there is a numeric badge, or a glyph currently on the badge,
        ''' it will be removed.
        ''' </summary>
        None = 0
        ''' <summary>
        ''' A glyph representing application activity.
        ''' </summary>
        Activity
        ''' <summary>
        ''' A glyph representing an alert.
        ''' </summary>
        Alert
        ''' <summary>
        ''' A glyph representing availability status.
        ''' </summary>
        Available
        ''' <summary>
        ''' A glyph representing away status
        ''' </summary>
        Away
        ''' <summary>
        ''' A glyph representing busy status.
        ''' </summary>
        Busy
        ''' <summary>
        ''' A glyph representing that a new message is available.
        ''' </summary>
        NewMessage
        ''' <summary>
        ''' A glyph representing that media is paused.
        ''' </summary>
        Paused
        ''' <summary>
        ''' A glyph representing that media is playing.
        ''' </summary>
        Playing
        ''' <summary>
        ''' A glyph representing unavailable status.
        ''' </summary>
        Unavailable
        ''' <summary>
        ''' A glyph representing an error.
        ''' </summary>
        [Error]
        ''' <summary>
        ''' A glyph representing attention status.
        ''' </summary>
        Attention
        ''' <summary>
        ''' A glyph representing an alarm.
        ''' </summary>
        Alarm
    End Enum
End Namespace
