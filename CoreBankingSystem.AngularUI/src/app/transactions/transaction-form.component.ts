import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-transaction-form',
  template: `
    <section>
      <h2>Nueva Transacción</h2>
      <form [formGroup]="form" (ngSubmit)="save()" novalidate>
        <div class="row"><label>Cuenta</label><input formControlName="accountNumber" />
          <div class="val" *ngIf="f['accountNumber'].touched && f['accountNumber'].errors">Cuenta requerida</div></div>
        <div class="row"><label>Fecha</label><input type="date" formControlName="date" />
          <div class="val" *ngIf="f['date'].touched && f['date'].errors">Fecha requerida</div></div>
        <div class="row"><label>Tipo</label><input formControlName="transactionType" placeholder="Deposit/Withdrawal" />
          <div class="val" *ngIf="f['transactionType'].touched && f['transactionType'].errors">Tipo requerido</div></div>
        <div class="row"><label>Monto</label><input type="number" formControlName="amount" />
          <div class="val" *ngIf="f['amount'].touched && f['amount'].errors">Monto requerido</div></div>
        <div class="actions">
          <button type="submit" [disabled]="form.invalid">Guardar</button>
          <button type="button" (click)="cancel()">Cancelar</button>
        </div>
        <p class="error" *ngIf="error">{{error}}</p>
      </form>
    </section>
  `,
  styles: [`
    .row{display:flex;flex-direction:column;margin-bottom:.5rem}
    label{font-weight:600;margin-bottom:.25rem}
    input{padding:.5rem;border:1px solid #ccc;border-radius:4px}
    .val{color:#b00020;font-size:.85rem}
    .actions{margin-top:.75rem}
    button{margin-right:.5rem}
    .error{color:#b00020;margin-top:.5rem}
  `]
})
export class TransactionFormComponent{
  error='';
  form!: FormGroup;
  constructor(private fb:FormBuilder, private api:ApiService, private router:Router){
    this.form = this.fb.group({
      accountNumber:['', Validators.required],
      date:['', Validators.required],
      transactionType:['', Validators.required],
      amount:[0, Validators.required]
    });
  }
  get f(){ return this.form.controls; }
  save(){
    if(this.form.invalid) return;
    const val = this.form.value as any;
    this.api.createTransaction(val).subscribe({ next:()=>this.router.navigate(['/transactions']), error:e=>this.error=this.friendlyError(e) });
  }
  cancel(){ this.router.navigate(['/transactions']); }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
