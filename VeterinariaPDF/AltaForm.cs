using System;
using System.Drawing;
using System.Windows.Forms;

public class AltaForm
{
    public AltaForm(Form form)
    {
        var txtProp = new TextBox { Text = "Nombre propietario", Top = 20, Left = 20, Width = 220, ForeColor = Color.Gray };
        var txtClave = new TextBox { Text = "Clave", Top = 20, Left = 260, Width = 160, ForeColor = Color.Gray };

        var txtMascota = new TextBox { Text = "Nombre de la mascota", Top = 60, Left = 20, Width = 220, ForeColor = Color.Gray };
        var txtEspecie = new TextBox { Text = "Especie", Top = 60, Left = 260, Width = 160, ForeColor = Color.Gray };

        var txtRaza = new TextBox { Text = "Raza", Top = 100, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtDx = new TextBox { Text = "DX", Top = 140, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtObs = new TextBox { Text = "Observaciones del médico", Top = 180, Left = 20, Width = 400, Height = 60, ForeColor = Color.Gray, Multiline = true };

        var dtpFecha = new DateTimePicker { Top = 260, Left = 20, Width = 200, Format = DateTimePickerFormat.Short };
        var dtpHora = new DateTimePicker { Top = 260, Left = 240, Width = 180, Format = DateTimePickerFormat.Time, ShowUpDown = true };

        var txtRetiro = new TextBox { Text = "Retiro anticipado de...", Top = 300, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtNombreAp = new TextBox { Text = "Nombre y apellido", Top = 340, Left = 20, Width = 400, ForeColor = Color.Gray };

        AgregarPlaceholder(txtProp, "Nombre propietario");
        AgregarPlaceholder(txtClave, "Clave");
        AgregarPlaceholder(txtMascota, "Nombre de la mascota");
        AgregarPlaceholder(txtEspecie, "Especie");
        AgregarPlaceholder(txtRaza, "Raza");
        AgregarPlaceholder(txtDx, "DX");
        AgregarPlaceholder(txtObs, "Observaciones del médico");
        AgregarPlaceholder(txtRetiro, "Retiro anticipado de...");
        AgregarPlaceholder(txtNombreAp, "Nombre y apellido");

        var btn = new Button
        {
            Text = "Generar PDF",
            Top = 380,
            Left = 20,
            Width = 400,
            BackColor = Color.LightBlue
        };

        btn.Click += (s, e) =>
        {
            string V(TextBox tb) => VeterinariaPDF.Helpers.Valor(tb);
            var nombreArchivoBase = string.IsNullOrWhiteSpace(V(txtMascota)) ? "Alta.pdf" : $"Alta_{V(txtMascota)}.pdf";
            var ruta = VeterinariaPDF.Helpers.ElegirRutaGuardar(nombreArchivoBase);
            if (string.IsNullOrEmpty(ruta)) return;
            PDF.CrearAltaHospitalaria(
                V(txtProp),
                V(txtClave),
                V(txtMascota),
                V(txtEspecie),
                V(txtRaza),
                V(txtDx),
                V(txtObs),
                dtpFecha.Value,
                dtpHora.Value,
                V(txtRetiro),
                V(txtNombreAp),
                ruta
            );
        };

        form.Controls.Add(txtProp);
        form.Controls.Add(txtClave);
        form.Controls.Add(txtMascota);
        form.Controls.Add(txtEspecie);
        form.Controls.Add(txtRaza);
        form.Controls.Add(txtDx);
        form.Controls.Add(txtObs);
        form.Controls.Add(dtpFecha);
        form.Controls.Add(dtpHora);
        form.Controls.Add(txtRetiro);
        form.Controls.Add(txtNombreAp);
        form.Controls.Add(btn);
    }

    private void AgregarPlaceholder(TextBox txt, string placeholder) => VeterinariaPDF.Helpers.AgregarPlaceholder(txt, placeholder);
}
