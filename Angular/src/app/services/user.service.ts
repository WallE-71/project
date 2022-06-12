import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SecurityService } from '../services/security.service';
import { UserDto } from '../shared/models';
// import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private http: HttpClient, private securityService: SecurityService) { }

  private url = environment.apiUrl + "/v1/AccountApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  RegisterOrSignIn(phoneNumber: string): Observable<UserDto> {
    const browserId = this.securityService.GetBrowserId()!;
    return this.http.post<UserDto>(`${this.url}?phoneNumber=${phoneNumber}&browserId=${browserId}`, this.httpOptions);
  }

  ReciveCode(email: string) {
    return this.http.post(`${this.url}/SendCode?email=${email}`, this.httpOptions);
  }

  EditProfile(user: any) {
    const phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
    return this.http.put(`${this.url}?phoneNumber=${phoneNumber}&email=${user.email}&firstName=${user.firstName}&lastName=${user.lastName}&address=${user.address}`, this.httpOptions);
  }

  GetCreditCart(phoneNumber: string):Observable<any>{
    return this.http.post<any>(`${this.url}/CreditCart?phoneNumber=${phoneNumber}&credit=&nationalId=&bankCode=&getCart=${true}`, this.httpOptions);
  }

  IncreaseCredit(phoneNumber: string, credit: number, nationalId: string, bankCode: string){
    return this.http.post(`${this.url}/CreditCart?phoneNumber=${phoneNumber}&credit=${credit}&nationalId=${nationalId}&bankCode=${bankCode}&getCart=${false}`, this.httpOptions);
  }
}
