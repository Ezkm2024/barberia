using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace barberia
{
    public partial class FormClientes : Form
    {
        private string conexion = Config.GetConnectionString();

        public FormClientes()
        {
            InitializeComponent();
            CargarLocalidades();
            CargarClientes();
        }

        private void CargarLocalidades()
        {
            int idLocalidad = 0;
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var da = new MySqlDataAdapter("SELECT * FROM Localidades", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    cmbLocalidad.DisplayMember = "Localidad";
                    cmbLocalidad.ValueMember = "IdLocalidad";
                    cmbLocalidad.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar localidades: " + ex.Message);
            }
        }

        private void CargarClientes()
        {
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var da = new MySqlDataAdapter("SELECT c.*, l.Localidad FROM Clientes c LEFT JOIN Localidades l ON c.IdLocalidad = l.IdLocalidad", conn))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvClientes.DataSource = dt;
                    dgvClientes.Columns["IdCliente"].Visible = false;
                    dgvClientes.Columns["IdLocalidad"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
        }

        private bool ValidarDocumento(out string mensaje)
        {
            mensaje = null;
            var tipo = Convert.ToString(cmbTipoDoc.SelectedItem);
            var doc = txtDocumento.Text.Trim();
            if (string.IsNullOrWhiteSpace(tipo))
            {
                mensaje = "Seleccione un Tipo de Documento.";
                return false;
            }
            // verificar longitud máxima de columna en la base de datos
            try
            {
                int max = GetColumnMaxLength("Clientes", "TipoDoc");
                if (max > 0 && tipo.Length > max)
                {
                    mensaje = $"TipoDoc demasiado largo. Longitud máxima permitida en la base de datos: {max}.";
                    return false;
                }
            }
            catch { /* si falla consulta, continuar con validación local */ }
            string[] tiposValidos = new[] { "DNI", "CUIT", "CUIL", "PASAPORTE", "CI", "LE", "LC" };
            if (Array.IndexOf(tiposValidos, tipo) < 0)
            {
                mensaje = "Tipo de Documento inválido.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(doc))
            {
                mensaje = "Documento no puede estar vacío.";
                return false;
            }
            if (tipo == "CUIT" || tipo == "CUIL")
            {
                if (doc.Length != 11 || !doc.All(char.IsDigit))
                {
                    mensaje = "CUIT/CUIL debe tener exactamente 11 dígitos.";
                    return false;
                }
            }
            else
            {
                if (doc.Length < 7 || doc.Length > 8 || !doc.All(char.IsDigit))
                {
                    mensaje = "Documento debe tener entre 7 y 8 dígitos para este tipo.";
                    return false;
                }
            }
            return true;
        }

        private int GetColumnMaxLength(string tableName, string columnName)
        {
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand("SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t AND COLUMN_NAME = @c", conn))
                {
                    cmd.Parameters.AddWithValue("@t", tableName);
                    cmd.Parameters.AddWithValue("@c", columnName);
                    conn.Open();
                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                        return Convert.ToInt32(res);
                }
            }
            catch { }
            return -1;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarDocumento(out string msg))
            {
                MessageBox.Show(msg);
                return;
            }
            // validar localidad (puede escribirse una nueva)
            var localidadTexto = cmbLocalidad.Text.Trim();
            if (string.IsNullOrWhiteSpace(localidadTexto))
            {
                MessageBox.Show("Localidad no puede estar vacía.");
                return;
            }
            // validar teléfonos numéricos (permitir vacío)
            if (!string.IsNullOrWhiteSpace(txtTelefonoFijo.Text) && !txtTelefonoFijo.Text.All(char.IsDigit))
            {
                MessageBox.Show("Teléfono fijo debe contener solo números.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtTelefonoCelular.Text) && !txtTelefonoCelular.Text.All(char.IsDigit))
            {
                MessageBox.Show("Teléfono celular debe contener solo números.");
                return;
            }

            int idLocalidad = EnsureLocalidadExists(localidadTexto);
            if (idLocalidad <= 0)
            {
                MessageBox.Show("No se pudo obtener/crear la localidad.");
                return;
            }
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand(@"INSERT INTO Clientes (TipoDoc, Documento, Apellido, Nombres, Domicilio, IdLocalidad, TelefonoFijo, TelefonoCelular, Facebook, Twitter)", conn))
                {
                    cmd.CommandText += " VALUES (@tipo, @doc, @apellido, @nombres, @domicilio, @idlocalidad, @tfijo, @tcel, @fb, @tw)";
                    var tipoValor = cmbTipoDoc.SelectedItem?.ToString().Trim() ?? string.Empty;
                    cmd.Parameters.AddWithValue("@tipo", tipoValor);
                    cmd.Parameters.AddWithValue("@doc", txtDocumento.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombres", txtNombres.Text.Trim());
                    cmd.Parameters.AddWithValue("@domicilio", txtDomicilio.Text.Trim());
                    cmd.Parameters.AddWithValue("@idlocalidad", idLocalidad);
                    cmd.Parameters.AddWithValue("@tfijo", txtTelefonoFijo.Text.Trim());
                    cmd.Parameters.AddWithValue("@tcel", txtTelefonoCelular.Text.Trim());
                    cmd.Parameters.AddWithValue("@fb", txtFacebook.Text.Trim());
                    cmd.Parameters.AddWithValue("@tw", txtTwitter.Text.Trim());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                // recargar localidades para que la nueva localidad (si se creó) aparezca en el combo
                CargarLocalidades();
                try { cmbLocalidad.SelectedValue = idLocalidad; } catch { }
                CargarClientes();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar cliente: " + ex.Message);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un cliente para modificar.");
                return;
            }
            if (!ValidarDocumento(out string msg))
            {
                MessageBox.Show(msg);
                return;
            }
            var localidadTextoMod = cmbLocalidad.Text.Trim();
            if (string.IsNullOrWhiteSpace(localidadTextoMod))
            {
                MessageBox.Show("Localidad no puede estar vacía.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtTelefonoFijo.Text) && !txtTelefonoFijo.Text.All(char.IsDigit))
            {
                MessageBox.Show("Teléfono fijo debe contener solo números.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtTelefonoCelular.Text) && !txtTelefonoCelular.Text.All(char.IsDigit))
            {
                MessageBox.Show("Teléfono celular debe contener solo números.");
                return;
            }

            int id = Convert.ToInt32(dgvClientes.CurrentRow.Cells["IdCliente"].Value);
            int idLocalidad = 0;

            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand(@"UPDATE Clientes SET TipoDoc=@tipo, Documento=@doc, Apellido=@apellido, Nombres=@nombres, Domicilio=@domicilio, IdLocalidad=@idlocalidad, TelefonoFijo=@tfijo, TelefonoCelular=@tcel, Facebook=@fb, Twitter=@tw WHERE IdCliente=@id", conn))
                {
                    var tipoValorMod = cmbTipoDoc.SelectedItem?.ToString().Trim() ?? string.Empty;
                    cmd.Parameters.AddWithValue("@tipo", tipoValorMod);
                    cmd.Parameters.AddWithValue("@doc", txtDocumento.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombres", txtNombres.Text.Trim());
                    cmd.Parameters.AddWithValue("@domicilio", txtDomicilio.Text.Trim());
                    idLocalidad = EnsureLocalidadExists(localidadTextoMod);
                    if (idLocalidad <= 0)
                    {
                        MessageBox.Show("No se pudo obtener/crear la localidad.");
                        return;
                    }
                    cmd.Parameters.AddWithValue("@idlocalidad", idLocalidad);
                    cmd.Parameters.AddWithValue("@tfijo", txtTelefonoFijo.Text.Trim());
                    cmd.Parameters.AddWithValue("@tcel", txtTelefonoCelular.Text.Trim());
                    cmd.Parameters.AddWithValue("@fb", txtFacebook.Text.Trim());
                    cmd.Parameters.AddWithValue("@tw", txtTwitter.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                // recargar localidades y seleccionar la localidad asociada
                CargarLocalidades();
                try { cmbLocalidad.SelectedValue = idLocalidad; } catch { }
                CargarClientes();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar cliente: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un cliente para eliminar.");
                return;
            }
            int id = Convert.ToInt32(dgvClientes.CurrentRow.Cells["IdCliente"].Value);
            if (MessageBox.Show("Confirma eliminar el cliente?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            try
            {
                using (var conn = new MySqlConnection(conexion))
                using (var cmd = new MySqlCommand("DELETE FROM Clientes WHERE IdCliente = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                CargarClientes();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar cliente: " + ex.Message);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            txtDocumento.Text = "";
            txtApellido.Text = "";
            txtNombres.Text = "";
            txtDomicilio.Text = "";
            txtTelefonoFijo.Text = "";
            txtTelefonoCelular.Text = "";
            txtFacebook.Text = "";
            txtTwitter.Text = "";
            cmbTipoDoc.SelectedIndex = -1;
            cmbLocalidad.SelectedIndex = -1;
            dgvClientes.ClearSelection();
        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvClientes.Rows[e.RowIndex];
            txtDocumento.Text = Convert.ToString(row.Cells["Documento"].Value);
            txtApellido.Text = Convert.ToString(row.Cells["Apellido"].Value);
            txtNombres.Text = Convert.ToString(row.Cells["Nombres"].Value);
            txtDomicilio.Text = Convert.ToString(row.Cells["Domicilio"].Value);
            txtTelefonoFijo.Text = Convert.ToString(row.Cells["TelefonoFijo"].Value);
            txtTelefonoCelular.Text = Convert.ToString(row.Cells["TelefonoCelular"].Value);
            txtFacebook.Text = Convert.ToString(row.Cells["Facebook"].Value);
            txtTwitter.Text = Convert.ToString(row.Cells["Twitter"].Value);
            cmbTipoDoc.SelectedItem = Convert.ToString(row.Cells["TipoDoc"].Value);
            if (row.Cells["IdLocalidad"].Value != DBNull.Value)
                cmbLocalidad.SelectedValue = Convert.ToInt32(row.Cells["IdLocalidad"].Value);
            else
                cmbLocalidad.SelectedIndex = -1;
        }

        private int EnsureLocalidadExists(string localidad)
        {
            if (string.IsNullOrWhiteSpace(localidad)) return 0;
            try
            {
                using (var conn = new MySqlConnection(conexion))
                {
                    conn.Open();
                    // intentar obtener
                    using (var cmd = new MySqlCommand("SELECT IdLocalidad FROM Localidades WHERE Localidad = @loc LIMIT 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@loc", localidad);
                        var res = cmd.ExecuteScalar();
                        if (res != null && res != DBNull.Value)
                            return Convert.ToInt32(res);
                    }
                    // insertar nueva
                    using (var cmd2 = new MySqlCommand("INSERT INTO Localidades (Localidad) VALUES (@loc); SELECT LAST_INSERT_ID();", conn))
                    {
                        cmd2.Parameters.AddWithValue("@loc", localidad);
                        var id = cmd2.ExecuteScalar();
                        if (id != null && id != DBNull.Value)
                            return Convert.ToInt32(id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asegurar localidad: " + ex.Message);
            }
            return 0;
        }

        private void InitializeComponent()
        {
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.txtDomicilio = new System.Windows.Forms.TextBox();
            this.txtTelefonoFijo = new System.Windows.Forms.TextBox();
            this.txtTelefonoCelular = new System.Windows.Forms.TextBox();
            this.txtFacebook = new System.Windows.Forms.TextBox();
            this.txtTwitter = new System.Windows.Forms.TextBox();
            this.cmbTipoDoc = new System.Windows.Forms.ComboBox();
            this.cmbLocalidad = new System.Windows.Forms.ComboBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.lblDocumento = new System.Windows.Forms.Label();
            this.lblApellido = new System.Windows.Forms.Label();
            this.lblNombres = new System.Windows.Forms.Label();
            this.lblDomicilio = new System.Windows.Forms.Label();
            this.lblTelefonoFijo = new System.Windows.Forms.Label();
            this.lblTelefonoCelular = new System.Windows.Forms.Label();
            this.lblFacebook = new System.Windows.Forms.Label();
            this.lblTwitter = new System.Windows.Forms.Label();
            this.lblTipoDoc = new System.Windows.Forms.Label();
            this.lblLocalidad = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvClientes
            // 
            this.dgvClientes.AllowUserToAddRows = false;
            this.dgvClientes.AllowUserToDeleteRows = false;
            this.dgvClientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Location = new System.Drawing.Point(12, 12);
            this.dgvClientes.MultiSelect = false;
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvClientes.Size = new System.Drawing.Size(760, 260);
            this.dgvClientes.TabIndex = 0;
            this.dgvClientes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvClientes_CellClick);
            // 
            // Primera fila: Documento | Apellido | Nombres | TipoDoc | Localidad
            // 
            // lblDocumento
            // 
            this.lblDocumento.AutoSize = true;
            this.lblDocumento.Location = new System.Drawing.Point(12, 280);
            this.lblDocumento.Name = "lblDocumento";
            this.lblDocumento.Size = new System.Drawing.Size(62, 13);
            this.lblDocumento.Text = "Documento";
            // 
            // txtDocumento
            // 
            this.txtDocumento.Location = new System.Drawing.Point(12, 295);
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.Size = new System.Drawing.Size(120, 20);
            this.txtDocumento.TabIndex = 1;
            // 
            // lblApellido
            // 
            this.lblApellido.AutoSize = true;
            this.lblApellido.Location = new System.Drawing.Point(142, 280);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(44, 13);
            this.lblApellido.Text = "Apellido";
            // 
            // txtApellido
            // 
            this.txtApellido.Location = new System.Drawing.Point(140, 295);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(180, 20);
            this.txtApellido.TabIndex = 2;
            // 
            // lblNombres
            // 
            this.lblNombres.AutoSize = true;
            this.lblNombres.Location = new System.Drawing.Point(330, 280);
            this.lblNombres.Name = "lblNombres";
            this.lblNombres.Size = new System.Drawing.Size(49, 13);
            this.lblNombres.Text = "Nombres";
            // 
            // txtNombres
            // 
            this.txtNombres.Location = new System.Drawing.Point(330, 295);
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(180, 20);
            this.txtNombres.TabIndex = 3;
            // 
            // lblTipoDoc
            // 
            this.lblTipoDoc.AutoSize = true;
            this.lblTipoDoc.Location = new System.Drawing.Point(520, 280);
            this.lblTipoDoc.Name = "lblTipoDoc";
            this.lblTipoDoc.Size = new System.Drawing.Size(53, 13);
            this.lblTipoDoc.Text = "Tipo Doc";
            // 
            // cmbTipoDoc
            // 
            this.cmbTipoDoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoDoc.Items.AddRange(new object[] {
            "DNI",
            "CUIT",
            "CUIL",
            "PASAPORTE",
            "CI",
            "LE",
            "LC"});
            this.cmbTipoDoc.Location = new System.Drawing.Point(520, 295);
            this.cmbTipoDoc.Name = "cmbTipoDoc";
            this.cmbTipoDoc.Size = new System.Drawing.Size(100, 21);
            this.cmbTipoDoc.TabIndex = 9;
            // 
            // lblLocalidad
            // 
            this.lblLocalidad.AutoSize = true;
            this.lblLocalidad.Location = new System.Drawing.Point(630, 280);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(54, 13);
            this.lblLocalidad.Text = "Localidad";
            // 
            // cmbLocalidad
            // 
            // permitir escribir una nueva localidad además de seleccionar de la lista
            this.cmbLocalidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbLocalidad.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbLocalidad.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbLocalidad.Location = new System.Drawing.Point(630, 295);
            this.cmbLocalidad.Name = "cmbLocalidad";
            this.cmbLocalidad.Size = new System.Drawing.Size(142, 21);
            this.cmbLocalidad.TabIndex = 10;

            // 
            // Segunda fila: Domicilio (ancho completo)
            // 
            // lblDomicilio
            // 
            this.lblDomicilio.AutoSize = true;
            this.lblDomicilio.Location = new System.Drawing.Point(12, 325);
            this.lblDomicilio.Name = "lblDomicilio";
            this.lblDomicilio.Size = new System.Drawing.Size(52, 13);
            this.lblDomicilio.Text = "Domicilio";
            // 
            // txtDomicilio
            // 
            this.txtDomicilio.Location = new System.Drawing.Point(12, 340);
            this.txtDomicilio.Name = "txtDomicilio";
            this.txtDomicilio.Size = new System.Drawing.Size(460, 20);
            this.txtDomicilio.TabIndex = 4;

            // 
            // Tercera fila: TelefonoFijo | TelefonoCelular | Facebook | Twitter
            // 
            // lblTelefonoFijo
            // 
            this.lblTelefonoFijo.AutoSize = true;
            this.lblTelefonoFijo.Location = new System.Drawing.Point(12, 370);
            this.lblTelefonoFijo.Name = "lblTelefonoFijo";
            this.lblTelefonoFijo.Size = new System.Drawing.Size(67, 13);
            this.lblTelefonoFijo.Text = "Telefono Fijo";
            // 
            // txtTelefonoFijo
            // 
            this.txtTelefonoFijo.Location = new System.Drawing.Point(12, 385);
            this.txtTelefonoFijo.Name = "txtTelefonoFijo";
            this.txtTelefonoFijo.Size = new System.Drawing.Size(140, 20);
            this.txtTelefonoFijo.TabIndex = 5;
            // 
            // lblTelefonoCelular
            // 
            this.lblTelefonoCelular.AutoSize = true;
            this.lblTelefonoCelular.Location = new System.Drawing.Point(160, 370);
            this.lblTelefonoCelular.Name = "lblTelefonoCelular";
            this.lblTelefonoCelular.Size = new System.Drawing.Size(86, 13);
            this.lblTelefonoCelular.Text = "Telefono Celular";
            // 
            // txtTelefonoCelular
            // 
            this.txtTelefonoCelular.Location = new System.Drawing.Point(160, 385);
            this.txtTelefonoCelular.Name = "txtTelefonoCelular";
            this.txtTelefonoCelular.Size = new System.Drawing.Size(140, 20);
            this.txtTelefonoCelular.TabIndex = 6;
            // 
            // lblFacebook
            // 
            this.lblFacebook.AutoSize = true;
            this.lblFacebook.Location = new System.Drawing.Point(310, 370);
            this.lblFacebook.Name = "lblFacebook";
            this.lblFacebook.Size = new System.Drawing.Size(52, 13);
            this.lblFacebook.Text = "Facebook";
            // 
            // txtFacebook
            // 
            this.txtFacebook.Location = new System.Drawing.Point(310, 385);
            this.txtFacebook.Name = "txtFacebook";
            this.txtFacebook.Size = new System.Drawing.Size(180, 20);
            this.txtFacebook.TabIndex = 7;
            // 
            // lblTwitter
            // 
            this.lblTwitter.AutoSize = true;
            this.lblTwitter.Location = new System.Drawing.Point(500, 370);
            this.lblTwitter.Name = "lblTwitter";
            this.lblTwitter.Size = new System.Drawing.Size(41, 13);
            this.lblTwitter.Text = "Twitter";
            // 
            // txtTwitter
            // 
            this.txtTwitter.Location = new System.Drawing.Point(500, 385);
            this.txtTwitter.Name = "txtTwitter";
            this.txtTwitter.Size = new System.Drawing.Size(180, 20);
            this.txtTwitter.TabIndex = 8;

            // 
            // Botones
            // 
            this.btnAgregar.Location = new System.Drawing.Point(12, 420);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(75, 23);
            this.btnAgregar.TabIndex = 11;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            this.btnModificar.Location = new System.Drawing.Point(105, 420);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 12;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = true;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            this.btnEliminar.Location = new System.Drawing.Point(198, 420);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 23);
            this.btnEliminar.TabIndex = 13;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            this.btnNuevo.Location = new System.Drawing.Point(291, 420);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(75, 23);
            this.btnNuevo.TabIndex = 14;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // FormClientes
            // 
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.txtTwitter);
            this.Controls.Add(this.lblTwitter);
            this.Controls.Add(this.txtFacebook);
            this.Controls.Add(this.lblFacebook);
            this.Controls.Add(this.txtTelefonoCelular);
            this.Controls.Add(this.lblTelefonoCelular);
            this.Controls.Add(this.txtTelefonoFijo);
            this.Controls.Add(this.lblTelefonoFijo);
            this.Controls.Add(this.txtDomicilio);
            this.Controls.Add(this.lblDomicilio);
            this.Controls.Add(this.cmbLocalidad);
            this.Controls.Add(this.lblLocalidad);
            this.Controls.Add(this.cmbTipoDoc);
            this.Controls.Add(this.lblTipoDoc);
            this.Controls.Add(this.txtNombres);
            this.Controls.Add(this.lblNombres);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.lblApellido);
            this.Controls.Add(this.txtDocumento);
            this.Controls.Add(this.lblDocumento);
            this.Controls.Add(this.dgvClientes);
            this.Name = "FormClientes";
            this.Text = "Clientes";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.TextBox txtDomicilio;
        private System.Windows.Forms.TextBox txtTelefonoFijo;
        private System.Windows.Forms.TextBox txtTelefonoCelular;
        private System.Windows.Forms.TextBox txtFacebook;
        private System.Windows.Forms.TextBox txtTwitter;
        private System.Windows.Forms.ComboBox cmbTipoDoc;
        private System.Windows.Forms.ComboBox cmbLocalidad;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Label lblDocumento;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.Label lblNombres;
        private System.Windows.Forms.Label lblDomicilio;
        private System.Windows.Forms.Label lblTelefonoFijo;
        private System.Windows.Forms.Label lblTelefonoCelular;
        private System.Windows.Forms.Label lblFacebook;
        private System.Windows.Forms.Label lblTwitter;
        private System.Windows.Forms.Label lblTipoDoc;
        private System.Windows.Forms.Label lblLocalidad;
    }
}
