import { Component, OnInit, ElementRef } from '@angular/core';
import { SecurityService } from '../../../services/security.service';
import { UserComponent } from '../../account/user/user.component';
import { CartDto, CartItemsDto } from '../../../shared/models';
import { CartService } from '../../../services/cart.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-shoppingCart',
  templateUrl: './shoppingCart.component.html',
  styleUrls: ['./shoppingCart.component.css'],
})
export class ShoppingCartComponent implements OnInit {
  constructor(
    private element: ElementRef,
    private userService: UserService,
    private cartService: CartService,
     private securityService: SecurityService){}

  status: number = 0;
  errors: string[] = [];
  cartItems: CartItemsDto[];
  cart: CartDto = { productCount: 0, sumAmount: 0, data: this, cartItems: [] };
  private phoneNumber = this.securityService.GetFieldFromJWT('phoneNumber')!;
  
  LoadCart() {
    if (this.phoneNumber != null) {    
      this.cartService.Cart(this.phoneNumber).subscribe((result: any) => {
        if(result.data != null){
          this.cart = result.data;
          this.cartItems = result.data?.cartItems;
        }
      });
    }
    else {
      this.cart = { productCount: 0, sumAmount: 0, data: this.cart, cartItems: [] };
      this.cartItems = [];
    }
  }

  GetStatus() {
    const address = this.securityService.GetFieldFromJWT('address')!;
    if (this.phoneNumber! != "null" && address == "")
      this.status = 1;
    else if (this.phoneNumber! === null)
      this.status = 0;
    else if (address != "null")
      this.status = 2;
  }

  SaveItem(productId: string) {
    if (productId != null)
      this.cartService.AddToCart(productId, '').subscribe();
  }

  DeleteItem(productId: number) {
    this.cartService.Delete(productId).subscribe();
  }
  
  DeleteAll() {
    this.cartService.DeleteAll().subscribe();
  }
  
  RegisterOrLogin() {
    var user = new UserComponent(this.element,this.userService, this.securityService);
    user.RegisterOrLogin(),
    user.ngOnInit();
  }

  ngOnInit(): void {
    this.LoadCart();
    this.GetStatus();
    if (this.status == 0) {
      this.cart = { productCount: 0, sumAmount: 0, data: this.cart, cartItems: [] };
      this.cartItems = [];
    }
  }
}
