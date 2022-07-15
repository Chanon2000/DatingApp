import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs/operators';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy(); // เมื่อส่ง request จะทำอันนี้ทุก request
    return next.handle(request).pipe( // เมื่อ request นั้น response กลับมา แล้วทำใน pipe
      delay(1000), // มันให้ช้าเพื่อจำลอง
      finalize(() => {
        this.busyService.idle();
      })
    );
  }
}
