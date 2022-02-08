const DeliveryScheduleIndexURL = '/DeliverySchedules/Index';
const DeliveryScheduleSearchURL = '/DeliverySchedules/Search';
const DeliveryScheduleTodaysDeliveriesURL = '/DeliverySchedules/TodaysDeliveries'
const DeliveryScheduleTodaysDeliveriesSearchURL = '/DeliverySchedules/Search';

function SearchObjCreationForDeliveries(searchTxt, selectedSearchIn, nextPgnoSearch, selectedDriver, sortBy = 0, isSortDes = false) {
    var searchObj = {};
    searchObj.SearchText = searchTxt;
    searchObj.SearchIn = selectedSearchIn;
    searchObj.PageNumber = nextPgnoSearch;
    searchObj.SortOrder = sortBy;
    searchObj.IsSortDes = isSortDes;
    searchObj.SelectedDriver = selectedDriver;
    if (isDateRangeActive) {
        searchObj.FromDate = fromDate;
        searchObj.ToDate = toDate;
    }
    return searchObj;
}

function GetTodaysDeliveriesData(selectedPageNumber) {
    debugger;
    var selectedDriver = $('#ddlDrivers').val();
    if (isSearchActive == false) {
        ajaxBaseCall("GET", DeliveryScheduleTodaysDeliveriesURL,
            { 'sortOrder': sortedBy, 'isSortDes': isSortDes, 'pageNumber': nextPgno, 'isAjax': true, SelectedDriver: selectedDriver }
        );
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        if (searchText != null) {
            nextPgnoSearch = selectedPageNumber;
            var searchObj = SearchObjCreationForDeliveries(searchText, selectedSearchIn, nextPgnoSearch, selectedDriver, sortedBy, isSortDes);
            ajaxBaseCall("POST", DeliveryScheduleTodaysDeliveriesSearchURL, { 'searchModel': searchObj });
        }
    }
}



function LoadTodaysDeliveriesGrid(response) {
    $("#grid_table tbody tr").remove();
    $.each(response.data, function (i, item) {
        var color = i % 2 == 0 ? 'gray' : "white";
        var rows = "<tr style='color: " + color + "'>"
            + "<td>" + parseInt(i + 1) + "</td>"
            + "<td><input id =" + item.deliveryScheduleDetailsID + " type='checkbox' class='chkboxes' /></td > "
            + "<td>" + item.fullName + "</td>"
            + "<td>" + item.customerID + "</td>"
            + "<td>" + item.noOfBottles + "</td>"
            + "<td>" + item.bottlePrice + "</td>"
            + "<td><input  " + (item.isDelivered ? "checked" : "") + "  type='checkbox' class='chkboxes' /></td > "
            + "<td>" + item.totalAmount + "</td>"
            + "<td>" + item.paidAmount + "</td>"
            + " <td style='width: 9%;' class='action-center action-td'>" +
            `<a href='#' class='edit'><i class='glyphicon glyphicon-edit'></i></a> ` +
            `</td>` +
            "</tr>";
        $('#grid_table tbody').append(rows);
    });
    ButtonsOperations(response);
}



$('#ddlDrivers').change(function () {
    var value = $(this).val();
    $("#viewmap").attr("href", `/DeliverySchedules/ViewTodaysDeliveriesOnMap/${value}`);
    $("#downloadexcel").attr("href", `/DeliverySchedules/DownloadExcel/${value}`);
    isSortClick = false;
    isSortDes = !isSortDes;
    if (isSearchActive == false) {
        ajaxBaseCall("GET", DeliveryScheduleTodaysDeliveriesURL, {
            'sortOrder': 1, 'isSortDes': isSortDes, 'isSortClick': isSortClick, 'pageNumber': nextPgno,
            'isAjax': true, 'SelectedDriver': value
        });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreationForDeliveries(searchText, selectedSearchIn,value, nextPgnoSearch, sortedBy, isSortDes);
        ajaxBaseCall("POST", DeliveryScheduleTodaysDeliveriesURL, { 'searchModel': searchObj });
    }
});