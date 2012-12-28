(function($) {
    //global var
    var j = $;
    var currentPosition = 0;
    var slideWidth = 610;
    var slides = j("#hot_container li");
    var numberOfSlides = slides.length;

    j(document).ready(function() {
        initEvent();
    });
    //function
    function loginWin(objThis, objOther, thisTargetId, otherTargetId) {
        j(objThis).removeClass("abtn");
        j(objThis).parent().addClass("selectTag");

        objOther.addClass("abtn");
        objOther.parent().removeClass("selectTag");

        j("#" + thisTargetId).hide();
        j("#" + otherTargetId).show();
    }
    function checkLogin() {
        if (j("#username").val().trim() == "") {
            alert("用户名不能为空");
            return false;
        }
        if (j("#password").val().trim() == "") {
            alert("密码不能为空");
            return false;
        }
        return true;
    }
    function refCode() {
        j("#regimg").attr("src", "/verifyImg.aspx?seed=" + Math.random());
    }
    function regTips(obj) {
        if (j(obj).val().trim() == "") {
            j(obj).parent().find("img").show();
            return false;
        }
        else {
            j(obj).parent().find("img").hide();
        }
        return true;
    }
    function manageControls(obj) {
        var pageCount = numberOfSlides % 3 == 0 ? zhengChu(numberOfSlides, 3) : zhengChu(numberOfSlides, 3) + 1;
        if (currentPosition == 0) {
            j("#stepup").removeClass("left");
            j("#stepup").addClass("noleft");
            j("#stepup").unbind("click");
        }
        else if (currentPosition == pageCount - 1) {
            j("#next").removeClass("right");
            j("#next").addClass("noright");
            j("#next").unbind("click");
        }
        else {
            if (j(obj).attr("id") == "next") {
                if (currentPosition == 1) {
                    j("#stepup").bind("click", function() { animatePic(this) });
                }
            }
            if (j(obj).attr("id") == "stepup") {
                if (currentPosition == pageCount - 1 - 1) {
                    j("#next").bind("click", function() { animatePic(this) });
                }
            }
            j("#stepup").removeClass("noleft");
            j("#stepup").addClass("left");

            j("#next").removeClass("noright");
            j("#next").addClass("right");
        }

    }
    function animatePic(obj) {
        currentPosition = (j(obj).attr('id') == 'next') ? currentPosition + 1 : currentPosition - 1;
        manageControls(obj);
        // Move slideInner using margin-left
        j('#hot_container').animate({
            'marginLeft': slideWidth * (-currentPosition)
        });
    }
    function dispPop(obj) {
        var t = j(obj).parent().find("div:eq(0)").html();
        var c = j(obj).parent().find("div:eq(1)").html();
        j("#showTitle").html(t);
        j("#showContent").html(c);
        j(".notice").show();
    }


    //bind event
    function initEvent() {
        //登录
        j("#tmm_a_login").click(function() {
            loginWin(this, j("#tmm_a_reg"), "login_tagnum1", "login_tagnum0");
        });
        //注册
        j("#tmm_a_reg").click(function() {
            loginWin(this, j("#tmm_a_login"), "login_tagnum0", "login_tagnum1");
        });
        //登录按钮事件
        j("#btnLoginForm").click(function() {
            if (!checkLogin()) {
                return false;
            }
        });
        //更换验证码
        j("#regimg,#aRegImg").click(refCode);
        //注册提示
        j("#regloginemail,#regpassword").blur(function() {
            regTips(this);
        });
        //注册按钮事件
        j("#btnRegForm").click(function() {
            if (regTips(j("#regloginemail")) && regTips(j("#regpassword"))) {
                if (j("#yanzhengma").val().trim() == "") {
                    alert("验证码不能为空");
                    return false;
                }
                var r = new RegExp(/^([a-z0-9A-Z_\.-]+)@([\da-zA-Z\.-]+)\.([a-z\.]{2,6})$/);
                if (!r.test(j("#regloginemail").val().trim())) {
                    alert("邮箱地址格式不正确");
                    return false;
                }
                if (!j("#chAgree").attr("checked")) {
                    alert("您未同意服务条款");
                    return false;
                }
            }
            else {
                return false;
            }
        });

        //幻灯片
        j("#next").bind("click", function() { animatePic(this); });
        //关闭弹窗
        j(".item-skills-bg a.closed").click(function() {
            j("div.notice").hide();
        });
        //显示弹窗
        j("li.tmm_li_sysmsg a").click(function() {
            dispPop(this);
        });
    }

})(jQuery);