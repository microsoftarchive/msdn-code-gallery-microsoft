Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports PowerPoint = Microsoft.Office.Interop.PowerPoint
Imports Microsoft.Office.Tools.Ribbon
Imports ExportVisPPT = CodeSample_ExportVisio

<Runtime.InteropServices.ComVisible(True)> _
Public Class Ribbon1
    Implements Office.IRibbonExtensibility

    Private ribbon As Office.IRibbonUI
    Public errMsg As Boolean
    Private vsoApp As Visio.Application
    Private myAddIn As ThisAddIn

    Public Sub New()
    End Sub

    Public Function GetCustomUI(ByVal ribbonID As String) As String Implements Office.IRibbonExtensibility.GetCustomUI
        Return GetResourceText("CodeSample_ExportVisio.Ribbon1.xml")
    End Function

    Public Function GetImages(ByVal control As Office.IRibbonControl)
        ' Determine which image to load into the ribbon.
        Select Case control.Id
            Case "btnExportPage"
                Return My.Resources.ExportVisioPage_32x32

            Case "btnExportDiagram"
                Return My.Resources.ExportVisioDiagram_32x32

            Case Else
                Return My.Resources.ExportVisioDiagram_32x32

        End Select
    End Function

#Region "Ribbon Callbacks"

    Public Sub Ribbon_Load(ByVal ribbonUI As Office.IRibbonUI)
        Me.ribbon = ribbonUI
    End Sub

    Public Sub ExportPage(ByVal control As Global.Microsoft.Office.Core.IRibbonControl)

        vsoApp = Globals.ThisAddIn.Application
        myAddIn = ExportVisPPT.Globals.ThisAddIn

        ' Check for conditions affecting conversion or application errors.
        errMsg = vbFalse
        If Not (Validate(False)) Then
            Exit Sub
        End If

        'Export shapes on the active Visio page to PowerPoint
        myAddIn.ExportPage()

    End Sub

    Public Sub ExportDiagram(ByVal control As Global.Microsoft.Office.Core.IRibbonControl)

        vsoApp = Globals.ThisAddIn.Application
        myAddIn = ExportVisPPT.Globals.ThisAddIn

        ' Check for conditions affecting conversion or application errors.
        errMsg = vbFalse
        If Not (Validate(True)) Then
            Exit Sub
        End If

        ' Export the shapes on each page in the drawing to a PowerPoint slide.
        myAddIn.ExportAllPages()

    End Sub
#End Region

#Region "Helpers"

    Private Function Validate(ByVal checkAllPages As Boolean)

        Dim IsValid As Boolean = vbTrue

        ' Check to see whether there are any documents currently open.
        If (vsoApp.Documents.Count < 1) Then
            IsValid = vbFalse
            Return IsValid
        End If

        Dim vsoDoc As Visio.Document = vsoApp.ActiveDocument
        Dim vsoPages As Visio.Pages = vsoDoc.Pages
        Dim vsoPage As Visio.Page
        Dim count As Integer
        Dim num As Integer

        ' Determine how many of the pages in the Visio drawing
        ' to check the dimentions of.
        If checkAllPages Then
            num = 1
            count = vsoPages.Count
        Else
            num = vsoApp.ActivePage.Index
            count = vsoApp.ActivePage.Index
        End If

        For x As Integer = num To count
            vsoPage = vsoPages(x)

            ' Check Visio page dimensions against default PowerPoint layout.
            ' Normal page dimensions in Visio = 8.5" x 11" (landscape). 
            ' Normal slide dimensions in PowerPoint = 7.5" x 10" (landscape).
            If ((vsoPage.PageSheet.Cells("PageHeight").Result(vbSingle) > 7.5) Or _
                (vsoPage.PageSheet.Cells("PageWidth").Result(vbSingle) > 10)) And _
               Not errMsg Then

                Dim userInput As DialogResult = MessageBox.Show("Some of the pages are larger than a default PowerPoint slide." & vbCr & _
                                                                "Shapes may appear off the edge of the resulting slide." & vbCr & vbCr & _
                                                                "Continue?", "Export Visio to PowerPoint", MessageBoxButtons.OKCancel)
                errMsg = vbTrue
                If (userInput = Windows.Forms.DialogResult.Cancel) Then
                    IsValid = vbFalse
                End If

            End If
        Next x
        Return IsValid
    End Function

    Private Shared Function GetResourceText(ByVal resourceName As String) As String
        ' This method reads the data from the resource named in the resourceName parameter.
        Dim asm As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
        Dim resourceNames() As String = asm.GetManifestResourceNames()
        For i As Integer = 0 To resourceNames.Length - 1
            If String.Compare(resourceName, resourceNames(i), StringComparison.OrdinalIgnoreCase) = 0 Then
                Using resourceReader As IO.StreamReader = New IO.StreamReader(asm.GetManifestResourceStream(resourceNames(i)))
                    If resourceReader IsNot Nothing Then
                        Return resourceReader.ReadToEnd()
                    End If
                End Using
            End If
        Next
        Return Nothing
    End Function

#End Region

End Class
