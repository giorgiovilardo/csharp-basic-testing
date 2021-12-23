using MyApp.Core;

var builder = WebApplication.CreateBuilder(args);

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

app.MapControllers();

app.MapPost("add", (TwoNumbersRequestDto dto) =>
{
    var calc = new Calculator();
    var (firstNumber, secondNumber) = dto;
    var result = calc.Add(firstNumber, secondNumber);
    return new CalculatorResponseDto(result);
});

app.MapPost("subtract", (TwoNumbersRequestDto dto) =>
{
    var calc = new Calculator();
    var (firstNumber, secondNumber) = dto;
    var result = calc.Subtract(firstNumber, secondNumber);
    return new CalculatorResponseDto(result);
});

app.MapPost("multiply", (TwoNumbersRequestDto dto) =>
{
    var calc = new Calculator();
    var (firstNumber, secondNumber) = dto;
    var result = calc.Multiply(firstNumber, secondNumber);
    return new CalculatorResponseDto(result);
});

app.Run();

public record CalculatorResponseDto(int Result);

public record TwoNumbersRequestDto(int FirstNumber, int SecondNumber);

public partial class Program
{
}
