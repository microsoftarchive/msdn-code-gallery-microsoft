'***************************** Module Header *******************************\
' Module Name:  XpsDetails.vb
' Project:      EditXPS
' Copyright (c) Microsoft Corporation.
' 
' XpsDetails is a calss structure to maintain the XPS resoruces in list. 
' This helps us to copy the XPS resource and content from the source in 
' structured way.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/


Imports System.Collections.Generic
Imports System.Windows.Xps.Packaging

''' <summary>
''' Class to represent the basic properties we use from XPS files.
''' </summary>
Public Class XpsDetails
    Public Property resource() As XpsResource
        Get
            Return m_resource
        End Get
        Set(value As XpsResource)
            m_resource = value
        End Set
    End Property
    Private m_resource As XpsResource
    Public Property sourceURI() As Uri
        Get
            Return m_sourceURI
        End Get
        Set(value As Uri)
            m_sourceURI = value
        End Set
    End Property
    Private m_sourceURI As Uri
    Public Property destURI() As Uri
        Get
            Return m_destURI
        End Get
        Set(value As Uri)
            m_destURI = value
        End Set
    End Property
    Private m_destURI As Uri
End Class
