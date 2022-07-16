import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token, // เพราะอันนี้จะไม่ผ่าน interceptor เลยใส่ตรงนี้เลย
      isHTML5: true,
      allowedFileType: ['image'], // ชนิต file ที่อนุญาต
      removeAfterUpload: true, // remove ออกจาก drop zone หลังจาก upload แล้ว
      autoUpload: false, // เพราะเราจะทำ click button
      maxFileSize: 10 * 1024 * 1024 // คือ maximum ของ cloud แบบ free account นั้นคือ ten megabytes
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false; // false เพราะว่าเราใช้ authToken ในการส่ง credentials ของเราไปกับ file แล้ว
    }

    // ทำหลังจาก upload เสร็จ
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

  

}
