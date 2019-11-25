# How to add a hint text to a WPF TextBox
## License
- Apache License, Version 2.0
## Technologies
- .NET
- Windows
- Windows Desktop App Development
- Windows Presentation Framework (WPF)
## Topics
- TextBox
- code snippets
- hint text
## Updated
- 06/24/2014
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodesampletopbanner">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; margin-top:24pt; margin-bottom:0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to add a hint text to a WPF
</span><span style="font-weight:bold; font-size:14pt">TextBox</span></span> </p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; margin-top:10pt; margin-bottom:0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="">Adding a hint text to a </span><span style="">TextBox</span><span style=""> is a frequently used function in WPF applications. Many customers encounter problems when they try to implement such a function. This code
 snippet project will show you how to add a hint text to a WPF </span><span style="">TextBox</span><span style="">.</span></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; margin-top:10pt; margin-bottom:0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Step1. Create a WPF application in Visual Studio.
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="">Step2. Add the </span><span style="">following XAML</span><span style=""> code in
</span><span style="">MainWindow.xaml</span><span style="">.</span></span> </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">xaml</span>

<pre id="codePreview" class="xaml">
&lt;Window x:Class=&quot;WpfApplication1.MainWindow&quot;
        xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
        xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
        xmlns:local =&quot;clr-namespace:WpfApplication1&quot;
        Title=&quot;MainWindow&quot; Height=&quot;350&quot; Width=&quot;525&quot;&gt;
    &lt;Window.Resources&gt;
        &lt;SolidColorBrush x:Key=&quot;brushWatermarkBackground&quot; Color=&quot;White&quot; /&gt;
        &lt;SolidColorBrush x:Key=&quot;brushWatermarkForeground&quot; Color=&quot;LightSteelBlue&quot; /&gt;
        &lt;SolidColorBrush x:Key=&quot;brushWatermarkBorder&quot; Color=&quot;Indigo&quot; /&gt;
        &lt;BooleanToVisibilityConverter x:Key=&quot;BooleanToVisibilityConverter&quot; /&gt;
        &lt;local:TextInputToVisibilityConverter x:Key=&quot;TextInputToVisibilityConverter&quot; /&gt;
        &lt;Style x:Key=&quot;EntryFieldStyle&quot; TargetType=&quot;Grid&quot; &gt;
            &lt;Setter Property=&quot;HorizontalAlignment&quot; Value=&quot;Stretch&quot; /&gt;
            &lt;Setter Property=&quot;VerticalAlignment&quot; Value=&quot;Center&quot; /&gt;
            &lt;Setter Property=&quot;Margin&quot; Value=&quot;20,0&quot; /&gt;
        &lt;/Style&gt;
    &lt;/Window.Resources&gt;
    &lt;Grid&gt;
        &lt;Grid.RowDefinitions&gt;
            &lt;RowDefinition /&gt;
            &lt;RowDefinition /&gt;
            &lt;RowDefinition /&gt;
        &lt;/Grid.RowDefinitions&gt;
        &lt;Grid Grid.Row=&quot;0&quot; Background=&quot;{StaticResource brushWatermarkBackground}&quot; Style=&quot;{StaticResource EntryFieldStyle}&quot; &gt;
            &lt;TextBlock Margin=&quot;5,2&quot; Text=&quot;This prompt dissappears as you type...&quot; Foreground=&quot;{StaticResource brushWatermarkForeground}&quot;
                       Visibility=&quot;{Binding ElementName=txtUserEntry, Path=Text.IsEmpty, Converter={StaticResource BooleanToVisibilityConverter}}&quot; /&gt;
            &lt;TextBox Name=&quot;txtUserEntry&quot; Background=&quot;Transparent&quot; BorderBrush=&quot;{StaticResource brushWatermarkBorder}&quot; /&gt;
        &lt;/Grid&gt;
        &lt;Grid Grid.Row=&quot;1&quot; Background=&quot;{StaticResource brushWatermarkBackground}&quot; Style=&quot;{StaticResource EntryFieldStyle}&quot; &gt;
            &lt;TextBlock Margin=&quot;5,2&quot; Text=&quot;This dissappears as the control gets focus...&quot; Foreground=&quot;{StaticResource brushWatermarkForeground}&quot; &gt;
                &lt;TextBlock.Visibility&gt;
                    &lt;MultiBinding Converter=&quot;{StaticResource TextInputToVisibilityConverter}&quot;&gt;
                        &lt;Binding ElementName=&quot;txtUserEntry2&quot; Path=&quot;Text.IsEmpty&quot; /&gt;
                        &lt;Binding ElementName=&quot;txtUserEntry2&quot; Path=&quot;IsFocused&quot; /&gt;
                    &lt;/MultiBinding&gt;
                &lt;/TextBlock.Visibility&gt;
            &lt;/TextBlock&gt;
            &lt;TextBox Name=&quot;txtUserEntry2&quot; Background=&quot;Transparent&quot; BorderBrush=&quot;{StaticResource brushWatermarkBorder}&quot; /&gt;
        &lt;/Grid&gt;
    &lt;/Grid&gt;
&lt;/Window&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="">Step3. Create a class &quot;</span><span style="">TextInputToVisibilityConverter</span><span style="">&quot;. It implements the
</span><span style="">IMultiValueConverter</span><span style=""> interface.</span></span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<pre id="codePreview" class="csharp">
using System;
using <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.aspx" target="_blank" title="Auto generated link to System.Windows">System.Windows</a>;
using  <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Data.aspx" target="_blank" title="Auto generated link to System.Windows.Data">System.Windows.Data</a>;
namespace WpfApplication1
{
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Globalization.CultureInfo.aspx" target="_blank" title="Auto generated link to System.Globalization.CultureInfo">System.Globalization.CultureInfo</a> culture)
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];
                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Globalization.CultureInfo.aspx" target="_blank" title="Auto generated link to System.Globalization.CultureInfo">System.Globalization.CultureInfo</a> culture)
        {
            throw new NotImplementedException();
        }
    }
}
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="">Step4. You can build and run the application. The following is the screenshot of the running application.</span></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a name="_GoBack"></a><span style="font-size:11pt"><img src="117488-image.png" alt="" width="421" height="175" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; margin-top:10pt; margin-bottom:0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="">IMultiValueConverter</span><span style=""> Interface</span></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/en-us/library/system.windows.data.imultivalueconverter(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563C1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.windows.data.imultivalueconverter(v=vs.110).aspx</span></a></span>
</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; line-height:27.6pt; margin-bottom:10pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span> </p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers’ pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers’ frequently asked programming tasks, and allow
 developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
