function ShiMessage() { }

$(document).ready(function () {
    ShiMessage.MessageCookies("ERRORGLOBAL", "ERROR")
    ShiMessage.MessageCookies("WARNINGMESSAGE", "WARNING")
    ShiMessage.MessageCookies("SUCCESSMESSAGE", "SUCCESS")
})

ShiMessage.warning = function(title, message){
    toastr.warning(title, message)
}
ShiMessage.error = function(title, message){
    Sys.ErrorShow(title,message)
}
ShiMessage.success = function(title, message){
    toastr.success(title, message)
}
ShiMessage.info = function(title, message){
    $(".callout").remove();
    $("#divInfoContent").append("<div class=\"callout callout-info\"><h4>{0}</h4><p>{1}</p></div>"._format(title, message));
    setTimeout(function (){
        $("#divInfoContent").hide('slow');
    }, 60000);
}
ShiMessage.MessageCookies = function (cookiesName, type) {
    if (getCookie(cookiesName) != "") {
        if (type.toUpperCase() == "SUCCESS") {
            ShiMessage.success("Success Message", getCookie(cookiesName))
            delete_cookie(cookiesName)
        }
        if (type.toUpperCase() == "WARNING") {
            ShiMessage.warning("Warning Message", getCookie(cookiesName))
            delete_cookie(cookiesName)
        }
        if (type.toUpperCase() == "ERROR") {
            ShiMessage.error("Error Message", getCookie(cookiesName))
            delete_cookie(cookiesName)
        }
        if (type.toUpperCase() == "INFO") {
            ShiMessage.info("Info Message", getCookie(cookiesName))
            delete_cookie(cookiesName)
        }
    }
}
function delete_cookie(Name) {
    $.ajax({
        type: "POST",
        url: VMSGlobal.RemoveCookies,
        data: { Name: Name },
    success: function (data) {

    },
    error: function () {
        toastr.error('Please Contact your administrator!', 'Error!')
    }
})
}
function getCookie(cname) {
var name = cname + "=";
var decodedCookie = decodeURIComponent(document.cookie);
var ca = decodedCookie.split(';');
for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == ' ') {
        c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
    }
}
return "";
}