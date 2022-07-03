import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  loggedIn: boolean = false;


  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
    this.getCurrentUser(); // เปลี่ยนไปใช้วิธี currentUser$ แทน ()

  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      // this.loggedIn = true;
    }, error => {
      console.log(error);
    })
  }

  logout() {
    this.accountService.logout();
    // this.loggedIn = false;
  }

  getCurrentUser() {
    
    this.accountService.currentUser$.subscribe(user => {
      // การ subscribe นี้ไม่ใช้ http request (method ที่ใช้มัน auto complete ให้เรา) ซึ่งคุณก็ไม่ได้สั่ง complete เอาไว้ นั้นทำให้มันไม่มีวัน complete ซึ่งอาจทำให้เราเจอปัญหา memory leaks ได้
      this.loggedIn = !!user; // !! เพื่อบอกว่าถ้า user เป็น null มันจะเป็น false ไม่ถ้าไม่ใช่ null ก็จะเป็น true
    }, error => {
      console.log(error);
    })
  }

  // #. Using the async pipe (instand of subscribe())
  
}
