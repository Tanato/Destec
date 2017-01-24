import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { APP_BASE_HREF } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { HttpModule, XHRBackend, RequestOptions, Http } from '@angular/http';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { AuthHttp } from './auth.http';
import { TopnavService } from './shared/topnav/topnav.service';
import { BusyModule, BusyConfig } from 'angular2-busy';

import { LoginModule } from './login/login.module';
import { SignupModule } from './signup/signup.module';
import { DestecModule } from './destec/destec.module';
import { SharedModule } from './shared/shared.module';

export function httpFactory(backend: XHRBackend, defaultOptions: RequestOptions, router: Router) {
	return new AuthHttp(backend, defaultOptions, router);
}

var busyConfig = new BusyConfig({
	message: 'Aguarde...',
});

@NgModule({
	imports: [
		BrowserModule,
		FormsModule,
		HttpModule,
		RouterModule.forRoot(routes),
		LoginModule,
		SignupModule,
		DestecModule,
		SharedModule.forRoot(),
		BusyModule.forRoot(busyConfig),
	],
	declarations: [AppComponent],
	providers: [{
		provide: APP_BASE_HREF,
		useValue: '<%= APP_BASE %>',
	}, {
		provide: Http,
		useFactory: httpFactory,
		deps: [XHRBackend, RequestOptions, Router]
	},
		TopnavService,
	],
	bootstrap: [AppComponent]
})
export class AppModule { }
