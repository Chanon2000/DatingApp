import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, pipe } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map(); // Map คือหรือคล้ายกับ dictonary 
  user: User;
  userParams: UserParams;

  constructor(private http: HttpClient, private accountService: AccountService) {
    // เนื่องจากเมื่อเรา ทำการ filter ที่หน้า list แล้วจากนั้นก็กดเข้ามาดูข้อมูลคนนึง จากนั้นกลับมาที่หน้า list มันไม่ทำการ fiter แบบเดิม แต่จะ reset ใหม่ ซึ่งนั้นไม่ใช่พฤติกรรมที่เราต้องการ
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-')); // หา key ชื่อ Object.values(userParams).join('-') แล้วเก็บ value ลง response
    if (response) {
      return of(response); // ถ้ามี member ที่โหลดเก็บไว้อยู่แล้ว ก็เอามาใช้เลย
    }
    
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    // คลิก Refacter... จากนั้นคลิก Extract to method in class 'MembersService' (มันจะย้าย code ที่ highlight ทั้งหมดไปสร้าง method ใหม่ให้เลย)
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params) // Member[] คือ T ที่กำหนดใน method
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response); // key: any, value: any
        return response;
      }))
  }
  // เราจะจำ member ทุกครั้งที่ load ในแต่ละ userParams
  // เนื่องจากแต่ละ load ที่เราจะจำแยกกันนั้น userParams จะไม่เหมือนกันซักครั้ง

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), []) // [] คือ initial value (เป็นค่าก่อนที่จะเริ่ม concat array ต่างๆ)
      .find((member: Member) => member.username === username);

    if (member) {
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    // ทำให้เป็น generic ก็คือเปลี่ยนจาก Member[] เป็น T class ไปเลย
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;

  }
}
