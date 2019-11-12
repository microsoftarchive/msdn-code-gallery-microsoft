'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio
Imports Microsoft.Build.Evaluation
Imports System.Reflection
Imports Microsoft.VisualStudio.Shell
Imports System.ComponentModel.Design
Imports Microsoft.Build.Construction
Imports System.Linq

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class Utilities
        Private Sub New()
        End Sub
        Public Shared Function HasFunctionThrown(Of ExceptionType As Exception)(ByVal func As Action) As Boolean
            Dim hasThrown As Boolean = False
            Try
                func()
            Catch e1 As ExceptionType
                hasThrown = True
            Catch e As TargetInvocationException
                hasThrown = (TypeOf e.InnerException Is ExceptionType)
            End Try
            Return hasThrown
        End Function

        Private Shared _tempFiles As New List(Of String)()

        Public Shared Sub CleanUpTempFiles()
            For Each file As String In _tempFiles
                Try
                    System.IO.File.Delete(file)
                Catch
                End Try
            Next file
            _tempFiles.Clear()
        End Sub

        Public Shared Function CreateTempFile(ByVal content As String, ByVal encoding As Encoding, ByVal extension As String) As String
            Dim path As String = System.IO.Path.GetTempFileName()
            If extension IsNot Nothing Then
                path = System.IO.Path.ChangeExtension(path, ".txt")
            End If
            File.WriteAllText(path, content, encoding)
            _tempFiles.Add(path)
            Return path
        End Function

        Public Shared Function CreateTempTxtFile(ByVal content As String, ByVal encoding As Encoding) As String
            Return CreateTempFile(content, encoding, ".txt")
        End Function

        Public Shared Function CreateTempTxtFile(ByVal content As String) As String
            Return CreateTempTxtFile(content, Encoding.Unicode)
        End Function

        Public Shared Function CreateTempFile(ByVal content As String) As String
            Return CreateTempFile(content, Encoding.Unicode, Nothing)
        End Function

        Public Shared Function CreateBigFile() As String
            Dim content As New StringBuilder()

            For i As Integer = 0 To 999999
                content.Append("abcd ")
            Next i

            Return CreateTempFile(content.ToString())
        End Function

        Public Shared Function ListFromEnum(Of T)(ByVal enumerator As IEnumerable(Of T)) As List(Of T)
            Dim result As New List(Of T)()

            For Each ti As T In enumerator
                result.Add(ti)
            Next ti

            Return result
        End Function

        Public Shared Function TasksFromEnumerator(ByVal enumerator As IVsEnumTaskItems) As List(Of IVsTaskItem)
            Dim result As New List(Of IVsTaskItem)()

            Dim items() As IVsTaskItem = {Nothing}
            Dim fetched() As UInteger = {0}

            enumerator.Reset()
            Do While ((enumerator.Next(1, items, fetched) = VSConstants.S_OK) And (fetched(0) = 1))
                result.Add(items(0))
            Loop
            Return result
        End Function

        Public Shared Function TasksOfProvider(ByVal provider As IVsTaskProvider) As List(Of IVsTaskItem)
            Dim enumerator As IVsEnumTaskItems = Nothing
            provider.EnumTaskItems(enumerator)
            Return TasksFromEnumerator(enumerator)
        End Function

        Public Shared Function CreateTermTable(ByVal terms As IEnumerable(Of String)) As String
            Dim fileContent As New StringBuilder()

            fileContent.Append("<?xml version=""1.0""?>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("<xmldata>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("  <PLCKTT>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("    <Lang>" & Microsoft.VisualBasic.Constants.vbLf)

            For Each term As String In terms
                fileContent.Append("      <Term Term=""" & term & """ Severity=""2"" TermClass=""Geopolitical"">" & Microsoft.VisualBasic.Constants.vbLf)
                fileContent.Append("        <Comment>For detailed info see - http://relevant-url-here.com</Comment>" & Microsoft.VisualBasic.Constants.vbLf)
                fileContent.Append("      </Term>" & Microsoft.VisualBasic.Constants.vbLf)
            Next term

            fileContent.Append("    </Lang>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("  </PLCKTT>" & Microsoft.VisualBasic.Constants.vbLf)
            fileContent.Append("</xmldata>" & Microsoft.VisualBasic.Constants.vbLf)

            Return CreateTempTxtFile(fileContent.ToString())
        End Function

        Public Shared Function SetupMSBuildProject() As Project
            Dim project As New Project()
            project.FullPath = Path.GetTempFileName()
            Return project
        End Function
        Private NotInheritable Class AnonymousClass16
            Public project As Project = SetupMSBuildProject()
            Public projectFolder As String = Path.GetDirectoryName(project.FullPath)
            Public Function AnonymousMethod(ByVal rootedPath As String) As String
                Return CodeSweep.Utilities.RelativePathFromAbsolute(rootedPath, projectFolder)
            End Function

        End Class
        Public Shared Function SetupMSBuildProject(ByVal filesToScan As IList(Of String), ByVal termTableFiles As IList(Of String)) As Project
            Dim locals As New AnonymousClass16()

            If filesToScan.Count > 0 Then
                locals.project.AddItem("ItemGroup1", CodeSweep.Utilities.RelativePathFromAbsolute(filesToScan(0), locals.projectFolder))
                If filesToScan.Count > 1 Then
                    For i As Integer = 1 To filesToScan.Count - 1
                        locals.project.AddItem("ItemGroup2", CodeSweep.Utilities.RelativePathFromAbsolute(filesToScan(i), locals.projectFolder))
                    Next i
                End If
            End If

            Dim rootedTermTables As New List(Of String)(termTableFiles)
            Dim relativeTermTables As List(Of String) = rootedTermTables.ConvertAll(Of String)(AddressOf locals.AnonymousMethod)

            Dim target As ProjectTargetElement = locals.project.Xml.AddTarget("AfterBuild")

            Dim newTask As ProjectTaskElement = target.AddTask("ScannerTask")
            newTask.Condition = "'$(RunCodeSweepAfterBuild)' == 'true'"
            newTask.ContinueOnError = "false"
            newTask.SetParameter("FilesToScan", "@(ItemGroup1);@(ItemGroup2)")
            newTask.SetParameter("TermTables", CodeSweep.Utilities.Concatenate(relativeTermTables, ";"))
            newTask.SetParameter("Project", "$(MSBuildProjectFullPath)")

            Dim usingTaskAssembly As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetLoadedModules()(0).FullyQualifiedName) & "\BuildTask.dll"
            locals.project.Xml.AddUsingTask("CodeSweep.BuildTask.ScannerTask", usingTaskAssembly, Nothing)

            Dim group As ProjectPropertyGroupElement = locals.project.Xml.AddPropertyGroup()
            group.AddProperty("RunCodeSweepAfterBuild", "true")

            locals.project.ReevaluateIfNecessary()
            Return locals.project
        End Function

        Friend Shared Function GetTargetsPath() As String
            Return CodeSweep.Utilities.EncodeProgramFilesVar(Path.GetDirectoryName(GetType(CodeSweep.BuildTask.ScannerTask).Module.FullyQualifiedName) & "\CodeSweep.targets")
        End Function

        Friend Shared Sub CopyTargetsFileToBinDir()
            Dim binDir As String = Path.GetDirectoryName(GetType(CodeSweep.BuildTask.ScannerTask).Module.FullyQualifiedName)
            Dim targetName As String = "CodeSweep.targets"
            Dim destinationPath As String = Path.Combine(binDir, targetName)
            If (Not File.Exists(destinationPath)) Then
                Dim sourcePath As String = Path.Combine(binDir, Path.Combine("..\..\..\..\BuildTask", targetName))
                If (Not File.Exists(sourcePath)) Then
                    sourcePath = Path.Combine(Environment.GetEnvironmentVariable("suitebinaries"), targetName)
                    If (Not File.Exists(sourcePath)) Then
                        Throw New System.IO.FileNotFoundException(targetName)
                    End If
                End If
                File.Copy(sourcePath, destinationPath)
            End If
        End Sub

        Public Shared Function GetScannerTask(ByVal project As Project) As ProjectTaskElement
            Dim target As ProjectTargetElement = project.Xml.Targets.FirstOrDefault(Function(element) element.Name = "AfterBuild")

            If target IsNot Nothing Then
                For Each task As ProjectTaskElement In target.Tasks
                    If task IsNot Nothing AndAlso task.Name = "ScannerTask" Then
                        Return task
                    End If
                Next task
            End If

            Return Nothing
        End Function

        Friend Shared Function RegisterProjectWithMocks(ByVal project As Project, ByVal serviceProvider As IServiceProvider) As MockIVsProject
            Dim solution As MockSolution = TryCast(serviceProvider.GetService(GetType(SVsSolution)), MockSolution)
            Dim vsProject As New MockIVsProject(project.FullPath)

            For Each itemGroup As ProjectItemGroupElement In project.Xml.ItemGroups
                For Each item As ProjectItemElement In itemGroup.Items
                    vsProject.AddItem(item.Include)
                Next item
            Next itemGroup

            solution.AddProject(vsProject)

            Return vsProject
        End Function

        Public Shared Sub WaitForStop(ByVal accessor As CodeSweep.VSPackage.IBackgroundScanner_Accessor)
            Do While accessor.IsRunning
                System.Threading.Thread.Sleep(100)
            Loop
        End Sub

        Public Shared Sub WaitForStop(ByVal accessor As CodeSweep.VSPackage.BackgroundScanner_Accessor)
            Do While accessor.IsRunning
                System.Threading.Thread.Sleep(100)
            Loop
        End Sub

        Public Shared Sub RemoveCommandHandler(ByVal serviceProvider As IServiceProvider, ByVal id As UInteger)
            Dim mcs As OleMenuCommandService = TryCast(serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs IsNot Nothing Then
                Dim command As MenuCommand = mcs.FindCommand(New CommandID(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, CInt(Fix(id))))
                If command IsNot Nothing Then
                    mcs.RemoveCommand(command)
                End If
            End If
        End Sub

        Public Shared Sub RemoveCommandHandlers(ByVal serviceProvider As IServiceProvider)
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidIgnore)
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidConfig)
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidDoNotIgnore)
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidRepeatLastScan)
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidShowIgnoredInstances)
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidStopScan)
        End Sub
    End Class
End Namespace
