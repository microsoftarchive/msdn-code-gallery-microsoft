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
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
<System.Diagnostics.DebuggerStepThrough(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")> _
Friend Class BaseAccessor

	Protected m_privateObject As Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject

	Protected Sub New(ByVal target As Object, ByVal type As Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType)
		m_privateObject = New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(target, type)
	End Sub

	Protected Sub New(ByVal type As Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType)
		Me.New(Nothing, type)
	End Sub

	Friend Overridable ReadOnly Property Target() As Object
		Get
			Return m_privateObject.Target
		End Get
	End Property

	Public Overrides Function ToString() As String
		Return Me.Target.ToString()
	End Function

	Public Overrides Overloads Function Equals(ByVal obj As Object) As Boolean
		If GetType(BaseAccessor).IsInstanceOfType(obj) Then
			obj = (CType(obj, BaseAccessor)).Target
		End If
		Return Me.Target.Equals(obj)
	End Function

	Public Overrides Function GetHashCode() As Integer
		Return Me.Target.GetHashCode()
	End Function
End Class


<System.Diagnostics.DebuggerStepThrough(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")> _
Friend Class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPagePackageAccessor
	Inherits BaseAccessor

        Protected Shared m_privateType As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(GetType(Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackageVB))

        Friend Sub New(ByVal target As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackageVB)
            MyBase.New(target, m_privateType)
        End Sub

	Friend Sub Initialize()
            Dim args As Object() = New Object() {}
		m_privateObject.Invoke("Initialize", New System.Type(){}, args)
	End Sub
End Class
<System.Diagnostics.DebuggerStepThrough(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")> _
Friend Class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageCustomAccessor
	Inherits BaseAccessor

        Protected Shared m_privateType As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(GetType(Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom))

	Friend Sub New(ByVal target As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom)
		MyBase.New(target, m_privateType)
	End Sub

	Friend Property selectedImagePath() As String
		Get
			Dim ret As String = (CStr(m_privateObject.GetField("selectedImagePath")))
			Return ret
		End Get
		Set(ByVal value As String)
			m_privateObject.SetField("selectedImagePath", value)
		End Set
	End Property

	Friend ReadOnly Property Window() As Global.System.Windows.Forms.IWin32Window
		Get
			Dim ret As Global.System.Windows.Forms.IWin32Window = (CType(m_privateObject.GetProperty("Window"), Global.System.Windows.Forms.IWin32Window))
			Return ret
		End Get
	End Property
End Class
<System.Diagnostics.DebuggerStepThrough(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")> _
Friend Class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor
	Inherits BaseAccessor

        Protected Shared m_privateType As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(GetType(Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsCompositeControl))

	Friend Sub New(ByVal target As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsCompositeControl)
		MyBase.New(target, m_privateType)
	End Sub

	Friend Property pictureBox() As Global.System.Windows.Forms.PictureBox
		Get
			Dim ret As Global.System.Windows.Forms.PictureBox = (CType(m_privateObject.GetField("pictureBox"), Global.System.Windows.Forms.PictureBox))
			Return ret
		End Get
		Set(ByVal value As Global.System.Windows.Forms.PictureBox)
			m_privateObject.SetField("pictureBox", value)
		End Set
	End Property

	Friend Property openImageFileDialog() As Global.System.Windows.Forms.OpenFileDialog
		Get
			Dim ret As Global.System.Windows.Forms.OpenFileDialog = (CType(m_privateObject.GetField("openImageFileDialog"), Global.System.Windows.Forms.OpenFileDialog))
			Return ret
		End Get
		Set(ByVal value As Global.System.Windows.Forms.OpenFileDialog)
			m_privateObject.SetField("openImageFileDialog", value)
		End Set
	End Property

	Friend Property buttonChooseImage() As Global.System.Windows.Forms.Button
		Get
			Dim ret As Global.System.Windows.Forms.Button = (CType(m_privateObject.GetField("buttonChooseImage"), Global.System.Windows.Forms.Button))
			Return ret
		End Get
		Set(ByVal value As Global.System.Windows.Forms.Button)
			m_privateObject.SetField("buttonChooseImage", value)
		End Set
	End Property

	Friend Property buttonClearImage() As Global.System.Windows.Forms.Button
		Get
			Dim ret As Global.System.Windows.Forms.Button = (CType(m_privateObject.GetField("buttonClearImage"), Global.System.Windows.Forms.Button))
			Return ret
		End Get
		Set(ByVal value As Global.System.Windows.Forms.Button)
			m_privateObject.SetField("buttonClearImage", value)
		End Set
	End Property

	Friend Property customOptionsPage() As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom
		Get
			Dim ret As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom = (CType(m_privateObject.GetField("customOptionsPage"), Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom))
			Return ret
		End Get
		Set(ByVal value As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageCustom)
			m_privateObject.SetField("customOptionsPage", value)
		End Set
	End Property

	Friend Property components() As Global.System.ComponentModel.Container
		Get
			Dim ret As Global.System.ComponentModel.Container = (CType(m_privateObject.GetField("components"), Global.System.ComponentModel.Container))
			Return ret
		End Get
		Set(ByVal value As Global.System.ComponentModel.Container)
			m_privateObject.SetField("components", value)
		End Set
	End Property

	Friend Sub Dispose(ByVal disposing As Boolean)
            Dim args As Object() = New Object() {disposing}
		m_privateObject.Invoke("Dispose", New System.Type() { GetType(Boolean)}, args)
	End Sub

	Friend Sub InitializeComponent()
            Dim args As Object() = New Object() {}
		m_privateObject.Invoke("InitializeComponent", New System.Type(){}, args)
	End Sub

	Friend Sub OnChooseImage(ByVal sender As Object, ByVal e As Global.System.EventArgs)
            Dim args As Object() = New Object() {sender, e}
		m_privateObject.Invoke("OnChooseImage", New System.Type() { GetType(Object), GetType(Global.System.EventArgs)}, args)
	End Sub

	Friend Sub OnClearImage(ByVal sender As Object, ByVal e As Global.System.EventArgs)
		Dim args As Object() = New Object() { sender, e}
		m_privateObject.Invoke("OnClearImage", New System.Type() { GetType(Object), GetType(Global.System.EventArgs)}, args)
	End Sub

	Friend Sub RefreshImage()
		Dim args As Object() = New Object(){}
		m_privateObject.Invoke("RefreshImage", New System.Type(){}, args)
	End Sub
End Class
<System.Diagnostics.DebuggerStepThrough(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")> _
Friend Class Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor
	Inherits BaseAccessor

        Protected Shared m_privateType As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(GetType(Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageGeneral))

	Friend Sub New(ByVal target As Global.Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPageGeneral)
		MyBase.New(target, m_privateType)
	End Sub

	Friend Property optionCustomInteger() As Integer
		Get
			Dim ret As Integer = (CInt(Fix(m_privateObject.GetField("optionCustomInteger"))))
			Return ret
		End Get
		Set(ByVal value As Integer)
			m_privateObject.SetField("optionCustomInteger", value)
		End Set
	End Property

	Friend Property optionCustomSize() As Global.System.Drawing.Size
		Get
			Dim ret As Global.System.Drawing.Size = (CType(m_privateObject.GetField("optionCustomSize"), Global.System.Drawing.Size))
			Return ret
		End Get
		Set(ByVal value As Global.System.Drawing.Size)
			m_privateObject.SetField("optionCustomSize", value)
		End Set
	End Property

	Friend Property optionCustomString() As String
		Get
			Dim ret As String = (CStr(m_privateObject.GetField("optionCustomString")))
			Return ret
		End Get
		Set(ByVal value As String)
			m_privateObject.SetField("optionCustomString", value)
		End Set
	End Property

	Friend Sub OnActivate(ByVal e As Global.System.ComponentModel.CancelEventArgs)
		Dim args As Object() = New Object() { e}
		m_privateObject.Invoke("OnActivate", New System.Type() { GetType(Global.System.ComponentModel.CancelEventArgs)}, args)
	End Sub

	Friend Sub OnClosed(ByVal e As Global.System.EventArgs)
		Dim args As Object() = New Object() { e}
		m_privateObject.Invoke("OnClosed", New System.Type() { GetType(Global.System.EventArgs)}, args)
	End Sub

	Friend Sub OnDeactivate(ByVal e As Global.System.ComponentModel.CancelEventArgs)
		Dim args As Object() = New Object() { e}
		m_privateObject.Invoke("OnDeactivate", New System.Type() { GetType(Global.System.ComponentModel.CancelEventArgs)}, args)
	End Sub

	Friend Sub OnApply(ByVal e As Microsoft_VisualStudio_Shell_DialogPage_PageApplyEventArgsAccessor)
		Dim e_val_target As Object = Nothing
            If (e IsNot Nothing) Then
                e_val_target = e.Target
            End If
		Dim args As Object() = New Object() { e_val_target}
            Dim target As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("Microsoft.VisualStudio.Shell", "Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs")
		m_privateObject.Invoke("OnApply", New System.Type() { target.ReferencedType}, args)
	End Sub
End Class
<System.Diagnostics.DebuggerStepThrough(), System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")> _
Friend Class Microsoft_VisualStudio_Shell_DialogPage_PageApplyEventArgsAccessor
	Inherits BaseAccessor

        Protected Shared m_privateType As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("Microsoft.VisualStudio.Shell", "Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs")

	Friend Sub New(ByVal target As Object)
		MyBase.New(target, m_privateType)
	End Sub

	Friend Property ApplyBehavior() As Global.Microsoft.VisualStudio.Shell.DialogPage.ApplyKind
		Get
			Dim ret As Global.Microsoft.VisualStudio.Shell.DialogPage.ApplyKind = (CType(m_privateObject.GetProperty("ApplyBehavior"), Global.Microsoft.VisualStudio.Shell.DialogPage.ApplyKind))
			Return ret
		End Get
		Set(ByVal value As Global.Microsoft.VisualStudio.Shell.DialogPage.ApplyKind)
			m_privateObject.SetProperty("ApplyBehavior", value)
		End Set
	End Property

	Friend Shared Function CreatePrivate() As Global.System.EventArgs
		Dim args As Object() = New Object(){}
            Dim priv_obj As New Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject("Microsoft.VisualStudio.Shell", "Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs", New System.Type() {}, args)
		Return (CType(priv_obj.Target, Global.System.EventArgs))
	End Function
End Class
End Namespace
