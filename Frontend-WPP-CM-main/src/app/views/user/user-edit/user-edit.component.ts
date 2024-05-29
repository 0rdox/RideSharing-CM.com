import { Component, OnInit } from '@angular/core';
import { IUser, ICreateUser, Role } from '../../../models/user.interface';
import { UserService } from '../user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.css',
})
export class UserEditComponent implements OnInit {
  CreateUserModel = {
    id: 0,
    name: '',
    emailAddress: '',
    password: '',
    employeeNr: '',
    hasLicense: false,
    role: Role.User,
    roles: [Role.Admin, Role.User],
  };

  //Differentiate between edit and create
  isEditing = false;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    //Check if ID is in url; (yes = editing, no = creating)
    const userId = this.route.snapshot.paramMap.get('id');

    if (userId) {
      if (!isNaN(parseInt(userId))) {
        this.isEditing = true;
        this.userService.getUserById(userId).subscribe((user) => {
          if ((user.role as any) == 1) {
            this.CreateUserModel.role = Role.Admin;
          } else {
            this.CreateUserModel.role = Role.User;
          }

          this.CreateUserModel.id = user!.id;
          (this.CreateUserModel.name = user!.name),
            (this.CreateUserModel.emailAddress = user!.emailAddress),
            (this.CreateUserModel.employeeNr = user!.employeeNr),
            (this.CreateUserModel.hasLicense = user!.hasLicense);
        });
      }
    }
  }

  saveUser() {
    if (this.isEditing) {
      this.updateUser();
    } else {
      this.createUser();
    }
  }

  updateUser() {
    const updatedUser: IUser = {
      id: this.CreateUserModel.id,
      name: this.CreateUserModel.name,
      emailAddress: this.CreateUserModel.emailAddress,
      employeeNr: this.CreateUserModel.employeeNr,
      hasLicense: this.CreateUserModel.hasLicense,
      role: this.CreateUserModel.role,
    };

    this.userService.updateUser(updatedUser).subscribe({
      next: (value) => {
        this.router.navigate(['/users/list']);
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
  }

  createUser() {
    const newUser = {
      name: this.CreateUserModel.name,
      emailAddress: this.CreateUserModel.emailAddress,
      password: this.CreateUserModel.password,
      employeeNr: this.CreateUserModel.employeeNr,
      hasLicense: this.CreateUserModel.hasLicense,
      role: this.CreateUserModel.role,
    };

    this.userService.createUser(newUser).subscribe({
      next: (result) => {
        this.router.navigate(['/users/list']);
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
  }
}
