// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.HandlerConfigurations
{
    public partial class HandlerConfigurationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfHandlerConfigurationIsNullAndLogItAsync()
        {
            // given
            HandlerConfiguration nullHandlerConfiguration = null;

            var nullHandlerConfigurationException =
                new NullHandlerConfigurationException(
                    message: "Handler configuration is null.");

            var expectedHandlerConfigurationValidationException =
                new HandlerConfigurationValidationException(
                    message: "Handler configuration validation error occurred, fix the errors and try again.",
                    innerException: nullHandlerConfigurationException);

            // when
            ValueTask<HandlerConfiguration> modifyHandlerConfigurationTask =
                this.handlerConfigurationService.ModifyHandlerConfigurationAsync(
                    nullHandlerConfiguration);

            HandlerConfigurationValidationException actualHandlerConfigurationValidationException =
                await Assert.ThrowsAsync<HandlerConfigurationValidationException>(
                    modifyHandlerConfigurationTask.AsTask);

            // then
            actualHandlerConfigurationValidationException.Should()
                .BeEquivalentTo(expectedHandlerConfigurationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHandlerConfigurationValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectHandlerConfigurationByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateHandlerConfigurationAsync(It.IsAny<HandlerConfiguration>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
