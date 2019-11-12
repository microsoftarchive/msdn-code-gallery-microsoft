Imports System
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Taxonomy
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.ECM_FindTaggedItems

    ''' <summary>
    ''' This application page demonstrates how to locate a term in a Managed Metadata 
    ''' Service termset, then use it to locate all the items in a list that have been
    ''' tagged with it. It uses a CAML query to search the list.
    ''' </summary>
    ''' <remarks>
    ''' Before you use this sample, you must have a Managed Metadata service application
    ''' connected to your site collection and have tagged items in a list with a term from
    ''' the term store. Check the contents of the Taxonomy Hidden List in your site to
    ''' ensure these conditions are met. This list is at /lists/TaxonomyHiddenList/AllItem.aspx
    ''' in your SharePoint site collection.
    ''' </remarks>
    Partial Public Class FindTaggedItems
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub getItemsButton_Click(ByVal sender As Object, ByVal e As EventArgs)

            If (searchForTextBox.Text <> String.Empty) AndAlso (nameOfListTextbox.Text <> String.Empty) Then

                Dim currentSite As SPSite = SPContext.Current.Site
                'We must create a new Taxonomy Session object
                Dim session As TaxonomySession = New TaxonomySession(currentSite)
                'Get the termstore
                Dim termstore As TermStore = session.TermStores("Managed Metadata Service")

                'Now we can find the collection of terms that satisfy the user's request
                Dim terms As TermCollection = termstore.GetTerms(searchForTextBox.Text, True)

                If terms.Count > 0 Then

                    'For simplicity we'll just use the first term returned
                    Dim firstTerm As Term = terms(0)
                    resultLabel.Text = "Term Found. Term ID: " + firstTerm.Id.ToString() + "<br /><br />"

                    'Now that we have a term object to search for, we must get the
                    'WssIds of that term in the current site collection. Note that a
                    'single term from a term store can have different WssIds in each 
                    'site collection. Use the GetWssIdsOfTerm method to findout what 
                    'they are. You can also see these in the Taxonomy Hidden List.
                    Dim WssIds As Integer() = TaxonomyField.GetWssIdsOfTerm(currentSite, _
                        termstore.Id, firstTerm.TermSet.Id, firstTerm.Id, False, 500)

                    If WssIds.GetLength(0) = 0 Then

                        'This tag is in the tagstore but not the Taxonomy Hidden List
                        'That usually means it hasn't been used in the current site
                        resultLabel.Text += "There are no entries for that term in the taxonomy hidden list. " + _
                            "Have you tagged an item in the site with the tag you search for?"

                    Else

                        'Tell the user what the WSS IDs for the located term are 
                        'and formulate a CAML query
                        Dim camlQuery As String = "<Where><In><FieldRef Name='Tags' LookupId='TRUE' /><Values>"
                        resultLabel.Text += "Here are the WssIds: <br />"

                        For Each WssId As Integer In WssIds

                            camlQuery += String.Format("<Value Type='Integer'>{0}</Value>", WssId)
                            resultLabel.Text += WssId.ToString() + "<br />"

                        Next

                        'Complete the CAML query
                        camlQuery += "</Values></In></Where>"

                        'Get the list and create the query
                        Dim searchList As SPList = currentSite.RootWeb.Lists(nameOfListTextbox.Text)
                        Dim query As SPQuery = New SPQuery()
                        query.Query = camlQuery

                        If searchList IsNot Nothing Then

                            'Run the query
                            Dim items As SPListItemCollection = searchList.GetItems(query)
                            'Tell the user what tagged items were found
                            resultLabel.Text += "<br />Number of items with that tag: " + Items.Count.ToString() + "<br /><br />"

                            For Each currentItem As SPItem In items

                                resultLabel.Text += "Item Title: " + currentItem("Title") + "<br />"
                                resultLabel.Text += "Item ID: " + currentItem.ID.ToString() + "<br />"

                            Next

                        Else

                            'The list name the user entered did not match the name of a list
                            resultLabel.Text += "<br />There is no list by that name."

                        End If

                    End If

                Else

                    'The text the user entered did not match the name of a term in the termstore
                    resultLabel.Text = "There is no term in the termstore that matches your text"

                End If

            End If

        End Sub

    End Class

End Namespace
