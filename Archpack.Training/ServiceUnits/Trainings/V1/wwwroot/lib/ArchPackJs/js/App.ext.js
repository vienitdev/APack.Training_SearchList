/// <reference path="../../ts/core/base.ts" />
/// <reference path="../core/validation.js" />

//Shift-JIS関連
; (function () {

    var text = App.define("App.page.text");
    /**
     * 値のShift-JIS文字コードでのバイト数を取得します。
     */
    text.getShiftJISByteLength = function (val) {
        var byteLen = 0,
            i = 0, l, c;
        if (!App.isStr(val)) {
            return;
        }
        for (l = val.length; i < l; i++) {
            c = val.charCodeAt(i);
            // Shift_JIS: 0x0 ～ 0x80, 0xa0  , 0xa1   ～ 0xdf  , 0xfd   ～ 0xff
            // Unicode  : 0x0 ～ 0x80, 0xf8f0, 0xff61 ～ 0xff9f, 0xf8f1 ～ 0xf8f3
            if ((c >= 0x0 && c < 0x81) || (c == 0xf8f0) || (c >= 0xff61 && c < 0xffa0) || (c >= 0xf8f1 && c < 0xf8f4)) {
                byteLen += 1;
            } else {
                byteLen += 2;
            }
        }
        return byteLen;
    };
    /**
     * 指定された長さ未満のShift-JISでの文字列バイト長かどうかを検証します。
     */
    App.validation.addMethod("maxSjisByteLength", function (value, context, done) {
        if (App.validation.isEmpty(value)) {
            return done(true);
        }
        if (!App.isNum(context.parameter)) {
            return done(true);
        }

        var byteLen = App.page.text.getShiftJISByteLength(value + "");
        done(byteLen <= context.parameter);
    }, "text bytes more than equal {param}");

    /**
     * 指定された長さのShift-JISでの文字列バイト長かどうかを検証します。
     */
    App.validation.addMethod("sjisByteLength", function (value, context, done) {
        if (App.validation.isEmpty(value)) {
            return done(true);
        }
        if (!App.isNum(context.parameter)) {
            return done(true);
        }

        var byteLen = App.page.text.getShiftJISByteLength(value + "");
        done(byteLen === context.parameter);
    }, "text byts must be {param}");
    /**
     * 指定された長さより長いShift-JISでの文字列バイト長かどうかを検証します。
     */
    App.validation.addMethod("minSjisByteLength", function (value, context, done) {
        if (App.validation.isEmpty(value)) {
            return done(true);
        }
        if (!App.isNum(context.parameter)) {
            return done(true);
        }

        var byteLen = App.page.text.getShiftJISByteLength(value + "");
        done(byteLen >= context.parameter);
    }, "text bytes less than equal {param}");
})();
