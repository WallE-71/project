import { Component, OnInit, AfterViewInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { ScrollComponent } from './shared/scroll/scroll.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css', './app.component.scss']
})
export class AppComponent implements OnInit, AfterViewInit {
  constructor(private scrollComponent: ScrollComponent, private renderer: Renderer2) { }

  user: string = '';
  title = 'اسنپ مارکت';
  windowScrolled: boolean;
  private updateProgress: () => void;
  @ViewChild('progress', { static: false }) progress: ElementRef<Document>;

  ScrollToTop() {
    var progressPath: any;
    progressPath = document.querySelector('.progress-wrap path')!;
    if (progressPath != null) {
      var pathLength = progressPath.getTotalLength();
      progressPath.style.transition = progressPath.style.WebkitTransition = 'none';
      progressPath.style.strokeDasharray = pathLength + ' ' + pathLength;
      progressPath.style.strokeDashoffset = pathLength;
      progressPath.getBoundingClientRect();
      progressPath.style.transition = progressPath.style.WebkitTransition = 'stroke-dashoffset 10ms linear';
      this.updateProgress = () => {
        var scroll = document.documentElement.scrollTop;
        var height = document.documentElement.offsetHeight - window.innerHeight;
        var progress = pathLength - (scroll * pathLength / height);
        progressPath.style.strokeDashoffset = progress;
      }
    }
    window.addEventListener('scroll', this.updateProgress, true);
    window.addEventListener('scroll', () => {
      if (document.documentElement.scrollTop > 50)
        this.renderer.addClass(this.progress.nativeElement, 'active-progress');
      else
        this.renderer.removeClass(this.progress.nativeElement, 'active-progress');
    });
    this.progress.nativeElement?.addEventListener('click', (event: any) => {
      event.preventDefault();
      window.scrollTo(0, 0);
      // document.querySelector('html, body')!.animate({ scrollTop: 0 }, 1500);
      return false;
    });
  }

  ngAfterViewInit(){
    this.ScrollToTop();
  }

  ngOnInit(): void {
    
  }
}
