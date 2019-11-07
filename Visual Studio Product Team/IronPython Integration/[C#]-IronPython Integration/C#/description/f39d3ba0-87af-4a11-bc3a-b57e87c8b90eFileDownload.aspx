


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1"><link href="http://i1.code.msdn.s-msft.com/RequestReduceContent/0c412d9ee8c9d9f2d2da39833ac8154b-e407663ed09b6dba4f0a8aaf82840b9f-RequestReducedStyle.css" rel="Stylesheet" type="text/css" />
    <meta name="description" content="This sample illustrates how to integrate support for a new language into Visual Studio." />

    <link rel="shortcut icon" href="http://i1.code.msdn.s-msft.com/GlobalResources/images/Msdn/favicon.ico" type="image/x-icon" /> <meta name="CommunityInfo" content=" B=Msdn;A=Gallery::Samples;L=en-US;" />
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js" type="text/javascript" language="javascript"></script>
    
    <title>IronPython Integration - MSDN Samples Gallery</title>
    <script src="http://i1.code.msdn.s-msft.com/pageresource.js?groupname=samplesscripts&amp;v=2011.11.17.3866" type="text/javascript">

</script>
    <link href="/site/feeds/searchRss" rel="Alternate" type="application/rss+xml"
        title="MSDN Samples Gallery" />
    <link rel="P3Pv1" href="/W3C/p3p.xml" />
    
<script type="text/javascript" src="http://Ads1.msn.com/library/dap.js"></script>

    
        <script src="http://i1.code.msdn.s-msft.com/RequestReduceContent/87566195be263b58f2c27f6abee0e8f2-f47125960357127636c896564f992629-RequestReducedScript.js" type="text/javascript" ></script>
    
            <link rel="canonical" href="http://code.msdn.microsoft.com/windowsdesktop/IronPython-Integration-6b03988d" />
    
</head>
<body class="" dir= "ltr">
    <form method="post" action="IronPython-Integration-6b03988d" id="Form1">
<input type="hidden" name="__VIEWSTATE" id="__VIEWSTATE" value="/wEPDwUINDgwOTk0MzBkZM2UcMozgtVdBCaqPXpBy4kNaLrd" />

    </form>
    <div id="BodyBackground" class="ltr">
        <div id="JelloSizer">    
            <div id="JelloExpander">
                <div id="JelloWrapper">
                    <div class="Clear"> </div>
                    
                    <div class="Masthead">
                        
                        <div id="BrandLogo" class="BrandLogo"><a class="BrandLogoLink" href="http://msdn.microsoft.com/en-us/ms348103.aspx"><span id="BrandDisplayText">Microsoft Developer Network</span></a></div>
                                 
                        <div class="Search">
                            <div id="SearchBox" class="SearchBox"><input name="SearchTextBox" type="text" id="SearchTextBox" class="TextBoxSearch TextBoxSearchIE7" /><input type="image" name="SearchButton" id="SearchButton" class="SearchButton" src="http://i1.code.msdn.s-msft.com/globalresources/Images/trans.gif" /><span class="Bing"> </span></div>
                        </div>

                        <div class="GlobalBar">                       
                            <div class="PassportScarab">
                            <a id="idPPScarab" href="https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=code.msdn.microsoft.com&wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%3fstoAI%3d10&wp=MBI_FED_SSL&wlcxt=Msdn%24Msdn%24Msdn">Sign in</a>
                            </div> 
                                
                            <div id="LocaleFlyout" class="LocaleFlyout">
                                <div id="LocaleSelector">
                    <div id="TargetPanel">
                        <a>United States (English)</a><span class="DropDownArrow">&nbsp;</span>
                    </div>
                    <div id="PopupPanel" class="ContextMenuPanel">
                        <a href="/IronPython-Integration-6b03988d?localeName=es-AR" class="ContextMenuItem" rel="nofollow">Argentina (Español)</a><a href="/IronPython-Integration-6b03988d?localeName=pt-BR" class="ContextMenuItem" rel="nofollow">Brasil (Português)</a><a href="/IronPython-Integration-6b03988d?localeName=cs-CZ" class="ContextMenuItem" rel="nofollow">Česká republika (Čeština)</a><a href="/IronPython-Integration-6b03988d?localeName=de-DE" class="ContextMenuItem" rel="nofollow">Deutschland (Deutsch)</a><a href="/IronPython-Integration-6b03988d?localeName=es-ES" class="ContextMenuItem" rel="nofollow">España (Español)</a><a href="/IronPython-Integration-6b03988d?localeName=fr-FR" class="ContextMenuItem" rel="nofollow">France (Français)</a><a href="/IronPython-Integration-6b03988d?localeName=it-IT" class="ContextMenuItem" rel="nofollow">Italia (Italiano)</a><a href="/IronPython-Integration-6b03988d?localeName=pl-PL" class="ContextMenuItem" rel="nofollow">Polska (Polski)</a><a href="/IronPython-Integration-6b03988d?localeName=tr-TR" class="ContextMenuItem" rel="nofollow">Türkiye (Türkçe)</a><a href="/IronPython-Integration-6b03988d?localeName=ru-RU" class="ContextMenuItem" rel="nofollow">Россия (Русский)</a><a href="/IronPython-Integration-6b03988d?localeName=ko-KR" class="ContextMenuItem" rel="nofollow">대한민국 (한국어)</a><a href="/IronPython-Integration-6b03988d?localeName=zh-CN" class="ContextMenuItem" rel="nofollow">中华人民共和国 (中文)</a><a href="/IronPython-Integration-6b03988d?localeName=zh-TW" class="ContextMenuItem" rel="nofollow">台灣 (中文)</a><a href="/IronPython-Integration-6b03988d?localeName=ja-JP" class="ContextMenuItem" rel="nofollow">日本 (日本語)</a>
                    </div>
                 </div>
                            </div>
                            
                            <div class="Clear"> </div>

                            
                            <div class="UserName" id="UserName"><a id="ctl01_UserNameLink"></a></div>
                             

                        </div>
                    
                        <div class="Clear"> </div>

                        <div id="NetworkTagLineArea" class="NetworkLogo">
                            <a id="NetworkLink" href="http://Msdn.microsoft.com/en-US">
                                &nbsp;
                            </a>
                        </div>

                        <div class="leftcap" xmlns:msxsl="urn:schemas-microsoft-com:xslt"> </div><div class="internav" xmlns:msxsl="urn:schemas-microsoft-com:xslt"><a href="http://msdn.microsoft.com/en-us/ms348103.aspx">Home</a><a href="http://msdn.microsoft.com/en-us/library/default.aspx">Library</a><a href="http://msdn.microsoft.com/en-us/bb188199.aspx">Learn</a><a href="http://code.msdn.microsoft.com/">Samples</a><a href="http://msdn.microsoft.com/en-us/aa570309.aspx">Downloads</a><a href="http://msdn.microsoft.com/en-us/hh361695.aspx">Support</a><a href="http://msdn.microsoft.com/en-us/aa497440.aspx">Community</a><a href="http://social.msdn.microsoft.com/Forums/en-US/categories">Forums</a></div>
                    </div>
                     
                    
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
                



<div class="LocalNavigation">
    <div id="LocalNav" class="HeaderTabs">
        
        <div class="TabOff">
            <a href="/site/mydashboard">My Contributions</a>
        </div>
        <div class="TabOff">
            <a href="/site/favorites">My Favorites</a>
        </div>
        <div class="TabOff">
            <a id="notificationLink" href="/site/notifications">
                My Notifications
            </a>
        </div>
        
    <div class="TabOff">
        <a href="/site/FAQ">FAQ</a>
    </div>

        

    </div>
</div>

            </div>            
        
        
            <div id="subHeader">
                <div id="eyebrow" class="left">
                    






<div class="EyebrowContainer">
    

    
    <a href=http://msdn.microsoft.com/en-US/>
        Microsoft Developer Network
    </a>
    <text>&gt;</text>


        <a href="/">Samples</a> 


    &gt;
<div class="EyebrowElement">
	            <span>
IronPython Integration            </span>

</div></div>
                </div>
            </div>
            <div class="clear">
            </div>
        
        

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
            

<div id="userCard" class="section">
    <div class="titleBar">
        <h3>About the Author</h3>
    </div>
    <div class="profile-usercard-inline" 
         data-profile-usercard-customLink='{"href":"/site/search?f%5B0%5D.Type=User&f%5B0%5D.Value=Microsoft%20-%20Visual%20Studio%20Platform%20Team","text":"View Samples"}' 
         data-profile-userid="7fe1fc6c-49c3-4b22-8e91-f9750949b5d6">
    </div>
</div>
            <div class="advertisment collapsableSidebar">
                <div id="Rectangle300">
	<div id="96ec6f5c-de29-4421-b538-c2bec5f35701" name="96ec6f5c-de29-4421-b538-c2bec5f35701" align="center" style="margin:auto;" class="AdWidthFix"></div><script type="text/javascript">dapMgr.enableACB("96ec6f5c-de29-4421-b538-c2bec5f35701", false);dapMgr.renderAd("96ec6f5c-de29-4421-b538-c2bec5f35701", "&PG=CMSCGA&AP=1089", 300, 250);</script>
</div>

            </div>
            <div id="moreFromAuthor" class="collapsableSidebar">
                
<div class="section">
    <div class="titleBar">
        <h3>More From Microsoft - Visual Studio Platform Team</h3>
    </div>
    
        <div class="itemBarRight">
            <a href="/Build-Progress-Bar-0b182d5f">Build Progress Bar</a>
            <div>
                <span class="Rating"><img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="HalfRatingStar" />
</span>(18)
            </div>
        </div>
    
        <div class="itemBarRight">
            <a href="/Single-File-Generator-94d856d4">Single File Generator</a>
            <div>
                <span class="Rating"><img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="EmptyRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="EmptyRatingStar" />
</span>(4)
            </div>
        </div>
    
        <div class="itemBarRight">
            <a href="/VSSDK-IDE-Sample-Menu-And-6165af30">VSSDK IDE Sample&#58; Menu And Commands</a>
            <div>
                <span class="Rating"><img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
</span>(1)
            </div>
        </div>
    
        <div class="itemBarRight">
            <a href="/VS2010-Spell-Checker-3319862b">VS2010 Spell Checker</a>
            <div>
                <span class="Rating"><img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="EmptyRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="EmptyRatingStar" />
</span>(1)
            </div>
        </div>
    
        <div class="itemBarRight">
            <a href="/Editor-in-a-Toolwindow-d73a49e1">Editor in a Toolwindow</a>
            <div>
                <span class="Rating"><img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="FilledRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="HalfRatingStar" />
<img style="vertical-align: text-top;" src="http://i1.code.msdn.s-msft.com/content/common/trans.gif" class="EmptyRatingStar" />
</span>(3)
            </div>
        </div>
    
    <div class="bottomBar">
        <a href="/site/search?f%5B0%5D.Type=User&f%5B0%5D.Value=Microsoft%20-%20Visual%20Studio%20Platform%20Team">See All</a>
    </div>
    
</div>

            </div>
        </div>

        <div id="projectDetails">
            <div>
                


<h1 class="projectTitle">IronPython Integration</h1>
<div class="clear" ></div>

    <div id="Downloads">
        <div>
            <label for="Downloads">Download</label><br />
            <span>Select a language</span>
        </div>
        
                    <input type="button" data-url="/IronPython-Integration-6b03988d/file/43/1/IronPython%20Integration.zip" value="C#" />
        
    </div>

<div class="clear"></div>

<div class="clear"></div>
<div id="projectInfo">
    <div id="sectionLeft" class="section">
        <div class="itemBar">
            <label for="ReviewValue">Ratings</label>                   
            
    <div id="yourRating"">
        <form action="/IronPython-Integration-6b03988d/review/create" id="reviewForm" method="post" name="reviewForm"><input id="Rating" name="Rating" type="hidden" value="" />
            <input id="Text" name="Text" type="hidden" value="" />
        <div class="Rating stackLeft EditStarMode" id="Star_Span" onmouseout="onStarsMouseOut()">
        
            <div id="Star_0" class="RatingStar EmptyRatingStar"
                data-initialstarclass="EmptyRatingStar" 
                onmouseover="onStarMouseOver('Star_0')" onclick="onStarClick(0)"
                title="1">
                &nbsp;</div>
        
            <div id="Star_1" class="RatingStar EmptyRatingStar"
                data-initialstarclass="EmptyRatingStar" 
                onmouseover="onStarMouseOver('Star_1')" onclick="onStarClick(1)"
                title="2">
                &nbsp;</div>
        
            <div id="Star_2" class="RatingStar EmptyRatingStar"
                data-initialstarclass="EmptyRatingStar" 
                onmouseover="onStarMouseOver('Star_2')" onclick="onStarClick(2)"
                title="3">
                &nbsp;</div>
        
            <div id="Star_3" class="RatingStar EmptyRatingStar"
                data-initialstarclass="EmptyRatingStar" 
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
    
    <div id="RatingCount">(0)</div>
    

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
        var currentRating = 0;
        
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
                <span id="DownloadCount">1,043 times</span>
            </div>
        
    <div class="itemBar">
        <label for="Supports">Supports</label>
        <span id="Supports">Visual Studio 2010</span>
    </div>




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



        <div class="itemBar">
            <span id="reportAbuse">
                <a href="https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=code.msdn.microsoft.com&wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%2fview%2fReportAbuse%3fstoAI%3d10&wp=MBI_FED_SSL&wlcxt=microsoft%24microsoft%24microsoft">Report Abuse to Microsoft</a>
            </span>
        </div>
    </div>

    <div id="sectionRight" class="section">
        <div class="itemBar">
            <label for="LastUpdated">
                Last Updated</label>
            <span id="LastUpdated">
                11/30/2011</span>
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
 
<a href ="mailto&#58;&#63;subject&#61;IronPython&#37;20Integration&#38;Body&#61;http&#58;&#47;&#47;code.msdn.microsoft.com&#47;IronPython-Integration-6b03988d" target="_blank"><img alt="E-mail" src="http://i1.gallery.technet.s-msft.com/scriptcenter/content/common/sharethis/email.gif" /></a>

 
<a href ="http&#58;&#47;&#47;twitter.com&#47;home&#63;status&#61;IronPython&#37;20Integration&#32;&#58;&#32;http&#37;3a&#37;2f&#37;2fcode.msdn.microsoft.com&#37;2fIronPython-Integration-6b03988d" target="_blank"><img alt="Twitter" src="http://i1.gallery.technet.s-msft.com/scriptcenter/content/common/sharethis/twitter.gif" /></a>

 
<a href ="http&#58;&#47;&#47;del.icio.us&#47;post&#63;url&#61;http&#37;3a&#37;2f&#37;2fcode.msdn.microsoft.com&#37;2fIronPython-Integration-6b03988d&#38;title&#61;IronPython&#37;20Integration" target="_blank"><img alt="del.icio.us" src="http://i1.gallery.technet.s-msft.com/scriptcenter/content/common/sharethis/delicious.gif" /></a>

 
<a href ="http&#58;&#47;&#47;digg.com&#47;submit&#63;url&#61;http&#37;3a&#37;2f&#37;2fcode.msdn.microsoft.com&#37;2fIronPython-Integration-6b03988d&#38;title&#61;IronPython&#37;20Integration" target="_blank"><img alt="Digg" src="http://i1.gallery.technet.s-msft.com/scriptcenter/content/common/sharethis/digg.gif" /></a>

 
<a href ="http&#58;&#47;&#47;www.facebook.com&#47;sharer.php&#63;u&#61;http&#37;3a&#37;2f&#37;2fcode.msdn.microsoft.com&#37;2fIronPython-Integration-6b03988d&#38;t&#61;IronPython&#37;20Integration" target="_blank"><img alt="Facebook" src="http://i1.gallery.technet.s-msft.com/scriptcenter/content/common/sharethis/facebook.gif" /></a>


</div></span>
        </div>
        
            <div class="itemBar">
                <label for="Favorites">Favorites</label>
                
<script language="javascript">
    $(function () { $('a#addFavoriteLink').click(function () { $('form#favoriteform').submit(); }); });
</script>
<span id="Collections">

        <a class="bevelButton" href="https://login.live.com/login.srf?wa=wsignin1.0&wtrealm=code.msdn.microsoft.com&wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%3fstoAI%3d10&wp=MBI_FED_SSL&wlcxt=microsoft%24microsoft%24microsoft">Add To Favorites</a>

</span>

            </div>
        
    </div>

    <div class="clear"></div>
</div>

<script src="http://i1.code.msdn.s-msft.com/content/common/jquery-impromptu.3.1.min.js" type="text/javascript"></script>
<script type="text/javascript">

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
            <div id="Details_Tab">Description</div>
            
        <div id="SourceCode_Tab">Browse Code</div>
            
                <div id="Discussions_Tab">Q and A </div>
        </div>
        <div class="tabContents">
            <div id="Details_Content">
                
                    <iframe id="longdescIframe" frameborder="0" width="100%" scrolling="no" marginwidth="0" marginheight="0" src="/IronPython-Integration-6b03988d/description"></iframe>                
                
            </div>
            
        <div id="SourceCode_Content">
            

<script language="javascript">
    $(function () {
        Galleries.SourceCodeBrowser.init({
            fileUrl: '\x2fIronPython-Integration-6b03988d\x2fsourceitem\x3ffileId\x3dfileIdHolder\x26pathId\x3dpathIdHolder\x26streamRaw\x3dFalse',
            statisticsUrl: '\x2fIronPython-Integration-6b03988d\x2fstats\x2fRegisterPageView',
            loadedFileId : null,
            loadedPathId : null
        });
        $("#SourceCode_Content").split();
    });
</script>
<script src="http://i1.code.msdn.s-msft.com/content/common/chili/jquery.chili-2.2.js" type="text/javascript" ></script>
<script src="http://i1.code.msdn.s-msft.com/content/common/chili/recipes.js" type="text/javascript"></script>



<div class="sourceList">
    <div class="sourceListTabs">
        
            <div id="lang0">C#</div>
        
    </div>
    <div class="endTabs"></div>

    <div class="sourceListContent">
        
            <div id="lang0_Content" class="langContent" style="display: none;">
                <a class="browserDir languageNode">
                    <div class="sbfc"></div>
                </a>
                <div style="display:none;margin-left:20px;">
                                                          
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">integration</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">lib</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">IronMath.dll</div>
        <div class="ndbf">IronPython.dll</div>
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">src</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython.CompilerTask</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Properties</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=933411439" class="browseFile"  data-fileid="43" data-pathid="933411439" data-name="Resources.Designer.cs"><div class="sbf">Resources.Designer.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1600689876" class="browseFile"  data-fileid="43" data-pathid="1600689876" data-name="Resources.resx"><div class="sbf">Resources.resx</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2028916359" class="browseFile"  data-fileid="43" data-pathid="2028916359" data-name="AssemblyInfo.cs"><div class="sbf">AssemblyInfo.cs</div></a> 
        <div class="ndbf">ClassDiagram1.cd</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=581324502" class="browseFile"  data-fileid="43" data-pathid="581324502" data-name="Compiler.cs"><div class="sbf">Compiler.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=301992115" class="browseFile"  data-fileid="43" data-pathid="301992115" data-name="CompilerErrorSink.cs"><div class="sbf">CompilerErrorSink.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1269711341" class="browseFile"  data-fileid="43" data-pathid="1269711341" data-name="ExperimentalCompiler.cs"><div class="sbf">ExperimentalCompiler.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1578663283" class="browseFile"  data-fileid="43" data-pathid="1578663283" data-name="ICompiler.cs"><div class="sbf">ICompiler.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1266389869" class="browseFile"  data-fileid="43" data-pathid="1266389869" data-name="IronPython.CompilerTask.csproj"><div class="sbf">IronPython.CompilerTask.csproj</div></a> 
        <div class="ndbf">IronPython.targets</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1458155946" class="browseFile"  data-fileid="43" data-pathid="1458155946" data-name="IronPythonCompilerTask.cs"><div class="sbf">IronPythonCompilerTask.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython.Console</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Properties</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=994088326" class="browseFile"  data-fileid="43" data-pathid="994088326" data-name="AssemblyInfo.cs"><div class="sbf">AssemblyInfo.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Resources</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">Package.ico</div>
        </div>
        
        <div class="ndbf">ClassDiagram.cd</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2087986924" class="browseFile"  data-fileid="43" data-pathid="2087986924" data-name="CommandBuffer.cs"><div class="sbf">CommandBuffer.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1470902205" class="browseFile"  data-fileid="43" data-pathid="1470902205" data-name="ConsoleAuthoringScope.cs"><div class="sbf">ConsoleAuthoringScope.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=567639394" class="browseFile"  data-fileid="43" data-pathid="567639394" data-name="ConsoleWindow.cs"><div class="sbf">ConsoleWindow.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=714996995" class="browseFile"  data-fileid="43" data-pathid="714996995" data-name="ConsoleWindow.jpg"><div class="sbf">ConsoleWindow.jpg</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1409193321" class="browseFile"  data-fileid="43" data-pathid="1409193321" data-name="GlobalSuppressions.cs"><div class="sbf">GlobalSuppressions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1601322741" class="browseFile"  data-fileid="43" data-pathid="1601322741" data-name="Guids.cs"><div class="sbf">Guids.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=968743723" class="browseFile"  data-fileid="43" data-pathid="968743723" data-name="HistoryBuffer.cs"><div class="sbf">HistoryBuffer.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=274553998" class="browseFile"  data-fileid="43" data-pathid="274553998" data-name="IronPython.Console.csproj"><div class="sbf">IronPython.Console.csproj</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=311036705" class="browseFile"  data-fileid="43" data-pathid="311036705" data-name="IronPythonEngineProvider.cs"><div class="sbf">IronPythonEngineProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1073370623" class="browseFile"  data-fileid="43" data-pathid="1073370623" data-name="Overview.xml"><div class="sbf">Overview.xml</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1716458817" class="browseFile"  data-fileid="43" data-pathid="1716458817" data-name="PkgCmdID.cs"><div class="sbf">PkgCmdID.cs</div></a> 
        <div class="ndbf">PythonConsole.vsct</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1435453513" class="browseFile"  data-fileid="43" data-pathid="1435453513" data-name="PythonConsoleIcon_24Bit.bmp"><div class="sbf">PythonConsoleIcon_24Bit.bmp</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1569242626" class="browseFile"  data-fileid="43" data-pathid="1569242626" data-name="PythonConsoleIcon_32Bit.bmp"><div class="sbf">PythonConsoleIcon_32Bit.bmp</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1788412812" class="browseFile"  data-fileid="43" data-pathid="1788412812" data-name="PythonConsolePackage.cs"><div class="sbf">PythonConsolePackage.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=690818004" class="browseFile"  data-fileid="43" data-pathid="690818004" data-name="Resources.Designer.cs"><div class="sbf">Resources.Designer.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=899726465" class="browseFile"  data-fileid="43" data-pathid="899726465" data-name="Resources.resx"><div class="sbf">Resources.resx</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=688115926" class="browseFile"  data-fileid="43" data-pathid="688115926" data-name="TextBufferStream.cs"><div class="sbf">TextBufferStream.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1010859802" class="browseFile"  data-fileid="43" data-pathid="1010859802" data-name="VSPackage.resx"><div class="sbf">VSPackage.resx</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython.EditorExtensions</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">BraceMatching</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1928612668" class="browseFile"  data-fileid="43" data-pathid="1928612668" data-name="BraceMatchingFactory.cs"><div class="sbf">BraceMatchingFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2060797631" class="browseFile"  data-fileid="43" data-pathid="2060797631" data-name="BraceMatchingPresenter.cs"><div class="sbf">BraceMatchingPresenter.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1193350615" class="browseFile"  data-fileid="43" data-pathid="1193350615" data-name="PyBraceMatchCompilerSink.cs"><div class="sbf">PyBraceMatchCompilerSink.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1313799219" class="browseFile"  data-fileid="43" data-pathid="1313799219" data-name="PyBraceMatchProvider.cs"><div class="sbf">PyBraceMatchProvider.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Classification</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1643259429" class="browseFile"  data-fileid="43" data-pathid="1643259429" data-name="PyClassificationDefinitions.cs"><div class="sbf">PyClassificationDefinitions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=965206377" class="browseFile"  data-fileid="43" data-pathid="965206377" data-name="PyClassificationProvider.cs"><div class="sbf">PyClassificationProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1500568287" class="browseFile"  data-fileid="43" data-pathid="1500568287" data-name="PyClassificationTypes.cs"><div class="sbf">PyClassificationTypes.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=255428519" class="browseFile"  data-fileid="43" data-pathid="255428519" data-name="PyClassifier.cs"><div class="sbf">PyClassifier.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Completion</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1907195310" class="browseFile"  data-fileid="43" data-pathid="1907195310" data-name="CompletionCommandFilterProvider.cs"><div class="sbf">CompletionCommandFilterProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1459886376" class="browseFile"  data-fileid="43" data-pathid="1459886376" data-name="CompletionController.cs"><div class="sbf">CompletionController.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=166165483" class="browseFile"  data-fileid="43" data-pathid="166165483" data-name="CompletionControllerProvider.cs"><div class="sbf">CompletionControllerProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=652794205" class="browseFile"  data-fileid="43" data-pathid="652794205" data-name="CompletionSetExtensions.cs"><div class="sbf">CompletionSetExtensions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1213950958" class="browseFile"  data-fileid="43" data-pathid="1213950958" data-name="CompletionSource.cs"><div class="sbf">CompletionSource.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=584148343" class="browseFile"  data-fileid="43" data-pathid="584148343" data-name="CompletionSourceProvider.cs"><div class="sbf">CompletionSourceProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=842318394" class="browseFile"  data-fileid="43" data-pathid="842318394" data-name="PyCompletion.cs"><div class="sbf">PyCompletion.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1923996675" class="browseFile"  data-fileid="43" data-pathid="1923996675" data-name="SnippetsEnumerator.cs"><div class="sbf">SnippetsEnumerator.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Engine</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=630836825" class="browseFile"  data-fileid="43" data-pathid="630836825" data-name="Analyzer.cs"><div class="sbf">Analyzer.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=271823630" class="browseFile"  data-fileid="43" data-pathid="271823630" data-name="Definitions.cs"><div class="sbf">Definitions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1053738903" class="browseFile"  data-fileid="43" data-pathid="1053738903" data-name="Engine.cs"><div class="sbf">Engine.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=14594596" class="browseFile"  data-fileid="43" data-pathid="14594596" data-name="Inferred.cs"><div class="sbf">Inferred.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1957679825" class="browseFile"  data-fileid="43" data-pathid="1957679825" data-name="Locator.cs"><div class="sbf">Locator.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1959582591" class="browseFile"  data-fileid="43" data-pathid="1959582591" data-name="Modules.cs"><div class="sbf">Modules.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=278253981" class="browseFile"  data-fileid="43" data-pathid="278253981" data-name="Scope.cs"><div class="sbf">Scope.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1926366813" class="browseFile"  data-fileid="43" data-pathid="1926366813" data-name="ScopeWalker.cs"><div class="sbf">ScopeWalker.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Properties</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1343362869" class="browseFile"  data-fileid="43" data-pathid="1343362869" data-name="AssemblyInfo.cs"><div class="sbf">AssemblyInfo.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Service References</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Validation</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=961984008" class="browseFile"  data-fileid="43" data-pathid="961984008" data-name="ErrorListFactory.cs"><div class="sbf">ErrorListFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1964494382" class="browseFile"  data-fileid="43" data-pathid="1964494382" data-name="ErrorListPresenter.cs"><div class="sbf">ErrorListPresenter.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1550761341" class="browseFile"  data-fileid="43" data-pathid="1550761341" data-name="PyErrorListCompilerSink.cs"><div class="sbf">PyErrorListCompilerSink.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=593731343" class="browseFile"  data-fileid="43" data-pathid="593731343" data-name="PyErrorListProvider.cs"><div class="sbf">PyErrorListProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=824840988" class="browseFile"  data-fileid="43" data-pathid="824840988" data-name="ValidationError.cs"><div class="sbf">ValidationError.cs</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=261011268" class="browseFile"  data-fileid="43" data-pathid="261011268" data-name="Constants.cs"><div class="sbf">Constants.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=406930130" class="browseFile"  data-fileid="43" data-pathid="406930130" data-name="IronPython.EditorExtensions.csproj"><div class="sbf">IronPython.EditorExtensions.csproj</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=422695470" class="browseFile"  data-fileid="43" data-pathid="422695470" data-name="PyContentTypeDefinition.cs"><div class="sbf">PyContentTypeDefinition.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1035031771" class="browseFile"  data-fileid="43" data-pathid="1035031771" data-name="ServiceProvider.cs"><div class="sbf">ServiceProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1902834831" class="browseFile"  data-fileid="43" data-pathid="1902834831" data-name="TextBufferExtension.cs"><div class="sbf">TextBufferExtension.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython.Interfaces</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Properties</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=558961151" class="browseFile"  data-fileid="43" data-pathid="558961151" data-name="AssemblyInfo.cs"><div class="sbf">AssemblyInfo.cs</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=949499016" class="browseFile"  data-fileid="43" data-pathid="949499016" data-name="EngineInterfaces.cs"><div class="sbf">EngineInterfaces.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=566381096" class="browseFile"  data-fileid="43" data-pathid="566381096" data-name="IConsoleText.cs"><div class="sbf">IConsoleText.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=118716350" class="browseFile"  data-fileid="43" data-pathid="118716350" data-name="IronPython.Interfaces.csproj"><div class="sbf">IronPython.Interfaces.csproj</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">IronPython.Project</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">CodeSnippets</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Snippets</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">class.snippet</div>
        <div class="ndbf">ctor.snippet</div>
        <div class="ndbf">else.snippet</div>
        <div class="ndbf">for.snippet</div>
        <div class="ndbf">if.snippet</div>
        <div class="ndbf">while.snippet</div>
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1324757800" class="browseFile"  data-fileid="43" data-pathid="1324757800" data-name="SnippetsIndex.xml"><div class="sbf">SnippetsIndex.xml</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">FileCodeModel</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1711588555" class="browseFile"  data-fileid="43" data-pathid="1711588555" data-name="CodeDomCodeAttribute.cs"><div class="sbf">CodeDomCodeAttribute.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=491211796" class="browseFile"  data-fileid="43" data-pathid="491211796" data-name="CodeDomCodeClass.cs"><div class="sbf">CodeDomCodeClass.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1712327234" class="browseFile"  data-fileid="43" data-pathid="1712327234" data-name="CodeDomCodeDelegate.cs"><div class="sbf">CodeDomCodeDelegate.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1133555196" class="browseFile"  data-fileid="43" data-pathid="1133555196" data-name="CodeDomCodeElement.cs"><div class="sbf">CodeDomCodeElement.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1946419397" class="browseFile"  data-fileid="43" data-pathid="1946419397" data-name="CodeDomCodeElements.cs"><div class="sbf">CodeDomCodeElements.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1714769554" class="browseFile"  data-fileid="43" data-pathid="1714769554" data-name="CodeDomCodeEnum.cs"><div class="sbf">CodeDomCodeEnum.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=874244298" class="browseFile"  data-fileid="43" data-pathid="874244298" data-name="CodeDomCodeFunction.cs"><div class="sbf">CodeDomCodeFunction.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1042643199" class="browseFile"  data-fileid="43" data-pathid="1042643199" data-name="CodeDomCodeInterface.cs"><div class="sbf">CodeDomCodeInterface.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=860931235" class="browseFile"  data-fileid="43" data-pathid="860931235" data-name="CodeDomCodeNamespace.cs"><div class="sbf">CodeDomCodeNamespace.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=309674097" class="browseFile"  data-fileid="43" data-pathid="309674097" data-name="CodeDomCodeParameter.cs"><div class="sbf">CodeDomCodeParameter.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=645672619" class="browseFile"  data-fileid="43" data-pathid="645672619" data-name="CodeDomCodeProperty.cs"><div class="sbf">CodeDomCodeProperty.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1271825868" class="browseFile"  data-fileid="43" data-pathid="1271825868" data-name="CodeDomCodeStruct.cs"><div class="sbf">CodeDomCodeStruct.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=556998044" class="browseFile"  data-fileid="43" data-pathid="556998044" data-name="CodeDomCodeType.cs"><div class="sbf">CodeDomCodeType.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=943425550" class="browseFile"  data-fileid="43" data-pathid="943425550" data-name="CodeDomCodeTypeRef.cs"><div class="sbf">CodeDomCodeTypeRef.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1828813542" class="browseFile"  data-fileid="43" data-pathid="1828813542" data-name="CodeDomCodeVariable.cs"><div class="sbf">CodeDomCodeVariable.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1215394137" class="browseFile"  data-fileid="43" data-pathid="1215394137" data-name="FileCodeMerger.cs"><div class="sbf">FileCodeMerger.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1831468038" class="browseFile"  data-fileid="43" data-pathid="1831468038" data-name="FileCodeModel.cs"><div class="sbf">FileCodeModel.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=618924993" class="browseFile"  data-fileid="43" data-pathid="618924993" data-name="PythonCodeModel.cs"><div class="sbf">PythonCodeModel.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1852338986" class="browseFile"  data-fileid="43" data-pathid="1852338986" data-name="PythonCodeModelFactory.cs"><div class="sbf">PythonCodeModelFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1128694600" class="browseFile"  data-fileid="43" data-pathid="1128694600" data-name="SimpleCodeElement.cs"><div class="sbf">SimpleCodeElement.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=608122579" class="browseFile"  data-fileid="43" data-pathid="608122579" data-name="StringMerger.cs"><div class="sbf">StringMerger.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1003001829" class="browseFile"  data-fileid="43" data-pathid="1003001829" data-name="TextBufferMerger.cs"><div class="sbf">TextBufferMerger.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=654549937" class="browseFile"  data-fileid="43" data-pathid="654549937" data-name="TextPoint.cs"><div class="sbf">TextPoint.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Library</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1998540854" class="browseFile"  data-fileid="43" data-pathid="1998540854" data-name="Library.cs"><div class="sbf">Library.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1418610320" class="browseFile"  data-fileid="43" data-pathid="1418610320" data-name="LibraryNode.cs"><div class="sbf">LibraryNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1631966604" class="browseFile"  data-fileid="43" data-pathid="1631966604" data-name="PythonLibraryManager.cs"><div class="sbf">PythonLibraryManager.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=753569567" class="browseFile"  data-fileid="43" data-pathid="753569567" data-name="PythonLibraryNode.cs"><div class="sbf">PythonLibraryNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=100436834" class="browseFile"  data-fileid="43" data-pathid="100436834" data-name="PythonModuleId.cs"><div class="sbf">PythonModuleId.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">ProjectBase</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Diagrams</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">LangProj</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">misc</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Properties</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1091613165" class="browseFile"  data-fileid="43" data-pathid="1091613165" data-name="AssemblyInfo.cs"><div class="sbf">AssemblyInfo.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">RegistrationAttributes</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=781641811" class="browseFile"  data-fileid="43" data-pathid="781641811" data-name="RegisterSnippetsAttribute.cs"><div class="sbf">RegisterSnippetsAttribute.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=432568628" class="browseFile"  data-fileid="43" data-pathid="432568628" data-name="SingleFileGeneratorSupportRegistrationAttribute.cs"><div class="sbf">SingleFileGeneratorSupportRegistrationAttribute.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Resources</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1240610263" class="browseFile"  data-fileid="43" data-pathid="1240610263" data-name="imagelis.bmp"><div class="sbf">imagelis.bmp</div></a> 
        <div class="ndbf">Package.ico</div>
        <div class="ndbf">PythonAboutBox.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1302155461" class="browseFile"  data-fileid="43" data-pathid="1302155461" data-name="PythonImageList.bmp"><div class="sbf">PythonImageList.bmp</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1137047623" class="browseFile"  data-fileid="43" data-pathid="1137047623" data-name="PythonSplashScreenIcon.bmp"><div class="sbf">PythonSplashScreenIcon.bmp</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Templates</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Items</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Class</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1437531640" class="browseFile"  data-fileid="43" data-pathid="1437531640" data-name="Class.py"><div class="sbf">Class.py</div></a> 
        <div class="ndbf">Class.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Form</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1718035988" class="browseFile"  data-fileid="43" data-pathid="1718035988" data-name="Form.py"><div class="sbf">Form.py</div></a> 
        <div class="ndbf">UI_WinForm.ico</div>
        <div class="ndbf">windowsform.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">ResX</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">Resource.resX</div>
        <div class="ndbf">Resource.vstemplate</div>
        <div class="ndbf">Resource_Resx.ico</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Text</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">Text.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1269425756" class="browseFile"  data-fileid="43" data-pathid="1269425756" data-name="TextFile.txt"><div class="sbf">TextFile.txt</div></a> 
        <div class="ndbf">TextFile.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Web</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">BrowserFile</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">BrowserFile.Browser</div>
        <div class="ndbf">BrowserFile.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">ContentPage</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=640301923" class="browseFile"  data-fileid="43" data-pathid="640301923" data-name="ContentPage.aspx"><div class="sbf">ContentPage.aspx</div></a> 
        <div class="ndbf">ContentPage.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">GlobalAsax</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2103415349" class="browseFile"  data-fileid="43" data-pathid="2103415349" data-name="Global.asax"><div class="sbf">Global.asax</div></a> 
        <div class="ndbf">GlobalAsax.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Handler</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">Handler.ashx</div>
        <div class="ndbf">Handler.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">JScript</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1064532530" class="browseFile"  data-fileid="43" data-pathid="1064532530" data-name="JScript.js"><div class="sbf">JScript.js</div></a> 
        <div class="ndbf">JScript.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">MasterPage</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">MasterPage.vstemplate</div>
        <div class="ndbf">Site.Master</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">NestedWebConfig</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">NestedWebConfig.vstemplate</div>
        <div class="ndbf">Web.Config</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">SiteMap</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">SiteMap.vstemplate</div>
        <div class="ndbf">Web.sitemap</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">SkinFile</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">SkinFile.skin</div>
        <div class="ndbf">SkinFile.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">StyleSheet</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=38464082" class="browseFile"  data-fileid="43" data-pathid="38464082" data-name="StyleSheet.css"><div class="sbf">StyleSheet.css</div></a> 
        <div class="ndbf">StyleSheet.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebClass</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=249019504" class="browseFile"  data-fileid="43" data-pathid="249019504" data-name="Class.py"><div class="sbf">Class.py</div></a> 
        <div class="ndbf">WebClass.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebConfig</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">Web.Config</div>
        <div class="ndbf">WebConfig.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebForm</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1963431074" class="browseFile"  data-fileid="43" data-pathid="1963431074" data-name="Default.aspx"><div class="sbf">Default.aspx</div></a> 
        <div class="ndbf">WebForm.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebHtmlPage</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1330397392" class="browseFile"  data-fileid="43" data-pathid="1330397392" data-name="HtmlPage.htm"><div class="sbf">HtmlPage.htm</div></a> 
        <div class="ndbf">WebHtmlPage.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebServiceItem</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">WebService.asmx</div>
        <div class="ndbf">WebService.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebUserControl</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1061977271" class="browseFile"  data-fileid="43" data-pathid="1061977271" data-name="WebUserControl.ascx"><div class="sbf">WebUserControl.ascx</div></a> 
        <div class="ndbf">WebuserControl.vstemplate</div>
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WPFWindow</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">pyWPFWindow.vstemplate</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=573308396" class="browseFile"  data-fileid="43" data-pathid="573308396" data-name="Window.py"><div class="sbf">Window.py</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1584526792" class="browseFile"  data-fileid="43" data-pathid="1584526792" data-name="Window.xaml"><div class="sbf">Window.xaml</div></a> 
        <div class="ndbf">WPFWindow.ico</div>
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Projects</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">ClassLibrary</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1753709899" class="browseFile"  data-fileid="43" data-pathid="1753709899" data-name="Class.py"><div class="sbf">Class.py</div></a> 
        <div class="ndbf">IPClassLib.vstemplate</div>
        <div class="ndbf">IronPythonDll.pyproj</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">ConsoleApp</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <div class="ndbf">IPConsoleApp.vstemplate</div>
        <div class="ndbf">IronPythonApp.pyproj</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=203290911" class="browseFile"  data-fileid="43" data-pathid="203290911" data-name="Program.py"><div class="sbf">Program.py</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Web</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebApplication</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1109555854" class="browseFile"  data-fileid="43" data-pathid="1109555854" data-name="Default.aspx"><div class="sbf">Default.aspx</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2133622125" class="browseFile"  data-fileid="43" data-pathid="2133622125" data-name="Web.config"><div class="sbf">Web.config</div></a> 
        <div class="ndbf">WebApplication.pyproj</div>
        <div class="ndbf">WebApplication.vstemplate</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WebService</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <div class="ndbf">Service1.asmx</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1151268044" class="browseFile"  data-fileid="43" data-pathid="1151268044" data-name="Service1.asmx.py"><div class="sbf">Service1.asmx.py</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=160053719" class="browseFile"  data-fileid="43" data-pathid="160053719" data-name="Web.config"><div class="sbf">Web.config</div></a> 
        <div class="ndbf">WebService.pyproj</div>
        <div class="ndbf">WebService.vstemplate</div>
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WinformApp</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">__TemplateIcon.ico</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1147745414" class="browseFile"  data-fileid="43" data-pathid="1147745414" data-name="Form1.py"><div class="sbf">Form1.py</div></a> 
        <div class="ndbf">IPWinformApp.vstemplate</div>
        <div class="ndbf">IronPythonWinApp.pyproj</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=264951207" class="browseFile"  data-fileid="43" data-pathid="264951207" data-name="Program.py"><div class="sbf">Program.py</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WPFApp</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=636655714" class="browseFile"  data-fileid="43" data-pathid="636655714" data-name="Program.py"><div class="sbf">Program.py</div></a> 
        <div class="ndbf">pyWPFApplication.vstemplate</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1583687168" class="browseFile"  data-fileid="43" data-pathid="1583687168" data-name="Window1.py"><div class="sbf">Window1.py</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1867030939" class="browseFile"  data-fileid="43" data-pathid="1867030939" data-name="Window1.xaml"><div class="sbf">Window1.xaml</div></a> 
        <div class="ndbf">WPFApp.ico</div>
        <div class="ndbf">WPFApplication.pyproj</div>
        </div>
        
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">WPFProviders</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2004497926" class="browseFile"  data-fileid="43" data-pathid="2004497926" data-name="PythonEventBindingProvider.cs"><div class="sbf">PythonEventBindingProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1022976974" class="browseFile"  data-fileid="43" data-pathid="1022976974" data-name="PythonRuntimeNameProvider.cs"><div class="sbf">PythonRuntimeNameProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1742318855" class="browseFile"  data-fileid="43" data-pathid="1742318855" data-name="PythonWPFFlavor.cs"><div class="sbf">PythonWPFFlavor.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=564236100" class="browseFile"  data-fileid="43" data-pathid="564236100" data-name="PythonWPFProjectFactory.cs"><div class="sbf">PythonWPFProjectFactory.cs</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=25725320" class="browseFile"  data-fileid="43" data-pathid="25725320" data-name="Automation.cs"><div class="sbf">Automation.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1486457185" class="browseFile"  data-fileid="43" data-pathid="1486457185" data-name="ConfigurationPropertyPages.cs"><div class="sbf">ConfigurationPropertyPages.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1442316610" class="browseFile"  data-fileid="43" data-pathid="1442316610" data-name="Constants.cs"><div class="sbf">Constants.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=800466049" class="browseFile"  data-fileid="43" data-pathid="800466049" data-name="EditorFactory.cs"><div class="sbf">EditorFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1807225334" class="browseFile"  data-fileid="43" data-pathid="1807225334" data-name="Enums.cs"><div class="sbf">Enums.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=431935167" class="browseFile"  data-fileid="43" data-pathid="431935167" data-name="GlobalSuppressions.cs"><div class="sbf">GlobalSuppressions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1022185949" class="browseFile"  data-fileid="43" data-pathid="1022185949" data-name="HierarchyListener.cs"><div class="sbf">HierarchyListener.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1508551911" class="browseFile"  data-fileid="43" data-pathid="1508551911" data-name="IronPython.Project.csproj"><div class="sbf">IronPython.Project.csproj</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=5553326" class="browseFile"  data-fileid="43" data-pathid="5553326" data-name="IronPython.Project.csproj.user"><div class="sbf">IronPython.Project.csproj.user</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2041369542" class="browseFile"  data-fileid="43" data-pathid="2041369542" data-name="IronPython_large.png"><div class="sbf">IronPython_large.png</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=977387547" class="browseFile"  data-fileid="43" data-pathid="977387547" data-name="IronPython_small.png"><div class="sbf">IronPython_small.png</div></a> 
        <div class="ndbf">key.snk</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=869217427" class="browseFile"  data-fileid="43" data-pathid="869217427" data-name="Overview.xml"><div class="sbf">Overview.xml</div></a> 
        <div class="ndbf">PkgCmd.vsct</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2069750825" class="browseFile"  data-fileid="43" data-pathid="2069750825" data-name="Project.jpg"><div class="sbf">Project.jpg</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1461904597" class="browseFile"  data-fileid="43" data-pathid="1461904597" data-name="ProjectDocumentsListenerForMainFileUpdates.cs"><div class="sbf">ProjectDocumentsListenerForMainFileUpdates.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1293509433" class="browseFile"  data-fileid="43" data-pathid="1293509433" data-name="PropertyPages.cs"><div class="sbf">PropertyPages.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1186059616" class="browseFile"  data-fileid="43" data-pathid="1186059616" data-name="PythonConfigProvider.cs"><div class="sbf">PythonConfigProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1779379952" class="browseFile"  data-fileid="43" data-pathid="1779379952" data-name="PythonFileNode.cs"><div class="sbf">PythonFileNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=646543100" class="browseFile"  data-fileid="43" data-pathid="646543100" data-name="PythonFileNodeProperties.cs"><div class="sbf">PythonFileNodeProperties.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1183267066" class="browseFile"  data-fileid="43" data-pathid="1183267066" data-name="PythonMenus.cs"><div class="sbf">PythonMenus.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1630425970" class="browseFile"  data-fileid="43" data-pathid="1630425970" data-name="PythonProjectFactory.cs"><div class="sbf">PythonProjectFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=528184150" class="browseFile"  data-fileid="43" data-pathid="528184150" data-name="PythonProjectFileConstants.cs"><div class="sbf">PythonProjectFileConstants.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=107212166" class="browseFile"  data-fileid="43" data-pathid="107212166" data-name="PythonProjectNode.cs"><div class="sbf">PythonProjectNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=96971962" class="browseFile"  data-fileid="43" data-pathid="96971962" data-name="PythonProjectNodeProperties.cs"><div class="sbf">PythonProjectNodeProperties.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=599892321" class="browseFile"  data-fileid="43" data-pathid="599892321" data-name="PythonProjectPackage.cs"><div class="sbf">PythonProjectPackage.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=652978226" class="browseFile"  data-fileid="43" data-pathid="652978226" data-name="PythonProjectReferenceNode.cs"><div class="sbf">PythonProjectReferenceNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1573460892" class="browseFile"  data-fileid="43" data-pathid="1573460892" data-name="PythonReferenceContainerNode.cs"><div class="sbf">PythonReferenceContainerNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=148220315" class="browseFile"  data-fileid="43" data-pathid="148220315" data-name="Resources.cs"><div class="sbf">Resources.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=739152003" class="browseFile"  data-fileid="43" data-pathid="739152003" data-name="Resources.resx"><div class="sbf">Resources.resx</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1009019167" class="browseFile"  data-fileid="43" data-pathid="1009019167" data-name="SelectionElementValueChangedListener.cs"><div class="sbf">SelectionElementValueChangedListener.cs</div></a> 
        <div class="ndbf">source.extension.vsixmanifest</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1548030546" class="browseFile"  data-fileid="43" data-pathid="1548030546" data-name="TextLineEventListener.cs"><div class="sbf">TextLineEventListener.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2105751983" class="browseFile"  data-fileid="43" data-pathid="2105751983" data-name="VSIXProject_large.png"><div class="sbf">VSIXProject_large.png</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=132729201" class="browseFile"  data-fileid="43" data-pathid="132729201" data-name="VSIXProject_small.png"><div class="sbf">VSIXProject_small.png</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1063321536" class="browseFile"  data-fileid="43" data-pathid="1063321536" data-name="VSMDPythonProvider.cs"><div class="sbf">VSMDPythonProvider.cs</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=657178279" class="browseFile"  data-fileid="43" data-pathid="657178279" data-name="IronPython.sln"><div class="sbf">IronPython.sln</div></a> 
        <div class="ndbf">IronPython.suo</div>
        </div>
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">MPFProj</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Automation</div>
        </a>
        <div style="display:none;margin-left:20px;">
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">VSProject</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1723028936" class="browseFile"  data-fileid="43" data-pathid="1723028936" data-name="OAAssemblyReference.cs"><div class="sbf">OAAssemblyReference.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=362355259" class="browseFile"  data-fileid="43" data-pathid="362355259" data-name="OABuildManager.cs"><div class="sbf">OABuildManager.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=875513526" class="browseFile"  data-fileid="43" data-pathid="875513526" data-name="OAComReference.cs"><div class="sbf">OAComReference.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1701651551" class="browseFile"  data-fileid="43" data-pathid="1701651551" data-name="OAProjectReference.cs"><div class="sbf">OAProjectReference.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1242644523" class="browseFile"  data-fileid="43" data-pathid="1242644523" data-name="OAReferenceBase.cs"><div class="sbf">OAReferenceBase.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=833358186" class="browseFile"  data-fileid="43" data-pathid="833358186" data-name="OAReferences.cs"><div class="sbf">OAReferences.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=902304362" class="browseFile"  data-fileid="43" data-pathid="902304362" data-name="OAVSProject.cs"><div class="sbf">OAVSProject.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1193287207" class="browseFile"  data-fileid="43" data-pathid="1193287207" data-name="OAVSProjectItem.cs"><div class="sbf">OAVSProjectItem.cs</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=925938836" class="browseFile"  data-fileid="43" data-pathid="925938836" data-name="AutomationScope.cs"><div class="sbf">AutomationScope.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=413101654" class="browseFile"  data-fileid="43" data-pathid="413101654" data-name="OAFileItem.cs"><div class="sbf">OAFileItem.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=938077347" class="browseFile"  data-fileid="43" data-pathid="938077347" data-name="OAFolderItem.cs"><div class="sbf">OAFolderItem.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1709715840" class="browseFile"  data-fileid="43" data-pathid="1709715840" data-name="OANavigableProjectItems.cs"><div class="sbf">OANavigableProjectItems.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1359260971" class="browseFile"  data-fileid="43" data-pathid="1359260971" data-name="OANestedProjectItem.cs"><div class="sbf">OANestedProjectItem.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=610905712" class="browseFile"  data-fileid="43" data-pathid="610905712" data-name="OANullProperty.cs"><div class="sbf">OANullProperty.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1475341994" class="browseFile"  data-fileid="43" data-pathid="1475341994" data-name="OAProject.cs"><div class="sbf">OAProject.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1321764133" class="browseFile"  data-fileid="43" data-pathid="1321764133" data-name="OAProjectItem.cs"><div class="sbf">OAProjectItem.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1842825379" class="browseFile"  data-fileid="43" data-pathid="1842825379" data-name="OAProjectItems.cs"><div class="sbf">OAProjectItems.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1245573857" class="browseFile"  data-fileid="43" data-pathid="1245573857" data-name="OAProperties.cs"><div class="sbf">OAProperties.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1329015887" class="browseFile"  data-fileid="43" data-pathid="1329015887" data-name="OAProperty.cs"><div class="sbf">OAProperty.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1082321560" class="browseFile"  data-fileid="43" data-pathid="1082321560" data-name="OAReferenceFolderItem.cs"><div class="sbf">OAReferenceFolderItem.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1572652227" class="browseFile"  data-fileid="43" data-pathid="1572652227" data-name="OAReferenceItem.cs"><div class="sbf">OAReferenceItem.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2075755253" class="browseFile"  data-fileid="43" data-pathid="2075755253" data-name="OASolutionFolder.cs"><div class="sbf">OASolutionFolder.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Diagrams</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <div class="ndbf">AutomationClasses.cd</div>
        <div class="ndbf">ConfigurationClasses.cd</div>
        <div class="ndbf">DocumentManagerClasses.cd</div>
        <div class="ndbf">HierarchyClasses.cd</div>
        <div class="ndbf">PropertiesClasses.cd</div>
        <div class="ndbf">ReferenceClasses.cd</div>
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">LangProj</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Misc</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=488755772" class="browseFile"  data-fileid="43" data-pathid="488755772" data-name="ConnectionPointContainer.cs"><div class="sbf">ConnectionPointContainer.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1633015242" class="browseFile"  data-fileid="43" data-pathid="1633015242" data-name="ExternDll.cs"><div class="sbf">ExternDll.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2126026776" class="browseFile"  data-fileid="43" data-pathid="2126026776" data-name="NativeMethods.cs"><div class="sbf">NativeMethods.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=68723290" class="browseFile"  data-fileid="43" data-pathid="68723290" data-name="UnsafeNativeMethods.cs"><div class="sbf">UnsafeNativeMethods.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Properties</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1945588856" class="browseFile"  data-fileid="43" data-pathid="1945588856" data-name="AssemblyInfo.cs"><div class="sbf">AssemblyInfo.cs</div></a> 
        </div>
                                                      
        <a href="javascript: void(0)" class="browserDir">
            <div class="sbfc">Resources</div>
        </a>
        <div style="display:none;margin-left:20px;">
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1751845227" class="browseFile"  data-fileid="43" data-pathid="1751845227" data-name="imagelis.bmp"><div class="sbf">imagelis.bmp</div></a> 
        </div>
        
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=863058662" class="browseFile"  data-fileid="43" data-pathid="863058662" data-name="AssemblyReferenceNode.cs"><div class="sbf">AssemblyReferenceNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=504935726" class="browseFile"  data-fileid="43" data-pathid="504935726" data-name="Attributes.cs"><div class="sbf">Attributes.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1600321844" class="browseFile"  data-fileid="43" data-pathid="1600321844" data-name="BuildDependency.cs"><div class="sbf">BuildDependency.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1935523780" class="browseFile"  data-fileid="43" data-pathid="1935523780" data-name="BuildPropertyPage.cs"><div class="sbf">BuildPropertyPage.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=775148575" class="browseFile"  data-fileid="43" data-pathid="775148575" data-name="BuildStatus.cs"><div class="sbf">BuildStatus.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=998498314" class="browseFile"  data-fileid="43" data-pathid="998498314" data-name="ComReferenceNode.cs"><div class="sbf">ComReferenceNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2126804935" class="browseFile"  data-fileid="43" data-pathid="2126804935" data-name="ConfigProvider.cs"><div class="sbf">ConfigProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1916822478" class="browseFile"  data-fileid="43" data-pathid="1916822478" data-name="ConfigurationProperties.cs"><div class="sbf">ConfigurationProperties.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1985953340" class="browseFile"  data-fileid="43" data-pathid="1985953340" data-name="DataObject.cs"><div class="sbf">DataObject.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=635182242" class="browseFile"  data-fileid="43" data-pathid="635182242" data-name="DependentFileNode.cs"><div class="sbf">DependentFileNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2124231801" class="browseFile"  data-fileid="43" data-pathid="2124231801" data-name="DesignPropertyDescriptor.cs"><div class="sbf">DesignPropertyDescriptor.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=753194160" class="browseFile"  data-fileid="43" data-pathid="753194160" data-name="DesignTimeAssemblyResolution.cs"><div class="sbf">DesignTimeAssemblyResolution.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1723362004" class="browseFile"  data-fileid="43" data-pathid="1723362004" data-name="DocumentManager.cs"><div class="sbf">DocumentManager.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=42437843" class="browseFile"  data-fileid="43" data-pathid="42437843" data-name="EnumDependencies.cs"><div class="sbf">EnumDependencies.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1558140313" class="browseFile"  data-fileid="43" data-pathid="1558140313" data-name="FileChangeManager.cs"><div class="sbf">FileChangeManager.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=725330703" class="browseFile"  data-fileid="43" data-pathid="725330703" data-name="FileDocumentManager.cs"><div class="sbf">FileDocumentManager.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=781057786" class="browseFile"  data-fileid="43" data-pathid="781057786" data-name="FileNode.cs"><div class="sbf">FileNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1876377557" class="browseFile"  data-fileid="43" data-pathid="1876377557" data-name="FolderNode.cs"><div class="sbf">FolderNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=914028207" class="browseFile"  data-fileid="43" data-pathid="914028207" data-name="GlobalSuppressions.cs"><div class="sbf">GlobalSuppressions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1956220633" class="browseFile"  data-fileid="43" data-pathid="1956220633" data-name="HierarchyNode.cs"><div class="sbf">HierarchyNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=846944096" class="browseFile"  data-fileid="43" data-pathid="846944096" data-name="IDEBuildLogger.cs"><div class="sbf">IDEBuildLogger.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1490990564" class="browseFile"  data-fileid="43" data-pathid="1490990564" data-name="ImageHandler.cs"><div class="sbf">ImageHandler.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=704624217" class="browseFile"  data-fileid="43" data-pathid="704624217" data-name="Interfaces.cs"><div class="sbf">Interfaces.cs</div></a> 
        <div class="ndbf">Key.snk</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1574323510" class="browseFile"  data-fileid="43" data-pathid="1574323510" data-name="LocalizableProperties.cs"><div class="sbf">LocalizableProperties.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=307986897" class="browseFile"  data-fileid="43" data-pathid="307986897" data-name="Microsoft.VisualStudio.Project.csproj"><div class="sbf">Microsoft.VisualStudio.Project.csproj</div></a> 
        <div class="ndbf">Microsoft.VisualStudio.Project.csproj.vspscc</div>
        <div class="ndbf">MPFProjectAll.files</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1382180472" class="browseFile"  data-fileid="43" data-pathid="1382180472" data-name="NestedProjectBuildDependency.cs"><div class="sbf">NestedProjectBuildDependency.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1919743302" class="browseFile"  data-fileid="43" data-pathid="1919743302" data-name="NestedProjectNode.cs"><div class="sbf">NestedProjectNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1890696267" class="browseFile"  data-fileid="43" data-pathid="1890696267" data-name="NodeProperties.cs"><div class="sbf">NodeProperties.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=325167604" class="browseFile"  data-fileid="43" data-pathid="325167604" data-name="OleServiceProvider.cs"><div class="sbf">OleServiceProvider.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=247435001" class="browseFile"  data-fileid="43" data-pathid="247435001" data-name="Output.cs"><div class="sbf">Output.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=503603134" class="browseFile"  data-fileid="43" data-pathid="503603134" data-name="OutputGroup.cs"><div class="sbf">OutputGroup.cs</div></a> 
        <div class="ndbf">ProjectBase.files</div>
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1490091578" class="browseFile"  data-fileid="43" data-pathid="1490091578" data-name="ProjectConfig.cs"><div class="sbf">ProjectConfig.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1073272953" class="browseFile"  data-fileid="43" data-pathid="1073272953" data-name="ProjectContainerNode.cs"><div class="sbf">ProjectContainerNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=521331222" class="browseFile"  data-fileid="43" data-pathid="521331222" data-name="ProjectDesignerDocumentManager.cs"><div class="sbf">ProjectDesignerDocumentManager.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=891547067" class="browseFile"  data-fileid="43" data-pathid="891547067" data-name="ProjectDocumentsListener.cs"><div class="sbf">ProjectDocumentsListener.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1893217616" class="browseFile"  data-fileid="43" data-pathid="1893217616" data-name="ProjectElement.cs"><div class="sbf">ProjectElement.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1524371994" class="browseFile"  data-fileid="43" data-pathid="1524371994" data-name="ProjectFactory.cs"><div class="sbf">ProjectFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=524249017" class="browseFile"  data-fileid="43" data-pathid="524249017" data-name="ProjectFileConstants.cs"><div class="sbf">ProjectFileConstants.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=792426122" class="browseFile"  data-fileid="43" data-pathid="792426122" data-name="ProjectNode.CopyPaste.cs"><div class="sbf">ProjectNode.CopyPaste.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=677582" class="browseFile"  data-fileid="43" data-pathid="677582" data-name="ProjectNode.cs"><div class="sbf">ProjectNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1133220256" class="browseFile"  data-fileid="43" data-pathid="1133220256" data-name="ProjectNode.Events.cs"><div class="sbf">ProjectNode.Events.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1601370183" class="browseFile"  data-fileid="43" data-pathid="1601370183" data-name="ProjectOptions.cs"><div class="sbf">ProjectOptions.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=498092120" class="browseFile"  data-fileid="43" data-pathid="498092120" data-name="ProjectPackage.cs"><div class="sbf">ProjectPackage.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1555862790" class="browseFile"  data-fileid="43" data-pathid="1555862790" data-name="ProjectReferenceNode.cs"><div class="sbf">ProjectReferenceNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=337438316" class="browseFile"  data-fileid="43" data-pathid="337438316" data-name="PropertiesEditorLauncher.cs"><div class="sbf">PropertiesEditorLauncher.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=2011303928" class="browseFile"  data-fileid="43" data-pathid="2011303928" data-name="ReferenceContainerNode.cs"><div class="sbf">ReferenceContainerNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=861057871" class="browseFile"  data-fileid="43" data-pathid="861057871" data-name="ReferenceNode.cs"><div class="sbf">ReferenceNode.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=895228554" class="browseFile"  data-fileid="43" data-pathid="895228554" data-name="RegisteredProjectType.cs"><div class="sbf">RegisteredProjectType.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=26779091" class="browseFile"  data-fileid="43" data-pathid="26779091" data-name="SelectionListener.cs"><div class="sbf">SelectionListener.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1814282288" class="browseFile"  data-fileid="43" data-pathid="1814282288" data-name="SettingsPage.cs"><div class="sbf">SettingsPage.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=498721032" class="browseFile"  data-fileid="43" data-pathid="498721032" data-name="SingleFileGenerator.cs"><div class="sbf">SingleFileGenerator.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=499248984" class="browseFile"  data-fileid="43" data-pathid="499248984" data-name="SingleFileGeneratorFactory.cs"><div class="sbf">SingleFileGeneratorFactory.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=550118118" class="browseFile"  data-fileid="43" data-pathid="550118118" data-name="SolutionListener.cs"><div class="sbf">SolutionListener.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1739326025" class="browseFile"  data-fileid="43" data-pathid="1739326025" data-name="SolutionListenerForBuildDependencyUpdate.cs"><div class="sbf">SolutionListenerForBuildDependencyUpdate.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=580038452" class="browseFile"  data-fileid="43" data-pathid="580038452" data-name="SolutionListenerForProjectEvents.cs"><div class="sbf">SolutionListenerForProjectEvents.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=437765785" class="browseFile"  data-fileid="43" data-pathid="437765785" data-name="SolutionListenerForProjectOpen.cs"><div class="sbf">SolutionListenerForProjectOpen.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=951205941" class="browseFile"  data-fileid="43" data-pathid="951205941" data-name="SolutionListenerForProjectReferenceUpdate.cs"><div class="sbf">SolutionListenerForProjectReferenceUpdate.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1908859235" class="browseFile"  data-fileid="43" data-pathid="1908859235" data-name="StructuresEnums.cs"><div class="sbf">StructuresEnums.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=679477073" class="browseFile"  data-fileid="43" data-pathid="679477073" data-name="SuspendFileChanges.cs"><div class="sbf">SuspendFileChanges.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1393873852" class="browseFile"  data-fileid="43" data-pathid="1393873852" data-name="TokenProcessor.cs"><div class="sbf">TokenProcessor.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=656223562" class="browseFile"  data-fileid="43" data-pathid="656223562" data-name="Tracing.cs"><div class="sbf">Tracing.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1106714065" class="browseFile"  data-fileid="43" data-pathid="1106714065" data-name="TrackDocumentsHelper.cs"><div class="sbf">TrackDocumentsHelper.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=530560961" class="browseFile"  data-fileid="43" data-pathid="530560961" data-name="TypeConverters.cs"><div class="sbf">TypeConverters.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1841631104" class="browseFile"  data-fileid="43" data-pathid="1841631104" data-name="UIThread.cs"><div class="sbf">UIThread.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=692572913" class="browseFile"  data-fileid="43" data-pathid="692572913" data-name="UpdateSolutionEventsListener.cs"><div class="sbf">UpdateSolutionEventsListener.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1725989037" class="browseFile"  data-fileid="43" data-pathid="1725989037" data-name="Url.cs"><div class="sbf">Url.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=168242134" class="browseFile"  data-fileid="43" data-pathid="168242134" data-name="Utilities.cs"><div class="sbf">Utilities.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1590604811" class="browseFile"  data-fileid="43" data-pathid="1590604811" data-name="VisualStudio.Project.cs"><div class="sbf">VisualStudio.Project.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=253287041" class="browseFile"  data-fileid="43" data-pathid="253287041" data-name="VisualStudio.Project.resx"><div class="sbf">VisualStudio.Project.resx</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=574030874" class="browseFile"  data-fileid="43" data-pathid="574030874" data-name="VsCommands.cs"><div class="sbf">VsCommands.cs</div></a> 
        <a href="/IronPython-Integration-6b03988d/sourcecode?fileId=43&amp;pathId=1973809780" class="browseFile"  data-fileid="43" data-pathid="1973809780" data-name="VSShellUtilities.cs"><div class="sbf">VSShellUtilities.cs</div></a> 
        <div class="ndbf">WebProjectBase.Files</div>
        </div>
        
        </div>
        
            </div>
            </div>
    </div>
</div>

<div id="sourceItem">

</div>

        </div>

                <div id="Discussions_Content">
                        <a href="https://login.live.com/login.srf?wa=wsignin1.0&amp;wtrealm=code.msdn.microsoft.com&amp;wreply=https%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d%2fview%2fDiscussions%2f0%3fshowDiscussionForm%3dTrue%26stoAI%3d10&amp;wp=MBI_FED_SSL&amp;wlcxt=microsoft%24microsoft%24microsoft">Sign in to Ask a Question</a>
                    <br /><br />
                    <div id="DiscussionsTabPane">
                        


<script language="javascript">
    function displayCreateDiscussion() {
        $('#newDiscussion').show();
        $('#defaultDicussionText').hide();
    }
    
    function cancelCreateDiscussion() {
        $('#newDiscussion').hide();
        $('#defaultDicussionText').show();
        $('#Title').val('');
        $('.discussionFormTable #Text').val('');
        $('.discussionFormTable .field-validation-error').text('');
    }

    function ToggleSaveReply(replyLink, threadId) {
        var saveReplySectionToOpen = $('#postReplyDiv');
        if (saveReplySectionToOpen.is(":visible")) {
            $('#saveReplyErrorMessage').text('');
            saveReplySectionToOpen.hide();
        }
        if (replyLink != null) {
            var parentLoc = $('.postSaveReplyLocation', $(replyLink).parents('.thread'));
            $('#PostReply').val('');
            $('#PostReplyThreadId').val(threadId);
            saveReplySectionToOpen.appendTo(parentLoc).slideToggle("normal");
            saveReplySectionToOpen[0].scrollIntoView();
        }
    }

    function SetDiscussionHidden(id, discussionType, isHidden) {
        var form = $('#setDiscussionForm');
        $('#id', form).val(id);
        $('#discussionType', form).val(discussionType);
        $('#isHidden', form).val(isHidden);
        form.submit();
    }

    function saveReply() {
        var replyText = $("#PostReply").val();
        var threadId = $('#PostReplyThreadId').val();
        
        var hasErrors = false;
        var errorMessage = "";
        if (!replyText || replyText.length == 0) {
            errorMessage = $('#postReplyDiv').attr("requiredMessage");
            hasErrors = true;
        }
        else if(replyText.length > 2000) {
             errorMessage = $('#postReplyDiv').attr("maxLengthMessage");
             hasErrors = true;
        }

        if(hasErrors)
        {
            $('#saveReplyErrorMessage').text(errorMessage);
            $('#saveReplyErrorMessage').show();
            return;
        }

        $('#saveReplyErrorMessage').hide();
        $('#PostReply').val('');

        $.ajax(
        {
            type: "POST",
            url: "/IronPython-Integration-6b03988d/discussion/reply",
            data: {threadId : threadId, threadText : replyText},
            dataType: "json",
            success: function(result) {
                if (result.success) {
                    window.location = "/IronPython-Integration-6b03988d/view/Discussions";
                }
                else {
                    var errorMessage = result.errorMessage;
                    $('#saveReplyErrorMessage').text(errorMessage);
                    $('#saveReplyErrorMessage').show();
                }
            },
            error: function(req, status, Error) {
                var errorMessage = $('#postReplyDiv').attr("unexpectedErrorMessage");
                $('#saveReplyErrorMessage').text(errorMessage);
                $('#saveReplyErrorMessage').show();
            }
        });
    }
</script>

<div id="newDiscussion" style="display:none">
    <form action="/IronPython-Integration-6b03988d/discussion" id="reviewsForm" method="post"><input name="__RequestVerificationToken" type="hidden" value="k/lelGUfZuAsk4ENkChOsAf+3cfVcGi3HjLyyXUY2ZfK8OiZlSaRACbvHWG6ouM/AhE3ZEfJLEg28qTej4Dlg+KOr+guNe6ypO+4V8Fwx5I0qnuNqvKBOnMkKxsXyIjpnD+6GA==" />
       <table class="discussionFormTable">
            <tr>
                <td>
                    Question: <span class="required">*</span>
                </td>
            </tr>
            <tr>
                <td>
                    <input id="Title" maxlength="250" name="Title" type="text" value="" />
                    <br />
                    
                </td>
            </tr>
            <tr>
                <td>
                    Text ( Maximum of 2000 Characters ): <span class="required">*</span>
                </td>
            </tr>
            <tr>
                <td>
                    <textarea cols="20" id="Text" maxlength="2000" name="Text" rows="2">
</textarea>
                    <br />
                    
                </td>
            </tr>
    </table>
    <div id="Actions">
        <div class="middle">
            <input id="uploadButton" class="bevelButton" type="submit" value="Submit" />
            <input id="cancelButton" class="bevelButton" type="button" value="Cancel" onclick="cancelCreateDiscussion()" />
        </div>
        <div class="clear">
        </div>
    </div>
    </form>
</div>
<div id="setHiddenDiv">
    <form action="/IronPython-Integration-6b03988d/discussion/SetHidden" id="setDiscussionForm" method="post"><input id="pageIndex" name="pageIndex" type="hidden" value="0" />
        <input id="id" name="id" type="hidden" value="" />
        <input id="discussionType" name="discussionType" type="hidden" value="" />
        <input id="isHidden" name="isHidden" type="hidden" value="" />
    </form>
</div>
<div id="postReplyDiv" class="postSaveReply" unexpectederrormessage="An unexpected error occurred."
    requiredmessage="The reply text is required." maxlengthmessage="Text has a maximum of 2000 characters">
    <hr />
    <table class="discussionFormTable">
        <tr>
            <td>
                <div>
                    <input id="PostReplyThreadId" name="PostReplyThreadId" type="hidden" value="" />
                    <textarea cols="20" id="PostReply" maxlength="2000" name="PostReply" rows="2">
</textarea>
                </div>
                <div id="saveReplyErrorMessage" class="saveReplyErrorMessage">
                </div>
                <div>
                    <input id="SaveReply" class="bevelButton" type="button" value="Save Answer" onclick="saveReply()" />
                    <input id="CancelReply" class="bevelButton" type="button" value="Cancel" onclick="ToggleSaveReply(null, null)" />
                </div>
            </td>
        </tr>
    </table>
</div>



    <div id="defaultDicussionText">
        Be the first to ask a question.
    </div>

                    </div>
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

             $('.lnkBackToSearch').click(function() {
                gTracker.createActionEvent('EyeBrow', 'Link', 'Click', 'Link', 'BackToSearchResult');
             });

             registerPageView();
             $('.tabHeaders > div').Tabify('current', 'Details', function(tabId) {
                if ((tabId == 'SourceCode_Tab') || (tabId == 'SamplePack_Tab')) {
                    $('div.collapsableSidebar').addClass('hiddenSidebar');
                    $('#projectBody > div:first-child').addClass('fullProjectBody');
                    Galleries.utility.waitFor(
                        function(){ return $("#userCard .profile-biography").length > 0;},
                        function(){$("#userCard .profile-biography").addClass('hiddenSidebar');});
                    
                }
                else {
                    $('div.collapsableSidebar, #userCard .profile-biography').removeClass('hiddenSidebar');
                    $('#projectBody > div:first-child').removeClass('fullProjectBody');
                }
                Galleries.project.resizeDesc();
                
                 if ($('#' + tabId).data('pageLoad') !== true) {
                     Galleries.url.recordAdditionalPageView(tabId, '\x2fIronPython-Integration-6b03988d\x2fstats\x2fRegisterPageView' );
                 }
                 if (gTracker) {
                     gTracker.createSectionEvent(tabId);
                     gTracker.createActionEvent(null, 'Tab', 'Click', 'Tab', tabId);
                 }
             });

             var reportAbuseFormUrl = '\x2fIronPython-Integration-6b03988d\x2freportabuse';
             $("#reportAbuse a[href='']").click(function() {
                    $('#projectContent').load(reportAbuseFormUrl);
                    return false;
                });
            
             
        });
        
        function registerPageView() {
            $.post('\x2fIronPython-Integration-6b03988d\x2fstats\x2fRegisterPageView');
        }
    </script>


        </div>
        <div class="clear">
        </div>
    </div>
    <div class="advertisment">
        <div id="Banner728">
	<div id="a82f74c4-e87c-4fc0-8087-6bdd8fc21505" name="a82f74c4-e87c-4fc0-8087-6bdd8fc21505" align="center" style="margin:auto;" class="AdWidthFix"></div><script type="text/javascript">dapMgr.enableACB("a82f74c4-e87c-4fc0-8087-6bdd8fc21505", false);dapMgr.renderAd("a82f74c4-e87c-4fc0-8087-6bdd8fc21505", "&PG=CMSCGB&AP=1390", 728, 90);</script>
</div>

    </div>

                                    <div class="Clear"></div>
                                </div>
    
                            </div>
                        </div>
                    </div>
                
                    <div class="Clearbottom"></div>          
                    <div class="bottomleftcorner"></div>            
                    <div class="bottomrightcorner"></div> 
                    
                    <div id="Footer">
                        <div class="FooterLogoContainer"><a href="http://www.microsoft.com/en/us/default.aspx"><div class="FooterLogo" title="Microsoft Corporation"> </div></a></div><div id="FooterCopyright" class="FooterCopyright">© 2011  Microsoft. All rights reserved.</div><div class="FooterLinks"><span class="FooterAnchorList"><a href="http://msdn.microsoft.com/cc300389.aspx">Terms of Use</a><span class="Pipe">|</span><a href="http://www.microsoft.com/library/toolbar/3.0/trademarks/en-us.mspx">Trademarks</a><span class="Pipe">|</span><a href="http://www.microsoft.com/info/privacy.mspx">Privacy Statement</a><span class="Pipe">|</span><a href="https://lab.msdn.microsoft.com/mailform/contactus.aspx?refurl=http%3a%2f%2fcode.msdn.microsoft.com%2fIronPython-Integration-6b03988d">Site Feedback</a></span></div>
                        
    <div id="buildVersion">
        Version:
        2011.11.17.3866</div>

                    </div>                             
                </div>
            </div>
        </div>  
    </div>
    <div data-chameleon-template="megablade" ></div>
    
    <script type="text/javascript">
        Galleries.utility.loadJavaScript("http://widgets.membership.s-msft.com/v1/loader.js?brand=Msdn&lang=en-US", true);
        Galleries.utility.loadJavaScript('http://js.microsoft.com/library/svy/sto/broker.js', true);
    </script>    

    <script src="http://i1.services.social.s-msft.com/search/Widgets/SearchBox.jss?boxid=SearchTextBox&btnid=SearchButton&brand=Msdn&loc=en-US&resref=203&addEnglish=&rn=&rq=&watermark=&focusOnInit=False&beta=0&iroot=samplesgallery&overrideWatermark=false&cver=0001" type="text/javascript" language="javascript"></script>
    <script src="http://i1.code.msdn.s-msft.com/GlobalResources/scripts/common.min.js?cver=0001" type="text/javascript" language="javascript"></script>
    <script src="http://i1.code.msdn.s-msft.com/GlobalResources/scripts/localepop.js?cver=0001" type="text/javascript" language="javascript"></script>
    
        <script type="text/javascript">
            loadJavaScript('http://i1.code.msdn.s-msft.com/GlobalResources/Scripts/omni_rsid_social_min.js?cver=0001');
        </script>
        <noscript><a href="http://www.omniture.com" title="Web Analytics">
        <img src="http://msstonojssocial.112.2O7.net/b/ss/msstonojssocial/1/H.20.2--NS/0" height="1" width="1" border="0" alt="" />
        </a></noscript>
    
</body>
</html>
