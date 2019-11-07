# Excel 2010: Writing Asynchronous User-Defined Functions
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Excel 2010
- Office 2010
## Topics
- user-defined functions
- asynchronous user-defined functions
## Updated
- 07/27/2011
## Description

<h2><strong>Introduction</strong></h2>
<p>Learn how to create an asynchronous user-defined function (UDF) within an XLL. Microsoft Excel 2010 introduces the ability to define an asynchronous UDF within an XLL. This sample accompanies the article
<a href="http://msdn.microsoft.com/en-us/library/ff796219.aspx">Writing Asynchronous User-Defined Functions in Excel 2010</a> in the MSDN Library.</p>
<h2><strong>Description</strong></h2>
<p>Excel 2010 introduces the ability to create asynchronous UDFs within an XLL. You might want to use an asynchronous UDF if the operation might wait on a query or calculation, which can be an issue for network operations. In such a case, instead of blocking
 Excel on each call, you could start many asynchronous operations in parallel.</p>
<p>An Excel calculation thread calls UDFs in a serial fashion, one after the other, waiting for each call to complete before proceeding down the calculation chain. Asynchronous UDFs return control quickly to the calling thread, and then send the calculation
 result to Excel later, on a separate thread. Asynchronous UDFs are ideal for any function that takes a long time to complete, but does not actually perform local, intensive processor operations.</p>
<p>This code sample bases its asynchronous UDF on <strong>XllEcho</strong>, which is a synchronous UDF.
<strong>XllEcho</strong> simulates an external operation and takes one second to complete a calculation. The function takes a single parameter, sleeps for one second, and returns the original input value (multiplied by 2 if it is a number).</p>
