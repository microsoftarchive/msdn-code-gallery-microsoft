# How to ignore Self Signed Certificate (SSL) errors in universal Windows apps
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- C#
- XAML
- C++
- Visual Studio 2013
## Topics
- ssl
- universal app
## Updated
- 10/22/2015
## Description

<h2>How to ignore Self Signed Certificate errors in universal Windows apps</h2>
<h2>Introduction</h2>
<p>There are some very limited times when we need to ignore Server Certificate errors. For instance, we have endpoint internal to our network and the certificate is self-signed certificate and for some reasons, we can&rsquo;t deploy the certificate to all the
 necessary clients and install them as a trusted root.</p>
<h2>Running the Sample</h2>
<p>You must run this code sample on Visual Studio 2013 or newer versions on Windows 8.1 or newer versions.</p>
<p>After you successfully build and run the sample project in Visual Studio 2013, the screen below will show up:</p>
<p><img id="143977" src="143977-8789.png" alt="" width="734" height="416"></p>
<p>Enter the URI which needs to be tested. We can create a sample site with self-signed certificated and test. After you click the &lsquo;Test URI&rsquo; button, certificate errors will be displayed.</p>
<p>&nbsp;</p>
<h2>Using the Code</h2>
<p><span style="font-size:10px">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>
<pre class="hidden">&lt;StackPanel Margin=&quot;10,0,0,0&quot;&gt;

                    &lt;TextBox Header=&quot;Enter URl to test certificate errors:&quot; x:Name=&quot;txtURI&quot; HorizontalAlignment=&quot;Left&quot; TextWrapping=&quot;Wrap&quot; Text=&quot;&quot; Width=&quot;500&quot;/&gt;

                    &lt;Button Content=&quot;Test URI&quot; HorizontalAlignment=&quot;Left&quot; Click=&quot;Button_Click&quot;/&gt;

                    &lt;TextBlock Margin=&quot;10,0,0,0&quot; Style=&quot;{StaticResource BodyTextBlockStyle}&quot; HorizontalAlignment=&quot;Left&quot; TextWrapping=&quot;Wrap&quot; Text=&quot;&quot; Name=&quot;txtResult&quot;/&gt;

                &lt;/StackPanel&gt;        </pre>
<div class="preview">
<pre class="xaml"><span class="xaml__tag_start">&lt;StackPanel</span>&nbsp;<span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;10,0,0,0&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;TextBox</span>&nbsp;<span class="xaml__attr_name">Header</span>=<span class="xaml__attr_value">&quot;Enter&nbsp;URl&nbsp;to&nbsp;test&nbsp;certificate&nbsp;errors:&quot;</span>&nbsp;x:<span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;txtURI&quot;</span>&nbsp;<span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Left&quot;</span>&nbsp;<span class="xaml__attr_name">TextWrapping</span>=<span class="xaml__attr_value">&quot;Wrap&quot;</span>&nbsp;<span class="xaml__attr_name">Text</span>=<span class="xaml__attr_value">&quot;&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;500&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;Button</span>&nbsp;<span class="xaml__attr_name">Content</span>=<span class="xaml__attr_value">&quot;Test&nbsp;URI&quot;</span>&nbsp;<span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Left&quot;</span>&nbsp;<span class="xaml__attr_name">Click</span>=<span class="xaml__attr_value">&quot;Button_Click&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;TextBlock</span>&nbsp;<span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;10,0,0,0&quot;</span>&nbsp;<span class="xaml__attr_name">Style</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;BodyTextBlockStyle}&quot;</span>&nbsp;<span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Left&quot;</span>&nbsp;<span class="xaml__attr_name">TextWrapping</span>=<span class="xaml__attr_value">&quot;Wrap&quot;</span>&nbsp;<span class="xaml__attr_name">Text</span>=<span class="xaml__attr_value">&quot;&quot;</span>&nbsp;<span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;txtResult&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_end">&lt;/StackPanel&gt;</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</pre>
</div>
</div>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private async Task&lt;string&gt; TestCertificate(Uri theUri, string theExpectedIssuer)

{

    // Simple GET for URI passed in

    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, theUri);

    // Retry for cert error issues

    bool retryIgnoreCertErrors = false;

    // return value

    string retVal = &quot;trying to GET&quot;;

    // Base  filter 

    HttpBaseProtocolFilter httpBaseProtocolFilter = null;

 

    try

    {

        HttpResponseMessage response = await m_httpClient.SendRequestAsync(request);

        // hit here if no exceptions!

        retVal = &quot;No Cert errors&quot;;

    }

    catch (Exception ex)

    {

        retVal = ex.Message;

 

        // Mask the HResult and if this is error code 12045 which means there was a certificate error

        if ((ex.HResult &amp; 65535) == 12045)

        {

            // Get a list of the server cert errors

            IReadOnlyList&lt;ChainValidationResult&gt; errors = request.TransportInformation.ServerCertificateErrors;

 

            // I expect that the cert is expired and it is untrusted for my scenario...

            if ((errors != null) &amp;&amp; (errors.Contains(ChainValidationResult.Expired)

                   &amp;&amp; errors.Contains(ChainValidationResult.Untrusted)))

            {

                // Specifically validate that this came from a particular Issuer

                if (request.TransportInformation.ServerCertificate.Issuer == theExpectedIssuer)

                {

                    // Create a Base Protocol Filter to add certificate errors I want to ignore...

                    httpBaseProtocolFilter = new HttpBaseProtocolFilter();

                    // I purposefully have an expired cert to show setting multiple Ignorable Errors

                    httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);

                    // Untrused because this is a self signed cert that is not installed

                    httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);

                    // OK to retry since I expected these errors from this host!

                    retryIgnoreCertErrors = true;

                }

            }

        }

    }

 

    try

    {

        // Retry with a temporary HttpClient and ignore some very specific errors!

        if (retryIgnoreCertErrors)

        {

            // Create a Client to use just for this request and ignore some cert errors.

            HttpClient aTempClient = new HttpClient(httpBaseProtocolFilter);

            // Try to execute the request (should not fail now for those two errors)

            HttpRequestMessage aTempReq = new HttpRequestMessage(HttpMethod.Get, theUri);

            HttpResponseMessage aResp2 = await aTempClient.SendRequestAsync(aTempReq);

            retVal = &quot;No Cert errors&quot;;

        }

    }

    catch (Exception ex2)

    {

        // some other exception occurred

        retVal = ex2.Message;

    }

    return retVal;

}

 

private async void Button_Click(object sender, RoutedEventArgs e)

{

    Uri targetUri = new Uri(txtURI.Text);

 

    txtResult.Text = await TestCertificate(targetUri, targetUri.Host);

 

}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;async&nbsp;Task&lt;<span class="cs__keyword">string</span>&gt;&nbsp;TestCertificate(Uri&nbsp;theUri,&nbsp;<span class="cs__keyword">string</span>&nbsp;theExpectedIssuer)&nbsp;
&nbsp;
{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Simple&nbsp;GET&nbsp;for&nbsp;URI&nbsp;passed&nbsp;in</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpRequestMessage&nbsp;request&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpRequestMessage(HttpMethod.Get,&nbsp;theUri);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Retry&nbsp;for&nbsp;cert&nbsp;error&nbsp;issues</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">bool</span>&nbsp;retryIgnoreCertErrors&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;return&nbsp;value</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;retVal&nbsp;=&nbsp;<span class="cs__string">&quot;trying&nbsp;to&nbsp;GET&quot;</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Base&nbsp;&nbsp;filter&nbsp;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpBaseProtocolFilter&nbsp;httpBaseProtocolFilter&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpResponseMessage&nbsp;response&nbsp;=&nbsp;await&nbsp;m_httpClient.SendRequestAsync(request);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;hit&nbsp;here&nbsp;if&nbsp;no&nbsp;exceptions!</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;retVal&nbsp;=&nbsp;<span class="cs__string">&quot;No&nbsp;Cert&nbsp;errors&quot;</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;retVal&nbsp;=&nbsp;ex.Message;&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Mask&nbsp;the&nbsp;HResult&nbsp;and&nbsp;if&nbsp;this&nbsp;is&nbsp;error&nbsp;code&nbsp;12045&nbsp;which&nbsp;means&nbsp;there&nbsp;was&nbsp;a&nbsp;certificate&nbsp;error</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;((ex.HResult&nbsp;&amp;&nbsp;<span class="cs__number">65535</span>)&nbsp;==&nbsp;<span class="cs__number">12045</span>)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Get&nbsp;a&nbsp;list&nbsp;of&nbsp;the&nbsp;server&nbsp;cert&nbsp;errors</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IReadOnlyList&lt;ChainValidationResult&gt;&nbsp;errors&nbsp;=&nbsp;request.TransportInformation.ServerCertificateErrors;&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;I&nbsp;expect&nbsp;that&nbsp;the&nbsp;cert&nbsp;is&nbsp;expired&nbsp;and&nbsp;it&nbsp;is&nbsp;untrusted&nbsp;for&nbsp;my&nbsp;scenario...</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;((errors&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;&amp;&amp;&nbsp;(errors.Contains(ChainValidationResult.Expired)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&amp;&amp;&nbsp;errors.Contains(ChainValidationResult.Untrusted)))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Specifically&nbsp;validate&nbsp;that&nbsp;this&nbsp;came&nbsp;from&nbsp;a&nbsp;particular&nbsp;Issuer</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(request.TransportInformation.ServerCertificate.Issuer&nbsp;==&nbsp;theExpectedIssuer)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Create&nbsp;a&nbsp;Base&nbsp;Protocol&nbsp;Filter&nbsp;to&nbsp;add&nbsp;certificate&nbsp;errors&nbsp;I&nbsp;want&nbsp;to&nbsp;ignore...</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpBaseProtocolFilter&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpBaseProtocolFilter();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;I&nbsp;purposefully&nbsp;have&nbsp;an&nbsp;expired&nbsp;cert&nbsp;to&nbsp;show&nbsp;setting&nbsp;multiple&nbsp;Ignorable&nbsp;Errors</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Untrused&nbsp;because&nbsp;this&nbsp;is&nbsp;a&nbsp;self&nbsp;signed&nbsp;cert&nbsp;that&nbsp;is&nbsp;not&nbsp;installed</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;OK&nbsp;to&nbsp;retry&nbsp;since&nbsp;I&nbsp;expected&nbsp;these&nbsp;errors&nbsp;from&nbsp;this&nbsp;host!</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;retryIgnoreCertErrors&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Retry&nbsp;with&nbsp;a&nbsp;temporary&nbsp;HttpClient&nbsp;and&nbsp;ignore&nbsp;some&nbsp;very&nbsp;specific&nbsp;errors!</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(retryIgnoreCertErrors)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Create&nbsp;a&nbsp;Client&nbsp;to&nbsp;use&nbsp;just&nbsp;for&nbsp;this&nbsp;request&nbsp;and&nbsp;ignore&nbsp;some&nbsp;cert&nbsp;errors.</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpClient&nbsp;aTempClient&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpClient(httpBaseProtocolFilter);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Try&nbsp;to&nbsp;execute&nbsp;the&nbsp;request&nbsp;(should&nbsp;not&nbsp;fail&nbsp;now&nbsp;for&nbsp;those&nbsp;two&nbsp;errors)</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpRequestMessage&nbsp;aTempReq&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpRequestMessage(HttpMethod.Get,&nbsp;theUri);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpResponseMessage&nbsp;aResp2&nbsp;=&nbsp;await&nbsp;aTempClient.SendRequestAsync(aTempReq);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;retVal&nbsp;=&nbsp;<span class="cs__string">&quot;No&nbsp;Cert&nbsp;errors&quot;</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex2)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;some&nbsp;other&nbsp;exception&nbsp;occurred</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;retVal&nbsp;=&nbsp;ex2.Message;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;retVal;&nbsp;
&nbsp;
}&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
<span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;Button_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;RoutedEventArgs&nbsp;e)&nbsp;
&nbsp;
{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Uri&nbsp;targetUri&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Uri(txtURI.Text);&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;txtResult.Text&nbsp;=&nbsp;await&nbsp;TestCertificate(targetUri,&nbsp;targetUri.Host);&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
}</pre>
</div>
</div>
</div>
<h2>More Information</h2>
<p><a href="https://crypto.stanford.edu/~dabo/pubs/abstracts/ssl-client-bugs.html" target="_blank">The most dangerous code in the world: validating SSL certificates in non-browser software</a></p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.web.http.httpclient.aspx" target="_blank">HttpClient</a></p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.web.http.filters.httpbaseprotocolfilter.aspx" target="_blank">HttpBaseProtocolFilter</a></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
