import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { take } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  // 'editForm' คือชื่อที่ template, editForm คือชื่อที่ component
  member: Member;
  user: User;
  // @ = decorator
  // HostListener จะเข้าถึง event ของ browser ที่ระบุ (window:beforeunload)
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true; // ทำให้มี popup ขึ้นมาก่อนว่าคุณยังไม่ได้ save form ของคุณ เมื่อคุณ edit form ไปบางส่วนแล้ว
    }
  }

  constructor(
    private accountService: AccountService, 
    private memberService: MembersService,
    private toastr: ToastrService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => {
      this.member = member;
    })
  }

  updateMember() {
    console.log(this.member);
    this.toastr.success('Profile updated successfully');
    this.editForm.reset(this.member); // จะทำให้ reset form เพื่อให้alert หรือปุ่ม กลับไปอยู่ใน state เริ่มต้น
  }

}
