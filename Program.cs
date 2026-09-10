using AuthenticationDemo.Data;
using AuthenticationDemo.Entities;
using AuthenticationDemo.Interfaces;
using AuthenticationDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddRateLimiter(options =>
{
	// Return 429 Too Many Requests when limit is exceeded
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	// Define policies 
	options.AddFixedWindowLimiter("fixed", fixedOptions =>
	{
		fixedOptions.PermitLimit = 5;                        // Max 5 requests
		fixedOptions.Window = TimeSpan.FromSeconds(10);      // Per 10-second window
		fixedOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		fixedOptions.QueueLimit = 0;                         // No queuing (reject immediately)
	});
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService , AuthService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<RefreshToeknServices>(); 
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	var publicKeyPath = Path.Combine(
	   AppContext.BaseDirectory,
	   "keys",
	   "public.pem");

	if (!File.Exists(publicKeyPath))
	{
		throw new FileNotFoundException(
			"JWT public key was not found.",
			publicKeyPath);
	}

	var publicKey = File.ReadAllText(publicKeyPath);

	var rsa = RSA.Create();               
	rsa.ImportFromPem(publicKey);

	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,

		ValidIssuer = jwtSettings["Issuer"],
		ValidAudience = jwtSettings["Audience"],

		IssuerSigningKey = new RsaSecurityKey(rsa),

		ClockSkew = TimeSpan.FromSeconds(5),
		RoleClaimType = ClaimTypes.Role,
		NameClaimType = ClaimTypes.Name
	};
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options.Title = "My-Scalar API";
		options.DarkMode = true;
		options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
		options.CustomCss = "";
		options.AddPreferredSecuritySchemes("Bearer")
			.AddHttpAuthentication("Bearer", auth =>
			{
				auth.Token = "sobhy.xxt.ghorab";

			}).EnablePersistentAuthentication(); 
	});
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
