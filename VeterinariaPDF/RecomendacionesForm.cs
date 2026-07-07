using System;
using System.Drawing;
using System.Windows.Forms;

public class RecomendacionesForm
{
    public RecomendacionesForm(Form form)
    {
        var txtMascota = new TextBox { Text = "Nombre mascota", Top = 20, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtCirugia = new TextBox { Text = "Nombre de la cirugía", Top = 60, Left = 20, Width = 400, ForeColor = Color.Gray };

        AgregarPlaceholder(txtMascota, "Nombre mascota");
        AgregarPlaceholder(txtCirugia, "Nombre de la cirugía");

        var btn = new Button
        {
            Text = "Generar PDF",
            Top = 100,
            Left = 20,
            Width = 400,
            BackColor = Color.LightPink
        };

        btn.Click += (s, e) =>
        {
            string V(TextBox tb) => VeterinariaPDF.Helpers.Valor(tb);
            var nombreArchivoBase = string.IsNullOrWhiteSpace(V(txtMascota)) ? "Recomendaciones.pdf" : $"Recomendaciones_{V(txtMascota)}.pdf";
            var ruta = VeterinariaPDF.Helpers.ElegirRutaGuardar(nombreArchivoBase);
            if (string.IsNullOrEmpty(ruta)) return;
            PDF.CrearRecomendaciones(V(txtMascota), V(txtCirugia), ruta);
        };

        form.Controls.Add(txtMascota);
        form.Controls.Add(txtCirugia);
        form.Controls.Add(btn);
    }

    private void AgregarPlaceholder(TextBox txt, string placeholder) => VeterinariaPDF.Helpers.AgregarPlaceholder(txt, placeholder);
}
