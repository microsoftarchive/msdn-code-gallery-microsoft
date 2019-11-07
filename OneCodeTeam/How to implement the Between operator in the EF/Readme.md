# How to implement the Between operator in the EF
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
## Topics
- Entity Framework
- Between
- Expression Tree
## Updated
- 03/22/2013
## Description

<h1>Implement the Between Operator in Entity Framework (<span class="SpellE">CSEFBetweenOperator</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample demonstrates how to implement the <span class="GramE">
Between</span> operator in Entity Framework.</p>
<p class="MsoNormal">In this sample, we use two ways to implement the Entity Framework Between
<span class="GramE">operator</span>:</p>
<p class="MsoNormal">1. Use the Entity SQL;</p>
<p class="MsoNormal">2. Use the extension method and expression tree.</p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you need to finish the following steps:</p>
<p class="MsoNormal">Step1. Attach the database file MySchool.mdf under the folder _External_Dependecies to your SQL Server 2008 database instance.</p>
<p class="MsoNormal">Step2. Modify the connection string in the App.config file according to your SQL Server 2008 database instance name.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span style=""><img src="78788-image.png" alt="" width="562" height="325" align="middle">
</span></p>
<p class="MsoNormal">First, we get the courses by Entity SQL. In the Entity SQL statement, we select the courses on the Department column which the value is between 1 and 5.
</p>
<p class="MsoNormal">Then we get the courses by extension method. In this statement, we select the courses on the CourseID column which the value is between C1050 and C3141.</p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Get the Courses by Entity SQL.</p>
<p class="MsoNormal">We select the courses on the Department column by Entity SQL.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
return school.Courses.Where(
&quot;it.DepartmentID between @lowerbound And @highbound&quot;,
new ObjectParameter(&quot;lowerbound&quot;, 1),
new ObjectParameter(&quot;highbound&quot;, 5)).ToList();

</pre>
<pre id="codePreview" class="csharp">
return school.Courses.Where(
&quot;it.DepartmentID between @lowerbound And @highbound&quot;,
new ObjectParameter(&quot;lowerbound&quot;, 1),
new ObjectParameter(&quot;highbound&quot;, 5)).ToList();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. Get the Courses by Extension Method.<span style="font-size:9.5pt; font-family:Consolas; color:green">
</span></p>
<p class="MsoNormal">We select the courses on the CourseID column by the Bwtween extension method. In this method, we need pass three parameters: lambda expression, low boundary of the value, high boundary of the value.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
return school.Courses.Between(c =&gt; c.CourseID, &quot;C1050&quot;, &quot;C3141&quot;).ToList();

</pre>
<pre id="codePreview" class="csharp">
return school.Courses.Between(c =&gt; c.CourseID, &quot;C1050&quot;, &quot;C3141&quot;).ToList();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
In the extension method, we use two expressions to implement the <span class="GramE">
Between</span> operation, and so we need to use the <span class="SpellE"><span style="color:black">Expression.LessThanOrEqual</span></span><span style="color:black"> and
<span class="SpellE">Expression.GreaterThanOrEqual</span> methods to return the two expressions.</span><span style="font-size:9.5pt; font-family:Consolas">
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span class="SpellE">Expression.LessThanOrEqual</span> and <span class="SpellE">
Expression.GreaterThanOrEqua</span> method are only used <span class="SpellE">inthe</span> numeric
<span class="SpellE">comparision</span>. If we want to compare the non-numeric type, we can't directly use the two methods.
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
So we first use the Compare method to compare the objects, and the Compare method will return an
<span class="SpellE">int</span> number. Then we can use the <span class="SpellE">
LessThanOrEqual</span> and <span class="SpellE">GreaterThanOrEqua</span> method.</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
For this reason, we ask all the <span class="SpellE">TKey</span> types implement the
<span class="SpellE">IComparable</span>&lt;&gt; interface.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
public static IQueryable&lt;TSource&gt; Between&lt;TSource, TKey&gt;
&nbsp;&nbsp;&nbsp;&nbsp; (this IQueryable&lt;TSource&gt; source,
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;Expression&lt;Func&lt;TSource, TKey&gt;&gt; keySelector,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; TKey low, TKey high) where TKey : IComparable&lt;TKey&gt;
{
&nbsp;&nbsp;&nbsp; ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource));


&nbsp;&nbsp;&nbsp; Expression body = keySelector.Body;
&nbsp;&nbsp;&nbsp; ParameterExpression parameter = null;
&nbsp;&nbsp;&nbsp; if (keySelector.Parameters.Count&gt;0)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; parameter = keySelector.Parameters[0];


&nbsp;&nbsp;&nbsp; MethodInfo compareMethod = typeof(TKey).GetMethod(&quot;CompareTo&quot;, new[] { typeof(TKey) });


&nbsp;&nbsp;&nbsp; Expression upper = Expression.LessThanOrEqual(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;Expression.Call(body, compareMethod, Expression.Constant(high)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Constant(0, typeof(int)));
&nbsp;&nbsp;&nbsp; Expression lower = Expression.GreaterThanOrEqual(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Call(body, compareMethod, Expression.Constant(low)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Constant(0, typeof(int)));


&nbsp;&nbsp;&nbsp; Expression andExpression = Expression.And(upper, lower);


&nbsp;&nbsp;&nbsp; MethodCallExpression whereCallExpression = Expression.Call(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; typeof(Queryable),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &quot;Where&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Type[] { source.ElementType },
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; source.Expression,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Lambda&lt;Func&lt;TSource, bool&gt;&gt;(andExpression,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new ParameterExpression[] { parameter }));
&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;return source.Provider.CreateQuery&lt;TSource&gt;(whereCallExpression);
}

</pre>
<pre id="codePreview" class="csharp">
public static IQueryable&lt;TSource&gt; Between&lt;TSource, TKey&gt;
&nbsp;&nbsp;&nbsp;&nbsp; (this IQueryable&lt;TSource&gt; source,
&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;Expression&lt;Func&lt;TSource, TKey&gt;&gt; keySelector,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; TKey low, TKey high) where TKey : IComparable&lt;TKey&gt;
{
&nbsp;&nbsp;&nbsp; ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource));


&nbsp;&nbsp;&nbsp; Expression body = keySelector.Body;
&nbsp;&nbsp;&nbsp; ParameterExpression parameter = null;
&nbsp;&nbsp;&nbsp; if (keySelector.Parameters.Count&gt;0)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; parameter = keySelector.Parameters[0];


&nbsp;&nbsp;&nbsp; MethodInfo compareMethod = typeof(TKey).GetMethod(&quot;CompareTo&quot;, new[] { typeof(TKey) });


&nbsp;&nbsp;&nbsp; Expression upper = Expression.LessThanOrEqual(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;Expression.Call(body, compareMethod, Expression.Constant(high)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Constant(0, typeof(int)));
&nbsp;&nbsp;&nbsp; Expression lower = Expression.GreaterThanOrEqual(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Call(body, compareMethod, Expression.Constant(low)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Constant(0, typeof(int)));


&nbsp;&nbsp;&nbsp; Expression andExpression = Expression.And(upper, lower);


&nbsp;&nbsp;&nbsp; MethodCallExpression whereCallExpression = Expression.Call(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; typeof(Queryable),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &quot;Where&quot;,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Type[] { source.ElementType },
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; source.Expression,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Expression.Lambda&lt;Func&lt;TSource, bool&gt;&gt;(andExpression,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new ParameterExpression[] { parameter }));
&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;return source.Provider.CreateQuery&lt;TSource&gt;(whereCallExpression);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/bb506649(v=vs.100).aspx"><span class="SpellE">System.Linq.Expressions</span> Namespace</a></p>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/4d7sx9hd(v=vs.100).aspx"><span class="SpellE">IComparable</span>&lt;T&gt; Interface</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
