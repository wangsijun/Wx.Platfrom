var root = "";//http://f.emoney.cn/ymstock
var bs64 = (typeof (Uint8Array) != "function");
var em = {
    rType: {
        json: 0,
        text: 1,
        buffer: 2
    },
    gi: new RegExp("{i}", "g"),
    rnd: function () {
        return Math.random();
    },
    url: function (p) {
        if (p == "dsUrl1") return "222.186.14.61:80";//"222.186.14.6:80"; //192.168.8.115:8890
        var u = location.search;
        var i = u.toLowerCase().indexOf(p.toLowerCase() + "=");
        if (i == -1) return "";
        var r = u.substr(i);
        i = r.indexOf("&");
        return unescape((i == -1) ? r.split("=")[1] : r.substring(0, i).split("=")[1]);
    },
    hasList: function (o) {
        if (o == null || o.length == 0) return false;
        return true;
    },
    remove: function (a, o) {
        if (a == null) return;
        for (var i = 0; i < a.length; i++) {
            if (a[i] == o) {
                a.splice(i, 1);
                break;
            }
        }
        return a;
    },
    getCookie: function (s) {
        var r = document.cookie.split(";");
        for (var i = 0; i < r.length; i++) {
            if (r[i].split("=")[0].trim() == s) return unescape(r[i].split("=")[1]);
        }
        return "";
    },
    setCookie: function (s, v) {
        document.cookie = s + "=" + escape(v) + ";path=/";
    },
    delCookie: function (s, v) {
        var r = em.getCookie(s);
        r = ("," + r).replace("," + v, "");
        if (r.indexOf(",") == 0) r = r.substring(1);
        em.setCookie(s,r);
    },
    addCookie: function (s, v) {
        var r = em.getCookie(s).split(",");
        r.push(v);
        em.setCookie(s,r.join(","));
    },
    getU: function (u) {
        return u;
        var p = em.url("p");
        if (p == "") p = escape(location.pathname.substr(2));
        return u + ((u.indexOf("?") > -1) ? "&" : "?") + "ses=262418885&dsUrl1=222.186.14.61:80&p="+p+((u.indexOf("uid=")>=0)?"":"&uid="+em.url("uid"));
        //return u + ((u.indexOf("?") > -1) ? "&" : "?") + "uid=" + em.url("uid") + "&ses=" + em.url("ses") + "&dsUrl1=" + em.url("dsUrl1");
    },
    fStock: function (s) {
        return s.substr(s.length - 6);
        //stocks[i].substring(0, 1) == "6"
    },
    fTitle:function(id)
    {
        var tit = em.url("tit");
        var n = em.url("N"), s = em.url("S");
        if (tit != "") {
            $("#" + id).html(tit);
        } else if (n != "" && s != "") {
            $("#" + id).css("line-height", "20px");
            $("#" + id).html(n + "<br><span style='font-size:small'>" + em.fStock(s) + "</span>");
        }
    },
    aStock: function (s) {
        var r = s;
        if (r.indexOf("6")==0) {
            r = "SH" + r;
        }
        else if(r.indexOf("1")==0){
            r = "SZ" + r.substring(1);
        }
        return r;
    },
    getPc: function (p) {
        if (p == 0 || p == null) return "t";
        return (p > 0) ? "z" : "d"
    },
    alert:function(m)
    {
        var msg = "";
        if (typeof (m) == "string") {
            msg = m; 
        } else{
            msg = m.msg;
        }
        if (msg == "" || msg==null) return;
        alert(msg);
    },
    tip: function (m) {
        var msg = "";
        if (typeof (m) == "string") {
            msg = m;
        } else {
            msg = m.msg;
        }
        if (msg == "" || msg == null) return;
        var t = $("#tip");
        if (t.length == 0) {
            t = $("<div id='tip'></div>");
            $("body").append(t);
        }
        t.html(msg);
        t.show();
        setTimeout(function () {
            t.hide();
        },2000);
    },
    showOverDueTip: function () {
		var overdue = $("#overdue");
		var buyTel = em.getCookie("telStr");
		var buyCon = em.getCookie("conStr");
		if (overdue.length == 0) {
			overdue = $("<div id=\"overdue\"><div id=\"overdue-backImg\" style=\"width:100%; height:100%; float:left;\"></div><div style=\"right:0px;z-index:2;position:absolute;\"><a id=\"overdue-closeBtn\">X</a></div></div>");
			$("body").append(overdue);
			$("#overdue-closeBtn").click(function() {
				$("#overdue").hide(500);
			});
			$("#overdue-backImg").click(function(){
				var sUserAgent = navigator.userAgent.toLowerCase();  
				var bIsIphoneOs = sUserAgent.match(/iphone os/i) == "iphone os";
				if (bIsIphoneOs) {
					location.href = "sms:" + buyTel;
				}
				else {
					location.href = "sms:" + buyTel + "?body=" + buyCon;
				}
			});
		}
	},
    isLoading: function () {
        if (arguments.length == 0) return ($("#loading").css("display") == "none");
        $("#loading").css("display", (arguments[0] == true) ? "block" : "none");
    },
    xhr: function (url) {
        var req = req = new XMLHttpRequest();
        var t = em.rType.json;
        var rt = ["text", "text", "arraybuffer"];
        var ct = ["text/plain", "text/plain", "application/octet-stream"];
        var prm = null, sl = null;
        var u = url;
        u = u + ((u.indexOf("?") > -1) ? "&" : "?") + "r=" + this.rnd();
        var succ = null, err = null;
        var asy = true;
        for (var i = 1; i < arguments.length; i++) {
            if (arguments[i] == -1) {//有-1参数时，禁用loading
                sl = -1;
            } else if (typeof (arguments[i]) == typeof (0)) {
                t = arguments[i];
            } else if (typeof (arguments[i]) == "function") {
                if (succ == null) {
                    succ = arguments[i];
                } else {
                    err = arguments[i];
                }
            } else if (typeof (arguments[i]) == typeof (true)) {
                asy = arguments[i];
            }
            else if (arguments[i] != null) {
                prm = arguments[i];
            }
        }
       
        req.open((prm == null) ? "GET" : "POST", u, asy);
        try {
            //req.responseType = rt[t];
            //req.setRequestHeader("Content-Type", ct[t]);
            //req.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        } catch (e) {

        }
        
        req.onreadystatechange = function () {
            if (req.readyState == 4) {
                var rtn = {};                
                if (req.status == 200) {
                    if (t == em.rType.json) {
                        eval("rtn.data=" + req.responseText + ";");
                    } else {
                        rtn.data = req.responseText;
                    }
                    succ(rtn);
                } else if (err) {
                    err(rtn);
                } else if (req.statusText != null && req.statusText!="") {
                    alert(req.statusText);
                }
                if (sl != -1) {
                    em.remove(em.requestUrls, u);
                    if (em.requestUrls.length == 0) { em.isLoading(false); }
                }
            }
        };
        if (sl != -1) {
            em.requestUrls.push(u);
            em.isLoading(true);
        }
        try {
            if (prm == null) {
                req.send();
            } else {
                if (bs64) {
                    req.send();
                } else {
                    req.send(prm);
                }
            }
        } catch (e) {
            if (sl != -1) {
                em.remove(em.requestUrls, u);
                if (em.requestUrls.length == 0) { em.isLoading(false); }
            }
            alert(e);
        }
        return this;
    },
    requestUrls: [],
    viewObject: function (o, w) {
        for (var p in o) {
            if (typeof (o[p]) == "function") continue;
            w.html(w.html() + "<br>" + p + "：" + o[p]);
        }
    },
    val: function (o, prop) {
        var r = o;
        var a = prop.split(".");
        for (var i = 0; i < a.length; i++) {
            r = r[a[i]];
            if (r == null) return null
        }
        return r
    },
    eval: function (af, o) {
        if (o == null) return "";
        var f = af;
        var fm = f.split("#")[1];
        if (fm != null) {
            var r = this.val(o, f.split("#")[0]);
            if (r == null) return "";
            if (typeof (r) == typeof (1)) {
                if (fm == "") return r.toFixed(2);
                if (fm.indexOf(".") == -1) return r;
            }
            return this.str(r);
        } else if (f.indexOf("$") == -1) {
            return this.str(this.val(o, f));
        } else {
            var arr = f.match(/\$(\w+\.)*\w+/g);
            for (var i = 0; i < arr.length; i++) {
                f = f.replace(arr[i], this.val(o, arr[i].substr(1)));
            }
            return this.str(eval(f));
        }
    },
    removeField: function (htm, prm, o, n) {
        if (prm == null) return htm;
        var r = htm;
        if (n != null) r = r.replace(this.gi, n);
        for (var i = 0; i < prm.length; i++) {
            r = r.replace(prm[i].p, this.eval(prm[i].v, o));
        }
        r = r.replace(/ value= /g, " value=\"\" ");
        return r;
    },
    getFields: function (o) {
        var r = o.attr("htm").match(/\{([^{}])+\}/g);
        if (r == null) return r;
        var a = [];
        for (var i = 0; i < r.length; i++) {
            a.push({
                "v": em.deStr(r[i].substring(1, r[i].length - 1)),
                "p": r[i]
            });
        }
        return a;
    },
    setHtmData: function (s, b, xr) {
        s.attr("htm", em.formatField(b.html()));
        s[0].prm = em.getFields(s);
        if (s[0].prm != null && xr != true) b.html("");
    },
    formatField: function (str) {
        return str.replace(new RegExp("%7B", "g"), "{").replace(new RegExp("%7D", "g"), "}");
    },
    str: function (v) {
        if (typeof (v) == 'undefined' || v == null || v == "null" || v == "undefined") return "";
        return v;
    },
    enStr: function (v) {
        var r = v + "";
        r = r.replace(/"/g, "&quot;");
        r = r.replace(/</g, "&lt;");
        r = r.replace(/>/g, "&gt;");
        r = r.replace(/&/g, "&amp;");
        return r;
    },
    deStr: function (v) {
        var r = v + "";
        r = r.replace(/&quot;/g, "\"");
        r = r.replace(/&lt;/g, "<");
        r = r.replace(/&gt;/g, ">");
        r = r.replace(/&amp;/g, "&");
        return r;
    },
    unQuot: function (s) {
        if (axu.isEmpty(s)) return s;
        if (s.indexOf("'") == 0) {
            return s.substr(1, s.length - 2).replace(/','/g, ",");
        } else {
            return s;
        }
    },
    quot: function (s) {
        if (axu.isEmpty(s)) return "''";
        if (s.indexOf("'") == 0) {
            return s;
        } else {
            return "'" + s.replace(/,/g, "','") + "'";
        }
    }
};
Math.left = function (v, l) {
    var x = v + "";
    if (x.indexOf(".") > 0 && x.indexOf(".") <= l - 1) {
        return x.substring(0, l + 1);
    } else {
        return x.substring(0, l);
    }
};
String.prototype.strFormat = function () {
    var r = this;
    for (var i = 0; i < arguments.length; i++) {
        var u = new RegExp("\\{" + i + "\\}", "g");
        r = r.replace(u, em.str(arguments[i]))
    }
    return r;
};
String.prototype.has = function (s) {
    return ("," + this + ",").indexOf("," + s + ",") >= 0;
};
String.prototype.remove = function (s) {
    return em.remove(this.split(","), s).join(",");
};
String.prototype.add = function (s) {
    var r = this + "," + s;
    if (r.indexOf(",") == 0) r = r.substr(1);
    return r;
};
$.fn.bd = function () {
    return $(this[0].tBodies[0]);
};
$.fn.hd = function () {
    return $(this[0].tHead);
};


$(function () {
    /*
    if (location.pathname.toLowerCase().substr(2) == "/h" || location.pathname.toLowerCase().substr(2) == "/h/") {
        em.xhr(em.getU("Scripts/em.aspx?p=/h"), function (rtn) {
            eval(rtn.data);
        }, em.rType.text, false);
    } else {
        em.xhr(em.getU("../Scripts/em.aspx"), function (rtn) {
            eval(rtn.data);
        }, em.rType.text, false);
    }
    */
/* 	显示将要过期提示 */
    $("table.ax-grid").each(function () {
        var me = $(this);
        em.setHtmData(me, me.bd());
    });
    $(".ax-repeat").each(function () {
        var me = $(this);
        em.setHtmData(me, me);
    });
    $("a.icon-back").each(function () {
        var me = $(this);
        me.click(function () {
            window.history.go(-1);
        });
    });
    document.title=$("div.hd>div.tit").first().text();
    $("div.acc").each(function () {
        var me = $(this);
        me.children("div.bar").each(function () {
            var me2 = $(this);
            if (me2.children("div.icon-d").length == 0) {
                me2.append("<div class='icon icon-r' ></div>");
                me2.next().hide();
            }
            me2.click(function () {
                me.children("div.bar").next().hide();
                me.children("div.bar").children("div.icon").removeClass("icon-d").addClass("icon-r");
                me2.children("div.icon").removeClass("icon-r").addClass("icon-d");
                me2.next().show();
            });
        });
    });
    //$("div.bd").height($("body").height() - $("div.bd").offset().top - $("div.ft").outerHeight() - 2);
    //$(window).bind("resize", function () {
    //    $("div.bd").height($(window).height() - $("div.bd").offset().top - $("div.ft").outerHeight() - 2);
    //});
});

$.fn.extend({
    axgrid: function () {
        this.loadData = function (arr) {
            this.bd().html("");
            this.addRows(arr);
            return this;
        };
        this.addRows = function (lst, pre) {
            if (!em.hasList(lst)) return this;
            var arr = (pre == true) ? lst.reverse() : lst;
            for (var i = 0; i < arr.length; i++) {
                var obj = arr[i];
                var tr = em.removeField(this.attr("htm"), this[0].prm, obj, this.bd().children("tr").length + 1);
                var t = {};
                if (pre == true) {
                    this.bd().prepend(tr);
                    t = this.bd().children().first();
                } else {
                    this.bd().append(tr);
                    t = this.bd().children().last();
                }
                this.trigger("itemDataBind", [t, { "data": obj, "index": i}]);
            }
            return this;
        };
        return this;
    },
    axrepeat: function () {
        this.loadData = function (arr) {
            this.html("");
            this.addRows(arr);
            return this;
        };
        this.addRows = function (arr) {
            if (!em.hasList(arr)) return this;
            var prm = this[0].prm, htm = this.attr("htm");
            for (var i = 0; i < arr.length; i++) {
                var obj = arr[i];
                var li = em.removeField(htm, prm, obj, this.children().length + 1);
                this.append(li);
                this.trigger("itemDataBind", [this.children().last(), { "data": obj, "index": i }]);
            }
            return this;
        };
        return this;
    }
});

