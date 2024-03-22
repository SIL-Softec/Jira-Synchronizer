using JiraSynchronizer.Core.Interfaces.Repositories;
using JiraSynchronizer.Core.Interfaces.Services;
using JiraSynchronizer.Core.Services;
using JiraSynchronizer.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Configuration;

[ExcludeFromCodeCoverage]
internal static class IocContainerConfiguration
{
    public static IServiceCollection AddIocContainerConfiguration(this IServiceCollection services)
    {
        // TODO
        // services.AddHttpContextAccessor();
        // services.AddHttpClient();
        // services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
        // ???

        // Core
        services.AddSingleton<IHashService, HashService>();

        // Infrastructure
        services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
        services.AddScoped<ILeistungsartRepository, LeistungsartRepository>();
        services.AddScoped<ILeistungserfassungRepository, LeistungserfassungRepository>();
        services.AddScoped<IMitarbeiterRepository, MitarbeiterRepository>();
        services.AddScoped<IProjektRepository, ProjektRepository>();
        services.AddScoped<IProjektMitarbeiterRepository, ProjektMitarbeiterRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWhitelistRepository, WhitelistRepository>();
        services.AddScoped<IZeitklasseRepository, ZeitklasseRepository>();

        return services;
    }
}
