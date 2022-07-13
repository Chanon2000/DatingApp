import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private memberService: MembersService, private route:ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    // knownAs ของ member ขึ้นที่ template แต่ มันเกิด error ว่า ...undefined (reading 'knownAs')
    // เพราะว่า เมื่อ angular create template เสร็จ ข้อมูล user มันยังไม่มา ดังนั้นจึงแก้ด้วยการใส่ *ngIf="member" ที่ template

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

    // this.galleryImages = this.getImages(); // ใน ngOnInit จะเกิดขึ้นแบบ sync นั้นคือพอสั่ง loadMember มันจะมาทำอย่างอื่นต่อเลย ซึ่งพอมาถึง getImages() ข้อมูลรูปของ user มันยังไม่มา
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

}
