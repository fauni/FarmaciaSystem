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
    public partial class InventoryEntryForm : BaseForm
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
        private Label lblLocation;
        private ComboBox cmbLocation;
        private Label lblSupplier;
        private ComboBox cmbSupplier;
        private Label lblBatchNumber;
        private TextBox txtBatchNumber;
        private Label lblExpiryDate;
        private DateTimePicker dtpExpiryDate;
        private Label lblQuantity;
        private NumericUpDown nudQuantity;
        private Label lblPurchasePrice;
        private NumericUpDown nudPurchasePrice;
        private Label lblReference;
        private TextBox txtReference;
        private Label lblReason;
        private TextBox txtReason;
        private Button btnSave;
        private Button btnCancel;

        // Información del producto seleccionado
        private Panel pnlProductInfo;
        private Label lblProductInfo;
        private Label lblCurrentStock;

        private InventoryService _inventoryService;
        private ProductService _productService;
        private WarehouseRepository _warehouseRepository;
        private LocationRepository _locationRepository;
        private SupplierRepository _supplierRepository;

        private Product _selectedProduct;

        public InventoryEntryForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureInventoryEntryForm();
        }

        protected override void CheckPermissions()
        {
            if (!PermissionManager.HasPermission("inventario_entrada"))
            {
                ShowMessage("No tiene permisos para registrar entradas de inventario", MessageType.Warning);
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
            _locationRepository = new LocationRepository();
            _supplierRepository = new SupplierRepository();
        }

        private void ConfigureInventoryEntryForm()
        {
            this.Text = "Entrada de Mercancía";
            this.Size = new Size(700, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            CreateInventoryEntryInterface();
            this.Load += InventoryEntryForm_Load;
        }

        private void CreateInventoryEntryInterface()
        {
            // Panel de encabezado
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(46, 204, 113),
                Padding = new Padding(20, 15, 20, 15)
            };

            lblTitle = new Label
            {
                Text = "Entrada de Mercancía",
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
                Text = "Registrar Entrada",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(46, 204, 113),
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
                Size = new Size(120, 36),
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
            const int spacing = 40;

            // Producto
            lblProduct = new Label
            {
                Text = "Producto: *",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            cmbProduct = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(500, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnSearchProduct = new Button
            {
                Text = "Buscar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, controlHeight),
                Location = new Point(510, yPosition + 20),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearchProduct.FlatAppearance.BorderSize = 0;
            btnSearchProduct.Click += BtnSearchProduct_Click;

            yPosition += spacing;

            // Panel de información del producto
            pnlProductInfo = new Panel
            {
                Location = new Point(0, yPosition),
                Size = new Size(600, 60),
                BackColor = Color.FromArgb(241, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Visible = false
            };

            lblProductInfo = new Label
            {
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblCurrentStock = new Label
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Dock = DockStyle.Bottom,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlProductInfo.Controls.AddRange(new Control[] { lblProductInfo, lblCurrentStock });

            yPosition += 70;

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
                Size = new Size(290, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Ubicación
            lblLocation = new Label
            {
                Text = "Ubicación: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(310, yPosition),
                Size = new Size(100, labelHeight)
            };

            cmbLocation = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(310, yPosition + 20),
                Size = new Size(280, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            yPosition += spacing;

            // Proveedor
            lblSupplier = new Label
            {
                Text = "Proveedor:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            cmbSupplier = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(590, controlHeight),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            yPosition += spacing;

            // Número de lote
            lblBatchNumber = new Label
            {
                Text = "Número de Lote: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(150, labelHeight)
            };

            txtBatchNumber = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(290, controlHeight),
                MaxLength = 50,
                // PlaceholderText = "Ej: LOT2024001"
            };

            // Fecha de vencimiento
            lblExpiryDate = new Label
            {
                Text = "Fecha de Vencimiento: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(310, yPosition),
                Size = new Size(200, labelHeight)
            };

            dtpExpiryDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(310, yPosition + 20),
                Size = new Size(280, controlHeight),
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today.AddDays(1),
                Value = DateTime.Today.AddYears(2)
            };

            yPosition += spacing;

            // Cantidad
            lblQuantity = new Label
            {
                Text = "Cantidad: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            nudQuantity = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(290, controlHeight),
                Maximum = 999999,
                Minimum = 1,
                Value = 1
            };

            // Precio de compra
            lblPurchasePrice = new Label
            {
                Text = "Precio de Compra: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(310, yPosition),
                Size = new Size(150, labelHeight)
            };

            nudPurchasePrice = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(310, yPosition + 20),
                Size = new Size(280, controlHeight),
                DecimalPlaces = 2,
                Maximum = 999999.99m,
                Minimum = 0.01m,
                Value = 1.00m
            };

            yPosition += spacing;

            // Referencia
            lblReference = new Label
            {
                Text = "Referencia:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            txtReference = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(290, controlHeight),
                MaxLength = 100,
                // PlaceholderText = "Factura, orden de compra..."
            };

            yPosition += spacing;

            // Motivo
            lblReason = new Label
            {
                Text = "Motivo:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(100, labelHeight)
            };

            txtReason = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 20),
                Size = new Size(590, controlHeight * 2),
                MaxLength = 200,
                Multiline = true,
                // PlaceholderText = "Descripción del motivo de la entrada..."
            };

            // Agregar controles al panel principal
            pnlMain.Controls.AddRange(new Control[]
            {
                lblProduct, cmbProduct, btnSearchProduct,
                pnlProductInfo,
                lblWarehouse, cmbWarehouse,
                lblLocation, cmbLocation,
                lblSupplier, cmbSupplier,
                lblBatchNumber, txtBatchNumber,
                lblExpiryDate, dtpExpiryDate,
                lblQuantity, nudQuantity,
                lblPurchasePrice, nudPurchasePrice,
                lblReference, txtReference,
                lblReason, txtReason
            });

            // Eventos
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            cmbWarehouse.SelectedIndexChanged += CmbWarehouse_SelectedIndexChanged;
        }

        private async void InventoryEntryForm_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await LoadComboBoxDataAsync();

                // Configurar valores por defecto
                txtReason.Text = "Entrada de mercancía";
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
            // Cargar productos
            var products = await _productService.GetAllProductsAsync();
            cmbProduct.Items.Clear();
            cmbProduct.Items.Add(new ComboBoxItem { Text = "-- Seleccionar Producto --", Value = null });
            foreach (var product in products.Where(p => p.IsActive))
            {
                var displayText = $"{product.Name}";
                if (!string.IsNullOrEmpty(product.Barcode))
                    displayText += $" - {product.Barcode}";
                if (!string.IsNullOrEmpty(product.Concentration))
                    displayText += $" ({product.Concentration})";

                cmbProduct.Items.Add(new ComboBoxItem { Text = displayText, Value = product });
            }
            cmbProduct.SelectedIndex = 0;

            // Cargar almacenes
            var warehouses = await _warehouseRepository.GetActiveWarehousesAsync();
            cmbWarehouse.Items.Clear();
            cmbWarehouse.Items.Add(new ComboBoxItem { Text = "-- Seleccionar Almacén --", Value = null });
            foreach (var warehouse in warehouses)
            {
                cmbWarehouse.Items.Add(new ComboBoxItem { Text = warehouse.Name, Value = warehouse });
            }
            cmbWarehouse.SelectedIndex = 0;

            // Cargar proveedores
            var suppliers = await _supplierRepository.GetAllAsync();
            cmbSupplier.Items.Clear();
            cmbSupplier.Items.Add(new ComboBoxItem { Text = "-- Seleccionar Proveedor --", Value = null });
            foreach (var supplier in suppliers)
            {
                cmbSupplier.Items.Add(new ComboBoxItem { Text = supplier.Name, Value = supplier });
            }
            cmbSupplier.SelectedIndex = 0;
        }

        private async void CmbWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbLocation.Items.Clear();
            cmbLocation.Items.Add(new ComboBoxItem { Text = "-- Seleccionar Ubicación --", Value = null });
            cmbLocation.SelectedIndex = 0;
            cmbLocation.Enabled = false;

            if (cmbWarehouse.SelectedItem is ComboBoxItem warehouseItem &&
                warehouseItem.Value is Warehouse warehouse)
            {
                try
                {
                    var locations = await _locationRepository.GetLocationsByWarehouseAsync(warehouse.Id);
                    foreach (var location in locations)
                    {
                        var displayText = $"{location.Code}";
                        if (!string.IsNullOrEmpty(location.Description))
                            displayText += $" - {location.Description}";

                        cmbLocation.Items.Add(new ComboBoxItem { Text = displayText, Value = location });
                    }
                    cmbLocation.Enabled = cmbLocation.Items.Count > 1;
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error al cargar ubicaciones: {ex.Message}", MessageType.Error);
                }
            }
        }

        private async void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem is ComboBoxItem productItem &&
                productItem.Value is Product product)
            {
                _selectedProduct = product;
                await ShowProductInfoAsync(product);

                // Establecer precio de compra sugerido si está disponible
                if (product.PurchasePrice > 0)
                {
                    nudPurchasePrice.Value = product.PurchasePrice;
                }
            }
            else
            {
                _selectedProduct = null;
                HideProductInfo();
            }
        }

        private async Task ShowProductInfoAsync(Product product)
        {
            try
            {
                lblProductInfo.Text = $"Producto: {product.Name} | Precio Venta: {product.SalePrice:C2} | Stock Min/Max: {product.MinStock}/{product.MaxStock}";

                var totalStock = await _inventoryService.GetTotalStockByProductAsync(product.Id);
                lblCurrentStock.Text = $"Stock Total Actual: {totalStock} unidades";

                if (totalStock <= product.MinStock)
                {
                    lblCurrentStock.ForeColor = Color.FromArgb(231, 76, 60);
                    lblCurrentStock.Text += " (STOCK BAJO)";
                }
                else
                {
                    lblCurrentStock.ForeColor = Color.FromArgb(46, 204, 113);
                }

                pnlProductInfo.Visible = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al obtener información del producto: {ex.Message}", MessageType.Warning);
            }
        }

        private void HideProductInfo()
        {
            pnlProductInfo.Visible = false;
        }

        private void BtnSearchProduct_Click(object sender, EventArgs e)
        {
            using (var searchDialog = new ProductSearchDialog())
            {
                if (searchDialog.ShowDialog() == DialogResult.OK && searchDialog.SelectedProduct != null)
                {
                    // Buscar el producto en el combo y seleccionarlo
                    for (int i = 0; i < cmbProduct.Items.Count; i++)
                    {
                        if (cmbProduct.Items[i] is ComboBoxItem item &&
                            item.Value is Product product &&
                            product.Id == searchDialog.SelectedProduct.Id)
                        {
                            cmbProduct.SelectedIndex = i;
                            break;
                        }
                    }
                }
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

                var batch = CreateBatchFromForm();
                var username = CurrentUser?.Username ?? "Sistema";
                var reference = txtReference.Text.Trim();

                var success = await _inventoryService.ProcessEntryAsync(batch, username, reference);

                if (success)
                {
                    ShowMessage("Entrada de mercancía registrada exitosamente", MessageType.Success);

                    if (ShowConfirmation("¿Desea registrar otra entrada?"))
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
                    ShowMessage("No se pudo registrar la entrada de mercancía", MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al registrar entrada: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnSave.Enabled = true;
                btnSave.Text = "Registrar Entrada";
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
                !(warehouseItem.Value is Warehouse))
            {
                ShowMessage("Debe seleccionar un almacén", MessageType.Warning);
                cmbWarehouse.Focus();
                return false;
            }

            if (!(cmbLocation.SelectedItem is ComboBoxItem locationItem) ||
                !(locationItem.Value is Location))
            {
                ShowMessage("Debe seleccionar una ubicación", MessageType.Warning);
                cmbLocation.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBatchNumber.Text))
            {
                ShowMessage("Debe ingresar el número de lote", MessageType.Warning);
                txtBatchNumber.Focus();
                return false;
            }

            if (dtpExpiryDate.Value <= DateTime.Today)
            {
                ShowMessage("La fecha de vencimiento debe ser futura", MessageType.Warning);
                dtpExpiryDate.Focus();
                return false;
            }

            if (nudQuantity.Value <= 0)
            {
                ShowMessage("La cantidad debe ser mayor a cero", MessageType.Warning);
                nudQuantity.Focus();
                return false;
            }

            if (nudPurchasePrice.Value <= 0)
            {
                ShowMessage("El precio de compra debe ser mayor a cero", MessageType.Warning);
                nudPurchasePrice.Focus();
                return false;
            }

            return true;
        }

        private Batch CreateBatchFromForm()
        {
            var warehouse = ((ComboBoxItem)cmbWarehouse.SelectedItem).Value as Warehouse;
            var location = ((ComboBoxItem)cmbLocation.SelectedItem).Value as Location;
            var supplier = cmbSupplier.SelectedItem is ComboBoxItem supplierItem ?
                          supplierItem.Value as Supplier : null;

            return new Batch
            {
                ProductId = _selectedProduct.Id,
                WarehouseId = warehouse.Id,
                LocationId = location.Id,
                BatchNumber = txtBatchNumber.Text.Trim(),
                ExpiryDate = dtpExpiryDate.Value.Date,
                CurrentStock = (int)nudQuantity.Value,
                PurchasePrice = nudPurchasePrice.Value,
                SupplierId = supplier?.Id,
                EntryDate = DateTime.Now
            };
        }

        private void ClearForm()
        {
            cmbProduct.SelectedIndex = 0;
            cmbWarehouse.SelectedIndex = 0;
            cmbLocation.SelectedIndex = 0;
            cmbSupplier.SelectedIndex = 0;
            txtBatchNumber.Clear();
            dtpExpiryDate.Value = DateTime.Today.AddYears(2);
            nudQuantity.Value = 1;
            nudPurchasePrice.Value = 1.00m;
            txtReference.Clear();
            txtReason.Text = "Entrada de mercancía";
            HideProductInfo();
            _selectedProduct = null;
            cmbProduct.Focus();
        }

        private bool HasUnsavedData()
        {
            return cmbProduct.SelectedIndex > 0 ||
                   cmbWarehouse.SelectedIndex > 0 ||
                   !string.IsNullOrWhiteSpace(txtBatchNumber.Text) ||
                   nudQuantity.Value > 1 ||
                   nudPurchasePrice.Value > 1.00m ||
                   !string.IsNullOrWhiteSpace(txtReference.Text);
        }
    }
}
