using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class RichTextProcessor
{
    private static readonly Regex tagPattern = new(@"<(?<tag>\w+)(=[^>]+)?>|</(?<endtag>\w+)>", RegexOptions.Compiled);

    public static List<string> Process(string input)
    {
        var result = new List<string>();
        ParseRecursive(input, new Stack<string>(), result);
        return result;
    }

    private static void ParseRecursive(string input, Stack<string> activeTags, List<string> result)
    {
        int index = 0;
        while (index < input.Length)
        {
            var match = tagPattern.Match(input, index);

            if (match.Success && match.Index == index)
            {
                if (!string.IsNullOrEmpty(match.Groups["tag"].Value))
                {
                    // It's an opening tag
                    string fullTag = match.Value;
                    string tagName = match.Groups["tag"].Value;

                    // Find the matching closing tag
                    string closeTag = $"</{tagName}>";
                    int closeIndex = input.IndexOf(closeTag, index + match.Length, StringComparison.Ordinal);
                    if (closeIndex == -1)
                    {
                        // No closing tag, just treat it as text
                        result.Add(BuildFormattedChar(input[index], activeTags));
                        index++;
                        continue;
                    }

                    // Extract inner content
                    int innerStart = index + match.Length;
                    string inner = input.Substring(innerStart, closeIndex - innerStart);

                    // Push this tag and recurse
                    activeTags.Push(fullTag);
                    ParseRecursive(inner, activeTags, result);
                    activeTags.Pop();

                    // Move index past closing tag
                    index = closeIndex + closeTag.Length;
                }
                else if (!string.IsNullOrEmpty(match.Groups["endtag"].Value))
                {
                    // Unexpected closing tag — just skip
                    index += match.Length;
                }
            }
            else
            {
                result.Add(BuildFormattedChar(input[index], activeTags));
                index++;
            }
        }
    }

    private static string BuildFormattedChar(char c, Stack<string> activeTags)
    {
        string open = string.Concat(activeTags.ToArray());
        string close = string.Concat(GetClosingTags(activeTags));
        return $"{open}{c}{close}";
    }

    private static IEnumerable<string> GetClosingTags(Stack<string> tags)
    {
        var closing = new List<string>();
        foreach (var tag in tags)
        {
            // Example: <color=#ffff00> → </color>
            var tagName = Regex.Match(tag, @"<(\w+)").Groups[1].Value;
            closing.Insert(0, $"</{tagName}>");
        }
        return closing;
    }
}
