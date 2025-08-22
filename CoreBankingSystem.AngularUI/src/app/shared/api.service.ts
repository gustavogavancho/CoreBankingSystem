import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  // Configurable desde environments
  baseApi = `${environment.apiRoot}/api`;
  reportRoot = `${environment.apiRoot}`; // /report/* vive en raíz

  constructor(private http: HttpClient) {}

  // Clients
  getClients(): Observable<any[]> { return this.http.get<any[]>(`${this.baseApi}/clients`); }
  getClient(id: string): Observable<any> { return this.http.get<any>(`${this.baseApi}/clients/${id}`); }
  createClient(body: any): Observable<any> { return this.http.post<any>(`${this.baseApi}/clients`, body); }
  updateClient(id: string, body: any): Observable<any> { return this.http.put<any>(`${this.baseApi}/clients/${id}`, body); }
  deleteClient(id: string): Observable<void> { return this.http.delete<void>(`${this.baseApi}/clients/${id}`); }

  // Accounts
  getAccounts(): Observable<any[]> { return this.http.get<any[]>(`${this.baseApi}/accounts`); }
  getAccount(accountNumber: string): Observable<any> { return this.http.get<any>(`${this.baseApi}/accounts/${accountNumber}`); }
  createAccount(body: any): Observable<any> { return this.http.post<any>(`${this.baseApi}/accounts`, body); }
  updateAccount(accountNumber: string, body: any): Observable<any> { return this.http.put<any>(`${this.baseApi}/accounts/${accountNumber}`, body); }
  deleteAccount(accountNumber: string): Observable<void> { return this.http.delete<void>(`${this.baseApi}/accounts/${accountNumber}`); }

  // Transactions
  getTransactions(): Observable<any[]> { return this.http.get<any[]>(`${this.baseApi}/transactions`); }
  getTransaction(id: string): Observable<any> { return this.http.get<any>(`${this.baseApi}/transactions/${id}`); }
  createTransaction(body: any): Observable<any> { return this.http.post<any>(`${this.baseApi}/transactions`, body); }

  // Reports
  getReportJson(params: { identification?: string; Cliente?: string; clientId?: string; start?: string; end?: string; rangoFechas?: string }): Observable<any> {
    const query = new URLSearchParams(params as any).toString();
    return this.http.get<any>(`${this.reportRoot}/report/json?${query}`);
    }
  getReportPdfBase64(params: { identification?: string; Cliente?: string; clientId?: string; start?: string; end?: string; rangoFechas?: string }): Observable<{ base64: string }> {
    const query = new URLSearchParams(params as any).toString();
    return this.http.get<{ base64: string }>(`${this.reportRoot}/report/pdf?${query}`);
  }
}
