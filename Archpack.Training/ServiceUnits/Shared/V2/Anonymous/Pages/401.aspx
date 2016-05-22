<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="401.aspx.cs" Inherits="Archpack.Training.ServiceUnits.Shared.V2.Anonymous.Pages._401" %>
<%@ Import Namespace="Archpack.Training.ServiceUnits.Shared.V2.Resources" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta runat="server" id="refresh" http-equiv="refresh" />
    <title><%=StringContents.UnauthorizedErrorTitle %></title>

    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/lib/bootstrap/css/bootstrap.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/Content/site.css") %>" type="text/css" />

    <style type="text/css">

        body {
            background-color: #ffffff;
        }

        .error-title{
            font-size: 120px;
        }

        .error-title-message {
            margin-top: 18px;
            padding-left: 12px;
            padding-top: 6px;
            padding-bottom: 6px;
            border-left: solid 4px #ff0000;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="page-header">
            <h1 class="error-title"><span>:(</span></h1>
            <h3 class="error-title-message"><%=StringContents.UnauthorizedErrorMessage %></h3>
        </div>
        <div class="" role="alert">
            <a href='<%=RedirectUrl %>'><%=StringContents.UnauthorizedErrorNavigationMessage %></a>
        </div>        
    </div>

        
    <!--[if IE 8]>
        <script type="text/javascript">
            if (navigator.appVersion.indexOf("MSIE 8.") != -1 || navigator.appVersion.indexOf("MSIE 7.") != -1) {
                JSON = undefined;
            }
        </script>        
    <![endif]-->

    <!--[if lt IE 9]>
          <script src="<%=ResolveUrl("../../wwwroot/Scripts/respond.min.js") %>"></script>
    <![endif]-->

    <script src="<%=ResolveUrl("../../wwwroot/lib/jquery/jquery.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../wwwroot/lib/bootstrap/js/bootstrap.js") %>" type="text/javascript"></script>
</body>
</html>
