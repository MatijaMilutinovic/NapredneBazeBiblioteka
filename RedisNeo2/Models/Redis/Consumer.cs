using StackExchange.Redis;

namespace RedisNeo2.Models.Redis
{
    public class Consumer : BackgroundService
    {
        private readonly IConnectionMultiplexer _cmux;
        private readonly ILogger<Consumer> _logger;
        private readonly string kanal = "Kalan";
        public Consumer(IConnectionMultiplexer cmux, ILogger<Consumer> logger)
        {
            _cmux = cmux;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subsciber = _cmux.GetSubscriber();

            while (!stoppingToken.IsCancellationRequested)
            {
                

                await subsciber.SubscribeAsync(kanal, (x, poruka) => {
                    _logger.LogInformation("Proces running" + x + " " + poruka);
                });
                //await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
