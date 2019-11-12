# ADO.NET Entity Framework
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- ADO.NET Entity Framework
- WPF
## Topics
- Data Access
- XAML
- MVVM
## Updated
- 06/20/2011
## Description

<h1>Introduction</h1>
<p>This sample shows a WPF application built on top of the Microsoft ADO.NET Entity Framework. The application shows how the Entity Framework can be used in some common design patterns that promote testability and maintainability of code.</p>
<p>There is a 'useFakes' boolean flag located in the code behind App.xaml in the EmployeeTracker project that determines whether the application runs against Microsoft SQL Server using the Entity Framework or against in-memory fakes. When set to false the app
 will try and attach the MDF file located in the EntityFramework project to the local Microsoft SQL Server Express instance. This connection can be changed in the App.config file of the EmployeeTracker project.</p>
<h1><span>Building the Sample</span></h1>
<p>Press F5<em><br>
</em></p>
<h1>Description</h1>
<p>The solution is made up of the following projects:</p>
<h4>Model</h4>
<p>This project contains the business model.</p>
<p>The Department and Employee objects have some embedded logic that keeps navigation properties synchronized. Setting the Department property on an Employee results in that Employee being added to the Employees collection on the new Department and being removed
 from the Employees collection on the old Department. Similar logic applies if an Employee is added to or removed from the Employees collection on a Department. The same fixup approach is also implemented on the Manager/Reports relationship. This logic is not
 required when running against the Entity Framework using change tracking proxies as the proxies will perform this fix-up automatically but the logic is central to the business model and should remain in place when using Fakes or any other persistence mechanism.</p>
<p>There are a set of tests defined in the abstract class Tests\Model\Entities\FixupTestsBase.cs that test fix-up behavior. This class is derived from to run the same tests against the following versions of the business objects:</p>
<ul>
<li>Pure POCO (i.e. the base classes) - Tests\Model\Entities\BaseModelTypeFixupTests.cs
</li><li>EF Proxies attached to an ObjectContext - Tests\EntityFramework\DetachedProxyFixupTests.cs
</li><li>EF Proxies not attached to an ObjectContext - Tests\EntityFramework\AttachedProxyFixupTests.cs
</li></ul>
<h4>Common</h4>
<p>This project contains a set of interfaces for data retrieval and persistence using the Unit of Work and Repository pattern. The project includes an implementation for some of the interfaces that can be re-used with multiple data access approaches. The IEmployeeContext
 interface represents the underlying functionality required for data access and is implemented in the EntityFramework and Fakes projects.</p>
<h4>EntityFramework</h4>
<p>This project contains the Entity Data Model (EmployeeModel.edmx) that maps between the database (Employee.mdf) and the business objects defined in the Model project. There is also a custom T4 template (ContextTemplate.tt) that is based on the default template
 but has entity generation removed as we are using the pre-defined business objects. The T4 template also adds the IEmployeeContext interface to the generated context.</p>
<p>The Entity Data Model includes a Model Defined Function (MDF) which calculates an Employee&rsquo;s tenure. The Employee repository in the Common project has a private method for calculating tenure which is marked with an EdmFunction attribute. When running
 against the Entity Framework the MDF will be used instead of this method and evaluation will occur in the database. The method includes an implementation which is used when running against fakes.</p>
<h4>Fakes</h4>
<p>This project contains in-memory versions of the data access components, these are primarily used for unit testing but the WPF application can also be run against these implementations. Also included is a class for instantiating a fake context that is pre-populated
 with a set of sample data.</p>
<h4>EmployeeTracker</h4>
<p>This project is a user interface implemented in WPF using the Model-View-ViewModel pattern. The entry point for the application is in the code behind file for App.xaml where a UnitOfWork and repositories are constructed. When running against EF there is
 a connection string in App.config which controls access to the database. If you don't have a local Microsoft Sql Server Express instance available called .\SQLEXPRESS then you will need to update this connection.</p>
<h4>Tests</h4>
<p>This project contains unit tests for components in the other projects. When testing EF components a connection string in App.config is used. The database is not attached during testing so only the metadata sections of the connection string are used.</p>
<p><br>
<em>&nbsp;&nbsp;</em></p>
<h1>Screenshot</h1>
<p><img src="22514-screenshot.png" alt="" width="904" height="550"></p>
<p>&nbsp;</p>
<h1>Sample Code</h1>
<h1>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre id="codePreview" class="csharp">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Initializes&nbsp;a&nbsp;new&nbsp;instance&nbsp;of&nbsp;the&nbsp;MainViewModel&nbsp;class.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;unitOfWork&quot;&gt;UnitOfWork&nbsp;for&nbsp;co-ordinating&nbsp;changes&lt;/param&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;departmentRepository&quot;&gt;Repository&nbsp;for&nbsp;querying&nbsp;department&nbsp;data&lt;/param&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;employeeRepository&quot;&gt;Repository&nbsp;for&nbsp;querying&nbsp;employee&nbsp;data&lt;/param&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;MainViewModel(IUnitOfWork&nbsp;unitOfWork,&nbsp;IDepartmentRepository&nbsp;departmentRepository,&nbsp;IEmployeeRepository&nbsp;employeeRepository)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(unitOfWork&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;ArgumentNullException(<span class="cs__string">&quot;unitOfWork&quot;</span>);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(departmentRepository&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;ArgumentNullException(<span class="cs__string">&quot;departmentRepository&quot;</span>);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(employeeRepository&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;ArgumentNullException(<span class="cs__string">&quot;employeeRepository&quot;</span>);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.unitOfWork&nbsp;=&nbsp;unitOfWork;&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Build&nbsp;data&nbsp;structures&nbsp;to&nbsp;populate&nbsp;areas&nbsp;of&nbsp;the&nbsp;application&nbsp;surface&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ObservableCollection&lt;EmployeeViewModel&gt;&nbsp;allEmployees&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;ObservableCollection&lt;EmployeeViewModel&gt;();&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ObservableCollection&lt;DepartmentViewModel&gt;&nbsp;allDepartments&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;ObservableCollection&lt;DepartmentViewModel&gt;();&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;dep&nbsp;<span class="cs__keyword">in</span>&nbsp;departmentRepository.GetAllDepartments())&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allDepartments.Add(<span class="cs__keyword">new</span>&nbsp;DepartmentViewModel(dep));&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;emp&nbsp;<span class="cs__keyword">in</span>&nbsp;employeeRepository.GetAllEmployees())&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allEmployees.Add(<span class="cs__keyword">new</span>&nbsp;EmployeeViewModel(emp,&nbsp;allEmployees,&nbsp;allDepartments,&nbsp;<span class="cs__keyword">this</span>.unitOfWork));&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.DepartmentWorkspace&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DepartmentWorkspaceViewModel(allDepartments,&nbsp;unitOfWork);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.EmployeeWorkspace&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;EmployeeWorkspaceViewModel(allEmployees,&nbsp;allDepartments,&nbsp;unitOfWork);&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Build&nbsp;non-interactive&nbsp;list&nbsp;of&nbsp;long&nbsp;serving&nbsp;employees&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;List&lt;BasicEmployeeViewModel&gt;&nbsp;longServingEmployees&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;List&lt;BasicEmployeeViewModel&gt;();&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;emp&nbsp;<span class="cs__keyword">in</span>&nbsp;employeeRepository.GetLongestServingEmployees(<span class="cs__number">5</span>))&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;longServingEmployees.Add(<span class="cs__keyword">new</span>&nbsp;BasicEmployeeViewModel(emp));&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.LongServingEmployees&nbsp;=&nbsp;longServingEmployees;&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.SaveCommand&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DelegateCommand((o)&nbsp;=&gt;&nbsp;<span class="cs__keyword">this</span>.Save());&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre id="codePreview" class="vb">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;summary&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;Initializes&nbsp;a&nbsp;new&nbsp;instance&nbsp;of&nbsp;the&nbsp;MainViewModel&nbsp;class.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;/summary&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;param&nbsp;name=&quot;unitOfWork&quot;&gt;UnitOfWork&nbsp;for&nbsp;co-ordinating&nbsp;changes&lt;/param&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;param&nbsp;name=&quot;departmentRepository&quot;&gt;Repository&nbsp;for&nbsp;querying&nbsp;department&nbsp;data&lt;/param&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'''&nbsp;&lt;param&nbsp;name=&quot;employeeRepository&quot;&gt;Repository&nbsp;for&nbsp;querying&nbsp;employee&nbsp;data&lt;/param&gt;&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Public</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;<span class="visualBasic__keyword">New</span>(<span class="visualBasic__keyword">ByVal</span>&nbsp;unitOfWork&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;IUnitOfWork,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;departmentRepository&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;IDepartmentRepository,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;employeeRepository&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;IEmployeeRepository)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;unitOfWork&nbsp;<span class="visualBasic__keyword">Is</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Throw</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ArgumentNullException(<span class="visualBasic__string">&quot;unitOfWork&quot;</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;departmentRepository&nbsp;<span class="visualBasic__keyword">Is</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Throw</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ArgumentNullException(<span class="visualBasic__string">&quot;departmentRepository&quot;</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;employeeRepository&nbsp;<span class="visualBasic__keyword">Is</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Throw</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ArgumentNullException(<span class="visualBasic__string">&quot;employeeRepository&quot;</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.unitOfWork&nbsp;=&nbsp;unitOfWork&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Build&nbsp;data&nbsp;structures&nbsp;to&nbsp;populate&nbsp;areas&nbsp;of&nbsp;the&nbsp;application&nbsp;surface&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;allEmployees&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ObservableCollection(<span class="visualBasic__keyword">Of</span>&nbsp;EmployeeViewModel)()&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;allDepartments&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ObservableCollection(<span class="visualBasic__keyword">Of</span>&nbsp;DepartmentViewModel)()&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;dep&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;departmentRepository.GetAllDepartments()&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allDepartments.Add(<span class="visualBasic__keyword">New</span>&nbsp;DepartmentViewModel(dep))&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;dep&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;emp&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;employeeRepository.GetAllEmployees()&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;allEmployees.Add(<span class="visualBasic__keyword">New</span>&nbsp;EmployeeViewModel(emp,&nbsp;allEmployees,&nbsp;allDepartments,&nbsp;<span class="visualBasic__keyword">Me</span>.unitOfWork))&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;emp&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.DepartmentWorkspace&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;DepartmentWorkspaceViewModel(allDepartments,&nbsp;unitOfWork)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.EmployeeWorkspace&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;EmployeeWorkspaceViewModel(allEmployees,&nbsp;allDepartments,&nbsp;unitOfWork)&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Build&nbsp;non-interactive&nbsp;list&nbsp;of&nbsp;long&nbsp;serving&nbsp;employees&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;longServingEmployees&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;List(<span class="visualBasic__keyword">Of</span>&nbsp;BasicEmployeeViewModel)()&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;emp&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;employeeRepository.GetLongestServingEmployees(<span class="visualBasic__number">5</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;longServingEmployees.Add(<span class="visualBasic__keyword">New</span>&nbsp;BasicEmployeeViewModel(emp))&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;emp&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.LongServingEmployees&nbsp;=&nbsp;longServingEmployees&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Me</span>.SaveCommand&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;DelegateCommand(<span class="visualBasic__keyword">Sub</span>(o)&nbsp;<span class="visualBasic__keyword">Me</span>.Save())&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
</div>
<div class="endscriptcode">Sample Source Code Files</div>
</h1>
<ul>
<li>C#
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23476&pathId=885145180">DepartmentRepository.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1756978627">EmployeeRepository.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1679466054">IDepartmentRepository.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1674523977">IEmployeeContext.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=405701389">IEmployeeRepository.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=125827832">IUnitOfWork.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=570021258">UnitOfWork.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=375478376">AddressDetailView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1344812351">DepartmentDetailView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=314245331">DepartmentWorkspaceView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1474972076">EmailDetailView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=491631008">EmployeeContactsDetailView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1448278886">EmployeeDetailView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1258547359">EmployeeWorkspaceView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1370733782">MainView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1479273981">PhoneDetailView.xaml</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=58226921">DelegateCommand.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=857059571">ViewModelBase.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=758568729">AddressViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=243537010">BasicEmployeeViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1824062559">ContactDetailViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1401570386">DepartmentViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=634103700">DepartmentWorkspaceViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=715843300">EmailViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=222146792">EmployeeViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1302877200">EmployeeWorkspaceViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1405316131">MainViewModel.cs</a>
</li><li><a class="browseFile" href="sourcecode?fileId=23476&pathId=1481281714">PhoneViewModel.cs</a>
</li></ul>
</li><li>VB
<ul>
<li><a class="browseFile" href="sourcecode?fileId=22513&pathId=458597646">DepartmentRepository.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=2113993403">EmployeeRepository.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=764757789">IDepartmentRepository.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=403744473">IEmployeeContext.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=507228960">IEmployeeRepository.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=383015026">IUnitOfWork.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=870290730">UnitOfWork.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=6605358">DelegateCommand.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=495781560">ViewModelBase.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=838934698">AddressViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=1431842439">BasicEmployeeViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=1925216985">ContactDetailViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=2041639966">DepartmentViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=1492838839">DepartmentWorkspaceViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=1286996921">EmailViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=326458949">EmployeeViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=872587223">EmployeeWorkspaceViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=533765914">MainViewModel.vb</a>
</li><li><a class="browseFile" href="sourcecode?fileId=22513&pathId=108047891">PhoneViewModel.vb</a>
</li></ul>
</li></ul>
<h1>More Information</h1>
<p>For more information on Entity Framework and related topics with this sample, click a link below.</p>
<ul>
<li><a href="http://msdn.microsoft.com/en-us/library/bb399572.aspx" target="_blank">ADO.NET Entity Framework</a>
</li><li><a href="http://msdn.microsoft.com/en-us/library/ms754130.aspx" target="_blank">WPF</a>
</li><li><a href="http://msdn.microsoft.com/en-us/magazine/dd419663.aspx" target="_blank">MVVM (Model View ViewModel)</a>
</li></ul>
