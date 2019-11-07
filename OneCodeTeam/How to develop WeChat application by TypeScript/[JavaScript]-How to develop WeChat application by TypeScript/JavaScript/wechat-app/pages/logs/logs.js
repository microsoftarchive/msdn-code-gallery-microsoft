/// <reference path="../../wxAPI.d.ts" />
"use strict";
var util_1 = require("../../utils/util");
Page({
    data: {
        logs: []
    },
    onLoad: function () {
        this.setData({
            logs: (wx.getStorageSync("logs") || []).map(function (log) { return util_1.formatTime(new Date(log)); })
        });
        wx.showModal({
            title: '提示',
            content: '这是一个模态弹窗',
            success: function (res) {
                if (res.confirm) {
                    console.log('用户点击确定');
                }
            }
        });
    }
});
//# sourceMappingURL=logs.js.map