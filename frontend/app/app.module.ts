import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { TiposDeCorteComponent } from './tiposdecorte.component';
import { ClientesComponent } from './clientes.component';
import { ApiService } from './api.service';

@NgModule({
  declarations: [AppComponent, TiposDeCorteComponent, ClientesComponent],
  imports: [BrowserModule, HttpClientModule, FormsModule, ReactiveFormsModule],
  providers: [ApiService],
  bootstrap: [AppComponent]
})
export class AppModule { }
