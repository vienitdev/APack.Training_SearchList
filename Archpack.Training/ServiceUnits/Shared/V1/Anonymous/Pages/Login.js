; (function () {

    "use strict";

    /**
    * ページのレイアウト構造に対応するオブジェクトを定義します。
    */
    var page = {
        options: {　//ページ内で共有する設定
            urls: {
            },
            messages: App.str.text("Messages"),
            strings: App.str.text("StringContents")
        },
        values: {}, //ページ内で共有する値
        events: {   //イベント
            defaults: { //パターン既定イベント
                initialize: {},
                getParameters: {},
                initializeControl: {},
                initializeControlEvent: {},
                loadMasterData: {},
                loadData: {},
                loadDialogs: {},
                createValidator: {},
                validateAll: {},
                transfer: {}
            }
        },
        operations: { //ユーザー操作イベント
            defaults: {} //既定ユーザー操作イベント
        },
        //単票
        loginForm: {
            options: {　//単票で共有する設定
                urls: {},
                validations: {},
                bindOption: {}
            },
            values: {}, //単票で共有する値
            events: {   //単票のイベント
                defaults: { //単票の既定イベント
                    initialize: {},
                    createFilter: {},
                    change: {}
                },
            },
            operations: { //単票のユーザー操作イベント
                defaults: { //単票の既定ユーザー操作イベント
                    search: {}
                }
            }
        },
        dialogs: {
        }
    };


    /**
    * 画面の初期化処理を行います。
    */
    page.events.defaults.initialize = function () {

        var defer = $.Deferred();

        page.notify = App.ui.page.notify();
        App.ui.loading.show();

        page.events.defaults.getParameters();

        page.events.defaults.initializeControl();
        page.events.defaults.initializeControlEvent();

        page.loginForm.events.defaults.initialize();

        page.events.defaults.loadMasterData().then(function (result) {
            //マスターデータ以外の取得処理を実行します。
            return page.events.defaults.loadData();

        }).then(function () {

            return page.events.defaults.loadDialogs();

        }).then(function () {

            //TODO: 画面の初期化処理成功時の処理を記述します。

        }).fail(function (error) {
            App.ui.page.normalizeError(error).forEach(function (e) {
                page.notify.alert.message(e.message).show();
            });
        }).always(function (result) {
            $(":input:visible:not(:disabled):first").focus();
            App.ui.loading.close();

            defer.resolve();
        });

        return defer.promise();
    };

    /**
     * 遷移元から渡されたパラメーターの取得を行います。
     */
    page.events.defaults.getParameters = function () {
        //TODO: 遷移元から渡された値を取得し、保持する処理を記述します。
        //var query = App.uri.splitQuery(location.href);
    };

    /**
     * 画面コントロールの初期化処理を行います。
     */
    page.events.defaults.initializeControl = function () {

        //TODO: 画面全体で利用するコントロールの初期化処理をここに記述します。
        //TODO: ページのタイトルの取得先プロパティ名を修正すること
    };

    /**
     * コントロールへのイベントの紐づけを行います。
     */
    page.events.defaults.initializeControlEvent = function () {

        App.ui.page.applyCollapsePanel();

        //TODO: 画面全体で利用するコントロールのイベントの紐づけ処理をここに記述します。

        //TODO: 新規登録画面への遷移を有効にする場合は、page.options.urls.editPage プロパティの値を設定し
        //以下のコメントを解除します。
        //$("#add").on('click', function (e) {
        //    page.events.defaults.transfer();
        //});
    };

    /**
     * マスターデータのロード処理を実行します。
     */
    page.events.defaults.loadMasterData = function () {

        //TODO: 画面内のドロップダウンなどで利用されるマスターデータを取得し、画面にバインドする処理を記述します。
        return App.async.all({
            //service: $.ajax(App.ajax.webapi.get(/*TODO: マスターデータ取得サービスの URL*/))
        }).then(function (result) {
            //TODO:マスターデータ取得成功時のデータ設定処理を記述します
            var data = result.successes;

            //var serviceData = data.service;
        });
    };
    /**
     * マスターデータ以外のロード処理を実行します。
     */
    page.events.defaults.loadData = function () {
        //TODO: マスターデータ以外のデータのロード処理を記述します。
        return App.async.success();
    };

    page.events.defaults.loadDialogs = function () {
        //TODO: ダイアログがある場合はダイアログのロード処理を記述します。
        return App.async.all({
            //    employee: $.get(/*TODO: ダイアログの画面URL*/),
        }).then(function (result) {
            //    var dialogs = result.successes,
            //        dialog;
            //    $("#dialog-container").append(dialogs.employee);
            //    
            //    TODO: ダイアログで宣言しているグローバル変数（ダイアログ定義オブジェクト）を代入します。
            //    dialog = employeeDialog;

            //    page.dialogs.employeeDialog.dialog = dialog;
            //    dialog.events.defaults.initialize();
            //    dialog.events.defaults.complete = page.detail.events.setEmployee;
        });
    };

    /**
      * 指定された定義をもとにバリデータを作成します。
      * @param target バリデーション定義
      * @param options オプションに設定する値。指定されていない場合は、
      *                画面の success/fail/always のハンドル処理が指定されたオプションが設定されます。
      */
    page.events.defaults.createValidator = function (target, options) {
        page.events.defaults.validationSuccess =
           page.events.defaults.validationSuccess || App.ui.page.validationSuccess(page.notify);
        page.events.defaults.validationFail =
            page.events.defaults.validationFail || App.ui.page.validationFail(page.notify);
        page.events.defaults.validationAlways =
            page.events.defaults.validationAlways || App.ui.page.validationAlways(page.notify);

        return App.validation(target, options || {
            success: page.events.defaults.validationSuccess,
            fail: page.events.defaults.validationFail,
            always: page.events.defaults.validationAlways
        });
    };

    /**
     * すべてのバリデーションを実行します。
     */
    page.events.defaults.validateAll = function () {

        var validations = [];

        validations.push(page.loginForm.validator.validate());

        return App.async.all(validations);
    };


    //TODO: 以下の page.loginForm の各宣言は画面仕様ごとにことなるため、
    //不要の場合は削除してください。

    page.loginForm.options.validations = {
        //TODO: 単票のバリデーションの定義を記述します。
        userName: {
            rules: {
                required: true
            },
            options: {
                name: page.options.strings.UserName
            },
            messages: {
                required: page.options.messages.Required
            }
        },
        password: {
            rules: {
                required: true
            },
            options: {
                name: page.options.strings.Password
            },
            messages: {
                required: page.options.messages.Required
            }
        }

    };

    page.loginForm.options.bindOption = {
        // TODO: 単票の画面項目に値を設定する際に処理を追加します。
        appliers: {},
        // TODO: 単票の画面項目から値を取得する際に処理を追加します。
        converters: {}
    };

    /**
     * 単票の初期化処理を行います。
     */
    page.loginForm.events.defaults.initialize = function () {

        var defer = $.Deferred();

        var element = $(".main");
        page.loginForm.validator = element.validation(page.events.defaults.createValidator(page.loginForm.options.validations), {
            immediate: true
        });
        page.loginForm.element = element;
        App.ui.page.applyInput(element);

        //TODO: 単票の初期化処理をここに記述します。

        //TODO: 単票で利用するコントロールのイベントの紐づけ処理をここに記述します。
        var commands = $(".commands");
        commands.on("click", "#login", page.loginForm.operations.defaults.login);

        if (errorMessage.length > 0) {
            page.notify.alert.message(errorMessage).show();
        }

        return defer.promise();
    };

    /**
     * 単票にある入力項目の変更イベントの処理を行います。
     */
    page.loginForm.events.defaults.change = function () {
        //if ($("#nextsearch").is(":visible")) {
        //    $("#nextsearch").hide();
        //    page.notify.info.message(page.options.messages.HasChangeSearchCriteria).show();
        //}
    };


    /**
     * ログイン理を定義します。
     */
    page.loginForm.operations.defaults.login = function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();

        page.loginForm.validator.validate().then(function () {
            $("#mainform").submit();
        });

        return false;
    };


    /**
     * jQuery イベントで、ページの読み込み処理を定義します。
     */
    $(document).ready(function () {
        if (App.page.mode === "test") {
            App.page.test.run = function () {
                page.events.defaults.initialize().then(function () {
                    jasmine.getGlobal().page = page;
                    jasmine.getEnv().execute();
                });
            };
        } else {
            // ページの初期化処理を呼び出します。
            page.events.defaults.initialize();
        }
    });

    App.ui.page.extend(page);
})();