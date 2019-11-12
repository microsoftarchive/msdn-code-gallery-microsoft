# Use  PerformanceCounter to get CPU Usage
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- WMI
- System Administration
- Windows Desktop App Development
## Topics
- CPU
- PerformanceCounter
## Updated
- 08/05/2013
## Description

<h1>Track the system CPU usage (CppCpuUsage)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This code sample demonstrates how to use the PerformanceCounter to track the CPU usage of the system or a certain process.<span style="">&nbsp;
</span>It lets the user visualize a Plot of one or more Performance Counter Value against time.</p>
<h2>Building the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open Solution in Visual Studio 2010.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Go to &quot;Build&quot; -&gt; &quot;Build Solution&quot;</p>
<h2>Running the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open Solution in Visual Studio 2012</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Go to &quot;Debug&quot; -&gt; &quot;Start without Debugging&quot;</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>From Drop Down, Select Performance Counter of Interest.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Click &quot;Add&quot;</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>See a graph of Performance Counter value against time.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">6.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>You may add more counters by Repeating Steps 3 and 4.</p>
<p class="MsoListParagraphCxSpLast"><span style=""><img src="93822-image.png" alt="" width="701" height="562" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">This sample code functions in following high-level steps</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>First List Down Valid Counter Names. For each processor a &quot;Processor Time&quot; and &quot;Idle Time&quot; performance counter is added. For each running process a &quot;Processor Time&quot; performance counter is added.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>A List of &quot;Selected&quot; Performance Counter is maintained. It is initialized to Empty Vector. When user selects a performance counter and clicks &quot;Add&quot;, that counter is added to the vector.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>A Thread runs in parallel. This Periodically, Queries each Performance Counter in the &quot;Selected&quot; list. The performance counters are plotted against time using GDI&#43;.</p>
<p class="MsoNormal">Following are the reusable components of the Sample Code</p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Get the Processor Count of System </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
DWORD GetProcessorCount()
{
    SYSTEM_INFO sysinfo; 
    DWORD dwNumberOfProcessors;


    GetSystemInfo(&sysinfo);


    dwNumberOfProcessors = sysinfo.dwNumberOfProcessors;


    return dwNumberOfProcessors;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Get list of running process </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
vector&lt;PCTSTR&gt; GetProcessNames()
{
    DWORD dwProcessID[SIZE];
    DWORD cbProcess;
    DWORD cProcessID;
    BOOL fResult = FALSE;
    DWORD index;


    HANDLE hProcess;
    HMODULE lphModule[SIZE];
    DWORD cbNeeded;    
    int len;


    vector&lt;PCTSTR&gt; vProcessNames;


    TCHAR * szProcessName;
    TCHAR * szProcessNameWithPrefix;


    fResult = EnumProcesses(dwProcessID, sizeof(dwProcessID), &cbProcess);


    if(!fResult)
    {
        goto cleanup;
    }


    cProcessID = cbProcess / sizeof(DWORD);


    for( index = 0; index &lt; cProcessID; index&#43;&#43; )
    {
        szProcessName = new TCHAR[MAX_PATH];        
        hProcess = OpenProcess( PROCESS_QUERY_INFORMATION |
        PROCESS_VM_READ,
        FALSE, dwProcessID[index] );
        if( NULL != hProcess )
        {
            if ( EnumProcessModulesEx( hProcess, lphModule, sizeof(lphModule), 
                &cbNeeded,LIST_MODULES_ALL) )
            {
                if( GetModuleBaseName( hProcess, lphModule[0], szProcessName, 
                    MAX_PATH ) )
                {
                    len = _tcslen(szProcessName);
                    _tcscpy(szProcessName&#43;len-4, TEXT(&quot;\0&quot;));
                    
                    bool fProcessExists = false;
                    int count = 0;
                    szProcessNameWithPrefix = new TCHAR[MAX_PATH];
                    _stprintf(szProcessNameWithPrefix, TEXT(&quot;%s&quot;), szProcessName);
                    do
                    {
                        if(count&gt;0)
                        {
                            _stprintf(szProcessNameWithPrefix,TEXT(&quot;%s#%d&quot;),szProcessName,count);
                        }
                        fProcessExists = false;
                        for(auto it = vProcessNames.begin(); it &lt; vProcessNames.end(); it&#43;&#43;)
                        {
                            if(_tcscmp(*it,szProcessNameWithPrefix)==0)
                            {
                                fProcessExists = true;
                                break;
                            }
                        }                    
                        count&#43;&#43;;
                    }
                    while(fProcessExists);
                    
                    vProcessNames.push_back(szProcessNameWithPrefix);
                }
            }
        }
    }


cleanup:
    szProcessName = NULL;
    szProcessNameWithPrefix = NULL;
    return vProcessNames;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Get list of valid<span style="">&nbsp; </span>
Performance Counter Names </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
vector&lt;PCTSTR&gt; GetValidCounterNames()
{
    vector&lt;PCTSTR&gt; validCounterNames;
    DWORD dwNumberOfProcessors = GetProcessorCount();
    DWORD index;
    vector&lt;PCTSTR&gt; vszProcessNames;
    TCHAR * szCounterName;


    validCounterNames.push_back(TEXT(&quot;\\Processor(_Total)\\% Processor Time&quot;));
    validCounterNames.push_back(TEXT(&quot;\\Processor(_Total)\\% Idle Time&quot;));


    for( index = 0; index &lt; dwNumberOfProcessors; index&#43;&#43; )
    {
        szCounterName = new TCHAR[MAX_PATH];
        _stprintf(szCounterName, TEXT(&quot;\\Processor(%u)\\%% Processor Time&quot;),index);
        validCounterNames.push_back(szCounterName);
        szCounterName = new TCHAR[MAX_PATH];
        _stprintf(szCounterName, TEXT(&quot;\\Processor(%u)\\%% Idle Time&quot;),index);
        validCounterNames.push_back(szCounterName);
    }


    vszProcessNames = GetProcessNames();


    for(auto element = vszProcessNames.begin(); 
        element &lt; vszProcessNames.end(); 
        element&#43;&#43; )
    {
        szCounterName = new TCHAR[MAX_PATH];
        _stprintf(szCounterName, TEXT(&quot;\\Process(%s)\\%% Processor Time&quot;),*element);
        validCounterNames.push_back(szCounterName);
    }    
    
cleanup:
    szCounterName = NULL;
    return validCounterNames;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>class Query: For querying Performance Counters</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:1.0in; text-indent:-.25in">
<span style=""><span style="">a)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Adds a Performance Counter to Log.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void Query::AddCounterInfo(PCWSTR name)
{
    if(fIsWorking)
    {
        PDH_STATUS status;
        CounterInfo ci;
        ci.counterName = name;
        status = PdhAddCounter(query, ci.counterName, 0 , &ci.counter);


        if(status != ERROR_SUCCESS)
        {
            return;
        }


        vciSelectedCounters.push_back(ci);
    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="">&nbsp;&nbsp; </span>b) Query once for each Selected Performance Counter.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void Query::Record()
{
    PDH_STATUS status;
    ULONG CounterType;
    ULONG WaitResult;
    PDH_FMT_COUNTERVALUE DisplayValue;    


    status = PdhCollectQueryData(query);


    if(status != ERROR_SUCCESS)
    {
        return;
    }


    status = PdhCollectQueryDataEx(query, SAMPLE_INTERVAL, Event);


    if(status != ERROR_SUCCESS)
    {
        return;
    }


    WaitResult = WaitForSingleObject(Event, INFINITE);


    if (WaitResult == WAIT_OBJECT_0) 
    {
        for(auto it = vciSelectedCounters.begin(); it &lt; vciSelectedCounters.end(); it&#43;&#43;)
        {
            status = PdhGetFormattedCounterValue(it-&gt;counter, PDH_FMT_DOUBLE, &CounterType, &DisplayValue);            


            if(status != ERROR_SUCCESS)
            {
                continue;
            }


            Log log;
            log.time = time;
            log.value = DisplayValue.doubleValue;
            it-&gt;logs.push_back(log);                
        }
    }


    time&#43;&#43;;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
