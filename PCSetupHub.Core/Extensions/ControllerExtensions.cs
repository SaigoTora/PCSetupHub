using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Core.Extensions
{
	public static class ControllerExtensions
	{
		public static void SetFirstError(this Controller controller,
			string viewDataIndex = "FirstError")
		{
			controller.ViewData[viewDataIndex] = controller.ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
		}
	}
}