/* global App jQuery */
/*!
 * jQuery dataTable card layout plugin.
 * Copyright(c) 2015 Archway Inc. All rights reserved.
 */

/// <reference path="../../ts/core/base.ts" />
/// <reference path="../../ts/core/num.ts" />

(function (global, $) {
    "use strict";

    var DOMUtil = {},
        BrowserSupport = {};

    App.define("App.ui.datalistModule");

    var setNewSelectedRow = function (self, selectedRow, e) {

        if (self.selectedRowElement) {
            self.lastSelectedRowElement = self.selectedRowElement;
        }

        self.selectedRowElement = selectedRow;

        if (self.paramContructors.options.onselect && selectedRow) {
            self.paramContructors.options.onselect(e || {}, selectedRow);
        }
    };

    var onListItemFocus = function (e, self) {
        self.paramContructors.options.onselecting();
        var target = $(e.target),
            divDataItem = target.closest("div.datalist-item"),
            rowid = divDataItem.attr("data-rowid"),
            row = self.paramContructors.accessor.getBodyView().cache.id(rowid);

        var divBody = self.selectors.$divBody[0],
            $container = self.selectors.$divBody.closest(".dt-container"),
            container = $container[0];

        if (!container) {
            return;
        }

        var left = target.offset().left - self.selectors.$flowTable.offset().left;
        if (left < divBody.scrollLeft) {
            divBody.scrollLeft = left - 10;
        }

        setNewSelectedRow(self, row, e);
    };

    var onListItemClick = function (e, self) {
        var target = $(e.target),
            divDataItem = target.closest("div.datalist-item"),
            rowid = divDataItem.attr("data-rowid"),
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

    var onKeyDown = function (e, self) {
        var target = $(e.target),
            dataListItem = target.closest("div.datalist-item"),
            dataRowId = dataListItem.attr("data-rowid"),
            targetRow = self.paramContructors.accessor.getBodyView().cache.id(dataRowId),
            getForcusable = function (targetFocus) {
                return $(targetFocus.element[0]).find(":focusable").toArray();
            },
            focusable = getForcusable(targetRow),
            currentTabIndex,
            currentRowIndex, nextRowIndex, nextRow, prevRowIndex, prevRow,
            KeyCodes = {
                Tab: 9,
                ArrowDown: 40,
                ArrowUp: 38
            };

        if (focusable.length <= 0) {
            return false;
        }
        else if (e.keyCode === KeyCodes.ArrowDown) {
            // On press Arrow Down Key.
            currentRowIndex = self.paramContructors.accessor.getBodyView().cache.rowids.indexOf(dataRowId);
            currentTabIndex = focusable.indexOf(target[0]);
            nextRowIndex = currentRowIndex + 1;

            if (nextRowIndex === self.paramContructors.accessor.getBodyView().cache.rowids.length) {
                return;
            }
            nextRow = self.paramContructors.accessor.getBodyView().cache.index(nextRowIndex);
            focusable = getForcusable(nextRow, self);
            if (focusable.length > 0) {
                focusable[currentTabIndex].focus();
            }
            e.preventDefault();
            e.stopImmediatePropagation();

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
            if (focusable.length > 0) {
                self.paramContructors.options.ontabing();
                focusable[currentTabIndex].focus();
                self.paramContructors.options.ontabed();
                e.preventDefault();
                e.stopImmediatePropagation();
            }
        }
    };

    function BodyRow(containerElement, paramContructors) {
        this.containerElement = containerElement;
        this.paramContructors = paramContructors;
        this.initialize.call(this);
    }

    BodyRow.prototype.initialize = function () {
        var self = this;
        self.flowElement = self.containerElement[0];
    };

    BodyRow.prototype.addChangeEvent = function (handler) {
        this.containerElement.on("change", "div.datalist-item", handler);
    };

    BodyRow.prototype.detachChangeEvent = function () {
        this.containerElement.off("change");
    };

    BodyRow.prototype.addKeyDownEvent = function (handler) {
        this.containerElement.on("keydown", handler);
    };

    BodyRow.prototype.addCellClickEvent = function (handler) {
        this.containerElement.on("click", handler);
    };

    BodyRow.prototype.detachCellClickEvent = function () {
        this.containerElement.off("click");
    };

    BodyRow.prototype.addCellFocusEvent = function (handler) {
        this.containerElement.on("focus", "div.datalist-item", handler);
    };

    BodyRow.prototype.detachCellFocusEvent = function () {
        this.containerElement.off("focus", "div.datalist-item");
    };

    BodyRow.prototype.addCellBlurEvent = function (handler) {
        this.containerElement.on("blur", "div.datalist-item", handler);
    };

    BodyRow.prototype.detachCellBlurEvent = function () {
        this.containerElement.off("blur", "div.datalist-item");
    };

    BodyRow.prototype.showColumn = function () {
        return; //not support in current

//         var self = this,
//             $flowcontainer = $(self.flowElement).closest("div.datalist-item"),
//             $targetRow = DOMUtil.query$($flowcontainer, "div.row");
//
//         if (typeof index === "string") {
//             throw new Error("index must be a number");
//         } else {
//             var row, col, i, ilen;
//             for (i = 0, ilen = $targetRow.length; i < ilen; i++) {
//                 row = $targetRow[i];
//                 col = DOMUtil.query(row, "div");
//                 if (col[index]) {
//                     col[index].style.display = "";
//                 }
//             }
//         }
    };

    BodyRow.prototype.hideColumn = function () {
        return; //not support in current

//         var self = this,
//             $flowcontainer = $(self.flowElement).closest("div.datalist-item"),
//             $targetRow = DOMUtil.query$($flowcontainer, "div.row");
//
//         if (typeof index === "string") {
//             throw new Error("index must be a number");
//         } else {
//             var row, col, i, ilen;
//             for (i = 0, ilen = $targetRow.length; i < ilen; i++) {
//                 row = $targetRow[i];
//                 col = DOMUtil.query(row, "div");
//                 if (col[index]) {
//                     col[index].style.display = "none";
//                 }
//             }
//         }
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

    BodyContainer.prototype.initialize = function () {
        var self = this,
            $container, $divBody;

        //throw exception if srcElement is HTML table

        if (self.paramContructors.srcElement.is("table")) {
            throw "HTML テーブルレイアウトはサポートされない."; //HTML table layout is not supported.
        }

        if (self.paramContructors.containerElements.length > 1) {
            self.selectors.fixElement = self.paramContructors.containerElements[0];
            self.selectors.flowElement = self.paramContructors.containerElements[1];
        }
        else {
            self.selectors.flowElement = self.paramContructors.containerElements[0];
        }

        $container = self.selectors.flowElement.closest(".dt-container");
        $divBody = self.selectors.$flowTable.wrap("<div class='dt-body'></div>").parent();
        self.selectors.$flowScroll = self.selectors.$flowTable.wrap("<div class='dt-vscroll'></div>").parent();
        self.selectors.$divBody = $divBody;

        var scrollWidth = $divBody[0].offsetWidth - self.selectors.$flowScroll[0].offsetWidth;
        if (scrollWidth > 0) {
            BrowserSupport.scrollBar.width = scrollWidth;
        }

        $divBody.outerHeight(self.paramContructors.options.height);

        self.medianHeight = self.templateHeight = DOMUtil.single$($container, "div.item-tmpl").outerHeight();
        DOMUtil.single$($container, "div.item-tmpl").hide();

        self.selectors.$flowTable.css({ "position": "absolute", "top": 0 });

        self.addScrollEvent(function (e) {

            if (self.scrollTimer) {
                clearTimeout(self.scrollTimer);
            }

            self.scrollTimer = setTimeout(function () {
                var top = $divBody.scrollTop();

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
    };

    BodyContainer.prototype.getSelectedRow = function () {
        return this.selectedRowElement;
    };

    BodyContainer.prototype.getLastSelectedRow = function () {
        return this.lastSelectedRowElement;
    };

    BodyContainer.prototype.recalc = function (rows) {
        var self = this,
            $container = self.selectors.flowElement.closest(".dt-container");
        self.templateHeight = DOMUtil.single$($container, "div.item-tmpl").outerHeight();
        self.rowHeights = [];
        self.rowHeights.push(self.templateHeight);
        (rows || []).forEach(function (row) {
            if (row.height) {
                self.rowHeights.push(row.height);
            }
        });
        self.medianHeight = App.num.median(this.rowHeights);
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

    BodyContainer.prototype.getPhysicalHeight = function () {
        return this.selectors.$divBody[0].clientHeight;
    };

    BodyContainer.prototype.getRowIndex = function (rowid) {
        var self = this,
            elements = self.selectors.$flowTable.children(),
            len = elements.length,
            index, targetElement;

        for (index = 0; index < len; index++) {
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

        self.setContainerOffset(0);
    };

    BodyContainer.prototype.clearBodyRows = function () {
        var self = this, flowRow;

        self.rows.forEach(function (bodyRow) {
            flowRow = bodyRow.containerElement[0];
            self.selectors.$flowTable[0].removeChild(flowRow);
        });

        self.rows = []; // Array of BodyRow.
        self.renderHeight = 0;
    };

    BodyContainer.prototype.removeRow = function (index) {

        var self = this,
            targetRow = self.rows[index],
            rowRemove = self.paramContructors.accessor.getBodyView().cache.id(targetRow.rowid);

        targetRow.detachCellFocusEvent();
        targetRow.detachCellClickEvent();
        targetRow.detachChangeEvent();
        targetRow.detachCellBlurEvent();

        if (!App.isUndefOrNull(targetRow.containerElement[0].parentNode)) {
            targetRow.containerElement[0].parentNode.removeChild(targetRow.containerElement[0]);
        }

        self.rows.splice(index, 1);
        rowRemove.visible = false;
        if (self.selectedRowElement === rowRemove) {
            self.selectedRowElement = undefined;
        }

        if (self.lastSelectedRowElement === rowRemove) {
            self.lastSelectedRowElement = undefined;
        }

        return rowRemove;
    };

    BodyContainer.prototype.resizeFitToBottom = function (self, $container, $divBody, $divFoot, $fixBody, $fixHead, $fixFoot, complete) {

        if (self.resizeTimer) {
            clearTimeout(self.resizeTimer);
        }

        self.resizeTimer = setTimeout(function () {
            var offsetTop = $container.offset().top,
                currentHeight = $(window).height(),
                bodyHeight = Math.floor(currentHeight - offsetTop - self.paramContructors.options.resizeOffset);

            $divBody.height(bodyHeight);

            if (bodyHeight > self.viewHeight) {
                $divBody.closest(".part").css("margin-bottom", "0px");
            } else {
                $divBody.closest(".part").css("margin-bottom", "32px");
            }

            if (App.isFunc(complete)) {
                complete();
            }

        }, 5);
    };

    BodyContainer.prototype.resize = function (complete) {
        var self = this,
            $container = self.selectors.flowElement.closest(".dt-container"),
            $divBody = DOMUtil.single$(self.selectors.flowElement, ".dt-body");

        self.resizeFitToBottom(self, $container, $divBody, undefined, undefined, undefined, undefined, complete);
    };

    BodyContainer.prototype.getScrollTop = function () {
        return this.selectors.$flowScroll[0].parentNode.scrollTop;
    };

    BodyContainer.prototype.setContainerOffset = function (containerOffset) {
        var self = this;
        self.selectors.$flowTable.css("top", containerOffset);
    };

    BodyContainer.prototype.addContainerOffset = function (containerOffset) {
        var self = this;
        var newTop = Math.max(parseInt(self.selectors.$flowTable.css("top"), 10) + containerOffset, 0);
        self.setContainerOffset(newTop);
    };

    BodyContainer.prototype.syncScrollPosition = function () {
        return false;
    };

    BodyContainer.prototype.getDOMRowIdAttribute = function(target){
        var rowid,
            $target = $(target);

        rowid = $target.attr("data-rowid");

        return rowid;
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

    BodyContainer.prototype.cloneTemplateItem = function () {
        var self = this, $flowElement,
            template = DOMUtil.single$(self.selectors.$divBody, "div.item-tmpl");

        if (template.length <= 0) {
            throw "テンプレートが存在しない"; //Template is not exist
        }

        $flowElement = DOMUtil.cloneNode(template);

        return [undefined, $flowElement];
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

    BodyContainer.prototype.attachEventToBodyRow = function (bodyRow) {
        var self = this;
        bodyRow.addChangeEvent(function (e) {
            var target = $(e.target),
                tbody = target.closest("div.datalist-item"),
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
            onListItemFocus(e, self);

            e.preventDefault();
            e.stopImmediatePropagation();
        });

        bodyRow.addCellClickEvent(function (e) {
            onListItemFocus(e, self);
            onListItemClick(e, self);
        });

        bodyRow.addKeyDownEvent(function (e) {
            onKeyDown(e, bodyRow);
        });
    };

    BodyContainer.prototype.insertRow = function (index, dataRow) {
        var self = this,
            flowTable = self.selectors.$flowTable[0],
            bodyRow = new BodyRow(dataRow.element, self.paramContructors),
            flowRow,
            needRecalcMedian = false;

        self.attachEventToBodyRow(bodyRow);

        bodyRow.rowid = dataRow.rowid;
        flowRow = bodyRow.containerElement[0];

        if (index >= self.rows.length) {
            flowTable.appendChild(flowRow);
            if (!dataRow.height) {
                dataRow.height = flowRow.clientHeight;
                needRecalcMedian = true;
            }
            self.rows.push(bodyRow);
        }
        else {
            flowTable.insertBefore(flowRow, self.rows[index].containerElement[0]);
            if (!dataRow.height) {
                dataRow.height = flowRow.clientHeight;
                needRecalcMedian = true;
            }
            self.rows.splice(index, 0, bodyRow);
        }
        if (needRecalcMedian) {
            this.rowHeights.push(dataRow.height);
            this.medianHeight = App.num.median(this.rowHeights);
            self.addContainerHeight(dataRow.height, dataRow.temporaryHeight);
        }

        //hidden column if exist
        if (App.isNumeric(self.indexColumnHide)) {
            bodyRow.hideColumn(self.indexColumnHide);
        } else {
            var $targetRow = DOMUtil.query(dataRow.element, "div.row"),
                i, ilen, row;
            for (i = 0, ilen = $targetRow.length; i < ilen; i++) {
                row = $targetRow[i];
                DOMUtil.query$(row, "div").css("display", "");
            }
        }
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

    BodyContainer.prototype.addScrollEvent = function (handler) {
        this.selectors.$divBody.on("scroll", handler);
    };

    BodyContainer.prototype.detachScrollEvent = function () {
        this.selectors.$divBody.off("scroll");
    };

    App.ui.datalistModule = function () {
        DOMUtil = $.fn.dataTable.DOMUtil;
        BrowserSupport = $.fn.dataTable.BrowserSupport;
        return {
            HeaderContainer: $.fn.dataTable.emptyHeaderContainer,
            BodyContainer: BodyContainer,
            FooterContainer: $.fn.dataTable.emptyFooterContainer
        };
    };


})(this, jQuery);
