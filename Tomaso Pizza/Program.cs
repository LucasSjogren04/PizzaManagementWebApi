using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Tomaso_Pizza.Data;
var builder = WebApplication.CreateBuilder(args);

var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId");
var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
var keyVaultDirectoryID = builder.Configuration.GetSection("KeyVault:DirectoryID");

builder.Configuration.AddAzureKeyVault(keyVaultURL.Value!.ToString(),
keyVaultClientId.Value!.ToString(),
keyVaultClientSecret.Value!.ToString(),

new DefaultKeyVaultSecretManager());
var credential = new ClientSecretCredential(keyVaultDirectoryID.Value!.ToString(),
keyVaultClientId.Value!.ToString(), keyVaultClientSecret.Value!.ToString());
var client = new SecretClient(new Uri(keyVaultURL.Value!.ToString()), credential);
var connString = client.GetSecret("PizzaConn").Value.Value.ToString();

builder.Services.AddDbContext<PizzaContext>(options =>
            options.UseSqlServer(connString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 1;
}).AddEntityFrameworkStores<PizzaContext>()
.AddDefaultTokenProviders();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
