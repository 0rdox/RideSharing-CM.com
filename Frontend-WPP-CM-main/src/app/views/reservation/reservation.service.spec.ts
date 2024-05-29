import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReservationService } from './reservation.service';
import { HttpClient, HttpHandler } from '@angular/common/http';



describe('ReservationService', () => {
  let service: ReservationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HttpClient, HttpHandler]
    });
    service = TestBed.inject(ReservationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });


  
});
