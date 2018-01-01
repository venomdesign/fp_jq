function url2json(url) {
    var obj = {};
    function arr_vals(arr) {
        if (arr.indexOf(',') > 1) {
            var vals = arr.slice(1, -1).split(',');
            var arr = [];
            for (var i = 0; i < vals.length; i++)
                arr[i] = vals[i];
            return arr;
        }
        else
            return arr.slice(1, -1);
    }
    function eval_var(avar) {
        if (!avar[1])
            obj[avar[0]] = '';
        else
            if (avar[1].indexOf('[') == 0)
                obj[avar[0]] = arr_vals(avar[1]);
            else
                obj[avar[0]] = avar[1];
    }
    if (url.indexOf('?') > -1) {
        var params = url.split('?')[1];
        if (params.indexOf('&') > 2) {
            var vars = params.split('&');
            for (var i in vars)
                eval_var(vars[i].split('='));
        }
        else
            eval_var(params.split('='));
    }
    return obj;
}

function trimInputs() {
    $("input").each(function () {
        $(this).val($(this).val().trim());
    });

    $("textarea").each(function () {
        $(this).val($(this).val().trim());
    });
}

$(function () {
    $("input").change(function () {
        $(this).val($(this).val().trim());
    });

    $("textarea").change(function () {
        $(this).val($(this).val().trim());
    })
});
