import { Component, OnInit, Inject, Renderer2, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { Router } from '@angular/router';
import { ShoppingCartComponent } from '../../cart/shoppingCart/shoppingCart.component';
import { SecurityService } from '../../../services/security.service';
import { ProductService } from '../../../services/product.service';
import { CartService } from '../../../services/cart.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-details-modal',
  templateUrl: './details-modal.component.html',
  styleUrls: ['./details-modal.component.css']
})
export class DetailsModalComponent implements OnInit, AfterViewInit {
  constructor(
    private router: Router,
    private element: ElementRef,
    private renderer: Renderer2,
    private userService: UserService,
    private cartService: CartService,
    private productService: ProductService,
    private securityService: SecurityService,
    public dialogRef: MatDialogRef<DetailsModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.productId = data.productId;
  }

  product: any = [];
  productsRelated: any = [];
  public productId: string;
  @ViewChild('bookmark') public bookmark: ElementRef

  onCancel(): void {
    this.dialogRef.close();
  }

  Details(productId: string) {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    this.productService.ProductDetails(this.productId.toString(), userId).subscribe((result: any) => {
      if (result.data != null) {
        this.product = result.data.product;
        this.productsRelated = result.data.productsRelated;
      }
    });
  }

  AddToCart(productId: string) {
    var shoppingCart = new ShoppingCartComponent(this.element, this.userService, this.cartService, this.securityService);
    shoppingCart.SaveItem(productId);
  }

  setBookmark() {
    this.renderer.setAttribute(this.bookmark.nativeElement, 'class', 'fa fa-bookmark');
  }

  removeBookmark() {
    this.renderer.setAttribute(this.bookmark.nativeElement, 'class', 'fa fa-bookmark-o');
  }

  Bookmark() {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    if (userId == null || userId == "") {
      Swal.fire({
        icon: 'error',
        title: 'امکان بوکمارک نیست !!!',
        text: 'برای بوکمارک کردن محصول ابتدا باید وارد سایت شوید.',
        confirmButtonText: 'بستن'
      });
    }
    else {
      this.productService.Bookmark(this.productId, userId).subscribe((result: any) => {
        if (result.data == true)
          this.setBookmark();
        else if (result.data == false)
          this.removeBookmark();
      });
    }
  }

  goDetails(id: string) {
    this.router.navigate(['/details', id]).then((result: any) => {
      if(result)
        this.onCancel();
    });
  }

  ngAfterViewInit() {
    this.setBookmark();
    this.removeBookmark();
  }

  ngOnInit(): void {
    this.Details(this.productId);
  }
}
