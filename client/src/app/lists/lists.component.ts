import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>; // ทำให้ properties inside the member เป็น optional
  predicate = 'liked';
  // pageNumber = 1;
  // pageSize = 5;
  // pagination: Pagination;


  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate).subscribe(response => {
      this.members = response;
    })
  }

}
