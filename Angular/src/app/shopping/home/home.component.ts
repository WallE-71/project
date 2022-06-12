import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { ScrollComponent } from '../../shared/scroll/scroll.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private scrollComponent: ScrollComponent) { }

  showHeader = false;
  showFooter = false;

  ngOnInit() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.showHeader = this.activatedRoute.firstChild!.snapshot.data['showHeader'] !== false;
        this.showFooter = this.activatedRoute.firstChild!.snapshot.data['showFooter'] !== false;
      }
    });
    this.scrollComponent.ScrollToTop();
  }
}
