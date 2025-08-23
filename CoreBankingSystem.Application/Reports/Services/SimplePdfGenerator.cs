using System.Text;
using CoreBankingSystem.Application.Abstractions.Reports;

namespace CoreBankingSystem.Application.Reports.Services;

public class SimplePdfGenerator : IPdfGenerator
{
    public byte[] GenerateFromText(string text)
    {
        // Page setup
        const int pageWidth = 612;   // 8.5in * 72dpi
        const int pageHeight = 792;  // 11in * 72dpi
        const int margin = 36;       // 0.5in margins
        const int left = margin;
        const int top = pageHeight - margin;
        const int fontSize = 9;      // smaller font to fit table width
        const int leading = 11;      // line height
        // Courier: ~600 units per 1000em -> 0.6 * fontSize points per char
        var charWidth = 0.6 * fontSize;
        var maxCharsPerLine = (int)Math.Floor((pageWidth - 2 * margin) / charWidth);

        // Escape PDF special chars and wrap lines to respect right margin
        var safe = text
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");

        var rawLines = safe.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        foreach (var raw in rawLines)
        {
            lines.AddRange(Wrap(raw, maxCharsPerLine));
        }

        var sbStream = new StringBuilder();
        // Begin text object, set font, position, and leading for new lines
        sbStream.Append($"BT /F1 {fontSize} Tf {left} {top} Td {leading} TL ");
        foreach (var line in lines)
        {
            sbStream.Append('(').Append(line).Append(") Tj T* ");
        }
        sbStream.Append("ET");

        var stream = sbStream.ToString();
        var objects = new List<string>();
        objects.Add("1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj");
        objects.Add("2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj");
        objects.Add("3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>endobj");
        objects.Add($"4 0 obj<< /Length {stream.Length} >>stream\n{stream}\nendstream endobj");
        // Use Courier (monospaced) to better align ASCII table columns
        objects.Add("5 0 obj<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>endobj");

        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");
        var offsets = new List<int>();
        sb.Append("%\u00e2\u00e3\u00cf\u00d3\n");
        foreach (var obj in objects)
        {
            offsets.Add(sb.Length);
            sb.Append(obj).Append('\n');
        }
        int xrefPos = sb.Length;
        sb.Append($"xref\n0 {objects.Count + 1}\n");
        sb.Append("0000000000 65535 f \n");
        foreach (var off in offsets)
        {
            sb.Append(off.ToString("D10")).Append(" 00000 n \n");
        }
        sb.Append("trailer<< /Size ").Append(objects.Count + 1).Append(" /Root 1 0 R >>\nstartxref\n");
        sb.Append(xrefPos).Append("\n%%EOF");
        return Encoding.ASCII.GetBytes(sb.ToString());

        static IEnumerable<string> Wrap(string input, int maxChars)
        {
            if (string.IsNullOrEmpty(input) || maxChars <= 0)
            {
                yield break;
            }
            int idx = 0;
            while (idx < input.Length)
            {
                int take = Math.Min(maxChars, input.Length - idx);
                int end = idx + take;
                if (take == maxChars && end < input.Length)
                {
                    // try to break at last space within window
                    int lastSpace = input.LastIndexOf(' ', end - 1, take);
                    if (lastSpace > idx + maxChars / 2)
                    {
                        end = lastSpace;
                        take = end - idx;
                    }
                }
                yield return input.Substring(idx, take).TrimEnd();
                idx += take;
                // skip a single space if we broke on it
                if (idx < input.Length && input[idx] == ' ')
                    idx++;
            }
        }
    }
}
