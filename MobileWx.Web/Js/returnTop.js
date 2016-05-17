window.onscroll = function toggle() {

    var scrolltop = document.body.scrollTop || document.documentElement.scrollTop;

    if (scrolltop >200) {
        target = document.getElementById("returnTop");
        target.style.display = "block";
    }
    else {
        target = document.getElementById("returnTop");
        target.style.display = "none";
    }
}