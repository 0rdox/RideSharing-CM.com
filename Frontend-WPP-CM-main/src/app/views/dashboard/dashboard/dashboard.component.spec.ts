import { DebugElement } from '@angular/core';
import {
  ComponentFixture,
  TestBed,
  tick,
  fakeAsync,
} from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { DashboardService } from '../dashboard.service';
import { IReservation } from '../../../models/reservation.interface';
import { ICar } from '../../../models/car.interface';
import { IUser } from '../../../models/user.interface';
import { Role } from '../../../models/user.interface';
import { BehaviorSubject, of } from 'rxjs';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { UserService } from '../../user/user.service';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { IRequest, Status } from '../../../models/request.interface';

const testHistoryReservations$ = new BehaviorSubject<IReservation[]>([
  {
    id: 0,
    departureDate: new Date('2023-12-14T09:30:00'),
    arrivalDate: new Date('2023-12-14T13:45:00'),
    destination: 'Breda',
    willReturn: true,
    seats: 4,
    creationDate: new Date('2023-12-15T17:30:00'),
    car: {
      id: 0,
      licensePlate: 'ABC123',
      brand: 'Volkswagen',
      model: 'Camry',
      seats: 6,
      imageUrl:
        'https://imageio.forbes.com/specials-images/imageserve/5d35eacaf1176b0008974b54/0x0.jpg?format=jpg&crop=4560,2565,x790,y784,safe&height=900&width=1600&fit=bounds',
      location: 'Breda',
      isAvailable: false,
    },
    user: {
      id: 0,
      name: 'John Doe',
      emailAddress: 'johndoe@example.com',
      employeeNr: '123456',
      hasLicense: true,
      role: Role.User,
    },
  },
  {
    id: 1,
    departureDate: new Date('2023-12-15T14:00:00'),
    arrivalDate: new Date('2023-12-15T17:30:00'),
    destination: 'Amsterdam',
    willReturn: false,
    seats: 2,
    creationDate: new Date('2023-12-15T17:30:00'),
    car: {
      id: 1,
      licensePlate: 'ABC123',
      brand: 'Volkswagen',
      model: 'Camry',
      seats: 6,
      imageUrl: '',
      location: 'Breda',
      isAvailable: false,
    },
    user: {
      id: 1,
      name: 'John Noe',
      emailAddress: 'johnnoe@example.com',
      employeeNr: '23456',
      hasLicense: true,
      role: Role.User,
    },
  },
]);

const testUpcomingReservations$ = new BehaviorSubject<IReservation[]>([
  {
    id: 2,
    departureDate: new Date('3000-12-14T09:30:00'),
    arrivalDate: new Date('3000-12-14T13:45:00'),
    destination: 'Breda',
    willReturn: true,
    seats: 4,
    creationDate: new Date('2023-12-15T17:30:00'),
    car: {
      id: 0,
      licensePlate: 'ABC123',
      brand: 'Volkswagen',
      model: 'Camry',
      seats: 6,
      imageUrl:
        'https://imageio.forbes.com/specials-images/imageserve/5d35eacaf1176b0008974b54/0x0.jpg?format=jpg&crop=4560,2565,x790,y784,safe&height=900&width=1600&fit=bounds',
      location: 'Breda',
      isAvailable: false,
    },
    user: {
      id: 0,
      name: 'John Doe',
      emailAddress: 'johndoe@example.com',
      employeeNr: '123456',
      hasLicense: true,
      role: Role.User,
    },
  },
  {
    id: 3,
    departureDate: new Date('3000-12-15T14:00:00'),
    arrivalDate: new Date('3000-12-15T17:30:00'),
    destination: 'Amsterdam',
    willReturn: false,
    seats: 2,
    creationDate: new Date('2023-12-15T17:30:00'),
    car: {
      id: 1,
      licensePlate: 'ABC123',
      brand: 'Volkswagen',
      model: 'Camry',
      seats: 6,
      imageUrl: '',
      location: 'Breda',
      isAvailable: false,
    },
    user: {
      id: 1,
      name: 'John Noe',
      emailAddress: 'johnnoe@example.com',
      employeeNr: '23456',
      hasLicense: true,
      role: Role.User,
    },
  },
]);

const testRequests$ = new BehaviorSubject<IRequest[]>([
  {
    id: 0,
    seats: 2,
    status: Status.Pending,
    creationDate: new Date('2023-12-15T17:30:00'),
    user: {
      id: 0,
      name: 'John Doe',
      emailAddress: 'johndoe@example.com',
      employeeNr: '123456',
      hasLicense: true,
      role: Role.User,
    },
    reservation: {
      id: 0,
      departureDate: new Date('2023-12-15T17:30:00'),
      arrivalDate: new Date('2023-12-16T17:30:00'),
      destination: 'Breda',
      willReturn: true,
      seats: 4,
      creationDate: new Date('2023-12-14T17:30:00'),
      car: {
        id: 0,
        licensePlate: 'ABC123',
        brand: 'Volkswagen',
        model: 'Camry',
        seats: 6,
        imageUrl:
          'https://imageio.forbes.com/specials-images/imageserve/5d35eacaf1176b0008974b54/0x0.jpg?format=jpg&crop=4560,2565,x790,y784,safe&height=900&width=1600&fit=bounds',
        location: 'Breda',
        isAvailable: false,
      },
      user: {
        id: 1,
        name: 'Jane Doe',
        emailAddress: 'janedoe@example.com',
        employeeNr: '123456',
        hasLicense: true,
        role: Role.User,
      },
    },
  },
]);

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let debugElement: DebugElement;
  let dashboardService: DashboardService;
  let userService: UserService;

  let getReservationsSpy: jasmine.Spy;

  let getProfileSpy: jasmine.Spy;
  const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DashboardComponent],
      providers: [
        { provide: Router, useClass: Router },
        DashboardService,
        UserService,
      ],
      imports: [ToastrModule.forRoot(), HttpClientModule],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    debugElement = fixture.debugElement;

    dashboardService = debugElement.injector.get(DashboardService);
    userService = debugElement.injector.get(UserService);

    getReservationsSpy = spyOn(dashboardService, 'getReservations')
      .and.callThrough()
      .and.returnValue(
        of({
          ...testHistoryReservations$.value,
          ...testUpcomingReservations$.value,
        })
      );

    getProfileSpy = spyOn(userService, 'getProfile')
      .and.callThrough()
      .and.returnValue(of(testHistoryReservations$.value[0].user));
  });

  afterEach(() => {
    getReservationsSpy.calls.reset();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // it('should retrieve history reservations from the service', async () => {
  //   getReservationsSpy.and.returnValue(testHistoryReservations$);

  //   await fixture.whenStable();
  //   component.ngOnInit();

  //   expect(getProfileSpy).toHaveBeenCalled();
  //   expect(getReservationsSpy).toHaveBeenCalled();
  //   expect(component.historyReservations).toEqual(testHistoryReservations$.value);
  // });

  // it('should retrieve upcoming reservations from the service', async () => {
  //   getReservationsSpy.and.returnValue(testUpcomingReservations$);

  //   await fixture.whenStable();
  //   component.ngOnInit();

  //   expect(getProfileSpy).toHaveBeenCalled();
  //   expect(getReservationsSpy).toHaveBeenCalled();
  //   expect(component.upcomingReservations).toEqual(testUpcomingReservations$.value);
  // });

  it('should delete a reservation', fakeAsync(() => {
    const deletedReservationId = 1;

    const deleteReservationSpy = spyOn(
      dashboardService,
      'deleteReservation'
    ).and.returnValue(of(true));

    fixture.detectChanges();
    tick();

    const reservationToDelete = testHistoryReservations$.value.find(
      (reservation) => reservation.id === deletedReservationId
    );

    if (reservationToDelete) {
      dashboardService.deleteReservation(reservationToDelete.id);

      expect(deleteReservationSpy).toHaveBeenCalledWith(reservationToDelete.id);
      expect(component.historyReservations).not.toContain(reservationToDelete);
    } else {
      fail('Reservation to delete not found');
    }
  }));

  it('should delete a request', fakeAsync(() => {
    const deletedRequestId = 0;

    const deleteRequestSpy = spyOn(
      dashboardService,
      'deleteRequest'
    ).and.returnValue(of(true));

    fixture.detectChanges();
    tick();

    const requestToDelete = testRequests$.value.find(
      (request) => request.id === deletedRequestId
    );

    if (requestToDelete) {
      dashboardService.deleteRequest(requestToDelete.id);

      expect(deleteRequestSpy).toHaveBeenCalledWith(requestToDelete.id);
      expect(component.requests).not.toContain(requestToDelete);
    } else {
      fail('Request to delete not found');
    }
  }));
});
