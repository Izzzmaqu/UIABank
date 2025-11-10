using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Services
{
    public class TransferecenciaProgramada : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransferecenciaProgramada> _logger;


        public TransferecenciaProgramada(
            IServiceProvider serviceProvider,
            ILogger<TransferecenciaProgramada> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var servicio = scope.ServiceProvider
                        .GetRequiredService<ITransferenciaProgramadaBW>();

                    await servicio.EjecutarPendientesAsync();

                    _logger.LogInformation("Job de transferencias programadas ejecutado a: {time}",
                        DateTime.Now);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // cada 5 minutos
            }
        }
    }
}
