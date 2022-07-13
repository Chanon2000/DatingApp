import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';

@Injectable()
export class JwtInterceptor implements HttpInterceptor { // Interceptor จะถูก init แค่ครั้งเดียวตอนเริ่ม app (เพราะมันเป็นส่วนนึงของ app module เนื่องจากคุณใส่มันใน providers และจะอยู่จนกว่าคุณจะปิด app)

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // ใช้ const ไม่ได้ เพราะถ้าใช้ต้องใส่ค่าเริ่มต้นด้วย
    let currentUser: User;
    // เราต้อง subscribe เพื่อเอาค่าออกมาจากมัน และต้อง unsubscribe ด้วย
    this.accountService.currentUser$.pipe(take(1)).subscribe((user:User) => currentUser = user);
    // take(1) เอาค่าที่ emit มาแค่ค่าเดียว แล้ว complete เลย นั้นก็คือทำการ unsubscribe เลย

    if (currentUser) {
      request = request.clone({ // clone ก็คือเหมือนคัดลอกมานั้นแหละ แล้วเอามาเพื่อ headers เข้าไป
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      })
    }

    return next.handle(request);
  }
}
