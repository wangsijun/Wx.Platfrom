document.writeln("<div id=\"bq\">益盟操盘手 创建于2002年<br>2013 益盟操盘手 版权所有<br>ICP许可证号 沪ICP备06000340</div>");
//window.onload = function showDiv() {
//    //var width = document.documentElement.clientWidth;
//    var height = document.documentElement.clientHeight;
//    var div1 = document.getElementById("bq");
//    if (height < 320) {
//        div1.style.position = 'inherit';
//    }
//    else {
//        div1.style.position = 'absolute';
//    }
//}
function showDiv1() {
    //var width = document.documentElement.clientWidth;
    var height = document.documentElement.clientHeight;
    var div1 = document.getElementById("bq");
    if (height < 320) {
        div1.style.position = 'inherit';
    }
    else {
        div1.style.position = 'absolute';
    }
}