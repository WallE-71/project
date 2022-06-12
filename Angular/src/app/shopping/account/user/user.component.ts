import { Component, ViewChild, ViewChildren, QueryList, OnInit, ElementRef } from '@angular/core';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { Time } from '@angular/common';
import { SecurityService } from '../../../services/security.service';
import { UserService } from '../../../services/user.service';
import { UserDto } from '../../../shared/models';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css'],
})
export class UserComponent implements OnInit {
  constructor(
    private element: ElementRef,
    public userService: UserService,
    public securityService: SecurityService) { }

  credit: number;
  duration: number;
  user: string = '';
  errors: string[] = [];
  reSendCode: boolean = false;
  lastCodeRecived: any[] = [];
  // @ViewChild('code') private code: ElementRef
  // @ViewChild('reSend') private reSend: ElementRef<Document>
  @ViewChildren('phoneNumber') private _notificationsElements: QueryList<ElementRef>

  RegisterOrLogin() {
    Swal.fire({
      html: `<h3>برای ثبت سفارش وارد شوید</h3><br/>
      <small> ایمیل*</small><br/>
      <input type="text" id="email" class="swal2-input" style="width: 380px;" placeholder="ایمیل"/><br/>
      <small>شماره موبایل*</small><br/>
      <input type="text" id="phoneNumber" class="swal2-input" style="width: 380px;" placeholder="شماره موبایل"/><br/>
      <small>ایمیل و شماره موبایل خود را وارد کنید تا کد تایید به ایمیل تان ارسال شود.</small>`,
      confirmButtonText: 'ارسال کد تایید',
      showCloseButton: true,
      padding: '30px',
      width: '650px',
      inputValidator: (value) => {
        return new Promise((resolve) => {
          console.log(value)
          if (value.length < 11)
            resolve('کد تایید برای ایمیل شما ارسال گردید');
          else
            resolve('لطفا ایمیل و شماره همراه را صحیح وارد نمایید!')
        });
      }
    }).then((result) => {
      if (result.isConfirmed) {
        var email = (<HTMLInputElement>document.getElementById('email')).value;
        var phoneNumber = (<HTMLInputElement>document.getElementById('phoneNumber')).value;
        // this.SendCode(email, phoneNumber);
        this.AccountUser(phoneNumber);
      }
      else if (result.isDenied) {
        Swal.fire(
          'محدودیت دسترسی'
        )
      }
    })
  }

  SendCode(email: string, phoneNumber: string, success: boolean = true) {
    if (success) {
      var codeRecived: string = "";
      this.userService.ReciveCode(email).subscribe((result: any) => {
        codeRecived = result.data;
        this.lastCodeRecived.push(result.data);
      });
    }

    var lastcode = "";
    var user = new UserComponent(this.element, this.userService, this.securityService);
    Swal.fire({
      html: success ? `<h4>کد ارسال شده به ایمیل خود را وارد نمایید</h4><br/>
      <input type="text" id="code" style="max-width: 300px" class="swal2-input" placeholder="کد تایید"/><br/>
      <small> <a #reSend id="reSend" style="cursor: pointer;">ارسال مجدد کد</a></small><br/>` :
        `<p>کد تایید را مجددا وارد نمایید!</p><br/>
      <input type="text" id="code" style="max-width: 300px" class="swal2-input" placeholder="کد تایید"/>`,
      confirmButtonText: 'تایید',
      showCloseButton: true,
      padding: '30px',
      width: '650px',
      footer: `<small id="seconde">ثانیه {{this.duration}} دریافت مجدد</small>`,
      inputValidator: (value) => {
        return new Promise((resolve) => {
          if (value.length == 5)
            resolve('منتظر بمانید...');
          else {
            this.reSendCode = true;
            resolve('کد تایید صحیح نمی باشد!')
          }
        });
      }
    }).then((result) => {
      if (result.isConfirmed) {
        var code = (<HTMLInputElement>document.getElementById('code')).value;
        lastcode = code;
        this.VerifyCode(code, codeRecived == undefined ? this.lastCodeRecived[0] : codeRecived, phoneNumber, email);
      }
    }).then((result: any) => {
      debugger
      user.reSendCode = false;
    });

    (<HTMLInputElement>document.getElementById('reSend')).addEventListener('click', (result: any) => {
      debugger
      if (user.reSendCode == true)
        user.SendCode(email, phoneNumber, true);
      else if (lastcode != "")
        alert('کد تایید ارسال شده است');
    });
  }

  VerifyCode(code: string, codeRecived: string, phoneNumber: string, email: string) {
    if (code == codeRecived)
      this.AccountUser(phoneNumber);
    else
      this.SendCode(email, phoneNumber, false);
  }

  AccountUser(phoneNumber: string) {
    this.userService.RegisterOrSignIn(phoneNumber).subscribe((result: UserDto) => {
      this.securityService.SaveData(result.data);
      if (result.data != null) {
        var fName = this.securityService.GetFieldFromJWT('firstName')!;
        var lName = this.securityService.GetFieldFromJWT('lastName')!;
        this.user = fName + lName;
      }
      else
        this.user = 'کاربر عزیز';
    });
  }

  GetUser() {
    const IsValid = this.securityService.IsAuthenticated();
    if (IsValid) {
      const lastName = this.securityService.GetFieldFromJWT('lastName')!;
      const firstName = this.securityService.GetFieldFromJWT('firstName')!;
      if (firstName != 'null')
        this.user = firstName + " " + lastName;
      else
        this.user = 'کاربر عزیز';
    }
    else
      this.user = '';
  }

  ngAfterViewInit() {
    this._notificationsElements.forEach((element: any) => {
      const htmlElement = element.nativeElement as HTMLElement;
      setTimeout(htmlElement.style.display = 'none', this.duration);
    });
  }

  ngOnInit(): void {
    this.GetUser();
  }
}
