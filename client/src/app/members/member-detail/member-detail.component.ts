import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;

  constructor(private memberService: MembersService, private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    // knownAs ของ member ขึ้นที่ template แต่ มันเกิด error ว่า ...undefined (reading 'knownAs')
    // เพราะว่า เมื่อ angular create template เสร็จ ข้อมูล user มันยังไม่มา ดังนั้นจึงแก้ด้วยการใส่ *ngIf="member" ที่ template
  }

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member => {
      this.member = member;
    })
  }

}
