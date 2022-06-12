import { Component, OnInit, AfterViewInit, Input, ViewChild, ElementRef } from '@angular/core';
import { CommentService } from '../../../../services/comment.service';

@Component({
  selector: 'app-sub-comments',
  templateUrl: './sub-comments.component.html',
  styleUrls: ['./sub-comments.component.css']
})
export class SubCommentsComponent implements OnInit, AfterViewInit {
  constructor(private commentService: CommentService) { }

  panelOpenState = false;
  @Input('subComments') subComments: any; 
  @ViewChild('subCommentslike') public subCommentslike: ElementRef
  @ViewChild('subCommentsdislike') public subCommentsdislike: ElementRef

  LikeOrDisLikeOfSubComments(commentId: number, isLiked: any) {
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      this.commentService.LikeOrDisLike(commentId, browserId, isLiked).subscribe((result: any)=> {
        this.subCommentslike.nativeElement.textContent = " " + result.data.like + " ";
        this.subCommentsdislike.nativeElement.textContent = " " + result.data.dislike + " ";
      });
    }
  }

  ngAfterViewInit() {
  }

  ngOnInit(): void {
  }
}
