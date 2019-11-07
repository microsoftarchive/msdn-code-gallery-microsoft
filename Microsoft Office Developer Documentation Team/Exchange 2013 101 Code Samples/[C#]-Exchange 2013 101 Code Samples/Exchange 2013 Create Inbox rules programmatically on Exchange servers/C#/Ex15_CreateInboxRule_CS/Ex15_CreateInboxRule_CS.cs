using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class InboxRules
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateInboxRule(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Creates a disabled Inbox rule that searches for a string in the subject, and if the condition is met,
        /// moves the email message to the Junk Email folder and adds a category to the mail. This Inbox rule 
        /// is not visible from Outlook 2013.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void CreateInboxRule(ExchangeService service)
        {
            // Create an Inbox rule.
            Rule newRule = new Rule();
            newRule.DisplayName = "MoveInterestingToJunk";
            newRule.Priority = 1;
            newRule.IsEnabled = false;

            // Specify the conditions that must be met to trigger the rule. There are many condition
            // options for specifying an Inbox rule.
            newRule.Conditions.ContainsSubjectStrings.Add("Interesting");

            // Specify the actions that will be taken on items that meet the conditions. There are 
            // many action options for specifying an Inbox rule.
            newRule.Actions.MoveToFolder = WellKnownFolderName.JunkEmail;
            newRule.Actions.AssignCategories.Add("Junk category");

            // Specify the exceptions that exclude the rule from running on an email message. There are
            // many exceptions that can be specified on an Inbox rule.
            newRule.Exceptions.IsMeetingRequest = true;

            // Create the CreateRuleOperation object.
            CreateRuleOperation createOperation = new CreateRuleOperation(newRule);
            
            // Update the collection of Inbox rules with the new rule. This results in an UpdateInboxRules operation
            // call to EWS.
            service.UpdateInboxRules(new RuleOperation[] { createOperation }, true);
        }
    }
}
