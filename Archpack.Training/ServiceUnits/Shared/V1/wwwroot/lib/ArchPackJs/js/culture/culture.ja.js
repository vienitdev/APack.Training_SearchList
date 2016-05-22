/*global App */
/// <reference path="../culture.js" />

(function () {

    "use strict";

    if (!App && !App.culture) {
        return;
    }

    App.culture("ja", {
        "name": "ja-JP",
        "engName": "Japanese (Japan)",
        "lang": "ja",
        "dateTimeFormat": {
            "months": {
                "shortNames": ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", ""],
                "names": ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月", ""]
            },
            "weekdays": {
                "shortNames": ["日", "月", "火", "水", "木", "金", "土"],
                "names": ["日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"]
            },
            "meridiem": {
                "ante": "午前",
                "post": "午後"
            },
            "patterns": {
                "d": "yyyy/MM/dd",
                "D": "yyyy'年'M'月'd'日'",
                "t": "H:mm",
                "T": "H:mm:ss",
                "f": "yyyy'年'M'月'd'日' H:mm",
                "F": "yyyy'年'M'月'd'日' H:mm:ss",
                "M": "M'月'd'日'",
                "S": "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                "Y": "yyyy'年'M'月'"
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
            "posInfSymbol": "+∞",
            "negInfSymbol": "-∞",
            "nanSymbol": "NaN (非数値)",
            "currency": {
                "symbol": "¥",
                "decDigits": 0,
                "groupSep": ",",
                "decSep": ".",
                "groupSizes": [
                    3
                ],
                "pattern": {
                    "pos": "$n",
                    "neg": "-$n"
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
                    "pos": "n%",
                    "neg": "-n%"
                }
            },
            "named": {
                "currency": "#,##0",
                "number": "#",
                "decimal": "#.00"
            }
        },
        text: {}
    });

})();
