' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System.IO
Imports System.Xml
Imports System.Linq
Imports System.Xml.Linq
Imports System.Net

Module Module1
    Const feedUrl As String = "http://+:8086/VBfeeds/"


    Sub Main()
        Dim listener As New HttpListener

        listener.Prefixes.Add(feedUrl)
        listener.Start()

        'Open a browser pointing at the feeds being served.
        System.Diagnostics.Process.Start("iexplore.exe", "http://localhost:8086/VBfeeds/")

        'Serve requests.
        While True
            Dim context As HttpListenerContext = listener.GetContext()
            Dim body As XElement = GetReplyBody()
            context.Response.ContentType = "text/xml"

            Using writer As XmlWriter = New XmlTextWriter(context.Response.OutputStream, System.Text.Encoding.UTF8)
                body.WriteTo(writer)
            End Using
        End While

    End Sub

    Function GetReplyBody() As XElement
        Dim feeds() As String = {"http://blogs.msdn.com/vbteam/rss.aspx?Tags=Amanda+Silver&AndTags=1", _
                                 "http://blogs.msdn.com/vbteam/rss.aspx?Tags=Beth+Massi&AndTags=1", _
                                 "http://blogs.msdn.com/vbteam/rss.aspx?Tags=Matt+Gertz&AndTags=1", _
                                 "http://blogs.msdn.com/vbteam/rss.aspx?Tags=LINQ_2F00_VB9&AndTags=1", _
                                 "http://blogs.msdn.com/vbteam/rss.aspx?Tags=VB6_5F00_Migration_2F00_Interop&AndTags=1", _
                                 "http://www.panopticoncentral.net/Rss.aspx", _
                                 "http://blogs.msdn.com/vsdata/rss.xml", _
                                 "http://blogs.msdn.com/vbteam/rss.aspx?Tags=IDE&AndTags=1"}

        Return <rss version="2.0">
                   <channel>
                       <title>VB Genius</title>
                       <link>http://+:8086/VBfeeds/</link>
                       <description>VB Team Members</description>
                       <generator>XLinq-based RSS aggregator using VB Only Super Cool XML Literals Feature</generator>
                       <%= From f In feeds Let feed = XDocument.Load(f) Select feeditem = feed.Root.Element("channel").Elements("item") From si In feeditem Select si %>
                   </channel>
               </rss>
    End Function
End Module

