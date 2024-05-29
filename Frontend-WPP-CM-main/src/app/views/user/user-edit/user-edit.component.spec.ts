import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { UserEditComponent } from './user-edit.component';
import { UserService } from '../user.service';
import { FormsModule } from '@angular/forms';
import { ICreateUser, Role } from '../../../models/user.interface';
import { ToastrModule } from 'ngx-toastr';

describe('UserEditComponent', () => {
  let component: UserEditComponent;
  let fixture: ComponentFixture<UserEditComponent>;
  let userServiceSpy = jasmine.createSpyObj('UserService', [
    'createUser',
    'getUserById',
  ]);
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const userServiceSpyObj = jasmine.createSpyObj('UserService', [
      'updateUser',
    ]);
    const routerSpyObj = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      declarations: [UserEditComponent],
      imports: [FormsModule, ToastrModule.forRoot()],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => '1' } } },
        },
        { provide: UserService, useValue: userServiceSpyObj },
        { provide: Router, useValue: routerSpyObj },
      ],
    });

    fixture = TestBed.createComponent(UserEditComponent);
    component = fixture.componentInstance;
    userServiceSpy = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Creating', () => {
    it('should set isEditing to false when userId is not present', () => {
      const activatedRoute = TestBed.inject(ActivatedRoute);
      spyOn(activatedRoute.snapshot.paramMap, 'get').and.returnValue(null);

      fixture.detectChanges();
      expect(component.isEditing).toBe(false);
    });

    it('should call createUser method when isEditing is false', () => {
      spyOn(component, 'createUser');
      component.isEditing = false;
      component.saveUser();
      expect(component.isEditing).toBe(false);
      expect(component.createUser).toHaveBeenCalled();
    });
  });
  describe('Updating', () => {
    it('should call updateUser method when isEditing is true', () => {
      spyOn(component, 'updateUser');
      component.isEditing = true;
      component.saveUser();
      expect(component.isEditing).toBe(true);
      expect(component.updateUser).toHaveBeenCalled();
    });

    it('should set isEditing to true when userId is present', () => {
      const mockUser = {
        id: '1',
        name: 'Sandro Dekkers',
        emailAddress: 'sandro@cm.com',
        employeeNr: 'Nr1',
        hasLicense: true,
        role: Role.Admin,
      };
      userServiceSpy.getUserById = jasmine
        .createSpy()
        .and.returnValue(of(mockUser));

      const snapshot = { paramMap: { get: () => '1' } };
      spyOn(snapshot.paramMap, 'get').and.returnValue('1');

      fixture.detectChanges();
      expect(component.isEditing).toBe(true);
    });

    it('should fetch user details and update properties in edit mode', () => {
      const mockUser = {
        id: 1,
        name: 'Sandro Dekkers',
        emailAddress: 'sandro@cm.com',
        employeeNr: 'Nr1',
        hasLicense: true,
        role: Role.Admin,
      };
      userServiceSpy.getUserById = jasmine
        .createSpy()
        .and.returnValue(of(mockUser));

      const snapshot = { paramMap: { get: () => '1' } };

      spyOn(snapshot.paramMap, 'get').and.returnValue('1');
      fixture.detectChanges();

      expect(component.CreateUserModel.id).toEqual(1);
      expect(component.CreateUserModel.emailAddress).toEqual('sandro@cm.com');
      expect(component.CreateUserModel.employeeNr).toEqual('Nr1');
    });
  });
});
