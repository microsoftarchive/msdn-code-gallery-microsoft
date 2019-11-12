# Getting Started : Messaging With Topics
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
- Service Bus
## Topics
- Service Bus
## Updated
- 08/13/2014
## Description

<h1>Introduction</h1>
<p><br>
<span style="color:black">This sample demonstrates how to use Windows Azure Service Bus to send and receive messages from a topic with multiple subscriptions.</span></p>
<p><span style="color:black">Similar to a queue, a topic provides decoupled, asynchronous communication between a sender and any number of receivers. Furthermore, a topic allows duplication and filtering of messages.</span></p>
<p><span style="color:black">Imagine a scenario where a company tracks issues sent by customers, using two different systems - one for resolving the opened issues and one that logs the issues for audit purposes. This sample demonstrates how to do this with
 a topic and two subscriptions, AgentSubscription and AuditSubscription. Both subscriptions use the default settings, so messages sent into the topic will be delivered to both subscriptions. (See the AdvancedFilters sample for how to set up a subscription so
 that only certain messages are delivered, or a custom action is performed as those messages are delivered.)</span></p>
<p><span style="color:black">The sample has a single message receiver processing messages from each subscription, but additional receivers can be added on that same subscription. In that case, the subscription behaves like a queue, with competing consumers.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">Create a Service Bus Namespace from <a href="http://windows.azure.com">
http://windows.azure.com</a> </span></p>
<p>&nbsp;</p>
<h1>Sample Flow</h1>
<p><br>
<span style="font-size:small">The sample flows in the following manner:</span></p>
<ol>
<li><span style="font-size:small">Creation of Topics/Subscriptions</span> </li><li><span style="font-size:small">Send messages to&nbsp;Topic</span> </li><li><span style="font-size:small">Receive messages for Subscriptions&nbsp;</span>&nbsp;
</li></ol>
<h1>Running the Sample</h1>
<p><span style="font-size:small">To run the sample:</span></p>
<ol>
<li><span style="font-size:small">Download Microsoft.ServiceBus.dll via NuGet.</span>
</li><li><span style="font-size:small">Build the solution in Visual Studio. </span></li><li><span style="font-size:small">Provide a Service Bus connection string from Portal in app.config file.</span>
</li><li><span style="font-size:small">Run the project</span>. </li></ol>
<h2><br>
<strong>Expected Output - Sender</strong></h2>
<p><span style="font-size:small">&nbsp;</span></p>
<p><span style="font-size:small">Creating Topic SampleTopic...</span></p>
<p><span style="font-size:small">Creating Subscriptions 'AuditSubscription' and 'AgentSubscription'...</span></p>
<p><span style="font-size:small">Sending messages to topic...</span></p>
<p><span style="font-size:small">Message sent: Id = 1, Body = First message information</span></p>
<p><span style="font-size:small">Message sent: Id = 2, Body = Second message information</span></p>
<p><span style="font-size:small">Message sent: Id = 3, Body = Third message information</span></p>
<p><span style="font-size:small">Finished sending messages, press ENTER to clean up and exit.</span></p>
<p>&nbsp;</p>
<h2><strong><span style="color:black">Expected Output - Receiver</span></strong></h2>
<p><span style="font-size:small">&nbsp;</span></p>
<p><span style="font-size:small">Receiving messages from AgentSubscription...</span></p>
<p><span style="font-size:small">Message received: Id = 1, Body = First message information</span></p>
<p><span style="font-size:small">Message received: Id = 2, Body = Second message information</span></p>
<p><span style="font-size:small">Message received: Id = 3, Body = Third message information</span></p>
<p><span style="font-size:small">Receiving messages from AuditSubscription...</span></p>
<p><span style="font-size:small">Message received: Id = 1, Body = First message information</span></p>
<p><span style="font-size:small">Message received: Id = 2, Body = Second message information</span></p>
<p><span style="font-size:small">Message received: Id = 3, Body = Third message information</span></p>
<p><span style="font-size:small">End of scenario, press ENTER to exit.<span style="font-family:Times New Roman; font-size:small">
</span></span></p>
