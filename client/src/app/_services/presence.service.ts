import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) { // ใช้เมื่อ user login
    // เพราะเราต้องส่ง JWT token เมื่อเราสร้าง connection
    // เราไม่ใช้ interceptor เพราะนี้ไม่ใช่ Http request
    // นั้นคือเราใช้ web sockets ซึ่งไม่ support เรื่อง authentication header ด้วย
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect() // เพื่อแก้ปัญหา network client เราควรจะ auto try and reconnect to our hub
      .build();

    this.hubConnection
      .start() // start hub connection จริงๆ ตรงนี้
      .catch(error => console.log(error)); // ถ้ามี error ก็จะ console ออกมา

    this.hubConnection.on('UserIsOnline', username => { // UserIsOnline ชื่อตรงนี้จะต้องตรงกับใน method ที่เขียนใน api
      this.toastr.info(username + ' has connected');
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.toastr.warning(username + ' has disconnected');
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
      this.toastr.info(knownAs + ' has sent you a new message!')
        .onTap // เพื่อที่เราจะสามารถทำการ click แล้วทำบางอย่างได้ (ในที่นี้คือเราต้องการให้มันย้ายไปที่หน้า message)
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'))
    })
  }

  stopHubConnection() { // ใช้เมื่อ user logout
    this.hubConnection.stop().catch(error => console.log(error)); // stop แล้วก็แค่ catch error เฉยๆ
  }
}
