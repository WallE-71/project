import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
// import { Guid } from 'guid-typescript';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SliderService {
  constructor(private http: HttpClient) { }

  private Url = environment.apiUrl + "/v1/SliderApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  GetSliders():Observable<any> {
    return this.http.get<any>(this.Url, this.httpOptions);
  }
}
