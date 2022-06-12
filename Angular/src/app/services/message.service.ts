import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private http: HttpClient) { }

  private Url = environment.apiUrl + "/v1/MessageApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  Send(message: any) {
    var email = message.email;
    var description = message.description;
    var typeFeedBack = message.typeFeedBack;
    return this.http.post(`${this.Url}?email=${email}&description=${description}&typeFeedBack=${typeFeedBack}`, this.httpOptions);
  }

  Answer(email: string): Observable<any> {
    return this.http.get<any>(`${this.Url}?email=${email}`, this.httpOptions);
  }
}
