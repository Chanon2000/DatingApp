import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component'
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component'

@NgModule({
  declarations: [ // component ที่จะใช้ใน project
    AppComponent, NavComponent, HomeComponent, RegisterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    BsDropdownModule.forRoot()
    // forRoot() เพื่อให้มั้นใจว่ามันจะ load ทุก service ที่ module เราต้องการ
    // แสดงว่าไม่ใส่ก็ได้?
  ],
  providers: [], // angular version เก่าจะต้องใส่ service ลงตรงนี้ แต่ตอนนี้ใช้ providedIn ที่เป็น metadata แทน
  bootstrap: [AppComponent]
})
export class AppModule { }
