import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];

  constructor(
    private memberService: MembersService, 
    private route:ActivatedRoute, 
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadMember();
    
    // สามารถ subscribe queryParams ได้
    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0); // เขียนแบบ ternary operator
    })
    // มันจะ error ว่า tab undifind นั้นเป็นเพราะว่า คุณจะเห็นที่บรรทัดแรกของ template จะมีเงื่อนไขคือ *ngIf="member" นั้นหมายความว่า #memberTabs ที่ template จะไม่ถูกสร้างจนกว่า member จะมีค่า
    // การใส่ {static: true} ไปที่ ViewChild ซึ่งก็ไม่ได้ช่วงแก้ปัญหานี้เพราะมันยังเร็วไม่พอ แต่มันทำให้เราใช้ dynamic version ของ your child
    // วิธีที่ทำให้ error นี้หายคือ เอา *ngIf="member" ออก แต่ก็ยังทำให้เกิด error ที่จุดอื่นอยู่ นั้นก็คือ member จะ undifind แทน ซึ่งคุณก็สามารถแก้ปัญหาได้ด้วยการใส่ ? ในทุกที่ แต่ก็มันทำให้ดูไม่ค่อย clean เท่าใหร่ เราเลยจะใช้ resolvers แทน

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
  }

  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    }
    return imageUrls;
  }

  loadMember() {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member => {
      this.member = member;
      this.galleryImages = this.getImages();
    })
  }

  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true; // tabs เป็น property ใน TabDirective
  }

  // ทำให้มัน loadMessages ตอนที่คลิกที่ Messages Tab (วิธีก่อนหน้ามันจะ loadMessage หลังจาก loadMember เสร็จเลย ทำให้การ loading มีพฤติกรรมแปลกๆ)
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.loadMessages();
    }
  }

}
