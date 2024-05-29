import { Component, OnDestroy, OnInit } from '@angular/core';
import { initDropdowns } from 'flowbite';
import { Subscription } from 'rxjs';
import { StorageService } from '../../common/localstorage/localstorage.service';
import { Role } from '../../models/user.interface';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit, OnDestroy {
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;
  subscription: Subscription | null = null;

  constructor(private readonly storageService: StorageService) {}

  ngOnInit(): void {
    initDropdowns();
    this.subscription = this.storageService
      .getUserToken()
      .subscribe((value) => {
        if (value) {
          this.isLoggedIn = true;

          this.storageService.getUserRole().subscribe((role) => {
            if (role == Role.Admin) {
              this.isAdmin = true;
            } else {
              this.isAdmin = false;
            }
          });
        } else {
          this.isLoggedIn = false;
          this.isAdmin = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.subscription) this.subscription.unsubscribe();
  }
}
