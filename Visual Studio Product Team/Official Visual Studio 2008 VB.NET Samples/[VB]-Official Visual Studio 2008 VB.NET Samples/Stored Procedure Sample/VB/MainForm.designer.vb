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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.pgeCreateSprocs = New System.Windows.Forms.TabPage
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnCreateDatabase = New System.Windows.Forms.Button
        Me.tabApp = New System.Windows.Forms.TabControl
        Me.pgeNoParams = New System.Windows.Forms.TabPage
        Me.txtTenMostExpProds = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.getListOfFirstNames = New System.Windows.Forms.Button
        Me.pgeInputParam = New System.Windows.Forms.TabPage
        Me.productsGrid = New System.Windows.Forms.DataGridView
        Me.Label4 = New System.Windows.Forms.Label
        Me.btnGetEmployees = New System.Windows.Forms.Button
        Me.cboCategoriesInputParam = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.pgeAllTypes = New System.Windows.Forms.TabPage
        Me.lblProductCountAndAvgPrice = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.btnGetCountInCountry = New System.Windows.Forms.Button
        Me.cboCountries = New System.Windows.Forms.ComboBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.pgeCreateSprocs.SuspendLayout()
        Me.tabApp.SuspendLayout()
        Me.pgeNoParams.SuspendLayout()
        Me.pgeInputParam.SuspendLayout()
        CType(Me.productsGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pgeAllTypes.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pgeCreateSprocs
        '
        Me.pgeCreateSprocs.AccessibleDescription = "TabPage with text ""Create Sprocs"""
        Me.pgeCreateSprocs.AccessibleName = "Create Sprocs tab"
        Me.pgeCreateSprocs.Controls.Add(Me.Label2)
        Me.pgeCreateSprocs.Controls.Add(Me.btnCreateDatabase)
        Me.pgeCreateSprocs.Location = New System.Drawing.Point(4, 22)
        Me.pgeCreateSprocs.Name = "pgeCreateSprocs"
        Me.pgeCreateSprocs.Size = New System.Drawing.Size(440, 334)
        Me.pgeCreateSprocs.TabIndex = 0
        Me.pgeCreateSprocs.Text = "Create Sprocs"
        Me.pgeCreateSprocs.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with description of Create Sprocs example"
        Me.Label2.AccessibleName = "Create Sprocs intro"
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(8, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(424, 51)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Click the button to create the SQL Server StoredProceduresDemo Database that are " & _
            "required for this demo. " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If it already exist, the database and all the objects " & _
            "will be dropped and recreated."
        '
        'btnCreateDatabase
        '
        Me.btnCreateDatabase.AccessibleDescription = "Button with text ""Create DataBase"""
        Me.btnCreateDatabase.AccessibleName = "Create Sprocs"
        Me.btnCreateDatabase.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateDatabase.Location = New System.Drawing.Point(159, 62)
        Me.btnCreateDatabase.Name = "btnCreateDatabase"
        Me.btnCreateDatabase.Size = New System.Drawing.Size(104, 23)
        Me.btnCreateDatabase.TabIndex = 0
        Me.btnCreateDatabase.Text = "&Create Database"
        '
        'tabApp
        '
        Me.tabApp.AccessibleDescription = "TabControl for the entire application"
        Me.tabApp.AccessibleName = "Tab Control"
        Me.tabApp.Controls.Add(Me.pgeCreateSprocs)
        Me.tabApp.Controls.Add(Me.pgeNoParams)
        Me.tabApp.Controls.Add(Me.pgeInputParam)
        Me.tabApp.Controls.Add(Me.pgeAllTypes)
        Me.tabApp.ItemSize = New System.Drawing.Size(79, 18)
        Me.tabApp.Location = New System.Drawing.Point(24, 62)
        Me.tabApp.Name = "tabApp"
        Me.tabApp.SelectedIndex = 0
        Me.tabApp.Size = New System.Drawing.Size(448, 360)
        Me.tabApp.TabIndex = 6
        '
        'pgeNoParams
        '
        Me.pgeNoParams.AccessibleDescription = "TabPage with text ""No Params"""
        Me.pgeNoParams.AccessibleName = "No Params tab"
        Me.pgeNoParams.Controls.Add(Me.txtTenMostExpProds)
        Me.pgeNoParams.Controls.Add(Me.Label1)
        Me.pgeNoParams.Controls.Add(Me.getListOfFirstNames)
        Me.pgeNoParams.Location = New System.Drawing.Point(4, 22)
        Me.pgeNoParams.Name = "pgeNoParams"
        Me.pgeNoParams.Size = New System.Drawing.Size(440, 334)
        Me.pgeNoParams.TabIndex = 1
        Me.pgeNoParams.Text = "No Params"
        Me.pgeNoParams.UseVisualStyleBackColor = False
        '
        'txtTenMostExpProds
        '
        Me.txtTenMostExpProds.AccessibleDescription = "TextBox to display the 10 Most Expensive Products"
        Me.txtTenMostExpProds.AccessibleName = "Get the 10 Most Expensive Products TextBox"
        Me.txtTenMostExpProds.Location = New System.Drawing.Point(24, 104)
        Me.txtTenMostExpProds.Multiline = True
        Me.txtTenMostExpProds.Name = "txtTenMostExpProds"
        Me.txtTenMostExpProds.Size = New System.Drawing.Size(392, 208)
        Me.txtTenMostExpProds.TabIndex = 13
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with description of No Params example"
        Me.Label1.AccessibleName = "No Params intro"
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(424, 56)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "This example shows how to execute one of the stored procedures added to the sampl" & _
            "e database. It takes no input parameters. This example also shows how to use a S" & _
            "qlDataReader."
        '
        'getListOfFirstNames
        '
        Me.getListOfFirstNames.AccessibleDescription = "Button with text ""Get the 10 Most Expensive Products"""
        Me.getListOfFirstNames.AccessibleName = "Get the 10 Most Expensive Products"
        Me.getListOfFirstNames.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.getListOfFirstNames.Location = New System.Drawing.Point(96, 69)
        Me.getListOfFirstNames.Name = "getListOfFirstNames"
        Me.getListOfFirstNames.Size = New System.Drawing.Size(240, 23)
        Me.getListOfFirstNames.TabIndex = 0
        Me.getListOfFirstNames.Text = "&Get List of First Names"
        '
        'pgeInputParam
        '
        Me.pgeInputParam.AccessibleDescription = "TabPage with text ""Input Param"""
        Me.pgeInputParam.AccessibleName = "Input Param tab"
        Me.pgeInputParam.Controls.Add(Me.productsGrid)
        Me.pgeInputParam.Controls.Add(Me.Label4)
        Me.pgeInputParam.Controls.Add(Me.btnGetEmployees)
        Me.pgeInputParam.Controls.Add(Me.cboCategoriesInputParam)
        Me.pgeInputParam.Controls.Add(Me.Label3)
        Me.pgeInputParam.Location = New System.Drawing.Point(4, 22)
        Me.pgeInputParam.Name = "pgeInputParam"
        Me.pgeInputParam.Size = New System.Drawing.Size(440, 334)
        Me.pgeInputParam.TabIndex = 2
        Me.pgeInputParam.Text = "Input Param"
        Me.pgeInputParam.UseVisualStyleBackColor = False
        '
        'productsGrid
        '
        Me.productsGrid.Location = New System.Drawing.Point(8, 124)
        Me.productsGrid.Name = "productsGrid"
        Me.productsGrid.Size = New System.Drawing.Size(412, 197)
        Me.productsGrid.TabIndex = 14
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with description of Input Param example"
        Me.Label4.AccessibleName = "Input Param intro"
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(8, 8)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(424, 56)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = resources.GetString("Label4.Text")
        '
        'btnGetEmployees
        '
        Me.btnGetEmployees.AccessibleDescription = "Button with text ""Get Products"""
        Me.btnGetEmployees.AccessibleName = "Get Products"
        Me.btnGetEmployees.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetEmployees.Location = New System.Drawing.Point(250, 68)
        Me.btnGetEmployees.Name = "btnGetEmployees"
        Me.btnGetEmployees.Size = New System.Drawing.Size(94, 23)
        Me.btnGetEmployees.TabIndex = 1
        Me.btnGetEmployees.Text = "&Get Employees"
        '
        'cboCategoriesInputParam
        '
        Me.cboCategoriesInputParam.AccessibleDescription = "ComboBox containing product categories for Input Param example"
        Me.cboCategoriesInputParam.AccessibleName = "Categories for Input Param example"
        Me.cboCategoriesInputParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCategoriesInputParam.FormattingEnabled = True
        Me.cboCategoriesInputParam.ItemHeight = 13
        Me.cboCategoriesInputParam.Location = New System.Drawing.Point(122, 69)
        Me.cboCategoriesInputParam.Name = "cboCategoriesInputParam"
        Me.cboCategoriesInputParam.Size = New System.Drawing.Size(121, 21)
        Me.cboCategoriesInputParam.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label with text ""Category"""
        Me.Label3.AccessibleName = "Category "
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(54, 72)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 23)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "First Name:"
        '
        'pgeAllTypes
        '
        Me.pgeAllTypes.AccessibleDescription = "TabPage with text ""All Types"""
        Me.pgeAllTypes.AccessibleName = "All Types tab"
        Me.pgeAllTypes.Controls.Add(Me.lblProductCountAndAvgPrice)
        Me.pgeAllTypes.Controls.Add(Me.Label5)
        Me.pgeAllTypes.Controls.Add(Me.btnGetCountInCountry)
        Me.pgeAllTypes.Controls.Add(Me.cboCountries)
        Me.pgeAllTypes.Controls.Add(Me.Label6)
        Me.pgeAllTypes.Location = New System.Drawing.Point(4, 22)
        Me.pgeAllTypes.Name = "pgeAllTypes"
        Me.pgeAllTypes.Size = New System.Drawing.Size(440, 334)
        Me.pgeAllTypes.TabIndex = 3
        Me.pgeAllTypes.Text = "All Types"
        Me.pgeAllTypes.UseVisualStyleBackColor = False
        '
        'lblProductCountAndAvgPrice
        '
        Me.lblProductCountAndAvgPrice.AccessibleDescription = "Label to display Product Count and Avg Price"
        Me.lblProductCountAndAvgPrice.AccessibleName = "Product Count and Avg Price results"
        Me.lblProductCountAndAvgPrice.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblProductCountAndAvgPrice.Location = New System.Drawing.Point(16, 157)
        Me.lblProductCountAndAvgPrice.Name = "lblProductCountAndAvgPrice"
        Me.lblProductCountAndAvgPrice.Size = New System.Drawing.Size(416, 40)
        Me.lblProductCountAndAvgPrice.TabIndex = 18
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label with description of All Types example"
        Me.Label5.AccessibleName = "All Types intro"
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(8, 8)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(424, 72)
        Me.Label5.TabIndex = 17
        Me.Label5.Text = resources.GetString("Label5.Text")
        '
        'btnGetCountInCountry
        '
        Me.btnGetCountInCountry.AccessibleDescription = "Button with text ""Get Product Count and Avg Price"""
        Me.btnGetCountInCountry.AccessibleName = "Get Product Count and Avg Price"
        Me.btnGetCountInCountry.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetCountInCountry.Location = New System.Drawing.Point(95, 117)
        Me.btnGetCountInCountry.Name = "btnGetCountInCountry"
        Me.btnGetCountInCountry.Size = New System.Drawing.Size(232, 23)
        Me.btnGetCountInCountry.TabIndex = 1
        Me.btnGetCountInCountry.Text = "&Get Count of Employees in Each Country"
        '
        'cboCountries
        '
        Me.cboCountries.AccessibleDescription = "ComboBox containing product categories for All Types example"
        Me.cboCountries.AccessibleName = "Categories for All Types example"
        Me.cboCountries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCountries.FormattingEnabled = True
        Me.cboCountries.ItemHeight = 13
        Me.cboCountries.Location = New System.Drawing.Point(163, 85)
        Me.cboCountries.Name = "cboCountries"
        Me.cboCountries.Size = New System.Drawing.Size(121, 21)
        Me.cboCountries.TabIndex = 0
        '
        'Label6
        '
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(113, 89)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(56, 23)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Country:"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = "Label with text ""Using Stored Procedures"""
        Me.Label7.AccessibleName = "Application Title"
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(20, 35)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(208, 23)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Using Stored Procedures"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(493, 24)
        Me.MenuStrip1.TabIndex = 8
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
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(493, 457)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.tabApp)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.pgeCreateSprocs.ResumeLayout(False)
        Me.tabApp.ResumeLayout(False)
        Me.pgeNoParams.ResumeLayout(False)
        Me.pgeNoParams.PerformLayout()
        Me.pgeInputParam.ResumeLayout(False)
        CType(Me.productsGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pgeAllTypes.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pgeCreateSprocs As System.Windows.Forms.TabPage
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnCreateDatabase As System.Windows.Forms.Button
    Friend WithEvents tabApp As System.Windows.Forms.TabControl
    Friend WithEvents pgeNoParams As System.Windows.Forms.TabPage
    Friend WithEvents txtTenMostExpProds As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents getListOfFirstNames As System.Windows.Forms.Button
    Friend WithEvents pgeInputParam As System.Windows.Forms.TabPage
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnGetEmployees As System.Windows.Forms.Button
    Friend WithEvents cboCategoriesInputParam As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents pgeAllTypes As System.Windows.Forms.TabPage
    Friend WithEvents lblProductCountAndAvgPrice As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btnGetCountInCountry As System.Windows.Forms.Button
    Friend WithEvents cboCountries As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents productsGrid As System.Windows.Forms.DataGridView
End Class
