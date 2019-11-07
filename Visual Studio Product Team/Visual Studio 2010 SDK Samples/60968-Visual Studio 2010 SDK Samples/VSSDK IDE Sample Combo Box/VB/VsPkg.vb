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
Imports System.Diagnostics
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.Win32
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell
Imports System.Windows.Forms
Imports Microsoft.VisualStudio
<Assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope := "namespace", Target := "Microsoft.Samples.VisualStudio.ComboBox")>
Namespace Microsoft.Samples.VisualStudio.ComboBox
	''' <summary>
	''' This is the class that implements the package exposed by this assembly.
	'''
	''' The minimum requirement for a class to be considered a valid package for Visual Studio
	''' is to implement the IVsPackage interface and register itself with the shell.
	''' This package uses the helper classes defined inside the Managed Package Framework (MPF)
	''' to do it: it derives from the Package class that provides the implementation of the 
	''' IVsPackage interface and uses the registration attributes defined in the framework to 
	''' register itself and its components with the shell.
	''' </summary>
	' This attribute tells the registration utility (regpkg.exe) that this class needs
	' to be registered as package.
	' A Visual Studio component can be registered under different regitry roots; for instance
	' when you debug your package you want to register it in the experimental instance. This
	' attribute specifies the registry root to use if no one is provided to regpkg.exe with
	' the /root switch.
	' This attribute is used to register the informations needed to show the this package
	' in the Help/About dialog of Visual Studio.
	' In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
	' package needs to have a valid load key (it can be requested at 
	' http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
	' package has a load key embedded in its resources.
	' This attribute is needed to let the shell know that this package exposes some menus.
	' This attribute registers a tool window exposed by this package.
	<PackageRegistration(UseManagedResourcesOnly := True), InstalledProductRegistration("#100", "#102", "1.0", IconResourceID := 400), ProvideMenuResource(1000, 1), Guid(GuidList.guidComboBoxPkgString)> _
	Public NotInheritable Class ComboBoxPackage
		Inherits Package
		''' <summary>
		''' Default constructor of the package.
		''' Inside this method you can place any initialization code that does not require 
		''' any Visual Studio service because at this point the package object is created but 
		''' not sited yet inside Visual Studio environment. The place to do all the other 
		''' initialization is the Initialize method.
		''' </summary>
		Public Sub New()
			Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", Me.ToString()))
		End Sub

		'///////////////////////////////////////////////////////////////////////////
		' Overriden Package Implementation
		#Region "Package Members"

		''' <summary>
		''' Initialization of the package; this method is called right after the package is sited, so this is the place
		''' where you can put all the initilaization code that rely on services provided by VisualStudio.
		''' </summary>
		Protected Overrides Sub Initialize()
			Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", Me.ToString()))
			MyBase.Initialize()

            ' Add our command handlers for menu (commands must exist in the .vsct file).
			Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
			If Not Nothing Is mcs Then
				' DropDownCombo
				'	 a DROPDOWNCOMBO does not let the user type into the combo box; they can only pick from the list.
				'   The string value of the element selected is returned.
				'	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
				'
				'   A DropDownCombo box requires two commands:
				'     One command (cmdidMyCombo) is used to ask for the current value of the combo box and to 
				'     set the new value when the user makes a choice in the combo box.
				'
				'     The second command (cmdidMyComboGetList) is used to retrieve this list of choices for the combo box.
				' 
				' Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
				' enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
				' control the statue of a combo box, e.g. what list of items should be shown and what is the 
				' current value. In order to communicate this information actually IOleCommandTarget::Exec
				' is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
				' QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
				' two commands to retrieve this information. The main command id for the command is used to 
				' retrieve the current value and the second command is used to retrieve the full list of 
				' choices to be displayed as an array of strings.
                Dim menuMyDropDownComboCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyDropDownCombo)))
                Dim menuMyDropDownComboCommand As New OleMenuCommand(New EventHandler(AddressOf OnMenuMyDropDownCombo), menuMyDropDownComboCommandID)
				menuMyDropDownComboCommand.ParametersDescription = "$" ' accept any argument string
				mcs.AddCommand(menuMyDropDownComboCommand)

                Dim menuMyDropDownComboGetListCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyDropDownComboGetList)))
                Dim menuMyDropDownComboGetListCommand As MenuCommand = New OleMenuCommand(New EventHandler(AddressOf OnMenuMyDropDownComboGetList), menuMyDropDownComboGetListCommandID)
				mcs.AddCommand(menuMyDropDownComboGetListCommand)

				' IndexCombo
				'	 An INDEXCOMBO is the same as a DROPDOWNCOMBO in that it is a "pick from list" only combo.
				'	 The difference is an INDEXCOMBO returns the selected value as an index into the list (0 based).
				'	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
				'
				'   An IndexCombo box requires two commands:
				'     One command is used to ask for the current value of the combo box and to set the new value when the user
				'     makes a choice in the combo box.
				'
				'     The second command is used to retrieve this list of choices for the combo box.
                Dim menuMyIndexComboCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyIndexCombo)))
                Dim menuMyIndexComboCommand As New OleMenuCommand(New EventHandler(AddressOf OnMenuMyIndexCombo), menuMyIndexComboCommandID)
				menuMyIndexComboCommand.ParametersDescription = "$" ' accept any argument string
				mcs.AddCommand(menuMyIndexComboCommand)

                Dim menuMyIndexComboGetListCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyIndexComboGetList)))
				Dim menuMyIndexComboGetListCommand As MenuCommand = New OleMenuCommand(New EventHandler(AddressOf OnMenuMyIndexComboGetList), menuMyIndexComboGetListCommandID)
				mcs.AddCommand(menuMyIndexComboGetListCommand)

				' MRUCombo
				'   An MRUCOMBO allows the user to type into the edit box. The history of strings entered
				'   is automatically persisted by the IDE on a per-user/per-machine basis. 
				'	 For example, this type of combo is used for the "Find" combo on the "Standard" toolbar.
				'
				'   An MRU Combo box requires only one command:
				'     One command is used to ask for the current value of the combo box and to set the new value when the user
				'     makes a choice in the combo box.
				'
				'     The list of choices entered is automatically remembered by the IDE on a per-user/per-machine basis.
                Dim menuMyMRUComboCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyMRUCombo)))
                Dim menuMyMRUComboCommand As New OleMenuCommand(New EventHandler(AddressOf OnMenuMyMRUCombo), menuMyMRUComboCommandID)
				menuMyMRUComboCommand.ParametersDescription = "$" ' accept any argument string
				mcs.AddCommand(menuMyMRUComboCommand)

				' DynamicCombo
				'   A DYNAMICCOMBO allows the user to type into the edit box or pick from the list. The 
				'	 list of choices is usually fixed and is managed by the command handler for the command.
				'	 For example, this type of combo is used for the "Zoom" combo on the "Class Designer" toolbar.
				'
				'   A Combo box requires two commands:
				'     One command is used to ask for the current value of the combo box and to set the new value when the user
				'     makes a choice in the combo box.
				'
				'     The second command is used to retrieve this list of choices for the combo box.
                Dim menuMyDynamicComboCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyDynamicCombo)))
                Dim menuMyDynamicComboCommand As New OleMenuCommand(New EventHandler(AddressOf OnMenuMyDynamicCombo), menuMyDynamicComboCommandID)
				menuMyDynamicComboCommand.ParametersDescription = "$" ' accept any argument string
				mcs.AddCommand(menuMyDynamicComboCommand)

                Dim menuMyDynamicComboGetListCommandID As New CommandID(GuidList.guidComboBoxCmdSet, CInt(Fix(PkgCmdIDList.cmdidMyDynamicComboGetList)))
				Dim menuMyDynamicComboGetListCommand As MenuCommand = New OleMenuCommand(New EventHandler(AddressOf OnMenuMyDynamicComboGetList), menuMyDynamicComboGetListCommandID)
				mcs.AddCommand(menuMyDynamicComboGetListCommand)
			End If
		End Sub

		#End Region

		#Region "Combo Box Commands"

		Private dropDownComboChoices As String() = { Resources.Apples, Resources.Oranges, Resources.Pears, Resources.Bananas }
		Private currentDropDownComboChoice As String = Resources.Apples

		' DropDownCombo
		'	 a DROPDOWNCOMBO does not let the user type into the combo box; they can only pick from the list.
		'   The string value of the element selected is returned.
		'	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
		'
		'   A DropDownCombo box requires two commands:
		'     One command (cmdidMyCombo) is used to ask for the current value of the combo box and to 
		'     set the new value when the user makes a choice in the combo box.
		'
		'     The second command (cmdidMyComboGetList) is used to retrieve this list of choices for the combo box.
		Private Sub OnMenuMyDropDownCombo(ByVal sender As Object, ByVal e As EventArgs)
            If e Is System.EventArgs.Empty Then
                ' We should never get here; EventArgs are required.
                Throw (New ArgumentException(Resources.EventArgsRequired)) ' force an exception to be thrown
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim newChoice As String = TryCast(eventArgs.InValue, String)
                Dim vOut As IntPtr = eventArgs.OutValue

                If vOut <> IntPtr.Zero AndAlso newChoice IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.BothInOutParamsIllegal))
                ElseIf vOut <> IntPtr.Zero Then
                    ' When vOut is non-NULL, the IDE is requesting the current value for the combo.
                    Marshal.GetNativeVariantForObject(Me.currentDropDownComboChoice, vOut)

                ElseIf newChoice IsNot Nothing Then
                    ' New value was selected or typed in
                    ' see if it is one of our items.
                    Dim validInput As Boolean = False
                    Dim indexInput As Integer = -1
                    For indexInput = 0 To dropDownComboChoices.Length - 1
                        If String.Compare(dropDownComboChoices(indexInput), newChoice, StringComparison.CurrentCultureIgnoreCase) = 0 Then
                            validInput = True
                            Exit For
                        End If
                    Next indexInput

                    If validInput Then
                        Me.currentDropDownComboChoice = dropDownComboChoices(indexInput)
                        ShowMessage(Resources.MyDropDownCombo, Me.currentDropDownComboChoice)
                    Else
                        ' Force an exception to be thrown.
                        Throw (New ArgumentException(Resources.ParamNotValidStringInList))
                    End If
                Else
                    ' We should never get here.
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.InOutParamCantBeNULL))
                End If
            Else
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If
		End Sub

		' A DropDownCombo box requires two commands:
		'    This command is used to retrieve this list of choices for the combo box.
		' 
		' Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
		' enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
		' control the statue of a combo box, e.g. what list of items should be shown and what is the 
		' current value. In order to communicate this information actually IOleCommandTarget::Exec
		' is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
		' QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
		' two commands to retrieve this information. The main command id for the command is used to 
		' retrieve the current value and the second command is used to retrieve the full list of 
		' choices to be displayed as an array of strings.
		Private Sub OnMenuMyDropDownComboGetList(ByVal sender As Object, ByVal e As EventArgs)
            If (Nothing Is e) OrElse (e Is System.EventArgs.Empty) Then
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentNullException(Resources.EventArgsRequired))
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim inParam As Object = eventArgs.InValue
                Dim vOut As IntPtr = eventArgs.OutValue

                If inParam IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.InParamIllegal))
                ElseIf vOut <> IntPtr.Zero Then
                    Marshal.GetNativeVariantForObject(Me.dropDownComboChoices, vOut)
                Else
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.OutParamRequired))
                End If
            End If

		End Sub

		Private indexComboChoices As String() = { Resources.Lions, Resources.Tigers, Resources.Bears}
		Private currentIndexComboChoice As Integer = 0

		' IndexCombo
		'	 An INDEXCOMBO is the same as a DROPDOWNCOMBO in that it is a "pick from list" only combo.
		'	 The difference is an INDEXCOMBO returns the selected value as an index into the list (0 based).
		'	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
		'
		'   An IndexCombo box requires two commands:
		'     One command is used to ask for the current value of the combo box and to set the new value when the user
		'     makes a choice in the combo box.
		'
		'     The second command is used to retrieve this list of choices for the combo box.
		Private Sub OnMenuMyIndexCombo(ByVal sender As Object, ByVal e As EventArgs)
            If (Nothing Is e) OrElse (e Is System.EventArgs.Empty) Then
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim input As Object = eventArgs.InValue
                Dim vOut As IntPtr = eventArgs.OutValue

                If vOut <> IntPtr.Zero AndAlso input IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.BothInOutParamsIllegal))
                End If
                If vOut <> IntPtr.Zero Then
                    ' When vOut is non-NULL, the IDE is requesting the current value for the combo.
                    Marshal.GetNativeVariantForObject(Me.indexComboChoices(Me.currentIndexComboChoice), vOut)

                ElseIf input IsNot Nothing Then
                    Dim newChoice As Integer = -1
                    Try
                        ' User typed a string argument in command window.
                        Dim index As Integer = Integer.Parse(input.ToString(), CultureInfo.CurrentCulture)
                        If index >= 0 AndAlso index < indexComboChoices.Length Then
                            newChoice = index
                        Else
                            Dim errorMessage As String = String.Format(CultureInfo.CurrentCulture, Resources.InvalidIndex, indexComboChoices.Length)
                            Throw (New ArgumentOutOfRangeException(errorMessage))
                        End If
                    Catch e1 As FormatException
                        ' User typed in a non-numeric value, see if it is one of our items.
                        For i As Integer = 0 To indexComboChoices.Length - 1
                            If String.Compare(indexComboChoices(i), input.ToString(), StringComparison.CurrentCultureIgnoreCase) = 0 Then
                                newChoice = i
                                Exit For
                            End If
                        Next i
                    Catch e2 As OverflowException
                        ' User typed in too large of a number, ignore it.
                    End Try

                    ' New value was selected or typed in.
                    If newChoice <> -1 Then
                        Me.currentIndexComboChoice = newChoice
                        ShowMessage(Resources.MyIndexCombo, Me.currentIndexComboChoice.ToString(CultureInfo.CurrentCulture))
                    Else
                        ' Force an exception to be thrown.
                        Throw (New ArgumentException(Resources.ParamMustBeValidIndexOrStringInList))
                    End If
                Else
                    ' We should never get here; EventArgs are required.
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.EventArgsRequired))
                End If
            Else
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If
		End Sub

		' An IndexCombo box requires two commands:
		'    This command is used to retrieve this list of choices for the combo box.
		' 
		' Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
		' enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
		' control the statue of a combo box, e.g. what list of items should be shown and what is the 
		' current value. In order to communicate this information actually IOleCommandTarget::Exec
		' is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
		' QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
		' two commands to retrieve this information. The main command id for the command is used to 
		' retrieve the current value and the second command is used to retrieve the full list of 
		' choices to be displayed as an array of strings.
		Private Sub OnMenuMyIndexComboGetList(ByVal sender As Object, ByVal e As EventArgs)
            If e Is System.EventArgs.Empty Then
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim inParam As Object = eventArgs.InValue
                Dim vOut As IntPtr = eventArgs.OutValue

                If inParam IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.InParamIllegal))
                ElseIf vOut <> IntPtr.Zero Then
                    Marshal.GetNativeVariantForObject(Me.indexComboChoices, vOut)
                Else
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.OutParamRequired))
                End If
            End If
		End Sub

		Private currentMRUComboChoice As String = Nothing

		' MRUCombo
		'   An MRUCOMBO allows the user to type into the edit box. The history of strings entered
		'   is automatically persisted by the IDE on a per-user/per-machine basis. 
		'	 For example, this type of combo is used for the "Find" combo on the "Standard" toolbar.
		'
		'   An MRU Combo box requires only one command:
		'     One command is used to ask for the current value of the combo box and to set the new value when the user
		'     makes a choice in the combo box.
		'
		'     The list of choices entered is automatically remembered by the IDE on a per-user/per-machine basis.
		Private Sub OnMenuMyMRUCombo(ByVal sender As Object, ByVal e As EventArgs)
            If e Is System.EventArgs.Empty Then
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim input As Object = eventArgs.InValue
                Dim vOut As IntPtr = eventArgs.OutValue

                If vOut <> IntPtr.Zero AndAlso input IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.BothInOutParamsIllegal))
                ElseIf vOut <> IntPtr.Zero Then
                    ' When vOut is non-NULL, the IDE is requesting the current value for the combo.
                    Marshal.GetNativeVariantForObject(Me.currentMRUComboChoice, vOut)

                ElseIf input IsNot Nothing Then
                    Dim newChoice As String = input.ToString()

                    ' New value was selected or typed in.
                    If (Not String.IsNullOrEmpty(newChoice)) Then
                        Me.currentMRUComboChoice = newChoice
                        ShowMessage(Resources.MyMRUCombo, Me.currentMRUComboChoice)
                    Else
                        ' We should never get here
                        ' Force an exception to be thrown.
                        Throw (New ArgumentException(Resources.EmptyStringIllegal))
                    End If
                Else
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.BothInOutParamsIllegal))
                End If
            Else
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If
		End Sub

		Private numericZoomLevels As Double() = { 4.0, 3.0, 2.0, 1.5, 1.25, 1.0,.75,.66,.50,.33,.25,.10 }
		Private zoomToFit As String = Resources.ZoomToFit
		Private zoom_to_Fit As String = Resources.Zoom_to_Fit
		Private zoomLevels As String() = Nothing
		Private numberFormatInfo As NumberFormatInfo
		Private currentZoomFactor As Double = 1.0

		' DynamicCombo
		'   A DYNAMICCOMBO allows the user to type into the edit box or pick from the list. The 
		'	 list of choices is usually fixed and is managed by the command handler for the command.
		'	 For example, this type of combo is used for the "Zoom" combo on the "Class Designer" toolbar.
		'
		'   A Combo box requires two commands:
		'     One command is used to ask for the current value of the combo box and to set the new value when the user
		'     makes a choice in the combo box.
		'
		'     The second command is used to retrieve this list of choices for the combo box.
		Private Sub OnMenuMyDynamicCombo(ByVal sender As Object, ByVal e As EventArgs)
            If (Nothing Is e) OrElse (e Is System.EventArgs.Empty) Then
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim input As Object = eventArgs.InValue
                Dim vOut As IntPtr = eventArgs.OutValue

                If vOut <> IntPtr.Zero AndAlso input IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.BothInOutParamsIllegal))
                ElseIf vOut <> IntPtr.Zero Then
                    ' When vOut is non-NULL, the IDE is requesting the current value for the combo.
                    If Me.currentZoomFactor = 0 Then
                        Marshal.GetNativeVariantForObject(Me.zoom_to_Fit, vOut)
                    Else
                        Dim factorString As String = currentZoomFactor.ToString("P0", Me.numberFormatInfo)
                        Marshal.GetNativeVariantForObject(factorString, vOut)
                    End If

                ElseIf input IsNot Nothing Then
                    ' New zoom value was selected or typed in.
                    Dim inputString As String = input.ToString()

                    If inputString.Equals(Me.zoomToFit) OrElse inputString.Equals(Me.zoom_to_Fit) Then
                        currentZoomFactor = 0
                        ShowMessage(Resources.MyDynamicCombo, Me.zoom_to_Fit)
                    Else
                        ' There doesn't appear to be any percent-parsing routines in the framework (even though you can create
                        ' a localized percentage in a string!).  So, we need to remove any occurence of the localized Percent 
                        ' symbol, then parse the value that's left
                        Try
                            Dim newZoom As Single = Single.Parse(inputString.Replace(numberFormatInfo.InvariantInfo.PercentSymbol, ""), CultureInfo.CurrentCulture)

                            newZoom = CSng(Math.Round(newZoom))
                            If newZoom < 0 Then
                                ' Force an exception to be thrown.
                                Throw (New ArgumentException(Resources.ZoomMustBeGTZero))
                            End If

                            currentZoomFactor = newZoom / CSng(100.0)

                            ShowMessage(Resources.MyDynamicCombo, newZoom.ToString(CultureInfo.CurrentCulture))
                        Catch e1 As FormatException
                            ' User typed in a non-numeric value, ignore it.
                        Catch e2 As OverflowException
                            ' User typed in too large of a number, ignore it.
                        End Try
                    End If
                Else
                    ' We should never get here.
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.InOutParamCantBeNULL))
                End If
            Else
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentException(Resources.EventArgsRequired))
            End If
		End Sub

		' A Combo box requires two commands:
		'    This command is used to retrieve this list of choices for the combo box.
		' 
		' Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
		' enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
		' control the statue of a combo box, e.g. what list of items should be shown and what is the 
		' current value. In order to communicate this information actually IOleCommandTarget::Exec
		' is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
		' QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
		' two commands to retrieve this information. The main command id for the command is used to 
		' retrieve the current value and the second command is used to retrieve the full list of 
		' choices to be displayed as an array of strings.
		Private Sub OnMenuMyDynamicComboGetList(ByVal sender As Object, ByVal e As EventArgs)
            If (Nothing Is e) OrElse (e Is System.EventArgs.Empty) Then
                ' We should never get here; EventArgs are required.
                ' Force an exception to be thrown.
                Throw (New ArgumentNullException(Resources.EventArgsRequired))
            End If

			Dim eventArgs As OleMenuCmdEventArgs = TryCast(e, OleMenuCmdEventArgs)

            If eventArgs IsNot Nothing Then
                Dim inParam As Object = eventArgs.InValue
                Dim vOut As IntPtr = eventArgs.OutValue

                If inParam IsNot Nothing Then
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.InParamIllegal))
                ElseIf vOut <> IntPtr.Zero Then
                    ' Initialize the zoom value array if needed.
                    If zoomLevels Is Nothing Then
                        Me.numberFormatInfo = CType(CultureInfo.CurrentUICulture.NumberFormat.Clone(), NumberFormatInfo)
                        If Me.numberFormatInfo.PercentPositivePattern = 0 Then
                            Me.numberFormatInfo.PercentPositivePattern = 1
                        End If
                        If Me.numberFormatInfo.PercentNegativePattern = 0 Then
                            Me.numberFormatInfo.PercentNegativePattern = 1
                        End If

                        zoomLevels = New String(numericZoomLevels.Length) {}
                        For i As Integer = 0 To numericZoomLevels.Length - 1
                            zoomLevels(i) = numericZoomLevels(i).ToString("P0", Me.numberFormatInfo)
                        Next i

                        zoomLevels(zoomLevels.Length - 1) = zoom_to_Fit
                    End If

                    Marshal.GetNativeVariantForObject(zoomLevels, vOut)
                Else
                    ' Force an exception to be thrown.
                    Throw (New ArgumentException(Resources.OutParamRequired))
                End If
            End If
		End Sub
		#End Region

        ' Helper method to show a message box using the SVsUiShell/IVsUiShell service.
		Public Sub ShowMessage(ByVal title As String, ByVal message As String)
			Dim uiShell As IVsUIShell = CType(GetService(GetType(SVsUIShell)), IVsUIShell)
			Dim clsid As Guid = Guid.Empty
			Dim result As Integer = VSConstants.S_OK
			Dim hr As Integer = uiShell.ShowMessageBox(0, clsid, title, message, Nothing, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_INFO, 0, result)
			ErrorHandler.ThrowOnFailure(hr)
		End Sub
	End Class
End Namespace