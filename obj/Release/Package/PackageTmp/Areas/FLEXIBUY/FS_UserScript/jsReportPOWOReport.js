


// var token1 = document.getElementById('antiForgeryForm').childNodes[1].value;
var token = $('[name=__RequestVerificationToken]').val();//$("#txtantiforgery").val();
//alert(token);
debugger
$.ajaxSetup({
    cache: false,
    headers: { "__RequestVerificationToken": token }
});

var SearchModel = function (id) {

    var self = this;

    self.ResultArray = ko.observableArray();

    self.formatNumber = function (num) {
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


    self.search = function () {
        debugger;
        var inputval = "";
        if ($("#txtFrom").val() == null || $("#txtFrom").val() == "") {
            jAlert("From Date Should not be empty !", "Message");
            return false;
        }
        else if ($("#txtto").val() == null || $("#txtto").val() == "") {
            jAlert("To Date Should not be empty!", "Message");
            return false;
        }
        var Inputfilter = {
            pono: inputval,
            Fromdate: $("#txtFrom").val(),
            Todate: $("#txtto").val()
        }
        $.ajax({
            type: "post",
            url: "../Report/FetchReportPOInvoiceData",
            data: ko.toJSON(Inputfilter),
            async: false,
            contentType: "application/json;",
            success: function (response) {
                if (response != null && response != "") {
                    var Data1 = "", Data2 = "";
                    $("#gvReport").DataTable({
                        "destroy": true
                    }).destroy();
                    self.ResultArray([]);

                    if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                        Data1 = JSON.parse(response.Data1);

                        if (response.Data2 != null && response.Data2 != "" && JSON.parse(response.Data2) != null)
                            Data2 = JSON.parse(response.Data2);
                        self.ResultArray(Data2);

                        if (Data1[0].Clear == false) {
                            jAlert(Data1[0].Message, "Message");
                            return false;
                        } else {
                            if (Data2 == "") {
                                jAlert("Sorry No Record Found!", "Message");
                                return false;
                            }
                        }
                    }
                } else {
                    jAlert("unable to process your request. please try again!", "Message");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

            }
        });
    }



    //selva Created 20-05-2021

    self.search = function () {
        debugger;
        var datefrom = "";
        var dateto = "";
        datefrom = $("#txtFrom").val();
        dateto = $("#txtto").val();    

       var dtf = $("#txtFrom").datepicker("getDate");
       var dt0 = $("#txtto").datepicker("getDate");
       var days = (dt0 - dtf) / (1000 * 60 * 60 * 24);



        if ($("#txtFrom").val() == null || $("#txtFrom").val() == "") {
            jAlert("From Date Should not be empty !", "Message");
            return false;
        }

        else if ($("#txtto").val() == null || $("#txtto").val() == "") {
            jAlert("To Date Should not be empty!", "Message");
            return false;
        }

        else if (days > 90) {
            jAlert("Date Should no be greater than 90 days ", "Message");
            return false;
        }



        var rbpoworpt;
        rbpoworpt = $('input:radio[name=rbpoworpt]:checked').val();
        showProgress();

        $.ajax({
            url: '../POWOreport/FetchReportPOWOData?datefrom=' + datefrom + '&dateto=' + dateto + '&rbpoworpt=' + rbpoworpt,
            data: '',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                debugger;

                if (response != null && response != "") {
                    var Data1 = "", Data2 = "";
                    $("#gvReport").DataTable({
                        "destroy": true
                    }).destroy();
                    self.ResultArray([]);
                    if (response.Data1 != null && response.Data1 != "" && JSON.parse(response.Data1) != null) {
                        Data1 = JSON.parse(response.Data1);
                        self.ResultArray(Data1);
                        hideProgress();
                        $('#btnExporExcel').attr('disabled', false);
                    }

                    else {
                        if (Data1 == "") {
                            jAlert("Sorry No Record Found!", "Message");
                            hideProgress();
                            $('#btnExporExcel').attr('disabled', true);
                            return false;

                        }
                    }
                }

            },
            error: function (result) {
                hideProgress();
                jAlert("Errors.", result);
                return false;
            }
        });
    }

    //selva Created 20-05-2021


    self.Download = function () {
        debugger;
        // alert('download');
        var datefrom = "";
        var dateto = "";
        datefrom = $("#txtFrom").val();
        dateto = $("#txtto").val();
        if ($("#txtFrom").val() == null || $("#txtFrom").val() == "") {
            jAlert("From Date Should not be empty !", "Message");
            return false;
        }
        else if ($("#txtto").val() == null || $("#txtto").val() == "") {
            jAlert("To Date Should not be empty!", "Message");
            return false;
        }


        var rbpoworpt;
        rbpoworpt = $('input:radio[name=rbpoworpt]:checked').val();
        showProgress();

        $.ajax({
            type: "post",
            url: '../POWOreport/GetPOWOQueryReport?datefrom=' + datefrom + '&dateto=' + dateto + '&rbpoworpt=' + rbpoworpt,
            data: ko.toJSON(Inputfilter),
            async: false,
            contentType: "application/json;",
            success: function (response) {
                if (response == "process") {
                    ko.utils.postJson("../POWOreport/GetPOWOQueryReportexport");
                } else {
                    jAlert("Sorry No Record Found!", "Message");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            }
        });
    };


    self.DatatableCall = function () {
        if ($("#gvReport > tbody > tr").length == self.ResultArray().length) {
            $("#gvReport").DataTable({
                "autoWidth": false,
                "destroy": true,
                "scrollX": true,
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

    self.clearFilter = function () {
        $("#txtpono").val("");
        $("#txtFrom").val("");
        $("#txtto").val("");
        //self.search();
        if (self.ResultArray().length > 0) {
            $("#gvReport").DataTable({
                "autoWidth": false,
                "destroy": true
            }).destroy();
            self.ResultArray.removeAll();
        }
    };

    self.loadGrid = function () {
        $("#gvReport").DataTable({
            "autoWidth": false,
            "destroy": true
        }).destroy();
        self.ResultArray.removeAll();
    }
    self.loadGrid();

};

var mainViewModel = new SearchModel();
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
})