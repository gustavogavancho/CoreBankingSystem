import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-transactions-list',
  template: `
    <section>
      <h2>Movimientos</h2>
      <app-search-bar (search)="onSearch($event)"></app-search-bar>
      <div class="actions"><button (click)="create()">Nuevo</button></div>
      <table class="grid">
        <thead>
          <tr>
            <th>Fecha</th><th>Tipo</th><th>Monto</th><th>Saldo</th><th>Cuenta</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let t of filtered">
            <td>{{t.date | date: 'yyyy-MM-dd'}}</td>
            <td>{{t.transactionType}}</td>
            <td>{{t.amount}}</td>
            <td>{{t.balance}}</td>
            <td>{{t.accountNumber}}</td>
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
    .error{color:#b00020;margin-top:.5rem}
  `]
})
export class TransactionsListComponent implements OnInit {
  list:any[]=[]; filtered:any[]=[]; error='';
  constructor(private api:ApiService, private router:Router){}
  ngOnInit(){ this.load(); }
  load(){ this.api.getTransactions().subscribe({ next:d=>{this.list=d; this.filtered=d;}, error:e=>this.error=this.friendlyError(e) }); }
  onSearch(term:string){
    const t = term.toLowerCase();
    this.filtered = this.list.filter(x =>
      x.transactionType?.toLowerCase().includes(t) || String(x.amount).includes(t) || x.accountNumber?.toLowerCase().includes(t)
    );
  }
  create(){ this.router.navigate(['/transactions/new']); }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
