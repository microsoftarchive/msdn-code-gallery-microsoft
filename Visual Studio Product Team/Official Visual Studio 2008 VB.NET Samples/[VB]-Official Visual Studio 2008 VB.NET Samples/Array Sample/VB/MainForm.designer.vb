Partial Public Class MainForm
    Inherits System.Windows.Forms.Form



    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
     Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub





    Friend WithEvents lblNumElements As System.Windows.Forms.Label
    Friend WithEvents lblSearchFor As System.Windows.Forms.Label
    Friend WithEvents txtBSearchFor As System.Windows.Forms.TextBox
    Friend WithEvents lblDataAfter As System.Windows.Forms.Label
    Friend WithEvents lblDataAsLoaded As System.Windows.Forms.Label
    Friend WithEvents grpCompareField As System.Windows.Forms.GroupBox
    Friend WithEvents optName As System.Windows.Forms.RadioButton
    Friend WithEvents optId As System.Windows.Forms.RadioButton
    Friend WithEvents lstAfter As System.Windows.Forms.ListBox
    Friend WithEvents lstArrayData As System.Windows.Forms.ListBox
    Friend WithEvents grpArrayType As System.Windows.Forms.GroupBox
    Friend WithEvents optObjects As System.Windows.Forms.RadioButton
    Friend WithEvents optValues As System.Windows.Forms.RadioButton
    Friend WithEvents cmdCreateMatrix As System.Windows.Forms.Button
    Friend WithEvents cmdBinarySearch As System.Windows.Forms.Button
    Friend WithEvents cmdReverse As System.Windows.Forms.Button
    Friend WithEvents cmdSort As System.Windows.Forms.Button
    Friend WithEvents cmdCreateDynamic As System.Windows.Forms.Button
    Friend WithEvents cmdCreateStatic As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents numberElements As System.Windows.Forms.NumericUpDown



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.lblNumElements = New System.Windows.Forms.Label
        Me.lblSearchFor = New System.Windows.Forms.Label
        Me.txtBSearchFor = New System.Windows.Forms.TextBox
        Me.lblDataAfter = New System.Windows.Forms.Label
        Me.lblDataAsLoaded = New System.Windows.Forms.Label
        Me.grpCompareField = New System.Windows.Forms.GroupBox
        Me.optName = New System.Windows.Forms.RadioButton
        Me.optId = New System.Windows.Forms.RadioButton
        Me.lstAfter = New System.Windows.Forms.ListBox
        Me.lstArrayData = New System.Windows.Forms.ListBox
        Me.grpArrayType = New System.Windows.Forms.GroupBox
        Me.optObjects = New System.Windows.Forms.RadioButton
        Me.optValues = New System.Windows.Forms.RadioButton
        Me.cmdCreateMatrix = New System.Windows.Forms.Button
        Me.cmdBinarySearch = New System.Windows.Forms.Button
        Me.cmdReverse = New System.Windows.Forms.Button
        Me.cmdSort = New System.Windows.Forms.Button
        Me.cmdCreateDynamic = New System.Windows.Forms.Button
        Me.cmdCreateStatic = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.numberElements = New System.Windows.Forms.NumericUpDown
        Me.grpCompareField.SuspendLayout()
        Me.grpArrayType.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.numberElements, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblNumElements
        '
        Me.lblNumElements.Location = New System.Drawing.Point(14, 268)
        Me.lblNumElements.Name = "lblNumElements"
        Me.lblNumElements.Size = New System.Drawing.Size(96, 32)
        Me.lblNumElements.TabIndex = 23
        Me.lblNumElements.Text = "# of &Elements for Dynamic Array"
        '
        'lblSearchFor
        '
        Me.lblSearchFor.Location = New System.Drawing.Point(14, 180)
        Me.lblSearchFor.Margin = New System.Windows.Forms.Padding(3, 2, 3, 1)
        Me.lblSearchFor.Name = "lblSearchFor"
        Me.lblSearchFor.Size = New System.Drawing.Size(144, 23)
        Me.lblSearchFor.TabIndex = 20
        Me.lblSearchFor.Text = "Search F&or:"
        '
        'txtBSearchFor
        '
        Me.txtBSearchFor.Location = New System.Drawing.Point(14, 204)
        Me.txtBSearchFor.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.txtBSearchFor.Name = "txtBSearchFor"
        Me.txtBSearchFor.Size = New System.Drawing.Size(144, 20)
        Me.txtBSearchFor.TabIndex = 21
        '
        'lblDataAfter
        '
        Me.lblDataAfter.Location = New System.Drawing.Point(166, 268)
        Me.lblDataAfter.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.lblDataAfter.Name = "lblDataAfter"
        Me.lblDataAfter.Size = New System.Drawing.Size(264, 23)
        Me.lblDataAfter.TabIndex = 30
        Me.lblDataAfter.Text = "No Data Displayed"
        '
        'lblDataAsLoaded
        '
        Me.lblDataAsLoaded.Location = New System.Drawing.Point(166, 140)
        Me.lblDataAsLoaded.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.lblDataAsLoaded.Name = "lblDataAsLoaded"
        Me.lblDataAsLoaded.Size = New System.Drawing.Size(264, 23)
        Me.lblDataAsLoaded.TabIndex = 28
        Me.lblDataAsLoaded.Text = "Data As Loaded"
        '
        'grpCompareField
        '
        Me.grpCompareField.Controls.Add(Me.optName)
        Me.grpCompareField.Controls.Add(Me.optId)
        Me.grpCompareField.Location = New System.Drawing.Point(294, 52)
        Me.grpCompareField.Name = "grpCompareField"
        Me.grpCompareField.Size = New System.Drawing.Size(136, 80)
        Me.grpCompareField.TabIndex = 27
        Me.grpCompareField.TabStop = False
        Me.grpCompareField.Text = "F&ield to use for Sorts"
        '
        'optName
        '
        Me.optName.Location = New System.Drawing.Point(8, 48)
        Me.optName.Name = "optName"
        Me.optName.Size = New System.Drawing.Size(120, 24)
        Me.optName.TabIndex = 1
        Me.optName.Text = "Customer Name"
        '
        'optId
        '
        Me.optId.Location = New System.Drawing.Point(8, 24)
        Me.optId.Name = "optId"
        Me.optId.Size = New System.Drawing.Size(110, 24)
        Me.optId.TabIndex = 0
        Me.optId.Text = "Customer Id"
        '
        'lstAfter
        '
        Me.lstAfter.FormattingEnabled = True
        Me.lstAfter.Location = New System.Drawing.Point(166, 292)
        Me.lstAfter.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.lstAfter.Name = "lstAfter"
        Me.lstAfter.Size = New System.Drawing.Size(264, 108)
        Me.lstAfter.TabIndex = 31
        '
        'lstArrayData
        '
        Me.lstArrayData.FormattingEnabled = True
        Me.lstArrayData.Location = New System.Drawing.Point(166, 164)
        Me.lstArrayData.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.lstArrayData.Name = "lstArrayData"
        Me.lstArrayData.Size = New System.Drawing.Size(264, 95)
        Me.lstArrayData.TabIndex = 29
        '
        'grpArrayType
        '
        Me.grpArrayType.Controls.Add(Me.optObjects)
        Me.grpArrayType.Controls.Add(Me.optValues)
        Me.grpArrayType.Location = New System.Drawing.Point(166, 52)
        Me.grpArrayType.Name = "grpArrayType"
        Me.grpArrayType.Size = New System.Drawing.Size(120, 80)
        Me.grpArrayType.TabIndex = 26
        Me.grpArrayType.TabStop = False
        Me.grpArrayType.Text = "&Array of . . ."
        '
        'optObjects
        '
        Me.optObjects.Location = New System.Drawing.Point(8, 48)
        Me.optObjects.Name = "optObjects"
        Me.optObjects.Size = New System.Drawing.Size(75, 24)
        Me.optObjects.TabIndex = 1
        Me.optObjects.Text = "Objects"
        '
        'optValues
        '
        Me.optValues.Location = New System.Drawing.Point(8, 24)
        Me.optValues.Name = "optValues"
        Me.optValues.Size = New System.Drawing.Size(75, 24)
        Me.optValues.TabIndex = 0
        Me.optValues.Text = "Integers"
        '
        'cmdCreateMatrix
        '
        Me.cmdCreateMatrix.Location = New System.Drawing.Point(14, 300)
        Me.cmdCreateMatrix.Name = "cmdCreateMatrix"
        Me.cmdCreateMatrix.Size = New System.Drawing.Size(144, 23)
        Me.cmdCreateMatrix.TabIndex = 25
        Me.cmdCreateMatrix.Text = "Create a &Matrix Array"
        '
        'cmdBinarySearch
        '
        Me.cmdBinarySearch.Location = New System.Drawing.Point(14, 154)
        Me.cmdBinarySearch.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.cmdBinarySearch.Name = "cmdBinarySearch"
        Me.cmdBinarySearch.Size = New System.Drawing.Size(144, 23)
        Me.cmdBinarySearch.TabIndex = 19
        Me.cmdBinarySearch.Text = "&Binary Search"
        '
        'cmdReverse
        '
        Me.cmdReverse.Location = New System.Drawing.Point(14, 122)
        Me.cmdReverse.Name = "cmdReverse"
        Me.cmdReverse.Size = New System.Drawing.Size(144, 23)
        Me.cmdReverse.TabIndex = 18
        Me.cmdReverse.Text = "&Reverse"
        '
        'cmdSort
        '
        Me.cmdSort.Location = New System.Drawing.Point(14, 90)
        Me.cmdSort.Name = "cmdSort"
        Me.cmdSort.Size = New System.Drawing.Size(144, 23)
        Me.cmdSort.TabIndex = 17
        Me.cmdSort.Text = "&Sort"
        '
        'cmdCreateDynamic
        '
        Me.cmdCreateDynamic.Location = New System.Drawing.Point(14, 236)
        Me.cmdCreateDynamic.Name = "cmdCreateDynamic"
        Me.cmdCreateDynamic.Size = New System.Drawing.Size(144, 23)
        Me.cmdCreateDynamic.TabIndex = 22
        Me.cmdCreateDynamic.Text = "Create &Dynamic Array"
        '
        'cmdCreateStatic
        '
        Me.cmdCreateStatic.Location = New System.Drawing.Point(13, 58)
        Me.cmdCreateStatic.Name = "cmdCreateStatic"
        Me.cmdCreateStatic.Size = New System.Drawing.Size(144, 23)
        Me.cmdCreateStatic.TabIndex = 16
        Me.cmdCreateStatic.Text = "&Create Static Array"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(446, 24)
        Me.MenuStrip1.TabIndex = 32
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DoubleClickEnabled = True
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.DoubleClickEnabled = True
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'numberElements
        '
        Me.numberElements.Location = New System.Drawing.Point(106, 267)
        Me.numberElements.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numberElements.Name = "numberElements"
        Me.numberElements.Size = New System.Drawing.Size(52, 20)
        Me.numberElements.TabIndex = 24
        Me.numberElements.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(446, 419)
        Me.Controls.Add(Me.numberElements)
        Me.Controls.Add(Me.cmdBinarySearch)
        Me.Controls.Add(Me.cmdCreateStatic)
        Me.Controls.Add(Me.cmdSort)
        Me.Controls.Add(Me.cmdReverse)
        Me.Controls.Add(Me.cmdCreateDynamic)
        Me.Controls.Add(Me.cmdCreateMatrix)
        Me.Controls.Add(Me.grpArrayType)
        Me.Controls.Add(Me.lstArrayData)
        Me.Controls.Add(Me.lblNumElements)
        Me.Controls.Add(Me.grpCompareField)
        Me.Controls.Add(Me.lblDataAsLoaded)
        Me.Controls.Add(Me.lblDataAfter)
        Me.Controls.Add(Me.txtBSearchFor)
        Me.Controls.Add(Me.lblSearchFor)
        Me.Controls.Add(Me.lstAfter)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "MainForm"
        Me.Text = "Working with Arrays"
        Me.grpCompareField.ResumeLayout(False)
        Me.grpArrayType.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.numberElements, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend Shared ReadOnly Property GetInstance() As MainForm
        Get
            If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                SyncLock m_SyncObject
                    If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                        m_DefaultInstance = New MainForm
                    End If
                End SyncLock
            End If
            Return m_DefaultInstance
        End Get
    End Property

    Private Shared m_DefaultInstance As MainForm
    Private Shared m_SyncObject As New Object


End Class
