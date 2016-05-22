<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Archpack.Training.ServiceUnits.Shared.V2.Anonymous.Pages.Login" %>
<%@ Import Namespace="Archpack.Training.ServiceUnits.Shared.V2.Resources" %>
<%@ Import Namespace="Archpack.Training.ArchUnits.Routing.WebForm.V1" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <title><%= StringContents.Login %></title>

    <link media="all" rel="stylesheet" href="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/bootstrap/css/bootstrap.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=this.ResolveSuitableFileUrl("../../wwwroot/Content/site.css") %>" type="text/css" />

    <style type="text/css">
        .container {
        }

        .form-login-heading {
            margin-bottom: 12px;
        }

        .main {
            margin: auto;
            max-width: 450px;

            background-color: #ffffff;
            border-radius: 4px;
            padding: 24px;
            box-shadow: 0 2px 2px 0 rgba(32,32,32,.14),0 3px 1px -2px rgba(0,0,0,.2),0 1px 5px 0 rgba(0,0,0,.12);

            position: relative;
            top: 100px;
        }

        pre {
            overflow: auto;
        }

        input[type=text], input[type=password] {
            padding: 6px 12px;
        }

        .footer {
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
        }
    </style>

    <%if (this.Context.GetServiceUnitContext().IsTestMode())
    { %>
    <link media="all" rel="stylesheet" href="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/jasmine/jasmine-core/jasmine.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=this.ResolveSuitableFileUrl("../../wwwroot/Content/screentest.css") %>" type="text/css" />
    <%} %>


</head>
<body>
    <div class="wrap">


        <div class="main container">
            <form class="form-horizontal" id="mainform" runat="server">
                <h2 class="form-login-heading page-title"><%= StringContents.Login %></h2>
                <div class="form-group">
                    <label for="userName" class="col-sm-3 control-label"><%= StringContents.UserName %></label>
                    <div class="col-sm-9">
                        <input class="form-control" type="text" id="userName" name="userName" data-prop="userName" runat="server"/>
                    </div>
                </div>
                <div class="form-group">
                    <label for="password" class="col-sm-3 control-label"><%= StringContents.Password %></label>
                    <div class="col-sm-9">
                        <input type="password" class="form-control" id="password" data-prop="password" runat="server">
                    </div>
                </div>
                <p class="commands pull-right">
                    <button name="loginButton" id="login" type="submit" class="btn btn-primary"><%= StringContents.Login %></button>
                </p>
            </form>
        </div>
        <div class="footer">
            <div class="footer-container">
                <div data-part-template="slideupmessage">
                    <div class="message-area slideup-area">
                        <div class="alert-message" style="display: none">
                            <ul></ul>
                        </div>
                        <div class="info-message" style="display: none">
                            <ul></ul>
                        </div>
                    </div>
                </div>
            </div>
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
          <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/Scripts/respond.min.js") %>"></script>
    <![endif]-->

    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/jquery/jquery.js") %>" type="text/javascript"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/bootstrap/js/bootstrap.js") %>" type="text/javascript"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/bootstrap-datepicker/js/bootstrap-datepicker.js") %>" type="text/javascript"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/ArchPackJs/js/app.js") %>" type="text/javascript"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/ArchPackJs/js/app.ui.js") %>" type="text/javascript"></script>

    <%-- TODO: 言語ごとのリソースファイルの読み込み --%>
    <script src="<%=this.ResolveSuitableFileUrl("~/Shared/V2/resource/StringContents?c=" + System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName) %>" type="text/javascript"></script>
    <script src="<%=this.ResolveSuitableFileUrl("~/Shared/V2/resource/Messages?c=" + System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName) %>" type="text/javascript"></script>

    <script type="text/javascript">
        App.culture.current("<%= System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName %>");
        App.define("App.page").mode = "<%= this.Context.GetServiceUnitContext().IsTestMode() ? "test": "" %>";
        var errorMessage = "<%= this.LoginErrorMessage %>";
    </script>


    <%if (this.Context.GetServiceUnitContext().IsTestMode()) { %>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/jasmine/jasmine-core/jasmine.js") %>"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/jasmine/jasmine-core/jasmine-html.js") %>"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/jasmine/jasmine-core/boot.js") %>"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/jasmine-jquery/jasmine-jquery.js") %>"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/lib/blanket/qunit/blanket.js") %>"></script>
    <script src="<%=this.ResolveSuitableFileUrl("../../wwwroot/Scripts/screentest.js") %>"></script>
    <%} %>

    <%if (this.Context.GetServiceUnitContext().IsTestMode())
      { %>
        <script src="<%=this.ResolveSuitableFileUrl("./Login.test.js") %>"></script>
    <%} %>
    <script src="<%=this.ResolveSuitableFileUrl("./Login.js") %>" data-cover></script>    
</body>
</html>
