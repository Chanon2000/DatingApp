import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
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

  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('Hello', Validators.required),
      password: new FormControl('', [Validators.required, 
        Validators.minLength(4), Validators.maxLength(8)]), // เมื่อต้องการใส่มากกว่า 1 validator ให้ใส่ []
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
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
