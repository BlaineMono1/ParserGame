using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ParserService.Interfaces;
using ParserService.Models;
using ParserService.Parsers;
using ParserService.Service;
using static ParserService.Models.GameModel.ModelAddon;
using static ParserService.Models.GameModel.ModelGame;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
            .Json
            .ReferenceLoopHandling
            .Ignore
    );

// ����������� ��������
builder.Services.AddSingleton<IParser<ConceptDto>, ConceptParser>();
builder.Services.AddSingleton<IParser<DataGame>, GameParser>();
builder.Services.AddSingleton<IParser<DataAddon>, AddonParser>();

// ����������� ������� ��������
builder.Services.AddSingleton<ParserAdapter>(provider =>
{
    var parsers = new Dictionary<string, dynamic>
    {
        { "concept", provider.GetService<IParser<ConceptDto>>() },
        { "cusacode", provider.GetService<IParser<DataGame>>() },
        { "addon", provider.GetService<IParser<DataAddon>>() },
    };

    var urlGenerator = provider.GetService<UrlGeneratorService>();

    return new ParserAdapter(parsers);
});

builder.Services.AddSwaggerGen(config =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    config.IncludeXmlComments(xmlPath);
});

//To Do: ��������� Cors
builder.Services.AddCors(options =>
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        }
    )
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

//ToDo: �������� Middleware ��� ��������� ������
app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
