/// <reference path="../../wxAPI.d.ts"/>
var app = getApp();
Page({
    data: {
        motto: 'Hello Wechat via TypeScript',
        userInfo: {}
    },
    // 事件处理函数
    bindViewTap: function () {
        wx.navigateTo({
            url: '../logs/logs'
        });
    },
    onLoad: function () {
        console.log('onLoad');
        var that = this;
        //调用应用实例的方法获取全局数据
        app.getUserInfo(function (userInfo) {
            that.setData({
                userInfo: userInfo
            });
        });
    }
});
//# sourceMappingURL=index.js.map