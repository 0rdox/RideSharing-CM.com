using DomainServices;
using DonainModel;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories; 

public class CarRepository : ICarRepository {
    private readonly AppDbContext _context;
    
    public CarRepository(AppDbContext context) {
        _context = context;
    }

    public IEnumerable<Car> GetAll(
        DateTime? departureDate = null, 
        string? destination = null, 
        int? numberOfSeats = null) {
        var cars = _context.Cars.Include(i => i.reservations).ThenInclude(i => i.requests).AsEnumerable();

        //Get all cars that have either 
        //1. departureDate that same and at least 1 seat available
        //2. departureDate falls not in between departure and arrival date of another reservation
        //3. there are no reservations
        if (departureDate != null) {
            cars = cars
                .Where
                    (i => i.reservations.Where(j => DateIsSame(j.departureDate, (DateTime)departureDate) && j.getTotalAvailalbeSeats() >= 1).Count() >= 1
                    || i.reservations.Where(j => DateFallsInBetweenTwoDates((DateTime)departureDate, j.departureDate, j.arrivalDate) == false).Count() >= 1
                    || i.reservations.IsNullOrEmpty())
                .AsEnumerable();
        }

        //Destinatin filter
        //if there is a departureDate also give cars that are not registerd at that time
        if (destination != null) {
            //Get all cars that have either
            //1. where the destination is the same
            //2. departureDate falls not in between departure and arrival date of another reservation 
            //3. where there are no reservations
            if (departureDate != null) {
                cars = cars.Where(i => i.reservations.Where(j => j.destination == destination).Count() >= 1 
                    || i.reservations.Where(j => DateFallsInBetweenTwoDates((DateTime)departureDate, j.departureDate, j.arrivalDate) == false).Count() >= 1
                    || i.reservations.IsNullOrEmpty())
                    .AsEnumerable();
            } else {
                //Get all cars that have either
                //1. where the destination is the same
                //2. where there are no reservations
                cars = cars.Where(i => i.reservations.Where(j => j.destination == destination).Count() >= 1 
                    || i.reservations.IsNullOrEmpty())
                    .AsEnumerable();
            }
        }

        //Get all cars that have either
        //1. there are registrations available with enough seats available
        //2. there are no reservations
        if(numberOfSeats != null) {
            cars = cars.Where(i => i.reservations.Where(j => (j.getTotalAvailalbeSeats() - numberOfSeats) >= 0).Any() 
                || (i.reservations.IsNullOrEmpty() && i.seats >= numberOfSeats))
                .AsEnumerable();
        }

        //don't return reservations aswell
        foreach (var item in cars)
        {
            item.reservations = new List<Reservation>();
        }
        return cars;
    }

    private static bool DateIsSame(DateTime date1, DateTime date2) {
        if (date1.Year == date2.Year && 
            date1.Month == date2.Month && 
            date1.Day == date2.Day) {
            return true; 
        }

        return false;
    }

    private static bool DateFallsInBetweenTwoDates(DateTime checkDate, DateTime start, DateTime end) {
        if (checkDate.Ticks > start.Ticks && checkDate.Ticks < end.Ticks) {
            return true;
        }
        return false;
    }

    public Car? GetById(int id) {
        return _context.Cars.Find(id);
    }

    public async Task<Car?> Create(Car car) {
        Car newCar;

        //check if licensePlate is duplicate
        if (_context.Cars.Where(i => i.licensePlate == car.licensePlate).Any()) {
            throw new Exception("Duplicate licensePlate");
        }

        var result = _context.Cars.Add(car);
        if (result == null) {
            return null;
        }
        newCar = result.Entity;

        var amountAdded = await _context.SaveChangesAsync();
        if (amountAdded != 1) {
            return null;
        }
        return newCar;
    }

    public async Task<Car> Update(int id, Car car) {
        car.id = id;

        //find car
        Car? carToUpdate = _context.Cars.Find(id);
        if (carToUpdate == null) {
            return null;
        }

        //check if licensePlate is duplicate
        if(_context.Cars.Where(i => i.licensePlate == car.licensePlate && i.id != id).Any()) {
            throw new Exception("Duplicate licensePlate");
        }

        //set values
        carToUpdate.licensePlate = car.licensePlate;
        carToUpdate.brand = car.brand;
        carToUpdate.model = car.model;
        carToUpdate.seats = car.seats;
        carToUpdate.imageUrl = car.imageUrl;
        carToUpdate.location = car.location;
        carToUpdate.isAvailable = car.isAvailable;

        //update
        _context.Cars.Update(carToUpdate);
        await _context.SaveChangesAsync();

        return carToUpdate;
    }

    public async Task Delete(int id) {
        Car? carToDelete = _context.Cars.Find(id);
        if (carToDelete == null) {
            return;
        }

        //Delete requests and reservations
        _context.Requests.RemoveRange(_context.Requests.Include(i => i.reservation).ThenInclude(i => i.car).Where(i => i.reservation.car.id == id).AsEnumerable());
        _context.Reservations.RemoveRange(_context.Reservations.Include(i => i.car).Where(i => i.car.id == id).AsEnumerable());

        _context.Cars.Remove(carToDelete);
        await _context.SaveChangesAsync();
    }
}