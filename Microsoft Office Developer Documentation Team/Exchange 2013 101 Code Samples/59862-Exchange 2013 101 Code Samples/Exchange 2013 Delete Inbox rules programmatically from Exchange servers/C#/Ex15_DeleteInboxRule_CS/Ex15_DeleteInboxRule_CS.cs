using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_DeleteInboxRule_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DeleteInboxRule(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Deletes all Inbox rules named MoveInterestingToJunk. Deletion to the Inbox rules 
        /// collection can be batched into a single EWS call.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void DeleteInboxRule(ExchangeService service)
        {
            // Get all the Inbox rules in the user's mailbox. This results in a GetInboxRules operation
            // call to EWS.
            RuleCollection ruleCollection = service.GetInboxRules();

            Console.WriteLine("Returned your inbox rules from Exchange...");

            // Inbox rule updates, including deletions, can be batched into a single call to EWS.
            Collection<RuleOperation> ruleOperations = new Collection<RuleOperation>();

            foreach (Rule rule in ruleCollection)
            {
                if (rule.DisplayName == "MoveInterestingToJunk")
                {
                    Console.WriteLine("Found an Inbox rule to delete...");

                    DeleteRuleOperation deleteRule = new DeleteRuleOperation(rule.Id);

                    // Add each rule to deletion into a RuleOperation collection. Update operations
                    // can also be added to batch up changes to Inbox rules.
                    ruleOperations.Add(deleteRule);

                    Console.WriteLine("Added the Inbox rule to the collection of rules to update...");
                }
            }

            // The inbox rules are deleted here. This results in an UpdateInboxrules operaion call to EWS. 
            service.UpdateInboxRules(ruleOperations, true);

            Console.WriteLine("Deleted Inbox rule(s)...");
        }
    }
}
