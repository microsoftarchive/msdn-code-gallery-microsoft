# How to login on office 365 and get account info with REST using web application
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- REST
- Office 365
## Topics
- C#
- REST
- Office 365
## Updated
- 03/12/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">How to log in office 365 and get account info with REST API using web application</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>This sample demonstrates how to log in office 365 and get the account information with REST using web application.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>When users visit the website, they will be redirected to office 365 to finish the authentication. Then, they will go forward to our website with a code to request REST API for a token. At last, they will get the user account information and picture
 with the token.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Register the application for your OneDrive,&nbsp;while the related&nbsp;details will be&nbsp;described in&nbsp;the&nbsp;next section</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Go to the
</span><a href="https://apps.dev.microsoft.com/Disambiguation?ru=https%3a%2f%2fapps.dev.microsoft.com%2f" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Register and manage apps</span></a><span> to register you application.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>When prompted, sign in with you Microsoft account credentials.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Find
</span><span style="font-weight:bold">My applications </span><span>and click </span>
<span style="font-weight:bold">Add an app</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="170932" src="170932-untitled.png" alt="" width="594" height="428"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Enter your app&rsquo;s name and click
</span><span style="font-weight:bold">Create application</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157891" src="157891-2.png" alt="" width="803" height="260"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157892" src="157892-3.png" alt="" width="501" height="280"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Scroll to the page bottom and tick in the
</span><span style="font-weight:bold">Live ADK support </span><span>box.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157893" src="157893-4.png" alt="" width="501" height="280"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Generate the new password below the
</span><span style="font-weight:bold">Application Secrets</span><span>, and save it for later use.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157894" src="157894-5.png" alt="" width="395" height="121"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="170933" src="170933-untitled.png" alt="" width="456" height="254"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Create a web app below the Platforms header, and then set the Redirect URIs to you web app callback address such as
</span><a href="http://localhost:1438/Home/OnAuthComplate" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://localhost:1438/Home/OnAuthComplate</span></a><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157897" src="157897-7.png" alt="" width="596" height="292"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click
</span><span style="font-weight:bold">Save</span><span> at the very bottom of the page.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Building the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Double-Click
</span><span style="font-weight:bold">CSLoginOnOffice365AndGetAccountInfo.sln</span><span> file to open this sample solution using Microsoft Visual Studio 2015 which has the web develop component installed.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Set project LoginOnOffice365AndGetAccountInfo as startup project.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157898" src="157898-8.png" alt="" width="466" height="512"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Config the following parameter in:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold">&nbsp;</span><span>Project: </span><span style="font-weight:bold">LoginOnOffice365AndGetAccountInfo/Controllers/HomeController.cs</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157899" src="157899-9.png" alt="" width="511" height="117"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>About the ClientId you can find it here:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="170935" src="170935-untitled.png" alt="" width="534" height="444"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Secret is the key for the application where you can set password only once after it shows up.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157901" src="157901-11.png" alt="" width="625" height="176"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>For CallbackUri, you can find it here:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="157902" src="157902-12.png" alt="" width="579" height="298"></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Open the sample solution using Visual Studio, then press
</span><span style="font-weight:bold">F5 key</span><span> or select </span><span style="font-weight:bold">Debug -&gt; Start Debugging</span><span> in menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>When the site is started, you can see this:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<img id="170936" src="170936-untitled.png" alt="" width="409" height="413"></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>After you have filled all the fields and clicked the Sign in button, you can see this:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>&nbsp;<img id="170937" src="170937-untitled.png" alt="" width="539" height="345"></span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>The Office 365 Rest API access base class</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public abstract class OAuthAccessBase
{
    public string ClientId { get; }
    public string ClientSecret { get; }
 
    //when user complated the authenticate, will retrun this code
    public string AccessCode { get; protected set; }
    //use this token to request Office 365 API
    public string AccessToken { get; protected set; }
    //when accessToken had expires, can use this token to refresh accessToken
    public string RefreshToken { get; protected set; }
 
    public string UserId { get; protected set; }
    public DateTime RefreshTime { get; protected set; }
    public TimeSpan RefreshTimeSpan { get; protected set; }
 
    public string RedirectURI { get; set; }
 
    public OAuthAccessBase(string clientId, string clientSecret, string redirectURI)
    {
        this.ClientId = clientId;
        this.ClientSecret = clientSecret;
        this.RedirectURI = redirectURI;
    }
 
    //retrun authenticate url for redirect
    public string GetLoginUrl(string scopes)
    {
        string urlStr =
            &quot;https://login.live.com/oauth20_authorize.srf&quot; &#43;
            &quot;?client_id=&quot; &#43; ClientId &#43;
            &quot;&amp;scope=offline_access &quot; &#43; scopes &#43;
            &quot;&amp;response_type=code&quot; &#43;
            &quot;&amp;redirect_uri=&quot; &#43; RedirectURI;
 
        return urlStr.ToString();
    }
 
    //get token use code
    public async Task RedeemTokensAsync(string code)
    {
        this.AccessCode = code;
 
        string url = &quot;https://login.live.com/oauth20_token.srf&quot;;
        string paramStr =
            &quot;client_id=&quot; &#43; ClientId &#43;
            &quot;&amp;redirect_uri=&quot; &#43; RedirectURI &#43;
            &quot;&amp;client_secret=&quot; &#43; ClientSecret &#43;
            &quot;&amp;code=&quot; &#43; AccessCode &#43;
            &quot;&amp;grant_type=authorization_code&quot;;
 
        APIRequest request = GetRequest(url, HTTPMethod.Post, paramStr.ToString());
        string response = await request.GetResponseToStringAsync();
 
        JObject jo = JObject.Parse(response);
 
        this.RefreshToken = jo.SelectToken(&quot;refresh_token&quot;).Value&lt;string&gt;();
        this.AccessToken = jo.SelectToken(&quot;access_token&quot;).Value&lt;string&gt;();
        this.UserId = jo.SelectToken(&quot;user_id&quot;).Value&lt;string&gt;();
        this.RefreshTimeSpan = new TimeSpan(0, 0, Convert.ToInt32(jo.SelectToken(&quot;expires_in&quot;).Value&lt;string&gt;()));
        this.RefreshTime = DateTime.Now;
    }
 
 
    protected async Task&lt;string&gt; AuthRequestToStringAsync(string url, HTTPMethod httpMethod = HTTPMethod.Get, string data = &quot;&quot;)
    {
        await RefreshTokenIfNeededAsync();
 
        APIRequest request = GetRequest(url, httpMethod, data);
        return await request.GetResponseToStringAsync();
    }
 
    protected async Task&lt;byte[]&gt; AuthRequestToBytesAsync(string url, HTTPMethod httpMethod = HTTPMethod.Get, string data = &quot;&quot;)
    {
        await RefreshTokenIfNeededAsync();
 
        APIRequest request = GetRequest(url, httpMethod, data);
        return await request.GetResponseTobytesAsync();
    }
 
    private APIRequest GetRequest(string url, HTTPMethod httpMethod = HTTPMethod.Get, string data = &quot;&quot;)
    {
        APIRequest apiRequest = new APIRequest(url, httpMethod, data);
 
        if (!string.IsNullOrEmpty(AccessToken))
        {
            apiRequest.Request.Headers.Add(&quot;Authorization&quot;, &quot;bearer &quot; &#43; AccessToken);
        }
 
        return apiRequest;
    }
 
    private async Task RefreshTokenIfNeededAsync()
    {
        if (RefreshTimeSpan == null || DateTime.Now - RefreshTime &gt; RefreshTimeSpan)
        {
            string url = &quot;https://login.live.com/oauth20_token.srf&quot;;
            string paramStr = &quot;client_id=&quot; &#43; ClientId &#43;
                &quot;&amp;redirect_uri=&quot; &#43; RedirectURI &#43;
                &quot;&amp;client_secret=&quot; &#43; ClientSecret &#43;
                &quot;&amp;refresh_token=&quot; &#43; RefreshToken &#43;
                &quot;&amp;grant_type=refresh_token&quot;;
 
            APIRequest request = GetRequest(url, HTTPMethod.Post, paramStr.ToString());
            string response = await request.GetResponseToStringAsync();
 
            JObject jo = JObject.Parse(response);
 
            this.RefreshToken = jo.SelectToken(&quot;refresh_token&quot;).Value&lt;string&gt;();
            this.AccessToken = jo.SelectToken(&quot;access_token&quot;).Value&lt;string&gt;();
            this.UserId = jo.SelectToken(&quot;user_id&quot;).Value&lt;string&gt;();
            this.RefreshTimeSpan = new TimeSpan(0, 0,Convert.ToInt32(jo.SelectToken(&quot;expires_in&quot;).Value&lt;string&gt;()));
            this.RefreshTime = DateTime.Now;
        }
    }
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">abstract</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;OAuthAccessBase&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;ClientId&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;ClientSecret&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//when&nbsp;user&nbsp;complated&nbsp;the&nbsp;authenticate,&nbsp;will&nbsp;retrun&nbsp;this&nbsp;code</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;AccessCode&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//use&nbsp;this&nbsp;token&nbsp;to&nbsp;request&nbsp;Office&nbsp;365&nbsp;API</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;AccessToken&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//when&nbsp;accessToken&nbsp;had&nbsp;expires,&nbsp;can&nbsp;use&nbsp;this&nbsp;token&nbsp;to&nbsp;refresh&nbsp;accessToken</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;RefreshToken&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;UserId&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;DateTime&nbsp;RefreshTime&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;TimeSpan&nbsp;RefreshTimeSpan&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;RedirectURI&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;OAuthAccessBase(<span class="cs__keyword">string</span>&nbsp;clientId,&nbsp;<span class="cs__keyword">string</span>&nbsp;clientSecret,&nbsp;<span class="cs__keyword">string</span>&nbsp;redirectURI)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.ClientId&nbsp;=&nbsp;clientId;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.ClientSecret&nbsp;=&nbsp;clientSecret;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RedirectURI&nbsp;=&nbsp;redirectURI;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//retrun&nbsp;authenticate&nbsp;url&nbsp;for&nbsp;redirect</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;GetLoginUrl(<span class="cs__keyword">string</span>&nbsp;scopes)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;urlStr&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;https://login.live.com/oauth20_authorize.srf&quot;</span>&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;?client_id=&quot;</span>&nbsp;&#43;&nbsp;ClientId&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;scope=offline_access&nbsp;&quot;</span>&nbsp;&#43;&nbsp;scopes&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;response_type=code&quot;</span>&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;redirect_uri=&quot;</span>&nbsp;&#43;&nbsp;RedirectURI;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;urlStr.ToString();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//get&nbsp;token&nbsp;use&nbsp;code</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;async&nbsp;Task&nbsp;RedeemTokensAsync(<span class="cs__keyword">string</span>&nbsp;code)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.AccessCode&nbsp;=&nbsp;code;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;url&nbsp;=&nbsp;<span class="cs__string">&quot;https://login.live.com/oauth20_token.srf&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;paramStr&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;client_id=&quot;</span>&nbsp;&#43;&nbsp;ClientId&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;redirect_uri=&quot;</span>&nbsp;&#43;&nbsp;RedirectURI&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;client_secret=&quot;</span>&nbsp;&#43;&nbsp;ClientSecret&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;code=&quot;</span>&nbsp;&#43;&nbsp;AccessCode&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;grant_type=authorization_code&quot;</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;APIRequest&nbsp;request&nbsp;=&nbsp;GetRequest(url,&nbsp;HTTPMethod.Post,&nbsp;paramStr.ToString());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;response&nbsp;=&nbsp;await&nbsp;request.GetResponseToStringAsync();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JObject&nbsp;jo&nbsp;=&nbsp;JObject.Parse(response);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RefreshToken&nbsp;=&nbsp;jo.SelectToken(<span class="cs__string">&quot;refresh_token&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.AccessToken&nbsp;=&nbsp;jo.SelectToken(<span class="cs__string">&quot;access_token&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.UserId&nbsp;=&nbsp;jo.SelectToken(<span class="cs__string">&quot;user_id&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RefreshTimeSpan&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;TimeSpan(<span class="cs__number">0</span>,&nbsp;<span class="cs__number">0</span>,&nbsp;Convert.ToInt32(jo.SelectToken(<span class="cs__string">&quot;expires_in&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;()));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RefreshTime&nbsp;=&nbsp;DateTime.Now;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">protected</span>&nbsp;async&nbsp;Task&lt;<span class="cs__keyword">string</span>&gt;&nbsp;AuthRequestToStringAsync(<span class="cs__keyword">string</span>&nbsp;url,&nbsp;HTTPMethod&nbsp;httpMethod&nbsp;=&nbsp;HTTPMethod.Get,&nbsp;<span class="cs__keyword">string</span>&nbsp;data&nbsp;=&nbsp;<span class="cs__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;RefreshTokenIfNeededAsync();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;APIRequest&nbsp;request&nbsp;=&nbsp;GetRequest(url,&nbsp;httpMethod,&nbsp;data);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;await&nbsp;request.GetResponseToStringAsync();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">protected</span>&nbsp;async&nbsp;Task&lt;<span class="cs__keyword">byte</span>[]&gt;&nbsp;AuthRequestToBytesAsync(<span class="cs__keyword">string</span>&nbsp;url,&nbsp;HTTPMethod&nbsp;httpMethod&nbsp;=&nbsp;HTTPMethod.Get,&nbsp;<span class="cs__keyword">string</span>&nbsp;data&nbsp;=&nbsp;<span class="cs__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;RefreshTokenIfNeededAsync();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;APIRequest&nbsp;request&nbsp;=&nbsp;GetRequest(url,&nbsp;httpMethod,&nbsp;data);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;await&nbsp;request.GetResponseTobytesAsync();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;APIRequest&nbsp;GetRequest(<span class="cs__keyword">string</span>&nbsp;url,&nbsp;HTTPMethod&nbsp;httpMethod&nbsp;=&nbsp;HTTPMethod.Get,&nbsp;<span class="cs__keyword">string</span>&nbsp;data&nbsp;=&nbsp;<span class="cs__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;APIRequest&nbsp;apiRequest&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;APIRequest(url,&nbsp;httpMethod,&nbsp;data);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!<span class="cs__keyword">string</span>.IsNullOrEmpty(AccessToken))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;apiRequest.Request.Headers.Add(<span class="cs__string">&quot;Authorization&quot;</span>,&nbsp;<span class="cs__string">&quot;bearer&nbsp;&quot;</span>&nbsp;&#43;&nbsp;AccessToken);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;apiRequest;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;async&nbsp;Task&nbsp;RefreshTokenIfNeededAsync()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(RefreshTimeSpan&nbsp;==&nbsp;<span class="cs__keyword">null</span>&nbsp;||&nbsp;DateTime.Now&nbsp;-&nbsp;RefreshTime&nbsp;&gt;&nbsp;RefreshTimeSpan)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;url&nbsp;=&nbsp;<span class="cs__string">&quot;https://login.live.com/oauth20_token.srf&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;paramStr&nbsp;=&nbsp;<span class="cs__string">&quot;client_id=&quot;</span>&nbsp;&#43;&nbsp;ClientId&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;redirect_uri=&quot;</span>&nbsp;&#43;&nbsp;RedirectURI&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;client_secret=&quot;</span>&nbsp;&#43;&nbsp;ClientSecret&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;refresh_token=&quot;</span>&nbsp;&#43;&nbsp;RefreshToken&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&amp;grant_type=refresh_token&quot;</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;APIRequest&nbsp;request&nbsp;=&nbsp;GetRequest(url,&nbsp;HTTPMethod.Post,&nbsp;paramStr.ToString());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;response&nbsp;=&nbsp;await&nbsp;request.GetResponseToStringAsync();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JObject&nbsp;jo&nbsp;=&nbsp;JObject.Parse(response);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RefreshToken&nbsp;=&nbsp;jo.SelectToken(<span class="cs__string">&quot;refresh_token&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.AccessToken&nbsp;=&nbsp;jo.SelectToken(<span class="cs__string">&quot;access_token&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.UserId&nbsp;=&nbsp;jo.SelectToken(<span class="cs__string">&quot;user_id&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RefreshTimeSpan&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;TimeSpan(<span class="cs__number">0</span>,&nbsp;<span class="cs__number">0</span>,Convert.ToInt32(jo.SelectToken(<span class="cs__string">&quot;expires_in&quot;</span>).Value&lt;<span class="cs__keyword">string</span>&gt;()));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.RefreshTime&nbsp;=&nbsp;DateTime.Now;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>The Office 365 Rest API access class:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public class OAuthAccess : OAuthAccessBase
{
    public OAuthAccess(string clientId, string clientSecret, string redirectURI) : base(clientId, clientSecret, redirectURI)
    {
    }
 
    public async Task&lt;Dictionary&lt;string, string&gt;&gt; GetAccountInfoAsync()
    {
        string response = await AuthRequestToStringAsync(&quot;https://apis.live.net/v5.0/me?suppress_response_codes=true&amp;suppress_redirects=true&quot;);
 
        JObject jo = JObject.Parse(response);
 
        return jo.ToObject&lt;Dictionary&lt;string, string&gt;&gt;();
    }
 
    public async Task&lt;byte[]&gt; GetAccountPicture()
    {
        byte[] response = await AuthRequestToBytesAsync(&quot;https://apis.live.net/v5.0/me/picture&quot;);
 
        return response;
    }
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;OAuthAccess&nbsp;:&nbsp;OAuthAccessBase&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;OAuthAccess(<span class="cs__keyword">string</span>&nbsp;clientId,&nbsp;<span class="cs__keyword">string</span>&nbsp;clientSecret,&nbsp;<span class="cs__keyword">string</span>&nbsp;redirectURI)&nbsp;:&nbsp;<span class="cs__keyword">base</span>(clientId,&nbsp;clientSecret,&nbsp;redirectURI)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;async&nbsp;Task&lt;Dictionary&lt;<span class="cs__keyword">string</span>,&nbsp;<span class="cs__keyword">string</span>&gt;&gt;&nbsp;GetAccountInfoAsync()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;response&nbsp;=&nbsp;await&nbsp;AuthRequestToStringAsync(<span class="cs__string">&quot;https://apis.live.net/v5.0/me?suppress_response_codes=true&amp;suppress_redirects=true&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JObject&nbsp;jo&nbsp;=&nbsp;JObject.Parse(response);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;jo.ToObject&lt;Dictionary&lt;<span class="cs__keyword">string</span>,&nbsp;<span class="cs__keyword">string</span>&gt;&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;async&nbsp;Task&lt;<span class="cs__keyword">byte</span>[]&gt;&nbsp;GetAccountPicture()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">byte</span>[]&nbsp;response&nbsp;=&nbsp;await&nbsp;AuthRequestToBytesAsync(<span class="cs__string">&quot;https://apis.live.net/v5.0/me/picture&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;response;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#000000; font-size:9.5pt">The Controller class:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public class HomeController : Controller
{
    private const string ClientId = &quot;cdc1dc9d-159f-4b5f-a2a6-21a30b0b45ae&quot;;
    private const string Secret = &quot;xChUsnbQthVgYN5VoAjEcYy&quot;;
    private const string CallbackUri = &quot;http://localhost:1438/Home/OnAuthComplate&quot;;
 
    public OAuthAccess OfficeAccess
    {
        get
        {
            var officeAccess = Session[&quot;OfficeAccess&quot;];
            if (officeAccess == null)
            {
                officeAccess = new OAuthAccess(ClientId, Secret, CallbackUri);
                Session[&quot;OfficeAccess&quot;] = officeAccess;
            }
            return officeAccess as OAuthAccess;
        }
    }
 
    public async Task&lt;ActionResult&gt; Index()
    {
        //if user is not login, redirect to office 365 for authenticate
        if (string.IsNullOrEmpty(OfficeAccess.AccessCode))
        {
            string url = OfficeAccess.GetLoginUrl(&quot;onedrive.appfolder&quot;);
 
            return new RedirectResult(url);
        }
 
        //when user is authenticated get user account info
        ViewBag.UserInfo = await OfficeAccess.GetAccountInfoAsync();
        return View();
    }
 
    //when user complate authenticate, will be call back this url with a code
    public async Task&lt;RedirectResult&gt; OnAuthComplate(string code)
    {
        //get token by the code
        await OfficeAccess.RedeemTokensAsync(code);
 
        return new RedirectResult(&quot;Index&quot;);
    }
 
    //download user picture
    public async Task&lt;ActionResult&gt; UserPicture()
    {
        var btyes = await OfficeAccess.GetAccountPicture();
        return base.File(btyes, &quot;image/jpeg&quot;);
    }
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;HomeController&nbsp;:&nbsp;Controller&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">const</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;ClientId&nbsp;=&nbsp;<span class="cs__string">&quot;cdc1dc9d-159f-4b5f-a2a6-21a30b0b45ae&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">const</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;Secret&nbsp;=&nbsp;<span class="cs__string">&quot;xChUsnbQthVgYN5VoAjEcYy&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">const</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;CallbackUri&nbsp;=&nbsp;<span class="cs__string">&quot;http://localhost:1438/Home/OnAuthComplate&quot;</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;OAuthAccess&nbsp;OfficeAccess&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;officeAccess&nbsp;=&nbsp;Session[<span class="cs__string">&quot;OfficeAccess&quot;</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(officeAccess&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;officeAccess&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OAuthAccess(ClientId,&nbsp;Secret,&nbsp;CallbackUri);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Session[<span class="cs__string">&quot;OfficeAccess&quot;</span>]&nbsp;=&nbsp;officeAccess;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;officeAccess&nbsp;<span class="cs__keyword">as</span>&nbsp;OAuthAccess;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;async&nbsp;Task&lt;ActionResult&gt;&nbsp;Index()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//if&nbsp;user&nbsp;is&nbsp;not&nbsp;login,&nbsp;redirect&nbsp;to&nbsp;office&nbsp;365&nbsp;for&nbsp;authenticate</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">string</span>.IsNullOrEmpty(OfficeAccess.AccessCode))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;url&nbsp;=&nbsp;OfficeAccess.GetLoginUrl(<span class="cs__string">&quot;onedrive.appfolder&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;RedirectResult(url);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//when&nbsp;user&nbsp;is&nbsp;authenticated&nbsp;get&nbsp;user&nbsp;account&nbsp;info</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ViewBag.UserInfo&nbsp;=&nbsp;await&nbsp;OfficeAccess.GetAccountInfoAsync();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;View();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//when&nbsp;user&nbsp;complate&nbsp;authenticate,&nbsp;will&nbsp;be&nbsp;call&nbsp;back&nbsp;this&nbsp;url&nbsp;with&nbsp;a&nbsp;code</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;async&nbsp;Task&lt;RedirectResult&gt;&nbsp;OnAuthComplate(<span class="cs__keyword">string</span>&nbsp;code)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//get&nbsp;token&nbsp;by&nbsp;the&nbsp;code</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;OfficeAccess.RedeemTokensAsync(code);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;RedirectResult(<span class="cs__string">&quot;Index&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//download&nbsp;user&nbsp;picture</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;async&nbsp;Task&lt;ActionResult&gt;&nbsp;UserPicture()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;btyes&nbsp;=&nbsp;await&nbsp;OfficeAccess.GetAccountPicture();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">base</span>.File(btyes,&nbsp;<span class="cs__string">&quot;image/jpeg&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">More information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>Onedrive develp documentation: </span><a href="https://dev.onedrive.com/getting-started.htm" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://dev.onedrive.com/getting-started.htm</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
