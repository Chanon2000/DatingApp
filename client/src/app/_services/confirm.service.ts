import { Injectable } from '@angular/core';
import { BsModalRef,BsModalService } from 'ngx-bootstrap/modal';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModelRef: BsModalRef;

  constructor(private modelService: BsModalService) { }

  confirm(title = 'Confirmation', 
    message = 'Are you sure you want to do this?', 
    btnOkText = 'Ok', 
    btnCancelText = 'Cancel') {
      const config = {
        initialState: {
          title,
          message,
          btnOkText,
          btnCancelText
        }
      }

    this.bsModelRef = this.modelService.show('confirm', config);
  }
}
