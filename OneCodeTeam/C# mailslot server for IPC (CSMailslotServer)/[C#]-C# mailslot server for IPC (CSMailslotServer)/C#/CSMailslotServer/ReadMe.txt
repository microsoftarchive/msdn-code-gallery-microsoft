=============================================================================
          CONSOLE APPLICATION : CSMailslotServer Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

Mailslot is a mechanism for one-way inter-process communication in the local
machine or across the computers in the intranet. Any clients can store 
messages in a mailslot. The creator of the slot, i.e. the server, retrieves 
the messages that are stored there:

Client (GENERIC_WRITE) ---> Server (GENERIC_READ)

This code sample demonstrates calling CreateMailslot to create a mailslot 
named "\\.\mailslot\SampleMailslot". The security attributes of the slot are  
customized to allow Authenticated Users read and write access to the slot, 
and to allow the Administrators group full access to it. The sample first 
creates such a mailslot, then it reads and displays new messages in the slot 
when user presses ENTER in the console.


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
Code Logic (P/Invoke the native APIs):

1. Create a mailslot. 

  1.1 Prepare the security attributes (the lpSecurityAttributes parameter in 
  CreateMailslot) for the mailslot. This is optional. If the 
  lpSecurityAttributes parameter of CreateMailslot is NULL, the mailslot gets 
  a default security descriptor and the handle cannot be inherited. The ACLs 
  in the default security descriptor of a mailslot grant full control to the 
  LocalSystem account, (elevated) administrators, and the creator owner. They 
  also give only read access to members of the Everyone group and the 
  anonymous account. However, if you want to customize the security 
  permission of the mailslot, (e.g. to allow Authenticated Users to read from 
  and write to the mailslot), you need to create a SECURITY_ATTRIBUTES structure.
  
  The CreateMailslotSecurity helper function creates and initializes a new 
  SECURITY_ATTRIBUTES structure to allow Authenticated Users read and write 
  access to a mailslot, and to allow the Administrators group full access to 
  the mailslot.
  
  1.2 Create the mailslot. (CreateMailslot)

2. Check messages in the mailslot. (ReadMailslot)
 
  2.1. Check for the number of messages in the mailslot. (GetMailslotInfo)
 
  2.2. Retrieve the messages one by one from the mailslot. While reading,  
  update the number of messages that are left in the mailslot. 
  (ReadFile, GetMailslotInfo)

3. Close the handle of the mailslot instance. (CloseHandle)


/////////////////////////////////////////////////////////////////////////////
References:

Using Mailslots / Reading from a Mailslot
http://msdn.microsoft.com/en-us/library/aa365785.aspx


/////////////////////////////////////////////////////////////////////////////
