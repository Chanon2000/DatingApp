import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}
  // ทำให้เราสามารถ "intercept" request ที่ออกไป หรือ response ที่เข้ามาได้

  // request => request ที่ออกไป
  // next => response ที่เข้ามาได้
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // จะเห็ฯว่ามัน return เป็น Observable เลยคิดได้ว่าสามารถใช้ pipe ในการจัดการได้
    return next.handle(request).pipe(
      catchError(error => {
        if (error) { // ถ้ามี error ก็จับมันที่ switch ถ้าไม่มี error ก็ return ออกไปเลย
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key])
                  }
                }
                // flat() method มีใน "es2019" = JavaScript 2019
                throw modalStateErrors.flat();
              } else {
                this.toastr.error(error.statusText, error.status);
              }
              break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}}
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    );
  }
}
