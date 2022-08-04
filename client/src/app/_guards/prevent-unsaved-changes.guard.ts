import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmService) {}

  canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean { // union type คือ เป็นได้มากกว่า 1 type ( | )
    if (component.editForm.dirty) {
      return this.confirmService.confirm(); // เราอยู่ใน Route guard นั้นทำมันมันจะ subscribe ที่ confirm() ให้เราเอง
    }
    return true;
  }
  
}
