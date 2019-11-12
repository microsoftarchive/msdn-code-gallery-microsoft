'/***************************** Module Header ******************************\
' Module Name:	ExcelTableEntity.cs
' Project:		VBAZureBulkImportExportExeclTableStorage
'Copyright (c) Microsoft Corporation.
' 
' This sample shows how to define properties at the run time which will be 
' added to the table when inserting the entities.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************/
Imports Microsoft.WindowsAzure.Storage.Table
Imports Microsoft.WindowsAzure.Storage

Public Class ExcelTableEntity
    Implements ITableEntity
#Region "ITableEntity properties"
    ' Summary:
    '     Gets or sets the entity's current ETag. Set this value to '*' in order to
    '     blindly overwrite an entity as part of an update operation.
    Public Property ETag() As String Implements ITableEntity.ETag
        Get
            Return m_ETag
        End Get
        Set(value As String)
            m_ETag = value
        End Set
    End Property
    Private m_ETag As String

    '
    ' Summary:
    '     Gets or sets the entity's partition key.
    Public Property PartitionKey() As String Implements ITableEntity.PartitionKey
        Get
            Return m_PartitionKey
        End Get
        Set(value As String)
            m_PartitionKey = value
        End Set
    End Property

    Private m_PartitionKey As String

    '
    ' Summary:
    '     Gets or sets the entity's row key.
    Public Property RowKey() As String Implements ITableEntity.RowKey
        Get
            Return m_RowKey
        End Get
        Set(value As String)
            m_RowKey = value
        End Set
    End Property
    Private m_RowKey As String

    '
    ' Summary:
    '     Gets or sets the entity's time stamp.
    Public Property Timestamp() As DateTimeOffset Implements ITableEntity.Timestamp
        Get
            Return m_Timestamp
        End Get
        Set(value As DateTimeOffset)
            m_Timestamp = value
        End Set
    End Property
    Private m_Timestamp As DateTimeOffset

#End Region

    'Use this Dictionary store table's properties. 
    Public Property properties() As IDictionary(Of String, EntityProperty)
        Get
            Return m_properties
        End Get
        Private Set(value As IDictionary(Of String, EntityProperty))
            m_properties = value
        End Set
    End Property
    Private m_properties As IDictionary(Of String, EntityProperty)

    Public Sub New()
        properties = New Dictionary(Of String, EntityProperty)()
    End Sub

    Public Sub New(ByVal PartitionKey As String, ByVal RowKey As String)
        Me.PartitionKey = PartitionKey
        Me.RowKey = RowKey
        properties = New Dictionary(Of String, EntityProperty)()
    End Sub

#Region "ITableEntity implementation"

    Public Sub ReadEntity(properties As IDictionary(Of String, EntityProperty), operationContext As OperationContext) Implements ITableEntity.ReadEntity
        Me.properties = properties
    End Sub

    Public Function WriteEntity(operationContext As OperationContext) As IDictionary(Of String, EntityProperty) Implements ITableEntity.WriteEntity
        Return Me.properties
    End Function

#End Region

    ''' <summary>
    ''' Convert object value to EntityProperty.
    ''' </summary>
    Public Function ConvertToEntityProperty(ByVal key As String, ByVal value As Object) As EntityProperty
        If value Is Nothing Then
            Return New EntityProperty(CStr(Nothing))
        End If
        If value.GetType() Is GetType(Byte()) Then
            Return New EntityProperty(CType(value, Byte()))
        End If
        If value.GetType() Is GetType(Boolean) Then
            Return New EntityProperty(CBool(value))
        End If
        If value.GetType() Is GetType(DateTimeOffset) Then
            Return New EntityProperty(CType(value, DateTimeOffset))
        End If
        If value.GetType() Is GetType(DateTime) Then
            Return New EntityProperty(CDate(value))
        End If
        If value.GetType() Is GetType(Double) Then
            Return New EntityProperty(CDbl(value))
        End If
        If value.GetType() Is GetType(Guid) Then
            Return New EntityProperty(CType(value, Guid))
        End If
        If value.GetType() Is GetType(Integer) Then
            Return New EntityProperty(CInt(Fix(value)))
        End If
        If value.GetType() Is GetType(Long) Then
            Return New EntityProperty(CLng(Fix(value)))
        End If
        If value.GetType() Is GetType(String) Then
            Return New EntityProperty(CStr(value))
        End If
        Throw New Exception("This value type" & Convert.ToString(value.[GetType]()) & " for " & key)
        Throw New Exception(String.Format("This value type {0} is not supported for {1}", key))
    End Function

    ''' <summary>
    ''' Get the edm type, if the type is not a edm type throw a exception.
    ''' </summary>
    Public Overloads Function [GetType](ByVal edmType As EdmType) As Type
        Select Case edmType
            Case edmType.Binary
                Return GetType(Byte())
            Case edmType.Boolean
                Return GetType(Boolean)
            Case edmType.DateTime
                Return GetType(Date)
            Case edmType.Double
                Return GetType(Double)
            Case edmType.Guid
                Return GetType(Guid)
            Case edmType.Int32
                Return GetType(Integer)
            Case edmType.Int64
                Return GetType(Long)
            Case edmType.String
                Return GetType(String)
            Case Else
                Throw New TypeLoadException(String.Format("not supported edmType:{0}", edmType))
        End Select
    End Function
End Class