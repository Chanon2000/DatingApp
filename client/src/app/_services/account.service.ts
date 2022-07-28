import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user:User) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role; // เพราะเมื่อ decode ออกมาจะได้ property หนึ่งในนั้นคือ role (คุณเลยสามารถ .role หลัง getDecodedToken ได้)
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles); // คือ if-else check ว่า roles มันเป็น array หรือป่าว
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null as any);
  }

  getDecodedToken(token) {
    // atob ทำการ decode information ใน token แล้ว return // token ไม่ได้ encrypted ที่ encrypted ในส่วนของ signature ที่อยู่ใน token
    return JSON.parse(atob(token.split('.')[1])); // แต่ละส่วนของ token มันแยกกันด้วย . โดยส่วนแรกคือ payload
  }
}
