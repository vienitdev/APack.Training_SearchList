/*global App jQuery */

/// <reference path="../../ts/core/date.ts" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, $, undef) {

    "use strict";

    $.ajaxSetup({
        cache: false
    });

    if (!Date.prototype.toJSON) {
        //Web API のリクエストに利用する日付型のフォーマット設定
        Date.prototype.toJSON = function () { //eslint-disable-line no-extend-native
            //yyyy-MM-ddTHH:mm:sszzz
            return App.date.format(this, "ISO8601Full");
        };
    }

    /**
     * Ajax リクエストに関する関数を定義します。
     * リクエストを実行する関数はプロミスオブジェクトを戻り値として返却するため、
     * 複数の非同期処理の待ち合わせや順次処理などの実装が可能となります。
     */
    var ajax = App.define("App.ajax"),
        errorMessageHandlers = [],
        setETag = function (method, eTag, header) {
            if (eTag && (method === "put" || method === "delete")) {
                header["If-Match"] = eTag;
            }
        };

    var buildAjaxParameters = function (url, data, type, options) {
        function prepareGetData(target) {
            //dataの中に日付型がある場合は自動的にフォーマット
            return !App.isObj(target) ? target : Object.keys(target).reduce(function (value, key) {
                if (App.isDate(target[key])) {
                    value[key] = target[key].toJSON();
                } else {
                    value[key] = target[key];
                }
                //TODO: jQuery側でencode処理をしてくれない場合はここでエンコードが必要
                //if (App.isStr(value[key])) {
                //    value[key] = encodeURIComponent(value[key]);
                //}
                return value;
            }, {});
        }

        // ajax settings の初期化を行います。
        var settings = {
            url: url,
            type: type,
            data: type === "GET" ? prepareGetData(data) : JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            converters: {},
            headers: {}
        };

        // options に E-TAG の設定がある場合にはヘッダーに e-tag の設定を行います。
        if (options && options.eTag) {
            setETag(type, options.eTag, settings.headers);
            delete options.eTag;
        }
        if (options && options.enableXHTTP) {
            if (type === "PUT") {
                settings.type = "POST";
                settings.headers["x-http-method"] = "MERGE";
            }
            delete options.enableXHTTP;
        }
        if (type === "GET") {
            settings.converters["text json"] = function (item) {
                return JSON.parse(item);
            };
        }
        return $.extend({}, settings, options);
    };

    $.each(["webapi"], function (srvIndex, srvName) {
        var srv = {};
        ajax[srvName] = srv;
        $.each(["get", "put", "post", "delete", "remove"], function (index, item) {
            srv[item] = function (url, data, options) {
                var method = item;
                if (method === "remove") {
                    method = "delete";
                }
                return buildAjaxParameters(url, data, method.toUpperCase(), options);
            };
        });
    });

    var errorHandler = {
        "text/html": function (result) {
            var message = result.responseText.match(/<title\>(.*)<\/title\>/i);
            message = (message && message.length > 1) ? message[1] : undefined;
            return message;

        },
        "application/json": function (result) {
            var ret = JSON.parse(result.responseText),
                message = "raise error";
            if (ret.error && ret.error.message) {
                message = ret.error.message.value;
            } else if (ret["odata.error"] && ret["odata.error"].message) {
                message = ret["odata.error"].message.value;
            } else if (ret.Message) {
                message = ret.Message;
            } else if (ret.message) {
                message = ret.message;
            }
            return message;
        }
    };

    /**
     * Ajax エラーの際のエラーハンドラーを追加します。
     * @param {String} key エラーハンドラーのキー名
     * @param {Function} handler 引数にメッセージ及び Ajax レスポンス、戻り値として message プロパティをもつオブジェクトを返すエラーハンドラー関数
     */
    ajax.addErrorMessageHandler = function (key, handler) {
        var i = 0,
            length = errorMessageHandlers.length,
            item;
        for (; i < length; i++) {
            item = errorMessageHandlers[i];
            if (item.key === key) {
                break;
            }
            item = undef;
        }
        if (item) {
            item.handler = handler;
        } else {
            errorMessageHandlers.push({
                key: key,
                handler: handler
            });
        }
    };

    /**
     * Ajax エラーの際のエラーハンドラーを削除します。
     * @param {String} key エラーハンドラーのキー名
     */
    ajax.removeErrorMessageHandler = function (key) {
        var i = 0, item,
            length = errorMessageHandlers.length,
            index;
        for (; i < length; i++) {
            index = i;
            item = errorMessageHandlers[i];
            if (item.key === key) {
                break;
            }
            item = undef;
        }
        if (item) {
            errorMessageHandlers.splice(index, 1);
        }
    };

    var handleMessage = function (message, response) {
        var i = 0,
            length = errorMessageHandlers.length,
            handler,
            handleResult;

        if (!App.isStr(message)) {
            if (typeof message === "undefined" || message === null) {
                message = "";
            }
            message = message.toString();
        }

        for (; i < length; i++) {
            handler = errorMessageHandlers[i].handler;
            if (handler) {
                handleResult = handler(message, response);
                if (handleResult) {
                    return handleResult;
                }
            }
        }
        return {
            message: message,
            type: "unknown",
            level: "critical"
        };
    };

    ajax.handleError = function (result) {

        var contentType, mime,
            message = "",
            messageHandleResult;

        if (!result.getResponseHeader) {
            return;
        }
        contentType = result.getResponseHeader("content-type");
        mime = contentType ? contentType.split(";")[0] : contentType;

        if (mime && errorHandler[mime]) {
            message = errorHandler[mime](result);
        } else {
            if (result.statusText) {
                message = result.statusText;
            }
        }

        messageHandleResult = handleMessage(message, result);

        return {
            message: messageHandleResult.message,
            rawText: result.responseText,
            status: result.status,
            statusText: result.statusText,
            response: result,
            type: messageHandleResult.type,
            level: messageHandleResult.level
        };
    };

    /**
     * OData システムクエリオプションを生成します。
     * @param {Object} query クエリオブジェクト
     * @returns {String} URL 文字列
     */
    ajax.toODataFormat = function (query) {
        var parameters = [],
            p;

        for (p in query) {
            if (!query.hasOwnProperty(p) || p === "url") {
                continue;
            }
            if (!App.isUndefOrNull(query[p]) && query[p].toString().length > 0) {
                parameters.push("$" + p + "=" + query[p]);
            }
        }

        return query.url + "?" + parameters.join("&");
    };

})(window, jQuery);

/*global App*/
///<reference path="../../ts/core/base.ts" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, App) {

    "use strict";

    App.define("App.data", {
        /**
        * WCF Data Services の OData システムクエリオプションを生成します。
        * @param {Object} query クエリオブジェクト
        * @returns {String} URL 文字列
        */
        toODataFormat: function (query) {
            var logger = new App.logging.Logger("data");
            logger.debug("this API no longer suppurt");
            return App.ajax.toODataFormat(query);
        }
    });

})(this, App);

/*global App jQuery*/

///<reference path="../../ts/core/base.ts" />
///<reference path="../../ts/core/uuid.ts" />

/**
* 変更管理オブジェクトを定義します。
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, App, $) {

    "use strict";

    var EntityState = {
        Unchanged: 2,
        Added: 4,
        Deleted: 8,
        Modified: 16
    };

    function clean(source) {
        var key, removes = [], i, l, remove;

        for (key in source) {
            if (source.hasOwnProperty(key)) {
                if (key.substr(0, 2) === "__") {
                    removes.push(key);
                }
            }
        }
        for (i = 0, l = removes.length; i < l; i++) {
            remove = removes[i];
            source[remove] = undefined;
            delete source[remove];
        }
        return source;
    }

    /**
     * 値をコピーしたオブジェクトを作成します。
     * @param source コピー元のオブジェクト
     * @param removeSpecialProperties 管理用の特殊なプロパティを削除するかどうか
     */
    function copy(source, removeSpecialProperties) {
        var value = $.extend({}, source);
        if (removeSpecialProperties) {
            value = clean(value);
        }
        return value;
    }

    /**
     * 変更セットで追跡されるエンティティの状態を表します。
     */
    var EntityEntry = function (entity) {
        entity.__id = App.uuid();
        this.__id = entity.__id;
        this.original = $.extend({}, entity);
        this.current = entity;
        this.state = EntityState.Unchanged;
    };

    /**
     * 変更追跡を行うオブジェクトの定義です。
     */
    var DataSet = function () {
        this.entries = {};
    };

    /**
     * 変更管理にエンティティをアタッチします。
     * @param {Object, Array} entity 変更管理にアタッチされるエンティティ
     * @return {EntityEntry} 変更されるエンティティ状態オブジェクト
     */
    DataSet.prototype.attach = function (entities) {
        var data = App.isArray(entities) ? entities : [entities],
            i, len, entity, entry;

        for (i = 0, len = data.length; i < len; i++) {
            entity = data[i];
            entry = new EntityEntry(entity);
            this.entries[entry.__id] = entry;
        }
    };

    /**
     * 変更管理からエンティティを取得します。
     * @return {EntityEntry} ID に一致するエンティティ状態オブジェクト
     */
    DataSet.prototype.entry = function (id) {
        var entry = this.entries[id];
        if (!entry) {
            return;
        }
        if (entry.current) {
            return entry.current;
        }
    };

    /**
     * 変更管理にエンティティを追加します。
     * @param {Object} entity 変更管理に追加されるエンティティ
     * @return {EntityEntry} 変更されるエンティティ状態オブジェクト
     */
    DataSet.prototype.add = function (entity) {
        var entry = new EntityEntry(entity);
        entry.state = EntityState.Added;
        this.entries[entry.__id] = entry;

        return entry;
    };

    /**
     * 変更管理のエンティティを変更します。
     * @param {Object} entity 変更管理で変更状態にされるエンティティ
     * @return {EntityEntry} 変更されるエンティティ状態オブジェクト
     */
    DataSet.prototype.update = function (entity, force) {

        if (!entity.__id) {
            throw Error("__id is not contains.");
        }
        var entry = this.entries[entity.__id];
        entry.current = entity;

        switch (entry.state) {
            case EntityState.Unchanged:
                entry.state = EntityState.Modified;
                break;
            case EntityState.Added:
                break;
            case EntityState.Deleted:
                if (!force) {
                    throw new Error("__id[" + entry.__id + "] is already removed.");
                } else {
                    this.reject(entity.__id);
                    this.update(entity);
                }

                break;
            case EntityState.Modified:
                break;
            default:
                break;
        }

        this.entries[entry.__id] = entry;

        return entry;
    };

    /**
     * 変更管理のエンティティを削除します。
     * @param {Object} entity 変更管理で変更状態にされるエンティティ
     * @return {EntityEntry} 変更されるエンティティ状態オブジェクト
     */
    DataSet.prototype.remove = function (entity) {

        if (!entity.__id) {
            throw Error("__id is not contains.");
        }
        var entry = this.entries[entity.__id];

        switch (entry.state) {
            case EntityState.Unchanged:
                entry.state = EntityState.Deleted;
                break;
            case EntityState.Added:
                delete this.entries[entry.__id];
                break;
            case EntityState.Deleted:
                break;
            case EntityState.Modified:
                this.reject(entity.__id);
                entry.state = EntityState.Deleted;
                break;
            default:
        }

        return entry;
    };

    /**
     * 変更セットを取得します。
     * @return {Object} 変更セットオブジェクト
     */
    DataSet.prototype.getChangeSet = function () {

        var changeSet = {
            created: [], updated: [], deleted: []
        };

        var self = this,
            key, entry;
        for (key in self.entries) {
            if (!self.entries.hasOwnProperty(key)) {
                continue;
            }
            entry = self.entries[key];
            switch (entry.state) {
                case EntityState.Unchanged:
                    break;
                case EntityState.Added:
                    changeSet.created.push(copy(entry.current, true));
                    break;
                case EntityState.Deleted:
                    changeSet.deleted.push(copy(entry.current, true));
                    break;
                case EntityState.Modified:
                    changeSet.updated.push(copy(entry.current, true));
                    break;
                default:
            }
        }

        return changeSet;
    };

    /**
     * 変更されているエンティティがあるかどうかを取得します。
     * @return {Boolean} 変更されている場合 True, されていない場合 False
     */
    DataSet.prototype.isChanged = function () {
        var entries = this.entries,
            key,
            entry;

        for (key in entries) {
            if (!entries.hasOwnProperty(key)) {
                continue;
            }
            entry = entries[key];
            if (entry.state !== EntityState.Unchanged) {
                return true;
            }
        }
        return false;
    };

    /**
     * 変更されたエンティティをアタッチされた時点の状態に復元します。
     * @param {String} id アタッチされているエンティティの __id プロパティの値
     * @returns 復元されたエンティティ
     */
    DataSet.prototype.reject = function (id) {

        var self = this,
            entry = self.entries[id];

        if (!entry) {
            return;
        }

        entry.current = $.extend({}, entry.original);
        if (entry.state === EntityState.Added) {
            delete self.entries[id];
            return;
        }

        entry.state = EntityState.Unchanged;
        return entry.current;
    };

    /**
     * 指定された関数の実行結果が truthy になる一番最初のエンティティを返します。
     * @param callback エンティティの値をチェックする関数
     * @param context callback で指定された関数を実行する際のコンテキスト
     * @return 指定された関数の実行結果が truthy になる一番最初のエンティティ
     */
    DataSet.prototype.find = function (callback, context) {
        var key, entry, entries = this.entries;

        if (!App.isFunc(callback)) {
            return;
        }

        for (key in entries) {
            if (!entries.hasOwnProperty(key)) {
                continue;
            }
            entry = entries[key];

            if (callback.call(context || this, entry.current, entry)) {
                return entry.current;
            }
        }
    };
    /**
     * 指定された関数の実行結果が truthy になるすべてのエンティティを配列で返します。
     * @param callback エンティティの値をチェックする関数
     * @param context callback で指定された関数を実行する際のコンテキスト
     * @return 指定された関数の実行結果が truthy になるすべてのエンティティの配列
     */
    DataSet.prototype.findAll = function (callback, context) {
        var key, entry, entries = this.entries, results = [];

        if (!App.isFunc(callback)) {
            return;
        }

        for (key in entries) {
            if (!entries.hasOwnProperty(key)) {
                continue;
            }
            entry = entries[key];

            if (callback.call(context || this, entry.current, entry)) {
                results.push(entry.current);
            }
        }
        return results;
    };

    var page = App.define("App.ui.page");

    /**
     * 変更管理オブジェクトを初期化します。
     * @return {Object} 変更管理オブジェクト
     */
    page.dataSet = function () {
        return new DataSet();
    };

    /**
     * 変更状態の列挙値を返します。
     */
    page.dataSet.status = EntityState;

    page.dataSet.clean = clean;

})(this, App, jQuery);

/* global jQuery App */
// /*
// * Copyright(c) Archway Inc. All rights reserved.
// */

///<reference path="../../ts/core/uuid.ts" />
///<reference path="../../ts/core/uri.ts" />

(function ($) {

    "use strict";

    App.define("App.download", {
        /**
         * 引数で指定されたオプションを使用し、ファイルのダウンロードを行います。
         * @param {Object} options オプション
         * @returns {jqXHR}
         */
        downloadfile: function (options) {

            var win,
                key = App.uuid(),
                url = (App.settings.base.applicationRootUrl || "/") + "Shared/V1/Users/page/Download",
                recieveMessage = function (e) {
                    if (e.data !== key) {
                        return;
                    }
                    if (window.attachEvent) {
                        window.detachEvent("message", recieveMessage);
                    } else {
                        window.removeEventListener("message", recieveMessage);
                    }

                    var targetOrigin = location.protocol + "//" + location.host;
                    if (win && !win.closed && win.postMessage) {
                        win.postMessage(options, targetOrigin);
                    }
                };

            // options が指定されていない場合はデフォルト値を使用します。
            options = $.extend(true, {
                url: window.location.href,
                data: {}
            }, options);

            if (window.attachEvent) {
                window.attachEvent("message", recieveMessage);
            } else {
                window.addEventListener("message", recieveMessage, false);
            }

            url = App.uri.setQuery(url, {
                "dlkey": key
            });

            win = window.open(url, "_blank");
        }

    });

})(jQuery);

/*global App jQuery */

/// <reference path="../core/validation.js" />

(function (global, App, $) {

    "use strict";

    var validatorDataKey = "app-validator-key",
        targetElements = "input, select, textarea";

    /**
    * jQuery 用のバリデーション拡張の実態を定義します。
    */
    function JqValidator(validator, target, options) {
        this._context = {
            validator: validator,
            target: target,
            options: options || {}
        };
    }

    JqValidator.prototype = {
        /**
        * 実際のバリデーションを実行する App.validator のオブジェクトを返します。
        */
        inner: function () {
            return this._context.validator;
        },
        /**
        * バリデーションを実行します。
        */
        validate: function (options) {
            var self, elems, elem, param = {}, elemHolder = {},
                i = 0, l,
                itemName, inner = this.inner(),
                execOptions;

            if (!this._context || !this._context.target) {
                return;
            }
            //引数先頭が文字列の場合は、グループ名が指定されたとみなす
            if (App.isStr(options)) {
                options = {
                    groups: [options]
                };
            }
            //引数が配列の場合は、グループ名が指定されたとみなす
            if (App.isArray(options)) {
                options = {
                    groups: options
                };
            }
            execOptions = $.extend({}, options, {
                //結果に element の値を付加するために beforeReturnResult を設定します。
                beforeReturnResult: function (result) {
                    var idx, len, target;
                    for (idx = 0, len = result.successes.length; idx < len; idx++) {
                        target = result.successes[idx];
                        target.element = elemHolder[target.item];
                    }
                    for (idx = 0, len = result.fails.length; idx < len; idx++) {
                        target = result.fails[idx];
                        target.element = elemHolder[target.item];
                    }
                    return result;
                }
            });

            self = this._context.target;
            elems = (function () {
                var targets = execOptions.targets,
                    jq, idx = 0, len;
                if (targets) {
                    if (App.isArray(targets)) {
                        jq = $();
                        for (len = targets.length; idx < len; idx++) {
                            jq.add(targets[idx]);
                        }
                    } else {
                        jq = $(targets);
                    }
                    return jq;

                }
                return self.find(targetElements);
            })();

            //対象となる要素を探し出してパラメーターを生成します。
            for (l = elems.length; i < l; i++) {
                elem = $(elems[i]);
                itemName = elem.attr("data-prop");
                if (!inner.hasItem(itemName)) {
                    itemName = elem.attr("name");
                }
                if (inner.hasItem(itemName)) {
                    //param[itemName] = elem.val();
                    //input を特別扱いするように変更
                    param[itemName] = elem.hasClass("aw-input") ? elem.input("getValue") : elem.val();

                    elemHolder[itemName] = elem[0];
                }
            }

            execOptions.elements = elemHolder;
            return inner.validate(param, execOptions);
        }
    };

    /**
    * jQuery 用の validation 拡張を定義します。
    */
    $.fn.validation = function (define, options) {
        var self = $(this),
            validator;
        options = options || {};

        if (arguments.length === 0) {
            return self.data(validatorDataKey);
        }

        validator = new JqValidator(App.validation(define, options), self, options);
        self.data(validatorDataKey, validator);

        if (!options.immediate) {
            return validator;
        }

        self.on("change", targetElements, function (e) {
            var target = $(e.currentTarget),
                param = {},
                inner = validator.inner(),
                itemName = target.attr("data-prop");

            if (!itemName) {
                itemName = target.attr("name");
            }

            if (!inner.hasItem(itemName)) {
                return;
            }

            //input を特別扱いするように変更
            param[itemName] = target.hasClass("aw-input") ? target.input("getValue") : target.val();

            inner.validate(param, {
                beforeReturnResult: function (result) {
                    if (result.successes.length) {
                        result.successes[0].element = target[0];
                    }
                    if (result.fails.length) {
                        result.fails[0].element = target[0];
                    }
                    return result;
                }
            });
        });

        return validator;
    };
})(window, App, jQuery);

/*global App jQuery*/
///<reference path="../../ts/core/base.ts" />

/**
* 状態管理マネージャーを定義します。
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, App, $) {

    "use strict";

    var defaultOptions = {
        events: [],
        roles: []
    };

    function changeControlState(stateManager, groups) {
        var i, len, isInRole, control, children;

        $.each(groups, function (index, group) {
            isInRole = false;
            for (i = 0, len = stateManager._options.roles.length; i < len; i++) {
                isInRole = $.inArray(stateManager._options.roles[i], group.roles) > -1;
                if (isInRole) {
                    break;
                }
            }

            if (!isInRole) {
                return;
            }

            $.each(group.rules, function (key, val) {

                control = $("[data-prop='" + key + "']");
                if (control.length == 0) {
                    control = $("#" + key);
                }
                if (control.length == 0) {
                    control = $("." + key);
                }

                children = control.find(":input");

                if (val.disable) {
                    control.removeProp("disabled");
                    if (children.length > 0) {
                        children.prop("disabled", "disabled");
                    } else {
                        control.prop("disabled", "disabled");
                    }
                } else if (val.disable) {
                    control.removeProp("disabled");
                    if (children.length > 0) {
                        children.removeProp("disabled");
                    }
                }

                if (val.readonly) {
                    control.removeProp("disabled");
                    if (children.length > 0) {
                        children.removeProp("disabled");
                        children.prop("readonly", "true");
                    } else {
                        control.prop("readonly", "true");
                    }
                } else if (val.readonly) {
                    control.removeProp("disabled");
                    if (children.length > 0) {
                        children.removeProp("disabled");
                        children.prop("readonly", "false");
                    } else {
                        control.prop("readonly", "false");
                    }
                }

                if (val.visible) {
                    control.removeProp("disabled");
                    if (children.length > 0) {
                        children.removeProp("disabled");
                        children.hide();
                    } else {
                        control.hide();
                    }
                } else if (val.visible) {
                    control.removeProp("disabled");
                    if (children.length > 0) {
                        children.removeProp("disabled");
                        children.show();
                    } else {
                        control.show();
                    }
                }

                if (App.isFunc(val.custom)) {
                    val.custom(control, children);
                }
            });

            if (App.isFunc(group.transfered)) {
                group.transfered();
            }
        });
    }


    /**
     * 状態管理マネージャを初期化します。
     * @param {String} status 初期ステータス
     * @param {Object} options ステータス、権限によるコントロールの制御の設定オブジェクト
     */
    function StateManager(status, statusOptions) {
        this._status = status;
        this._statusEvents = [];
        var options = $.extend({}, defaultOptions, statusOptions);

        var statusid, statusObj, groupid, groupObj;

        if (options.rules) {

            if (!options.rules.base) {
                options.rules.base = {};
            }

            for (statusid in options.rules) {

                if (statusid === "base") {
                    continue;
                }

                statusObj = options.rules[statusid];

                if (!statusObj.base) {
                    statusObj.base = {};
                }

                for (groupid in statusObj) {

                    if (groupid === "base") {
                        continue;
                    }

                    groupObj = statusObj[groupid];
                    statusObj[groupid] = $.extend(true, {}, options.rules.base, statusObj.base, groupObj);
                }
            }
        }

        this._options = options;

        var event, i, len;
        for (i = 0, len = options.events.length; i < len; i++) {
            event = options.events[i];
            this.on(event.from, event.to, event.operation);
        }

        var rules = this._options.rules || {};
        changeControlState(this, rules[status] || {});
    }

    /**
     * 状態遷移時のイベントを登録します。
     * @param {String} from 初期ステータス
     * @param {String} to 完了後ステータス
     * @param {Function} operation 状態遷移時に実行されるイベント処理
     */
    StateManager.prototype.on = function (from, to, operation) {
        this._statusEvents.push({
            from: from,
            to: to,
            operation: App.isFunc(operation) ? operation : function (e) { e.done(); }
        });
    };

    /**
     * 現在のステータスを取得します。
     * @return 現在のステータス
     */
    StateManager.prototype.current = function () {
        return this._status;
    };
    /**
     * 状態遷移処理を実行します。
     * @param {String} state 完了後ステータス
     */
    StateManager.prototype.change = function (toStatus) {
        var self = this,
            event, i, len, trans;

        for (i = 0, len = this._statusEvents.length; i < len; i++) {
            trans = this._statusEvents[i];

            if (trans.from === this._status && trans.to === toStatus) {
                event = trans;
                break;
            }
        }

        // 状態に対応するイベントがなかった際にはエラーをスローする。
        if (App.isUndefOrNull(event)) {
            throw new Error("current state['" + this._status + "'] and to state ['" + toStatus + "'] is not defined.");
        }

        // イベントの結果を done メソッド のフラグで確認する。
        var ev = {
            from: this._status,
            to: toStatus,
            ruleGroup: this._options.rules[toStatus],
            done: function (isDone) {
                if (App.isBool(isDone) && !isDone) {
                    return;
                }

                changeControlState(self, ev.ruleGroup);
                self._status = toStatus;
            }
        };

        event.operation.call(this, ev);
    };

    var page = App.define("App.ui.page");

    /**
     * 状態管理マネージャを初期化します。
     * @param {String} state 初期ステータス
     * @param {Object} stateOptions ステータス、権限によるコントロールの制御の設定オブジェクト
     */
    page.stateManager = function (state, stateOptions) {
        return new StateManager(state, stateOptions);
    };
})(this, App, jQuery);

/*global App*/

///<reference path="../../ts/core/base.ts" />
(function (global, App) {

    "use strict";

    /**
     * 指定されたタイプの DOM ストレージにアクセスするオブジェクトを取得します。
     */
    App.storage = function (type) {
        var storage = type === "session" ? global.sessionStorage :
            type === "local" ? global.localStorage : undefined;
        if (!storage) {
            return;
        }
        return {
			/**
            * 指定されたキーに対応する JSON でシリアライズされた文字列をデシリアライズした値を取得します。
            */
            get: function (key) {
                var value = storage.getItem(key);
                if (!App.isUndef(value)) {
                    return JSON.parse(value);
                }
            },
			/**
            *  指定されたキーで指定された値を JSON でシリアライズして設定します。
            */
            set: function (key, value) {
                value = JSON.stringify(value);
                return storage.setItem(key, value);
            },
			/**
            * 指定されたキーの値を削除します。
            */
            remove: function (key) {
                storage.removeItem(key);
            },
			/**
            * 全ての値を削除します。
            */
            clear: function () {
                storage.clear();
            }
        };
    };
	/**
    * Session DOM ストレージにアクセスするオブジェクトを取得します。
    */
    App.storage.session = function () {
        return App.storage("session");
    };
	/**
    * Local DOM ストレージにアクセスするオブジェクトを取得します。
    */
    App.storage.local = function () {
        return App.storage("local");
    };

})(window || this, App);

/* global jQuery */

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function ($, undef) {

    "use strict";

    var toAbsolutePath = function (rel) {
        if (/^https?:\/\//.test(rel)) {
            return rel;
        } else {
            var current = location.pathname,
                i, l, splited = [];
            current = current.substr(0, current.lastIndexOf("/"));
            current = current.split("/");

            for (i = 0, l = current.length; i < l; i++) {
                if (current[i] !== "") {
                    splited.push(current[i]);
                }
            }
            rel = rel.split("/");
            for (i = 0, l = rel.length; i < l; i++) {
                if (rel[i] === "..") {
                    splited.pop();
                } else if (rel[i] !== ".") {
                    splited.push(rel[i]);
                }
            }
            return location.protocol + "//" + location.host + "/" + splited.join("/");
        }
    };

    // オプションで指定された data から hidden フィールドのタグを生成します。
    var createAndAppendInput = function (data, form) {
        var inputs = [],
            input;
        for (var key in data) {
            if (data.hasOwnProperty(key)) {
                input = $("<input type='hidden' name='" + key + "' value='" + data[key] + "'>");
                form.append(input);
                inputs.push(input);
            }
        }
        return inputs;
    };

    // 引数で指定されたオプションを使用し、ファイルのアップロードを行います。
    $.fn.uploadfile = function (options) {
        var self = this,
            inputs,
            key = (new Date()).getTime().toString(),
            iframeName = "file_upload" + key,
            iframe = $("<iframe name='" + iframeName + "' style='width: 0; height: 0; position:absolute;top:-9999px' />").appendTo("body"),
            form = "<form target='" + iframeName + "' method='post' enctype='multipart/form-data' accept-charset='UTF-8' />",
            deferr = $.Deferred(), //eslint-disable-line new-cap
            targetOrigin;

        // options が指定されていない場合はデフォルト値を使用します。
        options = $.extend(true, {
            url: window.location.href,
            data: {
                __key: key
            }
        }, options);

        targetOrigin = (function () {
            var url = toAbsolutePath(options.url),
                a = $("<a href='" + url + "'></a>")[0],
                result = a.protocol + "//" + a.hostname;
            if ((a.protocol === "http:" && a.port !== "80") || (a.protocol === "https:" && a.port !== "443")) {
                result += ":" + a.port;
            }
            return result;
        })();

        // IFrame 上の form タグに action 属性を追加し、ポストするパラメーターを追加します
        form = self.wrapAll(form).parent().attr("action", options.url);

        // オプションで指定された data を使って IFrame 上の form タグに hidden フィールドを追加します。
        inputs = createAndAppendInput(options.data, form);

        // IFrame 上で FORM タグのサブミットを実行します。
        form.submit(function () {
            var handled = false,
                clienterror = {
                    result: false,
                    message: "アップロードの結果を取得できませんでした。"
                };

            function cleanup() {
                var i = 0,
                    inputLength = inputs.length;
                //作成した input および form と iframe の削除
                for (i = 0; i < inputLength; i++) {
                    inputs[i].remove();
                }
                self.unwrap();
                iframe.remove();
            }

            function receiveMessage(ev) {
                var result;
                if (window.detachEvent) {
                    window.detachEvent("onmessage", receiveMessage);
                } else {
                    window.removeEventListener("message", receiveMessage);
                }
                if (handled) {
                    return;
                }
                handled = true;
                if (ev.origin !== targetOrigin || !ev.data) {
                    deferr.reject(clienterror);
                    return cleanup();
                }
                try {
                    result = (new Function("return " + ev.data.replace(/\r?\n/g, " ") + ";"))(); //eslint-disable-line no-new-func
                } catch (ex) {
                    clienterror.message += ex;
                    deferr.reject(clienterror);
                    return cleanup();
                }

                if (!result || !result.key || result.key !== key) {
                    clienterror.message += "結果がない、もしくはキーが一致しません。";
                    deferr.reject(clienterror);
                    return cleanup();
                }
                cleanup();
                deferr[result.result ? "resolve" : "reject"](result);
            }

            if (window.attachEvent) {
                window.attachEvent("onmessage", receiveMessage);
            } else {
                window.addEventListener("message", receiveMessage, false);
            }


            iframe.load(function () {
                var contents, uploadResult, message, data, success = true, errorType;
                if (handled) {
                    return;
                }
                handled = true;
                try {
                    contents = iframe.contents();
                } catch (e) {
                    clienterror.message += e;
                    deferr.reject(clienterror);
                    return cleanup();
                }

                uploadResult = contents.find(".result").text();
                message = contents.find(".message").text();
                data = contents.find(".data").text();
                errorType = contents.find(".errorType").text();

                cleanup();

                if (!uploadResult) {
                    success = false;
                } else {
                    success = uploadResult.toString().toUpperCase() === "TRUE";
                }
                if (data) {
                    data = JSON.parse(data);
                }
                if (success) {
                    deferr.resolve({
                        result: true,
                        message: message,
                        data: !!data ? data : undef
                    });
                } else {
                    deferr.reject({
                        result: false,
                        message: message,
                        data: !!data ? data : undef,
                        errorType: !!errorType ? errorType : undef
                    });
                }
            });
        });

        setTimeout(function () {
            form.submit();
        }, 500);

        return deferr.promise();
    };
})(jQuery);

/* global jQuery */

/**
 *  コード値検索を行う jQuery Plugin を定義します。
 */
(function ($) {

    "use strict";

    var defaultOptions = {
        //textLength: 1
        /*eslint-disable no-unused-vars*/
        ajax: function (val) {
        },
        success: function (data, element) {
        },
        error: function (error, element) {
        },
        clear: function (element) {
        }
        /*eslint-enable no-unused-vars*/
    };

    /**
     *  テキストボックスに入力された値をもとに、Ajax サービスを利用してコード値による検索を行います。
     */
    $.fn.complete = function (options) {
        var self = $(this);
        options = $.extend({}, defaultOptions, options);

        var lastVal;

        self.on("change", function (e) {

            options.clear(self);

            var $target = $(e.target);
            var val = $target.val();
            if (lastVal !== val) {
                options.ajax(val)
                .then(function (result) {
                    options.success(result, self);
                })
                .fail(function (error) {
                    options.error(error, self);
                });
            } else {
                options.clear(self);
            }

            lastVal = val;
        });
    };
})(jQuery);

/* global jQuery App */
/*
* Copyright(c) Archway Inc. All rights reserved.
*/

/// <reference path="../../ts/core/base.ts" />

(function ($) {

    "use strict";

    $.findP = function (value) {
        return $("[data-prop='" + (value || "") + "']");
    };

    $.fn.findP = function (value) {
        return this.find("[data-prop='" + (value || "") + "']");
    };

    /**
     * @example
     * $(":p(propvalue)")
     */
    $.expr[":"].p = $.expr.createPseudo(function (text) {
        return function (elem) {
            var value = text || "";
            if (!elem.hasAttribute("data-prop")) {
                return false;
            }
            if (value === "") {
                return true;
            }
            return elem.getAttribute("data-prop") === value;
        };
    });
})(jQuery);

//form
(function ($) {

    "use strict";

    /**
     * @param {DOMElement} HTML 要素
     * @param {Object} options オプション
     * @returns {Object} 変換されたオブジェクト。
     */
    function Form(element, options) {
        this.options = $.extend(true, {}, Form.DEFAULTS, options);
        this.element = $(element);
    }

    Form.DEFAULTS = {
        converters: {},
        omitNames: [],
        appliers: {}
    };

    /**
     * HTML 要素からオブジェクトへ変換します。
     */
    Form.prototype.data = function () {
        var self = this,
            $controls = self.element.find("[data-prop]").not(":button"),
            result = {};

        // HTML フォーム内のコントロールごとにオブジェクトへの変換処理を実行します。
        $.each($controls, function () {

            var $control = $(this),
                name = $control.attr("data-prop"),
                i, l, value, key, classes, classNames, className,
                formatAttr;

            if (!name) {
                name = this.name;
            }

            if (!name) {
                return;
            }

            // コントロールが省略（オブジェクトへの変換を除外する）かどうかを判定します。
            // 省略される場合は処理を終了します。
            if (self.options.omitNames && $.isArray(self.options.omitNames)) {
                for (key in self.options.omitNames) {
                    if (self.options.omitNames.hasOwnProperty(key) && self.options.omitNames[key] === name) {
                        return;
                    }
                }
            }

            // コントロールが input タグかどうかを判定し値を取得します。
            value = $control.is(":input") ? $control.val() : $control.text();

            // name 属性をもとに適用されるコンバーターの対象になる場合はコンバーターの変換処理を適用します。
            if (name in self.options.converters) {
                result[name] = self.options.converters[name]($control, value);
                return;
            }

            // class 属性をもとに適用されるコンバーターの対象になる場合はコンバーターの変換処理を適用します。
            classes = $control.attr("class");
            if (classes) {
                classNames = classes.toString().split(" ");
                for (i = 0, l = classNames.length; i < l; i++) {
                    className = classNames[i];
                    if (className in self.options.converters) {
                        result[name] = self.options.converters[className]($control, value);
                        return;
                    }
                }
            }

            //data-format属性の値に夜コンバーターの変換処理を適用します。
            formatAttr = $control.attr("data-role");
            if (formatAttr) {
                result[name] = App.perse.converteByFormatDataAnnotation(formatAttr, $control, value);
                return;
            }

            // コントロールが checkbox, radio の場合にはチェックされていないコントロールの値を NULL に設定します。
            if ($control.is("input:checkbox") || $control.is("input:radio")) {
                if (!this.checked) {
                    if (typeof result[name] === "undefined") {
                        result[name] = null;
                    }
                    return;
                }
            }

            // 値がから文字列の場合には NULL を設定します。
            if (value === "") {
                result[name] = null;
                return;
            }

            // コンバーターによる変換処理が適用されなかった場合には値を設定します。
            result[name] = value;
        });

        // JSON オブジェクト内に日付フォーマット文字列が含まれている場合には日付型に変換します。
        // # AJAX によるサーバーへのデータ送信処理で日付型の認識をさせるため。
        $.each(result, function (index, value) {
            if (typeof value === "string"
                && value.match(/\/Date\((-?\d*)\)\//g)) {
                result[index] = (new Function("return " + value.replace(/\/Date\((-?\d*)\)\//g, "new Date($1)")))(); //eslint-disable-line no-new-func
            }
        });

        return result;
    };

    /**
     * オブジェクトのキーをHTMLにバインドします。
     */
    Form.prototype.bindKey = function (data) {
        var self = this;

        if (data.__id) {
            self.element.attr("data-key", data.__id);
        }
    };

    /**
     * オブジェクトのプロパティを HTML にバインドします。
     */
    Form.prototype.bind = function (data) {
        var self = this,
            $target, target, classes, formatAttr;

        this.bindKey(data);

        // オブジェクトのプロパティごとに HTML フォーム要素への値設定処理を行います。

        var props, name, value, type, tagName,
            i, ilen, j, jlen,
            setText = function (element, text) {
                //IE8
                element.innerText = text;
                element.textContent = text;
            };

        for (i = 0, ilen = self.element.length; i < ilen; i++) {

            props = self.element[i].querySelectorAll("[data-prop]");

            for (j = 0, jlen = props.length; j < jlen; j++) {

                target = props[j];

                name = target.getAttribute("data-prop");
                value = data[name];

                type = target.getAttribute("type");
                type = !type ? "" : type.toLowerCase();

                tagName = target.tagName;
                tagName = !tagName ? "" : tagName.toLowerCase();

                $target = $(target);

                if (name in self.options.appliers) {
                    if (self.options.appliers[name](value, $target, data, name)) {
                        continue;
                    }
                }

                var isClassAppied = false;
                classes = target.getAttribute("class");
                if (classes) {
                    var classNames = classes.toString().split(" ");
                    for (var k = 0; k < classNames.length; k++) {
                        if (classNames[k] in self.options.appliers) {
                            isClassAppied = self.options.appliers[classNames[k]](value, $target, data, name);
                            if (isClassAppied) {
                                break;
                            }
                        }
                    }
                }

                if (isClassAppied) {
                    continue;
                }

                formatAttr = target.getAttribute("data-role");
                if (formatAttr) {
                    if (App.perse.applyByFormatDataAnnotation(formatAttr, value, $target)) {
                        continue;
                    }
                }

                if (typeof value === "undefined" || value === null) {
                    continue;
                }

                // コントロールが select の場合にはコントロールの値を設定します。
                if (tagName === "select") {
                    if ($target.text().indexOf(value) === -1) {
                        $target.val(value);
                        continue;
                    }
                }

                // コントロールが checkbox, radio の場合にはコントロールの値を設定します。
                if (type === "checkbox" || type === "radio") {
                    if (!$.isArray(value)) {
                        $target.val([value]);
                        continue;
                    }
                }

                // コントロールが input タグかどうかを判定し値を設定します。
                if (tagName === "input" || tagName === "select" || tagName === "textarea") {
                    $target.val(value);
                }
                else {
                    if (!App.isUndefOrNull(value)) {
                        setText(target, value);
                    } else {
                        setText(target, "");
                    }
                }
            }
        }

        return self;
    };

    $.fn.form = function (options) {
        return new Form(this, options);
    };
    $.fn.form.Constructor = Form;
})(jQuery);

/*global jQuery App*/

///<reference path="../../ts/core/base.ts" />
///<reference path="../../ts/core/num.ts" />
/// <reference path="form.js" />

//-----------------------------------
//キー制御補助
//-----------------------------------
(function ($) {
    "use strict";

    App.ui = App.ui || {};
    App.ui.util = App.ui.util || {};
    App.ui.util.inputHelper = function (element, options) {

        var el = $(element),
            opts = options || {},
            result = {};

        function blur() {
            if (App.isFunc(result.blur)) {
                result.blur();
            }
        }

        function keypress(e) {
            var key = e.which,
                char;

            if (!(opts.allowRegx instanceof RegExp)) {
                return true;
            }

            if (e.ctrlKey || e.altKey || e.metaKey || key < 32) {
                return true;
            }

            char = String.fromCharCode(key);
            if (opts.allowRegx.test(char)) {
                return true;
            }
            return false;
        }

        //el.on("focus", focus);
        //el.on("keydown", keydown);
        el.on("keypress", keypress);
        el.on("blur", blur);
        //el.on("paste", paste);

        return result;
    };

})(jQuery);

//-----------------------------------
//input コントロール本体
//-----------------------------------
(function ($) {
    "use strict";

    var noop = function () { },
        controlHolder = {};

    function Input(element, options) {
        this._element = $(element);
        this._options = options || {};
        //bind や form 時の判別などに利用予定
        this._element.addClass("aw-input");

        if (this._options.type) {
            this._element.addClass("aw-input-" + this._options.type);
        }

        this._control = (controlHolder[this._options.type] || function () {
            return {
                getValue: noop,
                setValue: noop,
                getText: noop
            };
        })(this._element, this._options, this);
    }

    Input.prototype.setValue = function setValue(value) {
        this._control.setValue(value);
        return;
    };

    Input.prototype.getValue = function getValue() {
        var res = this._control.getValue();
        if (App.isUndef(res)) {
            return Input.nullValue;
        } else {
            return res;
        }
    };

    Input.prototype.getText = function getText() {
        return this._control.getText();
    };

    Input.nullValue = {};

    $.fn.input = function (methodOrOptions) {
        var args = arguments,
            firstResult,
            chainResult = this.each(function () {
                if (typeof firstResult !== "undefined") {
                    return;
                }
                var target = $(this),
                    data = target.data("aw.input");

                if (!data) {
                    target.data("aw.input", (data = new Input(target, methodOrOptions)));
                } else if (typeof methodOrOptions === "string") {
                    firstResult = data[methodOrOptions].apply(data, Array.prototype.slice.call(args, 1));
                }
            });
        return firstResult === Input.nullValue ? (void 0) :
            App.isUndefOrNull(firstResult) ? chainResult : firstResult;
    };

    $.fn.input.Constructor = Input;

    $.fn.input.addControl = function (name, initializer) {
        if (!App.isStr(name) || !App.isFunc(initializer)) {
            return;
        }
        controlHolder[name] = initializer;
    };
    //Form 処理の登録
    if ($.fn.form && $.fn.form.Constructor && $.fn.form.Constructor.DEFAULTS) {
        var formDefaults = $.fn.form.Constructor.DEFAULTS;
        if (formDefaults.converters) {
            formDefaults.converters["aw-input"] = function (element) {
                return element.input("getValue");
            };
        }
        if (formDefaults.appliers) {
            formDefaults.appliers["aw-input"] = function (value, element) {
                var target = value;
                element.input("setValue", target);
                return true;
            };
        }
    }
})(jQuery);

//-----------------------------------
//NumberInput
//-----------------------------------
(function ($) {
    "use strict";

    //入力されているテキストを確認して値を設定します。
    function checkValue() {
        var text = this._element.val(),
            commalessText, splitNums, i;
        if (this._lastText === text) {
            return;
        }
        this._lastText = text;
        this._value = undefined;
        //カンマを削除して、数値として正しい文字列か確認。
        //commalessText = text.replace(/,/g, "");
        if (!/^(-?)(([0-9\,]+)(\.[0-9]+)?)$/.test(text)) {
            return;
        }
        if (text.indexOf(",") > -1) {

            splitNums = text.replace(/-/g, "").split(".")[0].replace(/^0*/g, "").split(",");
            //末尾からループ
            for (i = splitNums.length; !!i; i--) {
                //先頭だったら空もしくは3文字より多い場合は数値でないと判断
                if (i < 2) {
                    if (splitNums[i - 1].length < 1 || splitNums[i - 1].length > 3) {
                        return;
                    }
                } else {
                    //先頭じゃなかったら3文字以外は数値でないと判断
                    if (splitNums[i - 1].length !== 3) {
                        return;
                    }
                }
            }
        }
        //カンマを削除して、パースできる文字列に変更。
        commalessText = text.replace(/,/g, "");
        //number型に変換
        this._value = parseFloat(commalessText);
        //例えばフォーマットが#.00と指定されていて、入力された文字列が123.123だった場合に
        //表示は123.12に変わるが、以下の処理がないと値は123.123のまま保持してしまうため
        //一旦フォーマットしたものを再度パースして桁合わせを行う
        this._value = App.num.parse(App.num.format(this._value, this._options.format), this._options.format);
    }

    //値をもとにテキストを設定します。
    function setFormattedText() {
        var applyText = App.num.format(this._value, this._options.format) || "";
        this._element.val(applyText);
        this._lastText = applyText;
    }

    function initialize() {
        var helper = App.ui.util.inputHelper(this._element, {
            allowRegx: (function () {
                var pattern = "0-9\\-",
                    format = this._options.format + "";
                if (format.indexOf(".") > -1) {
                    pattern += "\\.";
                }
                if (format.indexOf(",") > -1) {
                    pattern += "\\,";
                }
                return new RegExp("[" + pattern + "]");
            }).bind(this)()
            //this._options.format.indexOf(".") > -1 ? /[0-9\.,\-]/ : /[0-9,\-]/
        });
        //フォーカスアウト時に表示のテキストをフォーマットします。
        helper.blur = function () {
            var oldValue = this._value;

            checkValue.apply(this);
            setFormattedText.apply(this);
            //作成された新しい値と、古い値が違う場合は change イベントを発行します。
            if (oldValue !== this._value) {
                this._element.change();
            }

        }.bind(this);

        checkValue.apply(this);
    }

    function NumberInput(element, options, parent) {
        this._options = $.extend({}, {
            format: "#,#"
        }, options || {});
        this._element = $(element);
        this._parent = parent;

        initialize.apply(this);
    }
    //必須プロパティ
    //入力されているテキストをもとに型変換をした値を返します。
    //型変換できない場合はテキストが入力されていても undefined を返します。
    //テキストが空の場合は、undefined を返します。
    //型変換可能だった場合は値を返します。
    NumberInput.prototype.getValue = function getValue() {
        checkValue.apply(this);
        return this._value;
    };
    //必須プロパティ
    //指定された値を設定します。
    //指定された値は必ず対象の型かどうか、または対象の型に変換可能かをチェックします。
    //上記でない場合は値を undefined にセットして、表示テキストを空にします。
    NumberInput.prototype.setValue = function setValue(value) {
        var commalessText,
            splitTexts;

        this._value = undefined;
        if (App.isNum(value)) {
            if (!isNaN(value) && isFinite(value)) {
                this._value = value;
            }
        } else if (App.isStr(value)) {
            //小数点区切りで分ける
            splitTexts = value.split(".");
            //先頭を整数部とみなしてカンマを削除する
            //小数部とみなせる部分のカンマを削除して正となる動作は許容しすぎと判断
            splitTexts[0] = splitTexts[0].replace(/,/g, "");
            commalessText = splitTexts.join("");

            if (App.isNumeric(commalessText)) {
                this._value = parseFloat(commalessText);
            }
        }
        if (App.isNum(this._value)) {
            this._value = App.num.parse(App.num.format(this._value, this._options.format), this._options.format);
        }
        setFormattedText.apply(this);
        return;
    };
    //必須プロパティ
    //画面に表示されている文字列をそのまま返します。
    NumberInput.prototype.getText = function getText() {
        return this._element.val();
    };

    //input を有効化するときのオプションの type の値を追加します。
    $.fn.input.addControl("number", function (element, options, parent) {
        return new NumberInput(element, options, parent);
    });

})(jQuery);

//-----------------------------------
//ZipcodeInput
//-----------------------------------
(function ($) {
    "use strict";

    function convertValue(text) {
        var hyphenlessText;

        if (!App.isStr(text)) {
            return;
        }
        //7桁または8桁（ハイフンを含む場合）の郵便番号として正しい文字列か確認。
        if (!/^(([0-9]{3})([-]?)([0-9]{4}))$/.test(text)) {
            return;
        }
        // 123-4567 の場合は、1234567
        hyphenlessText = text.replace(/-/g, "");
        return hyphenlessText.slice(0, 3) + "-" + hyphenlessText.slice(3, 7);
    }

    //入力されているテキストを確認して値を設定します。
    function checkValue() {
        var text = this._element.val();
        if (this._lastText === text) {
            return;
        }
        this._lastText = text;
        this._value = undefined;
        //郵便番号フォーマット 000-0000 に変換する
        this._value = convertValue(text || "");
    }

    //値をもとにテキストを設定します。
    function setFormattedText() {
        var applyText = this._value || "";
        this._element.val(applyText);
        this._lastText = applyText;
    }

    function initialize() {
        var helper = App.ui.util.inputHelper(this._element, {
            allowRegx: /[0-9\-]/
        });
        //フォーカスアウト時に表示のテキストをフォーマットします。
        helper.blur = function () {
            var oldValue = this._value;

            checkValue.apply(this);
            setFormattedText.apply(this);
            //作成された新しい値と、古い値が違う場合は change イベントを発行します。
            if (oldValue !== this._value) {
                this._element.change();
            }

        }.bind(this);

        checkValue.apply(this);
    }

    function ZipcodeInput(element, options, parent) {
        this._options = $.extend({}, {
        }, options || {});
        this._element = $(element);
        this._parent = parent;

        initialize.apply(this);
    }
    //必須プロパティ
    //入力されているテキストをもとに型変換をした値を返します。
    //型変換できない場合はテキストが入力されていても undefined を返します。
    //テキストが空の場合は、undefined を返します。
    //型変換可能だった場合は値を返します。
    ZipcodeInput.prototype.getValue = function getValue() {
        checkValue.apply(this);
        return this._value;
    };
    //必須プロパティ
    //指定された値を設定します。
    //指定された値は必ず対象の型かどうか、または対象の型に変換可能かをチェックします。
    //上記でない場合は値を undefined にセットして、表示テキストを空にします。
    ZipcodeInput.prototype.setValue = function setValue(value) {
        this._value = convertValue(value);
        setFormattedText.apply(this);
        return;
    };
    //必須プロパティ
    //画面に表示されている文字列をそのまま返します。
    ZipcodeInput.prototype.getText = function getText() {
        return this._element.val();
    };

    //input を有効化するときのオプションの type の値を追加します。
    $.fn.input.addControl("zipcode", function (element, options, parent) {
        return new ZipcodeInput(element, options, parent);
    });

})(jQuery);

//-----------------------------------
//TimeInput
//-----------------------------------
(function ($) {
    "use strict";

    function getHourMinutes(text) {
        var splitTime, hour, minute;

        if (!text || !App.isStr(text)) {
            return;
        }

        if (!/^(([0-9]{0,2})([:]?)([0-9]{0,2})?)$/.test(text)) {
            return;
        }
        if (text.indexOf(":") > -1) {

            splitTime = text.split(":");
            //時間を前ゼロ埋めにして2桁にする
            //09 の場合は、09
            // 9 の場合は、09
            //空白 の場合は、00
            //分を前ゼロ埋めにして2桁にする
            //09 の場合は、09
            // 9 の場合は、09
            //空白 の場合は、00
            text = ("00" + splitTime[0]).slice(-2) + ("00" + splitTime[1]).slice(-2);
        } else {
            //ハイフンなしで3桁の場合は前0埋めにして4桁にする
            //011 の場合は、0011
            if (text.length === 3) {
                text = "0" + text;
            } else if (text.length <= 2) {
                //ハイフンなしで1または2桁の場合は前1桁0埋めかつ後ろ2桁0埋めにして4桁にする
                // 9 の場合は、0900
                //12 の場合は、1200
                text = ("0" + text + "00").slice(-4);
            }
        }
        // 09:10 の場合は、0910
        //  0910 の場合は、0910
        //colonlessText = text.replace(/:/g, "");
        hour = parseInt(text.slice(0, 2), 10);
        minute = parseInt(text.slice(2, 4), 10);
        //時間が0～23でない場合は時間ではないと判断
        if (hour < 0 || hour > 23) {
            return;
        }
        //分が0～59でない場合は時間ではないと判断
        if (minute < 0 || minute > 59) {
            return;
        }
        return [hour, minute];
    }

    //入力されているテキストを確認して値を設定します。
    function checkValue() {
        var text = this._element.val(),
            hourMinite, currentValue, tempDate;
        if (this._lastText === text) {
            return;
        }
        this._lastText = text;
        currentValue = this._value;
        this._value = undefined;

        hourMinite = getHourMinutes(text);
        if (!hourMinite) {
            return;
        }

        //元の値がDate型の場合は、年月日はそのままに時刻を置き換える
        if (App.isDate(currentValue)) {
            tempDate = currentValue;
        } else {
            tempDate = new Date();
        }

        this._value = new Date(
            tempDate.getFullYear(),
            tempDate.getMonth(),
            tempDate.getDate(),
            hourMinite[0],
            hourMinite[1],
            0);
    }

    //値をもとにテキストを設定します。
    function setFormattedText() {
        var applyText = App.date.format(this._value, "HH:mm") || "";
        this._element.val(applyText);
        this._lastText = applyText;
    }

    function initialize() {
        var helper = App.ui.util.inputHelper(this._element, {
            allowRegx: /[0-9:]/
        });
        //フォーカスアウト時に表示のテキストをフォーマットします。
        helper.blur = function () {
            var oldValue = this._value;

            checkValue.apply(this);
            setFormattedText.apply(this);
            //作成された新しい値と、古い値が違う場合は change イベントを発行します。
            if (oldValue !== this._value) {
                this._element.change();
            }

        }.bind(this);

        checkValue.apply(this);
    }

    function TimeInput(element, options, parent) {
        this._options = $.extend({}, {
            //default
        }, options || {});
        this._element = $(element);
        this._parent = parent;

        initialize.apply(this);
    }
    //必須プロパティ
    //入力されているテキストをもとに型変換をした値を返します。
    //型変換できない場合はテキストが入力されていても undefined を返します。
    //テキストが空の場合は、undefined を返します。
    //型変換可能だった場合は値を返します。
    TimeInput.prototype.getValue = function getValue() {
        checkValue.apply(this);
        return this._value;
    };
    //必須プロパティ
    //指定された値を設定します。
    //指定された値は必ず対象の型かどうか、または対象の型に変換可能かをチェックします。
    //上記でない場合は値を undefined にセットして、表示テキストを空にします。
    TimeInput.prototype.setValue = function setValue(value) {
        var hourMinutes, i, l, result, defaultFormat,
            //入力可能なフォーマット
            defaultParseFormats = [
                "JsonDate",
                "yyyy/M/d H:m",
                "yyyy/M/d H:m:s",
                "yyyyMd H:m:s",
                "yyyyMd H:m",
                "H:m:s"
            ];

        this._value = undefined;
        if (App.isValidDate(value) && App.isDate(value)) {
            this._value = value;
        } else if (App.isStr(value)) {
            hourMinutes = getHourMinutes(value);
            if (hourMinutes) {
                this._value = new Date();
                this._value.setHours(hourMinutes[0]);
                this._value.setMinutes(hourMinutes[1]);
            } else {
                //デフォルトのフォーマット一覧で順番にパース
                for (i = 0, l = defaultParseFormats.length; i < l; i++) {
                    defaultFormat = defaultParseFormats[i];
                    result = App.date.parse(value, defaultFormat, undefined);
                    if (result) {
                        this._value = result;
                    }
                }
                result = new Date(value);
                if(App.isValidDate(result)){
                    this._value = result;
                }
            }
        }
        setFormattedText.apply(this);
        return;
    };
    //必須プロパティ
    //画面に表示されている文字列をそのまま返します。
    TimeInput.prototype.getText = function getText() {
        return this._element.val();
    };

    //input を有効化するときのオプションの type の値を追加します。
    $.fn.input.addControl("time", function (element, options, parent) {
        return new TimeInput(element, options, parent);
    });

})(jQuery);

/*global jQuery App*/

//-----------------------------------
//Datepicker
//bootstrap-datepickerの拡張
//-----------------------------------

/// <reference path="input.js" />
/// <reference path="../../ts/core/date.ts" />

(function ($) {
    "use strict";

    //標準の実装を保持
    /*eslint-disable no-unused-vars */
    var oldParseDate = $.fn.datepicker.DPGlobal.parseDate,
        oldFormatDate = $.fn.datepicker.DPGlobal.formatDate,
        oldFill = $.fn.datepicker.Constructor.prototype.fill,
        oldClick = $.fn.datepicker.Constructor.prototype.click,
        oldKeyDown = $.fn.datepicker.Constructor.prototype.keydown,
        oldBuildEvents = $.fn.datepicker.Constructor.prototype._buildEvents,
        oldHide = $.fn.datepicker.Constructor.prototype.hide,
        oldSetValue = $.fn.datepicker.Constructor.prototype.setValue,
        oldSetDate = $.fn.datepicker.Constructor.prototype._setDate,
        /*eslint-enable no-unused-vars */
        //入力可能なフォーマット
        defaultParseFormats = [
            "yyyy/M/d",
            "yyyyMd",
            "yyyy/M",
            "yyyy/M/",
            "yy/M/d",
            "yy/M",
            "yy/M/",
            "yyyyM",
            "JsonDate"
        ];
    //日本語表示用定義の設定
    $.fn.datepicker.dates.en.days = App.culture.current().dateTimeFormat.weekdays.names.concat();
    $.fn.datepicker.dates.en.daysShort = App.culture.current().dateTimeFormat.weekdays.shortNames.concat();
    $.fn.datepicker.dates.en.daysMin = App.culture.current().dateTimeFormat.weekdays.shortNames.concat();
    $.fn.datepicker.dates.en.months = App.culture.current().dateTimeFormat.months.names.concat();
    $.fn.datepicker.dates.en.monthsShort = App.culture.current().dateTimeFormat.months.names.concat();
    $.fn.datepicker.dates.en.today = "今日";
    $.fn.datepicker.dates.en.clear = "クリア";

    //_toggle_multidate の日付押下した時の利用用
    function untoggleMultidate(date) {
        var ix = this.dates.contains(date);
        if (!date) {
            this.dates.clear();
        } else {
            if (this.o.multidate === false) {
                this.dates.clear();
                this.dates.push(date);
            } else if (ix === -1) {
                this.dates.push(date);
            }
        }
        if (typeof this.o.multidate === "number") {
            while (this.dates.length > this.o.multidate) {
                this.dates.remove(0);
            }
        }
    }

    function UTCDate() {
        return new Date(Date.UTC.apply(Date, arguments));
    }

    function parseDate(date, format, toUtc) {
        //bootstrap-datepickerの実装上,
        //UTCの値でローカル日時を作成する必要がある
        var toDummyUtc = function (target) {
            if (!toUtc) {
                return target;
            }
            return new Date(Date.UTC(target.getFullYear(),
                target.getMonth(),
                target.getDate(),
                target.getHours(),
                target.getMinutes(),
                target.getSeconds(),
                target.getMilliseconds()));
        };
        //値が日付なら処理なし
        if (App.isDate(date)) {
            if (App.isValidDate(date)) {
                return date;
            }
            return;
        }
        //値がfalsy(多くの場合は空文字)なら処理なし
        if (!date) {
            return;
        }
        var jCal = App.calendar.japaneseCalendar(),
            //表示フォーマットでパース
            result = App.date.parse(date, format, format.indexOf("g") > -1 ? jCal : undefined),
            i, l, defaultFormat;

        if (result) {
            return toDummyUtc(result);
        }
        //デフォルトのフォーマット一覧で順番にパース
        for (i = 0, l = defaultParseFormats.length; i < l; i++) {
            defaultFormat = defaultParseFormats[i];
            result = App.date.parse(date, defaultFormat, defaultFormat.indexOf("g") > -1 ? jCal : undefined);
            if (result) {
                return toDummyUtc(result);
            }
        }
        result = new Date(date);
        if(App.isValidDate(result)){
            return toDummyUtc(result);
        }
        return;
    }

    //入力時に実行される文字列から日付への変換処理
    $.fn.datepicker.DPGlobal.parseDate = function (date, format, language) { //eslint-disable-line no-unused-vars
        return parseDate(date, format, true);
    };
    //表示時に内部で保持している日付から文字列への変換処理
    $.fn.datepicker.DPGlobal.formatDate = function (date, format, language) { //eslint-disable-line no-unused-vars
        var cal;
        if (format.indexOf("g") > -1) {
            cal = App.calendar.japaneseCalendar();
        }
        //bootstrap-datepickerの実装上,
        //UTCの値でローカル日時を作成する必要がある
        if (App.isValidDate(date)) {
            date = App.date.addMinutes(date, date.getTimezoneOffset());
        }

        return App.date.format(date, format, cal);
    };

    $.fn.datepicker.Constructor.prototype.setValue = function () {
        var formattedValue = this.getFormattedDate(),
            lastFormattedValue = App.isUndefOrNull(this._lastFormattedValue) ? "" : this._lastFormattedValue,
            currentText = this.element.val() || "";
        if (currentText !== lastFormattedValue) {
            lastFormattedValue = currentText;
        }

        if (lastFormattedValue !== (App.isUndefOrNull(formattedValue) ? "" : formattedValue)) {
            oldSetValue.apply(this);

        }
        this._lastFormattedValue = formattedValue;
    };

    $.fn.datepicker.Constructor.prototype._setDate = function (date, which) {
        if (!which || which === "date") {
            this._toggle_multidate(date && new Date(date));
        }
        if (which === "day_awinput") {
            untoggleMultidate.apply(this, [date]);
        }

        if (!which || which === "view") {
            this.viewDate = date && new Date(date);
        }

        this.fill();
        this.setValue();
    };

    //日付適用時などのポップアップの表示書き換え処理
    $.fn.datepicker.Constructor.prototype.fill = function () {
        //標準の実装を最初に実行
        var result = oldFill.apply(this, arguments),
            opts = this._o,
            year = this.viewDate.getFullYear(),
            month = this.viewDate.getMonth(),
            cal = App.calendar.japaneseCalendar();
        //useJapaneseCalendarオプションが指定されている場合は、表示を和暦にする
        if (opts.useJapaneseCalendar) {
            this.picker.find(".datepicker-days thead th.datepicker-switch")
                .text(App.date.format(this.viewDate, "gggyy年MM月", cal) || App.date.format(this.viewDate, "yyyy年MM月"));
            this.picker.find(".datepicker-months thead th.datepicker-switch")
                .text(App.date.format(new Date(year, 0, 1), "gggyy年", cal) || year + "年");
            this.picker.find(".datepicker-years thead th.datepicker-switch")
                .text(
                (App.date.format(
                    new Date((year / 10 ^ 0) * 10, 0, 1), "gggyy年", cal)
                    || ((year / 10 ^ 0) * 10) + "年") + " - " +
                (App.date.format(
                    new Date((year / 10 ^ 0) * 10 + 9, 0, 1), "gggyy年", cal)
                    || (((year / 10 ^ 0) * 10) + 9) + "年"));
        } else {
            //useJapaneseCalendarオプションが指定されていないくて、"年"や"月"の表示を行う
            this.picker.find(".datepicker-days thead th.datepicker-switch")
                .text(year + "年" + (month + 1) + "月");
            this.picker.find(".datepicker-months thead th.datepicker-switch")
                .text(year + "年");
            this.picker.find(".datepicker-years thead th.datepicker-switch")
                .text(((year / 10 ^ 0) * 10) + "年" + " - " +
                (((year / 10 ^ 0) * 10) + 9) + "年");
        }
        //年一覧の表示時のテキスト置き換え
        //和暦 or "年" 表示
        this.picker.find(".datepicker-years tbody .year").each(function (index, elem) {
            var el = $(elem),
                yearVal = parseInt(el.text(), 10),
                eraDef;
            //表示する年の設定
            el.attr("data-year", yearVal);

            if (opts.useJapaneseCalendar) {
                eraDef = cal.getEraInfo(new Date(yearVal, 0, 1));
                if (eraDef) {
                    el.text(eraDef.shortEra + "." + eraDef.year);
                }
            } else {
                el.text(yearVal + "年");
            }

        });
        return result;
    };
    //ポップアップの選択時処理
    $.fn.datepicker.Constructor.prototype.click = function (e) {
        e.preventDefault();
        //年一覧の表示時の年の選択
        var target = $(e.target).closest("span, td, th"),
            year, month, day, tag;

        if (target.length === 1 && !target.is(".disabled")) {
            tag = target[0].nodeName.toLowerCase();
            if (tag === "span" && !target.is(".month")) {

                day = 1;
                month = 0;
                //選択された要素のdata-yearから対象の年を取得
                //標準では要素のテキストから値を取得している
                year = parseInt(target.attr("data-year"), 10) || 0;
                this.viewDate.setUTCFullYear(year);
                this._trigger("changeYear", this.viewDate);
                if (this.o.minViewMode === 2) {
                    this._setDate(UTCDate(year, month, day)); //eslint-disable-line new-cap
                }

                this.showMode(-1);
                this.fill();

                if (this.picker.is(":visible") && this._focused_from) {
                    $(this._focused_from).focus();
                }
                delete this._focused_from;
                return;
            }
            if (tag === "td" && target.is(".day")) {
                day = parseInt(target.text(), 10) || 1;
                year = this.viewDate.getUTCFullYear();
                month = this.viewDate.getUTCMonth();
                if (target.is(".old")) {
                    if (month === 0) {
                        month = 11;
                        year -= 1;
                    }
                    else {
                        month -= 1;
                    }
                }
                else if (target.is(".new")) {
                    if (month === 11) {
                        month = 0;
                        year += 1;
                    }
                    else {
                        month += 1;
                    }
                }
                this._setDate(UTCDate(year, month, day), "day_awinput"); //eslint-disable-line new-cap
                this.hide(true);
                return;
            }
        }
        oldClick.apply(this, arguments);
    };
    //標準の挙動ではキーはカレンダーの移動に割り当てられている
    //今回の要件や利用方法はキー入力に重点をおいており
    //カレンダーは最低限の入力補助であり、主ではないため
    //テキストボックス操作の挙動(テキストボックス標準の挙動)にする
    $.fn.datepicker.Constructor.prototype.keydown = function (e) {
        switch (e.keyCode) {
            case 27: // escape
                if (this.picker.is(":visible")) {
                    this.hide(true);
                }

                //TODO: 本来は上記のif文内に含めたい
                //テキストボックスデフォルトのescキー押下時に元の値にもどる挙動を抑制してしまう
                //但し、抑制せずに元の値に戻す挙動がされた場合に内部で管理されている値と差異が出てしまうため
                //元の値に戻ったことをフックする手段が必要。フックする手段は要調査。
                //フックすする手段が不明な場合は、preventDefaultはこの位置がベター
                e.preventDefault();
                break;
            case 9: // tab
                //pickerが表示されていない場合に、値設定が設定されない為一旦表示する
                if (!this.picker.is(":visible")) {
                    this.show();
                }
                this.hide();
                break;
            case 13: //enter
                if (!this.picker.is(":visible")) {
                    this.show();
                } else {
                    this.hide(true);
                }
                e.preventDefault();
        }
    };

    $.fn.datepicker.Constructor.prototype.hide = function (noSet) {
        if (this.isInline) {
            return;
        }
        if (!this.picker.is(":visible")) {
            return;
        }
        this.focusDate = null;
        this.picker.hide().detach();
        this._detachSecondaryEvents();
        this.viewMode = this.o.startView;
        this.showMode();

        if (this.o.forceParse &&
            (this.isInput && this.element.val() || this.hasInput && this.element.find("input").val())) {
            if (!noSet) {
                this.setValue();
            }

        }
        this._trigger("hide");
    };

    //和暦での日本語入力も可になるため、入力制限は無し
    //$.fn.datepicker.Constructor.prototype._buildEvents = function _buildEvents(value) {
    //    var result = oldFill.apply(this, arguments);

    //    if (this.isInput) { // single input
    //        this.element.on("keypress", function (e) {
    //            var key = e.which,
    //                char;

    //            if (e.ctrlKey || e.altKey || e.metaKey || key < 32) {
    //                return true;
    //            }

    //            char = String.fromCharCode(key);
    //            if (/[0-9a-zA-Z\/]/.test(char)) {
    //                return true;
    //            }
    //            return false;
    //        });
    //    }
    //};

    function initialize() {
        var self = this;
        var applyDatePicker = function () {
            //self._options.todayHighlight = true;
            self._element.datepicker(self._options);
            self._element.off("focus", applyDatePicker);
            self._element.datepicker("show");
            self.isInitialized = true;
        };

        this._element.on("focus", applyDatePicker);
    }

    function DatePickerForBs(element, options, parent) {
        this._options = $.extend({}, {
            format: "yyyy/MM/dd",
            keyboardNavigation: false,
            todayHighlight: true,
            multidate: false
        }, options || {});
        this._element = $(element);
        this._parent = parent;

        initialize.apply(this);
    }

    DatePickerForBs.prototype.getValue = function getValue() {
        var value, inputText, originVal;

        if (this.isInitialized) {
            inputText = this._element.val();
            originVal = this._element.datepicker("getDate");

            value = parseDate(inputText, this._options.format);

            if (App.isUndefOrNull(value) || originVal.getTime() !== value.getTime()) {
                this._element.datepicker("setDate", value);
            }
        }
        else {
            value = $.fn.datepicker.DPGlobal.parseDate(this._element.val(), this._options.format);
        }

        if (App.isValidDate(value)) {
            return value;
        }

        return;
    };

    DatePickerForBs.prototype.setValue = function setValue(value) {
        var val = parseDate(value, this._options.format);

        if (this.isInitialized) {
            return this._element.datepicker("setDate", val);
        } else {
            if (App.isValidDate(val)) {
                val = App.date.addMinutes(val, -val.getTimezoneOffset());
            }
            val = $.fn.datepicker.DPGlobal.formatDate(val, this._options.format);
            this._element.val(val || "");
        }
    };

    DatePickerForBs.prototype.getText = function getText() {
        return this._element.val();
    };


    //input を有効化するときのオプションの type の値を追加します。
    $.fn.input.addControl("date", function (element, options, parent) {
        return new DatePickerForBs(element, options, parent);
    });
})(jQuery);

/*global jQuery App*/
///<reference path="../../ts/core/base.ts" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function ($) {

    "use strict";

    /**
     * データ取得など長時間にわたって行われる処理の間に表示するローディングを定義します。
     */
    var loading = App.define("App.ui.loading");

    /**
     * ローディングを表示します。
     * @param {String} message 表示するメッセージ
     * @param {String} target 表示する要素。指定されていない場合は body 要素が利用されます。
     */
    loading.show = function (message, target) {

        var place = !!target ? $(target) : $(window.document.body),
            element = place.children(".loading"),
            container, overlay, info,
            position = place.css("position");

        message = App.isUnusable(message) ? "" : message;

        if (element.length > 0) {
            element.find(".loading-message").text(message);
            return;
        }

        if (position) {
            place.css("position", "relative");
        }
        container = $("<div class='loading' style='position:" + (!!target ? "absolute" : "fixed") + ";'></div>");
        overlay = $("<div class='loading-overlay'></div>");
        info = $("<div class='loading-holder'><div class='loading-image'></div><div class='loading-message' unselectable='on'>" + message + "</div></div>");
        container.append(overlay);
        container.append(info);
        place.append(container);
        info.css("margin-top", -info.outerHeight() / 2);
    };

    /**
    * ローディングのメッセージを設定します。
    * @param {String} message 表示するメッセージ
    * @param {String} target ローディングが表示されている要素。指定されていない場合は body 要素が利用されます。
    */
    loading.message = function (message, target) {
        var place = !!target ? $(target) : $(window.document.body),
            element = place.children(".loading");
        if (element.length > 0) {
            element.find(".loading-message").text(App.isUnusable(message) ? "" : message);
            return;
        }
    };

    /**
    * ローディングを閉じます。
    * @param {String} target ローディングが表示されている要素。指定されていない場合は body 要素が利用されます。
    */
    loading.close = function (target) {
        var place = !!target ? $(target) : $(window.document.body),
            element = place.children(".loading");
        element.remove();
    };

})(jQuery);

/*global jQuery */
/*global App */

/// <reference path="../../ts/core/base.ts" />

(function ($) {
    "use strict";

    var defaultPagingButtonDisplayCount = 5,
        previousCaption = "前へ",
        nextCaption = "次へ",
        morePageCaption = "...";

    function prepareNumber(value) {
        return App.isNum(value) ? Math.max(value, 0) : 0;
    }

    function getCurrentPageInfo() {
        var startOfPage = this.options.countPerPage * (this.state.currentPage - 1),
            endOfPage = Math.min(startOfPage + this.options.countPerPage - 1, this.options.dataCount - 1);
        if (this.options.dataCount <= 0 || this.options.countPerPage <= 0) {
            startOfPage = -1;
            endOfPage = -1;
        }
        return {
            totalPageCount: this.state.totalPageCount,
            currentPage: this.state.currentPage,
            totalDataCount: this.options.dataCount,
            countPerPage: this.options.countPerPage,
            startOfPage: startOfPage,
            endOfPage: endOfPage
        };
    }

    function preparePageNumber() {
        var that = this,
            state = that.state,
            firstButtonNum = 1,
            pagingButtonDisplayCount = state.pagingButtonDisplayCount,
            i = 0, maxDisplayPage = Math.min(pagingButtonDisplayCount, state.totalPageCount),
            button;

        firstButtonNum = state.currentPage - pagingButtonDisplayCount < 1 ? 1 :
            (state.currentPage - pagingButtonDisplayCount + 1);

        if (state.currentPage <= 1) {
            state.previousButton.addClass("disabled");
        } else {
            state.previousButton.removeClass("disabled");
        }
        if (state.totalPageCount <= state.currentPage) {
            state.nextButton.addClass("disabled");
        } else {
            state.nextButton.removeClass("disabled");
        }
        state.morePageLabel.hide();
        state.lastButton.hide();
        state.previousButton.data("paging-target", state.currentPage - 1 < 1 ? null : state.currentPage - 1);
        state.nextButton.data("paging-target", state.currentPage + 1 > state.totalPageCount ? null : state.currentPage + 1);
        state.lastButton.data("paging-target", state.totalPageCount);

        if (this.options.dataCount <= 0 || this.options.countPerPage <= 0) {
            maxDisplayPage = 1;
            state.currentPage = 1;
            state.totalPageCount = 1;
        }

        if (state.currentPage > state.totalPageCount - (pagingButtonDisplayCount - 1)){
            firstButtonNum = Math.max(state.totalPageCount - (pagingButtonDisplayCount - 1), 1);
        }

        for (; i < maxDisplayPage; i++) {
            button = state.buttons[i];
            button.removeClass("active").data("paging-target", firstButtonNum + i).find("a").text(firstButtonNum + i);
            if ((firstButtonNum + i) === state.currentPage) {
                button.addClass("active");
            }
        }

        if (state.totalPageCount > pagingButtonDisplayCount &&
            state.currentPage < state.totalPageCount &&
            firstButtonNum < state.totalPageCount - (pagingButtonDisplayCount - 1)) {

            state.morePageLabel.show();
            state.lastButton.show();
        }
        that.options.onPageSelect(getCurrentPageInfo.bind(this)());

    }

    function renderButtons() {
        var that = this,
            state = that.state,
            i = 0, l = Math.min(state.totalPageCount, state.pagingButtonDisplayCount),
            button;

        if (this.options.dataCount <= 0 || this.options.countPerPage <= 0) {
            l = 1;
        }

        for (; i < l; i++) {
            button = $("<li class='page'><a href='#'></a></li>");
            state.nextButton.before(button);
            if (!state.buttons) {
                state.buttons = [];
            }
            state.buttons.push(button);
        }
        state.morePageLabel.addClass("disabled");
        state.nextButton.before(state.morePageLabel);
        state.nextButton.before(state.lastButton);
        preparePageNumber.bind(that)();
    }

    function onPagingButtonClick(e) {
        var that = this,
            state = that.state,
            target = $(e.currentTarget),
            targetPage = target.data("paging-target"),
            eventArg = getCurrentPageInfo.bind(that)(),
            beforePage = eventArg.currentPage;
        if (!targetPage || targetPage === that.state.currentPage) {
            return;
        }

        eventArg.requestPage = targetPage;
        eventArg.cancel = false;
        that.options.onPagingRequest(eventArg);
        if(eventArg.cancel){
            return;
        }

        state.currentPage = targetPage;
        preparePageNumber.bind(that)();

        eventArg = getCurrentPageInfo.bind(that)();
        eventArg.beforePage = beforePage;
        that.options.onPagingRequested(eventArg);

    }

    function Pagenation(element, options) {
        this.element = $(element);

        this.element.on("click", "li", onPagingButtonClick.bind(this));
        this.initialize(options);
    }

    Pagenation.DEFAULTS = {
        dataCount: 0,
        countPerPage: 0,
        onPageSelect: App.noop,
        onPagingRequest: App.noop,
        onPagingRequested: App.noop
    };

    Pagenation.prototype.initialize = function (options) {
        var that = this;
        that.options = $.extend({}, Pagenation.DEFAULTS, options);
        if (!App.isFunc(that.options.onPageSelect)) {
            that.options.onPageSelect = App.noop;
        }
        if (!App.isFunc(that.options.onPagingRequest)) {
            that.options.onPagingRequest = App.noop;
        }
        if (!App.isFunc(that.options.onPagingRequested)) {
            that.options.onPagingRequested = App.noop;
        }
        var totalPageCount = Math.ceil(App.ifUnusable(prepareNumber(options.dataCount) / prepareNumber(options.countPerPage), 0)),
            root = $("<ul class='pagination'>"),
            previousButton = $("<li class='navigation'><a href='#' class='previous'>" + previousCaption + "</a></li>"),
            nextButton = $("<li class='navigation'><a href='#' class='next'>" + nextCaption + "</a></li>"),
            // morePageLabel = $("<li class='navigation'><span>" + morePageCaption + "</span></li>"),
            morePageLabel = $("<li><span>" + morePageCaption + "</span></li>"),
            lastButton = $("<li class='page'><a href='#' class='last'>" + totalPageCount + "</a></li>");

        that.element.empty();
        that.state = {
            totalPageCount: totalPageCount,
            currentPage: totalPageCount ? 1 : 0,
            root: root,
            previousButton: previousButton,
            nextButton: nextButton,
            morePageLabel: morePageLabel,
            lastButton: lastButton,
            pagingButtonDisplayCount: defaultPagingButtonDisplayCount
        };

        root.append(previousButton);
        root.append(nextButton);
        that.element.append(root);
        renderButtons.bind(this)();
    };

    Pagenation.prototype.getCurrentPage = function (operation) {
        if (App.isFunc(operation)) {
            operation(getCurrentPageInfo.bind(this)());
        }
    };

    Pagenation.prototype.reset = function (options) {
        options = options || {};
        options.onPageSelect = this.options.onPageSelect;
        options.onPagingRequest = this.options.onPagingRequest;
        options.onPagingRequested = this.options.onPagingRequested;
        this.initialize(options);
    };

    $.fn.pagenation = function (options) {
        var args = arguments;
        return this.each(function () {
            var $self = $(this),
                data = $self.data("aw.pagenation");
            if (!data) {
                $self.data("aw.pagenation", (data = new Pagenation($self, options)));
            }
            if (typeof options === "string") {
                data[options].apply(data, Array.prototype.slice.call(args, 1).concat(data));
            }
        });
    };
})(jQuery);

/*global App */
/*global jQuery */

///<reference path="../../ts/core/base.ts" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function ($) {

    "use strict";

    App.define("App.ui.util", {
        /**
         * select 要素の選択項目を追加します。
         */
        appendOptions: function (options) {
            var container,
                target,
                data = [],
                filter = function () {
                    return true;
                },
                reviver = function (obj) {
                    return {
                        value: obj[options.value],
                        display: obj[options.display]
                    };
                },
                createOptionElement = function (obj) {
                    var optionElement = document.createElement("option");
                    obj = obj || {};
                    optionElement.value = App.isUndefOrNull(obj.value) ? "" : (obj.value + "");
                    optionElement.appendChild(document.createTextNode(App.isUndefOrNull(obj.display) ? "" : (obj.display + "")));
                    return optionElement;
                },
                i, l, item, appendObj,
                fragment = document.createDocumentFragment();

            options = options || {};
            target = App.isUndefOrNull(options.target) ? target : $(options.target);
            if (target.length === 0) {
                return;
            }

            container = target[0];
            data = App.isArray(options.data) ? options.data : data;
            filter = App.isFunc(options.filter) ? options.filter : filter;
            reviver = App.isFunc(options.reviver) ? options.reviver : reviver;

            if (!container) {
                return;
            }
            if (options.useEmpty) {
                fragment.appendChild(createOptionElement());
            }

            for (i = 0, l = data.length; i < l; i++) {
                item = data[i];
                if (!filter(item)) {
                    continue;
                }
                appendObj = reviver(item);
                if (!appendObj) {
                    continue;
                }
                fragment.appendChild(createOptionElement(appendObj, item));
            }
            container.appendChild(fragment);
        },
        /*
         * 新規ウィンドウを開く
         * @param url 開くコンテンツのURL
         * @param [target] ウィンドウ名。未指定の場合は _blank が利用される
         * @param [activate] ウィンドウを開いた後に新しいウィンドウにフォーカスをうつすかどうか
         */
        openNewWindow: function (url, target, activate) {
            var newWin = window.open(url, target || "_blank");
            if (activate && newWin) {
                if (!newWin.closed) {
                    newWin.focus();
                }
            }
            return newWin;
        }
    });
})(jQuery);

/*global jQuery App*/
///<reference path="../../ts/core/base.ts" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function ($) {

    "use strict";

    var notify = App.define("App.ui.notify"),
        alertNotifySetting = {
            containerClass: "alert-notify",
            textContainerClass: "alert-notify-text",
            labelClass: "error",
            clickableClass: "alert-clickable",
            targetElemData: "alert-target",
            targetElemClass: "error",
            defaultTimeout: 0
        },
        infoNotifySetting = {
            containerClass: "info-notify",
            textContainerClass: "info-notify-text",
            labelClass: "info",
            clickableClass: "info-clickable",
            targetElemData: "info-target",
            targetElemClass: "info",
            defaultTimeout: 3000
        },
        setupSlideUpMessage = function (root, opts, notifySetting) {
            var container,
                messagesContainer,
                clearTimeoutId = 0,
                notifyObj,
                titleHolder = $("<div class='notify-title-holder'>" +
                    "<div class='notify-title-open' style='display:inline-block;'><span class='glyphicon glyphicon-chevron-down'></span></div>" +
                    "<div class='notify-title-close' style='display:none;'><span class='glyphicon glyphicon-chevron-up'></span></div>" +
                    "<p class='notify-title-message-length badge' style='display:inline-block;'></p>" +
                    "<p class='notify-title-message' style='display:inline-block;'></p>" +
                    "</div>"),
                settings = {
                    container: "<div class='notify " + notifySetting.containerClass + "' style='display:none;'><ul></ul></div>",
                    messagesContainerQuery: "ul",
                    textContainer: "<li class='" + notifySetting.textContainerClass + "'></li>",
                    show: function () { },
                    clear: function () { }
                },
                closeButton = titleHolder.find(".notify-title-close"),
                openButton = titleHolder.find(".notify-title-open"),
                titleMessageLength = titleHolder.find(".notify-title-message-length"),
                hasTitle = false;

            root = $(root ? root : document.body);

            settings = $.extend({}, settings, opts);
            container = $(settings.container);
            titleHolder.insertBefore(container.children(":first"));
            if (settings.title) {
                titleHolder.find(".notify-title-message").text(settings.title);
                hasTitle = true;
            }else if (container.attr("title")) {
                titleHolder.find(".notify-title-message").text(container.attr("title"));
                hasTitle = true;
                container.attr("title", "");
            }
            messagesContainer = container.find(settings.messagesContainerQuery);
            if (container.parent().length < 1) {
                root.append(container);
            }

            titleHolder.on("click", function () {
                if (closeButton.css("display") === "none") {
                    messagesContainer.hide();
                    closeButton.css("display", "inline-block");
                    openButton.css("display", "none");
                } else {
                    messagesContainer.show();
                    closeButton.css("display", "none");
                    openButton.css("display", "inline-block");
                }
            });

            notifyObj = {
                message: function (text, unique) {
                    var textElem;

                    //jquery を解除
                    if (unique && unique.jquery) {
                        unique = unique[0];
                    }

                    if (unique) {
                        messagesContainer.children().each(function (index, elem) {
                            var current = $(elem),
                                target = current.data(notifySetting.targetElemData);
                            if ((unique.nodeType && unique === target) || unique === target) {
                                current.off("click");
                                current.children().text(text);
                                textElem = current;
                            }
                        });
                    }

                    if (!textElem) {
                        textElem = $(settings.textContainer);
                        textElem.append("<pre class='" + notifySetting.labelClass + "'></pre>");
                        textElem.children().text(text);
                        messagesContainer.append(textElem);
                    }

                    if((unique && unique.nodeType) || App.isFunc(opts.click)){
                        textElem.addClass(notifySetting.clickableClass);
                        if(unique && unique.nodeType){
                            $(unique).addClass(notifySetting.targetElemClass);
                            textElem.data(notifySetting.targetElemData, unique);
                        }
                        textElem.on("click", function () {
                            var arg = {
                                unique: unique,
                                handled: false
                            };
                            if (App.isFunc(opts.click)) {
                                opts.click(unique, text);
                            } else if(unique.nodeType && !arg.handled && $(unique).is(":visible") && !$(unique).is(":disabled")) {
                                unique.focus();
                            }
                        });
                    }

                    titleMessageLength.text(messagesContainer.children().length);

                    return notifyObj;
                },
                action: function (text, options) {
                    var textElem, html;

                    if (!textElem) {
                        textElem = $(settings.textContainer);
                        textElem.append("<pre class='" + notifySetting.labelClass + "'></pre>");
                        textElem.children().text(text);
                        messagesContainer.append(textElem);

                        if (options) {
                            if (options.html instanceof jQuery) {
                                html = options.html;
                            } else {
                                html = $(options.html);
                            }

                            textElem.find("pre").append(html);
                            if (App.isFunc(options.handler)) {
                                textElem.on(options.events, options.selector, options.handler);
                            }
                        }
                    }

                    titleMessageLength.text(messagesContainer.children().length);

                    return notifyObj;
                },
                show: function (timeout) {
                    timeout = timeout ? timeout : notifySetting.defaultTimeout;

                    if (messagesContainer.children().length > 0) {
                        //container.show("slide", { direction: "down" }, 500);
                        container.show();
                    }

                    if (clearTimeoutId !== 0) {
                        clearTimeout(clearTimeoutId);
                    }
                    clearTimeoutId = 0;

                    if (timeout > 0) {
                        clearTimeoutId = setTimeout(function () {
                            notifyObj.clear();
                        }, timeout);
                    }
                    if (App.isFunc(settings.show)) {
                        settings.show();
                    }
                    return notifyObj;
                },

                remove: function (unique) {

                    //jquery を解除
                    if (unique && unique.jquery) {
                        unique = unique[0];
                    }

                    messagesContainer.children().each(function (index, elem) {
                        var current = $(elem),
                            target = current.data(notifySetting.targetElemData);
                        if (unique) {
                            if ((unique.nodeType && unique === target)) {
                                current.css("cursor", "default");
                                current.off("click");
                                $(unique).removeClass(notifySetting.targetElemClass);
                            }
                            if ((unique.nodeType && unique === target) || unique === target) {
                                current.remove();
                            }
                        } else if (App.isUndef(target)) {
                            current.remove();
                        }
                    });
                    if (hasTitle) {
                        titleMessageLength.text(messagesContainer.children().length);
                    }
                    if (messagesContainer.children().length < 1) {
                        messagesContainer.empty();
                        container.hide();
                        messagesContainer.show();
                        if (App.isFunc(settings.clear)) {
                            settings.clear();
                        }
                    }
                    return notifyObj;
                },

                clear: function () {
                    messagesContainer.children().each(function (index, elem) {
                        var target = $($(elem).data(notifySetting.targetElemData));
                        target.removeClass(notifySetting.targetElemClass);
                    });

                    messagesContainer.empty();
                    container.hide();
                    messagesContainer.show();

                    if (App.isFunc(settings.clear)) {
                        settings.clear();
                    }
                    return notifyObj;
                },
                count: function () {
                    return messagesContainer.children().length;
                }
            };
            return notifyObj;
        };

    /**
    * 情報メッセージを表示する機能を提供します。
    * 戻りで返されるオブジェクトは message / show / clear メソッドをもち、メッセージの追加、表示、削除と非表示を制御します。
    * @param {String} title タイトル
    * @param {String} subtitle サブタイトル
    * @return {Object} 情報メッセージを表示するためのオブジェクト
    */
    notify.info = function (root, opts) {
        return setupSlideUpMessage(root, opts, infoNotifySetting);
    };
    /**
    * 警告メッセージを表示する機能を提供します。
    * 戻りで返されるオブジェクトは message / show / clear メソッドをもち、メッセージの追加、表示、削除と非表示を制御します。
    * @param title タイトル
    * @param subtitle サブタイトル
    * @return {Object} 警告メッセージを表示するためのオブジェクト
    */
    notify.alert = function (root, opts) {
        return setupSlideUpMessage(root, opts, alertNotifySetting);
    };

})(jQuery);

/* global App jQuery */

///<reference path="../../ts/core/base.ts" />
/// <reference path="../core/async.js" />
/// <reference path="../ui/notify.js" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, $) {

    "use strict";

    function ifUndefTree(ns, def) {
        var targetString = ns + "",
            names = targetString.split("."),
            current = global,
            i, l, item;

        for (i = 0, l = names.length; i < l; i++) {
            item = names[i];
            current = current[item];
            if (App.isUndef(current)) {
                return def;
            }
        }
        return current;
    }

    var page = App.define("App.ui.page"),
        messages = ifUndefTree("App.messages.base", {});

    /*
    * 確認ダイアログを表示します。
    * @param options ダイアログのパラメーター
    * e.g. App.ui.page.defaults.confirm({
    *   ok: "実行", //OKボタンのキャプション(default "OK")
    *   cancel: "取り消し", //Cancelボタンのキャプション(default "キャンセル")
    *   title: "実行確認", //ダイアログのタイトル(default "確認")
    *   text: "実行しますか？" //ダイアログに表示されるメッセージ
    * })
    * @returns jQuery.Promise
    */
    page.confirm = function confirm(options) {
        options = options || {};

        var ok = options.ok || "OK",
            cancel = options.cancel || "キャンセル",
            title = options.title || "確認",
            text = options.text,
            //TODO: maruyama 利用しているHTMLを外部化できないか検討
            //Sharedの下に置くイメージ
            dialog = $("#confirm-dialog"),
            footer = dialog.find(".modal-footer"),
            defer = $.Deferred(), //eslint-disable-line new-cap
            isOk = false,
            show = function (el, txt) {
                if (txt) {
                    el.html(txt); el.show();
                } else {
                    el.hide();
                }
            };

        show(dialog.find(".modal-body .item-label"), text);

        dialog.find(".modal-body").css("padding-bottom", 0);
        dialog.find(".modal-body .item-label").css("font-size", 14).css("height", "100%");
        show(dialog.find(".modal-header h4"), title);
        footer.find(".btn-primary").unbind("click").html(ok);
        footer.find(".btn-cancel").unbind("click").html(cancel);
        dialog.modal("show");

        var dialogHeight = dialog.find(".modal-dialog").height();
        dialog.css("padding-top",
            ($(window).height() / 2) - (dialogHeight === 0 ? 110 : dialogHeight / 2));

        footer.find(".btn-primary").on("click", function () {
            isOk = true;
            dialog.modal("hide");
        });
        footer.find(".btn-cancel").on("click", function () {
            isOk = false;
            dialog.modal("hide");
        });
        dialog.on("hide.bs.modal", function () {
            (isOk ? defer.resolve : defer.reject)();
        });

        return defer.promise();
    };

    page.applyCollapsePanel = function (target) {
        var panel = !!target ? $(target) : $(".panel");
        panel.on("hidden.bs.collapse", function (e) {
            var $head = $(e.target).parent().find(".panel-heading");
            $head.find(".glyphicon-chevron-down").show();
            $head.find(".glyphicon-chevron-up").hide();
        });

        panel.on("shown.bs.collapse", function (e) {
            var $head = $(e.target).parent().find(".panel-heading");
            $head.find(".glyphicon-chevron-down").hide();
            $head.find(".glyphicon-chevron-up").show();
        });
    };
    /**
     * 画面のメッセージ通知処理をデフォルト値で初期化します。
     */
    page.notify = function (click) {
        return {
            info: App.ui.notify.info(document.body, {
                container: ".slideup-area .info-message",
                messageContainerQuery: "ul",
                show: function () {
                    $(".slideup-area .info-message").show();
                },
                clear: function () {
                    $(".slideup-area .info-message").hide();
                },
                title: messages.InfoMessagesTitle || "",
                click: click
            }),
            alert: App.ui.notify.alert(document.body, {
                container: ".slideup-area .alert-message",
                messageContainerQuery: "ul",
                show: function () {
                    $(".slideup-area .alert-message").show();
                },
                clear: function () {
                    $(".slideup-area .alert-message").hide();
                },
                title: messages.AlertMessagesTitle || "",
                click: click
            })
        };
    };

    /**
     * ダイアログのメッセージ通知処理をデフォルト値で初期化します。
     */
    page.dialogNotify = function (root) {
        return {
            info: App.ui.notify.info(root, {
                container: root.find(".dialog-slideup-area .info-message"),
                messageContainerQuery: "ul",
                show: function () {
                    root.find(".info-message").show();
                },
                clear: function () {
                    root.find(".info-message").hide();
                },
                title: messages.InfoMessagesTitle || ""
            }),
            alert: App.ui.notify.alert(root, {
                container: root.find(".dialog-slideup-area .alert-message"),
                messageContainerQuery: "ul",
                show: function () {
                    root.find(".alert-message").show();
                },
                clear: function () {
                    root.find(".alert-message").hide();
                },
                title: messages.AlertMessagesTitle || ""
            })
        };
    };

    page.applyInput = function (root) {
        var elem = $(root || window.body),
            apply = function (target, option) {
                target.each(function (i, item) {
                    $(item).input($.extend({}, option));
                });
            };
        var curr = App.culture.current(),
            namedDateFormat = curr.dateTimeFormat.named || {},
            namedNumberFormat = curr.numberFormat.named || {},
            key;

        apply(elem.find("[data-role='time']"), { type: "time" });
        apply(elem.find("[data-role='zipcode']"), { type: "zipcode" });

        for (key in namedDateFormat) {
            if (!namedDateFormat.hasOwnProperty(key)) {
                continue;
            }
            if (key === "time" || key === "zipcode") {
                continue;
            }
            apply(elem.find("[data-role='" + key + "']"),
                { type: "date", format: namedDateFormat[key] });
        }
        for (key in namedNumberFormat) {
            if (!namedNumberFormat.hasOwnProperty(key)) {
                continue;
            }
            if (key === "time" || key === "zipcode") {
                continue;
            }
            apply(elem.find("[data-role='" + key + "']"),
                { type: "number", format: namedNumberFormat[key] });
        }
    };
    /**
     * 単票カラムの正常状態のスタイルの標準の適用を定義します。
     */
    page.setColValidStyle = function (target) {
        var targetHolder = $(target).closest(".control"),
            labelHolder,
            getControlLabelHolder = function (item, depth) {
                var prev = item.prev();
                if (!prev.length) {
                    if (depth === 0) {
                        return getControlLabelHolder(item.closest(".row").closest(".control"), 1);
                    }
                    return $();
                }
                if (prev.hasClass("control-label")) {
                    return prev;
                }
                return getControlLabelHolder(prev, depth);
            },
            hasError = function (item, depth) {
                var nextTarget;

                if (!item.hasClass("control")) {
                    return false;
                } else {
                    if (item.children(":first").hasClass("row")) {
                        if (depth === 0) {
                            if (hasError(item.find(".control:first"), 1)) {
                                return true;
                            }
                            nextTarget = item.next();
                            if (!nextTarget.length) {
                                return false;
                            }
                            return hasError(nextTarget, depth);
                        }
                    }
                }

                if (item.hasClass("control-required")) {
                    return true;
                }
                nextTarget = item.next();
                if (!nextTarget.length) {
                    return false;
                }
                return hasError(nextTarget, depth);
            };
        labelHolder = getControlLabelHolder(targetHolder, 0);
        targetHolder.addClass("control-success");
        targetHolder.removeClass("control-required");

        if (!hasError(labelHolder.next(), 0)) {
            labelHolder.addClass("control-success-label");
            labelHolder.removeClass("control-required-label");
        }
    };
    /**
     * 単票カラムの異常状態のスタイルの標準の適用を定義します。
     */
    page.setColInvalidStyle = function (target) {

        var targetHolder = $(target).closest(".control"),
            labelHolder,
            getControlLabelHolder = function (item, depth) {
                var prev = item.prev();
                if (!prev.length) {
                    if (depth === 0) {
                        return getControlLabelHolder(item.closest(".row").closest(".control"), 1);
                    }
                    return $();
                }
                if (prev.hasClass("control-label")) {
                    return prev;
                }
                return getControlLabelHolder(prev, depth);
            };
        labelHolder = getControlLabelHolder(targetHolder, 0);
        targetHolder.addClass("control-required");
        targetHolder.removeClass("control-success");
        labelHolder.addClass("control-required-label");
        labelHolder.removeClass("control-success-label");
    };

    /**
     * バリデーション成功時の標準のハンドラーを定義します。
     */
    page.validationSuccess = function (notify) {
        return function validationSuccess(results, state) {
            var i = 0, l = results.length,
                item;

            for (; i < l; i++) {
                item = results[i];
                if (state && state.isGridValidation && state.dataTable) {
                    /*eslint-disable no-loop-func */
                    state.dataTable.dataTable("getRow", $(item.element), function (row) {
                        if (row && row.element) {
                            row.element.removeClass("has-error");
                        }
                    });
                    /*eslint-enable no-loop-func */
                } else {
                    App.ui.page.setColValidStyle(item.element);
                }
                notify.alert.remove(item.element);
            }
        };
    };
    /**
     * バリデーション失敗時の標準のハンドラーを定義します。
     */
    page.validationFail = function (notify) {
        return function validationFail(results, state) {

            var i = 0, l = results.length,
                item;

            for (; i < l; i++) {
                item = results[i];
                if (state && state.isGridValidation && state.dataTable) {
                    /*eslint-disable no-loop-func */
                    state.dataTable.dataTable("getRow", $(item.element), function (row) {
                        if (row && row.element) {
                            row.element.addClass("has-error");
                        }
                    });
                    /*eslint-enable no-loop-func */
                } else {
                    App.ui.page.setColInvalidStyle(item.element);
                }

                if (state && state.suppressMessage) {
                    continue;
                }
                notify.alert.message(item.message, item.element).show();
            }
        };
    };
    /**
     * バリデーション実行完了時の標準のハンドラーを定義します。
     */
    page.validationAlways = function (notify) {  //eslint-disable-line no-unused-vars
        return function validationAlways(results, state) { //eslint-disable-line no-unused-vars
        };
    };

    function createNormalizedErrorByApi(err) {
        if (err.responseJSON) {
            err = err.responseJSON;
        }
        if (!!err.errorType) {
            return $.extend({}, err, {
                processType: "api"
            });
        }
    }

    function createNormalizedErrorByHttp(err) {
        var result = App.ajax.handleError(err);
        if (result) {
            return $.extend({}, result, {
                processType: "http",
                errorType: "system_error"
            });
        }
    }

    function createNormalizedErrorByJsError(err) {
        if (err instanceof Error) {
            return {
                processType: "client",
                errorType: "system_error",
                message: err.message,
                description: err.stack || ""
            };
        }
    }

    function createNormalizedErrorByUnknown(err) {
        var e = App.isUnusable(err) ? {} : err,
            message = e + "";
        if (e.message) {
            message = e.message;
        }
        return $.extend({}, App.isObj(e) ? e : {}, {
            processType: "client",
            errorType: "system_error",
            message: message
        });
    }

    function createNormalizedError(err) {
        var result;
        result = !result ? createNormalizedErrorByApi(err) : result;
        result = !result ? createNormalizedErrorByHttp(err) : result;
        result = !result ? createNormalizedErrorByJsError(err) : result;
        return !result ? createNormalizedErrorByUnknown(err) : result;
    }

    function retrievePromiseAllChainErrors(err) {
        var result = [];
        Object.keys(err.fails).forEach(function (key) {
            var item = err.fails[key];
            if (item) {
                if (item.fails && item.successes) {
                    result = result.concat(retrievePromiseAllChainErrors(item));
                } else {
                    result.push(createNormalizedError(item));
                }
            }
        });
        return result;
    }

    page.normalizeError = function (error) {
        var errors = App.isArray(error) ? error : [error],
            i = 0, l = errors.length, err,
            result = [];
        for (; i < l; i++) {
            err = errors[i];
            if (err.fails && err.successes) {
                result = result.concat(retrievePromiseAllChainErrors(err));
            } else {
                result.push(createNormalizedError(err));
            }
        }
        return result;
    };

    page.resolveUrl = function (url) {
        var appRoot = "/",
            target = url + "";
        if (App.settings && App.settings.base && App.settings.base.applicationRootUrl) {
            appRoot = App.settings.base.applicationRootUrl + "";
        }
        if (page.appRootPath) {
            appRoot = page.appRootPath + "";
        }
        if (!App.str.startsWith(appRoot, "/")) {
            appRoot = "/" + appRoot;
        }
        if (App.str.startsWith(target, "/")) {
            target = target.substr(1);
        }
        if (!target.length) {
            return appRoot;
        }
        if (!App.str.endsWith(appRoot, "/")) {
            appRoot = appRoot + "/";
        }
        return appRoot + target;
    };

    //validation の 結果を jQuery Promise で取得するように設定
    if (App.validation) {
        App.validation.setReturnPromise(function (callback) {
            var defer = $.Deferred(); //eslint-disable-line new-cap
            callback(defer.resolve, defer.reject);
            return defer.promise();
        });
    }
    //async の 結果を jQuery Promise で取得するように設定
    if (App.async) {
        App.async.setReturnPromise(function (callback) {
            var defer = $.Deferred(); //eslint-disable-line new-cap
            callback(defer.resolve, defer.reject);
            return defer.promise();
        });
    }

    page.extend = function (pageObject) {
        var basePage = {
            on: function ($element) {
                var self = this,
                    args = Array.prototype.slice.call(arguments);
                args.shift();

                var i, len, arg, handler;
                for (i = 0, len = args.length; i < len; i++) {
                    arg = args[i];
                    if (App.isFunc(arg)) {
                        handler = arg;
                        break;
                    }
                }
                if (handler) {
                    args[i] = function (ev) {
                        if (ev.data && ev.data.message) {
                            self.logger.info(ev.data.message);
                        }
                        handler(ev);
                    };
                }

                $element.on.apply($element, args);
            },
            logger: new App.logging.Logger(pageObject.name)
        };

        $.extend(pageObject, basePage);

        $(document).ajaxError(function (event, req, otpions, thrownError) { // eslint-disable-line no-unused-vars
            App.ui.page.normalizeError(req).forEach(function (e) {
                pageObject.logger.error(e.message);
            });
        });
    };

})(window || this, jQuery);

/* global App */

/// <reference path="../../ts/core/culture.ts" />
/// <reference path="../../ts/core/num.ts" />
/// <reference path="../../ts/core/date.ts" />

//perse
(function () {

    "use strict";

    App.perse = {
        converteByFormatDataAnnotation: function (format, element, defaultValue) {
            var result,
                curr = App.culture.current(),
                namedDateFormat = curr.dateTimeFormat.named || {},
                namedNumberFormat = curr.numberFormat.named || {};

            if (format in namedNumberFormat) {
                return App.num.parse(defaultValue, format) || defaultValue;
            }
            if (format in namedDateFormat) {
                if (App.date.hasNamedParser(format)) {
                    return App.date.parse(defaultValue, format) || defaultValue;
                }
                return App.date.parse(defaultValue, namedDateFormat[format]) || defaultValue;
            }

            result = App.num.parse(defaultValue, format);
            if (!result) {
                result = App.date.parse(defaultValue, format);
            }
            return result || defaultValue;
        },
        applyByFormatDataAnnotation: function (format, value, element) {
            var apply = function (elem, val) {
                if (elem.is(":input")) {
                    elem.val(val);
                } else {
                    elem.text(val);
                }
            };

            var curr = App.culture.current(),
                namedDateFormat = curr.dateTimeFormat.named || {},
                namedNumberFormat = curr.numberFormat.named || {};
            if (format in namedNumberFormat) {
                if (App.num.hasNamedFormatter(format)) {
                    apply(element, App.num.format(value || 0, format));
                    return true;
                }
                apply(element, App.num.format(value || 0, namedNumberFormat[format]));
                return true;
            }
            if (format in namedDateFormat) {
                if (!App.isDate(value)) {
                    var formats = ["ISO8601Full", "ISO8601", "ISO8601DateTimeMs", "yyyy/MM/dd"]
                        .concat(App.obj.values(namedDateFormat));
                    value = App.date.parse(value + "", formats);
                }
                if (App.date.hasNamedFormatter(format)) {
                    apply(element, App.date.format(value || 0, format));
                    return true;
                }
                apply(element, App.date.format(value || 0, namedDateFormat[format]));
                return true;
            }
            return false;
        }
    };

})(this);
