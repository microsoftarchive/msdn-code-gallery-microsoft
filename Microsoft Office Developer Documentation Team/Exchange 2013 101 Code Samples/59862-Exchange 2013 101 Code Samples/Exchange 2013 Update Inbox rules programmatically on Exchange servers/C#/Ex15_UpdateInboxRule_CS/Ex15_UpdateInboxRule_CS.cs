using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UpdateInboxRule_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            UpdateInboxRule(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Updates the rule conditions, exceptions, and actions on a rule named MoveInterestingToJunk.
        /// Updates to the Inbox rules collection can be batched.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void UpdateInboxRule(ExchangeService service)
        {
            // Get all the Inbox rules in the user's mailbox. This results in a GetInboxRules operation call to EWS.
            RuleCollection ruleCollection = service.GetInboxRules();

            // Inbox rule updates can be batched into a single call to EWS.
            Collection<RuleOperation> ruleOperations = new Collection<RuleOperation>();

            foreach (Rule rule in ruleCollection)
            {
                if (rule.DisplayName == "MoveInterestingToJunk")
                {
                    Console.WriteLine("Found a rule named MoveInterestingToJunk...");

                    // Update the ContainsSubjectStrings condition predicate. The GetInboxRule
                    // sample contains a list of rule predicates.
                    rule.Conditions.ContainsSubjectStrings.Clear();
                    rule.Conditions.ContainsSubjectStrings.Add("This is Junk");

                    Console.WriteLine("Cleared the ContainsSubjectStrings condition and added a new condition " +
                                      "ContainsSubjectStrings condition where the subject string contains 'This is Junk'...");

                    // Update the IsMeetingResponse exception predicate.
                    rule.Exceptions.IsMeetingResponse = true;

                    Console.WriteLine("Added an IsMeetingResponse exception for the conditions...");

                    // Update the rule actions.
                    rule.Actions.StopProcessingRules = true;

                    Console.WriteLine("Updated the rule actions with a StopProcessingRules action.");

                    // Add each rule to update into a RuleOperation collection.
                    ruleOperations.Add(new SetRuleOperation(rule));
                }
            }

            // The Inbox rules are updated here. This results in an UpdateInboxRules operation call to EWS.
            service.UpdateInboxRules(ruleOperations, true);

            Console.WriteLine("Successfully updated the rules collection in your mailbox.");

        }
    }
}
