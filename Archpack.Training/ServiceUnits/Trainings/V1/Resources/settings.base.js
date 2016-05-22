/**
 * Web アプリケーションで定義する設定・定数です。
 */
(function () {

    App.define("App.settings.base", {
        dataTakeCount: 100,
        errorTypes: {
            inputError: "input_error",
            databaseError: "db_error",
            authenticationError: "authentication_error",
            conflictError: "conflict_error",
            systemError: "system_error",
            apiError: "api_error"
        }
    });

})();