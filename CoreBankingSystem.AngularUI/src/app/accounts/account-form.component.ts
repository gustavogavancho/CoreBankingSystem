import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-account-form',
  template: `
    <section>
      <h2>{{accountNumber ? 'Editar' : 'Nueva'}} Cuenta</h2>
      <form [formGroup]="form" (ngSubmit)="save()" novalidate>
        <div class="row"><label>Número</label><input formControlName="accountNumber" [readonly]="!!accountNumber" />
          <div class="val" *ngIf="f['accountNumber'].touched && f['accountNumber'].errors">Número requerido</div></div>
        <div class="row"><label>Tipo</label><input formControlName="accountType" />
          <div class="val" *ngIf="f['accountType'].touched && f['accountType'].errors">Tipo requerido</div></div>
        <div class="row"><label>Saldo Inicial</label><input type="number" formControlName="initialBalance" />
          <div class="val" *ngIf="f['initialBalance'].touched && f['initialBalance'].errors">Saldo inválido</div></div>
        <div class="row"><label>Cliente Id</label><input formControlName="clientId" />
          <div class="val" *ngIf="f['clientId'].touched && f['clientId'].errors">Cliente requerido</div></div>
        <div class="row"><label>Estado</label><input type="checkbox" formControlName="status" /></div>
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
export class AccountFormComponent implements OnInit{
  accountNumber:string|undefined; error='';
  form!: FormGroup;
  constructor(private fb:FormBuilder, private api:ApiService, private route:ActivatedRoute, private router:Router){
    this.form = this.fb.group({
      accountNumber:['', Validators.required],
      accountType:['', Validators.required],
      initialBalance:[0, [Validators.required]],
      status:[true],
      clientId:['', Validators.required]
    });
  }
  get f(){ return this.form.controls; }
  ngOnInit(){
    this.accountNumber = this.route.snapshot.paramMap.get('accountNumber') || undefined;
    if(this.accountNumber){
      this.api.getAccount(this.accountNumber).subscribe({ next:a=> this.form.patchValue(a), error:e=> this.error=this.friendlyError(e) });
    }
  }
  save(){
    if(this.form.invalid) return;
    const val = this.form.value as any;
    if(this.accountNumber){
      this.api.updateAccount(this.accountNumber, val).subscribe({ next:()=>this.router.navigate(['/accounts']), error:e=>this.error=this.friendlyError(e) });
    } else {
      this.api.createAccount(val).subscribe({ next:()=>this.router.navigate(['/accounts']), error:e=>this.error=this.friendlyError(e) });
    }
  }
  cancel(){ this.router.navigate(['/accounts']); }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
