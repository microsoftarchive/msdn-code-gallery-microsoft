' Copyright (c) Microsoft Corporation. All rights reserved.
Option Explicit On

Imports System.Xml
Imports System.IO
Imports System.Collections.Generic


Public Class MainForm

    ' Collection of XML Tasks
    Private taskList As List(Of Task)

    ' Form level variables for working with the DOM
    Private xmlDoc As XmlDocument
    Private xmlNode As XmlNode
    Private xmlNodeList As XmlNodeList

    ' Source XML Files located in the application bin folder
    Private modifyXmlFile As String = My.Application.Info.DirectoryPath & "\..\..\new.xml"
    Private simpleXmlFile As String = My.Application.Info.DirectoryPath & "\..\..\simple.xml"
    Private badXmlFile As String = My.Application.Info.DirectoryPath & "\..\..\bad.xml"

    ' Temp files used to save state between operations
    Private tempModifyXmlFile As String = My.Application.Info.DirectoryPath & "\temp_new.xml"
    Private tempSimpleXmlFile As String = My.Application.Info.DirectoryPath & "\temp_simple.xml"

    Private Const FindNodesXPath As String = "//Item[New]"
    Private Const FindNodeXPath As String = "//Department[@Name='Fruits']"

    Private formattedNewLine As String = vbCrLf & New String("="c, 35) & vbCrLf

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, MyBase.Load
        Dim fileNotFoundMsg As String = "The file '{0}' was not found.  Please place a copy in the same directory as the application Exe and restart the application."
        Dim filesNotFound As Boolean

        ' Verify we have the base XML files we need
        If My.Computer.FileSystem.FileExists(modifyXmlFile) = False Then
            MsgBox(String.Format(fileNotFoundMsg, modifyXmlFile), MsgBoxStyle.Critical, Me.Text)
            filesNotFound = True
        End If
        If My.Computer.FileSystem.FileExists(simpleXmlFile) = False Then
            MsgBox(String.Format(fileNotFoundMsg, simpleXmlFile), MsgBoxStyle.Critical, Me.Text)
            filesNotFound = True
        End If
        If My.Computer.FileSystem.FileExists(badXmlFile) = False Then
            MsgBox(String.Format(fileNotFoundMsg, badXmlFile), MsgBoxStyle.Critical, Me.Text)
            filesNotFound = True
        End If

        ' Populate the private task collection
        taskList = New List(Of Task)
        taskList.Add(New Task(My.Resources.cmdLoadXmlFile, AddressOf loadAndDisplayXML))
        taskList.Add(New Task(My.Resources.cmdLoadXmlString, AddressOf loadXmlString))
        taskList.Add(New Task(My.Resources.cmdTestForChildNodes, AddressOf testForChildNodes))
        taskList.Add(New Task(My.Resources.cmdIterateAllNodes, AddressOf iterateAllNodes))
        taskList.Add(New Task(My.Resources.cmdDetermineNodeType, AddressOf determineNodeType))
        taskList.Add(New Task(My.Resources.cmdListAllElementNodes, AddressOf listAllElementNodes))
        taskList.Add(New Task(My.Resources.cmdListElementsByTag, AddressOf listElementsByTag))
        taskList.Add(New Task(My.Resources.cmdSelectNodes, AddressOf selectNodes))
        taskList.Add(New Task(My.Resources.cmdSelectNode, AddressOf selectSingleNode))
        taskList.Add(New Task(My.Resources.cmdNavigateRelatedNodes, AddressOf navigateRelatedNodes))
        taskList.Add(New Task(My.Resources.cmdRetrieveAttributes, AddressOf retrieveAttributes))
        taskList.Add(New Task(My.Resources.cmdCreateXML, AddressOf createXML))
        taskList.Add(New Task(My.Resources.cmdAddOrDeleteElements, AddressOf addDeleteElements))
        taskList.Add(New Task(My.Resources.cmdAddOrDeleteAttributes, AddressOf addDeleteAttributes))
        taskList.Add(New Task(My.Resources.cmdModifyElement, AddressOf modifyElement))
        taskList.Add(New Task(My.Resources.cmdModifyAttribute, AddressOf modifyAttribute))
        taskList.Add(New Task(My.Resources.cmdValidXML, AddressOf validXML))
        taskList.Add(New Task(My.Resources.cmdParseErrors, AddressOf parseErrors))

        ' Load the list of tasks into the listbox
        xmlTasks.Items.Clear()
        For Each xmlTask As Task In taskList
            xmlTasks.Items.Add(xmlTask, False)
        Next

        ' If we don't have any files then disable the ListBox
        If filesNotFound Then
            xmlTasks.Enabled = False
        End If

        Me.StatusStrip1.Text = "Select an operation"
    End Sub

    Private Function loadXmlFile() As String
        ' Defer to more complex version
        Me.loadXmlFile(False)
        Return ""
    End Function

    Private Function loadXmlFile(ByVal ForceReload As Boolean) As String
        ' intialize XML Document
        ' Only re-load if ForceReload is true or
        ' the document is uninitialized.
        ' OrElse performs short-circut evaluation
        ' unlike Or which will execute both tests
        ' even if ForceReload is True
        If ForceReload OrElse (xmlDoc Is Nothing) Then
            xmlDoc = New XmlDocument
            xmlDoc.Load(simpleXmlFile)
        End If
        Return ""
    End Function

    Private Function loadAndDisplayXML() As String
        Me.loadXmlFile()
        Return xmlDoc.OuterXml()
    End Function

    Private Function loadXmlString() As String
        ' Build an arbitrary string with XML Markup.
        ' Note that the carriage returns are for humans.
        ' The XML processor doesn't need them.
        ' You can adjust whether or not the parser 'respects'
        ' whitespace by adjusting the PreserveWhitespace
        ' property of the XmlDocument class as shown below.

        ' Use the StringWriter class
        Dim writer As New StringWriter
        With writer
            .WriteLine("<xml>")
            .WriteLine("<Family>")
            .WriteLine("    <Person type='father'>Ben Smith</Person>")
            .WriteLine("    <Person type='mother'>Denise Smith</Person>")
            .WriteLine("    <Person type='child' gender='male'>Jeff Smith</Person>")
            .WriteLine("    <Person type='child' gender='female'>Samantha Smith</Person>")
            .WriteLine("</Family>")
            .WriteLine("</xml>")
        End With

        ' intialize a new local XML Document instance
        Dim xDoc As New XmlDocument
        ' Tell the parser to leave CRLFs in the document.
        xDoc.PreserveWhitespace = True
        xDoc.LoadXml(writer.ToString())
        writer.Close()

        Return xDoc.OuterXml()
    End Function

    Private Function testForChildNodes() As String
        ' intialize XML Document
        Me.loadXmlFile()
        Dim writer As New StringWriter

        Dim tab2x As String = vbTab & vbTab

        ' If the document has children, 
        ' look at them all.
        If xmlDoc.HasChildNodes Then
            For Each xmlNode In xmlDoc.ChildNodes
                With writer
                    .WriteLine(formattedNewLine)
                    .WriteLine("Name: {0}{1}", tab2x, xmlNode.Name)
                    .WriteLine("Type: {0}{1}", tab2x, xmlNode.NodeType)
                    .WriteLine("Type (String): {0}{1}", vbTab, xmlNode.NodeType.ToString())
                    .WriteLine("Value: {0}{1}", tab2x, xmlNode.Value)
                    .WriteLine("Outer XML: {0}{1}", vbTab, xmlNode.OuterXml)
                End With
            Next xmlNode
        End If
        Return writer.ToString
    End Function

    Private Function iterateAllNodes() As String
        ' Intialize XML Document.
        Me.loadXmlFile()
        Dim writer As New StringWriter

        ' Use a recursive function to visit all nodes.
        TraverseTree(writer, xmlDoc, 0)

        Return writer.ToString

    End Function

    Private Function determineNodeType() As String
        Me.loadXmlFile()

        ' Use a recursive function to visit all nodes.
        Dim writer As New StringWriter
        TraverseTreeType(writer, xmlDoc, 0)

        writer.Close()
        Return writer.ToString
    End Function

    Private Function listElementsByTag() As String
        ' Initialize XML Document.
        Me.loadXmlFile()

        Dim xElem As XmlElement
        Dim strTag As String
        Dim writer As New StringWriter

        strTag = InputBox("Enter an XPath expression to find:", "Enter Search Expression", FindNodesXPath)

        If strTag.Length > 0 Then
            ' Find a group of nodes based upon the enterd XPath expression.
            xmlNodeList = xmlDoc.SelectNodes(strTag)
            With writer
                .WriteLine("All text elements matching '{0}':", strTag)
                .WriteLine(formattedNewLine)

                If Not xmlNodeList Is Nothing Then
                    For Each xElem In xmlNodeList
                        .WriteLine("Name: " & xElem.Name)
                        .WriteLine("InnerText: " & xElem.InnerText)
                        .WriteLine("InnerXml: " & xElem.InnerXml)
                    Next xElem
                End If

            End With
        End If
        writer.Close()
        Return writer.ToString
    End Function

    Private Function listAllElementNodes() As String
        ' Intialize XML Document.
        Me.loadXmlFile()
        Dim writer As New StringWriter

        ' Search for elements by tag name.
        ' This example is hard-coded to find ALL (*) elements
        xmlNodeList = xmlDoc.GetElementsByTagName("*")

        With writer
            .WriteLine("Elements matching '*':")
            .WriteLine(formattedNewLine)
            For Each xmlNode In xmlNodeList
                .WriteLine(xmlNode.Name)
            Next xmlNode
        End With

        writer.Close()
        Return writer.ToString
    End Function

    Private Function selectNodes() As String
        ' Intialize XML Document.
        Me.loadXmlFile()

        Dim xElem As XmlElement
        Dim xnodTemp As XmlNode

        ' Print out the name of each new item:
        xmlNodeList = xmlDoc.SelectNodes("//New")

        Dim writer As New StringWriter
        If xmlNodeList IsNot Nothing Then
            With writer
                .WriteLine("All New Items:")
                .WriteLine(formattedNewLine)
                For Each xmlNode In xmlNodeList
                    xnodTemp = xmlNode.ParentNode.SelectSingleNode("Name")
                    If xnodTemp IsNot Nothing Then
                        If TypeOf xnodTemp Is XmlElement Then
                            xElem = CType(xnodTemp, XmlElement)
                            .WriteLine(xElem.InnerText)
                        End If
                    End If
                Next xmlNode
            End With
        End If
        writer.Close()
        Return writer.ToString
    End Function

    Private Function selectSingleNode() As String
        ' Intialize XML Document.
        Me.loadXmlFile()
        Dim writer As New StringWriter

        Dim xElem As XmlElement

        Dim strTag As String

        strTag = InputBox("Enter an XPath expression to find:", "Enter Search Expression", FindNodeXPath)
        If strTag.Length > 0 Then
            With writer
                .WriteLine("All elements matching '{0}':", strTag)
                .WriteLine(formattedNewLine)

                xmlNode = xmlDoc.SelectSingleNode(strTag)
                If xmlNode IsNot Nothing Then
                    If TypeOf xmlNode Is XmlElement Then
                        xElem = CType(xmlNode, XmlElement)
                        .WriteLine(xElem.InnerText)
                    End If
                End If
            End With
        End If
        writer.Close()
        Return writer.ToString
    End Function

    Private Function navigateRelatedNodes() As String
        ' Intialize XML Document.
        Me.loadXmlFile()

        Dim xElem As XmlElement
        Dim xnodTemp As XmlNode

        ' Print out the name of each new item:
        xmlNodeList = xmlDoc.SelectNodes("//New")

        Dim writer As New StringWriter
        If xmlNodeList IsNot Nothing Then
            With writer
                .WriteLine("All New Items:")
                .WriteLine(formattedNewLine)
                For Each xmlNode In xmlNodeList
                    xnodTemp = xmlNode.ParentNode.SelectSingleNode("Name")
                    If xnodTemp IsNot Nothing Then
                        If TypeOf xnodTemp Is XmlElement Then
                            xElem = CType(xnodTemp, XmlElement)
                            .WriteLine(xElem.InnerText)
                        End If
                    End If
                Next xmlNode
            End With
        End If
        writer.Close()
        Return writer.ToString
    End Function

    Private Function retrieveAttributes() As String
        ' intialize XML Document
        Me.loadXmlFile()
        Dim writer As New StringWriter

        Dim xAttr As XmlAttribute
        Dim xTmpNode As XmlNode

        With writer
            xmlNodeList = xmlDoc.SelectNodes("//Item")

            If xmlNodeList IsNot Nothing Then
                .WriteLine("All Item Attributes:")
                .WriteLine(formattedNewLine)

                For Each xmlNode In xmlNodeList
                    .WriteLine("{0} ({1})", xmlNode.Name, xmlNode.InnerText)
                    For Each xAttr In xmlNode.Attributes
                        .WriteLine("   {0}: {1}", xAttr.Name, xAttr.Value)
                    Next
                Next
            End If

            .WriteLine()
            .WriteLine()

            xmlNodeList = xmlDoc.SelectNodes("//Department")

            If xmlNodeList IsNot Nothing Then
                .WriteLine("Departments:")
                .WriteLine(formattedNewLine)

                For Each xmlNode In xmlNodeList
                    xTmpNode = xmlNode.Attributes.GetNamedItem("Name")
                    If xTmpNode IsNot Nothing Then
                        xAttr = CType(xTmpNode, XmlAttribute)
                        .WriteLine(xAttr.Value)
                    End If
                Next
            End If
        End With
        writer.Close()
        Return writer.ToString
    End Function

    Private Function createXML() As String
        ' This method shows how to build an XML file all from code.
        Dim xDoc As New XmlDocument

        Dim xPI As XmlProcessingInstruction
        Dim xComment As XmlComment
        Dim xElmntRoot As XmlElement
        Dim xElmntFamily As XmlElement

        xPI = xDoc.CreateProcessingInstruction("xml", "version='1.0'")
        xDoc.AppendChild(xPI)

        xComment = xDoc.CreateComment("Family Information")
        xDoc.AppendChild(xComment)

        xElmntRoot = xDoc.CreateElement("xml")
        xDoc.AppendChild(xElmntRoot)

        ' Rather than creating new nodes individually,
        ' count on the fact that AppendChild returns a reference
        ' to the newly added node.
        xElmntFamily = CType(xElmntRoot.AppendChild(xDoc.CreateElement("Family")), XmlElement)
        Call xElmntFamily.AppendChild(xDoc.CreateElement("Father"))

        ' Obviously this could fail if we don't have permission.
        ' Note that if the file exists, Save just overwrites.
        ' You might want to check for its existance like this:

        If My.Computer.FileSystem.FileExists(modifyXmlFile) Then
            If MsgBox(String.Format("Do you want to overwrite the file {0}?", modifyXmlFile), MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
                xDoc.Save(modifyXmlFile)
            End If
        End If

        xDoc.Save(tempModifyXmlFile)

        ' Note that the XML that is output is not 'pretty'.
        ' The parser doesn't introduce whitespace like 
        ' carriage returns, etc.
        Return xDoc.OuterXml
    End Function

    ''' <summary>
    ''' This method will remove a node and then add four new nodes.
    ''' </summary>
    ''' <returns>Modified XML</returns>
    Private Function addDeleteElements() As String
        Dim xDoc As New XmlDocument

        xDoc.Load(tempModifyXmlFile)

        Dim startXML As String = "Original XML:" & formattedNewLine & xDoc.OuterXml

        Dim xNode As XmlNode
        Dim xElmntFamily As XmlElement = Nothing
        ' Search for a particular node
        xNode = xDoc.SelectSingleNode("//Family")

        If xNode IsNot Nothing Then
            If TypeOf xNode Is XmlElement Then
                xElmntFamily = CType(xNode, XmlElement)
            End If

            xElmntFamily.RemoveChild(xElmntFamily.SelectSingleNode("Father"))

            ' Insert node for each family member:
            InsertTextNode(xDoc, xElmntFamily, "Person", "Ben Smith")
            ' Here's what InsertTextNode does:
            'Set xNode = xDoc.createElement("father")
            'xNode.appendChild xDoc.createTextNode("Ben Smith")
            'xElmntFamily.appendChild xNode
            InsertTextNode(xDoc, xElmntFamily, "Person", "Denise Smith")
            InsertTextNode(xDoc, xElmntFamily, "Person", "Jeff Smith")
            InsertTextNode(xDoc, xElmntFamily, "Person", "Samantha Smith")

            xDoc.Save(tempModifyXmlFile)

            ' Display the modified XML
            Me.xmlEdit.Text = "Modified XML:" & formattedNewLine & xDoc.OuterXml
            Return startXML
        Else
            Return String.Format("Family Node was not found. Please try the '{0}' option first.", My.Resources.cmdCreateXML)
        End If

    End Function

    ''' <summary>
    ''' This method will remove all child nodes of the Family 
    ''' node and then re-add them with some attributes.
    ''' It also shows how to manipulate existing attributes.
    ''' </summary>
    ''' <returns>Modified XML string</returns>
    Private Function addDeleteAttributes() As String
        ' This method will remove all child nodes of the Family 
        ' node and then re-add them with some attributes.
        ' It also shows how to manipulate existing attributes.
        Dim xDoc As New XmlDocument

        xDoc.Load(tempModifyXmlFile)

        Dim startXml As String = "Original XML:" & formattedNewLine & xDoc.OuterXml

        Dim xNode As XmlNode
        Dim xElem As XmlElement

        Dim xElmntFamily As XmlElement = Nothing

        ' Search for a particular node.
        xNode = xDoc.SelectSingleNode("//Family")

        If xNode IsNot Nothing Then
            If TypeOf xNode Is XmlElement Then
                xElmntFamily = CType(xNode, XmlElement)
            End If

            ' Delete all the nodes created in the previous answer.
            For Each xNode In xElmntFamily
                xElmntFamily.RemoveChild(xNode)
            Next

            ' Insert node for each family member:
            xElem = InsertTextNode(xDoc, xElmntFamily, "Person", "Ben Smith")
            xElem.SetAttribute("type", "parent")
            xElem.SetAttribute("age", "70")

            xElem = InsertTextNode(xDoc, xElmntFamily, "Person", "Denise Smith")
            xElem.SetAttribute("type", "mother")

            xElem = InsertTextNode(xDoc, xElmntFamily, "Person", "Jeff Smith")
            xElem.SetAttribute("type", "son")

            xElem = InsertTextNode(xDoc, xElmntFamily, "Person", "Samantha Smith")
            xElem.SetAttribute("type", "daughter")

            ' Attributes aren't quite right. Fix up father's.
            xNode = xDoc.SelectSingleNode("//Person[@type='parent']")
            If xNode IsNot Nothing Then
                ' Remove "age" attribute, and change "type" attribute
                ' to "father".
                xElem = CType(xNode, XmlElement)
                xElem.Attributes.RemoveNamedItem("age")
                xElem.SetAttribute("type", "father")
            End If

            xDoc.Save(tempModifyXmlFile)
            Me.xmlEdit.Text = "Modified XML: " & formattedNewLine & xDoc.OuterXml

            Return startXml
        Else
            Return String.Format("Family Node was not found. Please try the '{0}' option first.", My.Resources.cmdCreateXML)
        End If
    End Function

    Private Function modifyElement() As String
        ' Shows how to modify an element's text value
        Dim xDoc As New XmlDocument

        xDoc.Load(tempModifyXmlFile)

        Dim startxml As String = "Original XML:" & formattedNewLine & xDoc.OuterXml

        Dim xNode As XmlNode
        Dim xElem As XmlElement

        Dim xElmntFamily As XmlElement = Nothing
        xNode = xDoc.SelectSingleNode("//Person")

        If Not (xNode Is Nothing) Then
            xElem = CType(xNode, XmlElement)

            ' Change "Ben Smith" to "John Smith"
            xElem.InnerText = "John Smith"

            xDoc.Save(tempModifyXmlFile)
            Me.XmlEdit.Text = "Modified XML:" & formattedNewLine & xDoc.OuterXml
            Return startxml
        Else
            Return String.Format("Family Node was not found. Please try the '{0}' option first.", My.Resources.cmdCreateXML)
        End If
    End Function

    Private Function modifyAttribute() As String
        ' Shows how to modify existing attribute values
        Dim xDoc As New XmlDocument

        xDoc.Load(tempModifyXmlFile)

        Dim startXml As String = "Original XML:" & formattedNewLine & xDoc.OuterXml

        Dim xNode As XmlNode
        Dim xElem As XmlElement

        For Each xNode In xDoc.SelectNodes("//Person")
            xElem = CType(xNode, XmlElement)

            Select Case xElem.GetAttribute("type")
                Case "father"
                    xElem.SetAttribute("type", "parent")
                    xElem.SetAttribute("gender", "male")
                Case "mother"
                    xElem.SetAttribute("type", "parent")
                    xElem.SetAttribute("gender", "female")
                Case "son"
                    xElem.SetAttribute("type", "child")
                    xElem.SetAttribute("gender", "male")
                Case "daughter"
                    xElem.SetAttribute("type", "child")
                    xElem.SetAttribute("gender", "female")
            End Select
        Next
        xDoc.Save(tempModifyXmlFile)

        Me.XmlEdit.Text = "Modified XML:" & formattedNewLine & xDoc.OuterXml
        Return startXml
    End Function

    Private Function validXML() As String
        Dim xDoc As New XmlDocument
        Dim result As String

        Try
            ' If the XML file (or string for that matter) is invalid, an exception 
            ' of the type XmlException will be thrown.
            xDoc.Load(badXmlFile)

            ' You should not see this if using the 'bad' file.
            result = "Valid XML File: " & badXmlFile
        Catch exp As XmlException
            result = "Invalid XML File: " & badXmlFile
        Catch exp As Exception
            ' Catch any other exceptions.
            MsgBox(exp.Message, MsgBoxStyle.Critical, exp.Source)
            result = "Exception occured."
        End Try
        Return result
    End Function

    Private Function parseErrors() As String
        Dim xDoc As New XmlDocument
        Dim writer As New StringWriter

        Try
            ' If the XML file (or string for that matter)
            ' is invalid, an exception of the type
            ' XmlException will be thrown
            xDoc.Load(badXmlFile)

            ' You should not see this if using our 'bad' file.
            writer.Write("Valid XML File: " & badXmlFile)
        Catch exp As XmlException
            With writer
                .WriteLine(exp.Message)
                ' You can get these items individually.
                .WriteLine(exp.LineNumber)
                .WriteLine(exp.LinePosition)

                ' Other items are buried in the args array
                ' Take a look in break mode.
            End With
        Catch exp As Exception
            ' Just in case.
            MsgBox(exp.Message, MsgBoxStyle.Critical, exp.Source)
        End Try
        Return writer.ToString

    End Function

    Private Function InsertTextNode(ByVal xDoc As XmlDocument, ByVal xNode As XmlNode, ByVal strTag As String, ByVal strText As String) As XmlElement
        ' Insert a text node as a child of xNode.
        ' Set the tag to be strTag, and the
        ' text to be strText. Return a pointer
        ' to the new node.

        Dim xNodeTemp As XmlNode

        xNodeTemp = xDoc.CreateElement(strTag)
        xNodeTemp.AppendChild(xDoc.CreateTextNode(strText))
        xNode.AppendChild(xNodeTemp)

        Return CType(xNodeTemp, XmlElement)
    End Function

    Private Sub TraverseTree(ByVal sw As StringWriter, ByVal xNode As XmlNode, ByVal intLevel As Integer)
        Dim xNodeLoop As XmlNode

        ' Print out the node name for the
        ' current node.
        Dim s As New String(System.Convert.ToChar(vbTab), intLevel)
        sw.WriteLine(s & xNode.Name)

        ' If the current node has children, call this
        ' same procedure, passing in that node. Increase
        ' the lngLevel value, so the output can be indented.
        If xNode.HasChildNodes Then
            For Each xNodeLoop In xNode.ChildNodes
                Call TraverseTree(sw, xNodeLoop, intLevel + 1)
            Next xNodeLoop
        End If
    End Sub

    Private Sub TraverseTreeType(ByVal sw As StringWriter, ByVal xNode As XmlNode, ByVal intLevel As Integer)
        Dim xNodeLoop As XmlNode

        ' Print out the node name for the
        ' current node.
        Dim s As New String(System.Convert.ToChar(vbTab), intLevel)
        Dim strValues() As String = {s, xNode.Name, xNode.NodeType.ToString()}
        sw.WriteLine("{0}{1} ({2})", strValues)

        ' If the current node has children, call this
        ' same procedure, passing in that node. Increase
        ' the lngLevel value, so the output can be indented.
        If xNode.HasChildNodes Then
            For Each xNodeLoop In xNode.ChildNodes
                Call TraverseTreeType(sw, xNodeLoop, intLevel + 1)
            Next xNodeLoop
        End If
    End Sub

    Private Sub XmlTasks_ItemCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles XmlTasks.ItemCheck
        ' The check box next to a command has been changed to checked, run the command.
        If e.NewValue = CheckState.Checked Then
            Me.XmlEdit.Text = String.Empty
            Me.XmlDisplay.Text = String.Empty

            ' Execute the task associated with the checked item.
            Dim theTask As Task = CType(Me.XmlTasks.Items(e.Index), Task)
            Dim result As String = theTask.Method.Invoke()

            ' Update the status bar with the last run command
            Me.StatusStrip1.Text = "Last run command: " & theTask.Name
            Me.XmlDisplay.Text = result
        End If
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub resetDisplayMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles resetDisplayMenuItem.Click
        ' Reset the check boxes that are checked to be not checked.
        If Me.xmlTasks.CheckedItems.Count > 0 Then
            Me.xmlEdit.Text = String.Empty
            Me.xmlDisplay.Text = String.Empty
            For item As Integer = 0 To Me.xmlTasks.Items.Count - 1
                Me.xmlTasks.SetItemCheckState(item, CheckState.Unchecked)
            Next
        End If
    End Sub
End Class


