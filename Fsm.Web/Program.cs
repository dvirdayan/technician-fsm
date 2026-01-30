using FSM.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
// This tells the app to look for [ApiController] classes (like yours)
builder.Services.AddControllers();

// 2. Add Swagger/OpenAPI support
// This generates the documentation page you are trying to see
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // This enables the nice blue/white webpage
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 4. Map the controllers so they can respond to requests
app.MapControllers();

app.Run();