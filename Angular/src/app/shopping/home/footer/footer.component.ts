import { Component, OnInit } from '@angular/core';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { NewsletterService } from '../../../services/newsletter.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  constructor(private newsletterService: NewsletterService, private formBuilder: FormBuilder) { }

  email: string;
  form: FormGroup;

  NewsLetter() {
    this.newsletterService.Send(this.email).subscribe((result: any) => {
      if (result.data == null) {
        result.data = result.message;
      }
      return Swal.fire({
        title: result.data,
        confirmButtonText: 'بستن'
      });
    })
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      email: ['', {
        validators: [Validators.required]
      }]
    })
  }
}
