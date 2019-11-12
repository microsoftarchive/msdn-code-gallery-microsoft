'**************************** Module Header ******************************\
' Module Name:	DynamicObjectTableEntity.vb
' Project:		VBAzureDynamicTableEntity
' Copyright (c) Microsoft Corporation.
' 
' This sample shows how to define properties at the run time which will be 
' added to the table when inserting the entities.
' Windows Azure table has flexible schema, so we needn't to define an entity 
' class to serialize the entity.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************/
Imports Microsoft.Data.Edm.Library
Imports System.Dynamic
Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Table
Public Class DynamicObjectTableEntity
    Inherits DynamicObject
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

    Public Sub New(PartitionKey As String, RowKey As String)
        Me.PartitionKey = PartitionKey
        Me.RowKey = RowKey
        properties = New Dictionary(Of String, EntityProperty)()
    End Sub

#Region "override DynamicObject's mehtods"
    Public Overrides Function TryGetMember(binder As GetMemberBinder, ByRef result As Object) As Boolean
        If Not properties.ContainsKey(binder.Name) Then
            properties.Add(binder.Name, ConvertToEntityProperty(binder.Name, Nothing))
        End If
        result = properties(binder.Name)
        Return True
    End Function

    Public Overrides Function TrySetMember(binder As SetMemberBinder, value As Object) As Boolean
        Dim [property] As EntityProperty = ConvertToEntityProperty(binder.Name, value)

        If properties.ContainsKey(binder.Name) Then
            properties(binder.Name) = [property]
        Else
            properties.Add(binder.Name, [property])
        End If

        Return True
    End Function

#End Region

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
    Private Function ConvertToEntityProperty(key As String, value As Object) As EntityProperty
        If value Is Nothing Then
            Return New EntityProperty(DirectCast(Nothing, String))
        End If
        If value.[GetType]() Is GetType(Byte()) Then
            Return New EntityProperty(DirectCast(value, Byte()))
        End If
        If value.[GetType]() Is GetType(Boolean) Then
            Return New EntityProperty(CBool(value))
        End If
        If value.[GetType]() Is GetType(DateTimeOffset) Then
            Return New EntityProperty(DirectCast(value, DateTimeOffset))
        End If
        If value.[GetType]() Is GetType(DateTime) Then
            Return New EntityProperty(DirectCast(value, DateTime))
        End If
        If value.[GetType]() Is GetType(Double) Then
            Return New EntityProperty(CDbl(value))
        End If
        If value.[GetType]() Is GetType(Guid) Then
            Return New EntityProperty(DirectCast(value, Guid))
        End If
        If value.[GetType]() Is GetType(Integer) Then
            Return New EntityProperty(CInt(value))
        End If
        If value.[GetType]() Is GetType(Long) Then
            Return New EntityProperty(CLng(value))
        End If
        If value.[GetType]() Is GetType(String) Then
            Return New EntityProperty(DirectCast(value, String))
        End If
        Throw New Exception("This value type" & Convert.ToString(value.[GetType]()) & " for " & key)
        Throw New Exception(String.Format("This value type {0} is not supported for {1}", key))
    End Function

    ''' <summary>
    ''' Get the edm type, if the type is not a edm type throw a exception.
    ''' </summary>
    Private Overloads Function [GetType](edmType__1 As Microsoft.WindowsAzure.Storage.Table.EdmType) As Type
        Select Case edmType__1
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.Binary
                Return GetType(Byte())
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.[Boolean]
                Return GetType(Boolean)
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.DateTime
                Return GetType(DateTime)
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.[Double]
                Return GetType(Double)
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.Guid
                Return GetType(Guid)
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.Int32
                Return GetType(Integer)
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.Int64
                Return GetType(Long)
            Case Microsoft.WindowsAzure.Storage.Table.EdmType.[String]
                Return GetType(String)
            Case Else
                Throw New TypeLoadException(String.Format("not supported edmType:{0}", edmType__1))
        End Select
    End Function


End Class
