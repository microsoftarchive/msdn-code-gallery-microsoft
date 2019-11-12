using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetInboxRule_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetInboxRule(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Gets all the Inbox rules for a mailbox. This includes all conditions, exceptions, and
        /// actions for each rule. 
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void GetInboxRule(ExchangeService service)
        {
            // Get all the inbox rules in the user's mailbox. This results in a GetInboxRules operation call to EWS.
            RuleCollection ruleCollection = service.GetInboxRules();

            foreach (Rule rule in ruleCollection)
            {
                int count = 0;

                // Access rule information.
                Console.WriteLine("\n\r\n\rRule display name: " + rule.DisplayName);
                Console.WriteLine("Rule id: " + rule.Id);
                Console.WriteLine("Is rule enabled: " + rule.IsEnabled.ToString());
                Console.WriteLine("Does the rule have an error: " + rule.IsInError);
                Console.WriteLine("Is the rule supported: " + rule.IsNotSupported.ToString());
                Console.WriteLine("Rule priority: " + rule.Priority);

                // Access rule conditions.
                Console.WriteLine("\n\rRule Conditions");
                
                foreach (string str in rule.Conditions.Categories)
                {
                    count++;
                    Console.WriteLine("Category condition #{0}: " + str, count);
                }
                count = 0; 
                foreach (string str in rule.Conditions.ContainsSubjectStrings)
                {
                    count++;
                    Console.WriteLine("Contains subject strings condition #{0}: " + str, count);
                }
                
                // These are the rule predicates that can be returned for an Inbox rule
                // condition or exception. Your code should check the value of each potential
                // rule predicate for conditions and exceptions.

                // ContainsBodyStrings
                // ContainsHeaderStrings
                // ContainsRecipientStrings
                // ContainsSenderStrings
                // ContainsSubjectOrBodyStrings
                // FlaggedForAction
                // FromAddresses
                // FromConnectedAccounts
                // HasAttachments
                // Importance
                // IsApprovalRequest
                // IsAutomaticForward
                // IsAutomaticReply
                // IsEncrypted
                // IsMeetingRequest
                // IsMeetingResponse
                // IsNonDeliveryReport
                // IsPermissionControlled
                // IsReadReceipt
                // IsSigned
                // IsVoicemail
                // ItemClasses
                // MessageClassifications
                // NotSentToMe
                // Sensitivity
                // SentCcMe
                // SentOnlyToMe
                // SentToAddresses
                // SentToMe
                // SentToOrCcMe
                // WithinDateRange
                // WithinSizeRange

                // Access rule exceptions. Rule exceptions are the same RulePredicates
                // as for rule conditions.
                Console.WriteLine("\n\rRule exceptions");
                Console.WriteLine("IsMeetingRequest rule exception: " + rule.Exceptions.IsMeetingRequest.ToString());

                // Access rule actions. Your code should process all potential 
                // rule actions.
                Console.WriteLine("\n\rRule actions");
                if (rule.Actions.MoveToFolder != null)
                {
                    Console.WriteLine("Move to folder ID: " + rule.Actions.MoveToFolder.UniqueId);
                }
                foreach (string str in rule.Actions.AssignCategories)
                {
                    count++;
                    Console.WriteLine("Category assignment action #{0}: " + str, count);
                }

                // These are the rule actions that can be returned for an Inbox rule. Your code should check 
                // the value of each potential rule action. 

                // AssignCategories
                // CopyToFolder
                // Delete
                // ForwardAsAttachmentToRecipients
                // ForwardToRecipients
                // MarkAsRead
                // MarkImportance
                // MoveToFolder
                // PermanentDelete
                // RedirectToRecipients
                // SendSMSAlertToRecipients
                // ServerReplyWithMessage
                // StopProcessingRules
            }
        }
    }
}
