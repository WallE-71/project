import { Component, OnInit, ElementRef } from '@angular/core';
import { OwlOptions } from 'ngx-owl-carousel-o';
import { SwiperOptions } from 'swiper';
import { DetailsModalComponent } from '../details-modal/details-modal.component';
import { ProductService } from '../../../services/product.service';
import { SliderService } from '../../../services/slider.service';
import { ProductDTO } from '../../../shared/models';
import { MatDialog } from '@angular/material/dialog';
import Swiper, { Autoplay } from 'swiper';
Swiper.use([Autoplay]);

@Component({
  selector: '....',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {
  constructor(
    public dialog: MatDialog,
    private sliderService: SliderService,
    private productService: ProductService) {
    // setInterval(() => this.ngOnInit(), 5000);
  }

  product1: any = [];
  product2: any = [];
  product3: any = [];
  product4: any = [];
  product5: any = [];
  product6: any = [];
  product7: any = [];
  product8: any = [];
  product9: any = [];
  product10: any = [];
  product11: any = [];
  product12: any = [];
  product13: any = [];
  product14: any = [];
  product15: any = [];

  sliders: any = [];

  imageFiles: any = [];

  mostTalkProduct: any;
  preferedProducts: any;
  mostViewedProduct: any;
  mostPopularProducts: any;
  mostDiscountProducts: any;
  specialDiscountProducts: any;
  preferedProductsNoDiscount: any;
  dayOfDiscount: string = 'سه شنبه';

  customOptions: OwlOptions = {
    rtl: true,
    loop: false,
    dots: false,
    rewind: false,
    mouseDrag: true,
    touchDrag: true,
    pullDrag: false,
    // nav: true,
    // navSpeed: 600,
    // navText: ['<i class="fa fa-angle-right"></i>'],
    responsive: {
      0: {
        items: 1,
        slideBy: 1
      },
      576: {
        items: 1,
        slideBy: 1
      },
      768: {
        items: 4,
        slideBy: 2
      },
      992: {
        items: 4,
        slideBy: 2
      },
      1400: {
        items: 5,
        slideBy: 3
      }
    }
  }

  config: SwiperOptions = {
    pagination: false,
    // loop: true,
    spaceBetween: 30,
    effect: "fade",
    allowSlideNext: true,
    allowSlidePrev: true,
    autoplay: {
      // waitForTransition: true,
      delay: 3000
    }
  }

  Details(id: any) {
    const dialogRef = this.dialog.open(DetailsModalComponent, {
      width: '70%',
      data: { productId: id }
    });

    // dialogRef.afterClosed().subscribe((result: any) => {
    //   console.log(`Dialog result: ${result}`);
    // });
  }

  LoadProducts() {
    this.productService.GetProductInCategory("لبنیات").subscribe((data: any) => { this.product1 = data.data.products; });
    this.productService.GetProductInCategory("خواربار و نان").subscribe((data: any) => { this.product2 = data.data.products; });
    this.productService.GetProductInCategory("دستمال و شوینده").subscribe((data: any) => { this.product3 = data.data.products; });
    this.productService.GetProductInCategory("تنقلات").subscribe((data: any) => { this.product4 = data.data.products; });
    this.productService.GetProductInCategory("نوشیدنی").subscribe((data: any) => { this.product5 = data.data.products; });
    this.productService.GetProductInCategory("مواد پروتئینی").subscribe((data: any) => { this.product6 = data.data.products; });
    this.productService.GetProductInCategory("آرایشی و بهداشتی").subscribe((data: any) => { this.product7 = data.data.products; });
    this.productService.GetProductInCategory("چاشنی و افزودنی").subscribe((data: any) => { this.product8 = data.data.products; });
    this.productService.GetProductInCategory("میوه و سبزیجات تازه").subscribe((data: any) => { this.product9 = data.data.products; });
    this.productService.GetProductInCategory("کنسرو و غذای آماده").subscribe((data: any) => { this.product10 = data.data.products; });
    this.productService.GetProductInCategory("صبحانه").subscribe((data: any) => { this.product11 = data.data.products; });
    this.productService.GetProductInCategory("خشکبار، دسر و شیرینی").subscribe((data: any) => { this.product12 = data.data.products; });
    this.productService.GetProductInCategory("خانه و سبک زندگی").subscribe((data: any) => { this.product13 = data.data.products; });
    this.productService.GetProductInCategory("کودک و نوزاد").subscribe((data: any) => { this.product14 = data.data.products; });
    this.productService.GetProductInCategory("مد و پوشاک").subscribe((data: any) => { this.product15 = data.data.products; });
  }

  Index() {
    this.productService.Index().subscribe((data: any) => {
      this.mostTalkProduct = data.data.mostTalkProduct;
      this.preferedProducts = data.data.preferedProducts;
      this.mostViewedProduct = data.data.mostViewedProduct;
      this.mostPopularProducts = data.data.mostPopularProducts;
      this.mostDiscountProducts = data.data.mostDiscountProducts;
      this.specialDiscountProducts = data.data.specialDiscountProducts;
      this.preferedProductsNoDiscount = data.data.preferedProductsNoDiscount;
    });
  }

  GetSliders() {
    this.sliderService.GetSliders().subscribe((data: any) => {
      this.sliders = data.data;
    });
  }

  ngOnInit(): void {
    this.GetSliders();
    this.LoadProducts();
    this.Index();
  }
}
