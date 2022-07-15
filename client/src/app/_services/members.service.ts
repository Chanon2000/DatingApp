import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  // service เป็น singletons ดังนั้น servide จึงเหมาะมากในการเก็บ state ของ application ของเรา
  // ความจริงมี state management solutions อื่นๆ เช่น Redux ใส่ลง application แต่ angular ไม่จำเป็นเพราะมี service อยู่แล้ว
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers(): Observable<Member[]> {
    if (this.members.length > 0) return of(this.members); // ถ้ามีข้อมูล member ใช้เลย ไม่ต้องยิงเรียกข้อมูลอีกที
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe( //เพื่อเก็บข้อมูล member ลง service
      map(members => {
        this.members = members; 
        return members; // members ตรงนี้ก็เป็น obv นะ (อยู่ใน map)
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(x => x.username === username); // ถ้าไม่เจออะไรเลย find จะ return undefined
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe( // เพื่อที่เมื่อ update แล้วจะได้ไม่ต้องเป็นยิงเพื่อดึงข้อมูลที่ update แล้วมา
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }
}
