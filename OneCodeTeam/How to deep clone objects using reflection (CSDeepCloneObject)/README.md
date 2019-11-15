# How to deep clone objects using reflection (CSDeepCloneObject)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- .NET Framework
- CLR
## Topics
- Reflection
- Object Clone
- Property
## Updated
- 12/03/2012
## Description

<h1>The Sample Demonstrates How to Deep Clone Objects Using Reflection (CSDeepCloneObject)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample demonstrates how to implement deep clone between objects in C# using reflection.
</p>
<p class="MsoNormal">We can use the MemberwiseClone to get a copy, but the MemberwiseClone method creates a shallow copy by creating a new object, and then copying the non-static fields of the current object to the new object. If a field is a value type,
 a<span style="">&nbsp;&nbsp; </span>bit-by-bit copy of the field is performed. If a field is a reference type, the reference is copied but the referred object is not.
</p>
<p class="MsoNormal">In this sample, we make use metadata information to clone a new object and drill down each field, ultimately, copy this field.
</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to running the sample, the flowing is the result.</p>
<p class="MsoNormal"><span style=""><img src="71825-image.png" alt="" width="643" height="319" align="middle">
</span></p>
<p class="MsoNormal">The shallow clones of the original objects are the new objects and contain the new objects of the value type fields or the string type fields. But the reference fields refer to the same referred object. The deep clones are the new objects,
 and their reference fields also refer to the new referred object.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">A. Implement deep clone using reflection.</p>
<p class="MsoNormal">1. If the type of object is the value type, we will always get a new object when the original object is assigned to another variable. So if the type of the object is primitive or enum, we just return the object. We will process the struct
 type subsequently because the struct type may contain the reference fields. </p>
<p class="MsoNormal">If the string variables contain the same chars, they always refer to the same string in the heap. So if the type of the object is string, we also return the object.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
if (type.IsPrimitive || type.IsEnum || type == typeof(string))
{
&nbsp;&nbsp;&nbsp; return obj;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. If the type of the object is the Array, we use the CreateInstance method to get a new instance of the array. We also process recursively this method in the elements of the original array because the type of the element may be the reference type.<span style="font-size:9.5pt; font-family:Consolas; color:green">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
else if (type.IsArray)
{
&nbsp;&nbsp;&nbsp; Type typeElement = Type.GetType(type.FullName.Replace(&quot;[]&quot;, string.Empty));
&nbsp;&nbsp;&nbsp; var array = obj as Array;
&nbsp;&nbsp;&nbsp; Array copiedArray = Array.CreateInstance(typeElement, array.Length);
&nbsp;&nbsp;&nbsp; for (int i = 0; i &lt; array.Length; i&#43;&#43;)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Get the deep clone of the element in the original array and assign the 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// clone to the new array.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; copiedArray.SetValue(CloneProcedure(array.GetValue(i)), i);


&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; return copiedArray;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">3. If the type of the object is class or struct, it may contain the reference fields, so we use reflection and process recursively this method in the fields of the object to get the deep clone of the object.
</p>
<p class="MsoNormal">We use Type.IsValueType method here because there is no way to indicate directly whether the Type is a struct type.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
else if (type.IsClass||type.IsValueType)
{
&nbsp;&nbsp;&nbsp; object copiedObject = Activator.CreateInstance(obj.GetType());
&nbsp;&nbsp;&nbsp; // Get all FieldInfo.
&nbsp;&nbsp;&nbsp; FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
&nbsp;&nbsp;&nbsp; foreach (FieldInfo field in fields)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; object fieldValue = field.GetValue(obj);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (fieldValue != null)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Get the deep clone of the field in the original object and assign the 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// clone to the field in the new object.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; field.SetValue(copiedObject, CloneProcedure(fieldValue));
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; return copiedObject;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
B. Demonstrate the different clones of the Employee class.<span style="font-size:9.5pt; font-family:Consolas; color:green">
</span></p>
<p class="MsoNormal">Demonstrate the difference between the shallow clone and the deep clone of the Employee class.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
public static void CloneEmployee()
{
&nbsp;&nbsp;&nbsp; Address address = new Address { City = &quot;ShangHai&quot; };
&nbsp;&nbsp;&nbsp; Employee originalEmployee = new Employee { Id = 101, Name = &quot;Gail Erickson&quot;, Address = address };


&nbsp;&nbsp;&nbsp; // Get a shallow copy of the originalEmployee and set the new values in the copy.
&nbsp;&nbsp;&nbsp; Employee shallowCloneEmployee = originalEmployee.ShallowCopy();
&nbsp;&nbsp;&nbsp; shallowCloneEmployee.Id = 102;
&nbsp;&nbsp;&nbsp; shallowCloneEmployee.Name = &quot;Dylan Miller&quot;;
&nbsp;&nbsp;&nbsp; shallowCloneEmployee.Address.City = &quot;RedMond&quot;;  // Change the shallow copy's address information.


&nbsp;&nbsp;&nbsp; // Show the information of originalEmployee and the shallow copy.
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;It is the shallow clone of the Employee class.&quot;);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; originalEmployee.Id, originalEmployee.Name, originalEmployee.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; shallowCloneEmployee.Id, shallowCloneEmployee.Name, shallowCloneEmployee.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine();


&nbsp;&nbsp;&nbsp; // Get a deep copy of the originalEmployee and set the new values in the copy.
&nbsp;&nbsp;&nbsp; address.City = &quot;ShangHai&quot;;
&nbsp;&nbsp;&nbsp; Employee deepCloneEmployee = DeepCloneHelper.DeepClone(originalEmployee);
&nbsp;&nbsp;&nbsp; deepCloneEmployee.Id = 102;
&nbsp;&nbsp;&nbsp; deepCloneEmployee.Name = &quot;Dylan Miller&quot;;
&nbsp;&nbsp;&nbsp; deepCloneEmployee.Address.City = &quot;RedMond&quot;;  // Change the deep copy's address information.


&nbsp;&nbsp;&nbsp; // Show the information of originalEmployee and the deep copy.
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;It is the deep clone of the Employee class.&quot;);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; originalEmployee.Id, originalEmployee.Name, originalEmployee.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; deepCloneEmployee.Id, deepCloneEmployee.Name, deepCloneEmployee.Address.City);
 &nbsp;&nbsp;&nbsp;Console.WriteLine();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
C. Demonstrate the different clones of the Customer struct. </p>
<p class="MsoNormal">Demonstrate the difference between the shallow clone and the deep clone of the Customer struct.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
public static void CloneCustomer()
{
&nbsp;&nbsp;&nbsp; Address address = new Address { City = &quot;Los Angeles&quot; }; 
&nbsp;&nbsp;&nbsp;&nbsp;Customer originalCustomer = new Customer { Id = 201, Name = &quot;Kevin Brown&quot;, Address = address };


&nbsp;&nbsp;&nbsp; // Get a shallow copy of the originalCustomer and set the new values in the copy.
&nbsp;&nbsp;&nbsp; Customer shallowCloneCustomer = originalCustomer;
&nbsp;&nbsp;&nbsp; shallowCloneCustomer.Id = 202;
&nbsp;&nbsp;&nbsp; shallowCloneCustomer.Name = &quot;John Wood&quot;;
&nbsp;&nbsp;&nbsp; shallowCloneCustomer.Address.City = &quot;Boston&quot;;//Change the shallow copy's address information.


&nbsp;&nbsp;&nbsp; // Show the information of originalCustomer and the shallow copy.
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;It is the shallow clone of the Customer struct.&quot;);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; originalCustomer.Id, originalCustomer.Name, originalCustomer.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; shallowCloneCustomer.Id, shallowCloneCustomer.Name, originalCustomer.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine();


&nbsp;&nbsp;&nbsp; // Get a deep copy of the originalCustomer and set the new values in the copy.
&nbsp;&nbsp;&nbsp; address.City = &quot;Los Angeles&quot;;
&nbsp;&nbsp;&nbsp; Customer deepCloneCustomer = DeepCloneHelper.DeepClone(originalCustomer);
&nbsp;&nbsp;&nbsp; deepCloneCustomer.Id = 202;
&nbsp;&nbsp;&nbsp; deepCloneCustomer.Name = &quot;John Wood&quot;;
&nbsp;&nbsp;&nbsp; deepCloneCustomer.Address.City = &quot;Boston&quot;;  // Change the deep copy's address information.


&nbsp;&nbsp;&nbsp; // Show the information of originalCustomer and the deep copy.
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;It is the deep clone of the Customer struct.&quot;);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; originalCustomer.Id, originalCustomer.Name, originalCustomer.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;{0,-5} {1,-25} {2}&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; deepCloneCustomer.Id, deepCloneCustomer.Name, deepCloneCustomer.Address.City);
&nbsp;&nbsp;&nbsp; Console.WriteLine();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.OBJECT.MEMBERWISECLONE.aspx" target="_blank" title="Auto generated link to SYSTEM.OBJECT.MEMBERWISECLONE">SYSTEM.OBJECT.MEMBERWISECLONE</a>);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true"><span class="SpellE">MemberwiseClone</span>
 Method</a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.TYPE.aspx" target="_blank" title="Auto generated link to SYSTEM.TYPE">SYSTEM.TYPE</a>);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true">Type Class</a>
</p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.REFLECTION.FIELDINFO.aspx" target="_blank" title="Auto generated link to SYSTEM.REFLECTION.FIELDINFO">SYSTEM.REFLECTION.FIELDINFO</a>);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true"><span class="SpellE">FieldInfo</span>
 Class</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
