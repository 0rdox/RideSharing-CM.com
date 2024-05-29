import { Component, OnDestroy, OnInit } from '@angular/core';
import { IUser } from '../../../models/user.interface'; // Changed ICar to IUser
import { UserService } from '../user.service'; // Changed CarService to UserService
import { Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { StorageService } from '../../../common/localstorage/localstorage.service';
import { Role } from '../../../models/user.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-list', // Changed app-car-list to app-user-list
  templateUrl: './user-list.component.html', // Changed car-list.component.html to user-list.component.html
  styleUrl: './user-list.component.css', // Changed car-list.component.css to user-list.component.css
})
export class UserListComponent implements OnInit, OnDestroy {
  users: IUser[] | null = null; // Changed cars to users
  subscription: Subscription | null = null;
  adminSubscription: Subscription | null = null;
  filtersHidden = false;
  filters = {
    departureDate: new Date(),
    destination: null,
    seats: 1,
  };
  currentDateTime = new Date().toISOString();
  isAdmin: boolean = false;

  constructor(
    private readonly userService: UserService, // Changed carService to userService
    private toastr: ToastrService,
    private storageService: StorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscription = this.userService.getUsers().subscribe({
      next: (results) => {
        this.users = results; // Changed cars to users
      },
      error: (error) => {
        if (error.status == 401) {
          this.router.navigateByUrl('/auth/login');
        }

        if (error instanceof HttpErrorResponse)
          this.toastr.error(error.error.message, 'Error!');
        else {
          this.toastr.error('Please try again later', 'Server error');
        }
      },
    });

    //check if is admin
    this.adminSubscription = this.storageService
      .getUserRole()
      .subscribe((value) => {
        if (value == Role.Admin) {
          this.isAdmin = true;
        } else {
          this.isAdmin = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.subscription) this.subscription.unsubscribe();
    if (this.adminSubscription) this.adminSubscription.unsubscribe();
  }

  onSubmit(): void {}

  onEdit(id: number): void {
    this.router.navigate(['/users/' + id + '/edit/']);
  }

  onDelete(id: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.users = this.users!.filter((user) => user.id !== id);
        },
        error: (error) => {},
      });
    }
  }
}
