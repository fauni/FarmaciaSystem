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

namespace FarmaciaSystem.Forms.Configuration
{
    public partial class LocationManagerForm : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Panel pnlMain;
        private Panel pnlLeft;
        private Panel pnlRight;
        private Splitter splitter;

        // Controles del lado izquierdo - Lista de almacenes
        private Label lblWarehouses;
        private ListBox lstWarehouses;
        private Button btnRefreshWarehouses;

        // Controles del lado derecho - Ubicaciones del almacén seleccionado
        private Label lblLocations;
        private DataGridView dgvLocations;
        private Panel pnlLocationButtons;
        private Button btnAddLocation;
        private Button btnEditLocation;
        private Button btnDeleteLocation;

        // Formulario de edición de ubicación
        private Panel pnlLocationForm;
        private Label lblLocationFormTitle;
        private Label lblCode;
        private TextBox txtCode;
        private Label lblDescription;
        private TextBox txtDescription;
        private Button btnSaveLocation;
        private Button btnCancelLocation;

        private WarehouseRepository _warehouseRepository;
        private LocationRepository _locationRepository;

        private List<Warehouse> _warehouses;
        private List<Location> _locations;
        private Warehouse _selectedWarehouse;
        private Location _editingLocation;
        private bool _isAddingLocation;

        public LocationManagerForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureLocationManagerForm();
        }

        protected override void CheckPermissions()
        {
            if (!PermissionManager.HasPermission("config_almacenes"))
            {
                ShowMessage("No tiene permisos para gestionar ubicaciones", MessageType.Warning);
                this.Close();
            }
        }

        private void InitializeServices()
        {
            _warehouseRepository = new WarehouseRepository();
            _locationRepository = new LocationRepository();
        }

        private void ConfigureLocationManagerForm()
        {
            this.Text = "Gestión de Ubicaciones";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            CreateLocationManagerInterface();
            this.Load += LocationManagerForm_Load;
        }

        private void CreateLocationManagerInterface()
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
                Text = "Gestión de Ubicaciones",
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
                Padding = new Padding(10)
            };

            // Panel izquierdo - Almacenes
            pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10)
            };

            CreateWarehouseControls();

            // Splitter
            splitter = new Splitter
            {
                Dock = DockStyle.Left,
                Width = 3,
                BackColor = Color.FromArgb(149, 165, 166)
            };

            // Panel derecho - Ubicaciones
            pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            CreateLocationControls();

            // Agregar controles
            pnlMain.Controls.Add(pnlRight);
            pnlMain.Controls.Add(splitter);
            pnlMain.Controls.Add(pnlLeft);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlHeader);
        }

        private void CreateWarehouseControls()
        {
            lblWarehouses = new Label
            {
                Text = "Almacenes",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.BottomLeft
            };

            lstWarehouses = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = SelectionMode.One
            };
            lstWarehouses.SelectedIndexChanged += LstWarehouses_SelectedIndexChanged;

            btnRefreshWarehouses = new Button
            {
                Text = "Actualizar",
                Font = new Font("Segoe UI", 9F),
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefreshWarehouses.FlatAppearance.BorderSize = 0;
            btnRefreshWarehouses.Click += BtnRefreshWarehouses_Click;

            pnlLeft.Controls.Add(lstWarehouses);
            pnlLeft.Controls.Add(btnRefreshWarehouses);
            pnlLeft.Controls.Add(lblWarehouses);
        }

        private void CreateLocationControls()
        {
            lblLocations = new Label
            {
                Text = "Ubicaciones - Seleccione un almacén",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.BottomLeft
            };

            dgvLocations = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 249, 250)
                }
            };

            ConfigureLocationGridColumns();
            dgvLocations.SelectionChanged += DgvLocations_SelectionChanged;
            dgvLocations.CellDoubleClick += DgvLocations_CellDoubleClick;

            // Panel de botones para ubicaciones
            pnlLocationButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(10, 8, 10, 8)
            };

            btnAddLocation = new Button
            {
                Text = "Nueva Ubicación",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Size = new Size(130, 34),
                Location = new Point(10, 8),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnAddLocation.FlatAppearance.BorderSize = 0;
            btnAddLocation.Click += BtnAddLocation_Click;

            btnEditLocation = new Button
            {
                Text = "Editar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 34),
                Location = new Point(150, 8),
                BackColor = Color.FromArgb(243, 156, 18),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnEditLocation.FlatAppearance.BorderSize = 0;
            btnEditLocation.Click += BtnEditLocation_Click;

            btnDeleteLocation = new Button
            {
                Text = "Eliminar",
                Font = new Font("Segoe UI", 9F),
                Size = new Size(80, 34),
                Location = new Point(240, 8),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDeleteLocation.FlatAppearance.BorderSize = 0;
            btnDeleteLocation.Click += BtnDeleteLocation_Click;

            pnlLocationButtons.Controls.AddRange(new Control[] {
                btnAddLocation, btnEditLocation, btnDeleteLocation
            });

            // Panel de formulario de ubicación (oculto inicialmente)
            pnlLocationForm = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = Color.FromArgb(241, 243, 244),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20),
                Visible = false
            };

            CreateLocationForm();

            pnlRight.Controls.Add(dgvLocations);
            pnlRight.Controls.Add(pnlLocationForm);
            pnlRight.Controls.Add(pnlLocationButtons);
            pnlRight.Controls.Add(lblLocations);
        }

        private void ConfigureLocationGridColumns()
        {
            dgvLocations.Columns.AddRange(new DataGridViewColumn[]
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
                    Name = "Code",
                    HeaderText = "Código",
                    DataPropertyName = "Code",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Descripción",
                    DataPropertyName = "Description",
                    Width = 250,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewCheckBoxColumn
                {
                    Name = "IsActive",
                    HeaderText = "Activo",
                    DataPropertyName = "IsActive",
                    Width = 60
                }
            });

            dgvLocations.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvLocations.EnableHeadersVisualStyles = false;
        }

        private void CreateLocationForm()
        {
            lblLocationFormTitle = new Label
            {
                Text = "Nueva Ubicación",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.BottomLeft
            };

            int yPosition = 50;
            const int spacing = 60;

            lblCode = new Label
            {
                Text = "Código de Ubicación: *",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(300, 20)
            };

            txtCode = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 25),
                Size = new Size(300, 25),
                MaxLength = 20,
                // PlaceholderText = "Ej: A1, REF1, VIT1"
            };

            yPosition += spacing;

            lblDescription = new Label
            {
                Text = "Descripción:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition),
                Size = new Size(300, 20)
            };

            txtDescription = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, yPosition + 25),
                Size = new Size(300, 60),
                MaxLength = 200,
                Multiline = true,
                // PlaceholderText = "Descripción detallada de la ubicación..."
            };

            yPosition += 100;

            btnSaveLocation = new Button
            {
                Text = "Guardar",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(140, 35),
                Location = new Point(0, yPosition),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSaveLocation.FlatAppearance.BorderSize = 0;
            btnSaveLocation.Click += BtnSaveLocation_Click;

            btnCancelLocation = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(140, 35),
                Location = new Point(160, yPosition),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelLocation.FlatAppearance.BorderSize = 0;
            btnCancelLocation.Click += BtnCancelLocation_Click;

            pnlLocationForm.Controls.AddRange(new Control[] {
                lblLocationFormTitle, lblCode, txtCode, lblDescription, txtDescription,
                btnSaveLocation, btnCancelLocation
            });
        }

        private async void LocationManagerForm_Load(object sender, EventArgs e)
        {
            await LoadWarehousesAsync();
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _warehouses = (await _warehouseRepository.GetActiveWarehousesAsync()).ToList();

                lstWarehouses.Items.Clear();
                foreach (var warehouse in _warehouses)
                {
                    lstWarehouses.Items.Add(new ListBoxItem
                    {
                        Text = $"{warehouse.Name} ({warehouse.Location})",
                        Value = warehouse
                    });
                }

                if (lstWarehouses.Items.Count > 0)
                {
                    lstWarehouses.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar almacenes: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void LstWarehouses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWarehouses.SelectedItem is ListBoxItem selectedItem &&
                selectedItem.Value is Warehouse warehouse)
            {
                _selectedWarehouse = warehouse;
                lblLocations.Text = $"Ubicaciones - {warehouse.Name}";
                btnAddLocation.Enabled = true;
                await LoadLocationsAsync();
            }
            else
            {
                _selectedWarehouse = null;
                lblLocations.Text = "Ubicaciones - Seleccione un almacén";
                btnAddLocation.Enabled = false;
                dgvLocations.DataSource = null;
            }
        }

        private async Task LoadLocationsAsync()
        {
            if (_selectedWarehouse == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                _locations = (await _locationRepository.GetLocationsByWarehouseAsync(_selectedWarehouse.Id)).ToList();
                dgvLocations.DataSource = _locations;

                UpdateLocationButtons();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar ubicaciones: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DgvLocations_SelectionChanged(object sender, EventArgs e)
        {
            UpdateLocationButtons();
        }

        private void UpdateLocationButtons()
        {
            var hasSelection = dgvLocations.SelectedRows.Count > 0;
            btnEditLocation.Enabled = hasSelection;
            btnDeleteLocation.Enabled = hasSelection;
        }

        private void DgvLocations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                BtnEditLocation_Click(sender, e);
            }
        }

        private void BtnAddLocation_Click(object sender, EventArgs e)
        {
            if (_selectedWarehouse == null) return;

            _isAddingLocation = true;
            _editingLocation = null;
            ShowLocationForm("Nueva Ubicación");
            ClearLocationForm();
            txtCode.Focus();
        }

        private void BtnEditLocation_Click(object sender, EventArgs e)
        {
            if (dgvLocations.SelectedRows.Count == 0) return;

            var selectedLocation = dgvLocations.SelectedRows[0].DataBoundItem as Location;
            if (selectedLocation == null) return;

            _isAddingLocation = false;
            _editingLocation = selectedLocation;
            ShowLocationForm("Editar Ubicación");
            LoadLocationForm(selectedLocation);
            txtCode.Focus();
        }

        private async void BtnDeleteLocation_Click(object sender, EventArgs e)
        {
            if (dgvLocations.SelectedRows.Count == 0) return;

            var selectedLocation = dgvLocations.SelectedRows[0].DataBoundItem as Location;
            if (selectedLocation == null) return;

            if (!ShowConfirmation($"¿Está seguro que desea eliminar la ubicación '{selectedLocation.Code}'?\n\nEsta acción no se puede deshacer."))
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var success = await _locationRepository.DeleteAsync(selectedLocation.Id);
                if (success)
                {
                    ShowMessage("Ubicación eliminada exitosamente", MessageType.Success);
                    await LoadLocationsAsync();
                }
                else
                {
                    ShowMessage("No se pudo eliminar la ubicación", MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al eliminar ubicación: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnSaveLocation_Click(object sender, EventArgs e)
        {
            if (!ValidateLocationForm()) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                btnSaveLocation.Enabled = false;
                btnSaveLocation.Text = _isAddingLocation ? "Guardando..." : "Actualizando...";

                var location = CreateLocationFromForm();

                if (_isAddingLocation)
                {
                    await _locationRepository.AddAsync(location);
                    ShowMessage("Ubicación creada exitosamente", MessageType.Success);
                }
                else
                {
                    location.Id = _editingLocation.Id;
                    await _locationRepository.UpdateAsync(location);
                    ShowMessage("Ubicación actualizada exitosamente", MessageType.Success);
                }

                HideLocationForm();
                await LoadLocationsAsync();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al guardar ubicación: {ex.Message}", MessageType.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnSaveLocation.Enabled = true;
                btnSaveLocation.Text = "Guardar";
            }
        }

        private void BtnCancelLocation_Click(object sender, EventArgs e)
        {
            if (HasUnsavedLocationChanges())
            {
                if (!ShowConfirmation("¿Está seguro que desea cancelar? Se perderán los cambios."))
                    return;
            }

            HideLocationForm();
        }

        private async void BtnRefreshWarehouses_Click(object sender, EventArgs e)
        {
            await LoadWarehousesAsync();
        }

        private void ShowLocationForm(string title)
        {
            lblLocationFormTitle.Text = title;
            pnlLocationForm.Visible = true;
            dgvLocations.Width = dgvLocations.Width - pnlLocationForm.Width;
        }

        private void HideLocationForm()
        {
            pnlLocationForm.Visible = false;
            dgvLocations.Width = dgvLocations.Parent.Width - 20;
            _editingLocation = null;
        }

        private void ClearLocationForm()
        {
            txtCode.Clear();
            txtDescription.Clear();
        }

        private void LoadLocationForm(Location location)
        {
            txtCode.Text = location.Code ?? "";
            txtDescription.Text = location.Description ?? "";
        }

        private bool ValidateLocationForm()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                ShowMessage("El código de ubicación es requerido", MessageType.Warning);
                txtCode.Focus();
                return false;
            }

            if (txtCode.Text.Trim().Length < 2)
            {
                ShowMessage("El código debe tener al menos 2 caracteres", MessageType.Warning);
                txtCode.Focus();
                return false;
            }

            // Verificar código único en el almacén
            if (_locations != null)
            {
                var duplicateLocation = _locations.FirstOrDefault(l =>
                    l.Code.Equals(txtCode.Text.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    (_isAddingLocation || l.Id != _editingLocation.Id));

                if (duplicateLocation != null)
                {
                    ShowMessage("Ya existe una ubicación con este código en el almacén seleccionado", MessageType.Warning);
                    txtCode.Focus();
                    return false;
                }
            }

            return true;
        }

        private Location CreateLocationFromForm()
        {
            return new Location
            {
                WarehouseId = _selectedWarehouse.Id,
                Code = txtCode.Text.Trim().ToUpper(),
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                IsActive = true
            };
        }

        private bool HasUnsavedLocationChanges()
        {
            if (_isAddingLocation)
            {
                return !string.IsNullOrWhiteSpace(txtCode.Text) ||
                       !string.IsNullOrWhiteSpace(txtDescription.Text);
            }
            else if (_editingLocation != null)
            {
                return txtCode.Text.Trim() != (_editingLocation.Code ?? "") ||
                       txtDescription.Text.Trim() != (_editingLocation.Description ?? "");
            }

            return false;
        }
    }

    // Clase auxiliar para ListBox
    public class ListBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
