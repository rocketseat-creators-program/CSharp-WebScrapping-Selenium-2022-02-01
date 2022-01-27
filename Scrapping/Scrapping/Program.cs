using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Scrapping;
using Scrapping.Models;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json");

var config = builder.Build();
var seleniumConfig = new SeleniumConfig();
new ConfigureFromConfigurationOptions<SeleniumConfig>(
    config.GetSection("SeleniumConfigurations"))
        .Configure(seleniumConfig);

Console.WriteLine("Programa de Web Scrapping iniciado!");

// Chamar classe de scrapping
var page = new WebServerExport(seleniumConfig);
page.InitExport();
page.Dispose();
Console.WriteLine("Pressione enter para encerrar...");
Console.ReadLine();