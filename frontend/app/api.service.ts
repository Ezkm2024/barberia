import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class ApiService {
  baseUrl = 'http://localhost:5000/api';
  constructor(private http: HttpClient) { }

  // TiposDeCorte
  getTipos(): Observable<any> { return this.http.get(this.baseUrl + '/tiposdecorte'); }
  addTipo(data: any): Observable<any> { return this.http.post(this.baseUrl + '/tiposdecorte', data); }
  updateTipo(id: any, data: any): Observable<any> { return this.http.put(this.baseUrl + '/tiposdecorte/' + id, data); }
  deleteTipo(id: any): Observable<any> { return this.http.delete(this.baseUrl + '/tiposdecorte/' + id); }

  // Clientes
  getClientes(): Observable<any> { return this.http.get(this.baseUrl + '/clientes'); }
  addCliente(data: any): Observable<any> { return this.http.post(this.baseUrl + '/clientes', data); }
  updateCliente(id: any, data: any): Observable<any> { return this.http.put(this.baseUrl + '/clientes/' + id, data); }
  deleteCliente(id: any): Observable<any> { return this.http.delete(this.baseUrl + '/clientes/' + id); }

  // Localidades
  getLocalidades(): Observable<any> { return this.http.get(this.baseUrl + '/localidades'); }
}
