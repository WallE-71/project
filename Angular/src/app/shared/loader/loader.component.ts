import { Component, OnInit } from '@angular/core';
import { LoaderService } from '../../services/loader.service';
import { NgProgress, NgProgressRef } from 'ngx-progressbar';

@Component({
  selector: 'loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.css'],
})
export class LoaderComponent implements OnInit {
  constructor(private progress: NgProgress, private loader: LoaderService) {
    this.progressRef = this.progress.ref('myProgress');
  }
  
  progressRef: NgProgressRef;

  startLoading() {
    this.progressRef.start();
  }

  completeLoading() {
    this.progressRef.complete();
  }

  ngOnInit(): void {
    this.loader.getLoading().subscribe((val: boolean) => {
      if (val) this.startLoading();
      else this.completeLoading();
    });
  }
}
