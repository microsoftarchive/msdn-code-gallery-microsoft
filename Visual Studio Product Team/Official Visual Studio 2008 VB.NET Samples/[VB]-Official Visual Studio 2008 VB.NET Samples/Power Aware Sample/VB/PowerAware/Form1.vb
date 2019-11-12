Imports System.Runtime.InteropServices

Public Class Form1
    Dim myManagedPower As New ManagedPower()

    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'Set label to initial status
        Label1.Text = myManagedPower.ToString()
    End Sub

    Sub timerTickHandler(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        Label1.Text = myManagedPower.ToString()
    End Sub
End Class

'
' This class contains the definitions of the unmanaged methods,
' structs and enums that will be used in the call to the 
' unmanaged power API.
'
Public Class ManagedPower
    ' GetSystemPowerStatus() is the only unmanaged API called.
    Declare Auto Function GetSystemPowerStatus Lib "kernel32.dll" _
    Alias "GetSystemPowerStatus" (ByRef sps As SystemPowerStatus) As Boolean

    Public Overrides Function ToString() As String
        Dim text As String = ""
        Dim sysPowerStatus As SystemPowerStatus
        ' Get the power status of the system
        If ManagedPower.GetSystemPowerStatus(sysPowerStatus) Then
            ' Current power source - AC/DC
            Dim currentPowerStatus = sysPowerStatus.ACLineStatus
            text += "Power source: " + sysPowerStatus.ACLineStatus.ToString() + Environment.NewLine

            ' Current power status
            text += "Power status: "

            ' Check for unknown
            If sysPowerStatus.BatteryFlag = ManagedPower._BatteryFlag.Unknown Then
                text += "Unknown"
            Else
                ' Check if currently charging
                Dim fCharging = (ManagedPower._BatteryFlag.Charging = _
      (sysPowerStatus.BatteryFlag & ManagedPower._BatteryFlag.Charging))

                If fCharging Then
                    Dim currentChargingStatus = ManagedPower._BatteryFlag.Charging
                End If

                ' Print out power level
                ' If the power is not High, Low, or Critical, report it as "Medium".
                Dim currentPowerLevel As String
                If sysPowerStatus.BatteryFlag = 0 Then
                    currentPowerLevel = "Medium"
                Else
                    currentPowerLevel = sysPowerStatus.BatteryFlag.ToString()
                End If
                text += currentPowerLevel

                ' Print out charging status
                If fCharging Then
                    Dim currentChargingStatus = ManagedPower._BatteryFlag.Charging.ToString()
                    text += " (" + ManagedPower._BatteryFlag.Charging.ToString() + ") "
                End If
            End If

            ' Finally print the percentage of the battery life remaining.
            Dim currentBatteryPercentage = sysPowerStatus.BatteryLifePercent
            text += Environment.NewLine + "Battery life remaining is " + _
            sysPowerStatus.BatteryLifePercent.ToString() + "%"
        End If
        Return text
    End Function

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SystemPowerStatus
        Public ACLineStatus As _ACLineStatus
        Public BatteryFlag As _BatteryFlag
        Public BatteryLifePercent As Byte
        Public Reserved1 As Byte
        Public BatteryLifeTime As System.UInt32
        Public BatteryFullLifeTime As System.UInt32
    End Structure

    Public Enum _ACLineStatus As Byte
        Battery = 0
        AC = 1
        Unknown = 255
    End Enum

    <Flags()> _
    Public Enum _BatteryFlag As Byte
        High = 1
        Low = 2
        Critical = 4
        Charging = 8
        NoSystemBattery = 128
        Unknown = 255
    End Enum
End Class

