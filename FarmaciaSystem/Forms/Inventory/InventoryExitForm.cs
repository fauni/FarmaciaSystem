using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Forms.Base;
using FarmaciaSystem.Forms.Products;
using FarmaciaSystem.Models;
using FarmaciaSystem.Utils.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmaciaSystem.Forms.Inventory
{
    public partial class InventoryExitForm : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlMain;
        private Panel pnlButtons;

        // Controles del formulario
        private Label lblProduct;
        private ComboBox cmbProduct;
        private Button btnSearchProduct;
        private Label lblWarehouse;
        private ComboBox cmbWarehouse;
        private Label lblQuantity;
        private NumericUpDown nudQuantity;
        private Label lblReason;
        private ComboBox cmbReason;
        private Label lblCustomReason;
        private TextBox txtCustomReason;
        private Label lblReference;
        private TextBox txtReference;
        private Button btnSave;
        private Button btnCancel;

        // Información del producto y stock
        private Panel pnlProductInfo;
        private Label lblProductInfo;
        private DataGridView dgvAvailableStock;

        private InventoryService _inventoryService;
        private ProductService _productService;
        private WarehouseRepository _warehouseRepository;

        private Product _selectedProduct;
        private List<Batch> _availableBatches;

        public InventoryExitForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureInventoryExitForm();
        }

        protected override void CheckPermissions()
        {
            if (!PermissionManager.HasPermission("inventario_salida"))
            {
                ShowMessage("No tiene permisos para registrar salidas de inventario", MessageType.Warning);
                this.Close();
            }
        }

        private void InitializeServices()
        {
            var productRepository = new ProductRepository();
            var inventoryRepository = new InventoryRepository();
            var movementRepository = new MovementRepository();
            var productValidator = new Business.Validators.ProductValidator(productRepository);
            var inventoryValidator = new Business.Validators.InventoryValidator();

            _productService = new ProductService(productRepository, productValidator);
            _inventoryService = new InventoryService(inventoryRepository, movementRepository, productRepository, inventoryValidator);
            _warehouseRepository = new WarehouseRepository();
        }

        private void ConfigureInventoryExitForm()
        {
            this.Text = "Salida de Mercancía";
            this.Size = new Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            CreateInventoryExitInterface();
            this.Load += InventoryExitForm_Load;
        }

        private void CreateInventoryExitInterface()
        {
            // Panel de encabezado
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(231, 76, 60),
                Padding = new Padding(20, 15, 20, 15)
            };

            lblTitle = new Label
            {
                Text = "Salida de Mercancía",
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
                Text = "Registrar Salida",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right
            };
            btnSave.Location = new Point(pnlButtons.Width - 280, 12);
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(100, 36),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right
            };
            btnCancel.Location = new Point(pnlButtons.Width - 120, 12);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnSave, btnCancel });

            this.Controls.AddRange(new Control[] { pnlMain, pnlButtons, pnlHeader });
        }

        private void CreateFormControls()
        {
            const int labelHeight = 20;
            const int controlHeight = 25;
            const int spacing = 50;
            int yPosition = 20;

            // Producto
            lblProduct = new Label
            {
                Text = "Producto: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            cmbProduct = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(400, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnSearchProduct = new Button
            {
                Text = "Buscar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, controlHeight),
                Location = new Point(410, yPosition + 20),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearchProduct.FlatAppearance.BorderSize = 0;

            yPosition += spacing;

            // Almacén
            lblWarehouse = new Label
            {
                Text = "Almacén: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            cmbWarehouse = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(300, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            yPosition += spacing;

            // Panel de información del producto
            pnlProductInfo = new Panel
            {
                Location = new Point(0, yPosition),
                Size = new Size(700, 200),
                BackColor = Color.FromArgb(241, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Visible = false
            };

            lblProductInfo = new Label
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvAvailableStock = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                ColumnHeadersHeight = 30
            };

            ConfigureStockGridColumns();

            pnlProductInfo.Controls.AddRange(new Control[] { dgvAvailableStock, lblProductInfo });

            yPosition += 210;

            // Cantidad
            lblQuantity = new Label
            {
                Text = "Cantidad a Retirar: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            nudQuantity = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(200, controlHeight),
                Maximum = 999999,
                Minimum = 1,
                Value = 1
            };

            yPosition += spacing;

            // Motivo
            lblReason = new Label
            {
                Text = "Motivo de Salida: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            cmbReason = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(300, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Cargar motivos predefinidos
            cmbReason.Items.AddRange(new[]
            {
                "Venta",
                "Vencimiento",
                "Devolución",
                "Transferencia",
                "Uso interno",
                "Pérdida",
                "Daño",
                "Otro"
            });

            yPosition += spacing;

            // Motivo personalizado
            lblCustomReason = new Label
            {
                Text = "Especificar motivo:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight),
                Visible = false
            };

            txtCustomReason = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(400, controlHeight),
                Visible = false
            };

            yPosition += spacing;

            // Referencia
            lblReference = new Label
            {
                Text = "Referencia/Documento:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            txtReference = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(400, controlHeight)
            };

            // Agregar todos los controles al panel principal
            pnlMain.Controls.AddRange(new Control[]
            {
                lblProduct, cmbProduct, btnSearchProduct,
                lblWarehouse, cmbWarehouse,
                pnlProductInfo,
                lblQuantity, nudQuantity,
                lblReason, cmbReason,
                lblCustomReason, txtCustomReason,
                lblReference, txtReference
            });

            // Configurar eventos
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            btnSearchProduct.Click += BtnSearchProduct_Click;
            cmbWarehouse.SelectedIndexChanged += CmbWarehouse_SelectedIndexChanged;
            cmbReason.SelectedIndexChanged += CmbReason_SelectedIndexChanged;
        }

        private void ConfigureStockGridColumns()
        {
            dgvAvailableStock.Columns.Clear();

            dgvAvailableStock.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "BatchNumber",
                    HeaderText = "Lote",
                    DataPropertyName = "BatchNumber",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "ExpiryDate",
                    HeaderText = "Vencimiento",
                    DataPropertyName = "ExpiryDate",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "dd/MM/yyyy"
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "CurrentStock",
                    HeaderText = "Stock",
                    DataPropertyName = "CurrentStock",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "LocationCode",
                    HeaderText = "Ubicación",
                    DataPropertyName = "LocationCode",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "PurchasePrice",
                    HeaderText = "Precio Compra",
                    DataPropertyName = "PurchasePrice",
                    Width = 120,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "C2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "DaysToExpiry",
                    HeaderText = "Días",
                    Width = 60,
                    ReadOnly = true,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                }
            });

            dgvAvailableStock.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvAvailableStock.EnableHeadersVisualStyles = false;
            dgvAvailableStock.CellFormatting += DgvAvailableStock_CellFormatting;
            dgvAvailableStock.DataBindingComplete += DgvAvailableStock_DataBindingComplete;
        }

        private void DgvAvailableStock_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Calcular y asignar valores para la columna DaysToExpiry después del data binding
            foreach (DataGridViewRow row in dgvAvailableStock.Rows)
            {
                var batch = row.DataBoundItem as Batch;
                if (batch != null)
                {
                    var daysToExpiry = (batch.ExpiryDate - DateTime.Now).Days;
                    row.Cells["DaysToExpiry"].Value = daysToExpiry;
                }
            }
        }

        private void DgvAvailableStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var batch = dgvAvailableStock.Rows[e.RowIndex].DataBoundItem as Batch;
            if (batch == null) return;

            var daysToExpiry = (batch.ExpiryDate - DateTime.Now).Days;

            // Colorear filas según proximidad al vencimiento
            if (daysToExpiry <= 0)
            {
                e.CellStyle.BackColor = Color.FromArgb(231, 76, 60);
                e.CellStyle.ForeColor = Color.White;
            }
            else if (daysToExpiry <= 30)
            {
                e.CellStyle.BackColor = Color.FromArgb(243, 156, 18);
                e.CellStyle.ForeColor = Color.White;
            }
            else if (daysToExpiry <= 90)
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 235, 59);
                e.CellStyle.ForeColor = Color.Black;
            }
        }

        private async void InventoryExitForm_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await LoadComboBoxDataAsync();
                cmbProduct.Focus();
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
            // Cargar productos con stock disponible
            var products = await _productService.GetAllProductsAsync();
            cmbProduct.Items.Clear();
            cmbProduct.Items.Add(new ComboBoxItem { Text = "-- Seleccionar Producto --", Value = null });

            foreach (var product in products.Where(p => p.IsActive))
            {
                var totalStock = await _inventoryService.GetTotalStockByProductAsync(product.Id);
                if (totalStock > 0)
                {
                    cmbProduct.Items.Add(new ComboBoxItem
                    {
                        Text = $"{product.Name} (Stock: {totalStock})",
                        Value = product
                    });
                }
            }

            // Cargar almacenes
            var warehouses = await _warehouseRepository.GetAllAsync();
            cmbWarehouse.Items.Clear();
            cmbWarehouse.Items.Add(new ComboBoxItem { Text = "-- Seleccionar Almacén --", Value = null });

            foreach (var warehouse in warehouses.Where(w => w.IsActive))
            {
                cmbWarehouse.Items.Add(new ComboBoxItem
                {
                    Text = warehouse.Name,
                    Value = warehouse
                });
            }
        }

        private async void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbProduct.SelectedItem is ComboBoxItem item) || item.Value == null)
            {
                _selectedProduct = null;
                pnlProductInfo.Visible = false;
                return;
            }

            _selectedProduct = item.Value as Product;
            await LoadProductBatchesAsync();
        }

        private async Task LoadProductBatchesAsync()
        {
            if (_selectedProduct == null) return;

            try
            {
                _availableBatches = (await _inventoryService.GetBatchesByProductAsync(_selectedProduct.Id))
                    .Where(b => b.CurrentStock > 0)
                    .OrderBy(b => b.ExpiryDate)
                    .ToList();

                dgvAvailableStock.DataSource = _availableBatches;

                lblProductInfo.Text = $"Stock disponible de: {_selectedProduct.Name}";
                pnlProductInfo.Visible = true;

                // Configurar cantidad máxima basada en stock total
                var totalStock = _availableBatches.Sum(b => b.CurrentStock);
                nudQuantity.Maximum = totalStock;
                nudQuantity.Value = 1;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar información del producto: {ex.Message}", MessageType.Error);
            }
        }

        private void BtnSearchProduct_Click(object sender, EventArgs e)
        {
            var searchDialog = new ProductSearchDialog();
            if (searchDialog.ShowDialog() == DialogResult.OK)
            {
                var selectedProduct = searchDialog.SelectedProduct;
                if (selectedProduct != null)
                {
                    // Buscar el producto en el combo y seleccionarlo
                    for (int i = 1; i < cmbProduct.Items.Count; i++)
                    {
                        var item = cmbProduct.Items[i] as ComboBoxItem;
                        if (item?.Value is Product product && product.Id == selectedProduct.Id)
                        {
                            cmbProduct.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void CmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Actualizar información si es necesario
        }

        private void CmbReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool showCustomReason = cmbReason.SelectedItem?.ToString() == "Otro";
            lblCustomReason.Visible = showCustomReason;
            txtCustomReason.Visible = showCustomReason;

            if (showCustomReason)
            {
                txtCustomReason.Focus();
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                btnSave.Enabled = false;
                btnSave.Text = "Procesando...";

                var warehouse = (cmbWarehouse.SelectedItem as ComboBoxItem)?.Value as Warehouse;
                var quantity = (int)nudQuantity.Value;
                var reason = cmbReason.SelectedItem?.ToString() == "Otro"
                    ? txtCustomReason.Text.Trim()
                    : cmbReason.SelectedItem?.ToString();

                if (!string.IsNullOrWhiteSpace(txtReference.Text))
                {
                    reason += $" - Ref: {txtReference.Text.Trim()}";
                }

                var username = SessionManager.CurrentUser?.Username ?? "Sistema";

                var success = await _inventoryService.ProcessExitAsync(
                    _selectedProduct.Id,
                    warehouse.Id,
                    quantity,
                    reason,
                    username);

                if (success)
                {
                    ShowMessage("Salida de mercancía registrada exitosamente", MessageType.Success);

                    if (ShowConfirmation("¿Desea registrar otra salida?"))
                    {
                        ClearForm();
                    }
                    else
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    ShowMessage("No se pudo registrar la salida. Verifique que haya suficiente stock disponible.", MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al registrar salida: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnSave.Enabled = true;
                btnSave.Text = "Registrar Salida";
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (HasUnsavedData())
            {
                if (!ShowConfirmation("¿Está seguro que desea cancelar? Se perderán los datos ingresados."))
                    return;
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            if (_selectedProduct == null)
            {
                ShowMessage("Debe seleccionar un producto", MessageType.Warning);
                cmbProduct.Focus();
                return false;
            }

            if (!(cmbWarehouse.SelectedItem is ComboBoxItem warehouseItem) ||
                warehouseItem.Value == null)
            {
                ShowMessage("Debe seleccionar un almacén", MessageType.Warning);
                cmbWarehouse.Focus();
                return false;
            }

            if (nudQuantity.Value <= 0)
            {
                ShowMessage("La cantidad debe ser mayor a cero", MessageType.Warning);
                nudQuantity.Focus();
                return false;
            }

            if (cmbReason.SelectedIndex < 0)
            {
                ShowMessage("Debe seleccionar un motivo de salida", MessageType.Warning);
                cmbReason.Focus();
                return false;
            }

            if (cmbReason.SelectedItem?.ToString() == "Otro" &&
                string.IsNullOrWhiteSpace(txtCustomReason.Text))
            {
                ShowMessage("Debe especificar el motivo personalizado", MessageType.Warning);
                txtCustomReason.Focus();
                return false;
            }

            // Validar que hay suficiente stock
            var totalAvailableStock = _availableBatches?.Sum(b => b.CurrentStock) ?? 0;
            if (nudQuantity.Value > totalAvailableStock)
            {
                ShowMessage($"No hay suficiente stock disponible. Stock actual: {totalAvailableStock}", MessageType.Warning);
                nudQuantity.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            cmbProduct.SelectedIndex = 0;
            cmbWarehouse.SelectedIndex = 0;
            cmbReason.SelectedIndex = -1;
            txtCustomReason.Clear();
            txtReference.Clear();
            nudQuantity.Value = 1;

            _selectedProduct = null;
            _availableBatches = null;
            pnlProductInfo.Visible = false;

            lblCustomReason.Visible = false;
            txtCustomReason.Visible = false;

            cmbProduct.Focus();
        }

        private bool HasUnsavedData()
        {
            return _selectedProduct != null ||
                   cmbWarehouse.SelectedIndex > 0 ||
                   cmbReason.SelectedIndex >= 0 ||
                   !string.IsNullOrWhiteSpace(txtCustomReason.Text) ||
                   !string.IsNullOrWhiteSpace(txtReference.Text) ||
                   nudQuantity.Value > 1;
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
    }
}