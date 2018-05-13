using MongoDB.Driver;

namespace MonitoramentoAplicacoes
{
    public class DisponibilidadeRepository
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private IMongoCollection<ResultadoMonitoramento> _disponibilidade;

        public DisponibilidadeRepository(
            ServiceConfigurations configurations)
        {
            _client = new MongoClient(
                configurations.LogDatabase);
            _db = _client.GetDatabase("DBMonitoramento");
            _disponibilidade =
                _db.GetCollection<ResultadoMonitoramento>("Disponibilidade");
        }

        public void Incluir(
            ResultadoMonitoramento monitoramento)
        {
            _disponibilidade.InsertOne(monitoramento);
        }
    }
}