using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace TimaticApi
{
    public class TimaticResultParser
    {
        public IEnumerable<TimaticResultSection> ParseTimaticResult(string timaticResult)
        {
            var sections = new List<TimaticResultSection>();
            var document = new HtmlDocument();
            document.LoadHtml(timaticResult);

            var resultSection = document.DocumentNode.SelectSingleNode("//div[@class='result']");

            var summary = resultSection.SelectSingleNode("//div[@class='item summary']");
            var passport = resultSection.SelectSingleNode("//div[@class='item passport']");
            var visa = resultSection.SelectSingleNode("//div[@class='item visa']");
            var health = resultSection.SelectSingleNode("//div[@class='item health']");


            sections.Add(new TimaticResultSection("Summary", summary));
            sections.Add(new TimaticResultSection("Passport", passport));
            sections.Add(new TimaticResultSection("Visa", visa));
            sections.Add(new TimaticResultSection("Health", health));

            return sections;
        }
    }
}