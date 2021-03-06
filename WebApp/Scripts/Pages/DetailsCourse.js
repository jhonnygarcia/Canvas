var pageData = {
    courseId: null
};
$(document).ready(function () {
    pageData.courseId = $('#hd-course-id').val();
    pageData.parentAccountId = $('#hd-account-id').val();//accountId de la cuenta padre de la subcuenta que contiene este curso
    $('#btn-volver').click(function() {
        gotoController('Account/Index/' + pageData.parentAccountId);
    });
    $('#btn-migrar-curso').click(function () {
        var dialog = null;
        var dialogBody = null;
        showPopupPage({
            title: Globalize.localize('TitleMigracionCurso'),
            url: appData.siteUrl + 'Account/PopupAccountPeriodo',
            buttons: {
                Guardar: function () {
                    var selected = $('#cbx-account-generate', dialogBody).jquick('getId');
                    if (selected == null) {
                        showErrors([Globalize.localize('ErrorSeleccioneUnaCuenta')]);
                    } else {
                        hasCourseGenerate(dialog, dialogBody, selected);
                    }
                },
                Cancelar: function () {
                    dialog.modal('hide');
                }
            }
        }, function (content, popup) {
            dialog = popup;
            dialogBody = content;
            $('#cbx-account-generate', content).jquick({
                type: 'GET',
                url: appData.siteUrl + 'api/v1/canvas-extend/accounts-generates',
                fnParams: function() {
                    return {
                        accountId: pageData.course.Account.AccountId
                    }
                }
            });
        });
    });
    getCourseExtend();
});
function getCourseExtend() {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas-extend/courses/' + pageData.courseId,
        type: 'GET',
        success: function (data, status, xhr) {
            pageData.course = data;
            $('.title-course').text(data.Name);
            $('#txt-id-gestor').text(data.Asignatura.Id);
            $('#txt-nombre-asignatura').text(data.Asignatura.Nombre);
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
function hasCourseGenerate(dialog, content, accountSelected) {
    content.block();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/accounts/' + accountSelected + '/has-course-generate/' + pageData.courseId,
        type: 'GET',
        success: function (data, status, xhr) {
            dialog.modal('hide');
            if (data) {
                var popup = null;
                showConfirmation({
                    message: Globalize.localize('MessageSeRecrearaElCurso'),
                    open: function (content) {
                        popup = content;
                    },
                    btnYes: function () {
                        popup.modal('hide');
                        MigrateCourse(accountSelected, pageData.courseId);
                    },
                    btnNo: function () {
                        popup.modal('hide');
                    }
                });
            } else {
                MigrateCourse(accountSelected, pageData.courseId);
            }
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
                dialog.modal('hide');
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}
function MigrateCourse(account, courseId) {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/accounts/' + account + '/migrar-cursos/' + courseId,
        type: 'POST',
        success: function (data, status, xhr) {
            $.unblockUI();
            showPopupProgress({
                title: Globalize.localize('TitleMigracionCurso')
            }, progressInfo, showTableMigration, data);
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
function showTableMigration(progressId) {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/progress/' + progressId + '/migration',
        type: 'GET',
        success: function (data, status, xhr) {
            $.unblockUI();
            var dialog = null;
            var dialogBody = null;
            showPopupPage({
                title: Globalize.localize('LabelResultadoMigracionCurso'),
                url: appData.siteUrl + 'Account/PopupTableMigration'
            }, function (content, popup) {
                dialog = popup;
                dialogBody = content;
                $('#tb-cursos', content).parent().parent().remove();
                var configTable = {
                    searching: false,
                    "paging": false,
                    "info": false,
                    "ordering": false,
                    "scrollY": "100px",
                    "scrollX": true,
                    "language": {
                        "zeroRecords": Globalize.localize('MessageSinIrregularidades')
                    },
                    "scrollCollapse": true
                };
                var tbAlumnos = $('#tb-alumnos', content).DataTable(configTable);
                var tbTareas = $('#tb-tareas', content).DataTable(configTable);
                var info = '<span class="btn-information glyphicon glyphicon-info-sign" data-trigger="hover" data-toggle="popover" data-container="body" title="{0}" data-content="{1}"></span>';
                $.each(data.Stundents, function (index, value) {
                    tbAlumnos.row.add([
                            value.Id, value.Nombres, value.Apellidos, value.Nif,
                            info.format(Globalize.localize('TitleEstudiante'),
                                Globalize.localize('CodeMessage' + value.Code))
                    ]).draw(false);
                });
                $.each(data.Assignments, function (index, value) {
                    tbTareas.row.add([
                            value.Id, '<a href="' + value.HtmlUrl + '" target="_blank">' + value.Name + '</a>',
                            info.format(Globalize.localize('TitleTarea'), Globalize.localize('CodeMessage' + value.Code))
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