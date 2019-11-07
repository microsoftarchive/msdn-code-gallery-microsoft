
Imports System.Management
Imports System.Net.Sockets
Imports System.Net
Imports Microsoft.VisualBasic.Constants




Module Module1
    Dim udpClt As New UdpClient


    Sub Main()

        Dim MacAddress As String = "4437E694B11E"
        executeWOL(MacAddress)
    End Sub

    Private Sub executeWOL(ByVal MAC As String)


        Dim packet As [Byte]()

        ReDim packet(101)

        Dim localBCast As New System.Net.IPEndPoint(System.Net.IPAddress.Parse("10.171.69.147"), CInt(137))

        generatePacket(MAC, packet)

        udpClt.Send(packet, 102, localBCast)

        localBCast = Nothing

    End Sub
    Private Function generatePacket(ByVal MAC As String, ByRef packet As Byte()) As Boolean


        generatePacket = True



        Dim byteLength As Integer

        Select Case MAC.Length


            Case 12


                byteLength = 2

            Case 17


                byteLength = 3

            Case Else


                generatePacket = False

                Exit Function

        End Select



        Dim i, j As Integer

        Dim block As [Byte]()

        ReDim block(5)



        Try


            For i = 0 To 5


                block(i) = Convert.ToByte(MAC.Substring(byteLength * i, 2), 16)

            Next

        Catch


            generatePacket = False

            Exit Function

        End Try



        For i = 0 To 5


            packet(i) = &HFF

        Next

        For i = 6 To 97 Step 6


            For j = 0 To 5


                packet(i + j) = block(j)

            Next

        Next

    End Function


End Module
