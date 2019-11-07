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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockBuildEvents
        Implements EnvDTE.BuildEvents
        Public ReadOnly Property OnBuildBeginSubscriberCount() As Integer
            Get
                If (OnBuildBeginEvent Is Nothing) Then
                    Return 0
                Else
                    Return OnBuildBeginEvent.GetInvocationList().Length
                End If
            End Get
        End Property

        Public Sub FireOnBuildBegin(ByVal scope As EnvDTE.vsBuildScope, ByVal action As EnvDTE.vsBuildAction)
            If OnBuildBeginEvent IsNot Nothing Then
                RaiseEvent OnBuildBegin(scope, action)
            End If
        End Sub

        Public Sub FireOnBuildDone(ByVal scope As EnvDTE.vsBuildScope, ByVal action As EnvDTE.vsBuildAction)
            If OnBuildDoneEvent IsNot Nothing Then
                RaiseEvent OnBuildDone(scope, action)
            End If
        End Sub

#Region "_dispBuildEvents_Event Members"

        Public Event OnBuildBegin As EnvDTE._dispBuildEvents_OnBuildBeginEventHandler Implements EnvDTE._dispBuildEvents_Event.OnBuildBegin

        Public Event OnBuildDone As EnvDTE._dispBuildEvents_OnBuildDoneEventHandler Implements EnvDTE._dispBuildEvents_Event.OnBuildDone

        ' These are unused currently, but they must exist to satisfy the interface contract.
        ' Disable the warning for unused variables.
        Public Event OnBuildProjConfigBegin As EnvDTE._dispBuildEvents_OnBuildProjConfigBeginEventHandler Implements EnvDTE._dispBuildEvents_Event.OnBuildProjConfigBegin

        Public Event OnBuildProjConfigDone As EnvDTE._dispBuildEvents_OnBuildProjConfigDoneEventHandler Implements EnvDTE._dispBuildEvents_Event.OnBuildProjConfigDone

#End Region
    End Class
End Namespace
