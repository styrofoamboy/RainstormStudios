
if (typeof ($) === "undefined")
    alert('The ToolBar control requires JQuery.  Please include a JQuery reference on your page before using this control.');

function initToolbar(clientID, actClsNm) {
    var toolbar = $("[id='" + clientID + "']");
    if (toolbar.length > 0) {
        toolbar.children("span.tbItemCont").hover(
            function () {
                $(this).addClass(actClsNm);
            },
            function () {
                $(this).removeClass(actClsNm);
            }
        );
    } else {
        alert('Unable to locate ToolBar control on page: ' + clientID);
    }
}