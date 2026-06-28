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

    public static void SaveIconToFile(string path)
    {
        // Generate at 256x256 for crisp Explorer display
        using var bmp = new Bitmap(256, 256);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        g.Clear(Color.FromArgb(30, 30, 46));

        using var borderPen = new Pen(Color.FromArgb(74, 158, 219), 8f);
        g.DrawRectangle(borderPen, 4, 4, 248, 248);

        using var font = new Font("Segoe UI", 110f, FontStyle.Bold);
        using var brush = new SolidBrush(Color.FromArgb(74, 158, 219));
        var text = "XI";
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush,
            (256 - size.Width) / 2f,
            (256 - size.Height) / 2f);

        // Write as valid ICO with PNG-compressed 256x256 frame
        using var ms = new System.IO.MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        var png = ms.ToArray();

        using var fs = new FileStream(path, FileMode.Create);
        using var w = new BinaryWriter(fs);
        w.Write((short)0);       // reserved
        w.Write((short)1);       // type = ICO
        w.Write((short)1);       // 1 image
        w.Write((byte)0);        // width 0 = 256
        w.Write((byte)0);        // height 0 = 256
        w.Write((byte)0);        // color count
        w.Write((byte)0);        // reserved
        w.Write((short)1);       // planes
        w.Write((short)32);      // bit depth
        w.Write(png.Length);     // data size
        w.Write(22);             // data offset (6 + 16)
        w.Write(png);            // PNG data
    }
}
