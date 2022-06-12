import { Component, Renderer2, ViewChild, ViewChildren, QueryList, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css']
})
export class OrderComponent implements OnInit, AfterViewInit {
  constructor(private orderService: OrderService,  private renderer: Renderer2) { }

  orders: any = [];
  status: number = 0;
  @ViewChild('ready', { static: false }) ready: ElementRef<Element>;
  @ViewChild('confirm', { static: false }) confirm: ElementRef<Element>;
  @ViewChild('proccess', { static: false }) proccess: ElementRef<Element>;
  @ViewChild('delivery', { static: false }) delivery: ElementRef<Element>;
  @ViewChildren('step') public step!: QueryList<ElementRef<HTMLLIElement>>

  ShowOrder() {
    this.orderService.Order().subscribe((data: any) => {
      this.orders = data.data;
    });
  }

  RemoveOrder(orderId: number){
    this.orderService.Remove(orderId).subscribe((result: any) => {
      if (result.data == true) {

        // $("#remove").html(' style="display:none;transition: 0.5s linear;transition-timing-function: ease-in;');
      }
    });
  }

  stepsOrder(){
    if (this.step != null && this.step != undefined) {
      if (!this.proccess.nativeElement.classList.contains('acrive')) {
        this.renderer.setAttribute(this.proccess.nativeElement, 'class', 'active' );
      }
      else if (!this.confirm.nativeElement.classList.contains('acrive')) {
        this.renderer.setAttribute(this.confirm.nativeElement, 'class', 'active' );
      }
      else if (!this.delivery.nativeElement.classList.contains('acrive')) {
        this.renderer.setAttribute(this.delivery.nativeElement, 'class', 'active' );
      }
      else if (!this.ready.nativeElement.classList.contains('acrive')) {
        this.renderer.setAttribute(this.ready.nativeElement, 'class', 'active' );
      }
    }
  }
  
  ngAfterViewInit() {
    this.stepsOrder();
  }

  ngOnInit(): void {
    this.ShowOrder();
  }
}
