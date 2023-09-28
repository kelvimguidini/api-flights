using System;
using System.Collections.Generic;
using System.Security.Claims;
using flights.application.Interfaces;
using flights.crosscutting.Messages.Interfaces;
using flights.domain.Models.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace flights.api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingsController : MainController
    {
        private readonly IBookingService _bookingService;
        private readonly INotificator _notification;
        public BookingsController(IBookingService bookingService,
            INotificator notification) : base(notification)
        {
            _bookingService = bookingService;
            _notification = notification;
        }


        /// <summary>
        /// Reservar
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
        public IActionResult CreateBooking(BookingDTO booking)
        {
            var username = User.FindFirst(ClaimTypes.Email).Value;
            Booking result = new Booking();
            try
            {
                result = _bookingService.Sell(booking, username);
            }
            catch (Exception e)
            {
                _notification.notify(e.Message);
            }
            return CustomResponse(result);
        }
    }
}