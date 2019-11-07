==============================================================================
 ASP.NET APPLICATION : CSASPNETBackgroundWorker Project Overview
==============================================================================

//////////////////////////////////////////////////////////////////////////////
Summary:

Sometimes we do an operation which needs long time to complete. It will 
stop the response and the page is blank until the operation finished. 
In this case, We want the operation to run in the background, and in the page, 
we want to display the progress of the running operation. Therefore, the user 
can know the operation is running and can know the progress.

On the other hand, we want to schedule some operations (send email/report ect.).
We want the operations can be run at the specific time. 

This project creates a class named "BackgroundWorker" to achieve these goals. It
creates a page named "Default.aspx" to run the long time operation. And it 
creates a Background Worker to do the schedule when application starts up, then
it uses "GlobalBackgroundWorker.aspx" page to check the progress.


//////////////////////////////////////////////////////////////////////////////
Demo the Sample:

1. Session based Background Worker.
    a. Open Default.aspx, then click the button to run the operation in background.
    b. Open Default.aspx in two browsers, then click the buttons at the same time.
       You will see that two Background Workers work independently.

2. Application Level Background Worker.
    a. Open GlobalBackgroundWorker.aspx. You will see that the Background Worker
       is running.
    b. Close the browser, wait for 10 seconds and then open the page again. 
       You will see that the Background Worker is still running even we closed 
       the browser.

//////////////////////////////////////////////////////////////////////////////
Code Logical:

1. Open the class named "BackgroundWorker". You will see that it starts an
   operation (method) in a separated thread.

    _innerThread = new Thread(() =>
    {
        _progress = 0;
        DoWork.Invoke(ref _progress, ref _result, arguments);
        _progress = 100;
    });
    _innerThread.Start();

2. In the page named "Default.aspx". It uses UpdatePanel to achieve partial 
   refreshing. And it uses Timer control to update the operation progress.

    <!-- UpdateUpanel let the progress can be updated without updating the whole page (partial update). -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <!-- The timer which used to update the progress. -->
            <asp:Timer ID="Timer1" runat="server" Interval="100" Enabled="false" ontick="Timer1_Tick"></asp:Timer>

            <!-- The Label which used to display the progress and the result -->
            <asp:Label ID="lbProgress" runat="server" Text=""></asp:Label><br />

            <!-- Start the operation by inputting value and clicking the button -->
            Input a parameter: <asp:TextBox ID="txtParameter" runat="server" Text="Hello World"></asp:TextBox><br />
            <asp:Button ID="btnStart" runat="server" Text="Click to Start the Background Worker" onclick="btnStart_Click" />

        </ContentTemplate>
    </asp:UpdatePanel>

    In btnStart_Click() method which handles button click event. It creates
    a Background Worker and saves it to Session State.
    So that the Background Worker is bound to current Session.

        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
        worker.RunWorker(txtParameter.Text);

        // It needs Session Mode is "InProc"
        // to keep the Background Worker working.
        Session["worker"] = worker;

3. In the Global class, you will find Application_Start() method creates a 
   Background Worker and then saves it to Application State. Therefore, the
   Background Worker will keep running in background and it is shared by all
   users.
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
        worker.RunWorker(null);

        // This Background Worker is Applicatoin Level,
        // so it will keep working and it is shared by all users.
        Application["worker"] = worker;

//////////////////////////////////////////////////////////////////////////////
References:

Using Threads and Threading
http://msdn.microsoft.com/en-us/library/e1dx6b2h.aspx

UpdatePanel Control Overview
http://msdn.microsoft.com/en-us/library/bb386454.aspx

Timer Control Overview
http://msdn.microsoft.com/en-us/library/bb398865.aspx

Events (C# Programming Guide)
http://msdn.microsoft.com/en-us/library/awbftdfh.aspx