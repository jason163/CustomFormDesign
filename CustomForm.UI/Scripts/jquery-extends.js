/**
 * jQuery bindEntity
 * */
(function ($) {
    // form: Jquery Object
    function buildEntity(form) {
        var obj = {};
        form.find("[title]").each(function () {
            var name = $(this).attr("title");
            var names = name.split(".");
            var temp;
            var val;
            if ($(this).attr("type") == "checkbox"
                || $(this).attr("type") == "radio") {
                if ($(this).prop("checked")) {
                    val = $(this).val();
                }
            } else {
                val = $.trim($(this).val());
            }
            if (names.length > 1) {
                for (var i = 0; i < names.length; i++) {
                    if (temp) {
                        if (!temp[names[i]]) {
                            if (i == (names.length - 1)) temp[names[i]] = val;
                            else temp[names[i]] = {}
                        }
                        temp = temp[names[i]];
                    } else {
                        if (!obj[names[i]]) {
                            if (i == (names.length - 1)) obj[names[i]] = val;
                            else obj[names[i]] = {}
                        }
                        temp = obj[names[i]]
                    }
                }
            } else {
                obj[name] = val;
            }
        });
        return obj;
    }

    $.buildEntity = buildEntity;
})(jQuery);