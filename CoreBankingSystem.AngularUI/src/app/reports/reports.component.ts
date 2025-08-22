import { Component } from '@angular/core';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-reports',
  template: `
    <section>
      <h2>Reportes</h2>
      <form (ngSubmit)="getReport()" #f="ngForm">
        <div class="row"><label>Identificación</label><input name="identification" [(ngModel)]="identification" required /></div>
        <div class="row"><label>Rango Fechas</label><input name="rangoFechas" [(ngModel)]="rangoFechas" placeholder="YYYY-MM-DD,YYYY-MM-DD" required /></div>
        <div class="actions">
          <button type="submit">Obtener</button>
          <button type="button" (click)="downloadPdf()">Descargar PDF</button>
        </div>
      </form>
      <p class="error" *ngIf="error">{{error}}</p>

      <div *ngIf="report">
        <h3>Cliente: {{report.name}}</h3>
        <p>Rango: {{report.startDate | date:'yyyy-MM-dd'}} a {{report.endDate | date:'yyyy-MM-dd'}}</p>

        <!-- Resumen por cuenta -->
        <table class="grid" *ngIf="report.accounts?.length; else noData">
          <thead>
            <tr>
              <th>Cuenta</th>
              <th>Tipo</th>
              <th>Estado</th>
              <th>Saldo Inicial</th>
              <th>Créditos</th>
              <th>Débitos</th>
              <th>Saldo Final</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let a of report.accounts">
              <td>{{a.accountNumber}}</td>
              <td>{{a.accountType}}</td>
              <td>{{a.status ? 'Activo' : 'Inactivo'}}</td>
              <td>{{a.startingBalance | number:'1.2-2'}}</td>
              <td>{{a.totalCredits | number:'1.2-2'}}</td>
              <td>{{a.totalDebits | number:'1.2-2'}}</td>
              <td>{{a.endingBalance | number:'1.2-2'}}</td>
              <td><button type="button" (click)="a._open = !a._open">{{a._open ? 'Ocultar' : 'Ver'}} movimientos</button></td>
            </tr>
            <tr *ngFor="let a of report.accounts">
              <td colspan="8" *ngIf="a._open">
                <div class="tx">
                  <h4>Movimientos - {{a.accountNumber}}</h4>
                  <table class="grid small" *ngIf="a.transactions?.length; else noTx">
                    <thead>
                      <tr>
                        <th>Fecha</th>
                        <th>Tipo</th>
                        <th>Monto</th>
                        <th>Saldo</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr *ngFor="let t of a.transactions">
                        <td>{{t.date | date:'yyyy-MM-dd'}}</td>
                        <td>{{t.transactionType}}</td>
                        <td>{{t.amount | number:'1.2-2'}}</td>
                        <td>{{t.balance | number:'1.2-2'}}</td>
                      </tr>
                    </tbody>
                  </table>
                  <ng-template #noTx><p>Sin movimientos en el rango.</p></ng-template>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        <ng-template #noData>
          <p>Sin cuentas.</p>
        </ng-template>
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
    .grid{width:100%;border-collapse:collapse;margin-top:.5rem}
    th,td{border:1px solid #ddd;padding:.5rem;vertical-align:top}
    th{background:#fafafa;text-align:left}
    .grid.small th,.grid.small td{padding:.35rem}
    .tx{background:#fcfcfc;padding:.5rem;border:1px solid #eee;border-radius:4px}
  `]
})
export class ReportsComponent{
  identification=''; rangoFechas=''; report:any=null; error='';
  constructor(private api:ApiService){}
  getReport(){
    this.error=''; this.report=null;
    if(!this.identification || !this.rangoFechas){ this.error='Complete los campos'; return; }
    this.api.getReportJson({ identification:this.identification, rangoFechas:this.rangoFechas }).subscribe({ next:d=> this.report=d, error:e=> this.error=this.friendlyError(e) });
  }
  downloadPdf(){
    this.error='';
    if(!this.identification || !this.rangoFechas){ this.error='Complete los campos'; return; }
    this.api.getReportPdfBase64({ identification:this.identification, rangoFechas:this.rangoFechas }).subscribe({ next:res=> this.downloadBase64File(res.base64, 'estado-cuenta.pdf'), error:e=> this.error=this.friendlyError(e) });
  }
  private downloadBase64File(base64: string, fileName: string){
    try{
      const byteChars = atob(base64);
      const byteNumbers = new Array(byteChars.length);
      for(let i=0;i<byteChars.length;i++){ byteNumbers[i] = byteChars.charCodeAt(i); }
      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: 'application/pdf' });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    } catch {}
  }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
