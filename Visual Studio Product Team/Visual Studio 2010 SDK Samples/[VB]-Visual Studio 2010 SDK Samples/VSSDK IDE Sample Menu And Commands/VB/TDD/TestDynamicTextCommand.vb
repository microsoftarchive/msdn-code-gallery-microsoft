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
Imports System.ComponentModel.Design
Imports System.Reflection

Imports Microsoft.VisualStudio.Shell
Imports Microsoft.Samples.VisualStudio.MenuCommands

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Microsoft.Samples.VisualStudio.MenuCommands.UnitTest

    Friend Class DynamicTextCommandWrapper

        ' ============================================================================================
        '      Static members
        Private Shared dynamicTextConstructor As ConstructorInfo
        Private Shared dynamicTextType As Type
        Public Shared Function GetDynamicTextCommandInstance(ByVal id As CommandID, ByVal text As String) As OleMenuCommand
            ' Check if we have found the constructor before.
            If dynamicTextConstructor Is Nothing Then
                Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetAssembly(GetType(MenuCommandsPackage))
                For Each t As Type In asm.GetTypes()
                    If t.FullName.EndsWith("DynamicTextCommand") Then
                        dynamicTextType = t
                        Exit For
                    End If
                Next t
                Assert.IsNotNull(dynamicTextType, "Can not get the type for DynamicTextCommand.")
                dynamicTextConstructor = dynamicTextType.GetConstructor(New Type() {GetType(CommandID), GetType(String)})
            End If

            ' Now we must have the constructor.
            Assert.IsNotNull(dynamicTextConstructor, "Can not get the constructor for DynamicTextCommand")
            ' Use the constructor to get an instance of the object.
            Dim command As Object = dynamicTextConstructor.Invoke(New Object() {id, text})
            Assert.IsNotNull(command, "Can not create an instance of DynamicTextCommand")
            Dim oleCommand As OleMenuCommand = TryCast(command, OleMenuCommand)
            Assert.IsNotNull(oleCommand, "DynamicTextCommand does not derive from OleMenuCommand")
            Return oleCommand
        End Function
        '      End of static members
        ' ============================================================================================

        Private innerCommand As OleMenuCommand
        Public Sub New(ByVal id As CommandID, ByVal text As String)
            innerCommand = GetDynamicTextCommandInstance(id, text)
        End Sub
    End Class

    <TestClass()> _
    Public Class DynamicTextCommanTest

        <TestMethod()> _
        Public Sub CreateInstance()
            Dim commandGuid As New Guid()
            Dim commandId As Integer = 42
            Dim id As New CommandID(commandGuid, commandId)
            Dim text As String = "Test DynamicTextCommand"
            Dim command As OleMenuCommand = DynamicTextCommandWrapper.GetDynamicTextCommandInstance(id, text)
            Assert.AreEqual(commandGuid, command.CommandID.Guid, "Guid not correct for the DynamicTextCommand.")
            Assert.AreEqual(commandId, command.CommandID.ID, "ID not correct for the DynamicTextCommand.")
        End Sub

        <TestMethod()> _
        Public Sub CheckText()
            Dim command As OleMenuCommand = DynamicTextCommandWrapper.GetDynamicTextCommandInstance(New CommandID(New Guid(), 77), "Test")
            Dim returnedText As String = command.Text
            Assert.AreEqual("Test (Clicked 0 times)", returnedText, "Text not correct before first click.")

            ' Simulate the click calling invoke.
            command.Invoke(Nothing)
            ' Get the new text
            returnedText = command.Text
            Assert.AreEqual("Test (Clicked 1 times)", returnedText, "Text not correct after first click.")
        End Sub
    End Class
End Namespace
