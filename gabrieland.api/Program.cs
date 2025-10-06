using gabrieland.Data;
using Oracle.ManagedDataAccess.Client;
using gabrieland.api.Controllers;
using gabrieland.api.Data;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// "Cross-Origin Resource Sharing" es una vara del navegador que bloquea solicitudes cuando el origen es blablabla
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add fileManagerServi
builder.Services.AddSingleton<FileStorageService>();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<DataBaseConnector>();
builder.Services.AddScoped<ServicioReservaData>();
builder.Services.AddScoped<ReservasData>();
builder.Services.AddScoped<TiposPagoData>();
builder.Services.AddScoped<FacturaData>();

OracleConfiguration.TnsAdmin = Path.Combine(Directory.GetCurrentDirectory(), "Wallet");
OracleConfiguration.WalletLocation = OracleConfiguration.TnsAdmin;

builder.Services.AddScoped(_ => new OracleConnection(builder.Configuration.GetConnectionString("OracleConnection")));

builder.Services.AddScoped<EmailService>();
var app = builder.Build();
var stripeSettings = builder.Configuration.GetSection("Stripe");
StripeConfiguration.ApiKey = stripeSettings["SecretKey"];


app.UseStaticFiles();
//this just for debuggin purpuses
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
// Configurar el uso de CORS
app.UseCors("AllowAll");

//Quité lo de https aunq eso no tenia nada que ver al parecer
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
