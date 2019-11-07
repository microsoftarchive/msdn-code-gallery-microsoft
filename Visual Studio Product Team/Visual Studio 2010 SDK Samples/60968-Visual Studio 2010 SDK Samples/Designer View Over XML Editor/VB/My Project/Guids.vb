' Guids.vb
' MUST match guids.h

Namespace Microsoft.VsTemplateDesigner
	Friend NotInheritable Class GuidList
		Public Const guidVsTemplateDesignerPkgString As String = "28d60403-f5aa-4745-9e52-ac634cbf0c5c"
		Public Const guidVsTemplateDesignerCmdSetString As String = "22de8a49-aa75-49f7-9180-83d225bbc303"
		Public Const guidVsTemplateDesignerEditorFactoryString As String = "6bf3ea12-98bb-41e2-ba01-8662f713d293"

		Public Shared ReadOnly guidVsTemplateDesignerCmdSet As New Guid(guidVsTemplateDesignerCmdSetString)
		Public Shared ReadOnly guidVsTemplateDesignerEditorFactory As New Guid(guidVsTemplateDesignerEditorFactoryString)

		Public Const guidXmlChooserEditorFactory As String = "{32CC8DFA-2D70-49b2-94CD-22D57349B778}"
	End Class
End Namespace