import { Injectable } from '@angular/core';
import { BsModalRef,BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModelRef: BsModalRef;

  constructor(private modelService: BsModalService) { }

  confirm(title = 'Confirmation', 
    message = 'Are you sure you want to do this?', 
    btnOkText = 'Ok', 
    btnCancelText = 'Cancel'): Observable<boolean> {
      const config = {
        initialState: {
          title,
          message,
          btnOkText,
          btnCancelText
        }
      }

    this.bsModelRef = this.modelService.show(ConfirmDialogComponent, config);

    return new Observable<boolean>(this.getResult()); // เหมือนใส่ callback เข้าไป
  }

  private getResult() {
    return (observer) => { // callback ที่ใส่ใน Observable() จะทำให้คุณเข้าถึง observer ของ obv ได้
      const subscription = this.bsModelRef.onHidden.subscribe(() => {
        observer.next(this.bsModelRef.content.result); // เอา observer ของ Observable มา next เพื่อ emit ค่าที่ได้จาก bsModelRef
        observer.complete();
      });

      return {
        unsubscribe() {
          subscription.unsubscribe(); // ทำการ unsubscribe ไปที่ subscription
        }
      }
    }
  }
}
