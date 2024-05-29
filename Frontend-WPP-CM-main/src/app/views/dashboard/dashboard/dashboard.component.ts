import { Component, OnDestroy, OnInit } from '@angular/core';
import { IReservation } from '../../../models/reservation.interface';
import { DashboardService } from '../dashboard.service';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { UserService } from '../../user/user.service';
import { Id } from '../../../models/id.type';
import { IUser } from '../../../models/user.interface';
import { Router } from '@angular/router';
import { IRequest, IRequest2, Status } from '../../../models/request.interface';
import { StorageService } from '../../../common/localstorage/localstorage.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit, OnDestroy {
  upcomingReservations: IReservation[] | null = null;
  historyReservations: IReservation[] | null = null;
  selectedReservation: IReservation | null = null;

  requests: IRequest[] | null = null;
  incomingRequests: IRequest2[] | null = null;
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
    private readonly dashboardService: DashboardService,
    private readonly userService: UserService,
    private readonly localStorage: StorageService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userSubscription = this.userService.getProfile().subscribe({
      next: (results) => {
        this.user = results;
        this.getReservations(this.user.id);
        this.getRequests(this.user.id);
        this.getIncomingRequests(this.user.id);
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
    this.reservationsSubscription = this.dashboardService
      .getReservations(id)
      .subscribe({
        next: (results) => {
          this.upcomingReservations = results.filter(
            (reservation) => new Date(reservation.arrivalDate) > new Date()
          );
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

  private getRequests(id: Id): void {
    this.requestsSubscription = this.dashboardService
      .getRequests(id)
      .subscribe({
        next: (results) => {
          this.requests = results;
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

  private getIncomingRequests(id: Id): void {
    let token2: any;

    this.localStorage.getUserToken().subscribe((token) => {
      token2 = token;
      this.requestsSubscription = this.dashboardService
        .getIncomingRequests(token2)
        .subscribe({
          next: (results) => {
            this.incomingRequests = results;
          },
          error: (error) => {
            if (error instanceof HttpErrorResponse)
              this.toastr.error(error.error.message, 'Error!');
            else {
              this.toastr.error('Please try again later', 'Server error');
            }
          },
        });
    });
  }

  acceptRequest(id: Id, token: string): void {
    this.dashboardService.responseRequest(id, token, 'ACCEPTED').subscribe(
      (updatedDashboard) => {
        this.selectedRequest = null;
        if (this.user != null) {
          this.requests = null;
          this.getRequests(this.user!.id);
          this.incomingRequests =
            this.incomingRequests?.filter((request) => request.id !== id) ??
            null;
        }
      },
      (error) => {
        if (error instanceof HttpErrorResponse)
          this.toastr.error(error.error.message, 'Error!');
        else {
          this.toastr.error('Please try again later', 'Server error');
        }
      }
    );
  }

  hasPendingRequests(): boolean {
    return Boolean(
      this.incomingRequests?.filter(
        (request) => request.status === Status.Pending
      )?.length
    );
  }

  rejectRequest(id: Id, token: string): void {
    this.dashboardService.responseRequest(id, token, 'DENIED').subscribe(
      (updatedDashboard) => {
        this.selectedRequest = null;
        if (this.user != null) {
          this.requests = null;
          this.getRequests(this.user!.id);
          this.incomingRequests =
            this.incomingRequests?.filter((request) => request.id !== id) ??
            null;
        }
      },
      (error) => {
        if (error instanceof HttpErrorResponse)
          this.toastr.error(error.error.message, 'Error!');
        else {
          this.toastr.error('Please try again later', 'Server error');
        }
      }
    );
  }

  ngOnDestroy(): void {
    if (this.reservationsSubscription)
      this.reservationsSubscription.unsubscribe();

    if (this.userSubscription) this.userSubscription.unsubscribe();
  }

  confirmDelete(reservation: IReservation) {
    this.selectedReservation = reservation;

    if (
      this.selectedReservation &&
      confirm('Are you sure you want to cancel this reservation?')
    ) {
      this.dashboardService
        .deleteReservation(this.selectedReservation.id)
        .subscribe(
          (updatedDashboard) => {
            this.selectedReservation = null;
            if (this.user != null) {
              this.upcomingReservations = null;
              this.historyReservations = null;
              this.getReservations(this.user!.id);
            }
          },
          (error) => {
            if (error instanceof HttpErrorResponse)
              this.toastr.error(error.error.message, 'Error!');
            else {
              this.toastr.error('Please try again later', 'Server error');
            }
          }
        );
    }
  }

  confirmDeleteRequest(request: IRequest) {
    this.selectedRequest = request;

    if (
      this.selectedRequest &&
      confirm('Are you sure you want to cancel this request?')
    ) {
      this.dashboardService.deleteRequest(this.selectedRequest.id).subscribe(
        (updatedDashboard) => {
          this.selectedRequest = null;
          if (this.user != null) {
            this.requests = null;
            this.getRequests(this.user!.id);
          }
        },
        (error) => {
          if (error instanceof HttpErrorResponse)
            this.toastr.error(error.error.message, 'Error!');
          else {
            this.toastr.error('Please try again later', 'Server error');
          }
        }
      );
    }
  }

  isAfterNow(date: Date): boolean {
    var today = new Date();
    var checkDate = new Date(date);
    return checkDate > today;
  }
}
