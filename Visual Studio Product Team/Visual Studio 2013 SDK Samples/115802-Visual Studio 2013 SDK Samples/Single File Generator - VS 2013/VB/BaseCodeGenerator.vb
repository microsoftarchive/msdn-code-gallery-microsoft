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

Imports System.IO
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.GeneratorSample
	''' <summary>
	''' A managed wrapper for VS's concept of an IVsSingleFileGenerator which is
	''' a custom tool invoked at design time which can take any file as an input
	''' and provide any file as output.
	''' </summary>
	Public MustInherit Class BaseCodeGenerator
		Implements IVsSingleFileGenerator

        Private codeGeneratorProgress_local As IVsGeneratorProgress
        Private codeFileNameSpace As String = String.Empty
        Private codeFilePath As String = String.Empty

#Region "IVsSingleFileGenerator Members"

        ''' <summary>
        ''' Implements the IVsSingleFileGenerator.DefaultExtension method. 
        ''' Returns the extension of the generated file
        ''' </summary>
        ''' <param name="pbstrDefaultExtension">Out parameter, will hold the extension that is to be given to the output file name. The returned extension must include a leading period</param>
        ''' <returns>S_OK if successful, E_FAIL if not</returns>
        Private Function DefaultExtension(<System.Runtime.InteropServices.Out()> ByRef pbstrDefaultExtension As String) As Integer Implements IVsSingleFileGenerator.DefaultExtension
            Try
                pbstrDefaultExtension = GetDefaultExtension()
                Return VSConstants.S_OK
            Catch e As Exception
                Debug.WriteLine(Strings.GetDefaultExtensionFailed)
                Debug.WriteLine(e.ToString())
                pbstrDefaultExtension = String.Empty
                Return VSConstants.E_FAIL
            End Try
        End Function

        ''' <summary>
        ''' Implements the IVsSingleFileGenerator.Generate method.
        ''' Executes the transformation and returns the newly generated output file, whenever a custom tool is loaded, or the input file is saved
        ''' </summary>
        ''' <param name="wszInputFilePath">The full path of the input file. May be a null reference (Nothing in Visual Basic) in future releases of Visual Studio, so generators should not rely on this value</param>
        ''' <param name="bstrInputFileContents">The contents of the input file. This is either a UNICODE BSTR (if the input file is text) or a binary BSTR (if the input file is binary). If the input file is a text file, the project system automatically converts the BSTR to UNICODE</param>
        ''' <param name="wszDefaultNamespace">This parameter is meaningful only for custom tools that generate code. It represents the namespace into which the generated code will be placed. If the parameter is not a null reference (Nothing in Visual Basic) and not empty, the custom tool can use the following syntax to enclose the generated code</param>
        ''' <param name="rgbOutputFileContents">[out] Returns an array of bytes to be written to the generated file. You must include UNICODE or UTF-8 signature bytes in the returned byte array, as this is a raw stream. The memory for rgbOutputFileContents must be allocated using the .NET Framework call, System.Runtime.InteropServices.AllocCoTaskMem, or the equivalent Win32 system call, CoTaskMemAlloc. The project system is responsible for freeing this memory</param>
        ''' <param name="pcbOutput">[out] Returns the count of bytes in the rgbOutputFileContent array</param>
        ''' <param name="pGenerateProgress">A reference to the IVsGeneratorProgress interface through which the generator can report its progress to the project system</param>
        ''' <returns>If the method succeeds, it returns S_OK. If it fails, it returns E_FAIL</returns>
        Private Function Generate(ByVal wszInputFilePath As String, ByVal bstrInputFileContents As String,
                                  ByVal wszDefaultNamespace As String, ByVal rgbOutputFileContents() As IntPtr,
                                  <System.Runtime.InteropServices.Out()> ByRef pcbOutput As UInteger,
                                  ByVal pGenerateProgress As IVsGeneratorProgress) As Integer Implements IVsSingleFileGenerator.Generate
            If bstrInputFileContents Is Nothing Then
                Throw New ArgumentNullException(bstrInputFileContents)
            End If

            codeFilePath = wszInputFilePath
            codeFileNameSpace = wszDefaultNamespace
            codeGeneratorProgress_local = pGenerateProgress

            Dim bytes() As Byte = GenerateCode(bstrInputFileContents)

            If bytes Is Nothing Then
                ' This signals that GenerateCode() has failed. Tasklist items have been put up in GenerateCode()
                rgbOutputFileContents = Nothing
                pcbOutput = 0

                ' Return E_FAIL to inform Visual Studio that the generator has failed (so that no file gets generated)
                Return VSConstants.E_FAIL
            Else
                ' The contract between IVsSingleFileGenerator implementors and consumers is that 
                ' any output returned from IVsSingleFileGenerator.Generate() is returned through  
                ' memory allocated via CoTaskMemAlloc(). Therefore, we have to convert the 
                ' byte[] array returned from GenerateCode() into an unmanaged blob.  

                Dim outputLength = bytes.Length
                rgbOutputFileContents(0) = Marshal.AllocCoTaskMem(outputLength)
                Marshal.Copy(bytes, 0, rgbOutputFileContents(0), outputLength)
                pcbOutput = CUInt(outputLength)
                Return VSConstants.S_OK
            End If
        End Function

#End Region

        ''' <summary>
        ''' Namespace for the file
        ''' </summary>
        Protected ReadOnly Property FileNameSpace() As String
            Get
                Return codeFileNameSpace
            End Get
        End Property

        ''' <summary>
        ''' File-path for the input file
        ''' </summary>
        Protected ReadOnly Property InputFilePath() As String
            Get
                Return codeFilePath
            End Get
        End Property

        ''' <summary>
        ''' Interface to the VS shell object we use to tell our progress while we are generating
        ''' </summary>
        Friend ReadOnly Property CodeGeneratorProgress() As IVsGeneratorProgress
            Get
                Return codeGeneratorProgress_local
            End Get
        End Property

		''' <summary>
		''' Gets the default extension for this generator
		''' </summary>
		''' <returns>String with the default extension for this generator</returns>
		Protected MustOverride Function GetDefaultExtension() As String

		''' <summary>
		''' The method that does the actual work of generating code given the input file
		''' </summary>
		''' <param name="inputFileContent">File contents as a string</param>
		''' <returns>The generated code file as a byte-array</returns>
		Protected MustOverride Function GenerateCode(ByVal inputFileContent As String) As Byte()

		''' <summary>
		''' Method that will communicate an error via the shell callback mechanism
		''' </summary>
		''' <param name="level">Level or severity</param>
		''' <param name="message">Text displayed to the user</param>
		''' <param name="line">Line number of error</param>
		''' <param name="column">Column number of error</param>
        Protected Overridable Sub GeneratorError(ByVal level As UInteger, ByVal message As String,
                                                 ByVal line As UInteger, ByVal column As UInteger)
            Dim progress As IVsGeneratorProgress = CodeGeneratorProgress
            If progress IsNot Nothing Then
                progress.GeneratorError(0, level, message, line, column)
            End If
        End Sub

		''' <summary>
		''' Method that will communicate a warning via the shell callback mechanism
		''' </summary>
		''' <param name="level">Level or severity</param>
		''' <param name="message">Text displayed to the user</param>
		''' <param name="line">Line number of warning</param>
		''' <param name="column">Column number of warning</param>
        Protected Overridable Sub GeneratorWarning(ByVal level As UInteger, ByVal message As String,
                                                   ByVal line As UInteger, ByVal column As UInteger)
            Dim progress As IVsGeneratorProgress = CodeGeneratorProgress
            If progress IsNot Nothing Then
                progress.GeneratorError(1, level, message, line, column)
            End If
        End Sub
	End Class
End Namespace