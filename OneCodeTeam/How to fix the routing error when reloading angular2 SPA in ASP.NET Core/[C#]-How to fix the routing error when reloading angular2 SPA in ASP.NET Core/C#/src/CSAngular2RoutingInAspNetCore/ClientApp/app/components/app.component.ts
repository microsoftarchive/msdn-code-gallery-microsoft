import { Component } from '@angular/core';
@Component({
    selector: 'app',
    template: `<h1>My First Angular App</h1>
        <p><a routerLink="/Component1">Component1</a> | <a routerLink="/Component2">Component2</a></p>
        <router-outlet></router-outlet>
    `
})
export class AppComponent { }