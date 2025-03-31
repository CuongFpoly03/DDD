using DomainDrivenDesign.Domain.Repositories;
using DomainDrivenDesign.Infrastructure.Data;
using DomainDrivenDesign.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//đọc tệp ENV
Env.Load();
// Lấy chuỗi kết nối và secret key từ biến môi trường
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
var secretKey = Environment.GetEnvironmentVariable("SECREC_KEY");
var jwtacesskey = Environment.GetEnvironmentVariable("JWT_ACCESS_KEY");

//add services to the container
//connect database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
//add scoped
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IStudentRepositoty, StudentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddControllers().AddNewtonsoftJson();

// vvvvvvvvv
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints?.MapControllers();
}); 

app.Run();
