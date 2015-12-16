using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UI.Logic
{
    BR = 0,
    Bold = 1,
    Italic = 2,
    Underline = 3,
    Strikeout = 4,
    Sup = 9,
    Sub = 10,
    Link = 5,
    Image = 6,
    Color = 7,
    Headling = 8,
    Quote = 11,
    Code = 12
}

public class BBParser
{
    string[] replaceIterationArray;

    public readonly Dictionary<BBPatternEnum, string> BBPatterns = new Dictionary<BBPatternEnum, string>
    {
        { BBPatternEnum.BR,         "\\n" },
        { BBPatternEnum.Bold,       "\\[b\\]([^(\\[/b\\])][\\s\\S]*?)\\[/b\\]" },
        { BBPatternEnum.Italic,     "\\[i\\]([^(\\[/i\\])][\\s\\S]*?)\\[/i\\]" },
        { BBPatternEnum.Underline,  "\\[u\\]([^(\\[/u\\])][\\s\\S]*?)\\[/u\\]" },
        { BBPatternEnum.Strikeout,  "\\[s\\]([^(\\[/s\\])][\\s\\S]*?)\\[/s\\]" },
        { BBPatternEnum.Sup,        "\\[sup\\]([^(\\[/sup\\])][\\s\\S]*?)\\[/sup\\]" },
        { BBPatternEnum.Sub,        "\\[sub\\]([^(\\[/sub\\])][\\s\\S]*?)\\[/sub\\]" },
        { BBPatternEnum.Quote,      "\\[quote(\\s?user\\s?=\\s?[\"'](.*?)[\"'])?\\]([\\s\\S]*?)\\[/quote\\]" },
        { BBPatternEnum.Link,       "\\[link\\s?url\\s?=\\s?[\"'](?<url>.*?)[\"'].*?\\](?<title>[\\S\\s]*?)\\[/link\\]" },
        { BBPatternEnum.Image,      "\\[image.*?url\\s?=\\s?[\"'](?<url>.*?)[\"'].*?(width\\s?=\\s?[\"'](?<width>[0-9]{1,1000}(px|%)?)[\"'])?\\]\\[/image\\]" },
        { BBPatternEnum.Code,       "\\[code(\\s?syntax\\s?=\\s?[\"'](.*?)[\"'])?\\]([^\\[/code\\]][\\s\\S]*?)\\[/code\\]" },
        { BBPatternEnum.Color,      "\\[color(\\s?color\\s?=\\s?[\"'](.*?)[\"'])?\\]([^\\[/color\\]][\\s\\S]*?)\\[/color\\]" },
        { BBPatternEnum.Headling,   "\\[head(\\s?size\\s?=\\s?[\"']([0-4]{0,1})[\"'])?\\]([^(\\[/head\\])][\\s\\S]*?)\\[/head\\]" }
    };

    public BBParser()
    {
        this.replaceIterationArray = new string[this.BBPatterns.Count + 1];
    }

    public string FormatBB(string text)
    {

        replaceIterationArray[0] = text ?? string.Empty;

        var i = 1;

        if (!string.IsNullOrEmpty(text))
        {

            var sb = new StringBuilder();

            foreach (var pat in BBPatterns)
            {
                replaceIterationArray[i] = new Regex(pat.Value, RegexOptions.Multiline).Replace(replaceIterationArray[i - 1], (m) =>
                {

                    switch (pat.Key)
                    {
                        case BBPatternEnum.BR:
                            return "<br/>";

                        case BBPatternEnum.Bold:
                            return string.Format("<strong>{0}</strong>", m.Groups[1].Value);

                        case BBPatternEnum.Italic:
                            return string.Format("<i>{0}</i>", m.Groups[1].Value);

                        case BBPatternEnum.Strikeout:
                            return string.Format("<s>{0}</s>", m.Groups[1].Value);

                        case BBPatternEnum.Underline:
                            return string.Format("<u>{0}</u>", m.Groups[1].Value);

                        case BBPatternEnum.Sup:
                            return string.Format("<sup>{0}</sup>", m.Groups[1].Value);

                        case BBPatternEnum.Sub:
                            return string.Format("<sub>{0}</sub>", m.Groups[1].Value);

                        case BBPatternEnum.Link:
                            return string.Format("<a href=\"{0}\">{1}</a>", m.Groups["url"].Value, (!string.IsNullOrEmpty(m.Groups["title"].Value) ? m.Groups["title"].Value : m.Groups["url"].Value));

                        case BBPatternEnum.Image:

                            var widthApplied = false;

                            var width = string.Empty;

                            if (m.Groups["width"].Success)
                            {
                                widthApplied = true;

                                width = m.Groups["width"].Value;

                                if (!width.Contains("%") &&
                                    !width.Contains("px"))
                                {
                                    width += "%";
                                }
                            }

                            return string.Format("<img src=\"{0}\" {1} />", m.Groups["url"].Value, widthApplied ? "width=\"" + width + "\"" : string.Empty);

                        case BBPatternEnum.Quote:
                            sb.Clear();

                            sb.Append("<div class=\"quote\">");

                            if (!string.IsNullOrEmpty(m.Groups[2].Value))
                                sb.AppendFormat("<div class=\"user\">{0}</div>", m.Groups[2].Value);

                            sb.AppendFormat("<div>{0}</div></div>", m.Groups[3].Value);

                            return sb.ToString();

                        case BBPatternEnum.Code:
                            sb.Clear();

                            if (!string.IsNullOrEmpty(m.Groups[2].Value))
                                sb.AppendFormat("<div syntax=\"{0}\">{1}</div>", m.Groups[2].Value, m.Groups[3].Value);
                            else
                                sb.AppendFormat("<div syntax=\"cplusplus\">{0}</div>", m.Groups[3].Value);

                            return sb.ToString();

                        case BBPatternEnum.Color:
                            sb.Clear();

                            var colorString = !string.IsNullOrEmpty(m.Groups[2].Value) && (m.Groups[2].Value == "red" || m.Groups[2].Value == "blue" || m.Groups[2].Value == "green") ? m.Groups[2].Value : "def";

                            sb.AppendFormat("<span class=\"color {0}\">{1}</div>", colorString, m.Groups[3].Value);

                            return sb.ToString();

                        case BBPatternEnum.Headling:
                            sb.Clear();

                            sb.AppendFormat("<div class=\"head {0}\">{1}</div>", !string.IsNullOrEmpty(m.Groups[2].Value) ? m.Groups[2].Value : "1", m.Groups[3].Value);

                            return sb.ToString();

                        default:
                            return string.Empty;
                    }

                });

                i++;
            }

        }

        return this.compliledHtmlString();
    }

    string compliledHtmlString()
    {
        return this.replaceIterationArray[this.replaceIterationArray.Length - 1];
    }

}