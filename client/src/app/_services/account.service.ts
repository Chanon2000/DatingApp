import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { User } from '../_models/user';

// Injectable decorator หมายความว่า service นี้สามารถ inject ลง component หรือ service อื่น ใน application ได้

// เกี่ยวกับ Services
// 1. service สามารถถูก inject ได้
// 2. service เป็น singleton นั้นคือ data ที่เราเก็บลง service จะไม่ถูกทำลายจนกว่า application จะถูกปิด
// (ต่างจาก component คือ เมื่อย้ายจาก component นึงไปอีก component นึง มันจะถูกทำลายเลย ต่างจาก service ที่เป็น singleton)
@Injectable({
  providedIn: 'root' // เรียก metadata
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      // การที่คุณทำ interface แล้วเอามาใช้ มันจะทำให้เห็น property ต่างๆ เมื่อ พิมพ์ . ต่อจากชื่อตัวแปรนั้นๆ ลงตัวแปลนั้นเป็ฯ vscode
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user:User) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    )
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null as any);
  }
}
