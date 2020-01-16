var mFn = null;

//消息遮罩
$.alert = function (msg, fn) {
    $("#AlertContent").html(msg);
    $("#Mask").show();
    $("#MaskAlert").show();
    mFn = fn;
}

//消息遮罩停止
$.alertClose = function () {
    $("#Mask").hide();
    $("#MaskAlert").hide();
    if (mFn != undefined)
        mFn();
}

//确认消息遮罩
$.confirm = function (msg, fn) {
    $("#ConfirmContent").html(msg);
    $("#Mask").show();
    $("#MaskConfirm").show();
    mFn = fn;
}

//确认消息遮罩停止
$.confirmYes = function () {
    $("#Mask").hide();
    $("#MaskConfirm").hide();
    if (mFn != undefined)
        mFn();
}

//消息遮罩停止
$.confirmNo = function () {
    $("#Mask").hide();
    $("#MaskConfirm").hide();
}

//载入遮罩是否执行判断
var mIfLoading = true;

//载入遮罩
$.laoding = function () {
    if (mIfLoading) {
        $("#Mask").show();
        $("#MaskLoading").show();
    }
}

//载入遮罩停止
$.laodingClose = function () {
    $("#Mask").hide();
    $("#MaskLoading").hide();
}

//载入遮罩延时0.2秒执行,如果期间mIfLoading变为false,$.laoding不会执行
$.loadingDelay = function () {
    mIfLoading = true;
    setTimeout($.laoding, 200);
}

//同步进行ajax.post//此处的同步不用async控制,用界面遮罩控制
$.postSync = function (url, data, fn) {
    $.loadingDelay();
    $.ajax({
        type: "post",
        url: url,
        async: true,//false为同步  true为异步
        data: data,
        success: function (data) {
            mIfLoading = false;
            $.laodingClose();
            fn(data);
        }
    });
}

//页面加载完毕后执行
$(document).ready(function () {
    //消息遮罩关闭
    $("[action=AlertClose]").click(function () {
        $.alertClose();
    });

    //确认消息遮罩确认
    $("[action=ConfirmYes]").click(function () {
        $.confirmYes();
    });

    //确认消息遮罩否定
    $("[action=ConfirmNo]").click(function () {
        $.confirmNo();
    });
});
