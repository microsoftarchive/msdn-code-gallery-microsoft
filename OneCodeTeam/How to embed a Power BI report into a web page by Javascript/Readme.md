# How to embed a Power BI report into a web page by Javascript
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- HTML
- power bi
- Languages
- Power BI Embedded
## Topics
- jQuery
- Javascript
- HTML
- power bi
- Power BI Embedded
## Updated
- 03/30/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">&nbsp;</span><a name="OLE_LINK1"></a><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">How
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">to
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">embed a Power BI</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt"> report into a web page by Javas</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">cript</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This sample demonstrates how </span><span>to </span><span>embed a Power BI report into a web page by Java</span><span>s</span><span>cript</span><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Power BI Embedded is one of the Azure services. Only the ISV who uses Azure Portal is charged for usage fees (per hourly user session), and the user who views the report isn't charged or even require an Azure subscription.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt">Before you run this sample, you should know your
</span><span style="font-weight:bold; font-size:10pt">Workspace Collection</span><span style="font-size:10pt"> name and Access Key. And in your
</span><span style="font-weight:bold; font-size:10pt">Workspace Collection</span><span style="font-size:10pt">, ther</span><span style="font-size:10pt">e</span><span style="font-size:10pt"> should be at least one workspace, and one report dataset.</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt">If you don&rsquo;t have a Power BI Embedded service, you can follow this link
</span><a href="https://docs.microsoft.com/en-us/azure/power-bi-embedded/power-bi-embedded-get-started-sample" style="text-decoration:none"><span style="color:#0563c1; font-size:10pt; text-decoration:underline">https://docs.microsoft.com/en-us/azure/power-b</span><span style="color:#0563c1; font-size:10pt; text-decoration:underline">i</span><span style="color:#0563c1; font-size:10pt; text-decoration:underline">-embedded/power-bi-embedded-get-started-sample</span></a><span style="font-size:10pt">
</span><span style="font-size:10pt">to create workspaces and import re</span><span style="font-size:10pt">port to it.</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><strong><span style="font-size:12pt"><span style="font-size:10pt">Important:
</span></span></strong><span style="font-size:12pt"><span style="font-size:10pt">If you're using Azure China service, please follow this
</span></span><span style="font-size:12pt"><span style="font-size:10pt"><a href="https://www.azure.cn/documentation/articles/power-bi-embedded-get-started-sample/">document&nbsp;for China</a>.&nbsp;</span></span><span style="font-size:10pt">And before running
 the sample, please replace </span><span style="font-size:10pt">the request url&nbsp;</span><span style="font-size:10pt">&quot;</span><span style="font-size:10pt">https://api.powerbi.com</span><span style="font-size:10pt">&quot; to &quot;</span><span style="font-size:10pt">https://api.powerbi.cn</span><span style="font-size:10pt">&quot;
 in the html.</span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt"><br>
</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt"><span style="font-size:10pt"><img src="165537-image.png" alt="" width="643" height="301" align="middle">
</span></span></p>
<p style="font-size:10.0pt; direction:ltr; unicode-bidi:normal; margin:0pt"><span style="font-size:12pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>T</span><span>his</span><span> is a static html page. Download the source and find the index.html. Open it in your Web B</span><span>rowser</span><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="165538-image.png" alt="" width="603" height="563" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Input </span><span style="font-weight:bold">Workspace Collection Name</span><span> and
</span><span style="font-weight:bold">Access Key</span><span>, then click </span>
<span style="font-weight:bold">Get Workspaces</span><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="165539-image.png" alt="" width="603" height="563" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>A</span><span>fter</span><span> the workspaces and reports are loaded, click Embed button. Then you can see the report.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="165540-image.png" alt="" width="830" height="867" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
Get access token:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>
<pre class="hidden">function getAccessToken(accessKey, workspaceId, reportId, workspaceCollectionName) {
    var token1 = '{&quot;alg&quot;: &quot;HS256&quot;,&quot;typ&quot;: &quot;JWT&quot;}';
    var nbf = new Date().getTime() / 1000 | 0;
    var exp = new Date().setTime(new Date().getTime() &#43; 60 * 60 * 1000) / 1000 | 0;
    var token2 = '{&quot;ver&quot;:&quot;0.2.0&quot;,&quot;wcn&quot;:&quot;' &#43; workspaceCollectionName &#43; '&quot;,&quot;wid&quot;: &quot;' &#43; workspaceId &#43; '&quot;,&quot;rid&quot;:&quot;' &#43; reportId &#43; '&quot;,&quot;iss&quot;:&quot;PowerBISDK&quot;,&quot;aud&quot;:&quot;https://analysis.windows.net/powerbi/api&quot;,&quot;exp&quot;:' &#43; exp &#43; ',&quot;nbf&quot;:' &#43; nbf &#43; '}'
    var endcodedToken = encode_helper(token1) &#43; '.' &#43; encode_helper(token2);
    var hash = CryptoJS.HmacSHA256(endcodedToken, accessKey);
    var hashInBase64 = CryptoJS.enc.Base64.stringify(hash);
    var sig = formatString(hashInBase64);
    var accessToken = endcodedToken &#43; '.' &#43; sig;
    return accessToken;
}

function encode_helper(arg) {
    var response = btoa(arg);
    return formatString(response);
}

//replace special char in the string
function formatString(arg) {
    var response = arg;
    response = response.replace(/\//g, &quot;_&quot;);
    response = response.replace(/\&#43;/g, &quot;-&quot;);
    response = response.replace(/=&#43;$/, '');;
    return response;
}</pre>
<div class="preview">
<pre class="js"><span class="js__operator">function</span>&nbsp;getAccessToken(accessKey,&nbsp;workspaceId,&nbsp;reportId,&nbsp;workspaceCollectionName)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;token1&nbsp;=&nbsp;<span class="js__string">'{&quot;alg&quot;:&nbsp;&quot;HS256&quot;,&quot;typ&quot;:&nbsp;&quot;JWT&quot;}'</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;nbf&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__object">Date</span>().getTime()&nbsp;/&nbsp;<span class="js__num">1000</span>&nbsp;|&nbsp;<span class="js__num">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;exp&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__object">Date</span>().setTime(<span class="js__operator">new</span>&nbsp;<span class="js__object">Date</span>().getTime()&nbsp;&#43;&nbsp;<span class="js__num">60</span>&nbsp;*&nbsp;<span class="js__num">60</span>&nbsp;*&nbsp;<span class="js__num">1000</span>)&nbsp;/&nbsp;<span class="js__num">1000</span>&nbsp;|&nbsp;<span class="js__num">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;token2&nbsp;=&nbsp;<span class="js__string">'{&quot;ver&quot;:&quot;0.2.0&quot;,&quot;wcn&quot;:&quot;'</span>&nbsp;&#43;&nbsp;workspaceCollectionName&nbsp;&#43;&nbsp;<span class="js__string">'&quot;,&quot;wid&quot;:&nbsp;&quot;'</span>&nbsp;&#43;&nbsp;workspaceId&nbsp;&#43;&nbsp;<span class="js__string">'&quot;,&quot;rid&quot;:&quot;'</span>&nbsp;&#43;&nbsp;reportId&nbsp;&#43;&nbsp;<span class="js__string">'&quot;,&quot;iss&quot;:&quot;PowerBISDK&quot;,&quot;aud&quot;:&quot;https://analysis.windows.net/powerbi/api&quot;,&quot;exp&quot;:'</span>&nbsp;&#43;&nbsp;exp&nbsp;&#43;&nbsp;<span class="js__string">',&quot;nbf&quot;:'</span>&nbsp;&#43;&nbsp;nbf&nbsp;&#43;&nbsp;<span class="js__string">'}'</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;endcodedToken&nbsp;=&nbsp;encode_helper(token1)&nbsp;&#43;&nbsp;<span class="js__string">'.'</span>&nbsp;&#43;&nbsp;encode_helper(token2);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;hash&nbsp;=&nbsp;CryptoJS.HmacSHA256(endcodedToken,&nbsp;accessKey);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;hashInBase64&nbsp;=&nbsp;CryptoJS.enc.Base64.stringify(hash);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;sig&nbsp;=&nbsp;formatString(hashInBase64);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;accessToken&nbsp;=&nbsp;endcodedToken&nbsp;&#43;&nbsp;<span class="js__string">'.'</span>&nbsp;&#43;&nbsp;sig;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;accessToken;&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
<span class="js__operator">function</span>&nbsp;encode_helper(arg)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;response&nbsp;=&nbsp;btoa(arg);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;formatString(response);&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
<span class="js__sl_comment">//replace&nbsp;special&nbsp;char&nbsp;in&nbsp;the&nbsp;string</span>&nbsp;
<span class="js__operator">function</span>&nbsp;formatString(arg)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;response&nbsp;=&nbsp;arg;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;response&nbsp;=&nbsp;response.replace(<span class="js__reg_exp">/\//g</span>,&nbsp;<span class="js__string">&quot;_&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;response&nbsp;=&nbsp;response.replace(<span class="js__reg_exp">/\&#43;/g</span>,&nbsp;<span class="js__string">&quot;-&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;response&nbsp;=&nbsp;response.replace(<span class="js__reg_exp">/=&#43;$/</span>,&nbsp;<span class="js__string">''</span>);;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;response;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;Get workspaces</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>
<pre class="hidden">function getWorkspaces(workspaceCollectionName, accessKey) {
    var requestUrl = &quot;https://api.powerbi.com/v1.0/collections/&quot; &#43; workspaceCollectionName &#43; &quot;/workspaces&quot;;
    var request = new Request(requestUrl, {
        headers: new Headers({
            'Authorization': 'AppKey ' &#43; accessKey
        })
    });
    fetch(request).then(function (response) {
        if (response.ok) {
            return response.json()
                .then(function (data) {
                    listWorkspaces(data.value, workspaceCollectionName, accessKey);
                });
        }
    });

}</pre>
<div class="preview">
<pre class="js"><span class="js__operator">function</span>&nbsp;getWorkspaces(workspaceCollectionName,&nbsp;accessKey)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;requestUrl&nbsp;=&nbsp;<span class="js__string">&quot;https://api.powerbi.com/v1.0/collections/&quot;</span>&nbsp;&#43;&nbsp;workspaceCollectionName&nbsp;&#43;&nbsp;<span class="js__string">&quot;/workspaces&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;request&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;Request(requestUrl,&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;headers:&nbsp;<span class="js__operator">new</span>&nbsp;Headers(<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">'Authorization'</span>:&nbsp;<span class="js__string">'AppKey&nbsp;'</span>&nbsp;&#43;&nbsp;accessKey&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;fetch(request).then(<span class="js__operator">function</span>&nbsp;(response)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(response.ok)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;response.json()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.then(<span class="js__operator">function</span>&nbsp;(data)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;listWorkspaces(data.value,&nbsp;workspaceCollectionName,&nbsp;accessKey);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
</div>
<div class="endscriptcode">Get reports</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>
<pre class="hidden">function getReports(workspaceId, workspaceCollectionName, accessKey) {
    var reportRequest = &quot;https://api.powerbi.com/v1.0/collections/&quot; &#43; workspaceCollectionName &#43; &quot;/workspaces/&quot; &#43; workspaceId &#43; &quot;/reports&quot;;
    var request1 = new Request(reportRequest, {
        headers: new Headers({
            'Authorization': 'AppKey ' &#43; accessKey
        })
    });
    fetch(request1).then(function (response) {
        if (response.ok) {
            return response.json().then(function (data) {
                listReports(data.value);
            });
        }
    });
}</pre>
<div class="preview">
<pre class="js"><span class="js__operator">function</span>&nbsp;getReports(workspaceId,&nbsp;workspaceCollectionName,&nbsp;accessKey)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;reportRequest&nbsp;=&nbsp;<span class="js__string">&quot;https://api.powerbi.com/v1.0/collections/&quot;</span>&nbsp;&#43;&nbsp;workspaceCollectionName&nbsp;&#43;&nbsp;<span class="js__string">&quot;/workspaces/&quot;</span>&nbsp;&#43;&nbsp;workspaceId&nbsp;&#43;&nbsp;<span class="js__string">&quot;/reports&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;request1&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;Request(reportRequest,&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;headers:&nbsp;<span class="js__operator">new</span>&nbsp;Headers(<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">'Authorization'</span>:&nbsp;<span class="js__string">'AppKey&nbsp;'</span>&nbsp;&#43;&nbsp;accessKey&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;fetch(request1).then(<span class="js__operator">function</span>&nbsp;(response)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(response.ok)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;response.json().then(<span class="js__operator">function</span>&nbsp;(data)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;listReports(data.value);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
</div>
<div class="endscriptcode">Embed report</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>
<pre class="hidden">function embedReport(report, accessKey, workspaceCollectionName, workspaceId) {
    var embedUrl = report.embedUrl;
    var name = report.name;
    var reportId = report.id;
    var webUrl = report.webUrl;
    var token = getAccessToken(accessKey, workspaceId, reportId, workspaceCollectionName);
    var embedConfiguration = {
        type: 'report',
        accessToken: token,
        id: reportId,
        embedUrl: embedUrl
    };
    var $reportContainer = $('#reportContainer');
    var report = powerbi.embed($reportContainer.get(0), embedConfiguration);
}</pre>
<div class="preview">
<pre class="js"><span class="js__operator">function</span>&nbsp;embedReport(report,&nbsp;accessKey,&nbsp;workspaceCollectionName,&nbsp;workspaceId)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;embedUrl&nbsp;=&nbsp;report.embedUrl;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;name&nbsp;=&nbsp;report.name;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;reportId&nbsp;=&nbsp;report.id;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;webUrl&nbsp;=&nbsp;report.webUrl;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;token&nbsp;=&nbsp;getAccessToken(accessKey,&nbsp;workspaceId,&nbsp;reportId,&nbsp;workspaceCollectionName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;embedConfiguration&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;type:&nbsp;<span class="js__string">'report'</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;accessToken:&nbsp;token,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;id:&nbsp;reportId,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;embedUrl:&nbsp;embedUrl&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;$reportContainer&nbsp;=&nbsp;$(<span class="js__string">'#reportContainer'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;report&nbsp;=&nbsp;powerbi.embed($reportContainer.get(<span class="js__num">0</span>),&nbsp;embedConfiguration);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/en-us/azure/power-bi-embedded/power-bi-embedded-what-is-power-bi-embedded" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">What is Microsoft Power BI Embedded</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/en-us/azure/power-bi-embedded/power-bi-embedded-get-started-sample"><span style="color:#0563c1; text-decoration:underline">Get started with Microsoft Power BI Embedded</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#0563c1; text-decoration:underline"><a href="https://docs.microsoft.com/en-us/azure/power-bi-embedded/power-bi-embedded-iframe" style="text-decoration:none">How to use Power BI Embedded with REST</a></span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<a href="https://www.azure.cn/documentation/articles/developerdifferences">Developer difference between Azure China and Global</a></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#0563c1; text-decoration:underline"><br>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
