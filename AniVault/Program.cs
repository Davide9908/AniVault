
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();


var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/hello", () => "Hello world!");


app.Run();

