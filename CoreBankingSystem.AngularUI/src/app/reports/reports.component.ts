import { Component } from '@angular/core';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-reports',
  template: `
    <section>
      <h2>Reportes</h2>
      <form (ngSubmit)="getReportJson()" #f="ngForm">
        <div class="row"><label>Cliente (ClientId)</label><input name="clientId" [(ngModel)]="clientId" required /></div>
        <div class="row"><label>Rango Fechas</label><input name="rangoFechas" [(ngModel)]="rangoFechas" placeholder="YYYY-MM-DD,YYYY-MM-DD" required /></div>
        <div class="actions">
          <button type="submit">Obtener JSON</button>
          <button type="button" (click)="getReportPdf()">Obtener PDF</button>
        </div>
      </form>
      <p class="error" *ngIf="error">{{error}}</p>

      <div *ngIf="json">
        <h3>JSON</h3>
        <pre>{{json | json}}</pre>
      </div>

      <div *ngIf="pdfBase64">
        <h3>PDF</h3>
        <iframe [src]="asDataUrl(pdfBase64)" style="width:100%;height:70vh;"></iframe>
      </div>
    </section>
  `,
  styles: [`
    .row{display:flex;flex-direction:column;margin-bottom:.5rem}
    label{font-weight:600;margin-bottom:.25rem}
    input{padding:.5rem;border:1px solid #ccc;border-radius:4px}
    .actions{margin:.75rem 0}
    button{margin-right:.5rem}
    .error{color:#b00020;margin-top:.5rem}
  `]
})
export class ReportsComponent{
  clientId=''; rangoFechas=''; json:any=null; pdfBase64=''; error='';
  constructor(private api:ApiService){}
  getReportJson(){
    this.error=''; this.pdfBase64=''; this.json=null;
    if(!this.clientId || !this.rangoFechas){ this.error='Complete los campos'; return; }
    this.api.getReportJson({ clientId:this.clientId, rangoFechas:this.rangoFechas }).subscribe({ next:d=> this.json=d, error:e=> this.error=this.friendlyError(e) });
  }
  getReportPdf(){
    this.error=''; this.pdfBase64='';
    if(!this.clientId || !this.rangoFechas){ this.error='Complete los campos'; return; }
    this.api.getReportPdfBase64({ clientId:this.clientId, rangoFechas:this.rangoFechas }).subscribe({ next:d=> this.pdfBase64=d.base64, error:e=> this.error=this.friendlyError(e) });
  }
  asDataUrl(b64:string){ return `data:application/pdf;base64,${b64}`; }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
