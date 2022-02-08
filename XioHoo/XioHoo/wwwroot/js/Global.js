const HttpVerbs = { POST: 'POST', GET:'GET' };
const AjaxResponseTypes = { Blob: 'blob' };

//TODO:after code segrigation this code will no longer remain
const isDateRangeActiveAttr = 'data-isDateRangeActive'

function Ajax(type, url, dataParams, successCallback, completeCallback, respType = "") {
    $.ajax({
        type: type,
        url: url,
        quietMillis: 100,
        data: dataParams,
        cache: true,
        beforeSend: function () {
            ShowLoader();
        },
        xhrFields: {
            responseType: respType
        },
        success: function (response) {
            if (typeof successCallback == 'function')
                successCallback(response);
        },
        error: function () {
            alert(ErrorMessage);
        },
        complete: function () {
            if (typeof completeCallback == 'function')
                completeCallback();
            else
                HideLoader();
        }
    });
}

function ShowLoader() {
    let ele = $('.ajax-loader');
    if (ele)
        ele.css("visibility", "visible");
}

function HideLoader() {
    let ele = $('.ajax-loader');
    if (ele)
        ele.css("visibility", "hidden");
}