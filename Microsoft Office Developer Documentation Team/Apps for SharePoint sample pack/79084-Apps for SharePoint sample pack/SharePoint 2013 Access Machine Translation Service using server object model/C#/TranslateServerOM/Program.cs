using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.TranslationServices;
using Microsoft.Office.TranslationServices.Parsing;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Management.Automation;
using System.Text;

class Program
{
    static string site;    
    static SPServiceContext sc;
           
    /// <summary>
    /// submit a sync job to translate a file
    /// </summary>
    /// <param name="culture">target langauge</param>
    /// <param name="input">full URL of input file on SharePoint</param>
    /// <param name="output">full URL of output file on SharePoint</param>
    static void AddSyncFile(string culture, string input, string output)
    {
        SyncTranslator job = createSyncTranslationJob(culture);
        Console.WriteLine("Adding files");
        Console.WriteLine("Input: " + input);
        Console.WriteLine("Output: " + output);
        TranslationItemInfo itemInfo = job.Translate(input, output);
        Console.WriteLine("Targetlang: {0}", job.TargetLanguage.Name);
        Console.WriteLine("OutputSaveBehavior: {0}", job.OutputSaveBehavior.ToString());
        PrintItemInfo(itemInfo);
    }

    /// <summary>
    /// submit a sync job to translate stream
    /// the input file will be converted to stream and send to translation
    /// </summary>
    /// <param name="culture">target langauge</param>
    /// <param name="input">input file location (not on SharePoint)</param>
    /// <param name="output">output file location (not on SharePoint)</param>
    /// <param name="fileFormat">file extension of the input file</param>
    static void AddSyncStream(string culture, string input, string output, string fileFormat)
    {
        SyncTranslator job = createSyncTranslationJob(culture);
        FileStream inputStream = new FileStream(input, FileMode.Open);
        FileStream outputStream = new FileStream(output, FileMode.Create);     
        TranslationItemInfo itemInfo = job.Translate(inputStream, outputStream, fileFormat);
        Console.WriteLine("Targetlang: {0}", job.TargetLanguage.Name);
        Console.WriteLine("OutputSaveBehavior: {0}", job.OutputSaveBehavior.ToString());
        PrintItemInfo(itemInfo);
        inputStream.Close();
        outputStream.Flush();
        outputStream.Close();        
    }

    /// <summary>
    /// submit a sync job to translate array of bytes
    /// the input file will be converted to array of bytes and send to translation
    /// </summary>
    /// <param name="culture">target langauge</param>
    /// <param name="input">input file location (not on SharePoint)</param>
    /// <param name="output">output file location (not on SharePoint)</param>
    /// <param name="fileFormat">file extension of the input file</param>
    static void AddSyncByte(string culture, string input, string output, string fileFormat)
    {
        SyncTranslator job = createSyncTranslationJob(culture);
        Byte[] inputByte;
        Byte[] outputByte;
        inputByte = File.ReadAllBytes(input);
        outputByte = null;
        TranslationItemInfo itemInfo = job.Translate(inputByte, out outputByte, fileFormat);
        Console.WriteLine("Targetlang: {0}", job.TargetLanguage.Name);
        Console.WriteLine("OutputSaveBehavior: {0}", job.OutputSaveBehavior.ToString());
        PrintItemInfo(itemInfo);
        Console.WriteLine("Writing to output file");
        FileStream outputStream = File.Open(output, FileMode.Create);
        MemoryStream memoryStream = new MemoryStream(outputByte);
        memoryStream.WriteTo(outputStream);
        outputStream.Flush();
        outputStream.Close();        
    }

    /// <summary>
    /// submit an async job to translate a file
    /// </summary>
    /// <param name="culture">target langauge</param>
    /// <param name="input">full URL of input file</param>
    /// <param name="output">full URL of output file</param>
    static void AddAsyncFile(string culture, string input, string output, string user)
    {
        SPServiceContext sc = SPServiceContext.GetContext(new SPSite(site));
        TranslationJob job = new TranslationJob(sc, CultureInfo.GetCultureInfo(culture));
        Encoding encoding = new System.Text.UTF8Encoding(); 
        if (!String.IsNullOrEmpty(user))
        {
            job.UserToken = ConvertHexStringToByteArray(user);
            //job.UserToken = encoding.GetBytes(user);
        } 
        Console.WriteLine("Input: " + input);
        Console.WriteLine("Output: " + output);
        Console.WriteLine("targetlang {0}:", job.TargetLanguage.Name);
        job.AddFile(input, output);      
        Console.WriteLine("Submitting the job");        
        job.Start();
        if (job.UserToken != null)
        {
            Console.WriteLine("User Token:" + encoding.GetString(job.UserToken));
        }
        ListJobItemInfo(job);
    }

    /// <summary>
    /// submit an async job to translate folder(s)
    /// </summary>
    /// <param name="culture">target langauge</param>
    /// <param name="inputFolder">Full URL of the input folder on SharePoint</param>
    /// <param name="outputFolder">Full URL of the output folder on SharePoint</param>
    static void AddAsyncFolder(string culture, string inputFolder, string outputFolder)
    {
        SPServiceContext sc = SPServiceContext.GetContext(new SPSite(site));
        TranslationJob job = new TranslationJob(sc, CultureInfo.GetCultureInfo(culture));                    
        using (SPSite siteIn = new SPSite(inputFolder))
        {
            Console.WriteLine("In site: {0}", siteIn);
            using (SPWeb webIn = siteIn.OpenWeb())
            {
                Console.WriteLine("In web: {0}", webIn);
                using (SPSite siteOut = new SPSite(outputFolder))
                {
                    Console.WriteLine("Out site: {0}", siteOut);
                    using (SPWeb webOut = siteOut.OpenWeb())
                    {
                        Console.WriteLine("Out web: {0}", webOut);
                        SPFolder folderIn = webIn.GetFolder(inputFolder);
                        SPFolder folderOut = webOut.GetFolder(outputFolder);                    
                        Console.WriteLine("Input: " + folderIn);
                        Console.WriteLine("Output: " + folderOut);
                        //true means to recursive all the sub folders
                        job.AddFolder(folderIn, folderOut, true);
                        Console.WriteLine("Submitting the job");                        
                        job.Start();
                        ListJobItemInfo(job);
                    }
                }
            }
        }
    }

    /// <summary>
    /// submit an async job to translate a document library
    /// </summary>
    /// <param name="culture">target langauge</param>
    /// <param name="inputList">Full URL of the input document library on SharePoint</param>
    /// <param name="outputList">Full URL of the output document library on SharePoint</param>
    static void AddAsyncLibrary(string culture, string inputList, string outputList)
    {
        SPServiceContext sc = SPServiceContext.GetContext(new SPSite(site));
        TranslationJob job = new TranslationJob(sc, CultureInfo.GetCultureInfo(culture));
        using (SPSite siteIn = new SPSite(inputList))
        {
            Console.WriteLine("In site: {0}", siteIn);
            using (SPWeb webIn = siteIn.OpenWeb())
            {
                Console.WriteLine("In web: {0}", webIn);
                using (SPSite siteOut = new SPSite(outputList))
                {
                    Console.WriteLine("Out site: {0}", siteOut);
                    using (SPWeb webOut = siteOut.OpenWeb())
                    {
                        Console.WriteLine("Out web: {0}", webOut);
                        SPDocumentLibrary listIn = (SPDocumentLibrary)webIn.GetList(inputList);
                        SPDocumentLibrary listOut = (SPDocumentLibrary)webOut.GetList(outputList);
                        Console.WriteLine("Input: " + listIn);
                        Console.WriteLine("Output: " + listOut);                        
                        job.AddLibrary(listIn, listOut);
                        Console.WriteLine("Submitting the job");                        
                        job.Start();
                        ListJobItemInfo(job);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Print 4 types of jobs
    ///     1.All active jobs of all users
    ///     2.All active jobs of the current user
    ///     3.All jobs of all users
    ///     4.All jobs of the current user    
    /// </summary>
    static void GetJobStatus()
    {
        ReadOnlyCollection<TranslationJobInfo> activeJobs;

        Console.WriteLine("=====Active jobs: All Users=====================");
        activeJobs = TranslationJobStatus.GetAllActiveJobs(sc, TranslationJobUserScope.AllUsers);
        foreach (TranslationJobInfo activeJobInfo in activeJobs)
        {
            Console.WriteLine("JobId:" + activeJobInfo.JobId + ", JobName: " + activeJobInfo.Name +
                ", Submitted:" + activeJobInfo.SubmittedTime + ", Canceled:" + activeJobInfo.CancelTime);
        }

        Console.WriteLine("=====Active jobs: Current Users=====================");
        activeJobs = TranslationJobStatus.GetAllActiveJobs(sc, TranslationJobUserScope.CurrentUser);
        foreach (TranslationJobInfo activeJobInfo in activeJobs)
        {
            Console.WriteLine("JobId:" + activeJobInfo.JobId + ", JobName: " + activeJobInfo.Name +
                ", Submitted:" + activeJobInfo.SubmittedTime + ", Canceled:" + activeJobInfo.CancelTime);
        }
        ReadOnlyCollection<TranslationJobInfo> allJobs;
        Console.WriteLine("=====All jobs: All Users=====================");
        allJobs = TranslationJobStatus.GetAllJobs(sc, TranslationJobUserScope.AllUsers);
        foreach (TranslationJobInfo allJobInfo in allJobs)
        {
            Console.WriteLine("JobId:" + allJobInfo.JobId + ", JobName: " + allJobInfo.Name +
                ", Submitted:" + allJobInfo.SubmittedTime + ", Canceled:" + allJobInfo.CancelTime);
        }
        Console.WriteLine("=====All jobs: Current Users=====================");
        allJobs = TranslationJobStatus.GetAllJobs(sc, TranslationJobUserScope.CurrentUser);
        foreach (TranslationJobInfo allJobInfo in allJobs)
        {
            Console.WriteLine("JobId:" + allJobInfo.JobId + ", JobName: " + allJobInfo.Name +
                ", Submitted:" + allJobInfo.SubmittedTime + ", Canceled:" + allJobInfo.CancelTime);
        }
    }

    /// <summary>
    /// get items (documents) of the job
    /// call GetJobStatus() to get job ids
    /// </summary>
    /// <param name="jobId">job guid</param>
    static void GetJobItems(string jobId)
    {
        SPServiceContext sc = SPServiceContext.GetContext(new SPSite(site));
        TranslationJobStatus jobStatus = new TranslationJobStatus(sc, new Guid(jobId));
        ListJobItems(jobStatus, ItemTypes.Canceled);
        ListJobItems(jobStatus, ItemTypes.Failed);
        ListJobItems(jobStatus, ItemTypes.InProgress);
        ListJobItems(jobStatus, ItemTypes.NotStarted);
        ListJobItems(jobStatus, ItemTypes.Succeeded);
    }

    /// <summary>
    /// Print supported target langugaes
    /// </summary>
    static void GetSupportedLanguages()
    {
        foreach (CultureInfo item in TranslationJob.EnumerateSupportedLanguages(sc))
        {
            Console.Write(item.Name + " ");
        }
    }

    /// <summary>
    /// Print supported file extensions
    /// </summary>
    static void GetSupportedFileExtensions()
    {
        foreach (string item in TranslationJob.EnumerateSupportedFileExtensions(sc))
        {
            Console.Write(item + " ");
        }
    }

    /// <summary>
    /// print maximum file size of each supported file extension that the service will take
    /// </summary>
    static void GetMaximumFileSize()
    {
        SPServiceContext sc = SPServiceContext.GetContext(new SPSite(site));
        string[] fileEx = { "docx", "doc", "docm", "dotx", "dotm", "dot", "rtf", "html", "htm", "aspx", "ascx", "xhtml", "xhtm", "txt", "xlf", "xxx" };
        foreach (string ex in fileEx)
        {
            Console.Write(ex + ": ");
            Console.WriteLine(TranslationJob.GetMaximumFileSize(sc, ex).ToString());
        }
    }

    /// <summary>
    /// cancel submitted job
    /// call GetJobStatus() to get job ids
    /// </summary>
    /// <param name="jobid">job guid</param>
    static void CancelJob(string jobid)
    {
        TranslationJob.CancelJob(sc, new Guid(jobid));
    }

    #region help methods
    static SyncTranslator createSyncTranslationJob(string jobCulture)
    {
        Console.WriteLine("Creating Sync job");
        SyncTranslator job = new SyncTranslator(sc, CultureInfo.GetCultureInfo(jobCulture));
        return job;
    }

    static TranslationJob createTranslationJob(string jobCulture)
    {
        Console.WriteLine("Creating job");
        TranslationJob job = new TranslationJob(sc, CultureInfo.GetCultureInfo(jobCulture));
        return job;
    }

    static string AddCultureToFileName(string fileName, string culture)
    {
        string outputFileName = fileName.Substring(0, fileName.IndexOf(".")) + "-" + culture + "." +
            fileName.Substring(fileName.IndexOf(".") + 1, fileName.Length - (fileName.IndexOf(".") + 1));
        return outputFileName;
    }

    static void ListJobItems(TranslationJobStatus jobStatus, ItemTypes type)
    {
        Console.WriteLine("List items of types " + type.ToString());
        IEnumerable<TranslationItemInfo> items = jobStatus.GetItems(type);
        foreach (TranslationItemInfo item in items)
        {
            Console.WriteLine("TranslationId: " + item.TranslationId + "; Succeeded: " + item.Succeeded + "\n" +
                              "InputFile: " + item.InputFile + "; OutputFile: " + item.OutputFile + "\n" +
                              "Canceled: " + item.Canceled + "; Failed: " + item.Failed + "\n" +
                              "InProgress: " + item.InProgress + "; NotStarted: " + item.NotStarted + "\n" +
                              "; ErrorMessage " + item.ErrorMessage);
           
        }
    }

    static void ListJobItemInfo(TranslationJob job)
    {
        Console.WriteLine("Enumerating items in the job...");
        foreach (KeyValuePair<string, string> kv in job.EnumerateItems())
        {
            Console.WriteLine(kv.Key + " " + kv.Value);
        }
    }

    static void PrintItemInfo(TranslationItemInfo itemInfo)
    {
        Console.WriteLine("Input: " + itemInfo.InputFile);
        Console.WriteLine("Output: " + itemInfo.OutputFile);
        Console.WriteLine("StartTime: " + itemInfo.StartTime);
        Console.WriteLine("CompleteTime: " + itemInfo.CompleteTime);
        Console.WriteLine("ErrorMessage: " + itemInfo.ErrorMessage);
        Console.WriteLine("TranslationId: " + itemInfo.TranslationId);
        Console.WriteLine("JobStatus....");
        Console.WriteLine("Succeeded: " + itemInfo.Succeeded);
        Console.WriteLine("Failed: " + itemInfo.Failed);
        Console.WriteLine("Canceled: " + itemInfo.Canceled);
        Console.WriteLine("InProgress: " + itemInfo.InProgress);
        Console.WriteLine("NotStarted: " + itemInfo.NotStarted);
    }

    static byte[] ConvertHexStringToByteArray(string hexString)
    {
        int byteLength = hexString.Length / 2;
        byte[] bytes = new byte[byteLength];
        string hex;
        int j = 0;
        for (int i=0; i<bytes.Length; i++)
        {
            hex = new String(new Char[] { hexString[j], hexString[j + 1] });
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            bytes[i] = byte.Parse(hex, NumberStyles.HexNumber);            
            j = j+2;
        }
        return bytes;
    }

    static void PrintHelp()
    {      
        Console.WriteLine("===Submit Synchronous translation================");
        Console.WriteLine("Sync File                     : 1f site targetLanguage inputfile(FullURL) outputfile(FullURL)");
        Console.WriteLine(@"                      Sample  : 1f http://sp th http://sp/docLib/test.docx http://sp/docLib/test-th.docx");
        Console.WriteLine("Sync Stream                   : 1s site targetLanguage inputFile outputFile fileformat");
        Console.WriteLine(@"                      Sample  : 1s http://sp th c:\data\test.docx c:\data\test-th.docx docx");
        Console.WriteLine("Sync Byte                     : 1b site targetLanguage inputFile outputFile fileformat");
        Console.WriteLine(@"                      Sample  : 1b http://sp th c:\data\test.docx c:\data\test-th.docx docx");
        Console.WriteLine(); 
        Console.WriteLine("===Submit Asynchronous translation*================");
        Console.WriteLine("Async File                    : 2f site targetLanguage inputfile(FullURL) outputfile(FullURL)");
        Console.WriteLine(@"                      Sample  : 2f http://sp th http://sp/docLib/test.docx http://sp/docLib/test-th.docx");
        Console.WriteLine("Async Folder                  : 2o site targetLanguage inputfolder(FullURL) outputfolder(FullURL)");
        Console.WriteLine(@"                      Sample  : 2o http://sp th http://sp/docLib/inputFolder http://sp/docLib/outputFolder");
        Console.WriteLine("Async Lib                     : 2l server targetLanguage inputfolder(FullURL) outputfolder(FullURL)");
        Console.WriteLine(@"                      Sample  : 2l http://sp th http://sp/inputDocLib http://sp/outputDocLib");
        Console.WriteLine();
        Console.WriteLine("===Others =======================================");
        Console.WriteLine("Get Job Status                : 3 site");
        Console.WriteLine("Get Job Items**               : 4 site jobId");        
        Console.WriteLine("Get Supported Languages       : 5 site");
        Console.WriteLine("Get Supported File Extensions : 6 site");
        Console.WriteLine("Get Maximum File Size         : 7 site");
        Console.WriteLine("Cancel Job**                  : 8 site jobId");
        Console.WriteLine(@"                      Sample  : 8 http://sp 00000000-0000-108b-8065-8ad2ad98e764");
        Console.WriteLine();
        Console.WriteLine(@"===Notes========================================");
        Console.WriteLine("*By default, the service will translate asyn jobs every 15 minutes."); 
        Console.WriteLine("**Call 'Get Job Status' to get jobId"); 
    }
    #endregion

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
        }
        else
        {
            site = args[1];
            sc = SPServiceContext.GetContext(new SPSite(site));
            switch (args[0])
            {                    
                case "1f":
                    AddSyncFile(args[2], args[3], args[4]);                    
                    break;
                case "1s":
                    AddSyncStream(args[2], args[3], args[4], args[5]);
                    break;
                case "1b":
                    AddSyncByte(args[2], args[3], args[4], args[5]);
                    break;
                case "2f":
                    if (args.Length == 6)
                    {
                        AddAsyncFile(args[2], args[3], args[4], args[5]);
                    }
                    else
                    {
                        AddAsyncFile(args[2], args[3], args[4], null);
                    }
                    break;
                case "2o":
                    AddAsyncFolder(args[2], args[3], args[4]);
                    break;
                case "2l":
                    AddAsyncLibrary(args[2], args[3], args[4]);
                    break;
                case "3":
                    GetJobStatus();
                    break;
                case "4":
                    GetJobItems(args[2]);
                    break;
                case "5":
                    GetSupportedLanguages();
                    break;
                case "6":
                    GetSupportedFileExtensions();
                    break;
                case "7":
                    GetMaximumFileSize();
                    break;
                case "8":
                    CancelJob(args[2]);
                    break;
                default:
                    Console.WriteLine("incorrect option");
                    break;
            }
        }
    }
}