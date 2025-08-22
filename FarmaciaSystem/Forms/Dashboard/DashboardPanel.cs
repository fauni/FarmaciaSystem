using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Data.Repositories;
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

namespace FarmaciaSystem.Forms.Dashboard
{
    public partial class DashboardPanel : UserControl
    {
        private Panel pnlHeader;
        private Label lblWelcome;
        private Label lblDateTime;
        private Panel pnlCards;
        private Panel cardTotalProducts;
        private Panel cardLowStock;
        private Panel cardExpiring;
        private Panel cardRecentMovements;
        private Panel pnlAlerts;
        private Label lblAlertsTitle;
        private ListBox lstAlerts;
        private Timer refreshTimer;

        private ProductService _productService;
        private InventoryService _inventoryService;
        private AlertService _alertService;

        private bool _isLoaded = false;

        public DashboardPanel()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureDashboard();
            StartRefreshTimer();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!_isLoaded)
            {
                _isLoaded = true;
                await LoadDashboardDataAsync();
            }
        }

        private async void DashboardPanel_Load(object sender, EventArgs e)
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                await LoadDashboardDataAsync();
            }
        }

        private void ConfigureDashboard()
        {
            this.Size = new Size(1000, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Padding = new Padding(20);

            CreateDashboardInterface();
        }

        private void CreateDashboardInterface()
        {
            // 
            // pnlHeader
            // 
            this.pnlHeader = new Panel();
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Height = 80;
            this.pnlHeader.BackColor = Color.White;

            // 
            // lblWelcome
            // 
            this.lblWelcome = new Label();
            this.lblWelcome.Text = $"¡Bienvenido, {SessionManager.CurrentUser?.FullName ?? "Usuario"}!";
            this.lblWelcome.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblWelcome.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblWelcome.Location = new Point(20, 15);
            this.lblWelcome.AutoSize = true;

            // 
            // lblDateTime
            // 
            this.lblDateTime = new Label();
            this.lblDateTime.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES"));
            this.lblDateTime.Font = new Font("Segoe UI", 11F);
            this.lblDateTime.ForeColor = Color.FromArgb(127, 140, 141);
            this.lblDateTime.Location = new Point(20, 50);
            this.lblDateTime.AutoSize = true;

            this.pnlHeader.Controls.Add(this.lblWelcome);
            this.pnlHeader.Controls.Add(this.lblDateTime);

            // 
            // pnlCards
            // 
            this.pnlCards = new Panel();
            this.pnlCards.Dock = DockStyle.Top;
            this.pnlCards.Height = 200;

            // Inicializar tarjetas
            this.cardTotalProducts = new Panel();
            this.cardLowStock = new Panel();
            this.cardExpiring = new Panel();
            this.cardRecentMovements = new Panel();

            // 
            // Crear tarjetas del dashboard
            // 
            CreateDashboardCard(this.cardTotalProducts, "Total Productos", "0", Color.FromArgb(52, 152, 219), 0);
            CreateDashboardCard(this.cardLowStock, "Stock Bajo", "0", Color.FromArgb(231, 76, 60), 1);
            CreateDashboardCard(this.cardExpiring, "Por Vencer", "0", Color.FromArgb(243, 156, 18), 2);
            CreateDashboardCard(this.cardRecentMovements, "Movimientos Hoy", "0", Color.FromArgb(46, 204, 113), 3);

            this.pnlCards.Controls.Add(this.cardTotalProducts);
            this.pnlCards.Controls.Add(this.cardLowStock);
            this.pnlCards.Controls.Add(this.cardExpiring);
            this.pnlCards.Controls.Add(this.cardRecentMovements);

            // 
            // pnlAlerts
            // 
            this.pnlAlerts = new Panel();
            this.pnlAlerts.Dock = DockStyle.Fill;

            // 
            // lblAlertsTitle
            // 
            this.lblAlertsTitle = new Label();
            this.lblAlertsTitle.Text = "Alertas del Sistema";
            this.lblAlertsTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblAlertsTitle.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblAlertsTitle.Dock = DockStyle.Top;
            this.lblAlertsTitle.Height = 40;
            this.lblAlertsTitle.TextAlign = ContentAlignment.BottomLeft;

            // 
            // lstAlerts
            // 
            this.lstAlerts = new ListBox();
            this.lstAlerts.Dock = DockStyle.Fill;
            this.lstAlerts.Font = new Font("Segoe UI", 10F);
            this.lstAlerts.BackColor = Color.White;
            this.lstAlerts.BorderStyle = BorderStyle.FixedSingle;
            this.lstAlerts.DrawMode = DrawMode.OwnerDrawFixed;
            this.lstAlerts.ItemHeight = 40;
            this.lstAlerts.DrawItem += LstAlerts_DrawItem;

            this.pnlAlerts.Controls.Add(this.lstAlerts);
            this.pnlAlerts.Controls.Add(this.lblAlertsTitle);

            // 
            // refreshTimer
            // 
            this.refreshTimer = new Timer();
            this.refreshTimer.Interval = 300000; // 5 minutos
            this.refreshTimer.Tick += RefreshTimer_Tick;

            // Agregar controles principales
            this.Controls.Add(this.pnlAlerts);
            this.Controls.Add(this.pnlCards);
            this.Controls.Add(this.pnlHeader);
        }

        private void CreateDashboardCard(Panel card, string title, string value, Color color, int position)
        {
            card.Size = new Size(220, 160);
            card.Location = new Point(20 + (position * 240), 20);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;

            // Panel de color en la parte superior
            var colorPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = color
            };

            // Label del título
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(15, 20),
                AutoSize = true
            };

            // Label del valor
            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 50),
                AutoSize = true,
                Name = $"lblValue{position}"
            };

            // Label de descripción
            var lblDescription = new Label
            {
                Text = GetCardDescription(position),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(15, 120),
                Size = new Size(190, 30),
                Name = $"lblDesc{position}"
            };

            card.Controls.Add(colorPanel);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblDescription);
        }

        private string GetCardDescription(int cardIndex)
        {
            switch (cardIndex)
            {
                case 0:
                    return "Productos registrados en el sistema";
                case 1:
                    return "Productos con stock por debajo del mínimo";
                case 2:
                    return "Productos que vencen en 30 días";
                case 3:
                    return "Movimientos de inventario de hoy";
                default:
                    return "";
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
            _alertService = new AlertService(productRepository, inventoryRepository);
        }

        private async 
        Task
LoadDashboardDataAsync()
        {
            try
            {
                await LoadCardDataAsync();
                await LoadAlertsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del dashboard: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadCardDataAsync()
        {
            try
            {
                // Total de productos
                var totalProducts = await _productService.GetProductCountAsync();
                UpdateCardValue(0, totalProducts.ToString());

                // Productos con stock bajo
                var lowStockProducts = await _productService.GetLowStockProductsAsync();
                var lowStockCount = lowStockProducts.Count();
                UpdateCardValue(1, lowStockCount.ToString());

                // Productos próximos a vencer
                var expiringBatches = await _inventoryService.GetExpiringBatchesAsync(30);
                var expiringCount = expiringBatches.Count();
                UpdateCardValue(2, expiringCount.ToString());

                // Movimientos de hoy
                var todayMovements = await _inventoryService.GetMovementHistoryAsync(DateTime.Today, DateTime.Today.AddDays(1));
                var movementsCount = todayMovements.Count();
                UpdateCardValue(3, movementsCount.ToString());

                // Actualizar descripciones con más detalle
                UpdateCardDescription(1, lowStockCount > 0 ? "¡Requiere atención inmediata!" : "Stock en niveles normales");
                UpdateCardDescription(2, expiringCount > 0 ? "¡Verificar fechas de vencimiento!" : "Sin productos próximos a vencer");
            }
            catch (Exception ex)
            {
                // En caso de error, mostrar valores por defecto
                UpdateCardValue(0, "Error");
                UpdateCardValue(1, "Error");
                UpdateCardValue(2, "Error");
                UpdateCardValue(3, "Error");
            }
        }

        private async Task LoadAlertsAsync()
        {
            try
            {
                var alerts = await _alertService.GenerateAlertsAsync();

                if (lstAlerts.InvokeRequired)
                {
                    lstAlerts.Invoke(new Action(() => {
                        lstAlerts.Items.Clear();
                    }));
                }
                else
                {
                    lstAlerts.Items.Clear();
                }

                if (!alerts.Any())
                {
                    if (lstAlerts.InvokeRequired)
                    {
                        lstAlerts.Invoke(new Action(() => {
                            lstAlerts.Items.Add("No hay alertas pendientes");
                        }));
                    }
                    else
                    {
                        lstAlerts.Items.Add("No hay alertas pendientes");
                    }
                    return;
                }

                foreach (var alert in alerts.Take(10)) // Mostrar máximo 10 alertas
                {
                    if (lstAlerts.InvokeRequired)
                    {
                        lstAlerts.Invoke(new Action(() => {
                            lstAlerts.Items.Add(alert);
                        }));
                    }
                    else
                    {
                        lstAlerts.Items.Add(alert);
                    }
                }
            }
            catch (Exception ex)
            {
                if (lstAlerts.InvokeRequired)
                {
                    lstAlerts.Invoke(new Action(() => {
                        lstAlerts.Items.Clear();
                        lstAlerts.Items.Add($"Error al cargar alertas: {ex.Message}");
                    }));
                }
                else
                {
                    lstAlerts.Items.Clear();
                    lstAlerts.Items.Add($"Error al cargar alertas: {ex.Message}");
                }
            }
        }

        private void UpdateCardValue(int cardIndex, string value)
        {
            try
            {
                if (cardIndex >= 0 && cardIndex < pnlCards.Controls.Count)
                {
                    var card = pnlCards.Controls[cardIndex];
                    var lblValue = card.Controls.Find($"lblValue{cardIndex}", false).FirstOrDefault() as Label;
                    if (lblValue != null)
                    {
                        if (lblValue.InvokeRequired)
                        {
                            lblValue.Invoke(new Action(() => lblValue.Text = value));
                        }
                        else
                        {
                            lblValue.Text = value;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignorar errores de actualización de UI
            }
        }

        private void UpdateCardDescription(int cardIndex, string description)
        {
            try
            {
                if (cardIndex >= 0 && cardIndex < pnlCards.Controls.Count)
                {
                    var card = pnlCards.Controls[cardIndex];
                    var lblDesc = card.Controls.Find($"lblDesc{cardIndex}", false).FirstOrDefault() as Label;
                    if (lblDesc != null)
                    {
                        if (lblDesc.InvokeRequired)
                        {
                            lblDesc.Invoke(new Action(() => lblDesc.Text = description));
                        }
                        else
                        {
                            lblDesc.Text = description;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignorar errores de actualización de UI
            }
        }

        private void LstAlerts_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstAlerts.Items.Count) return;

            e.DrawBackground();

            var alert = lstAlerts.Items[e.Index];
            var alertObj = alert as Models.Alert;

            Color bgColor = Color.White;
            Color textColor = Color.Black;
            Color priorityColor = Color.Gray;

            if (alertObj != null)
            {
                switch (alertObj.Priority)
                {
                    case Models.AlertPriority.Critical:
                        priorityColor = Color.FromArgb(231, 76, 60);
                        break;
                    case Models.AlertPriority.High:
                        priorityColor = Color.FromArgb(243, 156, 18);
                        break;
                    case Models.AlertPriority.Medium:
                        priorityColor = Color.FromArgb(52, 152, 219);
                        break;
                    default:
                        priorityColor = Color.FromArgb(46, 204, 113);
                        break;
                }

                if (e.State.HasFlag(DrawItemState.Selected))
                {
                    bgColor = priorityColor;
                    textColor = Color.White;
                }
            }

            // Dibujar fondo
            using (var brush = new SolidBrush(bgColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            // Dibujar indicador de prioridad
            using (var brush = new SolidBrush(priorityColor))
            {
                e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X, e.Bounds.Y, 4, e.Bounds.Height));
            }

            // Dibujar texto
            var textRect = new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 5, e.Bounds.Width - 15, e.Bounds.Height - 10);
            var text = alertObj?.Message ?? alert.ToString();

            using (var brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(text, lstAlerts.Font, brush, textRect, StringFormat.GenericDefault);
            }

            e.DrawFocusRectangle();
        }

        private async void RefreshTimer_Tick(object sender, EventArgs e)
        {
            await LoadDashboardDataAsync();
        }

        private void StartRefreshTimer()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Enabled = true;
                refreshTimer.Start();
            }
        }

        public async Task RefreshDashboardAsync()
        {
            await LoadDashboardDataAsync();
        }

        // Método síncrono para compatibilidad hacia atrás
        public void RefreshDashboard()
        {
            _ = RefreshDashboardAsync(); // Fire and forget
        }

        private void StopRefreshTimer()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }
    }
}
