# Brokered Messaging: Duplicate Detection
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
- Service Bus
## Topics
- Service Bus
## Updated
- 09/02/2014
## Description

<h1>Introduction</h1>
<p>This sample demonstrates how to use the Windows Azure Service Bus duplicate message detection with queues. See the Service Bus documentation for more information about the Service Bus before exploring the samples.</p>
<p>This sample creates two queues, one with duplicate detection enabled and other one without duplicate detection.</p>
<p>&nbsp;</p>
<h1>Prerequisites</h1>
<p>If you haven't already done so, please read the release notes document that explains how to sign up for a Windows Azure account and how to configure your environment.</p>
<h1><br>
Sample Flow</h1>
<p>The sample flows in the following manner:</p>
<ol>
<li>Sample creates a queue called &ldquo;DefaultQueue&rdquo; without duplicate<br>
detection enabled:
<ol>
<li>Sends a message with MessageId &ldquo;MessageId123&rdquo;; </li><li>Sends another message with the same MessageId i.e. a duplicate<br>
message </li><li>Receives the messages. It receives both the messages as the queue<br>
does not detect duplicate messages. </li></ol>
</li><li>Sample creates another queue called &ldquo;RemoveDuplicatesQueue&rdquo; with<br>
duplicate detection enabled:
<ol>
<li>Sends a message with MessageId &ldquo;MessageId123&rdquo;; </li><li>Sends another message with the same MessageId i.e. a duplicate<br>
message </li><li>Receives the messages. This time it receives only one message<br>
since the duplicate messages are detected and dropped by the queue itself. </li></ol>
</li><li>Sample deletes both the queues </li></ol>
<p>&nbsp;</p>
<h1>Running the Sample</h1>
<p>To run the sample:</p>
<ol>
<li>Build the solution in Visual Studio and run the sample project. </li><li>When prompted enter the Service Bus connection string. </li></ol>
<h2><br>
<strong>Expected Output</strong></h2>
<p>Please provide a connection string to ServiceBus (/? for help): &lt;connection string&gt;<br>
&nbsp;<br>
Creating DefaultQueue ...<br>
Created DefaultQueue<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Sending messages to DefaultQueue ...<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; =&gt; Sent a message with messageId ABC123<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; =&gt; Sent a duplicate message with messageId ABC123<br>
&nbsp;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Waiting for messages from DefaultQueue ...<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;= Received a message with messageId ABC123<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;= Received a message with messageId ABC123<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; RECEIVED a DUPLICATE MESSAGE<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Done receiving messages from DefaultQueue<br>
&nbsp;<br>
Creating RemoveDuplicatesQueue ...<br>
Created RemoveDuplicatesQueue<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Sending messages to RemoveDuplicatesQueue ...<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; =&gt; Sent a message with messageId ABC123<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; =&gt; Sent a duplicate message with messageId ABC123<br>
&nbsp;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Waiting for messages from RemoveDuplicatesQueue ...<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;= Received a message with messageId ABC123<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Done receiving messages from RemoveDuplicatesQueue<br>
&nbsp;<br>
Press [Enter] to exit.<br>
<br>
</p>
