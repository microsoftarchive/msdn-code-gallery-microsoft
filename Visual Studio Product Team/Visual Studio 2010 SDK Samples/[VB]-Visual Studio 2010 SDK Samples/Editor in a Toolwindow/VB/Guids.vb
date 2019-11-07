' Guids.cs
' MUST match guids.h

Namespace Samples.EditorToolwindow

	Friend NotInheritable Class GuidList

		Public Const guidEditorToolwindowPkgString As String = "08cf7bd4-9cb6-4b32-b795-b932c001b016"
		Public Const guidEditorToolwindowCmdSetString As String = "bf87e998-e9f8-4d3b-bb38-5a660a27ba1d"
		Public Const guidToolWindowPersistanceString As String = "e3165f5f-3dd7-441f-9e5d-0f4233eb2743"

		Public Shared ReadOnly guidEditorToolwindowCmdSet As New Guid(guidEditorToolwindowCmdSetString)

	End Class

End Namespace