using FutbolDataService;
using Polly;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        FutbolWorkerOptions futbolWorkerOptions = configuration.GetSection("FutbolAPI").Get<FutbolWorkerOptions>();

        services.AddHttpClient("FootbalAPI", client =>
        {
            client.BaseAddress = new Uri(futbolWorkerOptions.FutbolApiBaseAddress);
            client.DefaultRequestHeaders.Add(futbolWorkerOptions.FutbolTokenName, futbolWorkerOptions.FutbolApiToken);
        })
        .AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)));

        services.AddSingleton(configuration);
        services.AddHostedService<FutbolWorker>();
    })
    .Build();

await host.RunAsync();
