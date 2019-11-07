


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1"><link href="http://i1.code.msdn.s-msft.com/RequestReduceContent/0d02cd2a5932a94a79386bbeca159fef-d55bbc9bfd71a63c47258ef291f4e1c9-RequestReducedStyle.css" rel="Stylesheet" type="text/css" />
    <meta name="description" content="This sample illustrates how to integrate support for a new language into Visual Studio." />
    <meta name="Search.BRAND" content="Msdn"/>
    <meta name="Search.IROOT" content="msdn"/>

    <link rel="shortcut icon" href="http://i1.code.msdn.s-msft.com/GlobalResources/images/Msdn/favicon.ico" type="image/x-icon" /> <meta name="CommunityInfo" content=" B=Msdn;A=Samples;L=en-US;" />
    <script src="http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.7.1.min.js" type="text/javascript" language="javascript"></script>
    <script src="http://i1.code.msdn.s-msft.com/RequestReduceContent/3fb79202141d1aeed1ae6bba90b0e7a8-37695a141fc2027e3d2da4f43de2397b-RequestReducedScript.js" type="text/javascript" ></script>    
    <title>IronPython Integration in C# for Visual Studio 2010</title> 
        <link href="/site/feeds/searchRss" rel="Alternate" type="application/rss+xml"
        title="Sample Code - MSDN Examples in C#, VB.NET, C++, JavaScript, F#" />
    <link rel="P3Pv1" href="/W3C/p3p.xml" />
    
    
            <link rel="canonical" href="http://code.msdn.microsoft.com/windowsdesktop/IronPython-Integration-6b03988d" />
    
    <meta name="ContentLanguage" content="L=en-US;"/>

<!--
Third party scripts and code linked to or referenced from this website are licensed to you by the parties that own such code, not by Microsoft.  See ASP.NET Ajax CDN Terms of Use – http://www.asp.net/ajaxlibrary/CDN.ashx.
-->
</head>
<body class=" en-us Samples " dir= "ltr">
    
    <div id="BodyBackground" class="ltr">
        <div id="JelloSizer">    
            <div id="JelloExpander">
                <div id="JelloWrapper">
                    <div class="Clear"> </div>
                    
                   

<div data-chameleon-template="epxheader"></div>
<div data-chameleon-template="header">



    
    <div id="ux-header" class=" ltr" xmlns="http://www.w3.org/1999/xhtml">
        <div class="BrandLogo">
            <a href="http://msdn.microsoft.com/en-us">
                <span class="BrandLogoImage msdn" title="MSDN"></span>
            </a>
        </div>
        <div class="GlobalBar">
            <div id="LocaleSelector">
                
<a title="Change your language" id="SelectLocale" href="http://msdn.microsoft.com/en-us/SelectLocale?fromPage=http%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d">United States (English)</a>
        
            
            
            </div>

            

        <div class="SignedOutProfileElement"><a class="createProfileLink" href="" title=""></a></div>
    <div class="signIn"><a class="scarabLink" href="https://login.live.com/login.srf?wa=wsignin1.0&amp;wtrealm=code.msdn.microsoft.com&amp;wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%3fstoAI%3d10&amp;wp=MBI_FED_SSL&amp;wlcxt=microsoft%24microsoft%24microsoft" title="Sign in">Sign in</a></div>

                
        </div>
        <div class="ux-mtps-internav">
            

    

<div class="SearchBox">
              <form id="HeaderSearchForm" name="HeaderSearchForm" method="get" action="http://social.msdn.microsoft.com/Search" onsubmit="return Epx.Controls.SearchBox.searchBoxOnSubmit(this, this.title);">
            <input id="HeaderSearchTextBox" name="query" type="text" maxlength="200" onfocus="Epx.Controls.SearchBox.watermarkFocus(this, this.title, 'TextBoxSearch')" onblur="Epx.Controls.SearchBox.watermarkBlur(this, this.title, 'TextBoxSearch')" />
            <input id="RefinementId" name="refinement" type="hidden" value="" />
            <button id="HeaderSearchButton" value="" type="submit" class="header-search-button"></button>
        </form>
        
    
</div>

            <div class="TocNavigation">
   
   <div class="toclevel1">
          <a class="normal" href="http://msdn.microsoft.com/" title="Home">Home</a>
          <a class="normal" href="http://msdn.microsoft.com/library/default.aspx" title="Library">Library</a>
          <a class="normal" href="http://msdn.microsoft.com/bb188199" title="Learn">Learn</a>
          <a class="normal" href="http://code.msdn.microsoft.com/" title="Samples">Samples</a>
          <a class="normal" href="http://msdn.microsoft.com/aa570309" title="Downloads">Downloads</a>
          <a class="normal" href="http://msdn.microsoft.com/hh361695" title="Support">Support</a>
          <a class="normal" href="http://msdn.microsoft.com/aa497440" title="Community">Community</a>
          <a class="normal" href="http://social.msdn.microsoft.com/forums/en-us/categories" title="Forums">Forums</a>

    </div>
  
</div>
        </div>
    </div>
<script type="text/javascript" xmlns="http://www.w3.org/1999/xhtml">
/*<![CDATA[*/
/**/
(window.MTPS || (window.MTPS = {})).cdnDomains || (window.MTPS.cdnDomains = { 
	"image": "http://i.msdn.microsoft.com", 
	"js": "http://i2.msdn.microsoft.com", 
	"css": "http://i3.msdn.microsoft.com"
});
/**/
/*]]>*/
</script><script type="text/javascript" src="http://i2.msdn.microsoft.com/Areas/Epx/Themes/Base/Content/SearchBox.js" xmlns="http://www.w3.org/1999/xhtml"></script><script type="text/javascript" src="http://i2.services.social.microsoft.com/search/Widgets/SearchBox.jss?boxid=HeaderSearchTextBox&amp;btnid=HeaderSearchButton&amp;brand=MSDN&amp;loc=en-us&amp;watermark=MSDN&amp;focusOnInit=false" xmlns="http://www.w3.org/1999/xhtml"></script></div>
                    
                    
                    <div class="Clear"> </div>
                          
                    <div class="topleftcorner"></div>           
                    <div class="toprightcorner"></div>                                
                    <div class="alley"> <!-- Left Side -->
                        <div class="wrapper"> <!-- Right Side -->
                            <div class="inner">  
                            
                                                 
                                <div class="Clear"></div>   


                                
                                
                                
                                
                                <div id="MainContent" class="Msdn">
                                    
    
    

    <script language="javascript" type="text/javascript">
        $(function () { Galleries.cookie.setTimezoneOffsetCookie(); })
    </script>

    <div id="container">
        
            <div id="GalleriesNavigation">
                
            </div>            
        
        
            <div id="subHeader">
                <div id="eyebrow" class="left">
                    








<div class="EyebrowContainer">
    
    

    
<div itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="EyebrowElement">
    <a href=http://msdn.microsoft.com/en-US/ itemprop="url">
        <span itemprop="title">Microsoft Developer Network</span>
    </a>
</div>
<text>></text>


    <div itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="EyebrowElement">
             <a href="/" itemprop="url">
                <span itemprop="title">Samples</span>
            </a> 
    </div>


    >
    <div itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="EyebrowElement">
             <a href="/IronPython-Integration-6b03988d" itemprop="url" class="EyebrowLeafLink">
                <span itemprop="title">IronPython Integration</span>
            </a> 
    </div>
</div>
                </div>
            </div>
            <div class="clear"></div>
        
        
            <div id="siteMessage">
                






            </div>
            <div class="clear"></div>
        
        

<div class="subMenu">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.internav > a').each(function (index, elem) {
                if ($(elem).attr('href').toUpperCase().indexOf('CODE') != -1) {
                    $(elem).addClass('active');
                }
                if ($(elem).attr('href').toUpperCase().indexOf('SMPGDB') != -1) {
                    $(elem).addClass('active');
                }

            });
        });
    </script>
</div>      
        <div class="clear"></div>

        <div id="bodyContainer">
            
    
    <div id="projectPage">
        <div class="sidebar">
            <div data-chameleon-template="actionbox"></div>
            


<div class="section" id="contributeSection">
<div class="contributeAction">
    <a href="http://www.microsoft.com/visualstudio/en-us"><img  src="http://i1.code.msdn.s-msft.com/content/common/download.png" />Download Visual Studio</a>
</div>
    <h3>Quick Access</h3>

<div id="myContributionsAction">
    <div>
        <a id="myContributionsLink" href="/site/mydashboard">
            My Samples
        </a>
    </div>
</div>

<div id="uploadAction">
    <a id="uploadLink" href="/site/upload">Upload a sample</a>
</div>

    <div id="requestAction">
        <a  id="exclamationLink" href="/site/requests">Browse Sample Requests</a>
    </div>
</div>



<div id="userCard" class="section">
    <div class="profile-usercard-inline" 
         data-profile-usercard-customLink='{"href":"/site/search?f%5B0%5D.Type=User&f%5B0%5D.Value=Microsoft%20-%20Visual%20Studio%20Platform%20Team","text":"View Samples"}' 
         data-profile-userid="7fe1fc6c-49c3-4b22-8e91-f9750949b5d6">
    </div>
</div>
<script language="javascript" type="text/javascript">
    if (true)
        Galleries.utility.waitFor(
            function() { $('#userCard .profile-statline, #userCard .profile-usercard-profileLink').hide(); });
</script>
            <div id="moreFromAuthor" class="collapsableSidebar">
                <div data-url="/IronPython-Integration-6b03988d/AuthorProjects" id="authorProjectContainer"></div>
            </div>
        </div>

        <div id="projectDetails">
            <div>
                


<h1 class="projectTitle">IronPython Integration</h1>
<h2 class="projectSummary">This sample illustrates how to integrate support for a new language into Visual Studio.</h2>
<div class="clear" ></div>

    <div id="Downloads">
        <div>
            <label for="Downloads">Download</label>
        </div>
        
                    <a href="/IronPython-Integration-6b03988d/file/43/8/IronPython%20Integration.zip" data-url="/IronPython-Integration-6b03988d/file/43/8/IronPython%20Integration.zip" class="button">C# (1.6 MB)</a>
        
    </div>

<div class="clear"></div>

<div class="clear"></div>
<div id="projectInfo">
    <div id="sectionLeft" class="section">
        <div class="itemBar">
            <label for="ReviewValue">Ratings</label>                   
            
    <div id="yourRating"">
        <form action="/IronPython-Integration-6b03988d/createreview" id="reviewForm" method="post" name="reviewForm"><input id="Rating" name="Rating" type="hidden" value="" />
            <input id="Text" name="Text" type="hidden" value="" />
        <div class="Rating stackLeft EditStarMode" id="Star_Span" onmouseout="onStarsMouseOut()">
        
            <div id="Star_0" class="RatingStar FilledRatingStar"
                data-initialstarclass="FilledRatingStar" 
                onmouseover="onStarMouseOver('Star_0')" onclick="onStarClick(0)"
                title="1">
                &nbsp;</div>
        
            <div id="Star_1" class="RatingStar FilledRatingStar"
                data-initialstarclass="FilledRatingStar" 
                onmouseover="onStarMouseOver('Star_1')" onclick="onStarClick(1)"
                title="2">
                &nbsp;</div>
        
            <div id="Star_2" class="RatingStar FilledRatingStar"
                data-initialstarclass="FilledRatingStar" 
                onmouseover="onStarMouseOver('Star_2')" onclick="onStarClick(2)"
                title="3">
                &nbsp;</div>
        
            <div id="Star_3" class="RatingStar FilledRatingStar"
                data-initialstarclass="FilledRatingStar" 
                onmouseover="onStarMouseOver('Star_3')" onclick="onStarClick(3)"
                title="4">
                &nbsp;</div>
        
            <div id="Star_4" class="RatingStar EmptyRatingStar"
                data-initialstarclass="EmptyRatingStar" 
                onmouseover="onStarMouseOver('Star_4')" onclick="onStarClick(4)"
                title="5">
                &nbsp;</div>
        
        </div>
        <div style="clear:both;"></div> 
        </form>
    </div>
    
    <div id="RatingCount">(2)</div>
    

<script language="javascript">
    function onStarMouseOver(id) {
        var id_prefix = id.substring(0, id.lastIndexOf('_') + 1);
        var index = parseInt(id.substring(id.lastIndexOf('_') + 1, id.length));

        for (i = 0; i < 5; i++) {
            var star = $('#' + id_prefix + i);
            var initialRatingClass = star.data('initialstarclass');
            if (i <= index)                star.removeClass(initialRatingClass).removeClass('EmptyRatingStar').addClass('FilledRatingStarHover');            else                star.removeClass(initialRatingClass).removeClass('FilledRatingStarHover').addClass('EmptyRatingStar');
        }
        
    }
    function onStarsMouseOut() {
        var stars = $("#RatingStar");
        var id_prefix = "Star_";
        var currentRating = 4;
        
        for (i = 0; i < 5; i++) {
            var star = $('#' + id_prefix + i.toString());
            var initialRatingClass = star.data('initialstarclass');
            star.removeClass('EmptyRatingStar').removeClass('FilledRatingStarHover').addClass(initialRatingClass);
        }
    }
    
    function onStarClick(index) {
        $('#Rating').val(index + 1);
        document.getElementById("reviewForm").submit();
    }
</script>
                                   
        </div>
        
            <div class="itemBar">
                <label for="DownloadCount">Downloaded</label>
                <span id="DownloadCount">3,013 times</span>
            </div>
        
            <div class="itemBar">
                <label for="Favorites">Favorites</label>
                
<script language="javascript">
    $(function () { $('a#addFavoriteLink').click(function () { $('form#favoriteform').submit(); }); });
</script>
<span id="Collections">

        <a href="https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=code.msdn.microsoft.com&wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%3fstoAI%3d10&wp=MBI_FED_SSL&wlcxt=microsoft%24microsoft%24microsoft">Add To Favorites</a>

</span>

            </div>
        


    <div class="itemBarLong">
        <label for="Requires">Requires</label>
        <div id="Requires">
                <a href="/site/search?f%5B0%5D.Type=VisualStudioVersion&amp;f%5B0%5D.Value=10.0&amp;f%5B0%5D.Text=Visual%20Studio%202010">Visual Studio 2010</a>

        </div>
    </div>
        
        
    </div>

    <div id="sectionRight" class="section">
        <div class="itemBar">
            <label for="LastUpdated">
                Last Updated</label>
            <span id="LastUpdated">
                11/29/2011</span>
        </div>
        
            <div class="itemBarLong">
                <label for="License">
                    License</label>
                <div id="License"><a href="javascript:showEula()">Apache License, Version 2.0</a></div>
            </div>
        
        <div class="itemBar">
            <label for="LastUpdated">
                Share</label>
            <span>
                
<script language="javascript" type="text/javascript">
    $(function () {
        $('.shareThisItem > a').WireTrackerForEvent('click', 'http://code.msdn.microsoft.com/Content/Common/tracker.gif', 'ShareItClick', 'IronPython-Integration-6b03988d', true);
    });
</script>

<div class="shareThisItem">
 
<a href ="mailto:?subject=IronPython%20Integration&amp;Body=http://code.msdn.microsoft.com/IronPython-Integration-6b03988d" target="_blank"><img alt="E-mail" src="http://i1.code.msdn.s-msft.com/content/common/sharethis/email.gif" /></a>

 
<a href ="http://twitter.com/home?status=IronPython%20Integration&#32;:&#32;http%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d" target="_blank"><img alt="Twitter" src="http://i1.code.msdn.s-msft.com/content/common/sharethis/twitter.gif" /></a>

 
<a href ="http://del.icio.us/post?url=http%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d&amp;title=IronPython%20Integration" target="_blank"><img alt="del.icio.us" src="http://i1.code.msdn.s-msft.com/content/common/sharethis/delicious.gif" /></a>

 
<a href ="http://digg.com/submit?url=http%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d&amp;title=IronPython%20Integration" target="_blank"><img alt="Digg" src="http://i1.code.msdn.s-msft.com/content/common/sharethis/digg.gif" /></a>

 
<a href ="http://www.facebook.com/sharer.php?u=http%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d&amp;t=IronPython%20Integration" target="_blank"><img alt="Facebook" src="http://i1.code.msdn.s-msft.com/content/common/sharethis/facebook.gif" /></a>


</div></span>
        </div>
        
        
    </div>
    <div class="clear"></div>
    <div class="section">
    




    <div class="itemBarLong tagsContainer">
        <label for="Technologies">Technologies</label>
        <div id="Technologies">
            <a href="/site/search?f%5B0%5D.Type=Technology&amp;f%5B0%5D.Value=Visual%20Studio%202010%20SDK&amp;f%5B0%5D.Text=Visual%20Studio%202010%20SDK">Visual Studio 2010 SDK</a>
        </div>
    </div>
    <div class="itemBarLong tagsContainer">
        <label for="Topics">Topics</label>
        <div id="Topics">
            <a href="/site/search?f%5B0%5D.Type=Topic&amp;f%5B0%5D.Value=IronPython%20Integrated%20Shell&amp;f%5B0%5D.Text=IronPython%20Integrated%20Shell">IronPython Integrated Shell</a>, <a href="/site/search?f%5B0%5D.Type=Topic&amp;f%5B0%5D.Value=VSX&amp;f%5B0%5D.Text=VSX">VSX</a>
        </div>
    </div>


    <a name="content"></a>
    <div class="itemBar">
            <span id="reportAbuse">
                <a href="https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=code.msdn.microsoft.com&wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%2fview%2fReportAbuse%3fstoAI%3d10&wp=MBI_FED_SSL&wlcxt=microsoft%24microsoft%24microsoft">Report Abuse to Microsoft</a>
            </span>
        </div>
    </div>
</div>

<script src="http://i1.code.msdn.s-msft.com/RequestReduceContent/07276395fc64606ca14b8a36563967ea-7af9a874efbc1f5aa0062aba1a48cd8f-RequestReducedScript.js" type="text/javascript" ></script><script type="text/javascript">

    $(function () {
        $('#Downloads input').click(function () {
            window.location.href = $(this).data('url');
         });
    });

    function rateItReviewIt() {
        displayCreateReview();
        $('#Reviews_Tab').click();
    }

    function showEula() {
        $.get('\x2fsite\x2fIronPython-Integration-6b03988d\x2feulapartial\x3flicenseType\x3dApache', function(data){
            $.prompt(data, {
                prefix: 'eulaPage',
                top: '15%',
                buttons: { 'Close': true }   
            });
        });
    };

</script>


            </div>
        </div>
        





<div id="projectBody">
    <div id="projectContent">
        <div class="tabHeaders">
            <div id="Details_Tab" class = current><a href="/IronPython-Integration-6b03988d#content" >Description</a></div>
            
        <div id="SourceCode_Tab"  class = ><a href="/IronPython-Integration-6b03988d/view/SourceCode#content" >Browse Code</a></div>

                <div id="Discussions_Tab"  class = ><a href="/IronPython-Integration-6b03988d/view/Discussions#content" >Q and A (1)</a></div>
        </div>
        <div class="tabContents">
                <div id="Details_Content">
                            <iframe id="longdescIframe" frameborder="0" width="100%" scrolling="no" marginwidth="0" marginheight="0" src="/IronPython-Integration-6b03988d/description"></iframe>
                    
                </div>
            
            
    


        </div>
    </div>
</div>    
    </div>
    <script language="javascript" type="text/javascript">

        $(document).ready(function() {
	    if (StreamEvents) {
                window.gTracker = StreamEvents.tracker.create(null, {
                    beaconUrl: 'setrack.gif',
                    pageEventType: 'GalleriesPage',
                    pageActionEventType: 'GalleriesPageAction',
                    pageEventArg: {
                        Brand: 'Samples',
                        Page: 'ProjectDetails',
                        Section: null,
                        PageContent: 'IronPython-Integration-6b03988d',
                        PageContentType: 'Project'
                    }
                });
             }


             registerPageView();
            
             Galleries.project.getAuthorProjects();
            
             loadTabData('Details');

             var reportAbuseFormUrl = '\x2fIronPython-Integration-6b03988d\x2freportabuse';
             $("#reportAbuse a[href='']").click(function() {
                    $('#projectContent').load(reportAbuseFormUrl);
                    return false;
                });
            
             
        });
        
         function loadTabData(tabId) {
                if ((tabId == 'SourceCode')) {
                    $('div.collapsableSidebar').addClass('hiddenSidebar');
                    $('#projectBody .tabContents').addClass('fullProjectBody');
                    Galleries.utility.waitFor(
                        function(){ return $("#userCard .profile-statline").length > 0;},
                        function(){$("#userCard .profile-biography, #userCard .profile-statline, #userCard .profile-footer, #userCard .profile-inline-secondary").addClass('hiddenSidebar'); });
                    
                }
                else {
                    $('div.collapsableSidebar, #userCard .profile-biography, #userCard .profile-statline, #userCard .profile-footer, #userCard .profile-inline-secondary').removeClass('hiddenSidebar');
                    $('#projectBody .tabContents').removeClass('fullProjectBody');
                }

                Galleries.project.resizeDesc();
        }

        function registerPageView() {
            $.post('\x2fIronPython-Integration-6b03988d\x2fstats\x2fRegisterPageView');
        }
    </script>


        </div>
        <div class="clear">
        </div>
    </div>
    <div class="advertisment">
        
    </div>

                                    <div class="Clear"></div>
                                </div>
    
                            </div>
                        </div>
                    </div>
                
                    <div class="Clearbottom"></div>          
                    <div class="bottomleftcorner"></div>            
                    <div class="bottomrightcorner"></div> 
                    
                     

<div data-chameleon-template="epxfooter" ></div>
<div data-chameleon-template="footer" >
   <div id="ux-footer" class=" ltr" xmlns="http://www.w3.org/1999/xhtml">
    <div id="ux-footer-cols">
        

        <div class="TocNavigation">
            <div class="linksContainer">
                    <ul class="links" style="width: 19.99%;">
                        <li class="linksTitle">
Dev Centers                        </li>
                                <li><a href="http://msdn.microsoft.com/vstudio/default.aspx" title="Visual Studio">Visual Studio</a></li>
                                <li><a href="http://msdn.microsoft.com/en-us/windows/apps/default.aspx" title="Windows">Windows</a></li>
                                <li><a href="https://dev.windowsphone.com/" title="Windows Phone">Windows Phone</a></li>
                                <li><a href="http://www.windowsazure.com/en-us/develop/overview/" title="Windows Azure">Windows Azure</a></li>
                                <li><a href="http://msdn.microsoft.com/office/default.aspx" title="Office">Office</a></li>
                                <li><a href="http://msdn.microsoft.com/sqlserver/default.aspx" title="SQL Server">SQL Server</a></li>
                                <li><a href="http://msdn.microsoft.com/en-us/aa937802" title="More...">More...</a></li>
                    </ul>
                    <ul class="links" style="width: 19.99%;">
                        <li class="linksTitle">
Resources                        </li>
                                <li><a href="http://msdn.microsoft.com/subscriptions/default.aspx" title="MSDN Subscriptions">MSDN Subscriptions</a></li>
                                <li><a href="http://code.msdn.microsoft.com/" title="Code Samples">Code Samples</a></li>
                                <li><a href="http://social.msdn.microsoft.com/forums/en-us/categories" title="MSDN Forums">MSDN Forums</a></li>
                                <li><a href="http://msdn.microsoft.com/magazine/default.aspx" title="MSDN Magazine">MSDN Magazine</a></li>
                                <li><a href="http://msdn.microsoft.com/aa570311" title="MSDN Flash Newsletter">MSDN Flash Newsletter</a></li>
                    </ul>
                    <ul class="links" style="width: 19.99%;">
                        <li class="linksTitle">
Programs                        </li>
                                <li><a href="http://www.microsoft.com/bizspark/" title="BizSpark (for startups)">BizSpark (for startups)</a></li>
                                <li><a href="https://www.dreamspark.com/" title="DreamSpark (for students)">DreamSpark (for students)</a></li>
                                <li><a href="https://www.microsoft.com/faculty" title="School faculty">School faculty</a></li>
                    </ul>
                    <ul class="links" style="width: 19.99%;">
                        <li class="linksTitle">
Learn                        </li>
                                <li><a href="https://msevents.microsoft.com/cui/searchdisplay.aspx?culture=en-us&amp;tgtaudhero=2#culture=en-us;advanced=mnp;pagenumber=1;sortkey=date;sortorder=asc;filtertype=4;pageevent=true;hdninitialcount=1;audience=50,11,43,42,36,55,26,18;audiencehero=2;eventtype=0;searchcontrol=yes;s=1" title="Virtual Labs">Virtual Labs</a></li>
                                <li><a href="http://channel9.msdn.com/" title="Channel 9">Channel 9</a></li>
                                <li><a href="http://www.microsoftvirtualacademy.com/" title="Microsoft Virtual Academy">Microsoft Virtual Academy</a></li>
                    </ul>
                    <ul class="links" style="width: 19.99%;">
                        <li class="linksTitle">
Get started for free                        </li>
                                <li><a href="http://tfs.visualstudio.com/" title="Team Foundation Service">Team Foundation Service</a></li>
                                <li><a href="http://www.microsoft.com/visualstudio/eng/products/visual-studio-express-products#product-express-summary" title="Visual Studio Express">Visual Studio Express</a></li>
                                <li><a href="http://msdn.microsoft.com/evalcenter/default.aspx" title="MSDN Evaluation Center">MSDN Evaluation Center</a></li>
                    </ul>
            </div>
        </div>        

    </div>

    <div class="ux-footer-clear"></div>
    <div id="footerRight">
        <div id="FooterLogoContainer"><div id="FooterLogo"></div></div>
        <div id="FooterCopyright">© 2013 Microsoft. All rights reserved.</div> 
    </div>
    <div id="footerLeft">
        <div id="footerGroup">
            <div id="LinkGroup">
                <div id="Links1"><div data-fragmentName="FooterLinks" id="Fragment_FooterLinks" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="LinkList">
    <div class="Links">
      <ul class="LinkColumn horizontal">
        <li>
          <a href="http://msdn.microsoft.com/en-us/newsletter.aspx" xmlns="http://www.w3.org/1999/xhtml">Newsletter</a>
        </li>
        <li>
          <div class="LinksDivider">|</div>
          <a href="http://msdn.microsoft.com/en-us/subscriptions/aa948875.aspx" xmlns="http://www.w3.org/1999/xhtml">Contact Us</a>
        </li>
        <li>
          <div class="LinksDivider">|</div>
          <a href="http://privacy.microsoft.com/en-us/default.mspx" xmlns="http://www.w3.org/1999/xhtml">Privacy Statement</a>
        </li>
        <li>
          <div class="LinksDivider">|</div>
          <a href="http://msdn.microsoft.com/en-US/cc300389.aspx" xmlns="http://www.w3.org/1999/xhtml">Terms of Use</a>
        </li>
        <li>
          <div class="LinksDivider">|</div>
          <a href="http://www.microsoft.com/About/Legal/EN/US/IntellectualProperty/Trademarks/EN-US.aspx" xmlns="http://www.w3.org/1999/xhtml">Trademarks</a>
        </li>
      </ul>
    </div>
  </div>
</div></div>
            </div>
            <div id="Feedback">
                <div class="feedbackListItem">
                    <div class="LinksDivider">|</div>
                    <div class="LinkColumn">
                        





<div class="FooterSiteFeedBack">
    <a id="SiteFeedbackLinkOpener" href="javascript:void(0)">
        <span class="FeedbackButton clip20x21" id="FeedbackButton">
            <img id="feedBackImg" class="cl_footer_feedback_icon" src="http://i.msdn.microsoft.com/Areas/Epx/Content/Images/ImageSprite.png" alt="Site Feedback" />
        </span>
        Site Feedback
    </a>
    
    
    

    
    
</div>

                    </div>
                </div>
            </div>
        </div>
        <div style="clear:both"></div>
        <div id="Links2"></div>
    </div>
</div> 
<script type="text/javascript" xmlns="http://www.w3.org/1999/xhtml">
/*<![CDATA[*/
/**/
(window.MTPS || (window.MTPS = {})).cdnDomains || (window.MTPS.cdnDomains = { 
	"image": "http://i.msdn.microsoft.com", 
	"js": "http://i2.msdn.microsoft.com", 
	"css": "http://i3.msdn.microsoft.com"
});
/**/
/*]]>*/
</script><script type="text/javascript" xmlns="http://www.w3.org/1999/xhtml">
/*<![CDATA[*/
$(document).ready(function () {
            var opener = "#SiteFeedbackLinkOpener";
            $(opener).click(function () {
                window.open('https://lab.msdn.microsoft.com/mailform/contactus.aspx?refurl=http%3a%2f%2fmsdn.microsoft.com%2fen-us%2fmsdn%2fux%2fSamples%2fuxservices%2ffooter(v%3dmsdn.10)&loc=en-us', 'SiteFeedback', 'width=670,resizable=no,height=700,toolbar=no,location=no,scrollbars=yes,directories=no,status=no,menubar=no'); return false;
            });
        });
/*]]>*/
</script></div>
                    

                           
                </div>
            </div>
        </div>
    </div>
      
    <div data-chameleon-template="megablade" ></div>
    
    <script type="text/javascript">
        Galleries.utility.loadJavaScript("http://widgets.membership.s-msft.com/v1/loader.js?brand=Msdn&lang=en-US", true);
        Galleries.utility.loadJavaScript('http://js.microsoft.com/library/svy/sto/broker.js', true);
    </script>    

    
    <script src="http://i1.code.msdn.s-msft.com/RequestReduceContent/c8e880a08198192649f9b47b92426cc1-4958017e9b075eb845f64c6aa0bffbfe-RequestReducedScript.js" type="text/javascript" ></script>        
                        <noscript><a href="http://www.omniture.com" title="Web Analytics">
        <img src="http://msstonojssocial.112.2O7.net/b/ss/msstonojssocial/1/H.20.2--NS/0" height="1" width="1" alt="" />
        </a></noscript>
    
</body>
</html>
