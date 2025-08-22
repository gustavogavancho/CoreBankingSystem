import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ApiService } from '../shared/api.service';

@Component({
  selector: 'app-client-form',
  template: `
    <section>
      <h2>{{id ? 'Editar' : 'Nuevo'}} Cliente</h2>
      <form [formGroup]="form" (ngSubmit)="save()" novalidate>
        <div class="row"><label>Nombre</label><input formControlName="name" />
          <div class="val" *ngIf="f['name'].touched && f['name'].errors">El nombre es requerido</div></div>
        <div class="row"><label>Género</label><input formControlName="gender" />
          <div class="val" *ngIf="f['gender'].touched && f['gender'].errors">Género requerido</div></div>
        <div class="row"><label>Edad</label><input type="number" formControlName="age" />
          <div class="val" *ngIf="f['age'].touched && f['age'].errors">Edad inválida</div></div>
        <div class="row"><label>Identificación</label><input formControlName="identification" />
          <div class="val" *ngIf="f['identification'].touched && f['identification'].errors">Identificación requerida</div></div>
        <div class="row"><label>Teléfono</label><input formControlName="phoneNumber" />
          <div class="val" *ngIf="f['phoneNumber'].touched && f['phoneNumber'].errors">Teléfono requerido</div></div>
        <div class="row" *ngIf="!id"><label>Contraseña</label><input type="password" formControlName="password" />
          <div class="val" *ngIf="f['password'].touched && f['password'].errors">Contraseña requerida</div></div>
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
export class ClientFormComponent implements OnInit{
  id:string|undefined; error='';
  form!: FormGroup;
  constructor(private fb:FormBuilder, private api:ApiService, private route:ActivatedRoute, private router:Router){
    this.form = this.fb.group({
      name:['', Validators.required],
      gender:['', Validators.required],
      age:[18, [Validators.required, Validators.min(0)]],
      identification:['', Validators.required],
      phoneNumber:['', Validators.required],
      password:['', Validators.required],
      status:[true]
    });
  }
  get f(){ return this.form.controls; }
  ngOnInit(){
    this.id = this.route.snapshot.paramMap.get('id') || undefined;
    if(this.id){
      this.api.getClient(this.id).subscribe({
        next: c=>{
          this.form.patchValue({ ...c, password: ''});
          this.form.get('password')?.clearValidators();
          this.form.get('password')?.updateValueAndValidity();
        }, error: e=> this.error=this.friendlyError(e)
      })
    }
  }
  save(){
    if(this.form.invalid) return;
    const val = this.form.value as any;
    if(this.id){
      const body = { id:this.id, name:val.name, gender:val.gender, age:val.age, identification:val.identification, phoneNumber:val.phoneNumber, status:val.status };
      this.api.updateClient(this.id, body).subscribe({ next:()=>this.router.navigate(['/clients']), error:e=>this.error=this.friendlyError(e) });
    } else {
      this.api.createClient(val).subscribe({ next:()=>this.router.navigate(['/clients']), error:e=>this.error=this.friendlyError(e) });
    }
  }
  cancel(){ this.router.navigate(['/clients']); }
  private friendlyError(e:any){ return e?.error?.message || 'Ocurrió un error'; }
}
