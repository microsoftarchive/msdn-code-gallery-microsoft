' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.Serialization.Formatters.Soap

' In this example, objects will be serialized and deserialized to and from a file. 
' If using the Soap formatter, the file will be called SoapFile.xml. If using 
' the Binary formatter, the file will be called BinaryFile.dat.

Public Class Form1

    ' These variables are initialized in the Form_Load event.
    Private soapFile As String = "SoapFile.xml"
    Private binaryFile As String = "BinaryFile.dat"
    Private customFile As String = "CustomFile.xml"
    Private soapPath As String
    Private binaryPath As String
    Private customPath As String

#Region "Load"
    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load, MyBase.Load
        ' Get the system temp path using the Path class
        ' in the System.Io namespace
        Dim tempPath As String = My.Computer.FileSystem.SpecialDirectories.Temp

        soapPath = tempPath & soapFile
        binaryPath = tempPath & binaryFile
        customPath = tempPath & customFile

        StatusStrip1.Text = "All files will be written to " & tempPath & "."
    End Sub
#End Region

#Region "SOAP"
    ' This routine creates a new instance of SerializableClass, then serializes it
    ' with the SOAP Formatter.
    Private Sub cmdStandardSerializationSoap_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles cmdStandardSerializationSoap.Click

        ' Create the object to be serialized.
        Dim instance As New SerializableClass(CInt(txtX.Text), CInt(txtY.Text), CInt(txtZ.Text))

        ' Get a filestream that writes to the file.
        Dim fs As New FileStream(soapPath, FileMode.OpenOrCreate)

        ' Get a SOAP Formatter instance.
        Dim sf As New SoapFormatter()

        ' Serialize the instance.
        sf.Serialize(fs, instance)

        ' Close the file and release resources.
        fs.Close()

        ' Deserialization is now available.
        cmdStandardDeserializationSoap.Enabled = True
        cmdViewClass1.Enabled = True
    End Sub

    ' This routine deserializes an object from the file
    ' and assigns it to a SerializableClass reference.
    Private Sub cmdStandardDeserializationSoap_Click(ByVal sender As System.Object, _
         ByVal e As System.EventArgs) Handles cmdStandardDeserializationSoap.Click

        ' Declare the reference that will point to the object to be deserialized.
        Dim instance As SerializableClass

        ' Get a filestream that reads from the file.
        Dim fs As New FileStream(soapPath, FileMode.Open)

        ' Get a SOAP Formatter instance.
        Dim sf As New SoapFormatter()

        ' Deserialize from the file, creating an instance of SerializableClass.
        ' The deserialized object must be cast to the proper type.
        instance = CType(sf.Deserialize(fs), SerializableClass)

        ' Close the file and release resources.
        fs.Close()

        ' Put the deserialized values for the fields into the textboxes.
        txtXAfter.Text = CStr(instance.PublicVariable)
        txtYAfter.Text = CStr(instance.PublicProperty)
        txtZAfter.Text = CStr(instance.NonSerializedVariable)

        ' Reset buttons after deserializing.
        cmdStandardDeserializationSoap.Enabled = False
        cmdViewClass1.Enabled = False
    End Sub
#End Region

#Region "Binary"
    ' This routine creates a new instance of SerializableClass, then serializes it 
    ' to a file using the Binary formatter.
    Private Sub cmdStandardSerializationBinary_Click(ByVal sender As System.Object, _
         ByVal e As System.EventArgs) Handles cmdStandardSerializationBinary.Click

        ' Create the object to be serialized.
        Dim instance As New SerializableClass(CInt(txtX.Text), CInt(txtY.Text), CInt(txtZ.Text))

        ' Get a filestream that writes to the file.
        Dim fs As New FileStream(binaryPath, FileMode.OpenOrCreate)

        'Get a Binary Formatter instance
        Dim bf As New BinaryFormatter()

        ' Serialize the instance to the file.
        bf.Serialize(fs, instance)

        ' Close the file and release resources.
        fs.Close()

        ' Deserialization is now available.
        cmdStandardDeserializationBinary.Enabled = True
    End Sub

    ' This routine deserializes an object from a file
    ' and assigns it to a SerializableClass reference.
    Private Sub cmdStandardDeserializationBinary_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles cmdStandardDeserializationBinary.Click

        ' Declare the reference that will point to the object to be deserialized.
        Dim instance As SerializableClass

        ' Get a filestream that reads from the file.
        Dim fs As New FileStream(binaryPath, FileMode.Open)

        ' Get a Binary Formatter instance.
        Dim bf As New BinaryFormatter()

        ' Deserialize an instance from the file.
        ' The deserialized object must be cast to the proper type.
        instance = CType(bf.Deserialize(fs), SerializableClass)

        ' Close the file and release resources.
        fs.Close()

        ' Put the deserialized values for the fields into the textboxes.
        txtXAfter.Text = CStr(instance.PublicVariable)
        txtYAfter.Text = CStr(instance.PublicProperty)
        txtZAfter.Text = CStr(instance.NonSerializedVariable)

        ' Reset button after deserializing.
        cmdStandardDeserializationBinary.Enabled = False
    End Sub
#End Region

#Region "Custom"
    ' This routine creates a new instance of CustomSerializableClass, 
    ' then serializes it to a file with the SOAP Formatter. 
    ' Even though CustomSerializableClass has custom serialization, 
    ' the client code is identical to that of standard serialization. 
    ' The difference is in the class code, not the client code.
    Private Sub cmdCustomSerialization_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles cmdCustomSerialization.Click

        ' Create the object to be serialized.
        Dim instance As New CustomSerializableClass(CInt(txtX.Text), CInt(txtY.Text), CInt(txtZ.Text))

        ' Get a filestream that writes to the file.
        Dim fs As New FileStream(customPath, FileMode.OpenOrCreate)

        ' Get a SOAP Formatter instance.
        Dim sf As New SoapFormatter()

        ' Serialize the instance.
        sf.Serialize(fs, instance)

        ' Close the file and release resources.
        fs.Close()

        ' Deserialization is now available.
        cmdCustomDeserialization.Enabled = True
        cmdViewClass2.Enabled = True
    End Sub

    ' This routine deserializes an object from a file 
    ' and assigns it to a CustomSerializableClass reference. Even though
    ' CustomSerializableClass has custom serialization, the client code is identical to that
    ' of standard serialization. The difference is in the class code, not the
    ' client code.
    Private Sub cmdCustomDeserialization_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles cmdCustomDeserialization.Click

        ' Declare the reference that will point to the object to be deserialized
        Dim instance As CustomSerializableClass

        ' Get a filestream that reads from the file.
        Dim fs As New FileStream(customPath, FileMode.Open)

        ' Get a SOAP Formatter instance.
        Dim sf As New SoapFormatter()

        ' Deserialize and create an instance.
        ' The deserialized object must be cast to the proper type.
        instance = CType(sf.Deserialize(fs), CustomSerializableClass)

        ' Close the file and release resources.
        fs.Close()

        ' Put the deserialized values for the fields into the textboxes.
        txtXAfter.Text = CStr(instance.PublicVariable)
        txtYAfter.Text = CStr(instance.PublicProperty)
        txtZAfter.Text = CStr(instance.NonSerializedVariable)

        ' Reset button after deserializing.
        cmdCustomDeserialization.Enabled = False
        cmdViewClass2.Enabled = False
    End Sub
#End Region

#Region "Views and Validation"
    ' Dump the file contents into a textbox. This routine quickly copies the file
    ' contents into a read-only textbox. It lets the user examine the 
    ' serialized object state of SerializableClass.
    Private Sub cmdViewClass1_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles cmdViewClass1.Click

        Dim fs As New FileStream(soapPath, FileMode.Open)
        Dim sr As New StreamReader(fs)
        txtView.Text = sr.ReadToEnd()
        sr.Close()
        fs.Close()
    End Sub

    ' Dump the file contents into a textbox. This routine quickly copies the file
    ' contents into a read-only textbox. It merely let's the user examine the 
    ' serialized object state of CustomSerializableClass.
    Private Sub cmdViewClass2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdViewClass2.Click

        Dim fs As New FileStream(customPath, FileMode.Open)
        Dim sr As New StreamReader(fs)
        txtView.Text = sr.ReadToEnd()
        sr.Close()
        fs.Close()
    End Sub

    Private Function IsValidInt32(ByVal data As String) As Boolean
        ' This is an alternative to IsNumeric that works for Int32 data only.
        Try
            Dim i As Integer = System.Convert.ToInt32(data)
            Return True
        Catch exp As Exception
            Return False
        End Try
    End Function

    Private Sub ValidatingTextIsInt32(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtZ.Validating, txtY.Validating, txtX.Validating
        ' Makre sure the value entered can be converted to an Int32 (Integer)
        Dim textBox As MaskedTextBox = CType(sender, MaskedTextBox)

        If Not IsValidInt32(textBox.Text) Then
            Dim strMsg As String
            strMsg = String.Format("The value you entered {0} is not a valid 32-bit integer. Value will be changed to zero.", textBox.Text)
            MsgBox(strMsg, MsgBoxStyle.Exclamation, "Validation Warning")
            textBox.Text = "0"
        End If
    End Sub
#End Region


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
