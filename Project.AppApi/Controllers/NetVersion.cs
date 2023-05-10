using IService;
using Microsoft.AspNetCore.Mvc;
using Model.Table;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// ����
    /// </summary>
    [ApiController]
    //[Route("[controller]")]
    public class NetVersion : BaseController
    {

        private readonly ILogger<NetVersion> _logger;

        //���ݿ�����
        private IRepository connection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connection"></param>
        public NetVersion(ILogger<NetVersion> logger, IRepository connection)  //, IRepository connection
        {
            _logger = logger;
            this.connection = connection;
        }


        /// <summary>
        /// ��ȡ��־
        /// </summary>
        /// <returns></returns>
        [Route("/api/WeatherForecast/get-log")]
        [HttpGet]
        public SystemLog Log()
        {
            //var connection = new Repository();
            var SystemLog = connection.QuerySet<SystemLog>().Get();
            _logger.LogInformation("INFO");
            _logger.LogError("ERROR");
            _logger.LogWarning("WARNING");
            return SystemLog;
        }
    }
}