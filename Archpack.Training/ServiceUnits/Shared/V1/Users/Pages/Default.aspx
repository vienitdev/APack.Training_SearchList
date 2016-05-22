<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Archpack.Training.ServiceUnits.Shared.V1.Users.Pages.Default" %>

<!DOCTYPE html>

<html lang="ja">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <title>ArchPack</title>

    <!-- CSS -->
    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/lib/bootstrap/css/bootstrap.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/lib/bootstrap-datepicker/css/bootstrap-datepicker.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/Content/site.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=ResolveUrl("Default.css") %>" type="text/css" />

</head>
<body id="body">
    <!-- ヘッダーのレイアウト -->
    <div class="screen-header">
        <div class="navbar navbar-inset navbar-fixed-top">
            <div class="navbar-header">
                <div class="navbar-brand">
    
                    <div class="screen-title">
                        <span class="page-title" data-prop="page-title">Archway Architecture Package</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="screen-body">
        <div class="side-bar">
            <ul class="side-bar-items">
                <li class="side-bar-item">
                    <a href="<%=ResolveUrl("~/Membership/V1/Users/page/UserSearchList") %>">ユーザー管理 (V1)</a>
                </li>
                <li class="side-bar-item">
                    <a href="<%=ResolveUrl("~/Membership/V1/Users/page/RoleSearchListInput") %>">ロール管理 (V1)</a>
                </li>
                <li class="side-bar-item">
                    <a href="<%=ResolveUrl("~/Develop/V1/Users/page/ProjectSearch") %>">生成ツール</a>
                </li>
            </ul>
        </div>
        <div class="card-content">
            <div class="card-commands">
                <div class="commands">
                    <button class="btn btn-primary add-card-button circle-btn">
                        <span style="position: relative; top: -19px; left: -7px; font-size: 36px">+</span>
                    </button>
                </div>
            </div>
            <%--<div class="datalist">
                <div class="row">
                    <div class="col-sm-4">
                        <div class="datalist-item" style="overflow: hidden;">
                            <button class="btn btn-primary circle-btn"></button>
                            <a href="/Membership/V1/Users/page/UserSearchList">ユーザー検索</a>
                            <hr style="margin-top: 0.5em; margin-bottom: 0.5em;" />
                            <div class="datalist-item-body" style="height: 130px; white-space: nowrap; -ms-overflow-x: hidden; -ms-overflow-y: auto;">
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="datalist-item" style="overflow: hidden;">
                            <button class="btn btn-primary circle-btn"></button>
                            <a href="/Membership/V1/Users/page/UserInput">ユーザー作成</a>
                            <hr style="margin-top: 0.5em; margin-bottom: 0.5em;" />
                            <div class="datalist-item-body" style="height: 130px; white-space: nowrap; -ms-overflow-x: hidden; -ms-overflow-y: auto;">
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-4">
                        <div class="datalist-item" style="overflow: hidden;">
                            <button class="btn btn-primary circle-btn"></button>
                            <a href="/Membership/V1/Users/page/RoleSearchListInput">ロール検索編集</a>
                            <hr style="margin-top: 0.5em; margin-bottom: 0.5em;" />
                            <div class="datalist-item-body" style="height: 130px; white-space: nowrap; -ms-overflow-x: hidden; -ms-overflow-y: auto;">
                            </div>
                        </div>
                    </div>
                </div>
            </div>--%>
        </div>
    </div>

    <script src="<%=ResolveUrl("../../wwwroot/lib/jquery/jquery.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../wwwroot/lib/bootstrap/js/bootstrap.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../wwwroot/lib/bootstrap-datepicker/js/bootstrap-datepicker.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../wwwroot/lib/ArchPackJs/js/app.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../wwwroot/lib/ArchPackJs/js/app.ui.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../wwwroot/Scripts/portalmenu.js") %>" type="text/javascript"></script>
    <!-- TODO: 言語ごとのリソースファイルの読み込み -->
    <script src="<%=ResolveUrl("../../Resources/settings.js") %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("../../Resources/settings.base.js") %>" type="text/javascript"></script>

</body>
</html>
