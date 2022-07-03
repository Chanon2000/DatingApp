import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';


const routes: Routes = [ // จะใช้ array นี้ในการบอก route ต่างๆ ให้ angular
  {path: '', component: HomeComponent},
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard], // หมายความว่า children จะถูกดูแลโดย AuthGuard ด้วย
    children: [
      {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]}, 
      {path: 'members/:id', component: MemberDetailComponent},
      {path: 'lists', component: ListsComponent},
      {path: 'messages', component: MessagesComponent},
    ]
  },
  
  {path: '**', component: HomeComponent, pathMatch: 'full'}, // ถ้าไม่ match กับอะไรจะเข้าที่ **
  // ** เรียก wild card route
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
