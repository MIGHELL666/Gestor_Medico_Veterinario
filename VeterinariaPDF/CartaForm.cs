using System;
using System.Drawing;
using System.Windows.Forms;

public class CartaForm
{
    public CartaForm(Form form)
    {
        var txtDueno = new TextBox { Text = "Nombre y apellido", Top = 20, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtTelefono = new TextBox { Text = "Teléfono", Top = 60, Left = 20, Width = 400, ForeColor = Color.Gray };
        var dtpFecha = new DateTimePicker { Top = 100, Left = 20, Width = 200, Format = DateTimePickerFormat.Short };
        var txtRaza = new TextBox { Text = "Raza", Top = 140, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtTamano = new TextBox { Text = "Tamaño", Top = 180, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtEdad = new TextBox { Text = "Edad", Top = 220, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtCarac = new TextBox { Text = "Características especiales", Top = 260, Left = 20, Width = 400, Height = 60, ForeColor = Color.Gray, Multiline = true };

        AgregarPlaceholder(txtDueno, "Nombre y apellido");
        AgregarPlaceholder(txtTelefono, "Teléfono");
        AgregarPlaceholder(txtRaza, "Raza");
        AgregarPlaceholder(txtTamano, "Tamaño");
        AgregarPlaceholder(txtEdad, "Edad");
        AgregarPlaceholder(txtCarac, "Características especiales");

        var btn = new Button
        {
            Text = "Generar PDF",
            Top = 340,
            Left = 20,
            Width = 400,
            BackColor = Color.LightGoldenrodYellow
        };
                                                                                                
        btn.Click += (s, e) =>
        {
            string V(TextBox tb) => VeterinariaPDF.Helpers.Valor(tb);

            var edadVal = V(txtEdad);
            if (!string.IsNullOrWhiteSpace(edadVal) && !VeterinariaPDF.Helpers.EsEdadConUnidades(edadVal))
            {
                MessageBox.Show("La edad debe ser un número con unidad (ej. 2 años, 3 meses, 2 semanas, 10 días) o una combinación (ej. 1 año 3 meses), o solo un número.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombreArchivoBase = string.IsNullOrWhiteSpace(V(txtDueno)) ? "IngresoAlbergue.pdf" : $"IngresoAlbergue_{V(txtDueno)}.pdf";
            var ruta = VeterinariaPDF.Helpers.ElegirRutaGuardar(nombreArchivoBase);
            if (string.IsNullOrEmpty(ruta)) return;
            PDF.CrearCartaEntrega(
                dtpFecha.Value,
                V(txtRaza),
                V(txtTamano),
                V(txtEdad),
                V(txtCarac),
                V(txtDueno),
                V(txtTelefono),
                ruta
            );
        };

        form.Controls.Add(txtDueno);
        form.Controls.Add(txtTelefono);
        form.Controls.Add(dtpFecha);
        form.Controls.Add(txtRaza);
        form.Controls.Add(txtTamano);
        form.Controls.Add(txtEdad);
        form.Controls.Add(txtCarac);
        form.Controls.Add(btn);
    }

    private void AgregarPlaceholder(TextBox txt, string placeholder) => VeterinariaPDF.Helpers.AgregarPlaceholder(txt, placeholder);
}
