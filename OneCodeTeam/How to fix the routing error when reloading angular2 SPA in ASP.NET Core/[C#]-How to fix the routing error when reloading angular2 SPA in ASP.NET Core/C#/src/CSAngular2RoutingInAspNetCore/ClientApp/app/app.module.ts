import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';

import { AppComponent } from './components/app.component';
import { Component1 } from './components/component1/component1';
import { Component2 } from './components/component2/component2';

@NgModule({
    bootstrap: [AppComponent],
    declarations: [
        AppComponent,
        Component1,
        Component2
    ],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: "", redirectTo: "/Component1", pathMatch: "full" },
            { path: "Component1", component: Component1 },
            { path: "Component2", component: Component2 }
        ])
    ]
})
export class AppModule {
}
