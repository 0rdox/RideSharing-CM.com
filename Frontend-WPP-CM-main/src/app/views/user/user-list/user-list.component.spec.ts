import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';

import { UserListComponent } from './user-list.component';
import { UserService } from '../user.service';
import { IUser, Role } from '../../../models/user.interface';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let debugElement: DebugElement;
  let userService: UserService;
  let getUsersSpy: any;

  const testUsers$ = new BehaviorSubject<IUser[]>([
    {
      id: 0,
      name: 'John Doe',
      emailAddress: 'john.doe@example.com',
      employeeNr: '1212312',
      hasLicense: true,
      role: Role.User,
    },
    {
      id: 1,
      name: 'Emily Jane',
      emailAddress: 'emily.jane@example.com',
      employeeNr: '827291',
      hasLicense: false,
      role: Role.User,
    },
  ]);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        FormsModule,
        RouterTestingModule,
        ToastrModule.forRoot(),
        HttpClientModule,
      ],
      declarations: [UserListComponent],
      providers: [UserService],
    }).compileComponents();

    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
    debugElement = fixture.debugElement;

    userService = debugElement.injector.get(UserService);
    getUsersSpy = spyOn(userService, 'getUsers')
      .and.callThrough()
      .and.returnValue(testUsers$);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve users from the service', () => {
    component.ngOnInit();
    expect(getUsersSpy).toHaveBeenCalled();
    expect(component.users).toEqual(testUsers$.value);
  });

  it('should display the list of users', () => {
    component.ngOnInit();

    fixture.detectChanges();

    const userListItems =
      debugElement.nativeElement.querySelectorAll('.user-item');
    expect(userListItems.length).toBe(testUsers$.value.length);
  });
});
