using Microsoft.AspNetCore.Mvc;

namespace PCSetupHub.Core.Extensions
{
	public static class ControllerExtensions
	{
		/// <summary>
		/// Sets the first validation error message from ModelState
		/// into the controller's ViewData dictionary under the specified key.
		/// </summary>
		/// <param name="controller">The controller instance.</param>
		/// <param name="viewDataIndex">The key for ViewData, default is "FirstError".</param>
		public static void SetFirstError(this Controller controller,
			string viewDataIndex = "FirstError")
		{
			controller.ViewData[viewDataIndex] = controller.ModelState.Values
				.SelectMany(v => v.Errors)
				.Select(e => e.ErrorMessage)
				.FirstOrDefault();
		}

		/// <summary>
		/// Checks if the model state is invalid, sets the first error message,
		/// optionally executes an additional asynchronous action, and returns
		/// a view result with the provided model.
		/// Returns null if the model state is valid.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <param name="controller">The controller instance.</param>
		/// <param name="model">The model to return with the view.</param>
		/// <param name="additionalAction">An optional asynchronous action to perform before returning the view.</param>
		/// <returns>A view result with the model if invalid, otherwise null.</returns>
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

		/// <summary>
		/// Checks if the model state is invalid, sets the first error message,
		/// optionally executes an additional asynchronous action, and returns
		/// a view result without a model.
		/// Returns null if the model state is valid.
		/// </summary>
		/// <param name="controller">The controller instance.</param>
		/// <param name="additionalAction">An optional asynchronous action to perform before returning the view.</param>
		/// <returns>A view result if invalid, otherwise null.</returns>
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