Imports System
Imports Microsoft.Office.RecordsManagement.RecordsRepository
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.RM_DeclareRecords

    ''' <summary>
    ''' This application page demonstrates how to evaluate, declare and un-declare
    ''' records in-place. It loops through all the items in a list, counts records
    ''' and non-records, then declares or undeclares all items as records depending
    ''' on which button you click
    ''' </summary>
    ''' <remarks>
    ''' You must enable the In-Place Records feature at the site-collection level 
    ''' before executing this code. Also it's helpful to try manually declaring a
    ''' record.
    ''' </remarks>
    Partial Public Class DeclareRecords
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub declareRecordsButton_Click(ByVal sender As Object, ByVal e As EventArgs)

            'These integers are to count records and non-records
            Dim existingRecords As Integer = 0
            Dim existingNonRecords As Integer = 0

            'Get the list
            Dim currentWeb As SPWeb = SPContext.Current.Web
            Dim list As SPList = currentWeb.Lists(listNameTextbox.Text)

            If list IsNot Nothing Then

                'Found a list, loop through the items
                For Each item As SPListItem In list.Items

                    'Is this item already a record? Use the static Records.IsRecord method to find out
                    If Records.IsRecord(item) Then
                        'This is already a record. Count it but take no action
                        existingRecords += 1
                    Else
                        'This is not a record. 
                        existingNonRecords += 1
                        'Declare this as a records using the static Records.DeclareItemAsRecord method
                        Records.DeclareItemAsRecord(item)
                    End If

                Next

                'Tell the user what happened.
                resultsLabel.Text = String.Format("There were {0} records and {1} non-records. <br />", existingRecords, existingNonRecords)
                resultsLabel.Text += "All items are now declared as records."

            Else

                'Couldn't find the list
                resultsLabel.Text = "Could not find a list by that name"

            End If

        End Sub

        Protected Sub undeclareRecordsButton_Click(ByVal sender As Object, ByVal e As EventArgs)

            'These integers are to count records and non-records
            Dim existingRecords As Integer = 0
            Dim existingNonRecords As Integer = 0

            'Get the list
            Dim currentWeb As SPWeb = SPContext.Current.Web
            Dim list As SPList = currentWeb.Lists(listNameTextbox.Text)

            If list IsNot Nothing Then

                'Found a list, loop through the items
                For Each item As SPListItem In list.Items

                    'Is this item already a record? Use the static Records.IsRecord method to find out
                    If Records.IsRecord(item) Then
                        'This is already a record. 
                        existingRecords += 1
                        'Undeclare this as a record
                        Records.UndeclareItemAsRecord(item)
                    Else
                        'This is not a record. 
                        existingNonRecords += 1
                    End If

                Next

                'Tell the user what happened.
                resultsLabel.Text = String.Format("There were {0} records and {1} non-records. <br />", existingRecords, existingNonRecords)
                resultsLabel.Text += "All items are now undeclared as records."

            Else

                'Couldn't find the list
                resultsLabel.Text = "Could not find a list by that name"

            End If

        End Sub

    End Class

End Namespace
