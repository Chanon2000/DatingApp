import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Member } from "../_models/member";
import { MembersService } from "../_services/members.service";

@Injectable({
    providedIn: 'root'
})

// เหมือนคือ class ที่ทำตอนที่คุณ activate route ซึ่งก็คือมันทำก่อน สร้าง template
export class MemberDetailedResolver implements Resolve<Member> {

    constructor(private memberService: MembersService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Member> {
        return this.memberService.getMember(route.paramMap.get('username')); // ทำให้เราสามารถ get member ก่อนที่ template จะสร้างเสร็จได้
        // getMember ไปดึงข้อมูล member จาก cache มา
        // และจะเห็นว่าเราไม่ต้อง subscribe ที่ getMember ก่อนจะ return เพราะ router มันจัดการเรื่องนี้ให้เราแล้ว เมื่อเราเอา resolve นี้ ไปใส่ที่ property ของ Routes ที่ app-routing
    }

}