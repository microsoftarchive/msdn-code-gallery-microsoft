# PowerPoint 2010: Insert, Move, Get Section Counts  Using PPT.WorkWithSections
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- PowerPoint 2010
- Office 2010
## Topics
- Office 2010 101 code samples
- inserting sections
## Updated
- 08/05/2011
## Description

<h1>Introduction</h1>
<p><span style="font-size:small">This sample shows how to insert sections, get counts and names of sections, and move sections in a Microsoft PowerPoint 2010 presentation.</span></p>
<p><span style="font-size:small">This code snippet is part of the Office 2010 101 code samples project. This sample, along with others, is offered here to incorporate directly in your code.</span></p>
<p><span style="font-size:small">Each code sample consists of approximately 5 to 50 lines of code demonstrating a distinct feature or feature set, in either VBA or both VB and C# (created in Visual Studio 2010). Each sample includes comments describing the
 sample, and setup code so that you can run the code with expected results or the comments will explain how to set up the environment so that the sample code runs.)</span></p>
<p><span style="font-size:small">Microsoft&reg; Office 2010 gives you the tools needed to create powerful applications. The Microsoft Visual Basic for Applications (VBA) code samples can assist you in creating your own applications that perform specific functions
 or as a starting point to create more complex solutions.</span></p>
<h1><span>Building the Sample</span></h1>
<p><span style="font-size:small">Copy this code into a module in a new presentation. Display the VBA window side-by-side with the PowerPoint window and press F8 and then Shift&#43;F8 to single step through this code for the most effective use of this demonstration.</span></p>
<p><span style="font-size:20px; font-weight:bold">Description</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Sub DemoSections()
    ' Use this procedure to demonstrate working with sections,
    ' focusing on new features in PowerPoint 2010.
   
    ' Copy this code into a module in a new presentation.
    ' Display the VBA window side-by-side with the PowerPoint window
    ' and press F8 and then Shift&#43;F8 to single step through this
    ' code for the most effective use of this demonstration.
    SetupDemo
   
    Dim i As Integer
   
    ' Add three sections:
    With ActivePresentation.SectionProperties
        ' Use the AddBeforeSlide method to insert a
        ' section in a position relative to
        ' a particular slide in the presentation:
        .AddBeforeSlide 2, &quot;Test Section 1&quot;
        .AddBeforeSlide 5, &quot;Test Section 2&quot;
        .AddBeforeSlide 9, &quot;Test Section 3&quot;
       
        ' Use the AddSection method to insert a section
        ' in relation to other sections, as opposed to
        ' in relation to a specific slide:
        .AddSection 1, &quot;Intro Section&quot;
       
        ' Use the Count property to return the total number of sections.
        ' Use the Name property to get the name of the section.
        ' Use the SlidesCount method to return the number of slides in a
        ' given section:
        For i = 1 To .Count
            Debug.Print &quot;Section &quot; &amp; i &amp; &quot;(&quot; &amp; .Name(i) &amp; &quot;) contains &quot; &amp; _
             .SlidesCount(i) &amp; &quot; slide(s);&quot;;
                       
            ' The FirstSlide method returns the index of the first slide
            ' in the section. If the section has no slides, it returns -1.
            Debug.Print &quot; The first slide is : &quot; &amp; .FirstSlide(i)
           
            ' Use the Rename method to change the name of a section:
            .Rename i, &quot;New Name &quot; &amp; i
        Next i
       
        ' Use the Move method to move a section, along with its slides:
        ' This code moves the second section to the end:
        .Move 2, .Count
    End With
   
    ' Use the SectionIndex property of a slide to
    ' determine what section it's in:
    With ActivePresentation
        Dim sld As Slide
        For Each sld In ActivePresentation.Slides
            Debug.Print &quot;Slide &quot; &amp; sld.SlideIndex &amp; &quot; is in section &quot; &amp; sld.sectionIndex
        Next sld
   
        ' Use the MoveToSectionStart method to move
        ' a slide to the beginning of the section.
        ' This code moves the final slide to the beginning of
        ' the first section:
        Set sld = .Slides(.Slides.Count)
        sld.MoveToSectionStart 1
       
        ' You can expand or contract any section, as well.
        ' Use the IsSectionExpanded property and the ExpandSection
        ' method to do the work.
       
        ActiveWindow.ViewType = ppViewSlideSorter
       
        ' First, contract every other section:
        For i = 1 To .SectionProperties.Count
            ' Contract the even-numbered sections:
            If i Mod 2 = 0 Then
                ActiveWindow.ExpandSection i, False
            End If
        Next i
       
        ' Now toggle the state of the expansion for all sections:
        Dim isExpanded As Boolean
        For i = 1 To .SectionProperties.Count
            isExpanded = ActiveWindow.IsSectionExpanded(i)
            ActiveWindow.ExpandSection i, Not isExpanded
        Next i
    End With
   
    With ActivePresentation.SectionProperties
        ' Unlike in the user interface, there's no simple way to remove
        ' all the sections, so you'll need to write code like this
        ' to do it:
        For i = .Count To 1 Step -1
            ' Pass True in the second parameter to delete the
            ' slides in the section:
            .Delete i, False
        Next i
       
        ' Now delete all the slides except the first one, to reset
        ' back to the way the demonstration started:
        CleanupDemo
    End With
       
   
End Sub

Private Sub SetupDemo()
    ' Set the title of the presentation, and add 10 slides.
    Dim i As Integer
    With ActivePresentation
        .Slides(1).Shapes(1).TextFrame.TextRange.Text = &quot;Title&quot;
        Dim sld As Slide
        For i = 1 To 10
            Set sld = .Slides.Add(i &#43; 1, ppLayoutText)
            sld.Shapes(1).TextFrame.TextRange.Text = &quot;Slide &quot; &amp; i
        Next i
    End With
End Sub

Private Sub CleanupDemo()
    ' Remove all but the first slide.
    ActiveWindow.ViewType = ppViewNormal
    Dim i As Integer
    For i = ActivePresentation.Slides.Count To 2 Step -1
        ActivePresentation.Slides(i).Delete
    Next i
End Sub
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Sub</span>&nbsp;DemoSections()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;this&nbsp;procedure&nbsp;to&nbsp;demonstrate&nbsp;working&nbsp;with&nbsp;sections,</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;focusing&nbsp;on&nbsp;new&nbsp;features&nbsp;in&nbsp;PowerPoint&nbsp;2010.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Copy&nbsp;this&nbsp;code&nbsp;into&nbsp;a&nbsp;module&nbsp;in&nbsp;a&nbsp;new&nbsp;presentation.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Display&nbsp;the&nbsp;VBA&nbsp;window&nbsp;side-by-side&nbsp;with&nbsp;the&nbsp;PowerPoint&nbsp;window</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;and&nbsp;press&nbsp;F8&nbsp;and&nbsp;then&nbsp;Shift&#43;F8&nbsp;to&nbsp;single&nbsp;step&nbsp;through&nbsp;this</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;code&nbsp;for&nbsp;the&nbsp;most&nbsp;effective&nbsp;use&nbsp;of&nbsp;this&nbsp;demonstration.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SetupDemo&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Add&nbsp;three&nbsp;sections:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;ActivePresentation.SectionProperties&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;AddBeforeSlide&nbsp;method&nbsp;to&nbsp;insert&nbsp;a</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;section&nbsp;in&nbsp;a&nbsp;position&nbsp;relative&nbsp;to</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;a&nbsp;particular&nbsp;slide&nbsp;in&nbsp;the&nbsp;presentation:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.AddBeforeSlide&nbsp;<span class="visualBasic__number">2</span>,&nbsp;<span class="visualBasic__string">&quot;Test&nbsp;Section&nbsp;1&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.AddBeforeSlide&nbsp;<span class="visualBasic__number">5</span>,&nbsp;<span class="visualBasic__string">&quot;Test&nbsp;Section&nbsp;2&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.AddBeforeSlide&nbsp;<span class="visualBasic__number">9</span>,&nbsp;<span class="visualBasic__string">&quot;Test&nbsp;Section&nbsp;3&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;AddSection&nbsp;method&nbsp;to&nbsp;insert&nbsp;a&nbsp;section</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;in&nbsp;relation&nbsp;to&nbsp;other&nbsp;sections,&nbsp;as&nbsp;opposed&nbsp;to</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;in&nbsp;relation&nbsp;to&nbsp;a&nbsp;specific&nbsp;slide:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.AddSection&nbsp;<span class="visualBasic__number">1</span>,&nbsp;<span class="visualBasic__string">&quot;Intro&nbsp;Section&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;Count&nbsp;property&nbsp;to&nbsp;return&nbsp;the&nbsp;total&nbsp;number&nbsp;of&nbsp;sections.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;Name&nbsp;property&nbsp;to&nbsp;get&nbsp;the&nbsp;name&nbsp;of&nbsp;the&nbsp;section.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;SlidesCount&nbsp;method&nbsp;to&nbsp;return&nbsp;the&nbsp;number&nbsp;of&nbsp;slides&nbsp;in&nbsp;a</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;given&nbsp;section:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Debug.Print&nbsp;<span class="visualBasic__string">&quot;Section&nbsp;&quot;</span>&nbsp;&amp;&nbsp;i&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;(&quot;</span>&nbsp;&amp;&nbsp;.Name(i)&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;)&nbsp;contains&nbsp;&quot;</span>&nbsp;&amp;&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.SlidesCount(i)&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;&nbsp;slide(s);&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;The&nbsp;FirstSlide&nbsp;method&nbsp;returns&nbsp;the&nbsp;index&nbsp;of&nbsp;the&nbsp;first&nbsp;slide</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;in&nbsp;the&nbsp;section.&nbsp;If&nbsp;the&nbsp;section&nbsp;has&nbsp;no&nbsp;slides,&nbsp;it&nbsp;returns&nbsp;-1.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Debug.Print&nbsp;<span class="visualBasic__string">&quot;&nbsp;The&nbsp;first&nbsp;slide&nbsp;is&nbsp;:&nbsp;&quot;</span>&nbsp;&amp;&nbsp;.FirstSlide(i)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;Rename&nbsp;method&nbsp;to&nbsp;change&nbsp;the&nbsp;name&nbsp;of&nbsp;a&nbsp;section:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Rename&nbsp;i,&nbsp;<span class="visualBasic__string">&quot;New&nbsp;Name&nbsp;&quot;</span>&nbsp;&amp;&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;Move&nbsp;method&nbsp;to&nbsp;move&nbsp;a&nbsp;section,&nbsp;along&nbsp;with&nbsp;its&nbsp;slides:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;code&nbsp;moves&nbsp;the&nbsp;second&nbsp;section&nbsp;to&nbsp;the&nbsp;end:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Move&nbsp;<span class="visualBasic__number">2</span>,&nbsp;.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;SectionIndex&nbsp;property&nbsp;of&nbsp;a&nbsp;slide&nbsp;to</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;determine&nbsp;what&nbsp;section&nbsp;it's&nbsp;in:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;ActivePresentation&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;sld&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Slide&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;sld&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;ActivePresentation.Slides&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Debug.Print&nbsp;<span class="visualBasic__string">&quot;Slide&nbsp;&quot;</span>&nbsp;&amp;&nbsp;sld.SlideIndex&nbsp;&amp;&nbsp;<span class="visualBasic__string">&quot;&nbsp;is&nbsp;in&nbsp;section&nbsp;&quot;</span>&nbsp;&amp;&nbsp;sld.sectionIndex&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;sld&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;MoveToSectionStart&nbsp;method&nbsp;to&nbsp;move</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;a&nbsp;slide&nbsp;to&nbsp;the&nbsp;beginning&nbsp;of&nbsp;the&nbsp;section.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;code&nbsp;moves&nbsp;the&nbsp;final&nbsp;slide&nbsp;to&nbsp;the&nbsp;beginning&nbsp;of</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;the&nbsp;first&nbsp;section:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;sld&nbsp;=&nbsp;.Slides(.Slides.Count)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sld.MoveToSectionStart&nbsp;<span class="visualBasic__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;You&nbsp;can&nbsp;expand&nbsp;or&nbsp;contract&nbsp;any&nbsp;section,&nbsp;as&nbsp;well.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Use&nbsp;the&nbsp;IsSectionExpanded&nbsp;property&nbsp;and&nbsp;the&nbsp;ExpandSection</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;method&nbsp;to&nbsp;do&nbsp;the&nbsp;work.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ActiveWindow.ViewType&nbsp;=&nbsp;ppViewSlideSorter&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;First,&nbsp;contract&nbsp;every&nbsp;other&nbsp;section:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;.SectionProperties.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Contract&nbsp;the&nbsp;even-numbered&nbsp;sections:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">Mod</span>&nbsp;<span class="visualBasic__number">2</span>&nbsp;=&nbsp;<span class="visualBasic__number">0</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ActiveWindow.ExpandSection&nbsp;i,&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Now&nbsp;toggle&nbsp;the&nbsp;state&nbsp;of&nbsp;the&nbsp;expansion&nbsp;for&nbsp;all&nbsp;sections:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;isExpanded&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Boolean</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;.SectionProperties.Count&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;isExpanded&nbsp;=&nbsp;ActiveWindow.IsSectionExpanded(i)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ActiveWindow.ExpandSection&nbsp;i,&nbsp;<span class="visualBasic__keyword">Not</span>&nbsp;isExpanded&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;ActivePresentation.SectionProperties&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Unlike&nbsp;in&nbsp;the&nbsp;user&nbsp;interface,&nbsp;there's&nbsp;no&nbsp;simple&nbsp;way&nbsp;to&nbsp;remove</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;all&nbsp;the&nbsp;sections,&nbsp;so&nbsp;you'll&nbsp;need&nbsp;to&nbsp;write&nbsp;code&nbsp;like&nbsp;this</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;to&nbsp;do&nbsp;it:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;.Count&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">Step</span>&nbsp;-<span class="visualBasic__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Pass&nbsp;True&nbsp;in&nbsp;the&nbsp;second&nbsp;parameter&nbsp;to&nbsp;delete&nbsp;the</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;slides&nbsp;in&nbsp;the&nbsp;section:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Delete&nbsp;i,&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Now&nbsp;delete&nbsp;all&nbsp;the&nbsp;slides&nbsp;except&nbsp;the&nbsp;first&nbsp;one,&nbsp;to&nbsp;reset</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;back&nbsp;to&nbsp;the&nbsp;way&nbsp;the&nbsp;demonstration&nbsp;started:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CleanupDemo&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;SetupDemo()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Set&nbsp;the&nbsp;title&nbsp;of&nbsp;the&nbsp;presentation,&nbsp;and&nbsp;add&nbsp;10&nbsp;slides.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;ActivePresentation&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Slides(<span class="visualBasic__number">1</span>).Shapes(<span class="visualBasic__number">1</span>).TextFrame.TextRange.Text&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Title&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;sld&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Slide&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;<span class="visualBasic__number">1</span>&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;<span class="visualBasic__number">10</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Set</span>&nbsp;sld&nbsp;=&nbsp;.Slides.Add(i&nbsp;&#43;&nbsp;<span class="visualBasic__number">1</span>,&nbsp;ppLayoutText)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sld.Shapes(<span class="visualBasic__number">1</span>).TextFrame.TextRange.Text&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Slide&nbsp;&quot;</span>&nbsp;&amp;&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">With</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
&nbsp;
<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;CleanupDemo()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Remove&nbsp;all&nbsp;but&nbsp;the&nbsp;first&nbsp;slide.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ActiveWindow.ViewType&nbsp;=&nbsp;ppViewNormal&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;i&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;i&nbsp;=&nbsp;ActivePresentation.Slides.Count&nbsp;<span class="visualBasic__keyword">To</span>&nbsp;<span class="visualBasic__number">2</span>&nbsp;<span class="visualBasic__keyword">Step</span>&nbsp;-<span class="visualBasic__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ActivePresentation.Slides(i).Delete&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;i&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;
</pre>
</div>
</div>
</div>
<h1><span>Source Code Files</span></h1>
<ul>
<li><span style="font-size:small"><em><em><a id="26183" href="/site/view/file/26183/1/PPT.WorkWithSections.txt">PPT.WorkWithSections.txt</a>&nbsp;- Download this sample only.<br>
</em></em></span></li><li><span style="font-size:small"><em><em><a id="26184" href="/site/view/file/26184/1/Office%202010%20101%20Code%20Samples.zip">Office 2010 101 Code Samples.zip</a>&nbsp;- Download all the samples.</em></em></span>
</li></ul>
<h1>More Information</h1>
<ul>
<li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/aa905465">PowerPoint Developer Center on MSDN</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/office/hh360994">101 Code Samples for Office 2010 Developers</a></span>
</li></ul>
