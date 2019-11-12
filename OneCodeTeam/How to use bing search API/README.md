# How to use bing search API
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- Microsoft Azure
## Topics
- Azure Market Place
## Updated
- 09/11/2016
## Description

<h1><span lang="EN-US">How to use </span><span lang="EN-US">Azure market place</span><span lang="EN-US">
</span><span lang="EN-US">Bing search API</span><span lang="EN-US"> (</span><span lang="EN-US">CSAzureMarketPlaceBingSearch</span><span lang="EN-US">)</span></h1>
<h2><span lang="EN-US">Introduction </span></h2>
<p class="Normal"><span lang="EN-US">Bing search now is available in Azure market place.
</span></p>
<p style="margin-bottom:12.0pt"><span lang="EN-US" style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; color:black">The Bing Search API offers multiple source types (or types of search results). You can request a single source type or multiple source
 types with each query. For instance, you can request web, images, news, and video results for a single search query.<br>
The Bing Search API returns results for the following source types: </span></p>
<table class="MsoNormalTable" border="0" cellpadding="0">
<tbody>
<tr>
<td width="123" style="width:91.9pt; padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><strong><span lang="EN-US" style="font-size:9.0pt; font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#333333">Source Type</span></strong><span lang="EN-US" style="font-size:9.0pt; font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#333333">
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><strong><span lang="EN-US" style="font-size:9.0pt; font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#333333">Description</span></strong><span lang="EN-US" style="font-size:9.0pt; font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#333333">
</span></p>
</td>
</tr>
<tr>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Web</span><span lang="EN-US" style="font-size:9.0pt; font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#333333">
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Web search results
</span></p>
</td>
</tr>
<tr>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Images
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Image search results
</span></p>
</td>
</tr>
<tr>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">News
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">News search results
</span></p>
</td>
</tr>
<tr>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Videos
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Video search results
</span></p>
</td>
</tr>
<tr>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Related Search
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Related search suggestions based on the query entered
</span></p>
</td>
</tr>
<tr>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Spelling Suggestions
</span></p>
</td>
<td style="padding:.75pt .75pt .75pt .75pt">
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal"><span lang="EN-US" style="color:black">Spelling suggestions based on the query entered
</span></p>
</td>
</tr>
</tbody>
</table>
<p class="Normal"><span lang="EN-US">&nbsp;</span></p>
<p class="Normal"><span lang="EN-US">This sample is based on windows Azure market place developer's requirement. It shows how to use each source type list above in your code project.</span></p>
<p class="Normal">See Universal Windows Platform version <strong><a href="https://code.msdn.microsoft.com/How-to-use-Bing-Search-API-80aebbd4">here</a></strong>.</p>
<p class="Normal"><span lang="EN-US"><br>
</span></p>
<h2><span lang="EN-US">Building the Sample </span></h2>
<p class="MsoNormal"><span lang="EN-US" style="color:black">You need to get your market place account key first.
</span></p>
<p class="MsoNormal"><span lang="EN-US" style="color:black">First go to the <a href="https://datamarket.azure.com/">
Azure market place website</a> and sign in with your live id. </span></p>
<p class="MsoNormal"><span lang="EN-US" style="color:black">Second open <a href="https://datamarket.azure.com/dataset/bing/search">
Bing search API</a>, and choose the suitable subscription you prefer. </span></p>
<p class="MsoNormal"><span lang="EN-US" style="color:black">Get the account key and open your project, in default.aspx.cs file, find the variable &quot;AccountKey&quot; and replace the value with your account key.
</span></p>
<h2><span lang="EN-US">Running the Sample</span></h2>
<p class="MsoNormal"><span lang="EN-US">Input &quot;xbox&quot; in the text box, and click every button to check the result.
</span></p>
<h2><span lang="EN-US">Using the Code</span></h2>
<p class="MsoNormal"><span lang="EN-US">Step 1: </span><span lang="EN-US">Create an empty web form application</span><span lang="EN-US">.</span></p>
<p class="MsoNormal"><span lang="EN-US">Step 2:</span><span lang="EN-US"> Create a text box.
</span></p>
<p class="MsoNormal"><span lang="EN-US">Step 3: Create 7 buttons with code: </span>
</p>
<p class="MsoNormal"><span lang="EN-US">&nbsp;</span></p>
<p class="MsoNormal"><span lang="EN-US">&nbsp;</span></p>
<p class="MsoNormal"><span lang="EN-US">Step 4: Build and run the application. </span>
</p>
<p class="MsoNormal"><span lang="EN-US">&nbsp;</span></p>
<h2><span lang="EN-US">More Information</span><span lang="EN-US"> </span></h2>
<p class="MsoNormal"><span lang="EN-US" style="font-family:&quot;Segoe UI&quot;,&quot;sans-serif&quot;; color:#333333"><a href="http://datamarket.azure.com/dataset/bing/search">Bing Search API</a></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
