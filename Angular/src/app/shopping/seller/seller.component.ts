import { Component, OnInit, Renderer2, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2/dist/sweetalert2.js';
import { SellerService } from '../../services/seller.service';
import { ImageCroppedEvent } from 'ngx-image-cropper';

@Component({
  selector: 'app-seller',
  templateUrl: './seller.component.html',
  styleUrls: ['./seller.component.css']
})
export class SellerComponent implements OnInit, AfterViewInit {
  constructor(
    private formBuilder: FormBuilder,
    private sellerService: SellerService) { }

  form: FormGroup;
  message: string;
  imgChangeEvt: any = '';
  cropImgPreview: any = '';
  @ViewChild('uploadDemo', { static: false }) uploadDemo: ElementRef<ElementCSSInlineStyle>;

  RequestRegister() {
    this.sellerService.Request(this.form.value).subscribe((result: any) => {
      if (result.data != null) {
        localStorage.setItem("jkk955", result.data);
        Swal.fire({
          icon: 'success',
          title: 'ثبت نام انجام شد !!!',
          text: 'لطفا منتظر پاسخ مدیریت سایت بمانید.',
          confirmButtonText: 'بستن'
        });
      }
      else {
        Swal.fire({
          icon: 'error',
          title: 'ثبت نام انجام نشد !!!',
          text: 'در مراحل ثبت نام خطایی رخ داده, لطفا پس ازمدت کوتاهی مجددا تلاش نمایید.',
          confirmButtonText: 'بستن'
        });
      }
    })
  }

  ReciveAnswer(){
    const email = localStorage.getItem("jkk955")!;
    this.sellerService.Answer(email).subscribe((result: any) => {
      if(result.data != null){
        this.message = result.data;
        localStorage.removeItem("jkk955");
      }
    })
  }

  // CropImage(){
  //   // Crop image
  //   var $uploadCrop:any;
  //   var tempFilename:any;
  //   var rawImg:any;
  //   var imageId:any;

  //   function readFile(input: any) {
  //       if (input.files && input.files[0]) {
  //           var reader = new FileReader();
  //           reader.onload = function (e: any) {
              
  //               ('.upload-demo').addClass('ready');
  //               ('#cropImagePop').modal('show');
  //               rawImg = e.target.result;
  //           }
  //           reader.readAsDataURL(input.files[0]);
  //       }
  //       else {
  //         Swal.fire({
  //           text: "Sorry - you're browser doesn't support the FileReader API",
  //         });
  //       }
  //   }

  //   $uploadCrop = ('#upload-demo').croppie({
  //       viewport: {
  //           width: 740,
  //           height: 500,
  //       },
  //       enforceBoundary: false,
  //       enableExif: true
  //   });

  //   ('#cropImagePop').on('shown.bs.modal', function () {
  //       $uploadCrop.croppie('bind', {
  //           url: rawImg
  //       }).then(function () {
  //           console.log('jQuery bind complete');
  //       });
  //   });

  //   ('.item-img').on('change', function () {
  //       imageId = (this).data('id'); tempFilename =(this).val();
  //       ('#cancelCropBtn').data('id', imageId); readFile(this);
  //   });

  //   ('#cropImageBtn').on('click', function (e: any) {
  //       $uploadCrop.croppie('result', {
  //           type: 'base64',
  //           format: 'jpeg',
  //           size: { width: 740, height: 500 }
  //       }).then(function (resp: any) {
  //           ('#item-img-output').attr('src', resp);
  //           ("#base64").val(resp.split(',')[1]);
  //           ('#cropImagePop').modal('hide');
  //       });
  //   });  
  // }

  onFileChange(event: any): void {
      this.imgChangeEvt = event;
  }

  cropImg(e: ImageCroppedEvent) {
      this.cropImgPreview = e.base64;
  }

  imgLoad() {
      // display cropper tool
  }
  
  initCropper() {
      // init cropper
  }
  
  imgFailed() {
      // error msg
  }

  ngAfterViewInit() {
      
  }

  ngOnInit(): void {
    this.ReciveAnswer();
    this.form = this.formBuilder.group({
      imageFile: ['', {
        validators: [Validators.required]
      }],
      name: ['', {
        validators: [Validators.required]
      }],
      surName: ['', {
        validators: [Validators.required]
      }],
      nationalId: ['', {
        validators: [Validators.required]
      }],
      phoneNumber: ['', {
        validators: [Validators.required]
      }],
      email: ['', {
        validators: [Validators.required]
      }],
      webSite: ['', {
        validators: [Validators.required]
      }],
      brand: ['', {
        validators: [Validators.required]
      }],
      activityType: ['', {
        validators: [Validators.required]
      }],
      store: ['', {
        validators: [Validators.required]
      }],
      telNumber: ['', {
        validators: [Validators.required]
      }],
      address: ['', {
        validators: [Validators.required]
      }],
      postalCode: ['', {
        validators: [Validators.required]
      }],
      establishmentDate: ['', {
        validators: [Validators.required]
      }],
      sampleProduct: ['', {
        validators: [Validators.required]
      }],
      description: ['', {
        validators: [Validators.required]
      }],
      scanNationalIdDocument: ['', {
        validators: [Validators.required]
      }]
    })
  }
}
