using Core;
using MvcCore.Extension.AutoFac;
using MvcCore.Extension.Swagger;
using IService;
using Service;
using Autofac.Core;

var ApiName = "Project.AppApi";

var builder = WebApplication.CreateBuilder(args);

//��ȡ�����ַ���
GlobalConfig.ConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SqlServer");

// Add services to the container.

builder.Services.AddControllers();

//��ȡappsettings.json��ֵ �Ƿ���Swagger
//var getconfig = builder.Configuration["ConfigSettings:SwaggerEnable"];
var getconfig = builder.Configuration.GetValue<bool>("ConfigSettings:SwaggerEnable");
//Swagger
if (getconfig)
{
    builder.Services.AddSwaggerGens(ApiName, new string[] { "ViewModel.xml" });
}

//ע��DB����
builder.Services.AddScoped<IRepository, Repository>();

// ����ע�����
builder.Services.AddAutoFacs(new string[] { "Service.dll" });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//Swagger
if (getconfig)
{
    app.UseSwaggers(ApiName);
}

app.UseAuthorization();

app.MapControllers();

app.Run();
