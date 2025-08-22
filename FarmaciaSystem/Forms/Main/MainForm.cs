using FarmaciaSystem.Forms.Authentication;
using FarmaciaSystem.Forms.Base;
using FarmaciaSystem.Forms.Configuration;
using FarmaciaSystem.Forms.Dashboard;
using FarmaciaSystem.Forms.Inventory;
using FarmaciaSystem.Forms.Products;
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

namespace FarmaciaSystem.Forms.Main
{
    public partial class MainForm : BaseForm
    {
        // Controles principales - Ya declarados en Designer.cs
        // private MenuStrip menuStrip;
        // private ToolStrip toolStrip;
        // private StatusStrip statusStrip;

        private Panel pnlSidebar;
        private Panel pnlMain;
        private Splitter splitter;

        // Controles del sidebar
        private Panel pnlUserInfo;
        private Label lblUserName;
        private Label lblUserRole;
        private Label lblSessionTime;
        private TreeView tvNavigation;

        // Controles de la barra de estado
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel statusUser;
        private ToolStripStatusLabel statusTime;
        private ToolStripProgressBar statusProgress;

        // Timer para actualizar información
        private Timer updateTimer;

        // Control del contenido principal
        private UserControl currentContent;

        public MainForm()
        {
            InitializeComponent();
            ConfigureMainForm();
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void ConfigureMainForm()
        {
            this.Text = $"Sistema Farmacia - {SessionManager.CurrentUser?.FullName ?? "Usuario"}";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Icon = CreateApplicationIcon();

            CreateMainInterface();
            SetupTimer();
        }

        private void CreateMainInterface()
        {
            // Crear MenuStrip
            CreateMenuStrip();

            // Crear ToolStrip
            CreateToolStrip();

            // Crear StatusStrip
            CreateStatusStrip();

            // Crear Sidebar
            CreateSidebar();

            // Crear panel principal
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(10)
            };

            // Crear splitter
            splitter = new Splitter
            {
                Dock = DockStyle.Left,
                Width = 3,
                BackColor = Color.FromArgb(149, 165, 166)
            };

            // Agregar controles al formulario
            this.Controls.Add(pnlMain);
            this.Controls.Add(splitter);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(toolStrip);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);

            // Cargar dashboard por defecto
            LoadDashboard();
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            // Menú Archivo
            var fileMenu = new ToolStripMenuItem("&Archivo");
            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Nuevo Producto", "Ctrl+N", btnNewProduct_Click),
                CreateMenuItem("&Entrada de Mercancía", "Ctrl+E", btnInventoryEntry_Click),
                CreateMenuItem("&Salida de Mercancía", "Ctrl+S", btnInventoryExit_Click),
                new ToolStripSeparator(),
                CreateMenuItem("&Cambiar Contraseña", "", btnChangePassword_Click),
                new ToolStripSeparator(),
                CreateMenuItem("&Salir", "Alt+F4", btnExit_Click)
            });

            // Menú Productos
            var productsMenu = new ToolStripMenuItem("&Productos");
            productsMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Lista de Productos", "", btnProductList_Click),
                CreateMenuItem("&Nuevo Producto", "", btnNewProduct_Click),
                new ToolStripSeparator(),
                CreateMenuItem("&Categorías", "", btnCategories_Click),
                CreateMenuItem("&Proveedores", "", btnSuppliers_Click)
            });

            // Menú Inventario
            var inventoryMenu = new ToolStripMenuItem("&Inventario");
            inventoryMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Entrada de Mercancía", "", btnInventoryEntry_Click),
                CreateMenuItem("&Salida de Mercancía", "", btnInventoryExit_Click),
                CreateMenuItem("&Transferencias", "", btnTransfers_Click),
                new ToolStripSeparator(),
                CreateMenuItem("&Consultar Stock", "", btnStockQuery_Click),
                CreateMenuItem("&Productos por Vencer", "", btnExpiringProducts_Click),
                CreateMenuItem("&Historial de Movimientos", "", btnMovementHistory_Click)
            });

            // Menú Configuración
            var configMenu = new ToolStripMenuItem("&Configuración");
            configMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Almacenes", "", btnWarehouses_Click),
                CreateMenuItem("&Ubicaciones", "", btnLocations_Click),
                CreateMenuItem("&Usuarios", "", btnUsers_Click),
                CreateMenuItem("&Roles y Permisos", "", btnRoles_Click),
                new ToolStripSeparator(),
                CreateMenuItem("&Configuración General", "", btnGeneralConfig_Click)
            });

            // Menú Reportes
            var reportsMenu = new ToolStripMenuItem("&Reportes");
            reportsMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Reporte de Inventario", "", btnInventoryReport_Click),
                CreateMenuItem("&Productos por Vencer", "", btnExpiryReport_Click),
                CreateMenuItem("&Movimientos de Inventario", "", btnMovementReport_Click),
                new ToolStripSeparator(),
                CreateMenuItem("&Exportar Datos", "", btnExportData_Click)
            });

            // Menú Ayuda
            var helpMenu = new ToolStripMenuItem("A&yuda");
            helpMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("&Manual de Usuario", "F1", btnHelp_Click),
                CreateMenuItem("&Acerca de", "", btnAbout_Click)
            });

            menuStrip.Items.AddRange(new ToolStripItem[]
            {
                fileMenu, productsMenu, inventoryMenu, configMenu, reportsMenu, helpMenu
            });

            // Aplicar permisos a los menús
            ApplyMenuPermissions();
        }

        private void CreateToolStrip()
        {
            toolStrip = new ToolStrip
            {
                BackColor = Color.FromArgb(236, 240, 241),
                GripStyle = ToolStripGripStyle.Hidden,
                ImageScalingSize = new Size(24, 24)
            };

            var btnDashboard = CreateToolButton("Dashboard", "Ir al Dashboard", btnDashboard_Click);
            var btnNewProductTool = CreateToolButton("Nuevo Producto", "Crear nuevo producto", btnNewProduct_Click);
            var btnProductListTool = CreateToolButton("Productos", "Lista de productos", btnProductList_Click);
            var btnEntryTool = CreateToolButton("Entrada", "Entrada de mercancía", btnInventoryEntry_Click);
            var btnExitTool = CreateToolButton("Salida", "Salida de mercancía", btnInventoryExit_Click);
            var btnLocationsTool = CreateToolButton("Ubicaciones", "Gestionar ubicaciones", btnLocations_Click);

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                btnDashboard,
                new ToolStripSeparator(),
                btnNewProductTool,
                btnProductListTool,
                new ToolStripSeparator(),
                btnEntryTool,
                btnExitTool,
                new ToolStripSeparator(),
                btnLocationsTool
            });
        }

        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White
            };

            statusLabel = new ToolStripStatusLabel
            {
                Text = "Listo",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            statusUser = new ToolStripStatusLabel
            {
                Text = $"Usuario: {SessionManager.CurrentUser?.FullName ?? "Desconocido"}",
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };

            statusTime = new ToolStripStatusLabel
            {
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };

            statusProgress = new ToolStripProgressBar
            {
                Visible = false,
                Size = new Size(100, 16)
            };

            statusStrip.Items.AddRange(new ToolStripItem[]
            {
                statusLabel, statusProgress, statusUser, statusTime
            });
        }

        private void CreateSidebar()
        {
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = Color.FromArgb(44, 62, 80)
            };

            // Panel de información del usuario
            pnlUserInfo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(15, 10, 15, 10)
            };

            lblUserName = new Label
            {
                Text = SessionManager.CurrentUser?.FullName ?? "Usuario",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblUserRole = new Label
            {
                Text = "Administrador", // TODO: Obtener rol real
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(189, 195, 199),
                Dock = DockStyle.Top,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblSessionTime = new Label
            {
                Text = "Sesión activa",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(149, 165, 166),
                Dock = DockStyle.Top,
                Height = 18,
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlUserInfo.Controls.Add(lblSessionTime);
            pnlUserInfo.Controls.Add(lblUserRole);
            pnlUserInfo.Controls.Add(lblUserName);

            // TreeView de navegación
            tvNavigation = new TreeView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                LineColor = Color.FromArgb(149, 165, 166),
                ShowLines = false,
                ShowPlusMinus = true,
                ShowRootLines = false,
                HotTracking = true,
                ItemHeight = 25,
                Indent = 20
            };

            CreateNavigationTree();
            tvNavigation.NodeMouseClick += TvNavigation_NodeMouseClick;

            pnlSidebar.Controls.Add(tvNavigation);
            pnlSidebar.Controls.Add(pnlUserInfo);
        }

        private void CreateNavigationTree()
        {
            tvNavigation.Nodes.Clear();

            // Dashboard
            var dashboardNode = new TreeNode("📊 Dashboard") { Tag = "dashboard" };

            // Productos
            var productsNode = new TreeNode("📦 Productos") { Tag = "products" };
            productsNode.Nodes.Add(new TreeNode("Lista de Productos") { Tag = "product_list" });
            productsNode.Nodes.Add(new TreeNode("Nuevo Producto") { Tag = "product_new" });
            productsNode.Nodes.Add(new TreeNode("Categorías") { Tag = "categories" });

            // Inventario
            var inventoryNode = new TreeNode("📋 Inventario") { Tag = "inventory" };
            inventoryNode.Nodes.Add(new TreeNode("Entrada de Mercancía") { Tag = "inventory_entry" });
            inventoryNode.Nodes.Add(new TreeNode("Salida de Mercancía") { Tag = "inventory_exit" });
            inventoryNode.Nodes.Add(new TreeNode("Consultar Stock") { Tag = "stock_query" });
            inventoryNode.Nodes.Add(new TreeNode("Productos por Vencer") { Tag = "expiring_products" });

            // Configuración
            var configNode = new TreeNode("⚙️ Configuración") { Tag = "config" };
            configNode.Nodes.Add(new TreeNode("Almacenes") { Tag = "warehouses" });
            configNode.Nodes.Add(new TreeNode("Ubicaciones") { Tag = "locations" });
            configNode.Nodes.Add(new TreeNode("Usuarios") { Tag = "users" });
            configNode.Nodes.Add(new TreeNode("Proveedores") { Tag = "suppliers" });

            // Reportes
            var reportsNode = new TreeNode("📈 Reportes") { Tag = "reports" };
            reportsNode.Nodes.Add(new TreeNode("Reporte de Inventario") { Tag = "inventory_report" });
            reportsNode.Nodes.Add(new TreeNode("Movimientos") { Tag = "movement_report" });

            tvNavigation.Nodes.AddRange(new TreeNode[]
            {
                dashboardNode, productsNode, inventoryNode, configNode, reportsNode
            });

            // Expandir nodos principales
            foreach (TreeNode node in tvNavigation.Nodes)
            {
                node.Expand();
            }

            // Aplicar permisos
            ApplyNavigationPermissions();
        }

        private void SetupTimer()
        {
            updateTimer = new Timer
            {
                Interval = 1000 // 1 segundo
            };
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Actualizar hora
            statusTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            // Actualizar información de sesión
            if (SessionManager.IsSessionValid())
            {
                var remaining = SessionManager.GetRemainingTime();
                lblSessionTime.Text = $"Sesión: {remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}";

                // Advertir si la sesión está por expirar
                if (SessionManager.IsSessionExpiringSoon(15))
                {
                    lblSessionTime.ForeColor = Color.FromArgb(231, 76, 60);
                }
            }
            else
            {
                lblSessionTime.Text = "Sesión expirada";
                lblSessionTime.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Sistema iniciado correctamente";

            // Verificar permisos del usuario actual
            CheckUserPermissions();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!ShowConfirmation("¿Está seguro que desea salir del sistema?", "Confirmar Salida"))
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Limpiar recursos
            updateTimer?.Stop();
            updateTimer?.Dispose();
            SessionManager.EndSession();
        }

        // Métodos de navegación
        private void TvNavigation_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag == null) return;

            var action = e.Node.Tag.ToString();
            ExecuteNavigationAction(action);
        }

        private void ExecuteNavigationAction(string action)
        {
            try
            {
                statusLabel.Text = "Cargando...";
                statusProgress.Visible = true;

                switch (action)
                {
                    case "dashboard":
                        LoadDashboard();
                        break;
                    case "product_list":
                        LoadProductList();
                        break;
                    case "product_new":
                        OpenProductEdit();
                        break;
                    case "inventory_entry":
                        OpenInventoryEntry();
                        break;
                    case "inventory_exit":
                        OpenInventoryExit();
                        break;
                    case "locations":
                        OpenLocationManager();
                        break;
                    default:
                        ShowMessage($"Funcionalidad '{action}' en desarrollo", MessageType.Information);
                        break;
                }

                statusLabel.Text = "Listo";
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cargar la funcionalidad: {ex.Message}", MessageType.Error);
                statusLabel.Text = "Error";
            }
            finally
            {
                statusProgress.Visible = false;
            }
        }

        private void LoadContent(UserControl content, string title)
        {
            if (currentContent != null)
            {
                pnlMain.Controls.Remove(currentContent);
                currentContent.Dispose();
            }

            currentContent = content;
            content.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(content);

            this.Text = $"Sistema Farmacia - {title}";
            statusLabel.Text = $"Mostrando: {title}";
        }

        private void LoadDashboard()
        {
            var dashboard = new DashboardPanel();
            LoadContent(dashboard, "Dashboard");
        }

        private void LoadProductList()
        {
            // Como ProductListForm es un Form, necesitamos manejarlo diferente
            OpenProductList();
        }

        // Event handlers para botones y menús
        private void btnDashboard_Click(object sender, EventArgs e) => LoadDashboard();

        private void btnProductList_Click(object sender, EventArgs e) => OpenProductList();

        private void btnNewProduct_Click(object sender, EventArgs e) => OpenProductEdit();

        private void btnInventoryEntry_Click(object sender, EventArgs e) => OpenInventoryEntry();

        private void btnInventoryExit_Click(object sender, EventArgs e) => OpenInventoryExit();

        private void btnLocations_Click(object sender, EventArgs e) => OpenLocationManager();

        private void btnChangePassword_Click(object sender, EventArgs e) => OpenChangePassword();

        private void btnExit_Click(object sender, EventArgs e) => this.Close();

        // Métodos stub para funcionalidades futuras
        private void btnCategories_Click(object sender, EventArgs e) => ShowMessage("Gestión de categorías en desarrollo", MessageType.Information);
        private void btnSuppliers_Click(object sender, EventArgs e) => ShowMessage("Gestión de proveedores en desarrollo", MessageType.Information);
        private void btnTransfers_Click(object sender, EventArgs e) => ShowMessage("Transferencias en desarrollo", MessageType.Information);
        private void btnStockQuery_Click(object sender, EventArgs e) => ShowMessage("Consulta de stock en desarrollo", MessageType.Information);
        private void btnExpiringProducts_Click(object sender, EventArgs e) => ShowMessage("Productos por vencer en desarrollo", MessageType.Information);
        private void btnMovementHistory_Click(object sender, EventArgs e) => ShowMessage("Historial de movimientos en desarrollo", MessageType.Information);
        private void btnWarehouses_Click(object sender, EventArgs e) => ShowMessage("Gestión de almacenes en desarrollo", MessageType.Information);
        private void btnUsers_Click(object sender, EventArgs e) => ShowMessage("Gestión de usuarios en desarrollo", MessageType.Information);
        private void btnRoles_Click(object sender, EventArgs e) => ShowMessage("Roles y permisos en desarrollo", MessageType.Information);
        private void btnGeneralConfig_Click(object sender, EventArgs e) => ShowMessage("Configuración general en desarrollo", MessageType.Information);
        private void btnInventoryReport_Click(object sender, EventArgs e) => ShowMessage("Reporte de inventario en desarrollo", MessageType.Information);
        private void btnExpiryReport_Click(object sender, EventArgs e) => ShowMessage("Reporte de vencimientos en desarrollo", MessageType.Information);
        private void btnMovementReport_Click(object sender, EventArgs e) => ShowMessage("Reporte de movimientos en desarrollo", MessageType.Information);
        private void btnExportData_Click(object sender, EventArgs e) => ShowMessage("Exportación de datos en desarrollo", MessageType.Information);
        private void btnHelp_Click(object sender, EventArgs e) => ShowMessage("Manual de usuario en desarrollo", MessageType.Information);
        private void btnAbout_Click(object sender, EventArgs e) => ShowAboutDialog();

        // Métodos para abrir formularios
        private void OpenProductList()
        {
            using (var form = new ProductListForm())
            {
                form.ShowDialog(this);
            }
        }

        private void OpenProductEdit()
        {
            using (var form = new ProductEditForm())
            {
                form.ShowDialog(this);
            }
        }

        private void OpenInventoryEntry()
        {
            using (var form = new InventoryEntryForm())
            {
                form.ShowDialog(this);
            }
        }

        private void OpenInventoryExit()
        {
            using (var form = new InventoryExitForm())
            {
                form.ShowDialog(this);
            }
        }

        private void OpenLocationManager()
        {
            using (var form = new LocationManagerForm())
            {
                form.ShowDialog(this);
            }
        }

        private void OpenChangePassword()
        {
            using (var form = new ChangePasswordForm())
            {
                form.ShowDialog(this);
            }
        }

        private void ShowAboutDialog()
        {
            MessageBox.Show(
                "Sistema de Gestión de Farmacia\n\n" +
                "Versión 1.0.0\n" +
                "Desarrollado para el control integral de inventarios farmacéuticos\n\n" +
                "Características:\n" +
                "• Gestión completa de productos\n" +
                "• Control de inventarios con lotes\n" +
                "• Sistema de alertas automáticas\n" +
                "• Gestión de ubicaciones\n" +
                "• Control de vencimientos\n" +
                "• Sistema de usuarios y permisos\n\n" +
                "© 2025 - Todos los derechos reservados",
                "Acerca del Sistema",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // Métodos auxiliares
        private ToolStripMenuItem CreateMenuItem(string text, string shortcut, EventHandler clickHandler)
        {
            var item = new ToolStripMenuItem(text)
            {
                ShortcutKeyDisplayString = shortcut
            };
            item.Click += clickHandler;
            return item;
        }

        private ToolStripButton CreateToolButton(string text, string tooltip, EventHandler clickHandler)
        {
            var button = new ToolStripButton(text)
            {
                ToolTipText = tooltip,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            button.Click += clickHandler;
            return button;
        }

        private Icon CreateApplicationIcon()
        {
            // Crear un ícono simple para la aplicación
            var bitmap = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(52, 73, 94)), 0, 0, 32, 32);
                g.DrawString("F", new Font("Arial", 20, FontStyle.Bold), Brushes.White, 8, 4);
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        private void ApplyMenuPermissions()
        {
            // TODO: Implementar control de permisos en menús
            // Por ahora, todos los menús están habilitados
        }

        private void ApplyNavigationPermissions()
        {
            // TODO: Implementar control de permisos en navegación
            // Por ahora, toda la navegación está habilitada
        }

        private void CheckUserPermissions()
        {
            if (!SessionManager.IsLoggedIn)
            {
                ShowMessage("Sesión no válida", MessageType.Error);
                this.Close();
            }
        }
    }
}
