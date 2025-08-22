using CoreBankingSystem.Application.Reports.Models;
using CoreBankingSystem.Application.Common.Exceptions;
using MediatR;
using CoreBankingSystem.Application.Clients.Queries;

namespace CoreBankingSystem.Application.Reports.Queries;

public record GetClientStatementReportFromParamsQuery(
    Guid? ClientId,
    string? Identification,
    string? Cliente, // alias to support ?Cliente=ID-1001
    DateTime? Start,
    DateTime? End,
    string? RangoFechas
) : IRequest<ClientStatementReportDto>;

public class GetClientStatementReportFromParamsQueryHandler(IMediator mediator)
    : IRequestHandler<GetClientStatementReportFromParamsQuery, ClientStatementReportDto>
{
    public async Task<ClientStatementReportDto> Handle(GetClientStatementReportFromParamsQuery request, CancellationToken cancellationToken)
    {
        // Prefer identification (either in Identification or Cliente alias), otherwise allow ClientId for backward compatibility
        Guid? effectiveClientId = null;

        var identification = !string.IsNullOrWhiteSpace(request.Identification)
            ? request.Identification
            : (!string.IsNullOrWhiteSpace(request.Cliente) ? request.Cliente : null);

        if (!string.IsNullOrWhiteSpace(identification))
        {
            var client = await mediator.Send(new GetClientByIdentificationQuery(identification!), cancellationToken);
            if (client is null)
                throw new BadRequestException($"No se encontró cliente con identificación '{identification}'.");
            effectiveClientId = client.ClientId;
        }
        else if (request.ClientId is not null && request.ClientId != Guid.Empty)
        {
            effectiveClientId = request.ClientId;
        }

        if (effectiveClientId is null)
            throw new BadRequestException("Debe especificar la identificación del cliente (identification o Cliente) o un clientId.");

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
