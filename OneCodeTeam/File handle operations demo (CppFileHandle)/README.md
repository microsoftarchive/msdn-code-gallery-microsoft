# File handle operations demo (CppFileHandle)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- File System
## Updated
- 03/01/2012
## Description

<h1><span style="font-family:������">CONSOLE APPLICATION</span> (<span style="font-family:������">CppFileHandle</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">CppFileHandle demonstrates two typical scenarios of using file handles:
</p>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Enumerate file handles of a process<span style="">.</span> </p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Get file name from a file handle.<span style="">&nbsp; </span>
</p>
<h2><span style="">Running the sample </span></h2>
<p class="MsoNormal"><span style=""><img src="53151-image.png" alt="" width="576" height="330" align="middle">
</span><span style="">&nbsp;</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><b style="">A. Enumerate file handles of a process </b></p>
<p class="MsoNormal">It requires using some undocumented APIs to enumerate file handles of a process. Because these APIs and structs are internal to the operating system and subject to change from one release of Windows to another, to maintain the compatibility
 of your application, it is better not to use them. </p>
<p class="MsoNormal">1. Prepare for NtQuerySystemInformation and NtQueryInformationFile.The functions have no associated import library. You must use the LoadLibrary and GetProcAddress functions to dynamically link to ntdll.dll.<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
    HINSTANCE hNtDll = LoadLibrary(_T(&quot;ntdll.dll&quot;));
    assert(hNtDll != NULL);


    PFN_NTQUERYSYSTEMINFORMATION NtQuerySystemInformation = 
        (PFN_NTQUERYSYSTEMINFORMATION)GetProcAddress(hNtDll, 
        &quot;NtQuerySystemInformation&quot;);
    assert(NtQuerySystemInformation != NULL);


    PFN_NTQUERYINFORMATIONFILE NtQueryInformationFile = 
        (PFN_NTQUERYINFORMATIONFILE)GetProcAddress(hNtDll, 
        &quot;NtQueryInformationFile&quot;);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">2. Get system handle information. (NtQuerySystemInformation, SystemHandleInformation, SYSTEM_HANDLE_INFORMATION)NtQuerySystemInformation does not return the correct required buffer size if the buffer passed is too small. Instead you must
 call the function while increasing the buffer size until the function no longer returns STATUS_INFO_LENGTH_MISMATCH.<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
    DWORD nSize = 4096, nReturn;
    PSYSTEM_HANDLE_INFORMATION pSysHandleInfo = (PSYSTEM_HANDLE_INFORMATION)
        HeapAlloc(GetProcessHeap(), 0, nSize);


    // NtQuerySystemInformation does not return the correct required buffer 
    // size if the buffer passed is too small. Instead you must call the 
    // function while increasing the buffer size until the function no longer 
    // returns STATUS_INFO_LENGTH_MISMATCH.
    while (NtQuerySystemInformation(SystemHandleInformation, pSysHandleInfo, 
        nSize, &nReturn) == STATUS_INFO_LENGTH_MISMATCH)
    {
        HeapFree(GetProcessHeap(), 0, pSysHandleInfo);
        nSize &#43;= 4096;
        pSysHandleInfo = (SYSTEM_HANDLE_INFORMATION*)HeapAlloc(
            GetProcessHeap(), 0, nSize);
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">3. Enumerate file handles of the process. (SYSTEM_HANDLE_INFORMATION, HANDLE_TYPE_FILE, DuplicateHandle, NtQueryInformationFile)<span style="">
</span>Because handle is meaningful only to its hosting process, we need to duplicate other process's handle to the current process (DuplicateHandle) so as to further query the file information of the file object. NtQueryInformationFile is used to retrieve
 file name information.<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
    DWORD dwFiles = 0;
    
    // Get the handle of the target process. The handle will be used to 
    // duplicate the file handles in the process.
    HANDLE hProcess = OpenProcess(
        PROCESS_DUP_HANDLE | PROCESS_QUERY_INFORMATION, FALSE, pid);
    if (hProcess == NULL)
    {
        _tprintf(_T(&quot;OpenProcess failed w/err 0x%08lx\n&quot;), GetLastError());
        return -1;
    }


    for (ULONG i = 0; i &lt; pSysHandleInfo-&gt;NumberOfHandles; i&#43;&#43;)
    {
        PSYSTEM_HANDLE pHandle = &(pSysHandleInfo-&gt;Handles[i]);


        // Check for file handles of the specified process
        if (pHandle-&gt;ProcessId == pid && 
            pHandle-&gt;ObjectTypeNumber == HANDLE_TYPE_FILE)
        {
            dwFiles&#43;&#43;;    // Increase the number of file handles


            // Duplicate the handle in the current process
            HANDLE hCopy;
            if (!DuplicateHandle(hProcess, (HANDLE)pHandle-&gt;Handle, 
                GetCurrentProcess(), &hCopy, MAXIMUM_ALLOWED, FALSE, 0))
                continue;


            // Retrieve file name information about the file object.
            IO_STATUS_BLOCK ioStatus;
            PFILE_NAME_INFORMATION pNameInfo = (PFILE_NAME_INFORMATION)
                malloc(MAX_PATH * 2 * 2);
            DWORD dwInfoSize = MAX_PATH * 2 * 2;


            if (NtQueryInformationFile(hCopy, &ioStatus, pNameInfo, 
                dwInfoSize, FileNameInformation) == STATUS_SUCCESS)
            {
                // Get the file name and print it
                WCHAR wszFileName[MAX_PATH &#43; 1];
                StringCchCopyNW(wszFileName, MAX_PATH &#43; 1, 
                    pNameInfo-&gt;FileName, /*must be WCHAR*/
                    pNameInfo-&gt;FileNameLength /*in bytes*/ / 2);


                wprintf(L&quot;0x%x:\t%s\n&quot;, pHandle-&gt;Handle, wszFileName);
            }
            free(pNameInfo);


            CloseHandle(hCopy);
        }
    }


    CloseHandle(hProcess);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><b style="">B. Get file name from file handle </b></p>
<p class="MsoNormal">1. Create a file mapping object (CreateFileMapping, MapViewOfFile).<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
    hFileMap = CreateFileMapping(hFile, NULL, PAGE_READONLY, 0, 1, NULL);
    if (!hFileMap)
    {
        _tprintf(_T(&quot;CreateFileMapping failed w/err 0x%08lx\n&quot;), 
            GetLastError());
        return FALSE;
    }


    void* pMem = MapViewOfFile(hFileMap, FILE_MAP_READ, 0, 0, 1);
    if (!pMem)
    {
        _tprintf(_T(&quot;MapViewOfFile failed w/err 0x%08lx\n&quot;), GetLastError());
        CloseHandle(hFileMap);
        return FALSE;
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">2. Call the GetMappedFileName function to obtain the file name. File name returned by GetMappedFileName contains device file name like:
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
    if (GetMappedFileName(GetCurrentProcess(), pMem, szFileName, MAX_PATH))
    {
        // szFileName contains device file name like:
        // \Device\HarddiskVolume2\Users\JLG\AppData\Local\Temp\HLe6098.tmp
        _tprintf(_T(&quot;Device name is %s\n&quot;), szFileName);


        // Translate path with device name to drive letters.
        TCHAR szTemp[BUFFER_SIZE];
        szTemp[0] = '\0';


        // Get a series of null-terminated strings, one for each valid drive 
        // in the system, plus with an additional null character. Each string 
        // is a drive name. e.g. C:\\0D:\\0\0
        if (GetLogicalDriveStrings(BUFFER_SIZE - 1, szTemp)) 
        {
            TCHAR szName[MAX_PATH];
            TCHAR szDrive[3] = _T(&quot; :&quot;);
            BOOL bFound = FALSE;
            TCHAR* p = szTemp;


            do
            {
                // Copy the drive letter to the template string
                *szDrive = *p;


                // Look up each device name. For example, given szDrive is C:, 
                // the output szName may be \Device\HarddiskVolume2.
                if (QueryDosDevice(szDrive, szName, MAX_PATH))
                {
                    UINT uNameLen = _tcslen(szName);


                    if (uNameLen &lt; MAX_PATH)
                    {
                        // Match the device name e.g. \Device\HarddiskVolume2
                        bFound = _tcsnicmp(szFileName, szName, uNameLen) == 0;


                        if (bFound)
                        {
                            // Reconstruct szFileName using szTempFile
                            // Replace device path with DOS path
                            TCHAR szTempFile[MAX_PATH];
                            StringCchPrintf(szTempFile, MAX_PATH, _T(&quot;%s%s&quot;), 
                                szDrive, szFileName &#43; uNameLen);
                            StringCchCopyN(szFileName, MAX_PATH &#43; 1, 
                                szTempFile, _tcslen(szTempFile));
                        }
                    }
                }


                // Go to the next NULL character, i.e. the next drive name.
                while (*p&#43;&#43;);


            } while (!bFound && *p); // End of string
        }
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">To translate path with device name to drive letters, we need to enumerate logical drive letters (GetLogicalDriveStrings), and get the device names corresponding to the driver letters, then compare the device names with the name returned
 by GetMappedFileName. If the match is found, replace the device name in the file path with drive letter.
</p>
<h2>More Information<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/aa366789.aspx">MSDN: Obtaining a File Name From a File Handle</a><span style="">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
