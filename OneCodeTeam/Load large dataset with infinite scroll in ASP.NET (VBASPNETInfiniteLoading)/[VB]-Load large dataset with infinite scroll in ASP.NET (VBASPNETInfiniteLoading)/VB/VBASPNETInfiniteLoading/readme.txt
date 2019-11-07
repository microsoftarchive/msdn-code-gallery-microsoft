========================================================================
  ASP.NET APPLICATION :  VBASPNETInfiniteLoading Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

Infinite scroll, has also been called autopagerize, unpaginate, endless pages. 
But essentially it is pre-fetching content from a subsequent page and adding 
it directly to the user’s current page.  The code sample demonstrates loading 
a large number of data entries in an XML file. It support infinite scroll 
with the AJAX technology.


/////////////////////////////////////////////////////////////////////////////
Demo the Sample. 

Open the VBASPNETInfiniteLoading.sln directly, expand the web application 
node and press F5 to test the application.

Step 1.  View default.aspx in browser

Step 2.  By default, we could see a vertical scroll on the page, just drag it 
         scroll down, you will find the new content load infinitely meanwhile 
         the scroll bar becomes small and small.
	 note: if there is no vertical scroll bar on page, just do appropriate
	 scaling for the page till the vertical scroll bar appeared.



/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step 1.  Create a VB ASP.NET Empty Web Application in Visual Studio 2010.


Step 2.  Create a new directory, "Scripts". Right-click the directory and click
         Add -> New Item -> JScript File. We need to reference jquery javascript 
	 library files jquery-1.4.1.min.js


Step 3.  Create a new directory, "Styles". Right-click the directory and click
         Add -> New Item -> Style Sheet File. reference site.css.
		 

Step 4.  Open the Default.aspx,(If there is no Default.aspx, create one.)
         In the Head block, add javascript and style references like below.

	 [CODE]    	
    	 <link rel="stylesheet" href="Styles/Site.css" type="text/css" />
         <script type="text/javascript" src="Scripts/jquery-1.4.1.min.js"></script>
	 [/CODE]

	 write the autocomplete javascript as below.
	 [CODE]
	
         $(document).ready(function () {

            function lastPostFunc() {
                $('#divPostsLoader').html('<img src="images/bigLoader.gif">');

                //send a query to server side to present new content
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/Foo",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {

                        if (data != "") {
                            $('.divLoadData:last').after(data.d);
                        }
                        $('#divPostsLoader').empty();
                    }

                })
            };

            //When scroll down, the scroller is at the bottom with the function below and fire 
	    the lastPostFunc function
            $(window).scroll(function () {
                if ($(window).scrollTop() == $(document).height() - $(window).height()) {
                    lastPostFunc();
                }
            });

         });
    
	 [CODE]		
		 
	 For more details, please refer to the Default.aspx in this sample.

Step 7.  Everything is ready, test the application by scrolling down the page to see what happens. 


/////////////////////////////////////////////////////////////////////////////
References:

http://www.webresourcesdepot.com/load-content-while-scrolling-with-jquery/ 


/////////////////////////////////////////////////////////////////////////////