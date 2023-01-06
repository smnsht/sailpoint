import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AutocompleteModule } from '../../../autocomplete/src/lib/autocomplete.module';
import { FormsModule } from '@angular/forms';

@NgModule({
	declarations: [
		AppComponent
	],
	imports: [
		BrowserModule,
		FormsModule,
		AutocompleteModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
