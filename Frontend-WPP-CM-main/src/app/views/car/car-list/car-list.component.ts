import { Component, OnDestroy, OnInit } from '@angular/core';
import { ICar } from '../../../models/car.interface';
import { CarService } from '../car.service';
import { Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { StorageService } from '../../../common/localstorage/localstorage.service';
import { Role } from '../../../models/user.interface';
import { Router } from '@angular/router';
import { Destination } from '../../../models/destination.enum';

@Component({
  selector: 'app-car-list',
  templateUrl: './car-list.component.html',
  styleUrl: './car-list.component.css',
})
export class CarListComponent implements OnInit, OnDestroy {
  cars: ICar[] | null = null;
  filteredCars: ICar[] | null = null; 
  subscription: Subscription | null = null;
  adminSubscription: Subscription | null = null;
  filtersHidden = false;
  locations: string[] = [];
  destinations = this.getDestinations();
  filters = {
    departureDate: new Date(),
    destination: null,
    seats: 1,
    location: null
  };
  currentDateTime = new Date().toISOString();
  isAdmin: boolean = false;

  constructor(
    private readonly carService: CarService,
    private toastr: ToastrService,
    private storageService: StorageService,
    private router: Router
  ) {}

  

  ngOnInit(): void {
    this.subscription = this.carService.getCars().subscribe({
      next: (results) => {
        this.cars = results;
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

  toggleFilters(): void {
    const filters = document.getElementById('filters');

    if (this.filtersHidden) {
      filters!.style.removeProperty('display');
      this.filtersHidden = false;
    } else {
      filters!.style.display = 'block';
      this.filtersHidden = true;
    }
  }

  onSubmit(): void {
    if (this.cars) {
      // Apply filters based on the form input and update the filteredCars array
      this.filteredCars = this.cars.filter((car) => {
        // Check each filter condition
        const departureDateCondition = new Date(this.filters.departureDate);
        const seatsCondition = car.seats >= this.filters.seats;
        const locationCondition =
        !this.filters.location || car.location === this.filters.location;
        // Return true if all conditions are met
        
        return seatsCondition && locationCondition && departureDateCondition;
      });
    } else {
      this.filteredCars = null; // If this.cars is undefined, set filteredCars to null
    }
  }

  private getDestinations(): string[] {
    return Object.values(Destination).filter(
      (value) => typeof value === 'string' && value !== 'other'
    ) as string[];
  }

  resetFilters(): void{
    this.filters = {
      departureDate: new Date(),
      destination: null,
      seats: 1,
      location: null,
    };

    this.onSubmit();
  }
}
