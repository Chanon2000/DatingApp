import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {1
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup; // group ของ form control
  // ใช้ form builder service เพื่อลด code เล็กน้อย (ใช้เพื่อสร้าง form)
  constructor(
    private accountService: AccountService, 
    private toastr: ToastrService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.fb.group({ // fb.group คือสร้าง group ของ form
       // ไม่ต้องสนลำดับของแต่ละ property
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, 
        Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity(); // เพื่อทุกครั้งเมื่อ password มีการเปลี่ยนแปลงค่า ก็ให้ไป check ตัวว่ามันตรงกับ confirmPassword มั้ย
    })
  }

  matchValues(matchTo: string): ValidatorFn { // ValidatorFn คือ method นี้ return validator function
    return (control: AbstractControl) => { // control นี้คือ confirmPassword เพราะเราจะเอา Validator นี้ไปแนบที่ confirmPassword control
      return control?.value === control?.parent?.controls[matchTo].value 
        ? null : {isMatching: true};
      // isMatching เป็น make up name field เพื่อเมื่อเวลามันไม่ match กัน มันจะไปเพิ่ม field isMatching ที่ error ของ control ที่แทบ validatior ตัวนี้(ซึ่งทำให้เราสามารถรู้ได้ว่ามันไม่ match)
      // isMatching: true หมายถึง isMatching มี error เกิดขึ้นเป็นจริง
    }
  }

  register() {
    console.log(this.registerForm.value);
    // this.accountService.register(this.model).subscribe(response => {
    //   // console.log(response);
    //   this.cancel();
    // }, error => {
    //   console.log(error);
    //   this.toastr.error(error.error);
    // })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
