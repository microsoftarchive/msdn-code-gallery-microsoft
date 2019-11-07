/// <reference path="./wxAPI.d.ts"/>

App({
	onLaunch: function() {
		//调用API从本地缓存中获取数据
		let logs: any = wx.getStorageSync('logs');
		
		if (!Array.isArray(logs)) {
			logs = [];
		}
		(<any[]>logs).unshift(Date.now());
        wx.setStorageSync('logs', logs);
	},
	getUserInfo: function(cb: (param: any) => void) {
        let that = this
		if (this.globalData.userInfo) {
			cb(this.globalData.userInfo)
		} else {
            //调用登录接口
			wx.login({
				success: () => {
					wx.getUserInfo({
						success: (res) => {
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
