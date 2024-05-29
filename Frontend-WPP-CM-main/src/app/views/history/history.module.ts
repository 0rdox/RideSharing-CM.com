import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';

import { HeadersInterceptor } from '../../middleware/headers.interceptor';
import { UserModule } from './../user/user.module';
import { AuthGuard } from '../../common/guards/auth.guard';
import { HistoryComponent } from './history/history.component';
import { HistoryService } from './history.service';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: HistoryComponent,
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
  declarations: [HistoryComponent],
  providers: [
    HistoryService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HeadersInterceptor,
      multi: true,
    },
  ],
  exports: [RouterModule],
})
export class HistoryModule {}

