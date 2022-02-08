
const UsersIndexURL = '/Users/Index';
const UsersSearchURL = '/Users/Search';

function SearchObjCreationForUsers(searchTxt, selectedSearchIn, nextPgnoSearch, sortBy = 0, isSortDes = false, selectedRole = "DeliveryCustomer") {
    var searchObj = {};
    searchObj.SearchText = searchTxt;
    searchObj.SearchIn = selectedSearchIn;
    searchObj.PageNumber = nextPgnoSearch;
    searchObj.SortOrder = sortBy;
    searchObj.IsSortDes = isSortDes;
    searchObj.SelectedRole = selectedRole;
    if (isDateRangeActive) {
        searchObj.FromDate = fromDate;
        searchObj.ToDate = toDate;
    }
    return searchObj;
}

function GetUsersData(selectedPageNumber) {
    debugger;
    var selectedRole = $('#ddlRoles').val();
    if (isSearchActive == false) {
        ajaxBaseCall("GET", UsersIndexURL,
            { 'sortOrder': sortedBy, 'isSortDes': isSortDes, 'pageNumber': nextPgno, 'isAjax': true, 'selectedRole': selectedRole }
        );
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        if (searchText != null) {
            nextPgnoSearch = selectedPageNumber;
            var searchObj = SearchObjCreationForUsers(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes, selectedRole);
            ajaxBaseCall("POST", UsersSearchURL, { 'searchModel': searchObj });
        }
    }
}




function LoadUsersGrid(response) {
    $("#grid_table tbody tr").remove();
    $.each(response.data, function (i, item) {
        var delifre = "";
        if (item.deliveryFrequency == 1)
            delifre = "Daily";
        else if (item.deliveryFrequency == 2)
            delifre = "Thrice A week";
        else if (item.deliveryFrequency == 3)
            delifre = "Twice A week";
        else
            delifre = "Once A week";
        var color = i % 2 == 0 ? 'gray' : "white";
        var rows = "<tr style='color: " + color + "'>"
            + "<td>" + parseInt(i + 1) + "</td>"
            + "<td><input id =" + item.id + " type='checkbox' class='chkboxes' /></td > "
            + "<td>" + item.customerIdPos + "</td>"
            + "<td>" + item.fullName + "</td>"
            + "<td>" + item.phoneNumber + "</td>"
            + "<td>" + item.address + "</td>"
            + "<td>" + delifre + "</td>"
            + "<td>" + item.bottleSpecialPrice + "</td>" +
            " <td style='width: 9%;' class='action-center action-td'>" +
            `<a href='/Users/Details/${item.id}' class='edit'><i class='glyphicon glyphicon-user'></i></a>` +
            `<a  href='/Users/Edit/${item.id}' class='edit'><i class='glyphicon glyphicon-pencil'></i></a>` +
            `<a href='/Users/Delete/${item.id}' class='delete'><i class='glyphicon glyphicon-trash'></i></a> </td>` +
            "</tr>";
        $('#grid_table tbody').append(rows);
    });
    ButtonsOperations(response);
}



function onCLickResetUserSearch() {
    isSearchActive = false;
    $('#ddlRoles').val("DeliveryCustomer")
    $("#search_txt").val("");
    let url = UsersIndexURL;// searchValue != "isFingerPrint" ? SavedReportSearchURL : FingerPrintSearchURL;
    ajaxBaseCall("GET", url, { 'isAjax': true });
}

$('#btnCreateUser').click(function () {
    debugger;
    if ($('#RoleName').val() == "DeliveryCustomer") {
        var dayArr = $('#Days').val();
        if (dayArr == null || dayArr.length < 1) {
            alert('Please assigned a day for delivery to customer');
            return false;
        }
        var errors = [];
        //1 daily, 2 thrice a week, 3 twice a week, 7 once a week
        var frequency = $('#DeliveryFrequency').val();
        if (frequency == "1") {
            if (dayArr.length == 1) {
                if (dayArr[0] != "All") {
                    alert('Daily frequency must have all days selected or All option selected');
                    return false;
                }
            } else {
                if (dayArr.find(sa => sa == "All") == undefined && dayArr.length < 7) {
                    alert('Daily frequency must have all days selected or All option selected');
                    return false;
                }
            }
        }
        else {
            if (frequency == '2') {
                if (dayArr.length == 3 && dayArr.find(sa => sa == "All") != undefined) {
                    alert('Thrice a week frequency must have only 3 days selected in a week, and No All option.');
                    return false;
                }
                if (dayArr.length != 3) {
                    alert('Thrice a week frequency must have only 3 days selected in a week');
                    return false;
                }
            } else if (frequency == '3') {
                if (dayArr.length == 2 && dayArr.find(sa => sa == "All") != undefined) {
                    alert('Twice a week frequency must have only 2 days selected in a week, and No All option.');
                    return false;
                }
                if (dayArr.length != 2) {
                    alert('Twice a week frequency must have only 2 days selected in a week');
                    return false;
                }
            } else if (frequency == '7') {
                if (dayArr.length == 1 && dayArr.find(sa => sa == "All") != undefined) {
                    alert('Once a week frequency must have only 1 day selected in a week, and No All option.');
                    return false;
                }
                if (dayArr.length != 1) {
                    alert('Once a week frequency must have only 1 day selected in a week');
                    return false;
                }
            }


        }
    }

    $("form[name='userform']").submit();
});

$("#Days").change(function () {
    debugger;
    var value = $(this).val();
    if (value.find(s => s == "All") != undefined) {
        $('#Days').val(["All"]);
    }
});

$("#DeliveryFrequency").change(function () {
    //debugger;
    var value = this.value;
    if (value == "1") {
        $('#Days').val(["All"]);
    } else {
        $('#Days').val([]);
    }
});

$('#ddlRoles').change(function () {
    var value = $(this).val();
    isSortClick = false;
    isSortDes = !isSortDes;
    acrCatId = $('#acrCatDr').val();
    if (isSearchActive == false) {
        ajaxBaseCall("GET", UsersIndexURL, { 'sortOrder': 1, 'isSortDes': isSortDes, 'isSortClick': isSortClick, 'pageNumber': nextPgno, 'isAjax': true, 'selectedRole': value });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreation(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes, value);
        ajaxBaseCall("POST", UsersSearchURL, { 'searchModel': searchObj });
    }
});


