import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CarCalenderComponent } from './car-calender.component';
import { HttpClient, HttpHandler } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { AddReservationComponent } from '../../reservation/add-reservation/add-reservation.component';
import { CarService } from '../car.service';
import { of } from 'rxjs';
import { IUser, Role } from '../../../models/user.interface';
import { ICreateRequest } from '../../../models/request.interface';
import { ToastrModule } from 'ngx-toastr';

describe('CarCalenderComponent', () => {
  let component: CarCalenderComponent;
  let fixture: ComponentFixture<CarCalenderComponent>;
  let carService: jasmine.SpyObj<CarService>;

  beforeEach(async () => {
    const carServiceSpy = jasmine.createSpyObj('CarService', [
      'createRequest',
      'getReservations',
    ]);

    carServiceSpy.createRequest.and.returnValue(of({}));
    carServiceSpy.getReservations.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [ToastrModule.forRoot()],
      declarations: [CarCalenderComponent, AddReservationComponent],
      providers: [
        HttpClient,
        HttpHandler,
        DatePipe,
        { provide: CarService, useValue: carServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CarCalenderComponent);
    component = fixture.componentInstance;
    carService = carServiceSpy;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('formatTime', () => {
    it('formatTime should format 0 hours as 00:00', () => {
      const hour = 0;
      const result = component.formatTime(hour);
      expect(result).toEqual('00:00');
    });

    it('formatTime should format 5 as 05:00', () => {
      const hour = 5;
      const result = component.formatTime(hour);
      expect(result).toEqual('05:00');
    });

    it('formatTime should format 9 as 09:00', () => {
      const hour = 9;
      const result = component.formatTime(hour);
      expect(result).toEqual('09:00');
    });

    it('formatTime should format 10 as 10:00', () => {
      const hour = 10;
      const result = component.formatTime(hour);
      expect(result).toEqual('10:00');
    });

    it('formatTime should format 11 as 11:00', () => {
      const hour = 11;
      const result = component.formatTime(hour);
      expect(result).toEqual('11:00');
    });

    it('formatTime should format 15 as 15:00', () => {
      const hour = 15;
      const result = component.formatTime(hour);
      expect(result).toEqual('15:00');
    });

    it('formatTime should format 24 as 24:00', () => {
      const hour = 24;
      const result = component.formatTime(hour);
      expect(result).toEqual('24:00');
    });
  });

  describe('formatTimePipe', () => {
    it('formatTime should format 0 hours as 00:00', () => {
      const time = new Date(2024, 0, 1, 0, 0);
      const expected = '00:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 1 hours as 01:00', () => {
      const time = new Date(2024, 0, 1, 1, 0);
      const expected = '01:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 3 hours as 03:00', () => {
      const time = new Date(2024, 0, 1, 3, 0);
      const expected = '03:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 9 hours as 09:00', () => {
      const time = new Date(2024, 0, 1, 9, 0);
      const expected = '09:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 10 hours as 10:00', () => {
      const time = new Date(2024, 0, 1, 10, 0);
      const expected = '10:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 14 hours as 14:00', () => {
      const time = new Date(2024, 0, 1, 14, 0);
      const expected = '14:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 23 hours as 23:00', () => {
      const time = new Date(2024, 0, 1, 23, 0);
      const expected = '23:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 24 hours as 00:00', () => {
      const time = new Date(2024, 0, 1, 24, 0);
      const expected = '00:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });

    it('formatTime should format 30 hours as 6:00', () => {
      const time = new Date(2024, 0, 1, 30, 0);
      const expected = '06:00';
      const result = component.formatTimePipe(time);
      expect(result).toEqual(expected);
    });
  });

  describe('dateDifference', () => {
    it('same time should return 0', () => {
      const firstTime = new Date();
      const secondTime = firstTime;

      const result = component.dateDifference(firstTime, secondTime);
      expect(result).toEqual(0);
    });

    it('1 day difference should return 1', () => {
      const firstTime = new Date(2024, 0, 1);
      let secondTime = new Date(2024, 0, 2);

      const result = component.dateDifference(firstTime, secondTime);
      expect(result).toEqual(1);
    });

    it('2 day difference should return 2', () => {
      const firstTime = new Date(2024, 0, 1);
      let secondTime = new Date(2024, 0, 3);

      const result = component.dateDifference(firstTime, secondTime);
      expect(result).toEqual(2);
    });

    it('2 day difference should return 10', () => {
      const firstTime = new Date(2024, 0, 1);
      let secondTime = new Date(2024, 0, 11);

      const result = component.dateDifference(firstTime, secondTime);
      expect(result).toEqual(10);
    });
  });

  describe('getWeekStart', () => {
    it('get current weekStart', () => {
      const date = new Date();
      date.setDate(date.getDate() - date.getDay() + 1);
      date.setHours(0);
      date.setMinutes(0);
      date.setSeconds(0);

      const result = component.getWeekStart();
      expect(result).toEqual(date);
    });
  });

  describe('getWeekEnd', () => {
    it('getCurrentWeekEnd', () => {
      const date = new Date();
      date.setDate(date.getDate() + (7 - date.getDay()));
      date.setHours(23);
      date.setMinutes(59);
      date.setSeconds(59);

      const result = component.getWeekEnd();
      expect(result).toEqual(date);
    });
  });

  describe('isEvenNumber', () => {
    it('1 should not be an even number', () => {
      const number = 1;
      const expected = false;
      const result = component.isEvenNumber(number);
      expect(result).toEqual(expected);
    });

    it('2 should be an even number', () => {
      const number = 2;
      const expected = true;
      const result = component.isEvenNumber(number);
      expect(result).toEqual(expected);
    });
  });

  describe('createRequest', () => {
    it('should create a request', () => {
      const seats = 2;
      const reservationId = 123;

      component.seats = seats;
      component.focussedReservation = { id: reservationId } as any;

      component.createRequest();

      expect(carService.createRequest).toHaveBeenCalledWith({
        seats,
        reservationId,
      } as ICreateRequest);
    });
  });
});
