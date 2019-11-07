Imports System.Collections.ObjectModel
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Windows.Forms

<Extension()> _
Module ExpressionTreeExtension

    <Extension()> _
    Function ExtractName(ByVal name As String) As String
        Dim i = name.LastIndexOf("`")
        If i > 0 Then name = name.Substring(0, i)
        Return name
    End Function

    <Extension()> _
    Function ExtractGenericArguments(ByVal names() As Type) As String
        Dim builder = New StringBuilder("<")
        For Each genericArgument In names
            If builder.Length <> 1 Then builder.Append(", ")
            builder.Append(ObtainOriginalName(genericArgument))
        Next
        builder.Append(">")
        Return builder.ToString()
    End Function

    <Extension()> _
    Function ObtainOriginalName(ByVal type As Type) As String
        If Not type.IsGenericType Then
            Return type.Name
        Else
            Return ExtractName(type.Name) & ExtractGenericArguments(type.GetGenericArguments())
        End If
    End Function

    <Extension()> _
    Function ObtainOriginalMethodName(ByVal method As MethodInfo) As String
        If Not method.IsGenericMethod Then
            Return method.Name
        Else
            Return ExtractName(method.Name) & ExtractGenericArguments(method.GetGenericArguments())
        End If
    End Function

End Module

<Serializable()> _
Public Class ExpressionTreeNode
    Inherits TreeNode

    Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    Public Sub New(ByVal value As Object)
        Dim type = value.GetType()
        Text = type.ObtainOriginalName()

        If TypeOf value Is Expression Then
            ImageIndex = 2
            SelectedImageIndex = 2

            Dim propertyInfos() As PropertyInfo = Nothing
            If type.IsGenericType AndAlso type.GetGenericTypeDefinition() Is GetType(Expression(Of )) Then
                propertyInfos = type.BaseType.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Else
                propertyInfos = type.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            End If

            For Each propertyInfo In propertyInfos
                If propertyInfo.Name <> "nodeType" Then
                    Nodes.Add(New AttributeNode(value, propertyInfo))
                End If
            Next
        Else
            ImageIndex = 4
            SelectedImageIndex = 4
            Text = """" & value.ToString() & """"
        End If
    End Sub

    Protected Overrides Sub Serialize(ByVal si As SerializationInfo, ByVal context As StreamingContext)
        MyBase.Serialize(si, context)
    End Sub

End Class

<Serializable()> _
Public Class AttributeNode
    Inherits TreeNode

    Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        MyBase.new(info, context)
    End Sub

    Public Sub New(ByVal attribute As Object, ByVal propertyInfo As PropertyInfo)
        Text = propertyInfo.Name & " : " & propertyInfo.PropertyType.ObtainOriginalName()
        ImageIndex = 3
        SelectedImageIndex = 3

        Dim value = propertyInfo.GetValue(attribute, Nothing)
        If value IsNot Nothing Then
            If value.GetType().IsGenericType AndAlso value.GetType().GetGenericTypeDefinition() Is GetType(ReadOnlyCollection(Of )) Then
                If value.GetType().InvokeMember("get_Count", BindingFlags.InvokeMethod, Nothing, value, Nothing) = 0 Then
                    Text &= " : Empty"
                Else
                    For Each tree In value
                        If TypeOf tree Is Expression Then
                            Nodes.Add(New ExpressionTreeNode(tree))
                        ElseIf TypeOf tree Is MemberAssignment Then
                            Nodes.Add(New ExpressionTreeNode(CType(tree, MemberAssignment).Expression))
                        End If
                    Next
                End If
            ElseIf TypeOf value Is Expression Then
                Text &= CType(value, Expression).NodeType
                Nodes.Add(New ExpressionTreeNode(value))
            ElseIf TypeOf value Is MethodInfo Then
                Text &= " : """ & CType(value, MethodInfo).ObtainOriginalMethodName() & """"
            ElseIf TypeOf value Is Type Then
                Text &= " : """ & CType(value, Type).ObtainOriginalName() & """"
            Else
                Text &= " : """ & value.ToString() & """"
            End If
        Else
            Text &= " : Nothing"
        End If
    End Sub

    Protected Overrides Sub Serialize(ByVal si As SerializationInfo, ByVal context As StreamingContext)
        MyBase.Serialize(si, context)
    End Sub

End Class