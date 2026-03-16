using AppLogic;
using AppLogic.Services;
using DataAccess.Crud;
using DataAccess.Dao;
using DataAccess.Mappers;
using DTO;
using InfinityGrowth.DataAccess.Crud;
using InfinityGrowth_Proyecto2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

//configuration for JWT
var jwtSection = configuration.GetSection("JwtSettings");
var jwtKey = configuration["JwtSettings:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT signing key is not configured. Set JwtSettings__Key (environment variable) or provide JwtSettings:Key in configuration."
    );
}
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("TipoUsuario", "1"));
    options.AddPolicy("Asesor", policy => policy.RequireClaim("TipoUsuario", "2"));
    options.AddPolicy("Cliente", policy => policy.RequireClaim("TipoUsuario", "3"));
});


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelStateValidationFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// --- DataAccess (DI) ---
builder.Services.AddSingleton<ISqlDao, SqlDao>();
builder.Services.AddScoped<LoginCrud>();
builder.Services.AddScoped<RegistrarseCrud>();
builder.Services.AddScoped<RecuperarPasswordCrud>();
builder.Services.AddScoped<OTPCrud>();
builder.Services.AddScoped<UsuariosCrud>();
builder.Services.AddScoped<WalletCrud>();
builder.Services.AddScoped<ReporteClienteCrud>();
builder.Services.AddScoped<ReporteAsesorCrud>();
builder.Services.AddScoped<ReporteAdminCrud>();
builder.Services.AddScoped<RelacionAsesorClienteCrud>();
builder.Services.AddScoped<AsesoresCrud>();
builder.Services.AddScoped<InversionesActivasCrud>();
builder.Services.AddScoped<AjusteComisionesMapper>();
builder.Services.AddScoped<CatalogoComisionesCrud>();

// --- AppLogic services ---
builder.Services.AddScoped<IRegistrarseManager, RegistrarseManager>();
builder.Services.AddScoped<ILoginManager, LoginManager>();
builder.Services.AddScoped<IRecuperarPasswordManager, RecuperarPasswordManager>();
builder.Services.AddScoped<IEmailService>(provider => new Email_Service(configuration));
builder.Services.AddScoped<Encrypt_Service>();
builder.Services.AddScoped<OTP_Service>();
builder.Services.AddSingleton<Jwt_Service>();
builder.Services.AddScoped<StockManager>();
builder.Services.AddScoped<TwelveData_Service>();
builder.Services.AddScoped<IInversionesActivasManager, InversionesActivasManager>();
builder.Services.AddScoped<IPriceManager, PriceManager>();
builder.Services.AddScoped<PriceManager>();
builder.Services.AddScoped<IPortafoliosManager, PortafoliosManager>();
builder.Services.AddScoped<IReporteManager, ReporteManager>();
builder.Services.AddScoped<UsuariosManager>();
builder.Services.AddScoped<WalletManager>();
builder.Services.AddScoped<AsesoresManager>();
builder.Services.AddScoped<RelacionAsesorClienteManager>();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAjusteComisionesManager, AjusteComisionesManager>();


// --- PayPal Integration ---
builder.Services.Configure<PayPalOptions>(configuration.GetSection("PayPal"));

builder.Services.AddScoped<PayPalService>();
// --- Fin PayPal Integration ---


//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

