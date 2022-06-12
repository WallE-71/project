import { Component, OnInit, AfterViewInit, Input, ViewChild , ElementRef } from '@angular/core';
import { SecurityService } from '../../../services/security.service';
import { ProductService } from '../../../services/product.service';
import { CommentService } from '../../../services/comment.service';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnInit, AfterViewInit {
  constructor(private productService: ProductService, private commentService: CommentService, private securityService: SecurityService) { }

  panelOpenState = false;
  subPanelOpenState = false;
  @Input() comment!: any;
  productComments: any = [];
  @Input() ProductId: string;
  @ViewChild('sublike') public sublike: ElementRef
  @ViewChild('subdislike') public subdislike: ElementRef
  @ViewChild('commentlike') public commentlike: ElementRef
  @ViewChild('commentdislike') public commentdislike: ElementRef
  @ViewChild('commentlike') public liked: ElementRef<ElementCSSInlineStyle>
  @ViewChild('commentdislike') public disliked: ElementRef<ElementCSSInlineStyle>
  
  Comments() {
    var userId = this.securityService.GetFieldFromJWT('id')!;
    this.productService.ProductDetails(this.ProductId, userId).subscribe((result: any) => {
      if (result.data?.productComments != null)
        this.productComments = result.data.productComments;
    });
  }

  LikeOrDisLike(commentId: number, isLiked: boolean) {
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      this.commentService.LikeOrDisLike(commentId, browserId, isLiked).subscribe((result:any)=> {
        this.commentlike.nativeElement.textContent = " " + result.data.like + " ";
        this.commentdislike.nativeElement.textContent =  " " + result.data.dislike + " ";
      });
    }
  }

  LikeOrDisLikeOfSubComments(commentId: number, isLiked: boolean) {
    var browserId = localStorage.getItem('browserId')!;
    if(browserId != null && browserId != 'undefined'){
      this.commentService.LikeOrDisLike(commentId, browserId, isLiked).subscribe((result:any)=> {
        this.sublike.nativeElement.textContent =  " " + result.data.like + " ";
        this.subdislike.nativeElement.textContent =  " " + result.data.dislike + " ";
      });
    }
  }

  ngAfterViewInit() {
  }

  ngOnInit(): void {
    this.Comments();
  }
}
