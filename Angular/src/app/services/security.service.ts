import { Injectable } from '@angular/core';
// import { CookieService } from 'ngx-cookie-service';
import { decode_utf8 } from '../shared/utility';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  private readonly roleField = "role";
  private readonly token: string = "token";
  private readonly browserId: string = "browserId";
  private readonly expiration: string = "expiration";

  SignOut() {
    localStorage.removeItem('browserId');
    localStorage.removeItem('token');
    localStorage.removeItem('expiration');
  }

  SaveData(response: any) {
    if (response.browserId != null)
      localStorage.setItem('browserId', response.browserId);
    if (response.token != null)
      localStorage.setItem('token', response.token.token);
    if (response.token != null)
      localStorage.setItem('expiration', response.token.expiration);
  }

  GetFieldFromJWT(field: string): string {
    const token = this.GetToken();
    if (!token) { return ''; }
    const dataToken = JSON.parse(atob(token.split('.')[1]));

    if (field != 'phoneNumber' && field != 'email')
      return decode_utf8(dataToken[field]);
    else
      return dataToken[field]
  }

  IsAuthenticated(): boolean {
    const token = this.GetToken();
    if (!token) return false;

    const expiration = this.GetExpirationDate();
    const expirationDate = new Date(expiration);
    if (expirationDate <= new Date()){
      this.SignOut();
      return false;
    }
    return true;
  }

  GetRole(): string {
    return this.GetFieldFromJWT(this.roleField)!;
  }

  GetToken(){
    return localStorage.getItem(this.token)!;
  }

  GetExpirationDate(){
    return localStorage.getItem(this.expiration)!;
  }

  GetBrowserId(){
    return localStorage.getItem(this.browserId);
  }
}
