 /*
 * Copyright(c) Archway Inc. All rights reserved.
 */
 ;(function (global, $, undef) {
 
     "use strict";
 
     var ddlmenu = App.define("App.ui.ddlmenu", {
 
         setting: {},
         isShowed: false,
         context: {},
         settings: function (title, setting) {
             ddlmenu.settingsObj = {
                 title: title,
                 setting: setting
             };
         },
 
         settingsObj: {},
 
         setup: function (role, container, baseUrl) {
 
 
             var base = baseUrl.replace(/\/$/, "");
 
             ddlmenu.setting = ddlmenu.settingsObj || { setting: [], title: "" };
 
             role = App.ifUndefOrNull(role, "");
             var root = createTopElement(ddlmenu.setting.setting || [], 1001, role, base);
 
             $(container).append(root);
 
         }
     });
 
     var isVisibleRole = function (item, role) {
 
         var visible = false,
             i;
 
         if (!item.visible) {
             return true;
         }
 
         // visible が "*" 以外の文字列で指定されていて、 role と一致しない場合は表示しない
         if (App.isStr(item.visible) && item.visible !== "*" && item.visible !== role) {
             return visible;
         }
             // visible が 配列で role とどれも一致しない場合は表示しない
         else if (App.isArray(item.visible)) {
 
             for (i = 0; i < item.visible.length; i++) {
                 if (item.visible[i] === role) {
                     visible = true;
                     break;
                 }
             }
 
             return visible;
         }
             // visible が 関数で戻り値が false の場合は表示しない
         else if (App.isFunc(item.visible)) {
             return item.visible(role);
         }
 
         return true;
     };
 
     var createTopElement = function (items, zIndex, role, baseUrl) {
 
         var ul = $('<ul class="nav navbar-nav"></ul>'),
             li,
             i,
             item;
 
         for (i = 0; i < items.length; i++) {
 
             item = items[i];
             //if (!isVisibleRole(item, role)) {
             //    continue;
             //}
 
             li = $("<li></li>");
 
             if (item.items && item.items.length) {
                 li.append("<a class='dropdown-toggle' data-toggle='dropdown' href='" + (item.url ? baseUrl + item.url : "#") + "'>" + item.display + "<b class='caret'></b></a>");
                 var ddl = $('<ul class="dropdown-menu" role="menu" aria-labelledby="dropdownMenu"></ul>');
                 createItemsElement(ddl, item.items, zIndex, role, baseUrl);
                 li.append(ddl);
                 li.addClass("dropdown");
             }
             else if (item.url) {
                 li.append("<a href='" + baseUrl + item.url + "'>" + item.display + "</a>");
             }
 
             ul.append(li);
         }
 
         return ul;
     };
 
     var createItemsElement = function (ul, items, zIndex, role, baseUrl) {
 
         var li,
             i, len,
             item;
 
         for (i = 0, len = items.length; i < len; i++) {
 
             item = items[i];
             //if (!isVisibleRole(item, role)) {
             //    continue;
             //}
 
             li = $("<li role='presentation'></li>");
             ul.append(li);
 
             if (item.items && item.items.length) {
                 li.addClass("dropdown-header");
                 li.text(item.display);
 
                 createItemsElement(ul, item.items, zIndex, role, baseUrl);
 
                 ul.append("<li role='presentation' class='divider'></li>");
             }
             else if (item.url) {
                 li.append("<a href='" + baseUrl + item.url + "'>" + item.display + "</a>");
             }
         }
 
         return ul;
     };
 
 })(this, jQuery);
