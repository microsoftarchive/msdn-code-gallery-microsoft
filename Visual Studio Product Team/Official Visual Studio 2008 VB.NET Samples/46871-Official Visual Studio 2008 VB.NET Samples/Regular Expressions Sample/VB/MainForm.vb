' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Net

Public Class MainForm

    Private images As System.Collections.Generic.Dictionary(Of String, ImageAttributes)
    Protected viewerForm As ImageViewer
    Private statusForm As Status
    Public domain As String
    Public urlEntered As String
    Private requestScrape As HttpWebRequest
    Private responseScrape As HttpWebResponse

    ''' <summary>
    ''' Handles the "Find" button click event on the "RegEx Tester" tab. This routine
    ''' finds matches in the source text and displays the group and capture values in 
    ''' the results TextBox.
    ''' </summary>
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click
        Dim expression As Regex
        Dim options As RegexOptions = GetRegexOptions()

        Try
            expression = New Regex(txtRegEx.Text, options)
        Catch exp As Exception
            MsgBox("An error was encountered when attempting to parse the " & _
                "source text. This can be caused by an invalid regex pattern." & _
                "  Check your expression and try again." & vbCrLf & exp.Message, _
                MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End Try

        ' Get the collection of matches resulting from applying the pattern to the
        ' source text.
        Dim matches As MatchCollection = expression.Matches(txtSource.Text)

        ' Display the number of matches and clear any existing results.
        lblResultCount.Text = matches.Count.ToString & " matches"
        txtResults.Clear()

        ' Iterate through the collection of matches and, based on UI settings, 
        ' display the values of the groups and/or captures.
        For Each m As Match In matches
            AppendResults("'" & m.Value & "' at index " & m.Index)

            If m.Groups.Count > 1 And chkShowGroups.Checked Then
                Dim g As Group
                ' Skip the 0th group, which is the entire Match object.
                For i As Integer = 1 To m.Groups.Count - 1
                    ' Get a reference to the corresponding group.
                    g = m.Groups(i)

                    AppendResults(String.Format("   group({0}) = '{1}'", i, g.Value))

                    If chkShowCaptures.Checked = True Then
                        ' Get the capture collection for this group.                
                        Dim captures As CaptureCollection = g.Captures
                        ' Display information on each capture.
                        For Each c As Capture In captures
                            AppendResults(String.Format("      " & _
                                "capture '{0}' at index {1}", c.Value, c.Index))
                        Next
                    End If
                Next
            End If
        Next
    End Sub

    ' This routine handles the "Find the Number" button Click event. It finds the 
    ' first number in a character string and displays the number and its starting 
    ' position in the string.
    Private Sub btnFindNumber_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFindNumber.Click
        ' Create an instance of RegEx and pass in the pattern, which will find one 
        ' or more digits.
        Dim expression As New Regex("\d+")
        ' Call Match, passing in the source text.
        Dim m As Match = expression.Match(txtFindNumber.Text)

        ' If a match is found, show the results. If not, display an "error" message.
        If m.Success Then
            lblResults.Text = "RegEx found " & m.Value & " at position " & m.Index.ToString
        Else
            lblResults.Text = "You didn't enter a string containing a number."
        End If
    End Sub

    ' Handles the Click event for the "Replace" button. Replaces text in the source
    ' with the replacement text when a match is found.
    Private Sub btnReplace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReplace.Click
        Try
            txtResults.Text = Regex.Replace(txtSource.Text, txtRegEx.Text, _
                txtReplace.Text, GetRegexOptions())
        Catch exp As Exception
            MsgBox(exp.ToString, MsgBoxStyle.Critical, Me.Text)
        End Try
    End Sub

    ' Handles the "Scrape" button click event.
    Private Sub btnScrape_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScrape.Click
        Dim httpSource As String

        If IsUrlValid() Then
            ' Get a String containing the Web page's source code. See the two custom
            ' functions used here for further comments.
            Try
                httpSource = ConvertStreamToString(GetHttpStream(txtURL.Text))
            Catch exp As Exception
                ' Set text to exp.Message to show the custom error message set in the 
                ' function that was called.
                MsgBox(exp.Message, MsgBoxStyle.Critical, Me.Text)
                Exit Sub
            End Try

            Dim regExPattern As String

            ' Assign a variable to the full Url entered by the user. 
            urlEntered = Trim(txtURL.Text)
            ' Get the Domain name for use in various places. It's best to set the 
            ' value in the Click event so that the items displayed in either ListView
            ' control are always associated with the correct Domain name. If this
            ' value is assigned elsewhere, the user can change the Domain name and 
            ' then double-click an item in the ListView, which could cause an invalid 
            ' Url for the item.
            Dim astrDomain() As String = Regex.Split(urlEntered, "/")
            ' The first element (0) is "http:" and the 3rd element is the actual
            ' Domain name.
            domain = astrDomain(0) & "//" & astrDomain(2)

            If optLinks.Checked Then
                ShowStatusIndicators("Connecting to Web page to screen scrape " & _
                    "the links. Please stand by...")

                lvwLinks.Items.Clear()

                ' The regular expression used here to parse an HTML anchor, e.g.,
                ' <a href="http://www.microsoft.com/net">Microsoft</a> is explained 
                ' as follows:
                '   <a          The beginning of the HTML anchor
                '   \s+         One or more white space characters
                '   href        Continuing with exact text in HTML anchor
                '   \s*         Zero or more white space characters
                '   =           Continuing with exact text in HTML anchor
                '   \s*         Zero or more white space characters
                '   ""?         Zero or none quotation mark (escaped)
                '   (           Start of group defining a substring: The anchor URL.
                '   [^"" >]+    One or more matches of any character except those 
                '               in brackets.
                '   )           End of first group defining a substring
                '   ""?         Zero or none quotation mark (escaped)
                '   >           Continuing with exact text in HTML anchor
                '   (.+)        A group matching any character: The anchor text.
                '   </a>        Ending exact text of HTML anchor               
                '
                ' CAUTION: If you want to experiment with these patterns in the 
                ' RegEx Tester, make sure you un-escape the double quotes.
                regExPattern = "<a\s+href\s*=\s*""?([^"" >]+)""?>(.+)</a>"
            Else
                ShowStatusIndicators("Connecting to Web page to screen scrape the " & _
                    "images. Please stand by...")

                images = New System.Collections.Generic.Dictionary(Of String, ImageAttributes)
                lvwImages.Items.Clear()

                ' The regular expression to scrape images is conceptually similar to 
                ' the pattern for scraping HTML anchors. However, in this case the 
                ' pattern is conciderably more complex because we are capturing up to
                ' four different attributes which can appear in any order.
                '
                ' To keep the length and complexity of the regular expression used 
                ' here within reason, the following assumptions are made about the 
                ' HTML <img> tags being processed on any given Web page:
                '
                '   1. The src attribute is always present. It is the only required 
                '      attribute.
                '   2. The src attribute precedes width and height. If not, width and
                '      height are not grabbed.
                '   3. The alt attribute follows width and height. If not, alt is not 
                '      grabbed.
                '   4. Width and height can follow src in any order relative to each 
                '      other. The pattern covers both options.
                '
                ' This ensures that all images on the page are scaped. It means, 
                ' however, that some of the other attribute data may not appear.
                '
                ' Some of the key phrases used in this pattern are:
                ' 
                '   [^>]+       Match any characters except >. This is how you can 
                '               move to the next attribute you are interested in.
                '   (?:         Start a non-capturing group. This is used with a
                '               closing )? to create an optional group (zero or one
                '               match). This is how you make all attributes optional
                '               except src.
                '   |           Used with width and height as an Or expression. Notice
                '               that in the first pair width comes first, and in the 
                '               second pair, the order is reversed.
                regExPattern = "<img[^>]+(src)\s*=\s*""?([^ "">]+)""?(?:[^>]+(width|height)\s*=\s*""?([^ "">]+)""?\s+(height|width)\s*=\s*""?([^ "">]+)""?)?(?:[^>]+(alt)\s*=\s*""?([^"">]+)""?)?"
            End If

            Dim re As New Regex(regExPattern, RegexOptions.IgnoreCase)
            Dim m As Match = re.Match(httpSource)

            ' Process the HTML source code. Because the source is a long string, 
            ' instead of using Matches method use the more efficient Match(), which
            ' only returns one match at a time. The Success property determines
            ' whether another match exists. NextMatch() causes the iteration.
            While m.Success
                If optImages.Checked Then

                    Dim width As String
                    Dim height As String
                    ' Because the pattern gives optional ordering for the width and 
                    ' height attributes, determine which attribute was listed first
                    ' in order, and then assign the proper Group item value.
                    If m.Groups(3).Value.ToLower = "width" Then
                        width = m.Groups(4).Value
                        height = m.Groups(6).Value
                    Else ' The height attribute came first
                        width = m.Groups(6).Value
                        height = m.Groups(4).Value
                    End If

                    ' Call AddImage to add an instance of the custom ImageAttributes 
                    ' object to a Hashtable. See AddImage for further comments on why
                    ' a Hashtable is used.
                    AddImage(New ImageAttributes(m.Groups(2).Value, _
                        m.Groups(8).Value, height, width))
                Else
                    ' Create a ListViewItem object and set the first column's 
                    ' text ("src").
                    Dim lvi As New ListViewItem()
                    lvi.Text = m.Groups(1).Value
                    lvwLinks.Items.Add(lvi)
                End If

                ' Continue the loop to the next match.
                m = m.NextMatch()
            End While

            HideStatusIndicators()

            ' Display controls appropriate the results.
            If optImages.Checked Then
                If images IsNot Nothing Then
                    FillImagesListView()
                Else
                    MsgBox("No images were found whose attributes matched the " & _
                        "regular expression.", MsgBoxStyle.Information, Me.Text)
                End If
            Else
                If lvwLinks.Items.Count = 0 Then
                    MsgBox("No links were found whose Url matched the regular " & _
                    "expression.", MsgBoxStyle.Information, Me.Text)
                End If
            End If
        End If
    End Sub

    ' Handles the "Split" button click event. This routine splits the source text
    ' into a string array instead of a MatchCollection (as in the "Find" button).
    ' RegEx.Split is similar to String.Split except that it defines the delimiter
    ' by using a regular expression rather than a single character.
    Private Sub btnSplit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSplit.Click
        ' Clear existing results and then split the source text.
        txtResults.Clear()

        ' Here we are calling the shared Split method, without the use of a 
        ' RegEx instance.  A good split pattern to try is \s*,\s*, which creates 
        ' an array of any characters separated by a comma. In this case, 
        ' RegEx.Split(txtSource.Text, "\s*,\s*") would produce the same results as:
        '
        '       txtRegEx.Text.Split(New Char() {CChar(",")})
        '       Microsoft.VisualBasic.Split(txtRegEx.Text, ",")
        '
        ' You can further modify the regular expression to discard any empty 
        ' elements in the resulting string array: \s*[,]+\s*
        Dim results() As String
        Try
            results = Regex.Split(txtSource.Text, txtRegEx.Text, _
                GetRegexOptions())
        Catch exp As Exception
            MsgBox("An error was encountered when attempting to parse the " & _
                "source text. This can be caused by an invalid regex pattern." & _
                "  Check your expression and try again." & vbCrLf & exp.Message, _
                MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End Try

        ' To be split a string array of at least two elements must be created.
        If results.Length > 1 Then
            AppendResults("The source text was split into " & _
                results.Length & " items.")

            For Each s As String In results
                AppendResults(s)
            Next
        Else
            AppendResults("The source text could not be split. " & _
                "Check your regular expression pattern and try again.")
        End If
    End Sub

    ' Handles the Validate! button click event on the "Simple Samples" tab.
    Private Sub btnValidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnValidate.Click
        Dim isValid As Boolean = True

        ' When performing validation, a couple of pattern elements are normally
        ' used all of the time:
        '
        '   ^ and $     The source string can only contain what is in between, i.e.,
        ' a date. It's a good idea to wrap validation patterns in
        ' these characters or else it will merely match the pattern
        ' whereever it appears. For example, if you do not, this would 
        ' pass as a valid date in this application: "kjdi12-25-2000xpL" 
        '
        ' ^ and $     
        ' The ^ character matches the position at the beginning of the input string. 
        ' The $ character matches the position at the end of the input string. 
        ' It is a good idea to wrap validation patterns in these characters so the 
        ' string will validate only if the entire string matches the pattern. 
        ' Otherwise, a match will be reported if any substring of the string matches.
        ' For example, consider a regular expression that validates dates, 
        ' so "12-25-2000" validates. If the regular expression does not begin and end 
        ' with a ^ and $, the string "kjdi12-25-2000xpL" will also validate.

        '   \s*         At the beginning and end of the string, this indicates
        '               that white space is acceptable.
        '
        ' The following pattern checks whether the input string is a valid zip
        ' code in the format ddddd or ddddd-dddd, where d is any digit 0-9. Key 
        ' pattern elements used are:
        '
        '   \d          Any digit (0-9).
        '   |           A pipe denotes that the Zip code can either be 5 digits
        '               or 5 digits followed by a dash and four digits.
        '
        If Not Regex.IsMatch(txtZip.Text, "^\s*(\d{5}|(\d{5}-\d{4}))\s*$") Then
            txtZip.ForeColor = Color.Red
            isValid = False
        Else
            txtZip.ForeColor = Color.Black
        End If

        ' The following pattern checks whether the input string is a date in the 
        ' format mm-dd-yy or mm-dd-yyyy. Key pattern elements used are:
        '
        '   \d{1,2}     Month and day numbers can have 1 or 2 digits. The use of
        '               (\d{4}|\d{2}) means the year can have 2 or 4 digits.
        '   (/|-)       Either the slash or the dash are valid date separators.
        '   \1          The separator used for the day and year must be the same
        '               as the separator used for month and day. The 1 refers to the
        '               first numbered group, defined by parentheses, e.g, (/|-).
        '   
        ' You could improve on this pattern by ensuring that digits do not start with
        ' a zero and that they are in a valid numerical range.
        '
        If Not Regex.IsMatch(txtDate.Text, _
            "^\s*\d{1,2}(/|-)\d{1,2}\1(\d{4}|\d{2})\s*$") Then

            txtDate.ForeColor = Color.Red
            isValid = False
        Else
            txtDate.ForeColor = Color.Black
        End If

        ' The following pattern checks whether the input string is a valid email 
        ' address in the form "name@domain.com". Actually, it does not have to be a 
        ' ".com" address. Any combination of letters following the last period are 
        ' fine. Also, the email name can have a dash or be separated by one or more 
        ' periods. The Domain name can also have multiple words separated by periods. 

        ' [\w-]+    
        ' One or more matches of any character (a-z, A-Z, 0-9, and underscore) or dash. On either side of the @ character this ensures the address is in the form name@domainname.
        ' \.              
        ' An escaped period. (Without the backslash, a period matches any single character except the newline character.) Using this ensures there is at least one period in the domain name.
        ' *?            
        ' A non-greedy, or minimal, match of zero or more matches of the preceding expression.
        ' ([\w-]+\.)*?    
        ' Combination of the three preceding expressions:
        ' Zero or more non-greedy matches of the expression one or more matches of any character (a-z, A-Z, 0-9, and underscore) or dash, followed by only one period.

        If Not Regex.IsMatch(txtEmail.Text, _
            "^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$") Then

            txtEmail.ForeColor = Color.Red
            isValid = False
        Else
            txtEmail.ForeColor = Color.Black
        End If

        If isValid Then
            lblValid.Visible = True
        Else
            lblValid.Visible = False
        End If
    End Sub

    ' This routine handles the DoubleClick event for the "Images" ListView.
    ' Double-clicking an image opens frmImageViewer, requests the image as a Stream
    ' from the Web server, and then displays it in a PictureBox.
    Private Sub lvImages_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwImages.DoubleClick

        ' Wrap all code in the validation check to ensure the user has not changed
        ' the Url to an invalid value before double-clicking an item.
        If IsUrlValid() Then
            Dim imageSource As String = lvwImages.SelectedItems(0).Text

            ShowStatusIndicators("Connecting to Web page to retrieve the " & _
                "selected image. Please stand by...")

            If IsNothing(viewerForm) Then
                viewerForm = New ImageViewer()
            End If

            ' If the image path clicked by the user is not an absolute path, correct
            ' it and then Show the ImageViewer Form.
            If Microsoft.VisualBasic.Left(imageSource, 7) <> "http://" Then
                imageSource = MakeRelativeUrlAbsolute(imageSource)
            End If

            viewerForm.Show(imageSource)

            HideStatusIndicators()
        End If

    End Sub

    ' This routine handles the DoubleClick event for the "Links" ListView. 
    ' Double-clicking a link starts a new instance of Internet Explorer and 
    ' navigates to the requested page.
    Private Sub lvLinks_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwLinks.DoubleClick

        ' Wrap all code in the validation check to ensure the user has not changed
        ' the Url to an invalid value before double-clicking an item.
        If IsUrlValid() Then

            Dim clickedUrl As String = lvwLinks.SelectedItems(0).Text

            ShowStatusIndicators("Starting Internet Explorer and connecting to " & _
                "the selected Web page. Please stand by...")

            ' If the path to the page clicked by the user is not an absolute path, 
            ' correct it and then start Internet Explorer, navigating to the page.
            If Microsoft.VisualBasic.Left(clickedUrl, 7) <> "http://" Then
                clickedUrl = MakeRelativeUrlAbsolute(clickedUrl)
            End If

            ' It is either a root-relative or relative path. So make the relative 
            ' Url into an absolute Url.
            Process.Start("IExplore.exe", clickedUrl)

            HideStatusIndicators()

        End If

    End Sub

    ' This routine handles the CheckedChanged event for the RadioButton controls on 
    ' the "Screen Scrape" tab. The ListView controls are swapped out depending on 
    ' which option is checked.
    Private Sub RadioButtons_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optImages.CheckedChanged, optLinks.CheckedChanged
        If optLinks.Checked Then
            lvwLinks.Visible = True
            lvwImages.Visible = False
        Else
            lvwImages.Visible = True
            lvwLinks.Visible = False
        End If
    End Sub

    ' This routine adds an ImageAttributes object to the HashTable if it has not
    ' already been added. The "Src" attribute is used as the key for lookups. If
    ' the Src key already exists, the object is not added. This prevents identical
    ' images from appearing in the list more than once.
    Protected Sub AddImage(ByVal imgAttr As ImageAttributes)
        If Not images.ContainsKey(imgAttr.Src) Then
            images.Add(imgAttr.Src, imgAttr)
        End If
    End Sub

    ' This is a helper routine for appending text results.
    Sub AppendResults(ByVal msg As String)
        txtResults.AppendText(msg & ControlChars.CrLf)
    End Sub

    ' This function reads a Stream returned by an HttpWebResponse object and 
    ' converts it to a String for RegEx processing.
    Function ConvertStreamToString(ByVal stmSource As Stream) As String
        Dim sr As StreamReader = Nothing

        If Not IsNothing(stmSource) Then
            Try
                sr = New StreamReader(stmSource)
                ' Read and return the entire contents of the stream.
                Return sr.ReadToEnd
            Catch exp As Exception
                ' Don't show a MsgBox. Simply forward the custom error message 
                ' from GetHttpStream().
                Throw New Exception()
            Finally
                ' Clean up both the Stream and the StreamReader.
                responseScrape.Close()
                sr.Close()
            End Try
        End If
        Return Nothing
    End Function

    ' This routine iterates through a HashTable and adds a ListViewItem with
    ' ListViewItem.SubItems for each of the custom ImageAttributes objects in
    ' the HashTable.
    Protected Sub FillImagesListView()
        For Each source As String In images.Keys
            ' Create a ListViewItem object and set the first column's text.
            Dim lvi As New ListViewItem()
            lvi.Text = source

            ' Set the text in the remaining columns and add the ListViewItem object
            ' to the ListView.
            Dim imgAttr As ImageAttributes = CType(images(source), ImageAttributes)
            With lvi.SubItems
                .Add(imgAttr.Alt)
                .Add(imgAttr.Height)
                .Add(imgAttr.Width)
            End With

            ' Add the ListViewItem to the ListView's Items collection.
            lvwImages.Items.Add(lvi)
        Next
    End Sub

    ' This function uses .NET classes that derive from System.Net.WebRequest to 
    ' retrieve an HTTP response Stream that becomes the RegEx parsing source or the 
    ' image to be displayed when called by frmImageViewer.Show().
    Function GetHttpStream(ByVal url As String) As Stream
        ' Create the request using the WebRequestFactory.
        requestScrape = CType(WebRequest.Create(url), HttpWebRequest)
        With requestScrape
            .UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0b; Windows NT 5.1)"
            .Method = "GET"
            .Timeout = 10000
        End With

        Try
            ' Return the response stream.
            responseScrape = CType(requestScrape.GetResponse(), HttpWebResponse)
            Return responseScrape.GetResponseStream()
        Catch exp As Exception
            ' As the error is most likely caused by a mistyped Url or not having
            ' a connection to the Internet, create a custom error message that
            ' is forwarded back to the calling function.
            Throw New Exception("There was an error retrieving the Web page " & _
                "you requested. Please check the Url and your connection to " & _
                "the Internet, and try again.")
        End Try
    End Function

    ' This is a helper routine to retrieve various regular expression pattern 
    ' matching options.
    Function GetRegexOptions() As RegexOptions
        ' Using any of the RegExOptions is the same as placing the entire regular
        ' expression in parentheses (i.e., making the entire pattern its own group)
        ' and then prefacing it inside the left parenthesis with the option 
        ' character. If you want finer control over an option --i.e., within a 
        ' particular group--create a group within the pattern and use the same
        ' syntax. In this case you would not want to use one of the RegexOptions
        ' because these are applied to the entire pattern. To turn off an option 
        ' simply negate it (e.g., to turn case sensitivity off you would type
        ' ?-i: inside the left parenthesis of a group).
        '
        ' All Options are off by default.
        '
        ' The IgnoreCase enum turns case sensitivity on/off for the entire pattern.
        ' Using this enum is the same as typing (?i:OriginalPattern) using only 
        ' regular expression syntax.
        If chkIgnoreCase.Checked Then GetRegexOptions = RegexOptions.IgnoreCase

        ' The Singleline enum changes the behavior of the . (dot) character so that
        ' it now matches any character (instead of its default behavior of any 
        ' character except the newline character, \r or \n). Using this enum is the 
        ' same as typing (?s:OriginalPattern) using only regular expression syntax.
        ' 
        ' Note also that multiple RegExOptions can be used when separated by the
        ' bitwise Or operator. (Alternatively, you could enable multiple options
        ' using only regular expression syntax by placing options together after the
        ' ? character. For example, to turn on Singleline mode and ignore case, you 
        ' would type (?si:OriginalPattern).
        If chkSingleLine.Checked Then
            GetRegexOptions = GetRegexOptions Or RegexOptions.Singleline
        End If

        ' The Multiline enum changes the behavior of the ^ and $ characters so that
        ' they now match the beginning and end of a line instead of the beginning
        ' and of an entire string. Using this enum is the same as typing 
        ' (?m:OriginalPattern) using only regular expression syntax.
        '
        ' It may seem confusing that you can enable both Singleline and Multiline
        ' modes, but the confusion likely stems from the misleading names given to 
        ' these options. If you disregard their name and consider what they actually
        ' effect, there is no conflict.
        If chkMultiline.Checked Then
            GetRegexOptions = GetRegexOptions Or RegexOptions.Multiline
        End If
    End Function

    ' This routine turns off the status indicators enabled by ShowStatusIndicators.
    Private Sub HideStatusIndicators()
        statusForm.Hide()
        Me.Cursor = Cursors.Default
    End Sub

    ' This function takes a relative Url and makes it absolute. It is a helper 
    ' for the DoubleClick event handlers on the "Screen Scrape" tab. The "href" or 
    ' "url" attributes can contain an absolute path, a root-relative path, or a 
    ' relative path. If the path to the clicked item is not absolute, this function
    ' makes it absolute by prefacing it with the Domain name and a slash, if needed.
    Public Function MakeRelativeUrlAbsolute(ByVal relativeUrl As String) As String

        ' If it is a root-relative path (has a leading "/"), then it needs to be 
        ' prefaced by the Domain name. If it is a relative Url it needs to be 
        ' prefaced with the full Url as entered by the user. 

        ' Is it a root-relative path?
        If Microsoft.VisualBasic.Left(relativeUrl, 1) = "/" Then
            ' Preface the root-relative path with the Domain name.
            Return domain & relativeUrl
        ElseIf Microsoft.VisualBasic.Left(relativeUrl, 3) = "../" Then
            ' Remove the dots and preface the root-relative path with the 
            ' Domain name.
            Return domain & _
                Microsoft.VisualBasic.Replace(relativeUrl, "../", "/")
        Else ' It is a relative path.
            ' Check to see if the Url entered by the user has a trailing "/". If not, 
            ' add one.
            If Microsoft.VisualBasic.Right(urlEntered, 1) = "/" Then
                Return urlEntered & relativeUrl
            Else
                Return urlEntered & "/" & relativeUrl
            End If
        End If
    End Function

    ' This routine provides user feedback by showing a small status form with a 
    ' message and setting the WaitCursor to indicate a task is being performed.
    Private Sub ShowStatusIndicators(ByVal message As String)
        statusForm = New Status()
        statusForm.Show(message)
        Me.Cursor = Cursors.WaitCursor
    End Sub

    ' This function checks whether the Url entered by the user starts with 
    ' http://.
    Function IsUrlValid() As Boolean
        If Microsoft.VisualBasic.Left(Trim(txtURL.Text), 7) <> "http://" Then
            MsgBox("The Url must begin with http://.")
            Return False
        End If

        Return True
    End Function

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
