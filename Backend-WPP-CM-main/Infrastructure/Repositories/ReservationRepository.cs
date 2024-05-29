using DomainServices;
using DonainModel;
using DonainModel.dto;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Repositories; 

public class ReservationRepository : IReservationRepository {
    private readonly AppDbContext _context;
    public static readonly string internalServerErrorException = "Internal server error";

    public ReservationRepository(AppDbContext context) {
        _context = context;
    }

    public IEnumerable<Reservation> GetAll(int? userId = null, int? carId = null, DateTime ? beginDate = null, DateTime? endDate = null) {
        //get reservations
        var reservations = _context.Reservations.Include(i => i.car).Include(i => i.requests).Include(i => i.user).AsEnumerable();

        //user filter
        if(userId != null) {
            reservations = reservations.Where(i => i.user.id == userId).AsEnumerable();
        }

        //car filter
        if(carId != null) {
            reservations = reservations.Where(i => i.car.id == carId).AsEnumerable();
        }

        //beginDate filter
        if(beginDate != null) {
            reservations = reservations.Where(i => i.departureDate >= beginDate || i.arrivalDate >= beginDate).AsEnumerable();
        }

        //endDate filter
        if (endDate != null) {
            reservations = reservations.Where(i => i.arrivalDate <= endDate || i.departureDate <= endDate).AsEnumerable();
        }

        return reservations.OrderBy(i => i.departureDate);
    }

    public Reservation? GetById(int id) {
        var reservations = _context.Reservations.Where(i => i.id == id).Include(i => i.car).Include(i => i.user).AsEnumerable();

        if(reservations.Count() == 0) {
            return null;
        }

        return reservations.First();
    }

    public async Task<Reservation> Create(ReservationDto dto, int userId) {
        //check if dates are correct
        checkDates(dto.departureDate, dto.arrivalDate);

        //Check if reservation is possible
        if (_context.Reservations.Count() != 0) {
            CheckReservationPossible(dto);
        }

        //Get reservation
        var reservation = GetReservationForDto(dto, userId);

        //Add to Db
        var newReservation = _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var createdReservation = _context.Reservations.Where(i => i.id == newReservation.Entity.id).Include(i => i.car).Include(i => i.user).AsEnumerable();

        if(createdReservation.Count() == 0) {
            throw new Exception(internalServerErrorException);
        }

        return createdReservation.First();
    }

    private Reservation GetReservationForDto(ReservationDto dto, int userId) {
        //Check if car exist
        var car = _context.Cars.Find(dto.carId) ?? throw new Exception($"Car with id {dto.carId} does not exist");

        //Check if user exist
        var user = _context.Users.Find(userId) ?? throw new Exception($"User with id {userId} does not exist");

        //Check if enough seats
        if(car.seats < dto.seats) {
            throw new Exception($"More seats are wanted than available. Available seats: {car.seats}. Wanted seats: {dto.seats}");
        }

        //Return reservation with correct info
        return new Reservation() {
            departureDate = DateTime.Parse(dto.departureDate),
            arrivalDate = DateTime.Parse(dto.arrivalDate),
            destination = dto.destination,
            willReturn = dto.willReturn,
            car = car,
            user = user,
            seats = dto.seats,
        };
    }

    private void checkDates(string departureDate, string arrivalDate) {
        var result = DateTime.TryParse(departureDate, out DateTime outDepartureDate);
        if (!result) throw new Exception("departureDate needs to be formatted as a DateTime object");

        result = DateTime.TryParse(arrivalDate, out DateTime outArrivalDate);
        if (!result) throw new Exception("arrivalDate needs to be formatted as a DateTime object");
    }

    private void CheckReservationPossible(ReservationDto reservation) {
        //Adds to overlappingReservations if
        //1. departureDate is inbetween other departure and arrival dates
        //2. arrivalDate is inbetween other departure and arrival dates
        var departureDate = DateTime.Parse(reservation.departureDate);
        var arrivalDate = DateTime.Parse(reservation.arrivalDate);
        var overlappingReservations = _context.Reservations.Where(i => i.car.id == reservation.carId).AsEnumerable();

        overlappingReservations = overlappingReservations
            .Where(
                i => ((departureDate.Ticks >= i.departureDate.Ticks && departureDate.Ticks <= i.arrivalDate.Ticks) == true)
                || ((arrivalDate.Ticks >= i.departureDate.Ticks && arrivalDate.Ticks <= i.arrivalDate.Ticks) == true));



        //Check if there are any overlapping reservations
        if (overlappingReservations.Any()) {
            throw new Exception("Registration overlaps with other registrations");
        }
    }

    public Reservation Update(Reservation reservation) {
        throw new NotImplementedException();
    }

    public async Task Delete(int id) {
        //if no reservations with id than return because not found
        var reservation = _context.Reservations.Find(id);
        if(reservation == null) {
            throw new Exception("Reservation does not exist");
        }

        //TODO: sent mail to all requests to let know that reservation has been deleted

        //remove requests and reservations
        _context.Requests.RemoveRange(_context.Requests.Include(i => i.reservation).Where(i => i.reservation.id == reservation.id));
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }
}
