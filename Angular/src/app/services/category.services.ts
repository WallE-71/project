import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(private http: HttpClient) { }

  private Url = environment.apiUrl + "/v1/CategoryApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  GetCategories(): Observable<any[]> {
    return this.http.get<any[]>(this.Url);
  }

  SubCategories(parentName: string): Observable<any[]> {
    return this.http.get<any[]>(this.Url + '/SubCategories?parentName=' + parentName);
  }
}
