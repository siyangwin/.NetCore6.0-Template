using Core;
using MvcCore.Extension.AutoFac;
using MvcCore.Extension.Swagger;
using IService;
using Service;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcCore.Extension.Filter;

var ApiName = "Project.AppApi";

var builder = WebApplication.CreateBuilder(args);

//��ȡ�����ַ���
GlobalConfig.ConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SqlServer");

// Add services to the container.

builder.Services.AddControllers();

//�Ƿ���Swagger
var getconfig = builder.Configuration.GetValue<bool>("ConfigSettings:SwaggerEnable");
//Swagger
if (getconfig)
{
    builder.Services.AddSwaggerGens(ApiName, new string[] { "ViewModel.xml" });
}


//ע��DB����
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ISystemLogService, SystemLogService>();
//builder.Services.AddScoped<ILogService, SystemLogService>();

// �� MyActionFilter ע��Ϊȫ�ֹ�����
builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ApiFilterAttribute));
});

//GlobalConfig����ע��
//ע��������־
//GlobalConfig.SystemLogService()

// ����ע�����
//builder.Services.AddAutoFacs(new string[] { "Service.dll" });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//�Ƿ���Swagger
if (getconfig)
{
    app.UseSwaggers(ApiName);
}

app.UseAuthorization();

app.MapControllers();

app.Run();
