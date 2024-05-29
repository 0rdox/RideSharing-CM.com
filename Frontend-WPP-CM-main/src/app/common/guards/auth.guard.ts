import { Injectable } from '@angular/core';
import { Subscription } from 'rxjs';
import { tokenKey } from '../localstorage/localstorage.keys';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthGuard {
  constructor(private router: Router) {}
  canActivate() {
    const token = localStorage.getItem(tokenKey);
    if (token) {
      return true;
    }

    // redirect to login
    this.router.navigateByUrl('/auth/login');
    return false;
  }
}
