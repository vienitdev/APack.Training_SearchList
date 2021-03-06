/*global App */
///<reference path="../../../ts/core/culture.ts" />

(function () {

    "use strict";

    if (!App && !App.culture) {
        return;
    }

    App.culture("en", {
        "name": "en-US",
        "engName": "English (United States)",
        "lang": "en",
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
                "d": "M/d/yyyy",
                "D": "dddd, MMMM d, yyyy",
                "t": "h:mm tt",
                "T": "h:mm:ss tt",
                "f": "dddd, MMMM d, yyyy h:mm tt",
                "F": "dddd, MMMM d, yyyy h:mm:ss tt",
                "M": "MMMM d",
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
                "symbol": "$",
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
    });
})();




