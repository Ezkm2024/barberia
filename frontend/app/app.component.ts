import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <h1>Barbería - Frontend</h1>
    <nav>
      <button (click)="view='tipos'">Tipos de Corte</button>
      <button (click)="view='clientes'">Clientes</button>
    </nav>
    <div [ngSwitch]="view">
      <app-tiposdecorte *ngSwitchCase="'tipos'"></app-tiposdecorte>
      <app-clientes *ngSwitchCase="'clientes'"></app-clientes>
      <div *ngSwitchDefault>Seleccione una opción</div>
    </div>
  `
})
export class AppComponent { view = 'tipos'; }
