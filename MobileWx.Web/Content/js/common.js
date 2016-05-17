String.prototype.format = function () {
    var result = this.valueOf();
    for (var i = 0; i < arguments.length; i++) {
        result = result.replace("{" + i + "}", arguments[i]);
    }
    return result;
}
function htmlencode(value) {
    var temp = document.createElement("div");
    (temp.textContent != null) ? (temp.textContent = value) : (temp.innerText = value);
    var output = temp.innerHTML;
    temp = null;
    return output;
}
function fixstr(str, length, filler) {
    str = str + "";
    while (str.length < length) {
        str = filler + str;
    }
    return str;
}

Date.prototype.format = function (f) {
    return f.replace("yyyy", this.getFullYear())
    .replace("MM", fixstr(this.getMonth() + 1, 2, "0"))
    .replace("dd", fixstr(this.getDate(), 2, "0"))
    .replace("HH", fixstr(this.getHours(), 2, "0"))
    .replace("mm", fixstr(this.getMinutes() + 1, 2, "0"))
    .replace("ss", fixstr(this.getSeconds(), 2, "0"))
    .replace("yy", this.getYear())
    .replace("M", this.getMonth() + 1)
    .replace("d", this.getDate());
}

String.prototype.trim = function () {
    return this.replace(/^\s*|\s*$/g, "");
}

function GetJsonDateTime(value) {
    var reg = /^\/Date\((\d+)\)\/$/i.exec(value);
    if (reg) {
        return new Date(parseInt(reg[1]));
    }
}

function GetEnumValue(classname) {
    var v = 0;
    $("." + classname + ":checked").each(function () {
        var itemvalue = parseInt($(this).val());
        if (!isNaN(itemvalue)) {
            v |= itemvalue;
        }
    });
    return v;
}