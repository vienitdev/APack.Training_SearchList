/*global App */
///<reference path="../../../ts/core/culture.ts" />

(function () {

    "use strict";

    if (!App && !App.culture) {
        return;
    }

    App.culture("vi", {
        "name": "vi-VN",
        "engName": "Vietnamese (Vietnam)",
        "lang": "vi",
        "dateTimeFormat": {
            "months": {
                "shortNames": [
                    "Thg1", "Thg2", "Thg3", "Thg4", "Thg5", "Thg6",
                    "Thg7", "Thg8", "Thg9", "Thg10", "Thg11", "Thg12", ""
                ],
                "names": [
                    "Tháng Giêng",
                    "Tháng Hai",
                    "Tháng Ba",
                    "Tháng Tư",
                    "Tháng Năm",
                    "Tháng Sáu",
                    "Tháng Bảy",
                    "Tháng Tám",
                    "Tháng Chín",
                    "Tháng Mười",
                    "Tháng Mười Một",
                    "Tháng Mười Hai",
                    ""
                ]
            },
            "weekdays": {
                "shortNames": [
                    "CN",
                    "T2",
                    "T3",
                    "Tư",
                    "Năm",
                    "Sáu",
                    "Bảy"
                ],
                "names": [
                    "Chủ Nhật",
                    "Thứ Hai",
                    "Thứ Ba",
                    "Thứ Tư",
                    "Thứ Năm",
                    "Thứ Sáu",
                    "Thứ Bảy"
                ]
            },
            "meridiem": {
                "ante": "SA",
                "post": "CH"
            },
            "patterns": {
                "d": "dd/MM/yyyy",
                "D": "dd MMMM yyyy",
                "t": "h:mm tt",
                "T": "h:mm:ss tt",
                "f": "dd MMMM yyyy h:mm tt",
                "F": "dd MMMM yyyy h:mm:ss tt",
                "M": "dd MMMM",
                "S": "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                "Y": "MMMM yyyy"
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
            "groupSep": ".",
            "decSep": ",",
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
                "symbol": "₫",
                "decDigits": 2,
                "groupSep": ".",
                "decSep": ",",
                "groupSizes": [
                    3
                ],
                "pattern": {
                    "pos": "n $",
                    "neg": "-n $"
                }
            },
            "percent": {
                "symbol": "%",
                "permilleSynbol": "‰",
                "decDigits": 2,
                "groupSep": ".",
                "decSep": ",",
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
    });
})();
