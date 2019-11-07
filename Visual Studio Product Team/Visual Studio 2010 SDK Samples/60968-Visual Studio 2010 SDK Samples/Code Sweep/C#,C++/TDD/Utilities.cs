/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    static class Utilities
    {
        public static bool HasFunctionThrown<ExceptionType>(Action func) where ExceptionType : Exception
        {
            bool hasThrown = false;
            try
            {
                func();
            }
            catch (ExceptionType)
            {
                hasThrown = true;
            }
            catch (TargetInvocationException e)
            {
                hasThrown = (e.InnerException is ExceptionType);
            }
            return hasThrown;
        }

        static List<string> _tempFiles = new List<string>();

        static public void CleanUpTempFiles()
        {
            foreach (string file in _tempFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                }
            }
            _tempFiles.Clear();
        }

        static public string CreateTempFile(string content, Encoding encoding, string extension)
        {
            string path = Path.GetTempFileName();
            if (extension != null)
            {
                path = Path.ChangeExtension(path, ".txt");
            }
            File.WriteAllText(path, content, encoding);
            _tempFiles.Add(path);
            return path;
        }

        static public string CreateTempTxtFile(string content, Encoding encoding)
        {
            return CreateTempFile(content, encoding, ".txt");
        }

        static public string CreateTempTxtFile(string content)
        {
            return CreateTempTxtFile(content, Encoding.Unicode);
        }

        static public string CreateTempFile(string content)
        {
            return CreateTempFile(content, Encoding.Unicode, null);
        }

        static public string CreateBigFile()
        {
            StringBuilder content = new StringBuilder();

            for (int i = 0; i < 1000000; ++i)
            {
                content.Append("abcd ");
            }

            return CreateTempFile(content.ToString());
        }

        static public List<T> ListFromEnum<T>(IEnumerable<T> enumerator)
        {
            List<T> result = new List<T>();

            foreach (T t in enumerator)
            {
                result.Add(t);
            }

            return result;
        }

        static public List<IVsTaskItem> TasksFromEnumerator(IVsEnumTaskItems enumerator)
        {
            List<IVsTaskItem> result = new List<IVsTaskItem>();

            IVsTaskItem[] items = new IVsTaskItem[] { null };
            uint[] fetched = new uint[] { 0 };

            for (enumerator.Reset(); enumerator.Next(1, items, fetched) == VSConstants.S_OK && fetched[0] == 1; /*nothing*/ )
            {
                result.Add(items[0]);
            }

            return result;
        }

        static public List<IVsTaskItem> TasksOfProvider(IVsTaskProvider provider)
        {
            IVsEnumTaskItems enumerator = null;
            provider.EnumTaskItems(out enumerator);
            return TasksFromEnumerator(enumerator);
        }

        static public string CreateTermTable(IEnumerable<string> terms)
        {
            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("<?xml version=\"1.0\"?>\n");
            fileContent.Append("<xmldata>\n");
            fileContent.Append("  <PLCKTT>\n");
            fileContent.Append("    <Lang>\n");

            foreach (string term in terms)
            {
                fileContent.Append("      <Term Term=\"" + term + "\" Severity=\"2\" TermClass=\"Geopolitical\">\n");
                fileContent.Append("        <Comment>For detailed info see - http://relevant-url-here.com</Comment>\n");
                fileContent.Append("      </Term>\n");
            }

            fileContent.Append("    </Lang>\n");
            fileContent.Append("  </PLCKTT>\n");
            fileContent.Append("</xmldata>\n");

            return CreateTempTxtFile(fileContent.ToString());
        }

        static public Project SetupMSBuildProject()
        {
            Project project = new Project();
            project.FullPath = Path.GetTempFileName();
            return project;
        }

        static public Project SetupMSBuildProject(IList<string> filesToScan, IList<string> termTableFiles)
        {
            Project project = SetupMSBuildProject();
            string projectFolder = Path.GetDirectoryName(project.FullPath);

            if (filesToScan.Count > 0)
            {
                project.Xml.AddItem("ItemGroup1", CodeSweep.Utilities.RelativePathFromAbsolute(filesToScan[0], projectFolder));
                if (filesToScan.Count > 1)
                {
                    for (int i = 1; i < filesToScan.Count; ++i)
                    {
                        project.Xml.AddItem("ItemGroup2", CodeSweep.Utilities.RelativePathFromAbsolute(filesToScan[i], projectFolder));
                    }
                }
            }

            List<string> rootedTermTables = new List<string>(termTableFiles);
            List<string> relativeTermTables = rootedTermTables.ConvertAll<string>(
                delegate(string rootedPath)
                {
                    return CodeSweep.Utilities.RelativePathFromAbsolute(rootedPath, projectFolder);
                });

            ProjectTargetElement target = project.Xml.AddTarget("AfterBuild");

            ProjectTaskElement newTask = target.AddTask("ScannerTask");
            newTask.Condition = "'$(RunCodeSweepAfterBuild)' == 'true'";
            newTask.ContinueOnError = "false";
            newTask.SetParameter("FilesToScan", "@(ItemGroup1);@(ItemGroup2)");
            newTask.SetParameter("TermTables", CodeSweep.Utilities.Concatenate(relativeTermTables, ";"));
            newTask.SetParameter("Project", "$(MSBuildProjectFullPath)");

            string usingTaskAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetLoadedModules()[0].FullyQualifiedName) + "\\BuildTask.dll";
            project.Xml.AddUsingTask("CodeSweep.BuildTask.ScannerTask", usingTaskAssembly, null);

            ProjectPropertyGroupElement group = project.Xml.AddPropertyGroup();
            group.AddProperty("RunCodeSweepAfterBuild", "true");

            project.ReevaluateIfNecessary();
            return project;
        }

        static internal string GetTargetsPath()
        {
            return CodeSweep.Utilities.EncodeProgramFilesVar(Path.GetDirectoryName(typeof(CodeSweep.BuildTask.ScannerTask).Module.FullyQualifiedName) + "\\CodeSweep.targets");
        }

        static internal void CopyTargetsFileToBinDir()
        {
            string binDir = Path.GetDirectoryName(typeof(CodeSweep.BuildTask.ScannerTask).Module.FullyQualifiedName);
            if (!File.Exists(binDir + "\\CodeSweep.targets"))
            {
                File.Copy(binDir + "\\..\\..\\..\\..\\BuildTask\\CodeSweep.targets", binDir + "\\CodeSweep.targets");
            }
        }

        static public ProjectTaskElement GetScannerTask(Project project)
        {
            ProjectTargetElement target = project.Xml.Targets.FirstOrDefault(element => element.Name == "AfterBuild");

            if (target != null)
            {
                foreach (ProjectTaskElement task in target.Tasks)
                {
                    if (task != null && task.Name == "ScannerTask")
                    {
                        return task;
                    }
                }
            }

            return null;
        }

        internal static MockIVsProject RegisterProjectWithMocks(Project project, IServiceProvider serviceProvider)
        {
            MockSolution solution = serviceProvider.GetService(typeof(SVsSolution)) as MockSolution;
            MockIVsProject vsProject = new MockIVsProject(project.FullPath);

            foreach (ProjectItemGroupElement itemGroup in project.Xml.ItemGroups)
            {
                foreach (ProjectItemElement item in itemGroup.Items)
                {
                    vsProject.AddItem(item.Include);
                }
            }

            solution.AddProject(vsProject);

            return vsProject;
        }

        public static void WaitForStop(CodeSweep.VSPackage.IBackgroundScanner_Accessor accessor)
        {
            while (accessor.IsRunning)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        public static void WaitForStop(CodeSweep.VSPackage.BackgroundScanner_Accessor accessor)
        {
            while (accessor.IsRunning)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        public static void RemoveCommandHandler(IServiceProvider serviceProvider, uint id)
        {
            OleMenuCommandService mcs = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                MenuCommand command = mcs.FindCommand(new CommandID(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, (int)id));
                if (command != null)
                {
                    mcs.RemoveCommand(command);
                }
            }
        }

        public static void RemoveCommandHandlers(IServiceProvider serviceProvider)
        {
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidIgnore);
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidConfig);
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidDoNotIgnore);
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidRepeatLastScan);
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidShowIgnoredInstances);
            RemoveCommandHandler(serviceProvider, CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidStopScan);
        }
    }
}
