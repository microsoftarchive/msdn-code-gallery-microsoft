# Get Plain Text of a Word Document using Open XML (CSOpenXmlGetPlainText)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Office Development
## Topics
- Word
- Open XML
## Updated
- 12/03/2012
## Description

<h1><span style="">How to g</span>et Plain Text of a Word Document using Open XML (CSOpenXmlGetPlainText)</h1>
<h2>Introduction</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">This sample demonstrates how to <span id="ms-rterangepaste-start">
</span>extract the plain text from word document and export it to text files. </span>
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">The sample also can keep the basic style of the document, like white space and new line. Customers don't need to install Office Software and also can read the plain text of a word document.
</span></p>
<p class="MsoNormal"><span style=""></span></p>
<h2>Building the Sample</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Before building the sample , please make sure you have installed <a href="http://www.microsoft.com/en-us/download/details.aspx?id=5124">
Open XML SDK 2.0 for Microsoft Office</a>&lt;o:p&gt;.&lt;/o:p&gt;</span></p>
<p class="MsoNormal" style="margin-bottom:7.5pt; line-height:normal"><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black"></span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">The following steps walk through a demonstration of Getting plain text of a word document.
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step1. Open </span>CSOpenXmlGetPlainText<span style="">.sln and then click F5 to run this project. You will see the following form:
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><img src="71703-image.png" alt="" width="507" height="398" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step2. Click &quot;Open&quot; button to choose an existing word document
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step3. Click &quot;Get Plain Text&quot; button to extract plain text from a word document and display the text in
<b style="">RichTextBox</b> Control. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><img src="71704-image.png" alt="" width="502" height="397" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step4. Click &quot;Save as Text&quot; button to export the text in RichTextBox to a text file, if the process success, users can get successful message box.
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><img src="71705-image.png" alt="" width="449" height="148" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoNormal"><span style="">Step1. Create Windows Form project. On the File Menu, choose New, Project, in the templates pane, select Windows Forms Application and enter the name of the project.
</span></p>
<p class="MsoNormal"><span style="">Step2. Add the Open xml reference to the project. To reference the Open XML, right click the project file and click the &quot;Add Reference…&quot; button. In the Add Reference dialog, navigate to the .Net tab, find DocumentFormat.OpenXml
 and click OK. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step3. Create <b style="">GetWordPlainText </b>class to read word document using Open XML. Import the Open XML namespace in this class.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;


</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:新宋体; color:green"></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step4. Create ReadWordDocument method to read plain text of a word document
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
       /// &lt;summary&gt;
       ///  Read Word Document
       /// &lt;/summary&gt;
       /// &lt;returns&gt;Plain Text in document &lt;/returns&gt;
       public string ReadWordDocument()
       {
           StringBuilder sb = new StringBuilder();
           OpenXmlElement element = package.MainDocumentPart.Document.Body;
           if (element == null)
           {
               return string.Empty;
           }


           sb.Append(GetPlainText(element));
           return sb.ToString();
       }


       /// &lt;summary&gt;
       ///  Read Plain Text in all XmlElements of word document
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;element&quot;&gt;XmlElement in document&lt;/param&gt;
       /// &lt;returns&gt;Plain Text in XmlElement&lt;/returns&gt;
       public string GetPlainText(OpenXmlElement element)
       {
           StringBuilder PlainTextInWord = new StringBuilder();
           foreach (OpenXmlElement section in element.Elements())
           {             
               switch (section.LocalName)
               {
                   // Text
                   case &quot;t&quot;: 
                       PlainTextInWord.Append(section.InnerText);
                      break;


                   case &quot;cr&quot;:                          // Carriage return
                   case &quot;br&quot;:                          // Page break
                      PlainTextInWord.Append(Environment.NewLine);
                      break;


                   // Tab
                   case &quot;tab&quot;:
                      PlainTextInWord.Append(&quot;\t&quot;);
                      break;


                   // Paragraph
                   case &quot;p&quot;:
                       PlainTextInWord.Append(GetPlainText(section));
                       PlainTextInWord.AppendLine(Environment.NewLine);
                       break;


                   default:
                       PlainTextInWord.Append(GetPlainText(section));
                       break;
               }
           }


           return PlainTextInWord.ToString();
       }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:新宋体; color:green"></span></p>
<p class="MsoNormal" style=""><span style="">Step5. Click &quot;Open&quot; button to choose an existing word document
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
      /// &lt;summary&gt;
      ///  Handle the btnOpen Click event to load an Word file.
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
      /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
      private void btnOpen_Click(object sender, EventArgs e)
      {
          SelectWordFile(); 
      }


      /// &lt;summary&gt;
      /// Show an OpenFileDialog to select a Word document.
      /// &lt;/summary&gt;
      /// &lt;returns&gt;
      /// The file name.
      /// &lt;/returns&gt;
      private string SelectWordFile()
      {
          string fileName = null;
          using (OpenFileDialog dialog = new OpenFileDialog())
          {
              dialog.Filter = &quot;Word document (*.docx)|*.docx&quot;;
              dialog.InitialDirectory = Environment.CurrentDirectory;


              // Retore the directory before closing
              dialog.RestoreDirectory = true;
              if (dialog.ShowDialog()== DialogResult.OK)
              {
                  fileName = dialog.FileName;
                  tbxFile.Text = dialog.FileName;
                  rtbText.Clear();
              }
          }


          return fileName;
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style=""></span></p>
<p class="MsoNormal" style=""><span style="">Step6. Click &quot;Get Plain text&quot; to call ReadWordDocument in GetWordPlainText class
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
       /// &lt;summary&gt;
       /// Get Plain Text from Word file
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
       private void btnGetPlainText_Click(object sender, EventArgs e)
       {
           try
           {
               getWordPlainText = new GetWordPlainText(tbxFile.Text);
               this.rtbText.Clear();
               this.rtbText.Text = getWordPlainText.ReadWordDocument();


               // After read text in word document successfully&iuml;&frac14;&#338;make &quot;save as text&quot; button to be enabled.
               this.btnSaveas.Enabled = true;
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message, &quot;Error&quot;, MessageBoxButtons.OK, MessageBoxIcon.Warning);
           }
           finally
           {
               if (getWordPlainText != null)
               {
                   getWordPlainText.Dispose();
               }
           }
       }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style=""></span></p>
<p class="MsoNormal" style=""><span style="">Step7. Click &quot;Save as Text&quot; to save the text to text file.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
      /// &lt;summary&gt;
      ///  Save the text to text file
      /// &lt;/summary&gt;
      /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
      /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
      private void btnSaveas_Click(object sender, EventArgs e)
      {
          using (SaveFileDialog savefileDialog = new SaveFileDialog())
          {
              savefileDialog.Filter = &quot;txt document(*.txt)|*.txt&quot;;


              // default file name extension
              savefileDialog.DefaultExt = &quot;.txt&quot;;


              // Retore the directory before closing
              savefileDialog.RestoreDirectory = true;
              if (savefileDialog.ShowDialog() == DialogResult.OK)
              {
                  string filename = savefileDialog.FileName;
                  rtbText.SaveFile(filename, RichTextBoxStreamType.PlainText);
                  MessageBox.Show(&quot;Save Text file successfully, the file path is&iuml;&frac14;&#353; &quot; &#43; filename);
              }
          }
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style=""></span></p>
<p class="MsoNormal" style=""><span style=""><a href="http://msdn.microsoft.com/en-us/library/bb448854.aspx" target="_top">Open XML SDK 2.0</a></span><span style="">
</span></p>
<p class="MsoNormal" style=""><span style=""><a href="http://msdn.microsoft.com/en-us/library/cc850833.aspx">Word Processing</a>
</span></p>
<p class="MsoNormal" style=""><span style=""><a href="http://msdn.microsoft.com/en-us/office/bb265236">Open XML Developer Center</a>
</span></p>
<p class="MsoNormal" style=""><span style=""></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
