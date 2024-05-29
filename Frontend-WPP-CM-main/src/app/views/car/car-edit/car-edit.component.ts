import { Component, OnInit } from '@angular/core';
import { ICar, ICreateCar } from '../../../models/car.interface';
import { CarService } from '../car.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Destination } from '../../../models/destination.enum';

@Component({
  selector: 'app-car-edit',
  templateUrl: './car-edit.component.html',
  styleUrl: './car-edit.component.css',
})
export class CarEditComponent implements OnInit {
  id = 0;
  brand = '';
  model = '';
  seats = 0;
  license = '';
  location = 'undefined';
  imageUrl = '';
  isAvailable = false;
  errorMessage = '';
  destinations = this.getDestinations();

  //Differentiate between edit and create
  isEditing = false;

  constructor(
    private route: ActivatedRoute,
    private carService: CarService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    //Check if ID is in url; (yes = editing, no = creating)
    const carId = this.route.snapshot.paramMap.get('id');

    if (carId) {
      if (!isNaN(parseInt(carId))) {
        this.isEditing = true;
        this.carService.getCar(parseInt(carId)).subscribe((car) => {
          this.id = car!.id;
          this.brand = car!.brand;
          this.model = car!.model;
          this.seats = car!.seats;
          this.license = car!.licensePlate;
          this.location = car!.location;
          this.imageUrl = car!.imageUrl;
          this.isAvailable = car!.isAvailable;
        });
      }
    }
  }

  saveCar() {
    if (this.isEditing) {
      this.updateCar();
    } else {
      this.createCar();
    }
  }

  updateCar() {
    const updatedCar: ICar = {
      id: this.id,
      licensePlate: this.license,
      brand: this.brand,
      model: this.model,
      seats: this.seats,
      imageUrl: this.imageUrl,
      location: this.location,
      isAvailable: this.isAvailable,
    };

    this.carService.updateCar(updatedCar).subscribe({
      next: (value) => {
        this.router.navigateByUrl('/cars');
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

  createCar() {
    this.license = this.license.toUpperCase();
    this.validateLicensePlate(this.license);

    if (this.errorMessage) {
      return;
    }

    const newCar: ICreateCar = {
      licensePlate: this.license,
      brand: this.brand,
      model: this.model,
      seats: this.seats,
      imageUrl: this.imageUrl,
      location: this.location,
      isAvailable: this.isAvailable,
    };

    this.carService.createCar(newCar).subscribe({
      next: (result) => {
        this.router.navigate(['/cars']);
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

  validateLicensePlate(license: string) {
    const hyphenCount = (license.match(/-/g) || []).length;
    const hasVowels = /[AEIOU]/i.test(license);
    const restrictedWords = [
      'GVD',
      'KKK',
      'KVT',
      'LPF',
      'NSB',
      'PKK',
      'PSV',
      'TBS',
      'SS',
      'SD',
      'PVV',
      'SGP',
      'VVD',
    ];

    if (hasVowels) {
      this.errorMessage = 'License plate cannot contain vowels.';
    } else if (restrictedWords.some((word) => license.includes(word))) {
      this.errorMessage = 'License plate contains restricted words.';
    } else if (hyphenCount != 2) {
      this.errorMessage = 'License plate is invalid.';
    } else {
      this.errorMessage = '';
    }
  }

  private getDestinations(): string[] {
    return Object.values(Destination).filter(
      (value) => typeof value === 'string' && value !== 'other'
    ) as string[];
  }
}
