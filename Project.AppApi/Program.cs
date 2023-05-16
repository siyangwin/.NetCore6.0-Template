using Core;
using MvcCore.Extension.Swagger;
using IService;
using Service;
using MvcCore.Extension.Filter;
using Serilog;
using Azure.Core;
using Serilog.Events;
using Model.Table;
using Serilog.Sinks.MSSqlServer;
using static Serilog.Sinks.MSSqlServer.ColumnOptions;
using System.Collections.ObjectModel;
using System.Data;

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


// ���ӿ������������ʹ��������� ע��Ϊȫ�ֹ�����
builder.Services.AddMvc(options =>
{
    //�ӿ�����������
    options.Filters.Add(typeof(ApiFilterAttribute));
    //����������
    options.Filters.Add(typeof(ErrorFilterAttribute));
});


var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn
            {ColumnName = "EnvironmentUserName", PropertyName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 64},

        new SqlColumn
            {ColumnName = "UserId", DataType = SqlDbType.BigInt, NonClusteredIndex = true},

        new SqlColumn
            {ColumnName = "RequestUri", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = false},
    }
};


//SerilLog  ��Service�����ô�NuGet��
//ThreadId��Ҫ����ר�õ�NuGet��
//const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}";
const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


//�����־�ȼ�,���Խ�ֹ��� ASP.NET Core Ӧ�ó�������ʱ��¼�ģ�������ͨ��Ĭ�ϵ���־��¼������ģ�Information��
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() //������־��¼������С����Ϊ Debug����ֻ��¼ Debug��Information��Warning��Error �� Fatal �������־�¼���
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//�� Microsoft �����ռ��µ�������־�¼�������д������С��������Ϊ Information����ֻ��¼ Information��Warning��Error �� Fatal �������־�¼���
    //.ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "PRODUCTION"}.json", optional: true).Build())
    .Enrich.FromLogContext() //������־�����Ĺ��ܣ��Զ���ȡ��ǰ�̺߳ͷ�����һЩ��Ϣ������ӵ�ÿ����־�¼��С�
    .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
    //.WriteTo.File("logs/app.txt"
    //    , rollingInterval: RollingInterval.Day,
    //     rollOnFileSizeLimit: true, // ����־�ļ���С����ָ����Сʱ�Զ�������־�ļ�
    //     fileSizeLimitBytes: 1048576, // ��־�ļ�����СΪ 1MB
    //      retainedFileCountLimit: 7, // ��ౣ�� 7 �����־�ļ�
    //      outputTemplate: OUTPUT_TEMPLATE)
    .WriteTo.MSSqlServer("server=disk.risinguptech.com,36832;database=Project;user=risingup_admin;password=risingup2023;max pool size=300", "dbo.[test]", columnOptions: columnOptions,  autoCreateSqlTable: true) //restrictedToMinimumLevel: LogEventLevel.Information,
    .CreateLogger();

//������־���
Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);
Log.Warning("No coins remain at position {@Position}", new { Lat = 25, Long = 134 });
Log.Error("{EnvironmentUserName}{UserId}{RequestUri}",1,2,3);
Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);


//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//    .ReadFrom.Configuration(new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json")
//    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
//    optional: true).Build())
//    .Enrich.FromLogContext()
//    .WriteTo.Async(c => c.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/log/", "log")
//    , rollingInterval: RollingInterval.Day)).WriteTo.Async(c => c.Console())
//    .CreateLogger();




///д�����ݿ���Ҫ����ר�õ�NuGet��
//string connectionString = "Data Source=localhost;Initial Catalog=Logs;Integrated Security=True";

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.MSSqlServer(
//        connectionString,
//        sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents" },
//        columnOptions: new ColumnOptions
//        {
//            AdditionalDataColumns = new Collection<DataColumn>
//            {
//    new DataColumn { DataType = typeof(string), ColumnName = "Application" }
//            }
//        })
//    .CreateLogger();

//д��Log����
//Log.Information("Hello, Serilog!");
//Log.Error("err");
///Log.CloseAndFlush();

//ע�� �滻Ĭ����־
builder.Host.UseSerilog(Log.Logger, dispose: true);

builder.Services.AddControllersWithViews();


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
