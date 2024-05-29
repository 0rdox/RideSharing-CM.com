import { Injectable } from '@angular/core';
import { Subscription } from 'rxjs';
import { tokenKey, roleKey } from '../localstorage/localstorage.keys';
import { Role } from '../../models/user.interface';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class EmployeeGuard {
  constructor(private router: Router) {}

  canActivate() {
    //if no token not allowed;
    const token = localStorage.getItem(tokenKey);
    if (!token) {
      //redirect to login page
      this.router.navigateByUrl('/auth/login');
      return false;
    }

    //if no role not allowed
    const role = localStorage.getItem(roleKey) as Role | null;
    if (!role) {
      //redirect to dashboard
      this.router.navigateByUrl('/dashboard');
      return false;
    }

    //if role is admin than allowed
    if (role == Role.Admin) {
      return true;
    }

    //redirect to dashboard
    this.router.navigateByUrl('/dashboard');
    return false;
  }
}
