// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Processings.Traffics.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthTrafficClientV2Tests
    {
        private readonly Mock<ITrafficV2ProcessingService> trafficV2ProcessingServiceMock;
        private readonly IHealthTrafficClientV2 healthTrafficClientV2;

        public HealthTrafficClientV2Tests()
        {
            this.trafficV2ProcessingServiceMock =
                new Mock<ITrafficV2ProcessingService>();

            this.healthTrafficClientV2 =
                new HealthTrafficClientV2(
                    trafficV2ProcessingService:
                        this.trafficV2ProcessingServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static TrafficPeriodV2 GetRandomTrafficPeriodV2() =>
            TrafficPeriodV2.Day;

        private static TrafficSnapshotV2 CreateRandomTrafficSnapshotV2() =>
            new TrafficSnapshotV2
            {
                Period = GetRandomTrafficPeriodV2(),
                WindowStart = GetRandomDateTimeOffset(),
                WindowEnd = GetRandomDateTimeOffset(),
                WindowLabel = GetRandomString(),
                TotalEvents = new IntRange(min: 2, max: 9).GetValue(),
                TotalListenerEvents = new IntRange(min: 2, max: 9).GetValue(),
                Buckets = new List<TrafficBucketV2>()
            };
    }
}
