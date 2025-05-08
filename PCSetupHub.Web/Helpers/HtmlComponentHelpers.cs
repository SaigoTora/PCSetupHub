using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

using PCSetupHub.Data.Models.Attributes;

namespace PCSetupHub.Web.Helpers
{
	public static class HtmlComponentHelpers
	{
		private const string NOT_AVAILABLE_TEXT = "N/A";

		#region Number rendering
		public static IHtmlContent RenderPriceInTable(this IHtmlHelper html, double? price)
		{
			string content = price.HasValue
				? $"<td class='text-money'><b>{ToRoundedString(price.Value)} $</b></td>"
				: $"<td class='text-muted'>{NOT_AVAILABLE_TEXT}</td>";

			return new HtmlString(content);
		}

		public static IHtmlContent RenderNumber(this IHtmlHelper html, int? number,
			string measurementUnit = "")
		{
			if (number.HasValue)
				return new HtmlString($"{number.Value} {measurementUnit}");
			else
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");
		}
		public static IHtmlContent RenderRoundedNumber(this IHtmlHelper html, double? number,
			string measurementUnit = "")
		{
			if (number.HasValue)
				return new HtmlString($"{ToRoundedString(number.Value)} {measurementUnit}");
			else
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");
		}
		public static IHtmlContent RenderRoundedNumber(this IHtmlHelper html, double number)
			=> new HtmlString(ToRoundedString(number));

		private static string ToRoundedString(double number)
			=> Math.Round(number, 2).ToString("0.##");
		#endregion

		public static IHtmlContent RenderText(this IHtmlHelper html, string? text)
		{
			if (!string.IsNullOrWhiteSpace(text))
				return new HtmlString(text);
			else
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");
		}
		public static IHtmlContent RenderBoolIndicator(this IHtmlHelper html, bool value)
		{
			if (value)
				return new HtmlString("<i class='fa fa-check text-success'></i>");
			else
				return new HtmlString("<i class='fa fa-xmark text-danger'></i>");
		}
		public static IHtmlContent RenderColorIndicator(this IHtmlHelper html, IEnumerable<Color?> colors)
		{
			StringBuilder contentBuilder = new();
			if (colors == null || colors.Count() == 0)
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");

			foreach (Color? color in colors)
			{
				if (color != null)
					contentBuilder.Append($"<div title='{color.Name}' class='color-container " +
						$"color-container-{color.Name.Replace(' ', '-').ToLower()}'></div>");
			}

			return new HtmlString(contentBuilder.ToString().TrimEnd(',', ' '));
		}
	}
}