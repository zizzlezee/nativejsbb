using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public enum BBPatternEnum : byte
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

        /// <summary>
        /// regex pattern to validate available BB image attributes (src, width, align)
        /// </summary>
        readonly Regex imageAttrsRegex = new Regex("(?<opt>src|width|align)=[\"'](?<val>.*?)[\"']");

        readonly Regex linkAttrsRegex = new Regex("(?<opt>href|target)=[\"'](?<val>.*?)[\"']");

        const string brTag = "<br/>";

        readonly Dictionary<BBPatternEnum, string> BBPatterns = new Dictionary<BBPatternEnum, string>
        {
            { BBPatternEnum.BR,         "\\n" },
            { BBPatternEnum.Bold,       "\\[b\\]([^(\\[/b\\])][\\s\\S]*?)\\[/b\\]" },
            { BBPatternEnum.Italic,     "\\[i\\]([^(\\[/i\\])][\\s\\S]*?)\\[/i\\]" },
            { BBPatternEnum.Underline,  "\\[u\\]([^(\\[/u\\])][\\s\\S]*?)\\[/u\\]" },
            { BBPatternEnum.Strikeout,  "\\[s\\]([^(\\[/s\\])][\\s\\S]*?)\\[/s\\]" },
            { BBPatternEnum.Sup,        "\\[sup\\]([^(\\[/sup\\])][\\s\\S]*?)\\[/sup\\]" },
            { BBPatternEnum.Sub,        "\\[sub\\]([^(\\[/sub\\])][\\s\\S]*?)\\[/sub\\]" },
            { BBPatternEnum.Quote,      "\\[quote(\\s?user\\s?=\\s?[\"'](.*?)[\"'])?\\]([\\s\\S]*?)\\[/quote\\]" },
            { BBPatternEnum.Link,       "\\[link\\s?(?<linkOpts>.*?)\\s?\\](?<title>[\\S\\s]+?)\\[/link\\]" },
            { BBPatternEnum.Image,      "\\[image\\s?(?<imageOpts>.*?)\\s?\\]\\[/image\\]" },
            { BBPatternEnum.Code,       "\\[code(\\s?syntax\\s?=\\s?[\"'](?<syntax>.*?)[\"'])?\\](?<code>[\\s\\S]*?)\\[/code\\]" },
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

            var codeSourceWithoutBR = string.Empty;

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
                                return brTag;

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
                                return this.formatLinkTag(m.Groups["linkOpts"].Value, m.Groups["title"].Value);

                            case BBPatternEnum.Image:
                                return this.formatImageTag(m.Groups["imageOpts"].Value ?? string.Empty);

                            case BBPatternEnum.Quote:
                                sb.Clear();

                                sb.Append("<div class=\"quote\">");

                                if (!string.IsNullOrEmpty(m.Groups[2].Value))
                                    sb.AppendFormat("<div class=\"user\">{0}</div>", m.Groups[2].Value);

                                sb.AppendFormat("<div>{0}</div></div>", m.Groups[3].Value);

                                return sb.ToString();

                            case BBPatternEnum.Code:
                                sb.Clear();

                                codeSourceWithoutBR = !string.IsNullOrEmpty(m.Groups["code"].Value) ? m.Groups["code"].Value.Replace(brTag, Environment.NewLine) : "";

                                if (!string.IsNullOrEmpty(m.Groups["syntax"].Value))
                                    sb.AppendFormat("<div syntax=\"{0}\">{1}</div>", m.Groups["syntax"].Value, codeSourceWithoutBR);
                                else
                                    sb.AppendFormat("<div syntax=\"c++\">{0}</div>", codeSourceWithoutBR);

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

            return this.findLastNotNullIteration();
        }

        string findLastNotNullIteration()
        {
            return this.replaceIterationArray[this.replaceIterationArray.Length - 1];
        }
        
        string formatImageTag(string attributes)
        {
            if (this.imageAttrsRegex.Match(attributes).Success)
            {
                var sb = new StringBuilder();

                var i = new int();

                foreach (Match m in imageAttrsRegex.Matches(attributes))
                {
                    if (m.Success)
                    {

                        if (i < imageAttrsRegex.Matches(attributes).Count - 1)
                            sb.AppendFormat("{0}='{1}' ", m.Groups["opt"], m.Groups["val"]);

                        else 
                            sb.AppendFormat("{0}='{1}'", m.Groups["opt"], m.Groups["val"]);

                    }

                    i++;
                }

                return string.Format("<img {0} />", sb.ToString());
            }

            return string.Empty;
        }

        string formatLinkTag(string attributes, string title)
        {
            if (!string.IsNullOrEmpty(title) &&
                this.linkAttrsRegex.Match(attributes).Success)
            {
                var sb = new StringBuilder();

                var i = new int();

                foreach (Match m in linkAttrsRegex.Matches(attributes))
                {
                    if (m.Success)
                    {

                        if (i < imageAttrsRegex.Matches(attributes).Count - 1)
                        {
                            sb.AppendFormat("{0}='{1}' ", m.Groups["opt"], m.Groups["val"]);
                        }
                        else
                        {
                            sb.AppendFormat("{0}='{1}'", m.Groups["opt"], m.Groups["val"]);
                        }

                    }

                    i++;
                }


                return string.Format("<a {0}>{1}</a>", sb.ToString(), title);
            }

            return string.Empty;

        }
        
    }