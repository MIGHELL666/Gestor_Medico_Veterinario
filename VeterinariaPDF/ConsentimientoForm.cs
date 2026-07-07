using System;
using System.Drawing;
using System.Windows.Forms;

public class ConsentimientoForm
{
    public ConsentimientoForm(Form form)
    {
        var chkEsterilizacion = new CheckBox { Text = "Esterilización", Top = 20, Left = 20, Width = 120 };
        var chkOtro = new CheckBox { Text = "Otro", Top = 20, Left = 160, Width = 60 };
        var txtOtro = new TextBox { Text = "Nombre del procedimiento", Top = 20, Left = 230, Width = 190, ForeColor = Color.Gray };

        var txtMascota = new TextBox { Text = "Nombre del paciente", Top = 60, Left = 20, Width = 300, ForeColor = Color.Gray };
        var rbM = new RadioButton { Text = "M", Top = 60, Left = 330, Width = 40 };
        var rbH = new RadioButton { Text = "H", Top = 60, Left = 380, Width = 40 };

        var txtEdad = new TextBox { Text = "Edad", Top = 100, Left = 20, Width = 150, ForeColor = Color.Gray };
        var txtRaza = new TextBox { Text = "Raza", Top = 100, Left = 200, Width = 220, ForeColor = Color.Gray };

        var txtDueno = new TextBox { Text = "Nombre del dueño", Top = 140, Left = 20, Width = 400, ForeColor = Color.Gray };
        var txtTel = new TextBox { Text = "Tel", Top = 180, Left = 20, Width = 180, ForeColor = Color.Gray };
        var txtCel = new TextBox { Text = "Cel", Top = 180, Left = 220, Width = 200, ForeColor = Color.Gray };
        var dtpFecha = new DateTimePicker { Top = 220, Left = 20, Width = 200, Format = DateTimePickerFormat.Short };

        AgregarPlaceholder(txtOtro, "Nombre del procedimiento");
        AgregarPlaceholder(txtMascota, "Nombre del paciente");
        AgregarPlaceholder(txtEdad, "Edad");
        AgregarPlaceholder(txtRaza, "Raza");
        AgregarPlaceholder(txtDueno, "Nombre del dueño");
        AgregarPlaceholder(txtTel, "Tel");
        AgregarPlaceholder(txtCel, "Cel");

        var btn = new Button
        {
            Text = "Generar PDF",
            Top = 260,
            Left = 20,
            Width = 400,
            BackColor = Color.LightSalmon
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
            var telVal = V(txtTel);
            if (!string.IsNullOrWhiteSpace(telVal) && !VeterinariaPDF.Helpers.EsTelefono(telVal))
            {
                MessageBox.Show("El teléfono solo debe contener dígitos, espacios o guiones.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var celVal = V(txtCel);
            if (!string.IsNullOrWhiteSpace(celVal) && !VeterinariaPDF.Helpers.EsTelefono(celVal))
            {
                MessageBox.Show("El celular solo debe contener dígitos, espacios o guiones.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombreArchivoBase = string.IsNullOrWhiteSpace(V(txtMascota)) ? "Consentimiento.pdf" : $"Consentimiento_{V(txtMascota)}.pdf";
            var ruta = VeterinariaPDF.Helpers.ElegirRutaGuardar(nombreArchivoBase);
            if (string.IsNullOrEmpty(ruta)) return;

            PDF.CrearConsentimiento(
                V(txtMascota),
                V(txtEdad),
                V(txtRaza),
                V(txtDueno),
                V(txtTel),
                V(txtCel),
                rbM.Checked,
                rbH.Checked,
                chkEsterilizacion.Checked,
                V(txtOtro),
                dtpFecha.Value,
                ruta
            );
        };

        form.Controls.Add(chkEsterilizacion);
        form.Controls.Add(chkOtro);
        form.Controls.Add(txtOtro);
        form.Controls.Add(txtMascota);
        form.Controls.Add(rbM);
        form.Controls.Add(rbH);
        form.Controls.Add(txtEdad);
        form.Controls.Add(txtRaza);
        form.Controls.Add(txtDueno);
        form.Controls.Add(txtTel);
        form.Controls.Add(txtCel);
        form.Controls.Add(dtpFecha);
        form.Controls.Add(btn);
    }

    private void AgregarPlaceholder(TextBox txt, string placeholder) => VeterinariaPDF.Helpers.AgregarPlaceholder(txt, placeholder);
}
