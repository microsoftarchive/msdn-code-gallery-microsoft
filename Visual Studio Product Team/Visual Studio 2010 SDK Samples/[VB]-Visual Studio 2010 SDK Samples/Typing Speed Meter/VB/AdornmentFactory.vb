Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Utilities
Imports Microsoft.VisualStudio.Text.Tagging
Imports Microsoft.VisualStudio.Editor

Namespace TypingSpeed

    ''' <summary>
    ''' Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    ''' that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    ''' </summary>
    <Export(GetType(IWpfTextViewCreationListener)),
    ContentType("text"),
    TextViewRole(PredefinedTextViewRoles.Document)>
    Friend NotInheritable Class AdornmentFactory
        Implements IWpfTextViewCreationListener

        ''' <summary>
        ''' Defines the adornment layer for the scarlet adornment. This layer is ordered 
        ''' after the selection layer in the Z-order
        ''' </summary>
        <Export(GetType(AdornmentLayerDefinition)),
        Name("TypingSpeed"),
        Order(After:=PredefinedAdornmentLayers.Caret),
        TextViewRole(PredefinedTextViewRoles.Document)>
        Public editorAdornmentLayer As AdornmentLayerDefinition = Nothing

        ''' <summary>
        ''' Creates a PurpleCornerBox adornment manager when a textview is created
        ''' </summary>
        ''' <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed</param>
        Public Sub TextViewCreated(ByVal textView As IWpfTextView) Implements IWpfTextViewCreationListener.TextViewCreated
            textView.Properties.GetOrCreateSingletonProperty(Of TypingSpeedMeter)(Function() New TypingSpeedMeter(textView))
        End Sub

    End Class
End Namespace
