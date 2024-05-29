import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { CarListComponent } from './car-list.component';
import { CarService } from '../car.service';
import { ICar } from '../../../models/car.interface';
import { BehaviorSubject, of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';

const testCars$ = new BehaviorSubject<ICar[]>([
  {
    id: 0,
    licensePlate: '00-AA-AA',
    brand: 'Volkswagen',
    model: 'ID.3',
    seats: 5,
    imageUrl:
      'https://dam.broekhuis.online/online/broekhuis/modelpaginas/volkswagen/image-thumb__29831__original/hero-vw-id3-mob.webp',
    location: '51.602979678914934, 4.771009567888574',
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
    location: '51.602979678914934, 4.771009567888574',
    isAvailable: true,
  },
]);

describe('CarListComponent', () => {
  let component: CarListComponent;
  let fixture: ComponentFixture<CarListComponent>;
  let debugElement: DebugElement;
  let carService: CarService;
  let getCarsSpy: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        RouterModule,
        RouterTestingModule,
        ToastrModule.forRoot(),
        HttpClientModule,
      ],
      declarations: [CarListComponent],
      providers: [CarService, HttpClientModule],
    }).compileComponents();

    fixture = TestBed.createComponent(CarListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    debugElement = fixture.debugElement;

    carService = debugElement.injector.get(CarService);
    getCarsSpy = spyOn(carService, 'getCars')
      .and.callThrough()
      .and.returnValue(testCars$);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve cars from the service', () => {
    component.ngOnInit();
    expect(getCarsSpy).toHaveBeenCalled();
    expect(component.cars).toEqual(testCars$.value);
  });
});
