import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { MapComponent } from './map/map.component';
import { MawCommonModule } from 'maw-common';

@NgModule({
  declarations: [
    AppComponent,
    MapComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    MawCommonModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
