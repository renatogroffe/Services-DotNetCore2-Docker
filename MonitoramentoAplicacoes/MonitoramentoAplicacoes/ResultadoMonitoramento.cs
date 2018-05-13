using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonitoramentoAplicacoes
{
    public class ResultadoMonitoramento
    {
        public ObjectId _id { get; set; }
        public string Horario { get; set; }
        public string Host { get; set; }
        public string Status { get; set; }
        [BsonIgnoreIfNull]
        public object Exception { get; set; }
    }
}