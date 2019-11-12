import { Component, OnInit } from "@angular/core";

import { AuthService } from "../_services/auth.service";

@Component({
    selector: "my-home",
    templateUrl: "view.html",
    styleUrls: ["style.css"]
})
export class HomeComponent implements OnInit {
    isLogin = false;
    userName: string;
    
    constructor(
        private authService: AuthService
    ) { }

    ngOnInit(): void {
        this.isLogin = this.authService.checkLogin();
        if (this.isLogin) {
            this.authService.getUserInfo().then(res => {
                this.userName = (res.Data as any).UserName;
            });
        }

    }
}
