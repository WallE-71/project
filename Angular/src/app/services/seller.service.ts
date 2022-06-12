import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SellerService {
  constructor(private http: HttpClient) { }

  private url = environment.apiUrl + "/v1/SellerApi";
  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  public Request(request: any) {
    var imageFile = request.imageFile;
    var name = request.name;
    var surName = request.surName;
    var nationalId = request.nationalId;
    var phoneNumber = request.phoneNumber;
    var email = request.email;
    var webSite = request.webSite;
    var brand = request.brand;
    var activityType = request.activityType;
    var store = request.store;
    var telNumber = request.telNumber;
    var address = request.address;
    var postalCode = request.postalCode;
    var establishmentDate = request.establishmentDate;
    var sampleProduct = request.sampleProduct;
    var description = request.description;
    var scanNationalIdCart = request.scanNationalIdCart;
    var scanDocument = request.scanDocument;
    return this.http.post(`${this.url}?imageFile=${imageFile}&name=${name}&surName=${surName}&nationalId=${nationalId}&phoneNumber=${phoneNumber}
        &email=${email}&webSite=${webSite}&brand=${brand}&activityType=${activityType}&store=${store}&telNumber=${telNumber}&address=${address}
        &postalCode=${postalCode}&establishmentDate=${establishmentDate}&sampleProduct=${sampleProduct}&description=${description}
        &scanNationalIdCart=${scanNationalIdCart}&scanDocument=${scanDocument}`, this.httpOptions);
  }

  // public Request1(request: any) {
  //   var imageFile = request.imageFile;
  //   var name = request.name;
  //   var surName = request.surName;
  //   var nationalId = request.nationalId;
  //   var phoneNumber = request.phoneNumber;
  //   var email = request.email;
  //   var webSite = request.webSite;
  //   var brand = request.brand;
  //   var activityType = request.activityType;
  //   var store = request.store;
  //   var telNumber = request.telNumber;
  //   var address = request.address;
  //   var postalCode = request.postalCode;
  //   var establishmentDate = request.establishmentDate;
  //   var sampleProduct = request.sampleProduct;
  //   var description = request.description;
  //   var scanNationalIdCart = request.scanNationalIdCart;
  //   var scanDocument = request.scanDocument;

  //   var model:any = {
  //     imageFile: imageFile,
  //     name: name,
  //     surName: surName,
  //     nationalId: nationalId,
  //     phoneNumber: phoneNumber,
  //     email: email,
  //     webSite: webSite,
  //     brand: brand,
  //     activityType: activityType,
  //     store: store,
  //     telNumber: telNumber,
  //     address: address,
  //     postalCode: postalCode,
  //     establishmentDate: establishmentDate,
  //     sampleProduct: sampleProduct,
  //     description: description,
  //     scanNationalIdCart: scanNationalIdCart,
  //     scanDocument: scanDocument,
  //   }
  //   debugger

  //   return this.http.post<any>(this.url, JSON.stringify(model), this.httpOptions).pipe(
  //     catchError(this.errorHandler)
  //   );
  // }

  Answer(email: string): Observable<any> {
    return this.http.get<any>(`${this.url}?email=${email}`, this.httpOptions);
  }

  errorHandler(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent)
      errorMessage = error.error.message;
    else
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    return throwError(errorMessage);
  }
}
