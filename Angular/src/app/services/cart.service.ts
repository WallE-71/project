import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CartDto } from '../shared/models';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  constructor(private http: HttpClient) { }

  private Url = environment.apiUrl + "/v1/CartApi";
  private browserId = localStorage.getItem("efne434wgmwbq1176")!;
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  Cart(phoneNumber: string): Observable<any> {
    return this.http.get<any>(`${this.Url}?phoneNumber=${phoneNumber}&browserId=${this.browserId}`, this.httpOptions);
  }

  AddToCart(productId: string, color: string) {
    return this.http.post(`${this.Url}?productId=${productId}&browserId=${this.browserId}&color=${color}`, this.httpOptions);
  }
  
  Increase(cartItemId: number) {
    return this.http.put(`${this.Url}?cartItemId=${cartItemId}`, this.httpOptions);
  }

  Decrease(cartItemId: number) {
    return this.http.put(`${this.Url}/Decrease?cartItemId=${cartItemId}`, this.httpOptions);
  }

  Delete(productId: number) {
    return this.http.delete(`${this.Url}/Delete?productId=${productId}&browserId=${this.browserId}`, this.httpOptions);
  }

  DeleteAll() {
    return this.http.delete(`${this.Url}/DeleteAll?browserId=${this.browserId}`);
  }
}
