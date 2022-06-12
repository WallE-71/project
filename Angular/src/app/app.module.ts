import { RouterModule } from '@angular/router';
import { NgModule, Inject } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { NgxScrollTopModule } from 'ngx-scrolltop';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { ImageCropperModule } from 'ngx-image-cropper';
import { AppRoutingModule } from './app-routing.module';
import { NgxUsefulSwiperModule } from 'ngx-useful-swiper';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { ReactiveFormsModule, FormsModule } from '@angular/forms'
import { MaterialModule } from './shared/material/material.module';
import { JwtInterceptorService } from './services/jwt-interceptor.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';
import { HomeComponent } from './shopping/home/home.component';
import { OrderComponent } from './shopping/order/order.component';
import { ScrollComponent } from './shared/scroll/scroll.component';
import { LoaderComponent } from './shared/loader/loader.component';
import { SellerComponent } from './shopping/seller/seller.component';
import { UserComponent } from './shopping/account/user/user.component';
import { MessageComponent } from './shopping/message/message.component';
import { HeaderComponent } from './shopping/home/header/header.component';
import { FooterComponent } from './shopping/home/footer/footer.component';
import { CategoryComponent } from './shopping/category/category.component';
import { RatingComponent } from './shopping/seller/rating/rating.component';
import { CreditComponent } from './shopping/account/credit/credit.component';
import { ProfileComponent } from './shopping/account/profile/profile.component';
import { CommentComponent } from './shopping/product/comment/comment.component';
import { ProductComponent } from './shopping/product/product/product.component';
import { DetailsProductComponent } from './shopping/product/details/details.component';
import { ShoppingCartComponent } from './shopping/cart/shoppingCart/shoppingCart.component';
import { DetailsModalComponent } from './shopping/product/details-modal/details-modal.component';
import { SendCommentComponent } from './shopping/product/comment/send-comment/send-comment.component';
import { SubCommentsComponent } from './shopping/product/comment/sub-comments/sub-comments.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UserComponent,
    OrderComponent,
    ScrollComponent,
    LoaderComponent,
    HeaderComponent,
    FooterComponent,
    RatingComponent,
    SellerComponent,
    CreditComponent,
    ProfileComponent,
    ProductComponent,
    MessageComponent,
    CommentComponent,
    CategoryComponent,
    SendCommentComponent,
    SubCommentsComponent,
    ShoppingCartComponent,
    DetailsModalComponent,
    DetailsProductComponent,
  ],
  imports: [
    FormsModule,
    BrowserModule,
    CarouselModule,
    MaterialModule,
    HttpClientModule,
    AppRoutingModule,
    NgxScrollTopModule,
    ImageCropperModule,
    ReactiveFormsModule,
    NgxUsefulSwiperModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([]),
    SweetAlert2Module.forRoot()
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: JwtInterceptorService,
    multi: true
  },
  {
    provide: STEPPER_GLOBAL_OPTIONS,
    useValue: { showError: true }
  },
    Inject,
    Document
    // DOCUMENT
    // CookieService
  ],
  entryComponents: [
    DetailsModalComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
