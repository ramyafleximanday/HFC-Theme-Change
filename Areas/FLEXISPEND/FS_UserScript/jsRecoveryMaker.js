var objDialogy;
var countArray = "0";

UrlDet = UrlDet.replace("GetRecoveryMaker", "");
UrlHelper = UrlHelper.replace("GetAutoCompleteCourier", "");

var RecoveryCheckerModel = function () {
    var self = this;

    //load grid array
    self.RecoveryArray = ko.observableArray();
    self.CreditGLArray = ko.observableArray();

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
    self.txtInvoiceNoFocusout = function () {
        // checkRecoveryEntry();

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
                    var Data1 = "", Data2 = "";
                    if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                        Data1 = JSON.parse(response.Data1);

                        $("#txtRecoveryDate").val(Data1[0].InvDate);
                        //$("#txtInvoiceAmount").val(AddCommatoAmounts(Data1[0].invamt));
                        $("#txtInvoiceAmount").val(0);
                    } else {
                        $("#txtRecoveryDate").val("");
                        $("#txtInvoiceAmount").val("");

                        $("#txtRecoveryDate").removeClass('required').removeClass('valid');
                        $("#txtRecoveryDate").addClass('required');
                    }

                    if ($("#txtRecoveryDate").val().trim() != "") {
                        $("#txtRecoveryDate").removeClass('required').removeClass('valid');
                        $("#txtRecoveryDate").addClass('valid');
                    }

                    if (response.Data2 != null && response.Data2 != "" && JSON.parse(response.Data2) != null)
                        Data2 = JSON.parse(response.Data2);
                    self.CreditGLArray(Data2);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $("#txtRecoveryDate").val("");
                    $("#txtInvoiceAmount").val("");

                    $("#txtRecoveryDate").removeClass('required').removeClass('valid');
                    $("#txtRecoveryDate").addClass('required');
                }
            });
            checkRecoveryEntry();
            CreditGLType_Change();
        }
        else {
            $("#txtInvoiceNo").removeClass('required').removeClass('valid');
            $("#txtInvoiceNo").addClass('required');

            $("#txtRecoveryDate").removeClass('required').removeClass('valid');
            $("#txtRecoveryDate").addClass('required');

            $("#txtRecoveryDate").val("");
            $("#txtInvoiceAmount").val("");
        }
        checkRecoveryEntry();
    };
    self.search = function () {
        var data = {
            Rejected: $("input[name=modeStatus]:checked").val(),
            DateFrom: $("#txtDateFrom").val(),
            DateTo: $("#txtDateTo").val(),
            SupplierId: $("#hdfSupplier").val(),
            Recoveryno: $("#txtRecovery").val()
        };
        $.ajax({
            type: "post",
            url: UrlDet + "GetRecoveryMaker",
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
                self.RecoveryArray.removeAll();

                var Data1 = "", Data2 = "";
                if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    //show Message if error
                    if (Data1[0].Msg != "" ) {
                        jAlert(Data1[0].Msg, "Message");
                    }
                }

                if (response.Data2 != null && response.Data2 != "" && JSON.parse(response.Data2) != null) {
                    Data2 = JSON.parse(response.Data2);
                    self.RecoveryArray(Data2);
                }
                if ($("input[name=modeStatus]:checked").val() == "0") {
                    $('.showVisible').attr("style", "display:none;");
                } else {
                    $('.showVisible').attr("style", "display:block;");
                }

                if (self.RecoveryArray().length == 0 && countArray =="0") {
                    jAlert("Sorry! No Records Found.", "Message");
                }
                countArray = "0";
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(errorThrown);
            }
        });
    };

    self.GetRecoveryInfo = function () {
        $.ajax({
            type: "post",
            url: UrlDet + "LoadRecoveryInfo",
            data: '{}',
            async: false,
            contentType: "application/json;",
            success: function (response) {
                var Data1 = "";
                if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                    Data1 = JSON.parse(response.Data1);

                    $("#txtBookDate").val(Data1[0].BookingDate);
                    $("#txtRecoveryNo").val(Data1[0].Recoveryno);
                }

                if ($("#txtBookDate").val().trim() != "") {
                    $("#txtBookDate").removeClass('required').removeClass('valid');
                    $("#txtBookDate").addClass('valid');
                }

                if ($("#txtRecoveryNo").val().trim() != "") {
                    $("#txtRecoveryNo").removeClass('required').removeClass('valid');
                    $("#txtRecoveryNo").addClass('valid');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#txtBookDate").val("");
                $("#txtRecoveryNo").val("");
            }
        });
    };

    self.DatatableCall = function () {
        if ($("#gvChecker > tbody > tr").length == self.RecoveryArray().length) {
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
        $("#txtBookDate").attr("disabled", "disabled");
        $("#txtRecoveryNo").attr("disabled", "disabled");
        //$("#txtBookDate").removeAttr("disabled");
        $("#txtESupplier").removeAttr("disabled");
        //$("#txtRecoveryNo").removeAttr("disabled");
        //$("#txtRecoveryDate").removeAttr("disabled");
        $("#txtECFNo").removeAttr("disabled");
        $("#txtInvoiceNo").removeAttr("disabled");

        ClearRecoveryMakerEntry();

        self.GetRecoveryInfo();

        ShowDialog();
    };
    self.CreditGLOnchange = function () {
        debugger;
        var creditgl = $("select option:selected").map(function () {
            return this.value;
        }).get().join(",");

        var creditamtarray = creditgl.split(',');
        debugger;
        var creditamt = 0;
        for (i = 0; i < creditamtarray.length; i++) {

            creditamt += parseFloat(creditamtarray[i].split('_')[1]);
        }
        debugger;
        $("#txtInvoiceAmount").val(AddCommatoAmounts(creditamt));
    };
    self.Update = function () {
        //var creditgl = $("select option:selected").map(function () {
        //    return this.value;
        //}).get().join(",");
        var creditgl = $("input[name=rdoGLType]:radio[checked=checked]").attr("value");
        var data = {
            Recoverygid: $("#hdfCNMId").val(),
            supplierId: $("#hdfESupplier").val(),
            ecfid: $("#txtECFNo").val(),
            invid: $("#txtInvoiceNo").val(),
            Recoveryno: $("#txtRecoveryNo").val(),
            Recoveryamt: $("#txtRecoveryAmount").val().replace(/,/g, ''),
            remarks: $("#txtMakerRemark").val(),
            //creditgl: $("#hdfCreditGL").val(),
            creditgl:creditgl,
            isremoved: "0"
        };
        $.ajax({
            type: "post",
            url: UrlDet + "SetRecoveryMaker",
            data: JSON.stringify(data),
            async: false,
            contentType: "application/json;",
            success: function (response) {
                var Data1 = "";                
                if (response.Data1 == "Unauthorized User!") {
                    jAlert("Unauthorized User!");
                }
                else if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
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
    };

    self.Edit = function (item) {
        ClearRecoveryMakerEntry();

        $("#hdfCNMId").val(item.gid);
        $("#hdfESupplier").val(item.supplierid);
        $("#hdfCreditGL").val(item.Recovery_cr_gl_gid);
        $("#txtBookDate").val(item.BookingDate);
        $("#txtESupplier").val(item.suppliercode + '-' + item.suppliername);
        $("#txtRecoveryNo").val(item.Recoveryno);
        $("#txtRecoveryAmount").val(self.formatNumber(item.Recoveryamt));
        $("#txtRecoveryDate").val(item.Recoverydate);
        $("#txtECFNo").val(item.ECFNo);
        $("#txtInvoiceNo").val(item.invno);
        $("#txtInvoiceAmount").val(self.formatNumber(item.INVAMT));
        $("#txtMakerRemark").val(item.Remarks);
        $("#txtCreditGL").val(item.gl_noname);
        $("#txtBookDate").attr("disabled", "disabled");
        $("#txtESupplier").attr("disabled", "disabled");
        $("#txtRecoveryNo").attr("disabled", "disabled");
        //$("#txtRecoveryDate").attr("disabled", "disabled");
        $("#txtECFNo").attr("disabled", "disabled");
        $("#txtInvoiceNo").attr("disabled", "disabled");

        if ($("#txtBookDate").val().trim() != "") {
            $("#txtBookDate").removeClass('required').removeClass('valid');
        }
        if ($("#txtESupplier").val().trim() != "") {
            $("#txtESupplier").removeClass('required').removeClass('valid');
        }
        if ($("#txtRecoveryNo").val().trim() != "") {
            $("#txtRecoveryNo").removeClass('required').removeClass('valid');
        }
        if ($("#txtRecoveryAmount").val().trim() != "") {
            $("#txtRecoveryAmount").removeClass('required').removeClass('valid');
        }
        if ($("#txtRecoveryDate").val().trim() != "") {
            $("#txtRecoveryDate").removeClass('required').removeClass('valid');
        }
        if ($("#txtECFNo").val().trim() != "") {
            $("#txtECFNo").removeClass('required').removeClass('valid');
        }
        if ($("#txtInvoiceNo").val().trim() != "") {
            $("#txtInvoiceNo").removeClass('required').removeClass('valid');
        }
        if ($("#txtCreditGL").val().trim() != "") {
            $("#txtCreditGL").removeClass('required').removeClass('valid');
        }
        checkRecoveryEntry();
        ShowDialog();
    };

    self.Delete = function (item) {
        jConfirm("Do you want to delete the record?", "Message", function (callback) {
            if (callback == true) {
                var data = {
                    Recoverygid: item.gid,
                    supplierId: item.supplierid,
                    ecfid: item.ECFNo,
                    invid: item.invno,
                    Recoveryno: item.Recoveryno,
                    Recoveryamt: item.Recoveryamt,
                    remarks: item.Remarks,
                    isremoved: "1"
                };
                $.ajax({
                    type: "post",
                    url: UrlDet + "SetRecoveryMaker",
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

        $("#hdfCreditGL").val("0");
        $("#txtCreditGL").val("");

        $("#txtDateFrom").removeClass('required').removeClass('valid');
        $("#txtDateTo").removeClass('required').removeClass('valid');

        $("#txtDateFrom").addClass('required');
        $("#txtDateTo").addClass('required');

        checkValidationSearch();
        ClearRecoveryMakerEntry();

        $("#gvChecker").DataTable({
            "autoWidth": false,
            "destroy": true,
            "bFilter": false,
            "bLengthChange": false,
            "bPaginate": false,
            "bInfo": false
        }).destroy();
        self.RecoveryArray.removeAll();
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
        self.RecoveryArray.removeAll();
    }

    self.loadGrid();
};

var mainViewModel = new RecoveryCheckerModel();
ko.applyBindings(mainViewModel);


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
    
    $("input[name=rdoGLType]:radio").change(function () {
        CreditGLType_Change();
    });

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
        checkRecoveryEntry();

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

    $("#txtRecoveryNo").change(function () {
        checkRecoveryEntry();

        var _data = $("#txtRecoveryNo").val();
        if (_data.trim() != "") {
            $("#txtRecoveryNo").removeClass('required');
            $("#txtRecoveryNo").addClass('valid');
        }
        else {
            $("#txtRecoveryNo").removeClass('valid');
            $("#txtRecoveryNo").addClass('required');
        }
    });

    $("#txtRecoveryAmount").keyup(function (event) {
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

    $("#txtRecoveryAmount").focusout(function () {
        checkRecoveryEntry();

        var _data = $("#txtRecoveryAmount").val();
        if (_data.trim() != "" && _data.trim() != "0") {
            $("#txtRecoveryAmount").removeClass('required');
            $("#txtRecoveryAmount").addClass('valid');
        }
        else {
            $("#txtRecoveryAmount").removeClass('valid');
            $("#txtRecoveryAmount").addClass('required');
        }
    });

    $("#txtRecoveryAmount").focusin(function () {
        checkRecoveryEntry();

        var _data = $("#txtRecoveryAmount").val();
        if (_data.trim() != "" && _data.trim() != "0") {
            $("#txtRecoveryAmount").removeClass('required');
            $("#txtRecoveryAmount").addClass('valid');
        }
        else {
            $("#txtRecoveryAmount").removeClass('valid');
            $("#txtRecoveryAmount").addClass('required');
        }
    });

    $("#txtRecoveryDate").change(function () {
        checkRecoveryEntry();

        var _data = $("#txtRecoveryDate").val();
        if (_data.trim() != "") {
            $("#txtRecoveryDate").removeClass('required');
            $("#txtRecoveryDate").addClass('valid');
        }
        else {
            $("#txtRecoveryDate").removeClass('valid');
            $("#txtRecoveryDate").addClass('required');
        }
    });

    $("#txtECFNo").change(function () {
        checkRecoveryEntry();

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
        checkRecoveryEntry();

        var _data = $("#txtECFNo").val();
        if (_data.trim() == "") {
            jAlert("Ensure ECF Number!", "Message");
            $("#txtECFNo").focus();
            return false;
        }
    });

    
});

function ClearRecoveryMakerEntry() {
    $("#txtBookDate").removeClass('required').removeClass('valid');
    $("#txtESupplier").removeClass('required').removeClass('valid');
    $("#txtRecoveryNo").removeClass('required').removeClass('valid');
    $("#txtRecoveryAmount").removeClass('required').removeClass('valid');
    $("#txtRecoveryDate").removeClass('required').removeClass('valid');
    $("#txtECFNo").removeClass('required').removeClass('valid');
    $("#txtInvoiceNo").removeClass('required').removeClass('valid');

    $("#txtBookDate").addClass('required');
    $("#txtESupplier").addClass('required');
    $("#txtRecoveryNo").addClass('required');
    $("#txtRecoveryAmount").addClass('required');
    $("#txtRecoveryDate").addClass('required');
    $("#txtECFNo").addClass('required');
    $("#txtInvoiceNo").addClass('required');

    $("#hdfCNMId").val("0");
    $("#hdfESupplier").val("0");

    $("#txtBookDate").val("");
    $("#txtESupplier").val("");
    $("#txtRecoveryNo").val("");
    $("#txtRecoveryAmount").val("");
    $("#txtRecoveryDate").val("");
    $("#txtECFNo").val("");
    $("#txtInvoiceNo").val("");
    $("#txtInvoiceAmount").val("");
    $("#txtMakerRemark").val("");
    $("#hdfCreditGL").val("0");
    $("#txtCreditGL").val("");
    checkRecoveryEntry();
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

//GSTPhase3_1
function checkValidationSearch() {
    //var _dtFrm = $("#txtDateFrom").val();
    //var _dtTo = $("#txtDateTo").val();

    //if (_dtFrm.trim() == "" || _dtTo.trim() == "") {
    //    $('#btnsearch').attr('disabled', true);
    //}
    //else {
    //    $('#btnsearch').attr('disabled', false);
    //}
}

function checkRecoveryEntry() {
    var _bookdate = $("#txtBookDate").val();
    var _supplier = $("#hdfESupplier").val();
    var _creditNo = $("#txtRecoveryNo").val();
    var _creditAmt = $("#txtRecoveryAmount").val();
    var _creditDate = $("#txtRecoveryDate").val();
    var _ecfNo = $("#txtECFNo").val();
    var _invoiceNo = $("#txtInvoiceNo").val();

    if (_bookdate.trim() == "" || _supplier.trim() == "" || _supplier.trim() == "0" || _creditNo.trim() == "" || _creditAmt.trim() == "" || _creditAmt.trim() == "0"
        || _creditDate.trim() == "" || _ecfNo.trim() == "" || _invoiceNo.trim() == "") {
        $('#btnUpdate').attr('disabled', true);
    }
    else {
        $('#btnUpdate').attr('disabled', false);
    }
}
function CreditGLType_Change() {
    var ECFNo = $("#txtECFNo").val();
    var InvoiceNo = $("#txtInvoiceNo").val();
    var creditgltype = $("input[name=rdoGLType]:radio[checked=checked]").attr("value");
    var Params = {
        ECFNo: ECFNo,
        InvNo: InvoiceNo,
        type: creditgltype,
        ViewType: "2"
    };
    $.ajax({
        type: "post",
        url: UrlDet + "LoadInvoiceDetails",
        async: false,
        data: JSON.stringify(Params),
        contentType: "application/json;",
        success: function (response) {
            var Data1 = "";
            if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null)
                Data1 = JSON.parse(response.Data1);
            if (Data1.length > 0) {
                $("#txtCreditGL").val(Data1[0].gl_no);
                $("#hdfCreditGL").val(Data1[0].gl_gid);
                $("#txtInvoiceAmount").val(Data1[0].gl_amount);
                if ($("#txtCreditGL").val().trim() != "") {
                    $("#txtCreditGL").removeClass('required').removeClass('valid');
                }
            }
            else {
                $("#txtCreditGL").val("");
                $("#txtInvoiceAmount").val("0");
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown);
        }
    });
}
function ShowDialog() {
    $('#ShowDialog').attr("style", "display:block;");

    objDialogy.dialog({ title: 'Recovery Entry', width: '800', height: '350' });
    objDialogy.dialog("open");
    return false;
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

//Load Credit GL Auto Complete
$("#txtCreditGL").keyup(function (e) {
    if (e.which != 13) {
        $("#hdfCreditGL").val("0");
    }

    $("#txtCreditGL").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: UrlHelper + "GetAutoCompleteCreditGLAll",
                data: "{ 'txt' : '" + $("#txtCreditGL").val() + "'}",
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
            $("#hdfCreditGL").val(i.item.val);
            $("#txtCreditGL").val(i.item.label);
        }
    });
});

$("#txtCreditGL").focusout(function () {
    var hdfId = $("#hdfCreditGL").val();
    var _data = $("#txtCreditGL").val();

    if ((_data.trim() != "" || _data.trim() == "") && (hdfId.trim() == "" || hdfId.trim() == "0")) {
        $("#txtCreditGL").val("");
        $("#txtCreditGL").removeClass('valid');
        $("#txtCreditGL").addClass('required');
    } else {
        $("#txtCreditGL").removeClass('required');
        $("#txtCreditGL").addClass('valid');
    }
});
