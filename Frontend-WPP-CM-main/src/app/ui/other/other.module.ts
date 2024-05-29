import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HeadersInterceptor } from '../../middleware/headers.interceptor';
import { LoreComponent } from './lore/lore.component';
import { OtherService } from './other.service';

const routes: Routes = [
  {
    path: '',
    component: LoreComponent,
  },
];

@NgModule({
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule,
  ],
  declarations: [LoreComponent],
  providers: [
    OtherService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HeadersInterceptor,
      multi: true,
    },
  ],
  exports: [RouterModule],
})
export class OtherModule {}
