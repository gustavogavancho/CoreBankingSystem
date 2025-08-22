import { Component, OnInit } from '@angular/core';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-search',
  template: `
    <section>
      <h2>Búsqueda rápida</h2>
      <app-search-bar placeholder="Buscar en clientes, cuentas y movimientos" (search)="search($event)"></app-search-bar>

      <div class="results" *ngIf="term">
        <h3>Resultados para "{{term}}"</h3>
        <div class="result-group" *ngIf="clients.length">
          <h4>Clientes</h4>
          <ul>
            <li *ngFor="let c of clients">{{c.name}} - {{c.identification}} - {{c.phoneNumber}}</li>
          </ul>
        </div>
        <div class="result-group" *ngIf="accounts.length">
          <h4>Cuentas</h4>
          <ul>
            <li *ngFor="let a of accounts">{{a.accountNumber}} - {{a.accountType}} - {{a.clientId}}</li>
          </ul>
        </div>
        <div class="result-group" *ngIf="transactions.length">
          <h4>Movimientos</h4>
          <ul>
            <li *ngFor="let t of transactions">{{t.date | date:'yyyy-MM-dd'}} - {{t.transactionType}} - {{t.amount}} ({{t.accountNumber}})</li>
          </ul>
        </div>
        <p *ngIf="!clients.length && !accounts.length && !transactions.length">Sin coincidencias.</p>
      </div>

      <p class="error" *ngIf="error">{{error}}</p>
    </section>
  `,
  styles: [`
    .results{margin-top:1rem}
    .result-group{margin-bottom:1rem}
    h4{margin:.25rem 0}
    ul{margin:0;padding-left:1rem}
  `]
})
export class SearchComponent implements OnInit{
  term=''; clients:any[]=[]; accounts:any[]=[]; transactions:any[]=[]; error='';
  constructor(private api:ApiService){}
  ngOnInit(){ this.loadAll(); }
  loadAll(){
    // precarga para búsqueda en memoria
    this.api.getClients().subscribe({ next:d=> this.clients = d, error:e=> this.error=this.friendlyError(e) });
    this.api.getAccounts().subscribe({ next:d=> this.accounts = d, error:e=> this.error=this.friendlyError(e) });
    this.api.getTransactions().subscribe({ next:d=> this.transactions = d, error:e=> this.error=this.friendlyError(e) });
  }
  search(t:string){ this.term = t; }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
