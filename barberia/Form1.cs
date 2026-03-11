using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace barberia
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ShowFormTiposDeCorte()
        {
            var f = new FormTiposDeCorte();
            f.ShowDialog();
        }

        private void ShowFormClientes()
        {
            var f = new FormClientes();
            f.ShowDialog();
        }

        private void ShowFormClientesPorDia()
        {
            var f = new FormClientesPorDia();
            f.ShowDialog();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var m = new MenuStrip();
            var menu = new ToolStripMenuItem("Menú");
            var m1 = new ToolStripMenuItem("Tipos de Corte", null, (s, ev) => ShowFormTiposDeCorte());
            var m2 = new ToolStripMenuItem("Clientes", null, (s, ev) => ShowFormClientes());
            var m3 = new ToolStripMenuItem("Clientes por Día", null, (s, ev) => ShowFormClientesPorDia());
            menu.DropDownItems.Add(m1);
            menu.DropDownItems.Add(m2);
            menu.DropDownItems.Add(m3);
            m.Items.Add(menu);
            this.MainMenuStrip = m;
            this.Controls.Add(m);
        }
    }
}
