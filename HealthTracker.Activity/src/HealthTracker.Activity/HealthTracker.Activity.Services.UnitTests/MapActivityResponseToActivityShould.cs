using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthTracker.Activity.Common;
using HealthTracker.Activity.Services.Mappers;
using System.Linq;
using Xunit;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Activity.Services.UnitTests
{
    public class MapActivityResponseToActivityShould
    {
        [Fact]
        public void MapActivityResponseSuccessfully()
        {
            // Arrange
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();

            var mappingConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapActivityResponseToActivity());
            });

            var mapper = mappingConfiguration.CreateMapper();

            // Act
            var activity = mapper.Map<mdl.Activity>(activityResponse);

            // Assert
            using (new AssertionScope())
            {
                activity.CaloriesBurned.Should().Be(activityResponse.summary.caloriesOut);
                activity.Steps.Should().Be(activityResponse.summary.steps);
                activity.Distance.Should().Be(activityResponse.summary.distances.Where(x => x.activity == "total").Select(x => x.distance).FirstOrDefault());
                activity.Floors.Should().Be(activityResponse.summary.floors);
                activity.MinutesSedentary.Should().Be(activityResponse.summary.sedentaryMinutes);
                activity.MinutesLightlyActive.Should().Be(activityResponse.summary.lightlyActiveMinutes);
                activity.MinutesFairlyActive.Should().Be(activityResponse.summary.fairlyActiveMinutes);
                activity.MinutesVeryActive.Should().Be(activityResponse.summary.veryActiveMinutes);
                activity.ActivityCalories.Should().Be(activityResponse.summary.activityCalories);
            }
        }
    }
}
