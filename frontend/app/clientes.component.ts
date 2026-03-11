import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from './api.service';

@Component({
  selector: 'app-clientes',
  template: `
    <h2>Clientes</h2>
    <form [formGroup]="form" (ngSubmit)="save()">
      <div class="form-row">
        <label>Documento</label>
        <input class="input" formControlName="Documento" />
      </div>
      <div class="form-row">
        <label>Apellido</label>
        <input class="input" formControlName="Apellido" />
      </div>
      <div class="form-row">
        <label>Nombres</label>
        <input class="input" formControlName="Nombres" />
      </div>
      <div class="form-row">
        <label>TipoDoc</label>
        <select class="input" formControlName="TipoDoc">
          <option value="">--</option>
          <option *ngFor="let t of tiposDocs" [value]="t">{{t}}</option>
        </select>
      </div>
      <div class="form-row">
        <label>Localidad</label>
        <select class="input" formControlName="IdLocalidad">
          <option value="">--</option>
          <option *ngFor="let l of localidades" [value]="l.IdLocalidad">{{l.Localidad}}</option>
        </select>
      </div>
      <div class="form-row">
        <label>TelefonoFijo</label>
        <input class="input" formControlName="TelefonoFijo" />
      </div>
      <div class="form-row">
        <label>TelefonoCelular</label>
        <input class="input" formControlName="TelefonoCelular" />
      </div>
      <div>
        <button class="button" type="submit" [disabled]="form.invalid">Guardar</button>
        <button class="button" type="button" (click)="nuevo()">Nuevo</button>
      </div>
    </form>

    <table class="table">
      <thead><tr><th>Documento</th><th>Apellido</th><th>Nombres</th><th>Localidad</th><th></th></tr></thead>
      <tbody>
        <tr *ngFor="let c of clientes">
          <td>{{c.Documento}}</td>
          <td>{{c.Apellido}}</td>
          <td>{{c.Nombres}}</td>
          <td>{{c.Localidad}}</td>
          <td>
            <button (click)="edit(c)">Editar</button>
            <button (click)="delete(c.IdCliente)">Borrar</button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class ClientesComponent implements OnInit {
  clientes: any[] = [];
  localidades: any[] = [];
  tiposDocs = ['DNI','CUIT','CUIL','PASAPORTE','CI','LE','LC'];
  form: FormGroup;
  editingId: any = null;

  constructor(private fb: FormBuilder, private api: ApiService){
    this.form = this.fb.group({
      TipoDoc: ['', Validators.required],
      Documento: ['', Validators.required],
      Apellido: [''],
      Nombres: [''],
      IdLocalidad: [''],
      TelefonoFijo: ['', Validators.pattern('^[0-9]*$')],
      TelefonoCelular: ['', Validators.pattern('^[0-9]*$')]
    });
  }

  ngOnInit(){ this.load(); this.loadLocalidades(); }
  load(){ this.api.getClientes().subscribe(r=>this.clientes=r); }
  loadLocalidades(){ this.api.getLocalidades().subscribe(r=>this.localidades=r); }

  save(){
    if (this.form.invalid) return;
    const v = this.form.value;
    if (this.editingId){
      this.api.updateCliente(this.editingId, v).subscribe(()=>{ this.load(); this.nuevo(); });
    } else {
      this.api.addCliente(v).subscribe(()=>{ this.load(); this.nuevo(); });
    }
  }
  edit(c:any){ this.editingId = c.IdCliente; this.form.patchValue(c); }
  delete(id:any){ if(confirm('Confirma?')) this.api.deleteCliente(id).subscribe(()=>this.load()); }
  nuevo(){ this.editingId=null; this.form.reset(); }
}
