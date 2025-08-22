import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientsListComponent } from './clients/clients-list.component';
import { ClientFormComponent } from './clients/client-form.component';
import { AccountsListComponent } from './accounts/accounts-list.component';
import { AccountFormComponent } from './accounts/account-form.component';
import { TransactionsListComponent } from './transactions/transactions-list.component';
import { TransactionFormComponent } from './transactions/transaction-form.component';
import { ReportsComponent } from './reports/reports.component';

const routes: Routes = [
  { path: '', redirectTo: 'clients', pathMatch: 'full' },
  { path: 'clients', component: ClientsListComponent },
  { path: 'clients/new', component: ClientFormComponent },
  { path: 'clients/:id/edit', component: ClientFormComponent },

  { path: 'accounts', component: AccountsListComponent },
  { path: 'accounts/new', component: AccountFormComponent },
  { path: 'accounts/:accountNumber/edit', component: AccountFormComponent },

  { path: 'transactions', component: TransactionsListComponent },
  { path: 'transactions/new', component: TransactionFormComponent },
  { path: 'transactions/:id/edit', component: TransactionFormComponent },

  { path: 'reports', component: ReportsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
