function Sys() { }

Sys.mRoot = "";

Sys.AjaxError = function (xhr, error, text, callback) {
    console.log(xhr);
    console.log(error);
    console.log(text);

    if (text == "") {
        text = "No data return.";
    }

    Sys.ErrorShow("Invalid Data Return", text);
    if (!Sys.IsNull(callback)) {
        callback();
    }
}

Sys.AjaxSuccess = function (j, callback) {
    if (!Sys.IsNull(j["redirect"])) {
        window.location = j["redirect"];
        return;
    }
    if (j["result"] === true) {
        if (!Sys.IsNull(callback)) {
            callback();
        }
    }
    else {
        console.log(j["trace"]);
        if (Sys.IsNull(j["result"])) {
            Sys.ErrorShow("Error Occurs", "Cannot get result status from returned data!");
        }
        else {
            Sys.ErrorShow("Error Occurs", j["msg"]);
        }
    }
}

Sys.Click_NavLogout = function (e) {
    e.preventDefault();
    window.location = Sys.mRoot + "User/Logout.aspx";
}

Sys.DialogClose = function (modaledit) {
    $(".modalShi").modal("hide");
    if (modaledit == true) {
        $(".modal").modal("hide");
    }
}

Sys.DialogShow = function (options) {
    var title = Sys.IfNull(options["title"], "");
    var content = Sys.IfNull(options["content"], "");
    var buttons = Sys.IfNull(options["buttons"], []);
    var modalSize = Sys.IfNull(options["size"], "");

    var htmlBtn = "";

    buttons._loop(function (i, item) {
        var dismiss = Sys.IfNull(item["dismiss"], false);
        var dismissAct = "";
        if (dismiss) {
            dismissAct = " data-dismiss=\"modal\"";
        }
        htmlBtn += "<button type=\"button\" class=\"btn {1}\" id=\"dialogBtn_{2}\"{3}>{0}</button>"._format(item["text"], item["class"], i, dismissAct);
    });

    $(".modalShi").remove();
    $(".modal-backdrop").remove();

    $("body").append("<div class=\"modal modalShi\"tabindex=\"-1\" role=\"dialog\"><div class=\"modal-dialog modal-dialog-centered {3}\" role=\"document\"><div class=\"modal-content\"><div class=\"modal-header\"><h5 class=\"modal-title\">{0}</h5></div><div class=\"modal-body\">{1}</div><div class=\"modal-footer\">{2}</div></div></div></div>"._format(title, content, htmlBtn, modalSize));

    buttons._loop(function (i, item) {
        if (!Sys.IsNull(item["click"])) {
            $("#dialogBtn_{0}"._format(i)).click(item["click"]);
        }
    });

    $(".modalShi").modal({
        backdrop: "static",
        keyboard: false,
        show: true
    });
}

Sys.DrawFormDatePicker = function (name, label, value) {
    return "<div class=\"form-group\"><label for=\"tb{0}\">{1}</label><input type=\"text\" class=\"form-control is-datepicker\" id=\"tb{0}\" value=\"{2}\"><small class=\"help form-text text-danger\" id=\"help{0}\"></small></div>"._format(name, label, value);
}

Sys.DrawFormFile = function (name, label, filter) {
    return "<div class=\"form-group\">"
        + "<label>{1}</label>"._format(name, label)
        + "<div class=\"custom-file\">"
        + "<input type=\"file\" class=\"custom-file-input\" id=\"file{0}\" name=\"{0}\" accept=\"{1}\">"._format(name, filter)
        + "<label class=\"custom-file-label\" for=\"file{0}\">Choose file</label>"._format(name)
        + "</div></div>";
}

Sys.DrawFormSelection = function (name, label, selection, value) {
    var optionHtml = "";

    selection._loop(function (i, item) {
        var selected = (item["Value"] == value) ? " selected" : "";
        optionHtml += "<option value=\"{0}\"{2}>{1}</option>"._format(item["Value"], item["Label"], selected);
    });

    return "<div class=\"form-group\">"
        + "<label for=\"sel{0}\">{1}</label>"._format(name, label)
        + "<select class=\"custom-select tb-filter\" id=\"sel{0}\">{1}</select>"._format(name, optionHtml)
        + "<small class=\"help form-text text-danger\" id=\"help{0}\"></small>"._format(name)
        + "</div>";
}

Sys.DrawFormTextbox = function (name, label, placeHolder, length, value) {
    return "<div class=\"form-group\"><label for=\"tb{0}\">{1}</label><input type=\"text\" class=\"form-control\" id=\"tb{0}\" placeholder=\"{2}\" maxlength=\"{3}\" value=\"{4}\"><small class=\"help form-text text-danger\" id=\"help{0}\"></small></div>"._format(name, label, placeHolder, length, value);
}

Sys.DrawFormTextbox2 = function (name, label, placeHolder, length, prefix, value) {
    return "<div class=\"form-group\"><label for=\"tb{0}\">{1}</label><div class=\"input-group\"><div class=\"input-group-prepend\"><span class=\"input-group-text\">{5}</span></div><input type=\"text\" class=\"form-control\" id=\"tb{0}\" placeholder=\"{2}\" maxlength=\"{3}\" value=\"{4}\"></div><small class=\"help form-text text-danger\" id=\"help{0}\"></small></div>"._format(name, label, placeHolder, length, value, prefix);
}

//Added by Jemmy on 2019-03-17
Sys.DrawFormTextArea = function (name, label, placeHolder, row, length, value) {
    return "<div class=\"form-group\"><label for=\"tb{0}\">{1}</label><textarea rows=\"{3}\" class=\"form-control\" id=\"tb{0}\" placeholder=\"{2}\" maxlength=\"{4}\">\{5}\</textarea><small class=\"help form-text text-danger\" id=\"help{0}\"></small></div>"._format(name, label, placeHolder, row, length, value);
}
//end 

//Added by Jemmy on 2019-04-17 
Sys.DrawFormCheckBox = function (name, label, value) {
    var addstr = ""
    if (value == true) {
        addstr = "checked = 'checked'"
    }
    return "<div class='form-check'><input type='checkbox' " + addstr + " class='form-check-input' id=\"{0}\"><label class='form-check-label' for=\"{1}\">\{1}\</label></div>"._format(name, label, value);
}

////Added by Jemmy on 2019-03-17
//Sys.DrawTable = function (dt) {
//    var optionHtml = "<table class='table table-sm table-striped table-bordered'><thead><tr><th>No</th><th>Device Name</th></tr></thead><tbody><tr>";

//    dt._loop(function (i, item) {
//        optionHtml += "<tr><td>"+item["Label"]+"</td></tr>";
//    });
//    optionHtml += "</tbody></table>"
//    return optionHtml;
//}

////end
Sys.ErrorShow = function (title, msg) {
    Sys.DialogShow({
        title: "<span class=\"text-danger\">{0}</span>"._format(title),
        content: msg,
        buttons: [
            { text: "Close", class: "btn-secondary", dismiss: true }
        ]
    });
}

Sys.GetMealGrpName = function (grpValue, prefix) {
    if (grpValue === 5) {
        return "Management";
    }
    else if (grpValue === 6) {
        return "HT";
    }
    return prefix + grpValue;
};

Sys.IfNull = function (obj, ret) {
    if (Sys.IsNull(obj)) {
        return ret;
    }
    else {
        return obj;
    }
}

Sys.IsNull = function (obj) {
    return (obj === null || obj === undefined);
}

Sys.Init = function () {
    $(function () {
        $("#navLogout").click(Sys.Click_NavLogout);
    });
}

Sys.InitDatePicker = function (selector) {
    var jqDp = $(selector)
    jqDp.datepicker({
        autoclose: true,
        format: "yyyy/mm/dd",
        orientation: "auto",
        todayBtn: "linked",
        todayHighlight: true,
    });
    jqDp.datepicker("update", new Date());
};

Sys.LoadingShow = function () {
    Sys.DialogShow({
        content: "Loading.....<br/><div class='lds-ring'><div></div><div></div><div></div><div></div></div>"
    });
}

Sys.RefreshDataTableFilter = function (data) {
    var filter = {};

    data._loop(function (i, row) {
        var keys = Object.keys(row);

        keys._loop(function (i, key) {
            if (filter[key] == undefined) {
                filter[key] = [];
            }

            var i = 0;
            var value = row[key];

            if (Sys.IsNull(value) || value === "") {
                return true;
            }

            var find = false;
            for (i = 0; i < filter[key].length; i++) {
                if (filter[key][i] === value) {
                    find = true;
                    break;
                }
            }

            if (!find) {
                filter[key].push(value);
            }
        });
    });

    var keys = Object.keys(filter);

    keys._loop(function (i, key) {
        var sel = $("#tbFilter-{0}"._format(key));
        if (sel.length > 0) {
            filter[key] = filter[key].sort();

            var boolStrip = sel.attr("bool");

            var html = "<option value=\"\"></option>";

            if (!Sys.IsNull(boolStrip) && boolStrip !== "") {
                var lbl = boolStrip.split(";");
                filter[key]._loop(function (i, filterVal) {
                    html += "<option value=\"{0}\">{1}</option>"._format(filterVal, lbl[(filterVal) ? 1 : 0]);
                });
            }
            else {
                filter[key]._loop(function (i, filterVal) {
                    html += "<option value=\"{0}\">{1}</option>"._format(filterVal, filterVal);
                });
            }

            var current = sel.val();

            sel.html(html);
            sel.val(current);
        }
    });
}

Sys.Init();


///
function DP() { }
DP.AJaxAll = function (data, Url, callback) {
    Sys.LoadingShow()
    $.ajax({
        url: Url,
        cache: false,
        type: "POST",
        data: data,
        success: function (data) {
            Sys.DialogClose();
            callback(data)
        },
        error: function (jqXHR, exception) {
            Sys.DialogClose();
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
            }
            Sys.ErrorShow("Error From Ajax", msg)
            return null;
        }
    })
}
DP.Ajax = function (data, Url, callback) {
    Sys.LoadingShow()
    $.ajax({
        url: Url,
        cache: false,
        type: "POST",
        data: data,
        success: function (data) {
            Sys.DialogClose();
            if (data.Success) {
                callback(data.data)
            }
            else {
                Sys.ErrorShow("Error From Backend", data.Message)
                return null;
            }
        },
        error: function (jqXHR, exception) {
            Sys.DialogClose();
            var msg = '';
            if (jqXHR.status === 0) {
                msg = 'Not connect.\n Verify Network.';
            } else if (jqXHR.status == 404) {
                msg = 'Requested page not found. [404]';
            } else if (jqXHR.status == 500) {
                msg = 'Internal Server Error [500].';
            } else if (exception === 'parsererror') {
                msg = 'Requested JSON parse failed.';
            } else if (exception === 'timeout') {
                msg = 'Time out error.';
            } else if (exception === 'abort') {
                msg = 'Ajax request aborted.';
            } else {
                msg = 'Uncaught Error.\n' + jqXHR.responseText;
            }
            Sys.ErrorShow("Error From Ajax", msg)
            return null;
        }
    })
}


//Add By Dedy 2019-07-16
//Sko = Shimano Kendo Object
//=============================================================================
function Sko() {

}
Sko.KendoGrid = function (data, idGrid) {
    var column = Sys.IfNull(data["column"], null);
    var datasource = Sys.IfNull(data["datasource"], null);
    $("#" + idGrid).kendoGrid({
        dataSource: {
            data: datasource,
            pageSize: 20
        },
        column: column,
        height: 550,
        sortable: true,
        reorderable: true,
        groupable: true,
        resizable: true,
        filterable: {
            mode: "row"
        },
        columnMenu: true,
        pageable: true,
    });
}


function blinker() {
    $('.blinking').fadeOut(500);
    $('.blinking').fadeIn(1500);
}
setInterval(blinker, 2000);