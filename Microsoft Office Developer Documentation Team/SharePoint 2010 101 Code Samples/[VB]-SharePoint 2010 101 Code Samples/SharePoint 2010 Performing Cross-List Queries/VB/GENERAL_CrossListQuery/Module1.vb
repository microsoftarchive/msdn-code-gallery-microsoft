Imports Microsoft.SharePoint

Module Module1

    'This console application searches for all entries in all Tasks folders
    'that are assigned to the current user. Try creating tasks in several sites
    'within the site collection, assign them to your own account then run this
    'project. All tasks should be returned in a single operation
    Sub Main()
        'Start by formulating the query in CAML
        Dim query As SPSiteDataQuery = New SPSiteDataQuery()

        'Get IDs for the SharePoint built-in fields we want to use
        Dim assignedToID As String = SPBuiltInFieldId.AssignedTo.ToString("B")
        Dim taskDueDateID As String = SPBuiltInFieldId.TaskDueDate.ToString("B")
        Dim titleID As String = SPBuiltInFieldId.Title.ToString("B")
        Dim taskStatusID As String = SPBuiltInFieldId.TaskStatus.ToString("B")
        Dim percentCompleteID As String = SPBuiltInFieldId.PercentComplete.ToString("B")

        'This is the selection creterion
        Dim whereClause As String = "<Where><Eq><FieldRef ID='" + assignedToID + "' />" _
                + "<Value Type='Integer'><UserID/></Value>" _
                + "</Eq></Where>"

        'This is the sort order
        Dim orderByClause As String = "<OrderBy><FieldRef ID='" + taskDueDateID + "' /></OrderBy>"

        'Set the query CAML
        query.Query = whereClause + orderByClause

        'We will query all the Tasks lists
        query.Lists = "<Lists ServerTemplate='107' />"

        'Define the view fields in the result set
        Dim viewFields As String = "<FieldRef ID='" + titleID + "' />" _
                + "<FieldRef ID='" + taskDueDateID + "' Nullable='TRUE' />" _
                + "<FieldRef ID='" + taskStatusID + "' Nullable='TRUE' />" _
                + "<FieldRef ID='" + percentCompleteID + "' Nullable='TRUE' />"
        query.ViewFields = viewFields

        'Query all the SPWebs in this SPSite
        query.Webs = "<Webs Scope='SiteCollection'>"

        'Get the SPSite and SPWeb, ensuring correct disposal
        'Replace the URL with your own site collection
        Using site As SPSite = New SPSite("http://intranet.contoso.com/")

            Using topWeb As SPWeb = site.OpenWeb()

                'Run the query
                Dim resultsTable As DataTable = topWeb.GetSiteData(query)

                'Print a heading line
                Console.WriteLine("{0, -10} {1, -30} {2, -20} {3}", "Date Due", "Task", "Status", "% Complete")

                'Loop through the results and print them
                For Each currentRow As DataRow In resultsTable.Rows

                    'Extract various values
                    Dim dueDate As String = DirectCast(currentRow(taskDueDateID), String)
                    Dim task As String = DirectCast(currentRow(titleID), String)
                    Dim status As String = DirectCast(currentRow(taskStatusID), String)
                    Dim percentComplete As String = DirectCast(currentRow(percentCompleteID), String)

                    'Format the due date
                    Dim dueDateTime As DateTime
                    Dim hasDate As Boolean = DateTime.TryParse(dueDate, dueDateTime)
                    If hasDate Then
                        dueDate = dueDateTime.ToShortDateString()
                    Else
                        dueDate = String.Empty
                    End If

                    'Format the percent complete string
                    Dim pctDec As Decimal
                    Dim hasValue As Boolean = Decimal.TryParse(percentComplete, pctDec)
                    If hasValue Then
                        percentComplete = pctDec.ToString("P0")
                    Else
                        percentComplete = "0%"
                    End If

                    'Print a line for this row
                    Console.WriteLine("{0, -10} {1, -30} {2, -20} {3, 10}", dueDate, task, status, percentComplete)

                Next

                'Wait for the user to press a key before closing
                Console.ReadKey()

            End Using

        End Using

    End Sub

End Module
