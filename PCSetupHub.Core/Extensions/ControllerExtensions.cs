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
		public static async Task<ViewResult?> HandleInvalidModelStateAsync<TModel>(
			this Controller controller,
			TModel model,
			Func<Task>? additionalAction = null)
		{
			if (!controller.ModelState.IsValid)
			{
				controller.SetFirstError();

				if (additionalAction is not null)
					await additionalAction();

				return controller.View(model);
			}

			return null;
		}
		public static async Task<ViewResult?> HandleInvalidModelStateAsync(
			this Controller controller,
			Func<Task>? additionalAction = null)
		{
			if (!controller.ModelState.IsValid)
			{
				controller.SetFirstError();

				if (additionalAction is not null)
					await additionalAction();

				return controller.View();
			}

			return null;
		}
	}
}