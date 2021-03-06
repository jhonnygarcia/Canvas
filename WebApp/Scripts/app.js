window.getHeight = function (doc) {
    return doc.body.offsetHeight + 50;
}
window.forceResize = function (height) {
    var current_height = height;
    if (current_height == undefined) {
        current_height = getHeight(document);
    }

    parent.postMessage('{"subject":"lti.frameResize", "height":' + current_height + '}', '*');
}
$(document).ready(function () {
    deleteEntityGlobalize();
    /**************************************************************************/
    $.blockUI.defaults.message = appData.imgLoading;
    $.blockUI.defaults.css.border = "0px";
    $.blockUI.defaults.css.backgroundColor = "transparent";
    $.blockUI.defaults.overlayCSS.backgroundColor = "#000000";
    $.blockUI.defaults.overlayCSS.opacity = 0.5;
    $.blockUI.defaults.filter = "Alpha(Opacity=50)";
    $.blockUI.defaults.baseZ = 10;

    $.ajaxSetup({
        cache: false,
        contentType: "application/json"
    });

    $.jquickSetup({
        changeParams: {
            SearchText: 'searchText'
        },
        showToolbar: 'right'
    });
    $.fn.datepicker.defaults.format = appData.formatDate.toLowerCase();
    $('.input-group.date').datepicker({
        language: appData.defaultCulture,
        autoclose: true
    });
});
$.tableConfig = {
    "language": {
        "processing": "Procesando...",
        "lengthMenu": "Mostrar _MENU_ registros",
        "zeroRecords": "No se encontraron resultados",
        "info": "Mostrando desde _START_ hasta _END_ de _TOTAL_ registros",
        "infoEmpty": "Mostrando desde 0 hasta 0 de 0 registros",
        "sInfoFiltered": "(filtrado de _MAX_ registros en total)",
        "sInfoPostFix": "",
        "search": "Buscar:",
        "paginate": {
            "first": "Primero",
            "previous": "Anterior",
            "next": "Siguiente",
            "last": "&Uacute;ltimo"
        }
    }
}
// Prototipando Globalize para acatar la recomendación de no extener tipos integrados
Globalize.formatDateToISOString = function (date) {
    var datetimeMoment = moment(date);
    var dateMoment = moment.utc({
        year: datetimeMoment.year(),
        month: datetimeMoment.month(),
        day: datetimeMoment.date()
    });
    return dateMoment.toISOString();
};
Globalize.formatDateToUTCISOString = function (date) {
    var dateUtc = new Date(
        date.getUTCFullYear(),
        date.getUTCMonth(),
        date.getUTCDate(),
        date.getUTCHours(),
        date.getUTCMinutes(),
        date.getUTCSeconds());
    return Globalize.formatDateToISOString(dateUtc);
};
Globalize.parseDateISOString = function (isoString) {
    var dateMoment = moment(isoString, "YYYY-MM-DDTHH:mm:ss");
    return dateMoment.toDate();
};
Globalize.formatDateUsingMask = function (date, mask) {
    var dateMoment = moment(date);
    if (mask) {
        return dateMoment.format(mask);
    }
    return dateMoment.format('DD/MM/YYYY');
};
String.prototype.format = function () {
    var s = this,
            i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};
function gotoController(controller) {
    if (controller) {
        window.location.href = appData.siteUrl + controller;
    } else {
        window.location.href = appData.siteUrl;
    }
}
function showMessage(text, close) {
    var content = $('<div></div>').appendTo('body');
    content.append('<div class="modal fade modal-show-message" tabindex="-1" role="dialog" aria-labelledby="label-error-modal" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
            '<div class="modal-content">' +
            '<div class="modal-header btn-info">' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '<h4 class="modal-title label-message-modal">' + Globalize.localize('TitleApplication') + '</h4>' +
            '</div>' +
            '<div class="modal-body modal-message-body">' +
            '</div>' +
            '<div class="modal-footer modal-message-footer">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
    var body = $('.modal-message-body', content);
    body.append('<p class="text-center text-info center-block">' + text + '</p>');

    $('.modal-show-message', content).on('hidden.bs.modal', function (e) {
        content.remove();
    });

    $('.modal-show-message', content)
        .draggable({
            handle: '.modal-header'
        })
        .modal('show');
        //.modal({ backdrop: 'static' })

    if (close && close == true) {
        setTimeout(function () {
            $('.modal-show-message', content).modal('hide');
        }, 3000);
    }
}
function showCustomContent(htmlBody, fnClose) {
    var content = $('<div></div>').appendTo('body');
    content.append('<div class="modal fade modal-show-custom" tabindex="-1" role="dialog" aria-labelledby="label-error-modal" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
            '<div class="modal-content">' +
            '<div class="modal-header btn-info">' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '<h4 class="modal-title label-show-custom-modal">' + Globalize.localize('TitleApplication') + '</h4>' +
            '</div>' +
            '<div class="modal-body modal-show-custom-body">' +
            '</div>' +
            '<div class="modal-footer modal-show-custom-footer">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
    var body = $('.modal-show-custom-body', content);
    body.html(htmlBody);

    $('.modal-show-custom', content).on('hidden.bs.modal', function (e) {
        content.remove();
        if (fnClose) {
            fnClose();
        }
    });

    $('.modal-show-custom', content)
        .draggable({
            handle: '.modal-header'
        })
        .modal({ backdrop: 'static' })
        .modal('show');
}
function showMessageCorrectamente(close) {
    showMessage(Globalize.localize('MessageOperacionExitosamente'), close);
}
function showErrors(errors, warnings, fnClose) {
    var content = $('<div></div>').appendTo('body');
    content.append('<div class="modal fade modal-show-error" tabindex="-1" role="dialog" aria-labelledby="label-error-modal" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
            '<div class="modal-content">' +
            '<div class="modal-header btn-danger">' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '<h4 class="modal-title label-error-modal">' + Globalize.localize('TitleApplication') + '</h4>' +
            '</div>' +
            '<div class="modal-body modal-error-body">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
    var body = $('.modal-error-body', content);
    if (errors) {
        $.each(errors, function (index, text) {
            body.append('<p class="text-danger"><span class="glyphicon glyphicon-remove"></span> ' + text + '</p>');
        });
    }
    if (warnings) {
        $.each(warnings, function (index, text) {
            body.append('<p class="text-warning"><span class="glyphicon glyphicon-warning-sign"></span> ' + text + '</p>');
        });
    }
    $('.modal-show-error', content).on('hidden.bs.modal', function (e) {
        content.remove();
        if (fnClose && $.isFunction(fnClose)) {
            fnClose();
        }
    });
    $('.modal-show-error', content).draggable({
        handle: '.modal-header'
    }).modal('show');
}
function showPopupPage(config, callback) {
    var content = $('<div></div>').appendTo('body');
    content.append('<div class="modal fade modal-show-popup" tabindex="-1" role="dialog" aria-labelledby="label-popup-modal" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '<h4 class="modal-title">' + config.title + '</h4>' +
            '</div>' +
            '<div class="modal-body modal-popup-body">' +
            '</div>' +
            '<div class="modal-footer modal-popup-footer">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
    if (config.buttons) {
        var footer = $('.modal-footer', content);
        $.each(config.buttons, function (key, value) {
            var button = $('<button type="button" class="btn btn-default">' + key + '</button>').click(value);
            footer.append(button);
        });
    }

    $('.modal-show-popup', content).on('hidden.bs.modal', function (e) {
        content.remove();
    });

    $('.modal-show-popup', content).draggable({
            handle: '.modal-header'
        })
        .modal({ backdrop: 'static' });

    var body = $('.modal-popup-body', content);
    body.block();
    $.ajax({
        url: config.url,
        type: 'POST',
        success: function(data) {
            body.unblock();
            body.html(data);
            $('.modal-show-popup', content).modal('show');
            callback(body, $('.modal-show-popup', content));
        }
    });

}
function showConfirmation(config) {
    var content = $('<div></div>').appendTo('body');
    content.append('<div class="modal fade modal-show-confirmation" tabindex="-1" role="dialog" aria-labelledby="label-confirmation-modal" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '<h4 class="modal-title label-confirmation-modal">' + Globalize.localize('TitleApplication') + '</h4>' +
            '</div>' +
            '<div class="modal-body modal-confirmation-body">' +
            '</div>' +
            '<div class="modal-footer modal-confirmation-footer">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');

    $('.modal-confirmation-body', content).append('<p>' + (config.message ? config.message : '') + '</p>');

    var footer = $('.modal-footer', content);
    if (config.btnYes) {
        $('<button type="button" class="btn btn btn-default">' + Globalize.localize('TextSi') + '</button>')
            .click(config.btnYes)
            .appendTo(footer);
    }
    if (config.btnNo) {
        $('<button type="button" class="btn btn btn-default">' + Globalize.localize('TextNo') + '</button>')
            .click(function() {
                //$('.modal-show-confirmation', content).modal('hide');
                config.btnNo();
            })
            .appendTo(footer);
    }

    $('.modal-show-confirmation', content).on('hidden.bs.modal', function (e) {
        content.remove();
    });

    $('.modal-show-confirmation', content)
        .draggable({
            handle: '.modal-header'
        })
        .modal('show');
    if (config.open) {
        config.open($('.modal-show-confirmation', content));
    }
}
function isNull(value) {
    return (value == null || "undefined" === typeof value);
}
function isDate(str) {
    var d = moment(str, 'D/M/YYYY');
    if (d == null || !d.isValid()) return false;
    return str.indexOf(d.format('D/M/YYYY')) >= 0 || str.indexOf(d.format('DD/MM/YYYY')) >= 0;

    //return str.indexOf(d.format('D/M/YYYY')) >= 0
    //    || str.indexOf(d.format('DD/MM/YYYY')) >= 0
    //    || str.indexOf(d.format('D/M/YY')) >= 0
    //    || str.indexOf(d.format('DD/MM/YY')) >= 0;
}
function deleteEntityGlobalize() {
    var ta = $("<textarea />");
    $.each(Globalize.culture().messages, function (item, value) {
        Globalize.culture().messages[item] = ta.html(value).val();
    });
}
function showPopupProgress(config, fnCallback, fnClose, extraData) {
    var content = $('<div></div>').appendTo('body');
    content.append('<div class="modal fade modal-progressbar-popup" tabindex="-1" role="dialog" aria-labelledby="label-progressbar-modal" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
            //'<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '<h4 class="modal-title label-progressbar-modal">' + config.title + '</h4>' +
            '</div>' +
            '<div class="modal-body modal-progressbar-body">' +
            '</div>' +
            '<div class="modal-footer modal-progressbar-footer">' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>');
    $('.modal-progressbar-popup', content).on('hidden.bs.modal', function (e) {
        content.remove();
        if (fnClose) {
            fnClose(extraData);
        }
    });
    $('.modal-progressbar-popup', content)
    .draggable({
        handle: '.modal-header'
    })
    .modal({ backdrop: 'static' });
    var body = $('.modal-progressbar-body', content);
    body.html('<div></div>');
    var progress = $('div', body).bprogressbar({
        fnComplete:function() {
            $('.modal-progressbar-popup button.close', content).append('<span aria-hidden="true">&times;</span>');
            setTimeout(function () {
                if (content && $('.modal-progressbar-popup', content).length > 0)
                    $('.modal-progressbar-popup', content).modal('hide');
            }, 2000);
        }
    });
    progress.bprogressbar('setValue', 1);
    fnCallback(progress, extraData);
}
(function($) {
    $.widget("if.bprogressbar",{
        options: {
            fnComplete: null
        },
        _create: function() {
            var self = this,
                element = self.element,
                opc = self.options;
            self.clone = element.clone();
            self.value = 0;
            if (element.is('div')) {
                element.empty();
                element.addClass('progress');
                self.progressbar = $('<div class="progress-bar progress-bar-striped active" role="progressbar" ' +
                    'aria-valuemin="0" aria-valuemax="100" style="width:0%"></div>');
                element.append(self.progressbar);
            }
        },
        setValue: function(value) {
            var self = this,
                element = self.element,
                opc = self.options;
            self.value = value > 100 ? 100: value;
            self.progressbar.css('width', value + "%");
            self.progressbar.html(value + "%");
            if (self.value >= 100 && opc.fnComplete) {
                opc.fnComplete();
            }
        },
        getValue: function () {
            var self = this,
                element = self.element,
                opc = self.options;

            return self.value;
        },
        reset: function () {
            var self = this,
                element = self.element,
                opc = self.options;
            self.progressbar.css('width', "0%");
            self.progressbar.html('');
            self.value = 0;
        }
    });
})(jQuery);
function decode(param, values) {
    var retorno;
    $.each(values, function (index, value) {
        if ((index % 2) == 0) {
            if (param == value) {
                retorno = values[index + 1];
                return false;
            }
        }
    });
    if ((retorno == undefined) && ((values.length - 1) % 2 == 0)) {
        retorno = values[values.length - 1];
    }
    return retorno;
}