import {
  AfterViewInit,
  Component,
  ElementRef,
  Input,
  OnDestroy,
  OnInit,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { IReservation } from '../../../models/reservation.interface';
import { DatePipe } from '@angular/common';
import { CarService } from '../car.service';
import { Subscription } from 'rxjs';
import { Modal, initModals } from 'flowbite';
import { Router } from '@angular/router';
import { Destination } from '../../../models/destination.enum';
import { Id } from '../../../models/id.type';
import { IReservationInputInfoModel } from '../../../viewmodels/reservationInputInfoModel.interface';
import { ICar } from '../../../models/car.interface';
import { StorageService } from '../../../common/localstorage/localstorage.service';
import {
  ICreateRequest,
  IRequest,
  Status,
} from '../../../models/request.interface';
import { IUser } from '../../../models/user.interface';
import { ReservationService } from '../../reservation/reservation.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-car-calender',
  templateUrl: './car-calender.component.html',
  styleUrl: './car-calender.component.css',
})
export class CarCalenderComponent implements OnInit, AfterViewInit, OnDestroy {
  hoursInDay = new Array(48);
  daysInWeek = new Array(7);
  weekStart = this.getWeekStart();
  weekEnd = this.getWeekEnd();
  reservations: IReservation[] | null = null;
  focussedReservation: IReservation | null = null;
  subscription: Subscription | null = null;
  addReservationInputInfo: IReservationInputInfoModel | null = null;

  @Input() car: ICar | null = null;

  yourReservation: boolean = false;
  request: ICreateRequest | null = null;
  seats = 0;
  usedSeats = 0;

  @ViewChild('dayContainer1') dayContainerMonday:
    | ElementRef<HTMLDivElement>
    | any;
  @ViewChild('dayContainer2') dayContainerTuesday:
    | ElementRef<HTMLDivElement>
    | any;
  @ViewChild('dayContainer3') dayContainerWednesday:
    | ElementRef<HTMLDivElement>
    | any;
  @ViewChild('dayContainer4') dayContainerThursday:
    | ElementRef<HTMLDivElement>
    | any;
  @ViewChild('dayContainer5') dayContainerFriday:
    | ElementRef<HTMLDivElement>
    | any;
  @ViewChild('dayContainer6') dayContainerSaturday:
    | ElementRef<HTMLDivElement>
    | any;
  @ViewChild('dayContainer7') dayContainerSunday:
    | ElementRef<HTMLDivElement>
    | any;

  @ViewChild('timeIndicator') timeIndicator: ElementRef<HTMLDivElement> | any;

  constructor(
    private readonly carService: CarService,
    private readonly renderer: Renderer2,
    private readonly datePipe: DatePipe,
    private readonly router: Router,
    private readonly storageService: StorageService,
    private readonly toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.retrieveReservations();
  }

  ngAfterViewInit(): void {
    this.showReservations();
    this.setTimeIndicator();
  }

  ngOnDestroy(): void {
    if (this.subscription) this.subscription.unsubscribe();
  }

  setTimeIndicator(): void {
    const currentDate = new Date();

    // prevent from showing when not within focussed week
    if (currentDate < this.weekStart || currentDate > this.weekEnd) {
      this.renderer.setAttribute(
        this.timeIndicator.nativeElement,
        'class',
        'hidden'
      );
      return;
    }

    const dayOfWeek = currentDate.getDay();
    const hourOfDay = currentDate.getHours();
    const minutesOfDay = currentDate.getMinutes();
    const minutes = this.mapRange(
      hourOfDay * 60 + minutesOfDay,
      0,
      1440,
      0,
      1152
    );
    let dayOffset: string;
    let width: string;

    switch (dayOfWeek) {
      case 0:
        // Sunday
        dayOffset = 'ms-[calc(100%/7*6)]';
        width = 'max-w-[calc(100%/1)]';
        break;
      case 1:
        // Monday
        dayOffset = 'ms-[calc(100%/7*0)]';
        width = 'max-w-[calc(100%/7)]';
        break;
      case 2:
        // Tuesday
        dayOffset = 'ms-[calc(100%/7*1)]';
        width = 'max-w-[calc(100%/6)]';
        break;
      case 3:
        // Wednesday
        dayOffset = 'ms-[calc(100%/7*2)]';
        width = 'max-w-[calc(100%/5)]';
        break;
      case 4:
        // Thursday
        dayOffset = 'ms-[calc(100%/7*3)]';
        width = 'max-w-[calc(100%/4)]';
        break;
      case 5:
        // Friday
        dayOffset = 'ms-[calc(100%/7*4)]';
        width = 'max-w-[calc(100%/3)]';
        break;
      case 6:
        // Saturday
        dayOffset = 'ms-[calc(100%/7*5)]';
        width = 'max-w-[calc(100%/2)]';
        break;
      default:
        dayOffset = 'ms-[calc(100%/7*0)]';
        width = 'max-w-[calc(100%/7)]';
        break;
    }

    this.renderer.setAttribute(
      this.timeIndicator.nativeElement,
      'class',
      dayOffset
    );
    this.renderer.setStyle(
      this.timeIndicator.nativeElement,
      'margin-top',
      `${minutes}px`
    );
    this.renderer.setAttribute(
      this.timeIndicator.nativeElement.children[0],
      'class',
      width
    );
  }

  /**
   * Show a popup with the reservation component
   */
  addReservation(day: number): void {
    const startingTime = new Date(this.weekStart);
    startingTime.setDate(this.weekStart.getDate() + day);

    const endingTime = new Date(startingTime);
    endingTime.setDate(endingTime.getDate() + 1);

    this.addReservationInputInfo = {
      startingTime: startingTime,
      endingTime: endingTime,
      car: this.car as ICar,
    };
  }

  hideAddReservation(event: string): void {
    const $targetModal = document.getElementById('add-reservation');
    const options = {};
    const instanceOptions = {};
    const modal = new Modal($targetModal, options, instanceOptions);
    modal.hide();

    this.clearReservations();
    this.retrieveReservations();
  }

  /**
   * Render all the reservations in the calender
   */
  showReservations(): void {
    this.reservations?.forEach((reservation) => {
      // Get elementStart and elementEnd
      let elementStart = reservation.departureDate.getDay();

      // Get distance from top and bottom
      const distanceFromTop =
        reservation.departureDate.getHours() * 2 +
        Math.floor(reservation.departureDate.getMinutes() / 30);
      const distanceFromBottom =
        (24 - reservation.arrivalDate.getHours()) * 2 -
        Math.floor(reservation.departureDate.getMinutes() / 30);

      const dateDifference = this.dateDifference(
        reservation.departureDate,
        reservation.arrivalDate
      );

      let renderedDate = reservation.departureDate;

      for (let day = 0; day < dateDifference + 1; day++) {
        // Don't rendering when outside the date range
        if (renderedDate > this.weekEnd) break;
        if (renderedDate < this.weekStart) {
          elementStart++;
          if (elementStart > 6) elementStart = 0;
          renderedDate.setDate(renderedDate.getDate() + 1);
          continue;
        }

        // Select container to inject reservation element into
        let container: any;

        switch (elementStart) {
          case 0: // Sunday
            container = this.dayContainerSunday;
            break;

          case 1: // Monday
            container = this.dayContainerMonday;
            break;

          case 2: // Tuesday
            container = this.dayContainerTuesday;
            break;

          case 3: // Wednesday
            container = this.dayContainerWednesday;
            break;

          case 4: // Thursday
            container = this.dayContainerThursday;
            break;

          case 5: // Friday
            container = this.dayContainerFriday;
            break;

          case 6: // Saterday
            container = this.dayContainerSaturday;
            break;
        }

        // Inject reservation element into container
        let elementDistanceFromTop;
        let elementDistanceFromBottom;
        let colour;
        let available;

        if (day == 0) {
          elementDistanceFromTop = 1.5 * distanceFromTop;
        } else {
          elementDistanceFromTop = 0;
        }

        if (day < dateDifference) {
          elementDistanceFromBottom = 0;
        } else {
          elementDistanceFromBottom = 1.5 * distanceFromBottom;
        }

        if (reservation.seats < reservation.car.seats) {
          colour = 'bg-blue-600';
          available = true;
        } else {
          colour = 'bg-red-600';
          available = false;
        }

        const reservationContainer = this.renderer.createElement('button');
        this.renderer.setAttribute(
          reservationContainer,
          'class',
          `absolute top-[${elementDistanceFromTop}rem] bottom-[${elementDistanceFromBottom}rem] left-0 right-0 pointer-events-auto m-[2px] text-left`
        );
        this.renderer.setAttribute(
          reservationContainer,
          'data-modal-target',
          'reservation-info'
        );
        this.renderer.setAttribute(
          reservationContainer,
          'data-modal-show',
          'reservation-info'
        );
        this.renderer.listen(reservationContainer, 'click', () => {
          this.showReservationInfo(reservation.id);
        });

        const reservationComponent = this.renderer.createElement('div');
        this.renderer.setAttribute(
          reservationComponent,
          'class',
          `h-full px-1 md:px-2 rounded ${colour} text-sm text-white overflow-hidden`
        );

        const dateParagraph = this.renderer.createElement('p');
        this.renderer.setAttribute(
          dateParagraph,
          'class',
          'font-bold truncate'
        );
        const dateText = this.renderer.createText(
          `${this.formatTimePipe(
            reservation.departureDate
          )} - ${this.formatTimePipe(reservation.arrivalDate)}`
        );

        const destinationParagraph = this.renderer.createElement('p');
        this.renderer.setAttribute(
          destinationParagraph,
          'class',
          'break-words'
        );
        const destinationText = this.renderer.createText(
          `${reservation.destination as Destination}`
        );

        this.renderer.appendChild(dateParagraph, dateText);
        this.renderer.appendChild(reservationComponent, dateParagraph);
        this.renderer.appendChild(destinationParagraph, destinationText);
        this.renderer.appendChild(reservationComponent, destinationParagraph);
        this.renderer.appendChild(reservationContainer, reservationComponent);
        this.renderer.appendChild(
          container.nativeElement,
          reservationContainer
        );

        if (!available) {
          const noAvailableParagraph = this.renderer.createElement('p');
          this.renderer.setAttribute(noAvailableParagraph, 'class', 'truncate');
          const noAvailableIcon = this.renderer.createElement('i');
          this.renderer.setAttribute(
            noAvailableIcon,
            'class',
            'fa-solid fa-circle-exclamation'
          );
          const noAvailableText = this.renderer.createText(' Full');
          this.renderer.appendChild(noAvailableParagraph, noAvailableIcon);
          this.renderer.appendChild(noAvailableParagraph, noAvailableText);
          this.renderer.appendChild(reservationComponent, noAvailableParagraph);
        }

        // Incrementing
        elementStart++;
        if (elementStart > 6) elementStart = 0;
      }
    });
    initModals();
  }

  /**
   * Clears all the dayContainers.
   */
  clearReservations(): void {
    this.removeChildElements(this.dayContainerMonday);
    this.removeChildElements(this.dayContainerTuesday);
    this.removeChildElements(this.dayContainerWednesday);
    this.removeChildElements(this.dayContainerThursday);
    this.removeChildElements(this.dayContainerFriday);
    this.removeChildElements(this.dayContainerSaturday);
    this.removeChildElements(this.dayContainerSunday);

    this.reservations = [];
  }

  /**
   * Go back 1 week on the calender.
   */
  lastWeek(): void {
    this.weekStart.setDate(this.weekStart.getDate() - 7);
    this.weekEnd.setDate(this.weekEnd.getDate() - 7);

    this.clearReservations();
    this.retrieveReservations();
    this.setTimeIndicator();
  }

  /**
   * Go forwared 1 week on the calender.
   */
  nextWeek(): void {
    this.weekStart.setDate(this.weekStart.getDate() + 7);
    this.weekEnd.setDate(this.weekEnd.getDate() + 7);

    this.clearReservations();
    this.retrieveReservations();
    this.setTimeIndicator();
  }

  /**
   * Clear all children in a DOM container.
   * @param dayContainer The container that must be cleared.
   */
  removeChildElements(dayContainer: any) {
    const childElements = Array.from(dayContainer.nativeElement.children);

    for (let child of childElements) {
      this.renderer.removeChild(dayContainer.nativeElement, child);
    }
  }

  /**
   * Sets the attribute of focussedReservation to the reservation to show.
   * @param id The id of the reservation to be shown.
   */
  showReservationInfo(id: Id): void {
    if (this.reservations) {
      this.reservations.forEach((reservation) => {
        if (reservation.id === id) {
          this.focussedReservation = reservation;

          this.getAvailableSeats(reservation.id);

          this.storageService.getUserId().subscribe((userId) => {
            if (userId == reservation.user.id) {
              this.yourReservation = true;
            } else {
              this.yourReservation = false;
            }
          });
        }
      });
    }
  }

  /**
   * Checks if a number is an even number.
   * @param number The number that will be checked.
   * @returns True if the number is even. False if the number is uneven.
   */
  isEvenNumber(number: number): boolean {
    if (number % 2 == 0) {
      return true;
    }
    return false;
  }

  /**
   * @returns the id of the car
   */
  getCarId(): string {
    const splittedUrl = this.router.url.split('/');
    const carId = splittedUrl[splittedUrl.length - 1];

    return carId;
  }

  /**
   * @returns The first day of the week as a Date object.
   */
  getWeekStart(): Date {
    const date = new Date();
    date.setDate(date.getDate() - date.getDay() + 1);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  /**
   * @returns The last day of the week as a Date object.
   */
  getWeekEnd(): Date {
    const date = new Date();
    date.setDate(date.getDate() + (7 - date.getDay()));
    date.setHours(23);
    date.setMinutes(59);
    date.setSeconds(59);
    return date;
  }

  /**
   * Checks the difference between two dates in days.
   * @param first The first date.
   * @param second The second date.
   * @returns The difference between the dates in days.
   */
  dateDifference(first: any, second: any) {
    const dateDifference = second - first;
    const dateDifferenceDays = Math.floor(dateDifference / (1000 * 3600 * 24));
    return dateDifferenceDays;
  }

  /**
   * Format time as HH:mm using a Date object.
   * @param date The time that will be formatted as a Date object.
   * @returns the formatted time as a HH:mm string.
   */
  formatTimePipe(date: Date): string {
    return this.datePipe.transform(date.toISOString(), 'HH:mm')!;
  }

  /**
   * Formats the time in hours from a number to a 24:00 hour string.
   * @param hour The hour that needs to be formatted.
   * @returns The formatted time in 24:00 hours as a string.
   */
  formatTime(hour: number): string {
    if (hour < 10) {
      return `0${hour}:00`;
    }
    return `${hour.toString()}:00`;
  }

  /**
   * Re-maps a number from one range to another.
   * @param value the incoming value to be converted
   * @param low1 lower bound of the value's current range
   * @param high1 upper bound of the value's current range
   * @param low2 lower bound of the value's target range
   * @param high2 upper bound of the value's target range
   * @returns returns a re-mapped value
   */
  mapRange(
    value: number,
    low1: number,
    high1: number,
    low2: number,
    high2: number
  ) {
    return low2 + ((high2 - low2) * (value - low1)) / (high1 - low1);
  }

  /**
   * Gets all reservations from the carService and shows them.
   */
  retrieveReservations(): void {
    this.subscription = this.carService
      .getReservations(this.getCarId(), this.weekStart, this.weekEnd)
      .subscribe((results) => {
        this.reservations = results;
        this.showReservations();
      });
  }

  /**
   * Creates a request for the reservation
   */
  createRequest(): void {
    const $targetModal2 = document.getElementById('reservation-info');
    const options = {};
    const instanceOptions = {};
    const modal2 = new Modal($targetModal2, options, instanceOptions);
    modal2.hide();

    this.request = {
      seats: this.seats,
      reservationId: this.focussedReservation?.id!,
    };

    this.carService.createRequest(this.request!).subscribe({
      next: () => {
        //show popup that you requested a ride

        this.toastr.success(
          `Ride requested with ${this.seats} seats.`,
          'Request Sent!'
        );
      },
      error: (error) => {
        if (error instanceof HttpErrorResponse) {
          this.toastr.error(error.error.message, 'Error!');
        } else {
          this.toastr.error('Please try again alter', 'Server error');
        }
      },
    });
  }

  /**
   * Checks the seats being used by filtering the ACCEPTED requests by reservationId
   * @param id The id of the reservation
   */
  getAvailableSeats(id: number): void {
    this.carService.getAcceptedRequestsByReservationId(id).subscribe({
      next: (requests) => {
        const usedSeats = requests.reduce(
          (totalSeats, request) => totalSeats + request.seats,
          0
        );
        this.usedSeats = usedSeats;
      },
      error: (error) => {
        if (error instanceof HttpErrorResponse) {
          this.toastr.error(error.error.message, 'Error!');
        } else {
          this.toastr.error('Please try again alter', 'Server error');
        }
      },
    });
  }
}
