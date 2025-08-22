using System.Text;
using CoreBankingSystem.Application.Abstractions.Reports;

namespace CoreBankingSystem.Application.Reports.Services;

public class SimplePdfGenerator : IPdfGenerator
{
    public byte[] GenerateFromText(string text)
    {
        string sanitized = text.Replace("(", "[").Replace(")", "]");
        string contentStream = $"BT /F1 10 Tf 72 750 Td ({sanitized.Replace("\\n", ") Tj T* (")}) Tj ET";

        var objects = new List<string>();
        objects.Add("1 0 obj<< /Type /Catalog /Pages 2 0 R >>endobj");
        objects.Add("2 0 obj<< /Type /Pages /Kids [3 0 R] /Count 1 >>endobj");
        objects.Add("3 0 obj<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>endobj");
        objects.Add($"4 0 obj<< /Length {contentStream.Length} >>stream\n{contentStream}\nendstream endobj");
        objects.Add("5 0 obj<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>endobj");

        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");
        var offsets = new List<int>();
        sb.Append("%\u00e2\u00e3\u00cf\u00d3\n");
        foreach (var obj in objects.Select((val, idx) => new { val, idx }))
        {
            offsets.Add(sb.Length);
            sb.Append(obj.val);
            sb.Append("\n");
        }
        int xrefPos = sb.Length;
        sb.Append($"xref\n0 {objects.Count + 1}\n");
        sb.Append("0000000000 65535 f \n");
        foreach (var off in offsets)
        {
            sb.Append(off.ToString("D10"));
            sb.Append(" 00000 n \n");
        }
        sb.Append("trailer<< /Size ");
        sb.Append(objects.Count + 1);
        sb.Append(" /Root 1 0 R >>\nstartxref\n");
        sb.Append(xrefPos);
        sb.Append("\n%%EOF");
        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}
