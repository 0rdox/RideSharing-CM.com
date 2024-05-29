using DomainServices;
using DonainModel;
using DonainModel.dto;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Drawing;

namespace Infrastructure {
    public class RequestRepository : IRequestRepository {
        public static readonly string wrongVerifyToken = "Verify token is not correct";
        public static readonly string internalServerErrorException = "Internal Server Error";
        public static readonly int tokenLenght = 15;
        private readonly AppDbContext _context;

        public RequestRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<Request> Create(RequestDto dto, int userId) {
            //find reservation
            var reservations = _context.Reservations.Where(i => i.id == dto.reservationId).Include(i => i.car).Include(i => i.requests);
            if(reservations.Count() == 0) {
                throw new Exception($"Reservation with id {dto.reservationId} cannot be found");
            }
            var reservation = reservations.First();

            //find user
            var users = _context.Users.Where(i => i.id == userId).Include(i => i.reservations);
            if(users == null) {
                throw new Exception($"User with id {userId} cannot be found");
            }
            var user = users.First();


            //check for enough seats
            var availableSeats = reservation.getTotalAvailalbeSeats();
            if (availableSeats < dto.seats) {
                throw new Exception($"More seats are wanted than are available. Wanted seats: {dto.seats}. Available seats: {availableSeats}");
            }

            var token = generateToken(tokenLenght);

            //make object
            Request newRequest = new Request {
                user = user,
                reservation = reservation,
                seats = dto.seats,
                status = Status.PENDING,
                token = token
            };

            var newEntity = _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            var createdRequest = _context.Requests.Where(i => i.id == newEntity.Entity.id).Include(i => i.reservation).ThenInclude(i => i.car).Include(i => i.user).AsEnumerable();

            if(createdRequest.Count() == 0) {
                throw new Exception(internalServerErrorException);
            }

            return createdRequest.First();
        }

        //generate random token of custom lenght
        private string generateToken(int tokenLenght) {
            var alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rand = new Random();

            char[] chars = new char[tokenLenght];
            for (int i = 0; i < tokenLenght; i++) {
                chars[i] = alphabet[rand.Next(alphabet.Length)];
            }
            return new string(chars);
        }

        public async Task Delete(int id) {
            Request? request = _context.Requests.Find(id);
            if (request == null) {
                throw new Exception($"No request with id {id} could be found");
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Request> GetAll(int? reservationId = null, Status? requestStatus = null, DateTime? creationDateStart = null, DateTime? creationDateEnd = null, int? userId = null) {
            var requests = _context.Requests.Include(i => i.reservation).ThenInclude(i => i.car).Include(i => i.user).AsEnumerable();
            
            if(reservationId != null) {
                requests = requests.Where(i => i.reservation?.id == reservationId);
            }

            if (userId != null) {
                requests = requests.Where(i => i.user?.id == userId);
            }

            if (requestStatus != null){
                requests = requests.Where(i => i.status == requestStatus);
            }

            if(creationDateStart != null) {
                requests = requests.Where(i => i.creationDate >= creationDateStart);
            }

            if (creationDateEnd != null) {
                requests = requests.Where(i => i.creationDate <= creationDateEnd);
            }

            return requests.OrderBy(i => i.reservation.departureDate); ;
        }

        public Request? GetById(int id) {
            var requests = _context.Requests.Where(i => i.id == id).Include(i => i.user).Include(i => i.reservation).ThenInclude(i => i.car).AsEnumerable();
            if (requests.Count() == 0) return null;
            
            return requests.First();
        }

        public async Task<Request> VerifyRequest(int requestId, string verifyToken, Status newStatus) {
            //get request and throw exception if not found
            var request = GetById(requestId);
            if(request == null) {
                throw new Exception($"Request with id {requestId} does not exist");
            }

            //check if request is already verified
            if(request.token == null) {
                throw new Exception($"Request has already been accepted or denied, request is {request.status.ToString().ToLower()}");
            }

            //verify request token
            if (request.token != verifyToken) {
                throw new Exception(wrongVerifyToken);
            } 

            //check if newStatus is PENDING
            if(newStatus == Status.PENDING) {
                throw new Exception("Status needs to be either ACCEPTED or DENIED");
            }

            //check if enough seats are available
            if (newStatus == Status.ACCEPTED) {
                var availableSeats = request.reservation!.getTotalAvailalbeSeats();
                if (request.seats > availableSeats) {
                    throw new Exception($"Not enough seats available. Available seats: {availableSeats}, needed seats to accept {request.seats}");
                }
            }

            request.status = newStatus;
            request.token = null;
            await _context.SaveChangesAsync();
            return request;
        }
    }
}
