# User clipboard in MFC (MFCClipboard)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- MFC
- Windows General
## Topics
- Clipboad
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CONSOLE APPLICATION : MFCClipboard Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
The clipboard is a set of functions and messages that enable applications to <br>
transfer data. Because all applications have access to the clipboard, data <br>
can be easily transferred between applications or within an application.<br>
<br>
A user typically carries out clipboard operations by choosing commands from <br>
an application's Edit menu. Following is a brief description of the standard <br>
clipboard commands.<br>
<br>
Cut: Places a copy of the current selection on the clipboard and deletes the <br>
selection from the document. The previous content of the clipboard is <br>
destroyed.<br>
<br>
Copy: Places a copy of the current selection on the clipboard. The document <br>
remains unchanged. The previous content of the clipboard is destroyed.<br>
<br>
Paste: Replaces the current selection with the content of the clipboard. <br>
The content of the clipboard is not changed.<br>
<br>
Delete: Deletes the current selection from the document. The content of the <br>
clipboard is not changed. This command does not involve the clipboard, but <br>
it should appear with the clipboard commands on the Edit menu.<br>
<br>
The sample demostrates how to copy and paste simple text programmatically.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
Copy:<br>
<br>
1. Get text from edit control.<br>
<br>
2. Open and empty clipboard. (OpenClipboard, EmptyClipboard)<br>
<br>
3. Create global buffer. (GlobalAlloc)<br>
<br>
4. Lock the buffer. (GlobalLock)<br>
<br>
5. Copy text to the buffer. (strcpy)<br>
<br>
6. Unlock the buffer. (GlobalUnlock)<br>
<br>
7. Set buffer data to clipboard. (SetClipboardData)<br>
<br>
8. Close clipboard. (CloseClipboard)<br>
<br>
Cut:<br>
<br>
1. Copy<br>
<br>
2. Clear the text.<br>
<br>
Paste:<br>
<br>
1. Check and open clipboard. (IsClipboardFormatAvailable, OpenClipboard)<br>
<br>
2. Get clipboard data. (GetClipboardData)<br>
<br>
3. Set the data into edit control.<br>
<br>
4. Close clipboard. (CloseClipboard)<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
Clipboard<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms648709(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms648709(VS.85).aspx</a><br>
<br>
Using the Clipboard, Part I : Transferring Simple Text<br>
<a target="_blank" href="http://www.codeproject.com/KB/clipboard/archerclipboard1.aspx">http://www.codeproject.com/KB/clipboard/archerclipboard1.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
