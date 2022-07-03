import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
// AuthGuard คือชื่อ class ที่ implement CanActivate อยู่
  // ประเด็นหลักคือมัน implement CanActivate อยู่
export class AuthGuard implements CanActivate {
  // canActivate(
  //   route: ActivatedRouteSnapshot, // route
  //   state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {// state คือ route ปัจจุบัน
  //   return true;
  // }

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {

  }

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user) return true; // จะเห็นว่า จาก obv ของ user กลายเป็น obv ของ boolean ได้โดย map
        this.toastr.error('You shall not pass!')
        return false;
      })
    )
  }

  // ถ้าอยู่ใน CanActivate มัน auto subscribe ให้เรา 
}
