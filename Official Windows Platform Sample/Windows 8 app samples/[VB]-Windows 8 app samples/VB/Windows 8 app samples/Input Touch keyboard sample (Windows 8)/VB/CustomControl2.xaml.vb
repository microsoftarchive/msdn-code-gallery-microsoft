' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.UI.Xaml.Automation ' needed for enum SupportedTextSelection in ITextProvider
Imports Windows.UI.Xaml.Automation.Text ' needed for TextPatternRangePoint
Imports Windows.UI.Xaml.Automation.Peers ' needed for FrameworkElementAutomationPeer class
Imports Windows.UI.Xaml.Automation.Provider ' needed for ITextProvider and IValueProvider
' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

''' <summary>
''' A sample custom control displays the date/time when a key is pressed.  This control is for 
''' illustrative purposes only and does not contain a full implementation of a text control, and it
''' does not process any text that is entered.
''' The control turns to yellow when it has input focus, otherwise it is gray.  Lost focus will also 
''' clear the contents that is in the control.
''' </summary>
Partial Public NotInheritable Class CustomInputBox2
    Inherits UserControl
    Friend Property ContentText() As String
        Get
            Return m_ContentText
        End Get
        Set(value As String)
            m_ContentText = value
        End Set
    End Property
    Private m_ContentText As String

    ''' <summary>
    ''' Loads the XAML UI contents and set properties required for this custom control.
    ''' </summary>
    Public Sub New()
        Me.InitializeComponent()
        Me.IsTabStop = True
        Me.IsTapEnabled = True
        Me.ContentText = ""
    End Sub

    ''' <summary>
    ''' Create the Automation peer implementations for custom control (CustomInputBox2) to provide the accessibility support.
    ''' </summary>
    ''' <returns>Automation Peer implementation for this control</returns>
    Protected Overrides Function OnCreateAutomationPeer() As AutomationPeer
        Return New CustomControl2AutomationPeer(Me)
    End Function

    ''' <summary>
    ''' Override the default event handler for GotFocus.
    ''' When the control got focus, indicate it has focus by highlighting the control by changing the background color to yellow.
    ''' </summary>
    ''' <param name="e">State information and event data associated with GotFocus event.</param>
    Protected Overrides Sub OnGotFocus(e As RoutedEventArgs)
        Me.myBorder.Background = New SolidColorBrush(Windows.UI.Colors.Yellow)
    End Sub

    ''' <summary>
    ''' Override the default event handler for LostFocus.
    ''' When the control lost focus, indicate it does not have focus by changing the background color to gray.
    ''' And the content is cleared.
    ''' </summary>
    ''' <param name="e">State information and event data associated with LostFocus event.</param>
    Protected Overrides Sub OnLostFocus(e As RoutedEventArgs)
        Me.myBorder.Background = New SolidColorBrush(Windows.UI.Colors.Gray)
        ContentText = ""
        Me.myTextBlock.Text = ContentText
    End Sub

    ''' <summary>
    ''' Override the default event handler for Tapped.
    ''' Set input focus to the control when tapped on.
    ''' </summary>
    ''' <param name="e">State information and event data associated with Tapped event.</param>
    Protected Overrides Sub OnTapped(e As TappedRoutedEventArgs)
        Me.Focus(Windows.UI.Xaml.FocusState.Pointer)
    End Sub

    ''' <summary>
    ''' Override the default event handler for KeyDown.  
    ''' Displays the text "A key is pressed" and the approximate time when the key is pressed.
    ''' </summary>
    ''' <param name="e">State information and event data associated with KeyDown event.</param>
    Protected Overrides Sub OnKeyDown(e As KeyRoutedEventArgs)
        ContentText = "A key was pressed @ " & DateTime.Now.ToString
        Me.myTextBlock.Text = ContentText
    End Sub
End Class

''' <summary>
''' Automation Peer class for CustomInputBox2.  
''' 
''' Note: The difference between this and CustomControl1AutomationPeer is that this one implements
''' Text Pattern (ITextProvider) and Value Pattern (IValuePattern) interfaces.  So Touch keyboard shows 
''' automatically when user taps on the control with Touch or Pen.
''' </summary>
Public Class CustomControl2AutomationPeer
    Inherits FrameworkElementAutomationPeer
    Implements ITextProvider
    Implements IValueProvider
    Private customInputBox As CustomInputBox2
    Private accClass As String = "CustomInputBoxClass2"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="owner"></param>
    Public Sub New(owner As CustomInputBox2)
        MyBase.New(owner)
        Me.customInputBox = owner
    End Sub

    ''' <summary>
    ''' Override GetPatternCore to return the object that supports the specified pattern.  In this case the Value pattern, Text
    ''' patter and any base class patterns.
    ''' </summary>
    ''' <param name="patternInterface"></param>
    ''' <returns>the object that supports the specified pattern</returns>
    Protected Overrides Function GetPatternCore(patternInterface__1 As PatternInterface) As Object
        If patternInterface__1 = PatternInterface.Value Then
            Return Me
        ElseIf patternInterface__1 = PatternInterface.Text Then
            Return Me
        End If
        Return MyBase.GetPatternCore(patternInterface__1)
    End Function

    ''' <summary>
    ''' Override GetClassNameCore and set the name of the class that defines the type associated with this control.
    ''' </summary>
    ''' <returns>The name of the control class</returns>
    Protected Overrides Function GetClassNameCore() As String
        Return Me.accClass
    End Function

#Region "Implementation for ITextPattern interface"
    ' Complete implementation of the ITextPattern is beyond the scope of this sample.  The implementation provided
    ' is specific to this sample's custom control, so it is unlikely that they are directly transferable to other 
    ' custom control.

    Private ReadOnly Property DocumentRange() As ITextRangeProvider Implements ITextProvider.DocumentRange
        ' A real implementation of this method is beyond the scope of this sample.
        ' If your custom control has complex text involving both readonly and non-readonly ranges, 
        ' it will need a smarter implementation than just returning a fixed range
        Get
            Return New CustomControl2RangeProvider(customInputBox.ContentText, Me)
        End Get
    End Property

    Private Function GetSelection() As ITextRangeProvider() Implements ITextProvider.GetSelection
        Return New ITextRangeProvider(-1) {}
    End Function

    Private Function GetVisibleRanges() As ITextRangeProvider() Implements ITextProvider.GetVisibleRanges
        Dim ret As ITextRangeProvider() = New ITextRangeProvider(0) {}
        ret(0) = New CustomControl2RangeProvider(customInputBox.ContentText, Me)
        Return ret
    End Function

    Private Function RangeFromChild(childElement As IRawElementProviderSimple) As ITextRangeProvider Implements ITextProvider.RangeFromChild
        Return New CustomControl2RangeProvider(customInputBox.ContentText, Me)
    End Function

    Private Function RangeFromPoint(screenLocation As Point) As ITextRangeProvider Implements ITextProvider.RangeFromPoint
        Return New CustomControl2RangeProvider(customInputBox.ContentText, Me)
    End Function

    Private ReadOnly Property SupportedTextSelection() As SupportedTextSelection Implements ITextProvider.SupportedTextSelection
        Get
            Return SupportedTextSelection.[Single]
        End Get
    End Property

#End Region

#Region "Implementation for IValueProvider interface"
    ' Complete implementation of the IValueProvider is beyond the scope of this sample.  The implementation provided
    ' is specific to this sample's custom control, so it is unlikely that they are directly transferable to other 
    ' custom control.

    ''' <summary>
    ''' The value needs to be false for the Touch keyboard to be launched automatically because Touch keyboard
    ''' does not appear when the input focus is in a readonly UI control.
    ''' </summary>
    Private ReadOnly Property IsReadOnly() As Boolean Implements IValueProvider.IsReadOnly
        Get
            Return False
        End Get
    End Property

    Private Shadows Sub SetValue(value As String) Implements IValueProvider.SetValue
        customInputBox.ContentText = value
    End Sub

    Private ReadOnly Property Value() As String Implements IValueProvider.Value
        Get
            Return customInputBox.ContentText
        End Get
    End Property

#End Region

    Public Function GetRawElementProviderSimple() As IRawElementProviderSimple
        Return ProviderFromPeer(Me)
    End Function
End Class

''' <summary>
''' A minimal implementation of ITextRangeProvider, used by CustomControl2AutomationPeer
''' A real implementation is beyond the scope of this sample
''' </summary>
Public NotInheritable Class CustomControl2RangeProvider
    Implements ITextRangeProvider

    Private _text As String
    Private _peer As CustomControl2AutomationPeer

    Public Sub New(text As String, peer As CustomControl2AutomationPeer)
        _text = text
        _peer = peer
    End Sub

    Public Sub AddToSelection() Implements ITextRangeProvider.AddToSelection

    End Sub

    Public Function Clone() As ITextRangeProvider Implements ITextRangeProvider.Clone
        Return New CustomControl2RangeProvider(_text, _peer)
    End Function

    Public Function Compare(other As ITextRangeProvider) As Boolean Implements ITextRangeProvider.Compare
        Return True
    End Function

    Public Function CompareEndpoints(endpoint As TextPatternRangeEndpoint, targetRange As ITextRangeProvider, targetEndpoint As TextPatternRangeEndpoint) As Integer Implements ITextRangeProvider.CompareEndpoints
        Return 0
    End Function

    Public Sub ExpandToEnclosingUnit(unit As TextUnit) Implements ITextRangeProvider.ExpandToEnclosingUnit

    End Sub

    Public Function FindAttribute(attribute As Integer, value As [Object], backward As Boolean) As ITextRangeProvider Implements ITextRangeProvider.FindAttribute
        Return Me
    End Function

    Public Function FindText(text As String, backward As Boolean, ignoreCase As Boolean) As ITextRangeProvider Implements ITextRangeProvider.FindText
        Return Me
    End Function

    Public Function GetAttributeValue(attribute As Integer) As [Object] Implements ITextRangeProvider.GetAttributeValue
        Return Me
    End Function


    Public Function GetChildren() As IRawElementProviderSimple() Implements ITextRangeProvider.GetChildren
        Return New IRawElementProviderSimple(-1) {}
    End Function

    Public Function GetEnclosingElement() As IRawElementProviderSimple Implements ITextRangeProvider.GetEnclosingElement
        Return _peer.GetRawElementProviderSimple()
    End Function

    Public Function GetText(maxLength As Integer) As String Implements ITextRangeProvider.GetText
        Return If((maxLength < 0), _text, _text.Substring(0, Math.Min(_text.Length, maxLength)))
    End Function

    Public Function Move(unit As TextUnit, count As Integer) As Integer Implements ITextRangeProvider.Move
        Return 0
    End Function

    Public Sub MoveEndpointByRange(endpoint As TextPatternRangeEndpoint, targetRange As ITextRangeProvider, targetEndpoint As TextPatternRangeEndpoint) Implements ITextRangeProvider.MoveEndpointByRange

    End Sub

    Public Function MoveEndpointByUnit(endpoint As TextPatternRangeEndpoint, unit As TextUnit, count As Integer) As Integer Implements ITextRangeProvider.MoveEndpointByUnit
        Return 0
    End Function

    Public Sub RemoveFromSelection() Implements ITextRangeProvider.RemoveFromSelection

    End Sub

    Public Sub ScrollIntoView(alignToTop As Boolean) Implements ITextRangeProvider.ScrollIntoView

    End Sub

    Public Sub [Select]() Implements ITextRangeProvider.Select

    End Sub

    Public Sub GetBoundingRectangles(ByRef returnValue() As Double) Implements ITextRangeProvider.GetBoundingRectangles
        returnValue = New Double(-1) {}
    End Sub
End Class


