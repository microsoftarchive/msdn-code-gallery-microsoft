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
    Property Text As String

    ''' <summary>
    ''' The language of the text field.  This proprety overrides the language provided in the
    ''' containing notification object.  The language should be specified using the
    ''' abbreviated language code as defined by BCP 47.
    ''' </summary>
    Property Lang As String
End Interface

''' <summary>
''' A type contained by the tile and toast notification content objects that
''' represents an image in a template.
''' </summary>
Public Interface INotificationContentImage
    ''' <summary>
    ''' The location of the image.  Relative image paths use the BaseUri provided in the containing
    ''' notification object.  If no BaseUri is provided, paths are relative to ms-appx:///.
    ''' Only png and jpg images are supported.  Images must be 800x800 pixels or less, and smaller than
    ''' 150 kB in size.
    ''' </summary>
    Property Src As String

    ''' <summary>
    ''' Alt text that describes the image.
    ''' </summary>
    Property Alt As String
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
        Property StrictValidation As Boolean

        ''' <summary>
        ''' The language of the content being displayed.  The language should be specified using the
        ''' abbreviated language code as defined by BCP 47.
        ''' </summary>
        Property Lang As String

        ''' <summary>
        ''' The BaseUri that should be used for image locations.  Relative image locations use this
        ''' field as their base Uri.  The BaseUri must begin with http://, https://, ms-appx:///, or 
        ''' ms-appdata:///local/.
        ''' </summary>
        Property BaseUri As String

        ''' <summary>
        ''' Determines the application branding when tile notification content is displayed on the tile.
        ''' </summary>
        Property Branding As TileBranding

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
    Public Interface ISquareTileNotificationContent
        Inherits ITileNotificationContent
    End Interface

    ''' <summary>
    ''' Base wide tile notification content interface.
    ''' </summary>
    Public Interface IWideTileNotificationContent
        Inherits ITileNotificationContent

        ''' <summary>
        ''' Corresponding square tile notification content should be a part of every wide tile notification.
        ''' </summary>
        Property SquareContent As ISquareTileNotificationContent

        ''' <summary>
        ''' Whether square tile notification content needs to be added to pass
        ''' validation.  Square content is required by default.
        ''' </summary>
        Property RequireSquareContent As Boolean
    End Interface

    ''' <summary>
    ''' A square tile template that displays two text captions.
    ''' </summary>
    Public Interface ITileSquareBlock
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image.
    ''' </summary>
    Public Interface ITileSquareImage
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to show
    ''' four text fields.
    ''' </summary>
    Public Interface ITileSquarePeekImageAndText01
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to show
    ''' two text fields.
    ''' </summary>
    Public Interface ITileSquarePeekImageAndText02
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to show
    ''' four text fields.
    ''' </summary>
    Public Interface ITileSquarePeekImageAndText03
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText
        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays an image, then transitions to 
    ''' show a text field.
    ''' </summary>
    Public Interface ITileSquarePeekImageAndText04
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays four text fields.
    ''' </summary>
    Public Interface ITileSquareText01
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays two text fields.
    ''' </summary>
    Public Interface ITileSquareText02
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays four text fields.
    ''' </summary>
    Public Interface ITileSquareText03
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays a text field.
    ''' </summary>
    Public Interface ITileSquareText04
        Inherits ISquareTileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays six text fields.
    ''' </summary>
    Public Interface ITileWideBlockAndText01
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock As INotificationContentText
    End Interface

    ''' <summary>
    ''' A square tile template that displays three text fields.
    ''' </summary>
    Public Interface ITileWideBlockAndText02
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText

        ''' <summary>
        ''' A large block text field.
        ''' </summary>
        ReadOnly Property TextBlock As INotificationContentText

        ''' <summary>
        ''' The description under the large block text field.
        ''' </summary>
        ReadOnly Property TextSubBlock As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image.
    ''' </summary>
    Public Interface ITileWideImage
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text caption.
    ''' </summary>
    Public Interface ITileWideImageAndText01
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A caption for the image.
        ''' </summary>
        ReadOnly Property TextCaptionWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and two text captions.
    ''' </summary>
    Public Interface ITileWideImageAndText02
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' The first caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption1 As INotificationContentText

        ''' <summary>
        ''' The second caption for the image.
        ''' </summary>
        ReadOnly Property TextCaption2 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid.
    ''' </summary>
    Public Interface ITileWideImageCollection
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn1Row1 As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn2Row1 As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn1Row2 As INotificationContentImage

        ''' <summary>
        ''' A small square image on the tile.
        ''' </summary>
        ReadOnly Property ImageSmallColumn2Row2 As INotificationContentImage
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' two text fields.
    ''' </summary>
    Public Interface ITileWidePeekImage01
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' five text fields.
    ''' </summary>
    Public Interface ITileWidePeekImage02
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading() As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' a text field.
    ''' </summary>
    Public Interface ITileWidePeekImage03
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' a text field.
    ''' </summary>
    Public Interface ITileWidePeekImage04
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' another image and two text fields.
    ''' </summary>
    Public Interface ITileWidePeekImage05
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain As INotificationContentImage

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image, then transitions to show
    ''' another image and a text field.
    ''' </summary>
    Public Interface ITileWidePeekImage06
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property ImageMain As INotificationContentImage

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a portion of a text field,
    ''' then transitions to show all of the text field.
    ''' </summary>
    Public Interface ITileWidePeekImageAndText01
        Inherits IWideTileNotificationContent
        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text field,
    ''' then transitions to show the text field and four other text fields.
    ''' </summary>
    Public Interface ITileWidePeekImageAndText02
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show two
    ''' text fields.
    ''' </summary>
    Public Interface ITileWidePeekImageCollection01
        Inherits ITileWideImageCollection

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show five
    ''' text fields.
    ''' </summary>
    Public Interface ITileWidePeekImageCollection02
        Inherits ITileWideImageCollection

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show a
    ''' text field.
    ''' </summary>
    Public Interface ITileWidePeekImageCollection03
        Inherits ITileWideImageCollection

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show a
    ''' text field.
    ''' </summary>
    Public Interface ITileWidePeekImageCollection04
        Inherits ITileWideImageCollection

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show an image
    ''' and two text fields.
    ''' </summary>
    Public Interface ITileWidePeekImageCollection05
        Inherits ITileWideImageCollection

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five images - one main image,
    ''' and four square images in a grid, then transitions to show an image
    ''' and a text field.
    ''' </summary>
    Public Interface ITileWidePeekImageCollection06
        Inherits ITileWideImageCollection

        ''' <summary>
        ''' The secondary image on the tile.
        ''' </summary>
        ReadOnly Property ImageSecondary As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text field.
    ''' </summary>
    Public Interface ITileWideSmallImageAndText01
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and 5 text fields.
    ''' </summary>
    Public Interface ITileWideSmallImageAndText02
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and a text field.
    ''' </summary>
    Public Interface ITileWideSmallImageAndText03
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays an image and two text fields.
    ''' </summary>
    Public Interface ITileWideSmallImageAndText04
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' The main image on the tile.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five text fields.
    ''' </summary>
    Public Interface ITileWideText01
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nine text fields - a heading and two columns
    ''' of four text fields.
    ''' </summary>
    Public Interface ITileWideText02
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays a text field.
    ''' </summary>
    Public Interface ITileWideText03
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays a text field.
    ''' </summary>
    Public Interface ITileWideText04
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays five text fields.
    ''' </summary>
    Public Interface ITileWideText05
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody3 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody4 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody5 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays ten text fields - two columns
    ''' of five text fields.
    ''' </summary>
    Public Interface ITileWideText06
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn1Row5 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nine text fields - a heading and two columns
    ''' of four text fields.
    ''' </summary>
    Public Interface ITileWideText07
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays ten text fields - two columns
    ''' of five text fields.
    ''' </summary>
    Public Interface ITileWideText08
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextShortColumn1Row5 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays two text fields.
    ''' </summary>
    Public Interface ITileWideText09
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays nine text fields - a heading and two columns
    ''' of four text fields.
    ''' </summary>
    Public Interface ITileWideText10
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2() As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A wide tile template that displays ten text fields - two columns
    ''' of five text fields.
    ''' </summary>
    Public Interface ITileWideText11
        Inherits IWideTileNotificationContent

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row1 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row2 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row3 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row4 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextPrefixColumn1Row5 As INotificationContentText

        ''' <summary>
        ''' A text field displayed in a column and row.
        ''' </summary>
        ReadOnly Property TextColumn2Row5 As INotificationContentText
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
        Property Content As ToastAudioContent

        ''' <summary>
        ''' Whether the audio should loop.  If this property is set to true, the toast audio content
        ''' must be a looping sound.
        ''' </summary>
        Property [Loop] As Boolean
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
        Property StrictValidation As Boolean

        ''' <summary>
        ''' The language of the content being displayed.  The language should be specified using the
        ''' abbreviated language code as defined by BCP 47.
        ''' </summary>
        Property Lang As String

        ''' <summary>
        ''' The BaseUri that should be used for image locations.  Relative image locations use this
        ''' field as their base Uri.  The BaseUri must begin with http://, https://, ms-appx:///, or 
        ''' ms-appdata:///local/.
        ''' </summary>
        Property BaseUri As String

        ''' <summary>
        ''' The launch parameter passed into the Windows Store app when the toast is activated.
        ''' </summary>
        Property Launch As String

        ''' <summary>
        ''' The audio that should be played when the toast is displayed.
        ''' </summary>
        ReadOnly Property Audio As IToastAudio

        ''' <summary>
        ''' The length that the toast should be displayed on screen.
        ''' </summary>
        Property Duration As ToastDuration

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
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays an image and two text fields.
    ''' </summary>
    Public Interface IToastImageAndText02
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays an image and two text fields.
    ''' </summary>
    Public Interface IToastImageAndText03
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays an image and three text fields.
    ''' </summary>
    Public Interface IToastImageAndText04
        Inherits IToastNotificationContent

        ''' <summary>
        ''' The main image on the toast.
        ''' </summary>
        ReadOnly Property Image As INotificationContentImage

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays a text fields.
    ''' </summary>
    Public Interface IToastText01
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays two text fields.
    ''' </summary>
    Public Interface IToastText02
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBodyWrap As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays two text fields.
    ''' </summary>
    Public Interface IToastText03
        Inherits IToastNotificationContent

        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeadingWrap As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody As INotificationContentText
    End Interface

    ''' <summary>
    ''' A toast template that displays three text fields.
    ''' </summary>
    Public Interface IToastText04
        Inherits IToastNotificationContent
        ''' <summary>
        ''' A heading text field.
        ''' </summary>
        ReadOnly Property TextHeading As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody1 As INotificationContentText

        ''' <summary>
        ''' A body text field.
        ''' </summary>
        ReadOnly Property TextBody2 As INotificationContentText
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
    End Enum
End Namespace
