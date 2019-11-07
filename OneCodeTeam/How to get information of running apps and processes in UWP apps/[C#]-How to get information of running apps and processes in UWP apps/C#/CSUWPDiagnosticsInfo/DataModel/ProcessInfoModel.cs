using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Diagnostics;

namespace CSUWPDiagnosticsInfo.DataModel
{
    public class ProcessInfoModel
    {
        public string ExeName { get; set; }
        public string CpuUsageTime { get; set; }
        public long DiskBytesCount { get; set; }
        public ulong PageFileSize { get; set; }
        public ulong WorkingSetSize { get; set; }
        public uint ProcessId { get; set; }
        public ProcessInfoModel(ProcessDiagnosticInfo process)
        {
            ProcessCpuUsageReport cpuReport = process.CpuUsage.GetReport();
            if (cpuReport != null)
            {
                TimeSpan cpuUsageTime = cpuReport.KernelTime + cpuReport.UserTime;
                CpuUsageTime = string.Format("{0:hh\\:mm\\:ss}", cpuUsageTime);
            }
            ProcessDiskUsageReport diskReport = process.DiskUsage.GetReport();
            if (diskReport != null)
            {
                DiskBytesCount = diskReport.BytesReadCount + diskReport.BytesWrittenCount;
            }
            ProcessMemoryUsageReport memoryReport = process.MemoryUsage.GetReport();
            if (memoryReport != null)
            {
                PageFileSize = memoryReport.PageFileSizeInBytes;
                WorkingSetSize = memoryReport.WorkingSetSizeInBytes;
            }
            ProcessId = process.ProcessId;
            ExeName = process.ExecutableFileName;            
        }
    }
}
