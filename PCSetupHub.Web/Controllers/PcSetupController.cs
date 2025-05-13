using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Web.Controllers
{
	public class PcSetupController : Controller
	{
		[HttpGet("PcSetup/{id?}")]
		public async Task<IActionResult> Index(int? id)
		{
			return Ok(id);
		}
	}
}