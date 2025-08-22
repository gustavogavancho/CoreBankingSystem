using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CoreBankingSystem.Application.Abstractions.Reports;
using CoreBankingSystem.Application.Reports.Services;

namespace CoreBankingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Report services
        services.AddSingleton<IReportTextFormatter, ReportTextFormatter>();
        services.AddSingleton<IPdfGenerator, SimplePdfGenerator>();

        return services;
    }
}
