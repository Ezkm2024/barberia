using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace barberia
{
    public partial class FormClientesPorDia : Form
    {
        private string conexion = Config.GetConnectionString();

        public FormClientesPorDia()
        {
            InitializeComponent();
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var da = new MySqlDataAdapter("SELECT DATE(FechaVenta) AS Fecha, COUNT(IdCliente) AS CantidadClientes FROM VentaDeServicios GROUP BY DATE(FechaVenta)", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvClientesPorDia.DataSource = dt;
                    // sumar columna CantidadClientes
                    int total = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (int.TryParse(Convert.ToString(row["CantidadClientes"]), out int val))
                            total += val;
                    }
                    txtTotalClientes.Text = total.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener clientes por día: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            this.dgvClientesPorDia = new System.Windows.Forms.DataGridView();
            this.txtTotalClientes = new System.Windows.Forms.TextBox();
            this.btnMostrar = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientesPorDia)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvClientesPorDia
            // 
            this.dgvClientesPorDia.AllowUserToAddRows = false;
            this.dgvClientesPorDia.AllowUserToDeleteRows = false;
            this.dgvClientesPorDia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClientesPorDia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientesPorDia.Location = new System.Drawing.Point(12, 12);
            this.dgvClientesPorDia.MultiSelect = false;
            this.dgvClientesPorDia.Name = "dgvClientesPorDia";
            this.dgvClientesPorDia.ReadOnly = true;
            this.dgvClientesPorDia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientesPorDia.Size = new System.Drawing.Size(560, 260);
            this.dgvClientesPorDia.TabIndex = 0;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(12, 280);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(84, 13);
            this.lblTotal.Text = "Total Clientes";
            // 
            // txtTotalClientes
            // 
            this.txtTotalClientes.Location = new System.Drawing.Point(12, 295);
            this.txtTotalClientes.Name = "txtTotalClientes";
            this.txtTotalClientes.ReadOnly = true;
            this.txtTotalClientes.Size = new System.Drawing.Size(100, 20);
            this.txtTotalClientes.TabIndex = 1;
            // 
            // btnMostrar
            // 
            this.btnMostrar.Location = new System.Drawing.Point(130, 293);
            this.btnMostrar.Name = "btnMostrar";
            this.btnMostrar.Size = new System.Drawing.Size(75, 23);
            this.btnMostrar.TabIndex = 2;
            this.btnMostrar.Text = "Mostrar";
            this.btnMostrar.UseVisualStyleBackColor = true;
            this.btnMostrar.Click += new System.EventHandler(this.btnMostrar_Click);
            // 
            // FormClientesPorDia
            // 
            this.ClientSize = new System.Drawing.Size(584, 331);
            this.Controls.Add(this.btnMostrar);
            this.Controls.Add(this.txtTotalClientes);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvClientesPorDia);
            this.Name = "FormClientesPorDia";
            this.Text = "Clientes por Día";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientesPorDia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DataGridView dgvClientesPorDia;
        private System.Windows.Forms.TextBox txtTotalClientes;
        private System.Windows.Forms.Button btnMostrar;
        private System.Windows.Forms.Label lblTotal;
    }
}
