'****************************** Module Header ******************************\
' Module Name:  ChartSubArea.vb
' Project:      VBOpenXmlCreateChartInWord
' Copyright(c)  Microsoft Corporation.
' 
' The Class is an Entity class.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports DocumentFormat.OpenXml.Drawing

Public Class ChartSubArea
    Public Property Color() As SchemeColorValues
        Get
            Return m_Color
        End Get
        Set(value As SchemeColorValues)
            m_Color = Value
        End Set
    End Property
    Private m_Color As SchemeColorValues
    Public Property Label() As [String]
        Get
            Return m_Label
        End Get
        Set(value As [String])
            m_Label = Value
        End Set
    End Property
    Private m_Label As [String]
    Public Property Value() As String
        Get
            Return m_Value
        End Get
        Set(value As String)
            m_Value = Value
        End Set
    End Property
    Private m_Value As String
End Class
