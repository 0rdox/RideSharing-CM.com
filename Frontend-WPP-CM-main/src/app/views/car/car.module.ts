import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { CarListComponent } from './car-list/car-list.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';
import { CarService } from './car.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CarEditComponent } from './car-edit/car-edit.component';
import { CarDetailsComponent } from './car-details/car-details.component';
import { CarCalenderComponent } from './car-calender/car-calender.component';
import { HeadersInterceptor } from '../../middleware/headers.interceptor';
import { AuthGuard } from '../../common/guards/auth.guard';
import { EmployeeGuard } from '../../common/guards/employee.guard';
import { ReservationModule } from '../reservation/reservation.module';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: CarListComponent,
    canActivate: mapToCanActivate([AuthGuard]),
  },
  {
    path: 'create',
    component: CarEditComponent,
    canActivate: mapToCanActivate([EmployeeGuard]),
  },
  {
    path: ':id/edit',
    component: CarEditComponent,
    canActivate: mapToCanActivate([EmployeeGuard]),
  },
  {
    path: ':id',
    component: CarDetailsComponent,
    canActivate: mapToCanActivate([AuthGuard]),
  },
];

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule.forChild(routes),
    FormsModule,
    ReactiveFormsModule,
    ReservationModule,
  ],
  declarations: [
    CarListComponent,
    CarEditComponent,
    CarDetailsComponent,
    CarCalenderComponent,
  ],
  providers: [
    CarService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HeadersInterceptor,
      multi: true,
    },
    DatePipe,
  ],
  exports: [RouterModule],
})
export class CarModule {}
