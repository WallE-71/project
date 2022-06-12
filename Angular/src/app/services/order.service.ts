import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { SecurityService } from './security.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private http: HttpClient, public securityService: SecurityService) { }

  private Url = environment.apiUrl + "/v1/OrderApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  Order() {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    return this.http.get(`${this.Url}?userId=${userId}&offset=${0}&limit=${30}`, this.httpOptions);3
  }

  Remove(orderId: number){
    var userId = this.securityService.GetFieldFromJWT('id')!;
    return this.http.post(`${this.Url}?orderId=${orderId}&userId=${userId}`, this.httpOptions);
  }
}
