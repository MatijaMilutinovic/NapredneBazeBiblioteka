using StackExchange.Redis;

namespace RedisNeo2.Models.Redis
{
    public class Producer : BackgroundService
    {
        private readonly IConnectionMultiplexer _cmux;
        private readonly ILogger<Producer> _logger;
        private readonly string kanal = "Kalan";
        public Producer(IConnectionMultiplexer cmux, ILogger<Producer> logger) {
            _cmux = cmux;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subsciber = _cmux.GetSubscriber();

            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("Proces running");

                await subsciber.PublishAsync(kanal, "Tets");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
