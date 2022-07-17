import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor { // เราจะเข้าถึง form control แต่ละอันที่นี้
  // formControlName='username' คือ native form control in the DOM (คือสิ่งที่เราต้องการเข้าถึงในตอนนี้)
  // ที่ TextInputComponent เลือก Implement interface 'ControlValueAccessor' เมื่อคลิกแล้วคุณจะได้ 4 method ให้เรา
  @Input() label: string;
  @Input() type = 'text'; // 'text' เป็นค่า  default

  // เราจะ inject control ลง constructor นี้
  // @self เป็น decorator พิเศษ
  constructor(@Self() public ngControl: NgControl) {// NgControl คือ baseclass ที่ form control directives (formControlName) extend มา
    this.ngControl.valueAccessor = this; // ทำให้เราสามารถเข้าถึง control (เช่นที่ registerForm)ของเรา ใน component  นี้ได้
    // เมื่อเราใช้ @Self() มันคือการบอก angular ว่า เราไม่ต้องการ re-use เพราะเราจะสร้าง instance เฉพาะ ขึ้นมา ทุกครั้งที่ใช้ component นี้ // โดยที่เราใส่ @Self() ที่ ngControl นั้นก็หมายความว่า เราต้องการให้แต่ละ input field ที่ใส่ค่า ngControl ให้มันมีความ unique (ไม่แบ่งค่าให้ใคร)
  } 
  // ใส่ method เข้ามาแต่ไม่ implements เพื่อให้มันผ่านเข้ามาเฉยๆ // เป็นแค่ required method ที่ต้องมีใน components นี้ เนื่องจาก implements มาจาก ControlValueAccessor (ถ้าไม่ใส่ มันจะ error แค่นั้น) (เราสร้าง component นี้ เพื่อใช้ template เป็นหลักเท่านั้น)
  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }

  // เราจะไม่ implement อะไรกับ OnInit
  // ngOnInit(): void {
  // }

}
