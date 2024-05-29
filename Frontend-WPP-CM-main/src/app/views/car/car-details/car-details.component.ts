import { Component, OnDestroy, OnInit } from '@angular/core';
import { ICar } from '../../../models/car.interface';
import { Subscription } from 'rxjs';
import { CarService } from '../car.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { StorageService } from '../../../common/localstorage/localstorage.service';
import { Role } from '../../../models/user.interface';

@Component({
  selector: 'app-car-details',
  templateUrl: './car-details.component.html',
  styleUrl: './car-details.component.css',
})
export class CarDetailsComponent implements OnInit, OnDestroy {
  car: ICar | null = null;
  subscription: Subscription | null = null;
  adminSubscription: Subscription | null = null;
  isAdmin: boolean = false;

  constructor(
    private readonly carService: CarService,
    private readonly route: ActivatedRoute,
    private toastr: ToastrService,
    private storageService: StorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscription = this.route.paramMap.subscribe((params) => {
      // Extract the car ID from the URL
      var carId = params.get('id');
      if (!carId) return;
      if (!isNaN(parseInt(carId))) {
        // Fetch the car by ID from the service
        this.subscription = this.carService.getCar(parseInt(carId)).subscribe({
          next: (result) => {
            this.car = result || null;
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
    });

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

  confirmDelete() {
    if (this.car && confirm('Are you sure you want to delete this car?')) {
      this.carService.deleteCar(this.car.id).subscribe(
        (updatedCars) => {
          this.router.navigateByUrl('/cars');
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
}
