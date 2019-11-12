' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Management

Public Class MainForm


    ' This subroutine fills in the output text box with bios information from WMI
    Private Sub btnBios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBios.Click
        ' This is to show how to use the SelectQuery object in the place of a SELECT 
        ' statement.
        Dim query As New SelectQuery("Win32_bios")

        'ManagementObjectSearcher retrieves a collection of WMI objects based on 
        ' the query.
        Dim search As New ManagementObjectSearcher(query)

        ' Display each entry for Win32_bios
        Dim info As ManagementObject
        For Each info In search.Get()
            txtOutput.Text = "Bios version: " & info("version").ToString() & vbCrLf
        Next
    End Sub

    ' This subroutine fills in the output text box with computer system information
    ' from WMI
    Private Sub btnComputerSystem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnComputerSystem.Click
        ' ManagementObjectSearcher retrieves a collection of WMI objects based on 
        ' the query.  In this case a string is used instead of a SelectQuery object.
        Dim search As New ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem")

        ' Display each entry for Win32_ComputerSystem
        Dim info As ManagementObject
        For Each info In search.Get()
            txtOutput.Text = "Manufacturer: " & info("manufacturer").ToString() & vbCrLf
            txtOutput.Text &= "Model: " & info("model").ToString() & vbCrLf
            txtOutput.Text &= "System Type: " & info("systemtype").ToString() & vbCrLf
            txtOutput.Text &= "Total Physical Memory: " & _
                info("totalphysicalmemory").ToString() & vbCrLf
        Next
    End Sub

    ' This subroutine fills a list box with all WMI classes. 
    Private Sub btnClassEnum_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClassEnum.Click
        ' Clear out the list box
        lstWMIClasses.Items.Clear()

        ' Default constructor for ManagementClass will return cim root.  
        Dim root As New ManagementClass()

        ' If Subclasses checkbox check we will get all subclasses as well as the top
        ' level classes.
        Dim options As New EnumerationOptions()
        options.EnumerateDeep = chkIncludeSubclasses.Checked

        ' Add each WMI class in the enumeration to the list box.
        Dim info As ManagementObject
        For Each info In root.GetSubclasses(options)
            lstWMIClasses.Items.Add(info("__Class"))
        Next
    End Sub

    ' This subroutine fills in the output text box with Operating System information
    ' from WMI
    Private Sub btnOperatingSytem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOperatingSytem.Click
        ' ManagementObjectSearcher retrieves a collection of WMI objects based on 
        ' the query.  In this case a string is used instead of a SelectQuery object.
        Dim search As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")

        ' Display each entry for Win32_OperatingSystem
        Dim info As ManagementObject
        For Each info In search.Get()
            txtOutput.Text = "Name: " & info("name").ToString() & vbCrLf
            txtOutput.Text &= "Version: " & info("version").ToString() & vbCrLf
            txtOutput.Text &= "Manufacturer: " & info("manufacturer").ToString() & vbCrLf
            txtOutput.Text &= "Computer name: " & info("csname").ToString() & vbCrLf
            txtOutput.Text &= "Windows Directory: " & _
                info("windowsdirectory").ToString() & vbCrLf
        Next
    End Sub

    ' This subroutine fills in the output text box with Processor information
    ' from WMI
    Private Sub btnProcessor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProcessor.Click
        ' This is to show how to use the SelectQuery object in the place of a SELECT 
        ' statement.
        Dim query As New SelectQuery("Win32_processor")

        'ManagementObjectSearcher retrieves a collection of WMI objects based on 
        ' the query.
        Dim search As New ManagementObjectSearcher(query)

        ' Display each entry for Win32_processor
        Dim info As ManagementObject
        For Each info In search.Get()
            txtOutput.Text = "Processor: " & info("caption").ToString() & vbCrLf
        Next
    End Sub

    ' This subroutine fills in the output text box with Time zone information from WMI
    Private Sub btnTimeZone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTimeZone.Click
        ' This is to show how to use the SelectQuery object in the place of a SELECT 
        ' statement.
        Dim query As New SelectQuery("Win32_timezone")

        'ManagementObjectSearcher retrieves a collection of WMI objects based on 
        ' the query.
        Dim search As New ManagementObjectSearcher(query)

        ' Display each entry for Win32_timezone
        Dim info As ManagementObject
        For Each info In search.Get()
            txtOutput.Text = "Time zone: " & info("caption").ToString() & vbCrLf
        Next
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
