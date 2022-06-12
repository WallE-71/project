import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MaterialModule } from '../../../shared/material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CartRoutingModule } from './cart-routing.module';

import { PayWaysComponent } from '../pay-ways/pay-ways.component';
import { CartComponent } from './cart.component';

@NgModule({
  declarations: [
    CartComponent,
    PayWaysComponent
  ],
  imports: [
    FormsModule,
    CommonModule,
    MaterialModule,
    CartRoutingModule,
    ReactiveFormsModule
  ]
})
export class CartModule { }
