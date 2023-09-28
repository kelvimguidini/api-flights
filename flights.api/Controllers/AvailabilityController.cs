using System;
using Microsoft.AspNetCore.Mvc;
using flights.application.DTO;
using Microsoft.AspNetCore.Authorization;
using flights.application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using flights.crosscutting.Messages.Interfaces;
using flights.domain.Models.Availability;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using KissLog;

namespace flights.api.Controllers
{
    [ApiController]
    [Route("api")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Availability), StatusCodes.Status200OK)]
    public class AvailabilityController : MainController
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly INotificator _notification;
        private readonly ILogger _logger;

        public AvailabilityController(IAvailabilityService availabilityService,
            INotificator notification,
            ILogger logger) : base(notification)
        {
            _availabilityService = availabilityService;
            _notification = notification;
            _logger = logger;
        }

        /// <summary>
        /// Disponibilidade de Voos
        /// </summary>
        [HttpGet]
        [Route("search")]
        [Authorize]
        public async Task<IActionResult> SearchAvailability(
            string departureCode,
            string arrivalCode,
            DateTime departureDate,
            string returnDate=null,
            int adtCount=0,
            int chdCount=0,
            int infCount=0
        )
        {
            AvailabilityRQDTO availabilityRQ = new AvailabilityRQDTO()
            {
                DepartureCode = departureCode,
                ArrivalCode = arrivalCode,
                DepartureDate = departureDate,
                CountADT = adtCount,
                CountCHD = chdCount,
                CountINF = infCount,
                CountTotalPassangers = adtCount + chdCount
            };
                       
            if (returnDate != null)
            {
                availabilityRQ.ReturnDate = Convert.ToDateTime(returnDate);
            }

            var username = User.FindFirst(ClaimTypes.Email).Value;
            Availability result = new Availability();
            try
            {
                result = await _availabilityService.GetAvailability(availabilityRQ, username);
            }
            catch(Exception e)
            {
                _notification.notify(e.Message);
            }
            return CustomResponse(result);

        } 
        
        /// <summary>
        /// Disponibilidade de Voos
        /// </summary>
        [HttpGet]
        [Route("airports/autocomplete")]
        [Authorize]
        public async Task<IActionResult> Airport(
            string name
        )
        {
            List<domain.Entities.Airport> result = new List<domain.Entities.Airport>();
            try
            {
                result = (List<domain.Entities.Airport>)await _availabilityService.AirportAutocomplete(name);
            }
            catch (Exception e)
            {
                _notification.notify(e.Message);
            }
            return CustomResponse(result);
        }
    }
}