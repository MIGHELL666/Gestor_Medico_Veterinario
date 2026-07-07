using System;
using System.Drawing;
using System.Windows.Forms;

namespace VeterinariaPDF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Veterinaria Corazón Canino";
            this.BackColor = Color.LightSteelBlue;
            this.Size = new Size(600, 400);

            Label titulo = new Label
            {
                Text = "Generador de Documentos PDF",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(120, 20)
            };

            string[] botones = {
                "Historia Clínica",
                "Consentimiento de Cirugía",
                "Alta Hospitalaria",
                "Recomendaciones Postoperatorias",
                "Ingreso de Albergue"
            };

            int y = 80;
            foreach (string b in botones)
            {
                Button btn = new Button
                {
                    Text = b,
                    Width = 300,
                    Height = 40,
                    Location = new Point(150, y),
                    BackColor = Color.White,
                    Font = new Font("Arial", 12)
                };
                btn.Click += (s, e) => AbrirFormulario(b);
                this.Controls.Add(btn);
                y += 50;
            }

            this.Controls.Add(titulo);
        }

        private void AbrirFormulario(string tipo)
        {
            Form frm = new Form
            {
                Text = tipo,
                Size = new Size(500, 500),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.LightGray
            };

            switch (tipo)
            {
                case "Historia Clínica":
                    new HistoriaClinicaForm(frm);
                    break;
                case "Consentimiento de Cirugía":
                    new ConsentimientoForm(frm);
                    break;
                case "Alta Hospitalaria":
                    new AltaForm(frm);
                    break;
                case "Recomendaciones Postoperatorias":
                    new RecomendacionesForm(frm);
                    break;
                case "Ingreso de Albergue":
                    new CartaForm(frm);
                    break;
            }

            frm.ShowDialog();
        }
    }
}