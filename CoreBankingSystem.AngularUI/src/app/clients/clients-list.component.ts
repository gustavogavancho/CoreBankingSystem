import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-clients-list',
  template: `
    <section>
      <h2>Clientes</h2>
      <div class="search"><input placeholder="Buscar..." (input)="onSearch($any($event.target).value)" /></div>
      <div class="actions"><button (click)="create()">Nuevo</button></div>
      <table class="grid">
        <thead>
          <tr>
            <th>Nombre</th><th>Identificación</th><th>Teléfono</th><th>Estado</th><th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let c of filtered">
            <td>{{c.name}}</td>
            <td>{{c.identification}}</td>
            <td>{{c.phoneNumber}}</td>
            <td>{{c.status ? 'Activo' : 'Inactivo'}}</td>
            <td>
              <button (click)="edit(c.id)">Editar</button>
              <button (click)="remove(c.id)">Eliminar</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p *ngIf="filtered.length === 0">Sin resultados.</p>
      <p class="error" *ngIf="error">{{error}}</p>
    </section>
  `,
  styles: [`
    h2{margin:1rem 0}
    .actions{margin:.5rem 0}
    .grid{width:100%;border-collapse:collapse}
    th,td{border:1px solid #ddd;padding:.5rem}
    th{background:#fafafa;text-align:left}
    button{margin-right:.25rem}
    .error{color:#b00020;margin-top:.5rem}
  `]
})
export class ClientsListComponent implements OnInit {
  list:any[]=[]; filtered:any[]=[]; error='';
  constructor(private api:ApiService, private router:Router){}
  ngOnInit(){ this.load(); }
  load(){
    this.api.getClients().subscribe({
      next: d=>{ this.list = d; this.filtered = d; },
      error: e=> this.error = this.friendlyError(e)
    });
  }
  onSearch(term:string){
    const t = term.toLowerCase();
    this.filtered = this.list.filter(c =>
      c.name?.toLowerCase().includes(t) ||
      c.identification?.toLowerCase().includes(t) ||
      c.phoneNumber?.toLowerCase().includes(t));
  }
  create(){ this.router.navigate(['/clients/new']); }
  edit(id:string){ this.router.navigate(['/clients', id, 'edit']); }
  remove(id:string){ if(!confirm('¿Eliminar?')) return; this.api.deleteClient(id).subscribe({ next:()=>this.load(), error:e=>this.error=this.friendlyError(e) }); }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
