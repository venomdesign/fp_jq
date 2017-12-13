var selectedCount = 0; $(".invCount").text(selectedCount);
var selectedTotal = 0; $(".invTotal").text(selectedTotal);
var grid;

var GetTokensForUser = function (userSysId) {
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

var GetUnpaidInvoices = function (contactId) {
    $(".selectedInvoice").prop("disabled", true);

    $.ajax({
        url: "/api/v1/Crrar/GetUnpaidInvoices?contactId=" + contactId
    }).done(function (results) {
        var config = {
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
                    referenceData: { type: "string" },
                    currentBalance: { type: "number" }
                }
            },
            columnArray: [
                { template: "<input type='checkbox' class='checkbox' />", width: 30 },
                { field: "invoiceReference", title: "Ref. #" },
                { field: "billToContactName", title: "Name" },
                { field: "attentionName", title: "Attn To" },
                { field: "invoiceDate", title: "Date", template: "#= kendo.toString(invoiceDate, 'MM/dd/yyyy') #" },
                { field: "arCompanyOperation", title: "File No." },
                { field: "referenceData", title: "Ref. Data" },
                { field: "currentBalance", title: "Amount", format: "{0:c}" }
            ]
        };

        buildGrid("invoice", config, results);

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

var GetPaymentHistory = function (contactId) {
    $.ajax({
        url: "/api/v1/Crrar/GetHistory?contactId=" + contactId
    }).done(function (results) {
        var config = {
            parse: function (data) {
                var events = [];
                for (var i = 0; i < data.length; i++) {
                    var event = data[i];
                    event.TransactionDate = kendo.toString(event.date, 'MM/dd/yyyy');
                    events.push(event);
                }
                return events;
            },
            model: {
                fields: {
                    trxDate: { type: "date" },
                    confirmationNbr: { type: "string" },
                    userName: { type: "string" },
                    amount: { type: "number" },
                    TransactionDate: { type: "date" }
                }
            },
            columnArray: [
                { field: "TransactionDate", title: "Date", template: "#= kendo.toString(TransactionDate, 'MM/dd/yyyy') #" },
                { field: "confirmationNbr", title: "Confirmation No." },
                { field: "userName", title: "UserName" },
                { field: "amount", title: "Amount", format: "{0:c}" }
            ]
        };

        buildGrid("history", config, results);

    });
};

var buildGrid = function (gridType, config, results) {
    grid = $("#invoiceGrid").kendoGrid({
        dataSource: {
            data: results,
            pageSize: 10,
            schema: {
                parse: config.parse,
                model: config.model
            }
        },
        pageSize: 10,
        sortable: true,
        filterable: true,
        pageable: {
            alwaysVisible: true,
            pageSizes: [5, 10, 20, 100],
            previousNext: true,
            input: false,
            info: true,
            buttonCount: 5,
            messages: {
                display: "{0}-{1} of {2} Invoices",
                itemsPerPage: "Invoices"
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
        if (gridType == "invoice") {
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
            dataSource: { data: e.data.invoices, pageSize: 10 },
            columns: [
                { field: "invoiceNbr", title: "Invoice Number" },
                { field: "billTo", title: "Contact Name" },
                { field: "amount", title: "Amount", format: "{0:c}" }
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

