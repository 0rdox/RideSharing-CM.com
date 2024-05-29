using System.Net;
using System.Net.Mail;
using DomainServices;
using DonainModel;

namespace API.Services
{
    public class EmailService : IEmailService {
        private static readonly string _emailAddress = new("avanscmgroep5@outlook.com");
        private static readonly string _emailPassword = new("CeCunU^qi3+ZzR8");
        
        private readonly SmtpClient _smtpClient = new(){
            Host = "smtp-mail.outlook.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailAddress, _emailPassword)
        };

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailAddress),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<bool> SendRequestAsync(string name, string email, string destination, string departureTime, string requestId, string requestToken) {
            var mailMessage = new MailMessage{
                From = new MailAddress(_emailAddress),
                Subject = $"{name} has requested to ride along!",
                //The body is a message that is sent to the owner of the car when someone requests to join their car
                //It contains the message and a button to decline or accept the request
                Body = $"<div style='display: flex; align-items: center;'>" +
                        $"<img src='https://www.resalepartners.nl/wp-content/uploads/2021/11/CM_Tekengebied-1_Tekengebied-1.png' style='height:150px'/>" +
                        $"</div>" +
                        
                        $"<div style='background-color:#101f1f; text-align:center'>" +
                        $"<div style=''>" +
                        $"<img src='https://cdn-icons-png.flaticon.com/128/3097/3097144.png'/>" +
                        $"</div>" +
                        $"<h1 style='color:white; text-align:center; padding:30px; padding-top:0px;'>{name} wants to ride along!</h1>" +
                        $"</div>" +
                        
                        $"<div style='background-color:white'>" +   
                        $"<div style='text-color:black; text-align:center; font-size:16px'>" +
                        $"<p>Hello,</p>" +
                        $"<p>{name} wants to ride along to {destination} at {departureTime}.</p>" +
                        $"<p>To accept or deny this request, use the buttons below.</p>" +
                        $"</div>" +
                        $"</div>" +
                        
                        
                        $"<div style='text-align:center'>" +
                        $"<a href='https://cmxavans.azurewebsites.net/api/request/{requestId}/verify?token={requestToken}&status=ACCEPTED'>" +
                        $"<button style='display: inline-block; padding: 10px 20px; font-size: 16px; cursor: pointer; text-align: center; text-decoration: none; outline: none; border: none; border-radius: 5px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); background-color: #4CAF50; color: #fff;'>Accept</button>" +
                        $"</a>" +
                        
                        $"<a href='https://cmxavans.azurewebsites.net/api/request/{requestId}/verify?token={requestToken}&status=DENIED'>" +
                        $"<button style='display: inline-block; padding: 10px 20px; font-size: 16px; cursor: pointer; text-align: center; text-decoration: none; outline: none; border: none; border-radius: 5px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); background-color: #FF5733; color: #fff; margin-left: 10px;'>Deny</button>" +
                        $"</a>" +
                        $"</div>" +
                        
                        $"<div style='background-color:#101f1f; text-align:center; height:50px; margin-top:20px;'>" +
                        $"<p style='color:white; padding-top:15px'>CM.com ©  All Rights Reserved</p>" +
                        $"</div>"
,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await _smtpClient.SendMailAsync(mailMessage);
            return true;
        }

        public async Task<bool> SendDeletionEmailAsync(string[] emails, string name, string destination, string departureTime) {
            //Send an email to the original creator and all who created a request whenever a reservatrion is deleted
            //The email only contains a message that the reservation has been deleted with the details of the reservation
            foreach (var email in emails) {
                var mailMessage = new MailMessage{
                    From = new MailAddress(_emailAddress),
                    Subject = $"Your reservation has been cancelled",
                    Body = $"<div style='display: flex; align-items: center;'>" +
                        $"<img src='https://www.resalepartners.nl/wp-content/uploads/2021/11/CM_Tekengebied-1_Tekengebied-1.png' style='height:150px'/>" +
                        $"</div>" +

                        $"<div style='background-color:#101f1f; text-align:center'>" +
                        $"<div style=''>" +
                        $"<img src='https://cdn-icons-png.flaticon.com/128/8184/8184192.png'/>" +
                        $"</div>" +
                        $"<h1 style='color:white; text-align:center; padding:30px; padding-top:0px;'>Your reservation has been cancelled.</h1>" +
                        $"</div>" +

                        $"<div style='background-color:white'>" +
                        $"<div style='text-color:black; text-align:center; font-size:16px'>" +
                        $"<p>Hello,</p>" +
                        $"<p>{name} has cancelled their original reservation to {destination} at {departureTime}.</p>" +
                        $"<p>This means everyone's car-pool request has automatically been cancelled aswell...</p>" +
                        $"<p>We're sorry for the inconvenience.</p>" +
                        $"</div>" +
                        $"</div>" +



                        $"<div style='background-color:#101f1f; text-align:center; height:50px; margin-top:20px;'>" +
                        $"<p style='color:white; padding-top:15px'>CM.com ©  All Rights Reserved</p>" +
                        $"</div>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await _smtpClient.SendMailAsync(mailMessage);
            }
            return true;
        }
        public async Task<bool> SendRequestStatusAsync(string email, string name, Status status, string destination, string departureTime) {
            // Send email to the person who requested to join a car whenever their request is accepted or declined
            // The email contains a message that their request has been accepted or declined

            var mailMessage = new MailMessage {
                From = new MailAddress(_emailAddress),
                Subject = $"Your request has been {status.ToString().ToLower()}",
                Body = $"<div style='display: flex; align-items: center;'>" +
                                $"<img src='https://www.resalepartners.nl/wp-content/uploads/2021/11/CM_Tekengebied-1_Tekengebied-1.png' style='height:150px'/>" +
                                $"</div>" +

                                $"<div style='background-color:#101f1f; text-align:center'>" +
                                $"<div>" +
                                $"<img style='margin-top:10px'src='{(status == Status.ACCEPTED ? "https://cdn-icons-png.flaticon.com/128/4225/4225683.png" : "https://emojis.slackmojis.com/emojis/images/1643509886/45489/red-x-mark.png?1643509886")}'/>" +
                                $"</div>" +
                                $"<h1 style='color:white; text-align:center; padding:30px; padding-top:0px;'>Your request has been {(status == Status.ACCEPTED ? "accepted" : "denied")}.</h1>" +
                                $"</div>" +

                                $"<div style='background-color:white'>" +
                                $"<div style='text-color:black; text-align:center; font-size:16px'>" +
                                $"<p>Hello,</p>" +
                                $"<p>Your request has been {(status == Status.ACCEPTED ? "accepted" : "denied")}.</p>" +
                                $"{(status == Status.ACCEPTED ? $"<p>Your ride to {destination} leaves at {departureTime}.</p>" : "<p>We're sorry for the inconvenience.</p>")}" +
                                $"</div>" +
                                $"</div>" +

                                $"<div style='background-color:#101f1f; text-align:center; height:50px; margin-top:20px;'>" +
                                $"<p style='color:white; padding-top:15px'>CM.com ©  All Rights Reserved</p>" +
                                $"</div>",
                IsBodyHtml = true,
            };


            mailMessage.To.Add(email);

            await _smtpClient.SendMailAsync(mailMessage);
            return true;
        }
    }
}
