using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace deepbim
{
    public static class MarkdownToInlineConverter
    {
        private static readonly FontFamily HeaderFont = new FontFamily("Segoe UI Semibold");
        private static readonly FontFamily CodeFont = new FontFamily("Consolas");
        private const double HeaderFontSize = 14;
        private const double NormalFontSize = 12;

        public static IEnumerable<Block> ConvertToBlocks(string markdownText)
        {
            var blocks = new List<Block>();
            var lines = markdownText.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("###"))
                {
                    blocks.Add(CreateHeaderBlock(trimmedLine.Substring(3).Trim()));
                }
                else if (trimmedLine.StartsWith(">"))  // Blockquote
                {
                    blocks.Add(CreateBlockquote(trimmedLine.Substring(1).Trim()));
                }
                else if (IsListLine(trimmedLine))
                {
                    blocks.Add(CreateListItem(trimmedLine));
                }
                else
                {
                    blocks.Add(CreateFormattedParagraph(trimmedLine)); // Handles **bold**, *italic*, etc.
                }
            }

            return blocks;
        }

        private static Paragraph CreateHeaderBlock(string text)
        {
            return new Paragraph(new Bold(new Run(text)))
            {
                FontSize = HeaderFontSize,
                FontFamily = HeaderFont,
                Foreground = Brushes.White,  // 🔥 Text is white
                Margin = new Thickness(0, 5, 0, 10)
            };
        }

        private static Paragraph CreateBlockquote(string text)
        {
            return new Paragraph(new Run(text))
            {
                FontSize = NormalFontSize,
                Foreground = Brushes.White,  // 🔥 Changed from LightGray to White
                Margin = new Thickness(20, 5, 0, 5),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2, 0, 0, 0),
                Padding = new Thickness(5)
            };
        }

        private static Paragraph CreateListItem(string text)
        {
            var paragraph = new Paragraph
            {
                Margin = new Thickness(20, 2, 0, 2),
                TextIndent = -10,
                Foreground = Brushes.White  // 🔥 Changed to White
            };

            if (text.StartsWith("1.") || text.StartsWith("2.") || text.StartsWith("3."))
            {
                var parts = text.Split(new[] { ' ' }, 2);
                paragraph.Inlines.Add(new Bold(new Run(parts[0] + " ") { Foreground = Brushes.White }));
                paragraph.Inlines.Add(new Run(parts[1]) { Foreground = Brushes.White });
            }
            else
            {
                paragraph.Inlines.Add(new Run("• ") { Foreground = Brushes.White });
                paragraph.Inlines.Add(new Run(text) { Foreground = Brushes.White });
            }

            return paragraph;
        }

        private static Paragraph CreateFormattedParagraph(string text)
        {
            var paragraph = new Paragraph();
            var inlines = ParseMarkdownInline(text);

            foreach (var inline in inlines)
            {
                paragraph.Inlines.Add(inline);
            }

            paragraph.Foreground = Brushes.White;  // 🔥 Ensure normal text is White
            return paragraph;
        }

        private static List<Inline> ParseMarkdownInline(string text)
        {
            var inlines = new List<Inline>();

            // **Bold and Italic** (***text***)
            text = Regex.Replace(text, @"\*\*\*(.*?)\*\*\*", m => $"<b><i>{m.Groups[1].Value}</i></b>");

            // **Bold** text
            text = Regex.Replace(text, @"\*\*(.*?)\*\*", m => $"<b>{m.Groups[1].Value}</b>");

            // *Italic* text
            text = Regex.Replace(text, @"\*(.*?)\*", m => $"<i>{m.Groups[1].Value}</i>");

            // ~~Strikethrough~~
            text = Regex.Replace(text, @"~~(.*?)~~", m => $"<s>{m.Groups[1].Value}</s>");

            // `Inline code`
            text = Regex.Replace(text, @"`(.*?)`", m => $"<code>{m.Groups[1].Value}</code>");

            // Process formatted text
            var parts = Regex.Split(text, @"(<b>|</b>|<i>|</i>|<s>|</s>|<code>|</code>)");
            bool isBold = false, isItalic = false, isStrikethrough = false, isCode = false;

            foreach (var part in parts)
            {
                switch (part)
                {
                    case "<b>":
                        isBold = true;
                        continue;
                    case "</b>":
                        isBold = false;
                        continue;
                    case "<i>":
                        isItalic = true;
                        continue;
                    case "</i>":
                        isItalic = false;
                        continue;
                    case "<s>":
                        isStrikethrough = true;
                        continue;
                    case "</s>":
                        isStrikethrough = false;
                        continue;
                    case "<code>":
                        isCode = true;
                        continue;
                    case "</code>":
                        isCode = false;
                        continue;
                }

                Inline inline = new Run(part) { Foreground = Brushes.White };  // 🔥 Default text color is White

                if (isBold) inline = new Bold(inline);
                if (isItalic) inline = new Italic(inline);
                if (isStrikethrough) inline = new Run(part) { TextDecorations = TextDecorations.Strikethrough, Foreground = Brushes.White };
                if (isCode)
                {
                    inline = new Run(part)
                    {
                        FontFamily = CodeFont,
                        Background = Brushes.DarkGray,
                        Foreground = Brushes.White  // 🔥 Change from Black to White
                    };
                }

                inlines.Add(inline);
            }

            return inlines;
        }

        private static bool IsListLine(string text)
        {
            return text.StartsWith("• ") ||
                   text.StartsWith("1. ") ||
                   text.StartsWith("2. ") ||
                   text.StartsWith("3. ") ||
                   text.StartsWith("- ");
        }
    }
}
