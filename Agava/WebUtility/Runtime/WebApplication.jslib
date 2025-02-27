const library = {

    $webApplication: {
        initialize: function (onInBackgroundChangeCallbackPtr) {
            document.addEventListener('pointerdown', function () {
                window.focus();
            });

            let isInBackground = false;

            function handleStateChange(newState) {
                if (newState !== isInBackground) {
                    isInBackground = newState;
                    dynCall('vi', onInBackgroundChangeCallbackPtr, [isInBackground]);
                }
            }

            document.addEventListener('visibilitychange', function () {
                handleStateChange(document.hidden);
            });

            window.addEventListener('blur', function () {
                handleStateChange(true);
            });

            window.addEventListener('focus', function () {
                handleStateChange(false);
            });
        },

        getInBackground: function () {
            return document.hidden;
        },
    },

    WebApplicationInitialize: function (onInBackgroundChangeCallbackPtr) {
        webApplication.initialize(onInBackgroundChangeCallbackPtr);
    },

    GetWebApplicationInBackground: function () {
        return webApplication.getInBackground();
    },
}

autoAddDeps(library, '$webApplication');
mergeInto(LibraryManager.library, library);
