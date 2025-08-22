import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-accounts-list',
  template: `
    <section>
      <h2>Cuentas</h2>
      <div class="search"><input placeholder="Buscar..." (input)="onSearch($any($event.target).value)" /></div>
      <div class="actions"><button (click)="create()">Nueva</button></div>
      <table class="grid">
        <thead>
          <tr>
            <th>Número</th><th>Tipo</th><th>Saldo Inicial</th><th>Estado</th><th>ClienteId</th><th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let a of filtered">
            <td>{{a.accountNumber}}</td>
            <td>{{a.accountType}}</td>
            <td>{{a.initialBalance}}</td>
            <td>{{a.status ? 'Activa':'Inactiva'}}</td>
            <td>{{a.clientId}}</td>
            <td>
              <button (click)="edit(a.accountNumber)">Editar</button>
              <button (click)="remove(a.accountNumber)">Eliminar</button>
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
export class AccountsListComponent implements OnInit {
  list:any[]=[]; filtered:any[]=[]; error='';
  constructor(private api:ApiService, private router:Router){}
  ngOnInit(){ this.load(); }
  load(){
    this.api.getAccounts().subscribe({ next:d=>{this.list=d; this.filtered=d;}, error:e=>this.error=this.friendlyError(e) });
  }
  onSearch(term:string){
    const t = term.toLowerCase();
    this.filtered = this.list.filter(a =>
      a.accountNumber?.toLowerCase().includes(t) || a.accountType?.toLowerCase().includes(t) || String(a.initialBalance).includes(t) || String(a.clientId).toLowerCase().includes(t)
    );
  }
  create(){ this.router.navigate(['/accounts/new']); }
  edit(accountNumber:string){ this.router.navigate(['/accounts', accountNumber, 'edit']); }
  remove(accountNumber:string){ if(!confirm('¿Eliminar?')) return; this.api.deleteAccount(accountNumber).subscribe({ next:()=>this.load(), error:e=>this.error=this.friendlyError(e) }); }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
