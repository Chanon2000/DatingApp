import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {
  error: any;

  constructor(private router: Router) {
    // เราสร้างเข้าถึงข้อมูล router ได้แค่ที่ constructor เท่านั้น
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.error;
    // "?" คือ optional chaining operators
  }

  ngOnInit(): void {
  }

}
