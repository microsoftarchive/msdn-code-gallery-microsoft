using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Exchange.WebServices.Data;

namespace ManagingTasksUsingEWS
{
    public class TaskManager
    {
        private  ExchangeService _service;
        private  string _taskCreatorEmailID;
        private  string _password;
        public ExchangeService Service;
        

        public TaskManager(string taskCreatorEmailID,string password)
        {
            _taskCreatorEmailID = taskCreatorEmailID;
            _password = password;
            Service = GetExchangeService();
        }

        private ExchangeService GetExchangeService()
        {
            if (_service == null)
            {
                ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
                _service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
                _service.Credentials = new WebCredentials(_taskCreatorEmailID, _password);
                _service.TraceEnabled = true;
                _service.TraceFlags = TraceFlags.All;
                _service.AutodiscoverUrl(_taskCreatorEmailID, RedirectionUrlValidationCallback);
            }
            return _service;
        }

        private static bool CertificateValidationCallBack(
                            object sender,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// The method will create task in "Tasks" folder
        /// </summary>
        /// <param name="subject">Subject of the task</param>
        /// <param name="message">Message body of the task</param>
        /// <param name="startDate">Start date of the task</param>
        /// 
        public  void CreateTask(string subject, string message,DateTime startDate)
        {
            // Instaniate the Task object.
            Task task = new Task(Service);

            // Assign the subject, body and start date of the new task.
            task.Subject = subject;
            task.Body = new MessageBody(BodyType.Text,message);
            task.StartDate = startDate;

            // Create the new task in the Tasks folder.
            task.Save(WellKnownFolderName.Tasks);

        }

        /// <summary>
        /// The method will return all the tasks from "Tasks" folder
        /// </summary>
        /// <returns>List of tasks</returns>
        /// 
        public  FindItemsResults<Item> GetAllTasks()
        {
           
            // Specify the folder to search, and limit the properties returned in the result.
            TasksFolder tasksfolder = TasksFolder.Bind(Service,
                                                       WellKnownFolderName.Tasks,
                                                       new PropertySet(BasePropertySet.IdOnly, FolderSchema.TotalCount));

            // Set the count of items to be displayed.
            int numItems = tasksfolder.TotalCount > 50 ? tasksfolder.TotalCount : 50;

            // Instantiate the item view with the number of items to retrieve from the required folder.
            ItemView view = new ItemView(numItems);
            // Retrieve the items in the Tasks folder.
            FindItemsResults<Item> taskItems = Service.FindItems(WellKnownFolderName.Tasks, view);
            //Load Body and other 
            Service.LoadPropertiesForItems(taskItems, PropertySet.FirstClassProperties);

            if (taskItems!=null && taskItems.Count() > 0)
            {

                return taskItems;
            }
            else
            {
                return null;
            }
        }
    }
}