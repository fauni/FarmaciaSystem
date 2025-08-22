using FarmaciaSystem.Utils.Navigation;
using FarmaciaSystem.Utils.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmaciaSystem.Utils.UI
{
    /// <summary>
    /// Gestor centralizado para barras de herramientas y componentes de UI
    /// </summary>
    public static class ToolbarManager
    {
        private static Dictionary<string, ToolButtonInfo> _toolButtons;

        static ToolbarManager()
        {
            InitializeToolButtons();
        }

        #region Creación de Barras de Herramientas

        /// <summary>
        /// Crea la barra de herramientas principal del sistema
        /// </summary>
        public static ToolStrip CreateMainToolbar(EventHandler buttonClickHandler)
        {
            var toolStrip = new ToolStrip
            {
                BackColor = Color.FromArgb(236, 240, 241),
                GripStyle = ToolStripGripStyle.Hidden,
                ImageScalingSize = new Size(24, 24),
                Font = new Font("Segoe UI", 9F)
            };

            var availableButtons = GetAvailableToolButtons();
            var buttonGroups = GroupButtonsByCategory(availableButtons);

            bool firstGroup = true;
            foreach (var group in buttonGroups)
            {
                if (!firstGroup)
                {
                    toolStrip.Items.Add(new ToolStripSeparator());
                }

                foreach (var buttonInfo in group.Value)
                {
                    var button = CreateToolButton(buttonInfo, buttonClickHandler);
                    toolStrip.Items.Add(button);
                }

                firstGroup = false;
            }

            return toolStrip;
        }

        /// <summary>
        /// Crea una barra de herramientas de acceso rápido
        /// </summary>
        public static ToolStrip CreateQuickAccessToolbar(EventHandler buttonClickHandler)
        {
            var toolStrip = new ToolStrip
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                GripStyle = ToolStripGripStyle.Hidden,
                ImageScalingSize = new Size(16, 16),
                Font = new Font("Segoe UI", 8F)
            };

            var quickAccessButtons = new[]
            {
                "dashboard", "product_new", "inventory_entry", "inventory_exit"
            };

            foreach (var buttonKey in quickAccessButtons)
            {
                if (_toolButtons.ContainsKey(buttonKey) &&
                    MenuManager.HasPermissionForAction(buttonKey))
                {
                    var buttonInfo = _toolButtons[buttonKey];
                    var button = CreateToolButton(buttonInfo, buttonClickHandler);
                    button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                    button.BackColor = Color.Transparent;
                    button.ForeColor = Color.White;
                    toolStrip.Items.Add(button);
                }
            }

            return toolStrip;
        }

        /// <summary>
        /// Crea un menú contextual basado en el contexto
        /// </summary>
        public static ContextMenuStrip CreateContextMenu(string context, EventHandler itemClickHandler)
        {
            var contextMenu = new ContextMenuStrip();

            var contextButtons = GetContextButtons(context);
            var groupedButtons = GroupButtonsByCategory(contextButtons);

            bool firstGroup = true;
            foreach (var group in groupedButtons)
            {
                if (!firstGroup)
                {
                    contextMenu.Items.Add(new ToolStripSeparator());
                }

                foreach (var buttonInfo in group.Value)
                {
                    var menuItem = new ToolStripMenuItem
                    {
                        Text = buttonInfo.Text,
                        Tag = buttonInfo.Action,
                        ToolTipText = buttonInfo.ToolTip,
                        Enabled = MenuManager.HasPermissionForAction(buttonInfo.Action)
                    };
                    menuItem.Click += itemClickHandler;
                    contextMenu.Items.Add(menuItem);
                }

                firstGroup = false;
            }

            return contextMenu;
        }

        #endregion

        #region Inicialización de Botones

        /// <summary>
        /// Inicializa la configuración de todos los botones disponibles
        /// </summary>
        private static void InitializeToolButtons()
        {
            _toolButtons = new Dictionary<string, ToolButtonInfo>
            {
                // Botones Principales
                ["dashboard"] = new ToolButtonInfo
                {
                    Action = "dashboard",
                    Text = "Dashboard",
                    ToolTip = "Ir al dashboard principal",
                    Category = "Principal",
                    IconChar = "📊",
                    Shortcut = "F2"
                },

                // Botones de Productos
                ["product_new"] = new ToolButtonInfo
                {
                    Action = "product_new",
                    Text = "Nuevo",
                    ToolTip = "Crear nuevo producto (Ctrl+N)",
                    Category = "Productos",
                    IconChar = "➕",
                    Shortcut = "Ctrl+N"
                },
                ["product_list"] = new ToolButtonInfo
                {
                    Action = "product_list",
                    Text = "Productos",
                    ToolTip = "Lista de productos (Ctrl+P)",
                    Category = "Productos",
                    IconChar = "📦",
                    Shortcut = "Ctrl+P"
                },

                // Botones de Inventario
                ["inventory_entry"] = new ToolButtonInfo
                {
                    Action = "inventory_entry",
                    Text = "Entrada",
                    ToolTip = "Entrada de mercancía (Ctrl+E)",
                    Category = "Inventario",
                    IconChar = "📥",
                    Shortcut = "Ctrl+E"
                },
                ["inventory_exit"] = new ToolButtonInfo
                {
                    Action = "inventory_exit",
                    Text = "Salida",
                    ToolTip = "Salida de mercancía (Ctrl+S)",
                    Category = "Inventario",
                    IconChar = "📤",
                    Shortcut = "Ctrl+S"
                },
                ["stock_query"] = new ToolButtonInfo
                {
                    Action = "stock_query",
                    Text = "Stock",
                    ToolTip = "Consultar stock (F3)",
                    Category = "Inventario",
                    IconChar = "🔍",
                    Shortcut = "F3"
                },
                ["transfers"] = new ToolButtonInfo
                {
                    Action = "transfers",
                    Text = "Transferir",
                    ToolTip = "Transferir productos entre almacenes",
                    Category = "Inventario",
                    IconChar = "🔄",
                    Shortcut = ""
                },

                // Botones de Configuración
                ["locations"] = new ToolButtonInfo
                {
                    Action = "locations",
                    Text = "Ubicaciones",
                    ToolTip = "Gestionar ubicaciones",
                    Category = "Configuración",
                    IconChar = "📍",
                    Shortcut = ""
                },
                ["users"] = new ToolButtonInfo
                {
                    Action = "users",
                    Text = "Usuarios",
                    ToolTip = "Gestionar usuarios",
                    Category = "Configuración",
                    IconChar = "👥",
                    Shortcut = ""
                },
                ["warehouses"] = new ToolButtonInfo
                {
                    Action = "warehouses",
                    Text = "Almacenes",
                    ToolTip = "Gestionar almacenes",
                    Category = "Configuración",
                    IconChar = "🏪",
                    Shortcut = ""
                },

                // Botones de Reportes
                ["inventory_report"] = new ToolButtonInfo
                {
                    Action = "inventory_report",
                    Text = "Reportes",
                    ToolTip = "Generar reportes",
                    Category = "Reportes",
                    IconChar = "📊",
                    Shortcut = "F4"
                },

                // Botones Generales
                ["refresh"] = new ToolButtonInfo
                {
                    Action = "refresh",
                    Text = "Actualizar",
                    ToolTip = "Actualizar datos (F5)",
                    Category = "General",
                    IconChar = "🔄",
                    Shortcut = "F5"
                },
                ["save"] = new ToolButtonInfo
                {
                    Action = "save",
                    Text = "Guardar",
                    ToolTip = "Guardar cambios (Ctrl+G)",
                    Category = "General",
                    IconChar = "💾",
                    Shortcut = "Ctrl+G"
                },
                ["cancel"] = new ToolButtonInfo
                {
                    Action = "cancel",
                    Text = "Cancelar",
                    ToolTip = "Cancelar operación (Esc)",
                    Category = "General",
                    IconChar = "❌",
                    Shortcut = "Esc"
                },
                ["print"] = new ToolButtonInfo
                {
                    Action = "print",
                    Text = "Imprimir",
                    ToolTip = "Imprimir documento (Ctrl+P)",
                    Category = "General",
                    IconChar = "🖨️",
                    Shortcut = "Ctrl+P"
                },
                ["export"] = new ToolButtonInfo
                {
                    Action = "export",
                    Text = "Exportar",
                    ToolTip = "Exportar datos",
                    Category = "General",
                    IconChar = "📤",
                    Shortcut = ""
                },
                ["search"] = new ToolButtonInfo
                {
                    Action = "search",
                    Text = "Buscar",
                    ToolTip = "Buscar elementos (Ctrl+F)",
                    Category = "General",
                    IconChar = "🔍",
                    Shortcut = "Ctrl+F"
                },
                ["help"] = new ToolButtonInfo
                {
                    Action = "help",
                    Text = "Ayuda",
                    ToolTip = "Mostrar ayuda (F1)",
                    Category = "General",
                    IconChar = "❓",
                    Shortcut = "F1"
                }
            };
        }

        #endregion

        #region Creación de Controles

        /// <summary>
        /// Crea un botón de barra de herramientas
        /// </summary>
        private static ToolStripButton CreateToolButton(ToolButtonInfo buttonInfo, EventHandler clickHandler)
        {
            var button = new ToolStripButton
            {
                Text = buttonInfo.Text,
                ToolTipText = buttonInfo.ToolTip,
                Tag = buttonInfo.Action,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                Margin = new Padding(2, 1, 2, 1)
            };

            // Crear imagen simple con el carácter del ícono
            var image = CreateButtonImage(buttonInfo.IconChar, 24, 24);
            button.Image = image;

            button.Click += clickHandler;

            // Aplicar permisos
            var hasPermission = MenuManager.HasPermissionForAction(buttonInfo.Action);
            button.Enabled = hasPermission;
            button.Visible = hasPermission;

            return button;
        }

        /// <summary>
        /// Crea una imagen para el botón usando el carácter emoji
        /// </summary>
        private static Image CreateButtonImage(string iconChar, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                var font = new Font("Segoe UI Emoji", height * 0.6f, FontStyle.Regular);
                var brush = new SolidBrush(Color.FromArgb(52, 73, 94));

                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(iconChar, font, brush,
                    new RectangleF(0, 0, width, height), stringFormat);
            }
            return bitmap;
        }

        /// <summary>
        /// Crea un botón dropdown con elementos anidados
        /// </summary>
        public static ToolStripDropDownButton CreateDropDownButton(string text, string tooltip,
            EventHandler buttonClickHandler, params ToolButtonInfo[] dropDownItems)
        {
            var dropDownButton = new ToolStripDropDownButton
            {
                Text = text,
                ToolTipText = tooltip,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ShowDropDownArrow = true
            };

            foreach (var item in dropDownItems)
            {
                if (MenuManager.HasPermissionForAction(item.Action))
                {
                    var menuItem = new ToolStripMenuItem
                    {
                        Text = item.Text,
                        Tag = item.Action,
                        ToolTipText = item.ToolTip
                    };
                    menuItem.Click += buttonClickHandler;
                    dropDownButton.DropDownItems.Add(menuItem);
                }
            }

            return dropDownButton;
        }

        #endregion

        #region Gestión de Botones

        /// <summary>
        /// Obtiene todos los botones disponibles según permisos
        /// </summary>
        private static List<ToolButtonInfo> GetAvailableToolButtons()
        {
            return _toolButtons.Values
                .Where(b => MenuManager.HasPermissionForAction(b.Action))
                .OrderBy(b => GetCategoryOrder(b.Category))
                .ThenBy(b => b.Text)
                .ToList();
        }

        /// <summary>
        /// Agrupa botones por categoría
        /// </summary>
        private static Dictionary<string, List<ToolButtonInfo>> GroupButtonsByCategory(
            IEnumerable<ToolButtonInfo> buttons)
        {
            return buttons
                .GroupBy(b => b.Category)
                .OrderBy(g => GetCategoryOrder(g.Key))
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Obtiene el orden de las categorías
        /// </summary>
        private static int GetCategoryOrder(string category)
        {
            switch (category)
            {
                case "Principal": return 1;
                case "General": return 2;
                case "Productos": return 3;
                case "Inventario": return 4;
                case "Configuración": return 5;
                case "Reportes": return 6;
                default: return 99;
            }
        }

        /// <summary>
        /// Obtiene botones contextuales según el contexto
        /// </summary>
        private static List<ToolButtonInfo> GetContextButtons(string context)
        {
            var contextButtons = new List<ToolButtonInfo>();

            switch (context.ToLower())
            {
                case "product_list":
                    contextButtons.AddRange(GetButtonsByActions(new[]
                    {
                        "product_new", "refresh", "search", "export"
                    }));
                    break;

                case "inventory":
                    contextButtons.AddRange(GetButtonsByActions(new[]
                    {
                        "inventory_entry", "inventory_exit", "stock_query", "refresh"
                    }));
                    break;

                case "form_edit":
                    contextButtons.AddRange(GetButtonsByActions(new[]
                    {
                        "save", "cancel", "refresh"
                    }));
                    break;

                case "reports":
                    contextButtons.AddRange(GetButtonsByActions(new[]
                    {
                        "inventory_report", "print", "export", "refresh"
                    }));
                    break;

                case "dashboard":
                    contextButtons.AddRange(GetButtonsByActions(new[]
                    {
                        "refresh", "inventory_report", "product_new", "inventory_entry"
                    }));
                    break;

                default:
                    contextButtons.AddRange(GetButtonsByActions(new[]
                    {
                        "refresh", "search", "help"
                    }));
                    break;
            }

            return contextButtons.Where(b => MenuManager.HasPermissionForAction(b.Action)).ToList();
        }

        /// <summary>
        /// Obtiene botones por sus acciones
        /// </summary>
        private static List<ToolButtonInfo> GetButtonsByActions(string[] actions)
        {
            return actions
                .Where(action => _toolButtons.ContainsKey(action))
                .Select(action => _toolButtons[action])
                .ToList();
        }

        #endregion

        #region Gestión de Temas y Estilos

        /// <summary>
        /// Aplica un tema a la barra de herramientas
        /// </summary>
        public static void ApplyToolbarTheme(ToolStrip toolStrip, ToolbarTheme theme)
        {
            switch (theme)
            {
                case ToolbarTheme.Light:
                    toolStrip.BackColor = Color.FromArgb(245, 245, 245);
                    toolStrip.ForeColor = Color.FromArgb(33, 37, 41);
                    break;

                case ToolbarTheme.Dark:
                    toolStrip.BackColor = Color.FromArgb(52, 73, 94);
                    toolStrip.ForeColor = Color.White;
                    break;

                case ToolbarTheme.Blue:
                    toolStrip.BackColor = Color.FromArgb(52, 152, 219);
                    toolStrip.ForeColor = Color.White;
                    break;

                case ToolbarTheme.Green:
                    toolStrip.BackColor = Color.FromArgb(46, 204, 113);
                    toolStrip.ForeColor = Color.White;
                    break;

                case ToolbarTheme.Red:
                    toolStrip.BackColor = Color.FromArgb(231, 76, 60);
                    toolStrip.ForeColor = Color.White;
                    break;
            }

            // Aplicar el tema a todos los botones
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item is ToolStripButton button)
                {
                    button.BackColor = toolStrip.BackColor;
                    button.ForeColor = toolStrip.ForeColor;
                }
            }
        }

        #endregion

        #region Métodos de Control

        /// <summary>
        /// Habilita o deshabilita un botón específico
        /// </summary>
        public static void EnableButton(ToolStrip toolStrip, string action, bool enabled)
        {
            var button = toolStrip.Items.Cast<ToolStripItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == action);

            if (button != null)
            {
                button.Enabled = enabled;
            }
        }

        /// <summary>
        /// Muestra u oculta un botón específico
        /// </summary>
        public static void SetButtonVisibility(ToolStrip toolStrip, string action, bool visible)
        {
            var button = toolStrip.Items.Cast<ToolStripItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == action);

            if (button != null)
            {
                button.Visible = visible;
            }
        }

        /// <summary>
        /// Actualiza el texto de un botón
        /// </summary>
        public static void UpdateButtonText(ToolStrip toolStrip, string action, string newText)
        {
            var button = toolStrip.Items.Cast<ToolStripItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == action);

            if (button != null)
            {
                button.Text = newText;
            }
        }

        /// <summary>
        /// Actualiza el tooltip de un botón
        /// </summary>
        public static void UpdateButtonTooltip(ToolStrip toolStrip, string action, string newTooltip)
        {
            var button = toolStrip.Items.Cast<ToolStripItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == action);

            if (button != null)
            {
                button.ToolTipText = newTooltip;
            }
        }

        /// <summary>
        /// Agrega un botón personalizado a la barra
        /// </summary>
        public static void AddCustomButton(ToolStrip toolStrip, string action, string text,
            string tooltip, EventHandler clickHandler, string iconChar = "⚙️")
        {
            var customButton = new ToolStripButton
            {
                Text = text,
                ToolTipText = tooltip,
                Tag = action,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                Image = CreateButtonImage(iconChar, 24, 24)
            };

            customButton.Click += clickHandler;
            toolStrip.Items.Add(customButton);
        }

        /// <summary>
        /// Remueve un botón de la barra
        /// </summary>
        public static void RemoveButton(ToolStrip toolStrip, string action)
        {
            var button = toolStrip.Items.Cast<ToolStripItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == action);

            if (button != null)
            {
                toolStrip.Items.Remove(button);
                button.Dispose();
            }
        }

        /// <summary>
        /// Actualiza todos los botones según los permisos actuales
        /// </summary>
        public static void RefreshPermissions(ToolStrip toolStrip)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item.Tag != null)
                {
                    var hasPermission = MenuManager.HasPermissionForAction(item.Tag.ToString());
                    item.Enabled = hasPermission;
                    item.Visible = hasPermission;
                }
            }
        }

        #endregion

        #region Métodos de Información

        /// <summary>
        /// Obtiene información de un botón
        /// </summary>
        public static ToolButtonInfo GetButtonInfo(string action)
        {
            return _toolButtons.ContainsKey(action) ? _toolButtons[action] : null;
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        public static List<string> GetAvailableCategories()
        {
            return _toolButtons.Values
                .Where(b => MenuManager.HasPermissionForAction(b.Action))
                .Select(b => b.Category)
                .Distinct()
                .OrderBy(c => GetCategoryOrder(c))
                .ToList();
        }

        /// <summary>
        /// Obtiene todos los botones de una categoría
        /// </summary>
        public static List<ToolButtonInfo> GetButtonsByCategory(string category)
        {
            return _toolButtons.Values
                .Where(b => b.Category == category && MenuManager.HasPermissionForAction(b.Action))
                .OrderBy(b => b.Text)
                .ToList();
        }

        #endregion
    }

    #region Clases de Apoyo

    /// <summary>
    /// Información de un botón de la barra de herramientas
    /// </summary>
    public class ToolButtonInfo
    {
        public string Action { get; set; }
        public string Text { get; set; }
        public string ToolTip { get; set; }
        public string Category { get; set; }
        public string IconChar { get; set; }
        public string Shortcut { get; set; }
        public bool IsVisible => MenuManager.HasPermissionForAction(Action);
    }

    /// <summary>
    /// Temas disponibles para las barras de herramientas
    /// </summary>
    public enum ToolbarTheme
    {
        Light,
        Dark,
        Blue,
        Green,
        Red
    }

    #endregion

    #region StatusBarManager

    /// <summary>
    /// Gestor para la barra de estado
    /// </summary>
    public static class StatusBarManager
    {
        /// <summary>
        /// Crea una barra de estado estándar
        /// </summary>
        public static StatusStrip CreateStatusBar()
        {
            var statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            var statusLabel = new ToolStripStatusLabel
            {
                Name = "statusLabel",
                Text = "Listo",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var progressBar = new ToolStripProgressBar
            {
                Name = "progressBar",
                Visible = false,
                Size = new Size(100, 16),
                Style = ProgressBarStyle.Continuous
            };

            var userLabel = new ToolStripStatusLabel
            {
                Name = "userLabel",
                Text = $"Usuario: {SessionManager.CurrentUser?.FullName ?? "Desconocido"}",
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };

            var timeLabel = new ToolStripStatusLabel
            {
                Name = "timeLabel",
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };

            statusStrip.Items.AddRange(new ToolStripItem[]
            {
                statusLabel, progressBar, userLabel, timeLabel
            });

            return statusStrip;
        }

        /// <summary>
        /// Actualiza el mensaje de estado
        /// </summary>
        public static void UpdateStatus(StatusStrip statusStrip, string message)
        {
            var statusLabel = statusStrip.Items["statusLabel"] as ToolStripStatusLabel;
            if (statusLabel != null)
            {
                statusLabel.Text = message;
            }
        }

        /// <summary>
        /// Muestra u oculta la barra de progreso
        /// </summary>
        public static void ShowProgress(StatusStrip statusStrip, bool show, int value = 0)
        {
            var progressBar = statusStrip.Items["progressBar"] as ToolStripProgressBar;
            if (progressBar != null)
            {
                progressBar.Visible = show;
                if (show && value >= 0 && value <= 100)
                {
                    progressBar.Value = value;
                }
            }
        }

        /// <summary>
        /// Actualiza la hora en la barra de estado
        /// </summary>
        public static void UpdateTime(StatusStrip statusStrip)
        {
            var timeLabel = statusStrip.Items["timeLabel"] as ToolStripStatusLabel;
            if (timeLabel != null)
            {
                timeLabel.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        /// <summary>
        /// Actualiza la información del usuario
        /// </summary>
        public static void UpdateUser(StatusStrip statusStrip, string userName)
        {
            var userLabel = statusStrip.Items["userLabel"] as ToolStripStatusLabel;
            if (userLabel != null)
            {
                userLabel.Text = $"Usuario: {userName}";
            }
        }
    }

    #endregion
}
