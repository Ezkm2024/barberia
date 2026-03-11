using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace barberia
{
    public partial class FormTiposDeCorte : Form
    {
        private string conexion = Config.GetConnectionString();

        public FormTiposDeCorte()
        {
            InitializeComponent();
            CargarTiposDeCorte();
        }

        private void CargarTiposDeCorte()
        {
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var da = new MySqlDataAdapter("SELECT * FROM TiposDeCorte", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvTiposDeCorte.DataSource = dt;
                    dgvTiposDeCorte.Columns["IdTipoDeCorte"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipos de corte: " + ex.Message);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarTipoYPrecio(out decimal precio, out string msg))
            {
                MessageBox.Show(msg);
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand("INSERT INTO TiposDeCorte (TipoDeCorte, PrecioServicio) VALUES (@tipo, @precio)", conn))
                {
                    cmd.Parameters.AddWithValue("@tipo", txtTipoDeCorte.Text.Trim());
                    cmd.Parameters.AddWithValue("@precio", precio);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                CargarTiposDeCorte();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvTiposDeCorte.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un registro para modificar.");
                return;
            }
            if (!ValidarTipoYPrecio(out decimal precio, out string msg))
            {
                MessageBox.Show(msg);
                return;
            }

            int id = Convert.ToInt32(dgvTiposDeCorte.CurrentRow.Cells["IdTipoDeCorte"].Value);

            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand("UPDATE TiposDeCorte SET TipoDeCorte = @tipo, PrecioServicio = @precio WHERE IdTipoDeCorte = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@tipo", txtTipoDeCorte.Text.Trim());
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                CargarTiposDeCorte();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTiposDeCorte.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un registro para eliminar.");
                return;
            }

            int id = Convert.ToInt32(dgvTiposDeCorte.CurrentRow.Cells["IdTipoDeCorte"].Value);
            if (MessageBox.Show("Confirma eliminar el registro?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand("DELETE FROM TiposDeCorte WHERE IdTipoDeCorte = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                CargarTiposDeCorte();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            txtTipoDeCorte.Text = "";
            txtPrecio.Text = "";
            dgvTiposDeCorte.ClearSelection();
        }

        private bool ValidarTipoYPrecio(out decimal precio, out string mensaje)
        {
            precio = 0;
            mensaje = null;
            if (string.IsNullOrWhiteSpace(txtTipoDeCorte.Text))
            {
                mensaje = "El Tipo de Corte no puede estar vacío.";
                return false;
            }
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out precio))
            {
                mensaje = "El Precio debe ser un número válido.";
                return false;
            }
            if (precio <= 0)
            {
                mensaje = "El Precio debe ser mayor a 0.";
                return false;
            }
            return true;
        }

        private void dgvTiposDeCorte_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvTiposDeCorte.Rows[e.RowIndex];
            txtTipoDeCorte.Text = Convert.ToString(row.Cells["TipoDeCorte"].Value);
            txtPrecio.Text = Convert.ToString(row.Cells["PrecioServicio"].Value);
        }

        private void InitializeComponent()
        {
            this.dgvTiposDeCorte = new System.Windows.Forms.DataGridView();
            this.txtTipoDeCorte = new System.Windows.Forms.TextBox();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.lblTipoDeCorte = new System.Windows.Forms.Label();
            this.lblPrecio = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTiposDeCorte)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTiposDeCorte
            // 
            this.dgvTiposDeCorte.AllowUserToAddRows = false;
            this.dgvTiposDeCorte.AllowUserToDeleteRows = false;
            this.dgvTiposDeCorte.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTiposDeCorte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTiposDeCorte.Location = new System.Drawing.Point(12, 12);
            this.dgvTiposDeCorte.MultiSelect = false;
            this.dgvTiposDeCorte.Name = "dgvTiposDeCorte";
            this.dgvTiposDeCorte.ReadOnly = true;
            this.dgvTiposDeCorte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTiposDeCorte.Size = new System.Drawing.Size(560, 260);
            this.dgvTiposDeCorte.TabIndex = 0;
            this.dgvTiposDeCorte.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTiposDeCorte_CellClick);
            // 
            // lblTipoDeCorte
            // 
            this.lblTipoDeCorte.AutoSize = true;
            this.lblTipoDeCorte.Location = new System.Drawing.Point(12, 280);
            this.lblTipoDeCorte.Name = "lblTipoDeCorte";
            this.lblTipoDeCorte.Size = new System.Drawing.Size(75, 13);
            this.lblTipoDeCorte.Text = "Tipo de Corte";
            // 
            // txtTipoDeCorte
            // 
            this.txtTipoDeCorte.Location = new System.Drawing.Point(12, 295);
            this.txtTipoDeCorte.Name = "txtTipoDeCorte";
            this.txtTipoDeCorte.Size = new System.Drawing.Size(360, 20);
            this.txtTipoDeCorte.TabIndex = 1;
            // 
            // lblPrecio
            // 
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Location = new System.Drawing.Point(380, 280);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Size = new System.Drawing.Size(39, 13);
            this.lblPrecio.Text = "Precio";
            // 
            // txtPrecio
            // 
            this.txtPrecio.Location = new System.Drawing.Point(380, 295);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(120, 20);
            this.txtPrecio.TabIndex = 2;
            // 
            // Botones
            // 
            this.btnAgregar.Location = new System.Drawing.Point(12, 330);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(75, 23);
            this.btnAgregar.TabIndex = 3;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            this.btnModificar.Location = new System.Drawing.Point(105, 330);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 4;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = true;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            this.btnEliminar.Location = new System.Drawing.Point(198, 330);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 23);
            this.btnEliminar.TabIndex = 5;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            this.btnNuevo.Location = new System.Drawing.Point(291, 330);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(75, 23);
            this.btnNuevo.TabIndex = 6;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // FormTiposDeCorte
            // 
            this.ClientSize = new System.Drawing.Size(584, 371);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.txtTipoDeCorte);
            this.Controls.Add(this.lblTipoDeCorte);
            this.Controls.Add(this.dgvTiposDeCorte);
            this.Name = "FormTiposDeCorte";
            this.Text = "Tipos de Corte";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTiposDeCorte)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DataGridView dgvTiposDeCorte;
        private System.Windows.Forms.TextBox txtTipoDeCorte;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Label lblTipoDeCorte;
        private System.Windows.Forms.Label lblPrecio;
    }
}
