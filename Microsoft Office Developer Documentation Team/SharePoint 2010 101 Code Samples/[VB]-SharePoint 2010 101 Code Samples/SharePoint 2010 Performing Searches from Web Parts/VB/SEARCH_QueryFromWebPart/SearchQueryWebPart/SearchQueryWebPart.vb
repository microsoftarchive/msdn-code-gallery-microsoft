Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.Office.Server.Search.Administration
Imports Microsoft.Office.Server.Search.Query

Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

''' <summary>
''' This example Web Part shows how to call the search Query Object Model from a
''' Web Part. 
''' </summary>
''' <remarks>
''' This code works whether you use a SharePoint Search service application or a
''' FAST Search service application. However, make sure that your site is connected
''' to one or the other and that the service application has crawled the content
''' in your site. Otherwise you will get zero results.
''' </remarks>
<ToolboxItemAttribute(false)> _
Public Class SearchQueryWebPart
    Inherits WebPart

    'Controls
    Dim searchTermsLabel As Label
    Dim searchTermsTextbox As TextBox
    Dim searchButton As Button
    Dim resultsLabel As Label
    Dim resultsGrid As DataGrid

    Protected Overrides Sub CreateChildControls()
        Me.Controls.Clear()
        'Create the searchTermsLabel
        searchTermsLabel = New Label()
        searchTermsLabel.Text = "Search Terms:"
        Me.Controls.Add(searchTermsLabel)
        'Create the searchTearmsTextbox
        searchTermsTextbox = New TextBox()
        Me.Controls.Add(searchTermsTextbox)
        'Create the searchButton and add a click handler
        searchButton = New Button()
        searchButton.Text = "Search"
        AddHandler searchButton.Click, AddressOf searchButton_Click
        Me.Controls.Add(searchButton)
        'Create the resultsLabel
        resultsLabel = New Label()
        Me.Controls.Add(resultsLabel)
        'Create the datagrid
        resultsGrid = New DataGrid()
        Me.Controls.Add(resultsGrid)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'Render all the controls
        searchTermsLabel.RenderControl(writer)
        searchTermsTextbox.RenderControl(writer)
        searchButton.RenderControl(writer)
        writer.Write("<br />")
        resultsLabel.RenderControl(writer)
        writer.Write("<br />")
        resultsGrid.RenderControl(writer)
    End Sub

    Private Sub searchButton_Click(ByVal sender As Object, ByVal e As EventArgs)

        'Only search if there are search terms
        If searchTermsTextbox.Text <> String.Empty Then

            'First, we must connect to the service application proxy
            Dim proxy As SearchServiceApplicationProxy = TryCast(SearchServiceApplicationProxy.GetProxy(SPServiceContext.GetContext(SPContext.Current.Site)), SearchServiceApplicationProxy)

            'Create and configure a Keyword Query object
            Dim query As KeywordQuery = New KeywordQuery(proxy)
            query.ResultsProvider = SearchProvider.Default
            query.QueryText = searchTermsTextbox.Text
            'Relevant Results are the main search results
            query.ResultTypes = ResultType.RelevantResults

            'Now we can execute the query
            Dim searchResults As ResultTableCollection = query.Execute()

            If searchResults.Exists(ResultType.RelevantResults) Then

                'There are relevant results. We need them in a Data Table
                'so we can bind them to the Data Grid for display
                Dim resultTable As ResultTable = searchResults(ResultType.RelevantResults)
                'Tell the user how many results we got
                resultsLabel.Text = String.Format("There are {0} results", resultTable.TotalRows.ToString())
                'Set up and load the data table
                Dim resultDataTable As DataTable = New DataTable()
                resultDataTable.TableName = "Result"
                resultDataTable.Load(resultTable, LoadOption.OverwriteChanges)

                'Bind the datatable to the Data Grid
                resultsGrid.DataSource = resultDataTable
                resultsGrid.DataBind()

            Else

                'Search executed but found nothing
                resultsLabel.Text = "There were no relevant results. Try other search terms"

            End If

        Else

            'No search terms in the textbox
            resultsLabel.Text = "Please enter at least one search term"

        End If

    End Sub

End Class
