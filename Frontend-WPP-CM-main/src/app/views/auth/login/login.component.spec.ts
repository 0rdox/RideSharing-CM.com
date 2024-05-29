import { DebugElement } from '@angular/core';
import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { HttpClientModule } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let debugElement: DebugElement;
  let authService: AuthService;
  let loginSpy: any;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientModule, ToastrModule.forRoot()],
      declarations: [LoginComponent],
      providers: [AuthService, HttpClientModule],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    debugElement = fixture.debugElement;

    authService = debugElement.injector.get(AuthService);
    loginSpy = spyOn(authService, 'login').and.callThrough();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should submit when inputs are valid', fakeAsync(() => {
    const emailAddressInput: HTMLInputElement =
      fixture.nativeElement.querySelector('#emailAddress');
    const passwordInput: HTMLInputElement =
      fixture.nativeElement.querySelector('#password');

    emailAddressInput.value = 'test@example.com';
    emailAddressInput.dispatchEvent(new Event('input'));

    passwordInput.value = 'SecureP@ss1';
    passwordInput.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    tick();

    fixture.detectChanges();
    expect(component.loginForm.form.valid).toBe(true);
  }));

  it('should not submit when emailaddress is invalid', fakeAsync(() => {
    const emailAddressInput: HTMLInputElement =
      fixture.nativeElement.querySelector('#emailAddress');
    const passwordInput: HTMLInputElement =
      fixture.nativeElement.querySelector('#password');

    emailAddressInput.value = 'test@example.';
    emailAddressInput.dispatchEvent(new Event('input'));

    passwordInput.value = 'SecureP@ss1';
    passwordInput.dispatchEvent(new Event('input'));

    fixture.detectChanges();
    tick();

    fixture.detectChanges();
    expect(component.loginForm.form.valid).toBe(false);
  }));

  describe('should not submit when password is invalid', () => {
    it('No password', fakeAsync(() => {
      const emailAddressInput: HTMLInputElement =
        fixture.nativeElement.querySelector('#emailAddress');
      const passwordInput: HTMLInputElement =
        fixture.nativeElement.querySelector('#password');

      emailAddressInput.value = 'test@example.com';
      emailAddressInput.dispatchEvent(new Event('input'));

      passwordInput.value = '';
      passwordInput.dispatchEvent(new Event('input'));

      fixture.detectChanges();
      tick();

      fixture.detectChanges();
      expect(component.loginForm.form.valid).toBe(false);
    }));

    // it('No uppercase', fakeAsync(() => {
    //   const emailAddressInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#emailAddress');
    //   const passwordInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#password');

    //   emailAddressInput.value = 'test@example.com';
    //   emailAddressInput.dispatchEvent(new Event('input'));

    //   passwordInput.value = 'securep@ss1';
    //   passwordInput.dispatchEvent(new Event('input'));

    //   fixture.detectChanges();
    //   tick();

    //   fixture.detectChanges();
    //   expect(component.loginForm.form.valid).toBe(false);
    // }));

    // it('No lowercase', fakeAsync(() => {
    //   const emailAddressInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#emailAddress');
    //   const passwordInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#password');

    //   emailAddressInput.value = 'test@example.com';
    //   emailAddressInput.dispatchEvent(new Event('input'));

    //   passwordInput.value = 'SECUREP@SS1';
    //   passwordInput.dispatchEvent(new Event('input'));

    //   fixture.detectChanges();
    //   tick();

    //   fixture.detectChanges();
    //   expect(component.loginForm.form.valid).toBe(false);
    // }));

    // it('No non-alphanumeric', fakeAsync(() => {
    //   const emailAddressInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#emailAddress');
    //   const passwordInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#password');

    //   emailAddressInput.value = 'test@example.com';
    //   emailAddressInput.dispatchEvent(new Event('input'));

    //   passwordInput.value = 'SecurePass1';
    //   passwordInput.dispatchEvent(new Event('input'));

    //   fixture.detectChanges();
    //   tick();

    //   fixture.detectChanges();
    //   expect(component.loginForm.form.valid).toBe(false);
    // }));

    // it('No digit', fakeAsync(() => {
    //   const emailAddressInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#emailAddress');
    //   const passwordInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#password');

    //   emailAddressInput.value = 'test@example.com';
    //   emailAddressInput.dispatchEvent(new Event('input'));

    //   passwordInput.value = 'SecureP@ss';
    //   passwordInput.dispatchEvent(new Event('input'));

    //   fixture.detectChanges();
    //   tick();

    //   fixture.detectChanges();
    //   expect(component.loginForm.form.valid).toBe(false);
    // }));

    // it('Not long enough', fakeAsync(() => {
    //   const emailAddressInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#emailAddress');
    //   const passwordInput: HTMLInputElement =
    //     fixture.nativeElement.querySelector('#password');

    //   emailAddressInput.value = 'test@example.com';
    //   emailAddressInput.dispatchEvent(new Event('input'));

    //   passwordInput.value = 'Se@1';
    //   passwordInput.dispatchEvent(new Event('input'));

    //   fixture.detectChanges();
    //   tick();

    //   fixture.detectChanges();
    //   expect(component.loginForm.form.valid).toBe(false);
    // }));
  });
});
