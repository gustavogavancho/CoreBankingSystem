using CoreBankingSystem.Application.Common.Exceptions;
using CoreBankingSystem.Application.Reports.Queries;
using MediatR;
using Moq;

namespace CoreBankingSystem.Application.Tests.Reports;

public class GetClientStatementReportFromParamsQueryTests
{
    [Fact]
    public async Task Handle_ParsesRangoFechas_DelegatesToNormalizedQuery()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var clientId = Guid.NewGuid();
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 1, 31);

        mediator
            .Setup(m => m.Send(It.Is<GetClientStatementReportQuery>(q => q.ClientId == clientId && q.StartDate == start && q.EndDate == end), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoreBankingSystem.Application.Reports.Models.ClientStatementReportDto());

        var handler = new GetClientStatementReportFromParamsQueryHandler(mediator.Object);

        // Act
        var result = await handler.Handle(new GetClientStatementReportFromParamsQuery(
            ClientId: clientId,
            Identification: null,
            Cliente: null,
            Start: null,
            End: null,
            RangoFechas: "2025-01-01,2025-01-31"
        ), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        mediator.VerifyAll();
    }

    [Fact]
    public async Task Handle_InvalidDates_ThrowsBadRequest()
    {
        var mediator = new Mock<IMediator>();
        var handler = new GetClientStatementReportFromParamsQueryHandler(mediator.Object);

        await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await handler.Handle(new GetClientStatementReportFromParamsQuery(
                ClientId: Guid.NewGuid(),
                Identification: null,
                Cliente: null,
                Start: new DateTime(2025, 1, 31),
                End: new DateTime(2025, 1, 1),
                RangoFechas: null
            ), CancellationToken.None);
        });
    }
}
