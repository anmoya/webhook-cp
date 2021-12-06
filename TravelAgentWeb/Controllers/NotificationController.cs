using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Models;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace TravelAgentWeb.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly TravelAgentDbContext _context;

        public NotificationController(TravelAgentDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public ActionResult<FlightDetailUpdateDto> Post(FlightDetailUpdateDto flightDetailUpdateDto)
        {
            Console.WriteLine("Webhook Received from: " + flightDetailUpdateDto.Publisher);

            var secret = _context.WebhookSercrets.Where(f =>
                                           f.Publisher == flightDetailUpdateDto.Publisher &&
                                           f.Secret == flightDetailUpdateDto.Secret
            ).FirstOrDefault();

            if (secret == null)
            {
                LogToConsoler(ConsoleColor.Red, "Invalid Secret - Ignore Webhook");
                return NotFound();
            }
            else
            {
                LogToConsoler(ConsoleColor.Green, "Valid Webhook");
                LogToConsoler(ConsoleColor.Green, $"Old price {flightDetailUpdateDto.OldPrice} - New Price {flightDetailUpdateDto.NewPrice}");
            }




            return Ok();
        }

        private void LogToConsoler(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}