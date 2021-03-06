var pageData = {
    accountId: null
};
$(document).ready(function () {
    pageData.accountId = $('#hd-account-id').val();
    pageData.parentId = $('#hd-parent-id').val();
    $('#btn-volver').click(function () {
        gotoController('Account/Index/' + pageData.parentId);
    });
    $('#btn-verificar-datos').click(function () {
        verificarDatos();
    });
    $('#btn-generar-periodo').click(function () {
        $.blockUI();
        $.ajax({
            url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId + '/verificar-datos',
            type: 'GET',
            success: function (data, status, xhr) {
                $.unblockUI();
                if (data.Value) {
                    generarPeriodo();
                } else {
                    if (data.HasErrors) {
                        showErrors([Globalize.localize('MessageNoGenerarPeriodo')]);
                    } else {
                        showMessage(Globalize.localize('MessageSincronizarPrimero'));
                    }
                }
            },
            error: function (xhr) {
                if (xhr.status !== 500) {
                    var result = $.parseJSON(xhr.responseText);
                    showErrors(result.Errors);
                    $.unblockUI();
                } else {
                    showApplicationFatalErrorMessage();
                }
            }
        });
    });
    getAccountExtend();
});

function verificarDatos() {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId + '/verificar-datos',
        type: 'GET',
        success: function (data, status, xhr) {
            $.unblockUI();
            if (data.Value) {
                showMessage(Globalize.localize('MessageDatosSincronizados'));
            } else {
                if (data.HasErrors) {
                    showErrors(data.Errors);
                } else {
                    var messages = [];
                    if (data.HasWarnings) {
                        messages = messages.concat(data.Warnings);
                    }
                    if (data.HasMessages) {
                        messages = messages.concat(data.Messages);
                    }
                    showErrors(null, messages, sincronizarDatos);
                }
            }
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
                $.unblockUI();
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}
function getAccountExtend() {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId,
        type: 'GET',
        success: function (data, status, xhr) {
            $('.title-account').text(data.Name);
            $('#txt-id-gestor').text(data.Estudio.Id);
            $('#txt-nombre-estudio').text(data.Estudio.Nombre);
            var ul = $('ul.list-asiganturas'); ul.empty();
            $.each(data.Asignaturas, function (index, item) {
                var asociado = item.CourseId != null ? true : false;
                var href = asociado
                    ? appData.siteUrl + 'Course/Details/' + pageData.parentId + '/' + item.CourseId
                    : '';
                var li = asociado
                    ? $('<li class="list-group-item"><a href="' + href + '">' + item.Nombre + '</a></li>')
                    : $('<li class="list-group-item">' + item.Nombre + '</li>');
                li.data('data', item);
                ul.append(li);
            });
            ul = $('ul.list-periodos-activos'); ul.empty();
            $.each(data.PeriodoActivos, function (index, item) {
                var asociado = item.AccountIdPeriodo != null ? true : false;
                var href = asociado
                    ? appData.siteUrl + 'Account/DetailsPeriodo/' + pageData.parentId + '/' + item.AccountIdPeriodo
                    : '';
                var li = asociado
                    ? $('<li class="list-group-item"><a href="' + href + '">' + item.Nombre + '</a></li>')
                    : $('<li class="list-group-item">' + item.Nombre + '</li>');
                li.data('data', item);
                ul.append(li);
            });
            $.unblockUI();
            forceResize();
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
                $.unblockUI();
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}
function sincronizarDatos() {
    var popup = null;
    showConfirmation({
        message: Globalize.localize('MessageDatosModificados'),
        open: function (content) {
            popup = content;
        },
        btnYes: function () {
            popup.block();
            $.ajax({
                url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId,
                type: 'PUT',
                success: function (data, status, xhr) {
                    popup.modal('hide');
                    getAccountExtend();
                },
                error: function (xhr) {
                    if (xhr.status !== 500) {
                        var result = $.parseJSON(xhr.responseText);
                        showErrors(result.Errors);
                        popup.modal('hide');
                    } else {
                        showApplicationFatalErrorMessage();
                    }
                }
            });
        },
        btnNo: function () {
            popup.modal('hide');
        }
    });
}
function generarPeriodo() {
    var dialog = null;
    var dialogBody = null;
    showPopupPage({
        title: Globalize.localize('TitleGenerarPeriodo'),
        url: appData.siteUrl + 'Account/PopupGenerarPeriodo',
        buttons: {
            Guardar: function () {
                var param = {};
                param.Id = pageData.accountId;
                param.IdPeriodoMatriculacion = $('#cbx-periodo-activo', dialog).jquick('getId');
                param.Name = $('#txt-nombre-subcuenta-periodo', dialog).val();
                dialogBody.block();
                $.ajax({
                    url: appData.siteUrl + 'api/v1/canvas/accounts',
                    type: 'POST',
                    data: JSON.stringify(param),
                    success: function (data, status, xhr) {
                        dialog.modal('hide');
                        showMessageCorrectamente(true);
                        getAccountExtend();
                    },
                    error: function (xhr) {
                        if (xhr.status !== 500) {
                            var result = $.parseJSON(xhr.responseText);
                            showErrors(result.Errors, null, function() {
                                dialogBody.unblock();
                            });
                        } else {
                            showApplicationFatalErrorMessage();
                        }
                    }
                });
            },
            Cancelar: function () {
                dialog.modal('hide');
            }
        }
    }, function (content, popup) {
        dialog = popup;
        dialogBody = content;
        $('#cbx-periodo-activo', content).jquick({
            type: 'GET',
            url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId + '/periodo-activos',
            fnSelect: function () {
                var value = $('#cbx-periodo-activo', content).jquick('getValue');
                $('#txt-nombre-subcuenta-periodo', content).val(value);
                $('#txt-nombre-subcuenta-periodo', content).focus();
            }
        });
    });
}