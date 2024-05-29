import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';
import { UserService } from './user.service';
import { ProfileComponent } from './profile/profile.component';
import { HeadersInterceptor } from '../../middleware/headers.interceptor';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthGuard } from '../../common/guards/auth.guard';
import { UserListComponent } from './user-list/user-list.component';
import { UserEditComponent } from './user-edit/user-edit.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmployeeGuard } from '../../common/guards/employee.guard';

const routes: Routes = [
  {
    path: 'list',
    pathMatch: 'full',
    component: UserListComponent,
    canActivate: mapToCanActivate([EmployeeGuard]),
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: mapToCanActivate([AuthGuard]),
  },
  {
    path: 'create',
    pathMatch: 'full',
    component: UserEditComponent,
    canActivate: mapToCanActivate([EmployeeGuard]),
  },
  {
    path: ':id/edit',
    pathMatch: 'full',
    component: UserEditComponent,
    canActivate: mapToCanActivate([EmployeeGuard]),
  },
];

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule.forChild(routes),
    FormsModule,
    ReactiveFormsModule,
  ],
  declarations: [ProfileComponent, UserEditComponent, UserListComponent],
  providers: [
    UserService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HeadersInterceptor,
      multi: true,
    },
  ],
  exports: [RouterModule],
})
export class UserModule {}
