/// <reference path="../../wxAPI.d.ts"/>

let app = getApp();
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
		let that = this;

		//调用应用实例的方法获取全局数据
		app.getUserInfo((userInfo) => {
			that.setData({
				userInfo: userInfo
			});
		});
	}
});
