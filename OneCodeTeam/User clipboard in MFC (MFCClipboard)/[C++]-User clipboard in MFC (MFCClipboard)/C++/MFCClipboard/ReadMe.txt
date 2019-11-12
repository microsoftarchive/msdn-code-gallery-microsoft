========================================================================
    CONSOLE APPLICATION : MFCClipboard Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

The clipboard is a set of functions and messages that enable applications to 
transfer data. Because all applications have access to the clipboard, data 
can be easily transferred between applications or within an application.

A user typically carries out clipboard operations by choosing commands from 
an application's Edit menu. Following is a brief description of the standard 
clipboard commands.

Cut: Places a copy of the current selection on the clipboard and deletes the 
selection from the document. The previous content of the clipboard is 
destroyed.

Copy: Places a copy of the current selection on the clipboard. The document 
remains unchanged. The previous content of the clipboard is destroyed.

Paste: Replaces the current selection with the content of the clipboard. 
The content of the clipboard is not changed.

Delete: Deletes the current selection from the document. The content of the 
clipboard is not changed. This command does not involve the clipboard, but 
it should appear with the clipboard commands on the Edit menu.

The sample demostrates how to copy and paste simple text programmatically.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

Copy:

1. Get text from edit control.

2. Open and empty clipboard. (OpenClipboard, EmptyClipboard)

3. Create global buffer. (GlobalAlloc)

4. Lock the buffer. (GlobalLock)

5. Copy text to the buffer. (strcpy)

6. Unlock the buffer. (GlobalUnlock)

7. Set buffer data to clipboard. (SetClipboardData)

8. Close clipboard. (CloseClipboard)

Cut:

1. Copy

2. Clear the text.

Paste:

1. Check and open clipboard. (IsClipboardFormatAvailable, OpenClipboard)

2. Get clipboard data. (GetClipboardData)

3. Set the data into edit control.

4. Close clipboard. (CloseClipboard)


/////////////////////////////////////////////////////////////////////////////
References:

Clipboard
http://msdn.microsoft.com/en-us/library/ms648709(VS.85).aspx

Using the Clipboard, Part I : Transferring Simple Text
http://www.codeproject.com/KB/clipboard/archerclipboard1.aspx


/////////////////////////////////////////////////////////////////////////////
