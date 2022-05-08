using AutoMapper;
using HealthTracker.Activity.Common;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Activity.Services.Mappers
{
    public class MapActivityResponseToActivity : Profile
    {
        public MapActivityResponseToActivity()
        {
            CreateMap<ActivityResponse, mdl.Activity>()
                .ForMember(
                    dest => dest.CaloriesBurned,
                    opt => opt.MapFrom(src => src.summary.caloriesOut))
                .ForMember(
                    dest => dest.Steps,
                    opt => opt.MapFrom(src => src.summary.steps))
                .ForMember(
                    dest => dest.Distance,
                    opt => opt.MapFrom(src => src.summary.distances.Where(x => x.activity == "total").Select(x => x.distance).FirstOrDefault()))
                .ForMember(
                    dest => dest.Floors,
                    opt => opt.MapFrom(src => src.summary.floors))
                .ForMember(
                    dest => dest.MinutesSedentary,
                    opt => opt.MapFrom(src => src.summary.sedentaryMinutes))
                .ForMember(
                    dest => dest.MinutesLightlyActive,
                    opt => opt.MapFrom(src => src.summary.lightlyActiveMinutes))
                .ForMember(
                    dest => dest.MinutesFairlyActive,
                    opt => opt.MapFrom(src => src.summary.fairlyActiveMinutes))
                .ForMember(
                    dest => dest.MinutesVeryActive,
                    opt => opt.MapFrom(src => src.summary.veryActiveMinutes))
                .ForMember(
                    dest => dest.ActivityCalories,
                    opt => opt.MapFrom(src => src.summary.activityCalories));
        }
    }
}
