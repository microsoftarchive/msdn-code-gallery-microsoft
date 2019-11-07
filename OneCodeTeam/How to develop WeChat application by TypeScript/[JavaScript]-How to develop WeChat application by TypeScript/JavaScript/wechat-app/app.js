/// <reference path="./wxAPI.d.ts"/>
App({
    onLaunch: function () {
        //调用API从本地缓存中获取数据
        var logs = wx.getStorageSync('logs');
        if (!Array.isArray(logs)) {
            logs = [];
        }
        logs.unshift(Date.now());
        wx.setStorageSync('logs', logs);
    },
    getUserInfo: function (cb) {
        var that = this;
        if (this.globalData.userInfo) {
            cb(this.globalData.userInfo);
        }
        else {
            //调用登录接口
            wx.login({
                success: function () {
                    wx.getUserInfo({
                        success: function (res) {
                            that.globalData.userInfo = res.userInfo;
                            cb(that.globalData.userInfo);
                        }
                    });
                }
            });
        }
    },
    globalData: {
        userInfo: null
    }
});
//# sourceMappingURL=app.js.map