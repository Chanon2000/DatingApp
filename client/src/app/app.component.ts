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

  constructor(private http: HttpClient, private accountService: AccountService) {
    
  }

  ngOnInit() {
    this.getUser();
    this.setCurrentUser();
  }

  setCurrentUser() {
    // JSON.parse() คือแปลงจาก stringify form ให้เป็น obj js
    const user: User = JSON.parse(localStorage.getItem('user') as any);
    this.accountService.setCurrentUser(user);
  }

  getUser() {
    this.http.get('https://localhost:5001/api/users').subscribe(response => {
      this.users = response;
      // console.log(this.users)
    }, error => {
      console.log(error);
    })
  }
}
