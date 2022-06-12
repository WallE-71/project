import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { Router } from '@angular/router';
import { ShoppingCartComponent } from '../cart/shoppingCart/shoppingCart.component';
import { CategoryService } from '../../services/category.services';
import { SecurityService } from '../../services/security.service';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent implements OnInit, AfterViewInit {
  constructor(
    private router: Router,
    private element: ElementRef,
    private route: ActivatedRoute,
    private userService: UserService,
    private cartService: CartService,
    private productService: ProductService,
    private categoryService: CategoryService,
    private securityService: SecurityService) { }

  parentName: any;
  valueFilter: any;
  discountType: any;
  products: any = [];
  subCategories: any = [];
  @ViewChild('pillActive', { static: false }) btn: ElementRef;

  GetProductInCategory(categoryName: string) {
    this.productService.GetProductInCategory(categoryName).subscribe((data: any) => {
      this.products = data.data.products; 
      this.discountType = data.data.discountType;
    });
  }

  GetProductInCategoryByFilter(filterId: any, filterByCheched: any) {
    this.productService.GetProductInCategoryByFilter(this.parentName, filterId, filterByCheched).subscribe((data: any) => {
       this.products = data.data; 
    });
  }

  LoadSubCategories(parentName: string) {
    this.categoryService.SubCategories(parentName).subscribe((data: any) => {
      this.subCategories = data.data;
    });
  }

  Details(id: string) {
    this.router.navigate(['/details', id]);
  }

  AddToCart(productId: string) {
    var shoppingCart = new ShoppingCartComponent(this.element, this.userService, this.cartService, this.securityService);
    shoppingCart.SaveItem(productId);
  }

  ngAfterViewInit() {
    window.onload =() => {
      if (this.btn != null && this.btn != undefined)
          this.btn.nativeElement.click();
      else{
        window.setTimeout(() => {
            this.btn.nativeElement.click();
        }, 7000);
      }
    }
  }

  ngOnInit(): void {
    this.parentName = this.route.snapshot.paramMap.get('title')!;
    this.LoadSubCategories(this.parentName);
  }
}
