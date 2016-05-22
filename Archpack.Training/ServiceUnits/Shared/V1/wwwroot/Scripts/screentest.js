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
                $(document.body).append(reporterContainer);
            }, 100);
        }
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
            blanket.onTestsDone();
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