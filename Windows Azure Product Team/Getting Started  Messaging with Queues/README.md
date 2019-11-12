# Getting Started : Messaging with Queues
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
<p style="text-align:justify">This sample demonstrates how to use Windows Azure Service Bus to send and receive messages from a queue. The queue provides decoupled, asynchronous communication between a sender and any number of receivers (here, a single receiver).</p>
<p><span style="color:black">Imagine a scenario where a company tracks issues sent by customers. This sample demonstrates how to do this with a&nbsp;Queue. Messages are delivered in the order recieved.
</span><span style="color:black">The sample has a single message receiver processing messages from the Queue, but additional receivers can be added on that same Queue. In that case competing consumers can be used to load-balance and load level between the systems.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">Create a Service Bus Namespace from <a href="http://windows.azure.com">
http://windows.azure.com</a> </span></p>
<p>&nbsp;</p>
<h1>Sample Flow</h1>
<p><br>
<span style="font-size:small">The sample flows in the following manner:</span></p>
<ol>
<li><span style="font-size:small">Creation of Queue</span> </li><li><span style="font-size:small">Send messages to&nbsp;Queue</span> </li><li><span style="font-size:small">Receive messages for&nbsp;Queue&nbsp;</span>&nbsp;
</li></ol>
<h1>Running the Sample</h1>
<p><span style="font-size:small">To run the sample:</span></p>
<ol>
<li><span style="font-size:small">Download Microsoft.ServiceBus.dll via NuGet.</span>
</li><li><span style="font-size:small">Build the solution in Visual Studio. </span></li><li><span style="font-size:small">Provide a Service Bus connection string from Portal in app.config file.</span>
</li><li><span style="font-size:small">Run the project.</span> </li></ol>
<h2><strong>Expected Output - Sender</strong></h2>
<p><span style="font-size:small">Creating Queue</span></p>
<p><span style="font-size:small">Sending messages to Queue...</span></p>
<p><span style="font-size:small">Message sent: Id = 1, Body = First message information</span></p>
<p><span style="font-size:small">Message sent: Id = 2, Body = Second message information</span></p>
<p><span style="font-size:small">Message sent: Id = 3, Body = Third message information</span></p>
<p>&nbsp;</p>
<h2><strong><span style="color:black">Expected Output - Receiver</span></strong></h2>
<p><span style="font-size:small">&nbsp;</span></p>
<p><span style="font-size:small">Receiving messages from Queue...</span></p>
<p><span style="font-size:small">Message received: Id = 1, Body = First message information</span></p>
<p><span style="font-size:small">Message received: Id = 2, Body = Second message information</span></p>
<p><span style="font-size:small">Message received: Id = 3, Body = Third message information</span></p>
<p><span style="font-size:small">End of scenario, press ENTER to exit.<span style="font-family:Times New Roman; font-size:small">
</span></span></p>
