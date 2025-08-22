import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ClientsListComponent } from './clients/clients-list.component';
import { ClientFormComponent } from './clients/client-form.component';
import { AccountsListComponent } from './accounts/accounts-list.component';
import { AccountFormComponent } from './accounts/account-form.component';
import { TransactionsListComponent } from './transactions/transactions-list.component';
import { TransactionFormComponent } from './transactions/transaction-form.component';
import { ReportsComponent } from './reports/reports.component';
import { SearchComponent } from './search/search.component';
import { SearchBarComponent } from './shared/search-bar.component';

@NgModule({
  declarations: [
    AppComponent,
    ClientsListComponent,
    ClientFormComponent,
    AccountsListComponent,
    AccountFormComponent,
    TransactionsListComponent,
    TransactionFormComponent,
    ReportsComponent,
    SearchComponent,
    SearchBarComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
