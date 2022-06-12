import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { SecurityService } from './security.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PayService {
  constructor(private http: HttpClient, public securityService: SecurityService) { }

  private Url = environment.apiUrl + "/v1/PayApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  // SaveData()
  // {
  //     return this.http.post()
  //         .map((res: Response) => res.json())
  //         .do(() => {
  //             console.log('request finished');
  //         });
  // }

  Payment(transport: any, discountCode: string) {
    const browserId = localStorage.getItem('browserId')!;
    const userId = this.securityService.GetFieldFromJWT('id')!;
    return this.http.get(`${this.Url}?userId=${userId}&browserId=${browserId}&transport=${transport}&discountCode=${discountCode}}`, this.httpOptions);
  }

  // Verify(guid: Guid, userId: number, Authority: string, Status: string){
  //   return this.http.post(`${this.Url}?guid=${guid}&userId=${userId}&Authority=${Authority}&Status=${Status}`, this.httpOptions)
  //   // .map((res: Response) => res.json())
  //   // .do(() => {
  //   //     console.log('request finished');
  //   // });
  // }
}
