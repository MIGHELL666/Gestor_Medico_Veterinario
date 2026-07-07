using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class HistoriaClinicaForm
{
    public HistoriaClinicaForm(Form form)
    {
        form.AutoScroll = true;
        int y = 20;
        int gap = 40;
        int col2 = 240;
        int wFull = 420;
        int wHalf = 200;

        // --- DATOS DEL PACIENTE ---
        var lblPaciente = new Label { Text = "DATOS DEL PACIENTE", Top = y, Left = 20, Width = wFull, Font = new Font("Arial", 10, FontStyle.Bold) };
        form.Controls.Add(lblPaciente); y += 30;

        var txtPacNombre = new TextBox { Text = "Nombre Paciente", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtPacEspecie = new TextBox { Text = "Especie", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtPacNombre); form.Controls.Add(txtPacEspecie); y += gap;

        var txtPacRaza = new TextBox { Text = "Raza", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtPacSexo = new TextBox { Text = "Sexo", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtPacRaza); form.Controls.Add(txtPacSexo); y += gap;

        var txtPacEdad = new TextBox { Text = "Edad", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtPacColor = new TextBox { Text = "Color", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtPacEdad); form.Controls.Add(txtPacColor); y += gap;

        var txtPacPeso = new TextBox { Text = "Peso", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtPacPeso); y += gap;

        // --- DATOS DEL PROPIETARIO ---
        var lblDueno = new Label { Text = "DATOS DEL PROPIETARIO", Top = y, Left = 20, Width = wFull, Font = new Font("Arial", 10, FontStyle.Bold) };
        form.Controls.Add(lblDueno); y += 30;

        var txtDuenoNombre = new TextBox { Text = "Nombre Dueño", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtDuenoNombre); y += gap;

        var txtDuenoTel = new TextBox { Text = "Teléfono", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtDuenoTel); y += gap;

        // --- ANAMNESIS ---
        var lblAnamnesis = new Label { Text = "ANAMNESIS", Top = y, Left = 20, Width = wFull, Font = new Font("Arial", 10, FontStyle.Bold) };
        form.Controls.Add(lblAnamnesis); y += 30;

        var txtDespProd = new TextBox { Text = "Desparasitación Producto", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtDespFecha = new TextBox { Text = "Fecha Desp.", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtDespProd); form.Controls.Add(txtDespFecha); y += gap;

        var txtVacuna = new TextBox { Text = "Vacuna", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtVacunaFecha = new TextBox { Text = "Fecha Vac.", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtVacuna); form.Controls.Add(txtVacunaFecha); y += gap;

        var txtHistRepro = new TextBox { Text = "Historia Reproductiva", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtHistRepro); y += gap;

        var txtEnfPrev = new TextBox { Text = "Enfermedades Anteriores", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtEnfPrev); y += gap;

        var txtAlimentacion = new TextBox { Text = "Alimentación", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtAlimentacion); y += gap;

        var txtTratPrev = new TextBox { Text = "Tratamientos Previos", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtTratPrev); y += gap;

        // --- MOTIVO CONSULTA ---
        var txtMotivo = new TextBox { Text = "Motivo de Consulta", Top = y, Left = 20, Width = wFull, Height = 50, Multiline = true, ForeColor = Color.Gray };
        form.Controls.Add(txtMotivo); y += 60;

        // --- EXAMEN CLINICO ---
        var lblExamen = new Label { Text = "EXAMEN CLÍNICO", Top = y, Left = 20, Width = wFull, Font = new Font("Arial", 10, FontStyle.Bold) };
        form.Controls.Add(lblExamen); y += 30;

        var txtFResp = new TextBox { Text = "F. Respiratoria", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtFCard = new TextBox { Text = "F. Cardíaca", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtFResp); form.Controls.Add(txtFCard); y += gap;

        var txtTemp = new TextBox { Text = "Temperatura", Top = y, Left = 20, Width = wHalf, ForeColor = Color.Gray };
        var txtLlenado = new TextBox { Text = "Llenado capilar", Top = y, Left = col2, Width = wHalf, ForeColor = Color.Gray };
        form.Controls.Add(txtTemp); form.Controls.Add(txtLlenado); y += gap;

        var txtGanglios = new TextBox { Text = "Ganglios linfáticos", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtGanglios); y += gap;

        var txtDiag = new TextBox { Text = "Diagnóstico presuntivo", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtDiag); y += gap;

        var txtTrat = new TextBox { Text = "Tratamiento", Top = y, Left = 20, Width = wFull, ForeColor = Color.Gray };
        form.Controls.Add(txtTrat); y += gap;

        var txtExam = new TextBox { Text = "Exámenes complementarios", Top = y, Left = 20, Width = wFull, Height = 50, ForeColor = Color.Gray, Multiline = true };
        form.Controls.Add(txtExam); y += 60;

        // Placeholders
        AgregarPlaceholder(txtPacNombre, "Nombre Paciente");
        AgregarPlaceholder(txtPacEspecie, "Especie");
        AgregarPlaceholder(txtPacRaza, "Raza");
        AgregarPlaceholder(txtPacSexo, "Sexo");
        AgregarPlaceholder(txtPacEdad, "Edad");
        AgregarPlaceholder(txtPacColor, "Color");
        AgregarPlaceholder(txtPacPeso, "Peso");
        AgregarPlaceholder(txtDuenoNombre, "Nombre Dueño");
        AgregarPlaceholder(txtDuenoTel, "Teléfono");
        AgregarPlaceholder(txtDespProd, "Desparasitación Producto");
        AgregarPlaceholder(txtDespFecha, "Fecha Desp.");
        AgregarPlaceholder(txtVacuna, "Vacuna");
        AgregarPlaceholder(txtVacunaFecha, "Fecha Vac.");
        AgregarPlaceholder(txtHistRepro, "Historia Reproductiva");
        AgregarPlaceholder(txtEnfPrev, "Enfermedades Anteriores");
        AgregarPlaceholder(txtAlimentacion, "Alimentación");
        AgregarPlaceholder(txtTratPrev, "Tratamientos Previos");
        AgregarPlaceholder(txtMotivo, "Motivo de Consulta");
        AgregarPlaceholder(txtFResp, "F. Respiratoria");
        AgregarPlaceholder(txtFCard, "F. Cardíaca");
        AgregarPlaceholder(txtTemp, "Temperatura");
        AgregarPlaceholder(txtLlenado, "Llenado capilar");
        AgregarPlaceholder(txtGanglios, "Ganglios linfáticos");
        AgregarPlaceholder(txtDiag, "Diagnóstico presuntivo");
        AgregarPlaceholder(txtTrat, "Tratamiento");
        AgregarPlaceholder(txtExam, "Exámenes complementarios");

        var btn = new Button
        {
            Text = "Generar PDF",
            Top = y,
            Left = 20,
            Width = 400,
            BackColor = Color.LightGreen
        };

        btn.Click += (s, e) =>
        {
            string V(TextBox tb) => VeterinariaPDF.Helpers.Valor(tb);

            var nombreArchivoBase = string.IsNullOrWhiteSpace(V(txtPacNombre)) ? "Historia.pdf" : $"Historia_{V(txtPacNombre)}.pdf";
            var ruta = VeterinariaPDF.Helpers.ElegirRutaGuardar(nombreArchivoBase);
            if (string.IsNullOrEmpty(ruta)) return;
            
            PDF.CrearHistoriaClinica(
                // Paciente
                V(txtPacNombre), V(txtPacEspecie), V(txtPacRaza), V(txtPacSexo), V(txtPacEdad), V(txtPacColor), V(txtPacPeso),
                // Dueño
                V(txtDuenoNombre), V(txtDuenoTel),
                // Anamnesis
                V(txtDespProd), V(txtDespFecha), V(txtVacuna), V(txtVacunaFecha), V(txtHistRepro), V(txtEnfPrev), V(txtAlimentacion), V(txtTratPrev),
                // Motivo
                V(txtMotivo),
                // Examen
                V(txtFResp), V(txtFCard), V(txtTemp), V(txtLlenado), V(txtGanglios),
                V(txtDiag), V(txtTrat), V(txtExam),
                ruta
            );
        };

        form.Controls.Add(btn);
    }

    private void AgregarPlaceholder(TextBox txt, string placeholder) => VeterinariaPDF.Helpers.AgregarPlaceholder(txt, placeholder);
}
