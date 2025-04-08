import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoggerModule, NgxLoggerLevel } from 'ngx-logger';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule,
    LoggerModule.forRoot({
      level: NgxLoggerLevel.DEBUG, // Niveau de logs
      serverLoggingUrl: '/api/logs', // URL d'envoi des logs (optionnel)
      serverLogLevel: NgxLoggerLevel.ERROR // Niveau de logs à envoyer au serveur
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
