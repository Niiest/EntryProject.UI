var Index = (function ($) {
    var config = {
        uploadFileUrl: '',
        maxFileSize: 0,
        allowedFileExensions: [],
        defaultErrorMessage: '',
        spinner: null,
        spinnerTarget: null,
        form: null,
        alert: null,
        errorList: null,
        fileUploaderId: '',
        fileUploader: null,
        fileErrorMessage: null,
        selectedFileDisplay: null
    };

    /* Errors */

    function clearErrorMessages() {
        config.alert.hide();
        config.errorList.html('');

        config.fileErrorMessage.hide();
    }

    function appendErrorMessage(message) {
        if (!message) {
            return;
        }

        config.alert.show();

        var li = document.createElement('li');
        li.innerText = message;

        config.errorList.append(li);
    }

    /* End */

    /* Enable/disable controls */

    function getControls() {
        return config.form.find('input,button,select');
    }

    function enableControls() {
        var controls = getControls();
        controls.attr('disabled', null);

        $('#display-blocker').hide();
    }

    function disableControls() {
        var controls = getControls();
        controls.attr('disabled', 'disabled');

        $('#display-blocker').show();
    }

    /* End */

    /* Validation */

    function isFileSelected(fileElement) {
        return fileElement && fileElement.files && fileElement.files[0];
    }

    function isFileSizeValid(fileElement) {
        return isFileSelected(fileElement) && fileElement.files[0].size <= config.maxFileSize;
    }

    function isFileExtensionValid(fileElement) {
        if (isFileSelected(fileElement)) {
            var fileName = fileElement.files[0].name.toLowerCase();

            return config.allowedFileExensions.some(function (arrayValue) {
                return fileName.endsWith(arrayValue);
            });
        }

        return false;
    }

    function submitHandler(form) {
        var data = new FormData();

        var formData = $(form).serializeArray();
        for (var i = 0, el; el = formData[i]; i++) {
            data.append(el.name, el.value);
        }

        data.append('UserPhones', config.fileUploader[0].files[0]);

        clearErrorMessages();
        disableControls();
        config.spinner.spin(config.spinnerTarget);

        $.ajax({
            url: config.uploadFileUrl,
            type: 'POST',
            data: data,
            processData: false,
            contentType: false,
            dataType: 'json'
        })
        .always(function () {
            config.spinner.stop();
            enableControls();
        })
        .done(function (response) {
            config.alert.removeClass('hidden');

            if (response.hasErrors) {
                if (response.messages && response.messages[0]) {
                    for (var i = 0, msg; msg = response.messages[i]; i++) {
                        appendErrorMessage(msg);
                    }
                }
                else {
                    appendErrorMessage(config.defaultErrorMessage);
                }
            }

        }).fail(function () {
            appendErrorMessage(config.defaultErrorMessage);
        });
    };

    function setValidation() {
        $.validator.messages.required = 'Обязательное поле';

        $.validator.addMethod('fileSelected', function (value, element) {
            return isFileSelected(config.fileUploader[0]);
        }, 'Выберите файл');

        $.validator.addMethod('fileSize', function (value, element) {
            return isFileSizeValid(config.fileUploader[0]);
        }, 'Слишком большой файл');

        $.validator.addMethod('fileExtension', function (value, element) {
            return isFileExtensionValid(config.fileUploader[0]);
        }, 'Слишком большой файл');

        $.validator.addClassRules('fileUpload', {
            fileSelected: true,
            fileExtension: true,
            fileSize: true
        });

        config.form.validate({
            submitHandler: submitHandler,
            errorPlacement: function(error, element) {
                if (element.hasClass('error-after-parent')) {
                    error.insertAfter(element.parent());
                }
                else {
                    error.appendTo(element.parent());
                }
            }
        });
    }

    /* End */

    $(function () {
        /* Config initialization */

        config.form = $(document.forms['upload-form']);

        config.errorList = $('#error-list');
        config.alert = config.errorList.closest('[role="alert"]');

        config.fileUploader = $('#' + config.fileUploaderId);
        config.fileErrorMessage = $('#file-error-message');
        config.selectedFileDisplay = $('#selected-file-name');

        /* End */

        setValidation(config.form);

        config.fileUploader.on('change', function (event) {
            var element = this;
            if (isFileSelected(element)) {
                config.selectedFileDisplay.val(element.files[0].name);
            }
            else {
                config.selectedFileDisplay.val('');
            }
        });

        config.alert.children('[data-role="dismiss-alert"]').on('click', function (e) {
            config.alert.hide();
        });

        /* Spinner initialization */

        var opts = {
            lines: 10 // The number of lines to draw
            , length: 15 // The length of each line
            , width: 7 // The line thickness
            , radius: 15 // The radius of the inner circle
            , scale: 1 // Scales overall size of the spinner
            , corners: 1 // Corner roundness (0..1)
            , color: '#000' // #rgb or #rrggbb or array of colors
            , opacity: 0.25 // Opacity of the lines
            , rotate: 0 // The rotation offset
            , direction: 1 // 1: clockwise, -1: counterclockwise
            , speed: 1 // Rounds per second
            , fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
            , className: 'spinner' // The CSS class to assign to the spinner
            , top: '50%' // Top position relative to parent
            , left: '50%' // Left position relative to parent
            , hwaccel: false // Whether to use hardware acceleration
            , position: 'absolute' // Element positioning
        }

        config.spinner = new Spinner(opts);
        config.spinnerTarget = document.getElementById('spinner');

        /* End */
    });

    return {
        setConfig: function (cfg) {
            $.extend(config, cfg);
        }
    };
})(jQuery);
