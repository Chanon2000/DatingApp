import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';



@NgModule({
  declarations: [],
  imports: [
    CommonModule, // ทุก augular module ต้องการ CommonModule เสมอ
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    })
  ],
  exports: [
    // ใน export ไม่ต้องใส่ configure (พวก forRoot())
    BsDropdownModule,
    ToastrModule
  ],
})
export class SharedModule { }
