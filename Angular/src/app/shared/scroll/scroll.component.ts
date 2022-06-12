import { Component, OnInit, Injectable, Inject, HostListener } from '@angular/core';
import { DOCUMENT } from "@angular/common";

@Injectable({
  providedIn: 'root'
})
@Component({
  selector: 'app-scroll',
  templateUrl: './scroll.component.html',
  styleUrls: ['./scroll.component.css']
})
export class ScrollComponent implements OnInit {
  constructor(@Inject(DOCUMENT) private document: Document) { }
  
  windowScrolled: boolean;
  @HostListener("window:scroll", [])

  OnScroll() {
     debugger
    window.onscroll = (e: any) => {
      debugger
      if (window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop > 100) {
        this.windowScrolled = true;
        return true;
      }
      // else if (this.windowScrolled && window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop < 10) {
      //   this.windowScrolled = false;
      // }
      return false;
    }
    return false;
  }

  ScrollToTop() {
    (function smoothscroll() {
      var currentScroll = document.documentElement.scrollTop || document.body.scrollTop;
      if (currentScroll > 0) {
        window.requestAnimationFrame(smoothscroll);
        window.scrollTo(0, currentScroll - (currentScroll / 8));
      }
    })();
  }

  ngOnInit() { 
    
  }
}
