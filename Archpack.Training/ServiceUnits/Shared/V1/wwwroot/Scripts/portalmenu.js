(function (global, $, undef) {

    "use strict";

    var portalmenu = App.define("App.ui.portalmenu", {

        setting: {},
        isShowed: false,
        context: {},
        settings: function (title, setting) {
            portalmenu.settingsObj = {
                title: title,
                setting: setting
            };
        },

        settingsObj: {},

        setup: function (role, container, baseUrl, setting) {

            var base = baseUrl.replace(/\/$/, "");

            portalmenu.setting = setting || { setting: [], title: "" };

            role = App.ifUndefOrNull(role, []);

            var root = createTopElement(portalmenu.setting.setting || [], 1001, role, base);

            $(container).append(root);
        }
    });

    App.define("App.ui.breadcrumb", {
        setup: function (role, container, baseUrl, setting) {
            var baseUrl = baseUrl.replace(/\/$/, "");
            setting = setting || { setting: [], title: "" };
            role = App.ifUndefOrNull(role, []);
            var root = createBreadCrumb(setting.setting || [], role, baseUrl);
            $(container).append(root);
        }
    });

    var createBreadCrumb = function (items, role, baseUrl) {
        var targetPath = App.uri.parse(location.href).path || "",
            i, l, item,
            fragment = document.createDocumentFragment();
        if (baseUrl) {
            if (!App.str.startsWith(baseUrl, "/")) {
                baseUrl += "/" + baseUrl;
            }
            if (App.str.startsWith(targetPath, baseUrl)) {
                targetPath = targetPath.substr(0, baseUrl.length);
            }
        }
        var hierarchy = [];
        for (i = 0, l = items.length; i < l; i++) {
            var item = items[i];
            build(item, targetPath, hierarchy);
        }
        hierarchy.forEach(function (val) {
            var li = document.createElement("li");
            li.appendChild(document.createTextNode(val));
            fragment.appendChild(li);
        });
        return fragment;
    };

    var build = function (source, targetPath, hierarchy) {
        var i, l, items = source.items || [], item;
        for (i = 0, l = items.length; i < l; i++) {
            var item = items[i];
            if (!item) {
                continue;
            }
            if (App.str.startsWith(targetPath, item.path)) {
                if (source.display && i === 0) {
                    hierarchy.push(source.display);
                }
                if (!!item.url) {
                    if (targetPath === (item.path + "/page/" + (item.url + "").split("?")[0])) {
                        hierarchy.push(item.display);
                    }
                } else {
                    build(item, targetPath, hierarchy);
                }
            }
        }
    }

    var iconClasses = {
        open: 'glyphicon glyphicon-chevron-down',
        close: 'glyphicon glyphicon-chevron-left',
        folder: 'glyphicon glyphicon-list-alt'
    }

    var icons = {
        folder: '<i class="' + iconClasses.folder + '"></i>',
        collapse: '<i class="icon-collapse pull-right ' + iconClasses.open + '"></i>'
    };

    var isVisibleRole = function (item, role) {

        var visible = false,
            i, ilen, j, jlen, r;

        if (!item.visible) {
            return true;
        }

        // visible が "*" 以外の文字列で指定されていて、 role と一致しない場合は表示しない
        if (App.isStr(item.visible) && item.visible !== "*" && item.visible !== role) {
            return visible;
        }
            // visible が 配列で role とどれも一致しない場合は表示しない
        else if (App.isArray(item.visible)) {

            for (i = 0, ilen = item.visible.length; i < ilen; i++) {
                for (j = 0, jlen = role.length; j < jlen; j++) {
                    r = role[j];
                    if (item.visible[i] === r) {
                        return true;
                    }
                }
            }

            return false;
        }
            // visible が 関数で戻り値が false の場合は表示しない
        else if (App.isFunc(item.visible)) {
            return item.visible(role);
        }

        return true;
    };

    var createTopElement = function (items, zIndex, role, baseUrl) {
        var container = $('<div class="datalist"><div class="row"></div></div>'),
            row = container.find(".row"),
            colWidth,
            i,
            col, panel, ul, title, li, node, alink,
            item;

        for (i = 0; i < items.length; i++) {
            item = items[i];
            var childPanel = $("<div class='col-sm-3'>" +
                                "<div class='datalist-item' style='overflow: hidden;'>" +
                                    "<span style='white-space:nowrap;'>" + item.display + "</span>" +
                                    "<hr style='margin-top: .5em; margin-bottom: .5em;'/>" +
                                    "<div class='datalist-item-body' style='height:130px; overflow-y:auto; overflow-x:hidden; white-space:nowrap;'></div>" +
                                "</div>" +
                               "</div>");

            var detailChildren = $('<ul class="menu-child"></ul>');
            createItemsElement(detailChildren, item.items, zIndex, role, baseUrl, true);
            childPanel.find(".datalist-item-body").append(detailChildren);
            row.append(childPanel);
        }

        return container;
    }

    var createItemsElement = function (ul, items, zIndex, role, baseUrl, isNode) {

        var li, alink, node,
            i, len,
            item,
            href;

        for (i = 0, len = items.length; i < len; i++) {

            item = items[i];
            if (!isVisibleRole(item, role)) {
                continue;
            }

            href = (!!item.url && !!item.path) ? (baseUrl + item.path + "/page/" + item.url) : "#"

            li = $('<li href="' + href + '"></li>');

            alink = $('<a href="' + href + '" class="menu-item">' + item.display + '</a>');
            if (App.isFunc(item.load)) {
                item.load(alink);
            }

            if (App.isFunc(item.click)) {
                alink.on("click", item.click);
            }
                //} else if (item.url) {
            //    alink.on("click", function () {
            //        document.location.href = $(this).attr("href");
            //        return false;
            //    });
            //}

            node = $("<div></div>");

            if (item.items && item.items.length) {
                node.append(alink).appendTo(li);
                alink.prepend(icons.folder);
                var children = $('<ul class="menu-child"></ul>');
                li.append(children);
                if (isNode) {
                    createItemsElement(children, item.items, zIndex, role, baseUrl, true);
                }
                
            }
            else if (item.url) {
                node.append(alink).appendTo(li);
            }

            ul.append(li);
        }

        return ul;
    };

})(this, jQuery);