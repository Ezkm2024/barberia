import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from './api.service';

@Component({
  selector: 'app-tiposdecorte',
  template: `
    <h2>Tipos de Corte</h2>
    <form [formGroup]="form" (ngSubmit)="save()">
      <div class="form-row">
        <label>Tipo de Corte</label>
        <input class="input" formControlName="TipoDeCorte" />
      </div>
      <div class="form-row">
        <label>Precio</label>
        <input class="input" formControlName="PrecioServicio" />
      </div>
      <div>
        <button class="button" type="submit" [disabled]="form.invalid">Guardar</button>
        <button class="button" type="button" (click)="nuevo()">Nuevo</button>
      </div>
    </form>

    <table class="table">
      <thead>
        <tr><th>TipoDeCorte</th><th>PrecioServicio</th><th></th></tr>
      </thead>
      <tbody>
        <tr *ngFor="let t of tipos">
          <td>{{t.TipoDeCorte}}</td>
          <td>{{t.PrecioServicio}}</td>
          <td>
            <button (click)="edit(t)">Editar</button>
            <button (click)="delete(t.IdTipoDeCorte)">Borrar</button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class TiposDeCorteComponent implements OnInit {
  tipos: any[] = [];
  form: FormGroup;
  editingId: any = null;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.form = this.fb.group({
      TipoDeCorte: ['', Validators.required],
      PrecioServicio: ['', [Validators.required, Validators.pattern('^[0-9]+(\\.[0-9]{1,2})?$')]]
    });
  }

  ngOnInit() { this.load(); }
  load() { this.api.getTipos().subscribe(r => this.tipos = r); }
  save() {
    if (this.form.invalid) return;
    const v = this.form.value;
    if (this.editingId) {
      this.api.updateTipo(this.editingId, v).subscribe(() => { this.load(); this.nuevo(); });
    } else {
      this.api.addTipo(v).subscribe(() => { this.load(); this.nuevo(); });
    }
  }
  edit(t:any) { this.editingId = t.IdTipoDeCorte; this.form.patchValue(t); }
  delete(id:any) { if(confirm('Confirma?')) this.api.deleteTipo(id).subscribe(()=>this.load()); }
  nuevo() { this.editingId = null; this.form.reset(); }
}
