using CoreBankingSystem.Application.Reports.Models;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;

namespace CoreBankingSystem.Application.Reports.Queries;

public record GetClientStatementReportFromParamsQuery(
    Guid? ClientId,
    Guid? Cliente,
    DateTime? Start,
    DateTime? End,
    string? RangoFechas
) : IRequest<ClientStatementReportDto>;

public class GetClientStatementReportFromParamsQueryHandler(IMediator mediator)
    : IRequestHandler<GetClientStatementReportFromParamsQuery, ClientStatementReportDto>
{
    public async Task<ClientStatementReportDto> Handle(GetClientStatementReportFromParamsQuery request, CancellationToken cancellationToken)
    {
        var effectiveClientId = request.ClientId ?? request.Cliente;
        if (effectiveClientId is null || effectiveClientId == Guid.Empty)
            throw new BadRequestException("Debe especificar el cliente (clientId o Cliente).");

        // Resolve date range
        DateTime? start = request.Start;
        DateTime? end = request.End;

        if (!string.IsNullOrWhiteSpace(request.RangoFechas))
        {
            var parts = request.RangoFechas.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && DateTime.TryParse(parts[0], out var s) && DateTime.TryParse(parts[1], out var e2))
            {
                start = s;
                end = e2;
            }
            else
            {
                throw new BadRequestException("Formato inválido para rangoFechas. Use 'yyyy-MM-dd,yyyy-MM-dd'.");
            }
        }

        if (start is null || end is null)
            throw new BadRequestException("Debe especificar el rango de fechas (start/end o rangoFechas).");

        if (start > end)
            throw new BadRequestException("La fecha de inicio no puede ser mayor a la fecha de fin.");

        // Delegate to the normalized query
        return await mediator.Send(new GetClientStatementReportQuery(effectiveClientId.Value, start.Value, end.Value), cancellationToken);
    }
}
