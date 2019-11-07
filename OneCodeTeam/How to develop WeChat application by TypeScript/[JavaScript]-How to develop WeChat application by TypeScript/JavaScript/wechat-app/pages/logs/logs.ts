/// <reference path="../../wxAPI.d.ts" />

import { formatTime } from "../../utils/util";
Page({
	data: {
		logs: []
	},
	onLoad: function() {
		this.setData({
			logs: ((wx.getStorageSync("logs") as Array<any>) || []).map((log) => { return formatTime(new Date(log)); })
		});

		wx.showModal({
			title: '提示',
			content: '这是一个模态弹窗',
			success: function(res) {
				if (res.confirm) {
					console.log('用户点击确定')
				}
			}
		});
	}
});
