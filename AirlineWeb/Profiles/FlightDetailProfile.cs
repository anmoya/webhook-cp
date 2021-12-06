using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;

namespace AirlineWeb.Profiles
{
    public class FlightDetailProfile : Profile
    {
        public FlightDetailProfile()
        {

            CreateMap<FlightDetailCreateDto, FlightDetail>().ReverseMap();
            CreateMap<FlightDetailReadDto, FlightDetail>().ReverseMap();
            CreateMap<FlightDetailUpdateDto, FlightDetail>().ReverseMap();


        }
    }
}