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

namespace FarmaciaSystem.Forms.Inventory
{
    public partial class ProductSearchDialog : BaseForm
    {
        private TextBox txtSearch;
        private DataGridView dgvProducts;
        private Button btnSelect;
        private Button btnCancel;

        public Product SelectedProduct { get; private set; }

        public ProductSearchDialog()
        {
            InitializeComponent();
            ConfigureSearchDialog();
        }

        private void ConfigureSearchDialog()
        {
            this.Text = "Buscar Producto";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateSearchInterface();
        }

        private void CreateSearchInterface()
        {
            var lblSearch = new Label
            {
                Text = "Buscar:",
                Location = new Point(20, 20),
                Size = new Size(60, 20)
            };

            txtSearch = new TextBox
            {
                Location = new Point(85, 18),
                Size = new Size(400, 25),
                // PlaceholderText = "Nombre, código de barras..."
            };

            var btnSearch = new Button
            {
                Text = "Buscar",
                Location = new Point(495, 16),
                Size = new Size(70, 29),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.Click += async (s, e) => await SearchProducts();

            dgvProducts = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(545, 250),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgvProducts.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nombre", DataPropertyName = "Name", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Barcode", HeaderText = "Código", DataPropertyName = "Barcode", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Concentration", HeaderText = "Concentración", DataPropertyName = "Concentration", Width = 100 },
                new DataGridViewTextBoxColumn
                {
                    Name = "SalePrice",
                    HeaderText = "Precio",
                    DataPropertyName = "SalePrice",
                    Width = 80,
                    DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                }
            });

            btnSelect = new Button
            {
                Text = "Seleccionar",
                Location = new Point(390, 325),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnSelect.Click += BtnSelect_Click;

            btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(490, 325),
                Size = new Size(75, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            dgvProducts.SelectionChanged += (s, e) => {
                btnSelect.Enabled = dgvProducts.SelectedRows.Count > 0;
            };

            dgvProducts.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) BtnSelect_Click(s, e);
            };

            txtSearch.KeyPress += async (s, e) => {
                if (e.KeyChar == (char)Keys.Enter) await SearchProducts();
            };

            this.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, dgvProducts, btnSelect, btnCancel
            });
        }

        private async Task SearchProducts()
        {
            try
            {
                var productService = new ProductService(new ProductRepository(),
                    new Business.Validators.ProductValidator(new ProductRepository()));

                var products = string.IsNullOrWhiteSpace(txtSearch.Text) ?
                    await productService.GetAllProductsAsync() :
                    await productService.SearchProductsAsync(txtSearch.Text.Trim());

                dgvProducts.DataSource = products.Where(p => p.IsActive).ToList();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al buscar productos: {ex.Message}", MessageType.Error);
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                SelectedProduct = dgvProducts.SelectedRows[0].DataBoundItem as Product;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
