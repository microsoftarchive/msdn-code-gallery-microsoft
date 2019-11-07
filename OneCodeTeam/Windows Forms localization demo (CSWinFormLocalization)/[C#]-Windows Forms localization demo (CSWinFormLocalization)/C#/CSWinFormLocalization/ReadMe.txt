================================================================================
       WINDOWS FORMS APPLICATION : CSWinFormLocalization Project Overview
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

The Windows Forms Localization sample demonstrates how to localize 
Windows Forms application.
   

/////////////////////////////////////////////////////////////////////////////
Creation:

1. Create a new Windows Application named "CSWinFormLocalization". 

2. In the Properties window, set the form's Localizable property to true.

3. Drag two Button controls and one Label control from the Windows Forms tab 
   of the Toolbox to the form, and set their Text property as follows:
   
   button1: "Hello World!",
   button2: "I'm a button.",
   label1 : "I'm a label".
   
4. Set the form's Language property to Chinese (Simplified Chinese).

5. Set the Text property for the three controls as follows:

   button1: "你好，世界！", 
   button2: "我是一个按钮。", 
   label1 : "我是一个标签。".

6. Save and build the solution.

7. Click the Show All Files button in Solution Explorer. 

   The resource files appear underneath Form1.cs. 
   Form1.resx is the resource file for the default culture.
   Form1.zh-CHS.resx is the resource file for Simplified Chinese as spoken in PRC. 

8. Press Ctrl+F5 to run the application, the buttons and label will display in 
   English or Simplified Chinese depending on the UI language of your o
   perating system. 
   
9. If you want the form always display in Simplified Chinese, you can set the 
   UI culture to Simplified Chinese before calling the InitializeComponent method.

    public Form1()
    {
        // Sets the UI culture to Simplified Chinese. 
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");

        InitializeComponent();
    }
    
    

/////////////////////////////////////////////////////////////////////////////
References:

1. Walkthrough: Localizing Windows Forms
   http://msdn.microsoft.com/en-us/library/y99d1cd3.aspx
   
2. Windows Forms General FAQ.
   http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/77a66f05-804e-4d58-8214-0c32d8f43191
   

/////////////////////////////////////////////////////////////////////////////
