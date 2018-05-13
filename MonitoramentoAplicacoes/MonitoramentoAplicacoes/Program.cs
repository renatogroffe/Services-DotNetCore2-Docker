using System;
using System.Threading;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MonitoramentoAplicacoes
{
    class Program
    {
        private static Timer _timer;
        private static ServiceConfigurations _configurations;
        private static AutoResetEvent waitHandle = new AutoResetEvent(false);

        public static void TimerElapsed(object state)
        {
            DisponibilidadeRepository repository =
                new DisponibilidadeRepository(_configurations);

            foreach (string host in _configurations.Hosts)
            {
                Console.WriteLine(String.Empty);
                Console.WriteLine(
                    $"Verificando a disponibilidade do host {host}");

                var resultado = new ResultadoMonitoramento();
                resultado.Horario =
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                resultado.Host = host;

                // Verifica a disponibilidade efetuando um ping
                // no host que foi configurado em appsettings.json
                try
                {
                    using (Ping p = new Ping())
                    {
                        var resposta = p.Send(host);
                        resultado.Status = resposta.Status.ToString();
                    }
                }
                catch (Exception ex)
                {
                    resultado.Status = "Exception";
                    resultado.Exception = ex;
                }

                repository.Incluir(resultado);

                Console.WriteLine(
                    JsonConvert.SerializeObject(resultado));
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Carregando configurações...");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            var configuration = builder.Build();

            _configurations = new ServiceConfigurations();
            new ConfigureFromConfigurationOptions<ServiceConfigurations>(
                configuration.GetSection("ServiceConfigurations"))
                    .Configure(_configurations);

            // Configura o timer para execução do ping e inicia
            // sua execução imediata
            _timer = new Timer(
                 callback: TimerElapsed,
                 state: null,
                 dueTime: 0,
                 period: _configurations.Intervalo);

            // Tratando o encerramento da aplicação com
            // Control + C ou Control + Break
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Saindo...");

                // Libera a continuação da thread principal
                waitHandle.Set();
            };

            // Aguarda que o evento CancelKeyPress ocorra
            waitHandle.WaitOne();
        }
    }
}