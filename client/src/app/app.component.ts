import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {  //เพื่อใช้ life cycle
  title = 'The Dating app';
  users: any;

  constructor(private accountService: AccountService) {
    
  }

  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser() {
    // JSON.parse() คือแปลงจาก stringify form ให้เป็น obj js
    const user: User = JSON.parse(localStorage.getItem('user') as any);
    this.accountService.setCurrentUser(user);
  }


}
