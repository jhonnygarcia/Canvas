var pageData = {
    accountId: null
};
$(document).ready(function () {
    pageData.accountId = $('#hd-account-id').val();
    getAccount();
});
function getAccount() {
    $.blockUI();
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/accounts/' + pageData.accountId,
        type: 'GET',
        success: function (data, status, xhr) {
            $('#title-account').text(data.Name);
            loadTreView();
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}
function loadTreView() {
    $.ajax({
        url: appData.siteUrl + 'api/v1/canvas/accounts/' + pageData.accountId + '/sub-accounts-courses',
        type: 'GET',
        success: function (data, status, xhr) {
            var dataTree = [];
            $.each(data, function (index, item) {
                var tags = [];
                if (item.IsAccountPeriodo) {
                    tags.push(Globalize.localize('TextDetalle'));
                } else {
                    if (item.SisId) {
                        tags.push(Globalize.localize('TextDetalle'));
                    }
                    if (!item.Generated) {
                        tags.push(Globalize.localize('TextAsociarEstudio'));
                    }
                }
                var node = {
                    id: item.Id,
                    href: item.HtmlUrl,
                    text: '<a href="' + item.HtmlUrl + '" target="_blank" class="account-node tree-node-text">' +
                        item.Name + (item.SisId ? ' <span class="text-sis-id">(SIS-ID: ' + item.SisId + ')</span>' : '') + '</a>',
                    data: item,
                    tags: tags,
                    state: { expanded: true }
                };
                node.nodes = [];
                if (item.Courses.length > 0) {
                    $.each(item.Courses, function (i, value) {
                        var subNode = {
                            id: value.Id,
                            href: value.HtmlUrl,
                            text: '<a href="' + value.HtmlUrl + '" target="_blank" class="course-node tree-node-text">' +
                                value.Name + (value.SisId ? ' <span class="text-sis-id">(SIS-ID: ' + value.SisId + ')</span>' : '') + '</a>',
                            data: value
                        };
                        var tags = [];
                        if (!item.IsAccountPeriodo) {
                            if (value.SisId) {
                                tags.push(Globalize.localize('TextDetalle'));
                            }
                            if (!item.Generated && item.SisId) {
                                tags.push(Globalize.localize('TextAsociarAsignatura'));
                            }
                        }
                        if (tags.length > 0) {
                            subNode.tags = tags;
                        }
                        node.nodes.push(subNode);
                    });
                }
                dataTree.push(node);
            });
            $('#tree-account-courses').treeview({
                showBorder: false,
                showTags: true,
                levels: 2,
                enableLinks: true,
                highlightSelected: false,
                data: dataTree
            });
            $('#tree-account-courses').on('click', function (event) {
                if ($(event.target).is('span') && $(event.target).hasClass('badge')) {

                    var isEstudio = $(event.target).text() == Globalize.localize('TextAsociarEstudio');
                    var isAsignatura = $(event.target).text() == Globalize.localize('TextAsociarAsignatura');
                    var isDetalle = $(event.target).text() == Globalize.localize('TextDetalle');

                    if (isEstudio) {
                        clickAccountAsociar(event.target);
                    }
                    if (isAsignatura) {
                        clickCourseAsociar(event.target);
                    }
                    if (isDetalle) {
                        if ($('a.course-node', $(event.target).parent()).length > 0) {
                            clickDetailsCourse(event.target);
                        }
                        if ($('a.account-node', $(event.target).parent()).length > 0) {
                            var nodeId = $(event.target).parent().attr('data-nodeid');
                            var node = $('#tree-account-courses').treeview('getNode', nodeId);
                            if (!node.data.IsAccountPeriodo) {
                                clickDetailsAccount(event.target);
                            } else {
                                gotoController('Account/DetailsPeriodo/' +pageData.accountId + '/' + node.id);
                            }
                        }
                    }
                }
            });
            $.unblockUI();
            forceResize();
        },
        error: function (xhr) {
            if (xhr.status !== 500) {
                var result = $.parseJSON(xhr.responseText);
                showErrors(result.Errors);
            } else {
                showApplicationFatalErrorMessage();
            }
        }
    });
}

function clickDetailsAccount(target) {
    var nodeId = $(target).parents('li').attr('data-nodeid');
    var node = $('#tree-account-courses').treeview('getNode', nodeId);
    gotoController('Account/Details/' + pageData.accountId + '/' + node.id);
}
function clickDetailsCourse(target) {
    var nodeId = $(target).parents('li').attr('data-nodeid');
    var node = $('#tree-account-courses').treeview('getNode', nodeId);
    gotoController('Course/Details/' + pageData.accountId + '/' + node.id);
}
function clickAccountAsociar(target) {
    var nodeId = $(target).parents('li').attr('data-nodeid');
    var data = $('#tree-account-courses').treeview('getNode', nodeId);
    var dialog = null;
    var dialogBody = null;
    showPopupPage({
        title: Globalize.localize('TextAsociarUnEstudio'),
        url: appData.siteUrl + 'Account/PopupAsociarEstudio',
        buttons: {
            Guardar: function () {
                var param = {};
                var warnigs = [];
                param.Estudio = {
                    Id: $('#cbx-estudio', dialog).jquick('getId'),
                    Value: $('#cbx-estudio', dialog).jquick('getValue')
                };
                param.Name = data.data.Name;
                if (!param.Estudio.Id) {
                    warnigs.push(Globalize.localize('ErrorSeleccioneEstudio'));
                }
                if (warnigs.length > 0) {
                    showErrors(null, warnigs);
                    return true;
                }
                dialogBody.block();
                $.ajax({
                    url: appData.siteUrl + 'api/v1/canvas/accounts/' + data.id,
                    type: 'PUT',
                    data: JSON.stringify(param),
                    success: function (data, status, xhr) {
                        showMessageCorrectamente(true);
                        dialog.modal('hide');
                        loadTreView();
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
        $('#cbx-estudio', content).jquick({
            type: 'GET',
            url: appData.siteUrl + 'api/v1/gestor/estudios'
        });
        if (data.data.Estudio) {
            $('#cbx-estudio', content)
                .jquick('setId', data.data.Estudio.Id)
                .jquick('setValue', data.data.Estudio.Value);
        }
    });
}
function clickCourseAsociar(target) {
    var nodeId = $(target).parents('li').attr('data-nodeid');
    var node = $('#tree-account-courses').treeview('getNode', nodeId);
    var parentNode = $('#tree-account-courses').treeview('getNode', node.parentId);
    var dialog = null;
    var dialogBody = null;
    showPopupPage({
        title: Globalize.localize('TextAsociarUnaAsignatura'),
        url: appData.siteUrl + 'Account/PopupAsociarAsignatura',
        buttons: {
            Guardar: function () {
                var param = {};
                var warnigs = [];
                param.Asignatura = {
                    Id: $('#cbx-asignatura', dialog).jquick('getId'),
                    Value: $('#cbx-asignatura', dialog).jquick('getValue')
                };
                if (!param.Asignatura.Id) {
                    warnigs.push(Globalize.localize('ErrorSeleccioneAsignatura'));
                }
                param.Name = node.data.Name;
                param.AccountId = parentNode.id;
                if (warnigs.length > 0) {
                    showErrors(null, warnigs);
                    return true;
                }
                dialogBody.block();
                $.ajax({
                    url: appData.siteUrl + 'api/v1/canvas/courses/' + node.id,
                    type: 'PUT',
                    data: JSON.stringify(param),
                    success: function (data, status, xhr) {
                        showMessageCorrectamente(true);
                        dialog.modal('hide');
                        loadTreView();
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
        $('#cbx-asignatura', content).jquick({
            type: 'GET',
            url: appData.siteUrl + 'api/v1/gestor/asignaturas',
            fnParams: function () {
                return {
                    idEstudio: parentNode.data.SisId
                }
            }
        });
        if (node.data.Asignatura) {
            $('#cbx-asignatura', content)
                .jquick('setId', node.data.Asignatura.Id)
                .jquick('setValue', node.data.Asignatura.Value);
        }
    });
}