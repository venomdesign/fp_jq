var selectedCount = 0; $(".invCount").text(selectedCount);
var selectedTotal = 0; $(".invTotal").text(selectedTotal);
var grid, gridType, config, resultSet = [];
var id, gridDetailInit;

function showAdvanced() {
    $(".advancedSearch").toggle("show");
    $('#SuperSearch').val = '';
    $("#defaultSearch").prop("checked", true);
    $("#noSelection").show();
    $("#dates").hide();
    $("#amounts").hide();
}

function showDates() {
    $("#noSelection").hide();
    $("#dates").show();
    $("#amounts").hide();
}

function showAmounts() {
    $("#noSelection").hide();
    $("#dates").hide();
    $("#amounts").show();
}

function GetTokensForUser(userSysId) {
    $.ajax({
        url: "/api/v1/Administration/GetTokensForUser?UserSysId=" + userSysId
    }).done(function (result) {
        result = JSON.parse(result);

        $.each(result, function () {
            var len2 = this.TOKEN.toString().length;

            var lastFour = this.TOKEN.toString().slice(len2 - 4);

            $("select[name='CreditCard']")
                .append($('<option>', { "value": this.TOKEN.toString() })
                    .text(this.NICKNAME.toString() + ": ****" + lastFour));
        });
    });
};

function GetUnpaidInvoices(contactId) {
    $(".selectedInvoice").prop("disabled", true);

    $.ajax({
        url: "/api/v1/Crrar/GetUnpaidInvoices?contactId=" + contactId
    }).done(function (results) {
        console.log(results);
        resultSet = results;
        //grab a reference to the ID
        id = contactId;
        gridType = "invoice";

        config = {
            parse: function (data) {
                var events = [];
                for (var i = 0; i < data.length; i++) {
                    var event = data[i];
                    event.InvDt = kendo.toString(event.invoiceDate, 'MM/dd/yyyy');
                    events.push(event);
                }
                return events;
            },
            model: {
                fields: {
                    invoiceReference: { type: "string" },
                    billToContactName: { type: "string" },
                    attentionName: { type: "string" },
                    invoiceDate: { type: "date" },
                    arCompanyOperation: { type: "string" },
                    refData: { type: "string" },
                    currentBalance: { type: "number" },
                    customerReference: { type: "string" }
                }
            },
            columnArray: [
                { template: "<input type='checkbox' class='checkbox _row-selector' />", width: 30 },
                { field: "fileNumRefNum", title: "File#/Invoice#", width: 140 },
                { field: "customerReference", title: "Customer Ref#" },
                { field: "billToContactName", title: "Name" },
                { field: "attentionName", title: "Attn To" },
                { field: "invoiceDate", title: "Date", template: "#= kendo.toString(invoiceDate, 'MM/dd/yyyy') #" },
                { field: "refData", title: "Ref Data" },
                { field: "currentBalance", title: "Amount", format: "{0:c}" }
            ]
        };

        buildGrid(gridType, config, resultSet);

        grid.table.on("click", ".checkbox", selectRow);
        var checkedRows = {};
        function selectRow() {
            var checked = this.checked,
                row = $(this).closest("tr"),
                grid = $("#invoiceGrid").data("kendoGrid"),
                dataItem = grid.dataItem(row);

            checkedRows[dataItem.id] = checked;
            if (checked) {
                //-select the row
                row.addClass("k-state-selected");
                selectedCount += 1;
                selectedTotal += dataItem.currentBalance;
                $(".selectedInvoice").prop("disabled", false);
            } else {
                //-remove selection
                row.removeClass("k-state-selected");
                selectedCount -= 1;
                selectedTotal -= dataItem.currentBalance;

            }

            $(".invCount").text(selectedCount);
            $(".invTotal").text(parseInt(selectedTotal));
        };

    });
};

function GetPaymentHistory(contactId) {
    $.ajax({
        url: "/api/v1/Crrar/GetHistory?contactId=" + contactId
    }).done(function (results) {
        console.log(results);
        resultSet = JSON.parse(results);
        id = contactId;
        gridType = "payments";

        config = {
            parse: function (data) {
                var events = [];
                for (var i = 0; i < data.length; i++) {
                    var event = data[i];
                    event.TransactionDate = kendo.toString(event.TRX_DATE, 'MM/dd/yyyy');
                    events.push(event);
                }
                return events;
            },
            model: {
                fields: {
                    CONFIRMATION_NUMBER: { type: "string" },
                    USER_NAME: { type: "string" },
                    TRX_AMOUNT: { type: "number" },
                    TRX_DATE: { type: "date" },

                }
            },
            columnArray: [
                { field: "TRX_DATE", title: "Date", template: "#= kendo.toString(TRX_DATE, 'MM/dd/yyyy') #" },
                { field: "CONFIRMATION_NUMBER", title: "Confirmation No." },
                { field: "USER_NAME", title: "UserName" },
                { field: "TRX_AMOUNT", title: "Amount", format: "{0:c}" }
            ]
        };

        buildGrid(gridType, config, resultSet);

    });
};

function buildGrid(gridType, config, results) {
    console.log(gridType);
    var itemType = (gridType === "invoice") ? "Invoices" : "Payments";

    grid = $("#invoiceGrid").kendoGrid({
        dataSource: {
            data: results,
            pageSize: 100,
            schema: {
                parse: config.parse,
                model: config.model
            }
        },
        height: 650, // set the hieght to make the header static
        pageSize: 100,
        sortable: true,
        resizable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: [20, 50, 100, 200],
            previousNext: true,
            input: false,
            info: true,
            buttonCount: 5,
            messages: {
                display: "{0}-{1} of {2} " + itemType,
                itemsPerPage: itemType
            }
        },
        groupable: true,
        detailInit: detailInit,
        dataBound: function () {
            $("#invoiceGrid .k-grid-header").removeAttr("style");
            $('#invoiceGrid .k-grid-header-wrap').css("border-width", "0");
            $("table[role=treegrid").addClass("table table-hover nowrap dataTable dtr-inline");
            $("table[role=treegrid] tr.k-master-row td").addClass("easyPay_cell");
        },
        columns: config.columnArray,
        excel: {
            allPages: true
        },
    }).data("kendoGrid");

    function detailInit(e) {
        gridDetailInit = e;

        if (gridType === "invoice") {
            buildInvoiceDetails(e);
        } else {
            buildHistoryDetails(e);
        };
    };

    $("#excelExport").click(function (e) {
        var grid = $("#invoiceGrid").data("kendoGrid");
        grid.saveAsExcel();
    });
};

function buildInvoiceDetails(e) {
    $("<div/>").appendTo(e.detailCell).kendoGrid(
        {
            dataSource: { data: e.data.detail, pageSize: 10 },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            pageSize: 10,
            sortable: true,
            pageable: true,
            dataBound: function () {
                $("#invoiceGrid > div.k-grid-content.k-auto-scrollable > table > tbody > tr.k-detail-row > td.k-detail-cell > div > div.k-grid-header").removeAttr("style");
            },
            columns: [
                { field: "transactionDescription", title: "Transaction Description" },
                { field: "chargeAmount", title: "Charge Amount", format: "{0:c}" }
            ]
        });
};

function buildHistoryDetails(e) {
    $("<div class=\"invoiceDetails\"/>").appendTo(e.detailCell).kendoGrid(
        {
            dataSource: { data: e.data.TRANSACTION_HISTORY_DETAIL, pageSize: 10 },
            columns: [
                { field: "INVOICE_NUMBER", title: "Invoice Number" },
                { field: "BILL_TO", title: "Contact Name" },
                { field: "REF_DATA", title: "Ref Data" },
                { field: "TRX_AMOUNT", title: "Amount", format: "{0:c}" }
            ],
            dataBound: function () {
                $("#invoiceGrid > div.k-grid-content.k-auto-scrollable > table > tbody > tr.k-detail-row > td.k-detail-cell > div > div.k-grid-header").removeAttr("style");

            },
            serverPaging: false,
            serverSorting: true,
            serverFiltering: true,
            pageSize: 10,
            sortable: false,
            pageable: false
        });
};

/* Start of SuperSearch */
function compareObjects(o1, o2) {
    var k = '';
    for (k in o1) if (o1[k] != o2[k]) return false;
    for (k in o2) if (o1[k] != o2[k]) return false;
    return true;
}

function itemExists(haystack, needle) {
    for (var i = 0; i < haystack.length; i++) if (compareObjects(haystack[i], needle)) return true;
    return false;
}

function searchFor(toSearch, record) {
    if (toSearch.value.trim().length == 0) { refreshGrid(resultSet); }
    if (toSearch.value.trim().length > 1) {
        var results = [];
        toSearch = toSearch.value.trim();
        console.log("searching");
        //This is RETARTED, but the only way to get a copy of the array that was not a reference to the original data!!
        var data = JSON.parse(JSON.stringify(resultSet));

        $.each(data, function (key, obj) {
            var match = false;
            var subMatch = false;
            var newCharges = [];

            $.each(obj, function (key, prop) {
                //Make sure that the prop value is converted to Currency to search for $, and .
                if (key === "currentBalance" || key === "TRX_AMOUNT") {
                    var currency = '$' + prop.toFixed(2).replace(/(\d)(?=(\d{3})+$)/g, "$1,");
                    if (currency.toString().indexOf(toSearch.toUpperCase()) != -1) {
                        match = true;
                    }
                }else if (prop !== null) {
                    if (prop.toString().toUpperCase().indexOf(toSearch.toUpperCase()) != -1) {
                        match = true;
                    }
                }

                if (!match) {
                    var newCharges = [];

                    if (record === "invoices") {
                        newCharges = iterateDetails(record, obj, toSearch);
                        if (newCharges.length > 0) {
                            subMatch = true;
                            obj.detail = newCharges;
                        }
                    } else if (record === "payments") {
                        newCharges = iterateDetails(record, obj, toSearch);
                        if (newCharges.length > 0) {
                            obj.TRANSACTION_HISTORY_DETAIL = newCharges;
                            subMatch = true;
                        }
                    }

                }
            });

            if (match || subMatch) { results.push(this); }

        });

        refreshGrid(results);

    }
}

function refreshGrid(data) {
    var grid = $("#invoiceGrid").data("kendoGrid");
    var newDs = new kendo.data.DataSource({
        data: data,
        pageSize: 100,
        schema: {
            parse: config.parse,
            model: config.model
        }
    });

    grid.setDataSource(newDs);

}

function iterateDetails(recordType, obj, toSearch) {
    if (recordType === "invoices") {
        var newCharges = [];
        $.each(obj.detail, function (key, chg) {

            $.each(chg, function (subKey, subValue) {
                if (subKey === "chargeAmount") {
                    var currency = '$' + subValue.toFixed(2).replace(/(\d)(?=(\d{3})+$)/g, "$1,");
                    if (currency.toString().indexOf(toSearch.toUpperCase()) != -1) {
                        newCharges.push(chg);
                    }
                } else if (subValue !== null) {
                    if (subValue.toString().toUpperCase().indexOf(toSearch.toUpperCase()) != -1) {
                        newCharges.push(chg);
                    }
                }
            });
        })
        return newCharges;
    }

    if (recordType === "payments") {
        var newCharges = [];
        $.each(obj.TRANSACTION_HISTORY_DETAIL, function (key, chg) {

            $.each(chg, function (subKey, subValue) {
                if (subKey === "TRX_AMOUNT") {
                    var currency = '$' + subValue.toFixed(2).replace(/(\d)(?=(\d{3})+$)/g, "$1,");
                    if (currency.toString().indexOf(toSearch.toUpperCase()) != -1) {
                        newCharges.push(chg);
                    }
                } else if (subValue !== null) {
                    if (subValue.toString().toUpperCase().indexOf(toSearch.toUpperCase()) != -1) {
                        newCharges.push(chg);
                    }
                }
            });
        })
        return newCharges;
    }
}
/* End of SuperSearch */

/* Start of Advanced Search */
function searchDates(recordType) {
    //Neither invoices nor history detail records contain dates, no need to search them!
    var startDatePicker = $("#startDate").data("kendoDatePicker");
    var endDatePicker = $("#endDate").data("kendoDatePicker");
    var startDate = startDatePicker.value();
    var endDate = endDatePicker.value();
    var results = [];

    var hasBegin = (startDate != null);
    var hasEnd = (endDate != null);
    var searchType = "";
    if (hasBegin && !hasEnd) { searchType = "after"; }
    if (!hasBegin && hasEnd) { searchType = "before"; }
    if (hasBegin && hasEnd) { searchType = "between"; }

    var data = resultSet;

    $.each(data, function (key, obj) {
        var match = false;
        var subMatch = false;
        var newCharges = [];

        $.each(obj, function (key, prop) {

            //if the object is a date
            if (moment.isDate(prop)) {
                //alert('Found a date!');
                switch (searchType) {
                    case "before":
                        if (moment(prop).isSameOrBefore(endDate, 'day')) {
                            match = true;
                            //alert('Found one before ' + endDate);
                        }
                        break;
                    case "after":
                        if (moment(prop).isSameOrAfter(startDate, 'day')) {
                            match = true;
                            //alert('Found one after ' + startDate);
                        }
                        break;
                    case "between":
                        if (moment(prop).isBetween(startDate, endDate, null, '[)')) {
                            match = true;
                            //alert('Found one between ' + startDate + ' and ' + endDate);
                        }
                        break;
                    default:
                }
            }
        });

        if (match) { results.push(this); }

    });

    refreshGrid(results);
}

function searchAmounts(recordType) {
    var lowAmountControl = $("#lowAmount").data("kendoNumericTextBox");
    var highAmountControl = $("#highAmount").data("kendoNumericTextBox");
    var low = lowAmountControl.value();
    var high = highAmountControl.value();
    var results = [];

    var hasLow = (low != null);
    var hasHigh = (high != null);
    var searchType = "";
    if (hasLow && !hasHigh) { searchType = "gt"; }
    if (!hasLow && hasHigh) { searchType = "lt"; }
    if (hasLow && hasHigh) { searchType = "between"; }

    var data = resultSet;

    $.each(data, function (key, obj) {
        var match = false;
        var subMatch = false;
        var newCharges = [];

        $.each(obj, function (key, prop) {

            //if the object is a date
            if (key == "currentBalance" || key == "TRX_AMOUNT") {
                var amt = parseFloat(prop);

                switch (searchType) {
                    case "lt":
                        if (high >= amt) {
                            match = true;
                        }
                        break;
                    case "gt":
                        if (low <= amt) {
                            match = true;
                        }
                        break;
                    case "between":
                        if (high >= amt && low <= amt) {
                            match = true;
                        }
                        break;
                    default:
                }
            }
        });

        if (!match) {
            //Check the child rows (No amounts listed in Invoice details, only payment history details)
            newCharges = searchDetails(obj, low, high);
            if (newCharges.length > 0) { subMatch = true; obj.TRANSACTION_HISTORY_DETAIL = newCharges; }
        }

        if (match || subMatch) { results.push(this); }

    });

    refreshGrid(results);
}

function searchDetails(obj, low, high) {

    var newCharges = [];
    $.each(obj.TRANSACTION_HISTORY_DETAIL, function (key, chg) {

        $.each(chg, function (subKey, subValue) {
            if (key == "TRX_AMOUNT") {
                var amt = parseFloat(prop);

                switch (searchType) {
                    case "lt":
                        if (high <= amt) {
                            match = true;
                        }
                        break;
                    case "gt":
                        if (low >= amt) {
                            match = true;
                        }
                        break;
                    case "between":
                        if (high >= amt && low <= amt) {
                            match = true;
                        }
                        break;
                    default:
                }
            }
        });
    })
    return newCharges;
}
/* End of Advanced Search */

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();