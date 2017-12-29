$(document).mouseup(function (e) {
    var container = $(".list-container");
    if (!container.is(e.target) && container.has(e.target).length === 0) {
        $("ul.export-container").removeClass("active");
    }
});