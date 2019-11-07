using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.Office.TranslationServices.Client;
using System.Collections.ObjectModel;

namespace TranslateClientOM
{
    class Program
    {
        static ClientContext cc;
        static Site site = null;
        static string siteName;

        static void Sync(string culture, string input, string output)
        {            
            SyncTranslator job = new SyncTranslator(cc, culture);
            Console.WriteLine("Adding files");
            Console.WriteLine("Input: " + input);
            Console.WriteLine("Output: " + output);            
            job.OutputSaveBehavior = SaveBehavior.AlwaysOverwrite;
            ClientResult<TranslationItemInfo> cr = job.Translate(input, output);                    
            cc.ExecuteQuery();
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("OutputSaveBehavior: {0}", job.OutputSaveBehavior.ToString());
            Console.WriteLine("Input: " + cr.Value.InputFile);
            Console.WriteLine("Output: " + cr.Value.OutputFile);            
            Console.WriteLine("ErrorMessage: " + cr.Value.ErrorMessage);
            Console.WriteLine("TranslationId: " + cr.Value.TranslationId);
            Console.WriteLine("JobStatus....");
            Console.WriteLine("Succeeded: " + cr.Value.Succeeded);
            Console.WriteLine("Failed: " + cr.Value.Failed);
            Console.WriteLine("Canceled: " + cr.Value.Canceled);
            Console.WriteLine("InProgress: " + cr.Value.InProgress);
            Console.WriteLine("NotStarted: " + cr.Value.NotStarted);
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Done");  
        }
        
        static void AsyncFile(string culture, string input, string output, string name)
        {
            TranslationJob job = new TranslationJob(cc, culture);
            Console.WriteLine("Adding files");
            Console.WriteLine("Input: " + input);
            Console.WriteLine("Output: " + output);
            job.AddFile(input, output);
            job.Name = name;
            //test            
            job.Start();
            cc.Load(job);
            cc.ExecuteQuery();
            Console.WriteLine("JobId: " + job.JobId);
            Console.WriteLine("JobName: " + job.Name);
            Console.WriteLine("Done");  
        }

        static void TestLanguageAndFileExtension()
        {                        
            Console.WriteLine("Print File Extensions");
            IEnumerable<string> fileExitems = TranslationJob.EnumerateSupportedFileExtensions(cc);
            cc.ExecuteQuery();            
            foreach (string item in fileExitems)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine();
            Console.WriteLine("Print Supported Language");
            IEnumerable<string> langitems = TranslationJob.EnumerateSupportedLanguages(cc);
            cc.ExecuteQuery();
            foreach (string item in langitems)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine();

            TestIsFileExtensionSupported("docx");
            TestIsFileExtensionSupported("xlf");
            TestIsFileExtensionSupported("no");

            TestIsLanguageSupported("th-th");
            TestIsLanguageSupported("es");
            //invalid languages
            TestIsLanguageSupported("af-za");
            TestIsLanguageSupported("hi");

            TestGetMaximumFileSize("docx");
            TestGetMaximumFileSize("txt");
            TestGetMaximumFileSize("xlf");
        }

        static void TranslationJobStatusGetJob()
        {
            IEnumerable<TranslationJobInfo> jobs;
            Console.WriteLine("Active jobs: Current Users");
            jobs = TranslationJobStatus.GetAllActiveJobs(cc);
            cc.ExecuteQuery();
            foreach (TranslationJobInfo activeJobInfo in jobs)
            {
                Console.WriteLine("JobId:" + activeJobInfo.JobId + ", JobName: " + activeJobInfo.Name +
                    ", Submitted:" + activeJobInfo.SubmittedTime + ", Cancel Time:" + activeJobInfo.CancelTime +
                    ", Canceled:" + activeJobInfo.Canceled + ", PartiallySubmitted: " + activeJobInfo.PartiallySubmitted);
            }

            Console.WriteLine("All jobs: Current Users");
            jobs = TranslationJobStatus.GetAllJobs(cc);
            cc.ExecuteQuery();
            foreach (TranslationJobInfo allJobInfo in jobs)
            {
                Console.WriteLine("JobId:" + allJobInfo.JobId + ", JobName: " + allJobInfo.Name +
                    ", Submitted:" + allJobInfo.SubmittedTime + ", Cancel Time:" + allJobInfo.CancelTime +
                    ", Canceled:" + allJobInfo.Canceled + ", PartiallySubmitted: " + allJobInfo.PartiallySubmitted);
            }
            
        }

        static void TestGetItems(string guid)
        {
            TranslationJobStatus jobStatus = new TranslationJobStatus(cc, new Guid(guid));
            cc.Load(jobStatus);
            cc.ExecuteQuery();
            Console.WriteLine("Job Status");
            Console.WriteLine("jobStatus.Name" + jobStatus.Name);            
            Console.WriteLine("jobStatus.Count" + jobStatus.Count);
            Console.WriteLine("jobStatus.Canceled" + jobStatus.Canceled);
            Console.WriteLine("jobStatus.Failed" + jobStatus.Failed);
            Console.WriteLine("jobStatus.InProgress" + jobStatus.InProgress);            
            Console.WriteLine("jobStatus.NotStarted" + jobStatus.NotStarted);
            Console.WriteLine("jobStatus.Succeeded" + jobStatus.Succeeded);            
            ListJobItems(jobStatus, ItemTypes.Canceled);
            ListJobItems(jobStatus, ItemTypes.Failed);
            jobStatus.Refresh();
            cc.ExecuteQuery();
            ListJobItems(jobStatus, ItemTypes.InProgress);
            ListJobItems(jobStatus, ItemTypes.NotStarted);
            ListJobItems(jobStatus, ItemTypes.Succeeded);
        }

        static void ListJobItems(TranslationJobStatus jobStatus, ItemTypes type)
        {
            Console.WriteLine("List items of types " + type.ToString());
            IEnumerable<TranslationItemInfo> items = jobStatus.GetItems(type);
            cc.ExecuteQuery();
            foreach (TranslationItemInfo item in items)
            {
                Console.WriteLine("TranslationId: " + item.TranslationId + "; Succeeded: " + item.Succeeded + "\n" +
                                  "InputFile: " + item.InputFile + "; OutputFile: " + item.OutputFile + "\n" +
                                  "Canceled: " + item.Canceled + "; Failed: " + item.Failed + "\n" +
                                  "InProgress: " + item.InProgress + "; NotStarted: " + item.NotStarted + "\n" +
                                  "; ErrorMessage " + item.ErrorMessage);
            }
        }

        static void AsyncFolder(string culture, string inFolder, string outFolder, string name)
        {                       
            Folder folderIn = cc.Web.GetFolderByServerRelativeUrl(inFolder);
            Folder folderOut = cc.Web.GetFolderByServerRelativeUrl(outFolder);            
            TranslationJob job = new TranslationJob(cc, culture);
            job.AddFolder(folderIn, folderOut, true);
            job.Name = name;
            job.Start();            
            cc.Load(job);
            cc.ExecuteQuery();
            Console.WriteLine("JobId: " + job.JobId);
            Console.WriteLine("JobName: " + job.Name);
            Console.WriteLine("Done");  
        }

        static void AsyncLib(string culture, string inputList, string outputList, string name)
        {
            List inList = cc.Web.Lists.GetByTitle(inputList);
            List outList = cc.Web.Lists.GetByTitle(outputList);            
            TranslationJob job = new TranslationJob(cc, culture);
            job.AddLibrary(inList, outList);
            job.Name = name;
            job.Start();
            cc.Load(job);
            cc.ExecuteQuery();
            Console.WriteLine("JobId: " + job.JobId);
            Console.WriteLine("JobName: " + job.Name);
            Console.WriteLine("Done");                                  
        }

        static void TestCancelJob(string jobid)
        {
            TranslationJob.CancelJob(cc, new Guid(jobid));
            cc.ExecuteQuery();
        }

        static void TestIsFileExtensionSupported(string fileEx)
        {
            ClientResult<bool> bTemp;
            bTemp = TranslationJob.IsFileExtensionSupported(cc, fileEx);
            cc.ExecuteQuery();
            Console.WriteLine("IsFileExtensionSupported for " + fileEx + " " + bTemp.Value);
        }

        static void TestIsLanguageSupported(string lang)
        {
            ClientResult<bool> bTemp;
            bTemp = TranslationJob.IsLanguageSupported(cc, lang);
            cc.ExecuteQuery();
            Console.WriteLine("IsLanguageSupported for " + lang + " " + bTemp.Value);
        }

        static void TestGetMaximumFileSize(string fileEx)
        {
            ClientResult<int> intTemp;
            intTemp = TranslationJob.GetMaximumFileSize(cc, fileEx);
            cc.ExecuteQuery();
            Console.WriteLine("GetMaximumFileSize for " + fileEx + " " + intTemp.Value);
        }


        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Sync File                        : 1 server targetLanguage inputFile(FullURL) outputFile(FullURL)");
                Console.WriteLine(@"                         Sample : 1 http://sp th http://sp/docLib/test.docx http://sp/docLib/test-th.docx");
                Console.WriteLine("Async File                       : 2f server targetLanguage inputFile(FullURL) outputFile(FullURL) jobName");
                Console.WriteLine("Async Folder                     : 2o server targetLanguage Inputfolder(ServerRelativeURL) Outputfolder(ServerRelativeURL) jobName");
                Console.WriteLine(@"                         Sample : 2o http://sp th testlib/in testlib/out job1");
                Console.WriteLine("Async Lib                        : 2l server targetLanguage InputLibName OutputLibName jobName");
                Console.WriteLine(@"                         Sample : 2l http://sp th inTestLib outTestLib");
                Console.WriteLine("Get All/ActiveJob Status         : 3 server");
                Console.WriteLine("Get/Test Lang/FileExt/FileSize   : 4 server");
                Console.WriteLine("Cancel Job                       : 5 server jobId");   
                Console.WriteLine(@"                         Sample : 5 http://sp 00000000-0000-108b-8065-8ad2ad98e764");
                Console.WriteLine("Get Job Items                    : 6 server jobId");
            }
            else
            {
                cc = new ClientContext(args[1]);
                siteName = args[1];
                site = cc.Site;
                switch (args[0])
                {
                    case "1" : Sync(args[2], args[3], args[4]);
                        break;
                    case "2f": AsyncFile(args[2], args[3], args[4], args[5]);
                        break;
                    case "2o": AsyncFolder(args[2], args[3], args[4], args[5]);
                        break;
                    case "2l": AsyncLib(args[2], args[3], args[4], args[5]);
                        break;
                    case "3": TranslationJobStatusGetJob();
                        break;
                    case "4": TestLanguageAndFileExtension();
                        break;
                    case "5": TestCancelJob(args[2]);
                        break;
                    case "6": TestGetItems(args[2]);
                        break;

                }

                
            }  
        }
    }
}
