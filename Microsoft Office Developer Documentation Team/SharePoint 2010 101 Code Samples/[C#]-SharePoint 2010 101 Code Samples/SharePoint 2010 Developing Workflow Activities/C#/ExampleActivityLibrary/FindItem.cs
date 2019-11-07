using System;
using System.ComponentModel;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;

namespace ExampleActivityLibrary
{

    public partial class FindItem : Activity
    {
        public FindItem()
        {
            
        }

#region Dependency Property Bindings

        //ListID
        public static DependencyProperty ListIDProperty = 
            DependencyProperty.Register("ListID", typeof(string), typeof(FindItem));

        //SearchQuery
        public static DependencyProperty SearchQueryProperty = 
            DependencyProperty.Register("SearchQuery", typeof(string), typeof(FindItem)); 

        //ResultItemID
        public static DependencyProperty ResultItemIDProperty = 
            DependencyProperty.Register("ResultItemID", typeof(int), typeof(FindItem));

        //_Context
        public static DependencyProperty _ContextProperty = 
            DependencyProperty.Register("_Context", typeof(WorkflowContext), typeof(FindItem));

#endregion
   
#region Workflow Properties

        //This property gets or sets the ID of the list we're working with
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Required)]
        public string ListID
        {
            get 
            {
                return (string)base.GetValue(FindItem.ListIDProperty);
            }
            set
            {
                base.SetValue(FindItem.ListIDProperty, value);
            }
        }

        //This property gets or sets the CAML query that selects a list item
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Required)]
        public string SearchQuery
        {
            get 
            {
                return (string)base.GetValue(FindItem.SearchQueryProperty);
            }
            set
            {
                base.SetValue(FindItem.SearchQueryProperty, value);
            }
        }

        //This property gets or sets the ID of the item returned by the query
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Required)]
        public int ResultItemID
        {
            get 
            {
                return (int)base.GetValue(FindItem.ResultItemIDProperty);
            }
            set
            {
                base.SetValue(FindItem.ResultItemIDProperty, value);
            }
        }

        //This property returns the context in which this workflow instance runs
        //You can get the workflow item, the current SPWeb and other properties from this
        [ValidationOption(ValidationOption.Required)]
        public WorkflowContext _Context
        {
            get
            {
                return (WorkflowContext)base.GetValue(FindItem._ContextProperty);
            }
            set
            {
                base.SetValue(FindItem._ContextProperty, value);
            }
        }

#endregion

        //Override the Execute method to define the business logic,
        //i.e. what the activity does when it is called
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
 	        //Get the GUID for the current list
            Guid listGuid = Helper.GetListGuid(this._Context, this.ListID);
            if (this._Context != null)
            {
                SPWeb web = this._Context.Web;
                if (web != null) 
                {
                    SPList list = web.Lists[listGuid];
                    SPQuery query = new SPQuery();
                    query.Query = this.SearchQuery;
                    SPListItemCollection items = list.GetItems(query);
                    if (items.Count > 0)
                    {
                        ResultItemID = items[0].ID;
                    }
                }
            }
            return ActivityExecutionStatus.Closed;
        }
    }
}
