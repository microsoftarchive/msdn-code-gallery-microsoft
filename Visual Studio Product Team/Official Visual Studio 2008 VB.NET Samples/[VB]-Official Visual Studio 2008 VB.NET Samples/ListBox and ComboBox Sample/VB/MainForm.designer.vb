<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim IdLabel As System.Windows.Forms.Label
        Dim NameLabel As System.Windows.Forms.Label
        Dim Label1 As System.Windows.Forms.Label
        Dim Label5 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Dim Label6 As System.Windows.Forms.Label
        Me.tabOptions = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.lblFileName1 = New System.Windows.Forms.Label
        Me.btnFill1 = New System.Windows.Forms.Button
        Me.lstProcessesAddItem = New System.Windows.Forms.ListBox
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.lblFileInfo = New System.Windows.Forms.Label
        Me.btnFill2 = New System.Windows.Forms.Button
        Me.lstFiles = New System.Windows.Forms.ListBox
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.lstSelected = New System.Windows.Forms.ListBox
        Me.lstSelectedItems = New System.Windows.Forms.ListBox
        Me.cboSelectionMode = New System.Windows.Forms.ComboBox
        Me.btnFill3 = New System.Windows.Forms.Button
        Me.lstMultiSelect = New System.Windows.Forms.ListBox
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.lblFileName2 = New System.Windows.Forms.Label
        Me.btnFill4 = New System.Windows.Forms.Button
        Me.lstProcessesDataSource = New System.Windows.Forms.ListBox
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.lblResults = New System.Windows.Forms.Label
        Me.nudDropDownWidth = New System.Windows.Forms.NumericUpDown
        Me.nudDropDownItems = New System.Windows.Forms.NumericUpDown
        Me.cboDropDownStyle = New System.Windows.Forms.ComboBox
        Me.btnFill5 = New System.Windows.Forms.Button
        Me.cboDemo = New System.Windows.Forms.ComboBox
        Me.TabPage6 = New System.Windows.Forms.TabPage
        Me.CustomerBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
        Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel
        Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator
        Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox
        Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton
        Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.IdTextBox = New System.Windows.Forms.TextBox
        Me.CustomerBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.NameTextBox = New System.Windows.Forms.TextBox
        Me.BindingSourceFillButton = New System.Windows.Forms.Button
        Me.CustomerComboBox = New System.Windows.Forms.ComboBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EmployeesTableAdapter = New ListBoxComboBox.NorthwindDataSetTableAdapters.EmployeesTableAdapter
        Me.NorthwindDataSet = New ListBoxComboBox.NorthwindDataSet
        IdLabel = New System.Windows.Forms.Label
        NameLabel = New System.Windows.Forms.Label
        Label1 = New System.Windows.Forms.Label
        Label5 = New System.Windows.Forms.Label
        Label4 = New System.Windows.Forms.Label
        Label3 = New System.Windows.Forms.Label
        Label2 = New System.Windows.Forms.Label
        Label6 = New System.Windows.Forms.Label
        Me.tabOptions.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        CType(Me.nudDropDownWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudDropDownItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage6.SuspendLayout()
        CType(Me.CustomerBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CustomerBindingNavigator.SuspendLayout()
        CType(Me.CustomerBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.NorthwindDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'IdLabel
        '
        IdLabel.AutoSize = True
        IdLabel.Location = New System.Drawing.Point(316, 110)
        IdLabel.Name = "IdLabel"
        IdLabel.Size = New System.Drawing.Size(19, 13)
        IdLabel.TabIndex = 7
        IdLabel.Text = "Id:"
        '
        'NameLabel
        '
        NameLabel.AutoSize = True
        NameLabel.Location = New System.Drawing.Point(297, 137)
        NameLabel.Name = "NameLabel"
        NameLabel.Size = New System.Drawing.Size(38, 13)
        NameLabel.TabIndex = 9
        NameLabel.Text = "Name:"
        '
        'tabOptions
        '
        Me.tabOptions.Controls.Add(Me.TabPage1)
        Me.tabOptions.Controls.Add(Me.TabPage2)
        Me.tabOptions.Controls.Add(Me.TabPage3)
        Me.tabOptions.Controls.Add(Me.TabPage4)
        Me.tabOptions.Controls.Add(Me.TabPage5)
        Me.tabOptions.Controls.Add(Me.TabPage6)
        Me.tabOptions.Location = New System.Drawing.Point(14, 40)
        Me.tabOptions.Name = "tabOptions"
        Me.tabOptions.SelectedIndex = 0
        Me.tabOptions.Size = New System.Drawing.Size(617, 298)
        Me.tabOptions.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.lblFileName1)
        Me.TabPage1.Controls.Add(Me.btnFill1)
        Me.TabPage1.Controls.Add(Me.lstProcessesAddItem)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(609, 272)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Add Items"
        Me.TabPage1.UseVisualStyleBackColor = False
        '
        'lblFileName1
        '
        Me.lblFileName1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFileName1.Location = New System.Drawing.Point(16, 221)
        Me.lblFileName1.Name = "lblFileName1"
        Me.lblFileName1.Size = New System.Drawing.Size(572, 35)
        Me.lblFileName1.TabIndex = 2
        '
        'btnFill1
        '
        Me.btnFill1.Location = New System.Drawing.Point(265, 15)
        Me.btnFill1.Name = "btnFill1"
        Me.btnFill1.Size = New System.Drawing.Size(75, 23)
        Me.btnFill1.TabIndex = 1
        Me.btnFill1.Text = "Fill"
        '
        'lstProcessesAddItem
        '
        Me.lstProcessesAddItem.FormattingEnabled = True
        Me.lstProcessesAddItem.Location = New System.Drawing.Point(15, 15)
        Me.lstProcessesAddItem.Name = "lstProcessesAddItem"
        Me.lstProcessesAddItem.Size = New System.Drawing.Size(225, 199)
        Me.lstProcessesAddItem.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.lblFileInfo)
        Me.TabPage2.Controls.Add(Me.btnFill2)
        Me.TabPage2.Controls.Add(Me.lstFiles)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(609, 272)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Bind to Data Table"
        Me.TabPage2.UseVisualStyleBackColor = False
        '
        'lblFileInfo
        '
        Me.lblFileInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFileInfo.Location = New System.Drawing.Point(17, 222)
        Me.lblFileInfo.Name = "lblFileInfo"
        Me.lblFileInfo.Size = New System.Drawing.Size(558, 36)
        Me.lblFileInfo.TabIndex = 2
        '
        'btnFill2
        '
        Me.btnFill2.Location = New System.Drawing.Point(265, 15)
        Me.btnFill2.Name = "btnFill2"
        Me.btnFill2.Size = New System.Drawing.Size(75, 23)
        Me.btnFill2.TabIndex = 1
        Me.btnFill2.Text = "Fill"
        '
        'lstFiles
        '
        Me.lstFiles.FormattingEnabled = True
        Me.lstFiles.Location = New System.Drawing.Point(15, 15)
        Me.lstFiles.Name = "lstFiles"
        Me.lstFiles.Size = New System.Drawing.Size(225, 199)
        Me.lstFiles.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Label6)
        Me.TabPage3.Controls.Add(Me.lstSelected)
        Me.TabPage3.Controls.Add(Me.lstSelectedItems)
        Me.TabPage3.Controls.Add(Me.cboSelectionMode)
        Me.TabPage3.Controls.Add(Label1)
        Me.TabPage3.Controls.Add(Me.btnFill3)
        Me.TabPage3.Controls.Add(Me.lstMultiSelect)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(609, 272)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Selection Mode"
        Me.TabPage3.UseVisualStyleBackColor = False
        '
        'lstSelected
        '
        Me.lstSelected.FormattingEnabled = True
        Me.lstSelected.Location = New System.Drawing.Point(268, 79)
        Me.lstSelected.Name = "lstSelected"
        Me.lstSelected.Size = New System.Drawing.Size(72, 147)
        Me.lstSelected.TabIndex = 3
        '
        'lstSelectedItems
        '
        Me.lstSelectedItems.FormattingEnabled = True
        Me.lstSelectedItems.Location = New System.Drawing.Point(357, 78)
        Me.lstSelectedItems.Name = "lstSelectedItems"
        Me.lstSelectedItems.Size = New System.Drawing.Size(221, 147)
        Me.lstSelectedItems.TabIndex = 4
        '
        'cboSelectionMode
        '
        Me.cboSelectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSelectionMode.FormattingEnabled = True
        Me.cboSelectionMode.Items.AddRange(New Object() {"One", "MultiSimple", "MultiExtended"})
        Me.cboSelectionMode.Location = New System.Drawing.Point(15, 205)
        Me.cboSelectionMode.Name = "cboSelectionMode"
        Me.cboSelectionMode.Size = New System.Drawing.Size(227, 21)
        Me.cboSelectionMode.TabIndex = 5
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(265, 55)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(72, 13)
        Label1.TabIndex = 2
        Label1.Text = "You selected:"
        '
        'btnFill3
        '
        Me.btnFill3.Location = New System.Drawing.Point(265, 15)
        Me.btnFill3.Name = "btnFill3"
        Me.btnFill3.Size = New System.Drawing.Size(75, 23)
        Me.btnFill3.TabIndex = 1
        Me.btnFill3.Text = "Fill"
        '
        'lstMultiSelect
        '
        Me.lstMultiSelect.FormattingEnabled = True
        Me.lstMultiSelect.Location = New System.Drawing.Point(15, 15)
        Me.lstMultiSelect.Name = "lstMultiSelect"
        Me.lstMultiSelect.Size = New System.Drawing.Size(227, 173)
        Me.lstMultiSelect.TabIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.lblFileName2)
        Me.TabPage4.Controls.Add(Me.btnFill4)
        Me.TabPage4.Controls.Add(Me.lstProcessesDataSource)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(609, 272)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Bind to Array"
        Me.TabPage4.UseVisualStyleBackColor = False
        '
        'lblFileName2
        '
        Me.lblFileName2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblFileName2.Location = New System.Drawing.Point(15, 225)
        Me.lblFileName2.Name = "lblFileName2"
        Me.lblFileName2.Size = New System.Drawing.Size(560, 33)
        Me.lblFileName2.TabIndex = 3
        '
        'btnFill4
        '
        Me.btnFill4.Location = New System.Drawing.Point(265, 15)
        Me.btnFill4.Name = "btnFill4"
        Me.btnFill4.Size = New System.Drawing.Size(75, 23)
        Me.btnFill4.TabIndex = 1
        Me.btnFill4.Text = "Fill"
        '
        'lstProcessesDataSource
        '
        Me.lstProcessesDataSource.FormattingEnabled = True
        Me.lstProcessesDataSource.Location = New System.Drawing.Point(15, 15)
        Me.lstProcessesDataSource.Name = "lstProcessesDataSource"
        Me.lstProcessesDataSource.Size = New System.Drawing.Size(225, 199)
        Me.lstProcessesDataSource.TabIndex = 0
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.lblResults)
        Me.TabPage5.Controls.Add(Me.nudDropDownWidth)
        Me.TabPage5.Controls.Add(Me.nudDropDownItems)
        Me.TabPage5.Controls.Add(Me.cboDropDownStyle)
        Me.TabPage5.Controls.Add(Label5)
        Me.TabPage5.Controls.Add(Label4)
        Me.TabPage5.Controls.Add(Label3)
        Me.TabPage5.Controls.Add(Label2)
        Me.TabPage5.Controls.Add(Me.btnFill5)
        Me.TabPage5.Controls.Add(Me.cboDemo)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(609, 272)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "ComboBox"
        Me.TabPage5.UseVisualStyleBackColor = False
        '
        'lblResults
        '
        Me.lblResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblResults.Location = New System.Drawing.Point(440, 162)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(65, 20)
        Me.lblResults.TabIndex = 9
        '
        'nudDropDownWidth
        '
        Me.nudDropDownWidth.Location = New System.Drawing.Point(440, 98)
        Me.nudDropDownWidth.Maximum = New Decimal(New Integer() {400, 0, 0, 0})
        Me.nudDropDownWidth.Minimum = New Decimal(New Integer() {50, 0, 0, 0})
        Me.nudDropDownWidth.Name = "nudDropDownWidth"
        Me.nudDropDownWidth.Size = New System.Drawing.Size(65, 20)
        Me.nudDropDownWidth.TabIndex = 7
        Me.nudDropDownWidth.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'nudDropDownItems
        '
        Me.nudDropDownItems.Location = New System.Drawing.Point(440, 130)
        Me.nudDropDownItems.Maximum = New Decimal(New Integer() {20, 0, 0, 0})
        Me.nudDropDownItems.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.nudDropDownItems.Name = "nudDropDownItems"
        Me.nudDropDownItems.Size = New System.Drawing.Size(65, 20)
        Me.nudDropDownItems.TabIndex = 8
        Me.nudDropDownItems.Value = New Decimal(New Integer() {20, 0, 0, 0})
        '
        'cboDropDownStyle
        '
        Me.cboDropDownStyle.FormattingEnabled = True
        Me.cboDropDownStyle.Location = New System.Drawing.Point(440, 65)
        Me.cboDropDownStyle.Name = "cboDropDownStyle"
        Me.cboDropDownStyle.Size = New System.Drawing.Size(115, 21)
        Me.cboDropDownStyle.TabIndex = 6
        '
        'Label5
        '
        Label5.AutoSize = True
        Label5.Location = New System.Drawing.Point(352, 163)
        Label5.Name = "Label5"
        Label5.Size = New System.Drawing.Size(82, 13)
        Label5.TabIndex = 5
        Label5.Text = "Selected Value:"
        Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(328, 132)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(106, 13)
        Label4.TabIndex = 4
        Label4.Text = "MaxDropDownItems:"
        Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(345, 100)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(89, 13)
        Label3.TabIndex = 3
        Label3.Text = "DropDownWidth:"
        Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(350, 68)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(84, 13)
        Label2.TabIndex = 2
        Label2.Text = "DropDownStyle:"
        Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnFill5
        '
        Me.btnFill5.Location = New System.Drawing.Point(265, 15)
        Me.btnFill5.Name = "btnFill5"
        Me.btnFill5.Size = New System.Drawing.Size(75, 23)
        Me.btnFill5.TabIndex = 1
        Me.btnFill5.Text = "Fill"
        '
        'cboDemo
        '
        Me.cboDemo.FormattingEnabled = True
        Me.cboDemo.Location = New System.Drawing.Point(15, 15)
        Me.cboDemo.Name = "cboDemo"
        Me.cboDemo.Size = New System.Drawing.Size(219, 21)
        Me.cboDemo.TabIndex = 0
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.CustomerBindingNavigator)
        Me.TabPage6.Controls.Add(IdLabel)
        Me.TabPage6.Controls.Add(Me.IdTextBox)
        Me.TabPage6.Controls.Add(NameLabel)
        Me.TabPage6.Controls.Add(Me.NameTextBox)
        Me.TabPage6.Controls.Add(Me.BindingSourceFillButton)
        Me.TabPage6.Controls.Add(Me.CustomerComboBox)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(609, 272)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "BindingSource"
        Me.TabPage6.UseVisualStyleBackColor = False
        '
        'CustomerBindingNavigator
        '
        Me.CustomerBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.CustomerBindingNavigator.BindingSource = Me.CustomerBindingSource
        Me.CustomerBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.CustomerBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.CustomerBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem})
        Me.CustomerBindingNavigator.Location = New System.Drawing.Point(3, 3)
        Me.CustomerBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.CustomerBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.CustomerBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.CustomerBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.CustomerBindingNavigator.Name = "CustomerBindingNavigator"
        Me.CustomerBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.CustomerBindingNavigator.Size = New System.Drawing.Size(603, 25)
        Me.CustomerBindingNavigator.TabIndex = 11
        Me.CustomerBindingNavigator.Text = "BindingNavigator1"
        '
        'BindingNavigatorAddNewItem
        '
        Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
        Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorAddNewItem.Text = "Add new"
        '
        'BindingNavigatorCountItem
        '
        Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
        Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(36, 22)
        Me.BindingNavigatorCountItem.Text = "of {0}"
        Me.BindingNavigatorCountItem.ToolTipText = "Total number of items"
        '
        'BindingNavigatorDeleteItem
        '
        Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
        Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorDeleteItem.Text = "Delete"
        '
        'BindingNavigatorMoveFirstItem
        '
        Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
        Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveFirstItem.Text = "Move first"
        '
        'BindingNavigatorMovePreviousItem
        '
        Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
        Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMovePreviousItem.Text = "Move previous"
        '
        'BindingNavigatorSeparator
        '
        Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
        Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorPositionItem
        '
        Me.BindingNavigatorPositionItem.AccessibleName = "Position"
        Me.BindingNavigatorPositionItem.AutoSize = False
        Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
        Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 21)
        Me.BindingNavigatorPositionItem.Text = "0"
        Me.BindingNavigatorPositionItem.ToolTipText = "Current position"
        '
        'BindingNavigatorSeparator1
        '
        Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
        Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'BindingNavigatorMoveNextItem
        '
        Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
        Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveNextItem.Text = "Move next"
        '
        'BindingNavigatorMoveLastItem
        '
        Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
        Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
        Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
        Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 22)
        Me.BindingNavigatorMoveLastItem.Text = "Move last"
        '
        'BindingNavigatorSeparator2
        '
        Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
        Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'IdTextBox
        '
        Me.IdTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CustomerBindingSource, "Id", True))
        Me.IdTextBox.Location = New System.Drawing.Point(341, 107)
        Me.IdTextBox.Name = "IdTextBox"
        Me.IdTextBox.Size = New System.Drawing.Size(100, 20)
        Me.IdTextBox.TabIndex = 8
        '
        'CustomerBindingSource
        '
        Me.CustomerBindingSource.DataSource = GetType(ListBoxComboBox.Customer)
        '
        'NameTextBox
        '
        Me.NameTextBox.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.CustomerBindingSource, "Name", True))
        Me.NameTextBox.Location = New System.Drawing.Point(341, 134)
        Me.NameTextBox.Name = "NameTextBox"
        Me.NameTextBox.Size = New System.Drawing.Size(100, 20)
        Me.NameTextBox.TabIndex = 10
        '
        'BindingSourceFillButton
        '
        Me.BindingSourceFillButton.Location = New System.Drawing.Point(296, 40)
        Me.BindingSourceFillButton.Name = "BindingSourceFillButton"
        Me.BindingSourceFillButton.Size = New System.Drawing.Size(75, 23)
        Me.BindingSourceFillButton.TabIndex = 5
        Me.BindingSourceFillButton.Text = "Fill"
        '
        'CustomerComboBox
        '
        Me.CustomerComboBox.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.CustomerBindingSource, "Id", True))
        Me.CustomerComboBox.DataSource = Me.CustomerBindingSource
        Me.CustomerComboBox.DisplayMember = "Name"
        Me.CustomerComboBox.FormattingEnabled = True
        Me.CustomerComboBox.Location = New System.Drawing.Point(19, 42)
        Me.CustomerComboBox.Name = "CustomerComboBox"
        Me.CustomerComboBox.Size = New System.Drawing.Size(222, 21)
        Me.CustomerComboBox.TabIndex = 4
        Me.CustomerComboBox.ValueMember = "Id"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(645, 24)
        Me.MenuStrip1.TabIndex = 2
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(35, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(103, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'EmployeesTableAdapter
        '
        Me.EmployeesTableAdapter.ClearBeforeFill = True
        '
        'NorthwindDataSet
        '
        Me.NorthwindDataSet.DataSetName = "NorthwindDataSet"
        '
        'Label6
        '
        Label6.AutoSize = True
        Label6.Location = New System.Drawing.Point(12, 189)
        Label6.Name = "Label6"
        Label6.Size = New System.Drawing.Size(84, 13)
        Label6.TabIndex = 6
        Label6.Text = "Selection Mode:"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(645, 373)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.tabOptions)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ListBox / ComboBox Sample"
        Me.tabOptions.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        CType(Me.nudDropDownWidth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudDropDownItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        CType(Me.CustomerBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CustomerBindingNavigator.ResumeLayout(False)
        Me.CustomerBindingNavigator.PerformLayout()
        CType(Me.CustomerBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.NorthwindDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tabOptions As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents lblFileName1 As System.Windows.Forms.Label
    Friend WithEvents btnFill1 As System.Windows.Forms.Button
    Friend WithEvents lstProcessesAddItem As System.Windows.Forms.ListBox
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents lblFileInfo As System.Windows.Forms.Label
    Friend WithEvents btnFill2 As System.Windows.Forms.Button
    Friend WithEvents lstFiles As System.Windows.Forms.ListBox
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents lstSelected As System.Windows.Forms.ListBox
    Friend WithEvents lstSelectedItems As System.Windows.Forms.ListBox
    Friend WithEvents cboSelectionMode As System.Windows.Forms.ComboBox
    Friend WithEvents btnFill3 As System.Windows.Forms.Button
    Friend WithEvents lstMultiSelect As System.Windows.Forms.ListBox
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents lblFileName2 As System.Windows.Forms.Label
    Friend WithEvents btnFill4 As System.Windows.Forms.Button
    Friend WithEvents lstProcessesDataSource As System.Windows.Forms.ListBox
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents lblResults As System.Windows.Forms.Label
    Friend WithEvents nudDropDownWidth As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudDropDownItems As System.Windows.Forms.NumericUpDown
    Friend WithEvents cboDropDownStyle As System.Windows.Forms.ComboBox
    Friend WithEvents btnFill5 As System.Windows.Forms.Button
    Friend WithEvents cboDemo As System.Windows.Forms.ComboBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents BindingSourceFillButton As System.Windows.Forms.Button
    Friend WithEvents CustomerComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents IdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents NameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents CustomerBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents EmployeesTableAdapter As ListBoxComboBox.NorthwindDataSetTableAdapters.EmployeesTableAdapter
    Friend WithEvents NorthwindDataSet As ListBoxComboBox.NorthwindDataSet
    Friend WithEvents CustomerBindingNavigator As System.Windows.Forms.BindingNavigator
    Friend WithEvents BindingNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents BindingNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents BindingNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents BindingNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents BindingNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator

End Class
