using System;
using System.Diagnostics;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

public static class PDF
{
    public static void Crear(string titulo, string contenido, string archivo)
    {
        PdfDocument pdf = new PdfDocument();
        PdfPage pagina = pdf.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        XFont fuente = new XFont("Arial", 13);
        XFont fuenteTitulo = new XFont("Arial", 20, XFontStyle.Bold);
        XFont fuentePie = new XFont("Arial", 10);

        double margen = 20;
        double y = margen;
        DateTime fecha = DateTime.Now;

        DibujarEncabezado(gfx, pagina, fuenteTitulo, titulo, margen, ref y);

        double lineHeight = gfx.MeasureString("X", fuente).Height + 1;
        double maxY = pagina.Height - margen;
        double colGap = 12;
        double colWidth = (pagina.Width - 2 * margen - colGap) / 2;
        int colIndex = 0;
        foreach (var linea in contenido.Split('\n'))
        {
            bool esCampo = linea.Contains(":");
            if (esCampo)
            {
                if (y + lineHeight > maxY)
                {
                    pagina = pdf.AddPage();
                    gfx = XGraphics.FromPdfPage(pagina);
                    y = margen;
                    DibujarEncabezado(gfx, pagina, fuenteTitulo, titulo, margen, ref y);
                }
                double x = colIndex % 2 == 0 ? margen : margen + colWidth + colGap;
                gfx.DrawString(linea, fuente, XBrushes.Black, new XRect(x, y, colWidth, lineHeight), XStringFormats.TopLeft);
                if (colIndex % 2 == 1)
                {
                    y += lineHeight;
                }
                colIndex++;
            }
            else
            {
                if (colIndex % 2 == 1)
                {
                    y += lineHeight;
                    colIndex++;
                }
                if (y + lineHeight > maxY)
                {
                    pagina = pdf.AddPage();
                    gfx = XGraphics.FromPdfPage(pagina);
                    y = margen;
                    DibujarEncabezado(gfx, pagina, fuenteTitulo, titulo, margen, ref y);
                    gfx.DrawRectangle(XPens.LightGray, margen, y, pagina.Width - 2 * margen, pagina.Height - margen - y);
                }
                gfx.DrawString(linea, fuente, XBrushes.Black, new XRect(margen, y, pagina.Width - 2 * margen, lineHeight), XStringFormats.TopLeft);
                y += lineHeight;
            }
        }

        string ruta;
        if (!string.IsNullOrWhiteSpace(archivo) && (Path.IsPathRooted(archivo) || archivo.Contains(Path.DirectorySeparatorChar.ToString()) || archivo.Contains(Path.AltDirectorySeparatorChar.ToString())))
        {
            ruta = AsegurarExtensionPdf(archivo);
        }
        else
        {
            string nombreArchivo = AsegurarExtensionPdf(SanitizarNombreArchivo(archivo));
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombreArchivo);
        }

        try
        {
            for (int i = 0; i < pdf.PageCount; i++)
            {
                var p = pdf.Pages[i];
                using (var g = XGraphics.FromPdfPage(p))
                {
                    double yPie = p.Height - margen + 4;
                    g.DrawLine(XPens.Gray, margen, p.Height - margen - 8, p.Width - margen, p.Height - margen - 8);
                    g.DrawString($"Fecha: {fecha:dd/MM/yyyy HH:mm}", fuentePie, XBrushes.Gray, new XRect(margen, yPie, (p.Width - 2 * margen) / 2, 12), XStringFormats.TopLeft);
                    g.DrawString($"Página {i + 1} de {pdf.PageCount}", fuentePie, XBrushes.Gray, new XRect(margen + (p.Width - 2 * margen) / 2, yPie, (p.Width - 2 * margen) / 2, 12), XStringFormats.TopRight);
                }
            }
        }
        catch { }

        try
        {
            pdf.Save(ruta);
        }
        catch { }

        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch { }
    }

    public static void CrearConsentimiento(
        string paciente,
        string edad,
        string raza,
        string dueno,
        string tel,
        string cel,
        bool generoM,
        bool generoH,
        bool esterilizacion,
        string otro,
        DateTime fechaDoc,
        string archivo)
    {
        PdfDocument pdf = new PdfDocument();
        PdfPage pagina = pdf.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        XFont fuente = new XFont("Arial", 10);
        XFont fuenteBold = new XFont("Arial", 10, XFontStyle.Bold);
        XFont fuenteTitulo = new XFont("Arial", 12, XFontStyle.Bold);
        XFont fuentePie = new XFont("Arial", 9);

        double margen = 40;
        double y = margen;
        double ancho = pagina.Width - 2 * margen;

        // --- HEADER ---
        // Logo Left
        double logoHeight = 0;
        try
        {
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            string logoPath = BuscarLogoEnCarpeta(projectDir);
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                using (XImage logo = XImage.FromFile(logoPath))
                {
                    double logoWidth = 150;
                    double scale = logoWidth / logo.PixelWidth;
                    logoHeight = logo.PixelHeight * scale;
                    gfx.DrawImage(logo, margen, y, logoWidth, logoHeight);
                }
            }
        }
        catch { }

        // Title and checkboxes (centered/right area)
        double xCenter = pagina.Width / 2 - 100;
        double yTitle = y + 20;

        gfx.DrawString("CARTA DE CONSENTIMIENTO PARA CIRUGIA", fuenteTitulo, XBrushes.Black,
            new XRect(xCenter, yTitle, 300, 18), XStringFormats.TopLeft);
        yTitle += 30;

        // ESTERILIZACION checkbox
        double xCheck = xCenter + 50;
        gfx.DrawString("ESTERILIZACION", fuente, XBrushes.Black, new XRect(xCheck, yTitle, 100, 18), XStringFormats.TopLeft);
        double checkboxSize = 22;
        gfx.DrawRectangle(XPens.Black, xCheck + 120, yTitle - 2, checkboxSize, checkboxSize);
        if (esterilizacion)
        {
            gfx.DrawLine(XPens.Black, xCheck + 120, yTitle - 2, xCheck + 120 + checkboxSize, yTitle - 2 + checkboxSize);
            gfx.DrawLine(XPens.Black, xCheck + 120 + checkboxSize, yTitle - 2, xCheck + 120, yTitle - 2 + checkboxSize);
        }
        yTitle += 30;

        // OTRO checkbox
        gfx.DrawString("OTRO", fuente, XBrushes.Black, new XRect(xCheck, yTitle, 100, 18), XStringFormats.TopLeft);
        gfx.DrawRectangle(XPens.Black, xCheck + 120, yTitle - 2, checkboxSize, checkboxSize);
        if (!string.IsNullOrWhiteSpace(otro))
        {
            gfx.DrawLine(XPens.Black, xCheck + 120, yTitle - 2, xCheck + 120 + checkboxSize, yTitle - 2 + checkboxSize);
            gfx.DrawLine(XPens.Black, xCheck + 120 + checkboxSize, yTitle - 2, xCheck + 120, yTitle - 2 + checkboxSize);
        }
        gfx.DrawLine(XPens.Black, xCheck + 150, yTitle + 18, pagina.Width - margen, yTitle + 18);
        if (!string.IsNullOrWhiteSpace(otro))
        {
            gfx.DrawString(otro, fuenteBold, XBrushes.Black, new XRect(xCheck + 155, yTitle, pagina.Width - margen - xCheck - 160, 18), XStringFormats.TopLeft);
        }

        y = Math.Max(y + logoHeight + 20, yTitle + 40);

        // --- PATIENT INFO ---

        // Nombre del Paciente + M/H
        gfx.DrawString("NOMBRE DEL PACIENTE", fuente, XBrushes.Black, new XRect(margen, y, 150, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 150, y + 14, margen + 360, y + 14);
        gfx.DrawString(paciente ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 155, y - 2, 200, 18), XStringFormats.TopLeft);

        double xSex = margen + 380;
        gfx.DrawString("M", fuente, XBrushes.Black, new XRect(xSex, y, 20, 18), XStringFormats.TopLeft);
        gfx.DrawRectangle(XPens.Black, xSex + 20, y - 2, checkboxSize, checkboxSize);
        if (generoM)
        {
            gfx.DrawLine(XPens.Black, xSex + 20, y - 2, xSex + 20 + checkboxSize, y - 2 + checkboxSize);
            gfx.DrawLine(XPens.Black, xSex + 20 + checkboxSize, y - 2, xSex + 20, y - 2 + checkboxSize);
        }

        double xSexH = xSex + 60;
        gfx.DrawString("H", fuente, XBrushes.Black, new XRect(xSexH, y, 20, 18), XStringFormats.TopLeft);
        gfx.DrawRectangle(XPens.Black, xSexH + 20, y - 2, checkboxSize, checkboxSize);
        if (generoH)
        {
            gfx.DrawLine(XPens.Black, xSexH + 20, y - 2, xSexH + 20 + checkboxSize, y - 2 + checkboxSize);
            gfx.DrawLine(XPens.Black, xSexH + 20 + checkboxSize, y - 2, xSexH + 20, y - 2 + checkboxSize);
        }
        y += 30;

        // Edad + Raza
        gfx.DrawString("EDAD", fuente, XBrushes.Black, new XRect(margen, y, 50, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 40, y + 14, margen + 200, y + 14);
        gfx.DrawString(edad ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 45, y - 2, 150, 18), XStringFormats.TopLeft);

        gfx.DrawString("RAZA", fuente, XBrushes.Black, new XRect(margen + 220, y, 50, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 270, y + 14, pagina.Width - margen, y + 14);
        gfx.DrawString(raza ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 275, y - 2, 250, 18), XStringFormats.TopLeft);
        y += 30;

        // Nombre del Dueño
        gfx.DrawString("NOMBRE DEL DUEÑO", fuente, XBrushes.Black, new XRect(margen, y, 150, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 150, y + 14, pagina.Width - margen, y + 14);
        gfx.DrawString(dueno ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 155, y - 2, 360, 18), XStringFormats.TopLeft);
        y += 30;

        // Tel + Cel
        gfx.DrawString("TEL", fuente, XBrushes.Black, new XRect(margen, y, 40, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 40, y + 14, margen + 240, y + 14);
        gfx.DrawString(tel ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 45, y - 2, 190, 18), XStringFormats.TopLeft);

        gfx.DrawString("CEL (871)", fuente, XBrushes.Black, new XRect(margen + 260, y, 80, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 340, y + 14, pagina.Width - margen, y + 14);
        gfx.DrawString(cel ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 345, y - 2, 180, 18), XStringFormats.TopLeft);
        y += 40;

        // --- BODY ---
        XTextFormatter tf = new XTextFormatter(gfx);
        string cuerpo =
            "Autorizo a los médicos veterinarios de la Veterinaria Corazón Canino a realizar a mi mascota los procedimientos y cirugías necesarias, doy esta autorización bajo la comprensión de que cualquier operación o procedimiento implica algunos riesgos, los más comunes incluyen: infección, hemorragia, lesión nerviosa, coágulos sanguíneos, ataque cardiaco, reacciones alérgicas, neumonía, entre otros. Estos riesgos pueden ser graves como mortales." +
            "\n\n" +
            "La administración de anestésicos también implica riesgos, el más importante de estos, aunque poco frecuente es el riesgo a sufrir alguna reacción a los medicamentos que puede causar la muerte, autorizo el uso de estos anestésicos a la persona responsable de este servicio." +
            "\n\n" +
            "Procedimientos adicionales: si el médico selecciona uno diferente por alguna situación no sospechada en el momento de la operación lo autorizo a realizarlo si lo considera necesario." +
            "\n\n" +
            "Nadie puede predecir cuáles serán las complicaciones que ocurran en cada caso." +
            "\n\n" +
            "Tengo que leer y entender esta forma de consentimiento y comprendo que al firmarla estoy de acuerdo con todos los párrafos y todas mis dudas han sido aclaradas a mi entera satisfacción y entiendo cualquier término o palabra contenida en este documento.";

        double altoCuerpo = 250;
        tf.DrawString(cuerpo, fuente, XBrushes.Black, new XRect(margen, y, ancho, altoCuerpo), XStringFormats.TopLeft);
        y += altoCuerpo + 10;

        // Note
        XFont fuenteBoldSmall = new XFont("Arial", 9, XFontStyle.Bold);
        string nota = "Declaro bajo verdad que mi mascota viene con 12 horas de ayuno agua y alimentos y (en caso de ser hembras) que han pasado 25 días después del sangrado de celo.";
        tf.DrawString(nota, fuenteBoldSmall, XBrushes.Black, new XRect(margen, y, ancho, 40), XStringFormats.TopLeft);
        y += 60;

        // --- SIGNATURES ---
        double lineaW = 200;

        // Row 1: Nombre del Dueño / Firma
        double ySig1 = y;
        gfx.DrawLine(XPens.Black, margen, ySig1, margen + lineaW, ySig1);
        gfx.DrawLine(XPens.Black, margen + ancho - lineaW, ySig1, margen + ancho, ySig1);

        gfx.DrawString("NOMBRE DEL DUEÑO", fuente, XBrushes.Black, new XRect(margen, ySig1 + 5, lineaW, 18), XStringFormats.Center);
        gfx.DrawString("FIRMA", fuente, XBrushes.Black, new XRect(margen + ancho - lineaW, ySig1 + 5, lineaW, 18), XStringFormats.Center);

        y += 60;

        // Row 2: Fecha / Firma MVZ
        double ySig2 = y;
        gfx.DrawLine(XPens.Black, margen, ySig2, margen + lineaW, ySig2);
        gfx.DrawLine(XPens.Black, margen + ancho - lineaW, ySig2, margen + ancho, ySig2);

        gfx.DrawString("FECHA", fuente, XBrushes.Black, new XRect(margen, ySig2 + 5, lineaW, 18), XStringFormats.Center);
        gfx.DrawString("FIRMA MVZ", fuente, XBrushes.Black, new XRect(margen + ancho - lineaW, ySig2 + 5, lineaW, 18), XStringFormats.Center);

        string ruta;
        if (!string.IsNullOrWhiteSpace(archivo) && (Path.IsPathRooted(archivo) || archivo.Contains(Path.DirectorySeparatorChar.ToString()) || archivo.Contains(Path.AltDirectorySeparatorChar.ToString())))
        {
            ruta = AsegurarExtensionPdf(archivo);
        }
        else
        {
            string nombreArchivo = AsegurarExtensionPdf(SanitizarNombreArchivo(archivo));
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombreArchivo);
        }

        try
        {
            for (int i = 0; i < pdf.PageCount; i++)
            {
                var p = pdf.Pages[i];
                using (var g = XGraphics.FromPdfPage(p))
                {
                    double yPie = p.Height - margen + 4;
                    g.DrawLine(XPens.Gray, margen, p.Height - margen - 8, p.Width - margen, p.Height - margen - 8);
                    g.DrawString($"Fecha: {fechaDoc:dd/MM/yyyy}", fuentePie, XBrushes.Gray, new XRect(margen, yPie, (p.Width - 2 * margen) / 2, 12), XStringFormats.TopLeft);
                    g.DrawString($"Página {i + 1} de {pdf.PageCount}", fuentePie, XBrushes.Gray, new XRect(margen + (p.Width - 2 * margen) / 2, yPie, (p.Width - 2 * margen) / 2, 12), XStringFormats.TopRight);
                }
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al agregar pie de página: {ex.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }

        try
        {
            pdf.Save(ruta);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al guardar el PDF en:\n{ruta}\n\nError: {ex.Message}", "Error al Guardar", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"PDF guardado en:\n{ruta}\n\nPero no se pudo abrir automáticamente.\nError: {ex.Message}", "PDF Guardado", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }

    public static void CrearAltaHospitalaria(
        string propietario,
        string clave,
        string mascota,
        string especie,
        string raza,
        string dx,
        string observaciones,
        DateTime fecha,
        DateTime hora,
        string retiroDe,
        string nombreApellido,
        string archivo)
    {
        PdfDocument pdf = new PdfDocument();
        PdfPage pagina = pdf.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        XFont fuente = new XFont("Arial", 11);
        XFont fuenteBold = new XFont("Arial", 11, XFontStyle.Bold);
        XFont fuenteTitulo = new XFont("Arial", 18, XFontStyle.Bold);
        XFont fuentePie = new XFont("Arial", 10);

        double margen = 20;
        double y = margen;
        DibujarEncabezado(gfx, pagina, fuenteTitulo, "ALTA HOSPITALARIA", margen, ref y);

        double ancho = pagina.Width - 2 * margen;
        double hLabel = gfx.MeasureString("X", fuente).Height;
        double hText = gfx.MeasureString("X", fuenteBold).Height;
        double gap = 8;

        double xDerecha = margen + ancho - 180;
        gfx.DrawString("Fecha", fuente, XBrushes.Black, new XRect(xDerecha, y - 10, 80, 18), XStringFormats.TopLeft);
        gfx.DrawString("Hora", fuente, XBrushes.Black, new XRect(xDerecha + 90, y - 10, 80, 18), XStringFormats.TopLeft);
        double yTopLines = y + hLabel;
        gfx.DrawString(fecha.ToString("dd/MM/yyyy"), fuenteBold, XBrushes.Black, new XRect(xDerecha, yTopLines - hText, 80, 18), XStringFormats.TopLeft);
        gfx.DrawString(hora.ToString("HH:mm"), fuenteBold, XBrushes.Black, new XRect(xDerecha + 90, yTopLines - hText, 80, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, xDerecha, yTopLines, xDerecha + 80, yTopLines);
        gfx.DrawLine(XPens.Black, xDerecha + 90, yTopLines, xDerecha + 170, yTopLines);
        y += 18;

        gfx.DrawString("NOMBRE PROPIETARIO:", fuente, XBrushes.Black, new XRect(margen, y, 200, 18), XStringFormats.TopLeft);
        gfx.DrawString("CLAVE:", fuente, XBrushes.Black, new XRect(margen + 260, y, 60, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(propietario ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 240, 18), XStringFormats.TopLeft);
        gfx.DrawString(clave ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 320, y, 100, 18), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 250, y);
        gfx.DrawLine(XPens.Black, margen + 320, y, margen + 420, y);
        y += gap;

        gfx.DrawString("NOMBRE DE LA MASCOTA:", fuente, XBrushes.Black, new XRect(margen, y, 220, 18), XStringFormats.TopLeft);
        gfx.DrawString("ESPECIE:", fuente, XBrushes.Black, new XRect(margen + 260, y, 80, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(mascota ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 240, 18), XStringFormats.TopLeft);
        gfx.DrawString(especie ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 320, y, 100, 18), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 250, y);
        gfx.DrawLine(XPens.Black, margen + 320, y, margen + 420, y);
        y += gap;

        gfx.DrawString("RAZA:", fuente, XBrushes.Black, new XRect(margen, y, 80, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(raza ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, ancho - 100, 18), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + ancho - 100, y);
        y += gap;

        gfx.DrawString("DX:", fuente, XBrushes.Black, new XRect(margen, y, 60, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(dx ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, ancho - 100, 18), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + ancho - 100, y);
        y += gap;

        gfx.DrawString("OBSERVACIONES DEL MEDICO:", fuente, XBrushes.Black, new XRect(margen, y, 240, 18), XStringFormats.TopLeft);
        y += hLabel;
        double altoObs = 90;
        XTextFormatter tf = new XTextFormatter(gfx);
        tf.DrawString(observaciones ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, ancho - 40, altoObs - 12), XStringFormats.TopLeft);
        gfx.DrawRectangle(XPens.Black, margen, y - 2, ancho - 40, altoObs);
        y += altoObs + gap;

        gfx.DrawLine(XPens.Black, margen, y, pagina.Width - margen, y);
        y += gap;

        string par1 = "DECLARO BAJO JURAMENTO QUE :";
        string par2 = "HE SIDO INFORMADO SOBRE LOS RIESGOS QUE IMPLICAN EL RETIRO ANTICIPADO DEL AREA DE HOSPITALIZACION DE MI ANIMAL SIN LA AUTORIZACION DE ALTA RESPECTIVA.";
        string par3 = "HE REALIZADO LAS PREGUNTAS SUFICIENTES PARA INFORMARME AL RESPECTO DE LA SALUD DE MI MASCOTA.";
        string par4 = "SOY CONSCIENTE DE QUE EXISTE UN NIVEL DE RIESGO SOBRE LA SALUD DE MI ANIMAL POR LA ACCION QUE REALIZO.";
        tf.DrawString(par1 + "\n\n" + par2 + "\n" + par3 + "\n" + par4, fuente, XBrushes.Black, new XRect(margen, y, ancho, 120), XStringFormats.TopLeft);
        y += 120;

        string frase = "ASI PUES DE FORMA VOLUNTARIA, SOLICITO EL RETIRO ANTICIPADO DE";
        tf.DrawString(frase, fuente, XBrushes.Black, new XRect(margen, y, 400, 20), XStringFormats.TopLeft);
        double xBlank = margen + gfx.MeasureString(frase, fuente).Width + 8;
        double wBlank = 180;
        gfx.DrawLine(XPens.Black, xBlank, y + hLabel, xBlank + wBlank, y + hLabel);
        tf.DrawString(retiroDe ?? "", fuenteBold, XBrushes.Black, new XRect(xBlank + 2, y, wBlank - 4, 18), XStringFormats.TopLeft);
        string frase2 = "DEL SERVICIO DE HOSPITALIZACION Y ASUMO LA ENTERA RESPONSABILIDAD DE LAS CONSECUENCIAS SOBRE LA SALUD DE MI MASCOTA.";
        tf.DrawString(frase2, fuente, XBrushes.Black, new XRect(margen, y + 22, ancho, 40), XStringFormats.TopLeft);
        y += 60;

        gfx.DrawString("NOMBRE Y APELLIDO:", fuente, XBrushes.Black, new XRect(margen, y, 160, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(nombreApellido ?? propietario ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 260, hText), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 260, y);
        y += gap + 12;

        gfx.DrawString("FIRMA:", fuente, XBrushes.Black, new XRect(margen, y, 60, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 60, y + hLabel, margen + 300, y + hLabel);
        y += gap + 48;

        gfx.DrawString("FIRMA DEL MEDICO:", fuente, XBrushes.Black, new XRect(margen, y, 160, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 160, y + hLabel, margen + 400, y + hLabel);

        string ruta;
        if (!string.IsNullOrWhiteSpace(archivo) && (Path.IsPathRooted(archivo) || archivo.Contains(Path.DirectorySeparatorChar.ToString()) || archivo.Contains(Path.AltDirectorySeparatorChar.ToString())))
        {
            ruta = AsegurarExtensionPdf(archivo);
        }
        else
        {
            string nombreArchivo = AsegurarExtensionPdf(SanitizarNombreArchivo(archivo));
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombreArchivo);
        }

        try
        {
            for (int i = 0; i < pdf.PageCount; i++)
            {
                var p = pdf.Pages[i];
                using (var g = XGraphics.FromPdfPage(p))
                {
                    double yPie = p.Height - margen + 4;
                    g.DrawLine(XPens.Gray, margen, p.Height - margen - 8, p.Width - margen, p.Height - margen - 8);
                    g.DrawString($"Fecha: {fecha:dd/MM/yyyy} {hora:HH:mm}", fuentePie, XBrushes.Gray, new XRect(margen, yPie, (p.Width - 2 * margen) / 2, 12), XStringFormats.TopLeft);
                    g.DrawString($"Página {i + 1} de {pdf.PageCount}", fuentePie, XBrushes.Gray, new XRect(margen + (p.Width - 2 * margen) / 2, yPie, (p.Width - 2 * margen) / 2, 12), XStringFormats.TopRight);
                }
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al agregar pie de página: {ex.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }

        try
        {
            pdf.Save(ruta);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al guardar el PDF en:\n{ruta}\n\nError: {ex.Message}", "Error al Guardar", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"PDF guardado en:\n{ruta}\n\nPero no se pudo abrir automáticamente.\nError: {ex.Message}", "PDF Guardado", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }

    public static void CrearRecomendaciones(string mascota, string cirugia, string archivo)
    {
        PdfDocument pdf = new PdfDocument();
        PdfPage pagina = pdf.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        XFont fuente = new XFont("Arial", 11);
        XFont fuenteBold = new XFont("Arial", 11, XFontStyle.Bold);
        XFont fuenteTitulo = new XFont("Arial", 16, XFontStyle.Bold);

        double margen = 20;
        double ancho = pagina.Width - 2 * margen;
        double y = margen + 10;

        Action drawBlock = () =>
        {
            var tituloRect = new XRect(margen, y, ancho, 22);
            gfx.DrawString("RECOMENDACIONES POSTOPERATORIAS", fuenteTitulo, XBrushes.Black, tituloRect, XStringFormats.TopCenter);
            y += 30;

            string[] items = {
                "Reposo obligatorio 10 días.",
                "Cumplir con la medicación indicada.",
                "Aplicar cicatrizante una vez al día por 10 días. (si usa cono isabelino no es necesario el cicatrizante).",
                "Tener a la mascota en un lugar seco, limpio y sin tierra.",
                "Se recomienda el uso del collar isabelino, aunque es opcional, es indispensable para que su mascota no lama o se quite los puntos.",
                "Retiro de puntos 10 días después de la cirugía. (Solo traerlo, no es necesario cita)",
                "En el caso de los gatos el proceso de la recuperación de la anestesia puede ser más prolongada que en otras especies.",
                "NO lavar la herida."
            };

            for (int i = 0; i < items.Length; i++)
            {
                gfx.DrawString((i + 1) + ".", fuente, XBrushes.Black, new XRect(margen, y, 20, 16), XStringFormats.TopLeft);
                XTextFormatter tf = new XTextFormatter(gfx);
                tf.DrawString(items[i], fuente, XBrushes.Black, new XRect(margen + 22, y, ancho - 22, 20), XStringFormats.TopLeft);
                y += 22;
            }

            string yo = "Yo";
            double yoW = gfx.MeasureString(yo, fuente).Width;
            gfx.DrawString(yo, fuente, XBrushes.Black, new XRect(margen, y, yoW, 18), XStringFormats.TopLeft);
            double xLineaYo = margen + yoW + 8;
            double wLineaYo = 220;
            gfx.DrawLine(XPens.Black, xLineaYo, y + 16, xLineaYo + wLineaYo, y + 16);
            string resto = "entiendo que si no cumplo con las indicaciones mi mascota pudiera tener problemas postoperatorios como inflamación, hernias, infección, hemorragias, pudiendo requerir medicamento, restauración, lo que conlleva al uso de anestésicos.";
            XTextFormatter tfR = new XTextFormatter(gfx);
            tfR.DrawString(resto, fuente, XBrushes.Black, new XRect(margen, y + 24, ancho, 40), XStringFormats.TopLeft);
            y += 70;

            gfx.DrawString("COLLAR ISABELINO", fuente, XBrushes.Black, new XRect(margen, y, 140, 18), XStringFormats.TopLeft);
            gfx.DrawLine(XPens.Black, margen + 150, y + 16, margen + 400, y + 16);
            y += 30;

            gfx.DrawString("He leído y acepto las indicaciones", fuenteBold, XBrushes.Black, new XRect(margen, y, ancho, 18), XStringFormats.TopLeft);
            y += 40;

            gfx.DrawString("FIRMA", fuente, XBrushes.Black, new XRect(margen, y, 60, 18), XStringFormats.TopLeft);
            gfx.DrawLine(XPens.Black, margen + 60, y + 16, margen + 300, y + 16);
            gfx.DrawString("FECHA", fuente, XBrushes.Black, new XRect(margen + 340, y, 60, 18), XStringFormats.TopLeft);
            gfx.DrawLine(XPens.Black, margen + 400, y + 16, margen + 520, y + 16);
            y += 60;
        };

        drawBlock();

        string ruta;
        if (!string.IsNullOrWhiteSpace(archivo) && (Path.IsPathRooted(archivo) || archivo.Contains(Path.DirectorySeparatorChar.ToString()) || archivo.Contains(Path.AltDirectorySeparatorChar.ToString())))
        {
            ruta = AsegurarExtensionPdf(archivo);
        }
        else
        {
            string nombreArchivo = AsegurarExtensionPdf(SanitizarNombreArchivo(archivo));
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombreArchivo);
        }

        try
        {
            pdf.Save(ruta);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al guardar el PDF en:\n{ruta}\n\nError: {ex.Message}", "Error al Guardar", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"PDF guardado en:\n{ruta}\n\nPero no se pudo abrir automáticamente.\nError: {ex.Message}", "PDF Guardado", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }

    public static void CrearCartaEntrega(
        DateTime fecha,
        string raza,
        string tamano,
        string edad,
        string caracteristicas,
        string nombreFirmante,
        string telefono,
        string archivo)
    {
        PdfDocument pdf = new PdfDocument();
        PdfPage pagina = pdf.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(pagina);
        XFont fuente = new XFont("Arial", 11);
        XFont fuenteBold = new XFont("Arial", 11, XFontStyle.Bold);
        XFont fuenteTitulo = new XFont("Arial", 16, XFontStyle.Bold);

        double margen = 20;
        double ancho = pagina.Width - 2 * margen;
        double y = margen + 10;

        // Title
        gfx.DrawString("INGRESO DE ALBERGUE", fuenteTitulo, XBrushes.Black, new XRect(0, y, pagina.Width, 20), XStringFormats.TopCenter);
        y += 40;

        gfx.DrawString("Fecha", fuente, XBrushes.Black, new XRect(margen + ancho - 160, y, 60, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + ancho - 100, y + 16, margen + ancho, y + 16);
        gfx.DrawString(fecha.ToString("dd/MM/yyyy"), fuenteBold, XBrushes.Black, new XRect(margen + ancho - 98, y - 2, 94, 18), XStringFormats.TopLeft);
        y += 40;

        gfx.DrawString("Albergue Corazón Canino AC", fuente, XBrushes.Black, new XRect(margen, y, ancho, 18), XStringFormats.TopLeft);
        y += 24;
        gfx.DrawString("Atn Martha Raquel Téllez Girón y Garza", fuente, XBrushes.Black, new XRect(margen, y, ancho, 18), XStringFormats.TopLeft);
        y += 36;

        XTextFormatter tf = new XTextFormatter(gfx);
        string intro = "Por medio de la presente me deslindo completamente de la mascota que estoy entregando en este albergue:";
        tf.DrawString(intro, fuente, XBrushes.Black, new XRect(margen, y, ancho, 40), XStringFormats.TopLeft);
        y += 40;

        double hLabel = gfx.MeasureString("X", fuente).Height;
        double hText = gfx.MeasureString("X", fuenteBold).Height;
        double gap = 10;

        gfx.DrawString("Raza", fuente, XBrushes.Black, new XRect(margen, y, 60, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(raza ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 236, hText), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 300, y);
        y += gap;

        gfx.DrawString("Tamaño", fuente, XBrushes.Black, new XRect(margen, y, 60, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(tamano ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 236, hText), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 300, y);
        y += gap;

        gfx.DrawString("Edad", fuente, XBrushes.Black, new XRect(margen, y, 60, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(edad ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 236, hText), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 300, y);
        y += gap + 6;

        gfx.DrawString("Características especiales", fuente, XBrushes.Black, new XRect(margen, y, 200, 18), XStringFormats.TopLeft);
        double y1 = y + hLabel + 6;
        double y2 = y1 + 26;
        gfx.DrawLine(XPens.Black, margen, y1, margen + ancho - 40, y1);
        gfx.DrawLine(XPens.Black, margen, y2, margen + ancho - 40, y2);
        tf.DrawString(caracteristicas ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y1 + 2, ancho - 44, (y2 - y1) - 4), XStringFormats.TopLeft);
        y = y2 + 24;

        string clausula = "Entiendo que, al entregarla aquí, en las instalaciones del albergue en Blvd Constitución 2175 ote. Palmas San Isidro, la responsabilidad de la mascota es completamente del albergue, no recibiré ninguna información de los adoptantes por respeto a ellos y su confidencialidad, así como el albergue no tiene porque darme ninguna otra información respecto a esta mascota.";
        tf.DrawString(clausula, fuente, XBrushes.Black, new XRect(margen, y, ancho, 80), XStringFormats.TopLeft);
        y += 90;

        gfx.DrawString("NOMBRE:", fuente, XBrushes.Black, new XRect(margen, y, 80, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(nombreFirmante ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 320, hText), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 360, y);
        y += 24;

        gfx.DrawString("TELÉFONO:", fuente, XBrushes.Black, new XRect(margen, y, 80, 18), XStringFormats.TopLeft);
        y += hLabel;
        gfx.DrawString(telefono ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 2, y, 320, hText), XStringFormats.TopLeft);
        y += hText;
        gfx.DrawLine(XPens.Black, margen, y, margen + 360, y);
        y += 48;

        gfx.DrawString("FIRMA", fuente, XBrushes.Black, new XRect(margen + 160, y, 60, 18), XStringFormats.TopLeft);
        gfx.DrawLine(XPens.Black, margen + 120, y + 16, margen + 400, y + 16);

        string ruta;
        if (!string.IsNullOrWhiteSpace(archivo) && (Path.IsPathRooted(archivo) || archivo.Contains(Path.DirectorySeparatorChar.ToString()) || archivo.Contains(Path.AltDirectorySeparatorChar.ToString())))
        {
            ruta = AsegurarExtensionPdf(archivo);
        }
        else
        {
            string nombreArchivo = AsegurarExtensionPdf(SanitizarNombreArchivo(archivo));
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombreArchivo);
        }

        try
        {
            pdf.Save(ruta);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al guardar el PDF en:\n{ruta}\n\nError: {ex.Message}", "Error al Guardar", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"PDF guardado en:\n{ruta}\n\nPero no se pudo abrir automáticamente.\nError: {ex.Message}", "PDF Guardado", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }

    public static void CrearHistoriaClinica(
        string pacNombre, string pacEspecie, string pacRaza, string pacSexo, string pacEdad, string pacColor, string pacPeso,
        string duenoNombre, string duenoTel,
        string despProd, string despFecha, string vacuna, string vacunaFecha, string histRepro, string enfPrev, string alimentacion, string tratPrev,
        string motivo,
        string fRespiratoria, string fCardiaca, string temperatura, string llenadoCapilar, string ganglios,
        string diagnostico, string tratamiento, string examenes,
        string archivo)
    {
        PdfDocument pdf = new PdfDocument();
        XFont fuente = new XFont("Arial", 10);
        XFont fuenteBold = new XFont("Arial", 10, XFontStyle.Bold);
        XFont fuenteTitulo = new XFont("Arial", 14, XFontStyle.Bold);
        XFont fuenteSmall = new XFont("Arial", 8);

        double margen = 30;

        // --- PAGE 1 ---
        PdfPage page1 = pdf.AddPage();
        double ancho = page1.Width - 2 * margen;
        XGraphics gfx1 = XGraphics.FromPdfPage(page1);
        double y = margen;

        // Header Page 1
        DibujarEncabezadoHistoria(gfx1, page1, margen, ref y);

        // Title
        gfx1.DrawString("Historia Clínica", fuenteTitulo, XBrushes.Black, new XRect(0, y, page1.Width, 20), XStringFormats.TopCenter);

        // Fecha / No Historia (Top Right)
        double yDate = y;
        double xDate = page1.Width - margen - 150;
        gfx1.DrawString("Fecha", fuenteSmall, XBrushes.Black, new XRect(xDate, yDate, 40, 12), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xDate + 30, yDate + 10, xDate + 150, yDate + 10);
        gfx1.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), fuenteBold, XBrushes.Black, new XRect(xDate + 32, yDate - 2, 110, 12), XStringFormats.TopLeft);
        yDate += 16;
        gfx1.DrawString("Nº Historia clínica", fuenteSmall, XBrushes.Black, new XRect(xDate, yDate, 80, 12), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xDate + 80, yDate + 10, xDate + 150, yDate + 10);

        y += 40;

        // DATOS DEL PACIENTE
        double hBox = 16;

        gfx1.DrawRectangle(XPens.Black, margen, y, ancho, 80);
        gfx1.DrawString("DATOS DEL PACIENTE", fuenteBold, XBrushes.Black, new XRect(margen, y + 2, ancho, 16), XStringFormats.TopCenter);
        gfx1.DrawLine(XPens.Black, margen, y + 18, margen + ancho, y + 18);

        double yInner = y + 24;
        double xInner = margen + 6;

        // Row 1: Nombre
        gfx1.DrawString("Nombre:", fuente, XBrushes.Black, new XRect(xInner, yInner, 50, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 50, yInner + 12, xInner + 300, yInner + 12);
        gfx1.DrawString(pacNombre ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 52, yInner, 240, hBox), XStringFormats.TopLeft);
        yInner += 20;

        // Row 2: Edad, Sexo, Raza
        gfx1.DrawString("Edad", fuente, XBrushes.Black, new XRect(xInner, yInner, 30, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 30, yInner + 12, xInner + 100, yInner + 12);
        gfx1.DrawString(pacEdad ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 32, yInner, 60, hBox), XStringFormats.TopLeft);

        gfx1.DrawString("Sexo", fuente, XBrushes.Black, new XRect(xInner + 110, yInner, 30, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 140, yInner + 12, xInner + 200, yInner + 12);
        gfx1.DrawString(pacSexo ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 142, yInner, 50, hBox), XStringFormats.TopLeft);

        gfx1.DrawString("Raza", fuente, XBrushes.Black, new XRect(xInner + 210, yInner, 30, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 240, yInner + 12, xInner + 400, yInner + 12);
        gfx1.DrawString(pacRaza ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 242, yInner, 150, hBox), XStringFormats.TopLeft);
        yInner += 20;

        // Row 3: Especie, Color, Peso
        gfx1.DrawString("Especie", fuente, XBrushes.Black, new XRect(xInner, yInner, 50, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 50, yInner + 12, xInner + 150, yInner + 12);
        gfx1.DrawString(pacEspecie ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 52, yInner, 90, hBox), XStringFormats.TopLeft);

        gfx1.DrawString("Color", fuente, XBrushes.Black, new XRect(xInner + 160, yInner, 40, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 200, yInner + 12, xInner + 300, yInner + 12);
        gfx1.DrawString(pacColor ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 202, yInner, 90, hBox), XStringFormats.TopLeft);

        gfx1.DrawString("Peso", fuente, XBrushes.Black, new XRect(xInner + 310, yInner, 40, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 350, yInner + 12, xInner + 450, yInner + 12);
        gfx1.DrawString(pacPeso ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 352, yInner, 90, hBox), XStringFormats.TopLeft);

        y += 100;

        // DATOS DEL PROPIETARIO
        gfx1.DrawString("DATOS DEL PROPIETARIO", fuenteBold, XBrushes.Black, new XRect(margen, y, ancho, 16), XStringFormats.TopLeft);
        y += 20;
        gfx1.DrawString("Nombre:", fuente, XBrushes.Black, new XRect(margen, y, 50, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, margen + 50, y + 12, margen + 350, y + 12);
        gfx1.DrawString(duenoNombre ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 52, y, 290, hBox), XStringFormats.TopLeft);
        y += 24;
        gfx1.DrawString("Teléfono:", fuente, XBrushes.Black, new XRect(margen, y, 50, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, margen + 50, y + 12, margen + 200, y + 12);
        gfx1.DrawString(duenoTel ?? "", fuenteBold, XBrushes.Black, new XRect(margen + 52, y, 140, hBox), XStringFormats.TopLeft);
        y += 40;

        // ANAMNESIS
        gfx1.DrawRectangle(XPens.Black, margen, y, ancho, 180);
        gfx1.DrawString("ANAMNESIS", fuenteBold, XBrushes.Black, new XRect(margen, y + 2, ancho, 16), XStringFormats.TopCenter);
        gfx1.DrawLine(XPens.Black, margen, y + 18, margen + ancho, y + 18);

        yInner = y + 24;
        // Desparasitacion
        gfx1.DrawString("Desparasitación:", fuenteBold, XBrushes.Black, new XRect(xInner, yInner, 100, hBox), XStringFormats.TopLeft);
        yInner += 16;
        gfx1.DrawString("Producto", fuente, XBrushes.Black, new XRect(xInner, yInner, 50, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 50, yInner + 12, xInner + 250, yInner + 12);
        gfx1.DrawString(despProd ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 52, yInner, 190, hBox), XStringFormats.TopLeft);

        gfx1.DrawString("Fecha", fuente, XBrushes.Black, new XRect(xInner + 260, yInner, 40, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 300, yInner + 12, xInner + 400, yInner + 12);
        gfx1.DrawString(despFecha ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 302, yInner, 90, hBox), XStringFormats.TopLeft);
        yInner += 24;

        // Vacunas
        gfx1.DrawString("Vacunas:", fuenteBold, XBrushes.Black, new XRect(xInner, yInner, 100, hBox), XStringFormats.TopLeft);
        yInner += 16;
        gfx1.DrawString("Vacuna", fuente, XBrushes.Black, new XRect(xInner, yInner, 50, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 50, yInner + 12, xInner + 250, yInner + 12);
        gfx1.DrawString(vacuna ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 52, yInner, 190, hBox), XStringFormats.TopLeft);

        gfx1.DrawString("Fecha", fuente, XBrushes.Black, new XRect(xInner + 260, yInner, 40, hBox), XStringFormats.TopLeft);
        gfx1.DrawLine(XPens.Black, xInner + 300, yInner + 12, xInner + 400, yInner + 12);
        gfx1.DrawString(vacunaFecha ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + 302, yInner, 90, hBox), XStringFormats.TopLeft);
        yInner += 24;

        // Historia Repro / Alimentacion / Enf / Trat
        void LineaAnam(string label, string val, ref double curY)
        {
            gfx1.DrawString(label, fuente, XBrushes.Black, new XRect(xInner, curY, 150, hBox), XStringFormats.TopLeft);
            double wLabel = gfx1.MeasureString(label, fuente).Width;
            gfx1.DrawLine(XPens.Black, xInner + wLabel + 4, curY + 12, margen + ancho - 10, curY + 12);
            gfx1.DrawString(val ?? "", fuenteBold, XBrushes.Black, new XRect(xInner + wLabel + 6, curY, ancho - wLabel - 20, hBox), XStringFormats.TopLeft);
            curY += 18;
        }

        LineaAnam("Historia Reproductiva:", histRepro, ref yInner);
        LineaAnam("Enfermedades Anteriores:", enfPrev, ref yInner);
        LineaAnam("Alimentación:", alimentacion, ref yInner);
        LineaAnam("Tratamientos:", tratPrev, ref yInner);

        y += 200;

        // MOTIVO DE CONSULTA
        gfx1.DrawRectangle(XPens.Black, margen, y, ancho, 200);
        gfx1.DrawString("MOTIVO DE CONSULTA", fuenteBold, XBrushes.Black, new XRect(margen, y + 2, ancho, 16), XStringFormats.TopCenter);
        gfx1.DrawLine(XPens.Black, margen, y + 18, margen + ancho, y + 18);
        XTextFormatter tf = new XTextFormatter(gfx1);
        tf.Alignment = XParagraphAlignment.Left;
        XRect motivoRect = new XRect(margen + 6, y + 22, ancho - 12, 170);
        tf.LayoutRectangle = motivoRect;
        tf.DrawString(motivo ?? "", fuente, XBrushes.Black, motivoRect, XStringFormats.TopLeft);


        // --- PAGE 2 ---
        PdfPage page2 = pdf.AddPage();
        XGraphics gfx2 = XGraphics.FromPdfPage(page2);
        y = margen;

        DibujarEncabezadoHistoria(gfx2, page2, margen, ref y);

        // Create TextFormatter for page 2
        XTextFormatter tf2 = new XTextFormatter(gfx2);

        // EXAMEN CLINICO
        double yTitulo = y + 10;
        XRect boxTitulo = new XRect(margen, yTitulo, ancho, 20);
        gfx2.DrawRectangle(XPens.Black, boxTitulo);
        gfx2.DrawString("EXAMEN CLÍNICO", fuenteTitulo, XBrushes.Black, boxTitulo, XStringFormats.TopCenter);
        y = yTitulo + 30;

        // Vitals
        double x = margen;
        double h = 18;
        void Campo(string label, string valor, double w)
        {
            gfx2.DrawString(label, fuente, XBrushes.Black, new XRect(x, y, 100, h), XStringFormats.TopLeft);
            gfx2.DrawLine(XPens.Black, x + 90, y + h - 2, x + 90 + w, y + h - 2);
            if (!string.IsNullOrWhiteSpace(valor))
                gfx2.DrawString(valor, fuenteBold, XBrushes.Black, new XRect(x + 94, y - 2, w - 4, h), XStringFormats.TopLeft);
            x += 90 + w + 10;
        }

        x = margen;
        Campo("F. Respiratoria:", fRespiratoria, 60);
        Campo("F. Cardíaca:", fCardiaca, 60);
        Campo("Temperatura:", temperatura, 60);

        // New Row
        x = margen;
        y += 30;
        Campo("Llenado Capilar:", llenadoCapilar, 80);
        Campo("Ganglios Linfáticos:", ganglios, 80);
        y += 30;

        // Table
        double tablaTop = y;
        double tablaWidth = 450; // Adjusted width to fit content properly

        // Headers
        gfx2.DrawRectangle(XPens.Black, margen, tablaTop, tablaWidth, 20);
        gfx2.DrawString("Órganos y Sistemas", fuenteBold, XBrushes.Black, new XRect(margen + 4, tablaTop + 4, 120, 16), XStringFormats.TopLeft);
        gfx2.DrawString("N / AN / NE", fuenteBold, XBrushes.Black, new XRect(margen + 130, tablaTop + 4, 80, 16), XStringFormats.TopLeft);
        gfx2.DrawString("Órganos y Sistemas", fuenteBold, XBrushes.Black, new XRect(margen + 220, tablaTop + 4, 120, 16), XStringFormats.TopLeft);

        // Rows
        string[] leftRows = { "Condición Corporal", "Estado de hidratación", "Ojos", "Oídos", "Nariz", "Sistema digestivo" };
        string[] rightRows = { "S. Respiratorio", "S. Nervioso", "S. Musculoesquelético", "S. Cardiovascular", "S. Genitourinario" };

        double rowY = tablaTop + 20;
        double rowH = 20;

        for (int i = 0; i < Math.Max(leftRows.Length, rightRows.Length); i++)
        {
            gfx2.DrawRectangle(XPens.Black, margen, rowY, tablaWidth, rowH);

            // Left Col
            if (i < leftRows.Length)
            {
                gfx2.DrawString(leftRows[i], fuente, XBrushes.Black, new XRect(margen + 4, rowY + 4, 120, 16), XStringFormats.TopLeft);
                // Checkbox placeholders
                gfx2.DrawRectangle(XPens.Black, margen + 130, rowY + 4, 12, 12);
                gfx2.DrawRectangle(XPens.Black, margen + 150, rowY + 4, 12, 12);
                gfx2.DrawRectangle(XPens.Black, margen + 170, rowY + 4, 12, 12);
            }

            // Right Col
            if (i < rightRows.Length)
            {
                gfx2.DrawString(rightRows[i], fuente, XBrushes.Black, new XRect(margen + 220, rowY + 4, 120, 16), XStringFormats.TopLeft);
                // Checkbox placeholders
                gfx2.DrawRectangle(XPens.Black, margen + 350, rowY + 4, 12, 12);
            }
            rowY += rowH;
        }

        // Legend Box
        double legendX = margen + tablaWidth + 10;
        double legendW = ancho - tablaWidth - 10;
        gfx2.DrawRectangle(XPens.Black, legendX, tablaTop + 40, legendW, 80);
        gfx2.DrawString("N: Normal", fuenteSmall, XBrushes.Black, new XRect(legendX + 4, tablaTop + 44, legendW, 12), XStringFormats.TopLeft);
        gfx2.DrawString("AN: Anormal", fuenteSmall, XBrushes.Black, new XRect(legendX + 4, tablaTop + 60, legendW, 12), XStringFormats.TopLeft);
        gfx2.DrawString("NE: No Examinado", fuenteSmall, XBrushes.Black, new XRect(legendX + 4, tablaTop + 76, legendW, 12), XStringFormats.TopLeft);

        y = rowY + 20;

        // Diagnosis
        gfx2.DrawRectangle(XPens.Black, margen, y, ancho, 100);
        gfx2.DrawString("Diagnostico Presuntivo:", fuenteBold, XBrushes.Black, new XRect(margen + 4, y + 4, 200, 16), XStringFormats.TopLeft);
        if (!string.IsNullOrWhiteSpace(diagnostico))
            tf2.DrawString(diagnostico, fuente, XBrushes.Black, new XRect(margen + 6, y + 24, ancho - 12, 70), XStringFormats.TopLeft);
        y += 110;

        // Treatment
        gfx2.DrawRectangle(XPens.Black, margen, y, ancho, 100);
        gfx2.DrawString("Tratamiento:", fuenteBold, XBrushes.Black, new XRect(margen + 4, y + 4, 200, 16), XStringFormats.TopLeft);
        if (!string.IsNullOrWhiteSpace(tratamiento))
            tf2.DrawString(tratamiento, fuente, XBrushes.Black, new XRect(margen + 6, y + 24, ancho - 12, 70), XStringFormats.TopLeft);
        y += 110;

        // Exams
        gfx2.DrawRectangle(XPens.Black, margen, y, ancho, 80);
        gfx2.DrawString("Exámenes complementarios:", fuenteBold, XBrushes.Black, new XRect(margen + 4, y + 4, 200, 16), XStringFormats.TopLeft);
        if (!string.IsNullOrWhiteSpace(examenes))
            tf2.DrawString(examenes, fuente, XBrushes.Black, new XRect(margen + 6, y + 24, ancho - 12, 50), XStringFormats.TopLeft);

        string ruta;
        if (!string.IsNullOrWhiteSpace(archivo) && (Path.IsPathRooted(archivo) || archivo.Contains(Path.DirectorySeparatorChar.ToString()) || archivo.Contains(Path.AltDirectorySeparatorChar.ToString())))
        {
            ruta = AsegurarExtensionPdf(archivo);
        }
        else
        {
            string nombreArchivo = AsegurarExtensionPdf(SanitizarNombreArchivo(archivo));
            ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), nombreArchivo);
        }

        try
        {
            pdf.Save(ruta);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"Error al guardar el PDF en:\n{ruta}\n\nError: {ex.Message}", "Error al Guardar", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show($"PDF guardado en:\n{ruta}\n\nPero no se pudo abrir automáticamente.\nError: {ex.Message}", "PDF Guardado", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }
    }

    private static void DibujarEncabezadoHistoria(XGraphics gfx, PdfPage pagina, double margen, ref double y)
    {
        XFont fuenteSmall = new XFont("Arial", 8);
        double ancho = pagina.Width - 2 * margen;

        try
        {
            string info1 = "MARTHA RAQUEL TELLEZ GIRON Y GARZA";
            string info2 = "RFC: TEGM85042244L7";
            string info3 = "Blvd. Constitución #27 Ote. Col. Palmas San Isidro";
            string info4 = "C.P. 27104 Torreón, Coah. Tel. 871 204 0164";
            gfx.DrawString(info1, fuenteSmall, XBrushes.Black, new XRect(margen, y, ancho / 2, 12), XStringFormats.TopLeft);
            y += 12;
            gfx.DrawString(info2, fuenteSmall, XBrushes.Black, new XRect(margen, y, ancho / 2, 12), XStringFormats.TopLeft);
            y += 12;
            gfx.DrawString(info3, fuenteSmall, XBrushes.Black, new XRect(margen, y, ancho / 2, 12), XStringFormats.TopLeft);
            y += 12;
            gfx.DrawString(info4, fuenteSmall, XBrushes.Black, new XRect(margen, y, ancho / 2, 12), XStringFormats.TopLeft);
            y = margen + 36;
        }
        catch { }

        try
        {
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            string logoPath = BuscarLogoEnCarpeta(projectDir);
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                using (XImage logo = XImage.FromFile(logoPath))
                {
                    double logoWidth = 150;
                    double scale = logoWidth / logo.PixelWidth;
                    double logoHeight = logo.PixelHeight * scale;
                    gfx.DrawImage(logo, pagina.Width - margen - logoWidth, margen - 20, logoWidth, logoHeight);
                }
            }
        }
        catch { }
        y += 20;
    }

    private static void DibujarEncabezado(XGraphics gfx, PdfPage pagina, XFont fuenteTitulo, string titulo, double margen, ref double y)
    {
        try
        {
            string projectDir = AppDomain.CurrentDomain.BaseDirectory;
            string logoPath = BuscarLogoEnCarpeta(projectDir);
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                using (XImage logo = XImage.FromFile(logoPath))
                {
                    double logoWidth = 120;
                    double scale = logoWidth / logo.PixelWidth;
                    double logoHeight = logo.PixelHeight * scale;
                    gfx.DrawImage(logo, margen, y, logoWidth, logoHeight);
                    y += logoHeight + 6;
                }
            }
        }
        catch { }

        var tituloRect = new XRect(0, y, pagina.Width, 22);
        gfx.DrawString(titulo, fuenteTitulo, XBrushes.Black, tituloRect, XStringFormats.TopCenter);
        y += 22;
        gfx.DrawLine(XPens.Gray, margen, y, pagina.Width - margen, y);
        y += 8;
    }

    private static string BuscarLogoEnCarpeta(string carpeta)
    {
        string[] nombres = { "logo.png", "logo.jpg", "logo.jpeg" };
        foreach (var nombre in nombres)
        {
            string rutaCompleta = Path.Combine(carpeta, nombre);
            if (File.Exists(rutaCompleta))
                return rutaCompleta;
        }
        return null;
    }

    private static string SanitizarNombreArchivo(string nombre)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            nombre = nombre.Replace(c, '_');
        }
        nombre = nombre.Trim();
        return string.IsNullOrWhiteSpace(nombre) ? "Documento.pdf" : nombre;
    }

    private static string AsegurarExtensionPdf(string nombre)
    {
        return Path.GetExtension(nombre)?.Equals(".pdf", StringComparison.OrdinalIgnoreCase) == true
            ? nombre
            : Path.ChangeExtension(nombre, ".pdf");
    }
}
