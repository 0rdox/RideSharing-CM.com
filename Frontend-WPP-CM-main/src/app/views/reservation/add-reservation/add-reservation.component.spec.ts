import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddReservationComponent } from './add-reservation.component';
import { FormsModule } from '@angular/forms';
import { DebugElement } from '@angular/core';
import { ReservationService } from '../reservation.service';
import { HttpClient, HttpHandler } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';

describe('AddReservationComponent', () => {
  let component: AddReservationComponent;
  let fixture: ComponentFixture<AddReservationComponent>;
  let debugElement: DebugElement;
  let reservationService: ReservationService;
  let createReservationSpy: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsModule, ToastrModule.forRoot()],
      declarations: [AddReservationComponent],
      providers: [ReservationService, HttpClient, HttpHandler],
    }).compileComponents();

    fixture = TestBed.createComponent(AddReservationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    debugElement = fixture.debugElement;

    reservationService = debugElement.injector.get(ReservationService);
    createReservationSpy = spyOn(
      reservationService,
      'createReservation'
    ).and.callThrough();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('invalid date', () => {
    it('should be invalid when given date is 1 year before current date', () => {
      let currentDate: Date = new Date();
      let date = new Date(currentDate);
      date.setFullYear(currentDate.getFullYear() - 1);

      let result = component.invalidDate(date);

      expect(result).toBeTruthy();
    });

    it('should be invalid when given date is 1 second before current date', () => {
      let currentDate: Date = new Date();
      let date = new Date(currentDate);
      date.setSeconds(currentDate.getSeconds() - 1);

      let result = component.invalidDate(date);

      expect(result).toBeTruthy();
    });

    it('should be valid when given date is now', () => {
      let currentDate: Date = new Date();

      let result = component.invalidDate(currentDate);

      expect(result).toBeFalsy();
    });

    it('should be valid when given date is 1 second in the future', () => {
      let currentDate: Date = new Date();
      let date = new Date(currentDate);
      date.setSeconds(currentDate.getSeconds() + 1);

      let result = component.invalidDate(date);

      expect(result).toBeFalsy();
    });

    it('should be valid when given date is 1 year in the future', () => {
      let currentDate: Date = new Date();
      let date = new Date(currentDate);
      date.setFullYear(currentDate.getFullYear() + 1);

      let result = component.invalidDate(date);

      expect(result).toBeFalsy();
    });
  });

  // it('should submit when form is valid', fakeAsync(() => {
  //   component.ngOnInit();

  //   const startingTimeInput: HTMLInputElement =
  //     fixture.nativeElement.querySelector('#startingTime');
  //   const endingTimeInput: HTMLInputElement =
  //     fixture.nativeElement.querySelector('#endingTime');
  //   const destinationInput: HTMLInputElement =
  //     fixture.nativeElement.querySelector('#destination');
  //   const seats: HTMLInputElement =
  //     fixture.nativeElement.querySelector('#seats');

  //   const currentDate = new Date();
  //   const departureDate = new Date(currentDate);
  //   departureDate.setFullYear(currentDate.getFullYear() + 1);
  //   const arrivalDate = new Date(currentDate);
  //   arrivalDate.setFullYear(currentDate.getFullYear() + 2);

  //   console.log('currentdate = ' + currentDate);

  //   component.reservation!.departureDate = departureDate;
  //   component.reservation!.arrivalDate = arrivalDate;
  //   component.reservation!.destination = 'Konijnenberg 24, 4825 BD Breda';
  //   component.reservation!.seats = 3;

  //   startingTimeInput.dispatchEvent(new Event('input'));
  //   endingTimeInput.dispatchEvent(new Event('input'));
  //   destinationInput.dispatchEvent(new Event('input'));
  //   seats.dispatchEvent(new Event('input'));

  //   fixture.detectChanges();
  //   tick();

  //   console.log(component.reservation!.departureDate);
  //   console.log(component.reservation!.arrivalDate);
  //   console.log(component.reservation!.destination);
  //   console.log(component.reservation!.seats);

  //   fixture.detectChanges();
  //   expect(component.reservationForm.form.valid).toBe(true);
  // }));
});
