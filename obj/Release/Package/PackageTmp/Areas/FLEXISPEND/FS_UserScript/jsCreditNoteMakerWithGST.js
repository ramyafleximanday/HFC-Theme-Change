﻿var objDialogy;
var objDialogyCygnet;
var countArray = "0";


UrlDet = UrlDet.replace("GetCreditNoteMaker", "");
UrlHelper = UrlHelper.replace("GetAutoCompleteCourier", "");

var CreditNoteCheckerModel = function () {
    debugger;
    var self = this;

    //load grid array
    self.CreditNoteArray = ko.observableArray();

    self.formatNumber = function (num) {
        //return Number(num).toFixed(2);
        var currentval = Number(num).toFixed(2);

        var testDecimal;
        currentval.match(/\./g) === null ? testDecimal = 0 : testDecimal = currentval.match(/\./g);
        if (testDecimal.length > 1) {
            currentval = currentval.slice(0, -1);
        }

        var components = currentval.toString().split(".");
        if (components.length === 1)
            components[0] = currentval;
        components[0] = components[0].replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        if (components.length === 2)
            components[1] = components[1].replace(/\D/g, "");
        return components.join(".");
    };
    this.RadioChecked = function (obj, event) {
        if (event.originalEvent) {
            var _inBit = event.originalEvent.target.defaultValue;

            if (_inBit == "0") {
                $("#txtDateFrom").removeClass('required').removeClass('valid');
                $("#txtDateTo").removeClass('required').removeClass('valid');

                $("#txtDateFrom").addClass('required');
                $("#txtDateTo").addClass('required');

                $("#hdfSupplier").val("0");
                $("#txtDateFrom").val("");
                $("#txtDateTo").val("");
                $("#txtSupplier").val("");


            } else {
                $("#txtDateFrom").removeClass('required').removeClass('valid');
                $("#txtDateTo").removeClass('required').removeClass('valid');

                $("#txtDateFrom").addClass('required');
                $("#txtDateTo").addClass('required');

                $("#hdfSupplier").val("0");
                $("#txtDateFrom").val("");
                $("#txtDateTo").val("");
                $("#txtSupplier").val("");
            }
            checkValidationSearch();
            self.loadGrid();
        }
    }

    //selva 24-11-2020 Proceed grid display

    self.Proceedgriddisplay = function () {
        var data = {
            Rejected: $("input[name=modeStatus]:checked").val(),
            DateFrom: $("#txtDateFrom").val(),
            DateTo: $("#txtDateTo").val(),
            SupplierId: $("#hdfSupplier").val()
        };
        $.ajax({
            type: "post",
            url: UrlDet + "GetCreditNoteMaker",
            data: JSON.stringify(data),
            contentType: "application/json;",
            success: function (response) {

                $("#gvChecker").DataTable({
                    "autoWidth": false,
                    "destroy": true,
                    "bFilter": false,
                    "bLengthChange": false,
                    "bPaginate": false,
                    "bInfo": false
                }).destroy();
                self.CreditNoteArray.removeAll();

                var Data1 = "", Data2 = "";
                if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    //show Message if error
                    if (Data1[0].Msg != "") {
                        jAlert(Data1[0].Msg, "Message");
                    }
                }

                if (response.Data2 != null && response.Data2 != "" && JSON.parse(response.Data2) != null) {
                    Data2 = JSON.parse(response.Data2);
                    self.CreditNoteArray(Data2);
                }
                if ($("input[name=modeStatus]:checked").val() == "1") {
                    $('.showVisible').attr("style", "display:none;");
                } else {
                    $('.showVisible').attr("style", "display:block;");
                }

                if (self.CreditNoteArray().length == 0 && countArray == "0") {
                    jAlert("Sorry! No Records Found.", "Message");
                }
                countArray = "0";
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(errorThrown);
            }
        });
    };


    self.search = function () {
        var data = {
            Rejected: $("input[name=modeStatus]:checked").val(),
            DateFrom: $("#txtDateFrom").val(),
            DateTo: $("#txtDateTo").val(),
            SupplierId: $("#hdfSupplier").val()
        };
        $.ajax({
            type: "post",
            url: UrlDet + "GetCreditNoteMaker",
            data: JSON.stringify(data),
            contentType: "application/json;",
            success: function (response) {

                $("#gvChecker").DataTable({
                    "autoWidth": false,
                    "destroy": true,
                    "bFilter": false,
                    "bLengthChange": false,
                    "bPaginate": false,
                    "bInfo": false
                }).destroy();
                self.CreditNoteArray.removeAll();

                var Data1 = "", Data2 = "";
                if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    //show Message if error
                    if (Data1[0].Msg != "") {
                        jAlert(Data1[0].Msg, "Message");
                    }
                }

                if (response.Data2 != null && response.Data2 != "" && JSON.parse(response.Data2) != null) {
                    Data2 = JSON.parse(response.Data2);
                    self.CreditNoteArray(Data2);
                }
                if ($("input[name=modeStatus]:checked").val() == "1") {
                    $('.showVisible').attr("style", "display:none;");
                } else {
                    $('.showVisible').attr("style", "display:block;");
                }

                if (self.CreditNoteArray().length == 0 && countArray == "0") {
                    jAlert("Sorry! No Records Found.", "Message");
                }
                countArray = "0";
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(errorThrown);
            }
        });
    };

    self.GetCreditNoteInfo = function () {
        $.ajax({
            type: "post",
            url: UrlDet + "LoadCreditNoteInfo",
            data: '{}',
            async: false,
            contentType: "application/json;",
            success: function (response) {
                var Data1 = "";
                if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    $("#txtBookDate").val(Data1[0].BookingDate);
                    // $("#txtCreditNoteNo").val(Data1[0].Credinoteno);
                }

                if ($("#txtBookDate").val().trim() != "") {
                    $("#txtBookDate").removeClass('required').removeClass('valid');
                    $("#txtBookDate").addClass('valid');
                }

                if ($("#txtCreditNoteNo").val().trim() != "") {
                    $("#txtCreditNoteNo").removeClass('required').removeClass('valid');
                    $("#txtCreditNoteNo").addClass('valid');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#txtBookDate").val("");
                $("#txtCreditNoteNo").val("");
            }
        });
    };

    self.DatatableCall = function () {
        if ($("#gvChecker > tbody > tr").length == self.CreditNoteArray().length) {
            $("#gvChecker").DataTable({
                "autoWidth": false,
                "destroy": true,
                "aoColumnDefs": [{
                    "aTargets": ["nosort"],
                    "bSortable": false
                },
                {
                    "aTargets": ["colDate"],
                    "bSortable": true,
                    "sType": "date-uk"
                }]
            });
        }
    };

    self.showAdd = function () {
        debugger;
        $("#txtBookDate").attr("disabled", "disabled");
        $("#txtCreditNoteNo").attr("disabled", "disabled");
        //$("#txtBookDate").removeAttr("disabled");
        $("#txtESupplier").attr("disabled", "disabled");
        //$("#txtCreditNoteNo").removeAttr("disabled");
        //$("#txtCreditNoteDate").removeAttr("disabled");
        $("#txtECFNo").removeAttr("disabled");
        $("#txtInvoiceNo").removeAttr("disabled");
        $("#btnSubmit").attr("disabled", true);

        ClearCreditNoteMakerEntry();

        self.GetCreditNoteInfo();

        ShowDialog();
    };



    self.Update = function () {
        debugger;
        var data = {
            supplierId: $("#hdfESupplier").val(),
            ecfid: $("#txtECFNo").val(),
            invid: $("#txtInvoiceNo").val(),
            creditnoteno: $("#txtCreditNoteNo").val(),
            creditnoteamt: $("#txtCreditNoteAmount").val().replace(/,/g, ''),
            remarks: $("#txtMakerRemark").val(),
            //Credit Note Change
            HsnId: $("#HsnId").val(),
            providerlocationid: $("#ProviderLocation_gid").val(),
            receiverlocationid: $("#ReceiverLocationid").val(),
            isremoved: "0"
        };
        $.ajax({
            type: "post",
            url: UrlDet + "SetCreditNoteMakerGST",
            data: JSON.stringify(data),
            async: false,
            contentType: "application/json;",
            success: function (response) {
                debugger;
                var Data1 = "";
                if (response.Data1 == "Unauthorized User!") {
                    jAlert("Unauthorized User!");
                }
                else if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    if (Data1[0].Clear == "true" || Data1[0].Clear == "True" || Data1[0].Clear == "1") {
                        //  objDialogy.dialog("close");
                        debugger;
                        // alert('url');
                        // var payurl = '@(Url.Action("GSTCreditNoteMaker","_LoadGstDetails"))'
                        // alert(payurl);

                        jConfirm(Data1[0].Msg, "Message", function (callback) {
                            if (callback == true) {
                                debugger;
                                countArray = "1";
                                self.search();
                                $("#hdfCNMId").val(Data1[0].Creditnotegid);
                                var Creditnotegid = $("#hdfCNMId").val();
                                $("#btnSubmits").attr("disabled", false); // Credit note Submit button enable
                                $("#txtMakerRemark").attr("disabled", false);// Credit note remakrs enable

                                var url = UrlDet + "_LoadGstDetails";
                                //'@Url.Action("_EmpPaymentDetails", "TravelClaimNew")?' + Creditnotegid;
                                $("#divpopulateGrid").load(url);
                                $("#divpopulateGrid").show();


                            } else {
                                countArray = "1";
                                self.search();
                            }
                        });


                        //remove the cancel button from dialog box
                        $("#popup_ok").attr("style", "margin-left: 50px;");
                        $("#popup_cancel").attr("style", "visibility: hidden");
                    } else {
                        //show Message if error
                        if (Data1[0].Msg != "") {
                            jAlert(Data1[0].Msg, "Message");
                        }
                    }
                } else {
                    jAlert("unable to process your request. please try again!", "Message");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(errorThrown);
            }
        });
    };



    self.remaksupdate = function () {
        debugger;
        var data = {
            Creditnotegid: $("#hdfCNMId").val(),
            remarks: $("#txtMakerRemark").val(),
            Action: "submit",
            isremoved: "0"
        };
        $.ajax({
            type: "post",
            url: UrlDet + "SetCreditNoteRemarksUpdate",
            data: JSON.stringify(data),
            async: false,
            contentType: "application/json;",
            success: function (response) {
                debugger;
                var Data1 = "";
                if (response.Data1 == "Unauthorized User!") {
                    jAlert("Unauthorized User!");
                }
                else if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    if (Data1[0].Clear == "true" || Data1[0].Clear == "True" || Data1[0].Clear == "1") {

                        jConfirm(Data1[0].Msg, "Message", function (callback) {
                            if (callback == true) {
                                debugger;
                                countArray = "1";
                                self.search();
                                $("#hdfCNMId").val(Data1[0].Creditnotegid);
                                var Creditnotegid = $("#hdfCNMId").val();
                                $("#btnSubmits").attr("disabled", false); // Credit note Submit button enable
                                $("#txtMakerRemark").attr("disabled", false);// Credit note remakrs enable                                
                                $("#divpopulateGrid").show();
                            } else {
                                countArray = "1";
                                self.search();
                            }
                        });


                        //remove the cancel button from dialog box
                        $("#popup_ok").attr("style", "margin-left: 50px;");
                        $("#popup_cancel").attr("style", "visibility: hidden");
                    } else {
                        //show Message if error
                        if (Data1[0].Msg != "") {
                            jAlert(Data1[0].Msg, "Message");
                        }
                    }
                } else {
                    jAlert("unable to process your request. please try again!", "Message");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

            }
        });
    };

    self.Edit = function (item) {

        ClearCreditNoteMakerEntry();

        $("#hdfCNMId").val(item.gid);
        $("#hdfESupplier").val(item.supplierid);

        $("#txtBookDate").val(item.BookingDate);
        $("#txtESupplier").val(item.suppliercode + '-' + item.suppliername);
        $("#txtCreditNoteNo").val(item.creditnoteno);

      //  $("#txtCreditNoteAmount").val(self.formatNumber(item.creditnoteamt));
        $("#txtCreditNoteAmount").val(item.creditnoteamt);

        $("#txtCreditNoteDate").val(item.creditnotedate);
        $("#txtECFNo").val(item.ECFNo);
        $("#txtInvoiceNo").val(item.invno);
        $("#txtInvoiceAmount").val(self.formatNumber(item.INVAMT));
        $("#txtMakerRemark").val(item.Remarks);

        $("#txtBookDate").attr("disabled", "disabled");
        $("#txtESupplier").attr("disabled", "disabled");
        $("#txtCreditNoteNo").attr("disabled", "disabled");
        //$("#txtCreditNoteDate").attr("disabled", "disabled");
        $("#txtECFNo").attr("disabled", "disabled");
        $("#txtInvoiceNo").attr("disabled", "disabled");


        $("#txtMakerRemark").attr("disabled", "disabled");
        $('#btnUpdate').attr('disabled', false);
        //  $("#btnSubmits").attr('disabled', false);

        if ($("#txtBookDate").val().trim() != "") {
            $("#txtBookDate").removeClass('required').removeClass('valid');
        }
        if ($("#txtESupplier").val().trim() != "") {
            $("#txtESupplier").removeClass('required').removeClass('valid');
        }
        if ($("#txtCreditNoteNo").val().trim() != "") {
            $("#txtCreditNoteNo").removeClass('required').removeClass('valid');
        }
        if ($("#txtCreditNoteAmount").val().trim() != "") {
            $("#txtCreditNoteAmount").removeClass('required').removeClass('valid');
        }
        if ($("#txtCreditNoteDate").val().trim() != "") {
            $("#txtCreditNoteDate").removeClass('required').removeClass('valid');
        }
        if ($("#txtECFNo").val().trim() != "") {
            $("#txtECFNo").removeClass('required').removeClass('valid');
        }
        if ($("#txtInvoiceNo").val().trim() != "") {
            $("#txtInvoiceNo").removeClass('required').removeClass('valid');
        }

        checkCreditNoteEntry();
        ShowDialog();
    };

    self.Delete = function (item) {
        debugger;
        alert('a');
        jConfirm("Do you want to delete the record?", "Message", function (callback) {
            debugger;
            if (callback == true) {
                debugger;
                var data = {
                    Creditnotegid: item.gid,
                    supplierId: item.supplierid,
                    ecfid: item.ECFNo,
                    invid: item.invno,
                    creditnoteno: item.creditnoteno,
                    creditnoteamt: item.creditnoteamt,
                    remarks: item.Remarks,
                    isremoved: "1"
                };
                $.ajax({
                    type: "post",
                    url: UrlDet + "SetCreditNoteMakerGST",
                    data: JSON.stringify(data),
                    contentType: "application/json;",
                    success: function (response) {
                        var Data1 = "";
                        if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                            Data1 = JSON.parse(response.Data1);

                            if (Data1[0].Clear == "true" || Data1[0].Clear == "True" || Data1[0].Clear == "1") {

                                objDialogy.dialog("close");

                                jConfirm(Data1[0].Msg, "Message", function (callback) {
                                    if (callback == true) {
                                        countArray = "1";
                                        self.search();
                                    } else {
                                        countArray = "1";
                                        self.search();
                                    }
                                });
                                //remove the cancel button from dialog box
                                $("#popup_ok").attr("style", "margin-left: 50px;");
                                $("#popup_cancel").attr("style", "visibility: hidden");
                            } else {
                                //show Message if error
                                if (Data1[0].Msg != "") {
                                    jAlert(Data1[0].Msg, "Message");
                                }
                            }
                        } else {
                            jAlert("unable to process your request. please try again!", "Message");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert(errorThrown);
                    }
                });
            } else {
                return false;
            }
        });
    };

    self.clearFilter = function () {
        $("#txtDateFrom").val("");
        $("#txtDateTo").val("");

        $("#hdfSupplier").val("0");
        $("#txtSupplier").val("");

        $("#txtDateFrom").removeClass('required').removeClass('valid');
        $("#txtDateTo").removeClass('required').removeClass('valid');

        $("#txtDateFrom").addClass('required');
        $("#txtDateTo").addClass('required');

        checkValidationSearch();
        ClearCreditNoteMakerEntry();

        $("#gvChecker").DataTable({
            "autoWidth": false,
            "destroy": true,
            "bFilter": false,
            "bLengthChange": false,
            "bPaginate": false,
            "bInfo": false
        }).destroy();
        self.CreditNoteArray.removeAll();
    };

    self.loadGrid = function () {
        $("#gvChecker").DataTable({
            "autoWidth": false,
            "destroy": true,
            "bFilter": false,
            "bLengthChange": false,
            "bPaginate": false,
            "bInfo": false
        }).destroy();
        self.CreditNoteArray.removeAll();
    }

    self.loadGrid();
};

var mainViewModel = new CreditNoteCheckerModel();
ko.applyBindings(mainViewModel);

//document ready function start

$(document).ready(function () {



    jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "date-uk-pre": function (a) {
            a = a.split(">")[1].split("<")[0];
            var ukDatea = a.split("/");
            return (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
        },
        "date-uk-asc": function (a, b) {
            return ((a < b) ? -1 : ((a > b) ? 1 : 0));
        },
        "date-uk-desc": function (a, b) {
            return ((a < b) ? 1 : ((a > b) ? -1 : 0));
        }
    });

    objDialogy = $("[id$='ShowDialog']");
    objDialogy.dialog({
        autoOpen: false,
        modal: true,
        width: 800,
        height: 350,
        duration: 300
    });

    //selva 19-11-2020
    objDialogyCygnet = $("[id$='divCygnet']");
    objDialogyCygnet.dialog({
        autoOpen: false,
        modal: true,
        width: 1100,
        height: 600,
        duration: 300,
        close: function (event, ui) {
            objDialogyCygnet.dialog('destroy');


        }
    });

    var objDate = new Date();
    var Presentyear = objDate.getFullYear();
    $(".fsdatepicker").datepicker({
        yearRange: '1900:' + Presentyear,
        changeMonth: true,
        changeYear: true,
        maxDate: 'd',
        dateFormat: 'dd/mm/yy'
    });

    checkValidationSearch();

    //$("input:radio[name=modeStatus]").change(function () {
    //    var chkBit = $("input[name=modeStatus]:checked").val();
    //});


    $("#txtDateFrom").change(function () {
        checkValidationSearch();

        var txtDate = $("#txtDateFrom").val();
        if (txtDate.trim() != "") {
            $("#txtDateFrom").removeClass('required');
            $("#txtDateFrom").addClass('valid');
        }
        else {
            $("#txtDateFrom").removeClass('valid');
            $("#txtDateFrom").addClass('required');
        }
    });

    $("#txtDateTo").change(function () {
        checkValidationSearch();

        var txtDate = $("#txtDateTo").val();
        if (txtDate.trim() != "") {
            $("#txtDateTo").removeClass('required');
            $("#txtDateTo").addClass('valid');
        }
        else {
            $("#txtDateTo").removeClass('valid');
            $("#txtDateTo").addClass('required');
        }
    });

    //search auto complete
    $("#txtSupplier").keyup(function (e) {
        debugger;
        if (e.which != 13) {
            $("#hdfSupplier").val("0");
        }

        $("#txtSupplier").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: UrlHelper + "GetAutoCompleteSupplierAll",
                    data: "{ 'txt' : '" + $("#txtSupplier").val() + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.split('~')[1],
                                val: item.split('~')[0]
                            }
                        }))
                    },
                    error: function (response) {
                        //alert(response.responseText);
                    },
                    failure: function (response) {
                        //alert(response.responseText);
                    }
                });
            },
            minLength: 1,
            select: function (e, i) {
                $("#hdfSupplier").val(i.item.val);
                $("#txtSupplier").val(i.item.label);
            }
        });
    });

    $("#txtSupplier").focusout(function () {
        var hdfId = $("#hdfSupplier").val();
        var _data = $("#txtSupplier").val();
        if (_data.trim() != "" && (hdfId.trim() == "" || hdfId.trim() == "0")) {
            $("#txtSupplier").val("");
        }
    });

    //Load Supplier Name Auto Complete
    $("#txtESupplier").keyup(function (e) {
        if (e.which != 13) {
            $("#hdfESupplier").val("0");
        }

        $("#txtESupplier").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: UrlHelper + "GetAutoCompleteSupplierAll",
                    data: "{ 'txt' : '" + $("#txtESupplier").val() + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.split('~')[1],
                                val: item.split('~')[0]
                            }
                        }))
                    },
                    error: function (response) {
                        //alert(response.responseText);
                    },
                    failure: function (response) {
                        //alert(response.responseText);
                    }
                });
            },
            minLength: 1,
            select: function (e, i) {
                $("#hdfESupplier").val(i.item.val);
                $("#txtESupplier").val(i.item.label);
            }
        });
    });

    $("#txtESupplier").focusout(function () {
        var hdfId = $("#hdfESupplier").val();
        var _data = $("#txtESupplier").val();

        if ((_data.trim() != "" || _data.trim() == "") && (hdfId.trim() == "" || hdfId.trim() == "0")) {
            $("#txtESupplier").val("");
            $("#txtESupplier").removeClass('valid');
            $("#txtESupplier").addClass('required');
        } else {
            $("#txtESupplier").removeClass('required');
            $("#txtESupplier").addClass('valid');
        }
    });

    //validate change event
    $("#txtBookDate").change(function () {

        checkCreditNoteEntry();

        var txtDate = $("#txtBookDate").val();
        if (txtDate.trim() != "") {
            $("#txtBookDate").removeClass('required');
            $("#txtBookDate").addClass('valid');
        }
        else {
            $("#txtBookDate").removeClass('valid');
            $("#txtBookDate").addClass('required');
        }
    });

    $("#txtCreditNoteNo").change(function () {
        checkCreditNoteEntry();

        var _data = $("#txtCreditNoteNo").val();
        if (_data.trim() != "") {
            $("#txtCreditNoteNo").removeClass('required');
            $("#txtCreditNoteNo").addClass('valid');
        }
        else {
            $("#txtCreditNoteNo").removeClass('valid');
            $("#txtCreditNoteNo").addClass('required');
        }
    });

    $("#txtCreditNoteAmount").keyup(function (event) {
        debugger;
        if (event.which >= 37 && event.which <= 40) {
            event.preventDefault();
        }

        var currentval = $(this).val();
        var testDecimal;
        currentval.match(/\./g) === null ? testDecimal = 0 : testDecimal = currentval.match(/\./g);
        if (testDecimal.length > 1) {
            currentval = currentval.slice(0, -1);
        }

        var components = currentval.toString().split(".");
        if (components.length === 1)
            components[0] = currentval;
        components[0] = components[0].replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        if (components.length === 2)
            components[1] = components[1].replace(/\D/g, "");
        $(this).val(components.join("."));
    });

    $("#txtCreditNoteAmount").focusout(function () {
        checkCreditNoteEntry();

        var _data = $("#txtCreditNoteAmount").val();
        if (_data.trim() != "" && _data.trim() != "0") {
            $("#txtCreditNoteAmount").removeClass('required');
            $("#txtCreditNoteAmount").addClass('valid');
        }
        else {
            $("#txtCreditNoteAmount").removeClass('valid');
            $("#txtCreditNoteAmount").addClass('required');
        }
    });

    $("#txtCreditNoteAmount").focusin(function () {
        checkCreditNoteEntry();

        var _data = $("#txtCreditNoteAmount").val();
        if (_data.trim() != "" && _data.trim() != "0") {
            $("#txtCreditNoteAmount").removeClass('required');
            $("#txtCreditNoteAmount").addClass('valid');
        }
        else {
            $("#txtCreditNoteAmount").removeClass('valid');
            $("#txtCreditNoteAmount").addClass('required');
        }
    });

    $("#txtCreditNoteDate").change(function () {
        checkCreditNoteEntry();

        var _data = $("#txtCreditNoteDate").val();
        if (_data.trim() != "") {
            $("#txtCreditNoteDate").removeClass('required');
            $("#txtCreditNoteDate").addClass('valid');
        }
        else {
            $("#txtCreditNoteDate").removeClass('valid');
            $("#txtCreditNoteDate").addClass('required');
        }
    });

    $("#txtECFNo").change(function () {
        debugger;

        // alert('kumar');

        checkCreditNoteEntry();

        var _data = $("#txtECFNo").val();
        if (_data.trim() != "") {
            $("#txtECFNo").removeClass('required');
            $("#txtECFNo").addClass('valid');
        }
        else {
            $("#txtECFNo").removeClass('valid');
            $("#txtECFNo").addClass('required');
        }
    });

    $("#txtInvoiceNo").focusin(function () {
        // checkCreditNoteEntry();

        var _data = $("#txtECFNo").val();
        if (_data.trim() == "") {
            jAlert("Ensure ECF Number!", "Message");
            $("#txtECFNo").focus();
            return false;
        }
    });

    $("#txtInvoiceNo").focusout(function () {
        // checkCreditNoteEntry();
        debugger;

        var _data = $("#txtInvoiceNo").val();
        if (_data.trim() != "") {
            $("#txtInvoiceNo").removeClass('required').removeClass('valid');
            $("#txtInvoiceNo").addClass('valid');

            var data = {
                ECFNo: $("#txtECFNo").val(),
                InvNo: $("#txtInvoiceNo").val()
            };

            $.ajax({
                type: "post",
                url: UrlDet + "LoadInvoiceDetails",
                data: JSON.stringify(data),
                async: false,
                contentType: "application/json;",
                success: function (response) {
                    debugger;
                    var Data1 = "";
                    if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                        Data1 = JSON.parse(response.Data1);

                        $("#txtCreditNoteDate").val(Data1[0].InvDate);
                        $("#txtInvoiceAmount").val(AddCommatoAmounts(Data1[0].invamt));
                    } else {
                        $("#txtCreditNoteDate").val("");
                        $("#txtInvoiceAmount").val("");

                        $("#txtCreditNoteDate").removeClass('required').removeClass('valid');
                        $("#txtCreditNoteDate").addClass('required');
                    }

                    if ($("#txtCreditNoteDate").val().trim() != "") {
                        $("#txtCreditNoteDate").removeClass('required').removeClass('valid');
                        $("#txtCreditNoteDate").addClass('valid');
                    }

                    if ($("#txtInvoiceAmount").val().trim() != "") {
                        $("#txtInvoiceAmount").removeClass('required').removeClass('valid');
                        $("#txtInvoiceAmount").addClass('valid');
                    }




                    $('#btnUpdate').attr('disabled', false);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $("#txtCreditNoteDate").val("");
                    $("#txtInvoiceAmount").val("");

                    $("#txtCreditNoteDate").removeClass('required').removeClass('valid');
                    $("#txtCreditNoteDate").addClass('required');
                }
            });
            checkCreditNoteEntry();
        }
        else {
            $("#txtInvoiceNo").removeClass('required').removeClass('valid');
            $("#txtInvoiceNo").addClass('required');

            $("#txtCreditNoteDate").removeClass('required').removeClass('valid');
            $("#txtCreditNoteDate").addClass('required');

            $("#txtCreditNoteDate").val("");
            $("#txtInvoiceAmount").val("");
        }
        checkCreditNoteEntry();
    });
});

//document ready function end

function ClearCreditNoteMakerEntry() {
    $("#txtBookDate").removeClass('required').removeClass('valid');
    $("#txtESupplier").removeClass('required').removeClass('valid');
    $("#txtCreditNoteNo").removeClass('required').removeClass('valid');
    $("#txtCreditNoteAmount").removeClass('required').removeClass('valid');
    $("#txtCreditNoteDate").removeClass('required').removeClass('valid');
    $("#txtECFNo").removeClass('required').removeClass('valid');
    $("#txtInvoiceNo").removeClass('required').removeClass('valid');

    $("#txtBookDate").addClass('required');
    $("#txtESupplier").addClass('required');
    $("#txtCreditNoteNo").addClass('required');
    $("#txtCreditNoteAmount").addClass('required');
    $("#txtCreditNoteDate").addClass('required');
    $("#txtECFNo").addClass('required');
    $("#txtInvoiceNo").addClass('required');

    $("#hdfCNMId").val("0");
    $("#hdfESupplier").val("0");

    $("#txtBookDate").val("");
    $("#txtESupplier").val("");
    $("#txtCreditNoteNo").val("");
    $("#txtCreditNoteAmount").val("");
    $("#txtCreditNoteDate").val("");
    $("#txtECFNo").val("");
    $("#txtInvoiceNo").val("");
    $("#txtInvoiceAmount").val("");
    $("#txtMakerRemark").val("");

    checkCreditNoteEntry();
}

function AddCommatoAmounts(val) {
    var currentval = Number(val).toFixed(2);
    var components = currentval.toString().split(".");
    if (components.length === 1)
        components[0] = currentval;
    components[0] = components[0].replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    if (components.length === 2)
        components[1] = components[1].replace(/\D/g, "");
    return components.join(".");
}

function checkValidationSearch() {
    var _dtFrm = $("#txtDateFrom").val();
    var _dtTo = $("#txtDateTo").val();

    if (_dtFrm.trim() == "" || _dtTo.trim() == "") {
        $('#btnsearch').attr('disabled', true);
    }
    else {
        $('#btnsearch').attr('disabled', false);
    }
}

function checkCreditNoteEntry() {
    var _bookdate = $("#txtBookDate").val();
    var _supplier = $("#hdfESupplier").val();
    var _creditNo = $("#txtCreditNoteNo").val();
    var _creditAmt = $("#txtCreditNoteAmount").val();
    var _creditDate = $("#txtCreditNoteDate").val();
    var _ecfNo = $("#txtECFNo").val();
    var _invoiceNo = $("#txtInvoiceNo").val();

    if (_bookdate.trim() == "" || _supplier.trim() == "" || _supplier.trim() == "0" || _creditNo.trim() == "" || _creditAmt.trim() == "" || _creditAmt.trim() == "0"
        || _creditDate.trim() == "" || _ecfNo.trim() == "" || _invoiceNo.trim() == "") {
        $('#btnUpdate').attr('disabled', true);
    }
    else {
        // $('#btnUpdate').attr('disabled', false);
    }
}

function ShowDialog() {
    $('#ShowDialog').attr("style", "display:block;");
    //objDialogy.dialog({ title: 'Credit Note Entry', width: '800', height: '350' });
    objDialogy.dialog({ title: 'Credit Note Entry', width: '1200', height: '500' });
    objDialogy.dialog("open");
    return false;
}

//Selva 19-11-2020
function GetCygnetInvoiceDetails() {
    debugger;
   
    $('#divCygnet').attr("style", "display:block;");
    objDialogyCygnet.dialog({
        title: 'Invoice GSTN Details', width: '1100', height: '600', close: function (event, ui) {
            objDialogyCygnet.dialog('destroy');
            var Urlread = "../GSTCreditNoteMaker/CygnetGrid"
            $("#divInvoiceGrid").load(Urlread);
        }
    });
    objDialogyCygnet.dialog("open");

  
    //$("#txtCygInvoiceNo").val("");
    //var Urlread = "../GSTCreditNoteMaker/GetCygnetInvoiceDetails"
    //$("#divCygnet").load(Urlread);
    //objDialogyCygnet.dialog({ title: 'Invoice Search' });
    //objDialogyCygnet.dialog("open");


    //var Urlread = "../GSTCreditNoteMaker/CygnetSearch_Supplier";
    //objDialogyCygnet.load(Urlread);
    //objDialogyCygnet.dialog({ title: 'Invoice Search' });
    //objDialogyCygnet.dialog("open");
    //$("#divCygnet").load(Urlread);

}

//Selva 09-02-2020
function SelectInvoice(id) {
    debugger;
    var InvDetail = {
        "Cygnet_gid": id
    }
    $.ajax({
        type: "post",
        url: UrlDet + "GetCygnetInvoiceDetails",
        data: JSON.stringify(InvDetail),
        contentType: "application/json;",
        async: false,
        success: function (response) {
            debugger;
            var Data1 = "";
            if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null)
                Data1 = JSON.parse(response.Data1);
            if (Data1.length > 0) {
                debugger;
                $("#txtCygInvoiceNo").val("");
                $("#txtESupplier").val(Data1[0].Cygnet_Supplier_Name); // 09-02-2021
                $("#hdfESupplier").val(Data1[0].supplierheader_gid);
                $("#hfCygnet_gid").val(Data1[0].Cygnet_gid);

                //   $("#txtCreditNoteDate").val(Data1[0].InvDate);
                //  $("#txtInvoiceNo").val(Data1[0].InvNo);

                $("#txtCreditNoteNo").val(Data1[0].InvNo);

                $("#txtECFNo").val(Data1[0].ecf_no);
                $("#txtCreditNoteAmount").val(Data1[0].InvAmt);
                $("#txtServiceMonth").val(Data1[0].servicemonth);
                // $("#txtInvoiceAmount").val(Data1[0].InvAmt);
                $("#HsnId").val(Data1[0].HsnID);
                $("#txtWithoutTaxAmount").val(Data1[0].WOTaxAmount);
                //  $("#ProviderLocationid").val(Data1[0].Cygnet_Provider_Location_Name);
                $("#ProviderLocationid").val(Data1[0].ProviderLocation);


                $("#ReceiverLocationid").val(Data1[0].ReceiverLocation);

                $("#txtCreditNoteAmount").prop("disabled", true);  //selva -09-02-2021

                $("#btnUpdate").prop('disabled', false);

                $("#txtInvoiceNo").val("");
                $("#txtInvoiceAmount").val("");

                $("#txtGSTINFICCL").val(Data1[0].GstinFiccl);
                $("#hfCygnet_gid").val(Data1[0].Cygnet_gid);


                //selva 11-02-2021

                var vendorname = $("#txtESupplier").val();
                var credinoteno = $("#txtCreditNoteNo").val();
                var creditnoteamount = $("#txtCreditNoteAmount").val();
                var providerloc = $("#ProviderLocationid").val();
                var receiverloc = $("#ReceiverLocationid").val();

                if (vendorname != "") {
                    $("#txtESupplier").removeClass('required');
                    $("#txtESupplier").addClass('valid');
                }

                if (credinoteno != "") {
                    $("#txtCreditNoteNo").removeClass('required');
                    $("#txtCreditNoteNo").addClass('valid');
                }
                if (creditnoteamount != "") {
                    $("#txtCreditNoteAmount").removeClass('required');
                    $("#txtCreditNoteAmount").addClass('valid');
                }
                if (providerloc != "") {
                    $("#ProviderLocationid").removeClass('required');
                    $("#ProviderLocationid").addClass('valid');
                }
                if (receiverloc != "") {
                    $("#ReceiverLocationid").removeClass('required');
                    $("#ReceiverLocationid").addClass('valid');
                }



                $("#ddlgstProviderLoc,#ddlgstReceiverLoc").removeClass("valid").removeClass("required");

                if (parseInt($("#ddlgstProviderLoc").val()) == 0 || $("#ddlgstProviderLoc").val() == "") {
                    $("#ddlgstProviderLoc").addClass("required");
                } else {
                    $("#ddlgstProviderLoc").addClass("valid");
                }
                if (parseInt($("#ddlgstReceiverLoc").val()) == 0 || $("#ddlgstReceiverLoc").val() == "") {
                    $("#ddlgstReceiverLoc").addClass("required");
                } else {
                    $("#ddlgstReceiverLoc").addClass("valid");
                }
                $("#tabGST").show();
                $("#tabRCM").hide();
                $('#Invoicetabs').tabs('select', 1);
                objDialogyCygnet.dialog("close");
                // objdialogCygnet.dialog("close");


                $("#txtESupplier").prop('disabled', true);
                $("#txtCreditNoteNo").prop('disabled', true);

                //$("#txtInvoiceNo").prop('disabled', true);


                $("#txtGSTINpan").prop('disabled', true);
                $("#txtGSTINvertical").prop('disabled', true);
                $("#btnCygnetSearch").attr("style", "display:none;");
                $("#txtInvoiceDate").attr("disabled", "disabled");
                $("#txtServiceMonth").attr("disabled", "disabled");
                $("#txtInvoiceAmount").attr("disabled", "disabled");
                $("#txtWithoutTaxAmount").attr("disabled", "disabled");

                $("input[name=rdoProvisionFlag]").attr("disabled", "disabled");
                $("input[name=rdoRetentionFlag]").attr("disabled", "disabled");
                $("input[name=rdoGAmort]").attr("disabled", "disabled");
                $("input[name=rdoVerify]").attr("disabled", "disabled");

                $("#tdGSTCharged").attr("style", "display:none;");
                $("#ddlgstProviderLoc").attr("disabled", "disabled");
                $("#ddlgstReceiverLoc").attr("disabled", "disabled");
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function isNumberAndLetterAndSpace(evt) {
    var key = evt.keyCode;
    return ((key >= 65 && key <= 90) || key == 8 || (key >= 97 && key <= 122) || (key >= 48 && key <= 57) || key == 127 || key == 32);
}

function isEvent(evt) {
    return false;
}