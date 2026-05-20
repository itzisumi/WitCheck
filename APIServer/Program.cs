using APIServer.Hub;
using APIServer.Manager;
using Database;
using Database.DbContext;
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin =>
            {
                var uri = new Uri(origin);
                return uri.Port == 5049;
            });
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Connection string from appsettings.json / environment variable
var connectionString = builder.Configuration.GetConnectionString("WitCheck")
    ?? throw new InvalidOperationException("Connection string 'WitCheck' not found.");
builder.Services.AddDbContext<WitCheckContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<UserManager>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<AnswerRepository>();
builder.Services.AddTransient<PasswordRepository>();
builder.Services.AddTransient<QuestionRepository>();
builder.Services.AddTransient<QuizRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<HostHub>("/HostHub");
    endpoints.MapHub<PlayerHub>("/PlayerHub");
});

app.UseSwagger();
app.UseSwaggerUI();

GeneralManager.StartCleanTimer();

app.Run();
