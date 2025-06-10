using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Web.Controllers.HardwareComponents
{
	public class RamController : Controller
	{
		public IActionResult Add()
		{
			return Ok($"Add from {GetType().Name}");
		}
		public IActionResult Select()
		{
			return Ok($"Select from {GetType().Name}");
		}
		public IActionResult Clear()
		{
			return Ok($"Clear from {GetType().Name}");
		}
		public IActionResult Edit()
		{
			return Ok($"Edit from {GetType().Name}");
		}
		public IActionResult Delete()
		{
			return Ok($"Delete from {GetType().Name}");
		}
	}
}