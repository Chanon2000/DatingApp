import { HttpClient } from '@angular/common/http'; // แต่ถึงคุณจะ import สิ่งที่ไม่ได้ใช้ยังไง ตอน production นั้น angular ก็จะ remove ให้อยู่แล้ว
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

// #เนื่องจาก process token ของเรายังไม่ดีพอจึงต้องเปลี่ยนแปลงใหม่
// เพราะเรามีการ authentication ด้วย
// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: "Bearer " + JSON.parse(localStorage.getItem("user"))?.token // ใส่ ? เพราะการันทีไม่ได้ว่า จะได้ token มาทุกครั้ง
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers(): Observable<Member[]> {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
}
