Partial Public Class MainForm
    Inherits System.Windows.Forms.Form


    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub





    Friend WithEvents radioDictionary As System.Windows.Forms.RadioButton
    Friend WithEvents radioStack As System.Windows.Forms.RadioButton
    Friend WithEvents radioList As System.Windows.Forms.RadioButton
    Friend WithEvents radioQueue As System.Windows.Forms.RadioButton
    Friend WithEvents radioSortedDictionary As System.Windows.Forms.RadioButton
    Friend WithEvents groupDataStructure As System.Windows.Forms.GroupBox
    Friend WithEvents groupDataType As System.Windows.Forms.GroupBox
    Friend WithEvents radioLong As System.Windows.Forms.RadioButton
    Friend WithEvents radioString As System.Windows.Forms.RadioButton
    Friend WithEvents listSourceData As System.Windows.Forms.ListBox
    Friend WithEvents radioObject As System.Windows.Forms.RadioButton
    Friend WithEvents listTargetData As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmdSort As System.Windows.Forms.Button
    Friend WithEvents cmdEmpty As System.Windows.Forms.Button
    Friend WithEvents cmdReverse As System.Windows.Forms.Button
    Friend WithEvents cmdLoad As System.Windows.Forms.Button


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.groupDataStructure = New System.Windows.Forms.GroupBox
        Me.radioSortedDictionary = New System.Windows.Forms.RadioButton
        Me.radioDictionary = New System.Windows.Forms.RadioButton
        Me.radioStack = New System.Windows.Forms.RadioButton
        Me.radioList = New System.Windows.Forms.RadioButton
        Me.radioQueue = New System.Windows.Forms.RadioButton
        Me.listSourceData = New System.Windows.Forms.ListBox
        Me.groupDataType = New System.Windows.Forms.GroupBox
        Me.radioObject = New System.Windows.Forms.RadioButton
        Me.radioLong = New System.Windows.Forms.RadioButton
        Me.radioString = New System.Windows.Forms.RadioButton
        Me.listTargetData = New System.Windows.Forms.ListBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.cmdEmpty = New System.Windows.Forms.Button
        Me.cmdLoad = New System.Windows.Forms.Button
        Me.cmdSort = New System.Windows.Forms.Button
        Me.cmdReverse = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.groupDataStructure.SuspendLayout()
        Me.groupDataType.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupDataStructure
        '
        Me.groupDataStructure.Controls.Add(Me.radioSortedDictionary)
        Me.groupDataStructure.Controls.Add(Me.radioDictionary)
        Me.groupDataStructure.Controls.Add(Me.radioStack)
        Me.groupDataStructure.Controls.Add(Me.radioList)
        Me.groupDataStructure.Controls.Add(Me.radioQueue)
        Me.groupDataStructure.Location = New System.Drawing.Point(12, 43)
        Me.groupDataStructure.Name = "groupDataStructure"
        Me.groupDataStructure.Size = New System.Drawing.Size(244, 204)
        Me.groupDataStructure.TabIndex = 1
        Me.groupDataStructure.TabStop = False
        Me.groupDataStructure.Text = "Generic Data Structure"
        '
        'radioSortedDictionary
        '
        Me.radioSortedDictionary.Location = New System.Drawing.Point(14, 168)
        Me.radioSortedDictionary.Name = "radioSortedDictionary"
        Me.radioSortedDictionary.Size = New System.Drawing.Size(218, 25)
        Me.radioSortedDictionary.TabIndex = 4
        Me.radioSortedDictionary.Text = "Sorted List"
        '
        'radioDictionary
        '
        Me.radioDictionary.Location = New System.Drawing.Point(14, 132)
        Me.radioDictionary.Name = "radioDictionary"
        Me.radioDictionary.Size = New System.Drawing.Size(218, 25)
        Me.radioDictionary.TabIndex = 3
        Me.radioDictionary.Text = "Dictionary"
        '
        'radioStack
        '
        Me.radioStack.Location = New System.Drawing.Point(14, 96)
        Me.radioStack.Name = "radioStack"
        Me.radioStack.Size = New System.Drawing.Size(218, 25)
        Me.radioStack.TabIndex = 2
        Me.radioStack.Text = "Stack"
        '
        'radioList
        '
        Me.radioList.Location = New System.Drawing.Point(14, 60)
        Me.radioList.Name = "radioList"
        Me.radioList.Size = New System.Drawing.Size(218, 25)
        Me.radioList.TabIndex = 1
        Me.radioList.Text = "List"
        '
        'radioQueue
        '
        Me.radioQueue.Location = New System.Drawing.Point(14, 24)
        Me.radioQueue.Name = "radioQueue"
        Me.radioQueue.Size = New System.Drawing.Size(218, 25)
        Me.radioQueue.TabIndex = 0
        Me.radioQueue.Text = "Queue"
        '
        'listSourceData
        '
        Me.listSourceData.FormattingEnabled = True
        Me.listSourceData.Location = New System.Drawing.Point(14, 280)
        Me.listSourceData.Name = "listSourceData"
        Me.listSourceData.Size = New System.Drawing.Size(242, 134)
        Me.listSourceData.TabIndex = 2
        '
        'groupDataType
        '
        Me.groupDataType.Controls.Add(Me.radioObject)
        Me.groupDataType.Controls.Add(Me.radioLong)
        Me.groupDataType.Controls.Add(Me.radioString)
        Me.groupDataType.Location = New System.Drawing.Point(263, 43)
        Me.groupDataType.Name = "groupDataType"
        Me.groupDataType.Size = New System.Drawing.Size(242, 124)
        Me.groupDataType.TabIndex = 3
        Me.groupDataType.TabStop = False
        Me.groupDataType.Text = "Data Type"
        '
        'radioObject
        '
        Me.radioObject.Location = New System.Drawing.Point(14, 82)
        Me.radioObject.Name = "radioObject"
        Me.radioObject.Size = New System.Drawing.Size(192, 31)
        Me.radioObject.TabIndex = 2
        Me.radioObject.Text = "Customer"
        '
        'radioLong
        '
        Me.radioLong.Location = New System.Drawing.Point(14, 51)
        Me.radioLong.Name = "radioLong"
        Me.radioLong.Size = New System.Drawing.Size(192, 31)
        Me.radioLong.TabIndex = 1
        Me.radioLong.Text = "Long"
        '
        'radioString
        '
        Me.radioString.Checked = True
        Me.radioString.Location = New System.Drawing.Point(14, 20)
        Me.radioString.Name = "radioString"
        Me.radioString.Size = New System.Drawing.Size(192, 31)
        Me.radioString.TabIndex = 0
        Me.radioString.Text = "String"
        '
        'listTargetData
        '
        Me.listTargetData.FormattingEnabled = True
        Me.listTargetData.Location = New System.Drawing.Point(263, 280)
        Me.listTargetData.Name = "listTargetData"
        Me.listTargetData.Size = New System.Drawing.Size(242, 134)
        Me.listTargetData.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(12, 261)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(244, 16)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Data:"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(263, 261)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(242, 16)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Target Listbox:"
        '
        'cmdEmpty
        '
        Me.cmdEmpty.Location = New System.Drawing.Point(263, 211)
        Me.cmdEmpty.Name = "cmdEmpty"
        Me.cmdEmpty.Size = New System.Drawing.Size(118, 36)
        Me.cmdEmpty.TabIndex = 7
        Me.cmdEmpty.Text = "Empty"
        '
        'cmdLoad
        '
        Me.cmdLoad.Location = New System.Drawing.Point(262, 171)
        Me.cmdLoad.Name = "cmdLoad"
        Me.cmdLoad.Size = New System.Drawing.Size(118, 36)
        Me.cmdLoad.TabIndex = 0
        Me.cmdLoad.Text = "Load"
        '
        'cmdSort
        '
        Me.cmdSort.Location = New System.Drawing.Point(387, 171)
        Me.cmdSort.Name = "cmdSort"
        Me.cmdSort.Size = New System.Drawing.Size(118, 36)
        Me.cmdSort.TabIndex = 8
        Me.cmdSort.Text = "Sort"
        '
        'cmdReverse
        '
        Me.cmdReverse.Location = New System.Drawing.Point(387, 211)
        Me.cmdReverse.Name = "cmdReverse"
        Me.cmdReverse.Size = New System.Drawing.Size(118, 36)
        Me.cmdReverse.TabIndex = 9
        Me.cmdReverse.Text = "Reverse"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(517, 24)
        Me.MenuStrip1.TabIndex = 10
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(517, 426)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.cmdReverse)
        Me.Controls.Add(Me.cmdSort)
        Me.Controls.Add(Me.cmdEmpty)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.listTargetData)
        Me.Controls.Add(Me.groupDataType)
        Me.Controls.Add(Me.listSourceData)
        Me.Controls.Add(Me.groupDataStructure)
        Me.Controls.Add(Me.cmdLoad)
        Me.Name = "MainForm"
        Me.Text = "Generics Sample"
        Me.groupDataStructure.ResumeLayout(False)
        Me.groupDataType.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
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
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem


End Class
