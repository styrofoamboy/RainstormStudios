//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

Type.registerNamespace("RainstormStudios.Web.UI.WebControls");

RainstormStudios.Web.UI.WebControls.SelectableTableExtender = function (element) {
    RainstormStudios.Web.UI.WebControls.SelectableTableExtender.initializeBase(this, [element]);

    this._hoverStyleClass = null;
    this._selectedStyleClass = null;
    this._containerID = null;
    this._tableID = null;
    this._postbackCtrlID = null;
    this._selectRowOnClick = null;
    this._selectedRowIdx = null;
}

RainstormStudios.Web.UI.WebControls.SelectableTableExtender.prototype = {
    initialize: function () {
        RainstormStudios.Web.UI.WebControls.SelectableTableExtender.callBaseMethod(this, 'initialize');

        this._onRowClickHandler = Function.createDelegate(this, this._onRowClick);

        $addHandlers(this.get_element(),
        {
            'click': this._onRowClick
        },
        this);
    },

    dispose: function () {
        $clearHandlers(this.get_element());
        RainstormStudios.Web.UI.WebControls.SelectableTableExtender.callBaseMethod(this, 'dispose');
    },

    setTableSelectable: function (containerID, tblID, hdnSelRowIdx, hoverClass, selectClass, selectOnClick, lnkID) {
        //debugger;

        if (JQuery == null)
            return;

        var $container = $("[id$='" + containerID + "']");
        var pos = $.extend({
            width: $container.outerWidth(),
            height: $container.outerHeight()
        }, $container.position());

        var $rows = $("table[id$='" + tblID + "'] tr");
        $rows.click(function () {
            //debugger;

            var $lnk = _findPostbackCtrl(this, lnkID);

            if ($lnk) {
                // Create a "cover" to dim out the grid while we get the data.
                var $cover = $('<div>', {
                    id: 'overlay',
                    css: {
                        position: 'absolute',
                        top: pos.top,
                        left: pos.left,
                        width: pos.width,
                        height: pos.height,
                        backgroundColor: '#FFF',
                        opacity: 0.50,
                        zIndex: 100
                    }
                });
                $('<img>', {
                    src: '../Images/loader.gif',
                    alt: 'Loading details. Please wait...',
                    css: {
                        position: 'absolute',
                        top: pos.top,
                        left: (pos.width / 2) - (32 / 2),
                        top: (pos.height / 2) - (32 / 2)
                    }
                }).appendTo($cover);
                $cover.appendTo($container);

                if (selectOnClick) {
                    $("input[type='hidden'][id$='" + hdnSelRowIdx + "']").val(this.rowIndex);
                }

                eval($lnk);
            }
        });
        $rows.mouseover(function () {
            var $lnk = getLnkPostback(this, lnkID);
            if ($lnk)
                $(this).addClass(hoverClass);
        });
        $rows.mouseout(function () {
            var $lnk = getLnkPostback(this, lnkID);
            if ($lnk)
                $(this).removeClass(hoverClass);
        });
        $rows.filter(function () {
            if (getLnkPostback(this, lnkID)) {
                return true;
            } else {
                return false;
            }
        }).css("cursor", "pointer");

        //debugger;
        if (selectOnClick) {
            var selRowIdx = $("input[type='hidden'][id$='" + hdnSelRowIdx + "']").val();
            if (selRowIdx && selRowIdx > 0) {
                var actRow = $rows[selRowIdx];
                $(actRow).addClass(selectClass);
                this.selectedRowIdx = selRowIdx;
            }
        }
    },

    _findPostbackCtrl: function (ctrl, lnkID) {
        if (JQuery == null)
            return;

        var lnk = null;
        if (lnkID)
            lnk = $(ctrl).children("td").children("a[id*='" + lnkID + "']").attr('href');
        else
            lnk = $(ctrl).children("td").children("a").attr('href');

        return lnk;
    },

    _onRowClick: function (e) {
        if (this.get_element() && !this.get_element().disabled) {
            debugger;
        }
    },

    get_hoverStyleClass: function () {
        return this._hoverStyleClass;
    },

    set_hoverStyleClass: function (value) {
        if (this._hoverStyleClass !== value) {
            this._hoverStyleClass = value;
            this.raisePropertyChanged('hoverStyleClass');
        }
    },

    get_selectedStyleClass: function () {
        return this._selectedStyleClass;
    },

    set_selectedStyleClass: function (value) {
        if (this._selectedStyleClass !== value) {
            this._selectedStyleClass = value;
            this.raisePropertyChanged('selectedStyleClass');
        }
    },

    get_containerID: function () {
        return this._containerID;
    },

    set_containerID: function (value) {
        if (this._containerID !== value) {
            this._containerID = value;
            this.raisePropertyChanged('containerID');
        }
    },

    get_tableID: function () {
        return this._tableID;
    },

    set_tableID: function (value) {
        if (this._tableID !== value) {
            this._tableID = value;
            this.raisePropertyChanged('tableID');
        }
    },

    get_postbackCtrlID: function () {
        return this._postbackCtrlID;
    },

    set_postbackCtrlID: function (value) {
        if (this._postbackCtrlID !== value) {
            this._postbackCtrlID = value;
            this.raisePropertyChanged('postbackCtrlID');
        }
    },

    get_selectRowOnClick: function () {
        return this._selectRowOnClick;
    },

    set_selectRowOnClick: function (value) {
        if (this._selectRowOnClick !== value) {
            this._selectRowOnClick = value;
            this.raisePropertyChanged('selectRowOnClick');
        }
    },

    get_selectedRowIdx: function () {
        return this._selectedRowIdx;
    },

    set_selectedRowIdx: function (value) {
        if (this._selectedRowIdx !== value) {
            this._selectedRowIdx = value;
            this.raisePropertyChanged('selectedRowIdx');
        }
    }
}

// JSON serialization descriptor
RainstormStudios.Web.UI.WebControls.SelectableTableExtender.descriptor = {
    properties: [{ name: 'hoverStyleClass', type: String },
                 { name: 'selectedStyleClass', type: String },
                 { name: 'containerID', type: String },
                 { name: 'tableID', type: String },
                 { name: 'postbackCtrlID', type: String },
                 { name: 'selectRowOnClick', type: String },
                 { name: 'selectedRowIdx', type: Integer}]
}

// Register the class as a type that inherits from Sys.UI.Control.
RainstormStudios.Web.UI.WebControls.SelectableTableExtender.registerClass('RainstormStudios.Web.UI.WebControls.SelectableTableExtender', Sys.UI.Behavior);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
    

/*
    this._hoverStyleClass = null;
    this._selectedStyleClass = null;
    this._containerID = null;
    this._tableID = null;
    this._postbackCtrlID = null;
    this._selectRowOnClick = null;
    this._selectedRowIdx = null;

*/

function setTableSelectable(containerID, tblID, hdnSelRowIdx, hoverClass, selectClass, selectOnClick, lnkID) {

    //debugger;
    var $container = $("[id$='" + containerID + "']");
    var pos = $.extend({
        width: $container.outerWidth(),
        height: $container.outerHeight()
    }, $container.position());

    var $rows = $("table[id$='" + tblID + "'] tr");
    $rows.click(function () {
        //debugger;

        var $lnk = getLnkPostback(this, lnkID);

        if ($lnk) {
            // Create a "cover" to dim out the grid while we get the data.
            var $cover = $('<div>', {
                id: 'overlay',
                css: {
                    position: 'absolute',
                    top: pos.top,
                    left: pos.left,
                    width: pos.width,
                    height: pos.height,
                    backgroundColor: '#FFF',
                    opacity: 0.50,
                    zIndex: 100
                }
            });
            $('<img>', {
                src: '../Images/loader.gif',
                alt: 'Loading details. Please wait...',
                css: {
                    position: 'absolute',
                    top: pos.top,
                    left: (pos.width / 2) - (32 / 2),
                    top: (pos.height / 2) - (32 / 2)
                }
            }).appendTo($cover);
            $cover.appendTo($container);

            if (selectOnClick) {
                $("input[type='hidden'][id$='" + hdnSelRowIdx + "']").val(this.rowIndex);
            }

            eval($lnk);
        }
    });
    $rows.mouseover(function () {
        var $lnk = getLnkPostback(this, lnkID);
        if ($lnk)
            $(this).addClass(hoverClass);
    });
    $rows.mouseout(function () {
        var $lnk = getLnkPostback(this, lnkID);
        if ($lnk)
            $(this).removeClass(hoverClass);
    });
    $rows.filter(function () {
        if (getLnkPostback(this, lnkID)) {
            return true;
        } else {
            return false;
        }
    }).css("cursor", "pointer");

    //debugger;
    if (selectOnClick) {
        var selRowIdx = $("input[type='hidden'][id$='" + hdnSelRowIdx + "']").val();
        if (selRowIdx && selRowIdx > 0) {
            var actRow = $rows[selRowIdx];
            $(actRow).addClass(selectClass);
        } 
    }
}

function getLnkPostback(ctrl, lnkID) {
    var lnk = null;
    if (lnkID)
        lnk = $(ctrl).children("td").children("a[id*='" + lnkID + "']").attr('href');
    else
        lnk = $(ctrl).children("td").children("a").attr('href');

    return lnk;
}