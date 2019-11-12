' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System
Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports System.Reflection

'See the ReadMe.html for additional information
Public Class ObjectDumper
    Public Shared Sub Write(ByVal o As Object)
        Write(o, 0)
    End Sub

    Public Shared Sub Write(ByVal o As Object, ByVal depth As Integer)
        Write(o, depth, Console.Out)
    End Sub

    Public Shared Sub Write(ByVal o As Object, ByVal depth As Integer, ByVal log As TextWriter)
        Dim dumper As New ObjectDumper(depth)
        dumper.writer = log
        dumper.WriteObject(Nothing, o)
    End Sub

    Dim writer As TextWriter
    Dim pos, level, depth As Integer

    Private Sub New(ByVal depth As Integer)
        Me.depth = depth
    End Sub

    Private Sub Write(ByVal s As String)
        If (s IsNot Nothing) Then
            writer.Write(s)
            pos += s.Length
        End If
    End Sub

    Private Sub WriteIndent()
        For i As Integer = 0 To level - 1
            writer.Write("  ")
        Next i
    End Sub

    Private Sub WriteLine()
        writer.WriteLine()
        pos = 0
    End Sub

    Private Sub WriteTab()
        Write("  ")
        While ((pos Mod 8) <> 0)
            Write(" ")
        End While
    End Sub

    Private Sub WriteObject(ByVal prefix As String, ByVal o As Object)
        If (o Is Nothing OrElse TypeOf o Is ValueType OrElse TypeOf o Is String) Then
            WriteIndent()
            Write(prefix)
            WriteValue(o)
            WriteLine()
        ElseIf TypeOf o Is IEnumerable Then
            For Each element As Object In CType(o, IEnumerable)
                If (TypeOf element Is IEnumerable AndAlso Not TypeOf element Is String) Then
                    WriteIndent()
                    Write(prefix)
                    Write("...")
                    WriteLine()
                    If (level < depth) Then
                        level += 1
                        WriteObject(prefix, element)
                        level -= 1
                    End If
                Else
                    WriteObject(prefix, element)
                End If
            Next
        Else
            Dim t As Type
            Dim members As MemberInfo() = o.GetType.GetMembers((BindingFlags.Public Or BindingFlags.Instance))
            WriteIndent()
            Write(prefix)
            Dim propWritten As Boolean = False

            For Each m As MemberInfo In members
                Dim f As FieldInfo = TryCast(m, FieldInfo)
                Dim p As PropertyInfo = TryCast(m, PropertyInfo)
                If (f IsNot Nothing OrElse p IsNot Nothing) Then
                    If propWritten Then
                        WriteTab()
                    Else
                        propWritten = True
                    End If
                    Write(m.Name)
                    Write("=")
                    If (f IsNot Nothing) Then
                        t = f.FieldType
                    Else
                        t = p.PropertyType
                    End If

                    If (t.IsValueType OrElse t Is GetType(String)) Then
                        If (f IsNot Nothing) Then
                            WriteValue(f.GetValue(o))
                        Else
                            WriteValue(p.GetValue(o, Nothing))
                        End If
                    ElseIf GetType(IEnumerable).IsAssignableFrom(t) Then
                        Write("...")
                    Else
                        Write("{ }")
                    End If
                End If
            Next
            If propWritten Then WriteLine()

            If (level < depth) Then
                For Each m As MemberInfo In members
                    Dim f As FieldInfo = TryCast(m, FieldInfo)
                    Dim p As PropertyInfo = TryCast(m, PropertyInfo)
                    If (f IsNot Nothing OrElse p IsNot Nothing) Then
                        If (f IsNot Nothing) Then
                            t = f.FieldType
                        Else
                            t = p.PropertyType
                        End If

                        If (Not (t.IsValueType OrElse t Is GetType(String))) Then
                            Dim value As Object
                            If (f IsNot Nothing) Then
                                value = f.GetValue(o)
                            Else
                                value = p.GetValue(o, Nothing)
                            End If
                            If (value IsNot Nothing) Then
                                level += 1
                                WriteObject((m.Name & ": "), value)
                                level -= 1
                            End If
                        End If
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub WriteValue(ByVal o As Object)
        If (o Is Nothing) Then
            Write("null")
        ElseIf TypeOf o Is DateTime Then
            Write(CDate(o).ToShortDateString)
        ElseIf (TypeOf o Is ValueType OrElse TypeOf o Is String) Then
            Write(o.ToString)
        ElseIf TypeOf o Is IEnumerable Then
            Write("...")
        Else
            Write("{ }")
        End If
    End Sub

End Class

