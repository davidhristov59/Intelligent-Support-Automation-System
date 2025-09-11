using Intelligent_Support_Automation_System.Repository.Implementation;
using Intelligent_Support_Automation_System.Repository.Interface;
using Intelligent_Support_Automation_System.Service.Implementation;
using Intelligent_Support_Automation_System.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args);

// Add Controllers support
builder.Services.AddControllers();

// This automatically loads appsettings.json and user secrets
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Register Services (via interfaces)
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<AzureOpenAIService>(); // no interface yet, keep as concrete

// Register Repository
builder.Services.AddScoped<ICosmosDbRepository, CosmosDbRepository>(); 

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

app.MapControllers();

app.Run();

