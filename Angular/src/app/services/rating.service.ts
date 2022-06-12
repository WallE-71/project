import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RatingService {
  constructor(private http: HttpClient) { }

  private url = environment.apiUrl + "/v1/RateApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  public Rate(sellerId: number, productId: string, userId: string, rate: number) {
    return this.http.post(`${this.url}?sellerId=${sellerId}&productId=${productId}&userId=${userId}&rate=${rate}`, this.httpOptions);
  }
}
