using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using System.Windows.Forms;
using System.Drawing;

namespace VeterinariaPDF
{
    internal static class Helpers
    {
        public static void AgregarPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };
        }

        public static string Valor(TextBox txt, string placeholder = null)
        {
            // Si el control está en gris (placeholder) o coincide con el placeholder, devolver vacío
            if (txt.ForeColor == Color.Gray) return "";
            var t = (txt.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(placeholder) && t == placeholder) return "";
            return t;
        }

        public static bool EsEntero(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return int.TryParse(s, out _);
        }

        public static bool EsEdadConUnidades(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var t = s.Trim();
            if (EsEntero(t)) return true;
            var pattern = @"^\s*(\d+\s*(?:a|año|años|m|mes|meses|sem|semana|semanas|d|dia|día|dias|días))(?:\s+(?:y\s+)?\d+\s*(?:a|año|años|m|mes|meses|sem|semana|semanas|d|dia|día|dias|días))*\s*$";
            return Regex.IsMatch(t, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static bool EsDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            return double.TryParse(s, out _);
        }

        public static bool EsTelefono(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            foreach (char c in s)
            {
                if (!char.IsDigit(c) && c != ' ' && c != '-') return false;
            }
            return true;
        }

        public static string ElegirRutaGuardar(string nombreSugerido)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = nombreSugerido;
                var res = sfd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    return sfd.FileName;
                }
            }
            return null;
        }
    }
}
