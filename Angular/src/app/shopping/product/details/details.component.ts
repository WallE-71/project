import { Component, OnInit, Renderer2, ViewChild, QueryList, ElementRef, AfterViewInit } from '@angular/core';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { ActivatedRoute } from "@angular/router";
import { ShoppingCartComponent } from '../../cart/shoppingCart/shoppingCart.component';
import { SecurityService } from '../../../services/security.service';
import { ProductService } from '../../../services/product.service';
import { RatingService } from '../../../services/rating.service';
import { CartService } from '../../../services/cart.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class DetailsProductComponent implements OnInit, AfterViewInit {
  constructor(
    private element: ElementRef,
    private renderer: Renderer2,
    private route: ActivatedRoute,
    private userService: UserService,
    private cartService: CartService,
    private ratingsService: RatingService,
    private productService: ProductService,
    private securityService: SecurityService) { }

  seller: any = [];
  product: any = [];
  imageFiles: any = [];
  public productId: string;
  productsRelated: any = [];
  preferedProductsNoDiscount: any = [];
  @ViewChild('bookmark') public bookmark: ElementRef
  @ViewChild('image_list') public imageList?: QueryList<ElementRef<Element>>
  @ViewChild('active_image') public activeImage: ElementRef<HTMLImageElement>

  Details(productId: string) {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    this.productService.ProductDetails(productId, userId).subscribe((result: any) => {
      if (result.data != null) {
        if (result.data.product != null)
          this.imageFiles = result.data.product.imageFiles;
        this.product = result.data.product;
        this.seller = result.data.seller;
        this.productsRelated = result.data.productsRelated;
        this.preferedProductsNoDiscount = result.data.preferedProductsNoDiscount;
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

  Rating(sellerId: number, rate: number) {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    if (userId == '') {
      Swal.fire({
        icon: 'error',
        title: 'امکان امتیاز دهی نیست !!!',
        text: 'برای امتیاز دهی به فروشنده ابتدا باید وارد سایت شوید.',
        confirmButtonText: 'بستن'
      });
    }
    else {
      this.ratingsService.Rate(sellerId, this.productId, userId, rate).subscribe((result: any) => {
        if (result == false) {
          Swal.fire({
            icon: 'error',
            title: 'امکان امتیاز دهی نیست !!!',
            text: 'برای امتیازدهی به فروشنده باید محصولی از فروشنده خرید کرده باشید.',
            confirmButtonText: 'بستن'
          });
        }
      });
    }
  }

  changeImage(e: any) {
    this.activeImage.nativeElement.src = e.target.src;
  }

  ngAfterViewInit() {
    this.setBookmark();
    this.removeBookmark();
    if (this.imageList != null)
      this.imageList.forEach(image => image.nativeElement.addEventListener("click", this.changeImage));
  }

  ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.Details(this.productId);
  }
}
