namespace XIFocus;

internal static class InputDialog
{
    public static string? Show(string prompt, string title, string defaultValue = "")
    {
        Form form = new Form
        {
            Text = title,
            Size = new Size(340, 150),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };
        Label lbl = new Label { Left = 12, Top = 16, Width = 300, Text = prompt };
        TextBox txt = new TextBox { Left = 12, Top = 38, Width = 300, Text = defaultValue };
        Button btnOk = new Button { Text = "OK", Left = 148, Top = 72, Width = 75, DialogResult = DialogResult.OK };
        Button btnCancel = new Button { Text = "Cancel", Left = 232, Top = 72, Width = 75, DialogResult = DialogResult.Cancel };
        form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
        form.AcceptButton = btnOk;
        form.CancelButton = btnCancel;
        return form.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
    }
}
