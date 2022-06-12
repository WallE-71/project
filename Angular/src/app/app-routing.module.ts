import { NgModule } from '@angular/core';
import { AuthGuard } from './shared/guards/authGuard';
import { RouterModule, Routes } from '@angular/router';

import { OrderComponent } from './shopping/order/order.component';
import { SellerComponent } from './shopping/seller/seller.component';
import { MessageComponent } from './shopping/message/message.component';
import { CategoryComponent } from './shopping/category/category.component';
import { ProfileComponent } from './shopping/account/profile/profile.component';
import { CommentComponent } from './shopping/product/comment/comment.component';
import { ProductComponent } from './shopping/product/product/product.component';
import { DetailsProductComponent } from './shopping/product/details/details.component';
import { SendCommentComponent } from './shopping/product/comment/send-comment/send-comment.component';
import { SubCommentsComponent } from './shopping/product/comment/sub-comments/sub-comments.component';

const routes: Routes = [
  { path: '', pathMatch: 'full',
    children: [
      { path: '', component: ProductComponent }]
  },
  { path: 'category/:title', component: CategoryComponent }, 
  { path: 'order', component: OrderComponent, canActivate: [AuthGuard] },
  { path: 'seller', component: SellerComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  { path: 'message', component: MessageComponent, canActivate: [AuthGuard] }, 
  { path: 'details/:id', component: DetailsProductComponent,
    children: [
      { path: 'comment', component: CommentComponent,
        children: [
          { path: 'sub-comments', component: SubCommentsComponent, canActivate: [AuthGuard] },
          { path: 'send-comment', component: SendCommentComponent, canActivate: [AuthGuard] }]
      }]
  },
  {
    path: 'cart',
    loadChildren: () => import('./shopping/cart/index-cart/cart.module').then(m => m.CartModule),
    data: { showHeader: false, showFooter: false }
  },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
