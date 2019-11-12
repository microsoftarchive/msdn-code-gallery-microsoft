'Copyright (C) Microsoft Corporation.  All rights reserved.

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml

' The Reflector component generates an HTML document outlining the public API
' for a given assembly. The task is achieved in two phases. First, an XML 
' document is emitted in memory (see Emit*()). The document captures the 
' relevant aspects of the assembly's metadata. In the second phase, an HTML
' document is extracted from the XML document (see Extract*()). Note that the 
' two phases handle similar information with different representations using 
' the same query constructs.


Public Class Reflector
    Public document As XDocument

    Public Sub Reflect(ByVal assemblyFile As String)
        Dim Assembly As Assembly = Assembly.LoadFrom(assemblyFile)
        document = New XDocument(EmitAssembly(Assembly))
    End Sub

    Public Sub Transform(ByVal writer As XmlWriter)
        If document Is Nothing Then Return
        Dim assembly As XElement = document.Element("assembly")
        Dim transform As New XDocument(ExtractAssembly(assembly))
        transform.Save(writer)
    End Sub

    Public Function EmitAssembly(ByVal assembly As Assembly) As XElement
        Return <assembly name=<%= assembly.ManifestModule.Name %>>
                   <%= From type In assembly.GetTypes() _
                       Where GetVisible(type) _
                       Group type By GetNamespace = GetNamespace(type) Into g = Group _
                       Order By GetNamespace _
                       Select EmitNamespace(GetNamespace, g) %>
               </assembly>
    End Function

    Public Function EmitNamespace(ByVal ns As String, ByVal types As IEnumerable(Of Type)) As XElement
        Return <namespace name=<%= ns %>>
                   <%= From type In types _
                       Order By type.Name _
                       Select EmitType(type) %>
               </namespace>
    End Function

    Public Function EmitType(ByVal type As Type) As XElement
        Return <<%= If(type.IsEnum, "enum", If(type.IsValueType, "struct", If(type.IsInterface, "interface", "class"))) %> name=<%= type.Name %>>
                   <%= If(Not type.IsGenericTypeDefinition, Nothing, EmitGenericArguments(type.GetGenericArguments())) %>
                   <%= EmitModifiers(type) %>
                   <%= EmitExtends(type.BaseType) %>
                   <%= EmitImplements(type.GetInterfaces()) %>
                   <%= EmitDeclaringType(type.DeclaringType) %>
                   <%= EmitNestedTypes(type.GetNestedTypes()) %>
                   <%= EmitMethods(type.GetConstructors()) %>
                   <%= EmitProperties(type.GetProperties()) %>
                   <%= EmitMethods(type.GetMethods()) %>
               </>
    End Function

    Public Function EmitGenericArguments(ByVal args As IEnumerable(Of Type)) As IEnumerable(Of XElement)
        Return From arg In args _
                Select <genericArgument>
                           <%= EmitReference(arg) %>
                       </genericArgument>
    End Function

    Public Function EmitModifiers(ByVal type As Type) As XElement
        Dim builder As StringBuilder = New StringBuilder()
        If type.IsPublic Then
            builder.Append("public")
        ElseIf type.IsNestedPublic Then
            builder.Append("public")
        ElseIf type.IsNestedFamily Then
            builder.Append("protected")
        ElseIf type.IsNestedFamANDAssem Then
            builder.Append("protected internal")
        End If
        If type.IsSealed Then builder.Append(" sealed")
        If type.IsAbstract Then builder.Append(" abstract")

        Return <modifiers>
                   <%= builder %>
               </modifiers>
    End Function

    Public Function EmitExtends(ByVal baseType As Type) As XElement
        If baseType Is Nothing Or baseType Is GetType(Object) Or baseType Is GetType(ValueType) Or baseType Is GetType(System.Enum) Then Return Nothing
        Return <extends>
                   <%= EmitReference(baseType) %>
               </extends>
    End Function

    Public Function EmitImplements(ByVal ifaces As IEnumerable(Of Type)) As IEnumerable(Of XElement)
        Return From iface In ifaces _
               Select <implements>
                          <%= EmitReference(iface) %>
                      </implements>
    End Function

    Public Function EmitDeclaringType(ByVal declaringType As Type) As XElement
        If declaringType Is Nothing Then Return Nothing
        Return <declaringType>
                   <%= EmitReference(declaringType) %>
               </declaringType>
    End Function

    Public Function EmitNestedTypes(ByVal ntypes As IEnumerable(Of Type)) As IEnumerable(Of XElement)
        Return From ntype In ntypes _
                Where GetVisible(ntype) _
                Select EmitType(ntype)
    End Function

    Public Function EmitMethods(ByVal metds As IEnumerable(Of MethodBase)) As IEnumerable(Of XElement)
        Return From metd In metds _
        Where GetVisible(metd) _
        Select <method name=<%= metd.Name %>>
                   <%= If(Not metd.IsGenericMethodDefinition, Nothing, EmitGenericArguments(metd.GetGenericArguments())) %>
                   <%= EmitModifiers(metd) %>
                   <%= EmitReturnType(metd) %>
                   <%= If(Not metd.IsDefined(GetType(ExtensionAttribute), True), Nothing, EmitExtension(metd)) %>
                   <%= EmitParameters(metd.GetParameters()) %>
               </method>
    End Function

    Public Function EmitProperties(ByVal props As IEnumerable(Of PropertyInfo)) As IEnumerable(Of XElement)

        Return From prop In props _
                    Where GetVisible(prop.GetGetMethod()) Or _
                    GetVisible(prop.GetSetMethod()) _
                    Select <property name=<%= prop.Name %>>
                               <propertyType><%= EmitReference(prop.PropertyType) %>
                               </propertyType>
                           </property>

    End Function

    Public Function EmitReference(ByVal type As Type) As IEnumerable(Of Object)

        If (Not type.IsGenericType) Then
            Return New Object() {New XAttribute("name", type.Name), _
                                 New XAttribute("namespace", GetNamespace(type))}
        Else
            Return New Object() {New XAttribute("name", type.Name), _
                                 New XAttribute("namespace", GetNamespace(type)), _
                                 EmitGenericArguments(type.GetGenericArguments())}
        End If
    End Function

    Public Function EmitModifiers(ByVal metd As MethodBase) As XElement
        Dim builder As StringBuilder = New StringBuilder()
        If metd.IsPublic Then
            builder.Append("public")
        ElseIf metd.IsFamily Then
            builder.Append("protected")
        ElseIf metd.IsFamilyAndAssembly Then
            builder.Append("protected internal")
        End If

        If metd.IsAbstract Then builder.Append(" abstract")
        If metd.IsStatic Then builder.Append(" static")
        If metd.IsVirtual Then builder.Append(" virtual")

        Return New XElement("modifiers", builder.ToString())
    End Function

    Public Function EmitReturnType(ByVal metd As MethodBase) As XElement
        Dim metdInfo As MethodInfo = TryCast(metd, MethodInfo)
        If metdInfo Is Nothing Then Return Nothing
        Return <returnType>
                   <%= EmitReference(metdInfo.ReturnType) %>
               </returnType>
    End Function

    Public Function EmitExtension(ByVal metd As MethodBase) As XElement
        Return <extension>
               </extension>
    End Function

    Public Function EmitParameters(ByVal parms As IEnumerable(Of ParameterInfo)) As IEnumerable(Of XElement)
        Return From parm In parms _
               Select <parameter name=<%= parm.Name %>>
                          <parameterType>
                              <%= EmitReference(parm.ParameterType) %>
                          </parameterType>
                      </parameter>
    End Function

    Public Shared Function GetNamespace(ByVal type As Type) As String
        Dim ns As String = type.Namespace
        Return If(ns IsNot Nothing, ns, String.Empty)
    End Function

    Public Shared Function GetVisible(ByVal type As Type) As Boolean
        Return type.IsPublic Or type.IsNestedPublic Or type.IsNestedFamily Or type.IsNestedFamANDAssem
    End Function

    Public Shared Function GetVisible(ByVal metd As MethodBase) As Boolean
        Return metd IsNot Nothing AndAlso (metd.IsPublic Or metd.IsFamily Or metd.IsFamilyAndAssembly)
    End Function

    Public Function ExtractAssembly(ByVal assembly As XElement) As XElement
        Return <html>
                   <head>
                       <title>
                           <%= ExtractName(assembly) %>
                       </title>
                   </head>
                   <body>
                       <div>
                           <h1>Assembly: <%= ExtractName(assembly) %></h1>
                           <%= From ns In assembly.Elements("namespace") _
                               Select ExtractNamespace(ns) %>
                       </div>
                   </body>
               </html>
    End Function

    Public Function ExtractNamespace(ByVal ns As XElement) As XElement
        Return <div>
                   <h2>Namespace: <%= ExtractName(ns) %></h2>
                   <%= From name In New String() {"class", "interface", "struct", "enum"} _
                       Where (ns.Elements(name).Any()) _
                       Select From type In ns.Elements(name) _
                       Where Not type.Elements("declaringType").Any() _
                       Select ExtractType(type) %>
               </div>
    End Function

    Public Function ExtractType(ByVal type As XElement) As XElement
        Return <div>
                   <h3><%= ExtractModifiers(type) + " " %>
                       <%= type.Name.ToString & " " %>
                       <%= ExtractReference(type) %>
                       <%= ExtractInherits(type) %></h3>
                   <%= ExtractConstructors(type) %>
                   <%= ExtractProperties(type) %>
                   <%= ExtractOperators(type) %>
                   <%= ExtractMethods(type) %>
               </div>
    End Function

    Public Function ExtractModifiers(ByVal element As XElement) As String
        Return element.Element("modifiers").Value
    End Function

    Public Function ExtractName(ByVal element As XElement) As String
        Dim name As String = element.Attribute("name").Value
        Dim i As Integer = name.LastIndexOf("`")
        If i > 0 Then name = name.Substring(0, i) ' fix generic name
        Return name
    End Function

    Public Function ExtractGenericArguments(ByVal element As XElement) As String
        If Not element.Elements("genericArgument").Any() Then Return String.Empty
        Dim builder As StringBuilder = New StringBuilder("<")
        For Each genericArgument As XElement In element.Elements("genericArgument")
            If builder.Length <> 1 Then builder.Append(", ")
            builder.Append(ExtractReference(genericArgument))
        Next
        builder.Append(">")
        Return builder.ToString()
    End Function

    Public Function ExtractReference(ByVal element As XElement) As String
        Return ExtractName(element) + ExtractGenericArguments(element)
    End Function

    Public Function ExtractInherits(ByVal type As XElement) As String
        If Not type.Elements("extends").Concat(type.Elements("implements")).Any() Then Return String.Empty
        Dim builder As New StringBuilder()
        For Each [inherits] As XElement In type.Elements("extends").Concat(type.Elements("implements"))
            If builder.Length = 0 Then
                builder.Append(" : ")
            Else
                builder.Append(", ")
            End If
            builder.Append(ExtractReference([inherits]))
        Next
        Return builder.ToString()
    End Function

    Public Function ExtractConstructors(ByVal type As XElement) As XElement
        Dim ctors = From ctor In type.Elements("method") _
                    Where ExtractName(ctor) = ".ctor" _
                    Select <li>
                               <%= ExtractModifiers(ctor) + " " %>
                               <%= ExtractName(type) %>
                               <%= ExtractParameters(ctor) %>
                           </li>
        If Not ctors.Any() Then Return Nothing
        Return <div>
                   <h4>Constructors: </h4>
                   <ul><%= ctors %></ul>
               </div>
    End Function

    Public Function ExtractProperties(ByVal type As XElement) As XElement
        Dim props = From prop In type.Elements("property") _
        Let propName = ExtractName(prop) _
        Let getter = "get_" + propName _
        Let setter = "set_" + propName _
                        Select <ul>
                                   <%= propName %>
                                   <%= From metd In type.Elements("method") _
                                       Let metdName = ExtractName(metd) _
                                       Where metdName = getter Or metdName = setter _
                                       Select ExtractMethod(metd) %>
                               </ul>
        If Not props.Any() Then Return Nothing
        Return <div><h4>Properties: </h4><%= props %></div>
    End Function

    Public Function ExtractOperators(ByVal type As XElement) As XElement
        Dim ops = From op In type.Elements("method") _
        Let name = ExtractName(op) _
        Where name.StartsWith("op_") _
                      Select <ul>
                                 <%= name.Substring("op_".Length) %>
                                 <%= ExtractMethod(op) %>
                             </ul>
        If Not ops.Any() Then Return Nothing
        Return <div><h4>Operators: </h4><%= ops %></div>
    End Function

    Public Function ExtractMethods(ByVal type As XElement) As XElement
        Dim metds = From metd In type.Elements("method") _
                    Let name = ExtractName(metd) _
                    Where name <> ".ctor" And _
                          Not type.Elements("property").Where(Function(prop) name = "get_" & ExtractName(prop) Or _
                                                                             name = "set_" + ExtractName(prop)).Any() And Not name.StartsWith("op_") _
                    Select ExtractMethod(metd)

        If Not metds.Any() Then Return Nothing
        Return New XElement("div", New XElement("h4", "Methods: "), New XElement("ul", metds))
    End Function

    Public Function ExtractMethod(ByVal metd As XElement) As XElement
        Return <li>
                   <%= ExtractModifiers(metd) + " " %>
                   <%= ExtractReference(metd.Element("returnType")) + " " %>
                   <%= ExtractReference(metd) %>
                   <%= ExtractParameters(metd) %>
               </li>
    End Function

    Public Function ExtractParameters(ByVal metd As XElement) As String

        Dim builder As StringBuilder = New StringBuilder("(")
        For Each parm As XElement In metd.Elements("parameter")
            If builder.Length = 1 Then
                If metd.Element("extension") IsNot Nothing Then builder.Append("this ")
            Else
                builder.Append(", ")
            End If
            builder.Append(ExtractReference(parm.Element("parameterType")))
            builder.Append(" ")
            builder.Append(ExtractName(parm))
        Next
        builder.Append(")")
        Return builder.ToString()
    End Function
End Class