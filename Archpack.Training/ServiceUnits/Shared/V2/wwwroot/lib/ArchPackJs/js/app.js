(function () {

    "use strict";

    function Empty() { }

    var slice = Array.prototype.slice,
        hasDontEnumBug = (function () {
            for (var key in { "toString": null }) { //eslint-disable-line no-unused-vars
                return false;
            }
            return true;
        })(),
        dontEnums = [
            "toString",
            "toLocaleString",
            "valueOf",
            "hasOwnProperty",
            "isPrototypeOf",
            "propertyIsEnumerable",
            "constructor"
        ],
        dontEnumsLength = dontEnums.length,
        hasOwnProperty = Object.prototype.hasOwnProperty,
        isFunc = function(val){
            return Object.prototype.toString.call(val) === "[object Function]";
        },
        isStr = function (target) {
            return Object.prototype.toString.call(target) === "[object String]";
        },
        isNum = function (target) {
            return Object.prototype.toString.call(target) === "[object Number]";
        },
        isNumeric = function (target) {
            //文字列か数値以外を弾く
            //["123"]などは先頭の値を利用されるため
            if (isStr(target) || isNum(target)) {
                return !isNaN(parseFloat(target)) && isFinite(target);
            }
            return false;
        };

    if (!Object.keys) {
        Object.keys = function (value) {
            var keys = [],
                i = 0, l = dontEnumsLength, dontEnum;

            if ((typeof value !== "object" && typeof value !== "function") || value === null) {
                throw new TypeError("Object.keys called on a non-object");
            }

            for (var key in value) {
                if (hasOwnProperty.call(value, key)) {
                    keys.push(key);
                }
            }

            if (hasDontEnumBug) {
                for (; i < l; i++) {
                    dontEnum = dontEnums[i];
                    if (hasOwnProperty.call(value, dontEnum)) {
                        keys.push(dontEnum);
                    }
                }
            }
            return keys;
        };

    }

    // Function#bind
    if (!Function.prototype.bind) {
        Function.prototype.bind = function bind(context) { //eslint-disable-line no-extend-native
            var target = this,
                args = slice.call(arguments, 1),
                bound = function () {
                    if (this instanceof bound) {
                        var result = target.apply(
                            this,
                            args.concat(slice.call(arguments))
                        );
                        /*jshint newcap: false*/
                        if (Object(result) === result) {
                            return result;
                        }
                        return this;
                    } else {
                        return target.apply(
                            context,
                            args.concat(slice.call(arguments))
                        );
                    }

                };
            if (target.prototype) {
                Empty.prototype = target.prototype;
                bound.prototype = new Empty();
                Empty.prototype = null;
            }
            return bound;
        };
    }

    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function indexOf(item, fromIndex) { //eslint-disable-line no-extend-native
            var self = this,
                i = 0, l;
            if (isNumeric(fromIndex)) {
                i = Math.max(0, parseInt(fromIndex, 10));
            }
            for (l = self.length; i < l; i++) {
                if (i in self && self[i] === item) {
                    return i;
                }
            }
            return -1;
        };
    }

    if (!Array.prototype.lastIndexOf) {
        Array.prototype.lastIndexOf = function lastIndexOf(item, fromIndex) { //eslint-disable-line no-extend-native
            var self = this,
                start = fromIndex, l;

            if (arguments.length < 2) {
                start = self.length;
            }
            if (isNumeric(start)) {
                l = Math.max(Math.min(parseInt(start, 10) + 1, self.length), 0);
            }
            while (l--) {
                if (l in self && self[l] === item) {
                    return l;
                }
            }
            return -1;
        };
    }

    if (!Array.prototype.every) {
        Array.prototype.every = function (callback, context) { //eslint-disable-line no-extend-native
            var i, l,
                self = this;
            if (!isFunc(callback)) {
                throw new TypeError();
            }
            for (i = 0, l = self.length; i < l; i++) {
                if (i in self && !callback.call(context, self[i], i, self)) {
                    return false;
                }
            }
            return true;
        };
    }

    if (!Array.prototype.some) {
        Array.prototype.some = function (callback, context) { //eslint-disable-line no-extend-native
            var i, l,
                self = this;
            if (!isFunc(callback)) {
                throw new TypeError();
            }
            for (i = 0, l = self.length; i < l; i++) {
                if (i in self && callback.call(context, self[i], i, self)) {
                    return true;
                }
            }
            return false;
        };
    }

    if (!Array.prototype.map) {
        Array.prototype.map = function (callback, context) { //eslint-disable-line no-extend-native
            var result = [],
                i, l,
                self = this;
            if (!isFunc(callback)) {
                throw new TypeError();
            }
            for (i = 0, l = self.length; i < l; i++) {
                if (i in self) {
                    result.push(callback.call(context, self[i], i, self));
                }
            }
            return result;
        };
    }

    if (!Array.prototype.reduce) {
        Array.prototype.reduce = function (callback, initialValue) { //eslint-disable-line no-extend-native
            var value, i, l, self = this;
            if (!isFunc(callback)) {
                throw new TypeError();
            }
            i = 0;
            value = initialValue;
            if (arguments.length < 2) {
                i = 1;
                value = self[0];
            }
            for (l = self.length; i < l; i++) {
                value = callback(value, self[i], i, self);
            }
            return value;
        };
    }

    if (!Array.prototype.reduceRight) {
        Array.prototype.reduceRight = function (callback, initialValue) { //eslint-disable-line no-extend-native
            var value, l, self = this;
            if (!isFunc(callback)) {
                throw new TypeError();
            }
            l = self.length;
            value = initialValue;
            if (arguments.length < 2) {
                l--;
                value = self[l];
            }
            while (l--) {
                value = callback(value, self[l], l, self);
            }
            return value;
        };
    }

    if (!Array.prototype.forEach) {
        Array.prototype.forEach = function (iterator, context) { //eslint-disable-line no-extend-native

            for (var i = 0, len = this.length; i < len; i++) {
                if (i in this) {
                    iterator.call(context, this[i], i, this);
                }
            }
        };
    }

    if (!Array.prototype.filter) {
        Array.prototype.filter = function (callback, context) { //eslint-disable-line no-extend-native
            var self = this,
                i = 0, l = self.length,
                result = [], val;

            if (!isFunc(callback)) {
                throw new TypeError();
            }

            for (; i < l; i++) {
                if (i in self) {
                    val = self[i];
                    if (callback.call(context, val, i, self)) {
                        result.push(val);
                    }
                }
            }
            return result;
        };
    }

    if (!Date.now) {
        Date.now = function () {
            return new Date().getTime();
        };
    }

})();

///<buildref path="../../js/core/shim.js" />
var App;
(function (App) {
    "use strict";
    var toString = Object.prototype.toString;
    var glob = Function("return this;")();
    /**
     * 第1引数に指定された文字列に従ってドットで区切られた階層上にオブジェクトを定義します。
     * 第2引数に指定されているオブジェクトのプロパティを作成したオブジェクトのプロパティに設定します。
     * @param {string} name - 作成するnamespaceを表す . (dot) で区切られた文字列
     * @param {object} [props] - 作成するオブジェクトに設定するプロパティが定義されたオブジェクト
     * @param {object} [root] - 名前空間とオブジェクトを作成するルートオブジェクト。省略された場合はグローバルオブジェクト
     * @returns {object} 作成された名前空間に所属するオブジェクト
     * @example
     * console.log(typeof set === "undefined"); //true
     * let def = App.define("set.your.ns", { foo: "A", bar: 2 });
     * console.log(set.your.ns === def); //true
     */
    function define(name, props, root) {
        if (!name) {
            return;
        }
        var parent = root || glob, names = name.split("."), source = props;
        for (var _i = 0; _i < names.length; _i++) {
            var name_1 = names[_i];
            parent = parent[name_1] = parent[name_1] || {};
        }
        if (props) {
            for (var _a = 0, _b = Object.keys(props); _a < _b.length; _a++) {
                var key = _b[_a];
                parent[key] = source[key];
            }
        }
        return parent;
    }
    App.define = define;
    /**
     * 指定された値が {@link Object} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link Object} 型の場合は true 、そうでない場合は false
     */
    function isObj(target) {
        return toString.call(target) === "[object Object]" &&
            typeof target !== "undefined" && target !== null;
    }
    App.isObj = isObj;
    /**
     * 指定された値が {@link String} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link String} 型の場合は true 、そうでない場合は false
     */
    function isStr(target) {
        return toString.call(target) === "[object String]";
    }
    App.isStr = isStr;
    /**
     * 指定された値が {@link Number} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link Number} 型の場合は true 、そうでない場合は false
     */
    function isNum(target) {
        return toString.call(target) === "[object Number]";
    }
    App.isNum = isNum;
    /**
     * 指定された値が {@link Boolean} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link Boolean} 型の場合は true 、そうでない場合は false
     */
    function isBool(target) {
        return toString.call(target) === "[object Boolean]";
    }
    App.isBool = isBool;
    /**
     * 指定された値が {@link Date} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link Date} 型の場合は true 、そうでない場合は false
     */
    function isDate(target) {
        return toString.call(target) === "[object Date]";
    }
    App.isDate = isDate;
    /**
     * 指定された値が {@link Function} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link Function} 型の場合は true 、そうでない場合は false
     */
    function isFunc(target) {
        return toString.call(target) === "[object Function]";
    }
    App.isFunc = isFunc;
    /**
     * 指定された値が {@link Array} 型かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} {@link Array} 型の場合は true 、そうでない場合は false
     */
    function isArray(target) {
        return toString.call(target) === "[object Array]";
    }
    App.isArray = isArray;
    /**
     * 指定された値が valid な {@link Date} 型の値かを確認します。
     * @param {Any} target 判定する値
     * @return {Boolean} {@link Date} 型で valid な場合は true 、そうでない場合は false
     */
    function isValidDate(target) {
        if (isDate(target)) {
            return !isNaN(target.getTime());
        }
        return false;
    }
    App.isValidDate = isValidDate;
    /**
     * 指定された値が {@link RegExp}  型かどうかを確認します。
     * @param {Any} target 判定する値
     * @return {Boolean} {@link RegExp} 型の場合は true 、そうでない場合は false
     */
    function isRegExp(target) {
        return toString.call(target) === "[object RegExp]";
    }
    App.isRegExp = isRegExp;
    /**
     * 指定された値が undefined かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} undefined の場合は true 、そうでない場合は false
     */
    function isUndef(target) {
        if (arguments.length === 0) {
            return false;
        }
        return typeof target === "undefined";
    }
    App.isUndef = isUndef;
    /**
     * 指定された値が null かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} null の場合は true 、そうでない場合は false
     */
    function isNull(target) {
        return target === null;
    }
    App.isNull = isNull;
    /**
     * 指定された値が null もしくは undefined かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} null もしくは undefined の場合は true 、そうでない場合は false
     */
    function isUndefOrNull(target) {
        if (arguments.length === 0) {
            return false;
        }
        return (isUndef(target) || isNull(target));
    }
    App.isUndefOrNull = isUndefOrNull;
    /**
     * 指定された値が 利用可能でないかどうかを判断します。
     * @param {Any} target 判定する値
     * @return {Boolean} 利用不可能な場合は true 、そうでない場合は false
     */
    function isUnusable(target) {
        if (arguments.length === 0) {
            return false;
        }
        return isUndef(target) || isNull(target) ||
            (isNum(target) ? isNaN(target) || !isFinite(target) :
                (isDate(target) ? !isValidDate(target) : false));
    }
    App.isUnusable = isUnusable;
    /**
     * 指定された値が 数字 かどうかを確認します。
     * @param {Any} target - 判定する値
     * @return {boolean} 数字の場合は true 、そうでない場合は false
     */
    function isNumeric(target) {
        //文字列か数値以外を弾く
        //["123"]などは先頭の値を利用されるため
        if (isStr(target) || isNum(target)) {
            return !isNaN(parseFloat(target)) && isFinite(target);
        }
        return false;
    }
    App.isNumeric = isNumeric;
    /**
     * 指定された値が undefined かどうかによって戻す値を判断します。
     * @param {Any} target 判定する値
     * @param {Any} def undefined の場合に戻す値
     * @return {Any} undefinedの場合は def 、そうでない場合は target </returns>
     */
    function ifUndef(target, def) {
        return isUndef(target) ? def : target;
    }
    App.ifUndef = ifUndef;
    /**
     * 指定された値が null かどうかによって戻す値を判断します。
     * @param {Any} target 判定する値
     * @param {Any} def null の場合に戻す値
     * @return {Any} nullの場合は def 、そうでない場合は target
     */
    function ifNull(target, def) {
        return isNull(target) ? def : target;
    }
    App.ifNull = ifNull;
    /**
     * 指定された値が 利用可能かどうかによって戻す値を判断します。
     * @param {Any} target 判定する値
     * @param {Any} def 利用不可能な場合に戻す値
     * @return {Any}  利用不可能な場合は def 、そうでない場合は target
     */
    function ifUnusable(target, def) {
        return isUnusable(target) ? def : target;
    }
    App.ifUnusable = ifUnusable;
    /**
     * 指定された値が undefined もしくは null かどうかによって戻す値を判断します。
     * @param {Any} target 判定する値
     * @param {Any} def undefined もしくは null の場合に戻す値
     * @return {Any}   undefined もしくは null の場合は def 、そうでない場合は target
     */
    function ifUndefOrNull(target, def) {
        return (isUndef(target) || isNull(target)) ? def : target;
    }
    App.ifUndefOrNull = ifUndefOrNull;
    /**
     * 何も起こらない空の関数を取得します。
     */
    function noop() { }
    App.noop = noop;
    /**
     * global オブジェクトを取得します。
     */
    function global() {
        return glob;
    }
    App.global = global;
})(App || (App = {}));

/// <reference path="base.ts" />
var App;
(function (App) {
    var array;
    (function (array) {
        "use strict";
        /**
         * 引数で指定された配列の中から callback で true を返された最初の値を返します。
         */
        function find(target, callback) {
            if (!App.isArray(target) || !App.isFunc(callback)) {
                return;
            }
            for (var i = 0, l = target.length; i < l; i++) {
                if (callback(target[i], i, target)) {
                    return target[i];
                }
            }
        }
        array.find = find;
    })(array = App.array || (App.array = {}));
})(App || (App = {}));

///<reference path="base.ts" />
var App;
(function (App) {
    "use strict";
    var STATE_PENDING = 0;
    var STATE_FULFILLED = 1;
    var STATE_REJECTED = 2;
    function isThenable(value) {
        return App.isFunc((value || {}).then);
    }
    function execute(callback, value) {
        var that = this;
        if (!App.isFunc(callback)) {
            return;
        }
        (App.global().setImmediate || App.global().setTimeout)(function () {
            callback.call(that, value);
        }, 0);
    }
    function resolver(onFulfilled, onRejected) {
        this._innerThenableFulfill = onFulfilled;
        this._innerThenableReject = onRejected;
    }
    ;
    var ChildThenable = (function () {
        function ChildThenable(onFulfilled, onRejected) {
            this._onFulfilled = onFulfilled;
            this._onRejected = onRejected;
            /*eslint-disable no-use-before-define */
            this._innerThenable = new Thenable(resolver.bind(this));
            /*eslint-enable no-use-before-define */
        }
        ChildThenable.prototype.thenable = function () {
            return this._innerThenable;
        };
        ChildThenable.prototype.fulfill = function (value) {
            var result = value, that = this;
            if (!App.isFunc(this._onFulfilled)) {
                return;
            }
            try {
                var fulfillResult = that._onFulfilled.call(null, result);
                if (isThenable(fulfillResult)) {
                    fulfillResult.then(function (thenRes) {
                        that._innerThenableFulfill(thenRes);
                    }, function (thenRes) {
                        that._innerThenableReject(thenRes);
                    });
                }
                else {
                    that._innerThenableFulfill(fulfillResult);
                }
            }
            catch (e) {
                this._innerThenableReject(e);
            }
        };
        ChildThenable.prototype.reject = function (value) {
            var result = value, that = this;
            if (this._onRejected) {
                try {
                    var rejectResult = this._onRejected.call(null, result);
                    if (isThenable(rejectResult)) {
                        rejectResult.then(function (thenRes) {
                            that._innerThenableFulfill(thenRes);
                        }, function (thenRes) {
                            that._innerThenableReject(thenRes);
                        });
                    }
                    else {
                        this._innerThenableFulfill(rejectResult);
                    }
                    return;
                }
                catch (e) {
                    result = e;
                }
            }
            this._innerThenableReject(result);
        };
        return ChildThenable;
    })();
    function fulfilled(value) {
        var that = this, children = that._children;
        if (that._state) {
            return;
        }
        that._state = STATE_FULFILLED;
        that._result = value;
        for (var _i = 0; _i < children.length; _i++) {
            var child = children[_i];
            execute.call(child, child.fulfill, value);
        }
        that._children = [];
    }
    function rejected(value) {
        var that = this, children = that._children || [];
        if (that._state) {
            return;
        }
        that._state = STATE_REJECTED;
        that._result = value;
        for (var _i = 0; _i < children.length; _i++) {
            var child = children[_i];
            execute.call(child, child.reject, value);
        }
        that._children = [];
    }
    var Thenable = (function () {
        function Thenable(callback) {
            this._state = STATE_PENDING;
            this._children = [];
            this._state = STATE_PENDING;
            this._children = [];
            try {
                callback.call(this, fulfilled.bind(this), rejected.bind(this));
            }
            catch (e) {
                rejected.call(this, e);
            }
        }
        Thenable.prototype.then = function (onFulfilled, onRejected) {
            var child = new ChildThenable(onFulfilled, onRejected);
            if (!this._state) {
                this._children.push(child);
            }
            else {
                execute.call(child, (this._state === STATE_FULFILLED ? child.fulfill : child.reject), this._result);
            }
            return child.thenable();
            return null;
        };
        ;
        return Thenable;
    })();
    function thenable(callback) {
        var cb = App.isFunc(callback) ? callback : App.noop;
        return new Thenable(cb);
    }
    App.thenable = thenable;
    ;
})(App || (App = {}));
;

///<reference path="base.ts" />
///<reference path="thenable.ts" />
var App;
(function (App) {
    var async;
    (function (async) {
        "use strict";
        var customMaybePromise;
        function maybePromise(callback) {
            if (App.isFunc(customMaybePromise)) {
                return customMaybePromise(callback);
            }
            return App.thenable(callback);
        }
        function getTimeout(wait) {
            if (App.isNum(wait) && wait > 0) {
                return App.global().setTimeout;
            }
            else {
                return App.global().setImmediate || App.global().setTimeout;
            }
        }
        function hasThen(value) {
            return App.isFunc((value || {}).then);
        }
        /**
         * 指定時間後に成功の結果となるthenableを返します。
         */
        function timeout(wait, result) {
            if (result === void 0) { result = undefined; }
            var time = Math.max(App.isNum(wait) ? wait : 0, 0);
            return maybePromise(function (fulfill) {
                getTimeout(time)(function () {
                    fulfill(result);
                }, time);
            });
        }
        async.timeout = timeout;
        /**
         * 指定された値を利用して成功の結果となるthenableを返します。
         */
        function success(value, wait) {
            if (wait === void 0) { wait = 0; }
            var time = Math.max(App.isNum(wait) ? wait : 0, 0);
            return maybePromise(function (fulfill, reject) {
                getTimeout(time)(function () {
                    if (hasThen(value)) {
                        try {
                            value.then(fulfill, reject);
                        }
                        catch (e) {
                            reject(e);
                        }
                        return;
                    }
                    if (App.isFunc(value)) {
                        try {
                            fulfill(value());
                        }
                        catch (e) {
                            reject(e);
                        }
                    }
                    fulfill(value);
                }, time);
            });
        }
        async.success = success;
        /**
         * 指定された値を利用して失敗の結果となるthenableを返します。
         */
        function fail(value, wait) {
            if (wait === void 0) { wait = 0; }
            var time = Math.max(App.isNum(wait) ? wait : 0, 0);
            return maybePromise(function (fulfill, reject) {
                getTimeout(time)(function () {
                    if (App.isFunc(value)) {
                        try {
                            reject(value());
                        }
                        catch (e) {
                            reject(e);
                        }
                    }
                    reject(value);
                }, time);
            });
        }
        async.fail = fail;
        function prepare(value) {
            var isArray = App.isArray(value.successes);
            value.key = {
                successes: [],
                fails: []
            };
            for (var p in value.successes) {
                if (value.successes.hasOwnProperty(p)) {
                    value.key.successes.push(isArray ? parseInt(p, 10) : p);
                }
            }
            for (var p in value.fails) {
                if (value.fails.hasOwnProperty(p)) {
                    value.key.fails.push(isArray ? parseInt(p, 10) : p);
                }
            }
            return value;
        }
        function each(value, callback) {
            if (App.isUnusable(value)) {
                return;
            }
            if (App.isArray(value)) {
                var index = 0;
                var length_1 = value.length;
                for (index = 0; index < length_1; index++) {
                    callback(index, value[index]);
                }
            }
            else {
                for (var key in value) {
                    if (!value.hasOwnProperty(key)) {
                        continue;
                    }
                    callback(key, value[key]);
                }
            }
        }
        function all() {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i - 0] = arguments[_i];
            }
            var target;
            if (args.length < 2) {
                target = args[0];
            }
            else {
                target = args;
            }
            return maybePromise(function (fulfill, reject) {
                var deferreds = App.isArray(target) ? [] : {}, result = {
                    successes: App.isArray(target) ? [] : {},
                    fails: App.isArray(target) ? [] : {}
                }, hasReject = false, remaining = 0, updateDeferreds = function (value, key, isResolve) {
                    var res = isResolve ? result.successes : result.fails;
                    hasReject = hasReject ? true : (isResolve ? false : true);
                    res[key] = value;
                    if (!(--remaining)) {
                        if (hasReject) {
                            reject(prepare(result));
                        }
                        else {
                            fulfill(prepare(result));
                        }
                    }
                };
                each(target, function (key, item) {
                    remaining++;
                    deferreds[key] = App.async.success(item, 0);
                });
                if (remaining > 0) {
                    each(deferreds, function (key, item) {
                        item.then(function (value) {
                            updateDeferreds(value, key, true);
                        }, function (value) {
                            updateDeferreds(value, key, false);
                        });
                    });
                }
                else {
                    fulfill({
                        successes: {},
                        fails: {},
                        key: {
                            successes: [],
                            fails: []
                        }
                    });
                }
            });
        }
        async.all = all;
        /**
         * App.asyncで返されるPromiseの実装を設定します。
         */
        function setReturnPromise(promise) {
            customMaybePromise = promise;
        }
        async.setReturnPromise = setReturnPromise;
    })(async = App.async || (App.async = {}));
})(App || (App = {}));

/// <reference path="base.ts" />
var App;
(function (App) {
    var obj;
    (function (obj) {
        "use strict";
        function isProcTarget(value, deep) {
            if (deep === void 0) { deep = false; }
            return (App.isObj(value) || App.isArray(value)) && deep;
        }
        function mapFunc(value, callback, deep) {
            if (deep === void 0) { deep = false; }
            var val = value, target = value;
            if (App.isArray(value)) {
                return value.map(function (val, index) {
                    if (isProcTarget(val, deep)) {
                        return mapFunc(val, callback, deep);
                    }
                    else {
                        return callback(val, index, value);
                    }
                });
            }
            else if (App.isObj(value)) {
                var result = {};
                for (var _i = 0, _a = Object.keys(value); _i < _a.length; _i++) {
                    var key = _a[_i];
                    val = target[key];
                    if (isProcTarget(val, deep)) {
                        val = mapFunc(val, callback, deep);
                    }
                    else {
                        val = callback(val, key, value);
                    }
                    result[key] = val;
                }
                return result;
            }
            return val;
        }
        function map(target, callback, deep) {
            if (deep === void 0) { deep = false; }
            if (!App.isObj(target) && !App.isArray(target)) {
                return target;
            }
            return mapFunc(target, callback, deep);
        }
        obj.map = map;
        /**
         * 指定されたオブジェクトから指定されたプロパティのみ含んだ新しいオブジェクトを返します。
         * @param {Object} target - 抽出元のオブジェクト
         * @param {...String} args - 抽出するプロパティの可変長引数
         * @return {Object} 抽出されたプロパティを持つオブジェクト
         * @example
         * var source = {
         *     name: "John", age: 1, address: "kyoto"
         * };
         * var picked = App.obj.pick(source, "name", "age");
         * console.log(picked); // {name: "John", age: 1}
         */
        function pick(target) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            var result = {}, source = target;
            if (!App.isObj(target)) {
                return;
            }
            for (var _a = 0; _a < args.length; _a++) {
                var arg = args[_a];
                if (arg in target) {
                    result[arg] = source[arg];
                }
            }
            return result;
        }
        obj.pick = pick;
        /**
         * 指定されたオブジェクトから指定されたプロパティを除外した新しいオブジェクトを返します。
         * @param {Object} target - 除外元のオブジェクト
         * @param {...String} args - 除外するプロパティの可変長引数
         * @return {Object} 除外されたプロパティを含まないオブジェクト
         * @example
         * var source = {
         *     name: "John", age: 1, address: "kyoto"
         * };
         * var omitted = App.obj.omit(source, "name", "age");
         * console.log(omitted); // {address: "kyoto"}
         */
        function omit(target) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            var result = {}, source = target;
            if (!App.isObj(target)) {
                return;
            }
            for (var _a = 0, _b = Object.keys(target); _a < _b.length; _a++) {
                var key = _b[_a];
                if (args.indexOf(key) < 0) {
                    result[key] = source[key];
                }
            }
            return result;
        }
        obj.omit = omit;
        /**
         * 指定されたオブジェクトに第2引数以降で指定されたオブジェクトのプロパティが含まれない場合に、そのプロパティと値を設定します。
         * @param {Object} target - 追加元のオブジェクト
         * @param {...Object} args - 追加するプロパティを含むオブジェクトの可変長引数
         * @return {Object} 追加されたプロパティを含むオブジェクト
         * @example
         * var value = {a: 1, b: "2"}
         * App.obj.defaults(value, {
         *   a: 2,
         *   b: "3",
         *   c: true
         * }, {
         *   b: 4
         *   c: false,
         *   d: "123"
         * });
         * //{a:1, b:"2", c:true, d: "123"}
         */
        function defaults(target) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            var source = target;
            for (var _a = 0; _a < args.length; _a++) {
                var arg = args[_a];
                for (var p in arg) {
                    if (!(p in target) && arg.hasOwnProperty(p)) {
                        source[p] = arg[p];
                    }
                }
            }
            return target;
        }
        obj.defaults = defaults;
        /**
         * 指定されたオブジェクトに第2引数以降で指定されたオブジェクトのプロパティの値を上書きします。
         * @param {Object} target - 上書き先のオブジェクト
         * @param {...Object} args - 上書きするプロパティを含むオブジェクトの可変長引数
         * @return {Object} 上書きされたプロパティを含むオブジェクト
         * @example
         * var value = {a: 1, b: "2", e: "3"}
         * App.obj.mixin(value, {
         *   a: 2,
         *   b: "3",
         *   c: true
         * }, {
         *   b: 4
         *   d: "123"
         * });
         * //{a:2, b:4, c:true, d: "123", e: "3"}
         */
        function mixin(target) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            var def = target;
            if (typeof def !== "object" && !App.isFunc(def)) {
                def = {};
            }
            for (var _a = 0; _a < args.length; _a++) {
                var options = args[_a];
                for (var name_1 in options) {
                    var src = def[name_1];
                    var copy = options[name_1];
                    if (def === options) {
                        continue;
                    }
                    var copyIsArray = void 0;
                    if (copy && (App.isObj(copy) || (copyIsArray = App.isArray(copy)))) {
                        var clone = void 0;
                        if (copyIsArray) {
                            copyIsArray = false;
                            clone = src && App.isArray(src) ? src : [];
                        }
                        else {
                            clone = src && App.isObj(src) ? src : {};
                        }
                        def[name_1] = mixin(clone, copy);
                    }
                    else if (copy !== undefined) {
                        def[name_1] = copy;
                    }
                }
            }
            return def;
        }
        obj.mixin = mixin;
        /**
         * 指定されたオブジェクトの各プロパティに設定された値を取得します。
         */
        function values(target) {
            var results = [];
            for (var _i = 0, _a = Object.keys(target); _i < _a.length; _i++) {
                var key = _a[_i];
                results.push(key);
            }
            return results;
        }
        obj.values = values;
    })(obj = App.obj || (App.obj = {}));
})(App || (App = {}));

/// <reference path="base.ts" />
/// <reference path="obj.ts" />
var App;
(function (App) {
    "use strict";
    var defaultCulture = {
        "name": "",
        "engName": "Invariant Language (Invariant Country)",
        "lang": "iv",
        "dateTimeFormat": {
            "months": {
                "shortNames": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", ""],
                "names": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", ""]
            },
            "weekdays": {
                "shortNames": ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
                "names": ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]
            },
            "meridiem": {
                "ante": "AM",
                "post": "PM"
            },
            "patterns": {
                "d": "MM/dd/yyyy",
                "D": "dddd, dd MMMM yyyy",
                "t": "HH:mm",
                "T": "HH:mm:ss",
                "f": "dddd, dd MMMM yyyy HH:mm",
                "F": "dddd, dd MMMM yyyy HH:mm:ss",
                "M": "MMMM dd",
                "S": "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                "Y": "yyyy MMMM"
            },
            "dateSep": "/",
            "timeSep": ":",
            "twoDigitYearMax": 2029,
            "named": {
                "date": "yyyy/MM/dd",
                "month": "yyyy/MM",
                "time": "hh:mm"
            }
        },
        "numberFormat": {
            "decDigits": 2,
            "groupSep": ",",
            "decSep": ".",
            "groupSizes": [
                3
            ],
            "pattern": {
                "pos": "n",
                "neg": "-n"
            },
            "posSign": "+",
            "negSign": "-",
            "posInfSymbol": "Infinity",
            "negInfSymbol": "-Infinity",
            "nanSymbol": "NaN",
            "currency": {
                "symbol": "¤",
                "decDigits": 2,
                "groupSep": ",",
                "decSep": ".",
                "groupSizes": [
                    3
                ],
                "pattern": {
                    "pos": "$n",
                    "neg": "($n)"
                }
            },
            "percent": {
                "symbol": "%",
                "permilleSynbol": "‰",
                "decDigits": 2,
                "groupSep": ",",
                "decSep": ".",
                "groupSizes": [
                    3
                ],
                "pattern": {
                    "pos": "n %",
                    "neg": "-n %"
                }
            },
            "named": {
                "currency": "#,##0",
                "number": "#",
                "decimal": "#.00"
            }
        },
        text: {}
    }, currentCulture = "", cultures = {};
    function culture() {
        var args = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            args[_i - 0] = arguments[_i];
        }
        if (args.length === 0) {
            return cultures[currentCulture];
        }
        if (args.length === 1) {
            return cultures[args[0]];
        }
        if (args.length > 1) {
            var target = App.obj.mixin(true, {}, defaultCulture, cultures[args[0]] || {}, args[1]);
            target.name = args[0];
            cultures[args[0]] = target;
            return target;
        }
    }
    App.culture = culture;
    var culture;
    (function (culture) {
        function current() {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i - 0] = arguments[_i];
            }
            if (args.length > 0) {
                if (args[0] in cultures) {
                    currentCulture = args[0];
                }
            }
            return cultures[currentCulture];
        }
        culture.current = current;
    })(culture = App.culture || (App.culture = {}));
    App.culture(currentCulture, defaultCulture);
})(App || (App = {}));

/// <reference path="base.ts" />
/// <reference path="culture.ts" />
var App;
(function (App) {
    var str;
    (function (str) {
        "use strict";
        var isStr = App.isStr;
        /**
         * 指定された文字列を指定された回数文連結した文字列を返します。
         */
        function repeat(target, count) {
            var cnt;
            if (!App.isStr(target)) {
                return;
            }
            if (App.isNum(count)) {
                cnt = count;
            }
            else {
                cnt = parseInt((count || "").toString(), 10);
            }
            cnt = App.isNum(cnt) ? cnt < 0 ? 0 : Math.floor(cnt) : 0;
            if (cnt === 0) {
                return "";
            }
            return (new Array(cnt + 1)).join(target);
        }
        str.repeat = repeat;
        /**
         * 指定された文字列で正規表現の予約語にあたる文字列をエスケープ処理した文字列を返します。
         */
        function escapeRegExp(target) {
            if (!isStr(target)) {
                target = target + "";
            }
            return target.replace(/([\\/'*+?|()\[\]{}.^$])/g, "\\$1"); //eslint-disable-line no-irregular-whitespace
        }
        str.escapeRegExp = escapeRegExp;
        /**
         * 指定された文字列の先頭が指定された文字列もしくは正規表現と一致するかどうかをチェックします。
         */
        function startsWith(target, value, ignoreCase) {
            if (!isStr(target)) {
                return;
            }
            var source = App.isRegExp(value) ? value.source : escapeRegExp(value);
            ignoreCase = !App.isUndef(ignoreCase) ? !!ignoreCase :
                App.isRegExp(value) ? value.ignoreCase : false;
            source = (source.charAt(0) === "^" ? "" : "^") + source;
            return RegExp(source, ignoreCase ? "i" : "").test(target);
        }
        str.startsWith = startsWith;
        /**
         * 指定された文字列の末尾が指定された文字列もしくは正規表現と一致するかどうかをチェックします。
         */
        function endsWith(target, value, ignoreCase) {
            if (!isStr(target)) {
                return;
            }
            var source = App.isRegExp(value) ? value.source : escapeRegExp(value);
            ignoreCase = !App.isUndef(ignoreCase) ? !!ignoreCase :
                App.isRegExp(value) ? value.ignoreCase : false;
            source = source + (source.charAt(source.length - 1) === "$" ? "" : "$");
            return RegExp(source, ignoreCase ? "i" : "").test(target);
        }
        str.endsWith = endsWith;
        /**
        * 第1引数に指定された文字列のプレースホルダーを第2引数以降に指定された値で置き換えます。
        * example:
        * プレースホルダーがインデックスの数値の場合は、第2引数を0として開始した引数に指定された値を利用
        * App.str.format("{0} to {1}", 1, 10); // "1 to 10"
        * プレースホルダーが名称の場合は、第2引数に指定された値のプロパティに設定された値を利用
        * App.str.format("{min} to {max}", {min: 10, max: 20}); // "10 to 20"
        */
        function format(target) {
            var args = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                args[_i - 1] = arguments[_i];
            }
            if (!target) {
                return target;
            }
            if (args.length === 0) {
                return target;
            }
            return target.toString().replace(/\{?\{(.+?)\}\}?/g, function (match) {
                var matchdArgs = [];
                for (var _i = 1; _i < arguments.length; _i++) {
                    matchdArgs[_i - 1] = arguments[_i];
                }
                var param = matchdArgs[0];
                if (match.substr(0, 2) === "{{" && match.substr(match.length - 2) === "}}") {
                    return match.replace("{{", "{").replace("}}", "}");
                }
                var splitPos = Math.min(param.indexOf(".") === -1 ? param.length : param.indexOf("."), param.indexOf("[") === -1 ? param.length : param.indexOf("["));
                var prop;
                var rootProp;
                if (splitPos < param.length) {
                    rootProp = param.substr(0, splitPos);
                    prop = "['" + param.substr(0, splitPos) + "']" + param.substr(splitPos);
                }
                else {
                    rootProp = param;
                    prop = "['" + param + "']";
                }
                /*eslint-disable no-new-func */
                var val = (new Function("return arguments[0]" + prop + ";"))(App.isNumeric(rootProp) ?
                    ((App.isArray(args[0]) && args.length === 1) ? args[0] : args) :
                    args[0]);
                /*eslint-enable no-new-func */
                var strVal = App.isUndef(val) ? "" : (val + "");
                if (match.substr(0, 2) === "{{") {
                    strVal = "{" + strVal;
                }
                if (match.substr(match.length - 2) === "}}") {
                    strVal = strVal + "}";
                }
                return strVal;
            });
        }
        str.format = format;
        function text(key) {
            var def = App.culture.current().text || {}, text = (function (keyStr) {
                var val = def;
                for (var _i = 0, _a = keyStr.split("."); _i < _a.length; _i++) {
                    var keyVal = _a[_i];
                    val = val[keyVal];
                    if (!val) {
                        return "";
                    }
                }
                return val || "";
            })(key || ""), args;
            if (arguments.length === 1) {
                return text;
            }
            args = Array.prototype.slice.call(arguments);
            args.shift();
            args.unshift(text);
            return str.format.apply(null, args);
        }
        str.text = text;
        /**
         * 指定された文字列の先頭から指定された数までの文字列を返します。
         */
        function clipLeft(target, length) {
            if (!isStr(target)) {
                return;
            }
            length = App.isNum(length) ? length < 0 ? 0 : length : 0;
            if (target.length <= length) {
                return target;
            }
            return target.substr(0, length);
        }
        str.clipLeft = clipLeft;
        /**
         * 指定された文字列の末尾から指定された数までの文字列を返します。
         */
        function clipRight(target, length) {
            if (!isStr(target)) {
                return;
            }
            length = App.isNum(length) ? length < 0 ? 0 : length : 0;
            if (target.length <= length) {
                return target;
            }
            return target.substring(target.length - length, target.length);
        }
        str.clipRight = clipRight;
        /**
         * 指定された文字列の先頭に指定された文字数になるまで指定された文字を埋めます。
         */
        function padLeft(target, length, padChars) {
            if (!isStr(target)) {
                return;
            }
            var len;
            if (App.isNum(length)) {
                len = length;
            }
            if (App.isStr(length) && App.isNumeric(length)) {
                len = parseInt(length.toString(), 10);
            }
            len = App.isNum(len) ? len < 0 ? 0 : Math.floor(len) : 0;
            if (App.isUndef(padChars) || App.isNull(padChars)) {
                padChars = " ";
            }
            var margin = len - target.length;
            if (margin < 1) {
                return target;
            }
            var paddingChars = repeat(padChars.toString(), margin);
            return paddingChars.substr(0, margin) + target;
        }
        str.padLeft = padLeft;
        /**
         * 指定された文字列の末尾に指定された文字数になるまで指定された文字を埋めます。
         */
        function padRight(target, length, padChars) {
            if (!isStr(target)) {
                return;
            }
            var len;
            if (App.isNum(length)) {
                len = length;
            }
            if (App.isStr(length) && App.isNumeric(length)) {
                len = parseInt(length.toString(), 10);
            }
            len = App.isNum(len) ? len < 0 ? 0 : Math.floor(len) : 0;
            if (App.isUndef(padChars) || App.isNull(padChars)) {
                padChars = " ";
            }
            var margin = len - target.length;
            if (margin < 1) {
                return target;
            }
            var paddingChars = repeat(padChars.toString(), margin);
            return target + paddingChars.substr(0, margin);
        }
        str.padRight = padRight;
        /**
         * 指定された文字列の左側にある空白を削除します。
         */
        function trimLeft(target) {
            if (!isStr(target)) {
                return;
            }
            return target.replace(/^[\s　]+/, "");
        }
        str.trimLeft = trimLeft;
        /**
         * 指定された文字列の右側にある空白を削除します。
         */
        function trimRight(target) {
            if (!isStr(target)) {
                return;
            }
            return target.replace(/[\s　]+$/, "");
        }
        str.trimRight = trimRight;
        /**
         * 指定された文字列の両側にある空白を削除します。
         */
        function trim(target) {
            if (!isStr(target)) {
                return;
            }
            return target.replace(/^[\s　]+|[\s　]+$/g, "");
        }
        str.trim = trim;
        function surroundsWith(target, start, end, ignoreCase) {
            if (!isStr(target)) {
                return;
            }
            start = App.ifUndefOrNull(start, "");
            if (arguments.length === 3) {
                if (App.isBool(end)) {
                    ignoreCase = !!end;
                    end = void 0;
                }
            }
            var endVal = App.ifUndefOrNull(end, start);
            if (start.length + endVal.length > target.length) {
                return false;
            }
            return startsWith(target, start, ignoreCase) && endsWith(target, endVal, ignoreCase);
        }
        str.surroundsWith = surroundsWith;
        function surrounds(target, start, end, force) {
            var normalize = function (val) {
                if (App.isUndefOrNull(val)) {
                    val = "";
                }
                return val.toString();
            };
            if (!isStr(target)) {
                return;
            }
            start = normalize(start);
            if (arguments.length === 4) {
                end = normalize(end);
                force = !!force;
            }
            else if (arguments.length === 3) {
                if (App.isBool(end)) {
                    force = !!end;
                    end = start;
                }
                else {
                    force = false;
                    end = normalize(end);
                }
            }
            else if (arguments.length <= 2) {
                end = start;
                force = false;
            }
            if (startsWith(target, start, undefined) && !force) {
                start = "";
            }
            if (endsWith(target, end, undefined) && !force) {
                end = "";
            }
            return start + target + end;
        }
        str.surrounds = surrounds;
    })(str = App.str || (App.str = {}));
})(App || (App = {}));

/// <reference path="base.ts" />
var App;
(function (App) {
    var calendar;
    (function (calendar) {
        "use strict";
        function getTargetCalendarDefinition(definitions, target) {
            var i = definitions.length - 1, def, targetDef;
            for (; (i + 1); --i) {
                def = definitions[i];
                if (def.start <= target) {
                    targetDef = def;
                    break;
                }
            }
            if (targetDef) {
                return targetDef;
            }
        }
        function prepareDefitions(definitions) {
            var defs = App.isArray(definitions) ? definitions :
                !definitions ? [] : [definitions];
            return defs.map(function (item) {
                if (!App.isDate(item.start)) {
                    item.start = new Date(1, 0, 1, 0, 0, 0, 0);
                }
                return {
                    era: item.era || "",
                    shortEra: item.shortEra || "",
                    middleEra: item.middleEra || item.shortEra || "",
                    start: new Date(item.start.getTime()),
                    maxYearLength: App.isNum(item.maxYearLength) ?
                        Math.max(item.maxYearLength, 0) : undefined
                };
            });
        }
        var calendarPool = {};
        function create(name, definition) {
            //clone
            var defs = prepareDefitions(definition);
            var result = {
                getShortEra: function (target) {
                    var def = getTargetCalendarDefinition(defs, target);
                    if (def) {
                        return def.shortEra;
                    }
                },
                getMiddleEra: function (target) {
                    var def = getTargetCalendarDefinition(defs, target);
                    if (def) {
                        return def.middleEra;
                    }
                },
                getEra: function (target) {
                    var def = getTargetCalendarDefinition(defs, target);
                    if (def) {
                        return def.era;
                    }
                },
                getYear: function (target) {
                    var def = getTargetCalendarDefinition(defs, target);
                    if (def) {
                        return target.getFullYear() - def.start.getFullYear() + 1;
                    }
                },
                getEraInfo: function (target) {
                    var def = getTargetCalendarDefinition(defs, target);
                    if (def) {
                        return {
                            shortEra: def.shortEra,
                            middleEra: def.middleEra,
                            era: def.era,
                            year: target.getFullYear() - def.start.getFullYear() + 1,
                            start: new Date(def.start.getTime()),
                            maxYearLength: def.maxYearLength
                        };
                    }
                },
                getEras: function () {
                    return defs.map(function (item) {
                        return {
                            era: item.era,
                            shortEra: item.shortEra,
                            middleEra: item.middleEra,
                            start: new Date(item.start.getTime()),
                            maxYearLength: item.maxYearLength
                        };
                    });
                }
            };
            calendarPool[name] = function () {
                return result;
            };
            calendar[name] = function () {
                return result;
            };
            return result;
        }
        calendar.create = create;
        function get(name) {
            var calendarFunc = calendarPool[name];
            if (App.isFunc(calendarFunc)) {
                return calendarFunc();
            }
        }
        calendar.get = get;
        calendar.create("gregorianCalendar", [
            {
                era: "A.D.",
                shortEra: "AD",
                start: new Date(1, 0, 1, 0, 0, 0, 0)
            }
        ]);
        calendar.create("japaneseCalendar", [{
                era: "明治",
                middleEra: "明",
                shortEra: "M",
                start: new Date(1868, 0, 1, 0, 0, 0, 0),
                maxYearLength: 2
            }, {
                era: "大正",
                middleEra: "大",
                shortEra: "T",
                start: new Date(1912, 6, 30, 0, 0, 0, 0),
                maxYearLength: 2
            }, {
                era: "昭和",
                middleEra: "昭",
                shortEra: "S",
                start: new Date(1926, 11, 25, 0, 0, 0, 0),
                maxYearLength: 2
            }, {
                era: "平成",
                middleEra: "平",
                shortEra: "H",
                start: new Date(1989, 0, 8, 0, 0, 0, 0),
                maxYearLength: 2
            }]);
    })(calendar = App.calendar || (App.calendar = {}));
})(App || (App = {}));

///<reference path="base.ts" />
///<reference path="culture.ts" />
///<reference path="str.ts" />
///<reference path="calendar.ts" />
var App;
(function (App) {
    var date;
    (function (date_1) {
        "use strict";
        var defaultCalendar = App.calendar.get("gregorianCalendar"), proto = Date.prototype, getFullYear = proto.getFullYear, getMonth = proto.getMonth, getDate = proto.getDate, getHours = proto.getHours, getMinutes = proto.getMinutes, getSeconds = proto.getSeconds, getMilliseconds = proto.getMilliseconds, getTime = proto.getTime, getDay = proto.getDay, getUTCFullYear = proto.getUTCFullYear, getUTCMonth = proto.getUTCMonth, getUTCDate = proto.getUTCDate, getUTCHours = proto.getUTCHours, getUTCMinutes = proto.getUTCMinutes, getUTCSeconds = proto.getUTCSeconds, getUTCMilliseconds = proto.getUTCMilliseconds, getUTCDay = proto.getUTCDay, getTimezoneOffset = proto.getTimezoneOffset, setFullYear = proto.setFullYear, setMonth = proto.setMonth, setDate = proto.setDate, setHours = proto.setHours, setMinutes = proto.setMinutes, setSeconds = proto.setSeconds, setMilliseconds = proto.setMilliseconds, tokenCache = {}, formatterPool = {}, parserPool = {};
        //setTime = proto.setTime,
        var unitYear = "Year", unitMonth = "Month", unitDate = "Date", unitHour = "Hours", unitMinute = "Minutes", unitSecond = "Seconds", unitMillSecond = "Milliseconds", unitWeek = "Week", unitTime = "Time";
        function toInt(value) {
            if (App.isStr(value)) {
                return parseInt(value, 10);
            }
            else if (App.isNum(value)) {
                return Math.floor(value);
            }
            else {
                return parseInt(value, 10);
            }
        }
        function diff(target, unit, value) {
            if (App.isValidDate(target) && App.isStr(unit) && App.isValidDate(value)) {
                var fromTime = target.getTime(), toTime = value.getTime(), difference = toTime - fromTime;
                if (unit === unitMillSecond) {
                    difference = difference / 1;
                }
                else if (unit === unitSecond) {
                    difference = difference / 1000;
                }
                else if (unit === unitMinute) {
                    difference = difference / (1000 * 60);
                }
                else if (unit === unitHour) {
                    difference = difference / (1000 * 60 * 60);
                }
                else if (unit === unitDate) {
                    difference = difference / (1000 * 60 * 60 * 24);
                }
                else if (unit === unitWeek) {
                    difference = difference / (1000 * 60 * 60 * 24 * 7);
                }
                else if (unit === unitMonth) {
                    difference = ((value.getFullYear() - target.getFullYear()) * 12) - target.getMonth() + value.getMonth();
                }
                else if (unit === unitYear) {
                    difference = value.getFullYear() - target.getFullYear();
                }
                return Math.floor(difference);
            }
        }
        function add(target, unit, value) {
            var result;
            if (App.isValidDate(target) && App.isStr(unit) && App.isNumeric(value)) {
                var firstDayInMonth, addend;
                result = new Date(target.getTime());
                firstDayInMonth = new Date(getFullYear.call(result), getMonth.call(result), 1);
                addend = toInt(value);
                if (unit === unitYear) {
                    unit = "FullYear";
                }
                if (unit === unitWeek) {
                    setDate.call(result, getDate.call(result) + (addend * 7));
                }
                else {
                    result["set" + unit](result["get" + unit]() + addend);
                    if (unit === unitMonth) {
                        setMonth.call(firstDayInMonth, getMonth.call(firstDayInMonth) + addend);
                        if (getMonth.call(firstDayInMonth) !== getMonth.call(result)) {
                            setDate.call(result, 0);
                        }
                    }
                }
            }
            return result;
        }
        function dayOf(target, unit, start) {
            if (App.isValidDate(target) && App.isStr(unit)) {
                var newYear, diffDays_1;
                if (unit === unitYear) {
                    newYear = new Date(getFullYear.call(target), 0, 1);
                    return Math.floor((getTime.call(target) - getTime.call(newYear)) / (24 * 60 * 60 * 1000)) + 1;
                }
                if (unit === unitMonth) {
                    return getDate.call(target);
                }
                if (unit === unitWeek) {
                    var firstDayOfWeek = App.isNumeric(start) ?
                        Math.max(0, Math.min(toInt(start), 7)) : 0;
                    diffDays_1 = target.getDay() - firstDayOfWeek;
                    return diffDays_1 < 0 ? 7 + diffDays_1 :
                        diffDays_1 > 6 ? diffDays_1 - 6 :
                            diffDays_1;
                }
            }
        }
        function startOf(target, unit, start) {
            var result;
            if (App.isValidDate(target) && App.isStr(unit)) {
                var dayOfWeek_1 = 0, diffDays_2;
                result = new Date(target.getTime());
                if (unit === unitWeek) {
                    dayOfWeek_1 = dayOf(result, unitWeek, 0);
                    var firstDayOfWeek = App.isNumeric(start) ? Math.max(0, Math.min(toInt(start), 7)) : 0;
                    diffDays_2 = 0 - dayOfWeek_1 + firstDayOfWeek;
                    setDate.call(result, getDate.call(result) + (diffDays_2 > 0 ? diffDays_2 - 7 : diffDays_2));
                    unit = unitDate;
                }
                /*eslint-disable no-fallthrough */
                switch (unit) {
                    case unitYear:
                        setMonth.call(result, 0);
                    case unitMonth:
                        setDate.call(result, 1);
                    case unitDate:
                        setHours.call(result, 0);
                    case unitHour:
                        setMinutes.call(result, 0);
                    case unitMinute:
                        setSeconds.call(result, 0);
                    case unitSecond:
                        setMilliseconds.call(result, 0);
                }
            }
            return result;
        }
        function endOf(target, unit, start) {
            var result;
            if (App.isValidDate(target) && App.isStr(unit)) {
                result = startOf(target, unit, start);
                if (unit === unitWeek) {
                    setDate.call(result, getDate.call(result) + 7);
                }
                else {
                    result = add(result, unit, 1);
                }
                setMilliseconds.call(result, -1);
            }
            return result;
        }
        function tokenize(format) {
            var i, l, ch, tokens = [], escaped = false, token, quote;
            function tokenizeLiteral(target, index) {
                var match = format.substr(index).match(new RegExp(target + "+"));
                if (match) {
                    return match[0].length;
                }
                return 1;
            }
            if (tokenCache[format]) {
                return tokenCache[format];
            }
            for (i = 0, l = format.length; i < l; i++) {
                ch = format.charAt(i);
                if (escaped) {
                    tokens.push(ch);
                    escaped = false;
                    continue;
                }
                if (ch === "\\") {
                    escaped = true;
                    continue;
                }
                if (ch === "'" || ch === "\"") {
                    if (ch === quote) {
                        quote = false;
                    }
                    else {
                        quote = ch;
                    }
                    continue;
                }
                if (quote) {
                    tokens.push(ch);
                    continue;
                }
                switch (ch) {
                    case "d":
                    case "f":
                    case "h":
                    case "H":
                    case "m":
                    case "M":
                    case "s":
                    case "t":
                    case "y":
                    case "z":
                    case "g":
                        token = {
                            type: ch,
                            length: tokenizeLiteral(ch, i)
                        };
                        tokens.push(token);
                        i += (token.length - 1);
                        break;
                    case "/":
                    case ":":
                        token = {
                            type: ch
                        };
                        tokens.push(token);
                        break;
                    default:
                        tokens.push(ch);
                }
            }
            tokenCache[format] = tokens;
            return tokens;
        }
        function buildText(value, tokens, definition, useUtc, cal) {
            var tokenItem, meridiem, timezoneoffset, texts = [], clipRight = App.str.clipRight, i, l, eraValue, unusableToEmpty = function (val) {
                if (App.isUnusable(val)) {
                    return "";
                }
                return val;
            }, getNonMilitaryHour = function () {
                var hour = (useUtc ? getUTCHours : getHours).call(value);
                return hour > 12 ? hour - 12 :
                    (hour || 12);
            }, makeMd = function (len, division, addend, func, func2) {
                if (len === 1) {
                    texts.push(func.call(value) + addend);
                }
                if (len === 2) {
                    texts.push(clipRight("0" + (func.call(value) + addend), 2));
                }
                if (len >= 3) {
                    texts.push((len === 3 ? definition[division].shortNames : definition[division].names)[(func2 || func).call(value)]);
                }
            }, makeHhms = function (len, func) {
                if (len === 1) {
                    texts.push(func(value));
                }
                if (len >= 2) {
                    texts.push(clipRight("0" + func(value), 2));
                }
            };
            for (var _i = 0; _i < tokens.length; _i++) {
                var tokenItem_1 = tokens[_i];
                var type = void 0;
                var tlength = void 0;
                if (App.isStr(tokenItem_1)) {
                    texts.push(tokenItem_1);
                    continue;
                }
                else {
                    type = tokenItem_1.type;
                    tlength = tokenItem_1.length;
                }
                if (type === "/") {
                    texts.push(definition.dateSep);
                }
                else if (type === ":") {
                    texts.push(definition.timeSep);
                }
                else if (type === "g") {
                    eraValue = "";
                    if (tlength === 1) {
                        eraValue = unusableToEmpty((cal || defaultCalendar).getShortEra(value));
                    }
                    else if (tlength === 2) {
                        eraValue = unusableToEmpty((cal || defaultCalendar).getMiddleEra(value));
                    }
                    else {
                        eraValue = unusableToEmpty((cal || defaultCalendar).getEra(value));
                    }
                    //変換できない場合は終了
                    if (!eraValue) {
                        return;
                    }
                    texts.push(eraValue);
                }
                else if (type === "y") {
                    //実際は cal のほうに文字指定が何文字の場合は、何文字で出力するという制御が必要
                    if (cal) {
                        eraValue = unusableToEmpty(cal.getEraInfo(value));
                        //変換できない場合は終了
                        if (!eraValue) {
                            return;
                        }
                        texts.push(eraValue.maxYearLength ?
                            App.str.padLeft((eraValue.year + ""), Math.min(tlength, eraValue.maxYearLength), "0") :
                            (eraValue.year + ""));
                    }
                    else {
                        if (tlength < 3) {
                            texts.push(App.str.padLeft(parseInt(clipRight(((useUtc ? getUTCFullYear : getFullYear).call(value) + ""), 2), 10) + "", tlength, "0"));
                        }
                        else {
                            texts.push(App.str.padLeft((useUtc ? getUTCFullYear : getFullYear).call(value) + "", tlength, "0"));
                        }
                    }
                }
                else if (type === "M") {
                    makeMd(tlength, "months", 1, (useUtc ? getUTCMonth : getMonth));
                }
                else if (type === "d") {
                    makeMd(tlength, "weekdays", 0, (useUtc ? getUTCDate : getDate), (useUtc ? getUTCDay : getDay));
                }
                else if (type === "h") {
                    makeHhms(tlength, getNonMilitaryHour);
                }
                else if (type === "H") {
                    makeHhms(tlength, function (s) {
                        return (useUtc ? getUTCHours : getHours).call(s);
                    });
                }
                else if (type === "m") {
                    makeHhms(tlength, function (s) {
                        return (useUtc ? getUTCMinutes : getMinutes).call(s);
                    });
                }
                else if (type === "s") {
                    makeHhms(tlength, function (s) {
                        return (useUtc ? getUTCSeconds : getSeconds).call(s);
                    });
                }
                else if (type === "f") {
                    texts.push(clipRight("00" + (useUtc ? getUTCMilliseconds : getMilliseconds).call(value), 3).substr(0, tlength));
                }
                else if (type === "t") {
                    meridiem = tlength === 1 ? {
                        ante: definition.meridiem.ante[0],
                        post: definition.meridiem.post[0]
                    } : definition.meridiem;
                    texts.push((useUtc ? getUTCHours : getHours).call(value) > 12 ? meridiem.post : meridiem.ante);
                }
                else if (type === "z") {
                    timezoneoffset = useUtc ? 0 : getTimezoneOffset.call(value);
                    texts.push((timezoneoffset < 0 ? "+" : "-") + (function (len) {
                        if (len === 1) {
                            return Math.floor(Math.abs(timezoneoffset / 60)).toString();
                        }
                        else if (len === 2) {
                            return clipRight("0" + Math.floor(Math.abs(timezoneoffset / 60)).toString(), 2);
                        }
                        else {
                            return clipRight("0" + Math.floor(Math.abs(timezoneoffset / 60)).toString(), 2) +
                                ":" +
                                clipRight("0" + Math.floor(Math.abs(timezoneoffset % 60)).toString(), 2);
                        }
                    })(tlength));
                }
            }
            return texts.join("");
        }
        function toFormatedText(value, format, definition, useUtc, cal) {
            var tokens = tokenize(format);
            return buildText(value, tokens, definition, useUtc, cal);
        }
        function resolveTokenForParse(token, startPos, value, dateDef, definition, cal) {
            var tokenType = token.type, tokenLength = token.length;
            function zeroPaddingTwoLengthResolveToken(valPos) {
                var res, text, num, code;
                code = value.charCodeAt(startPos);
                if (code >= 48 && code <= 57) {
                    text = value.charAt(startPos);
                    res = 1;
                    code = value.charCodeAt(startPos + 1);
                    if (code >= 48 && code <= 57) {
                        text += value.charAt(startPos + 1);
                        res = 2;
                    }
                    num = parseInt(text, 10);
                    if (!isNaN(num)) {
                        dateDef.values[valPos] = num;
                        return res;
                    }
                }
            }
            function fixLengthResolveToken(valPos) {
                var match = value.substr(startPos).match(new RegExp("^\\d{" + tokenLength + "}"));
                if (match && match.length > 0) {
                    dateDef.values[valPos] = parseInt(match[0], 10);
                    return token.length;
                }
            }
            function millisecondsResolveToken() {
                var text = "0." + value.substr(startPos, tokenLength);
                if (App.isNumeric(text)) {
                    dateDef.values[6] = parseFloat(text) * 1000;
                    return token.length;
                }
            }
            function makeMd(index, key, matchName) {
                if (tokenLength === 1) {
                    return zeroPaddingTwoLengthResolveToken(index);
                }
                else if (tokenLength === 2) {
                    return fixLengthResolveToken(index);
                }
                else {
                    var names = tokenLength === 3 ? definition[key].shortNames : definition[key].names;
                    for (var i = 0, l = names.length; i < l; i++) {
                        var target = value.substr(startPos, names[i].length).toUpperCase();
                        if (target === names[i].toUpperCase()) {
                            matchName(i);
                            return names[i].length;
                        }
                    }
                }
            }
            function makeHhms(index) {
                if (tokenLength === 1) {
                    return zeroPaddingTwoLengthResolveToken(index);
                }
                else {
                    return fixLengthResolveToken(index);
                }
            }
            if (tokenType === "y") {
                if (tokenLength === 1 || tokenLength === 2) {
                    var result;
                    if (tokenLength === 1) {
                        result = zeroPaddingTwoLengthResolveToken(0);
                    }
                    else {
                        result = fixLengthResolveToken(0);
                    }
                    if (result && !cal) {
                        dateDef.values[0] += (dateDef.values[0] <= (definition.twoDigitYearMax % 100) ? 2000 : 1900);
                    }
                    return result;
                }
                else {
                    return fixLengthResolveToken(0);
                }
            }
            if (tokenType === "M") {
                return makeMd(1, "months", function (index) {
                    dateDef.values[1] = index + 1;
                });
            }
            if (tokenType === "d") {
                return makeMd(2, "weekdays", function (index) {
                    dateDef.weekday = index;
                });
            }
            if (tokenType === "h") {
                dateDef.militaryTime = false;
                return makeHhms(3);
            }
            if (tokenType === "H") {
                dateDef.militaryTime = true;
                return makeHhms(3);
            }
            if (tokenType === "m") {
                return makeHhms(4);
            }
            if (tokenType === "s") {
                return makeHhms(5);
            }
            if (tokenType === "f") {
                return millisecondsResolveToken();
            }
            if (tokenType === "t") {
                var meridiemVal = tokenLength === 1 ? {
                    ante: definition.meridiem.ante[0],
                    post: definition.meridiem.post[0]
                } : definition.meridiem;
                var target = value.substr(startPos, meridiemVal.ante.length).toUpperCase();
                if (target === meridiemVal.ante.toUpperCase()) {
                    dateDef.anteMeridiem = true;
                    return meridiemVal.ante.length;
                }
                target = value.substr(startPos, meridiemVal.post.length).toUpperCase();
                if (target === meridiemVal.post.toUpperCase()) {
                    dateDef.anteMeridiem = false;
                    return meridiemVal.post.length;
                }
            }
            if (tokenType === "z") {
                var target = value.substr(startPos, 1);
                var sign = 0;
                if (target === "+") {
                    sign = 1;
                }
                else if (target === "-") {
                    sign = -1;
                }
                else {
                    return;
                }
                if (tokenLength <= 2) {
                    var match = value.substr(startPos + 1, 2).match(tokenLength === 1 ? /\d{1,2}/ : /\d{2}/);
                    if (match) {
                        dateDef.timeoffset = parseInt(target[0], 10) * 60 * sign;
                        return match[0].length + 1;
                    }
                }
                else {
                    var match = value.substr(startPos + 1, 5).match(/(\d{1,2}):(\d{2})/);
                    if (!target) {
                        match = value.substr(startPos + 1, 4).match(/(\d{2})(\d{2})/);
                    }
                    if (match && parseInt(match[2], 10) < 60) {
                        dateDef.timeoffset = ((parseInt(match[1], 10) * 60) + parseInt(match[2], 10)) * sign;
                        return match[0].length + 1;
                    }
                }
            }
            if (tokenType === "g") {
                var eraType;
                if (tokenLength === 1) {
                    eraType = "shortEra";
                }
                else if (tokenLength === 2) {
                    eraType = "middleEra";
                }
                else {
                    eraType = "era";
                }
                var era = App.array.find((cal || defaultCalendar).getEras(), function (e) {
                    return (value.substr(startPos, e[eraType].length) || "").toUpperCase() === e[eraType].toUpperCase();
                });
                if (era) {
                    dateDef.era = era;
                    return era[eraType].length;
                }
                return;
            }
        }
        function makeDateFromArray(array, militaryTime, anteMeridiem, weekday, timeoffset, era, cal) {
            var now = new Date(), hour = array[3], year = array[0], currentEra;
            //全て数値じゃないとNG
            if (!array.every(function (item) {
                return !App.isUnusable(item) && App.isNum(item);
            })) {
                return;
            }
            //全て0未満だとNG
            if (!array.some(function (item) {
                return App.isNum(item) && item > -1;
            })) {
                return;
            }
            //eraの調整
            if (cal) {
                if (era && year < 0) {
                    //年号があって年がない場合
                    //今日が歴の何年にあたるかを取得して利用
                    currentEra = cal.getEraInfo(now);
                    year = now.getFullYear() - currentEra.start.getFullYear() + 1;
                }
                else if (!era && year > -1) {
                    //年号がなくて、年がある場合
                    //現在の年号をデフォルトとする
                    era = cal.getEraInfo(now);
                }
                //西暦を取り出す
                year = array[0] = (era.start.getFullYear() + year - 1);
            }
            //AM/PMが付与されてる場合で13時以上や時間未設定はNG
            if (!App.isUndef(anteMeridiem)) {
                if (hour > 12 || hour < 0) {
                    return;
                }
            }
            //12時間制で
            if (!App.isUndef(militaryTime) && !militaryTime && hour > -1) {
                //13時とかはNG
                if (hour > 12) {
                    return;
                }
                //AMの時の12時は0時
                if ((App.isUndef(anteMeridiem) || (!App.isUndef(anteMeridiem) && anteMeridiem)) && hour === 12) {
                    array[3] = hour = 0;
                }
                //PMの時の0-11時は+12時間
                if (!App.isUndef(anteMeridiem) && !anteMeridiem && hour < 12) {
                    array[3] = hour = (hour + 12);
                }
            }
            //24時間制で
            if (!App.isUndef(militaryTime) && militaryTime && hour > -1) {
                //午前が指定されている場合に12時以上はNG
                if (!App.isUndef(anteMeridiem) && anteMeridiem) {
                    if (hour >= 12) {
                        return;
                    }
                }
                //午後が指定されている場合に11時以前はNG
                if (!App.isUndef(anteMeridiem) && !anteMeridiem) {
                    if (hour < 12) {
                        return;
                    }
                }
            }
            //時間しか指定されてない場合は現在日付
            /*eslint-disable curly */
            if (array[0] < 0 && array[1] < 0 && array[2] < 0) {
                if (array[0] < 0)
                    array[0] = getFullYear.call(now);
                if (array[1] < 0)
                    array[1] = getMonth.call(now);
                if (array[2] < 0)
                    array[2] = getDate.call(now);
            }
            else {
                if (array[0] < 0)
                    array[0] = getFullYear.call(now);
                if (array[1] < 0)
                    array[1] = 0;
                if (array[2] < 0)
                    array[2] = 1;
            }
            if (array[3] < 0)
                array[3] = 0;
            if (array[4] < 0)
                array[4] = 0;
            if (array[5] < 0)
                array[5] = 0;
            if (array[6] < 0)
                array[6] = 0;
            /*eslint-enable curly */
            var result = new Date();
            setFullYear.call(result, array[0], array[1], array[2]);
            setHours.call(result, array[3], array[4], array[5], array[6]);
            if (getFullYear.call(result) === array[0] &&
                getMonth.call(result) === array[1] &&
                getDate.call(result) === array[2] &&
                getHours.call(result) === array[3] &&
                getMinutes.call(result) === array[4] &&
                getSeconds.call(result) === array[5] &&
                getMilliseconds.call(result) === array[6]) {
                //曜日が指定されていて日付と一致していない場合はNG
                if (!App.isUndef(weekday)) {
                    if (result.getDay() !== weekday) {
                        return;
                    }
                }
                //eraの調整
                if (cal) {
                    //年号と年が一致していない場合はNG
                    if (cal.getEraInfo(result).era !== era.era) {
                        return;
                    }
                }
                //timezoneの時刻調整
                if (!App.isUndef(timeoffset)) {
                    setMinutes.call(result, getMinutes.call(result) + 0 - timeoffset - now.getTimezoneOffset());
                }
                return result;
            }
        }
        function parseText(value, format, definition, cal) {
            var tokens = tokenize(format), dateDef = {
                values: [-1, -1, -1, -1, -1, -1, -1]
            }, lastValueEndPos = 0, resolveLength = 0, sepLen;
            for (var _i = 0; _i < tokens.length; _i++) {
                var token = tokens[_i];
                resolveLength = undefined;
                if (App.isStr(token)) {
                    if (value.substr(lastValueEndPos, token.length) === token) {
                        resolveLength = token.length;
                    }
                }
                else {
                    if (token.type === "/") {
                        sepLen = definition.dateSep.length;
                        if (value.substr(lastValueEndPos, sepLen) === definition.dateSep) {
                            resolveLength = sepLen;
                        }
                    }
                    else if (token.type === ":") {
                        sepLen = definition.timeSep.length;
                        if (value.substr(lastValueEndPos, sepLen) === definition.timeSep) {
                            resolveLength = sepLen;
                        }
                    }
                    else {
                        resolveLength = resolveTokenForParse(token, lastValueEndPos, value, dateDef, definition, cal);
                    }
                }
                if (App.isUndef(resolveLength)) {
                    dateDef = null;
                    break;
                }
                lastValueEndPos += resolveLength;
            }
            if (!dateDef) {
                return;
            }
            //文字のほうが余っている場合はエラー
            if (value.length !== lastValueEndPos) {
                return;
            }
            //月が0の場合はエラー
            if (dateDef.values[1] === 0) {
                return;
            }
            else if (dateDef.values[1] >= 1) {
                dateDef.values[1] -= 1;
            }
            return makeDateFromArray(dateDef.values, dateDef.militaryTime, dateDef.anteMeridiem, dateDef.weekday, dateDef.timeoffset, dateDef.era, cal);
        }
        function parse(text, format, cal) {
            if (App.isArray(format)) {
                return parseAll(text, format, cal);
            }
            if (!App.isStr(text) || !App.isStr(format)) {
                return;
            }
            var formatString = format + "";
            var def = App.culture.current().dateTimeFormat;
            if (formatString in parserPool) {
                return parserPool[formatString](text, def);
            }
            if (formatString.length === 1) {
                //標準書式指定に設定されているパターンの場合は、そちらに変更するが
                //含まれない場合は、そのままの文字列をフォーマットとする
                //.NET場合はエラーとなる
                if (formatString in def.patterns) {
                    formatString = def.patterns[formatString];
                }
            }
            return parseText(text, formatString, def, cal);
        }
        date_1.parse = parse;
        function parseAll(text, formats, cal) {
            for (var _i = 0; _i < formats.length; _i++) {
                var format_1 = formats[_i];
                var result = parse(text, format_1, cal);
                if (result) {
                    return result;
                }
            }
        }
        //App.date = {
        /**
        * 第1引数に指定された日付を第2引数で指定されたフォーマット文字列でフォーマットした文字列を返します。
        * 基本的には.NETのフォーマット文字列に準拠します。
        * y, M, d, H, h, m, s, f, t, z が利用可能です。
        * http://msdn.microsoft.com/ja-jp/library/vstudio/8kb3ddd4.aspx
        *
        * javascript の場合、 f のミリ秒が3桁までになるため、 3つ以上の f の連続は無視されます。
        */
        function format(value, format, useUtc) {
            var utc = false, cal;
            if (arguments.length > 2) {
                if (App.isBool(useUtc)) {
                    utc = useUtc;
                }
                else {
                    cal = useUtc;
                }
            }
            if (!App.isValidDate(value) || !App.isStr(format)) {
                return;
            }
            var def = App.culture.current().dateTimeFormat;
            if (format in formatterPool) {
                return formatterPool[format](value, def, utc);
            }
            if (format.length === 1) {
                //標準書式指定に設定されているパターンの場合は、そちらに変更するが
                //含まれない場合は、そのままの文字列をフォーマットとする
                //.NET場合はエラーとなる
                if (format in def.patterns) {
                    format = def.patterns[format];
                }
            }
            return toFormatedText(value, format, def, utc, cal);
        }
        date_1.format = format;
        function namedFormatter(name, formatter) {
            if (!App.isStr(name)) {
                return;
            }
            if (App.isFunc(formatter)) {
                formatterPool[name] = formatter;
            }
            else {
                return formatterPool[name];
            }
        }
        date_1.namedFormatter = namedFormatter;
        /**
         * 名前付きフォーマッターを削除します。
         */
        function removeNamedFormatter(name) {
            formatterPool[name] = void 0;
            delete formatterPool[name];
        }
        date_1.removeNamedFormatter = removeNamedFormatter;
        /**
         * 名前付きフォーマッターが登録されているかどうかを取得します。
         */
        function hasNamedFormatter(name) {
            return App.isFunc(formatterPool[name]);
        }
        date_1.hasNamedFormatter = hasNamedFormatter;
        function namedParser(name, parser) {
            if (!App.isStr(name)) {
                return;
            }
            if (App.isFunc(parser)) {
                parserPool[name] = parser;
            }
            else {
                return parserPool[name];
            }
        }
        date_1.namedParser = namedParser;
        /**
         * 名前付きパーサーを削除します。
         */
        function removeNamedParser(name) {
            parserPool[name] = void 0;
            delete parserPool[name];
        }
        date_1.removeNamedParser = removeNamedParser;
        /**
         * 名前付きパーサーが登録されているかどうかを取得します。
         */
        function hasNamedParser(name) {
            return App.isFunc(parserPool[name]);
        }
        date_1.hasNamedParser = hasNamedParser;
        /**
         * 日時をコピーます。
         */
        function copy(dest, source) {
            if (!App.isValidDate(dest) || !App.isValidDate(source)) {
                return;
            }
            dest.setUTCMilliseconds(source.getUTCMilliseconds());
            dest.setUTCSeconds(source.getSeconds());
            dest.setUTCMinutes(source.getUTCMinutes());
            dest.setUTCHours(source.getUTCHours());
            dest.setUTCDate(source.getUTCDate());
            dest.setUTCMonth(source.getUTCMonth());
            dest.setUTCFullYear(source.getUTCFullYear());
            return dest;
        }
        date_1.copy = copy;
        [unitYear, unitMonth, unitDate, unitHour, unitMinute, unitSecond, unitMillSecond, unitWeek, unitTime].forEach(function (item) {
            var name = item.charAt(item.length - 1) !== "s" ? (item + "s") : item;
            name = item === unitDate ? "Days" : name;
            App.date["add" + name] = function (target, value) {
                return add(target, item, value);
            };
        });
        [unitYear, unitMonth, unitDate, unitHour, unitMinute, unitSecond, unitMillSecond, unitWeek].forEach(function (item) {
            var name = item.charAt(item.length - 1) !== "s" ? (item + "s") : item;
            name = item === unitDate ? "Days" : name;
            App.date["diff" + name] = function (target, value) {
                return diff(target, item, value);
            };
        });
        [unitYear, unitMonth, unitWeek].forEach(function (item) {
            App.date["dayOf" + item] = function (target, start) {
                return dayOf(target, item, start);
            };
        });
        [unitYear, unitMonth, unitDate, unitHour, unitMinute, unitSecond, unitWeek].forEach(function (item) {
            var name = item.charAt(item.length - 1) === "s" ? item.substr(0, item.length - 1) : item;
            name = item === unitDate ? "Day" : name;
            App.date["startOf" + name] = function (target, start) {
                return startOf(target, item, start);
            };
            App.date["endOf" + name] = function (target, start) {
                return endOf(target, item, start);
            };
        });
        [unitYear, unitMonth].forEach(function (item) {
            App.date["isLastDayOf" + item] = function (target) {
                var clone;
                if (!App.isValidDate(target)) {
                    return;
                }
                clone = add(target, unitDate, 1);
                if (item === unitYear) {
                    return clone.getFullYear() !== target.getFullYear();
                }
                else {
                    return clone.getMonth() !== target.getMonth();
                }
            };
        });
        namedFormatter("ODataJSON", function (value, def, useUtc) {
            var tzo = getTimezoneOffset.call(value), tzoText = "+0";
            if (!useUtc) {
                tzoText = App.str.clipRight("0" + Math.abs((Math.floor(tzo / 60))), 2) +
                    App.str.clipRight("0" + (tzo % 60), 2);
                tzoText = (tzo < 0 ? "-" : "+") + tzoText;
            }
            return "/Date(" + getTime.call(value) + tzoText + ")/";
        });
        function iso8601DateTime(value, useUtc, withMs) {
            var year = (useUtc ? getUTCFullYear : getFullYear).call(value) + "", month = ((useUtc ? getUTCMonth : getMonth).call(value) + 1) + "", date = (useUtc ? getUTCDate : getDate).call(value) + "", hour = (useUtc ? getUTCHours : getHours).call(value) + "", minute = (useUtc ? getUTCMinutes : getMinutes).call(value) + "", second = (useUtc ? getUTCSeconds : getSeconds).call(value) + "", zeroPad = function (v) {
                return ("00" + v).match(/.{0,2}$/)[0];
            };
            return year + "-" + zeroPad(month) + "-" + zeroPad(date) + "T" + zeroPad(hour) + ":" + zeroPad(minute) + ":" + zeroPad(second) +
                (withMs ? ("." + (value.getMilliseconds() + "000").substr(0, 3)) : "");
        }
        namedFormatter("ISO8601DateTime", function (value, def, useUtc) {
            return iso8601DateTime(value, useUtc); //eslint-disable-line new-cap
        });
        namedFormatter("ISO8601DateTimeMs", function (value, def, useUtc) {
            return iso8601DateTime(value, useUtc, true);
        });
        namedFormatter("ISO8601Full", function (value, def, useUtc) {
            var dateTime = iso8601DateTime(value, useUtc, true), zeroPad = function (v) {
                return ("00" + v).match(/.{0,2}$/)[0];
            }, tzoff = useUtc ? 0 : getTimezoneOffset.call(value), tz = (tzoff > 0 ? "-" : "+") + (function () {
                var tzo = Math.abs(tzoff);
                return zeroPad(Math.floor(tzo / 60).toString()) +
                    ":" +
                    zeroPad(Math.floor(tzo % 60).toString());
            })();
            return dateTime + tz;
        });
        namedParser("ISO8601DateTime", function (value, def) {
            return parse(value, "yyyy-MM-dd'T'HH:mm:ss");
        });
        namedParser("ISO8601DateTimeMs", function (value, def) {
            return parse(value, "yyyy-MM-ddTHH:mm:ss.fff");
        });
        namedParser("ISO8601Full", function (value, def) {
            return parse(value, "yyyy-MM-dd'T'HH:mm:sszzz");
        });
        //ISO8601DateTimeのエイリアス
        namedParser("JsonDate", function (value, def) {
            var val = parse(value, "yyyy-MM-dd'T'HH:mm:ss");
            if (!val) {
                val = parse(value, "yyyy-MM-dd'T'HH:mm:sszzz");
            }
            return val;
        });
        namedFormatter("date", function (value, def, useUtc) {
            return format(value, "yyyy/MM/dd", useUtc);
        });
        namedParser("date", function (value, def) {
            return parse(value, "yyyy/MM/dd");
        });
    })(date = App.date || (App.date = {}));
})(App || (App = {}));
;

/// <reference path="base.ts" />
/// <reference path="str.ts" />
var App;
(function (App) {
    var logging;
    (function (logging) {
        "use strict";
        var global = App.global();
        /**
         * Default log output for console.(> Internet Explorer 9).
         */
        function outputToConsole(logEntry) {
            if (global.console && global.console.log) {
                global.console.log(logEntry.text);
            }
        }
        var Logger = (function () {
            function Logger(moduleName, output, logFormat) {
                this.moduleName = moduleName;
                this.output = output;
                this.logFormat = logFormat;
                if (App.isUndefOrNull(logFormat)) {
                    this.logFormat = "[{longdate}] [{module}] [{level}], {message}";
                }
                if (App.isUndefOrNull(output)) {
                    this.output = outputToConsole;
                }
            }
            Logger.prototype.debug = function (message) {
                var logEntry = {
                    level: "DEBUG",
                    message: message
                };
                this.write(logEntry);
            };
            ;
            Logger.prototype.info = function (message) {
                var logEntry = {
                    level: "INFO",
                    message: message
                };
                this.write(logEntry);
            };
            ;
            Logger.prototype.error = function (message) {
                var logEntry = {
                    level: "ERROR",
                    message: message
                };
                this.write(logEntry);
            };
            ;
            Logger.prototype.write = function (logEntry) {
                //logEntry.longdate = App.date.format(new Date(), "yyyy-MM-dd hh:mm:ss");
                logEntry.module = this.moduleName;
                logEntry.format = this.logFormat;
                logEntry.text = App.str.format(this.logFormat, logEntry);
                this.output(logEntry);
            };
            return Logger;
        })();
        logging.Logger = Logger;
    })(logging = App.logging || (App.logging = {}));
})(App || (App = {}));

///<reference path="base.ts" />
///<reference path="culture.ts" />
///<reference path="str.ts" />
//number
var App;
(function (App) {
    var num;
    (function (num) {
        "use strict";
        var tokenCache = {}, formatterPool = {}, parserPool = {};
        var math = Math;
        var numRegx = /^[+-]?[\d][\d,]*(\.\d+)?(e[+-]\d+)?$/i;
        var decRegx = /^[+-]?(\.\d+)(e[+-]\d+)?$/i;
        function tokenize(format) {
            var escaped = false, quote, result = {
                plus: {
                    ints: [],
                    percent: 0,
                    permil: 0
                }
            }, current = result.plus.ints, section = result.plus, sectionPos = 0, sectionName, isInts = true;
            function prepareSeparator(sec) {
                var index = 0, token, holder = false, separator = false, ints = sec.ints;
                /*eslint-disable no-loop-func */
                while ((function () {
                    var val = ints[ints.length - 1];
                    if (!!val && !App.isStr(val) && val.type === ",") {
                        sec.pows = (sec.pows || 0) + 1;
                        ints.pop();
                        return true;
                    }
                    return false;
                })()) { } //eslint-disable-line no-empty
                /*eslint-enable no-loop-func */
                while (index < ints.length) {
                    var val = ints[index];
                    if (!App.isStr(val)) {
                        if (val.holder) {
                            if (holder && separator) {
                                sec.separate = true;
                            }
                            holder = true;
                        }
                        else if (val.separator) {
                            separator = true;
                            ints.splice(index, 1);
                            index--;
                        }
                        else {
                            holder = false;
                            separator = false;
                        }
                    }
                    index++;
                }
            }
            if (tokenCache[format]) {
                return tokenCache[format];
            }
            for (var _i = 0, _a = (format || "").split(""); _i < _a.length; _i++) {
                var c = _a[_i];
                if (escaped) {
                    current.push(c);
                    escaped = false;
                }
                else if (c === "\\") {
                    escaped = true;
                }
                else if (c === "\"" || c === "'") {
                    if (quote === c) {
                        quote = undefined;
                    }
                    else {
                        quote = c;
                    }
                }
                else if (quote) {
                    current.push(c);
                }
                else if (c === "0" || c === "#") {
                    current.push({
                        type: c,
                        holder: true
                    });
                }
                else if (c === ",") {
                    if (isInts) {
                        current.push({
                            type: c,
                            separator: true
                        });
                    }
                }
                else if (c === "%" || c === "‰") {
                    //current.push(c);
                    current.push({
                        type: c
                    });
                    if (c === "%") {
                        section.percent = (section.percent || 0) + 1;
                    }
                    else if (c === "‰") {
                        section.permil = (section.permil || 0) + 1;
                    }
                }
                else if (c === ".") {
                    if (isInts) {
                        isInts = false;
                        current = [];
                        result[(sectionPos === 0) ? "plus" : (sectionPos === 1 ? "minus" : "zero")].decs = current;
                    }
                }
                else if (c === ";") {
                    prepareSeparator(section);
                    isInts = true;
                    if (sectionPos < 2) {
                        sectionName = sectionPos === 0 ? "minus" : "zero";
                        result[sectionName] = { ints: [], percent: 0, permil: 0 };
                        section = result[sectionName];
                        current = section.ints;
                        sectionPos++;
                    }
                    else {
                        break;
                    }
                }
                else {
                    current.push(c);
                }
            }
            prepareSeparator(section);
            tokenCache[format] = result;
            return result;
        }
        function buildText(value, tokens, definition) {
            var isMinus = value < 0, targetVal = value, targetStr, i, j, l, result = { ints: "", decs: "" }, hasDec = false, groupSizes = definition.groupSizes.concat(), lastGroupSize = 0, groupLen = 0;
            function hasToken(v) {
                if (!v) {
                    return;
                }
                if (v.ints && v.ints.length) {
                    return v;
                }
                return;
            }
            var targetTokens = isMinus ? (hasToken(tokens.minus) || tokens.plus) : tokens.plus;
            function hasDecPlaceholder(v) {
                if (!v) {
                    return false;
                }
                for (var _i = 0; _i < v.length; _i++) {
                    var val = v[_i];
                    if (!App.isStr(val)) {
                        if (val.holder) {
                            return true;
                        }
                    }
                }
                return false;
            }
            function splitPart(val) {
                var splited = val.split(".");
                if (splited.length === 1) {
                    splited[1] = "";
                }
                return splited;
            }
            function exponentToNumStr(val) {
                var ePos = val.indexOf("e");
                if (ePos < 0) {
                    return val;
                }
                var mantissa = val.substr(0, ePos);
                var exponent = parseInt(val.substr(ePos + 1), 10);
                if (val.charAt(0) === "-") {
                    mantissa = mantissa[1];
                }
                var dotPos = mantissa.indexOf(".");
                mantissa = mantissa.replace(".", "");
                if (dotPos < 0) {
                    dotPos = mantissa.length;
                }
                dotPos = dotPos + exponent;
                if (dotPos <= 0) {
                    return "0." + (new Array(math.abs(dotPos))).join("0") + mantissa;
                }
                else if (mantissa.length <= dotPos) {
                    return mantissa + (new Array(dotPos - mantissa.length + 1)).join("0");
                }
                return mantissa.substr(0, dotPos) + "." + mantissa.substr(dotPos);
            }
            function hasAllStrTokenPart(val, length) {
                var index = 0;
                for (; index < length; index++) {
                    if (!App.isStr(val[index])) {
                        return false;
                    }
                }
                return true;
            }
            if (targetTokens.percent > 0) {
                targetVal = targetVal * math.pow(100, targetTokens.percent);
            }
            if (targetTokens.permil > 0) {
                targetVal = targetVal * math.pow(1000, targetTokens.permil);
            }
            if (targetTokens.pows > 0) {
                targetVal = targetVal / math.pow(1000, targetTokens.pows);
            }
            targetStr = exponentToNumStr(math.abs(targetVal) + "");
            targetVal = parseFloat(targetStr);
            if (targetVal === 0) {
                targetTokens = hasToken(tokens.zero) || targetTokens;
                targetStr = "0";
            }
            else if (targetVal < 1 && !hasDecPlaceholder(targetTokens.decs)) {
                targetTokens = hasToken(tokens.zero) || targetTokens;
                targetStr = "0";
            }
            var part = splitPart(targetStr);
            var tokenPart = targetTokens.ints;
            var targetPart = part[0];
            hasDec = false;
            lastGroupSize = groupSizes.shift();
            groupLen = 0;
            for (i = tokenPart.length - 1; i > -1; i--) {
                var tokenPartVal = tokenPart[i];
                if (App.isStr(tokenPartVal)) {
                    result.ints = tokenPartVal + result.ints;
                }
                else {
                    if (tokenPartVal.type === "%") {
                        result.ints = definition.percent.symbol + result.ints;
                        continue;
                    }
                    if (tokenPartVal.type === "‰") {
                        result.ints = definition.percent.permilleSynbol + result.ints;
                        continue;
                    }
                    if (targetTokens.separate) {
                        if (groupLen === lastGroupSize && targetPart.length !== 0) {
                            result.ints = definition.groupSep + result.ints;
                            if (groupSizes.length > 0) {
                                lastGroupSize = groupSizes.shift();
                            }
                            groupLen = 0;
                        }
                    }
                    result.ints = (targetPart.length !== 0 ? targetPart.substr(targetPart.length - 1) :
                        tokenPartVal.type === "0" ? "0" : "") + result.ints;
                    targetPart = targetPart.substr(0, targetPart.length - 1);
                    groupLen++;
                    if (hasAllStrTokenPart(tokenPart, i)) {
                        if (targetTokens.separate) {
                            for (j = targetPart.length; j; j--) {
                                if (groupLen === lastGroupSize && targetPart.length !== 0) {
                                    result.ints = definition.groupSep + result.ints;
                                    if (groupSizes.length > 0) {
                                        lastGroupSize = groupSizes.shift();
                                    }
                                    groupLen = 0;
                                }
                                result.ints = targetPart.charAt(j - 1) + result.ints;
                                targetPart = targetPart.substr(0, targetPart.length - 1);
                                groupLen++;
                            }
                        }
                        else {
                            result.ints = targetPart + result.ints;
                        }
                    }
                }
            }
            targetPart = part[1];
            for (var _i = 0, _a = targetTokens.decs || []; _i < _a.length; _i++) {
                var tokenPartVal = _a[_i];
                if (App.isStr(tokenPartVal)) {
                    result.decs += tokenPartVal;
                }
                else {
                    if (tokenPartVal.type === "%") {
                        result.decs += definition.percent.symbol;
                        continue;
                    }
                    if (tokenPartVal.type === "‰") {
                        result.decs += definition.percent.permilleSynbol;
                        continue;
                    }
                    var dec = targetPart.charAt(0) ? targetPart.charAt(0) :
                        tokenPartVal.type === "0" ? "0" : "";
                    if (dec) {
                        hasDec = true;
                    }
                    result.decs += dec;
                    targetPart = targetPart.substr(1);
                }
            }
            return ((isMinus && targetTokens === tokens.plus) ? definition.negSign : "") +
                result.ints + (hasDec ? definition.decSep + result.decs : result.decs);
        }
        function toFormatedText(value, format, definition) {
            var tokens = tokenize(format);
            return buildText(value, tokens, definition);
        }
        function parseText(value, definition) {
            var source = App.str.trim(value), target = source, 
            //currSymbol = definition.currency.symbol,
            preCurrSymbol = false, postCurrSymbol = false, hasNegParen = false, result, replaceRegExps = [];
            if (target === definition.nanSymbol) {
                return Number.NaN;
            }
            if (target === definition.posInfSymbol) {
                return Number.POSITIVE_INFINITY;
            }
            if (target === definition.negInfSymbol) {
                return Number.NEGATIVE_INFINITY;
            }
            //normalize
            if (target.charAt(0) === "(" && target.substr(-1) === ")") {
                hasNegParen = true;
                target = target.substr(1, target.length - 2);
            }
            if (definition.groupSep !== ",") {
                replaceRegExps.push(App.str.escapeRegExp(definition.groupSep));
            }
            if (definition.decSep !== ".") {
                replaceRegExps.push(App.str.escapeRegExp(definition.decSep));
            }
            if (definition.currency.symbol !== "$") {
                replaceRegExps.push(App.str.escapeRegExp(definition.currency.symbol));
            }
            if (definition.posSign !== "+") {
                replaceRegExps.push(App.str.escapeRegExp(definition.posSign));
            }
            if (definition.negSign !== "-") {
                replaceRegExps.push(App.str.escapeRegExp(definition.negSign));
            }
            if (replaceRegExps.length) {
                target = target.replace(new RegExp("(" + replaceRegExps.join(")|(") + ")", "g"), function (val) {
                    /*eslint-disable curly*/
                    if (val === definition.groupSep)
                        return ",";
                    if (val === definition.decSep)
                        return ".";
                    if (val === definition.currency.symbol)
                        return "$";
                    if (val === definition.posSign)
                        return "+";
                    if (val === definition.negSign)
                        return "-";
                    /*eslint-enable curly*/
                    return val;
                });
            }
            target = target.replace(/^[+-]?\$/, function (val) {
                preCurrSymbol = true;
                return val.replace("$", "");
            });
            target = target.replace(/\$[+-]?$/, function (val) {
                postCurrSymbol = true;
                return val.replace("$", "");
            });
            if (preCurrSymbol && postCurrSymbol) {
                return;
            }
            if (target.match(/[+-]$/)) {
                target = target.substr(-1) + target.substr(0, target.length - 1);
            }
            if (!target.match(numRegx) && !target.match(decRegx)) {
                return;
            }
            target = target.replace(/\,/g, "");
            result = parseFloat(target);
            if (isNaN(result)) {
                return;
            }
            if (hasNegParen && result < 0) {
                return;
            }
            if (hasNegParen) {
                return 0 - result;
            }
            return result;
        }
        function format(value, format) {
            if (!App.isNum(value) || !App.isStr(format) || isNaN(value)) {
                return;
            }
            if (App.isNum(value) && !isFinite(value)) {
                return;
            }
            var def = App.culture.current().numberFormat;
            if (format in formatterPool) {
                return formatterPool[format](value, def);
            }
            return toFormatedText(value, format, def);
        }
        num.format = format;
        function namedFormatter(name, formatter) {
            if (!App.isStr(name)) {
                return;
            }
            if (App.isFunc(formatter)) {
                formatterPool[name] = formatter;
            }
            else {
                return formatterPool[name];
            }
        }
        num.namedFormatter = namedFormatter;
        function removeNamedFormatter(name) {
            formatterPool[name] = void 0;
            delete formatterPool[name];
        }
        num.removeNamedFormatter = removeNamedFormatter;
        function hasNamedFormatter(name) {
            return App.isFunc(formatterPool[name]);
        }
        num.hasNamedFormatter = hasNamedFormatter;
        function parse(text, parser) {
            if (!App.isStr(text)) {
                return;
            }
            var def = App.culture.current().numberFormat;
            if (parser in parserPool) {
                return parserPool[parser](text, def);
            }
            return parseText(text, def);
        }
        num.parse = parse;
        function namedParser(name, parser) {
            if (!App.isStr(name)) {
                return;
            }
            if (App.isFunc(parser)) {
                parserPool[name] = parser;
            }
            else {
                return parserPool[name];
            }
        }
        num.namedParser = namedParser;
        function removeNamedParser(name) {
            parserPool[name] = void 0;
            delete parserPool[name];
        }
        num.removeNamedParser = removeNamedParser;
        function hasNamedParser(name) {
            return App.isFunc(parserPool[name]);
        }
        num.hasNamedParser = hasNamedParser;
        function roundInternal(target, prec, func) {
            if (prec === void 0) { prec = 0; }
            if (!App.isNum(target)) {
                return NaN;
            }
            func = App.isFunc(func) ? func : math.round;
            prec = App.isNum(prec) ? prec : 0;
            var mul = math.pow(10, math.abs(prec));
            if (prec < 0) {
                mul = 1 / mul;
            }
            return func(target * mul) / mul;
        }
        function round(target, prec) {
            return roundInternal(target, prec);
        }
        num.round = round;
        function ceil(target, prec) {
            return roundInternal(target, prec, math.ceil);
        }
        num.ceil = ceil;
        function floor(target, prec) {
            return roundInternal(target, prec, math.floor);
        }
        num.floor = floor;
        function median(target) {
            if (!App.isArray(target)) {
                return NaN;
            }
            if (!target.length) {
                return NaN;
            }
            var clone = target.concat();
            clone.sort(function (left, right) {
                return left - right;
            });
            var center = Math.floor(clone.length / 2.0);
            if (clone.length % 2) {
                return clone[center];
            }
            return (clone[center - 1] + clone[center]) / 2;
        }
        num.median = median;
        namedFormatter("currency", function (value) {
            return format(value, "#,##0");
        });
        namedParser("currency", function (text) {
            return parse(text);
        });
        namedFormatter("number", function (value) {
            return format(value, "#");
        });
        namedParser("number", function (text) {
            return parse(text);
        });
        namedFormatter("decimal", function (value) {
            return format(value, "#.00");
        });
        namedParser("decimal", function (text) {
            return parse(text);
        });
    })(num = App.num || (App.num = {}));
})(App || (App = {}));

/// <reference path="base.ts" />
/// <reference path="obj.ts" />
var App;
(function (App) {
    var uri;
    (function (uri_1) {
        "use strict";
        var parseUriRegx = /^(([^:\/?#]+):)?(\/\/([^\/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?$/, //RFC 3986 appendix B.
        missingGroupSupport = (typeof "".match(/(a)?/)[1] !== "string"), parseUri = function (target) {
            var matches;
            if (!App.isStr(target)) {
                return;
            }
            matches = target.match(parseUriRegx);
            matches.shift();
            return {
                schema: !missingGroupSupport && matches[0] === "" ? undefined : matches[1],
                authority: !missingGroupSupport && matches[2] === "" ? undefined : matches[3],
                path: matches[4],
                query: !missingGroupSupport && matches[5] === "" ? undefined : matches[6],
                fragment: !missingGroupSupport && matches[7] === "" ? undefined : matches[8]
            };
        }, isAbsoluteRegx = /^[A-Z][0-9A-Z+\-\.]*:/i, isAbsolute = function (target) {
            return isAbsoluteRegx.test(target);
        }, 
        //RFC 3986 5.2.4
        removeDotSegments = function (path) {
            var startIsSeparator = false, segments, seg, keepers = [];
            // return empty string if entire path is just "." or ".."
            if (path === "." || path === "..") {
                return "";
            }
            // remove all "./" or "../" segments at the beginning
            while (path) {
                if (path.substring(0, 2) === "./") {
                    path = path.substring(2);
                }
                else if (path.substring(0, 3) === "../") {
                    path = path.substring(3);
                }
                else {
                    break;
                }
            }
            if (path.charAt(0) === "/") {
                path = path.substring(1);
                startIsSeparator = true;
            }
            if (path.substring(path.length - 2) === "/.") {
                path = path.substring(0, path.length - 1);
            }
            segments = path.split("/").reverse();
            while (segments.length) {
                seg = segments.pop();
                if (seg === "..") {
                    if (keepers.length) {
                        keepers.pop();
                    }
                    else if (!startIsSeparator) {
                        keepers.push(seg);
                    }
                    if (!segments.length) {
                        keepers.push("");
                    }
                }
                else if (seg !== ".") {
                    keepers.push(seg);
                }
            }
            return (startIsSeparator && "/" || "") + keepers.join("/");
        }, absolutize = function (base, relative) {
            var part, relSchema, relAuth, relPath, relQuery, relFrag, bSchema, bAuth, bPath, bQuery, resSchema, resAuth, resPath, resQuery, resFrag;
            if (!App.isStr(base) || !isAbsolute(base) || !App.isStr(relative)) {
                return;
            }
            if (relative === "" || relative.charAt(0) === "#") {
                return parseUri(base.split("#")[0] + relative);
            }
            part = parseUri(relative) || {};
            relSchema = part.schema;
            relAuth = part.authority;
            relPath = part.path;
            relQuery = App.isUndef(part.query) ? void 0 : part.query + "";
            relFrag = part.fragment;
            if (relSchema) {
                resSchema = relSchema;
                resAuth = relAuth;
                resPath = removeDotSegments(relPath);
                resQuery = relQuery;
            }
            else {
                part = parseUri(base) || {};
                bSchema = part.schema;
                bAuth = part.authority;
                bPath = part.path;
                bQuery = App.isUndef(part.query) ? void 0 : part.query + "";
                if (relAuth) {
                    resAuth = relAuth;
                    resPath = removeDotSegments(relPath);
                    resQuery = relQuery;
                }
                else {
                    if (!relPath) {
                        resPath = bPath;
                        resQuery = relQuery ? relQuery : bQuery;
                    }
                    else {
                        if (relPath.charAt(0) === "/") {
                            resPath = removeDotSegments(relPath);
                        }
                        else {
                            //RFC 3986 5.2.3
                            if (bAuth && !bPath) {
                                resPath = "/" + relPath;
                            }
                            else {
                                resPath = bPath.substring(0, bPath.lastIndexOf("/") + 1) + relPath;
                            }
                            resPath = removeDotSegments(resPath);
                        }
                        resQuery = relQuery;
                    }
                    resAuth = bAuth;
                }
                resSchema = bSchema;
            }
            resFrag = relFrag;
            return {
                schema: resSchema,
                authority: resAuth,
                path: resPath,
                query: resQuery,
                fragment: resFrag
            };
        }, toUri = function (target) {
            var result = "", path = target.path;
            if (!App.isUndef(target.schema)) {
                result += target.schema + ":";
            }
            if (!App.isUndef(target.authority)) {
                result += "//" + target.authority;
            }
            if (!App.isUndef(path) && path.length > 0) {
                if (!App.isUndef(target.authority)) {
                    if (path.charAt(0) !== "/") {
                        path = "/" + path;
                    }
                }
                result += path;
            }
            if (!App.isUndef(target.query)) {
                result += "?" + target.query;
            }
            if (!App.isUndef(target.fragment)) {
                result += "#" + target.fragment;
            }
            return result;
        };
        /**
         * 指定された uri の query をキーバリューの形式に変換します。
         */
        function splitQuery(query) {
            var queryStr = (query || ""), items = (queryStr.charAt(0) === "?" ? queryStr.substr(1) : queryStr).split("&"), keyValue, result = {};
            for (var _i = 0; _i < items.length; _i++) {
                var item = items[_i];
                if (!item) {
                    continue;
                }
                keyValue = item.split("=");
                if (keyValue.length < 1) {
                    continue;
                }
                if (keyValue.length < 2) {
                    result[keyValue[0]] = undefined;
                    continue;
                }
                result[keyValue[0]] = decodeURIComponent(keyValue[1].replace(/\+/g, " "));
            }
            return result;
        }
        uri_1.splitQuery = splitQuery;
        ;
        /**
         * 指定された値を uri の query 形式で連結します。
         */
        function joinQuery(query) {
            var vals = [], keys, i, l, key;
            if (!query) {
                return;
            }
            if (App.isStr(query)) {
                return query;
            }
            if (App.isArray(query)) {
                return query.join("&");
            }
            var q = query;
            for (var _i = 0, _a = Object.keys(q); _i < _a.length; _i++) {
                var key_1 = _a[_i];
                if (App.isUndefOrNull(q[key_1]) || q[key_1] === "") {
                    vals.push(key_1 + "=");
                }
                else {
                    vals.push(key_1 + "=" + encodeURIComponent(q[key_1]));
                }
            }
            return vals.join("&");
        }
        uri_1.joinQuery = joinQuery;
        ;
        /**
         * 指定された uri 文字列を分解したオブジェクトを返します。
         */
        function parse(uri) {
            var result = parseUri(uri);
            result.query = splitQuery(App.isUndef(result.query) ? undefined : (result.query + ""));
            return result;
        }
        uri_1.parse = parse;
        ;
        /**
         * uri構成のオブジェクトからuri文字列を作成します。
         */
        function toUriString(uriElement) {
            var elem = App.obj.omit(uriElement, "query");
            elem.query = joinQuery(uriElement.query);
            return toUri(elem);
        }
        uri_1.toUriString = toUriString;
        ;
        /**
        * 指定された絶対パスをもとに、指定された相対パスを絶対パスに変換します。
        */
        function resolve(base, relative) {
            if (!App.isStr(base) || !App.isStr(relative)) {
                return;
            }
            var uri = absolutize(base, relative);
            return toUri(uri);
        }
        uri_1.resolve = resolve;
        ;
        /**
         * 指定されたuri文字列に指定されたparamオブジェクトをURIクエリーに設定した文字列を返します。
         */
        function setQuery(uriString, param) {
            var result = parseUri(uriString), query = splitQuery(App.isUndef(result.query) ? undefined : (result.query + ""));
            query = App.obj.mixin(query, param);
            result.query = joinQuery(query);
            return toUri(result);
        }
        uri_1.setQuery = setQuery;
        ;
    })(uri = App.uri || (App.uri = {}));
})(App || (App = {}));

var App;
(function (App) {
    "use strict";
    var defaultFormat = "N";
    var formats = {
        "N": "{0}{1}{2}{3}{4}{5}",
        "D": "{0}-{1}-{2}-{3}{4}-{5}",
        "B": "{{0}-{1}-{2}-{3}{4}-{5}}",
        "P": "({0}-{1}-{2}-{3}{4}-{5})"
    };
    /**
    * 指定された形式で一意識別子を作成します。
    * @param {String} format 指定するフォーマット形式。形式を省略した場合は"{0}{1}{2}{3}{4}{5}"の形式で作成
    *                        "N": "{0}{1}{2}{3}{4}{5}",
    *                        "D": "{0}-{1}-{2}-{3}{4}-{5}",
    *                        "B": "{{0}-{1}-{2}-{3}{4}-{5}}",
    *                        "P": "({0}-{1}-{2}-{3}{4}-{5})"
    * @return 作成された一意識別子
    */
    function uuid(format) {
        var rand = function (range) {
            /*eslint-disable curly*/
            if (range < 0)
                return NaN;
            if (range <= 30)
                return Math.floor(Math.random() * (1 << range));
            if (range <= 53)
                return Math.floor(Math.random() * (1 << 30)) +
                    Math.floor(Math.random() * (1 << range - 30)) * (1 << 30);
            /*eslint-enable curly*/
            return NaN;
        }, prepareVal = function (value, length) {
            var ret = "000000000000" + value.toString(16);
            return ret.substr(ret.length - length);
        }, vals = [
            rand(32),
            rand(16),
            0x4000 | rand(12),
            0x80 | rand(6),
            rand(8),
            rand(48)
        ], result = formats[format] || formats[defaultFormat];
        result = result.replace("{0}", prepareVal(vals[0], 8));
        result = result.replace("{1}", prepareVal(vals[1], 4));
        result = result.replace("{2}", prepareVal(vals[2], 4));
        result = result.replace("{3}", prepareVal(vals[3], 2));
        result = result.replace("{4}", prepareVal(vals[4], 2));
        result = result.replace("{5}", prepareVal(vals[5], 12));
        return result;
    }
    App.uuid = uuid;
})(App || (App = {}));

/*global App*/

/// <reference path="../../ts/core/base.ts" />
/// <reference path="../../ts/core/thenable.ts" />
/// <reference path="../../ts/core/obj.ts" />
/// <reference path="../../ts/core/str.ts" />

/*
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, App) {

    "use strict";

    var customMaybePromise = null;

    /**
     * 定義を保持してバリデーションを実行するオブジェクトを定義します。
     */
    function Validator(definition, options) {
        this._context = {
            def: definition || {},
            options: options || {}
        };
    }
    function maybePromise(callback){
        if(App.isFunc(customMaybePromise)){
            return customMaybePromise(callback);
        }
        return App.thenable(callback);
    }

    var isEmpty = function (value) {
            return App.isUndef(value) || App.isNull(value) || (App.isStr(value) && value.length === 0);
        },
        //バリデーションメソッド
        methods = {
            //required は特別扱いのため、内部で定義
            required: {
                name: "required",
                handler: function (value, context, done) {
                    if (!context.parameter) { //param が falsy だったら必須ではない
                        done(true);
                    }
                    done(!isEmpty(value));
                },
                priority: -1
            }
        },
        //メッセージの保持
        messages = {
            required: "this item required"
        },
        //validate関数に渡された引数を整形します。
        prepareValidateArgs = function (args) {
            var result = {
                values: {},
                options: {}
            };
            // 引数が2つで第１引数が文字列の場合は、 validate("item1", "value") のように
            // 項目名と値が渡されたとみなす
            if (args.length === 2 && App.isStr(args[0])) {
                result.values[args[0]] = args[1];
                result.options = args[2] || {};
                return result;
            } else {
                result.values = args[0] || {};
                //先頭が文字列または配列の場合はターゲットとなるグループが指定されたとみなします。
                if (App.isStr(args[1])) {
                    result.options = {
                        groups: [args[1]]
                    };
                } else if (App.isArray(args[1])) {
                    result.options = {
                        groups: args[1]
                    };
                } else {
                    //上記以外はオブジェクトでオプション全体が渡されたとみなします。
                    result.options = args[1] || {};
                }
                return result;
            }
        },
        prepareExecuteTargets = function (targets, defs) {
            var result = {},
                defKey, def,
                groups,
                hasGroup;

            if (!targets) {
                return defs;
            }
            if (App.isStr(targets)) {
                targets = [targets];
            }
            if (!App.isArray(targets)) {
                return defs;
            }
            for (defKey in defs) {
                if (!defs.hasOwnProperty(defKey)) {
                    continue;
                }
                def = defs[defKey];
                groups = App.isStr(def.groups) ? [def.groups] :
                         App.isArray(def.groups) ? def.groups : [];
                /*eslint-disable no-loop-func*/
                hasGroup = targets.some(function (value) {
                    return groups.indexOf(value) > -1 || groups.length === 0;
                });
                /*eslint-enable no-loop-func*/
                if (hasGroup) {
                    result[defKey] = defs[defKey];
                }
            }
            return result;
        },
        getMessage = function (item, value, method, rules, msgs, opts, isCustom) {
            var format = msgs[method] || messages[method] || "";
            return App.str.format(format, App.obj.mixin({}, opts, {
                value: value,
                item: item,
                method: method,
                param: isCustom ? "" : rules[method]
            }));
        },
        //バリデーションメソッドを順番に実行します。
        executeFirstMethod = function (item, defer, execMethods, value, rules, msgs, opts, grps, execOptions) {
            var execMethod,
                callback = function (result) {
                    if (!result) {
                        return defer.reject({
                            item: item,
                            message: getMessage(item, value, execMethod.name, rules, msgs, opts, execMethod.isCustom)
                        });
                    } else {
                        executeFirstMethod(item, defer, execMethods, value, rules, msgs, opts, grps, execOptions);
                    }
                },
                filter = execOptions.filter || function () { return true; };
            //バリデーションメソッドがない場合は、すべて完了してエラーがなかったとみなし
            //成功で完了する
            if (!execMethods.length) {
                return defer.resolve({
                    item: item
                });
            }

            execMethod = execMethods.shift();
            if (!execMethod || !App.isFunc(execMethod.handler)) {
                executeFirstMethod(item, defer, execMethods, value, rules, msgs, opts, grps, execOptions);
            } else {
                if (filter(item, execMethod.name, execOptions.state, {
                    item: item,
                    rule: execMethod.name,
                    options: opts,
                    groups: grps,
                    executeOptions: execOptions,
                    executeGroups: execOptions.groups,
                        targetElements: execOptions.elements || {}
                })) {
                    execMethod.handler(value, {
                        item: item,
                        rule: execMethod.name,
                        custom: !!execMethod.isCustom,
                        parameter: execMethod.isCustom ? undefined : rules[execMethod.name],
                        options: opts,
                        //grops: grps,
                        //targetGroups: execOptions.groups,
                        state: execOptions.state,
                        targetElements: execOptions.elements || {}
                    }, callback);
                } else {
                    executeFirstMethod(item, defer, execMethods, value, rules, msgs, opts, grps, execOptions);
                }
            }
        },
        //項目ごとのバリデーションを実行します。
        validateItem = function (item, values, options, defs) {
            return App.thenable(function(fullfille, reject){
                var defer = {
                        resolve: fullfille,
                        reject: reject
                    },
                    value = values[item],
                    def = defs[item],
                    rules = def.rules || {},
                    msgs = def.messages || {},
                    opts = def.options || {},
                    grps = def.groups,
                    method, targetMethod,
                    execMethods = [];

                for (method in rules) {
                    if (!rules.hasOwnProperty(method)) {
                        continue;
                    }
                    targetMethod = methods[method];
                    if (!targetMethod) {
                        if (!App.isFunc(rules[method])) {
                            throw new Error("custom method value must be function");
                        }
                        //methodが登録されておらず値が定義の値が関数の場合は
                        //カスタムmethodとして利用。但し優先度は最低に設定
                        targetMethod = {
                            isCustom: true,
                            name: method,
                            handler: rules[method],
                            priority: Number.MAX_VALUE
                        };
                    }
                    execMethods.push(targetMethod);
                }
                //優先度順にソートします。
                execMethods.sort(function (left, right) {
                    if (left.priority < right.priority) {
                        return -1;
                    }
                    if (left.priority > right.priority) {
                        return 1;
                    }
                    return 0;
                });
                executeFirstMethod(item, defer, execMethods, value, rules, msgs, opts, grps, options);
            });
        },
        toNum = function (val) {
            if (isEmpty(val)) {
                return (void 0);
            }
            if (!App.isNum(val)) {
                if (App.isNumeric(val)) {
                    return parseFloat(val);
                } else {
                    return (void 0);
                }
            }
            return val;
        };

    Validator.prototype = {
        /**
        * 定義内に指定されたアイテム名があるかどうかを取得します。
        */
        hasItem: function (itemName) {
            var def;
            if (!this._context) {
                return false;
            }
            def = this._context.def;
            return !!def[itemName];
        },
        /**
        * 指定された値のバリデーションを実行します。
        */
        validate: function (values, options) {
            var that = this,
                args = Array.prototype.slice.call(arguments);

            return maybePromise(function(fullfille, reject) {
                var successes = [],
                    fails = [],
                    count = 0,
                    completed = false,
                    arg, defs, item,
                    keys = [], i, l,
                    execTargets,
                    //全てのvalidationItemの実行が完了したかをチェックして
                    //完了していた場合、promise を完了させます。
                    checkComplete = function () {
                        var result;
                        if (!completed && (successes.length + fails.length) >= count) {
                            completed = true;
                            result = {
                                successes: successes,
                                fails: fails
                            };
                            //validate のオプションに beforeReturnResult 関数が指定されている場合は、
                            //実行します。
                            if (App.isFunc(options.beforeReturnResult)) {
                                options.beforeReturnResult(result, options.state);
                            }
                            //validator のオプションに success 関数または fail 関数が指定されている場合は、
                            //success 関数には成功データ、fail 関数には失敗データを渡して実行します。
                            if (that._context && that._context.options) {
                                if (App.isFunc(that._context.options.success)) {
                                    that._context.options.success(result.successes, options.state);
                                }
                                if (App.isFunc(that._context.options.fail)) {
                                    that._context.options.fail(result.fails, options.state);
                                }
                                if (App.isFunc(that._context.options.always)) {
                                    that._context.options.always(result.successes, result.fails, options.state);
                                }
                            }
                            //結果を deferred に設定します。
                            (result.fails.length ? reject : fullfille)(result, options.state);
                        }
                    };

                if (!that._context || !that._context.def) {
                    return fullfille();
                }
                arg = prepareValidateArgs(args);
                defs = that._context.def;
                values = arg.values;
                options = arg.options;
                execTargets = prepareExecuteTargets(options.groups, defs);

                for (item in values) {
                    if (!values.hasOwnProperty(item) || !(item in execTargets)) {
                        continue;
                    }
                    keys.push(item);
                    count++;
                }

                for (i = 0, l = keys.length; i < l; i++) {
                    item = keys[i];
                    //validationItemを実行し、結果を保存します。
                    /*eslint-disable no-loop-func */
                    validateItem(item, values, options, execTargets).then(function (result) {
                        successes.push(result);
                        checkComplete();
                    }, function (result) {
                        fails.push(result);
                        checkComplete();
                    });
                    /*eslint-enable no-loop-func */
                }
                //すべてのvalidationItemが完了しているかをチェックします。
                //非同期バリデーションが含まれていない場合は、この時点で完了されていますが、
                //非同期バリデーションが含まれている場合は、この時点では完了していません。
                checkComplete();
            });
        }
    };
    /**
    * Validation オブジェクトを指定された定義およびオプションで作成します。
    */
    App.validation = function (definition, options) {
        if (definition instanceof Validator) {
            definition._context.options = App.obj.mixin({}, options, definition._context.options);
            return definition;
        }
        return new Validator(definition, options);
    };

    /**
    * Validation 用の関数を追加します。
    */
    App.validation.addMethod = function (name, handler, message, priority) {
        methods[name] = {
            name: name,
            handler: handler,
            priority: (!App.isNum(priority) || isNaN(priority) || isFinite(priority)) ? 10 : Math.max(0, priority)
        };
        messages[name] = message || "";
    };

    /**
    * Validatin 用の関数を削除します。
    */
    App.validation.removeMethod = function (name) {
        if (name in methods) {
            delete methods[name];
        }
        if (name in messages) {
            delete messages[name];
        }
    };
    /**
     * 指定された名前のメソッドが登録されているかどうかを取得します。
     */
    App.validation.hasMethod = function(name){
        return name in methods;
    };
    /**
     * 指定された名前で登録されているメソッドのメッセージを設定します。
     */
    App.validation.setMessage = function(name, message){
        if(name in methods) {
            messages[name] = message + "";
        }
    };
    /**
     * 指定された名前で登録されているメソッドのメッセージを取得します。
     */
    App.validation.getMessage = function(name) {
        return messages[name];
    };
    /**
    * 指定された値が空かどうかをチェックします。
    * これは required メソッドと同じチェックが実行されます。
    */
    App.validation.isEmpty = isEmpty;

    /**
     * validationメソッドの戻り値のPromiseを設定します。
     */
    App.validation.setReturnPromise = function(promise){
        customMaybePromise = promise;
    };

    /**
    * 指定された値が数値のみかどうかを検証します。
    * 値が空の場合は、検証を実行しない為成功となります。
    */
    App.validation.addMethod("digits", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }
        value = "" + value;
        done(/^\d+$/.test(value));

    }, "digits only");
    /**
     * 指定された値が整数値かどうかを検証します。
     * 値が空の場合は、けんしょうを実行しないため成功となります。
     */
    App.validation.addMethod("integer", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }
        if (App.isNum(value)) {
            return done(isFinite(value) && value > -9007199254740992 && value < 9007199254740992 && Math.floor(value) === value);
        }
        value = "" + value;
        done(value === "0" || /^\-?[1-9][0-9]*$/.test(value));
    }, "a invalid integer");

    /**
    * 指定された値が数値かどうかを検証します。
    * 値が空の場合は、検証を実行しない為成功となります。
    */
    App.validation.addMethod("number", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }
        done(App.isNumeric(value));
    }, "a invalid number");

    /**
    * 指定された値がパラメーターで指定された値以上かどうかを検証します。
    * 値が空または数値に変換できない場合、およびパラメーターが数値に変換できない場合は
    * 検証を実行しない為成功となります。
    */
    App.validation.addMethod("min", function (value, context, done) {
        var val, param;

        if (isEmpty(value)) {
            return done(true);
        }

        val = toNum(value);
        param = toNum(context.parameter);
        if (App.isUndef(val) || App.isUndef(param)) {
            return done(true);
        }
        return done(val >= param);
    }, "a value greater than or equal to {param}");
    /**
    * 指定された値がパラメーターで指定された値以下かどうかを検証します。
    * 値が空または数値に変換できない場合、およびパラメーターが数値に変換できない場合は
    * 検証を実行しない為成功となります。
    */
    App.validation.addMethod("max", function (value, context, done) {
        var val, param;
        if (isEmpty(value)) {
            return done(true);
        }
        val = toNum(value);
        param = toNum(context.parameter);
        if (App.isUndef(val) || App.isUndef(param)) {
            return done(true);
        }
        return done(val <= param);
    }, "a value less than or equal to {param}");

    /**
    * 指定された値がパラメーターで指定された配列のインデックス 0 に指定されている値以上、
    * インデックス 1 に指定されている値以下かどうかを検証します。
    * 値が空または数値に変換できない場合、およびパラメーターが配列でないもしくはインデックス 0 と 1 が数値に変換できない場合は
    * 検証を実行しない為成功となります。
    */
    App.validation.addMethod("range", function (value, context, done) {
        var val, param = [];

        if (isEmpty(value)) {
            return done(true);
        }

        if (!App.isArray(context.parameter)) {
            return done(true);
        }
        val = toNum(value);
        param[0] = toNum(context.parameter[0]);
        param[1] = toNum(context.parameter[1]);
        if (App.isUndef(val) || App.isUndef(param[0]) || App.isUndef(param[1])) {
            return done(true);
        }
        return done(val >= param[0] && val <= param[1]);
    }, "a value between {param[0]} and {param[1]}");

})(this, App);

/* global App */

///<reference path="../../ts/core/base.ts" />
/// <reference path="validation.js" />

/*
* バリデーションを拡張するメソッドを定義します。
* Copyright(c) Archway Inc. All rights reserved.
*/
(function (global, App) {

    "use strict";

    var isEmpty = App.validation.isEmpty;

    /**
     * 指定された値がパラメーターで指定された値より長い文字列かどうかを検証します。
     * 値が空または数値に変換できない場合、およびパラメーターが数値に変換できない場合は
     * 検証を実行しない為成功となります。
     */
    App.validation.addMethod("minlength", function (value, context, done) {

        if (isEmpty(value)) {
            return done(true);
        }
        if (!App.isNum(context.parameter)) {
            return done(true);
        }

        value = App.isNum(value) && App.isNumeric(value) ? value + "" : value;
        var length = App.isArray(value) ? value.length : App.str.trim(value).length;

        done((((value || "") + "") === "") || (length >= context.parameter));

    }, "at least {param} characters");

    /**
     * 指定された値がパラメーターで指定された値より短い文字列かどうかを検証します。
     * 値が空または数値に変換できない場合、およびパラメーターが数値に変換できない場合は
     * 検証を実行しない為成功となります。
     */
    App.validation.addMethod("maxlength", function (value, context, done) {

        if (isEmpty(value)) {
            return done(true);
        }
        if (!App.isNum(context.parameter)) {
            return done(true);
        }
        value = App.isNum(value) && App.isNumeric(value) ? value + "" : value;
        var length = App.isArray(value) ? value.length : App.str.trim(value).length;

        done(((value || "") + "") === "" || length <= context.parameter);

    }, "no more than {param} characters");

    /**
     * 指定された値がパラメーターで指定された値と同じ長さの文字列かどうかを検証します。
     * 値が空または数値に変換できない場合、およびパラメーターが数値に変換できない場合は
     * 検証を実行しない為成功となります。
     */
    App.validation.addMethod("length", function (value, context, done) {

        if (isEmpty(value)) {
            return done(true);
        }
        if (!App.isNum(context.parameter)) {
            return done(true);
        }
        value = App.isNum(value) && App.isNumeric(value) ? value + "" : value;
        var length = App.isArray(value) ? value.length : App.str.trim(value).length;

        done(((value || "") + "") === "" || length === context.parameter);

    }, "must text has {param} characters");

    App.validation.addMethod("rangelength", function (value, context, done) {
        var param = context.parameter;

        if (isEmpty(value)) {
            return done(true);
        }
        if (!App.isArray(param)) {
            return done(true);
        }
        if (!App.isNum(param[0])) {
            return done(true);
        }
        if (!App.isNum(param[0])) {
            return done(true);
        }

        value = App.isNum(value) && App.isNumeric(value) ? value + "" : value;
        var length = App.isArray(value) ? value.length : App.str.trim(value).length;

        done(((value || "") + "") === "" || (length >= param[0] && length <= param[1]));

    }, "a value between {param[0]} and {param[1]} characters long");

    /**
     * 文字列で指定された数値の小数点の桁数を検証します。
     * e.g. decimallength: 2 (2桁まで)
     */
    App.validation.addMethod("decimalmaxlength", function (value, context, done) {
        var param = context.parameter;

        //桁数が数値でない場合はバリデーションを実行できない為、 true で返す。
        if (!App.isNum(param)) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }
        var target = value + "";
        //そもそも数時とみなせるか(hexやoct、指数は対象外)
        if (!/^(-?)((([1-9]([0-9]*))(\.[0-9]+)?)|(0(\.[0-9]+)?))$/.test(target)) {
            //数値とみなせない場合は処理をしない為 true で返す。
            //バリデーションとしては同時にnumberかどうかのバリデーションを設定するべき
            return done(true);
        }
        done((target.split(".")[1] || "").length <= param);
    }, "max decimal length {param}");

    /**
     * 日付型のバリデーションを指定します。
     */
    App.validation.addMethod("date", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }

        value = App.isNum(value) ? value + "" : value;
        var regex = /Invalid|NaN/;

        done(((value || "") + "") === "" || !regex.test(new Date(value).toString()));

    }, "日付の値が不正です。");

    /**
     * 日付文字列のバリデーションを指定します。
     */
    App.validation.addMethod("datestring", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }

        if (value === "") {
            return done(true);
        }

        if (!(/^[0-9]{4}\/[0-9]{2}\/[0-9]{2}$/.test(value))) {
            return done(false);
        }

        var year = parseInt(value.substr(0, 4), 10);
        var month = parseInt(value.substr(5, 2), 10);
        var day = parseInt(value.substr(8, 2), 10);
        var inputDate = new Date(year, month - 1, day);

        done((inputDate.getFullYear() === year && inputDate.getMonth() === month - 1 && inputDate.getDate() === day));

    }, "日付の値が不正です。");

    /**
     * 長い形式（yyyy年MM月dd日）の日付文字列のバリデーションを指定します。
     */
    App.validation.addMethod("datelongstring", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }

        if (value === "") {
            return done(true);
        }

        if (!(/^[0-9]{4}年[0-9]{2}月[0-9]{2}日$/.test(value))) {
            return done(false);
        }

        var year = parseInt(value.substr(0, 4), 10);
        var month = parseInt(value.substr(5, 2), 10);
        var day = parseInt(value.substr(8, 2), 10);
        var inputDate = new Date(year, month - 1, day);

        done((inputDate.getFullYear() === year && inputDate.getMonth() === month - 1 && inputDate.getDate() === day));

    }, "日付の値が不正です。");


    /**
     * 時刻(HH:mm)のバリデーションを指定します。
     */
    App.validation.addMethod("hourminute", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }
        value = value + "";
        if (!(/^[0-9]{2}:[0-9]{2}$/.test(value))) {
            return done(false);
        }
        var hour = parseInt(value.substr(0, 2), 10);
        var minute = parseInt(value.substr(3, 2), 10);

        done(hour > -1 && hour < 25 && minute > -1 && hour < 60);
    }, "a invalid time");

    /**
     * 年月(yyyy/MM)のバリデーションを指定します。
     */
    App.validation.addMethod("yearmonth", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }
        value = value + "";
        if (!(/^[1-9][0-9]{0,3}\/[0-1][0-9]$/.test(value))) {
            return done(false);
        }

        var yearMonth = value.split("/");
        var month = parseInt(yearMonth[1], 10);
        //yearは正規表現上で0にならないことが確定しているためOK
        //monthは00の場合だけ正規表現を通ってしまうため、parseIntして1以上12以下でチェック

        done(month >= 1 && month <= 12);
    }, "a invalid year month");

    /**
     * 文字列の正規表現バリデーションを指定します。
     */
    App.validation.addMethod("regex", function (value, context, done) {

        if (isEmpty(value)) {
            return done(true);
        }

        var regex = new RegExp(context.parameter);
        done(regex.test(value));

    }, "a invalid text");

    /**
     * 全角のみかどうかのバリデーションを指定します。
     */
//     App.validation.addMethod("zenkaku", function (value, context, done) {
//         if (!context.parameter) {
//             return done(true);
//         }
//         if (isEmpty(value)) {
//             return done(true);
//         }
//
//         value = App.isStr(value) ? value + "" : value;
//         var regex = /^[^ -~｡-ﾟ]+$/;
//
//         done(((value || "") + "") === "" || regex.test(value));
//
//     }, "全角を入力してください");

    /**
     * 半角のみかどうかのバリデーションを指定します。
     */
//     App.validation.addMethod("hankaku", function (value, context, done) {
//         if (!context.parameter) {
//             return done(true);
//         }
//         if (isEmpty(value)) {
//             return done(true);
//         }
//
//         value = App.isStr(value) ? value + "" : value;
//         if (((value || "") + "") !== ""
//             && !/^([ -~｡-ﾟ]+)$/.test(value)) {
//             return done(false);
//         }
//
//         done(true);
//
//     }, "半角を入力してください");

    /**
     * 半角文字のみで且つ半角カナが含まれていないかどうかのバリデーションを指定します。
     */
//     App.validation.addMethod("haneisukigo", function (value, context, done) {
//         if (!context.parameter) {
//             return done(true);
//         }
//
//         if (isEmpty(value)) {
//             return done(true);
//         }
//
//         value = App.isNum(value) ? value + "" : value;
//         if (((value || "") + "") !== ""
//             && (/([ァ-ヶーぁ-ん]+)/.test(value) || /([ｧ-ﾝﾞﾟ]+)/.test(value) || /([ａ-ｚーＡ-Ｚ]+)/.test(value))) {
//             return done(false);
//         }
//
//         done(true);
//
//     }, "半角英数記号を入力してください");

    /**
     * 電話番号（例:012-345-6789）かどうかのバリデーションを指定します。
     */
    App.validation.addMethod("telnum", function (value, context, done) {
        if (!context.parameter) {
            return done(true);
        }
        if (isEmpty(value)) {
            return done(true);
        }

        value = App.isNum(value) && App.isNumeric(value) ? value + "" : value;
        var regex = /^[0-9-]{12}$/;

        done(((value || "") + "") === "" || regex.test(value));

    }, "電話番号を入力してください（例:012-345-6789）");


})(this, App);
