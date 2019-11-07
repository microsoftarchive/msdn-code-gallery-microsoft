Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WorkflowActions

Public Class FindItem
    Inherits Activity

#Region "Dependency Property Bindings"

    'ListID
    Public Shared ListIDProperty As DependencyProperty = _
        DependencyProperty.Register("ListID", GetType(String), GetType(FindItem))

    'SearchQuery
    Public Shared SearchQueryProperty As DependencyProperty = _
        DependencyProperty.Register("SearchQuery", GetType(String), GetType(FindItem))

    'ResultItemID
    Public Shared ResultItemIDProperty As DependencyProperty = _
        DependencyProperty.Register("ResultItemID", GetType(Integer), GetType(FindItem))

    '_Context
    Public Shared _ContextProperty As DependencyProperty = _
        DependencyProperty.Register("_Context", GetType(WorkflowContext), GetType(FindItem))

#End Region

#Region "Workflow Properties"

    'This property gets or sets the ID of the list we're working with
    <BrowsableAttribute(True)> _
    <DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)> _
    <ValidationOption(ValidationOption.Required)> _
    Public Property ListID As String
        Get
            Return TryCast(MyBase.GetValue(FindItem.ListIDProperty), String)
        End Get
        Set(ByVal value As String)
            MyBase.SetValue(FindItem.ListIDProperty, value)
        End Set
    End Property

    'This property gets or sets the CAML query that selects a list item
    <BrowsableAttribute(True)> _
    <DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)> _
    <ValidationOption(ValidationOption.Required)> _
    Public Property SearchQuery As String
        Get
            Return TryCast(MyBase.GetValue(FindItem.SearchQueryProperty), String)
        End Get
        Set(ByVal value As String)
            MyBase.SetValue(FindItem.SearchQueryProperty, value)
        End Set
    End Property

    'This property gets or sets the ID of the item returned by the query
    <BrowsableAttribute(True)> _
    <DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)> _
    <ValidationOption(ValidationOption.Required)> _
    Public Property ResultItemID As Integer
        Get
            Return TryCast(MyBase.GetValue(FindItem.ResultItemIDProperty), String)
        End Get
        Set(ByVal value As Integer)
            MyBase.SetValue(FindItem.ResultItemIDProperty, value)
        End Set
    End Property

    'This property returns the context in which this workflow instance runs
    'You can get the workflow item, the current SPWeb and other properties from this
    <ValidationOption(ValidationOption.Required)> _
    Public Property _Context As WorkflowContext
        Get
            Return TryCast(MyBase.GetValue(FindItem._ContextProperty), WorkflowContext)
        End Get
        Set(ByVal value As WorkflowContext)
            MyBase.SetValue(FindItem._ContextProperty, value)
        End Set
    End Property

#End Region

    'Override the Execute method to define the business logic,
    'i.e. what the activity does when it is called
    Protected Overrides Function Execute(ByVal executionContext As ActivityExecutionContext) As ActivityExecutionStatus

        'Get the GUID for the current list
        Dim listGuid As Guid = Helper.GetListGuid(Me._Context, Me.ListID)
        If Me._Context IsNot Nothing Then
            Dim web As SPWeb = Me._Context.Web
            If web IsNot Nothing Then
                Dim list As SPList = web.Lists(listGuid)
                Dim query As SPQuery = New SPQuery()
                query.Query = Me.SearchQuery
                Dim items As SPListItemCollection = list.GetItems(query)
                If items.Count > 0 Then
                    ResultItemID = items(0).ID
                End If
            End If
        End If

        Return ActivityExecutionStatus.Closed

    End Function

End Class
