'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Text
Imports System.Xml
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.Reflection

Namespace Microsoft.Samples.VisualStudio.GeneratorSample
	' In order to be compatible with this single file generator, the input file has to
	' follow the schema in XMLClassGeneratorSchema.xsd

	''' <summary>
	''' Generates source code based on a XML document
	''' </summary>
	Public NotInheritable Class SourceCodeGenerator
		''' <summary>
		''' Create a CodeCompileUnit based on the XmlDocument doc
		''' In order to be compatible with this single file generator, the input XmlDocument has to
		''' follow the schema in XMLClassGeneratorSchema.xsd
        ''' </summary>
        ''' <param name="doc">An XML document that contains the description of the code to be generated</param>
		''' <param name="namespaceName">If the root node of doc does not have a namespace attribute, use this instead</param>
        ''' <returns>The generated CodeCompileUnit</returns>
		
        Public Shared Function CreateCodeCompileUnit(ByVal doc As XmlDocument,
                                                     ByVal namespaceName As String) As CodeCompileUnit
            Dim root As XmlElement = doc.DocumentElement

            If root.Name <> "Types" Then
                Throw New ArgumentException(String.Format(Strings.InvalidRoot, root.Name))
            End If

            If root.ChildNodes.Count = 0 Then
                Throw New ArgumentException(Strings.NoTypes)
            End If

            If root.Attributes.GetNamedItem("namespace") IsNot Nothing Then
                namespaceName = root.Attributes.GetNamedItem("namespace").Value
            End If

            Dim code As New CodeCompileUnit()

            ' Just for VB.NET:
            ' Option Strict On (controls whether implicit type conversions are allowed)
            code.UserData.Add("AllowLateBound", False)
            ' Option Explicit On (controls whether variable declarations are required)
            code.UserData.Add("RequireVariableDeclaration", True)

            Dim codeNamespace_local As New CodeNamespace(namespaceName)

            For Each node As XmlNode In root.ChildNodes
                Select Case node.Name
                    Case "Class"
                        codeNamespace_local.Types.Add(CreateClass(node))
                    Case "Enum"
                        codeNamespace_local.Types.Add(CreateEnum(node))
                    Case Else
                        Throw New ArgumentException(String.Format(Strings.InvalidType, node.Name))
                End Select
            Next node

            code.Namespaces.Add(codeNamespace_local)
            Return code
        End Function

		Private Shared Function CreateClass(ByVal node As XmlNode) As CodeTypeDeclaration
            Dim className As String = Nothing
            Dim classAccess As String = Nothing
			Dim isPartial As Boolean

			GetClassInfo(node, className, classAccess, isPartial)

			Dim typeDeclaration As New CodeTypeDeclaration(className)

			Dim attrib As TypeAttributes

			Select Case classAccess
				Case "public"
					attrib = TypeAttributes.Public
				Case "internal"
					attrib = TypeAttributes.NotPublic
				Case Else
					Throw New ArgumentException(String.Format(Strings.BadTypeAccess, classAccess))
			End Select
            With typeDeclaration
                .IsPartial = isPartial
                .IsClass = True
                .TypeAttributes = attrib Or TypeAttributes.Class
            End With
            For Each n As XmlNode In node.ChildNodes
                Dim field As CodeMemberField = CreateField(n)
                typeDeclaration.Members.Add(field)
            Next n

            Return typeDeclaration
        End Function

        Private Shared Sub GetClassInfo(ByVal node As XmlNode,
                                        <System.Runtime.InteropServices.Out()> ByRef className As String,
                                        <System.Runtime.InteropServices.Out()> ByRef classAccess As String,
                                        <System.Runtime.InteropServices.Out()> ByRef isPartial As Boolean)

            If node.Attributes IsNot Nothing AndAlso node.Attributes.GetNamedItem("name") IsNot Nothing AndAlso node.Attributes.GetNamedItem("name").Value <> String.Empty Then
                className = node.Attributes.GetNamedItem("name").Value
            Else
                Throw New ArgumentException(Strings.ClassNodeNoName)
            End If

            If node.Attributes.GetNamedItem("access") IsNot Nothing Then
                classAccess = node.Attributes.GetNamedItem("access").Value
            Else
                classAccess = "public"
            End If

            If node.Attributes.GetNamedItem("partial") IsNot Nothing Then
                isPartial = Boolean.Parse(node.Attributes.GetNamedItem("partial").Value)
            Else
                isPartial = False
            End If
        End Sub

		Private Shared Function CreateField(ByVal n As XmlNode) As CodeMemberField
            Dim fieldName As String = Nothing
            Dim fieldAccess As String = Nothing
            Dim fieldValue As String = Nothing
            Dim fieldType As String = Nothing
			Dim fieldIsStatic As Boolean

			GetFieldInfo(n, fieldName, fieldAccess, fieldValue, fieldType, fieldIsStatic)

			Dim field As New CodeMemberField(fieldType, fieldName)

			If n.Name = "Constant" Then
				' Set the correct attributes for a constant field  
                field.Attributes = (field.Attributes And (Not MemberAttributes.AccessMask) And
                                    (Not MemberAttributes.ScopeMask)) Or MemberAttributes.Public Or MemberAttributes.Const

				If fieldType = "System.String" Then
					field.InitExpression = New CodePrimitiveExpression(fieldValue)
				Else
					field.InitExpression = New CodeSnippetExpression(fieldValue)
				End If
			Else
				Select Case fieldAccess
					Case "public"
						field.Attributes = MemberAttributes.Public
					Case "protected"
						field.Attributes = MemberAttributes.Family
					Case "private"
						field.Attributes = MemberAttributes.Private
					Case Else
						Throw New ArgumentException(String.Format(Strings.BadVariableAccess, fieldAccess))
				End Select

				If fieldIsStatic Then
					field.Attributes = field.Attributes Or MemberAttributes.Static
				End If
			End If

			Return field
		End Function

        Private Shared Sub GetFieldInfo(ByVal n As XmlNode, <System.Runtime.InteropServices.Out()> ByRef memberName As String,
                                        <System.Runtime.InteropServices.Out()> ByRef memberAccess As String,
                                        <System.Runtime.InteropServices.Out()> ByRef memberValue As String,
                                        <System.Runtime.InteropServices.Out()> ByRef memberType As String,
                                        <System.Runtime.InteropServices.Out()> ByRef memberIsStatic As Boolean)
            If n.Name <> "Variable" AndAlso n.Name <> "Constant" Then
                Throw New ArgumentException(String.Format(Strings.BadClassMemberName, n.Name))
            End If

            If n.Attributes IsNot Nothing AndAlso n.Attributes.GetNamedItem("name") IsNot Nothing AndAlso n.Attributes.GetNamedItem("name").Value <> String.Empty Then
                memberName = n.Attributes.GetNamedItem("name").Value
            Else
                Throw New ArgumentException(Strings.ClassMemberNoName)
            End If

            If n.Attributes.GetNamedItem("type") IsNot Nothing Then
                memberType = n.Attributes.GetNamedItem("type").Value
            Else
                Throw New ArgumentException(Strings.ClassMemberNoType)
            End If

            If n.Attributes.GetNamedItem("value") IsNot Nothing Then
                memberValue = n.Attributes.GetNamedItem("value").Value
            Else
                If n.Name = "Constant" Then
                    Throw New ArgumentException(Strings.ConstantNoValue)
                Else
                    memberValue = Nothing
                End If
            End If

            If n.Attributes.GetNamedItem("access") IsNot Nothing Then
                memberAccess = n.Attributes.GetNamedItem("access").Value
            Else
                memberAccess = "public"
            End If

            If n.Attributes.GetNamedItem("static") IsNot Nothing Then
                If n.Name = "Constant" Then
                    Throw New ArgumentException(Strings.ConstantNoValue)
                Else
                    memberIsStatic = Boolean.Parse(n.Attributes.GetNamedItem("static").Value)
                End If
            Else
                memberIsStatic = False
            End If
        End Sub

		Private Shared Function CreateEnum(ByVal node As XmlNode) As CodeTypeDeclaration
            Dim enumName As String = Nothing
            Dim enumAccess As String = Nothing
			Dim enumFlagsAttribute As Boolean

			GetEnumInfo(node, enumName, enumAccess, enumFlagsAttribute)

			Dim typeDeclaration As New CodeTypeDeclaration(enumName)
			typeDeclaration.IsEnum = True

			If enumFlagsAttribute Then
				typeDeclaration.CustomAttributes.Add(New CodeAttributeDeclaration("System.FlagsAttribute"))
			End If

			Dim attrib As TypeAttributes
			Select Case enumAccess
				Case "public"
					attrib = TypeAttributes.Public
				Case "internal"
					attrib = TypeAttributes.NotPublic
				Case Else
					Throw New ArgumentException(String.Format(Strings.BadTypeAccess, enumAccess))
			End Select

			typeDeclaration.TypeAttributes = attrib

			For Each n As XmlNode In node.ChildNodes
                Dim memberName As String = Nothing
				Dim memberValue As Integer
				Dim hasValue As Boolean

				GetEnumMemberInfo(enumFlagsAttribute, n, memberName, memberValue, hasValue)

				Dim field As New CodeMemberField("System.Int32", memberName)
				If hasValue Then
					field.InitExpression = New CodePrimitiveExpression(memberValue)
				End If

				typeDeclaration.Members.Add(field)
			Next n

			Return typeDeclaration
		End Function

        Private Shared Sub GetEnumMemberInfo(ByVal enumFlagsAttribute As Boolean, ByVal n As XmlNode,
                                             <System.Runtime.InteropServices.Out()> ByRef memberName As String,
                                             <System.Runtime.InteropServices.Out()> ByRef memberValue As Integer,
                                             <System.Runtime.InteropServices.Out()> ByRef hasValue As Boolean)
            If n.Name <> "EnumMember" Then
                Throw New ArgumentException(String.Format(Strings.EnumNodeNoName, n.Name))
            End If

            If n.Attributes IsNot Nothing AndAlso n.Attributes.GetNamedItem("name") IsNot Nothing AndAlso n.Attributes.GetNamedItem("name").Value <> String.Empty Then
                memberName = n.Attributes.GetNamedItem("name").Value
            Else
                Throw New ArgumentException(Strings.EnumMemberNodeNoName)
            End If

            If n.Attributes.GetNamedItem("value") IsNot Nothing Then
                memberValue = Int32.Parse(n.Attributes.GetNamedItem("value").Value)
                hasValue = True
            Else
                If enumFlagsAttribute Then
                    Throw New ArgumentException(Strings.EnumMemberValueMissing)
                Else
                    memberValue = 0
                    hasValue = False
                End If
            End If
        End Sub

        Private Shared Sub GetEnumInfo(ByVal node As XmlNode, <System.Runtime.InteropServices.Out()> ByRef enumName As String,
                                       <System.Runtime.InteropServices.Out()> ByRef enumAccess As String,
                                       <System.Runtime.InteropServices.Out()> ByRef enumFlagsAttribute As Boolean)
            If node.Attributes IsNot Nothing AndAlso node.Attributes.GetNamedItem("name") IsNot Nothing AndAlso node.Attributes.GetNamedItem("name").Value <> String.Empty Then
                enumName = node.Attributes.GetNamedItem("name").Value
            Else
                Throw New ArgumentException(Strings.EnumNodeNoName)
            End If

            If node.ChildNodes.Count = 0 Then
                Throw New ArgumentException(Strings.EnumNoMembers)
            End If

            If node.Attributes.GetNamedItem("access") IsNot Nothing Then
                enumAccess = node.Attributes.GetNamedItem("access").Value
            Else
                enumAccess = "public"
            End If

            If node.Attributes.GetNamedItem("flagsAttribute") IsNot Nothing Then
                enumFlagsAttribute = Boolean.Parse(node.Attributes.GetNamedItem("flagsAttribute").Value)
            Else
                enumFlagsAttribute = False
            End If
        End Sub
	End Class
End Namespace