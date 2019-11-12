using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using Microsoft.SharePoint.Client;
using Microsoft.Office.TranslationServices.Client;
using System.Threading;
using System.Windows.Threading;

namespace TranslateSilverlightSample
{
    public partial class MainPage : UserControl
    {
        ClientContext cc;
        Site site;                
        
        public MainPage()
        {
            InitializeComponent();            
        }

        private void CreateClientContext()
        {
            cc = new ClientContext(this.server.Text);
            site = cc.Site;
        }

        private void PrintResult(string s)
        {
            this.resultText.Text = s;
        }

        private void getAllJobs_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForGetAllJobs), null);          
        }

        private void CreateThreadForGetAllJobs(object dummy)
        {            
            try
            {
                string result = "";
                IEnumerable<TranslationJobInfo> jobs;
                result += "All jobs: Current Users \n";
                jobs = TranslationJobStatus.GetAllJobs(cc);
                cc.ExecuteQuery();
                foreach (TranslationJobInfo allJobInfo in jobs)
                {
                    result += "JobId:" + allJobInfo.JobId + ", JobName: " + allJobInfo.Name +
                       ", Submitted:" + allJobInfo.SubmittedTime + ", Cancel Time:" + allJobInfo.CancelTime +
                       ", Canceled:" + allJobInfo.Canceled + ", PartiallySubmitted: " + allJobInfo.PartiallySubmitted + "\n";
                }            
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);
             }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void getAllActiveJobs_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForGetAllActiveJobs), null);             
        }

        private void CreateThreadForGetAllActiveJobs(object dummy)
        {
            try
            {
                string result = "";
                IEnumerable<TranslationJobInfo> jobs;

                result += "Active jobs: Current Users \n";
                jobs = TranslationJobStatus.GetAllActiveJobs(cc);
                cc.ExecuteQuery();
                foreach (TranslationJobInfo activeJobInfo in jobs)
                {
                    result += "JobId:" + activeJobInfo.JobId + ", JobName: " + activeJobInfo.Name +
                        ", Submitted:" + activeJobInfo.SubmittedTime + ", Cancel Time:" + activeJobInfo.CancelTime +
                        ", Canceled:" + activeJobInfo.Canceled + ", PartiallySubmitted: " + activeJobInfo.PartiallySubmitted + "\n";
                }
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
            
        }

        private void ListAllLangs_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForListAllLangs), null);
        }

        private void CreateThreadForListAllLangs(object dummy)
        {
            try
            {
                string result = "";
                result += "Print Supported Languages \n";
                IEnumerable<string> langitems = TranslationJob.EnumerateSupportedLanguages(cc);
                cc.ExecuteQuery();
                foreach (string item in langitems)
                {
                    result += item + ", ";
                }
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void listAllFileExt_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForListAllFileExt), null);
        }

        private void CreateThreadForListAllFileExt(object dummy)
        {
            try
            {
                string result = "";
                result += "Print Supported file extensions \n";
                IEnumerable<string> fileExitems = TranslationJob.EnumerateSupportedFileExtensions(cc);
                cc.ExecuteQuery();
                foreach (string item in fileExitems)
                {
                    result += item + ", ";
                }
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void testLang_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForTestLang), this.inputItem.Text);     
        }

        private void CreateThreadForTestLang(object lang)
        {
            try
            {
                ClientResult<bool> bTemp;
                bTemp = TranslationJob.IsLanguageSupported(cc, (string)lang);
                cc.ExecuteQuery();
                string result = bTemp.Value.ToString();
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void testFileExt_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForTestFileExt), this.inputItem.Text); 
        }

        private void CreateThreadForTestFileExt(object fileExt)
        {
            try
            {
                ClientResult<bool> bTemp;
                bTemp = TranslationJob.IsFileExtensionSupported(cc, (string)fileExt);
                cc.ExecuteQuery();
                string result = bTemp.Value.ToString();
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);                
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void getFileSize_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForFileSize), this.inputItem.Text);
        }

        private void CreateThreadForFileSize(object fileExt)
        {            
            try
            {
                ClientResult<int> intTemp;
                intTemp = TranslationJob.GetMaximumFileSize(cc, (string)fileExt);
                cc.ExecuteQuery();            
                string result = intTemp.Value.ToString();
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);                
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void cancelJob_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForCancelJob), this.inputItem.Text);
        }

        private void CreateThreadForCancelJob(object jobId)
        {
            try
            {
                TranslationJob.CancelJob(cc, new Guid((string)jobId));
                cc.ExecuteQuery();
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), "Done"); 
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void getItem_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForGetItem), this.inputItem.Text);
        }

        private void CreateThreadForGetItem(object jobId)
        {
            try
            {
                string result = "";
                TranslationJobStatus jobStatus = new TranslationJobStatus(cc, new Guid((string)jobId));
                jobStatus.Refresh();
                cc.ExecuteQuery();
                result += ListJobItems(jobStatus, ItemTypes.Canceled);
                result += ListJobItems(jobStatus, ItemTypes.Failed);
                result += ListJobItems(jobStatus, ItemTypes.InProgress);
                result += ListJobItems(jobStatus, ItemTypes.NotStarted);
                result += ListJobItems(jobStatus, ItemTypes.Succeeded);
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result); 
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private string ListJobItems(TranslationJobStatus jobStatus, ItemTypes type)
        {            
            string result = "";
            result += "List items of types " + type.ToString() + "\n";
            IEnumerable<TranslationItemInfo> items = jobStatus.GetItems(type);
            cc.ExecuteQuery();
            foreach (TranslationItemInfo item in items)
            {
                result += "TranslationId: " + item.TranslationId + "; Succeeded: " + item.Succeeded + "\n" +
                                  "     InputFile: " + item.InputFile + "; OutputFile: " + item.OutputFile + "\n" +
                                  "     Canceled: " + item.Canceled + "; Failed: " + item.Failed + "\n" +
                                  "     InProgress: " + item.InProgress + "; NotStarted: " + item.NotStarted + "\n" +
                                  "; ErrorMessage " + item.ErrorMessage;
            }
            return result;            
        }

        private void sync_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            SyncTranslator job = new SyncTranslator(cc, this.culture.Text);            
            string input = this.inputFile.Text;
            string output = this.outputFile.Text;            
            job.OutputSaveBehavior = SaveBehavior.AlwaysOverwrite;
            ClientResult<TranslationItemInfo> cr = job.Translate(input, output);    
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForSync), cr); 
        }

        private void CreateThreadForSync(object translationJob)
        {
            try
            {
                string result = "";                             
                cc.ExecuteQuery();
                TranslationItemInfo itemInfo = ((ClientResult<TranslationItemInfo>) translationJob).Value;
                result += "DateTime.Now \n";                
                result += "Input: " + itemInfo.InputFile + "\n";
                result += "Output: " + itemInfo.OutputFile + "\n"; ;
                result += "ErrorMessage: " + itemInfo.ErrorMessage + "\n";
                result += "TranslationId: " + itemInfo.TranslationId + "\n";
                result += "JobStatus....\n";
                result += "Succeeded: " + itemInfo.Succeeded + "\n";
                result += "Failed: " + itemInfo.Failed + "\n";
                result += "Canceled: " + itemInfo.Canceled + "\n";
                result += "InProgress: " + itemInfo.InProgress + "\n";
                result += "NotStarted: " + itemInfo.NotStarted + "\n";
                result += DateTime.Now + "\n";
                result += "Done";
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result); 
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }

        private void asyncFile_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            TranslationJob job = new TranslationJob(cc, this.culture.Text);
            string input = this.inputFile.Text;
            string output = this.outputFile.Text;
            job.AddFile(input, output);
            job.Name = this.jobName.Text;
            job.Start();
            cc.Load(job);
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForAsync), job);       
        }

        private void asyncFolder_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            Folder folderIn = cc.Web.GetFolderByServerRelativeUrl(this.inputFile.Text);
            Folder folderOut = cc.Web.GetFolderByServerRelativeUrl(this.outputFile.Text);            
            TranslationJob job = new TranslationJob(cc, this.culture.Text);
            job.AddFolder(folderIn, folderOut, true);
            job.Start();
            cc.Load(job);
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForAsync), job);         
        }

        private void asyncLib_Click(object sender, RoutedEventArgs e)
        {
            CreateClientContext();
            List inList = cc.Web.Lists.GetByTitle(this.inputFile.Text);
            List outList = cc.Web.Lists.GetByTitle(this.outputFile.Text);            
            TranslationJob job = new TranslationJob(cc, this.culture.Text);
            job.AddLibrary(inList, outList);
            job.Start();
            cc.Load(job);
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateThreadForAsync), job);   
        }

        private void CreateThreadForAsync(object translationJob)
        {
            try
            {
                cc.ExecuteQuery();
                string result = "";
                TranslationJob job = (TranslationJob)translationJob;
                result += "JobId: " + job.JobId + "\n";
                result += "JobName: " + job.Name + "\n";
                result += "Done";
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), result);
            }
            catch (Exception e)
            {
                DispatcherOperation dop = Dispatcher.BeginInvoke(new Action<string>(PrintResult), e.ToString());
            }
        }
       
    }
}
