; (function () {

    App.ui.page.normalizeErroMessage = function (error, notify, messages) {
        var normalizedErrors = App.ui.page.normalizeError(error);
        normalizedErrors.forEach(function (normalizedError) {
            if (normalizedError.errorType === App.settings.errorTypes.inputError) {
                normalizedError.errors.forEach(function (item) {
                    if (item.message) { notify.alert.message(normalizedError.message + item.message).show(); }
                });
                return;
            }
            if (normalizedError.errorType === App.settings.errorTypes.conflictError) {
                notify.alert.message(messages.UpdateConfilict).show();
                return;
            }
            notify.alert.message(normalizedError.message).show();
        });
    };
})();