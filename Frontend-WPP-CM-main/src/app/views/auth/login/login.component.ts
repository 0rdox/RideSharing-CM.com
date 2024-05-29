import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AuthService } from '../auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import {
  tokenKey,
  roleKey,
} from '../../../common/localstorage/localstorage.keys';
import { StorageService } from '../../../common/localstorage/localstorage.service';
import { UserService } from '../../user/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  signInModel = {
    email: null,
    password: null,
  };

  @ViewChild('loginForm') loginForm: ElementRef<HTMLInputElement> | any;

  constructor(
    private readonly authService: AuthService,
    private toastr: ToastrService,
    private storageService: StorageService,
    private userService: UserService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.authService.login(this.signInModel).subscribe({
      next: (result) => {
        localStorage.setItem(tokenKey, result.bearerToken);
        this.storageService.setUserToken(result.bearerToken);
        this.setUserRole();
      },
      error: (error) => {
        if (error instanceof HttpErrorResponse) {
          this.toastr.error(error.error.message, 'Error!');
        } else {
          this.toastr.error('Please try again later', 'Server error');
        }
      },
    });
  }

  private setUserRole(): void {
    this.userService.getProfile().subscribe({
      next: (result) => {
        localStorage.setItem(roleKey, result.role);
        this.storageService.setUserRole(result.role);
        this.storageService.setUserId(result.id);
        this.storageService.setUser(result);
        this.router.navigateByUrl('/dashboard');
      },
      error: (error) => {
        if (error instanceof HttpErrorResponse) {
          this.toastr.error(error.error.message, 'Error!');
        } else {
          this.toastr.error('Please try again later', 'Server error');
        }
      },
    });
  }
}
