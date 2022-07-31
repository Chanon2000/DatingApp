import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  // OnDestroy เป็น Angular lifecycle methods ทำงานเมื่อย้ายไปอีก component อื่นแล้วทำให้ component ปัจุบัน ถูกทำลาย เป็นต้น
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user: User;

  constructor(
    public presence:PresenceService,
    private memberService: MembersService, 
    private route:ActivatedRoute, 
    private messageService: MessageService,
    private accountService: AccountService,
    private router: Router
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
    // เนื่องจาก ถ้าเราอยู่ที่ ?tab=3 อยู่แล้วแต่เป็นแชตกับ user คนอื่น พอมี user อีกคน แชตมาแล้วคุณไปคลิกที่ popup เพื่อย้ายหน้าไปที่แชตเขา มันจะเกิดปัญหาขึ้นนั้นคือข้อมูล message จะไม่ถูก load ขึ้นมา (เนื่องจาก angular เห็นว่าคุณเข้า route เดิมมันเลย reuse นั้นเอง)
    // เรา update route ไปเป็น route เดียวกับที่เราอยู่ในปัจจุบัน นั้นทำให้ angular มันทำการ trigger route เดิม แต่ไปอีก user นึ่ง (ซึ่งเราจะแก้ปัญหาโดยการ set การ reuse ให้เป็น false ตรงนี้)
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.member = data.member;
    })
    
    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

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

    this.galleryImages = this.getImages();
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

  loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.username); // สร้าง connection เมื่อมาที่ tab นี้
    } else {
      this.messageService.stopHubConnection(); // เมื่อย้ายไปที่ tab อื่นก็ให้ disconnection ด้วย
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection(); // เมื่อย้ายไป component อื่น ก็ให้ disconnection ด้วย
  }

}
