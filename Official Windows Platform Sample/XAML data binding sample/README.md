# XAML data binding sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows 8.1
- Windows Phone 8.1
## Topics
- User Interface
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates basic data binding techniques using the <a href="http://msdn.microsoft.com/library/windows/apps/br209820">
<b>Binding</b></a> class and <a href="http://msdn.microsoft.com/library/windows/apps/hh758283">
Binding markup extension</a>. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample covers:</p>
<ul>
<li>Controlling the direction of data flow and updates using the <a href="http://msdn.microsoft.com/library/windows/apps/br209829">
<b>Binding.Mode</b></a> property. </li><li>Changing the format of bound values for display purposes by implementing the <a href="http://msdn.microsoft.com/library/windows/apps/br209903">
<b>IValueConverter</b></a> interface. </li><li>Binding to a data model that implements <a href="http://msdn.microsoft.com/library/windows/apps/br209899">
<b>INotifyPropertyChanged</b></a> in order to receive change notifications. </li><li>Binding to string and integer indexer properties. </li><li>Using <a href="http://msdn.microsoft.com/library/windows/apps/br242348"><b>DataTemplate</b></a> instances to customize the appearance of your data.
</li><li>Using <a href="http://msdn.microsoft.com/library/windows/apps/br209833"><b>CollectionViewSource</b></a> to display data in groups.
</li><li>Responding to changes in bound collections using <a href="http://msdn.microsoft.com/library/windows/apps/ms668629">
<b>INotifyCollectionChanged</b></a>. </li><li>Using C&#43;&#43; to implement and bind to an incremental loading collection through the
<a href="http://msdn.microsoft.com/library/windows/apps/hh701916"><b>ISupportIncrementalLoading</b></a> interface.
</li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=243667">Windows 8.1 app samples</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=226564">XAML GridView grouping and SemanticZoom sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=228621">StorageDataSource and GetVirtualizedFilesVector sample</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209917"><b>Windows.UI.Xaml.Data</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209820"><b>Binding</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208713"><b>DataContext</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242362"><b>DependencyProperty</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209833"><b>CollectionViewSource</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/ms668604"><b>ObservableCollection(Of T)</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242348"><b>DataTemplate</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209391"><b>ControlTemplate</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209899"><b>INotifyPropertyChanged</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/ms668629"><b>INotifyCollectionChanged</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209903"><b>IValueConverter</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209878"><b>ICustomPropertyProvider</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701916"><b>ISupportIncrementalLoading</b></a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh464965">QuickStart: data binding to controls</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh758320">Data binding with XAML</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh758322">How to bind to hierarchical data and create a master/details view</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh758283">Binding markup extension</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj569302">PropertyPath syntax</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh758284">RelativeSource markup extension</a>
</dt></dl>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p></p>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>DataBinding.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build DataBinding.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DataBinding.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build DataBinding.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>DataBinding.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy DataBinding.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DataBinding.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy DataBinding.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>DataBinding.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>DataBinding.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
