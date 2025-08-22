using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Forms.Base;
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

namespace FarmaciaSystem.Forms.Products
{
    public partial class ProductListForm : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlSearch;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClear;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private DataGridView dgvProducts;
        private Panel pnlPagination;
        private Button btnFirst;
        private Button btnPrevious;
        private Label lblPageInfo;
        private Button btnNext;
        private Button btnLast;
        private ComboBox cmbPageSize;

        private ProductService _productService;
        private BindingSource _bindingSource;
        private List<Product> _allProducts;
        private List<Product> _filteredProducts;
        private int _currentPage = 1;
        private int _pageSize = 50;
        private int _totalPages = 1;

        public ProductListForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureProductListForm();
        }

        protected override void CheckPermissions()
        {
            if (!PermissionManager.HasPermission("productos_consultar"))
            {
                ShowMessage("No tiene permisos para acceder a esta función", MessageType.Warning);
                this.Close();
                return;
            }

            btnAdd.Enabled = PermissionManager.HasPermission("productos_crear");
            btnEdit.Enabled = PermissionManager.HasPermission("productos_editar");
            btnDelete.Enabled = PermissionManager.HasPermission("productos_eliminar");
        }

        private void InitializeServices()
        {
            var productRepository = new ProductRepository();
            var productValidator = new Business.Validators.ProductValidator(productRepository);
            _productService = new ProductService(productRepository, productValidator);
            _bindingSource = new BindingSource();
        }

        private void ConfigureProductListForm()
        {
            this.Text = "Gestión de Productos";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            CreateProductListInterface();
            this.Load += ProductListForm_Load;
        }

        private void CreateProductListInterface()
        {
            // Panel de encabezado
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            lblTitle = new Label
            {
                Text = "Gestión de Productos",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Left,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlHeader.Controls.Add(lblTitle);

            // Panel de búsqueda y acciones
            pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Controles de búsqueda
            var lblSearch = new Label
            {
                Text = "Buscar:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 15),
                Size = new Size(60, 20)
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(65, 12),
                Size = new Size(300, 25),
                // PlaceholderText = "Buscar por nombre, código de barras o principio activo..."
            };

            btnSearch = new Button
            {
                Text = "Buscar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 30),
                Location = new Point(375, 10),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;

            btnClear = new Button
            {
                Text = "Limpiar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 30),
                Location = new Point(465, 10),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;

            // Botones de acción
            btnAdd = new Button
            {
                Text = "Nuevo",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Size = new Size(100, 35),
                Location = new Point(600, 8),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;

            btnEdit = new Button
            {
                Text = "Editar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 35),
                Location = new Point(710, 8),
                BackColor = Color.FromArgb(243, 156, 18),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;

            btnDelete = new Button
            {
                Text = "Eliminar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 35),
                Location = new Point(800, 8),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;

            btnRefresh = new Button
            {
                Text = "Actualizar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 35),
                Location = new Point(890, 8),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;

            pnlSearch.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, btnClear,
                btnAdd, btnEdit, btnDelete, btnRefresh
            });

            // DataGridView
            dgvProducts = new DataGridView
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
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 249, 250)
                }
            };

            ConfigureDataGridViewColumns();

            // Panel de paginación
            pnlPagination = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            btnFirst = new Button
            {
                Text = "<<",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(40, 30),
                Location = new Point(20, 10),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnPrevious = new Button
            {
                Text = "<",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(40, 30),
                Location = new Point(70, 10),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            lblPageInfo = new Label
            {
                Text = "Página 1 de 1",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(120, 15),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnNext = new Button
            {
                Text = ">",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(40, 30),
                Location = new Point(330, 10),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnLast = new Button
            {
                Text = ">>",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(40, 30),
                Location = new Point(380, 10),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            var lblPageSize = new Label
            {
                Text = "Mostrar:",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(450, 15),
                Size = new Size(60, 20)
            };

            cmbPageSize = new ComboBox
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(515, 12),
                Size = new Size(80, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPageSize.Items.AddRange(new object[] { 25, 50, 100, 200 });
            cmbPageSize.SelectedItem = _pageSize;

            pnlPagination.Controls.AddRange(new Control[] {
                btnFirst, btnPrevious, lblPageInfo, btnNext, btnLast, lblPageSize, cmbPageSize
            });

            // Agregar eventos
            btnSearch.Click += BtnSearch_Click;
            btnClear.Click += BtnClear_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;
            txtSearch.KeyPress += TxtSearch_KeyPress;
            dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;
            btnFirst.Click += (s, e) => NavigateToPage(1);
            btnPrevious.Click += (s, e) => NavigateToPage(_currentPage - 1);
            btnNext.Click += (s, e) => NavigateToPage(_currentPage + 1);
            btnLast.Click += (s, e) => NavigateToPage(_totalPages);
            cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;

            // Agregar controles al formulario
            this.Controls.Add(dgvProducts);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlHeader);
        }

        private void ConfigureDataGridViewColumns()
        {
            dgvProducts.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Id",
                    HeaderText = "ID",
                    DataPropertyName = "Id",
                    Width = 50,
                    Visible = false
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Barcode",
                    HeaderText = "Código",
                    DataPropertyName = "Barcode",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Name",
                    HeaderText = "Nombre",
                    DataPropertyName = "Name",
                    Width = 250,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Concentration",
                    HeaderText = "Concentración",
                    DataPropertyName = "Concentration",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "SalePrice",
                    HeaderText = "Precio Venta",
                    DataPropertyName = "SalePrice",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "C2",
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "MinStock",
                    HeaderText = "Stock Mín.",
                    DataPropertyName = "MinStock",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "MaxStock",
                    HeaderText = "Stock Máx.",
                    DataPropertyName = "MaxStock",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleRight
                    }
                },
                new DataGridViewCheckBoxColumn
                {
                    Name = "RequiresPrescription",
                    HeaderText = "Prescripción",
                    DataPropertyName = "RequiresPrescription",
                    Width = 90
                },
                new DataGridViewCheckBoxColumn
                {
                    Name = "IsActive",
                    HeaderText = "Activo",
                    DataPropertyName = "IsActive",
                    Width = 60
                }
            });

            // Configurar estilos de encabezado
            dgvProducts.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvProducts.EnableHeadersVisualStyles = false;
        }

        private async void ProductListForm_Load(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                dgvProducts.DataSource = null;

                _allProducts = (await _productService.GetAllProductsAsync()).ToList();
                _filteredProducts = _allProducts.ToList();

                UpdatePagination();
                LoadCurrentPage();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar productos: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdatePagination()
        {
            if (_filteredProducts == null || !_filteredProducts.Any())
            {
                _totalPages = 1;
                _currentPage = 1;
            }
            else
            {
                _totalPages = (int)Math.Ceiling((double)_filteredProducts.Count / _pageSize);
                if (_currentPage > _totalPages)
                    _currentPage = _totalPages;
                if (_currentPage < 1)
                    _currentPage = 1;
            }

            lblPageInfo.Text = $"Página {_currentPage} de {_totalPages} ({_filteredProducts?.Count ?? 0} productos)";

            btnFirst.Enabled = _currentPage > 1;
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
            btnLast.Enabled = _currentPage < _totalPages;
        }

        private void LoadCurrentPage()
        {
            if (_filteredProducts == null || !_filteredProducts.Any())
            {
                dgvProducts.DataSource = null;
                return;
            }

            var skip = (_currentPage - 1) * _pageSize;
            var pageData = _filteredProducts.Skip(skip).Take(_pageSize).ToList();

            _bindingSource.DataSource = pageData;
            dgvProducts.DataSource = _bindingSource;
        }

        private void NavigateToPage(int page)
        {
            if (page >= 1 && page <= _totalPages && page != _currentPage)
            {
                _currentPage = page;
                UpdatePagination();
                LoadCurrentPage();
            }
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnSearch_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            _filteredProducts = _allProducts.ToList();
            _currentPage = 1;
            UpdatePagination();
            LoadCurrentPage();
        }

        private void PerformSearch()
        {
            var searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                _filteredProducts = _allProducts.ToList();
            }
            else
            {
                _filteredProducts = _allProducts.Where(p =>
                    (p.Name?.ToLower().Contains(searchTerm.ToLower()) == true) ||
                    (p.Barcode?.ToLower().Contains(searchTerm.ToLower()) == true) ||
                    (p.Concentration?.ToLower().Contains(searchTerm.ToLower()) == true)
                ).ToList();
            }

            _currentPage = 1;
            UpdatePagination();
            LoadCurrentPage();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ProductEditForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _ = LoadProductsAsync();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            EditSelectedProduct();
        }

        private void DgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                EditSelectedProduct();
            }
        }

        private void EditSelectedProduct()
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                ShowMessage("Debe seleccionar un producto para editar", MessageType.Warning);
                return;
            }

            var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
            if (selectedProduct == null) return;

            using (var form = new ProductEditForm(selectedProduct.Id))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _ = LoadProductsAsync();
                }
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                ShowMessage("Debe seleccionar un producto para eliminar", MessageType.Warning);
                return;
            }

            var selectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
            if (selectedProduct == null) return;

            if (!ShowConfirmation($"¿Está seguro que desea eliminar el producto '{selectedProduct.Name}'?"))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var success = await _productService.DeleteProductAsync(selectedProduct.Id);
                if (success)
                {
                    ShowMessage("Producto eliminado exitosamente", MessageType.Success);
                    await LoadProductsAsync();
                }
                else
                {
                    ShowMessage("No se pudo eliminar el producto", MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al eliminar producto: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private void CmbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPageSize.SelectedItem != null)
            {
                _pageSize = (int)cmbPageSize.SelectedItem;
                _currentPage = 1;
                UpdatePagination();
                LoadCurrentPage();
            }
        }
    }
}
