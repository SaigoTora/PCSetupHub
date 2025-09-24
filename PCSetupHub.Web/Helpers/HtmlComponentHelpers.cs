using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Text;

using PCSetupHub.Data.Models.Attributes;

namespace PCSetupHub.Web.Helpers
{
	public enum TextAlignment : byte
	{
		Left,
		Center,
		Right
	}

	public static class HtmlComponentHelpers
	{
		private const string NOT_AVAILABLE_TEXT = "N/A";

		private static Dictionary<TextAlignment, string> alignmentMap = new()
		{
			[TextAlignment.Left] = "text-start",
			[TextAlignment.Center] = "text-center",
			[TextAlignment.Right] = "text-end"
		};

		#region Number rendering
		/// <summary>
		/// Renders a price value inside a styled table cell.
		/// Shows price with dollar sign or "N/A" if no value.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="price">Nullable price value.</param>
		/// <param name="alignment">Text alignment inside the cell.</param>
		/// <param name="colspanValue">Number of columns the cell should span.</param>
		/// <returns>HTML content for the price cell.</returns>
		public static IHtmlContent RenderPriceInTable(this IHtmlHelper html, double? price,
			TextAlignment alignment = TextAlignment.Center, int colspanValue = 1)
		{
			string alignmentClass = alignmentMap[alignment];
			string content = price.HasValue
				? $"<td colspan='{colspanValue}' class='text-money {alignmentClass}'>" +
					$"<b>{ToRoundedString(price.Value)} $</b></td>"
				: $"<td colspan='{colspanValue}' class='text-muted {alignmentClass}'>" +
					$"{NOT_AVAILABLE_TEXT}</td>";

			return new HtmlString(content);
		}

		/// <summary>
		/// Renders an integer number with an optional measurement unit.
		/// Shows number or "N/A" if null.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="number">Nullable integer number.</param>
		/// <param name="measurementUnit">Optional unit string.</param>
		/// <returns>HTML content representing the number.</returns>
		public static IHtmlContent RenderNumber(this IHtmlHelper html, int? number,
			string measurementUnit = "")
		{
			if (number.HasValue)
				return new HtmlString($"{number.Value} {measurementUnit}");
			else
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");
		}

		/// <summary>
		/// Renders a nullable double rounded to 2 decimals with an optional unit.
		/// Shows rounded number or "N/A" if null.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="number">Nullable double number.</param>
		/// <param name="measurementUnit">Optional unit string.</param>
		/// <returns>HTML content representing the rounded number.</returns>
		public static IHtmlContent RenderRoundedNumber(this IHtmlHelper html, double? number,
			string measurementUnit = "")
		{
			if (number.HasValue)
				return new HtmlString($"{ToRoundedString(number.Value)} {measurementUnit}");
			else
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");
		}

		/// <summary>
		/// Renders a non-nullable double rounded to 2 decimals.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="number">Double number.</param>
		/// <returns>HTML content with rounded number string.</returns>
		public static IHtmlContent RenderRoundedNumber(this IHtmlHelper html, double number)
			=> new HtmlString(ToRoundedString(number));

		private static string ToRoundedString(double number)
			=> Math.Round(number, 2).ToString("0.##");
		#endregion

		/// <summary>
		/// Renders a string as HTML or "N/A" if null or whitespace.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="text">Input text string.</param>
		/// <returns>HTML content with the text or "N/A".</returns>
		public static IHtmlContent RenderText(this IHtmlHelper html, string? text)
		{
			if (!string.IsNullOrWhiteSpace(text))
				return new HtmlString(text);
			else
				return new HtmlString($"<span class='text-muted'>{NOT_AVAILABLE_TEXT}</span>");
		}

		/// <summary>
		/// Renders a boolean indicator icon: green check for true, red cross for false.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="value">Boolean value.</param>
		/// <returns>HTML content with the indicator icon.</returns>
		public static IHtmlContent RenderBoolIndicator(this IHtmlHelper html, bool value)
		{
			if (value)
				return new HtmlString("<i class='fa fa-check text-success'></i>");
			else
				return new HtmlString("<i class='fa fa-xmark text-danger'></i>");
		}

		/// <summary>
		/// Renders a list of color indicators as styled divs with tooltips.
		/// Shows "N/A" if the list is null or empty.
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="colors">Collection of colors (nullable).</param>
		/// <returns>HTML content with color indicators or "N/A".</returns>
		public static IHtmlContent RenderColorIndicator(this IHtmlHelper html,
			IEnumerable<Color?> colors)
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

		/// <summary>
		/// Renders a date separator for chat messages inside a styled div.
		/// Shows "Month dd" if the date is in the current year, otherwise "Month dd, yyyy".
		/// </summary>
		/// <param name="html">Html helper instance.</param>
		/// <param name="date">The message date to display.</param>
		/// <returns>HTML content with the formatted date separator.</returns>
		public static IHtmlContent RenderMessageDate(this IHtmlHelper html, DateOnly date)
		{
			StringBuilder contentBuilder = new();
			CultureInfo culture = new("en-US");

			contentBuilder.Append("<div class=\"message-date-separator\">");
			if (date.Year == DateTime.Now.Year)
				contentBuilder.Append(date.ToString("MMMM dd", culture));
			else
				contentBuilder.Append(date.ToString("MMMM dd, yyyy", culture));
			contentBuilder.Append("</div>");

			return new HtmlString(contentBuilder.ToString());
		}
	}
}