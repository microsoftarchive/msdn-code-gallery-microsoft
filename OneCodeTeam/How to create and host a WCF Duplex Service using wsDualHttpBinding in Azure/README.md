# How to create and host a WCF Duplex Service using wsDualHttpBinding in Azure
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Azure
- .NET
- .NET Framework
- Worker Role
- .NET Framework 4.5
- Cloud
- Azure Cloud Services
## Topics
- Worker Role
- Cloud
- Azure Cloud Service
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="color:#000000">How to create and host a WCF Duplex Service using wsDualHttpBinding</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span style="color:#000000">&nbsp;</span><span style="font-size:11pt">This sample shows h</span><span style="color:#000000">ow to create and host a WCF Duplex Service using wsDualHttpBinding</span><span style="color:#000000">.</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:12pt"><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="color:#000000; font-size:11pt">WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:12pt"><span style="color:#000000; font-size:11pt">&nbsp;</span><span style="color:#000000; font-size:11pt">These patterns provide the ability for a service to communicate back to the client via a callback.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step 1: Run VS2013 as administrator, then open the solution.&nbsp;
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step 2: Press Ctrl&#43;F5 start the work role on compute emulator.&nbsp;</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step </span><span>3</span><span>: Right click Client project, choose debug-&gt;start new instance.&nbsp;
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step </span><span>4</span><span>: This is the screenshot when the solution is ran successfully.&nbsp;
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span><img src="144063-image.png" alt="" width="574" height="285" align="middle">
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:12pt"><span style="font-size:11pt">Step 1:&nbsp; Create the WCF service class library.</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:12pt"><span style="font-size:11pt">&nbsp; </span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<pre class="csharp" id="codePreview">[ServiceContract(CallbackContract = typeof(IServiceCalulateCallBack), SessionMode = SessionMode.Required)]
   public interface IServiceCalculate
   {
       /// &lt;summary&gt;
       /// Get a value dbeOneParameter plus dbeTwoParameter when the mehtod finishes waiting for an underlying response message.
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;dbeOneParameter&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;dbeTwoParameter&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;strSymbol&quot;&gt;&lt;/param&gt;
       [OperationContract]
       void SumByInputValue(double dbeOneParameter, double dbeTwoParameter, string strSymbol);
       /// &lt;summary&gt;
       /// Get a value dbeOneParameter plus dbeTwoParameter without the method finishing waiting for an underlying response message.
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;dbeOneParameter&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;dbeTwoParameter&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;strSymbol&quot;&gt;&lt;/param&gt;
       [OperationContract(IsOneWay = true)]
       void SumByInputValueOneway(double dbeOneParameter, double dbeTwoParameter, string strSymbol);
   }
   public interface IServiceCalulateCallBack
   {
       /// &lt;summary&gt;
       /// The value will be displayed according to the format value chosen when the method finishes waiting for an underlying response message.
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;strSymbol&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;dbeSumValue&quot;&gt;&lt;/param&gt;
       [OperationContract]
       void DisplayResultByOption(string strSymbol, double dbeSumValue);
       
        /// &lt;summary&gt;
        /// The value will be displayed according to the format value chosen without the method finishing waiting for an underlying response message. 
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;strSymbol&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;dbeSumValue&quot;&gt;&lt;/param&gt;
       [OperationContract(IsOneWay=true)]
       void DisPlayResultByOptionOneWay(string strSymbol, double dbeSumValue);
   }
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step2:&nbsp; Host the WCF on the work role.&nbsp;
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>The following code shows how to run the service on work role.&nbsp;
</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<pre class="csharp" id="codePreview">public override void Run()
     {
         Trace.TraceInformation(&quot;HostWCFService is running&quot;);
         Trace.TraceInformation(&quot; Try to start hosting WCF service...&quot;);
         this.serviceHost = new ServiceHost(typeof(ServiceCalculate.ServiceCalculate));
         try
         {
              
             this.serviceHost.Open();
             Trace.TraceInformation(&quot;WCF service hosting started successfully.&quot;);
         }
         catch (TimeoutException timeoutException)
         {
             Trace.TraceError(&quot;The service operation timed out. {0}&quot;,
                              timeoutException.Message);
         }
         catch (CommunicationException communicationException)
         {
             Trace.TraceError(&quot;Could not start WCF service host. {0}&quot;,
                              communicationException.Message);
         }
         try
         {
             this.RunAsync(this.cancellationTokenSource.Token).Wait();
         }
         finally
         {
             this.runCompleteEvent.Set();
         }
     }
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step3:&nbsp; The following code shows the config file
</span><span>on the</span><span> work role.&nbsp;</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>

<pre class="xml" id="codePreview">&lt;system.serviceModel&gt;
    &lt;services&gt;
      &lt;service name=&quot;ServiceCalculate.ServiceCalculate&quot; behaviorConfiguration=&quot;CalculatorServiceBehavior&quot; &gt;
        &lt;host&gt;
          &lt;baseAddresses&gt;
            &lt;add baseAddress=&quot;http://localhost/CalculatorService&quot;/&gt;
          &lt;/baseAddresses&gt;
        &lt;/host&gt;
        &lt;endpoint address=&quot;&quot; binding=&quot;wsDualHttpBinding&quot; contract=&quot;ServiceCalculate.IServiceCalculate&quot;&gt; &lt;/endpoint&gt;
      &lt;/service&gt;
    &lt;/services&gt;
    &lt;behaviors&gt;
      &lt;serviceBehaviors &gt;
        &lt;behavior name=&quot;CalculatorServiceBehavior&quot;&gt;
          &lt;serviceMetadata httpGetEnabled=&quot;true&quot;/&gt;
        &lt;/behavior&gt;
      &lt;/serviceBehaviors&gt;
    &lt;/behaviors&gt;
  &lt;/system.serviceModel&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>Step3:&nbsp; Call WCF Service on the client project.</span></span></p>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-size:11pt"><span>&nbsp; </span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<pre class="csharp" id="codePreview">static void Main(string[] args)
      {
          InstanceContext instance = new InstanceContext(new Program());
          CalculatorServiceClient.ServiceCalculateClient client = new CalculatorServiceClient.ServiceCalculateClient(instance);
          try
          {
              WSDualHttpBinding binding = client.Endpoint.Binding as WSDualHttpBinding;
              binding.ClientBaseAddress = new Uri(&quot;http://localhost:8081/client&quot;);
              double dbeOneParameter = 0;
              double dbeTwoParameter = 0;
              Console.WriteLine(&quot;input the first parameter.&quot;);
              inputParameter(out dbeOneParameter);
              Console.WriteLine(&quot;input the second parameter.&quot;);
              inputParameter(out dbeTwoParameter);
              Console.WriteLine();
              Console.ForegroundColor = ConsoleColor.DarkGreen;
              Console.WriteLine(&quot;Starting to call WCF Service method SumByInputValue.&quot;);
              client.SumByInputValue(dbeOneParameter, dbeTwoParameter, strSymbol);
              Console.WriteLine(&quot;Calling WCF Service method SumByInputValue finished.&quot;);
              Console.WriteLine();
              Console.ResetColor();
              Console.ForegroundColor = ConsoleColor.DarkRed;
              Console.WriteLine(&quot;Starting to call WCF Service method SumByInputValueOneway.&quot;);
              client.SumByInputValueOneway(dbeOneParameter, dbeTwoParameter, strSymbol);
              Console.WriteLine(&quot;Calling WCF Service method SumByInputValueOneway finished.&quot;);
          }
          catch (Exception ex)
          {
          }
          Console.ReadLine();
          Console.ResetColor();
      }
   
      public void DisplayResultByOption(string strSymbol, double dbeSumValue)
      {
          Console.Write(&quot;Call WCFCallback method DisplayResultByOption: &quot;);
          Console.WriteLine(string.Format(strSymbol, dbeSumValue)&#43;&quot;.&quot;);
      }
      public void DisPlayResultByOptionOneWay(string strSymbol, double dbeSumValue)
      {
          Console.Write(&quot;Call WCFCallback method DisPlayResultByOptionOneWay:&quot;);
          Console.WriteLine(string.Format(strSymbol, dbeSumValue) &#43; &quot;.&quot;);
      }
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="https://msdn.microsoft.com/en-us/library/ms731354(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://msdn.microsoft.com/en-us/library/ms731354(v=vs.110).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="https://msdn.microsoft.com/en-us/library/ms731360(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://msdn.microsoft.com/en-us/library/ms731360(v=vs.110).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="https://msdn.microsoft.com/en-us/library/ms731298(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://msdn.microsoft.com/en-us/library/ms731298(v=vs.110).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
