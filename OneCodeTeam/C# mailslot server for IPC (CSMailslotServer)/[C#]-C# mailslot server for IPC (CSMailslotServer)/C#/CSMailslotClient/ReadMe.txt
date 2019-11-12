=============================================================================
         CONSOLE APPLICATION : CSMailslotClient Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

Mailslot is a mechanism for one-way inter-process communication in the local
machine or across the computers in the intranet. Any clients can store 
messages in a mailslot. The creator of the slot, i.e. the server, retrieves 
the messages that are stored there:

Client (GENERIC_WRITE) ---> Server (GENERIC_READ)

This sample demonstrates a mailslot client that connects and writes to the 
mailslot "\\.\mailslot\SampleMailslot". 


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the mailslot sample.

Step1. After you successfully build the CSMailslotClient and CSMailslotServer 
sample projects in Visual Studio 2008, you will get the applications: 
CSMailslotClient.exe and CSMailslotServer.exe. 

Step2. Run CSMailslotServer.exe in a command prompt to start up the server 
end of the mailslot. The application will output the following information 
in the command prompt if the mailslot is created successfully.

  Server:
    The mailslot (\\.\mailslot\SampleMailslot) is created.

Step3. Run CSMailslotClient.exe in another command prompt to start up the 
client end of the mailslot. The application should output the message below 
in the command prompt when the client successfully opens the mailslot.

  Client:
    The mailslot (\\.\mailslot\SampleMailslot) is opened.

Step4. The client attempts to write three messages to the mailslot. 

  Client:
    The message "Message 1 for mailslot" is written to the slot
    The message "Message 2 for mailslot" is written to the slot
    The message "Message 3 for mailslot" is written to the slot

There is a three seconds' interval between the second message and the third 
one. During the interval, if you press ENTER in the server application, the 
mailslot server will retrieve the first two messages and display them. 

  Server:
    Checking new messages...
    Message #1: Message 1 for mailslot
    Message #2: Message 2 for mailslot

After the interval, the client writes the thrid message. If you press ENTER 
again in the server application, the mailslot server prints the message:

  Server:
    Checking new messages...
    Message #1: Message 3 for mailslot

Step5. Enter 'Q' in the server application. This will close the mailslot and 
quit the server.


/////////////////////////////////////////////////////////////////////////////
Sample Relation:
(The relationship between the current sample and the rest samples in 
Microsoft All-In-One Code Framework http://1code.codeplex.com)

CSMailslotClient -> CSMailslotServer
CSMailslotServer creates the mailslot. CSMailslotClient opens the mailslot 
and writes messages to it.


/////////////////////////////////////////////////////////////////////////////
Code Logic(P/Invoke the native APIs):

1. Open the mailslot. (CreateFile)

2. Write messages to the mailslot. (WriteMailslot, WriteFile)

3. Close the slot. (CloseHandle)


/////////////////////////////////////////////////////////////////////////////
References:

Using Mailslots / Writing to a Mailslot
http://msdn.microsoft.com/en-us/library/aa365802.aspx


/////////////////////////////////////////////////////////////////////////////
