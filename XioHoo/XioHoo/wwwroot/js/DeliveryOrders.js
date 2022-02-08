
const DeliveryOrderIndexUrl = '/CustomersInAreaDeliveryOrders'
const DeliveryOrderSearchUrl = '/CustomersInAreaDeliveryOrders/Search';

function SearchObjCreationForDeliveryOrder(searchTxt, selectedSearchIn, nextPgnoSearch, selectedDriver,selectedDay,
    sortBy = 0, isSortDes = false) {
    var searchObj = {};
    searchObj.SearchText = searchTxt;
    searchObj.SearchIn = selectedSearchIn;
    searchObj.PageNumber = nextPgnoSearch;
    searchObj.SortOrder = sortBy;
    searchObj.IsSortDes = isSortDes;
    searchObj.SelectedDriver = selectedDriver;
    searchObj.SelectedDay = selectedDay;
    if (isDateRangeActive) {
        searchObj.FromDate = fromDate;
        searchObj.ToDate = toDate;
    }
    return searchObj;
}

function GetDeliveryOrderData(selectedPageNumber) {
    debugger;
    var selectedDriver = $('#ddlDrivers').val();
    var selectedDay = $('#ddlDaysDo').val();
    if (!isSearchActive) {
        ajaxBaseCall("GET", DeliveryOrderIndexUrl,
            { 'sortOrder': sortedBy, 'isSortDes': isSortDes, 'pageNumber': nextPgno, 'isAjax': true, SelectedDriver: selectedDriver, SelectedDay: selectedDay }
        );
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        if (searchText != null) {
            nextPgnoSearch = selectedPageNumber;
            var searchObj = SearchObjCreationForDeliveryOrder(searchText, selectedSearchIn, nextPgnoSearch, selectedDriver, selectedDay, sortedBy, isSortDes);
            ajaxBaseCall("POST", DeliveryOrderSearchUrl, { 'searchModel': searchObj });
        }
    }
}



function LoadDeliveryOrderGrid(response) {
    var days = ["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"]
    $("#grid_table tbody tr").remove();
    $.each(response.data, function (i, item) {
        var color = i % 2 == 0 ? 'gray' : "white";
        var rows = "<tr style='color: " + color + "'>"
            + "<td>" + parseInt(i + 1) + "</td>"
            + "<td><input id =" + item.customersInAreaDeliveryOrderID + " type='checkbox' class='chkboxes' /></td > "
            + "<td>" + item.applicationUser.fullName + "</td>"
            + "<td>" + item.orderNo + "</td>"
            + "<td>" + item.distanceFromPreviousStop + "</td>"
            + "<td>" + days[item.dayID] + "</td>"
            + " <td style='width: 9%;' class='action-center action-td'>" +
            `<a href='#' class='edit'><i class='glyphicon glyphicon-edit'></i></a> ` +
            `</td>` +
            "</tr>";
        $('#grid_table tbody').append(rows);
    });
    ButtonsOperations(response);
    $("#totalcount").text("(" + response.data.length + ")");
}

$('#ddlDaysDo').change(function () {
    debugger;
    var selectedDay = $(this).val();
    var value = $('#ddlDrivers').val();
    $("#viewmap").attr("href", `/CustomersInAreaDeliveryOrders/ViewSeletedDeliveriesOnMap/${value}/${selectedDay}`);
    $("#downloadexcel").attr("href", `/CustomersInAreaDeliveryOrders/DownloadExcel/${value}/${selectedDay}`);
    isSortClick = false;
    isSortDes = !isSortDes;
    if (isSearchActive == false) {
        ajaxBaseCall("GET", DeliveryOrderIndexUrl, {
            'sortOrder': 1, 'isSortDes': isSortDes, 'isSortClick': isSortClick, 'pageNumber': nextPgno,
            'isAjax': true, 'SelectedDriver': value, SelectedDay: selectedDay
        });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreationForDeliveryOrder(searchText, selectedSearchIn, value, selectedDay, nextPgnoSearch, sortedBy, isSortDes);
        ajaxBaseCall("POST", DeliveryOrderSearchUrl, { 'searchModel': searchObj });
    }
});

$('#ddlDrivers').change(function () {
    debugger;
    var value = $(this).val();
    var selectedDay = $('#ddlDaysDo').val();
    $("#viewmap").attr("href", `/CustomersInAreaDeliveryOrders/ViewSeletedDeliveriesOnMap/${value}/${selectedDay}`);
    $("#downloadexcel").attr("href", `/CustomersInAreaDeliveryOrders/DownloadExcel/${value}/${selectedDay}`);
    isSortClick = false;
    isSortDes = !isSortDes;
    if (isSearchActive == false) {
        ajaxBaseCall("GET", DeliveryOrderIndexUrl, {
            'sortOrder': 1, 'isSortDes': isSortDes, 'isSortClick': isSortClick, 'pageNumber': nextPgno,
            'isAjax': true, 'SelectedDriver': value, SelectedDay: selectedDay
        });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreationForDeliveryOrder(searchText, selectedSearchIn, value, selectedDay, nextPgnoSearch, sortedBy, isSortDes);
        ajaxBaseCall("POST", DeliveryOrderSearchUrl, { 'searchModel': searchObj });
    }
});