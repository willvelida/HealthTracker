using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Services.Mappers;
using Xunit;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Services.UnitTests
{
    public class MapSleepResponseToSleepShould
    {
        [Fact]
        public void MapResponseCorrectly()
        {
            var fixture = new Fixture();
            var sleepResponse = fixture.Create<SleepResponse>();

            var mappingConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapSleepResponseToSleep());
            });
            var mapper = mappingConfiguration.CreateMapper();

            var sleep = mapper.Map<mdl.Sleep>(sleepResponse);

            using (new AssertionScope())
            {
                sleep.SleepDate.Should().Be(sleepResponse.sleep[0].dateOfSleep);
                sleep.EndTime.Should().Be(sleepResponse.sleep[0].endTime);
                sleep.StartTime.Should().Be(sleepResponse.sleep[0].startTime);
                sleep.MinutesAsleep.Should().Be(sleepResponse.summary.totalMinutesAsleep);
                sleep.MinutesDeepSleep.Should().Be(sleepResponse.summary.stages.deep);
                sleep.MinutesLightSleep.Should().Be(sleepResponse.summary.stages.light);
                sleep.MinutesREMSleep.Should().Be(sleepResponse.summary.stages.rem);
                sleep.MinutesAwake.Should().Be(sleepResponse.summary.stages.wake);
                sleep.NumberOfAwakenings.Should().Be(sleepResponse.sleep[0].awakeningsCount);
                sleep.TimeInBed.Should().Be(sleepResponse.summary.totalTimeInBed);
            }
        }
    }
}
