/* global App */
/* global jQuery */
/*!
 * jQuery fixed header and column plugin.
 * Copyright(c) 2013 Archway Inc. All rights reserved.
 */

/// <reference path="../../ts/core/base.ts" />
/// <reference path="../../ts/core/num.ts" />

(function (global, $) {
    "use strict";

    /*
     * Function.prototype.bind not support IE8.
     */
    if (!Function.prototype.bind) {
        Function.prototype.bind = function (oThis) { // eslint-disable-line no-extend-native
            if (typeof this !== "function") {
                // closest thing possible to the ECMAScript 5
                // internal IsCallable function
                throw new TypeError("Function.prototype.bind - what is trying to be bound is not callable");
            }

            var aArgs = Array.prototype.slice.call(arguments, 1),
                fToBind = this,
                fNOP = function () { },
                fBound = function () {
                    return fToBind.apply((this instanceof fNOP && oThis) ? this : oThis,
                        aArgs.concat(Array.prototype.slice.call(arguments)));
                };

            fNOP.prototype = this.prototype;
            fBound.prototype = new fNOP(); // eslint-disable-line new-cap

            return fBound;
        };
    }

    /*
     * Utility functions for DOM API.
     */
    var DOMUtil = {
        domElementFromjQuery: function (element) {
            if (element instanceof jQuery) {
                if (element.length === 0) {
                    return null;
                }
                element = element[0];
            }

            return element;
        },

        query: function (element, selector) {
            element = DOMUtil.domElementFromjQuery(element);

            return element.querySelectorAll(selector);
        },

        query$: function (element, selector) {
            element = DOMUtil.domElementFromjQuery(element);

            if (!element) {
                return $();
            }

            return $(element.querySelectorAll(selector));
        },

        single: function (element, selector) {
            element = DOMUtil.domElementFromjQuery(element);

            return element.querySelector(selector);
        },

        single$: function (element, selector) {
            element = DOMUtil.domElementFromjQuery(element);

            if (!element) {
                return $();
            }

            return $(element.querySelector(selector));
        },

        cloneNode: function ($elem) {
            if ($elem.length === 0) {
                return $();
            }
            var nodes = [], i, l;
            for (i = 0, l = $elem.length; i < l; i++) {
                nodes.push($elem[i].cloneNode(true));
            }
            return $(nodes);
        },

        getHeight: function ($elem, fixHeight) {
            var targetHeight = arguments.length < 2 ? 0 :
                               !App.isNum(fixHeight) ? 0 :
                               fixHeight < 0 ? 0 :
                               fixHeight;
            if ($elem.length === 0) {
                return 0;
            }
            if (targetHeight) {
                return targetHeight;
            }

            var result = $elem[0].clientHeight;
            if (result) {
                return result;
            }
            return $elem[0].offsetHeight;
        },

        adjustHeight: function ($flow, $fix, margin, fixHeight) {

            var flowRowHeight = DOMUtil.getHeight($flow, fixHeight),
                fixRowHeight = DOMUtil.getHeight($fix, fixHeight),
                height, fixRows, flowRows, i, l;

            if (typeof margin === "undefined" || margin === null) {
                margin = 1;
            }

            height = (flowRowHeight >= fixRowHeight) ? flowRowHeight : fixRowHeight;

            //             fixRows = DOMUtil.query$($fix, "tr");
            //             fixRows.height((height / fixRows.length) + margin);
            //
            //             flowRows = DOMUtil.query$($flow, "tr");
            //             flowRows.height((height / flowRows.length) + margin);


            fixRows = DOMUtil.query$($fix, "tr");
            for (i = 0, l = fixRows.length; i < l; i++) {
                fixRows[i].style.height = ((height / fixRows.length) + margin) + "px";
            }
            flowRows = DOMUtil.query$($flow, "tr");
            for (i = 0, l = fixRows.length; i < l; i++) {
                flowRows[i].style.height = ((height / flowRows.length) + margin) + "px";
            }
            return height;
        },

        focusable: function focusable(element) {
            var map, mapName, img,
                nodeName = element.nodeName.toLowerCase();
            if (nodeName === "area") {
                map = element.parentNode;
                mapName = map.name;
                if (!element.href || !mapName || map.nodeName.toLowerCase() !== "map") {
                    return false;
                }
                img = $("img[usemap=#" + mapName + "]")[0];
                return !!img && DOMUtil.visible(img);
            }

            if (nodeName === "span" || nodeName === "label") {
                return false;
            }

            return (/input|select|textarea|button|object|a/.test(nodeName) ? !element.disabled : false) && DOMUtil.visible(element);
        },

        visible: function visible(element) {
            return $.expr.filters.visible(element) &&
                !$(element).parents().addBack().filter(function () {
                    return $.css(this, "visibility") === "hidden";
                }).length;
        }
    };

    if (!$.expr[":"].focusable) {
        $.extend($.expr[":"], {
            focusable: function (element) {
                return DOMUtil.focusable(element);
            }
        });
    }

    var BrowserSupport = {
        isLteIE8: function isLteIE8() {
            var ua = window.navigator.userAgent.toLowerCase();
            var ver = window.navigator.appVersion.toLowerCase();

            if (ua.indexOf("msie") !== -1) {
                if (ver.indexOf("msie 6.") !== -1) {
                    return true;
                } else if (ver.indexOf("msie 7.") !== -1) {
                    return true;
                } else if (ver.indexOf("msie 8.") !== -1) {
                    return true;
                }
            }
            return false;
        },
        scrollBar: { width: 18, height: 18 }
    };

    /*
     * Data row and cache definitions.
     */
    function DataRow(element) {
        this.element = element;
        this.rowid = App.uuid();
        this.height = 0; //cache actual height of row element
        this.visible = false;
        this.temporaryHeight = 0; //height when before render
    }

    function DataRowCache() {
        this.rows = {};
        this.rowids = [];
    }

    DataRowCache.prototype.id = function (id) {
        return this.rows[id];
    };

    DataRowCache.prototype.index = function (index) {
        var id = this.rowids[index];

        return this.rows[id];
    };

    DataRowCache.prototype.add = function (row) {
        this.rows[row.rowid] = row;
        this.rowids.push(row.rowid);

        return row;
    };

    DataRowCache.prototype.remove = function (id) {

        var i, len;
        for (i = 0, len = this.rowids.length; i < len; i++) {
            if (this.rowids[i] === id) {
                this.rowids.splice(i, 1);
            }
        }
        delete this.rows[id];
    };



    var SortIconClasses = {

        Acend: "glyphicon-sort-by-attributes",
        Descend: "glyphicon-sort-by-attributes-alt"
    };

    var onTheadClick = function (e, self) {
        var $target = e.target.tagName === "I" ? $(e.target).closest("th") : $(e.target),
            prop = $target.attr("data-prop"),
            modes = { none: 0, ascend: 1, descend: 2 },
            $container = $target.closest(".dt-container"),
            icon = DOMUtil.query$($target, ".glyphicon"),
            mode;

        if (!prop || prop === "") {
            return;
        }

        var otherIcons = DOMUtil.query$($container, "i.glyphicon").not(icon);
        otherIcons.removeClass(SortIconClasses.Acend);
        otherIcons.removeClass(SortIconClasses.Descend);

        if (DOMUtil.query$($target, "i." + SortIconClasses.Acend).length > 0) {
            mode = modes.descend;
            icon.removeClass(SortIconClasses.Acend);
            icon.addClass(SortIconClasses.Descend);
        }
        else if (DOMUtil.query$($target, "i." + SortIconClasses.Descend).length > 0) {

            if (App.isFunc(self.paramContructors.options.onreset)) {
                self.paramContructors.options.onreset();
                mode = modes.none;
            } else {
                mode = modes.ascend;
                icon.removeClass(SortIconClasses.Descend);
                icon.addClass(SortIconClasses.Acend);
            }
        }
        else {
            mode = modes.ascend;
            icon.removeClass(SortIconClasses.Descend);
            $(icon[0]).addClass(SortIconClasses.Acend);
        }

        self.sort(prop, mode);
        self.render({
            scrollTop: self.BodyContainer.getScrollTop(),
            scrollOffset: 0
        }, "rewrite");
    };

    var createFixedColumns = function ($target, cellSelector, containerClass, fixeColumnNumbers) {

        var $fixed = $target.clone(),
            $tr, $cell, tr, cell,
            $fixtr, $fixcell, fixtr, fixcell,
            i = 0, ilen = 0, j = 0, jlen = 0, logicalCellCounts;

        fixeColumnNumbers = fixeColumnNumbers + 1;

        var $fixedTableChildren = DOMUtil.single$($fixed, "table").children();
        var $targetTableChildren = DOMUtil.single$($target, "table").children();

        $fixedTableChildren.each(function (index, tchild) {

            // if (tchild.tagName.toLowerCase() === "tfoot") {
            //     return;
            // }

            $fixtr = DOMUtil.query$($(tchild), "tr");
            $tr = DOMUtil.query$($($targetTableChildren[index]), "tr");

            logicalCellCounts = new Array($fixtr.length);

            for (i = 0, ilen = $fixtr.length; i < ilen; i++) {
                logicalCellCounts[i] = 0;
            }

            var k = 0, klen = 0;

            for (i = 0, ilen = $fixtr.length; i < ilen; i++) {
                fixtr = $fixtr[i];
                tr = $tr[i];
                $fixcell = DOMUtil.query$($(fixtr), cellSelector);
                $cell = DOMUtil.query$($(tr), cellSelector);

                for (j = 0, jlen = $fixcell.length; j < jlen; j++) {
                    fixcell = $fixcell[j];
                    cell = $cell[j];

                    logicalCellCounts[i]++;

                    if ((fixcell.colSpan > 1 && logicalCellCounts[i] <= fixeColumnNumbers) &&
                        (logicalCellCounts[i] + (fixcell.colSpan - 1) > fixeColumnNumbers)) {

                        throw new Error("列の固定で指定されたインデックスでは固定位置を揃えることができません。");
                    }

                    logicalCellCounts[i] += (fixcell.colSpan > 1) ? (fixcell.colSpan - 1) : 0;

                    if (logicalCellCounts[i] <= fixeColumnNumbers) {
                        if (fixcell.rowSpan > 1) {
                            for (k = (i + 1), klen = (fixcell.rowSpan + i); k < klen; k++) {
                                logicalCellCounts[k]++;
                                logicalCellCounts[k] += (fixcell.colSpan > 1) ? (fixcell.colSpan - 1) : 0;
                            }
                        }
                    }

                    if (logicalCellCounts[i] > fixeColumnNumbers) {
                        $(fixcell).remove();
                    }
                    else {
                        $(cell).remove();
                    }
                }
            }
        });

        $fixed.css({
            overflow: "",
            overflowX: "",
            overflowY: "",
            width: ""
        });

        DOMUtil.single$($fixed, "table").css("width", "auto");
        DOMUtil.single$($fixed, "table").css("max-width", "");


        return $fixed.addClass(containerClass);
    };

    var hideSortIcons = function ($container) {
        var $divHead = DOMUtil.single$($container, ".flow-container .dt-head"),
            $fixHead = DOMUtil.single$($container, ".fix-columns .dt-head");
        DOMUtil.query$($divHead, ".glyphicon").replaceWith($("<i class='glyphicon'></i>"));
        if ($fixHead) {
            DOMUtil.query$($fixHead, ".glyphicon").replaceWith($("<i class='glyphicon'></i>"));
        }
    };

    function HeaderRow(containerElements) {
        this.containerElements = containerElements;
        this.initialize.call(this);
    }

    HeaderRow.prototype.initialize = function () {
        var self = this;
        if (self.containerElements.length > 1) {
            this.fixElement = self.containerElements[0];
            this.flowElement = self.containerElements[1];
        }
        else {
            this.flowElement = self.containerElements[0];
        }
    };

    HeaderRow.prototype.addCellClickEvent = function (handler) {
        DOMUtil.query$(this.flowElement, "th").on("click", handler);
        if (!App.isUndef(this.fixElement)) {
            DOMUtil.query$(this.fixElement, "th").on("click", handler);
        }
    };

    function validateHeaderRowLength($head) {
        if (DOMUtil.query$($head, "tr").length > 1) {
            throw new Error("多段行での列の表示・非表示はサポートされません。");
        }
    }

    HeaderRow.prototype.showColumn = function (index) {
        var colid;
        if (typeof index === "string") {
            colid = index;
        }
        var self = this,
            $container = self.flowElement.closest(".dt-container"),
            $head = DOMUtil.query$($container, ".flow-container .dt-head thead"),
            $fixcols = $(self.fixElement).find(".dt-head th").length,
            $flowcols = $(self.flowElement).find(".dt-head th").length,
            showThCol = function ($targetHead) {
                if (colid) {
                    return DOMUtil.query$($targetHead, "th[data-col='" + colid + "'])");
                } else {
                    var thead, th, i, ilen;
                    for (i = 0, ilen = $targetHead.length; i < ilen; i++) {
                        thead = $targetHead[i];
                        th = DOMUtil.query(thead, "th");
                        if (th[index]) {
                            th[index].style.display = "";
                        }
                    }
                }
            };
        validateHeaderRowLength($head);
        if (index <= ($fixcols - 1) && $flowcols) {
            var $fixHead = DOMUtil.query$($container, ".fix-columns .dt-head thead");
            showThCol($fixHead);
            var outerWidth = $fixHead.outerWidth();
            $head.closest("table").css("margin-left", outerWidth);
        }
        else {
            //index -= $fixcols;
            showThCol($head);
        }
    };

    HeaderRow.prototype.hideColumn = function (index) {
        var colid;
        if (typeof index === "string") {
            colid = index;
        }
        var self = this,
            $container = self.flowElement.closest(".dt-container"),
            $head = DOMUtil.query$($container, ".flow-container .dt-head thead"),
            $fixcols = $(self.fixElement).find(".dt-head th").length,
            $flowcols = $(self.flowElement).find(".dt-head th").length,
            hideThCol = function ($targetHead) {

                if (colid) {
                    return DOMUtil.query$($targetHead, "th[data-col='" + colid + "'])");
                } else {
                    var thead, th, i, ilen;
                    for (i = 0, ilen = $targetHead.length; i < ilen; i++) {
                        thead = $targetHead[i];
                        th = DOMUtil.query(thead, "th");
                        if (th[index]) {
                            th[index].style.display = "none";
                        }
                    }
                }
            };
        validateHeaderRowLength($head);
        if (index <= ($fixcols - 1) && $flowcols) {

            var $fixHead = DOMUtil.query$($container, ".fix-columns .dt-head thead");
            hideThCol($fixHead);
            var outerWidth = $fixHead.outerWidth();
            $head.closest("table").css("margin-left", outerWidth);
        }
        else {
            //index -= $fixcols;
            hideThCol($head);
        }
    };

    function HeaderContainer(paramContructors) {
        this.paramContructors = paramContructors;
        this.row = {};
        this.initialize(this);
    }

    HeaderContainer.prototype.initialize = function () {
        var self = this,
            $divHead, $fixHead,
            $head = DOMUtil.single$(self.paramContructors.srcElement, "thead").clone(),
            $flowTable = self.paramContructors.srcElement, $flowTheadCells;

        if ($head.length <= 0) {
            return;
        }

        if (self.paramContructors.containerElements.length > 1) {
            this.fixElement = self.paramContructors.containerElements[0];
            this.flowElement = self.paramContructors.containerElements[1];
        }
        else {
            this.flowElement = self.paramContructors.containerElements[0];
        }

        $flowTheadCells = DOMUtil.query$(this.flowElement, "thead th,thead td");
        $flowTheadCells.text("");
        $flowTable.css("margin-top", "-2px");

        $divHead = $("<div class='dt-head'><table></table></div>").prependTo(self.flowElement);

        this.row = new HeaderRow(self.paramContructors.containerElements);
        DOMUtil.single$($divHead, "table").prop("class", $flowTable.prop("class")).append($head);
        $divHead.css("margin-right", BrowserSupport.scrollBar.width);

        if (self.paramContructors.options.fixedColumn === true) {

            DOMUtil.single$($divHead, "table").css("width", self.paramContructors.options.innerWidth);
            DOMUtil.single$($divHead, "table").css("max-width", self.paramContructors.options.innerWidth);

            $fixHead = createFixedColumns($divHead, "th", "dt-fix-head", self.paramContructors.options.fixedColumns);
            $fixHead.css("margin-right", "");
            this.fixElement.append($fixHead);

            var w = DOMUtil.single$($fixHead, "table").width();
            DOMUtil.single$($fixHead, "table").css("width", w);
            DOMUtil.single$($divHead, "table").css("margin-left", w - 1);
            $divHead.css("min-width", w + 200 - BrowserSupport.scrollBar.width);

            DOMUtil.adjustHeight($divHead, $fixHead, 1);

            if (self.paramContructors.options.responsive) {
                if (!BrowserSupport.isLteIE8()) {
                    DOMUtil.query$($divHead, "table").css("table-layout", "fixed");
                    DOMUtil.query$($fixHead, "table").css("table-layout", "fixed");
                }
            }

            if (self.paramContructors.options.sortable === true) {
                $.each($divHead, function (index) {
                    DOMUtil.query$($divHead[index], "[data-prop]").css("cursor", "pointer").append($("<i class='glyphicon'></i>"));
                });

                $.each($fixHead, function (index) {
                    DOMUtil.query$($fixHead[index], "[data-prop]").css("cursor", "pointer").append($("<i class='glyphicon'></i>"));
                });

                self.row.addCellClickEvent(function (e) {
                    self.showWait({});
                    onTheadClick(e, self.paramContructors.accessor.getBodyView());
                    self.hideWait({});
                });
            }
        }
        else {
            if (self.paramContructors.options.responsive) {
                var setMinWidth = function (cells, width) {
                    var i, ilen, cell;
                    for (i = 0, ilen = cells.length; i < ilen; i++) {
                        cell = cells[i];
                        if (!$(cell).css("width")) {
                            $(cell).css("min-width", width);
                        }
                    }
                };
                setMinWidth(DOMUtil.query$($divHead, "th,td"), 40);
                DOMUtil.single$(this.flowElement, ".dt-head table").css("table-layout", "fixed");
            }

            if (self.paramContructors.options.sortable === true) {
                $.each($divHead, function (index) {
                    DOMUtil.query$($divHead[index], "[data-prop]").css("cursor", "pointer").append($("<i class='glyphicon'></i>"));
                });

                self.row.addCellClickEvent(function (e) {
                    self.showWait({});
                    onTheadClick(e, self.paramContructors.accessor.getBodyView());
                    self.hideWait({});
                });
            }
        }
    };

    HeaderContainer.prototype.addScrollEvent = function (handler) {
        this.flowElement.on("scroll", ".dt-head", handler);
    };

    HeaderContainer.prototype.detachScrollEvent = function () {
        this.flowElement.off("scroll");
    };

    HeaderContainer.prototype.showWait = function (operation) {
        this.flowElement.closest(".dt-container").append("<div class='wait'></div>");
        if (App.isFunc(operation)) {
            operation();
        }
    };

    HeaderContainer.prototype.hideWait = function (operation) {
        DOMUtil.query$(this.flowElement.closest(".dt-container"), ".wait").remove();
        if (App.isFunc(operation)) {
            operation();
        }
    };

    var setNewSelectedRow = function (self, selectedRow, e) {

        if (self.selectedRowElement) {
            self.lastSelectedRowElement = self.selectedRowElement;
        }
        self.selectedRowElement = selectedRow;

        if (self.paramContructors.options.onselect && selectedRow) {
            self.paramContructors.options.onselect(e || {}, selectedRow);
        }
    };

    var setFocus = function (row, self) {
        setTimeout(function () {

            var item = row.element.find(":focusable:first");

            if (item.length > 0) {
                $(item[0]).focus();
            }

            setNewSelectedRow(self, row);

        }, 0);
    };

    var onTbodyFocus = function (e, self) {
        self.paramContructors.options.onselecting();
        var target = $(e.target),
            tbody = target.closest("tbody"),
            rowid = tbody.attr("data-rowid"),
            row = self.paramContructors.accessor.getBodyView().cache.id(rowid);

        //var enableOperation = (self.paramContructors.options.onselect && row);

        setTimeout(function () {

            var $divBody = target.closest(".dt-body"),
                divBody = $divBody[0],
                $container = $divBody.closest(".dt-container"),
                container = $container[0],
                scrollTop = $divBody.scrollTop();

            if (!container) {
                return;
            }

            if ($divBody.hasClass("dt-fix-body")) {
                DOMUtil.single(container, ".flow-container>.dt-body").scrollTop = scrollTop;

            } else {
                DOMUtil.single(container, ".flow-container>.dt-body").scrollTop = scrollTop;
                var left = target.offset().left - DOMUtil.single$($divBody, "table").offset().left;
                if (left < divBody.scrollLeft) {
                    divBody.scrollLeft = left - 10;
                }
            }

            setNewSelectedRow(self, row, e);

        }, 0);
    };

    var onTbodyClick = function (e, self) {
        var target = $(e.target),
            tbody = target.closest("tbody"),
            rowid = tbody.attr("data-rowid"),
            row = self.paramContructors.accessor.getBodyView().cache.id(rowid);
        if (!App.isFunc(self.paramContructors.options.onclick)) {
            return;
        }
        setTimeout(function () {
            self.paramContructors.options.onclick({}, row);
        }, 0);

        e.preventDefault();
        e.stopImmediatePropagation();

    };

    var getForcusable = function (target, self) {
        var focusable = [],
            focusableFix = [];

        focusable = $(target.element[0]).find(":focusable").toArray();
        if (self.paramContructors.options.fixedColumn) {
            focusableFix = $(target.element[1]).find(":focusable").toArray();
            focusable = focusableFix.concat(focusable);
        }
        return focusable;
    };

    var KeyCodes = {
        Tab: 9,
        ArrowDown: 40,
        ArrowUp: 38
    };

    var onKeyDown = function (e, self) {
        var target = $(e.target),
            $tbody = target.closest("tbody"),
            dataRowId = $tbody.attr("data-rowid"),
            targetRow = self.paramContructors.accessor.getBodyView().cache.id(dataRowId),
            focusable = getForcusable(targetRow, self),
            isFirst = function (aTarget, focusableTarget) {
                return $(focusableTarget[0]).is(aTarget);
            },
            isLast = function (aTarget, focusableTarget) {
                return $(focusableTarget[focusableTarget.length - 1]).is(aTarget);
            },
            currentTabIndex,
            currentRowIndex, nextRowIndex, nextRow, prevRowIndex, prevRow;

        if (focusable.length <= 0) {
            return false;
        }

        if (e.keyCode === KeyCodes.Tab && e.shiftKey === false) {
            // On press Tab key

            currentTabIndex = focusable.indexOf(target[0]);
            var nextTabIndex = currentTabIndex + 1,
                nextTarget = focusable[nextTabIndex];

            if (!isLast(target, focusable)) {
                self.paramContructors.options.ontabing();
                nextTarget.focus();
                self.paramContructors.options.ontabed();

            } else {
                currentRowIndex = self.paramContructors.accessor.getBodyView().cache.rowids.indexOf(dataRowId);
                nextRowIndex = currentRowIndex + 1;

                if (nextRowIndex === self.paramContructors.accessor.getBodyView().cache.rowids.length) {
                    return false;
                }
                nextRow = self.paramContructors.accessor.getBodyView().cache.index(nextRowIndex);
                focusable = getForcusable(nextRow, self);
                if (focusable.length > 0) {
                    focusable[0].focus();
                }
            }

        } else if (e.keyCode === KeyCodes.Tab && e.shiftKey === true) {
            // On press Shift + Tab key

            currentTabIndex = focusable.indexOf(target[0]);
            var prevTabIndex = currentTabIndex - 1,
                prevTarget = focusable[prevTabIndex];

            if (!isFirst(target, focusable)) {
                self.paramContructors.options.ontabing();
                prevTarget.focus();
                self.paramContructors.options.ontabed();

            } else {
                currentRowIndex = self.paramContructors.accessor.getBodyView().cache.rowids.indexOf(dataRowId);
                prevRowIndex = currentRowIndex - 1;
                if (prevRowIndex < 0) {
                    return false;
                }

                prevRow = self.paramContructors.accessor.getBodyView().cache.index(prevRowIndex);
                focusable = getForcusable(prevRow, self);
                if (focusable.length > 0) {
                    self.paramContructors.options.ontabing();
                    focusable[focusable.length - 1].focus();
                    self.paramContructors.options.ontabed();
                }
            }
        } else if (e.keyCode === KeyCodes.ArrowDown) {
            // On press Arrow Down Key.
            currentRowIndex = self.paramContructors.accessor.getBodyView().cache.rowids.indexOf(dataRowId);
            currentTabIndex = focusable.indexOf(target[0]);
            nextRowIndex = currentRowIndex + 1;

            if (nextRowIndex === self.paramContructors.accessor.getBodyView().cache.rowids.length) {
                return;
            }
            nextRow = self.paramContructors.accessor.getBodyView().cache.index(nextRowIndex);
            focusable = getForcusable(nextRow, self);
            if (focusable && focusable[currentTabIndex]) {
                focusable[currentTabIndex].focus();
                e.preventDefault();
                e.stopImmediatePropagation();
            }

        } else if (e.keyCode === KeyCodes.ArrowUp) {
            // On press Arrow Up Key.
            currentRowIndex = self.paramContructors.accessor.getBodyView().cache.rowids.indexOf(dataRowId);
            currentTabIndex = focusable.indexOf(target[0]);
            prevRowIndex = currentRowIndex - 1;

            if (prevRowIndex < 0) {
                return;
            }

            prevRow = self.paramContructors.accessor.getBodyView().cache.index(prevRowIndex);
            focusable = getForcusable(prevRow, self);
            if (focusable && focusable[currentTabIndex]) {
                self.paramContructors.options.ontabing();
                focusable[currentTabIndex].focus();
                self.paramContructors.options.ontabed();
                e.preventDefault();
                e.stopImmediatePropagation();
            }
        }

        if (e.keyCode === KeyCodes.Tab) {
            e.preventDefault();
        }
    };

    function BodyRow(containerElement, paramContructors) {
        this.containerElement = containerElement;
        this.paramContructors = paramContructors;
        this.initialize.call(this);
    }

    var initializeBodyRowEvent = function (container, bodyRow) {

        bodyRow.addChangeEvent(function (e) {
            var target = $(e.target),
                tbody = target.closest("tbody"),
                rowid = tbody.attr("data-rowid"),
                row = bodyRow.paramContructors.accessor.getBodyView().cache.id(rowid);

            if (bodyRow.paramContructors.options.onchange && row) {
                bodyRow.paramContructors.options.onchange(e, row);
            }
        });

        bodyRow.addCellBlurEvent(function (e) {
            self.hasFocus = false;

            e.preventDefault();
            e.stopImmediatePropagation();
        });

        bodyRow.addCellFocusEvent(function (e) {
            self.hasFocus = true;
            onTbodyFocus(e, container);

            e.preventDefault();
            e.stopImmediatePropagation();
        });

        bodyRow.addCellClickEvent(function (e) {
            onTbodyFocus(e, container);
            onTbodyClick(e, container);
        });

        bodyRow.addKeyDownEvent(function (e) {
            onKeyDown(e, bodyRow);
        });
    };

    BodyRow.prototype.initialize = function () {
        var self = this;

        if (self.containerElement.length > 1) {
            self.fixElement = self.containerElement[0];
            self.flowElement = self.containerElement[1];
        }
        else {
            self.flowElement = self.containerElement[0];
        }
    };

    BodyRow.prototype.addChangeEvent = function (handler) {
        this.containerElement.on("change", "td", handler);
    };

    BodyRow.prototype.detachChangeEvent = function () {
        this.containerElement.off("change");
    };

    BodyRow.prototype.addKeyDownEvent = function (handler) {
        this.containerElement.on("keydown", "td", handler);
    };

    BodyRow.prototype.addCellClickEvent = function (handler) {
        this.containerElement.on("click", "td", handler);
    };

    BodyRow.prototype.detachCellClickEvent = function () {
        this.containerElement.off("click");
    };

    BodyRow.prototype.addCellFocusEvent = function (handler) {
        this.containerElement.on("focus", ":input", handler);
    };

    BodyRow.prototype.detachCellFocusEvent = function () {
        this.containerElement.off("focus", ":input");
    };

    BodyRow.prototype.addCellBlurEvent = function (handler) {
        this.containerElement.on("blur", ":input", handler);
    };

    BodyRow.prototype.detachCellBlurEvent = function () {
        this.containerElement.off("blur", ":input");
    };

    var showThInBody = function (row, index) {
        var $container = row.paramContructors.srcElement.closest(".dt-container"),
            $targetRow = DOMUtil.single$($container, ".flow-container .dt-body table thead tr"),
            $fixTargetRow = DOMUtil.single$($container, ".fix-columns .dt-fix-body table thead tr");
        $targetRow.find("th:eq(" + index + ")").css("display", "");
        if ($fixTargetRow) {
            $fixTargetRow.find("th:eq(" + index + ")").css("display", "");
        }
    };

    BodyRow.prototype.showColumn = function (index) {
        var self = this,
            $flowcontainer = $(self.flowElement).closest("tbody"),
            $flowrow = DOMUtil.query$($flowcontainer, "tr"),
            $fixcontainer = $(self.fixElement).closest("tbody"),
            $fixrow = DOMUtil.query$($fixcontainer, "tr"),
            $fixcols = $(self.fixElement).find("td").length,
            $flowcols = $(self.flowElement).find("td").length,
            showTdCol = function ($targetRow) {
                if (typeof index === "string") {
                    throw new Error("index must be a number");
                } else {
                    var tbody, td, i, ilen;
                    for (i = 0, ilen = $targetRow.length; i < ilen; i++) {
                        tbody = $targetRow[i];
                        td = DOMUtil.query(tbody, "td");
                        if (td[index]) {
                            td[index].style.display = "";
                        }
                    }
                }
            };
        if (index <= ($fixcols - 1) && $flowcols) {
            //showThInBody(self, index);
            showTdCol($fixrow);
            //showTdCol($flowrow);
        }
        else {
            //index -= $flowcols;
            if (index < 0) {
                index *= -1;
            }
            showThInBody(self, index);
            showTdCol($flowrow);
        }
    };

    var hideThInBody = function (row, index) {
        var $container = row.paramContructors.srcElement.closest(".dt-container"),
            $targetRow = DOMUtil.single$($container, ".flow-container .dt-body table thead tr"),
            $fixTargetRow = DOMUtil.single$($container, ".fix-columns .dt-fix-body table thead tr");
        $targetRow.find("th:eq(" + index + ")").css("display", "none");
        if ($fixTargetRow) {
            $fixTargetRow.find("th:eq(" + index + ")").css("display", "none");
        }
    };

    BodyRow.prototype.hideColumn = function (index) {
        var self = this,
            $flowcontainer = $(self.flowElement).closest("tbody"),
            $flowrow = DOMUtil.query$($flowcontainer, "tr"),
            $fixcontainer = self.fixElement === typeof (undefined) ? self.fixElement.closest("tbody") : {},
            $fixrow = self.fixElement === typeof (undefined) ? DOMUtil.query$($fixcontainer, "tr") : {},
            $fixcols = self.fixElement === typeof (undefined) ? $(self.fixElement).find("td").length : 0,
            $flowcols = $(self.flowElement).find("td").length,
            hideTdCol = function ($targetRow) {
                if (typeof index === "string") {
                    throw new Error("index must be a number");
                } else {
                    var tbody, td, i, ilen;
                    for (i = 0, ilen = $targetRow.length; i < ilen; i++) {
                        tbody = $targetRow[i];
                        td = DOMUtil.query(tbody, "td");
                        if (td[index]) {
                            td[index].style.display = "none";
                        }
                    }
                }
            };
        if (index <= ($fixcols - 1) && $flowcols) {
            //hideThInBody(self, index);
            //hideTdCol($flowrow);
            hideTdCol($fixrow);
        }
        else {
            //index -= $flowcols;
            if (index < 0) {
                index *= -1;
            }
            hideThInBody(self, index);
            hideTdCol($flowrow);
        }
    };

    function BodyContainer(paramContructors) {
        this.scrollTimer = null;
        this.rows = []; //BodyRows
        this.rowHeights = [];
        this.paramContructors = paramContructors;
        this.pageHeight = 0;
        this.renderHeight = 0;
        this.templateHeight = 0;
        this.medianHeight = 0;
        this.scrollTop = 0;
        this.offsetScrollbar = 0;
        this.scrollHeight = 0;
        this.isScrollDown = false;
        this.render = function () { };
        this.selectedRowElement = undefined;
        this.lastSelectedElement = undefined;
        this.selectors = {
            flowElement: undefined,
            $flowTable: paramContructors.srcElement,
            $flowScroll: undefined,
            $divBody: undefined,

            fixElement: undefined,
            $fixTable: undefined,
            $fixScroll: undefined
        };
        this.initialize.call(this);
    }

    BodyContainer.prototype.getSelectedRow = function () {
        return this.selectedRowElement;
    };

    BodyContainer.prototype.getLastSelectedRow = function () {
        return this.lastSelectedRowElement;
    };

    BodyContainer.prototype.recalc = function (rows) {
        var self = this,
            $container = self.selectors.flowElement.closest(".dt-container");
        self.templateHeight = DOMUtil.single$($container, "tbody.item-tmpl").outerHeight();
        self.rowHeights = [];
        self.rowHeights.push(self.templateHeight);
        (rows || []).forEach(function (row) {
            if (row.height) {
                self.rowHeights.push(row.height);
            }
        });
        self.medianHeight = App.num.median(this.rowHeights);
        self.medianHeight = self.paramContructors.options.autoRowHeight && self.fixRowHeight > -1 ? self.fixRowHeight : self.medianHeight;

        (rows || []).forEach(function (row) {
            if (!row.templateHeight) {
                row.temporaryHeight = self.medianHeight;
            }
        });
        self.renderHeight = 0;
        self.pageHeight = 0;
        (rows || []).forEach(function (row) {
            self.addContainerHeight(row.height || row.temporaryHeight);
        });
        self.lastApplyPageHeight = 0;
        self.adjustContainerHeight();
    };

    BodyContainer.prototype.redrawRow = function (row) {
        var self = this,
            currentActualHeight = row.element[0].clientHeight;

        self.addContainerHeight(currentActualHeight, row.height);
        self.adjustContainerHeight();

        if (row.height !== currentActualHeight) {
            row.height = currentActualHeight;
        }
    };

    BodyContainer.prototype.initialize = function () {
        var self = this,
            $divBody, $fixBody,
            $divHead, $divHeadCells, $fixHead,
            $divFoot, $fixFoot,  // eslint-disable-line no-unused-vars
            $container;
        if (self.paramContructors.containerElements.length > 1) {
            self.selectors.fixElement = self.paramContructors.containerElements[0];
            self.selectors.flowElement = self.paramContructors.containerElements[1];
        }
        else {
            self.selectors.flowElement = self.paramContructors.containerElements[0];
        }

        $container = self.selectors.flowElement.closest(".dt-container");
        $divHead = DOMUtil.single$(self.selectors.flowElement, ".dt-head");
        $divBody = self.selectors.$flowTable.wrap("<div class='dt-body'></div>").parent();
        $divFoot = DOMUtil.single$(self.selectors.flowElement, ".dt-foot");

        self.selectors.$flowScroll = self.selectors.$flowTable.wrap("<div class='dt-vscroll' tabindex='-1'></div>").parent();
        self.selectors.$divBody = $divBody;

        var scrollWidth = $divBody[0].offsetWidth - self.selectors.$flowScroll[0].offsetWidth;
        if (scrollWidth > 0) {
            BrowserSupport.scrollBar.width = scrollWidth;
        }

        $divBody.outerHeight(self.paramContructors.options.height);

        self.medianHeight = self.templateHeight = DOMUtil.single$($container, "tbody.item-tmpl").outerHeight();
        DOMUtil.single$($container, "tbody.item-tmpl").hide();
        DOMUtil.single$($divBody, "table tfoot").hide();

        $divHeadCells = DOMUtil.query$($divBody, "thead th,thead td");
        $divHeadCells.text("");

        self.selectors.$flowTable.css({ "table-layout": "fixed", "position": "absolute", "top": 0 });

        if (self.paramContructors.options.fixedColumn === true) {

            DOMUtil.single$($divBody, "table").css("width", self.paramContructors.options.innerWidth);
            DOMUtil.single$($divBody, "table").css("max-width", self.paramContructors.options.innerWidth);
            $divBody.css("overflow-x", "scroll");

            $fixBody = createFixedColumns($divBody, "td", "dt-fix-body", self.paramContructors.options.fixedColumns);
            createFixedColumns($divBody, "th", "dt-fix-body", self.paramContructors.options.fixedColumns);

            $fixHead = DOMUtil.single$(self.selectors.fixElement, ".dt-head.dt-fix-head");
            $fixFoot = DOMUtil.single$(self.selectors.fixElement, ".dt-foot.dt-fix-foot");

            DOMUtil.query$($fixBody, "table thead").remove();
            DOMUtil.query$($fixBody, "table tfoot").remove();

            var $cpHead = DOMUtil.query$($fixHead, "thead").clone();
            DOMUtil.query$($fixBody, "table tbody.item-tmpl").before($cpHead);
            $fixBody.height($divBody.height() + 1 - BrowserSupport.scrollBar.height);
            var fixBodyTheadCells = DOMUtil.query$($fixBody, "thead th, thead td");
            fixBodyTheadCells.text("");
            DOMUtil.query$($fixBody, "table tr").css("height", "0px");

            $fixHead.after($fixBody);

            // TODO: body offset (configure)
            var bodyOffset = 0;
            var w = DOMUtil.single$($fixHead, "table").width();
            DOMUtil.single$($fixBody, "table").width(w - bodyOffset);
            DOMUtil.single$($divBody, "table").css("margin-left", w - bodyOffset);
            $divBody.css("min-width", w + 200);

            self.selectors.$flowScroll.css("min-height", "3px").css("width", self.paramContructors.options.innerWidth + w - 1);

            self.selectors.$fixTable = DOMUtil.single$(self.selectors.fixElement, ".dt-fix-body table");
            self.selectors.$fixScroll = DOMUtil.single$(self.selectors.fixElement, ".dt-vscroll");


            self.addScrollEvent(function (e) {
                if (self.scrollTimer) {
                    clearTimeout(self.scrollTimer);
                }

                self.srollTimer = setTimeout(function () {
                    var $target = $(e.target),
                        top = $divBody.scrollTop();

                    //If scroll is happing but scrollTop has not change
                    //Set scrollLeft for $divHead
                    if (top !== self.scrollTop) {


                        self.render({
                            scrollTop: top,
                            scrollOffset: (top - self.scrollTop)
                        });
                        self.scrollTop = top;

                    } else {
                        $divHead.scrollLeft($target.scrollLeft());
                        $divFoot.scrollLeft($target.scrollLeft());
                    }
                }, 5);

                e.preventDefault();
                e.stopImmediatePropagation();
            });

            $fixBody.on("mousewheel", function (e) {
                var $target = $(this),
                    delta = e.originalEvent.wheelDelta,
                    top = $target.scrollTop() - (delta);
                $divBody.scrollTop(top);

                if (top > 0) {
                    return false;
                }
            });

            if (self.paramContructors.options.responsive) {
                if (!BrowserSupport.isLteIE8()) {
                    DOMUtil.query$($fixBody, "table").css("table-layout", "fixed");
                    DOMUtil.query$($divBody, "table").css("table-layout", "fixed");
                }
            }
        }
        else {
            if (self.paramContructors.options.responsive) {
                var setMinWidth = function (cells, width) {
                    var i, ilen, cell;
                    for (i = 0, ilen = cells.length; i < ilen; i++) {
                        cell = cells[i];
                        if (!cell.style.width) {
                            cell.style.minWidth = width;
                        }
                    }
                };

                setMinWidth(DOMUtil.query$($divHead, "th,td"), 40);
                setMinWidth(DOMUtil.query$(self.selectors.$flowTable, "th,td"), 40);
                self.selectors.$flowTable.css({ "table-layout": "fixed", "position": "absolute", "top": 0 });
            }

            self.addScrollEvent(function (e) {

                if (self.scrollTimer) {
                    clearTimeout(self.scrollTimer);
                }

                self.scrollTimer = setTimeout(function () {
                    var $target = $(e.target),
                        top = $divBody.scrollTop();
                    $divHead.scrollLeft($target.scrollLeft());

                    if (top !== self.scrollTop) {
                        self.render({
                            scrollTop: top,
                            scrollOffset: (top - self.scrollTop)
                        });
                        self.scrollTop = top;
                    }

                }, 5);

                e.preventDefault();
                e.stopImmediatePropagation();
            });
        }
    };

    BodyContainer.prototype.addContainerHeight = function (currentHeight, previousHeight) {
        var self = this;

        self.pageHeight += currentHeight;
        if (previousHeight) {
            self.pageHeight -= previousHeight;
        }
    };

    BodyContainer.prototype.adjustContainerHeight = function () {
        var self = this;

        if (self.pageHeight === self.lastApplyPageHeight) {
            return;
        }

        self.selectors.$flowScroll[0].style.height = self.pageHeight + "px";
        self.lastApplyPageHeight = self.pageHeight;

    };


    BodyContainer.prototype.getRowHeight = function (row) {
        var clientHeight;
        if (!this.paramContructors.options.autoRowHeight && this.fixRowHeight < 0) {
            clientHeight = row.clientHeight;
            this.fixRowHeight = clientHeight;
        }
        return this.fixRowHeight;
    };


    BodyContainer.prototype.insertRow = function (index, dataRow) {
        var self = this,
            $flowTable = self.selectors.$flowTable,
            $fixTable = self.selectors.$fixTable,
            flowTable, fixTable,
            bodyRow = new BodyRow(dataRow.element, self.paramContructors),
            flowRow, fixRow,
            needRecalcMedian = false;

        initializeBodyRowEvent(self, bodyRow);

        bodyRow.rowid = dataRow.rowid;
        flowTable = $flowTable[0];
        flowRow = bodyRow.containerElement[0];

        if (index >= self.rows.length) {
            flowTable.appendChild(flowRow);
            if (self.paramContructors.options.fixedColumn) {
                fixTable = $fixTable[0];
                fixRow = bodyRow.containerElement[1];
                fixTable.appendChild(fixRow);
                if (!dataRow.height) {
                    dataRow.height = DOMUtil.adjustHeight($(flowRow), $(fixRow), 0,
                        self.paramContructors.options.autoRowHeight ? -1 :
                        self.fixRowHeight < 0 ? -1 : self.fixRowHeight);
                    if (!self.paramContructors.options.autoRowHeight) {
                        self.fixRowHeight = dataRow.height;
                    }
                    needRecalcMedian = true;
                }
            }
            else {
                if (!dataRow.height) {
                    dataRow.height = flowRow.clientHeight;
                    needRecalcMedian = true;
                }
            }
            self.rows.push(bodyRow);
        }
        else {
            flowTable.insertBefore(flowRow, self.rows[index].containerElement[0]);
            if (self.paramContructors.options.fixedColumn) {
                fixTable = $fixTable[0];
                fixRow = bodyRow.containerElement[1];
                fixTable.insertBefore(fixRow, self.rows[index].containerElement[1]);
                if (dataRow.height === 0) {
                    dataRow.height = DOMUtil.adjustHeight($(flowRow), $(fixRow), 0,
                        self.paramContructors.options.autoRowHeight ? -1 :
                        self.fixRowHeight < 0 ? -1 : self.fixRowHeight);
                    if (!self.paramContructors.options.autoRowHeight) {
                        self.fixRowHeight = dataRow.height;
                    }
                    needRecalcMedian = true;
                }
            }
            else {
                if (!dataRow.height) {
                    dataRow.height = flowRow.clientHeight;
                    needRecalcMedian = true;
                }
            }
            self.rows.splice(index, 0, bodyRow);
        }
        if (needRecalcMedian) {
            this.rowHeights.push(dataRow.height);
            this.medianHeight = App.num.median(this.rowHeights);
            self.medianHeight = self.paramContructors.options.autoRowHeight && self.fixRowHeight > -1 ? self.fixRowHeight : self.medianHeight;

            self.addContainerHeight(dataRow.height, dataRow.temporaryHeight);
        }

        //hidden column if exist
        if (App.isNumeric(self.indexColumnHide)) {
            bodyRow.hideColumn(self.indexColumnHide);
        } else {
            //show all column if hidden not exist
            var showChildCol = function (targetRow) {
                var $targetRow = DOMUtil.query(targetRow, "tr"),
                    i, ilen, row;
                for (i = 0, ilen = $targetRow.length; i < ilen; i++) {
                    row = $targetRow[i];
                    DOMUtil.query$(row, "td").css("display", "");
                }
            };
            showChildCol(flowRow);
            if (fixRow) {
                showChildCol(fixRow);
            }
        }
    };

    var removeContainerElement = function (containerElement) {
        if (!App.isUndefOrNull(containerElement[0].parentNode)) {
            containerElement[0].parentNode.removeChild(containerElement[0]);
        }

        if (!App.isUndefOrNull(containerElement[1]) && !App.isUndefOrNull(containerElement[1].parentNode)) {
            containerElement[1].parentNode.removeChild(containerElement[1]);
        }
    };

    BodyContainer.prototype.removeRow = function (index) {
        var self = this,
            targetRow = self.rows[index],
            rowRemove = self.paramContructors.accessor.getBodyView().cache.id(targetRow.rowid);

        targetRow.detachCellFocusEvent();
        targetRow.detachCellClickEvent();
        targetRow.detachChangeEvent();
        targetRow.detachCellBlurEvent();

        if (!!targetRow.hasFocus) {
            targetRow.containerElement.find(":focusable").each(function (i, element) {
                element.blur();
            });
        }

        removeContainerElement(targetRow.containerElement);

        self.rows.splice(index, 1);
        rowRemove.visible = false;
        if (self.selectedRowElement === rowRemove) {
            self.selectedRowElement = undefined;
        }
        return rowRemove;
    };

    //get index of DataRow in Body
    BodyContainer.prototype.getRowIndex = function (rowid) {
        var self = this,
            elements = self.selectors.$flowTable[0].tBodies,
            index,
            targetElement;

        for (index = 0; index < elements.length; index++) {
            targetElement = elements[index];
            if (targetElement.getAttribute("data-rowid") === rowid) {
                index--;
                break;
            }
        }

        return index;
    };

    BodyContainer.prototype.clear = function () {
        var self = this;
        self.pageHeight = 0;
        self.renderHeight = 0;
        self.scrollTop = 0;
        self.scrollHeight = 0;
        self.lastApplyPageHeight = undefined;
        self.selectedRowElement = undefined;
        self.lastSelectedRowElement = undefined;
        self.medianHeight = self.templateHeight;

        self.rows.forEach(function (row) {
            var container = row.containerElement[0];
            container.parentNode.removeChild(container);
            container = row.containerElement[1];
            if (container) {
                container.parentNode.removeChild(container);
            }
        });

        self.rows = [];
        self.rowHeights = [];
        self.rowHeights.push(self.templateHeight);

        self.selectors.$flowScroll[0].parentNode.scrollTop = 0;
        self.selectors.$flowScroll[0].style.height = "0px";

        //self.selectors.$flowScroll.height("auto");
        if (self.selectors.$fixScroll) {
            self.selectors.$fixScroll[0].parentNode.scrollTop = 0;
            self.selectors.$fixScroll[0].style.height = "0px";
            //self.selectors.$fixScroll.height("auto");
        }

        self.setContainerOffset(0);
    };

    BodyContainer.prototype.cloneTemplateItem = function () {
        var self = this,
            $cloneflow, $clonefix;

        $cloneflow = DOMUtil.cloneNode(DOMUtil.single$(self.selectors.flowElement, ".item-tmpl"));

        if (self.paramContructors.options.fixedColumn) {
            $clonefix = DOMUtil.cloneNode(DOMUtil.single$(self.selectors.fixElement, ".dt-fix-body table tbody.item-tmpl"));
        }
        return [$clonefix, $cloneflow];
    };

    BodyContainer.prototype.addScrollEvent = function (handler) {
        this.selectors.$divBody.on("scroll", handler);
    };

    BodyContainer.prototype.detachScrollEvent = function () {
        this.selectors.$divBody.off("scroll");
    };

    BodyContainer.prototype.getPhysicalHeight = function () {
        return this.selectors.$divBody[0].clientHeight;
    };

    function FooterRow(containerElement) {
        this.containerElement = containerElement;
        this.initialize.call(this);
    }

    FooterRow.prototype.initialize = function () {
        var self = this;

        if (self.containerElement.length > 1) {
            this.fixElement = self.containerElement[0];
            this.flowElement = self.containerElement[1];
        }
        else {
            this.flowElement = self.containerElement[0];
        }
    };

    FooterRow.prototype.addCellClickEvent = function (handler) {
        this.containerElement.on("click", "td", handler);
    };

    FooterRow.prototype.detachCellClickEvent = function () {
        this.containerElement.off("click");
    };

    var validateFootRowLength = function ($foot) {
        if (DOMUtil.query$($foot, "tr").length > 1) {
            throw new Error("多段行での列の表示・非表示はサポートされません。");
        }
    };

    FooterRow.prototype.showColumn = function (index) {
        var colid;
        if (typeof index === "string") {
            colid = index;
        }

        var self = this,
            $container = $(self.flowElement).closest(".dt-container"),
            $foot = DOMUtil.query$($container, ".flow-container .dt-foot tfoot"),
            $fixcols = $(self.fixElement).find(".dt-foot td").length,
            $flowcols = $(self.flowElement).find(".dt-foot td").length,
            showTdCol = function ($targetFoot) {
                if (colid) {
                    return DOMUtil.query$($targetFoot, "td[data-col='" + colid + "'])");
                } else {
                    var tbody, td, i, ilen;
                    for (i = 0, ilen = $targetFoot.length; i < ilen; i++) {
                        tbody = $targetFoot[i];
                        td = DOMUtil.query$(tbody, "td");
                        if (td[index]) {
                            td[index].style.display = "";
                        }
                    }
                }
            };

        validateFootRowLength($foot);

        if (index <= ($fixcols - 1) && $flowcols) {
            var $fixFoot = DOMUtil.query$($container, ".fix-columns .dt-foot tfoot");
            showTdCol($fixFoot);
            var outerWidth = $fixFoot.outerWidth();
            $foot.closest("table").css("margin-left", outerWidth);
        }
        else {
            index -= $fixcols;
            showTdCol($foot);
        }
    };

    FooterRow.prototype.hideColumn = function (index) {
        var colid;
        if (typeof index === "string") {
            colid = index;
        }
        var self = this,
            $container = $(self.flowElement).closest(".dt-container"),
            $foot = DOMUtil.query$($container, ".flow-container .dt-foot tfoot"),
            $fixcols = $(self.fixElement).find(".dt-foot td").length,
            $flowcols = $(self.flowElement).find(".dt-foot td").length,
            hideTdCol = function ($targetFoot) {
                if (colid) {
                    return DOMUtil.query$($targetFoot, "td[data-col='" + colid + "'])");
                } else {
                    var tbody, td, i, ilen;
                    for (i = 0, ilen = $targetFoot.length; i < ilen; i++) {
                        tbody = $targetFoot[i];
                        td = DOMUtil.query$(tbody, "td");
                        if (td[index]) {
                            td[index].style.display = "none";
                        }
                    }
                }
            };

        validateFootRowLength($foot);

        if (index <= ($fixcols - 1) && $flowcols) {

            var $fixFoot = DOMUtil.query$($container, ".fix-columns .dt-foot tfoot");
            hideTdCol($fixFoot);
            var outerWidth = $fixFoot.outerWidth();
            $foot.closest("table").css("margin-left", outerWidth);
        }
        else {
            index -= $fixcols;
            hideTdCol($foot);
        }
    };

    function FooterContainer(paramContructors) {
        this.paramContructors = paramContructors;
        this.row = {};
        this.initialize.call(this);
    }

    FooterContainer.prototype.initialize = function () {
        var self = this,
            $divFoot, $fixFoot,
            $foot = DOMUtil.single$(self.paramContructors.srcElement, "tfoot").clone(),
            $flowTable = self.paramContructors.srcElement, $flowTfootCells;

        if ($foot.length <= 0) {
            return;
        }

        DOMUtil.single$(self.paramContructors.srcElement, "tfoot").remove();

        if (self.paramContructors.containerElements.length > 1) {
            this.fixElement = self.paramContructors.containerElements[0];
            this.flowElement = self.paramContructors.containerElements[1];
        }
        else {
            this.flowElement = self.paramContructors.containerElements[0];
        }

        $foot.show();
        $flowTfootCells = DOMUtil.query$(this.flowElement, "tfoot th,tfoot td");
        $flowTfootCells.css({ height: "0px", padding: "0px", borderTop: 0, borderBottom: 0 });
        $flowTfootCells.text("");

        $divFoot = $("<div class='dt-foot' style='overflow:hidden'><table></table></div>").appendTo(self.flowElement);

        this.row = new FooterRow(self.paramContructors.containerElements);
        DOMUtil.single$($divFoot, "table").prop("class", $flowTable.prop("class")).append($foot);
        $divFoot.css("margin-right", BrowserSupport.scrollBar.width);

        if (self.paramContructors.options.fixedColumn === true) {
            DOMUtil.single$($divFoot, "table").css("width", self.paramContructors.options.innerWidth);
            DOMUtil.single$($divFoot, "table").css("max-width", self.paramContructors.options.innerWidth);

            $fixFoot = createFixedColumns($divFoot, "td", "dt-fix-foot", self.paramContructors.options.fixedColumns);
            $fixFoot.css("margin-right", "");
            $fixFoot.css("margin-top", BrowserSupport.scrollBar.height - 1);
            this.fixElement.append($fixFoot);

            var w = DOMUtil.single$($fixFoot, "table").width();
            DOMUtil.single$($fixFoot, "table").css("width", w);
            DOMUtil.single$($divFoot, "table").css("margin-left", w - 1);
            $divFoot.css("min-width", w + 200 - BrowserSupport.scrollBar.width);

            DOMUtil.adjustHeight($divFoot, $fixFoot, 1);

            if (self.paramContructors.options.responsive) {
                if (!BrowserSupport.isLteIE8()) {
                    DOMUtil.query$($divFoot, "table").css("table-layout", "fixed");
                    DOMUtil.query$($fixFoot, "table").css("table-layout", "fixed");
                }
            }
        }
        else {
            if (self.paramContructors.options.responsive) {
                var setMinWidth = function (cells, width) {
                    var i, ilen, cell;
                    for (i = 0, ilen = cells.length; i < ilen; i++) {
                        cell = cells[i];
                        if (!$(cell).css("width")) {
                            $(cell).css("min-width", width);
                        }
                    }
                };
                setMinWidth(DOMUtil.query$($divFoot, "th,td"), 40);
                DOMUtil.single$(this.flowElement, ".dt-head table").css("table-layout", "fixed");
            }
        }
    };

    FooterContainer.prototype.addScrollEvent = function (handler) {
        this.flowElement.on("scroll", ".dt-foot", handler);
    };

    FooterContainer.prototype.detachScrollEvent = function () {
        this.flowElement.off("scroll");
    };

    function HeaderView(paramContructors) {
        this.paramContructors = paramContructors;
        this.HeaderContainer = function () { };
        this.initialize(this);
    }

    HeaderView.prototype.initialize = function () {
        var self = this;
        if (!App.isUndefOrNull(self.paramContructors.options.modules.HeaderContainer)) {
            self.HeaderContainer = new self.paramContructors.options.modules.HeaderContainer(self.paramContructors);
        } else {
            self.HeaderContainer = new HeaderContainer(self.paramContructors);
        }
    };

    HeaderView.prototype.showColumn = function (index) {
        var self = this,
            container = self.HeaderContainer;

        container.showColumn(index);
    };

    HeaderView.prototype.hideColumn = function (index) {
        var self = this,
            container = self.HeaderContainer;

        container.hideColumn(index);
    };

    HeaderView.prototype.showWait = function (operation) {
        var self = this,
            container = self.HeaderContainer;

        container.showWait(operation);
    };

    HeaderView.prototype.hideWait = function (operation) {
        var self = this,
            container = self.HeaderContainer;

        container.hideWait(operation);
    };

    function BodyView(paramContructors) {
        this.resizeTimer = null;
        this.paramContructors = paramContructors;
        this.cache = DataRowCache;
        this.lastTableOffset = 0;
        this.initialize.call(this);
        var self = this;
        // TODO: resize
        if (paramContructors.options.resize === true) {
            $(window).on("resize", function () {
                self.resize();
            });
            $(window).resize();
        }
    }

    BodyView.prototype.initialize = function () {
        var self = this;
        if (self.paramContructors.containerElements.length > 1) {
            this.fixElement = self.paramContructors.containerElements[0];
            this.flowElement = self.paramContructors.containerElements[1];
        }
        else {
            this.flowElement = self.paramContructors.containerElements[0];
        }
        self.cache = new DataRowCache();

        if (!App.isUndefOrNull(self.paramContructors.options.modules.BodyContainer)) {
            self.BodyContainer = new self.paramContructors.options.modules.BodyContainer(self.paramContructors);
        } else {
            self.BodyContainer = new BodyContainer(self.paramContructors);
        }

        //TODO: bodyContainer に持たせるか検討

        self.viewHeight = self.BodyContainer.getPhysicalHeight();

        self.BodyContainer.render = self.render.bind(self);
    };

    BodyView.prototype.selectedRow = function () {
        return this.BodyContainer.getSelectedRow();
    };

    BodyView.prototype.lastSelectedRow = function () {
        return this.BodyContainer.getLastSelectedRow();
    };

    BodyView.prototype.clearAllRender = function () {

        var self = this,
            container = self.BodyContainer,
            i, len = container.rows.length;

        for (i = 0; i < len; i++) {
            removeContainerElement(container.rows[i].containerElement);
        }

        container.rows = [];
        container.renderHeight = 0;
        container.rowHeights = [];
        container.rowHeights.push(container.templateHeight);
    };

    DataRowCache.prototype.getFromTopPosition = function (top, medianHeight) {
        var self = this,
            i, ilen, dataRow,
            height = 0;

        for (i = 0, ilen = self.rowids.length; i < ilen; i++) {
            dataRow = self.id(self.rowids[i]);
            height += (dataRow.height || dataRow.temporaryHeight || medianHeight);
            if (height >= top) {
                break;
            }
        }

        return { row: dataRow, offset: (height - top), index: i };
    };

    BodyContainer.prototype.clearBodyRows = function () {
        var self = this,
            flowRow, fixRow;

        self.rows.forEach(function (bodyRow) {
            flowRow = bodyRow.containerElement[0];
            self.selectors.$flowTable[0].removeChild(flowRow);
            if (self.paramContructors.options.fixedColumn) {
                fixRow = bodyRow.containerElement[1];
                self.selectors.$fixTable[0].removeChild(fixRow);
            }
        });

        self.rows = []; // Array of BodyRow.
        self.renderHeight = 0;
    };

    BodyContainer.prototype.isDisplayDataRow = function (dataRow) {
        var self = this,
            i, ilen, bodyRow;

        for (i = 0, ilen = self.rows.length; i < ilen; i++) {
            bodyRow = self.rows[i];
            if (dataRow.rowid === bodyRow.rowid) {
                return true;
            }
        }

        return false;
    };

    BodyContainer.prototype.setContainerOffset = function (containerOffset) {
        var self = this;
        self.selectors.$flowTable.css("top", containerOffset);
        if (self.paramContructors.options.fixedColumn) {
            self.selectors.$fixTable.css("top", containerOffset);
        }
    };

    BodyContainer.prototype.addContainerOffset = function (containerOffset) {
        var self = this;
        var newTop = Math.max(parseInt(self.selectors.$flowTable.css("top"), 10) + containerOffset, 0);
        self.setContainerOffset(newTop);
    };


    BodyContainer.prototype.syncScrollPosition = function () {
        var self = this;
        if (self.paramContructors.options.fixedColumn) {
            if (self.selectors.$flowScroll[0].parentNode.scrollTop !== self.selectors.$fixScroll[0].parentNode.scrollTop) {
                self.selectors.$fixScroll[0].parentNode.scrollTop = self.selectors.$flowScroll[0].parentNode.scrollTop;
            }
        }
    };

    BodyContainer.prototype.getDOMRowIdAttribute = function (target) {
        var rowid,
            $target = $(target);
        rowid = $target.closest("tbody").attr("data-rowid");
        return rowid;
    };

    BodyView.prototype.render = function (options, forceOrientation) {
        var self = this,
            container = self.BodyContainer,
            isScrollDown = forceOrientation === "down" ? true :
                forceOrientation === "up" ? false : (options.scrollOffset > 0),
            needAllClear = forceOrientation === "rewrite",
            targetIndex = 0,
            requireHeight = 0,
            i, l, forceZeroTop = false;

        // A) Get DataRow.
        var position = self.cache.getFromTopPosition(options.scrollTop, container.medianHeight);
        var bottomPosition = self.cache.getFromTopPosition(options.scrollTop + self.viewHeight, container.medianHeight);
        var lowRow = self.cache.rows[(position.row || { rowid: -1 }).rowid] || {};

        self.lastRenderPositionIndex = position.index;

        // B) Is DataRow displayed?
        if (!needAllClear && container.isDisplayDataRow(position.row)) {

            //補正要不要のチェック
            targetIndex = 0;
            App.array.find(container.rows, function (row, index) {
                if (position.row.rowid === row.rowid) {
                    targetIndex = index;
                    return true;
                }
            });

            requireHeight = Math.max(self.viewHeight - container.rows.slice(targetIndex).reduce(function (init, value) {
                var row = self.cache.rows[value.rowid];
                return init + (row.height || row.temporaryHeight || container.medianHeight);
            }, 0), 0);
            if (requireHeight) {
                if ((lowRow.height || container.medianHeight) < self.viewHeight) {
                    needAllClear = true;
                }
            } else {
                needAllClear = false;
                var rowIndex, nextRow, offsetHeight = 0;
                // E) Add DataRows to display area.
                // case scroll down
                if (isScrollDown) {
                    targetIndex = -1;
                    App.array.find(container.rows, function (row, index) {
                        if (bottomPosition.row.rowid === row.rowid) {
                            targetIndex = index;
                            return true;
                        }
                    });
                    if (targetIndex < 0) { //最終行にある状態で行追加された後など
                        requireHeight = self.viewHeight;
                    } else {
                        requireHeight = Math.max(self.viewHeight - container.rows.slice(targetIndex).reduce(function (init, value) {
                            var row = self.cache.rows[value.rowid];
                            return init + (row.height || row.temporaryHeight || container.medianHeight);
                        }, 0), 0);
                    }
                    while (requireHeight > 0) {
                        rowIndex = self.cache.rowids.lastIndexOf(container.rows[container.rows.length - 1].rowid) + 1;
                        nextRow = self.cache.index(rowIndex);
                        if (!nextRow) {
                            break;
                        }
                        container.insertRow(container.rows.length, nextRow);
                        requireHeight -= nextRow.height;
                    }
                }
                    // case scroll up
                else {
                    targetIndex = 0;
                    App.array.find(container.rows, function (row, index) {
                        if (position.row.rowid === row.rowid) {
                            targetIndex = index;
                            return true;
                        }
                    });
                    if (targetIndex > 0 || (targetIndex === 0 && position.index !== 0)) {
                        requireHeight = Math.max(self.viewHeight - container.rows.slice(0, targetIndex).reduce(function (init, value) {
                            var row = self.cache.rows[value.rowid];
                            return init + (row.height || row.temporaryHeight || container.medianHeight);
                        }, 0), 0);
                        while (requireHeight > 0) {
                            rowIndex = (self.cache.rowids.lastIndexOf(container.rows[0].rowid) - 1);
                            if (rowIndex < 0) {
                                forceZeroTop = true;
                                break;
                            }
                            nextRow = self.cache.index(rowIndex);
                            container.insertRow(0, nextRow);
                            requireHeight -= nextRow.height;
                            offsetHeight -= nextRow.height;
                        }
                        if ((self.cache.rowids.lastIndexOf(container.rows[0].rowid) - 1) < 0) {
                            forceZeroTop = true;
                        }
                    }
                }

                // F) Remove DataRows from display area.
                // case scroll down
                if (isScrollDown && position.row.rowid !== bottomPosition.row.rowid) {
                    targetIndex = 0;
                    App.array.find(container.rows, function (row, index) {
                        if (position.row.rowid === row.rowid) {
                            targetIndex = index;
                            return true;
                        }
                    });
                    if (targetIndex > 0) {
                        var restPos = container.rows.slice(0, targetIndex).reverse().reduce(function (init, value, index) {
                            if (init.height < self.viewHeight) {
                                var row = self.cache.rows[value.rowid];
                                init.height += (row.height || row.temporaryHeight || container.medianHeight);
                                init.index = index;
                            }
                            return init;
                        }, { height: 0, index: 0 });
                        for (i = 0, l = targetIndex - restPos.index - 1; i < l; i++) {
                            offsetHeight += self.cache.rows[container.rows[0].rowid].height;
                            container.removeRow(0);
                        }
                    }
                }

                    // case scroll up
                else if (position.row.rowid !== bottomPosition.row.rowid) {
                    targetIndex = 0;
                    App.array.find(container.rows, function (row, index) {
                        if (bottomPosition.row.rowid === row.rowid) {
                            targetIndex = index;
                            return true;
                        }
                    });
                    if (targetIndex > 0) {
                        var removeStart = container.rows.slice(targetIndex + 1).reduce(function (init, value, index) {
                            if (init.height < self.viewHeight) {
                                var row = self.cache.rows[value.rowid];
                                init.height += (row.height || row.temporaryHeight || container.medianHeight);
                                init.index = index;
                            }
                            return init;
                        }, { height: 0, index: targetIndex });
                        if (removeStart.height > self.viewHeight) {
                            for (i = targetIndex + removeStart.index; i < container.rows.length; i++) {
                                container.removeRow(container.rows.length - 1); //targetIndex2 + removeStart.index);
                            }
                        }
                    }
                }

                if (forceZeroTop) {
                    container.setContainerOffset(0);
                } else {
                    container.addContainerOffset(offsetHeight);
                }

            }
        } else {
            needAllClear = true;
        }
        if (needAllClear) {

            // C) Clear display area.
            container.clearBodyRows();

            // D) Add DataRows to display area.
            targetIndex = position.index + 1;
            offsetHeight = 0;
            requireHeight = (self.viewHeight * 2);
            while (requireHeight > 0) {
                nextRow = self.cache.index(targetIndex++);
                if (!nextRow) {
                    break;
                }
                container.insertRow(container.rows.length, nextRow);
                if (targetIndex === self.cache.rowids.length) {
                    break;
                }
                requireHeight -= nextRow.height;
            }
            requireHeight = self.viewHeight;
            targetIndex = position.index;
            while (requireHeight > 0) {
                nextRow = self.cache.index(targetIndex--);
                if (!nextRow) {
                    break;
                }
                container.insertRow(0, nextRow);
                requireHeight -= nextRow.height;
                offsetHeight += nextRow.height;
            }
            container.setContainerOffset(!!position.index ? Math.max(options.scrollTop - self.viewHeight + position.offset, 0) : 0);
        }

        container.adjustContainerHeight();
        container.syncScrollPosition();
    };

    BodyView.prototype.addRow = function (operation, isFocus) {
        var self = this,
            container = self.BodyContainer,
            cloneItem, $fixItem, $item, row;

        cloneItem = container.cloneTemplateItem();

        if (App.isUndefOrNull(cloneItem)) {
            return;
        }

        $fixItem = cloneItem[0];
        $item = cloneItem[1];

        hideSortIcons(self.paramContructors.srcElement.closest(".dt-container"));


        if (operation) {
            $item = $item.add($fixItem);
            operation($item);
        }
        $item.removeClass("item-tmpl");
        $item.addClass("new");
        $item.show();
        row = new DataRow($item);
        $item.attr("data-rowid", row.rowid);

        row.temporaryHeight = container.medianHeight;
        container.addContainerHeight(row.temporaryHeight);

        self.cache.add(row);

        //container.pageHeight += container.medianHeight;
        var newScrollTop = container.selectors.$flowScroll[0].parentNode.scrollHeight - self.viewHeight;

        container.adjustContainerHeight();

        setTimeout(function () {

            if (container.selectors.$flowScroll[0].parentNode.scrollTop !== newScrollTop) {
                container.selectors.$flowScroll[0].parentNode.scrollTop = newScrollTop + container.medianHeight;
            } else {
                self.render({
                    scrollTop: container.getScrollTop(), // + container.medianHeight,
                    scrollOffset: 0
                }, "down");
            }
            setTimeout(function () {
                if (row && isFocus) {
                    setFocus(row, container);
                }
            }, 10);
        }, 10);
    };

    BodyView.prototype.addRows = function (data, operation, isFocus) {
        var self = this,
            container = self.BodyContainer,
            $item, $fixItem, row, index, ilen, item,
            focusItem;

        hideSortIcons(self.paramContructors.srcElement.closest(".dt-container"));

        for (index = 0, ilen = data.length; index < ilen; index++) {
            var cloneitem = container.cloneTemplateItem();

            if (App.isUndefOrNull(cloneitem)) {
                return false;
            }

            $fixItem = cloneitem[0];
            $item = cloneitem[1];

            item = data[index];
            $item.__index = index;
            if (operation) {
                $item = $item.add($fixItem);
                operation($item, item);
            }

            $item.removeClass("item-tmpl");
            $item.addClass("new");
            $item.show();
            row = new DataRow($item);
            $item.attr("data-rowid", row.rowid);

            row.temporaryHeight = container.medianHeight;
            container.addContainerHeight(row.temporaryHeight);

            self.cache.add(row);

            if (isFocus === true && index <= 0) {
                focusItem = row;
            }
        }


        var newScrollTop = container.selectors.$flowScroll[0].parentNode.scrollHeight - self.viewHeight;

        container.adjustContainerHeight();

        setTimeout(function () {

            if (container.selectors.$flowScroll[0].parentNode.scrollTop !== newScrollTop) {
                container.selectors.$flowScroll[0].parentNode.scrollTop = newScrollTop + container.medianHeight;
            } else {
                self.render({
                    scrollTop: container.getScrollTop(),
                    scrollOffset: 0
                }, "down");
            }
            setTimeout(function () {
                if (focusItem) {
                    setFocus(focusItem, container);
                }
            }, 10);

        }, 10);
    };

    BodyView.prototype.getRow = function (target) {
        var self = this,
            $target = $(target),
            rowid = $target.attr("data-rowid"),
            row;

        if (App.isUndef(rowid)) {
            rowid = $target.closest("tbody").attr("data-rowid");
        }

        row = self.cache.id(rowid);

        return row;
    };

    BodyView.prototype.enableRowCount = function (operation) {
        var self = this;
        if (App.isFunc(operation)) {
            operation(self.cache.rowids.length);
        }
    };

    BodyView.prototype.updateRow = function (data, target) {
        var self = this,
            $target = $(target),
            $selectItem = $target.closest("tbody"),
            rowid = $selectItem.attr("data-rowid");
        Object.keys(data).forEach(function (key) {
            self.cache.id(rowid)[key] = data[key];
        });

    };

    BodyView.prototype.deleteRow = function (target, operation) {
        var self = this,
            container = self.BodyContainer,
            rowid = container.getDOMRowIdAttribute(target),
            row, nextRowSelected;

        if (App.isUndef(rowid)) {
            return;
        }

        row = self.cache.id(rowid);

        //call removeRow from BodyContainer
        var targetIndex = container.getRowIndex(rowid);

        //If target row deleted is last, set previous row selected
        //If not, set next row selected
        if (container.rows.length > 1) {
            if (targetIndex === container.rows.length - 1) {
                nextRowSelected = self.cache.id($(container.rows[targetIndex - 1].containerElement[0]).attr("data-rowid"));
            } else {
                nextRowSelected = self.cache.id($(container.rows[targetIndex + 1].containerElement[0]).attr("data-rowid"));
            }
        }

        var removedRow = container.removeRow(targetIndex) || {};
        container.addContainerHeight(0 - (removedRow.height || removedRow.temporaryHeight || 0));
        container.adjustContainerHeight();

        self.cache.remove(rowid);

        container.selectedRowElement = undefined;

        if (container.rows.length <= 0) {
            return;
        }

        var lastRow = container.rows[container.rows.length - 1],
            indexLastRow = self.cache.rowids.indexOf($(lastRow.containerElement[0]).attr("data-rowid")),
            nextLastIndex = indexLastRow + 1,
            nextLastRow = self.cache.index(nextLastIndex);

        if (!App.isUndefOrNull(nextLastRow)) {
            self.render({
                scrollTop: container.getScrollTop(),
                scrollOffset: 0
            });
        }

        if (App.isFunc(operation)) {
            operation(row.element);
        }

        if (nextRowSelected) {
            setTimeout(function () {
                setFocus(nextRowSelected, container);
            }, 10);
        }
    };

    BodyView.prototype.clear = function () {
        var self = this;

        hideSortIcons(self.paramContructors.srcElement.closest(".dt-container"));
        self.cache = new DataRowCache();
        self.BodyContainer.clear();
    };

    BodyView.prototype.showColumn = function (index) {
        var self = this,
            container = self.BodyContainer,
            rows = container.rows;

        $.each(rows, function (i) {
            rows[i].showColumn(index);
        });

        if (App.isNumeric(container.indexColumnHide)) {
            container.indexColumnHide = undefined;
        }
    };

    BodyView.prototype.hideColumn = function (index) {
        var self = this,
            container = self.BodyContainer,
            rows = container.rows;

        if (self.paramContructors.srcElement.is("table")) {
            index = index - 1;
            if (index < 0) {
                index = 0;
            }
            self.paramContructors.accessor.getHeaderView().HeaderContainer.row.hideColumn(index);
            //hideColumn of thead in BodyContainer


            //DOMUtil.query$(container.selectors.$divBody, "thead tr th")[index].css("display", "none");
            //            container.selectors.flowElement.find(".dt-body thead tr th")[index].css("display", "none");
        }

        $.each(rows, function (i) {
            rows[i].hideColumn(index);
        });


        //cache index of row have hidden
        container.indexColumnHide = index;

    };

    BodyView.prototype.sort = function (propertyName, desc) {
        var modes = {
            none: 0,
            ascend: 1,
            descend: 2
        },
            self = this,
            valOrText = function (datarow, prop) {
                var target, val, formatAttr;

                if (App.isUndefOrNull(prop)) {
                    return;
                }

                target = datarow.element[0].querySelector("[data-prop='" + prop + "']");
                if (!target && datarow.element.length > 1) {
                    target = datarow.element[1].querySelector("[data-prop='" + prop + "']");
                }

                val = target.value || target.innerText;
                formatAttr = target.getAttribute("data-format");

                if (self.paramContructors.options.parse && self.paramContructors.options.parse.converteByFormatDataAnnotation && formatAttr) {
                    val = self.paramContructors.options.parse.converteByFormatDataAnnotation(formatAttr, $(target), val);
                }

                if (val && val !== "") {
                    return val;
                }

                return target.innerText;
            },
            isNum = function (aTarget, bTarget) {
                return (Object.prototype.toString.call(aTarget) === "[object Number]") &&
                    (Object.prototype.toString.call(bTarget) === "[object Number]");
            },
            isDate = function (aTarget, bTarget) {
                return (Object.prototype.toString.call(aTarget) === "[object Date]") &&
                    (Object.prototype.toString.call(bTarget) === "[object Date]");
            },
            compareRow = function (a, b) {
                var values = {};
                values.a = valOrText(a, propertyName);
                values.b = valOrText(b, propertyName);

                if (desc === modes.ascend) {
                    if (isNum(values.a, values.b)) {
                        return values.a - values.b;
                    } else if (isDate(values.a, values.b)) {
                        return values.a.getTime() - values.b.getTime();
                    } else {
                        return (values.a < values.b) ? -1 : 1;
                    }
                }
                else {
                    if (isNum(values.a, values.b)) {
                        return values.b - values.a;
                    } else if (isDate(values.a, values.b)) {
                        return values.b.getTime() - values.a.getTime();
                    } else {
                        return (values.a > values.b) ? -1 : 1;
                    }
                }
            };

        self.cache.rowids.sort(function (x, y) {
            return compareRow(self.cache.id(x), self.cache.id(y));
        });
    };

    BodyContainer.prototype.resizeFitToBottom = function (self, $container, $divBody, $divFoot, $fixBody, $fixHead, $fixFoot, complete) {

        if (self.resizeTimer) {
            clearTimeout(self.resizeTimer);
        }

        self.resizeTimer = setTimeout(function () {
            var offsetTop = $container.offset().top,
                currentHeight = $(window).height(),
                bodyHeight = Math.floor(currentHeight - offsetTop - self.paramContructors.options.resizeOffset - DOMUtil.getHeight($divFoot));

            if (bodyHeight > self.viewHeight) {
                $divBody.height(bodyHeight);
                if (self.paramContructors.options.fixedColumn === true) {
                    $fixBody.height((bodyHeight + 1) - BrowserSupport.scrollBar.height);
                }
                $divBody.closest(".part").css("margin-bottom", "0px");
                // TODO
            } else {
                $divBody.height(bodyHeight);
                if (self.paramContructors.options.fixedColumn === true) {
                    $fixBody.height((bodyHeight + 1) - BrowserSupport.scrollBar.height);
                }
                $divBody.closest(".part").css("margin-bottom", "32px");
            }

            if (self.paramContructors.options.fixedColumn === true) {
                $fixFoot.css("top", DOMUtil.getHeight($fixHead) + DOMUtil.getHeight($fixBody) + BrowserSupport.scrollBar.height);
            }

            if (App.isFunc(complete)) {
                complete();
            }
        }, 5);
    };

    BodyContainer.prototype.getScrollTop = function () {
        return this.selectors.$flowScroll[0].parentNode.scrollTop;
    };

    BodyContainer.prototype.resize = function (complete) {
        var self = this,
            $container = self.selectors.flowElement.closest(".dt-container"),
            $divBody = DOMUtil.single$(self.selectors.flowElement, ".dt-body"),
            $fixBody = DOMUtil.single$(self.selectors.fixElement, ".dt-body"),
            $fixHead = DOMUtil.single$(self.selectors.fixElement, ".dt-head"),
            $divFoot = DOMUtil.single$(self.selectors.flowElement, ".dt-foot"),
            $fixFoot = DOMUtil.single$(self.selectors.fixElement, ".dt-foot");

        self.resizeFitToBottom(self, $container, $divBody, $divFoot, $fixBody, $fixHead, $fixFoot, complete);
    };

    BodyView.prototype.resize = function () {
        var self = this,
            container = self.BodyContainer;
        container.resize(function () {
            self.viewHeight = container.getPhysicalHeight();
            self.render({
                scrollTop: container.getScrollTop(),
                scrollOffset: 0
            }, "rewrite");
        });
    };

    BodyView.prototype.each = function (operation) {
        var key, row, i = 0,
            self = this;
        for (key in self.cache.rows) {
            ++i;

            row = self.cache.rows[key];
            if (row.element.is(".item-tmpl")) {
                continue;
            }

            if (operation(row, i)) {
                break;
            }
        }
    };

    BodyView.prototype.filter = function (operation) {
        var self = this,
            key, i = 0, row;
        for (key in self.cache.rows) {
            ++i;
            row = self.cache.rows[key];

            if (row.element.is(".item-tmpl")) {
                continue;
            }

            if (operation(row, i)) {
                break;
            }
        }
    };

    BodyView.prototype.setFocus = function (target) {
        var self = this,
            container = self.BodyContainer,
            rowid = container.getDOMRowIdAttribute(target),
            renderdRow = false;

        if (App.isUndef(rowid)) {
            return;
        }

        container.rows.forEach(function (row) {
            if (!renderdRow) {
                renderdRow = row.rowid === rowid;
            }
        });
        if (renderdRow) {
            target.focus();
            return;
        }

        var scrollTop = 0, i, len,
            cacheRows = $.map(self.cache.rows, function (value) {
                return [value];
            });

        for (i = 0, len = cacheRows.length; i < len; i++) {
            if (cacheRows[i].rowid === rowid) {
                break;
            }
            scrollTop += cacheRows[i].height;
        }

        container.selectors.$divBody.scrollTop(scrollTop);

        setTimeout(function () {
            target.focus();
        }, 800);

        return true;
    };

    BodyView.prototype.redraw = function () {
        var self = this,
            container = self.BodyContainer;

        self.viewHeight = container.selectors.$divBody[0].clientHeight;
        container.recalc(self.cache.rowids.map(function (rowid) {
            return self.cache.rows[rowid] || {};
        }));

        self.render({
            scrollTop: container.getScrollTop(),
            scrollOffset: 0
        }, "rewrite");
    };

    BodyView.prototype.redrawRow = function (row) {
        var self = this,
            container = self.BodyContainer;

        container.redrawRow(row);
    };


    function FooterView(paramContructors) {
        this.paramContructors = paramContructors;
        this.FooterContainer = function () { };
        this.initialize.call(this);
    }

    FooterView.prototype.initialize = function () {
        var self = this;

        if (!App.isUndefOrNull(self.paramContructors.options.modules.FooterContainer)) {
            self.FooterContainer = new self.paramContructors.options.modules.FooterContainer(self.paramContructors);
        } else {
            self.FooterContainer = new FooterContainer(self.paramContructors);
        }
    };

    function DataTable(element, options) {
        this.options = {};
        this.element = $(element);
        this.originalElement = this.element.clone();
        this.cache = new DataRowCache();
        this.pageTop = 0;
        this.pageBottom = 0;
        this.modules = {
            HeaderView: undefined,
            BodyView: undefined,
            FooterView: undefined
        };
        this.selectors = {
            thead: ".dt-head",
            tbody: ".dt-body",
            tfoot: ".dt-foot",
            scrollContainer: ".scroll-container",
            flowContainer: ".flow-container",
            fixColumns: ".fix-columns",
            container: ".dt-container"
        };

        this.options = $.extend({}, DataTable.DEFAULTS, options);
        this.initialize.call(this, this.options);
    }

    DataTable.DEFAULTS = {
        height: "100%",
        innerWidth: 2000,
        dataList: false,
        fixedColumn: false,
        fixedColumns: 0,
        sortable: false,
        footer: false,
        resize: false,
        resizeOffset: 110,
        addedOffset: 0,
        responsive: true,
        onchange: function () { },
        onsorting: function () { },
        onsorted: function () { },
        onselecting: function () { },
        onselected: function () { },
        ontabing: function () { },
        ontabed: function () { },
        autoRowHeight: false,

        /**
            * DataTable's module settings.
            */
        modules: {
            HeaderContainer: HeaderContainer,
            BodyContainer: BodyContainer,
            FooterContainer: FooterContainer
        }
    };

    DataTable.prototype.initialize = function (options) {
        var self = this,
            $body = self.element,
            $container = $body.wrap("<div class='dt-container'></div>").parent(),  // eslint-disable-line no-unused-vars
            $flowElement = $body.wrap("<div class='flow-container'></div>").parent(),
            $fixElement, containerElements = [$flowElement],
            paramContructors;

        if (options.fixedColumn === true) {
            $fixElement = $("<div class='fix-columns'></div>");
            $fixElement.insertBefore($flowElement);
            containerElements.unshift($fixElement);
        }

        var accessor = {
            getHeaderView: function () {
                return self.modules.HeaderView;
            },
            getBodyView: function () {
                return self.modules.BodyView;
            },
            getFooterView: function () {
                return self.modules.FooterView;
            }
        };

        paramContructors = {
            containerElements: containerElements,
            srcElement: self.element,
            options: options,
            accessor: accessor
        };

        self.modules = $.extend({}, options.modules, self.modules);

        self.modules.HeaderView = new HeaderView(paramContructors);

        self.modules.FooterView = new FooterView(paramContructors);

        self.modules.BodyView = new BodyView(paramContructors);
    };

    DataTable.prototype.filter = function (operation) {
        this.modules.BodyView.filter(operation);
    };

    DataTable.prototype.enableRowCount = function (operation) {
        this.modules.BodyView.enableRowCount(operation);
    };

    DataTable.prototype.addRow = function (data, isFocus) {
        this.modules.BodyView.addRow(data, isFocus);
    };

    DataTable.prototype.addRows = function (data, operation, isFocus) {
        this.modules.BodyView.addRows(data, operation, isFocus);
    };

    DataTable.prototype.getRow = function (target, operation) {
        operation(this.modules.BodyView.getRow(target));
    };

    DataTable.prototype.clear = function () {
        this.modules.BodyView.clear();
    };

    DataTable.prototype.showColumn = function (index) {
        this.modules.BodyView.showColumn(index);
    };

    DataTable.prototype.hideColumn = function (index) {
        this.modules.BodyView.hideColumn(index);
    };

    DataTable.prototype.deleteRow = function (target, operation) {
        this.modules.BodyView.deleteRow(target, operation);
    };

    DataTable.prototype.each = function (operation) {
        this.modules.BodyView.each(operation);
    };

    DataTable.prototype.selectedRow = function (operation) {
        operation(this.modules.BodyView.selectedRow());
    };

    DataTable.prototype.lastSelectedRow = function (operation) {
        operation(this.modules.BodyView.lastSelectedRow());
    };

    DataTable.prototype.showWait = function (operation) {
        this.modules.HeaderView.showWait(operation);
    };

    DataTable.prototype.hideWait = function (operation) {
        this.modules.HeaderView.hideWait(operation);
    };

    DataTable.prototype.setFocus = function (target) {
        this.modules.BodyView.setFocus(target);
    };

    DataTable.prototype.redraw = function () {
        this.modules.BodyView.redraw();
    };

    DataTable.prototype.redrawRow = function (target) {
        this.modules.BodyView.redrawRow(target);
    };

    $.fn.dataTable = function (options) {
        var args = arguments;
        return this.each(function () {

            var $self = $(this),
                data = $self.data("aw.dataTable");

            if (!data) {
                $self.data("aw.dataTable", (data = new DataTable($self, options)));
            }

            if (typeof options === "string") {
                data[options].apply(data, Array.prototype.slice.call(args, 1).concat(data));
            }
        });
    };

    $.fn.dataTable.Constructor = DataTable;

    $.fn.dataTable.DOMUtil = DOMUtil;
    $.fn.dataTable.BrowserSupport = BrowserSupport;

    $.fn.dataTable.HeaderView = HeaderView;
    $.fn.dataTable.HeaderContainer = HeaderContainer;
    $.fn.dataTable.HeaderRow = HeaderRow;
    $.fn.dataTable.BodyView = BodyView;
    $.fn.dataTable.BodyContainer = BodyContainer;
    $.fn.dataTable.BodyRow = BodyRow;
    $.fn.dataTable.FooterView = FooterView;
    $.fn.dataTable.FooterContainer = FooterContainer;
    $.fn.dataTable.FooterRow = FooterRow;

    //Empties

    function EmptyHeaderContainer() { }

    EmptyHeaderContainer.prototype.showWait = function () { };

    EmptyHeaderContainer.prototype.hideWait = function () { };

    function EmptyBodyContainer() { }

    EmptyHeaderContainer.prototype.hideSortIcons = function () { };

    EmptyBodyContainer.prototype.clear = function () { };

    EmptyBodyContainer.prototype.clearBodyRows = function () { };

    EmptyBodyContainer.prototype.getPhysicalHeight = function () { };

    EmptyBodyContainer.prototype.resize = function () { };

    EmptyBodyContainer.prototype.cloneTemplateItem = function () { };

    function EmptyFooterContainer() { }

    $.fn.dataTable.emptyHeaderContainer = EmptyHeaderContainer;
    $.fn.dataTable.emptyBodyContainer = EmptyBodyContainer;
    $.fn.dataTable.emptyFooterContainer = EmptyFooterContainer;

})(this, jQuery);
