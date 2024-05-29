import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { CarEditComponent } from './car-edit.component';
import { CarService } from '../car.service';
import { FormsModule } from '@angular/forms';
import { ICreateCar } from '../../../models/car.interface';
import { ToastrModule } from 'ngx-toastr';

describe('CarEditComponent', () => {
  let component: CarEditComponent;
  let fixture: ComponentFixture<CarEditComponent>;
  let carServiceSpy = jasmine.createSpyObj('CarService', ['createCar']);
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const carServiceSpyObj = jasmine.createSpyObj('CarService', ['updateCar']);
    const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      declarations: [CarEditComponent],
      imports: [FormsModule, ToastrModule.forRoot()],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => '1' } } },
        },
        { provide: CarService, useValue: carServiceSpyObj },
        { provide: Router, useValue: routerSpyObj },
      ],
    });

    fixture = TestBed.createComponent(CarEditComponent);
    component = fixture.componentInstance;
    carServiceSpy = TestBed.inject(CarService) as jasmine.SpyObj<CarService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Creating', () => {
    it('should set isEditing to false when carId is not present', () => {
      const activatedRoute = TestBed.inject(ActivatedRoute);
      spyOn(activatedRoute.snapshot.paramMap, 'get').and.returnValue(null);

      fixture.detectChanges();
      expect(component.isEditing).toBe(false);
    });

    it('should call createCar method when isEditing is false', () => {
      spyOn(component, 'createCar');
      component.isEditing = false;
      component.saveCar();
      expect(component.isEditing).toBe(false);
      expect(component.createCar).toHaveBeenCalled();
    });
  });
  describe('Updating', () => {
    it('should call updateCar method when isEditing is true', () => {
      spyOn(component, 'updateCar');
      component.isEditing = true;
      component.saveCar();
      expect(component.isEditing).toBe(true);
      expect(component.updateCar).toHaveBeenCalled();
    });

    it('should set isEditing to true when carId is present', () => {
      const mockCar = { id: '1', brand: 'Toyota', model: 'Camry' };
      carServiceSpy.getCar = jasmine.createSpy().and.returnValue(of(mockCar));

      const snapshot = { paramMap: { get: () => '1' } };
      spyOn(snapshot.paramMap, 'get').and.returnValue('1');

      fixture.detectChanges();
      expect(component.isEditing).toBe(true);
    });

    it('should fetch car details and update properties in edit mode', () => {
      const mockCar = { id: 1, brand: 'Toyota', model: 'Camry' };
      carServiceSpy.getCar = jasmine.createSpy().and.returnValue(of(mockCar));

      const snapshot = { paramMap: { get: () => '1' } };

      spyOn(snapshot.paramMap, 'get').and.returnValue('1');
      fixture.detectChanges();

      expect(carServiceSpy.getCar).toHaveBeenCalledTimes(1);
      expect(component.id).toEqual(1);
      expect(component.brand).toEqual('Toyota');
      expect(component.model).toEqual('Camry');
    });
  });
  describe('License Plate', () => {
    it('should validate license plate', () => {
      component.validateLicensePlate('00-00-00');
      expect(component.errorMessage).toBe('');

      component.validateLicensePlate('XP-99-14');
      expect(component.errorMessage).toBe('');

      component.validateLicensePlate('XX-YY-ZZ');
      expect(component.errorMessage).toBe('');
    });

    it('should not validate license plate, cannot contain vowels', () => {
      component.validateLicensePlate('OO-12-AA');
      expect(component.errorMessage).toBe(
        'License plate cannot contain vowels.'
      );

      component.validateLicensePlate('9F-1Z-AF');
      expect(component.errorMessage).toBe(
        'License plate cannot contain vowels.'
      );

      component.validateLicensePlate('QP-1A-A9');
      expect(component.errorMessage).toBe(
        'License plate cannot contain vowels.'
      );
    });

    it('should not validate license plate, license plate is invalid', () => {
      component.validateLicensePlate('00-00-00-00');
      expect(component.errorMessage).toBe('License plate is invalid.');

      component.validateLicensePlate('ZZ-14-46-PQ-VW');
      expect(component.errorMessage).toBe('License plate is invalid.');

      component.validateLicensePlate('19-QP');
      expect(component.errorMessage).toBe('License plate is invalid.');
    });

    it('should not validate license plate, restricted words', () => {
      component.validateLicensePlate('PVV-4P-19');
      expect(component.errorMessage).toBe(
        'License plate contains restricted words.'
      );

      component.validateLicensePlate('PF-KKK-19');
      expect(component.errorMessage).toBe(
        'License plate contains restricted words.'
      );

      component.validateLicensePlate('VVD-PKK-KVT');
      expect(component.errorMessage).toBe(
        'License plate contains restricted words.'
      );
    });
  });
});
