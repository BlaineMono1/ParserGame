using System.Reflection;
using Business.Data.Models;
using DataBaseToAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Models.GameModel;
using ParserService.Parsers;
using ParserService.Service;
using ParserService.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BaseDbContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("DataBaseConnection")));


builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);



//// Регистрация HttpClient через HttpClientFactory
//builder.Services.AddSingleton<HttpClient>(provider =>
//{
//    return HttpClientFactory.CreateClient();
//});

// Регистрация парсеров
builder.Services.AddSingleton<IParser<ConceptDto>, ConceptParser>();
builder.Services.AddSingleton<IParser<ConceptRetrieveResponse>, GameParser>();


// Регистрация сервиса для генерации URL
//builder.Services.AddSingleton<UrlGeneratorService>(provider =>
//    new UrlGeneratorService("https://store.playstation.com/en-tr/pages/browse"));

// Регистрация сервиса парсинга
builder.Services.AddSingleton<ParserAdapter>(provider =>
{
    var parsers = new Dictionary<string, dynamic>
        {
            { "concept", provider.GetService<IParser<ConceptDto>>() },
            { "cusacode", provider.GetService<IParser<ConceptRetrieveResponse>>() }
        };



    var urlGenerator = provider.GetService<UrlGeneratorService>();

    return new ParserAdapter(parsers);
});

builder.Services.AddSwaggerGen(config =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    config.IncludeXmlComments(xmlPath);
}
);

//To Do: Настроить Cors 
builder.Services.AddCors(options =>
options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();

}));


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

//ToDo: Напимать Middleware для обработки ошибок 
app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();


app.Run();
