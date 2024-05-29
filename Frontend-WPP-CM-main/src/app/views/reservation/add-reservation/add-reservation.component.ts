import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { ICreateReservation } from '../../../models/reservation.interface';
import {
  Destination,
  Destination2LabelMapping,
} from '../../../models/destination.enum';
import { ICar } from '../../../models/car.interface';
import { ReservationService } from '../reservation.service';
import { IReservationInputInfoModel } from '../../../viewmodels/reservationInputInfoModel.interface';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-reservation',
  templateUrl: './add-reservation.component.html',
  styleUrl: './add-reservation.component.css',
})
export class AddReservationComponent implements OnInit, OnChanges {
  reservation: ICreateReservation | null = null;
  car: ICar | null = null;
  currentDateTime: Date = new Date();
  address: string | null = null;

  destination2LabelMapping = Destination2LabelMapping;
  destinations: Destination[] = Object.values(Destination) as Destination[];

  @Input() inputInfo: IReservationInputInfoModel | null = null;
  @Output() submitEvent: EventEmitter<string> = new EventEmitter();

  @ViewChild('reservationForm') reservationForm:
    | ElementRef<HTMLInputElement>
    | any;

  constructor(
    private readonly reservationService: ReservationService,
    private readonly toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.reservation = {
      seats: 1,
    } as ICreateReservation;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.reservation && this.inputInfo) {
      this.car = this.inputInfo.car;
      this.reservation.departureDate = this.inputInfo.startingTime;
      this.reservation.arrivalDate = this.inputInfo.endingTime;
      this.reservation.carId = this.inputInfo.car.id;
    }
  }

  invalidDate(date: Date): boolean {
    const processedDate: Date = new Date(date);
    if (processedDate < this.currentDateTime) {
      return true;
    }
    return false;
  }

  onSubmit() {
    // Set correct destination when user selects 'other destination'
    if (this.reservation && this.reservation.destination === 'other') {
      this.reservation.destination = this.address as string;
    }

    this.reservationService
      .createReservation(this.reservation as ICreateReservation)
      .subscribe({
        next: (result) => {
          this.submitEvent.emit('submit');
        },
        error: (error) => {
          if (error instanceof HttpErrorResponse) {
            this.toastr.error(error.error.message, 'Error!');
          } else {
            this.toastr.error('Please try again later', 'Server error');
          }
        },
      });
  }
}
