import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';
import { DashboardService } from './dashboard.service';
import { HeadersInterceptor } from '../../middleware/headers.interceptor';
import { UserModule } from './../user/user.module';
import { AuthGuard } from '../../common/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: DashboardComponent,
    canActivate: mapToCanActivate([AuthGuard]),
  },
];

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule.forChild(routes),
    UserModule,
  ],
  declarations: [DashboardComponent],
  providers: [
    DashboardService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HeadersInterceptor,
      multi: true,
    },
  ],
  exports: [RouterModule],
})
export class DashboardModule {}
