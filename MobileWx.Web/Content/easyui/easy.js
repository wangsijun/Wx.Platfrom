
$.extend($.fn.validatebox.defaults.rules, {
    noblank: {
        validator: function (value, param) {
            return value.trim() != "";
        },
        message: "不能为空"
    },
    minLength: {
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: '最少需要{0}个字符'
    }
});

function encodeformatter(value, row, index) {
    var temp = document.createElement("div");
    (temp.textContent != null) ? (temp.textContent = value) : (temp.innerText = value);
    var output = temp.innerHTML;
    temp = null;
    return output;
}

$.fn.datebox.defaults.formatter = function (date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + "-" + m + "-" + d;
}

function ymdformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return "{0}年{1}月{2}日".format(y, m, d);
}
