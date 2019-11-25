# How to remove blank pages in word
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- Word
- Remove blank pages
## Updated
- 06/13/2013
## Description

<h1>How to Remove blank pages in Word document (CSWordRemoveBlankPage)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to remove blank pages in Word document automatically. Customers should choose a word document firstly, then click &quot;Remove Blank Page&quot; button to remove blank pages in word document.</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you should make sure Microsoft Office 2010 installed on your computer.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1. Open &quot;CSWordRemoveBlankPage<span style="">.sln</span>&quot; file to open the project and press F5 to run the project. The form resembles the following:</p>
<p class="MsoNormal"><span style=""><img src="84585-image.png" alt="" width="453" height="166" align="middle">
</span></p>
<p class="MsoNormal">Step2. Click &quot;Select&quot; button to choose a word document with blank pages from your file system. When you click &quot;Select&quot; button, there is an open file dialog to let you choose word document. After you choose an existing
 word document and click ok button in the open file dialog, you can see the path of word document that you selected show in textbox control.</p>
<p class="MsoNormal"><span style=""><img src="84586-image.png" alt="" width="450" height="163" align="middle">
</span></p>
<p class="MsoNormal">Step3. Click &quot;Remove Blank Page&quot; button to remove blank pages in word document.
<span style="">If the operation is successfully, you will receive the following form:
</span></p>
<p class="MsoNormal"><span style=""><img src="84587-image.png" alt="" width="507" height="174" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"></p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step1. Create Windows Forms application from the template in Visual Studio.</p>
<p class="MsoNormal">Step2. Add Microsoft.Office.Interop<span style="">.Word.dll assemble and add the
</span>Microsoft.Office.Interop<span style="">.Word namespace</span> into codes.</p>
<p class="MsoNormal">Step3. Coding the click event of &quot;Select&quot; button.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
        /// Open word document
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
        private void btnOpenWord_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog openfileDialog=new OpenFileDialog())
            {
                openfileDialog.Filter=&quot;Word document(*.doc,*.docx)|*.doc;*.docx&quot;;
                if (openfileDialog.ShowDialog() == DialogResult.OK)
                {
                    txbWordPath.Text = openfileDialog.FileName;
                    wordPath = openfileDialog.FileName;
                }
            }
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step4. Coding the click event of &quot;Remove Blank Page&quot; button.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
      /// Click event of Remove Blank Page button
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
      /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
      private void btnRemove_Click(object sender, EventArgs e)
      {
          if (!File.Exists(txbWordPath.Text))
          {
              MessageBox.Show(&quot;Please Select valid path of word document!&quot;,&quot;Error&quot;,MessageBoxButtons.OK,MessageBoxIcon.Error);
              return;
          }


          // Remove blank Page in word document
          if (RemoveBlankPage()==true)
          {
              MessageBox.Show(&quot;Remove blank page successfully!&quot;);
          }
      }


      /// &lt;summary&gt;
      /// Remove Blank Page in Word document
      /// &lt;/summary&gt;
      private bool RemoveBlankPage()
      {
          Word.Application wordapp = null;
          Word.Document doc = null;
          Word.Paragraphs paragraphs=null;
          try
          {
              // Start Word APllication and set it be invisible
              wordapp = new Word.Application();
              wordapp.Visible = false;
              doc = wordapp.Documents.Open(wordPath);
              paragraphs = doc.Paragraphs;
              foreach (Word.Paragraph paragraph in paragraphs)
              {
                  if (paragraph.Range.Text.Trim() == string.Empty)
                  {
                      paragraph.Range.Select();
                      wordapp.Selection.Delete();
                  }
              }


              // Save the document and close document
              doc.Save();
              ((Word._Document)doc).Close();


              // Quit the word application
              ((Word._Application)wordapp).Quit();


          }
          catch(Exception ex)
          {
              MessageBox.Show(&quot;Exception Occur, error message is: &quot;&#43;ex.Message);
              return false;
          }
          finally
          { 
              // Clean up the unmanaged Word COM resources by explicitly
              // call Marshal.FinalReleaseComObject on all accessor objects
              if (paragraphs != null)
              {
                  Marshal.FinalReleaseComObject(paragraphs);
                  paragraphs = null;
              }
              if (doc != null)
              {
                  Marshal.FinalReleaseComObject(doc);
                  doc = null;
              }
              if (wordapp != null)
              {
                  Marshal.FinalReleaseComObject(wordapp);
                  wordapp = null;
              }
          }


          return true;
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<h2>More Information</h2>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span>Office Dev Center
</span></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/office/aa905340">http://msdn.microsoft.com/en-us/office/aa905340</a>
</p>
<p class="MsoNormal">Word Object Model Overview</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/kw65a0we(v=VS.80).aspx">http://msdn.microsoft.com/en-us/library/kw65a0we(v=VS.80).aspx</a>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
