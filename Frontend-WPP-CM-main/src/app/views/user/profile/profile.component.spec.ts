import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileComponent } from './profile.component';
import { UserService } from '../user.service';
import { IUser, Role } from '../../../models/user.interface';
import { BehaviorSubject } from 'rxjs';
import { By } from '@angular/platform-browser';
import { ToastrModule } from 'ngx-toastr';
import { HttpClientModule } from '@angular/common/http';

const testUser$ = new BehaviorSubject<IUser>({
  id: 0,
  name: 'John Doe',
  emailAddress: 'johndoe@example.com',
  employeeNr: '12345',
  hasLicense: false,
  role: Role.Admin,
});

describe('ProfileComponent', () => {
  let component: ProfileComponent;
  let fixture: ComponentFixture<ProfileComponent>;
  let debugElement: DebugElement;
  let userService: UserService;
  let getProfileSpy: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProfileComponent],
      providers: [UserService, HttpClientModule],
      imports: [ToastrModule.forRoot(), HttpClientModule],
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    debugElement = fixture.debugElement;

    userService = debugElement.injector.get(UserService);
    getProfileSpy = spyOn(userService, 'getProfile')
      .and.callThrough()
      .and.returnValue(testUser$);
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should call getUserById', () => {
    component.ngOnInit();
    expect(getProfileSpy).toHaveBeenCalled();
    expect(getProfileSpy).toHaveBeenCalledTimes(1);
    expect(component.user).toEqual(testUser$.value);
  });

  it('should display user data', () => {
    component.user = testUser$.value;
    fixture.detectChanges();
    const name = debugElement.query(By.css('.name'));
    const email = debugElement.query(By.css('.email'));

    expect(name.nativeElement.textContent).toBeTruthy();
    expect(email.nativeElement.textContent).toBeTruthy();
  });
});
