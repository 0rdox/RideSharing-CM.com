import { Component, OnDestroy, OnInit } from '@angular/core';
import { IReservation } from '../../../models/reservation.interface';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { UserService } from '../../user/user.service';
import { Id } from '../../../models/id.type';
import { IUser } from '../../../models/user.interface';
import { Router } from '@angular/router';
import { IRequest, Status } from '../../../models/request.interface';
import { DashboardService } from '../../dashboard/dashboard.service';
import { HistoryService } from '../history.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})
export class HistoryComponent implements OnInit, OnDestroy {
  upcomingReservations: IReservation[] | null = null;
  historyReservations: IReservation[] | null = null;
  selectedReservation: IReservation | null = null;

  requests: IRequest[] | null = null;
  selectedRequest: IRequest | null = null;

  user: IUser | null = null;

  reservationsSubscription: Subscription | null = null;
  requestsSubscription: Subscription | null = null;
  userSubscription: Subscription | null = null;
  adminSubscription: Subscription | null = null;

  isAdmin: boolean = false;

  private static statusToString = {
    [Status.Pending]: 'pending',
    [Status.Accepted]: 'accepted',
    [Status.Denied]: 'denied',
  };

  constructor(
    private readonly historyService: HistoryService,
    private readonly userService: UserService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userSubscription = this.userService.getProfile().subscribe({
      next: (results) => {
        this.user = results;
        this.getReservations(this.user.id);
      },
      error: (error) => {
        if (error.status == 401) {
          this.router.navigate(['auth/login']);
        }

        if (error instanceof HttpErrorResponse)
          this.toastr.error(error.error.message, 'Error!');
        else {
          this.toastr.error('Please try again later', 'Server error');
        }
      },
    });
  }

  private getReservations(id: Id): void {
    this.reservationsSubscription = this.historyService
      .getReservations(id)
      .subscribe({
        next: (results) => {
          this.historyReservations = results.filter(
            (reservation) => new Date(reservation.arrivalDate) <= new Date()
          );
        },
        error: (error) => {
          if (error instanceof HttpErrorResponse)
            this.toastr.error(error.error.message, 'Error!');
          else {
            this.toastr.error('Please try again later', 'Server error');
          }
        },
      });
  }


  ngOnDestroy(): void {
    if (this.reservationsSubscription)
      this.reservationsSubscription.unsubscribe();

    if (this.userSubscription) this.userSubscription.unsubscribe();
  }

  isAfterNow(date: Date): boolean {
    var today = new Date();
    var checkDate = new Date(date);
    return checkDate > today;
  }
}
