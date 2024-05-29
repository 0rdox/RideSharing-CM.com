import {
  ComponentFixture,
  TestBed,
  tick,
  fakeAsync,
} from '@angular/core/testing';
import { BehaviorSubject, of } from 'rxjs';
import { ICar } from '../../../models/car.interface';
import { DebugElement } from '@angular/core';
import { CarService } from '../car.service';
import { CarDetailsComponent } from './car-details.component';
import {
  ActivatedRoute,
  DefaultUrlSerializer,
  Router,
  convertToParamMap,
} from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';
import { CarCalenderComponent } from '../car-calender/car-calender.component';
import { DatePipe } from '@angular/common';
import { AddReservationComponent } from '../../reservation/add-reservation/add-reservation.component';

const testCars$ = new BehaviorSubject<ICar[]>([
  {
    id: 0,
    licensePlate: '00-AA-AA',
    brand: 'Volkswagen',
    model: 'ID.3',
    seats: 5,
    imageUrl:
      'https://dam.broekhuis.online/online/broekhuis/modelpaginas/volkswagen/image-thumb__29831__original/hero-vw-id3-mob.webp',
    location: 'Konijnenberg 24, 4825 BD Breda',
    isAvailable: true,
  },
  {
    id: 1,
    licensePlate: '11-BB-BB',
    brand: 'Volkswagen',
    model: 'ID.3',
    seats: 5,
    imageUrl:
      'https://dam.broekhuis.online/online/broekhuis/modelpaginas/volkswagen/image-thumb__29831__original/hero-vw-id3-mob.webp',
    location: 'Konijnenberg 24, 4825 BD Breda',
    isAvailable: true,
  },
]);

xdescribe('CarDetailsComponent', () => {
  let component: CarDetailsComponent;
  let fixture: ComponentFixture<CarDetailsComponent>;
  let debugElement: DebugElement;
  let carService: CarService;
  let routerSpy: jasmine.SpyObj<Router>;
  let windowSpy: jasmine.SpyObj<Window>;
  let getCarSpy: jasmine.Spy;
  let deleteCarSpy: jasmine.Spy;

  beforeEach(async () => {
    const routerSpyObj = jasmine.createSpyObj('Router', ['navigateByUrl']);
    const windowSpyObj = jasmine.createSpyObj('Window', ['confirm']);

    await TestBed.configureTestingModule({
      declarations: [CarDetailsComponent, CarCalenderComponent, AddReservationComponent],
      imports: [ToastrModule.forRoot(), HttpClientModule],
      providers: [
        HttpClientModule,
        CarService,
        DatePipe,
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ id: '1' })), // Provide a paramMap with id '1'
          },
        },
        { provide: Router, useValue: routerSpyObj },
        { provide: Window, useValue: windowSpyObj },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CarDetailsComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;

    carService = debugElement.injector.get(CarService);
    windowSpy = TestBed.inject(Window) as jasmine.SpyObj<Window>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    getCarSpy = spyOn(carService, 'getCar')
      .and.callThrough()
      .and.returnValue(of(testCars$.getValue()[1]));
    deleteCarSpy = spyOn(carService, 'deleteCar')
      .and.callThrough()
      .and.returnValue(of(true));

    fixture.detectChanges(); // Trigger change detection and ngOnInit
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch the car details', () => {
    expect(getCarSpy).toHaveBeenCalledOnceWith(1);
    expect(component.car).toEqual(testCars$.getValue()[1]);
  });
});
