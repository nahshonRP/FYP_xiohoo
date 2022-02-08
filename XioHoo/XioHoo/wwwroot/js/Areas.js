const AreasIndexURL = '/Areas/Index';
const AreasSearchURL = '/Areas/Search';


function SearchObjCreationForAreas(searchTxt, selectedSearchIn, nextPgnoSearch, sortBy = 0, isSortDes = false) {
    var searchObj = {};
    searchObj.SearchText = searchTxt;
    searchObj.SearchIn = selectedSearchIn;
    searchObj.PageNumber = nextPgnoSearch;
    searchObj.SortOrder = sortBy;
    searchObj.IsSortDes = isSortDes;

    return searchObj;
}

function GetAreasData(selectedPageNumber) {
    debugger;
    if (isSearchActive == false) {
        ajaxBaseCall("GET", AreasIndexURL,
            { 'sortOrder': sortedBy, 'isSortDes': isSortDes, 'pageNumber': nextPgno, 'isAjax': true }
        );
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        if (searchText != null) {
            nextPgnoSearch = selectedPageNumber;
            var searchObj = SearchObjCreationForAreas(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes);
            ajaxBaseCall("POST", AreasSearchURL, { 'searchModel': searchObj });
        }
    }
}



function LoadAreaGrid(response) {
    $("#grid_table tbody tr").remove();
    $.each(response.data, function (i, item) {
        var color = i % 2 == 0 ? 'gray' : "white";
        var rows = "<tr style='color: " + color + "'>"
            + "<td>" + parseInt(i + 1) + "</td>"
            + "<td><input id =" + item.areaID + " type='checkbox' class='chkboxes' /></td > "
            + "<td>" + item.areaName + "</td>"
            + "<td>" + item.assignedDriverName + "</td>"
            + "<td>" + item.noOfCusotomers + "</td>"
            + "<td><input  " + (item.isActive ? "checked" : "") + "  type='checkbox' class='chkboxes' /></td > "
            + " <td style='width: 9%;' class='action-center action-td'>" +
            `<a href='/Areas/Details/${item.areaID}' class='edit'><i class='glyphicon glyphicon-edit'></i></a> ` +
            `<a  href='/Areas/Edit/${item.areaID}' class='edit'><i class='glyphicon glyphicon-pencil'></i></a>` +
            `<a href='/Areas/Delete/${item.areaID}' class='delete'><i class='glyphicon glyphicon-trash'></i></a></td>` +
            "</tr>";
        $('#grid_table tbody').append(rows);
    });
    ButtonsOperations(response);
}