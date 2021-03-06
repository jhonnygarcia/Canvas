var pageData = {
    accountId: null,
    accountPeriodo: null
};
$(document).ready(function () {
    pageData.accountId = $('#hd-account-id').val();
    pageData.parentId = $('#hd-parent-id').val();
    pageData.tbCursos = $('#tb-cursos');
    pageData.tbMigration = $('#tb-migrations');
    pageData.tbCursos.DataTable({
        columns: [{ orderable: true },
            { orderable: true },
            { orderable: true },
            { orderable: true },
            { orderable: true },
            { orderable: true },
            { orderable: false }],
        language: $.tableConfig.language,
        "createdRow": function (row, data, dataIndex) {
            $('.btn-delete', row).click(function () {
                var courseId = data[0];
                var popup = null;
                showConfirmation({
                    message: Globalize.localize('MessageEliminarCurso'),
                    open: function(content) {
                        popup = content;
                    },
                    btnYes: function () {
                        $('.modal-body', popup).block();
                        $.ajax({
                            url: appData.siteUrl + 'api/v1/canvas/courses/' + courseId,
                            type: 'DELETE',
                            success: function (data, status, xhr) {
                                popup.modal('hide');
                                showMessageCorrectamente(true);
                                getCourses();
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
                    btnNo:function() {
                        popup.modal('hide');
                    }
                });
            });
        },
        "initComplete": function () {
            forceResize();
        }
    });
    $('#btn-volver').click(function () {
        gotoController('Account/Index/' + pageData.parentId);
    });
    $('.btn-add').click(addPeriodoNoLectivo);
    $('.btn-remove').click(removePeriodoNoLectivo);
    $('#btn-verificar-datos').click(function () {
        var rows = $('#tb-periodo-no-lectivo tbody tr');
        var rangos = [];
        var errors = [];
        var isDateInvalid = false;
        var isDateIncioMayorFin = false;
        $.each(rows, function (index, row) {
            if (isDate($('.date-inicio input', row).val()) && isDate($('.date-fin input', row).val())) {
                var inicio = $('.date-inicio', row).datepicker('getDate');
                var fin = $('.date-fin', row).datepicker('getDate');
                if (inicio > fin) {
                    isDateIncioMayorFin = true;
                } else {
                    rangos.push({
                        Inicio: Globalize.formatDateToUTCISOString(inicio),
                        Fin: Globalize.formatDateToUTCISOString(fin)
                    });
                }
            } else {
                isDateInvalid = true;
            }
        });
        if (isDateIncioMayorFin) {
            errors.push(Globalize.localize('ErrorFechaInicioMayorFin'));
        }
        if (isDateInvalid) {
            errors.push(Globalize.localize('ErrorFechasIncorrectas'));
        }
        if (errors.length > 0) {
            showErrors(errors);
            return false;
        }
        var params = {};
        params.Fechas = rangos;
        $.blockUI();
        $.ajax({
            url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId + '/verificar-periodos-no-lectivos',
            type: 'POST',
            data: JSON.stringify(params),
            success: function (data, status, xhr) {
                $.unblockUI();
                if (data) {
                    showMessage(Globalize.localize('MessagePeriodosNoLectivosCorrectos'));
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
    $('#btn-guardar').click(function () {
        var rows = $('#tb-periodo-no-lectivo tbody tr');
        var rangos = [];
        var errors = [];
        var isDateInvalid = false;
        var isDateIncioMayorFin = false;
        $.each(rows, function (index, row) {
            if (isDate($('.date-inicio input', row).val()) && isDate($('.date-fin input', row).val())) {
                var inicio = $('.date-inicio', row).datepicker('getDate');
                var fin = $('.date-fin', row).datepicker('getDate');
                if (inicio > fin) {
                    isDateIncioMayorFin = true;
                } else {
                    var range = {
                        Inicio: Globalize.formatDateToUTCISOString(inicio),
                        Fin: Globalize.formatDateToUTCISOString(fin)
                    };
                    if ($(row).data('data') != null) {
                        range.Id = $(row).data('data').Id;
                    }
                    rangos.push(range);
                }
            } else {
                isDateInvalid = true;
            }
        });
        if (isDateIncioMayorFin) {
            errors.push(Globalize.localize('ErrorFechaInicioMayorFin'));
        }
        if (isDateInvalid) {
            errors.push(Globalize.localize('ErrorFechasIncorrectas'));
        }
        if (errors.length > 0) {
            showErrors(errors);
            return false;
        }
        var params = {};
        params.Fechas = rangos;
        $.blockUI();
        $.ajax({
            url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId + '/periodos-no-lectivos',
            type: 'POST',
            data: JSON.stringify(params),
            success: function (data, status, xhr) {
                $.unblockUI();
                loadRows(data.PeriodosNoLectivos);
                showMessageCorrectamente(true);
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
    $('#btn-migrar-cursos').click(function () {
        var popup = null;
        showConfirmation({
            message: Globalize.localize('MessageSeRecrearanCursos'),
            open: function (content) {
                popup = content;
            },
            btnYes: function () {
                popup.modal('hide');
                $.blockUI();
                $.ajax({
                    url: appData.siteUrl + 'api/v1/canvas/accounts/' + pageData.accountId + '/migrar-cursos',
                    type: 'POST',
                    success: function (data, status, xhr) {
                        $.unblockUI();
                        showPopupProgress({
                                title: Globalize.localize('LabelMigracionCursos')
                            },
                            progressInfo,
                            function (progressId) {
                                getCourses();
                                getMigrations();
                                showTableMigration(progressId);
                            },
                            data);
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
            },
            btnNo: function () {
                popup.modal('hide');
            }
        });
    });
    $('#btn-delete-sub-account').click(function() {
        var popup = null;
        showConfirmation({
            message: Globalize.localize('MessageEliminarCuenta'),
            open: function (content) {
                popup = content;
            },
            btnYes: function () {
                $('.modal-body', popup).block();
                $.ajax({
                    url: appData.siteUrl + 'api/v1/canvas/accounts/' + pageData.accountId,
                    type: 'DELETE',
                    success: function (data, status, xhr) {
                        popup.modal('hide');
                        var url = data.format(pageData.parentId);
                        url = url.format(pageData.parentId);
                        var message = '<p class="text-center text-info center-block">' + Globalize.localize('MessageDeleteAccount') + '</p>';
                        message += '<p class="text-center text-info center-block"><a href="' + url + '" target="_blank">' + url + '</a></p>';
                        showCustomContent(message, function() {
                            gotoController('Account/Index/' + pageData.parentId);
                        });
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
    });
    $(document).on('shown.bs.tab', 'a[data-toggle="tab"]', function (e) {
        forceResize();
    });
    getAccountPeriodo();
    getCourses();
    getMigrations();
});
function getAccountPeriodo() {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas-extend/accounts/' + pageData.accountId + '/periodo',
        type: 'GET',
        success: function (data, status, xhr) {
            $.unblockUI();
            pageData.accountPeriodo = data;
            $('.title-account').text(data.AccountCanvas.Name);
            $('#lb-codigo').text(data.AccountCanvas.SisId);
            $('#lb-periodo-matriculacion').text(data.NombrePeriodoMatriculacion);
            $('#lb-anio-academico').text(data.AnioAcademico);
            $('#lb-inicio-periodo').text(Globalize.formatDateUsingMask(Globalize.parseDateISOString(data.FechaInicio), 'DD/MM/YYYY'));
            $('#lb-fin-periodo').text(Globalize.formatDateUsingMask(Globalize.parseDateISOString(data.FechaFin), 'DD/MM/YYYY'));
            loadRows(data.PeriodosNoLectivos);
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
function showTableMigration(progressId, migrationId) {
    var api = migrationId
        ? 'api/v1/canvas/migrations/' + migrationId
        : 'api/v1/canvas/progress/' + progressId + '/migration';
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + api,
        type: 'GET',
        success: function (data, status, xhr) {
            $.unblockUI();
            var dialog = null;
            var dialogBody = null;
            showPopupPage({
                title: Globalize.localize('LabelResultadoMigracionCursos'),
                url: appData.siteUrl + 'Account/PopupTableMigration'
            }, function (content, popup) {
                dialog = popup;
                dialogBody = content;
                var configTable = {
                    searching: false,
                    "paging": false,
                    "info": false,
                    "language": {
                        "zeroRecords": Globalize.localize('MessageSinIrregularidades')
                    },
                    "ordering": false,
                    "scrollY": "100px",
                    "scrollX": true,
                    "scrollCollapse": true
                };
                var tbCursos = $('#tb-cursos', content).DataTable(configTable);
                var tbAlumnos = $('#tb-alumnos', content).DataTable(configTable);
                var tbTareas = $('#tb-tareas', content).DataTable(configTable);
                var info = '<span class="btn-information glyphicon glyphicon-info-sign" data-trigger="hover" data-toggle="popover" data-container="body" title="{0}" data-content="{1}"></span>';
                $.each(data.Courses,
                    function (index, value) {
                        tbCursos.row.add([
                                value.Id, '<a href="' + value.HtmlUrl + '" target="_blank">' + value.Name + '</a>',
                                value.SisId,
                                info.format(Globalize.localize('TitleCurso'),
                                    Globalize.localize('CodeMessage' + value.Code))
                        ]).draw(false);
                    });
                $.each(data.Stundents,
                    function (index, value) {
                        tbAlumnos.row.add([
                                value.Id, value.Nombres, value.Apellidos, value.Nif,
                                info.format(Globalize.localize('TitleEstudiante'),
                                    Globalize.localize('CodeMessage' + value.Code))
                        ]).draw(false);
                    });
                $.each(data.Assignments,
                    function (index, value) {
                        tbTareas.row.add([
                                value.Id, '<a href="' + value.HtmlUrl + '" target="_blank">' + value.Name + '</a>',
                                info.format(Globalize.localize('TitleTarea'),
                                    Globalize.localize('CodeMessage' + value.Code))
                        ]).draw(false);
                    });
                setTimeout(function () {
                    $($.fn.dataTable.tables(true)).css('width', '100%');
                    $($.fn.dataTable.tables(true)).DataTable().columns.adjust().draw();
                    $('.btn-information', dialogBody).popover({ trigger: "hover" });
                    forceResize();
                }, 200);
            });
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
function progressInfo(progress, progressId) {
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/progress/' + progressId,
        type: 'GET',
        success: function (data, status, xhr) {
            if (data.Finished) {
                progress.bprogressbar('setValue', 100);
            } else {
                progress.bprogressbar('setValue', data.Completion == 0 ? 1 : data.Completion);
                setTimeout(function () {
                    progressInfo(progress, progressId);
                }, 10000);
            }
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
                progress.bprogressbar('setValue', 100);
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}
function addPeriodoNoLectivo() {
    var currentRow = $(this).parents('tr');
    var tr = createRow();
    currentRow.after(tr);
    forceResize();
}
function loadRows(items) {
    var tbody = $('#tb-periodo-no-lectivo tbody');
    tbody.empty();
    if (items && items.length > 0) {
        $.each(items, function (index, value) {
            var newRow = createRow();
            newRow.data('data', value);
            tbody.append(newRow);
            $('.date-inicio', newRow).datepicker('setDate', Globalize.parseDateISOString(value.Inicio));
            $('.date-fin', newRow).datepicker('setDate', Globalize.parseDateISOString(value.Fin));
        });
    } else {
        var newRow = createRow();
        tbody.append(newRow);
    }
}
function removePeriodoNoLectivo() {
    if ($('#tb-periodo-no-lectivo tbody tr').length > 1) {
        $(this).parents('tr').remove();
    } else {
        var row = $('#tb-periodo-no-lectivo tbody tr:first');
        row.removeData();
        $('.date-fin', row).datepicker('setDate', null);
        $('.date-inicio', row).datepicker('setDate', null);
    }
}
function createRow() {
    var tr = $('<tr><td>' +
    '<p class="control-label">Inicio</p>' +
    '</td>' +
    '<td>' +
    '<div class="input-group date date-inicio">' +
    '<input type="text" class="form-control"><span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>' +
    '</div>' +
    '</td>' +
    '<td>' +
    '<p class="control-label">Fin</p>' +
    '</td>' +
    '<td>' +
    '<div class="input-group date date-fin">' +
    '<input type="text" class="form-control"><span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>' +
    '</div>' +
    '</td>' +
    '<td class="col-icons">' +
    '<span class="btn-add glyphicon glyphicon-plus" aria-hidden="true"></span>' +
    '<span class="btn-remove glyphicon glyphicon-remove" aria-hidden="true"></span>' +
    '</td>' +
    '</tr>');
    var startDate = Globalize.parseDateISOString(pageData.accountPeriodo.FechaInicio);
    $('.input-group.date', tr).datepicker({
        language: appData.defaultCulture,
        autoclose: true,
        startDate: startDate,
        endDate: Globalize.parseDateISOString(pageData.accountPeriodo.FechaFin),
        defaultViewDate: {
            year: startDate.getFullYear(),
            month: startDate.getMonth(),
            day: startDate.getDate()
        }
    });

    $('.input-group.date > input', tr).on('textchange', function () {
        if (isDate($(this).val())) {
            var date = $(this).parent().datepicker('getDate');
            if (date < Globalize.parseDateISOString(pageData.accountPeriodo.FechaInicio) ||
                date > Globalize.parseDateISOString(pageData.accountPeriodo.Fin)) {
                var input = $(this).css('background', '#f2baba');
                setTimeout(function () {
                    input.removeAttr('style');
                    input.val('');
                },
                    1000);
            }
        } else {
            $(this).removeAttr('style');
        }
    });

    $('.btn-add', tr).click(addPeriodoNoLectivo);
    $('.btn-remove', tr).click(removePeriodoNoLectivo);
    forceResize();
    return tr;
}
function getCourses() {
    pageData.tbCursos.block();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/accounts/' + pageData.accountId + '/courses',
        type: 'GET',
        success: function (data, status, xhr) {
            pageData.tbCursos.unblock();
            var table = new $.fn.dataTable.Api('#tb-cursos');
            table.clear().draw();
            $.each(data, function (index, value) {
                table.row.add([
                        value.Id, '<a href="' + value.Url + '" target="_blank">' + value.Name + '</a>', value.SisId,
                        (value.StartDate
                            ? Globalize.formatDateUsingMask(Globalize.parseDateISOString(value.StartDate), 'DD/MM/YYYY')
                            : ''),
                        (value.EndDate
                            ? Globalize.formatDateUsingMask(Globalize.parseDateISOString(value.EndDate), 'DD/MM/YYYY')
                            : ''),
                        value.TotalStudents,
                        '<span class="btn-delete glyphicon glyphicon-trash" style="cursor:pointer;cursor: hand;" aria-hidden="true"></span>'
                ], value).draw(false);
            });
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
function getMigrations() {
    var ul = $('.list-migrations');
    ul.block();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/accounts/' + pageData.accountId + '/migrations',
        type: 'GET',
        success: function (data, status, xhr) {
            ul.unblock();
            ul.empty();
            $.each(data, function (index, value) {
                var message = Globalize.localize('MessageMigration');
                var dateMoment = moment(value.Fin);
                dateMoment.locale('es');
                message = message.format(dateMoment.fromNow());
                var item = $('<li class="list-group-item">' + message + ' <span style="cursor:pointer; cursor: hand;" ' +
                        'class="lk-ver-detalle glyphicon glyphicon-list-alt" title="' + Globalize.localize('MessageVerDetalle') + '"></span></li>')
                    .data('data', value);
                ul.append(item);
            });
            $('.lk-ver-detalle', ul).click(function () {
                var migrationId = $(this).parent().data('data').Id;
                showTableMigration(null, migrationId);
                return false;
            });
            forceResize();
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
                pageData.tbMigration.unblock();
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}