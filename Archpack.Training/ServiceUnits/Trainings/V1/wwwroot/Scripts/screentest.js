; (function () {

    if (!jasmine || !blanket) {
        return;
    }
    if (!App || !App.page || App.page.mode !== "test") {
        return;
    }

    App.define("App.page.test", {
        run: App.noop
    });

    // modified by nakanishi
    var result = {}
    var specResults = [];
    result.spec = specResults;
    var isFirst = true;
    //

    //reporter
    jasmine.getEnv().addReporter({
        jasmineDone: function () {
            setTimeout(function () {
                var jasmineReporterElement = $(".jasmine_html-reporter"),
                    reporterContainer = $("<div class='test-reporter'>"),
                    blanketReporterElement = $("#blanket-main");
                //jasmineReporterElement.remove();
                reporterContainer.append(blanketReporterElement);
                reporterContainer.append(jasmineReporterElement);

                // modified by nakanishi
                var downloadFile = result.description + "_" + resultDate(result.coverage.stats.end) + ".json";
                reporterContainer.append("<div><a id='download' href='#' download='" + downloadFile + "'>Download Test Result</a></div>");
                reporterContainer.find("#download").on('click', function() {
                    var blob = new Blob([JSON.stringify(result, null, 4)], { type: "application\/json" });

                    if (window.navigator.msSaveBlob) {
                        window.navigator.msSaveBlob(blob, downloadFile);
                    } else {
                        window.URL = window.URL || window.webkitURL;
                        reporterContainer.find("#download").attr("href", window.URL.createObjectURL(blob));
                    }
                });
                //

                $(document.body).append(reporterContainer);

            }, 100);
        },
        // modified by nakanishi
        suiteStarted: function (suite) {
            if (isFirst) {
                result.description = suite.description;
                isFirst = false;
            }
        },
        specDone: function (spec) {
            specResults.push(spec);
        }
        //
    });

    function elapsed(startTime, endTime) {
        return (endTime - startTime) / 1000;
    }

    function ISODateString(d) {
        function pad(n) { return n < 10 ? '0' + n : n; }

        return d.getFullYear() + '-' +
            pad(d.getMonth() + 1) + '-' +
            pad(d.getDate()) + 'T' +
            pad(d.getHours()) + ':' +
            pad(d.getMinutes()) + ':' +
            pad(d.getSeconds());
    }

    // modified by nakanishi
    function resultDate(d) {
        function pad(n) { return n < 10 ? '0' + n : n; }

        return d.getFullYear() +
            pad(d.getMonth() + 1) +
            pad(d.getDate()) +
            pad(d.getHours()) +
            pad(d.getMinutes()) +
            pad(d.getSeconds());
    }
    //

    function trim(str) {
        return str.replace(/^\s+/, "").replace(/\s+$/, "");
    }

    function escapeInvalidXmlChars(str) {
        return str.replace(/\&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/\>/g, "&gt;")
            .replace(/\"/g, "&quot;")
            .replace(/\'/g, "&apos;");
    }

    var BlanketReporter = function (savePath, consolidate, useDotNotation) {
        blanket.setupCoverage();
    };
    BlanketReporter.finished_at = null;
    BlanketReporter.prototype = {
        specStarted: function (spec) {
            blanket.onTestStart();
        },

        specDone: function (result) {
            var passed = result.status === "passed" ? 1 : 0;
            blanket.onTestDone(1, passed);
        },

        jasmineDone: function () {
            // modified by nakanishi
            // blanket.onTestsDone();
            var coverageInfo = blanket.onTestsDone();
            result.coverage = coverageInfo;
            //
        },

        log: function (str) {
            var console = jasmine.getGlobal().console;
            if (console && console.log) {
                console.log(str);
            }
        }
    };

    //override existing jasmine execute
    var originalJasmineExecute = jasmine.getEnv().execute;
    jasmine.getEnv().execute = function () { console.log("waiting for blanket..."); };

    blanket.options("timeout", 3000);

    blanket.beforeStartTestRunner({
        checkRequirejs: true,
        callback: function () {
            jasmine.getEnv().addReporter(new BlanketReporter());
            jasmine.getEnv().execute = originalJasmineExecute;

            if (App.page.test && App.isFunc(App.page.test.run)) {
                App.page.test.run();
            }
        }
    });
})();