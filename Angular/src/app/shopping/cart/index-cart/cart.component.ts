import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { MatRadioChange } from '@angular/material/radio';
import { Router } from '@angular/router';
import { SecurityService } from '../../../services/security.service';
import { CartDto, CartItemsDto } from '../../../shared/models';
import { CartService } from '../../../services/cart.service';
import { PayService } from '../../../services/pay.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
  providers: [{
    provide: STEPPER_GLOBAL_OPTIONS, useValue: { showError: true }
  }]
})
export class CartComponent implements OnInit {
  constructor(
    private router: Router,
    private payService: PayService,
    private cartService: CartService,
    private formBuilder: FormBuilder,
    private securityService: SecurityService) { }

  transport: any;
  title = 'سبد خرید';
  errors: string[] = [];
  formGroup1: FormGroup;
  formGroup2: FormGroup;
  formGroup3: FormGroup;
  selectedColor: string ;
  cartItems: CartItemsDto[] = [];
  cart: CartDto = { productCount: 0, sumAmount: 0, data: this, cartItems: [] };

  loadItem() {
    const phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
    if (phoneNumber != null) {
      this.cartService.Cart(phoneNumber).subscribe((data: any) => {
        this.cart = data.data;
        this.cartItems = data.data.cartItems;
        debugger
        this.selectedColor = data.data.cartItems.selectedColor;
      });
    }
    else {
      this.cart = { productCount: 0, sumAmount: 0, data: this.cart, cartItems: [] };
      this.cartItems = [];
    }
  }

  SaveColor(productId: string, event: MatRadioChange) {
    var color = event.value.replace(/[^a-zA-Z0-9 ]/g, "");
    this.cartService.AddToCart(productId, color).subscribe();
  }

  AddCount(cartItemId: number) {
    this.cartService.Increase(cartItemId).subscribe();
  }

  LowCount(cartItemId: number) {
    this.cartService.Decrease(cartItemId).subscribe();
  }

  RequestPay() {
    debugger
    var transport = this.transport;
    var discountCode = this.formGroup2.value.discountCode;
    this.payService.Payment(transport, discountCode).subscribe((result: any) => {
      if (result.data == null)
        this.router.navigate(['/']);
      else {
        window.location.href = `https://sandbox.zarinpal.com/pg/StartPay/${result.data}`;

        // window.location.href = 'http://localhost:4200'
      }
      return transport;
    })
  }

  ngOnInit(): void {
    this.loadItem();
    this.formGroup1 = this.formBuilder.group({
      _color: ['', {
        validators: [Validators.required]
      }],
    });
    this.formGroup2 = this.formBuilder.group({
      discountCode: ['', {
        validators: [Validators.required]
      }],
      transport: ['', {
        validators: [Validators.required]
      }],
    });
    this.formGroup3 = this.formBuilder.group({
    });
  }
}
