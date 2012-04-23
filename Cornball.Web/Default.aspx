<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Cornball.Web.Default" %>
<%@ Import Namespace="Cornball.Web.Resources" %>
<?xml version="1.0" encoding="iso-8859-1" ?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="se" xml:lang="se">
    <head>
        <title><%= Strings.Title %></title>
        <meta http-equiv="content-type" content="text/html; charset=ISO-8859-1" />
        <meta name="description" content="<%= Strings.GameDescription %>" />
        <meta name="keywords" content="lantisen, cornball, stodell, patiens, kort, spel, card, game, solitaire" />
        <meta name="author" content="Victor Stodell" />
        <link rel="stylesheet" type="text/css" href="Resources/style.css" />
        <link rel="shortcut icon" type="image/x-icon" href="favicon.ico" />
        <script type="text/javascript" src="Resources/script.js"> </script>
        <script type="text/javascript">
            var _gaq = _gaq || [];
            _gaq.push(['_setAccount', 'UA-13022277-2']);
            _gaq.push(['_trackPageview']);
            (function () {
                var ga = document.createElement('script');
                ga.type = 'text/javascript';
                ga.async = true;
                ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(ga);
            })();
        </script>
    </head>
    <body onload="makeTopLocation();">
        <div>
            <object type="application/x-silverlight-2" data="data:application/x-silverlight-2,">
                <param name="source" value="Resources/Cornball.xap?v=<%= Version %>" />
                <param name="onError" value="onSilverlightError" />
                <param name="minRuntimeVersion" value="5.0.61118.0" />
                <param name="splashScreenSource" value="Resources/SplashScreen.xaml"/>
                <param name="onSourceDownloadProgressChanged" value="onSourceDownloadProgressChanged" />
                <param name="autoUpgrade" value="true" />
                <h1><%= Strings.Header %></h1>
                <h2><%= Strings.SubHeader %></h2>
                <p><%= Strings.GameDescription %></p>
                <p><%= Strings.SilverlightDescription %></p>
                <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0">
                    <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" />
                </a>
            </object>
        </div>
    </body>
</html>