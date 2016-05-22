/**
 * Web アプリケーションで定義する設定・定数です。
 */
(function () {

    App.define("App.settings", {
        dataTakeCount: 100,
        dialogDataTakeCount: 100,
        maxSearchDataCount: 1000,
        maxInputDataCount: 100,
        conflictStatus: 409, // TODO : サンプルコード修正後に削除する
        formats: {
            displayDate: "yyyy/MM/dd",
            displayMonth: "yyyy/MM",
            displayDateTime: "yyyy/MM/dd HH:mm:ss",
            displayJpDate: "gggyy年MM月dd日",
            displayJpMonth: "gggyy年MM月",
            displayJpDateTime: "gggyy年MM月dd日 HH時mm分ss秒",
            numberLikeYearMonth: "yyyyMM"
        }
    });

})();
