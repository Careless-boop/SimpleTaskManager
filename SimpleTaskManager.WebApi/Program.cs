using SimpleTaskManager.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddConfigurationServices(builder.Configuration);
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddTokenAuthentication(builder.Configuration);;
builder.Services.AddRepositoryServices();
builder.Services.AddAdditionalServices();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
