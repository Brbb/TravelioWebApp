using HtmlAgilityPack;

namespace TimaticApi
{
    public class TimaticResultSection
    {
		public TimaticResultSection(string name, HtmlNode node)
		{
			SectionName = name;

			if (node == null)
			{
				Content = "<div></div>";
			}
			else
			{
				Content = node.InnerHtml;
			}
		}

		public string SectionName { get; private set; }
		public string Content { get; private set; }
    }
}