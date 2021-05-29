import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {  //เพื่อใช้ life cycle
  title = 'The Dating app';
  users: any;

  constructor(private http: HttpClient) {
    
  }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.http.get('https://localhost:5001/api/users').subscribe(response => {
      this.users = response;
    }, error => {
      console.log(error);
    })
  }
}
