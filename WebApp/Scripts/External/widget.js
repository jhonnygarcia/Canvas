(function ($) {
    $.fn.extend({
        donetyping: function (callback, timeout) {
            timeout = timeout || 1000; // 1 segundo de tiempo de espera predeterminado
            var timeoutReference,
            doneTyping = function (el) {
                if (!timeoutReference) return;
                timeoutReference = null;
                callback.call(el);
            };

            return this.each(function (i, el) {
                var $el = $(el);
                // Chrome Fix (Utilizar keyup sobre teclas para detectar retroceso)
                // gracias @palerdot
                $el.is(':input') && $el.on('keyup keypress paste', function (e) {
                    // Esto captura el botón de retroceso en Chrome, pero también impide
                    // el evento de activar demasiado preventivamente. Sin esta línea,
                    // usando tab / shift + tab hará que el elemento enfocado dispare la devolución de llamada.
                    if (e.type == 'keyup' && e.keyCode != 8) return;
                    // Compruebe si se ha establecido el tiempo de espera. Si tiene, "restablecer" el reloj y
                    // empezar de nuevo.
                    if (timeoutReference) clearTimeout(timeoutReference);
                    timeoutReference = setTimeout(function () {
                        // si llegamos aquí, nuestro tiempo de espera ha transcurrido. Disparar el
                        // llamar de vuelta
                        doneTyping(el);
                    }, timeout);
                }).on('blur', function () {
                    // Si podemos, disparar el evento desde que salimos del campo
                    doneTyping(el);
                });
            });

        }
    });
})(jQuery);

$.jquickSetup = function (config) {
    $.opc_default_jquick = $.extend(true, {}, $.jquickSetup.defaults, config);
};


$.jquickSetup.defaults = {
    simple_list: false,
    minLength: 3,
    delay: 1000,
    fnParams: null,
    pageCount: 10,
    type: 'GET',
    url: null,
    positionScrollMax: null,
    changeParams: {
        SearchText: 'searchText,Description,Descripcion',
        PageIndex: 'pageIndex,Pagina',
        PageCount: 'pageCount'
    },
    successData: {
        Id: 'Id',
        Description: 'Description,Descripcion'
    },
    pagination: {
        currentPage: 1,
        totalPages: 0
    },
    response: function (data) {
        return data;
    },
    api: true,
    toolbar: {
        fncopy: true,
        fnedit: null,
        fnsearch: null,
        fnadd: null
    },
    showToolbar: 'right',
    fnloadPagination: null,
    fnPreloadValue: null,
    fnPreloadLabel: null,
    fnPrecondition: null,
    fnSelect: null,
    fnReset: null
};

(function ($) {
    $.widget("if.jquick", {
        options: {},
        _create: function () {
            var self = this,
                element = self.element,
                opc = self.options;
            self.disabled = false;
            self.options = opc = $.extend(true, {}, $.opc_default_jquick, opc);
            if (((opc.fnloadPagination == null) || (opc.fnloadPagination == undefined))) {
                if (opc.api) {
                    opc.fnloadPagination = function (data, status, xhr) {
                        return xhr.getResponseHeader("X-TotalPages");
                    };
                }
                else {
                    opc.fnloadPagination = function (data, status, xhr) {
                        return data.Pagination.TotalPages;
                    };
                }
            }
            element.hide();
            self.container = $('<div></div>');
            self.container.addClass('jquick-container jquick-container-single');
            element.after(self.container);
            self.btnClose = '<abbr class="search-jquick-close"></abbr>';
            self._render();
            self._setEvents();
        },
        _setDefault: function () {
            var self = this,
                element = self.element,
                opc = self.options;
        },
        _renderItemsToolbar: function () {
            var self = this,
            element = self.element,
            opc = self.options;

            if (!((opc.toolbar.fncopy == null) || (opc.toolbar.fncopy == undefined))) {
                $('<li class="toolbar-copy"/>').appendTo(self.itemsToolbar)
                .click(function () {
                    try {
                        clipboard.copy(self.selectedText.text());
                    } catch (exception) {
                        console.log('No suport copy to clipboard or not file js references');
                    }

                    if ($.isFunction(opc.toolbar.fncopy)) {
                        opc.toolbar.fncopy();
                    }
                    self._hideToolbar();
                    return false;
                });
            }
            if (!((opc.toolbar.fnsearch == null) || (opc.toolbar.fnsearch == undefined))) {
                $('<li class="toolbar-search"/>').appendTo(self.itemsToolbar)
                .click(function () {
                    if ($.isFunction(opc.toolbar.fnsearch)) {
                        opc.toolbar.fnsearch();
                    }
                    self._hideToolbar();
                    return false;
                });
            }
            if (!((opc.toolbar.fnedit == null) || (opc.toolbar.fnedit == undefined))) {
                $('<li class="toolbar-edit"/>').appendTo(self.itemsToolbar)
                .click(function () {
                    if ($.isFunction(opc.toolbar.fnedit)) {
                        opc.toolbar.fnedit();
                    }
                    self._hideToolbar();
                    return false;
                });
            }
            if (!((opc.toolbar.fnadd == null) || (opc.toolbar.fnadd == undefined))) {
                $('<li class="toolbar-add"/>').appendTo(self.itemsToolbar)
                .click(function () {
                    if ($.isFunction(opc.toolbar.fnadd)) {
                        opc.toolbar.fnadd();
                    }
                    self._hideToolbar();
                    return false;
                });
            }
        },
        _render: function () {
            var self = this,
            element = self.element,
            opc = self.options;

            var showToolbar = '';
            if (opc.showToolbar == 'bottom') {
                showToolbar = 'toolbar-bottom';
            } else if (opc.showToolbar == 'right') {
                showToolbar = 'toolbar-right';
            }
            self.dirToolbar = showToolbar;

            self.iconToolbar = $('<a class="jquick-toolbar"></a>')
                                .appendTo(self.container);

            self.boxToolbar = $('<div class="' + showToolbar + ' jquick-box-toolbar" style="display:none;"></div>')
                                .appendTo(self.container);

            self.containerDropDowAndTitle = $('<div class="container-drop-down-and-tittle"></div>').appendTo(self.container);

            self.containerTitle = $('<div class="jquick-single jquick-default"></div>')
                                .appendTo(self.containerDropDowAndTitle)
                                .append('<div><b class="jquick-info"></b></div>');

            self.selectedText = $('<span/>').appendTo(self.containerTitle);

            self.itemsToolbar = $('<ul class="toolbar-items"></ul>').appendTo(self.boxToolbar);
            self._renderItemsToolbar();


            self.boxContainer = $('<div class="jquick-drop"></div>').appendTo(self.containerDropDowAndTitle);
            self._renderSearch();
            self._renderItems();
            if (opc.simple_list) {
                self._loadOptions(self._getOptionsElement());
            }
        },
        _loadOptions: function (items) {
            var self = this,
            element = self.element,
            opc = self.options;
            if (opc.simple_list) {
                $.each(items, function (index, value) {
                    self._addItem(value._Id_, value._Label_, value);
                });
            }
        },
        _getOptionsElement: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            var items = [];
            if (opc.simple_list) {
                $.each($('option', element), function (index, value) {
                    var _id = $(value).prop('value');
                    var _value = $(value).text();
                    items.push({ _Id_: _id, _Value_: _value, _Label_: _value });
                });
            }
            return items;
        },
        _setEvents: function () {
            var self = this,
            element = self.element,
            opc = self.options;

            $(document).on('click', function (event) {
                var count = $(event.target).parents('div.jquick-container').length;
                if (count == 0) {
                    self._openOrClose(true);
                    self._hideToolbar();
                }
            });

            self.containerTitle.click(function (event) {
                if ($(event.target).is('input')) {
                    return false;
                }
                self._openOrClose();//sin argumentos efecto toggleClass
            });
            self.iconToolbar.click(function () {
                if (self.disabled) {
                    return false;
                }
                if (self.boxToolbar.hasClass('toolbar-right')) {
                    var widthBox = self.boxToolbar.find('li').length * 20;
                    self.boxToolbar.css({ width: widthBox, right: -(widthBox) });
                }
                self.boxToolbar.effect('fade', {}, 250, function () {
                    if (!$(this).is(':visible')) {
                        self._hideToolbar();
                    } else {
                        if (!self.iconToolbar.hasClass(self.dirToolbar)) {
                            self.iconToolbar.addClass(self.dirToolbar);
                        }
                    }
                });
                return false;
            });
            self.itemsToolbar.click(function (event) {
                return false;
            });
        },
        _hideToolbar: function () {
            var self = this,
            element = self.element,
            opc = self.options;

            self.boxToolbar.hide();
            self.iconToolbar.removeClass(self.dirToolbar);
        },
        _renderSearch: function () {
            var self = this,
            element = self.element,
            opc = self.options;

            self.boxSearch = $('<div class="jquick-search"></div>').appendTo(self.boxContainer);
            self.inputSearch = $('<input type="text" />').appendTo(self.boxSearch);
            self.inputSearch.donetyping(function () {
                var value = $(this).val();
                if (value.length >= opc.minLength) {
                    self.resetCurretPage();
                    self.serverData(value);
                } else if (value.length > 0 && opc.minLength) {
                    self.resetCurretPage();
                    self._clear();
                } else if (value.length == 0) {
                    self.resetCurretPage();
                    self.serverData('');
                }
            }, opc.delay);
        },
        _disable: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            if (!self.boxSearch.hasClass('jquick-loading')) {
                self.boxSearch.addClass('jquick-loading');
                $('input', self.boxSearch).prop('disabled', true);
            }
        },
        _enable: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            if (self.boxSearch.hasClass('jquick-loading')) {
                self.boxSearch.removeClass('jquick-loading');
            }
            $('input', self.boxSearch).prop('disabled', false);
        },
        _renderItems: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            self.itemsContainer = $('<ul class="jquick-results"></ul>').appendTo(self.boxContainer);
            self.itemsContainer.scroll(function (e) {
                var scrollTop = e.currentTarget.scrollTop + $(e.currentTarget).height();
                if (!opc.positionScrollMax || opc.positionScrollMax == 0) {
                    opc.positionScrollMax = e.currentTarget.scrollHeight - 2;
                }

                if (!opc.positionScrollMax ||
                (e.currentTarget.scrollHeight > 0 && e.currentTarget.scrollHeight <= scrollTop
                && scrollTop > opc.positionScrollMax)) {
                    self.getNextPage();
                    self.serverData(self.inputSearch.val(), true);
                    opc.positionScrollMax = scrollTop;
                }
            });
        },
        _contains: function contains(text, search) {
            var accent = function (str) {
                var rExps = [
                { re: /[\xC0-\xC6]/g, ch: 'A' },
                { re: /[\xE0-\xE6]/g, ch: 'a' },
                { re: /[\xC8-\xCB]/g, ch: 'E' },
                { re: /[\xE8-\xEB]/g, ch: 'e' },
                { re: /[\xCC-\xCF]/g, ch: 'I' },
                { re: /[\xEC-\xEF]/g, ch: 'i' },
                { re: /[\xD2-\xD6]/g, ch: 'O' },
                { re: /[\xF2-\xF6]/g, ch: 'o' },
                { re: /[\xD9-\xDC]/g, ch: 'U' },
                { re: /[\xF9-\xFC]/g, ch: 'u' },
                { re: /[\xD1]/g, ch: 'N' },
                { re: /[\xF1]/g, ch: 'n' }];
                for (var i = 0, len = rExps.length; i < len; i++)
                    str = str.replace(rExps[i].re, rExps[i].ch);

                return str;
            };
            var index = accent(text).toLowerCase().indexOf(accent(search.toLowerCase()));
            return index >= 0;
        },
        serverData: function (text, append) {
            var self = this,
            element = self.element,
            opc = self.options;
            if (opc.simple_list) {
                self._clear();
                var items = self._getOptionsElement();
                var finds = [];
                $.each(items, function (index, elem) {
                    if (self._contains(elem._Value_, text)) {
                        var _label = self._highlightText(elem._Value_, text);
                        elem._Label_ = _label;
                        finds.push(elem);
                    }
                });
                self._loadOptions(finds);
                return true;
            }
            if (($.isFunction(opc.fnPrecondition) && !opc.fnPrecondition())) {
                return true;
            }

            var _isNull = function (elem) {
                return ((elem == null) || (elem == undefined));
            }
            var fnvalue = function (obj, propertyes, index) {
                if (index > propertyes.length) return '';
                if (!_isNull(obj[propertyes[index]]) && obj[propertyes[index]].toString() != '') return obj[propertyes[index]];
                return fnvalue(obj, propertyes, index + 1);
            };
            var hasGet = opc.type.toLowerCase() == 'get';
            var urlComplete = self._paramQueryString(text);

            var config = {
                url: ($.isFunction(opc.url) ? opc.url() : opc.url) + (hasGet ? urlComplete : ''),
                type: opc.type,
                contentType: 'application/json',
                success: function (data, status, xhr) {
                    self.loadPagination(opc.fnloadPagination(data, status, xhr));
                    var texts = opc.successData.Description.split(',');
                    var ids = opc.successData.Id.split(',');
                    var responseData = opc.response(data);
                    $.each(responseData, function (index, value) {
                        var _value = fnvalue(value, texts, 0);
                        var _id = fnvalue(value, ids, 0);
                        var _label = self._highlightText(_value, text);

                        if ($.isFunction(opc.fnPreloadLabel)) {
                            _label = opc.fnPreloadLabel(value);
                        }

                        if ($.isFunction(opc.fnPreloadValue)) {
                            _value = opc.fnPreloadValue(value);
                        }

                        value['_Id_'] = _id;
                        value['_Value_'] = fnvalue(value, texts, 0);
                        self._addItem(_id, _label, value);
                    });
                },
                error: function (xhr) {
                    if (xhr.status !== 500) {
                        console.log(xhr.responseText);
                    } else {
                        showApplicationFatalErrorMessage();
                    }
                },
                complete: function (xhr, status) {
                    self._enable();
                    self.inputSearch.focus();
                }
            };
            var swAdd = !((append == null) || (append == undefined)) && append == true;
            if (!swAdd) {
                self._clear();
            }
            self._disable();
            config = $.extend({}, config, (!hasGet ? { data: $.toJSON(self._paramBody(text)) } : {}));
            $.ajax(config);
        },
        loadPagination: function (totalPages) {
            var self = this,
            element = self.element,
            opc = self.options;

            if (!((totalPages == null) || (totalPages == undefined))) {
                opc.pagination.totalPages = totalPages;
            }
        },
        getCurrentPage: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            return opc.pagination.currentPage;
        },
        getNextPage: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            opc.pagination.currentPage += 1;
            return opc.pagination.currentPage;
        },
        resetCurretPage: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            opc.pagination.currentPage = 1;
            opc.positionScrollMax = null;
        },
        _addItem: function (id, label, data) {
            var self = this,
            element = self.element,
            opc = self.options;
            var item = $('<li class="active-result" data-option-id="' + id + '">' + label + '</li>')
                        .appendTo(self.itemsContainer)
                        .data('data', data);
            item.click(function () {
                var oldData = self.selectedText.data('data');
                self.selectedText.text(item.text())
                    .prop('title', item.text())
                    .data('data', data);
                self._appendClose();
                self._openOrClose(true);
                self._fnSelect();
            });
        },
        _fnSelect: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            if ($.isFunction(opc.fnSelect)) {
                opc.fnSelect();
            }
        },
        _clear: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            self.itemsContainer.empty();
        },
        _paramQueryString: function (text) {
            var self = this,
            element = self.element,
            opc = self.options;

            if (!$.isFunction(opc.fnParams)) {
                opc.fnParams = function () {
                    return {};
                };
            } else {
                if (!$.isPlainObject(opc.fnParams())) {
                    opc.fnParams = function () {
                        return {};
                    };
                }
            }

            if (!$.isFunction(opc.fnPrecondition)) {
                opc.fnPrecondition = function () {
                    return true;
                };
            }

            var dataParams = opc.fnParams();
            var tempParams = [];
            $.each(dataParams, function (i, v) {
                tempParams.push(i + '=' + v);
            });
            var urlExtraParams = tempParams.join('&');
            var urlSearch = opc.changeParams.SearchText.split(',').map(function (value) {
                return value + '=' + text;
            }).join('&');

            var urlPageIndex = opc.changeParams.PageIndex.split(',').map(function (value) {
                return value + '=' + self.getCurrentPage();
            }).join('&');

            var urlPageCount = opc.changeParams.PageCount.split(',').map(function (value) {
                return value + '=' + opc.pageCount;
            }).join('&');

            var urlComplete = '';
            if (urlSearch != '') urlComplete = urlSearch;
            if (urlPageIndex != '') urlComplete += (urlComplete != '' ? '&' : '') + urlPageIndex;
            if (urlPageCount != '') urlComplete += (urlComplete != '' ? '&' : '') + urlPageCount;
            if (urlExtraParams != '') urlComplete += (urlComplete != '' ? '&' : '') + urlExtraParams;

            return (urlComplete != '' ? '?' + urlComplete : '');
        },
        _paramBody: function (text) {
            var self = this,
            element = self.element,
            opc = self.options;

            if (!$.isFunction(opc.fnParams)) {
                opc.fnParams = function () {
                    return {};
                };
            } else {
                if (!$.isPlainObject(opc.fnParams())) {
                    opc.fnParams = function () {
                        return {};
                    };
                }
            }

            if (!$.isFunction(opc.fnPrecondition)) {
                opc.fnPrecondition = function () {
                    return true;
                };
            }

            var dataParams = opc.fnParams();
            $.each(opc.changeParams.SearchText.split(','), function (i, v) {
                dataParams[v] = text;
            });
            $.each(opc.changeParams.PageIndex.split(','), function (i, v) {
                dataParams[v] = 1;
            });
            $.each(opc.changeParams.PageCount.split(','), function (i, v) {
                dataParams[v] = opc.pageCount;
            });
            return dataParams;
        },
        _escapeRegex: function (e) {
            return e.replace(/[\-\[\]{}()*+?.,\\\^$|#\s]/g, "\\$&")
        },
        _highlightText: function (text, searchText) {
            var self = this,
            element = self.element,
            opc = self.options;

            return text.toString().replace(
                    new RegExp(
                            "(?![^&;]+;)(?!<[^<>]*)(" +
                            self._escapeRegex(searchText) +
                            ")(?![^<>]*>)(?![^&;]+;)", "gi"
                            ), "<strong>$1</strong>");
        },
        _openOrClose: function (closeOrOpen) {
            var self = this,
            element = self.element,
            opc = self.options;

            if (self.disabled || ($.isFunction(opc.fnPrecondition) && !opc.fnPrecondition())) {
                return true;
            }

            if ((closeOrOpen == null) || (closeOrOpen == undefined)) {
                self.container.toggleClass('jquick-container-active jquick-with-drop');
                if (self.container.hasClass('jquick-container-active jquick-with-drop')) {
                    self.inputSearch.focus();
                    self.serverData(self.inputSearch.val());
                }
            }
            else {
                if (closeOrOpen == true) {
                    self.container.removeClass('jquick-container-active jquick-with-drop');
                }
                else {
                    self.container.addClass('jquick-container-active jquick-with-drop');
                    self.inputSearch.focus();
                    self.serverData('');
                }
            }
            self.resetCurretPage();
        },
        _appendClose: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            $('abbr', self.containerTitle).remove();
            self.containerTitle.append(self.btnClose);
            $('abbr', self.containerTitle).click(function (event) {
                self.reset();
                return false;
            });
        },
        reset: function () {
            var self = this,
            element = self.element,
            opc = self.options;

            self._removeClose();
            self.selectedText.text('')
                    .prop('title', '')
                    .data('data', null);
            self.inputSearch.val('');
            if ($.isFunction(opc.fnReset)) {
                opc.fnReset();
            }
        },
        _removeClose: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            $('abbr', self.containerTitle).remove();
        },
        setId: function (id) {
            var self = this,
            element = self.element,
            opc = self.options;
            var data = self.selectedText.data('data');
            if (!((data == null) || (data == undefined))) {
                data._Id_ = id;
            }
            else {
                self.selectedText.data('data', { _Id_: id });
            }
        },
        setValue: function (value) {
            var self = this,
            element = self.element,
            opc = self.options;
            var data = self.selectedText.data('data');
            if (!((data == null) || (data == undefined))) {
                data._Value_ = value;
            }
            else {
                self.selectedText.data('data', { _Value_: value });
            }
            self._appendClose();
            self.selectedText.text(value);
            self.selectedText.attr('title', value);
        },
        getId: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            var data = self.selectedText.data('data');
            if (!((data == null) || (data == undefined))) {
                return data._Id_;
            }
            return null;
        },
        getValue: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            var data = self.selectedText.data('data');
            if (data != null && (typeof data != 'undefined')) {
                return data._Value_;
            }
            return null;
        },
        getData: function () {
            var self = this,
            element = self.element,
            opc = self.options;
            var data = self.selectedText.data('data');
            if (!((data == null) || (data == undefined))) {
                return data;
            }
            return null;
        },
        disable: function () {
            var self = this;
            self.disabled = true;
        },
        enable: function () {
            var self = this;
            self.disabled = false;
        }
    });
})(jQuery);
