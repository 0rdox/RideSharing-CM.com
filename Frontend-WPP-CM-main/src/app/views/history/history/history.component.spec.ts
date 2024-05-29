import { DebugElement } from '@angular/core';
import {
  ComponentFixture,
  TestBed,
  tick,
  fakeAsync,
} from '@angular/core/testing';
import { HistoryComponent } from './history.component';
import { HistoryService } from '../history.service';
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

describe('HistoryComponent', () => {
  let component: HistoryComponent;
  let fixture: ComponentFixture<HistoryComponent>;
  let debugElement: DebugElement;
  let historyService: HistoryService;
  let userService: UserService;

  let getReservationsSpy: jasmine.Spy;
  let getProfileSpy: jasmine.Spy;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [HistoryComponent],
      providers: [
        { provide: Router, useValue: routerSpy },
        HistoryService,
        UserService,
      ],
      imports: [ToastrModule.forRoot(), HttpClientModule, RouterTestingModule],
    }).compileComponents();

    fixture = TestBed.createComponent(HistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    debugElement = fixture.debugElement;

    historyService = debugElement.injector.get(HistoryService);
    userService = debugElement.injector.get(UserService);

    getReservationsSpy = spyOn(historyService, 'getReservations')
      .and.callThrough()
      .and.returnValue(
        of({
          ...testHistoryReservations$.value,
          ...testHistoryReservations$.value,
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

  it('should retrieve history reservations from the service', async () => {
    getReservationsSpy.and.returnValue(testHistoryReservations$);

    await fixture.whenStable();
    component.ngOnInit();

    expect(getProfileSpy).toHaveBeenCalled();
    expect(getReservationsSpy).toHaveBeenCalled();
    expect(component.historyReservations).toEqual(testHistoryReservations$.value);
  });

  // Uncomment this test if needed
  // it('should retrieve upcoming reservations from the service', async () => {
  //   getReservationsSpy.and.returnValue(testUpcomingReservations$);

  //   await fixture.whenStable();
  //   component.ngOnInit();

  //   expect(getProfileSpy).toHaveBeenCalled();
  //   expect(getReservationsSpy).toHaveBeenCalled();
  //   expect(component.upcomingReservations).toEqual(testUpcomingReservations$.value);
  // });

  // Additional tests or modifications can be added as needed

});