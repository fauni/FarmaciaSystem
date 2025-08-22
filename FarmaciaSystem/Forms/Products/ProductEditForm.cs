using Dapper;
using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Forms.Base;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmaciaSystem.Forms.Products
{
    public partial class ProductEditForm : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlMain;
        private Panel pnlButtons;

        // Controles del formulario
        private Label lblBarcode;
        private TextBox txtBarcode;
        private Button btnGenerateBarcode;
        private Label lblName;
        private TextBox txtName;
        private Label lblActiveIngredient;
        private ComboBox cmbActiveIngredient;
        private Label lblConcentration;
        private TextBox txtConcentration;
        private Label lblPharmaceuticalForm;
        private ComboBox cmbPharmaceuticalForm;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblSalePrice;
        private NumericUpDown nudSalePrice;
        private Label lblPurchasePrice;
        private NumericUpDown nudPurchasePrice;
        private Label lblMinStock;
        private NumericUpDown nudMinStock;
        private Label lblMaxStock;
        private NumericUpDown nudMaxStock;
        private CheckBox chkRequiresPrescription;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnCancel;

        private ProductService _productService;
        private CategoryRepository _categoryRepository;
        private ActiveIngredientRepository _activeIngredientRepository;
        private PharmaceuticalFormRepository _pharmaceuticalFormRepository;

        private int? _productId;
        private Product _currentProduct;
        private bool _isEditMode;

        public ProductEditForm(int? productId = null)
        {
            _productId = productId;
            _isEditMode = productId.HasValue;

            InitializeComponent();
            InitializeServices();
            ConfigureProductEditForm();
        }

        private void InitializeServices()
        {
            var productRepository = new ProductRepository();
            var productValidator = new Business.Validators.ProductValidator(productRepository);
            _productService = new ProductService(productRepository, productValidator);
            _categoryRepository = new CategoryRepository();
            _activeIngredientRepository = new ActiveIngredientRepository();
            _pharmaceuticalFormRepository = new PharmaceuticalFormRepository();
        }

        private void ConfigureProductEditForm()
        {
            this.Text = _isEditMode ? "Editar Producto" : "Nuevo Producto";
            this.Size = new Size(600, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            CreateProductEditInterface();
            this.Load += ProductEditForm_Load;
        }

        private void CreateProductEditInterface()
        {
            // Panel de encabezado
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(20, 15, 20, 15)
            };

            lblTitle = new Label
            {
                Text = _isEditMode ? "Editar Producto" : "Nuevo Producto",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlHeader.Controls.Add(lblTitle);

            // Panel principal
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 20),
                AutoScroll = true
            };

            CreateFormControls();

            // Panel de botones
            pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(20, 12, 20, 12)
            };

            btnSave = new Button
            {
                Text = _isEditMode ? "Actualizar" : "Guardar",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(120, 36),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right
            };
            btnSave.Location = new Point(pnlButtons.Width - 260, 12);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(120, 36),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right
            };
            btnCancel.Location = new Point(pnlButtons.Width - 130, 12);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnSave, btnCancel });

            // Agregar paneles al formulario
            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);
        }

        private void CreateFormControls()
        {
            int yPosition = 10;
            const int labelHeight = 20;
            const int controlHeight = 25;
            const int spacing = 35;

            // Código de barras
            lblBarcode = new Label
            {
                Text = "Código de Barras:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            txtBarcode = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(400, controlHeight),
                MaxLength = 50
            };

            btnGenerateBarcode = new Button
            {
                Text = "Generar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, controlHeight),
                Location = new Point(410, yPosition + 20),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGenerateBarcode.FlatAppearance.BorderSize = 0;
            btnGenerateBarcode.Click += BtnGenerateBarcode_Click;

            yPosition += spacing + 15;

            // Nombre
            lblName = new Label
            {
                Text = "Nombre del Producto: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(200, labelHeight)
            };

            txtName = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(500, controlHeight),
                MaxLength = 200
            };

            yPosition += spacing;

            // Principio activo
            lblActiveIngredient = new Label
            {
                Text = "Principio Activo:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            cmbActiveIngredient = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(240, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Concentración
            lblConcentration = new Label
            {
                Text = "Concentración:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition),
                Size = new Size(150, labelHeight)
            };

            txtConcentration = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition + 20),
                Size = new Size(240, controlHeight),
                MaxLength = 50,
                // PlaceholderText = "Ej: 500mg, 25ml"
            };

            yPosition += spacing;

            // Forma farmacéutica
            lblPharmaceuticalForm = new Label
            {
                Text = "Forma Farmacéutica: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(200, labelHeight)
            };

            cmbPharmaceuticalForm = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(240, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Categoría
            lblCategory = new Label
            {
                Text = "Categoría:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition),
                Size = new Size(150, labelHeight)
            };

            cmbCategory = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition + 20),
                Size = new Size(240, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            yPosition += spacing;

            // Precio de compra
            lblPurchasePrice = new Label
            {
                Text = "Precio de Compra: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            nudPurchasePrice = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(240, controlHeight),
                DecimalPlaces = 2,
                Maximum = 999999.99m,
                Minimum = 0.01m,
                Value = 1.00m
            };

            // Precio de venta
            lblSalePrice = new Label
            {
                Text = "Precio de Venta: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition),
                Size = new Size(150, labelHeight)
            };

            nudSalePrice = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition + 20),
                Size = new Size(240, controlHeight),
                DecimalPlaces = 2,
                Maximum = 999999.99m,
                Minimum = 0.01m,
                Value = 1.00m
            };

            yPosition += spacing;

            // Stock mínimo
            lblMinStock = new Label
            {
                Text = "Stock Mínimo: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            nudMinStock = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(240, controlHeight),
                Maximum = 999999,
                Minimum = 0,
                Value = 10
            };

            // Stock máximo
            lblMaxStock = new Label
            {
                Text = "Stock Máximo: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition),
                Size = new Size(150, labelHeight)
            };

            nudMaxStock = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition + 20),
                Size = new Size(240, controlHeight),
                Maximum = 999999,
                Minimum = 1,
                Value = 1000
            };

            yPosition += spacing + 10;

            // Checkboxes
            chkRequiresPrescription = new CheckBox
            {
                Text = "Requiere Prescripción Médica",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(250, 25),
                Checked = false
            };

            chkIsActive = new CheckBox
            {
                Text = "Producto Activo",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(260, yPosition),
                Size = new Size(150, 25),
                Checked = true
            };

            // Agregar controles al panel principal
            pnlMain.Controls.AddRange(new Control[]
            {
                lblBarcode, txtBarcode, btnGenerateBarcode,
                lblName, txtName,
                lblActiveIngredient, cmbActiveIngredient,
                lblConcentration, txtConcentration,
                lblPharmaceuticalForm, cmbPharmaceuticalForm,
                lblCategory, cmbCategory,
                lblPurchasePrice, nudPurchasePrice,
                lblSalePrice, nudSalePrice,
                lblMinStock, nudMinStock,
                lblMaxStock, nudMaxStock,
                chkRequiresPrescription, chkIsActive
            });

            // Eventos
            nudPurchasePrice.ValueChanged += NudPurchasePrice_ValueChanged;
            nudMinStock.ValueChanged += NudMinStock_ValueChanged;
            cmbActiveIngredient.SelectedIndexChanged += CmbActiveIngredient_SelectedIndexChanged;
        }

        private async void ProductEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await LoadComboBoxDataAsync();

                if (_isEditMode)
                {
                    await LoadProductDataAsync();
                }
                else
                {
                    // Configurar valores por defecto para nuevo producto
                    chkIsActive.Checked = true;
                    txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar datos: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadComboBoxDataAsync()
        {
            // Cargar categorías
            var categories = await _categoryRepository.GetAllAsync();
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add(new ComboBoxItem { Text = "-- Seleccionar --", Value = null });
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(new ComboBoxItem { Text = category.Name, Value = category.Id });
            }
            cmbCategory.SelectedIndex = 0;

            // Cargar principios activos
            var activeIngredients = await _activeIngredientRepository.GetAllAsync();
            cmbActiveIngredient.Items.Clear();
            cmbActiveIngredient.Items.Add(new ComboBoxItem { Text = "-- Seleccionar --", Value = null });
            foreach (var ingredient in activeIngredients)
            {
                cmbActiveIngredient.Items.Add(new ComboBoxItem { Text = ingredient.Name, Value = ingredient.Id });
            }
            cmbActiveIngredient.SelectedIndex = 0;

            // Cargar formas farmacéuticas
            var pharmaceuticalForms = await _pharmaceuticalFormRepository.GetAllAsync();
            cmbPharmaceuticalForm.Items.Clear();
            cmbPharmaceuticalForm.Items.Add(new ComboBoxItem { Text = "-- Seleccionar --", Value = null });
            foreach (var form in pharmaceuticalForms)
            {
                cmbPharmaceuticalForm.Items.Add(new ComboBoxItem { Text = form.Name, Value = form.Id });
            }
            cmbPharmaceuticalForm.SelectedIndex = 0;
        }

        private async Task LoadProductDataAsync()
        {
            try
            {
                _currentProduct = await _productService.GetProductByIdAsync(_productId.Value);
                if (_currentProduct == null)
                {
                    ShowMessage("No se pudo cargar el producto", MessageType.Error);
                    this.Close();
                    return;
                }

                // Llenar controles con datos del producto
                txtBarcode.Text = _currentProduct.Barcode ?? "";
                txtName.Text = _currentProduct.Name ?? "";
                txtConcentration.Text = _currentProduct.Concentration ?? "";
                nudPurchasePrice.Value = _currentProduct.PurchasePrice;
                nudSalePrice.Value = _currentProduct.SalePrice;
                nudMinStock.Value = _currentProduct.MinStock;
                nudMaxStock.Value = _currentProduct.MaxStock;
                chkRequiresPrescription.Checked = _currentProduct.RequiresPrescription;
                chkIsActive.Checked = _currentProduct.IsActive;

                // Seleccionar items en combos
                SelectComboBoxItem(cmbCategory, _currentProduct.CategoryId);
                SelectComboBoxItem(cmbActiveIngredient, _currentProduct.ActiveIngredientId);
                SelectComboBoxItem(cmbPharmaceuticalForm, _currentProduct.PharmaceuticalFormId);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar producto: {ex.Message}", MessageType.Error);
                this.Close();
            }
        }

        private void SelectComboBoxItem(ComboBox combo, int? valueId)
        {
            if (!valueId.HasValue)
            {
                combo.SelectedIndex = 0;
                return;
            }

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is ComboBoxItem item &&
                    item.Value?.Equals(valueId.Value) == true)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            combo.SelectedIndex = 0;
        }

        private void BtnGenerateBarcode_Click(object sender, EventArgs e)
        {
            // Generar código de barras simple basado en timestamp
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var barcode = $"750{timestamp.ToString().Substring(5)}";
            txtBarcode.Text = barcode;
        }

        private void NudPurchasePrice_ValueChanged(object sender, EventArgs e)
        {
            // Sugerir precio de venta con margen del 30%
            if (nudPurchasePrice.Value > 0)
            {
                var suggestedPrice = nudPurchasePrice.Value * 1.30m;
                if (nudSalePrice.Value <= nudPurchasePrice.Value)
                {
                    nudSalePrice.Value = Math.Round(suggestedPrice, 2);
                }
            }
        }

        private void NudMinStock_ValueChanged(object sender, EventArgs e)
        {
            // Asegurar que el stock máximo sea mayor al mínimo
            if (nudMaxStock.Value <= nudMinStock.Value)
            {
                nudMaxStock.Value = nudMinStock.Value + 100;
            }
        }

        private void CmbActiveIngredient_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si se selecciona un principio activo, hacer visible el campo concentración
            var hasActiveIngredient = cmbActiveIngredient.SelectedIndex > 0;
            lblConcentration.Visible = hasActiveIngredient;
            txtConcentration.Visible = hasActiveIngredient;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                btnSave.Enabled = false;
                btnSave.Text = _isEditMode ? "Actualizando..." : "Guardando...";

                var product = CreateProductFromForm();

                if (_isEditMode)
                {
                    product.Id = _productId.Value;
                    await _productService.UpdateProductAsync(product);
                    ShowMessage("Producto actualizado exitosamente", MessageType.Success);
                }
                else
                {
                    await _productService.CreateProductAsync(product);
                    ShowMessage("Producto creado exitosamente", MessageType.Success);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al guardar producto: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnSave.Enabled = true;
                btnSave.Text = _isEditMode ? "Actualizar" : "Guardar";
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (HasUnsavedChanges())
            {
                if (!ShowConfirmation("¿Está seguro que desea cancelar? Se perderán los cambios."))
                    return;
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowMessage("El nombre del producto es requerido", MessageType.Warning);
                txtName.Focus();
                return false;
            }

            if (cmbPharmaceuticalForm.SelectedIndex <= 0)
            {
                ShowMessage("Debe seleccionar una forma farmacéutica", MessageType.Warning);
                cmbPharmaceuticalForm.Focus();
                return false;
            }

            if (nudPurchasePrice.Value <= 0)
            {
                ShowMessage("El precio de compra debe ser mayor a cero", MessageType.Warning);
                nudPurchasePrice.Focus();
                return false;
            }

            if (nudSalePrice.Value <= nudPurchasePrice.Value)
            {
                ShowMessage("El precio de venta debe ser mayor al precio de compra", MessageType.Warning);
                nudSalePrice.Focus();
                return false;
            }

            if (nudMaxStock.Value <= nudMinStock.Value)
            {
                ShowMessage("El stock máximo debe ser mayor al stock mínimo", MessageType.Warning);
                nudMaxStock.Focus();
                return false;
            }

            // Validar concentración si hay principio activo
            if (cmbActiveIngredient.SelectedIndex > 0 && string.IsNullOrWhiteSpace(txtConcentration.Text))
            {
                ShowMessage("Debe especificar la concentración cuando selecciona un principio activo", MessageType.Warning);
                txtConcentration.Focus();
                return false;
            }

            return true;
        }

        private Product CreateProductFromForm()
        {
            var categoryId = GetComboBoxValue(cmbCategory);
            var activeIngredientId = GetComboBoxValue(cmbActiveIngredient);
            var pharmaceuticalFormId = GetComboBoxValue(cmbPharmaceuticalForm);

            return new Product
            {
                Barcode = string.IsNullOrWhiteSpace(txtBarcode.Text) ? null : txtBarcode.Text.Trim(),
                Name = txtName.Text.Trim(),
                ActiveIngredientId = activeIngredientId,
                Concentration = string.IsNullOrWhiteSpace(txtConcentration.Text) ? null : txtConcentration.Text.Trim(),
                PharmaceuticalFormId = pharmaceuticalFormId ?? 0,
                CategoryId = categoryId,
                SalePrice = nudSalePrice.Value,
                PurchasePrice = nudPurchasePrice.Value,
                RequiresPrescription = chkRequiresPrescription.Checked,
                MinStock = (int)nudMinStock.Value,
                MaxStock = (int)nudMaxStock.Value,
                IsActive = chkIsActive.Checked
            };
        }

        private int? GetComboBoxValue(ComboBox combo)
        {
            if (combo.SelectedIndex <= 0 || !(combo.SelectedItem is ComboBoxItem item))
                return null;

            return item.Value as int?;
        }

        private bool HasUnsavedChanges()
        {
            if (!_isEditMode)
            {
                // Para nuevo producto, verificar si se ha ingresado algún dato
                return !string.IsNullOrWhiteSpace(txtName.Text) ||
                       !string.IsNullOrWhiteSpace(txtBarcode.Text) ||
                       cmbPharmaceuticalForm.SelectedIndex > 0;
            }

            // Para edición, comparar con datos originales
            if (_currentProduct == null) return false;

            return txtBarcode.Text.Trim() != (_currentProduct.Barcode ?? "") ||
                   txtName.Text.Trim() != (_currentProduct.Name ?? "") ||
                   txtConcentration.Text.Trim() != (_currentProduct.Concentration ?? "") ||
                   nudPurchasePrice.Value != _currentProduct.PurchasePrice ||
                   nudSalePrice.Value != _currentProduct.SalePrice ||
                   (int)nudMinStock.Value != _currentProduct.MinStock ||
                   (int)nudMaxStock.Value != _currentProduct.MaxStock ||
                   chkRequiresPrescription.Checked != _currentProduct.RequiresPrescription ||
                   chkIsActive.Checked != _currentProduct.IsActive ||
                   GetComboBoxValue(cmbCategory) != _currentProduct.CategoryId ||
                   GetComboBoxValue(cmbActiveIngredient) != _currentProduct.ActiveIngredientId ||
                   GetComboBoxValue(cmbPharmaceuticalForm) != _currentProduct.PharmaceuticalFormId;
        }
    }

    // Clase auxiliar para ComboBox
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    // Repositorios adicionales necesarios
    public class ActiveIngredientRepository : BaseRepository<ActiveIngredient>
    {
        public ActiveIngredientRepository() : base("ActiveIngredients", new[]
        {
            "Id", "Name", "Description"
        })
        { }

        public override async Task<ActiveIngredient> AddAsync(ActiveIngredient entity)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"INSERT INTO ActiveIngredients (Name, Description) VALUES (@Name, @Description); SELECT last_insert_rowid();";
                var id = await connection.QuerySingleAsync<int>(sql, entity);
                entity.Id = id;
                return entity;
            }
        }

        public override async Task<ActiveIngredient> UpdateAsync(ActiveIngredient entity)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"UPDATE ActiveIngredients SET Name = @Name, Description = @Description WHERE Id = @Id";
                await connection.ExecuteAsync(sql, entity);
                return entity;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "DELETE FROM ActiveIngredients WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<ActiveIngredient>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"SELECT {string.Join(", ", SelectColumns)} FROM {TableName} WHERE Name LIKE @term ORDER BY Name";
                return await connection.QueryAsync<ActiveIngredient>(sql, new { term = $"%{searchTerm}%" });
            }
        }
    }

    public class PharmaceuticalFormRepository : BaseRepository<PharmaceuticalForm>
    {
        public PharmaceuticalFormRepository() : base("PharmaceuticalForms", new[]
        {
            "Id", "Name", "Description"
        })
        { }

        public override async Task<PharmaceuticalForm> AddAsync(PharmaceuticalForm entity)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"INSERT INTO PharmaceuticalForms (Name, Description) VALUES (@Name, @Description); SELECT last_insert_rowid();";
                var id = await connection.QuerySingleAsync<int>(sql, entity);
                entity.Id = id;
                return entity;
            }
        }

        public override async Task<PharmaceuticalForm> UpdateAsync(PharmaceuticalForm entity)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"UPDATE PharmaceuticalForms SET Name = @Name, Description = @Description WHERE Id = @Id";
                await connection.ExecuteAsync(sql, entity);
                return entity;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "DELETE FROM PharmaceuticalForms WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<PharmaceuticalForm>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"SELECT {string.Join(", ", SelectColumns)} FROM {TableName} WHERE Name LIKE @term ORDER BY Name";
                return await connection.QueryAsync<PharmaceuticalForm>(sql, new { term = $"%{searchTerm}%" });
            }
        }
    }
}
