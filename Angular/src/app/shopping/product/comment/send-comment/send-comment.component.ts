import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { ActivatedRoute } from "@angular/router";
import { SecurityService } from '../../../../services/security.service';
import { CommentService } from '../../../../services/comment.service';

@Component({
  selector: 'app-send-comment',
  templateUrl: './send-comment.component.html',
  styleUrls: ['./send-comment.component.css']
})
export class SendCommentComponent implements OnInit {
  constructor(private commentService: CommentService, private securityService: SecurityService, private formBuilder: FormBuilder, private route: ActivatedRoute) { }

  form: FormGroup;
  productId: any;
  parentCommentId: any;
  @Input() ProductId: string;
  @Input() ParentCommentId: number;

  SendComment() {
    var browserId = localStorage.getItem('browserId')!;
    if (browserId != null && browserId != 'undefined') {
      var name = this.form.value.name;
      var email = this.form.value.email;
      var description = this.form.value.description;

      this.commentService.SendComment(name, email, description, this.ProductId, this.ParentCommentId).subscribe((result: any) => {
        debugger
        if (result.isSuccess) {
          return Swal.fire({
            icon: 'success',
            title: result.data,
            confirmButtonText: 'بستن',
          })
        }
        return result.data;
      });
    }
    else {
      return Swal.fire({
        icon: 'error',
        title: 'ابتدا وارد شوید!',
        confirmButtonText: 'بستن',
      })
    }
    return null;
  }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: ['', {
        validators: [Validators.required]
      }],
      email: ['', {
        validators: [Validators.required]
      }],
      description: ['', {
        validators: [Validators.required]
      }]
    })
  }
}
