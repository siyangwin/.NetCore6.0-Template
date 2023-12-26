using Core;
using MvcCore.Extension.Swagger;
using MvcCore.Extension;
using IService;
using Service;
using MvcCore.Extension.Filter;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using Model.EnumModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MvcCore.Extension.Auth;
using System.Configuration;
using Autofac.Core;
using System.Net;
using IService.App;
using Service.App;
using Microsoft.Extensions.FileProviders;
using System.Security.Policy;
using Newtonsoft.Json;
using Microsoft.AspNetCore.DataProtection;

var ApiName = "Project.AppApi";

var builder = WebApplication.CreateBuilder(args);

//设置跨域
//設置Cors共享不攔截
builder.Services.AddCors(options =>
{
    //options.AddDefaultPolicy(builders =>
    //{
    //    builders
    //        .AllowAnyOrigin()  //所有来源
    //        .AllowAnyHeader()  //所有标头
    //        .AllowAnyMethod();  //所有 HTTP 方法
    //});

    options.AddPolicy("cors", builders =>
    {
        builders
            .AllowAnyOrigin()  //所有来源
            .AllowAnyHeader()  //所有标头
            .AllowAnyMethod();  //所有 HTTP 方法
            //.AllowCredentials();   //是一个跨域请求（CORS）选项，用于指示服务端是否允许客户端在跨域请求中包含凭据信息，如 Cookies、Authorization 标头等。
    });


    //options.AddPolicy("cors", builders =>
    //{
    //    builders
    //        //.WithOrigins(builder.Configuration.GetValue<string>("ConfigSettings:Orgins").Split(','))  //部分来源，如果需要按域名的跨域，设置这个
    //        .AllowAnyHeader()  //所有标头
    //        .AllowAnyMethod()  //所有 HTTP 方法
    //        .AllowCredentials();   //是一个跨域请求（CORS）选项，用于指示服务端是否允许客户端在跨域请求中包含凭据信息，如 Cookies、Authorization 标头等。
    //});
});


//获取连接字符串
GlobalConfig.ConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SqlServer");

// Add services to the container.
builder.Services.AddControllers();

//是否开启Swagger
var getconfig = builder.Configuration.GetValue<bool>("ConfigSettings:SwaggerEnable");
//Swagger
if (getconfig)
{
    builder.Services.AddSwaggerGens(ApiName, new string[] { "ViewModel.xml" });
}


// 将接口请求拦截器和错误拦截器 注册为全局过滤器
builder.Services.AddMvc(options =>
{
    //错误拦截器
    options.Filters.Add(typeof(ErrorFilterAttribute));
    //接口请求拦截器
    options.Filters.Add(typeof(ApiFilterAttribute));
    //授权验证拦截器
    options.Filters.Add(typeof(AuthValidator));
});

#region SerilLog配置

//SerilLog再Service中引用次NuGet包
//ThreadId需要引用专用的NuGet包
//const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}";
const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


var columnOpts = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        ////唯一编号
        //new SqlColumn{ColumnName = "Guid", PropertyName = "Guid", DataType = SqlDbType.NVarChar, DataLength = 32, AllowNull = false},
        ////請求客戶類型 APP CMS
        //new SqlColumn{ColumnName = "ClientType", DataType = SqlDbType.NVarChar, DataLength = 10, AllowNull = false},
        ////API名称
        //new SqlColumn{ColumnName = "APIName", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = false},
        ////请求方式 POST GET等
        //new SqlColumn{ColumnName = "Request", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = false},
        ////用户编号
        //new SqlColumn{ColumnName = "UserId", DataType = SqlDbType.Int, AllowNull = false},
        ////设备唯一编号,如果有，默认0
        //new SqlColumn{ColumnName = "DeviceId", DataType = SqlDbType.Int, AllowNull = true},
        ////操作说明
        //new SqlColumn{ColumnName = "Instructions", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = false},
        ////请求参数内容
        //new SqlColumn{ColumnName = "ReqParameter", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = false},
        ////返回参数内容
        //new SqlColumn{ColumnName = "ResParameter", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true},
        ////耗费时间
        //new SqlColumn{ColumnName = "Time", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
        ////访问用户IP
        //new SqlColumn{ColumnName = "IP", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
        // //服务器名称(负载均衡记录)
        //new SqlColumn{ColumnName = "Server", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = false}


         //唯一编号
        new SqlColumn{ColumnName = "Guid", PropertyName = "Guid", DataType = SqlDbType.NVarChar, DataLength = 32, AllowNull = true},
        //請求客戶類型 APP CMS
        new SqlColumn{ColumnName = "ClientType", DataType = SqlDbType.NVarChar, DataLength = 10, AllowNull = true},
        //API名称
        new SqlColumn{ColumnName = "APIName", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = true},
        //请求方式 POST GET等
        new SqlColumn{ColumnName = "Request", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
        //用户编号
        new SqlColumn{ColumnName = "UserId", DataType = SqlDbType.Int, AllowNull = true},
        //设备唯一编号,如果有，默认0
        new SqlColumn{ColumnName = "DeviceId", DataType = SqlDbType.Int, AllowNull = true},
        //操作说明
        new SqlColumn{ColumnName = "Instructions", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = true},
        //请求参数内容
        new SqlColumn{ColumnName = "ReqParameter", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true},
        //返回参数内容
        new SqlColumn{ColumnName = "ResParameter", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true},
        //耗费时间
        new SqlColumn{ColumnName = "Time", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
        //访问用户IP
        new SqlColumn{ColumnName = "IP", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
         //服务器名称(负载均衡记录)
        new SqlColumn{ColumnName = "Server", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = true}
    }
};


//columnOpts.Store.Remove(StandardColumn.Message);  //日志消息的文本内容，即人类可读的日志信息。
//columnOpts.Store.Remove(StandardColumn.Properties);//结构化日志中的属性集合。当你使用 Serilog 记录结构化日志时，这个列用于表示附加的结构化数据。
//columnOpts.Store.Remove(StandardColumn.MessageTemplate);//日志消息的模板，即日志消息的格式模板。这个模板可以包含占位符，用于渲染消息文本和结构化数据。

//columnOpts.Store.Add(StandardColumn.LogEvent);
//columnOpts.LogEvent.DataLength = 2048;
//columnOpts.PrimaryKey = columnOpts.TimeStamp;
columnOpts.TimeStamp.NonClusteredIndex = true; //设置为非聚类索引

//BatchPeriod
string interval = "00:00:05"; //表示5秒
TimeSpan ts;
TimeSpan.TryParse(interval, out ts);

//输出日志等级, 可以禁止输出 ASP.NET Core 应用程序启动时记录的，并且是通过默认的日志记录器输出的（Information）
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() //设置日志记录器的最小级别为 Debug，即只记录 Debug、Information、Warning、Error 和 Fatal 级别的日志事件。
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//对 Microsoft 命名空间下的所有日志事件进行重写，将最小级别设置为 Information，即只记录 Information、Warning、Error 和 Fatal 级别的日志事件。
    //.ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "PRODUCTION"}.json", optional: true).Build())
    .Enrich.FromLogContext() //启用日志上下文功能，自动获取当前线程和方法的一些信息，并添加到每个日志事件中。
    .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
    .WriteTo.File("logs/app.txt"
        , rollingInterval: RollingInterval.Day,
         rollOnFileSizeLimit: true, // 当日志文件大小超过指定大小时自动滚动日志文件
         fileSizeLimitBytes: 1048576, // 日志文件最大大小为 1MB
          retainedFileCountLimit: 7, // 最多保留 7 天的日志文件
          outputTemplate: OUTPUT_TEMPLATE)
    .AuditTo.MSSqlServer(
        connectionString: GlobalConfig.ConnectionString,
        sinkOptions: new MSSqlServerSinkOptions { TableName = "SystemLog", SchemaName = "dbo", AutoCreateSqlTable = true, BatchPeriod=ts,BatchPostingLimit = 50 },
        columnOptions: columnOpts)
    .CreateLogger();

#region SerilLog写入数据库Demo
//WriteTo生效 AuditTo不生效
//BatchPostingLimit: 用于设置批处理日志事件的数量限制。默认值为 50，即当累积了 50 条日志事件时就会将它们作为一个批次进行写入数据库。这个选项可以帮助优化性能，因为一次提交大量的日志事件比一次提交少量的日志事件效率更高。
//BatchPeriod: 用于设置批处理的时间间隔。默认值为 2 秒，即每隔 2 秒就会将所有已缓存的日志事件作为一个批次进行写入数据库。这个选项可以保证在一定的时间间隔内一定会向数据库提交日志事件，以保证数据的实时性和完整性。

//Log.Logger = new LoggerConfiguration()
//    .WriteTo
//    .MSSqlServer(
//        connectionString: GlobalConfig.ConnectionString,
//        sinkOptions: new MSSqlServerSinkOptions { TableName = "testnew" ,SchemaName="dbo",AutoCreateSqlTable=true,BatchPostingLimit=1},
//        columnOptions: columnOpts)
//    .CreateLogger();


//测试日志输出
//Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);
//Log.Warning("No coins remain at position {@Position}", new { Lat = 25, Long = 134 });
//Log.Error("{UserName}{UserId}{RequestUri}", 1, 2, 3);
#endregion

//注入 替换默认日志
builder.Host.UseSerilog(Log.Logger, dispose: true);
#endregion

builder.Services.AddControllersWithViews();



//GlobalConfig方法注入
//注入配置日志
//GlobalConfig.SystemLogService()

// 批量注册服务
//builder.Services.AddAutoFacs(new string[] { "Service.dll" });

//注入DB链接
builder.Services.AddScoped<IRepository, Repository>();
//注入Log
builder.Services.AddScoped<ISystemLogService, SystemLogService>();
//注入用户类
builder.Services.AddScoped<IAppUserService,AppUserService>();
//注入植物类
builder.Services.AddScoped<IPlantService, PlantService>();
//注册第三方类
builder.Services.AddScoped<IOtherSystemService, OtherSystemService>();
//注入httphelper
builder.Services.AddTransient(typeof(HttpHelper));

#region jwt验证

//注入jwt
builder.Services.AddScoped<GenerateJwt>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

//实例化对象
var jwtConfig = new JwtConfig();
//从Appsetting.config中读取出来赋值给对象
builder.Configuration.Bind("JwtConfig", jwtConfig);

builder.Services.AddAuthentication(option =>
    {
        //认证middleware配置
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        //指定Jwt的验证
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //Token颁发机构
            //指定 JWT 的颁发者（issuer），即表示 JWT 令牌的来源。
            ValidIssuer = jwtConfig.Issuer,
            //颁发给谁
            //指定 JWT 的受众（audience），即表示 JWT 令牌应该被哪些客户端使用。
            ValidAudience = jwtConfig.Audience,
            //这里的key要进行加密
            //指定用于对 JWT 签名进行验证的密钥。在这里使用 SymmetricSecurityKey 类型来指定密钥，其可以是任何字节数组，例如使用 Encoding.UTF8.GetBytes() 方法将字符串转换为字节数组。
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
            //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
            //指示是否验证 JWT 令牌的有效期。当服务器收到一个 JWT 令牌时，会检查其 Claims 中的 NotBefore 和 Expires 是否在当前时间范围内。如果 ValidateLifetime 设置为 true，则服务器将拒绝过期的 JWT 令牌，并认为它是无效的。
            ValidateLifetime = true,
            //指示是否验证 JWT 的签名密钥。
            ValidateIssuerSigningKey = true,
            //指示是否验证 JWT 的颁发者。
            ValidateIssuer =true,
            //指示是否验证 JWT 的受众。
            ValidateAudience =true,
        };

        //指定Jwt的返回内容
        options.Events = new JwtBearerEvents
        {
            //此处为权限验证失败后触发的事件
            OnChallenge = context =>
            {
                //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                context.HandleResponse();

                //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                var payload = JsonConvert.SerializeObject(new { api_version = "v1", success = false, code = "401", message = "很抱歉，您无权访问,请授权!" });
                //var payload = "no";
                //自定义返回的数据类型
                context.Response.ContentType = "application/json";
                //自定义返回状态码，默认为401 我这里改成 200
                context.Response.StatusCode = StatusCodes.Status200OK;
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //输出Json数据结果
                context.Response.WriteAsync(payload);
                return Task.FromResult(0);
            }
        };
    });

#endregion

//判断文件夹是否存在,不存在则创建
string folderPath = Path.Combine(Directory.GetCurrentDirectory(), @"other");
if (!Directory.Exists(folderPath))
{
    Directory.CreateDirectory(folderPath);
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(folderPath, "keys")))
    .ProtectKeysWithDpapi();// 以 DPAPI 保护密钥，可替换为其他适合的保护机制,只适配windows，如果Linux 或 macOS 等非 Windows 环境下，使用.ProtectKeysWithCertificate("thumbprint");

var app = builder.Build();

// Configure the HTTP request pipeline. 會把 http 轉到 https。
//使用http,可以禁止使用
//app.UseHttpsRedirection();

//跨域第一種版本，請要ConfigureService中配置服務 services.AddCors();
//app.UseCors();
app.UseCors("cors");

//是否开启Swagger
if (getconfig)
{
    app.UseSwaggers(ApiName);
}

app.UseStaticFiles(new StaticFileOptions()
{
    //指定要公开的静态文件所在的目录路径。在此示例中，它将当前工作目录下的“Files”子目录指定为静态文件的根目录。
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"other")),

    //指定可用于访问静态文件的 URL 路径。在此示例中，它将 "/Files" 路径映射到 "Files" 目录。
    RequestPath = new PathString("/other"),

    //一个布尔值，指定是否应该在请求的文件类型未知时允许服务静态文件。默认情况下，这个属性设置为 false，表示只有已知 MIME 类型的文件才会被服务。如果设置为 true，则未知文件类型也可以被服务。
    ServeUnknownFileTypes = true,

    //用于指定特定文件扩展名的 MIME 类型的字典。如果请求的文件扩展名与此列表不匹配，则服务器返回一个 404 响应。在此示例中，注释掉了这个属性，因此所有响应的文件类型将根据其文件扩展名自动识别。
    //ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>
    //{
    //     { ".apk","application/vnd.android.package-archive"}
    //})

    //OnPrepareResponse = ctx =>
    //{
    //    // 检查文件夹是否存在，如果不存在则创建
    //    string filePath = Path.Combine(ctx.Context.Request.Path.Value, ctx.File.Name);
    //    string folderPath = Path.GetDirectoryName(filePath);
    //    if (!Directory.Exists(folderPath))
    //    {
    //        Directory.CreateDirectory(folderPath);
    //    }
    //}
});


app.Use(async (context, next) =>
{
    //表示此API是什么端
    context.Request.Headers.Add("ClientType", "APP");

    //注入Guid每次请求唯一编码
    context.Request.Headers.Add("Guid", Guid.NewGuid().ToString("N"));

    //获取默认语言
    string language = context.QueryOrHeaders("language");
    if (string.IsNullOrEmpty(language))
    { 
        language = ((int)LanguageEnum.CN).ToString(); 
    }
    context.SetHeaders("Language", language);

    //Token
    //context.SetHeaders("Token", context.QueryOrHeaders("Token"));
    string Token = context.QueryOrHeaders("Authorization");
    context.SetHeaders("Token", Token);
    
    ////街市id
    //string marketId = context.QueryOrHeaders("marketId");
    //if (string.IsNullOrEmpty(marketId))
    //{
    //    //默认MarkertId
    //    marketId = "1";
    //}
    //context.SetHeaders("MarketId", marketId);

    await next();
});

app.UseRouting();


//注意顺序，先认证后授权，不然接口加入Token认证也不会通过
app.UseAuthentication();//启动认证   

app.UseAuthorization();//启动授权

//添加 JWT 异常处理中间件
//app.UseMiddleware<AuthValidator>();

app.MapControllers();

app.Run();