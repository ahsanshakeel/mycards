
$(document).ready(function () {
    try {
        $("input[type='text']").each(function () {
            $(this).attr("autocomplete", "off");
        });
    }
    catch (e) { }
});
