import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NewsletterService {
  constructor(private http: HttpClient) { }

  private Url = environment.apiUrl + "/v1/NewsletterApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  Send(email: string) {
    return this.http.post(`${this.Url}?email=${email}`, this.httpOptions);
  }
}
