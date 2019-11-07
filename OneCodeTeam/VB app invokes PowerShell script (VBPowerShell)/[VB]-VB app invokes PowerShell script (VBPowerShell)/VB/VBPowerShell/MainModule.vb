'****************************** Module Header ******************************'
' Module Name:    MainModule.vb
' Project:        VBPowerShell
' Copyright (c) Microsoft Corporation.
' 
' This sample indicates how to call Powershell from VB.NET language. It first
' creats a Runspace object in System.Management.Automation namespace. Then 
' it creats a Pipeline from Runspace. The Pipeline is used to host a line of
' commands which are supposed to be executed. The example call Get-Process 
' command to get all processes whose name are started with "V"
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.Management.Automation
Imports System.Management.Automation.Runspaces
Imports System.Collections.ObjectModel

Module MainModule

    Sub Main()

        ' Create a RunSpace to host the Powershell script environment using 
        ' RunspaceFactory.CreateRunSpace
        Dim runSpace As Runspace = RunspaceFactory.CreateRunspace()
        runSpace.Open()

        ' Create a Pipeline to host commands to be executed using 
        ' Runspace.CreatePipeline
        Dim pipeLine As Pipeline = runSpace.CreatePipeline()

        ' Create a Command object by passing the command to the constructor
        Dim getProcessCStarted As New Command("Get-Process")

        ' Add parameters to the Command. 
        getProcessCStarted.Parameters.Add("name", "V*")

        ' Add the commands to the Pipeline
        pipeLine.Commands.Add(getProcessCStarted)

        ' Run all commands in the current pipeline by calling Pipeline.Invoke. 
        ' It returns a System.Collections.ObjectModel.Collection object
        ' In this example, the executed script is "Get-Process -name V*".
        Dim vNameProcesses As Collection(Of PSObject) = pipeLine.Invoke()
        Dim psObject As PSObject
        For Each psObject In vNameProcesses
            Dim process As Process = TryCast(psObject.BaseObject, Process)
            Console.WriteLine("Process Name: {0}", process.ProcessName)
        Next

    End Sub

End Module
