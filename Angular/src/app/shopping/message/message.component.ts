import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MessageService } from '../../services/message.service';
import { MessageDto } from '../../shared/models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {
  constructor(private messageService: MessageService, private formBuilder: FormBuilder) { }

  author: string;
  Answer: string;
  form: FormGroup;
  result: string = '';
  email: string;

  SendMessage(){
    this.messageService.Send(this.form.value).subscribe((result: any) => {
      if(result.data != null){
        localStorage.setItem("_glq", result.data);
        Swal.fire({      
          icon:'success',
          title: "پیام شما با موفقیت ارسال شد و بعد از تایید منتظر پاسخ پیام باشید.",
          confirmButtonText: 'بستن'
        });
      }
    })
  }

  ReciveAnswer(){
    const email = localStorage.getItem("_glq")!;
    this.messageService.Answer(email).subscribe((result: any) => {
      if(result.data != null){
        this.Answer = result.answer;
        this.author = result.author;
        localStorage.removeItem("_glq");
      }
    })
  }

  ShowErrorMessage(message: any) {
    Swal.fire({        
        title: message.isSuccess,
        text: message,
        confirmButtonText: 'بستن'
    });
  }

  ngOnInit(): void {
    this.ReciveAnswer();
    this.form = this.formBuilder.group({
      email: ['', {
        validators: [Validators.required]
      }],
      typeFeedBack: ['', {
        validators: [Validators.required]
      }],
      description: ['', {
        validators: [Validators.required]
      }]
    })
  }
}
