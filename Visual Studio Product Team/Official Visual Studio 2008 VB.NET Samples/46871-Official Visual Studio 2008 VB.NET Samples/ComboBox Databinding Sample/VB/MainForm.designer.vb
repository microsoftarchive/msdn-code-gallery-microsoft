<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class MainForm
    Inherits System.Windows.Forms.Form


    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.btnArray = New System.Windows.Forms.Button
        Me.btnArrayList = New System.Windows.Forms.Button
        Me.btnArrayListA = New System.Windows.Forms.Button
        Me.btnDS = New System.Windows.Forms.Button
        Me.btnDC = New System.Windows.Forms.Button
        Me.btnDV = New System.Windows.Forms.Button
        Me.grpAssocValue = New System.Windows.Forms.GroupBox
        Me.lblAssocValue = New System.Windows.Forms.Label
        Me.grpDataSource = New System.Windows.Forms.GroupBox
        Me.lblDataSource = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.ComboBox1 = New System.Windows.Forms.ComboBox
        Me.ProductID = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ProductName = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.SupplierID = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.CategoryID = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.QuantityPerUnit = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.UnitPrice = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.UnitsInStock = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.UnitsOnOrder = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ReorderLevel = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Discontinued = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.dataNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton
        Me.dataNavigatorCountItem = New System.Windows.Forms.ToolStripLabel
        Me.dataNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton
        Me.dataNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton
        Me.dataNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton
        Me.dataNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator
        Me.dataNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox
        Me.dataNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.dataNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton
        Me.dataNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton
        Me.dataNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.dataNavigatorSaveItem = New System.Windows.Forms.ToolStripButton
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NorthwindDataSet = New DataComboBox.NorthwindDataSet
        Me.ProductsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ProductsTableAdapter = New DataComboBox.NorthwindDataSetTableAdapters.ProductsTableAdapter
        Me.ProductsDataGridView = New System.Windows.Forms.DataGridView
        Me.DataGridViewTextBoxColumn28 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn29 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn30 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn31 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn32 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn33 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn34 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn35 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn36 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewCheckBoxColumn4 = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.ProductsBindingNavigator = New System.Windows.Forms.BindingNavigator(Me.components)
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
        Me.ProductsBindingNavigatorSaveItem = New System.Windows.Forms.ToolStripButton
        Me.grpAssocValue.SuspendLayout()
        Me.grpDataSource.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.NorthwindDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProductsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProductsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProductsBindingNavigator, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ProductsBindingNavigator.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnArray
        '
        Me.btnArray.Location = New System.Drawing.Point(22, 155)
        Me.btnArray.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.btnArray.Name = "btnArray"
        Me.btnArray.Size = New System.Drawing.Size(90, 34)
        Me.btnArray.TabIndex = 13
        Me.btnArray.Text = "Array"
        '
        'btnArrayList
        '
        Me.btnArrayList.Location = New System.Drawing.Point(119, 155)
        Me.btnArrayList.Name = "btnArrayList"
        Me.btnArrayList.Size = New System.Drawing.Size(90, 34)
        Me.btnArrayList.TabIndex = 14
        Me.btnArrayList.Text = "ArrayList"
        '
        'btnArrayListA
        '
        Me.btnArrayListA.Location = New System.Drawing.Point(216, 155)
        Me.btnArrayListA.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.btnArrayListA.Name = "btnArrayListA"
        Me.btnArrayListA.Size = New System.Drawing.Size(90, 34)
        Me.btnArrayListA.TabIndex = 15
        Me.btnArrayListA.Text = "ArrrayList - Advanced"
        '
        'btnDS
        '
        Me.btnDS.Location = New System.Drawing.Point(22, 204)
        Me.btnDS.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.btnDS.Name = "btnDS"
        Me.btnDS.Size = New System.Drawing.Size(90, 34)
        Me.btnDS.TabIndex = 16
        Me.btnDS.Text = "DataSet"
        '
        'btnDC
        '
        Me.btnDC.Location = New System.Drawing.Point(216, 204)
        Me.btnDC.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.btnDC.Name = "btnDC"
        Me.btnDC.Size = New System.Drawing.Size(90, 34)
        Me.btnDC.TabIndex = 18
        Me.btnDC.Text = "BindingSource"
        '
        'btnDV
        '
        Me.btnDV.Location = New System.Drawing.Point(119, 204)
        Me.btnDV.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.btnDV.Name = "btnDV"
        Me.btnDV.Size = New System.Drawing.Size(90, 34)
        Me.btnDV.TabIndex = 17
        Me.btnDV.Text = "DataView"
        '
        'grpAssocValue
        '
        Me.grpAssocValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpAssocValue.Controls.Add(Me.lblAssocValue)
        Me.grpAssocValue.Location = New System.Drawing.Point(359, 168)
        Me.grpAssocValue.Name = "grpAssocValue"
        Me.grpAssocValue.Size = New System.Drawing.Size(276, 70)
        Me.grpAssocValue.TabIndex = 20
        Me.grpAssocValue.TabStop = False
        Me.grpAssocValue.Text = "Associated Value"
        '
        'lblAssocValue
        '
        Me.lblAssocValue.Location = New System.Drawing.Point(14, 22)
        Me.lblAssocValue.Name = "lblAssocValue"
        Me.lblAssocValue.Size = New System.Drawing.Size(175, 35)
        Me.lblAssocValue.TabIndex = 0
        '
        'grpDataSource
        '
        Me.grpDataSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpDataSource.Controls.Add(Me.lblDataSource)
        Me.grpDataSource.Location = New System.Drawing.Point(359, 82)
        Me.grpDataSource.Name = "grpDataSource"
        Me.grpDataSource.Size = New System.Drawing.Size(276, 70)
        Me.grpDataSource.TabIndex = 19
        Me.grpDataSource.TabStop = False
        Me.grpDataSource.Text = "Data Source"
        '
        'lblDataSource
        '
        Me.lblDataSource.Location = New System.Drawing.Point(13, 22)
        Me.lblDataSource.Name = "lblDataSource"
        Me.lblDataSource.Size = New System.Drawing.Size(175, 35)
        Me.lblDataSource.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(19, 119)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 3, 3, 2)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(320, 33)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Populate the Combo Box using one of the data sources below"
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(21, 82)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(223, 21)
        Me.ComboBox1.TabIndex = 11
        '
        'ProductID
        '
        Me.ProductID.DataPropertyName = "ProductID"
        Me.ProductID.HeaderText = "ProductID"
        Me.ProductID.Name = "ProductID"
        Me.ProductID.ReadOnly = True
        '
        'ProductName
        '
        Me.ProductName.DataPropertyName = "ProductName"
        Me.ProductName.HeaderText = "ProductName"
        Me.ProductName.Name = "ProductName"
        '
        'SupplierID
        '
        Me.SupplierID.DataPropertyName = "SupplierID"
        Me.SupplierID.HeaderText = "SupplierID"
        Me.SupplierID.Name = "SupplierID"
        '
        'CategoryID
        '
        Me.CategoryID.DataPropertyName = "CategoryID"
        Me.CategoryID.HeaderText = "CategoryID"
        Me.CategoryID.Name = "CategoryID"
        '
        'QuantityPerUnit
        '
        Me.QuantityPerUnit.DataPropertyName = "QuantityPerUnit"
        Me.QuantityPerUnit.HeaderText = "QuantityPerUnit"
        Me.QuantityPerUnit.Name = "QuantityPerUnit"
        '
        'UnitPrice
        '
        Me.UnitPrice.DataPropertyName = "UnitPrice"
        Me.UnitPrice.HeaderText = "UnitPrice"
        Me.UnitPrice.Name = "UnitPrice"
        '
        'UnitsInStock
        '
        Me.UnitsInStock.DataPropertyName = "UnitsInStock"
        Me.UnitsInStock.HeaderText = "UnitsInStock"
        Me.UnitsInStock.Name = "UnitsInStock"
        '
        'UnitsOnOrder
        '
        Me.UnitsOnOrder.DataPropertyName = "UnitsOnOrder"
        Me.UnitsOnOrder.HeaderText = "UnitsOnOrder"
        Me.UnitsOnOrder.Name = "UnitsOnOrder"
        '
        'ReorderLevel
        '
        Me.ReorderLevel.DataPropertyName = "ReorderLevel"
        Me.ReorderLevel.HeaderText = "ReorderLevel"
        Me.ReorderLevel.Name = "ReorderLevel"
        '
        'Discontinued
        '
        Me.Discontinued.DataPropertyName = "Discontinued"
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Discontinued.DefaultCellStyle = DataGridViewCellStyle2
        Me.Discontinued.HeaderText = "Discontinued"
        Me.Discontinued.Name = "Discontinued"
        '
        'dataNavigatorAddNewItem
        '
        Me.dataNavigatorAddNewItem.Image = CType(resources.GetObject("dataNavigatorAddNewItem.Image"), System.Drawing.Image)
        Me.dataNavigatorAddNewItem.Name = "dataNavigatorAddNewItem"
        Me.dataNavigatorAddNewItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorAddNewItem.Text = "Add new"
        '
        'dataNavigatorCountItem
        '
        Me.dataNavigatorCountItem.Name = "dataNavigatorCountItem"
        Me.dataNavigatorCountItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorCountItem.Text = "of 0"
        Me.dataNavigatorCountItem.ToolTipText = "Total number of items"
        '
        'dataNavigatorDeleteItem
        '
        Me.dataNavigatorDeleteItem.Image = CType(resources.GetObject("dataNavigatorDeleteItem.Image"), System.Drawing.Image)
        Me.dataNavigatorDeleteItem.Name = "dataNavigatorDeleteItem"
        Me.dataNavigatorDeleteItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorDeleteItem.Text = "Delete"
        '
        'dataNavigatorMoveFirstItem
        '
        Me.dataNavigatorMoveFirstItem.Image = CType(resources.GetObject("dataNavigatorMoveFirstItem.Image"), System.Drawing.Image)
        Me.dataNavigatorMoveFirstItem.Name = "dataNavigatorMoveFirstItem"
        Me.dataNavigatorMoveFirstItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorMoveFirstItem.Text = "Move first"
        '
        'dataNavigatorMovePreviousItem
        '
        Me.dataNavigatorMovePreviousItem.Image = CType(resources.GetObject("dataNavigatorMovePreviousItem.Image"), System.Drawing.Image)
        Me.dataNavigatorMovePreviousItem.Name = "dataNavigatorMovePreviousItem"
        Me.dataNavigatorMovePreviousItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorMovePreviousItem.Text = "Move previous"
        '
        'dataNavigatorSeparator
        '
        Me.dataNavigatorSeparator.Name = "dataNavigatorSeparator"
        Me.dataNavigatorSeparator.Size = New System.Drawing.Size(6, 6)
        '
        'dataNavigatorPositionItem
        '
        Me.dataNavigatorPositionItem.Name = "dataNavigatorPositionItem"
        Me.dataNavigatorPositionItem.Size = New System.Drawing.Size(50, 25)
        Me.dataNavigatorPositionItem.Text = "0"
        Me.dataNavigatorPositionItem.ToolTipText = "Current position"
        '
        'dataNavigatorSeparator1
        '
        Me.dataNavigatorSeparator1.Name = "dataNavigatorSeparator1"
        Me.dataNavigatorSeparator1.Size = New System.Drawing.Size(6, 6)
        '
        'dataNavigatorMoveNextItem
        '
        Me.dataNavigatorMoveNextItem.Image = CType(resources.GetObject("dataNavigatorMoveNextItem.Image"), System.Drawing.Image)
        Me.dataNavigatorMoveNextItem.Name = "dataNavigatorMoveNextItem"
        Me.dataNavigatorMoveNextItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorMoveNextItem.Text = "Move next"
        '
        'dataNavigatorMoveLastItem
        '
        Me.dataNavigatorMoveLastItem.Image = CType(resources.GetObject("dataNavigatorMoveLastItem.Image"), System.Drawing.Image)
        Me.dataNavigatorMoveLastItem.Name = "dataNavigatorMoveLastItem"
        Me.dataNavigatorMoveLastItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorMoveLastItem.Text = "Move last"
        '
        'dataNavigatorSeparator2
        '
        Me.dataNavigatorSeparator2.Name = "dataNavigatorSeparator2"
        Me.dataNavigatorSeparator2.Size = New System.Drawing.Size(6, 6)
        '
        'dataNavigatorSaveItem
        '
        Me.dataNavigatorSaveItem.Enabled = False
        Me.dataNavigatorSaveItem.Image = CType(resources.GetObject("dataNavigatorSaveItem.Image"), System.Drawing.Image)
        Me.dataNavigatorSaveItem.Name = "dataNavigatorSaveItem"
        Me.dataNavigatorSaveItem.Size = New System.Drawing.Size(23, 23)
        Me.dataNavigatorSaveItem.Text = "Save Data"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(695, 24)
        Me.MenuStrip1.TabIndex = 21
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
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'NorthwindDataSet
        '
        Me.NorthwindDataSet.DataSetName = "NorthwindDataSet"
        Me.NorthwindDataSet.Locale = New System.Globalization.CultureInfo("en-US")
        '
        'ProductsBindingSource
        '
        Me.ProductsBindingSource.DataMember = "Products"
        Me.ProductsBindingSource.DataSource = Me.NorthwindDataSet
        '
        'ProductsTableAdapter
        '
        Me.ProductsTableAdapter.ClearBeforeFill = True
        '
        'ProductsDataGridView
        '
        Me.ProductsDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProductsDataGridView.AutoGenerateColumns = False
        Me.ProductsDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn28, Me.DataGridViewTextBoxColumn29, Me.DataGridViewTextBoxColumn30, Me.DataGridViewTextBoxColumn31, Me.DataGridViewTextBoxColumn32, Me.DataGridViewTextBoxColumn33, Me.DataGridViewTextBoxColumn34, Me.DataGridViewTextBoxColumn35, Me.DataGridViewTextBoxColumn36, Me.DataGridViewCheckBoxColumn4})
        Me.ProductsDataGridView.DataSource = Me.ProductsBindingSource
        Me.ProductsDataGridView.Location = New System.Drawing.Point(12, 251)
        Me.ProductsDataGridView.Name = "ProductsDataGridView"
        Me.ProductsDataGridView.Size = New System.Drawing.Size(671, 225)
        Me.ProductsDataGridView.TabIndex = 23
        '
        'DataGridViewTextBoxColumn28
        '
        Me.DataGridViewTextBoxColumn28.DataPropertyName = "ProductID"
        Me.DataGridViewTextBoxColumn28.HeaderText = "ProductID"
        Me.DataGridViewTextBoxColumn28.Name = "DataGridViewTextBoxColumn28"
        Me.DataGridViewTextBoxColumn28.ReadOnly = True
        '
        'DataGridViewTextBoxColumn29
        '
        Me.DataGridViewTextBoxColumn29.DataPropertyName = "ProductName"
        Me.DataGridViewTextBoxColumn29.HeaderText = "ProductName"
        Me.DataGridViewTextBoxColumn29.Name = "DataGridViewTextBoxColumn29"
        '
        'DataGridViewTextBoxColumn30
        '
        Me.DataGridViewTextBoxColumn30.DataPropertyName = "SupplierID"
        Me.DataGridViewTextBoxColumn30.HeaderText = "SupplierID"
        Me.DataGridViewTextBoxColumn30.Name = "DataGridViewTextBoxColumn30"
        '
        'DataGridViewTextBoxColumn31
        '
        Me.DataGridViewTextBoxColumn31.DataPropertyName = "CategoryID"
        Me.DataGridViewTextBoxColumn31.HeaderText = "CategoryID"
        Me.DataGridViewTextBoxColumn31.Name = "DataGridViewTextBoxColumn31"
        '
        'DataGridViewTextBoxColumn32
        '
        Me.DataGridViewTextBoxColumn32.DataPropertyName = "QuantityPerUnit"
        Me.DataGridViewTextBoxColumn32.HeaderText = "QuantityPerUnit"
        Me.DataGridViewTextBoxColumn32.Name = "DataGridViewTextBoxColumn32"
        '
        'DataGridViewTextBoxColumn33
        '
        Me.DataGridViewTextBoxColumn33.DataPropertyName = "UnitPrice"
        Me.DataGridViewTextBoxColumn33.HeaderText = "UnitPrice"
        Me.DataGridViewTextBoxColumn33.Name = "DataGridViewTextBoxColumn33"
        '
        'DataGridViewTextBoxColumn34
        '
        Me.DataGridViewTextBoxColumn34.DataPropertyName = "UnitsInStock"
        Me.DataGridViewTextBoxColumn34.HeaderText = "UnitsInStock"
        Me.DataGridViewTextBoxColumn34.Name = "DataGridViewTextBoxColumn34"
        '
        'DataGridViewTextBoxColumn35
        '
        Me.DataGridViewTextBoxColumn35.DataPropertyName = "UnitsOnOrder"
        Me.DataGridViewTextBoxColumn35.HeaderText = "UnitsOnOrder"
        Me.DataGridViewTextBoxColumn35.Name = "DataGridViewTextBoxColumn35"
        '
        'DataGridViewTextBoxColumn36
        '
        Me.DataGridViewTextBoxColumn36.DataPropertyName = "ReorderLevel"
        Me.DataGridViewTextBoxColumn36.HeaderText = "ReorderLevel"
        Me.DataGridViewTextBoxColumn36.Name = "DataGridViewTextBoxColumn36"
        '
        'DataGridViewCheckBoxColumn4
        '
        Me.DataGridViewCheckBoxColumn4.DataPropertyName = "Discontinued"
        Me.DataGridViewCheckBoxColumn4.HeaderText = "Discontinued"
        Me.DataGridViewCheckBoxColumn4.Name = "DataGridViewCheckBoxColumn4"
        '
        'ProductsBindingNavigator
        '
        Me.ProductsBindingNavigator.AddNewItem = Me.BindingNavigatorAddNewItem
        Me.ProductsBindingNavigator.BindingSource = Me.ProductsBindingSource
        Me.ProductsBindingNavigator.CountItem = Me.BindingNavigatorCountItem
        Me.ProductsBindingNavigator.DeleteItem = Me.BindingNavigatorDeleteItem
        Me.ProductsBindingNavigator.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.ProductsBindingNavigatorSaveItem})
        Me.ProductsBindingNavigator.Location = New System.Drawing.Point(0, 24)
        Me.ProductsBindingNavigator.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
        Me.ProductsBindingNavigator.MoveLastItem = Me.BindingNavigatorMoveLastItem
        Me.ProductsBindingNavigator.MoveNextItem = Me.BindingNavigatorMoveNextItem
        Me.ProductsBindingNavigator.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
        Me.ProductsBindingNavigator.Name = "ProductsBindingNavigator"
        Me.ProductsBindingNavigator.PositionItem = Me.BindingNavigatorPositionItem
        Me.ProductsBindingNavigator.Size = New System.Drawing.Size(695, 25)
        Me.ProductsBindingNavigator.TabIndex = 24
        Me.ProductsBindingNavigator.Text = "BindingNavigator1"
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
        'ProductsBindingNavigatorSaveItem
        '
        Me.ProductsBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ProductsBindingNavigatorSaveItem.Image = CType(resources.GetObject("ProductsBindingNavigatorSaveItem.Image"), System.Drawing.Image)
        Me.ProductsBindingNavigatorSaveItem.Name = "ProductsBindingNavigatorSaveItem"
        Me.ProductsBindingNavigatorSaveItem.Size = New System.Drawing.Size(23, 22)
        Me.ProductsBindingNavigatorSaveItem.Text = "Save Data"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(695, 488)
        Me.Controls.Add(Me.ProductsBindingNavigator)
        Me.Controls.Add(Me.ProductsDataGridView)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.btnArray)
        Me.Controls.Add(Me.btnArrayList)
        Me.Controls.Add(Me.btnArrayListA)
        Me.Controls.Add(Me.btnDS)
        Me.Controls.Add(Me.btnDC)
        Me.Controls.Add(Me.btnDV)
        Me.Controls.Add(Me.grpAssocValue)
        Me.Controls.Add(Me.grpDataSource)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Binding Data to a ComboBox"
        Me.grpAssocValue.ResumeLayout(False)
        Me.grpDataSource.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.NorthwindDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProductsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProductsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProductsBindingNavigator, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ProductsBindingNavigator.ResumeLayout(False)
        Me.ProductsBindingNavigator.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnArray As System.Windows.Forms.Button
    Friend WithEvents btnArrayList As System.Windows.Forms.Button
    Friend WithEvents btnArrayListA As System.Windows.Forms.Button
    Friend WithEvents btnDS As System.Windows.Forms.Button
    Friend WithEvents btnDC As System.Windows.Forms.Button
    Friend WithEvents btnDV As System.Windows.Forms.Button
    Friend WithEvents grpAssocValue As System.Windows.Forms.GroupBox
    Friend WithEvents lblAssocValue As System.Windows.Forms.Label
    Friend WithEvents grpDataSource As System.Windows.Forms.GroupBox
    Friend WithEvents lblDataSource As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn9 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewCheckBoxColumn1 As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents dataNavigatorAddNewItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents dataNavigatorCountItem As System.Windows.Forms.ToolStripLabel
    Friend WithEvents dataNavigatorDeleteItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents dataNavigatorMoveFirstItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents dataNavigatorMovePreviousItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents dataNavigatorSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents dataNavigatorPositionItem As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents dataNavigatorSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents dataNavigatorMoveNextItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents dataNavigatorMoveLastItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents dataNavigatorSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents dataNavigatorSaveItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents DataGridViewTextBoxColumn10 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn11 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn12 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn13 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn14 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn15 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn16 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn17 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn18 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewCheckBoxColumn2 As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NorthwindDataSet As DataComboBox.NorthwindDataSet
    Friend WithEvents ProductsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ProductsTableAdapter As DataComboBox.NorthwindDataSetTableAdapters.ProductsTableAdapter
    Friend WithEvents ProductID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend Shadows WithEvents ProductName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SupplierID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CategoryID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents QuantityPerUnit As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UnitPrice As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UnitsInStock As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UnitsOnOrder As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ReorderLevel As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Discontinued As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ProductsDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn28 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn29 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn30 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn31 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn32 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn33 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn34 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn35 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn36 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewCheckBoxColumn4 As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ProductsBindingNavigator As System.Windows.Forms.BindingNavigator
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
    Friend WithEvents ProductsBindingNavigatorSaveItem As System.Windows.Forms.ToolStripButton

End Class
