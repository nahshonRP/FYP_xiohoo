//Global Variables:
let isSearchActive = false;
let isDateRangeActive = false;
let nextPgno = 1;
let nextPgnoSearch = 1;
let sortedBy = "";
let acrCatId = 1;
let isSortClick = false;
let isSortDes = false;
let isUploadInProgress = false;
let toDate = "";
let fromDate = "";
//URLs:
const SavedReportSearchURL = '/SavedReports/Search';
const SavedReportIndexURL = '/SavedReports/Index';
const SavedReportDeleteURL = '/SavedReports/DeleteRecords';
const SavedReportSearchByDates = '/SavedReports/GetRecordsBySelectedDates';








const FingerPrintSearchURL = '/Fingerprint/Search';
const FingerPrintIndexURL = '/Fingerprint/Index';
const FingerPrintDeleteURL = '/Fingerprint/DeleteRecords';
const FingerPrintRetryURL = '/Fingerprint/Retry';
const FingerPrintRecognizeAudioURL = '/Fingerprint/RecognizeAudio';
const UploadToSoundmouseURL = '/Fingerprint/UploadToSoundmouse';

//Messages:
const ErrorMessage = 'Something went wrong while fetching the data';
const SearchTxtNullMsg = 'Nothing to Search';
const DeleteNullMsg = 'Please select records to delete';
const DeleteConfirmationMsg = 'Do you want to delete the records?';
const ConfirmUploadToSoundmouseMsg = 'Do you want to upload selected work to Soundmouse?';
const ConfirmUploadToSoundmouseMsgSuccessMsg = 'Uploading to soundmouse has started as a backgroud task';
const ConfirmAudioRecognitionMsg = 'Do you want to process selected work through audio recognition?';
const SeletRowToProcessMsg = 'Please select any row to process.';
const ConfirmAudioRecognitionForAllMsg = 'Do you want to process all works through audio recognition?';
const ConfirmAudioRecognitionForAllSuccessMsg = 'Audio recognition has started processing works as a backgroud task';
const NoFilesToProcess = "No songs selected for uploading";
const CloseWindow = "Are you sure you want to leave the page? Data will be lost";
const SelectFromDateMsg = "Please select from date";
const SelectToDateMsg = "Please select to date";
const FromDateGreater = "From date is greater than To date";

function ajaxBaseCall(type, url, dataParams, callback) {
    $.ajax({
        type: type,
        url: url,
        quietMillis: 100,
        data: dataParams,
        cache: true,
        beforeSend: function () {
            ShowLoader();
        },
        success: function (response) {
            //TODO: Better not to create specific behaviours in general method, rather shift this responsiblity to the one who is calling this method by providing desired behaviour as a callback.
            //Hence, Let's remove these conditional behavious alogn with this extra param 'isFingerPrintResult' from controller as well.
            if (response.isUsersResult) {
                LoadUsersGrid(response);
            } else if (response.isAreaResult) {
                LoadAreaGrid(response);
            } else if (response.isTodaysDeliveryResult) {
                LoadTodaysDeliveriesGrid(response);
            } else if (response.isDeliveryOrderResult) {
                LoadDeliveryOrderGrid(response);
            }

            if (typeof callback == 'function')
                callback();
            if (response.pagingHtml != "" && response.pagingHtml != null)
                LoadPaging(response);
        },
        error: function () {
            alert(ErrorMessage);
        },
        complete: function () {
            isSortClick = false;
            HideLoader();
            isUploadInProgress = false;
        }
    });
}

function SearchObjCreation(searchTxt, selectedSearchIn, nextPgnoSearch, sortBy = 0, isSortDes = false, selectedRole = "DeliveryCustomer") {
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
$("#btnNextFirst").click(OnNextBtnClick);
$("#btnNextSecond").click(OnNextBtnClick);
$("#btnPreviousFirst").click(OnPreviousBtnClick);
$("#btnPreviousSecond").click(OnPreviousBtnClick);
function OnNextBtnClick(ele) {
    debugger;
    var selectedPage = $('#gridPagination').attr("value")
    nextPgno++;
    nextPgnoSearch++;
    var selectedPageNumber = nextPgnoSearch;
    debugger;
    //todaysDeliveries
    if (selectedPage == 'users') {
        GetUsersData(selectedPageNumber);
    }
    else if (selectedPage == 'area') {
        GetAreasData(selectedPageNumber);
    }
    else if (selectedPage == 'todaysDeliveries') {
        GetTodaysDeliveriesData(selectedPageNumber);
    }


}

function OnPreviousBtnClick(ele) {
    // const searchValue = $(ele.target).val();
    debugger;
    var selectedPage = $('#gridPagination').attr("value")
    if (nextPgno == 1)
        return;

    --nextPgno;
    --nextPgnoSearch;
    var selectedPageNumber = nextPgnoSearch;
    debugger;
    //todaysDeliveries
    if (selectedPage == 'users') {
        GetUsersData(selectedPageNumber);
    }
    else if (selectedPage == 'area') {
        GetAreasData(selectedPageNumber);
    }
    else if (selectedPage == 'todaysDeliveries') {
        GetTodaysDeliveriesData(selectedPageNumber);
    }
}



$("#searchBtn").click(function (ele) {
    const searchText = $("#search_txt").val();
    var selectedRole = $('#ddlRoles').val();
    if (searchText == "")
        return alert(SearchTxtNullMsg);

    nextPgno = 1;
    nextPgnoSearch = 1;

    const selectedSearchIn = $("#SearchIn").val();
    var searchObj = SearchObjCreation(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes, selectedRole);
    isSearchActive = true;

    const searchValue = $(ele.target).val();
    let url = UsersSearchURL;// searchValue != "isFingerPrint" ? SavedReportSearchURL : FingerPrintSearchURL;
    ajaxBaseCall("POST", url, { 'searchModel': searchObj });
});

//$("#btnNextFirst").click(OnNextBtnClick);
//$("#btnNextSecond").click(OnNextBtnClick);



$(document).on('click', '.navigateToPage', function () {
    var selectedPageNumber = 0;
    var selectedPage = $(this).parent("ul").attr("value")
    var oldvalue = $('.navigateToPage').val($(this).text())[0].value;
    var newvalue = $(this).text();
    if (oldvalue != newvalue)
        selectedPageNumber = oldvalue;
    else
        selectedPageNumber = newvalue;

    nextPgno = selectedPageNumber;
    debugger;
    //todaysDeliveries
    if (selectedPage == 'users') {
        GetUsersData(selectedPageNumber);
    }
    else if (selectedPage == 'area') {
        GetAreasData(selectedPageNumber);
    }
    else if (selectedPage == 'todaysDeliveries') {
        GetTodaysDeliveriesData(selectedPageNumber);
    }




    //if (isSearchActive == false) {
    //    ajaxBaseCall("GET", requestUrl,
    //        { 'sortOrder': sortedBy, 'isSortDes': isSortDes, 'pageNumber': nextPgno, 'isAjax': true, 'selectedRole': selectedRole }
    //    );
    //} else {
    //    const searchText = $("#search_txt").val();
    //    const selectedSearchIn = $("#SearchIn").val();
    //    if (searchText != null) {
    //        nextPgnoSearch = selectedPageNumber;
    //        var searchObj = SearchObjCreation(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes, selectedRole);
    //        ajaxBaseCall("POST", requestUrl, { 'searchModel': searchObj });
    //    }
    //}
});

function LoadSavedReportGrid(response) {
    $("#grid_table tbody tr").remove();
    $.each(response.data, function (i, item) {
        var rows = "<tr>"
            + "<td><input id =" + item.id + " type='checkbox' value = " + item.id + ',' + item.audioEvidence + " class='chkboxes' /></td>"
            + "<td>" + item.id + "</td>"
            + "<td>" + item.channelName + "</td>"
            + "<td>" + item.channelType + "</td>"
            + "<td>" + item.programTitle + "</td>"
            + "<td>" + item.timestampUtcString + "</td>"
            + "<td>" + item.timestampUtcShort + "</td>"
            + "<td>" + item.audioId + "</td>"
            + "<td>" + item.title + "</td>"
            + "<td>" + item.performerName + "</td>"
            + "<td>" + item.playDuration + "</td>"
            + "<td><a href=" + item.originalSongURL + " target='_blank'>" + item.originalSongText + "</a></td>"
            + "<td><a href=" + item.audioEvidence + " target='_blank'>" + item.audioEvidenceText + "</a></td>"
            + "</tr>";
        $('#grid_table tbody').append(rows);
    });
    ButtonsOperations(response);
}

function LoadPaging(response) {
    $("#gridPagination").remove();
    var paginationDiv = $(response.pagingHtml);
    $('#paginationDiv').append(paginationDiv);
}

//<th>#</th>
//    <th>Customer ID</th>
//    <th>Name</th>
//    <th>Phone No</th>
//    <th>Address</th>
//    <th>Delivery Freq</th>
//    <th>Bottle Rate</th>
//    <th>Actions</th>








function ButtonsOperations(response) {
    //var isChecked = $('#checkall').is(':checked');
    //if (isChecked) {
    //    $("#checkall").prop('checked', false);
    //}
    //if (response.isNextBtnAvailable == true) {
    //    $("#btnNextFirst").attr("disabled", false);
    //    $("#btnNextSecond").attr("disabled", false);
    //} else {
    //    $("#btnNextFirst").attr("disabled", true);
    //    $("#btnNextSecond").attr("disabled", true);
    //}

    //if (response.isPreviousBtnAvailable == true) {
    //    $("#btnPreviousFirst").attr("disabled", false);
    //    $("#btnPreviousSecond").attr("disabled", false);
    //} else {
    //    $("#btnPreviousFirst").attr("disabled", true);
    //    $("#btnPreviousSecond").attr("disabled", true);
    //}
}

function ToggleChecked() {
    var isChecked = $('#checkall').is(':checked');
    $('input[type=checkbox]', "#grid_table").each(function () {
        if (isChecked) {
            $(this).prop('checked', true);
        }
        else {
            $(this).prop('checked', false);
        }
    })
}

function DeleteConfirmation(isFingerPrintDelete = false) {
    if (IsAnySelectedRow()) {
        if (!isFingerPrintDelete) {
            const confirmation = confirm(DeleteConfirmationMsg);
            if (confirmation == true) {
                DeleteRecords(isFingerPrintDelete, null);
            }
        } else {
            Swal.fire({
                title: DeleteConfirmationMsg,
                icon: "warning",
                showConfirmButton: false,
                showCloseButton: true,
                html: ` <p id='popupError'>Select an Option</p>
                        <div class="input-group date" style='padding:10px;'>
                           <input placeholder='dd-mm-yy'  id="datetimepicker" class="form-control" autofocus>
                        </div>
                        <div style='padding:5px;'>
                        <button id='btnDeleteAll' class="btn btn-primary" style='margin:5px;' onclick="onBtnClicked('all',${isFingerPrintDelete})">Delete All</button>
                        <button id='btnDeleteAfterdate' class="btn btn-danger" style='margin:5px;' onclick="onBtnClicked('afterDate',${isFingerPrintDelete})">Delete After Date</button>
                        <button id='btnCanceDelete' class="btn btn-secondary" style='margin:5px;' onclick="onBtnClicked('cancel',${isFingerPrintDelete})">Cancel</button>
                        </div>`,
                didOpen: function () {
                    $('#datetimepicker').datepicker({
                        dateFormat: 'dd-mm-yy',
                        showOn: 'button',
                        buttonImage: '/img/calender.png',
                        buttonImageOnly: true
                    });
                }
            });
        }
    }
    else
        return alert(DeleteNullMsg);
}
function onBtnClicked(btnselected, isFingerPrintDelete) {
    if (btnselected == 'afterDate') {
        var selectedDate = $('#datetimepicker').val();
        if (selectedDate == '' || selectedDate == undefined) {
            $('#popupError').text('Please select a date first');
            $('#popupError').addClass('error');
            return;
        } else {
            var formateddate = selectedDate.split("-").reverse().join("-");
            if (new Date() < new Date(formateddate)) {
                $('#popupError').text('Please select a date in the Past');
                $('#popupError').addClass('error');
                return;
            }
        }
    }

    Swal.close();
    if (btnselected != 'cancel')
        DeleteRecords(isFingerPrintDelete, btnselected);
};
function GetFingerprintIdsFromSelectedRows() {
    let fingerprintIds = [];
    $('.chkboxes').each(function () {
        if (this.checked) {
            let indexIdVal = 0;
            let checkedBoxVals = $(this).val().split(',');
            fingerprintIds.push(checkedBoxVals[indexIdVal]);
        }
    });
    return fingerprintIds;
}

function IsAnySelectedRow() {
    let isCheckBoxSelected = false;
    $('.chkboxes').each(function () {
        if (this.checked) {
            isCheckBoxSelected = true;
        }
    });
    return isCheckBoxSelected;
}

function OnUploadToSoundmouseBtnClick() {
    if (IsAnySelectedRow()) {
        const isOk = confirm(ConfirmUploadToSoundmouseMsg);
        if (isOk) {
            let fpIds = GetFingerprintIdsFromSelectedRows();
            let searchObj = GetSearchObj();
            let reqParamObj = { 'fingerPrintIds': fpIds, 'searchModel': searchObj, 'isSearch': isSearchActive };
            ajaxBaseCall("PUT", UploadToSoundmouseURL, reqParamObj, () => alert(ConfirmUploadToSoundmouseMsgSuccessMsg));
        }
    }
    else
        return alert(SeletRowToProcessMsg);
}

function OnRcognizeAudioBtnClick() {
    if (IsAnySelectedRow()) {
        const isOk = confirm(ConfirmAudioRecognitionMsg);
        if (isOk) {
            let fpIds = GetFingerprintIdsFromSelectedRows();
            let searchObj = GetSearchObj();
            let reqParamObj = { 'fingerPrintIds': fpIds, 'searchModel': searchObj, 'isSearch': isSearchActive };
            ajaxBaseCall("PUT", FingerPrintRecognizeAudioURL, reqParamObj, () => alert(ConfirmAudioRecognitionForAllSuccessMsg));
        }
    }
    else
        return alert(SeletRowToProcessMsg);
}

function OnRcognizeAudioForAllBtnClick() {
    const isOk = confirm(ConfirmAudioRecognitionForAllMsg);
    if (isOk) {
        let searchObj = GetSearchObj();
        let reqParamObj = { 'fingerPrintIds': null, 'searchModel': searchObj, 'isSearch': isSearchActive, 'processAll': true };
        ajaxBaseCall("PUT", FingerPrintRecognizeAudioURL, reqParamObj, () => alert(ConfirmAudioRecognitionForAllSuccessMsg));
    }
}

function DeleteRecords(isFingerPrintDelete, btnselected) {
    var deleteDataCollection = []
    var selectedDateForDelete = null;
    if (isFingerPrintDelete) {
        var selectedDateValue = $('#datetimepicker').val();
        if (selectedDateValue != undefined && selectedDateValue.length > 7 && btnselected == 'afterDate')
            selectedDateForDelete = selectedDateValue.split("-").reverse().join("-");
    }

    $('.chkboxes').each(function () {
        if (this.checked) {
            var array = $(this).val().split(',');
            var obj = {};
            if (array.length >= 2) {
                obj.Id = array[0];
                obj.Audio = array[1];
                obj.AcrId = array[2];
                if (isFingerPrintDelete) {
                    obj.TrackId = array[3];
                    obj.FingerPrint = array[4];
                    obj.OriginalSong = array[5];

                }
            } else {
                obj.Id = array[0];
                obj.AcrId = array[2];
                obj.Audio = null;
                if (isFingerPrintDelete) {
                    obj.TrackId = array[3];
                    obj.FingerPrint = array[4];
                    obj.OriginalSong = array[5];

                }
            }
            deleteDataCollection.push(obj);
        }
    });

    let searchObj = GetSearchObj();
    let url = isFingerPrintDelete ? FingerPrintDeleteURL : SavedReportDeleteURL;
    let reqParamObj = { 'deleteModel': deleteDataCollection, 'searchModel': searchObj, 'isSearch': isSearchActive, 'selectedDateForDelete': selectedDateForDelete };
    ajaxBaseCall("DELETE", url, reqParamObj);
}


function OnRetryBtnClick(id) {
    let searchObj = GetSearchObj();
    ajaxBaseCall("POST", FingerPrintRetryURL, { 'fingerPrintId': id, 'searchModel': searchObj, isSearch: isSearchActive });
}

function OnDeleteBtnClick(id) {
    let searchObj = GetSearchObj();
    var deleteDataCollection = [];
    var obj = {
        Id: id,
    };
    deleteDataCollection.push(obj);
    ajaxBaseCall("DELETE", FingerPrintDeleteURL, { 'deleteModel': deleteDataCollection, 'searchModel': searchObj, 'isSearch': isSearchActive });
}

function GetSearchObj() {
    var searchObj = {};
    if (isSearchActive) {
        searchObj = SearchObjCreation($("#search_txt").val(), $("#SearchIn").val(), nextPgnoSearch, sortedBy, isSortDes);
    }
    else {
        searchObj.PageNumber = nextPgno;
        searchObj.SortOrder = sortedBy;
        searchObj.IsSortDes = isSortDes;
        searchObj.AcrCatalogueId = $('#acrCatDr').val();
    }

    return searchObj;
}


function CatalogueDropdownSelected() {
    isSortClick = false;
    isSortDes = !isSortDes;
    acrCatId = $('#acrCatDr').val();
    if (isSearchActive == false) {
        ajaxBaseCall("GET", UsersIndexURL, { 'sortOrder': 1, 'isSortDes': isSortDes, 'isSortClick': isSortClick, 'pageNumber': nextPgno, 'isAjax': true, 'selectedRole': value });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreation(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes);
        ajaxBaseCall("POST", UsersSearchURL, { 'searchModel': searchObj });
    }
}

function CatalogueDropdownSelectedfromSaveReport() {
    isSortClick = false;
    isSortDes = !isSortDes;
    acrCatId = $('#acrCatDr').val();
    if (isSearchActive == false) {
        ajaxBaseCall("GET", SavedReportIndexURL, { 'isAjax': true });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreation(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes);
        ajaxBaseCall("POST", SavedReportSearchURL, { 'searchModel': searchObj });
    }
}

function SortClick(sortBy) {
    sortedBy = sortBy;
    isSortClick = true;
    isSortDes = !isSortDes;
    acrCatId = $('#acrCatDr').val();
    if (isSearchActive == false) {
        ajaxBaseCall("GET", UsersIndexURL, { 'sortOrder': sortBy, 'isSortDes': isSortDes, 'isSortClick': isSortClick, 'pageNumber': nextPgno, 'isAjax': true });
    } else {
        const searchText = $("#search_txt").val();
        const selectedSearchIn = $("#SearchIn").val();
        var searchObj = SearchObjCreation(searchText, selectedSearchIn, nextPgnoSearch, sortedBy, isSortDes);
        ajaxBaseCall("POST", FingerPrintSearchURL, { 'searchModel': searchObj });
    }
}

$(document).ready(function () {
    $("form#FileUploadForm").submit(function (event) {
        var count = 0;
        $('#fileErrorSpan').css('display', 'none');
        $('#textBoxErrorSpan').css('display', 'none');
        for (var i = 0; i < 10; i++) {
            var fileText = $(`input[id^="textInput[${i}]"]`).val();
            var fileUpload = $(`input[id^="fileUpload[${i}]"]`).val();
            if (fileText === "" && fileUpload !== "") {
                event.preventDefault();
                $(`input[id^="textInput[${i}]"]`).css({ 'border': 'solid 2px #FF0000' });
                $('#textBoxErrorSpan').css('display', 'block');
                HideLoader();
            }
            if (fileText !== "" && fileUpload === "") {
                event.preventDefault();
                $(`input[id^="fileUpload[${i}]"]`).css({ 'border': 'solid 2px #FF0000' });
                $('#fileErrorSpan').css('display', 'block');
                HideLoader();
            }
            if (fileText === "" && fileUpload === "") {
                count++;
            }
        }
        if (count === 10) {
            event.preventDefault();
            HideLoader();
            return alert(NoFilesToProcess);
        }
        window.onbeforeunload = null;
    });
});

function UploadingInProgress() {
    isUploadInProgress = true;
    $(function () {
        function confirmclose() {
            return CloseWindow;
        }
        if (isUploadInProgress == true)
            window.onbeforeunload = confirmclose;
    });
}

function ShowLoader() {
    $('.ajax-loader').css("visibility", "visible");
}
function ValidDate() {
    var dateEle = $('.SelectedDate');
    if (dateEle)
        dateEle.attr('max', new Date().toISOString().split('T')[0]);
}

$(document).ready(function () {
    $("#search_txt").keypress(function (e) {
        let enterKeyCode = 13;
        let keycode = e.keyCode || e.charCode || e.which //for cross browser
        if (keycode == enterKeyCode) {
            $('#searchBtn').trigger('click');
        }
    });

    $('#fDate').val(new Date().toISOString().split('T')[0]);
    $('#tDate').val(new Date().toISOString().split('T')[0]);

});

function HideLoader() {
    $('.ajax-loader').css("visibility", "hidden");
}

function OnDisplayBtnClick() {
    toDate = $("input[name = toDate]").val();
    fromDate = $("input[name = fromDate]").val();
    if (fromDate > toDate) {
        alert(FromDateGreater);
        return;
    }

    if (toDate != "" && fromDate == "") {
        alert(SelectFromDateMsg);
        return;
    }
    if (toDate == "" && fromDate != "") {
        alert(SelectToDateMsg);
        return;
    }

    isSearchActive = true;
    isDateRangeActive = true;
    let displayBtn = $('#displayBtn');
    if (displayBtn)
        displayBtn.attr(isDateRangeActiveAttr, '');

    let obj = {
        'searchModel':
        {
            'FromDate': fromDate,
            'ToDate': toDate,
            'AcrCatalogueId': $('#acrCatDr').val()
        }
    }

    nextPgno = 1;
    nextPgnoSearch = 1;

    ajaxBaseCall("POST", SavedReportSearchByDates, obj);
}

