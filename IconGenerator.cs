namespace XIFocus;

internal static class IconGenerator
{
    public static Icon CreateAppIcon()
    {
        var bmp = new Bitmap(32, 32);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        // Background — rounded dark square
        g.Clear(Color.FromArgb(30, 30, 46));

        // Accent border
        using var borderPen = new Pen(Color.FromArgb(74, 158, 219), 2f);
        g.DrawRectangle(borderPen, 1, 1, 30, 30);

        // "XI" text centered in crystal blue
        using var font = new Font("Segoe UI", 14f, FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(74, 158, 219));
        var text = "XI";
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush,
            (32 - size.Width) / 2f,
            (32 - size.Height) / 2f);

        // Convert to icon
        var handle = bmp.GetHicon();
        return Icon.FromHandle(handle);
    }
}
