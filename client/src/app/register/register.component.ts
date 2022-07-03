import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() usersFromHomeComponent: any; // usersFromHomeComponent ตั้งชื่อตัวแปรแบบนี้เพื่อให้รู้ว่ามันมาจากใหน
  @Output() cancelRegister = new EventEmitter(); // EventEmitter เป็น class เราเลยต้องใส่ ()
  model: any = {};

  constructor() { }

  ngOnInit(): void {
  }

  register() {
    console.log(this.model);
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
