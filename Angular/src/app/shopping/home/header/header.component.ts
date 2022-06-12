import { Component, Renderer2, ElementRef, ViewChild, OnInit, AfterViewInit, OnDestroy, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { Router } from '@angular/router';
import { ScrollComponent } from '../../../shared/scroll/scroll.component';
import { CategoryService } from '../../../services/category.services';
import { ProductService } from '../../../services/product.service';
import { TreeViewCategoryDto } from '../../../shared/models';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, AfterViewInit, OnDestroy {
  constructor(private categoryService: CategoryService, private productService: ProductService, private scrollComponent: ScrollComponent, private formBuilder: FormBuilder, private renderer: Renderer2, private elementRef: ElementRef, private router: Router) {
    this.form = formBuilder.group({
      productName: ['', {
        validators: [Validators.required]
      }]
    })
  }
  form: FormGroup;
  @Input() productName: any;
  categories: TreeViewCategoryDto[] = [];
  @ViewChild('navOverlay', { static: false }) navOverlay: ElementRef

  @ViewChild('collapse', { static: false }) collapse: ElementRef<Element>
  @ViewChild('categoriesMenu', { static: false }) categoriesMenu: ElementRef<Document>
  @ViewChild('collapse', { static: false }) collapseStyle: ElementRef<ElementCSSInlineStyle>

  private scrollChangeCallback: () => void;
  @ViewChild('firstSection', { static: false }) firstSection: ElementRef
  @ViewChild('closeMenu', { static: false }) closeMenu: ElementRef<Element>
  @ViewChild('secondaryHeader', { static: false }) secondaryHeader: ElementRef
  @ViewChild('secondarySearch', { static: false }) secondarySearch: ElementRef<Element>

  Search() {
    var productName = this.form.value.productName;
    this.productService.Search(productName).subscribe((result: any) => {
      if (result.isSuccess == true)
        this.router.navigate(['/details', result.data]);
      else {
        return Swal.fire({
          title: result.data,
          confirmButtonText: 'بستن'
        });
      }
      return productName;
    });
  }

  LoadCategories() {
    this.categoryService.GetCategories().subscribe((data: any) => {
      this.categories = data.data;
    });
  }

  Toggle() {
    this.collapse.nativeElement.classList.toggle('show');
  }

  closeCategoriesCollapse() {
    this.categoriesMenu.nativeElement.addEventListener('click', (event) => {
      this.collapseStyle.nativeElement.style.display = 'flex';

      window.onclick = (e: any) => {
        if (this.collapse.nativeElement.previousSibling != e.target)
          this.collapseStyle.nativeElement.style.display = 'none';
      }
    });
  }

  MouseEnter() {
    this.renderer.addClass(this.collapse.nativeElement, 'active');
    this.renderer.addClass(this.secondarySearch.nativeElement, 'active');

    this.renderer.addClass(this.navOverlay.nativeElement, 'active');
  }

  MouseLeave() {
    this.renderer.removeClass(this.navOverlay.nativeElement, 'active');
  }

  animatedHeaderOnScroll() {
    let scroll = document.documentElement.scrollTop;
    var firstSection = this.firstSection.nativeElement.offsetHeight;
    var headerHeight = this.secondaryHeader.nativeElement.offsetHeight;

    if (scroll > headerHeight * 3) {
      this.renderer.addClass(this.secondaryHeader.nativeElement, 'fixed');
      this.renderer.addClass(this.closeMenu.nativeElement, 'fixed');
      this.renderer.addClass(this.secondarySearch.nativeElement, 'fixed');
    }
    else {
      this.renderer.removeClass(this.secondaryHeader.nativeElement, 'fixed');
      this.renderer.removeClass(this.closeMenu.nativeElement, 'fixed');
      this.renderer.removeClass(this.secondarySearch.nativeElement, 'fixed');
      this.secondaryHeader.nativeElement.style.display = 'inline-block';
    }
    if (scroll > firstSection * 4) {
      this.renderer.addClass(this.secondaryHeader.nativeElement, 'in-view');
      this.renderer.addClass(this.closeMenu.nativeElement, 'in-view');
      this.renderer.addClass(this.secondarySearch.nativeElement, 'in-view');
    } 
    else {
      this.renderer.removeClass(this.secondaryHeader.nativeElement, 'in-view');
      this.renderer.removeClass(this.closeMenu.nativeElement, 'in-view');
      this.renderer.removeClass(this.secondarySearch.nativeElement, 'in-view');
    }
  }
  
  CloseHeader() {
    this.secondaryHeader.nativeElement.style.display = 'none';
  }

  scrolling() {
    this.scrollChangeCallback = () => this.animatedHeaderOnScroll();
    window.addEventListener('scroll', this.scrollChangeCallback, true);
  }

  ngOnDestroy() {
    window.removeEventListener('scroll', this.scrollChangeCallback, true);
  }

  ngAfterViewInit() {
    this.scrolling();
    this.MouseEnter();
    this.MouseLeave();
    this.closeCategoriesCollapse();
    this.elementRef.nativeElement.querySelector('.close-header').addEventListener('click', this.CloseHeader.bind(this));
  }

  ngOnInit(): void {
    this.LoadCategories();
  }
}
