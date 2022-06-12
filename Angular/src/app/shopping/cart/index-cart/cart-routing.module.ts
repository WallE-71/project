import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../../../shared/guards/authGuard';
import { PayWaysComponent } from '../pay-ways/pay-ways.component';
import { CartComponent } from './cart.component';

const routes: Routes = [
  {
    path: '', component: CartComponent,
    children: [
      { path: '', component: PayWaysComponent }]
    // , canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CartRoutingModule { }
