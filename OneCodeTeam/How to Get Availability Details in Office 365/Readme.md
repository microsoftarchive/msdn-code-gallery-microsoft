# How to Get Availability Details in Office 365
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Exchange Online
- Office 365
## Topics
- availability details
## Updated
- 09/22/2016
## Description

<h1><span><span>如何在 Office 365 中取得空閒時間詳細資料</span></span></h1>
<h2><span><span>簡介</span></span></h2>
<p><span><span>目前，Outlook Web App (OWA) 可讓您使用排程助理來檢查空閒時間。但是，您可能想要有一份追蹤會議室可用時間的事件清單。我們會在此應用程式中示範如何在 Office 365 中</span></span><span><span>取得</span><span>空閒時間詳細資料。</span></span></p>
<p><span><span>1. 您必須針對想得知空閒時間詳細資料的對象</span></span><span><span>輸入電子郵件地址和期間。</span></span></p>
<p><span><span>&nbsp;</span><span>2. 應用程式將檢查地址和日期。</span></span></p>
<p><span><span>3. 最後，應用程式將顯示空閒時間詳細資料的結果。</span></span></p>
<h2><span><span>執行範例</span></span></h2>
<p><span><span>按下 F5 以執行範例。</span></span></p>
<p><span><span>首先，</span><span>使用您的帳戶連線至 Exchange Online。</span></span></p>
<p><span><span><img id="154367" src="154367-image.png" alt="" width="408" height="57"></span></span></p>
<p><span><span><span><span>然後，您可針對想得知空閒時間詳細資料的對象</span><span>輸入識別和期間。</span></span><strong>&nbsp;</strong><em>&nbsp;</em></span></span></p>
<p><img id="154368" src="154368-8b90969f-5216-472f-b0fd-6fab8cc83073image.png" alt="" width="633" height="79"></p>
<p><span><span><span><span>最後，空閒時間結果將</span><span>顯示</span><span>如下</span><a name="_GoBack"></a><span>：</span></span><strong>&nbsp;</strong><em>&nbsp;</em></span></span></p>
<p><img id="154369" src="154369-aa143f28-4e41-4448-8be5-d5f5300833d0image.png" alt=""></p>
<h2><span><span>使用程式碼</span></span></h2>
<p><span><span>取得識別</span></span></p>
<p><span><span></span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Dim identities() As String = inputInfo.Split(&quot;,&quot;c)
 Dim emailAddresses As New List(Of String)()
 For Each identity As String In identities
     Dim nameResolutions As NameResolutionCollection =
         service.ResolveName(identity, ResolveNameSearchLocation.DirectoryOnly, True)
     If nameResolutions.Count &lt;&gt; 1 Then
         Console.WriteLine(&quot;{0} is invalid user identity.&quot;, identity)
     Else
         Dim emailAddress As String = nameResolutions(0).Mailbox.Address
         emailAddresses.Add(emailAddress)
     End If
 Next identity
 If emailAddresses.Count &gt; 0 Then
     GetAvailabilityDetails(service, startDate, endDate, emailAddresses.ToArray())
 End If</pre>
<pre class="hidden">String[] identities = inputInfo.Split(',');
List&lt;String&gt; emailAddresses = new List&lt;String&gt;();
foreach (String identity in identities)
{
    NameResolutionCollection nameResolutions =
        service.ResolveName(identity, ResolveNameSearchLocation.DirectoryOnly, true);
    if (nameResolutions.Count != 1)
    {
        Console.WriteLine(&quot;{0} is invalid user identity.&quot;, identity);
    }
    else
    {
        String emailAddress = nameResolutions[0].Mailbox.Address;
        emailAddresses.Add(emailAddress);
    }
}
if (emailAddresses.Count &gt; 0)
{
    GetAvailabilityDetails(service, startDate, endDate, emailAddresses.ToArray());
}
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Dim</span>&nbsp;identities()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;inputInfo.Split(<span class="visualBasic__string">&quot;,&quot;</span>c)&nbsp;
&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;emailAddresses&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;<span class="visualBasic__keyword">String</span>)()&nbsp;
&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;identity&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;identities&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;nameResolutions&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;NameResolutionCollection&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;service.ResolveName(identity,&nbsp;ResolveNameSearchLocation.DirectoryOnly,&nbsp;<span class="visualBasic__keyword">True</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;nameResolutions.Count&nbsp;&lt;&gt;&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="visualBasic__string">&quot;{0}&nbsp;is&nbsp;invalid&nbsp;user&nbsp;identity.&quot;</span>,&nbsp;identity)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;emailAddress&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;nameResolutions(<span class="visualBasic__number">0</span>).Mailbox.Address&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;emailAddresses.Add(emailAddress)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;identity&nbsp;
&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;emailAddresses.Count&nbsp;&gt;&nbsp;<span class="visualBasic__number">0</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;GetAvailabilityDetails(service,&nbsp;startDate,&nbsp;endDate,&nbsp;emailAddresses.ToArray())&nbsp;
&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p><span><span>設定期間和出席者</span></span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p><span><span></span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">' 如果日期無效，我們會將開始日期設為今天。
Dim startMeetingDate As Date
startMeetingDate = If(Date.TryParse(startDate, startMeetingDate), startMeetingDate, Date.Now)
' 如果日期無效，我們會將結束日期設為開始日期後兩天。
Dim endMeetingDate As Date
endMeetingDate = If(Date.TryParse(endDate, endMeetingDate) AndAlso
                    endMeetingDate &gt;= startMeetingDate, endMeetingDate,
                    startMeetingDate.AddDays(2))
Dim attendees As New List(Of AttendeeInfo)()
For Each emailAddress As String In emailAddresses
    Dim attendee As New AttendeeInfo(emailAddress)
    attendees.Add(attendee)
Next emailAddress
Dim timeWindow As New TimeWindow(startMeetingDate, endMeetingDate)
Dim availabilityOptions As New AvailabilityOptions()
availabilityOptions.MeetingDuration = 60</pre>
<pre class="hidden">// 如果日期無效，我們會將開始日期設為今天。
DateTime startMeetingDate;
startMeetingDate=
    DateTime.TryParse(startDate, out startMeetingDate)?startMeetingDate:DateTime.Now;
// 如果日期無效，我們會將結束日期設為開始日期後兩天。
DateTime endMeetingDate;
endMeetingDate = 
    DateTime.TryParse(endDate, out endMeetingDate)&amp;&amp;endMeetingDate&gt;=startMeetingDate ? 
    endMeetingDate : startMeetingDate.AddDays(2);
List&lt;AttendeeInfo&gt; attendees = new List&lt;AttendeeInfo&gt;();
foreach (String emailAddress in emailAddresses)
{
    AttendeeInfo attendee = new AttendeeInfo(emailAddress);
    attendees.Add(attendee);
}
TimeWindow timeWindow = new TimeWindow(startMeetingDate,endMeetingDate);
AvailabilityOptions availabilityOptions = new AvailabilityOptions();
availabilityOptions.MeetingDuration = 60;
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__com">'&nbsp;如果日期無效，我們會將開始日期設為今天。</span>&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;startMeetingDate&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Date</span>&nbsp;
startMeetingDate&nbsp;=&nbsp;<span class="visualBasic__keyword">If</span>(<span class="visualBasic__keyword">Date</span>.TryParse(startDate,&nbsp;startMeetingDate),&nbsp;startMeetingDate,&nbsp;<span class="visualBasic__keyword">Date</span>.Now)&nbsp;
<span class="visualBasic__com">'&nbsp;如果日期無效，我們會將結束日期設為開始日期後兩天。</span>&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;endMeetingDate&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Date</span>&nbsp;
endMeetingDate&nbsp;=&nbsp;<span class="visualBasic__keyword">If</span>(<span class="visualBasic__keyword">Date</span>.TryParse(endDate,&nbsp;endMeetingDate)&nbsp;<span class="visualBasic__keyword">AndAlso</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endMeetingDate&nbsp;&gt;=&nbsp;startMeetingDate,&nbsp;endMeetingDate,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startMeetingDate.AddDays(<span class="visualBasic__number">2</span>))&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;attendees&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;AttendeeInfo)()&nbsp;
<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;emailAddress&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;emailAddresses&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;attendee&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;AttendeeInfo(emailAddress)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;attendees.Add(attendee)&nbsp;
<span class="visualBasic__keyword">Next</span>&nbsp;emailAddress&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;timeWindow&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;TimeWindow(startMeetingDate,&nbsp;endMeetingDate)&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;availabilityOptions&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;AvailabilityOptions()&nbsp;
availabilityOptions.MeetingDuration&nbsp;=&nbsp;<span class="visualBasic__number">60</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p><span><span>取得空閒時間結果並將其顯示</span></span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p><span><span></span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">Dim userAvailabilityResults As GetUserAvailabilityResults =
    service.GetUserAvailability(attendees, timeWindow,
                                AvailabilityData.FreeBusyAndSuggestions, availabilityOptions)
Console.WriteLine(&quot;{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}&quot;,
                &quot;FreeBusyStatus&quot;, &quot;StartTime&quot;, &quot;EndTime&quot;, &quot;Subject&quot;, &quot;Location&quot;, &quot;IsMeeting&quot;)
For Each userAvailabilityResult As AttendeeAvailability In
    userAvailabilityResults.AttendeesAvailability
    If userAvailabilityResult.ErrorCode.CompareTo(ServiceError.NoError) = 0 Then
        For Each calendarEvent As CalendarEvent In userAvailabilityResult.CalendarEvents
            Console.WriteLine(&quot;{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}&quot;,
                              calendarEvent.FreeBusyStatus,
                              calendarEvent.StartTime.ToShortDateString() &amp; &quot; &quot; &amp;
                              calendarEvent.StartTime.ToShortTimeString(),
                              calendarEvent.EndTime.ToShortTimeString(),
                              calendarEvent.Details.Subject,
                              calendarEvent.Details.Location,
                              calendarEvent.Details.IsMeeting)
        Next calendarEvent
    End If
Next userAvailabilityResult
</pre>
<pre class="hidden">GetUserAvailabilityResults userAvailabilityResults = service.GetUserAvailability(attendees, 
    timeWindow, AvailabilityData.FreeBusyAndSuggestions, availabilityOptions);
Console.WriteLine(&quot;{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}&quot;, &quot;FreeBusyStatus&quot;, 
    &quot;StartTime&quot;, &quot;EndTime&quot;, &quot;Subject&quot;, &quot;Location&quot;, &quot;IsMeeting&quot;);
foreach (AttendeeAvailability userAvailabilityResult in 
    userAvailabilityResults.AttendeesAvailability)
{
    if (userAvailabilityResult.ErrorCode.CompareTo(ServiceError.NoError) == 0)
    {
        foreach (CalendarEvent calendarEvent in userAvailabilityResult.CalendarEvents)
        {
            Console.WriteLine(&quot;{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}&quot;, 
                calendarEvent.FreeBusyStatus, 
                calendarEvent.StartTime.ToShortDateString() &#43; &quot; &quot; &#43; 
                calendarEvent.StartTime.ToShortTimeString(), 
                calendarEvent.EndTime.ToShortTimeString(), 
                calendarEvent.Details.Subject, 
                calendarEvent.Details.Location, 
                calendarEvent.Details.IsMeeting);
        }
    }
}
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Dim</span>&nbsp;userAvailabilityResults&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;GetUserAvailabilityResults&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;service.GetUserAvailability(attendees,&nbsp;timeWindow,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AvailabilityData.FreeBusyAndSuggestions,&nbsp;availabilityOptions)&nbsp;
Console.WriteLine(<span class="visualBasic__string">&quot;{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__string">&quot;FreeBusyStatus&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;StartTime&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;EndTime&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;Subject&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;Location&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;IsMeeting&quot;</span>)&nbsp;
<span class="visualBasic__keyword">For</span><span class="visualBasic__keyword">Each</span>&nbsp;userAvailabilityResult&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AttendeeAvailability&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;userAvailabilityResults.AttendeesAvailability&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;userAvailabilityResult.ErrorCode.CompareTo(ServiceError.NoError)&nbsp;=&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">Then</span><span class="visualBasic__keyword">For</span><span class="visualBasic__keyword">Each</span>&nbsp;calendarEvent&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CalendarEvent&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;userAvailabilityResult.CalendarEvents&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="visualBasic__string">&quot;{0,-15}{1,-21}{2,-11}{3,-14}{4,-10}{5,-9}&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.FreeBusyStatus,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.StartTime.ToShortDateString()&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;&nbsp;&quot;</span>&nbsp;&amp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.StartTime.ToShortTimeString(),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.EndTime.ToShortTimeString(),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.Details.Subject,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.Details.Location,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;calendarEvent.Details.IsMeeting)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;calendarEvent&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span><span class="visualBasic__keyword">Next</span>&nbsp;userAvailabilityResult&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p><span><span>更多資訊</span></span></p>
<p><span><a href="http://msdn.microsoft.com/zh-tw/library/dd633709(v=exchg.80).aspx"><span>EWS Managed API 2.0</span></a></span></p>
<p><span><a href="http://msdn.microsoft.com/zh-tw/library/hh532567%28v=exchg.80%29.aspx"><span>使用 EWS Managed API 來取得使用者的空閒/忙碌資訊</span></a></span></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><span><img id="154370" src="154370-8ceaf4b9-39f1-47dc-ae5e-ffbff5b81e5fimage.png" alt="" width="341" height="57"></span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><span><span><br>
</span></span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><span style="color:#ffffff"><br>
</span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.<strong>&nbsp;</strong><em>&nbsp;</em></span></p>
<p><span>&nbsp;</span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><span><span><br>
</span></span></p>
