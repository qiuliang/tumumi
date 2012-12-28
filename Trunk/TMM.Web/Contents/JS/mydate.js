var mydate = function(obj) {
    //构造函数
    this.y = obj.yid;
    this.m = obj.mid;
    this.d = obj.did;
    this.bm = [1, 3, 5, 7, 8, 10, 12];
    this.mm = [4, 6, 9, 11];
    this.sm = [2];
    this.defaultYear = (obj.dy != "" && obj.dy) ? obj.dy : "1980";
    this.defaultMonth = (obj.dm != "" && obj.dm) ? obj.dm : "1";
    this.defaultDay = (obj.dd != "" && obj.dd) ? obj.dd : "1";
    this.startYear = obj.sy;
    this.offset = obj.offset;

    this.j = obj.adapter;
};
mydate.prototype.init = function() {
    var jq = this.j;
    var obj = this;
    this.getYear(function() {
        obj.getDay(function() {
            obj.defday();
        });
    }, this.offset);

    //月份change事件
    var selector = "#" + this.y + ",#" + this.m;
    this.j(selector).change(function() {
        var cd = jq("#" + obj.d).val();    //当前选中日期的天数
        obj.getDay(function() { });
        jq("#" + obj.d).val(cd);
    });
};
//返回当前年份
mydate.prototype.nowy = function() {
    var d = new Date();
    return d.getFullYear();
};
//返回当前月份
mydate.prototype.nowm = function() {
    var d = new Date();
    return d.getMonth() + 1;
};
//生成下拉框的年份
mydate.prototype.getYear = function(callBack, offset) {
    var y = this.nowy();
    if (offset) {
        y = y + offset;
    }
    var startYear = parseInt(this.startYear);
    for (var i = startYear; i <= y; i++) {
        if (i == this.defaultYear) {
            this.j("#" + this.y).append("<option selected='selected'>" + i + "</option>");
        }
        else {
            this.j("#" + this.y).append("<option>" + i + "</option>");
        }
    }
    callBack();
};
mydate.prototype.getDay = function(callBack) {
    this.j("#" + this.d).html("");
    var curm = this.j("#" + this.m).val();
    var dc = 32;

    if (this.mm.contain(curm)) {
        dc = 31;
    }
    else if (this.sm.contain(curm)) {
        var cy = this.j("#" + this.y).val();
        cy = parseInt(cy);
        if (cy % 4 == 0) {
            dc = 30;
        } else {
            dc = 29;
        }
    }
    this.j("#" + this.d).append("<option>请选择</option>");
    for (var i = 1; i < dc; i++) {
        this.j("#" + this.d).append("<option>" + i + "</option>");
    }
    callBack();
};
mydate.prototype.defday = function() {
    //设置默认选中月和日
    //this.j("#" + this.y).val(this.defaultYear);
    this.j("#" + this.m).val(this.defaultMonth);
    this.j("#" + this.d).val(this.defaultDay);
};

mydate.prototype.checkBirthday = function() { 
    var regexDate = new RegExp("^((\\d{4})|(\\d{2}))([-./])(\\d{1,2})\\4(\\d{1,2})$"); 
    var r = new RegExp(regexDate);
    var cb = r.test(this.getDateStr());
    return cb;
};
//公共方法，获取生日字符串 yyyy-mm-dd
mydate.prototype.getDateStr = function() {
    var regexDate = new RegExp("^((\\d{4})|(\\d{2}))([-./])(\\d{1,2})\\4(\\d{1,2})$");
    var r = new RegExp(regexDate);
    var str = this.j("#" + this.y).val() + "-" + this.j("#" + this.m).val() + "-" + this.j("#" + this.d).val();
    var cb = r.test(str);
    if (cb) {
        return str;
    }
    else {
        //alert("日期格式不正确");
        return "";
    }

};

//扩展array方法
Array.prototype.contain = function(n) {
    var f = false;
    for (var i = 0; i < this.length; i++) {
        if (n == this[i]) {
            f = true;
            break;
        }
    }
    return f;
};