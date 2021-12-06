using AirlineWeb.Dtos;
using AirlineWeb.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using AirlineWeb.Models;
using System;
using AirlineWeb.MessageBus;

namespace AirlineWeb.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class FlightDetailController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public FlightDetailController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _context = context;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }
        [HttpGet("{flightCode}", Name = "GetFlightByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightByCode(string flightCode)
        {
            var result = _context.FlightDetails.Where(w => w.FlightCode == flightCode).FirstOrDefault();

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public ActionResult<FlightDetailReadDto> Post(FlightDetailCreateDto flightDetailCreateDto)
        {
            var detail = _context.FlightDetails.Where(w => w.FlightCode == flightDetailCreateDto.FlightCode).FirstOrDefault();

            if (detail != null)
                return Conflict();

            var mappedCreate = _mapper.Map<FlightDetail>(flightDetailCreateDto);

            try
            {
                _context.Add(mappedCreate);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var result = _mapper.Map<FlightDetailReadDto>(mappedCreate);

            return CreatedAtRoute(nameof(GetFlightByCode), new { flightCode = result.FlightCode }, result);

        }

        [HttpPut("{id}")]
        public ActionResult<FlightDetailReadDto> Put(int id, FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.Id == id);

            if (flight == null)
                return NotFound();

            decimal oldPrice = flight.Price;

            _mapper.Map(flightDetailUpdateDto, flight);

            try
            {
                _context.SaveChanges();

                if (oldPrice != flight.Price)
                {
                    Console.WriteLine("Price Changed - Place a message on the bus");

                    var message = new NotificationMessageDto{
                        WebhookType = "pricechange",
                        OldPrice = oldPrice,
                        NewPrice = flight.Price,
                        FlightCode = flight.FlightCode
                    };

                    _messageBusClient.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No price change.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return NoContent();
        }
    }
}
