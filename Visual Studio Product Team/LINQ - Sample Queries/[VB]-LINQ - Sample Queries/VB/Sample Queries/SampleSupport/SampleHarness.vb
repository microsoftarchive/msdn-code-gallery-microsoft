' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Option Infer On

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms

Namespace SampleSupport
    <DefaultMember("Item")> _
    Public Class SampleHarness
        Implements IEnumerable(Of Sample), IEnumerable

        ' Methods

        Public Sub New()
            Dim type As Type = MyBase.GetType
            _Title = "Samples"
            Dim prefix As String = "Sample"
            Dim text2 As String = (type.Name & ".vb")
            Dim attribute As Attribute
            For Each attribute In type.GetCustomAttributes(False)
                If TypeOf attribute Is TitleAttribute Then
                    _Title = DirectCast(attribute, TitleAttribute).Title
                ElseIf TypeOf attribute Is PrefixAttribute Then
                    prefix = DirectCast(attribute, PrefixAttribute).Prefix
                End If
            Next
            Dim allCode As String = SampleHarness.readFile((Application.StartupPath & "\..\..\" & text2))
            Dim key As Integer = 1
            Dim methods As MethodInfo() = type.GetMethods(BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.DeclaredOnly)
            For Each method In methods


                If method.Name.StartsWith(prefix) Then
                    Dim category As String = "Miscellaneous"
                    Dim title As String = (prefix & " Sample " & key)
                    Dim description As String = "See code."
                    Dim list As New List(Of MethodInfo)
                    Dim list2 As New List(Of MethodInfo)
                    Dim list3 As New List(Of PropertyInfo)
                    Dim list4 As New List(Of Type)
                    Dim attribute2 As Attribute
                    For Each attribute2 In method.GetCustomAttributes(False)
                        If TypeOf attribute2 Is CategoryAttribute Then
                            category = DirectCast(attribute2, CategoryAttribute).Category
                        ElseIf TypeOf attribute2 Is TitleAttribute Then
                            title = DirectCast(attribute2, TitleAttribute).Title
                        ElseIf TypeOf attribute2 Is DescriptionAttribute Then
                            description = DirectCast(attribute2, DescriptionAttribute).Description
                        ElseIf TypeOf attribute2 Is LinkedMethodAttribute Then
                            Dim item As MethodInfo = type.GetMethod(DirectCast(attribute2, LinkedMethodAttribute).MethodName, (BindingFlags.NonPublic Or (BindingFlags.Public Or (BindingFlags.Static Or BindingFlags.Instance))))
                            If (item IsNot Nothing) Then
                                list.Add(item)
                            End If
                        ElseIf TypeOf attribute2 Is LinkedFunctionAttribute Then
                            Dim info3 As MethodInfo = type.GetMethod(DirectCast(attribute2, LinkedFunctionAttribute).MethodName, (BindingFlags.NonPublic Or (BindingFlags.Public Or (BindingFlags.Static Or BindingFlags.Instance))))
                            If (info3 IsNot Nothing) Then
                                list2.Add(info3)
                            End If
                        ElseIf TypeOf attribute2 Is LinkedClassAttribute Then
                            Dim nestedType As Type = type.GetNestedType(DirectCast(attribute2, LinkedClassAttribute).ClassName)
                            If (nestedType IsNot Nothing) Then
                                list4.Add(nestedType)
                            End If
                        End If
                    Next
                    Dim builder As New StringBuilder
                    builder.Append(SampleHarness.getCodeBlock(allCode, "Sub", method.Name))
                    Dim info4 As MethodInfo
                    For Each info4 In list
                        builder.Append(Environment.NewLine)
                        builder.Append(SampleHarness.getCodeBlock(allCode, "Sub", info4.Name))
                    Next
                    Dim info5 As MethodInfo
                    For Each info5 In list2
                        builder.Append(Environment.NewLine)
                        builder.Append(SampleHarness.getCodeBlock(allCode, "Function", info5.Name))
                    Next
                    Dim info6 As PropertyInfo
                    For Each info6 In list3
                        builder.Append(Environment.NewLine)
                        builder.Append(SampleHarness.getCodeBlock(allCode, "Property", info6.Name))
                    Next
                    Dim type3 As Type
                    For Each type3 In list4
                        builder.Append(Environment.NewLine)
                        builder.Append(SampleHarness.getCodeBlock(allCode, "Class", type3.Name))
                    Next
                    Dim sample As New Sample(Me, method, category, title, description, builder.ToString)
                    samples.Add(key, sample)
                    Do
                        key += 1
                        method = type.GetMethod((prefix & key))
                    Loop While ((method Is Nothing) AndAlso (key <= 120))
                End If

            Next
        End Sub

        Private Shared Function getCodeBlock(ByVal allCode As String, ByVal blockPrefix As String, ByVal blockName As String) As String
            Dim startIndex As Integer = -1
            Do
                startIndex = allCode.IndexOf((blockPrefix & " " & blockName), (startIndex + 1), StringComparison.OrdinalIgnoreCase)
                If (startIndex = -1) Then
                    Return String.Concat(New String() { "' ", blockPrefix, " ", blockName, " code not found" })
                End If
            Loop While (allCode.LastIndexOf(Environment.NewLine, startIndex) < allCode.LastIndexOf("'"c, startIndex))
            startIndex = allCode.LastIndexOf(Environment.NewLine, startIndex)
            If (startIndex = -1) Then
                startIndex = 0
            Else
                startIndex = (startIndex + Environment.NewLine.Length)
            End If
            Dim index As Integer = allCode.IndexOf(("End " & blockPrefix), startIndex, StringComparison.OrdinalIgnoreCase)
            If (index = -1) Then
                Return String.Concat(New String() { "' ", blockPrefix, " ", blockName, " code not found" })
            End If
            index = allCode.IndexOf(Environment.NewLine, index)
            If (index = -1) Then
                index = allCode.Length
            End If
            Return SampleHarness.removeIndent(allCode.Substring(startIndex, ((index - startIndex) + 1)))
        End Function

        Public Overridable Sub HandleException(ByVal e As Exception)
            Console.Write(e)
        End Sub

        Public Overridable Sub InitSample()
        End Sub

        Private Shared Function readFile(ByVal path As String) As String
            If File.Exists(path) Then
                Using reader As StreamReader = File.OpenText(path)
                    Return reader.ReadToEnd
                End Using
            End If
            Return ""
        End Function

        Private Shared Function removeIndent(ByVal code As String) As String
            Dim startIndex As Integer = 0
            Dim text As String = ""
            Do While (code.Chars(startIndex) = " "c)
                startIndex += 1
                [text] = ([text] & " "c)
            Loop
            Dim builder As New StringBuilder
            Dim text2 As String
            For Each text2 In code.Split(New String() {Environment.NewLine}, 0)
                If (startIndex < text2.Length) Then
                    If text2.StartsWith([text]) Then
                        builder.AppendLine(text2.Substring(startIndex))
                    Else
                        builder.AppendLine(text2)
                    End If
                Else
                    builder.AppendLine()
                End If
            Next
            Return builder.ToString
        End Function

        Public Sub RunAllSamples()
            Dim newOut As TextWriter = Console.Out
            Console.SetOut(StreamWriter.Null)
            Dim sample As Sample
            For Each sample In Me
                sample.Invoke
            Next
            Console.SetOut(newOut)
        End Sub

        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Sample) Implements System.Collections.Generic.IEnumerable(Of Sample).GetEnumerator
            Return samples.Values.GetEnumerator
        End Function
        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return samples.Values.GetEnumerator
        End Function



        ' Properties
        Default Public ReadOnly Property Item(ByVal i As Integer) As Sample
            Get
                Return samples.Item(i)
            End Get
        End Property

        Public ReadOnly Property OutputStreamWriter() As StreamWriter
            Get
                Return _OutputStreamWriter
            End Get
        End Property

        Public ReadOnly Property Title() As String
            Get
                Return _Title
            End Get
        End Property


        ' Fields
        Private ReadOnly _OutputStreamWriter As StreamWriter = New StreamWriter(New MemoryStream)
        Private ReadOnly _Title As String
        Private ReadOnly samples As IDictionary(Of Integer, Sample) = New Dictionary(Of Integer, Sample)
    End Class


End Namespace

