'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Drawing.Design
Imports System.Runtime.Serialization
Imports System.ComponentModel.Design
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Shell.Interop

''' <summary>
''' This custom ToolboxItem displays a simple dialog asking whether to 
''' initialize a certain value.
''' </summary>
<Serializable()> _
Class MyToolboxItem
    Inherits ToolboxItem

    Const IDYES As Integer = 6

    Public Sub New()
    End Sub

    Private Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        Deserialize(info, context)
    End Sub

    Protected Overloads Overrides Function CreateComponentsCore(ByVal host As IDesignerHost, ByVal defaultValues As IDictionary) As IComponent()
        Return RunWizard(host, MyBase.CreateComponentsCore(host, defaultValues))
    End Function

    ''' <summary>
    ''' This method sets various values on the newly created component.
    ''' </summary>
    Private Function RunWizard(ByVal host As IDesignerHost, ByVal comps As IComponent()) As IComponent()
        Dim result As DialogResult = DialogResult.No
        Dim uiShell As IVsUIShell = Nothing
        If Not host Is Nothing Then
            uiShell = DirectCast(host.GetService(GetType(IVsUIShell)), IVsUIShell)
        End If

        ' Always use the UI shell service to show a message box if possible
        ' There are also some useful helper methods for this in VsShellUtilities.
        If Not uiShell Is Nothing Then
            Dim nResult As Integer = 0
            Dim emptyGuid As Guid = Guid.Empty

            uiShell.ShowMessageBox(0, emptyGuid, "Question", "Do you want to set the Text property?", Nothing, 0, _
             OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND, OLEMSGICON.OLEMSGICON_QUERY, 0, nResult)

            If nResult = IDYES Then
                result = DialogResult.Yes
            End If
        Else
            result = MessageBox.Show("Do you want to set the Text property?", "Question", MessageBoxButtons.YesNo)
        End If

        If result = DialogResult.Yes Then
            If comps.Length > 0 Then
                ' Use types from the ITypeResolutionService.  Do not use locally defined types.
                Dim typeResolver As ITypeResolutionService = DirectCast(host.GetService(GetType(ITypeResolutionService)), ITypeResolutionService)
                If Not typeResolver Is Nothing Then
                    ' Check to ensure we got the right Type
                    Dim t As Type = typeResolver.GetType(GetType(MyCustomTextBoxWithPopup).FullName)

                    If Not t Is Nothing AndAlso comps(0).GetType().IsAssignableFrom(t) Then
                        ' Use a property descriptor instead of direct property access.
                        ' This will allow the change to appear in the undo stack and it will get
                        ' serialized correctly.
                        Dim pd As PropertyDescriptor = TypeDescriptor.GetProperties(comps(0))("Text")
                        If Not pd Is Nothing Then
                            pd.SetValue(comps(0), "Text Property was initialized!")
                        End If
                    End If
                End If
            End If
        End If

        Return comps
    End Function
End Class
