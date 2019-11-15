# Deep clone the Entity objects using reflection [C#-Visual Studio 2012]
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- ADO.NET
## Topics
- Reflection
- Serialization
- Entity Object Clone
## Updated
- 01/25/2013
## Description

<p class="MsoNormalCxSpFirst" style="margin-top:24.0pt; line-height:115%"><b><span style="font-size:14.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">How to deep clone/duplicate the Entity objects using reflection (CSEFDeepCloneObject)
</span></b></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Introduction
</span></b></p>
<p class="MsoNormal" style="margin-bottom:3.0pt"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">This sample demonstrates how to implement deep clone/duplicate entity objects using serialization and reflection.
</span></p>
<p class="MsoNormal" style="margin-bottom:3.0pt"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">When we need some similar entity objects, we can create them manually. If there are many properties in the entity types, it means more codes.<span style="">&nbsp;
</span>If we use the MemberwiseClone method to copy the entity objects, it just returns a shallow copy of the current object. In this sample, we use serialization and reflection to deep clone entity objects and their related entity objects, and then store the
 new objects and their related entity objects in the database. </span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Building the Sample
</span></b></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">1. Please run the EFCloneDB.sql script in your sqlserver.
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">2. Please modify the connection string in the App.config according to your database instance name.
</span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Running the Sample
</span></b></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">1．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Show the list of employees
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.25in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.25in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="75158-image.png" alt="" width="632" height="600" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.25in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">2．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Create the employee information
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="75159-image.png" alt="" width="502" height="294" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">3．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Create the employee's sales information
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="75160-image.png" alt="" width="370" height="234" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">4．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Employee's information before clone
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="75161-image.png" alt="" width="630" height="601" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">5．<span style="font:7.0pt &quot;Times New Roman&quot;">
</span></span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Employee's information after clone
</span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="75162-image.png" alt="" width="631" height="599" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormalCxSpMiddle" style="margin-top:0in; margin-right:0in; margin-bottom:10.0pt; margin-left:.5in; line-height:115%">
<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">Using the Code
</span></b></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">1. Create an ADO.NET Entity Data Model
</span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;&nbsp;
</span>1) Name it EmpModel.edmx. </span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;&nbsp;
</span>2) Set the connection string information of the EFDemoDB database. </span>
</p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;&nbsp;
</span>3) Select all the data tables. </span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">2. Create a static class DpCloneHelper used to define some extension methods for the Entity Framework's object<span style="">&nbsp;
</span>EntityObject. </span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;
</span>1) Create an extension method clone </span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">using serialization to implement the deep clone for an object which the type is EntityObject.
</span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
/// Extension method to Enitity Object. 
/// Deep clone the Object.
/// &lt;/summary&gt;
/// &lt;param name=&quot;source&quot;&gt;Entity Object need to be cloned &lt;/param&gt;
/// &lt;returns&gt;The Cloned object&lt;/returns&gt;
public static T Clone&lt;T&gt;(this T source) where T:EntityObject&nbsp; 
{ 
&nbsp;&nbsp;&nbsp;&nbsp;var ser = new DataContractSerializer(typeof(T)); 
&nbsp;&nbsp;&nbsp;&nbsp;using (var stream = new MemoryStream())
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ser.WriteObject(stream, source);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; stream.Seek(0, SeekOrigin.Begin);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return (T)ser.ReadObject(stream); 
&nbsp;&nbsp;&nbsp;&nbsp;} 
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><span style="">&nbsp;</span>2) Create the below methods to clear the Entity Reference on the cloned Entity. The cloned Entity will be attached to the object only after
 the Entity References are cleared. The cloned object should be treated as new data and should create new Primary Keys and associate with Referential Integrity.
</span></p>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
///&nbsp; Extension method of Entity Object This will clear the Entity of Object and all related Child Objects 
/// &lt;/summary&gt;
/// &lt;param name=&quot;source&quot;&gt;Entity Object need to clear&lt;/param&gt;
/// &lt;param name=&quot;bcheckHierarchy&quot;&gt;This Parameter will define to clear enitty of all Child Object or not&lt;/param&gt;
/// &lt;returns&gt;&lt;/returns&gt;
public static EntityObject ClearEntityReference(this EntityObject source, bool bCheckHierarchy)
{
&nbsp;&nbsp;&nbsp; return source.ClearEntityObject(bCheckHierarchy);
}


/// &lt;summary&gt;
///&nbsp; Extension method of Entity Object This will clear the Entity of Object and all related Child Objects 
/// &lt;/summary&gt;
/// &lt;param name=&quot;source&quot;&gt;Entity Object need to clear&lt;/param&gt;
/// &lt;param name=&quot;bcheckHierarchy&quot;&gt;This Parameter will define to clear enitty of all Child Object or not &lt;/param&gt;
/// &lt;returns&gt;&lt;/returns&gt;
private static T ClearEntityObject&lt;T&gt;(this&nbsp; T source, bool bCheckHierarchy) where T : class
{
&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; // Throw if passed object has nothing
&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; if (source == null) 
&nbsp;&nbsp;&nbsp;&nbsp;{ 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;throw new Exception(&quot;Null Object cannot be cloned&quot;); 
&nbsp;&nbsp;&nbsp;&nbsp;}


&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; // Get the Type of passed object 
&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp; Type tObj = source.GetType();


&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; // Check object passed does not have entity key Attribute 
&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp; if (tObj.GetProperty(&quot;EntityKey&quot;) != null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; tObj.GetProperty(&quot;EntityKey&quot;).SetValue(source, null, null);
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; // bCheckHierarchy is used to check and clear child object releation keys 
&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp; if (!bCheckHierarchy)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return (T)source;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; // Clearing the Entity for Child Objects 
&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp; List&lt;PropertyInfo&gt; PropertyList = (from a in source.GetType().GetProperties()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where a.PropertyType.Name.Equals(&quot;ENTITYCOLLECTION`1&quot;,StringComparison.OrdinalIgnoreCase)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select a).ToList();


&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp; // Loop the list of Child Object and Clear the Entity Reference 
&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp; foreach (PropertyInfo prop in PropertyList)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; IEnumerable keys = (IEnumerable)tObj.GetProperty(prop.Name).GetValue(source, null);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (object key in keys)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; //Clearing Entity Reference from Parnet Object 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var childProp = (from a in key.GetType().GetProperties()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where a.PropertyType.Name.Equals(&quot;EntityReference`1&quot;,StringComparison.OrdinalIgnoreCase)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select a).SingleOrDefault();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; childProp.GetValue(key, null).ClearEntityObject(false);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; //
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Recrusive clearing the the Entity Reference from Child object 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;//
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; key.ClearEntityObject(true);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; return (T)source;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoNormal" style="margin-top:10.0pt; line-height:115%"><b><span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">More Information
</span></b></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">ADO.NET Entity Framework
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><a href="http://msdn.microsoft.com/en-us/library/bb399572.aspx">http://msdn.microsoft.com/en-us/library/bb399572.aspx</a>
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Serialization
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><a href="http://msdn.microsoft.com/en-us/library/7ay27kt9(v=vs.100).aspx">http://msdn.microsoft.com/en-us/library/7ay27kt9(v=vs.100).aspx</a>
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Reflection
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><a href="http://msdn.microsoft.com/en-us/library/f7ykdhsy.aspx">http://msdn.microsoft.com/en-us/library/f7ykdhsy.aspx</a>
</span></p>
<p class="MsoNormal"><span style=""></span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
