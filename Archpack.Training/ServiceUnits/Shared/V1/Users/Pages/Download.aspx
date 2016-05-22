<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="Archpack.Training.ServiceUnits.Shared.V1.Users.Pages.Download" %>
<%@ Import Namespace="Archpack.Training.ServiceUnits.Shared.V1.Resources" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/lib/bootstrap/css/bootstrap.css") %>" type="text/css" />
    <link media="all" rel="stylesheet" href="<%=ResolveUrl("../../wwwroot/Content/site.css") %>" type="text/css" />

    <title>ファイルダウンロード</title>
</head>
<body>

    <div class="content-wrap container">
        <div class="page-header">
            <h3><%=StringContents.FileDownloadTitle %></h3>
        </div>
        <div class="alert alert-warning" role="alert">
            <h4><%=StringContents.FIleDownloadMessage %></h4>
        </div>
    </div>
   

    <div id="download-container">
        <form method="post" accept-charset="UTF-8">

        </form>
    </div>

    <script type="text/javascript">
        (function(){
            function recieveMessage(event) {
                var title = document.querySelector(".page-header h3");
                title.textContent = "<%=StringContents.DownloadStartMessage %>";
                title.innerText = "<%=StringContents.DownloadStartMessage %>";
                //TODO: データ取得サービスの URLとオプションを記述します。
                var element = document.getElementById("download-container"),
                    form = element.getElementsByTagName("form")[0],
                    url = event.data.url;
                createAndAppendInput(event.data.data, form);
                form.action = url;
                form.submit();
            }
            function createAndAppendInput(data, form) {
                var input;
                for (var key in data) {
                    if (data.hasOwnProperty(key)) {
                        input = document.createElement("input");
                        input.type = "hidden";
                        input.name = key;
                        input.value = data[key];
                        //input = $("<input type='hidden' name='" + key + "' value='" + data[key] + "'>");
                        form.appendChild(input);
                    }
                }
            };

            function splitQuery(query) {
                var queryStr = (query || ""),
                    items = (queryStr.charAt(0) === "?" ? queryStr.substr(1) : queryStr).split("&"),
                    i = 0, l = items.length, item, keyValue, result = {};
                for (; i < l; i++) {
                    item = items[i];
                    if (!item) {
                        continue;
                    }
                    keyValue = item.split("=");
                    if (keyValue.length < 1) {
                        continue;
                    }
                    if (keyValue.length < 2) {
                        result[keyValue[0]] = undef;
                        continue;
                    }
                    result[keyValue[0]] = decodeURIComponent(keyValue[1].replace(/\+/g, " "));
                }
                return result;
            }

            if (window.opener && !window.opener.closed && window.opener.postMessage) {

                var dlkey = splitQuery(location.search)["dlkey"];
                if (!dlkey) {
                    return;
                }

                if (window.attachEvent) {
                    window.attachEvent("message", recieveMessage);
                } else {
                    window.addEventListener("message", recieveMessage, false);
                }

                window.opener.postMessage(dlkey, location.protocol + "//" + location.host);
            }

        })();
    </script>

</body>
</html>
