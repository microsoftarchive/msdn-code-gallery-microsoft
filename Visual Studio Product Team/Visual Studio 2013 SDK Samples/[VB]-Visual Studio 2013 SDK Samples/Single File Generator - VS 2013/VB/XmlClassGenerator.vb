'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Runtime.InteropServices
Imports System.CodeDom.Compiler
Imports System.CodeDom
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Schema
Imports Microsoft.Win32
Imports Microsoft.VisualStudio.Shell
Imports VSLangProj80

Namespace Microsoft.Samples.VisualStudio.GeneratorSample
	''' <summary>
	''' This is the generator class. 
	''' When setting the 'Custom Tool' property of a C#, VB, or J# project item to "XmlClassGenerator", 
	''' the GenerateCode function will get called and will return the contents of the generated file 
	''' to the project system
	''' </summary>
	<ComVisible(True), Guid("52B316AA-1997-4c81-9969-83604C09EEB4"), 
	CodeGeneratorRegistration(GetType(XmlClassGenerator),"C# XML Class Generator",
	    vsContextGuids.vsContextGuidVCSProject,GeneratesDesignTimeSource:=True), 
	CodeGeneratorRegistration(GetType(XmlClassGenerator), "VB XML Class Generator", 
	    vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource := True), 
	CodeGeneratorRegistration(GetType(XmlClassGenerator), "J# XML Class Generator", 
	    vsContextGuids.vsContextGuidVJSProject, GeneratesDesignTimeSource := True), 
	ProvideObject(GetType(XmlClassGenerator))>
	Public Class XmlClassGenerator
		Inherits BaseCodeGeneratorWithSite


		'The name of this generator (use for 'Custom Tool' property of project item)
		Friend Shared name As String = "XmlClassGenerator"


		Friend Shared validXML As Boolean

		''' <summary>
		''' Function that builds the contents of the generated file based on the contents of the input file
		''' </summary>
		''' <param name="inputFileContent">Content of the input file</param>
		''' <returns>Generated file as a byte array</returns>
		Protected Overrides Function GenerateCode(ByVal inputFileContent As String) As Byte()
			'Validate the XML file against the schema
			Using contentReader As New StringReader(inputFileContent)
				VerifyDocumentSchema(contentReader)
			End Using

			If Not validXML Then
				'Returning null signifies that generation has failed
				Return Nothing
			End If

			Dim provider As CodeDomProvider = GetCodeProvider()

			Try
				'Load the XML file
				Dim doc As New XmlDocument()
				'We have already validated the XML. No XmlException can be thrown here
				doc.LoadXml(inputFileContent)

				'Create the CodeCompileUnit from the passed-in XML file
				Dim compileUnit As CodeCompileUnit = SourceCodeGenerator.CreateCodeCompileUnit(doc, FileNameSpace)

				If Me.CodeGeneratorProgress IsNot Nothing Then
					'Report that we are 1/2 done
					Me.CodeGeneratorProgress.Progress(50, 100)
				End If

				Using writer As New StringWriter(New StringBuilder())
					Dim options As New CodeGeneratorOptions()
					options.BlankLinesBetweenMembers = False
					options.BracingStyle = "C"

					'Generate the code
					provider.GenerateCodeFromCompileUnit(compileUnit, writer, options)

					If Me.CodeGeneratorProgress IsNot Nothing Then
						'Report that we are done
						Me.CodeGeneratorProgress.Progress(100, 100)
					End If
					writer.Flush()

					'Get the Encoding used by the writer. We're getting the WindowsCodePage encoding, 
					'which may not work with all languages
					Dim enc As Encoding = Encoding.GetEncoding(writer.Encoding.WindowsCodePage)

					'Get the preamble (byte-order mark) for our encoding
                    Dim preamble() = enc.GetPreamble()
                    Dim preambleLength = preamble.Length

					'Convert the writer contents to a byte array
                    Dim body() = enc.GetBytes(writer.ToString())

					'Prepend the preamble to body (store result in resized preamble array)
					Array.Resize(Of Byte)(preamble, preambleLength + body.Length)
					Array.Copy(body, 0, preamble, preambleLength, body.Length)

					'Return the combined byte array
					Return preamble
				End Using
			Catch e As Exception
				Me.GeneratorError(4, e.ToString(), 1, 1)
				'Returning null signifies that generation has failed
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Verify the XML document in contentReader against the schema in XMLClassGeneratorSchema.xsd
		''' </summary>
		''' <param name="contentReader">TextReader for XML document</param>
		Private Sub VerifyDocumentSchema(ByVal contentReader As TextReader)
			' Options for XmlReader object can be set only in constructor. After the object is created, 
			' they become read-only. Because of that we need to create XmlSettings structure, 
			' fill it in with correct parameters and pass into XmlReader constructor.
			Dim validatorSettings As New XmlReaderSettings()
			validatorSettings.ValidationType = ValidationType.Schema
			validatorSettings.XmlResolver = Nothing
			AddHandler validatorSettings.ValidationEventHandler, AddressOf OnSchemaValidationError

			'Schema is embedded in this assembly. Get its stream
			Dim schema As Stream = Me.GetType().Assembly.GetManifestResourceStream("XmlClassGeneratorSchema.xsd")

			Using schemaReader As New XmlTextReader(schema)
				Try
					validatorSettings.Schemas.Add("http://microsoft.com/XMLClassGeneratorSchema.xsd", schemaReader)
				' handle errors in the schema itself
				Catch e As XmlException
					Me.GeneratorError(4, "InvalidSchemaFileEmbeddedInGenerator " & e.ToString(), 1, 1)
					validXML = False
					Return
				' handle errors in the schema itself
				Catch e As XmlSchemaException
					Me.GeneratorError(4, "InvalidSchemaFileEmbeddedInGenerator " & e.ToString(), 1, 1)
					validXML = False
					Return
				End Try

				Using validator As XmlReader = XmlReader.Create(contentReader, validatorSettings, InputFilePath)
					validXML = True
                    Try
                        Do While validator.Read()
                            'empty body
                        Loop
                    Catch e As XmlException
                        Me.GeneratorError(4, "InvalidXmlContentEmbeddedInGenerator " & e.ToString(), 1, 1)
                        validXML = False
                        Return
                    End Try
                End Using
			End Using
		End Sub

		''' <summary>
		''' Receives any errors that occur while validating the documents's schema.
		''' </summary>
		''' <param name="sender">Sender object</param>
		''' <param name="args">Details about the validation error that has occurred</param>
		Private Sub OnSchemaValidationError(ByVal sender As Object, ByVal args As ValidationEventArgs)
			'signal that validation of document against schema has failed
			validXML = False

			'Report the error (so that it is shown in the error list)
            Me.GeneratorError(4, args.Exception.Message, CUInt(CUInt(args.Exception.LineNumber) - 1), CUInt(CUInt(args.Exception.LinePosition) - 1))
		End Sub
	End Class
End Namespace