﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project.AppApi.Controllers
{
	/// <summary>
	/// 自动生成模板
	/// </summary>
	[ApiExplorerSettings(IgnoreApi = false)]
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : BaseController
	{
		// GET api/values
		[AllowAnonymous]
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			Response.Redirect("/index.html");
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
