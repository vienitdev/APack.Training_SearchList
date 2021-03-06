﻿describe("画面表示", function () {

    "use strict";

    describe("画面表示", function () {
        describe("ウィンドウタイトル", function () {
            it("「ログイン」と表示される", function () {
                expect(document.title).toBe("ログイン");
            });
        });

        describe("画面タイトル", function () {
            it("「ログイン」と表示される", function () {
                expect($(".page-title")).toHaveText("ログイン");
            });
        });

        describe("初期フォーカス", function () {
            it("ユーザーIDにフォーカスが当たっている", function () {
                expect($("#userName")).toBeFocused();
            });
        });

        describe("ヘッダー情報", function () {
            it("ユーザー名がtextboxで表示されている", function () {
                expect($.findP("userName").attr("placeholder")).toEqual("ユーザー名");
            });

            it("パスワードがpasswordで表示されている", function () {
                expect($.findP("password").attr("placeholder")).toEqual("パスワード");
            });

        });
    });
});

describe("定型画面機能定義", function () {

    "use strict";

    describe("初期表示処理", function () {

        describe("画面コントロールとイベントの紐づけ処理", function () {
            it("page.header.operations.login関数が呼び出される", function (done) {
                $("#login").trigger("click");
                setTimeout(function () {
                    expect($(".alert-message")).toBeVisible();
                    done();
                }, 500);
            });
        });
    });
});

describe("固有画面機能定義", function () {

    "use strict";

    describe("ログイン処理", function () {
        describe("入力検証処理", function () {
            it("ユーザー名テキストボックスに何も入力されていない場合にエラーとなる", function (done) {
                $.findP("userName").val("");
                $("#login").trigger("click");
                setTimeout(function () {
                    expect($(".alert-message")).toBeVisible();
                    done();
                }, 500);
            });
            it("ユーザー名に何も入力されていない場合のエラーメッセージは「ユーザー名は必須入力です」となる", function (done) {
                $.findP("userName").val("");
                $("#login").trigger("click");
                setTimeout(function () {
                    expect($(".alert-message ul li:eq( 0 ) .error")).toHaveText("ユーザー名 は入力必須です。");
                    done();
                }, 500);
            });

            it("パスワードに何も入力されていない場合にエラーとなる", function (done) {
                $.findP("userName").val("aa");
                $.findP("password").val("");
                $("#login").trigger("click");
                setTimeout(function () {
                    expect($(".alert-message")).toBeVisible();
                    done();
                }, 500);
            });
            it("パスワードに何も入力されていない場合のエラーメッセージは「パスワードは必須入力です」となる", function (done) {
                $.findP("userName").val("aa");
                $.findP("password").val("");
                $("#login").trigger("click");
                setTimeout(function () {
                    expect($(".alert-message ul li:eq( 0 ) .error")).toHaveText("パスワード は入力必須です。");
                    done();
                }, 500);
            });

        });
    });
});