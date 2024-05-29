import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReservationService } from './reservation.service';
import { AddReservationComponent } from './add-reservation/add-reservation.component';
import { FormsModule } from '@angular/forms'

@NgModule({
  imports: [CommonModule, FormsModule],
  declarations: [AddReservationComponent],
  providers: [ReservationService],
  exports: [AddReservationComponent],
})
export class ReservationModule {}
