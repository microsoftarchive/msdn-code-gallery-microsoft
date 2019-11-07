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
Imports System.Text
Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Diagnostics
Imports System.ComponentModel.Design
Imports Microsoft.VisualStudio
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
Imports MsVsShell = Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class SccProviderServiceTest
		Private _serviceProvider As OleServiceProvider
		Private _solution As MockSolution
		Private _sccProvider As SccProvider

		''' <summary>
        ''' The service provider.
        ''' </summary>
		Private ReadOnly Property serviceProvider() As OleServiceProvider
			Get
				If _serviceProvider Is Nothing Then
					_serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
				End If

				Return _serviceProvider
			End Get
		End Property

		''' <summary>
        ''' The solution.
        ''' </summary>
		Private ReadOnly Property solution() As MockSolution
			Get
				If _solution Is Nothing Then
					_solution = New MockSolution()
				End If

				Return _solution
			End Get
		End Property

		''' <summary>
        ''' The provider.
        ''' </summary>
		Private ReadOnly Property sccProvider() As SccProvider
			Get
				If _sccProvider Is Nothing Then
                    ' Create a provider package.
					_sccProvider = New SccProvider()
				End If

				Return _sccProvider
			End Get
		End Property

		''' <summary>
        ''' Creates a SccProviderService object.
        ''' </summary>
		Public ReadOnly Property GetSccProviderServiceInstance() As SccProviderService
			Get
                ' Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it.
				Dim registerScciProvider As IVsRegisterScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider()
				serviceProvider.AddService(GetType(IVsRegisterScciProvider), registerScciProvider, True)

                ' Register solution events because the provider will try to subscribe to them.
				serviceProvider.AddService(GetType(SVsSolution), TryCast(solution, IVsSolution), True)

                ' Register TPD service because the provider will try to subscribe to TPD.
				Dim tpd As IVsTrackProjectDocuments2 = TryCast(MockTrackProjectDocumentsProvider.GetTrackProjectDocuments(), IVsTrackProjectDocuments2)
				serviceProvider.AddService(GetType(SVsTrackProjectDocuments), tpd, True)

                ' Site the package.
				Dim package As IVsPackage = TryCast(sccProvider, IVsPackage)
				package.SetSite(serviceProvider)

                '  Get the source control provider service object.
				Dim sccServiceMember As FieldInfo = GetType(SccProvider).GetField("sccService", BindingFlags.Instance Or BindingFlags.NonPublic)
				Dim target As SccProviderService = TryCast(sccServiceMember.GetValue(sccProvider), SccProviderService)

				Return target
			End Get
		End Property

		''' <summary>
        ''' A test for menu command status.
        ''' </summary>
		Private Sub VerifyCommandStatus(ByVal expectedStatus As OLECMDF, ByVal command As OLECMD())
			Dim guidCmdGroup As Guid = GuidList.guidSccProviderCmdSet

			Dim result As Integer = _sccProvider.QueryStatus(guidCmdGroup, 1, command, IntPtr.Zero)
			Assert.AreEqual(VSConstants.S_OK, result)
			Debug.Assert(CUInt(expectedStatus) = command(0).cmdf)
			Assert.AreEqual(CUInt(expectedStatus), command(0).cmdf)
		End Sub

		Private Sub VerifyCommandExecution(ByVal command As OLECMD())
			Dim mcs As MsVsShell.OleMenuCommandService = TryCast(sccProvider.GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            Dim cmd As New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CInt(Fix(command(0).cmdID)))
			Dim menuCmd As MenuCommand = mcs.FindCommand(cmd)
			menuCmd.Invoke()
		End Sub

		''' <summary>
        ''' A test for SccProviderService creation and interfaces.
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Assert.AreNotEqual(Nothing, target, "Could not create provider service")
			Assert.IsNotNull(TryCast(target, IVsSccProvider), "The object does not implement IVsPackage")
		End Sub

		''' <summary>
        ''' A test for Active.
        ''' </summary>
		<TestMethod()> _
		Public Sub ActiveTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

            ' After the object is created, the provider is inactive.
			Assert.AreEqual(False, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.")

            ' Activate the provider and test the result.
			target.SetActive()
			Assert.AreEqual(True, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.")

            ' Deactivate the provider and test the result.
			target.SetInactive()
			Assert.AreEqual(False, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.")
		End Sub

		''' <summary>
        ''' A test for AnyItemsUnderSourceControl (out int).
        ''' </summary>
		<TestMethod()> _
		Public Sub AnyItemsUnderSourceControlTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Dim pfResult As Integer = 0
			Dim actual As Integer = target.AnyItemsUnderSourceControl(pfResult)

            ' The method is not supposed to fail, and the basic provider cannot control any projects.
			Assert.AreEqual(VSConstants.S_OK, pfResult, "pfResult_AnyItemsUnderSourceControl_expected was not set correctly.")
			Assert.AreEqual(0, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.AnyItemsUnderSourceControl did not return the expected value.")
		End Sub

		''' <summary>
        ''' A test for SetActive ().
        ''' </summary>
		<TestMethod()> _
		Public Sub SetActiveTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Dim actual As Integer = target.SetActive()
			Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.SetActive failed.")
		End Sub

		''' <summary>
        ''' A test for SetInactive ().
        ''' </summary>
		<TestMethod()> _
		Public Sub SetInactiveTest()
			Dim target As SccProviderService = GetSccProviderServiceInstance

			Dim actual As Integer = target.SetInactive()
			Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.SetInactive failed.")
		End Sub

		''' <summary>
        ''' A test for QueryEditQuerySave interface.
        ''' </summary>
		<TestMethod()> _
		Public Sub QueryEditQuerySaveTest()
			Dim pfEditVerdict As UInteger
			Dim prgfMoreInfo As UInteger
			Dim pdwQSResult As UInteger
			Dim result As Integer

			Dim target As SccProviderService = GetSccProviderServiceInstance

            ' Check the functions that are not implemented.
			Assert.AreEqual(CInt(Fix(VSConstants.S_OK)), CInt(Fix(target.BeginQuerySaveBatch())))
			Assert.AreEqual(CInt(Fix(VSConstants.S_OK)), CInt(Fix(target.EndQuerySaveBatch())))
			Assert.AreEqual(CInt(Fix(VSConstants.S_OK)), CInt(Fix(target.DeclareReloadableFile("", 0, Nothing))))
			Assert.AreEqual(CInt(Fix(VSConstants.S_OK)), CInt(Fix(target.DeclareUnreloadableFile("", 0, Nothing))))
			Assert.AreEqual(CInt(Fix(VSConstants.S_OK)), CInt(Fix(target.OnAfterSaveUnreloadableFile("", 0, Nothing))))
			Assert.AreEqual(CInt(Fix(VSConstants.S_OK)), CInt(Fix(target.IsReloadable("", result))))
			Assert.AreEqual(1, result, "Not the right return value from IsReloadable")

            ' Create a basic service provider.

			Dim shell As IVsShell = TryCast(MockShellProvider.GetShellForCommandLine(), IVsShell)
			serviceProvider.AddService(GetType(IVsShell), shell, True)

            ' Command line tests.
			result = target.QueryEditFiles(CUInt(tagVSQueryEditFlags.QEF_ReportOnly), 1, New String() { "Dummy.txt" }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(0), prgfMoreInfo, "QueryEdit failed.")

			result = target.QuerySaveFile("Dummy.txt", 0, Nothing, pdwQSResult)
			Assert.AreEqual(VSConstants.S_OK, result, "QuerySave failed.")
			Assert.AreEqual(CUInt(tagVSQuerySaveResult.QSR_SaveOK), pdwQSResult, "QueryEdit failed.")

			serviceProvider.RemoveService(GetType(SVsShell))

            ' UI mode tests.
			shell = TryCast(MockShellProvider.GetShellForUI(), IVsShell)
			serviceProvider.AddService(GetType(SVsShell), shell, True)

            ' Edit of an uncontrolled file that doesn't exist on disk.
			result = target.QueryEditFiles(CUInt(tagVSQueryEditFlags.QEF_ReportOnly), 1, New String() { "Dummy.txt" }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(0), prgfMoreInfo, "QueryEdit failed.")

            ' Mock a solution with a project and a file.
			solution.SolutionFile = Path.GetTempFileName()
            Dim project As New MockIVsProject(Path.GetTempFileName())
			solution.AddProject(project)
			' Add only the project to source control.
            Dim uncontrolled As New Hashtable()
			uncontrolled(TryCast(project, IVsSccProject2)) = True
			target.AddProjectsToSourceControl(uncontrolled, False)
            ' Check that solution file is not controlled.
			Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(solution.SolutionFile), "Incorrect status returned")
            ' Make the solution read-only on disk.
			File.SetAttributes(solution.SolutionFile, FileAttributes.ReadOnly)

            ' QueryEdit in report mode for uncontrolled readonly file.
			result = target.QueryEditFiles(CUInt(tagVSQueryEditFlags.QEF_ReportOnly), 1, New String() { solution.SolutionFile }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditNotOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResultFlags.QER_EditNotPossible Or tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc), prgfMoreInfo, "QueryEdit failed.")

            ' QueryEdit in silent mode for uncontrolled readonly file.
			result = target.QueryEditFiles(CUInt(tagVSQueryEditFlags.QEF_SilentMode), 1, New String() { solution.SolutionFile }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditNotOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResultFlags.QER_NoisyPromptRequired), CUInt(tagVSQueryEditResultFlags.QER_NoisyPromptRequired) And prgfMoreInfo, "QueryEdit failed.")

            ' Mock the UIShell service to answer Yes to the dialog invocation.
			Dim mockUIShell As BaseMock = MockUiShellProvider.GetShowMessageBoxYes()
			serviceProvider.AddService(GetType(IVsUIShell), mockUIShell, True)

            ' QueryEdit for uncontrolled readonly file: allow the edit and make the file read-write.
			result = target.QueryEditFiles(0, 1, New String() { solution.SolutionFile }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(0), prgfMoreInfo, "QueryEdit failed.")
			Assert.AreEqual(Of FileAttributes)(FileAttributes.Normal, File.GetAttributes(solution.SolutionFile), "File was not made writable")
			serviceProvider.RemoveService(GetType(IVsUIShell))

            ' QueryEdit in report mode for controlled readonly file.
			result = target.QueryEditFiles(CUInt(tagVSQueryEditFlags.QEF_ReportOnly), 1, New String() { project.ProjectFile }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditNotOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResultFlags.QER_EditNotPossible Or tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc), prgfMoreInfo, "QueryEdit failed.")

            ' QueryEdit in silent mode for controlled readonly file: should allow the edit and make the file read-write.
			result = target.QueryEditFiles(CUInt(tagVSQueryEditFlags.QEF_SilentMode), 1, New String() { project.ProjectFile }, Nothing, Nothing, pfEditVerdict, prgfMoreInfo)
			Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResult.QER_EditOK), pfEditVerdict, "QueryEdit failed.")
			Assert.AreEqual(CUInt(tagVSQueryEditResultFlags.QER_MaybeCheckedout), prgfMoreInfo, "QueryEdit failed.")
			Assert.AreEqual(Of FileAttributes)(FileAttributes.Normal, File.GetAttributes(solution.SolutionFile), "File was not made writable")
			serviceProvider.RemoveService(GetType(IVsUIShell))
		End Sub

		''' <summary>
        ''' A test for GetFileStatus/GetSccGlyphs.
        ''' </summary>
		<TestMethod()> _
		Public Sub TestFileStatus()
			Dim target As SccProviderService = GetSccProviderServiceInstance
			solution.SolutionFile = Path.GetTempFileName()
            Dim project As New MockIVsProject(Path.GetTempFileName())
			project.AddItem(Path.GetTempFileName())
			solution.AddProject(project)

			Dim rgsiGlyphs As VsStateIcon() = New VsStateIcon(0){}
			Dim rgsiGlyphsFromStatus As VsStateIcon() = New VsStateIcon(0){}
			Dim rgdwSccStatus As UInteger() = New UInteger(0){}
			Dim result As Integer = 0
            Dim strTooltip As String = String.Empty

            ' Check glyphs and statuses for uncontrolled items.
			Dim files As IList(Of String) = New String() { solution.SolutionFile, project.ProjectFile, project.ProjectItems(0) }
			For Each file As String In files
				Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(file), "Incorrect status returned")

				result = target.GetSccGlyph(1, New String() { file }, rgsiGlyphs, rgdwSccStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(VsStateIcon.STATEICON_BLANK, rgsiGlyphs(0))
				Assert.AreEqual(Of UInteger)(CUInt(__SccStatus.SCC_STATUS_NOTCONTROLLED), rgdwSccStatus(0))

				result = target.GetSccGlyphFromStatus(rgdwSccStatus(0), rgsiGlyphsFromStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(rgsiGlyphs(0), rgsiGlyphsFromStatus(0))
			Next file

            ' Uncontrolled items should not have tooltips.
			target.GetGlyphTipText(TryCast(project, IVsHierarchy), VSConstants.VSITEMID_ROOT, strTooltip)
			Assert.IsTrue(strTooltip.Length = 0)

            Dim uncontrolled As New Hashtable()
			uncontrolled(TryCast(project, IVsSccProject2)) = True
			target.AddProjectsToSourceControl(uncontrolled, True)

			For Each file As String In files
				Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(file), "Incorrect status returned")

				result = target.GetSccGlyph(1, New String() { file }, rgsiGlyphs, rgdwSccStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(VsStateIcon.STATEICON_CHECKEDIN, rgsiGlyphs(0))
				Assert.AreEqual(Of UInteger)(CUInt(__SccStatus.SCC_STATUS_CONTROLLED), rgdwSccStatus(0))

				result = target.GetSccGlyphFromStatus(rgdwSccStatus(0), rgsiGlyphsFromStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(rgsiGlyphs(0), rgsiGlyphsFromStatus(0))
			Next file

            ' Checked in items should have tooltips.
			target.GetGlyphTipText(TryCast(project, IVsHierarchy), VSConstants.VSITEMID_ROOT, strTooltip)
			Assert.IsTrue(strTooltip.Length > 0)

			For Each file As String In files
				target.CheckoutFile(file)
				Assert.AreEqual(SourceControlStatus.scsCheckedOut, target.GetFileStatus(file), "Incorrect status returned")

				result = target.GetSccGlyph(1, New String() { file }, rgsiGlyphs, rgdwSccStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(VsStateIcon.STATEICON_CHECKEDOUT, rgsiGlyphs(0))
				Assert.AreEqual(Of UInteger)(CUInt(__SccStatus.SCC_STATUS_CHECKEDOUT), rgdwSccStatus(0))

				result = target.GetSccGlyphFromStatus(rgdwSccStatus(0), rgsiGlyphsFromStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(rgsiGlyphs(0), rgsiGlyphsFromStatus(0))
			Next file

            ' Checked out items should have tooltips, too.
			target.GetGlyphTipText(TryCast(project, IVsHierarchy), VSConstants.VSITEMID_ROOT, strTooltip)
			Assert.IsTrue(strTooltip.Length > 0)

			For Each file As String In files
				target.CheckinFile(file)
				Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(file), "Incorrect status returned")

				result = target.GetSccGlyph(1, New String() { file }, rgsiGlyphs, rgdwSccStatus)
				Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
				Assert.AreEqual(Of VsStateIcon)(VsStateIcon.STATEICON_CHECKEDIN, rgsiGlyphs(0))
				Assert.AreEqual(Of UInteger)(CUInt(__SccStatus.SCC_STATUS_CONTROLLED), rgdwSccStatus(0))
			Next file

            ' Add a new file to the project (don't worry about TPD events for now).
			Dim pendingAddFile As String = Path.GetTempFileName()
			project.AddItem(pendingAddFile)
			Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(pendingAddFile), "Incorrect status returned")

			result = target.GetSccGlyph(1, New String() { pendingAddFile }, rgsiGlyphs, rgdwSccStatus)
			Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
			Assert.AreEqual(Of VsStateIcon)(VsStateIcon.STATEICON_CHECKEDOUT, rgsiGlyphs(0))
			Assert.AreEqual(Of UInteger)(CUInt(__SccStatus.SCC_STATUS_CHECKEDOUT), rgdwSccStatus(0))

            ' Pending add items should have tooltips, too.
			target.GetGlyphTipText(TryCast(project, IVsHierarchy), 1, strTooltip)
			Assert.IsTrue(strTooltip.Length > 0)

            ' Checkin the pending add file.
			target.AddFileToSourceControl(pendingAddFile)
			Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(pendingAddFile), "Incorrect status returned")

			result = target.GetSccGlyph(1, New String() { pendingAddFile }, rgsiGlyphs, rgdwSccStatus)
			Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
			Assert.AreEqual(Of VsStateIcon)(VsStateIcon.STATEICON_CHECKEDIN, rgsiGlyphs(0))
			Assert.AreEqual(Of UInteger)(CUInt(__SccStatus.SCC_STATUS_CONTROLLED), rgdwSccStatus(0))
		End Sub

		''' <summary>
        ''' A test for TrackProjectDocuments.
        ''' </summary>
		<TestMethod()> _
		Public Sub TestTPDEvents()
			Dim result As Integer = 0

			Dim target As SccProviderService = GetSccProviderServiceInstance
			solution.SolutionFile = Path.GetTempFileName()
            Dim project As New MockIVsProject(Path.GetTempFileName())
			solution.AddProject(project)

            Dim uncontrolled As New Hashtable()
			uncontrolled(TryCast(project, IVsSccProject2)) = True
			target.AddProjectsToSourceControl(uncontrolled, True)
            ' In real live, a QueryEdit call on the project file would be necessary to add/rename/delete items.

            ' Add a new item and fire the appropriate events.
			Dim pendingAddFile As String = Path.GetTempFileName()
			Dim pSummaryResultAdd As VSQUERYADDFILERESULTS() = New VSQUERYADDFILERESULTS(0){}
			Dim rgResultsAdd As VSQUERYADDFILERESULTS() = New VSQUERYADDFILERESULTS(0){}
			result = target.OnQueryAddFiles(TryCast(project, IVsProject), 1, New String() {pendingAddFile}, Nothing, pSummaryResultAdd, rgResultsAdd)
			Assert.AreEqual(Of Integer)(VSConstants.E_NOTIMPL, result)
			project.AddItem(pendingAddFile)
			result = target.OnAfterAddFilesEx(1, 1, New IVsProject() { TryCast(project, IVsProject) }, New Integer() { 0 }, New String() { pendingAddFile }, Nothing)
			Assert.AreEqual(Of Integer)(VSConstants.E_NOTIMPL, result)
			Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(pendingAddFile), "Incorrect status returned")

            ' Checkin the pending add file.
			target.AddFileToSourceControl(pendingAddFile)

            ' Rename the item and verify the file remains is controlled.
			Dim newName As String = pendingAddFile & ".renamed"
			Dim pSummaryResultRen As VSQUERYRENAMEFILERESULTS() = New VSQUERYRENAMEFILERESULTS(0){}
			Dim rgResultsRen As VSQUERYRENAMEFILERESULTS() = New VSQUERYRENAMEFILERESULTS(0){}
			result = target.OnQueryRenameFiles(TryCast(project, IVsProject), 1, New String() { pendingAddFile }, New String() { newName }, Nothing, pSummaryResultRen, rgResultsRen)
			Assert.AreEqual(Of Integer)(VSConstants.E_NOTIMPL, result)
			project.RenameItem(pendingAddFile, newName)
			result = target.OnAfterRenameFiles(1, 1, New IVsProject() {TryCast(project, IVsProject)}, New Integer() {0}, New String() { pendingAddFile }, New String() { newName }, New VSRENAMEFILEFLAGS() {VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_NoFlags})
			Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
			Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(pendingAddFile), "Incorrect status returned")
			Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(newName), "Incorrect status returned")

            ' Mock the UIShell service to answer Cancel to the dialog invocation.
			Dim mockUIShell As BaseMock = MockUiShellProvider.GetShowMessageBoxCancel()
			serviceProvider.AddService(GetType(IVsUIShell), mockUIShell, True)
            ' Try to delete the file from project; the delete should not be allowed.
			Dim pSummaryResultDel As VSQUERYREMOVEFILERESULTS() = New VSQUERYREMOVEFILERESULTS(0){}
			Dim rgResultsDel As VSQUERYREMOVEFILERESULTS() = New VSQUERYREMOVEFILERESULTS(0){}
			result = target.OnQueryRemoveFiles(TryCast(project, IVsProject), 1, New String() { newName }, Nothing, pSummaryResultDel, rgResultsDel)
			Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
			Assert.AreEqual(Of VSQUERYREMOVEFILERESULTS)(VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK, pSummaryResultDel(0))
            ' Mock the UIShell service to answer Yes to the dialog invocation.
			serviceProvider.RemoveService(GetType(IVsUIShell))
			mockUIShell = MockUiShellProvider.GetShowMessageBoxYes()
			serviceProvider.AddService(GetType(IVsUIShell), mockUIShell, True)
            ' Try to delete the file from project; the delete should be allowed this time.
			result = target.OnQueryRemoveFiles(TryCast(project, IVsProject), 1, New String() { newName }, Nothing, pSummaryResultDel, rgResultsDel)
			Assert.AreEqual(Of Integer)(VSConstants.S_OK, result)
			Assert.AreEqual(Of VSQUERYREMOVEFILERESULTS)(VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK, pSummaryResultDel(0))
            ' Remove the file from project.
			project.RemoveItem(newName)
			result = target.OnAfterRemoveFiles(1, 1, New IVsProject() { TryCast(project, IVsProject) }, New Integer() { 0 }, New String() { newName }, Nothing)
			Assert.AreEqual(Of Integer)(VSConstants.E_NOTIMPL, result)
		End Sub

		''' <summary>
        ''' A test for opening and closing a controlled solution.
        ''' </summary>
		<TestMethod()> _
		Public Sub TestOpenCloseControlled()
			Const strProviderName As String = "Sample Source Control Provider:{8C902FDC-CE93-49b7-BF66-0144082555F9}"
			Const strSolutionPersistanceKey As String = "SampleSourceControlProviderSolutionProperties"
			Const strSolutionUserOptionsKey As String = "SampleSourceControlProvider"

			Dim result As Integer = 0

            ' Create a solution.
			Dim target As SccProviderService = GetSccProviderServiceInstance
			solution.SolutionFile = Path.GetTempFileName()
            Dim project As New MockIVsProject(Path.GetTempFileName())
			solution.AddProject(project)

            ' Check solution props.
			Dim saveSolnProps As VSQUERYSAVESLNPROPS() = New VSQUERYSAVESLNPROPS(0){}
			result = sccProvider.QuerySaveSolutionProps(Nothing, saveSolnProps)
			Assert.AreEqual(VSConstants.S_OK, result)
			Assert.AreEqual(Of VSQUERYSAVESLNPROPS)(VSQUERYSAVESLNPROPS.QSP_HasNoProps, saveSolnProps(0))

			' Add the solution to source control.
            Dim uncontrolled As New Hashtable()
			uncontrolled(TryCast(project, IVsSccProject2)) = True
			target.AddProjectsToSourceControl(uncontrolled, True)

            ' Solution should be dirty now.
			result = sccProvider.QuerySaveSolutionProps(Nothing, saveSolnProps)
			Assert.AreEqual(VSConstants.S_OK, result)
			Assert.AreEqual(Of VSQUERYSAVESLNPROPS)(VSQUERYSAVESLNPROPS.QSP_HasDirtyProps, saveSolnProps(0))

            ' Set the project offline so we'll have something to save in the "suo" stream.
			target.ToggleOfflineStatus(project)

            ' Force the provider to write the solution info into a stream. 
            Dim pOptionsStream As IStream = TryCast(New ComStreamFromDataStream(New MemoryStream()), IStream)
			sccProvider.WriteUserOptions(pOptionsStream, strSolutionUserOptionsKey)
            ' Move the stream position to the beginning.
			Dim liOffset As LARGE_INTEGER
			liOffset.QuadPart = 0
			Dim ulPosition As ULARGE_INTEGER() = New ULARGE_INTEGER(0){}
			pOptionsStream.Seek(liOffset, 0, ulPosition)

            ' Write solution props.
			Dim propertyBag As BaseMock = MockPropertyBagProvider.GetWritePropertyBag()
			sccProvider.WriteSolutionProps(Nothing, strSolutionPersistanceKey, TryCast(propertyBag, IPropertyBag))

            ' Close the solution to clean up the scc status.
			Dim pfCancel As Integer = 0
			target.OnQueryCloseProject(project, 0, pfCancel)
			target.OnQueryCloseSolution(Nothing, pfCancel)
			Assert.AreEqual(pfCancel, 0, "Solution close was canceled")
			target.OnBeforeCloseProject(project, 0)
            ' Theoretically the project should have called this, but especially after an add to scc, some projects forget to call it.
			' target.UnregisterSccProject(project);
			target.OnBeforeCloseSolution(Nothing)
			target.OnAfterCloseSolution(Nothing)

            ' Now attempt the "reopen".
            ' The solution reads the solution properties.
			propertyBag = MockPropertyBagProvider.GetReadPropertyBag()
			sccProvider.ReadSolutionProps(Nothing, Nothing, Nothing, strSolutionPersistanceKey, 1, TryCast(propertyBag, IPropertyBag))
            ' The solution reads the user options from the stream where they were written before.
			sccProvider.ReadUserOptions(pOptionsStream, strSolutionUserOptionsKey)
            ' Then the projects are opened and register with the provider.
			target.RegisterSccProject(project, "Project's location", "AuxPath", Path.GetDirectoryName(project.ProjectFile), strProviderName)
            ' solution event fired for this project.
			target.OnAfterOpenProject(project, 0)
            ' Then solution completes opening.
			target.OnAfterOpenSolution(Nothing, 0)

			Assert.IsTrue(target.IsProjectControlled(Nothing), "The solution's controlled status was not correctly persisted or read from property bag")
			Assert.IsTrue(target.IsProjectControlled(project), "The project's controlled status was not correctly set")
			Assert.IsTrue(target.IsProjectOffline(project), "The project's offline status was incorrectly persisted or read from suo stream")
		End Sub


		''' <summary>
        ''' A test for scc menu commands.
        ''' </summary>
		<TestMethod()> _
		Public Sub TestSccMenuCommands()
			Dim result As Integer = 0
            Dim badGuid As New Guid()
			Dim guidCmdGroup As Guid = GuidList.guidSccProviderCmdSet

			Dim cmdAddToScc As OLECMD() = New OLECMD(0){}
			cmdAddToScc(0).cmdID = CommandId.icmdAddToSourceControl
			Dim cmdCheckin As OLECMD() = New OLECMD(0){}
			cmdCheckin(0).cmdID = CommandId.icmdCheckin
			Dim cmdCheckout As OLECMD() = New OLECMD(0){}
			cmdCheckout(0).cmdID = CommandId.icmdCheckout
			Dim cmdUseSccOffline As OLECMD() = New OLECMD(0){}
			cmdUseSccOffline(0).cmdID = CommandId.icmdUseSccOffline
			Dim cmdViewToolWindow As OLECMD() = New OLECMD(0){}
			cmdViewToolWindow(0).cmdID = CommandId.icmdViewToolWindow
			Dim cmdToolWindowToolbarCommand As OLECMD() = New OLECMD(0){}
			cmdToolWindowToolbarCommand(0).cmdID = CommandId.icmdToolWindowToolbarCommand
			Dim cmdUnsupported As OLECMD() = New OLECMD(0){}
			cmdUnsupported(0).cmdID = 0

            ' Initialize the provider, etc.
			Dim target As SccProviderService = GetSccProviderServiceInstance

            ' Mock a service implementing IVsMonitorSelection.
			Dim monitorSelection As BaseMock = MockIVsMonitorSelectionFactory.GetMonSel()
			serviceProvider.AddService(GetType(IVsMonitorSelection), monitorSelection, True)

            ' Commands that don't belong to our package should not be supported.
			result = _sccProvider.QueryStatus(badGuid, 1, cmdAddToScc, IntPtr.Zero)
			Assert.AreEqual(CInt(Fix(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED)), result)

            ' The command should be invisible when there is no solution.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdAddToScc)

            ' Activate the provider and test the result.
			target.SetActive()
			Assert.AreEqual(True, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.")

            ' The commands should be invisible when there is no solution.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdUseSccOffline)

            ' Commands that don't belong to our package should not be supported.
			result = _sccProvider.QueryStatus(guidCmdGroup, 1, cmdUnsupported, IntPtr.Zero)
			Assert.AreEqual(CInt(Fix(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED)), result)

            ' Deactivate the provider and test the result.
			target.SetInactive()
			Assert.AreEqual(False, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.")

            ' Create a solution.
			solution.SolutionFile = Path.GetTempFileName()
            Dim project As New MockIVsProject(Path.GetTempFileName())
			project.AddItem(Path.GetTempFileName())
			solution.AddProject(project)

            ' The commands should be invisible when the provider is not active.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdUseSccOffline)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdViewToolWindow)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_INVISIBLE, cmdToolWindowToolbarCommand)

            ' Activate the provider and test the result.
			target.SetActive()
			Assert.AreEqual(True, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.")

            ' The command should be visible but disabled now, except the toolwindow ones.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdViewToolWindow)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdToolWindowToolbarCommand)

            ' Set selection to solution node.
			Dim selSolutionRoot As VSITEMSELECTION
			selSolutionRoot.pHier = TryCast(_solution, IVsHierarchy)
			selSolutionRoot.itemid = VSConstants.VSITEMID_ROOT
			monitorSelection("Selection") = New VSITEMSELECTION() { selSolutionRoot }

            ' The add command should be available, rest should be disabled.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Still solution hierarchy, but other way.
			selSolutionRoot.pHier = Nothing
			selSolutionRoot.itemid = VSConstants.VSITEMID_ROOT
			monitorSelection("Selection") = New VSITEMSELECTION() { selSolutionRoot }

            ' The add command should be available, rest should be disabled.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Set selection to project node.
			Dim selProjectRoot As VSITEMSELECTION
			selProjectRoot.pHier = TryCast(project, IVsHierarchy)
			selProjectRoot.itemid = VSConstants.VSITEMID_ROOT
			monitorSelection("Selection") = New VSITEMSELECTION() { selProjectRoot }

            ' The add command should be available, rest should be disabled.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Set selection to project item.
			Dim selProjectItem As VSITEMSELECTION
			selProjectItem.pHier = TryCast(project, IVsHierarchy)
			selProjectItem.itemid = 0
			monitorSelection("Selection") = New VSITEMSELECTION() { selProjectItem }

            ' The add command should be available, rest should be disabled.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Set selection to project and item node and add project to scc.
			monitorSelection("Selection") = New VSITEMSELECTION() { selProjectRoot, selProjectItem }
			VerifyCommandExecution(cmdAddToScc)

            ' The add command and checkin should be disabled, rest should be available now.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline)

            ' Checkout the project.
			VerifyCommandExecution(cmdCheckout)

            ' The add command and checkout should be disabled, rest should be available now.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline)

            ' Select the solution.
			monitorSelection("Selection") = New VSITEMSELECTION() { selSolutionRoot }

            ' The checkout and offline should be disabled, rest should be available now.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Checkin the project.
			VerifyCommandExecution(cmdCheckin)

            ' The add command and checkout should be enabled, rest should be disabled.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Add the solution to scc.
			VerifyCommandExecution(cmdAddToScc)

            ' The add command and checkin should be disabled, rest should be available now.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline)

            ' Select the solution and project.
			monitorSelection("Selection") = New VSITEMSELECTION() { selSolutionRoot, selProjectRoot }

            ' Take the project and solution offline.
			VerifyCommandExecution(cmdUseSccOffline)

			' The offline command should be latched
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED Or OLECMDF.OLECMDF_LATCHED, cmdUseSccOffline)

            ' Select the solution only.
			monitorSelection("Selection") = New VSITEMSELECTION() { selSolutionRoot}

            ' Take the solution online.
			VerifyCommandExecution(cmdUseSccOffline)

            ' The offline command should be normal again.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline)

            ' Select the solution and project.
			monitorSelection("Selection") = New VSITEMSELECTION() { selSolutionRoot, selProjectRoot }

            ' The offline command should be disabled for mixed selection.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline)

            ' Add a new item to the project.
			project.AddItem(Path.GetTempFileName())

            ' Select the new item.
			selProjectItem.pHier = TryCast(project, IVsHierarchy)
			selProjectItem.itemid = 1
			monitorSelection("Selection") = New VSITEMSELECTION() { selProjectItem }

            ' The add command and checkout should be disabled, rest should be available now.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED Or OLECMDF.OLECMDF_LATCHED, cmdUseSccOffline)

			' Checkin the new file (this should do an add)
			VerifyCommandExecution(cmdCheckin)

            ' The add command and checkout should be disabled, rest should be available now.
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED, cmdCheckout)
			VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED Or OLECMDF.OLECMDF_ENABLED Or OLECMDF.OLECMDF_LATCHED, cmdUseSccOffline)
		End Sub
	End Class
End Namespace
