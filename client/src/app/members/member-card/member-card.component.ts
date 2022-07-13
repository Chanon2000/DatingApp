import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
  // จะมี _ngcontent-qsu-c67 (property แปลงๆนี้) ใส่ให้ทุก html ในแต่ละ tag โดยถ้าอยู่คนละ component นั้น property นี้ จะแตกต่างกัน (ทำให้ style นี้คุณเขียนไม่ถูกใช้งานใน component อื่น)
  // encapsulation // แต่เราไม่ใช้ ให้มันเป็น default แบบนี้แหละ
  // default => จะทำการ encep แล้วทำให้ style อยู่แค่ใน component
  // ViewEncapsulation.None => ก็คือปิดการ encap ทำให้ style จะเป็น global เลย
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member;

  constructor() { }

  ngOnInit(): void {
  }

}
