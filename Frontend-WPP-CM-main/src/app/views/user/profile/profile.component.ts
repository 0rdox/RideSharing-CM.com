import { Component, OnDestroy, OnInit } from '@angular/core';
import { IUser } from '../../../models/user.interface';
import { Subscription } from 'rxjs';
import { UserService } from '../user.service';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { StorageService } from '../../../common/localstorage/localstorage.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit, OnDestroy {
  user: IUser | undefined = undefined;
  token: string | null = null;
  subscription: Subscription | null = null;

  constructor(
    private readonly userService: UserService,
    private toastr: ToastrService,
    private storageService: StorageService,
    private router: Router
  ) {}

  //Op het moment hardcoded naar 0, dit moet userId uit url (niet ingelogd) of userId uit localStorage (ingelogd) worden
  ngOnInit(): void {
    this.subscription = this.userService.getProfile().subscribe({
      next: (value) => {
        this.user = value;
      },
      error: (error) => {
        if (error.status == 401) {
          this.router.navigateByUrl('/auth/login');
        }

        if (error instanceof HttpErrorResponse) {
          this.toastr.error(error.error.message, 'Error!');
        } else {
          this.toastr.error('Please try again later', 'Server error');
        }
      },
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  signOut(): void {
    //remove all tokens
    this.storageService.setUserToken(null);
    this.storageService.setUserRole(null);
    this.storageService.setUserId(null);
    this.storageService.setUser(null);
    this.router.navigateByUrl('/auth/login');
  }
}
