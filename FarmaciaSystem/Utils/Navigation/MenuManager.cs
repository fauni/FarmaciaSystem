using FarmaciaSystem.Utils.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmaciaSystem.Utils.Navigation
{
    public static class MenuManager
    {
        private static Dictionary<string, MenuItemInfo> _menuItems;
        private static Dictionary<string, string> _menuPermissions;

        static MenuManager()
        {
            InitializeMenuStructure();
            InitializePermissionMappings();
        }

        public static void ApplyPermissions(MenuStrip menuStrip)
        {
            foreach (ToolStripMenuItem menuItem in menuStrip.Items)
            {
                ApplyPermissionsToMenuItem(menuItem);
            }
        }

        public static void ApplyPermissions(ContextMenuStrip contextMenu)
        {
            foreach (ToolStripMenuItem menuItem in contextMenu.Items)
            {
                ApplyPermissionsToMenuItem(menuItem);
            }
        }

        public static void ApplyPermissions(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                ApplyPermissionsToTreeNode(node);
            }
        }

        public static bool HasPermissionForAction(string action)
        {
            if (_menuPermissions.ContainsKey(action))
            {
                return PermissionManager.HasPermission(_menuPermissions[action]);
            }
            return true; // Si no hay permiso definido, permitir acceso
        }

        public static MenuItemInfo GetMenuItemInfo(string key)
        {
            return _menuItems.ContainsKey(key) ? _menuItems[key] : null;
        }

        public static List<MenuItemInfo> GetMenuItemsByCategory(string category)
        {
            return _menuItems.Values.Where(m => m.Category == category).ToList();
        }

        private static void InitializeMenuStructure()
        {
            _menuItems = new Dictionary<string, MenuItemInfo>
            {
                // Dashboard
                ["dashboard"] = new MenuItemInfo
                {
                    Key = "dashboard",
                    Title = "Dashboard",
                    Description = "Panel principal con resumen del sistema",
                    Category = "Principal",
                    IconName = "dashboard",
                    RequiredPermission = null
                },

                // Productos
                ["product_list"] = new MenuItemInfo
                {
                    Key = "product_list",
                    Title = "Lista de Productos",
                    Description = "Ver y gestionar productos",
                    Category = "Productos",
                    IconName = "list",
                    RequiredPermission = "productos_consultar"
                },
                ["product_new"] = new MenuItemInfo
                {
                    Key = "product_new",
                    Title = "Nuevo Producto",
                    Description = "Crear un nuevo producto",
                    Category = "Productos",
                    IconName = "add",
                    RequiredPermission = "productos_crear"
                },
                ["categories"] = new MenuItemInfo
                {
                    Key = "categories",
                    Title = "Categorías",
                    Description = "Gestionar categorías de productos",
                    Category = "Productos",
                    IconName = "category",
                    RequiredPermission = "productos_consultar"
                },

                // Inventario
                ["inventory_entry"] = new MenuItemInfo
                {
                    Key = "inventory_entry",
                    Title = "Entrada de Mercancía",
                    Description = "Registrar entrada de productos",
                    Category = "Inventario",
                    IconName = "entry",
                    RequiredPermission = "inventario_entrada"
                },
                ["inventory_exit"] = new MenuItemInfo
                {
                    Key = "inventory_exit",
                    Title = "Salida de Mercancía",
                    Description = "Registrar salida de productos",
                    Category = "Inventario",
                    IconName = "exit",
                    RequiredPermission = "inventario_salida"
                },
                ["stock_query"] = new MenuItemInfo
                {
                    Key = "stock_query",
                    Title = "Consultar Stock",
                    Description = "Consultar estado del inventario",
                    Category = "Inventario",
                    IconName = "search",
                    RequiredPermission = "inventario_consultar"
                },
                ["expiring_products"] = new MenuItemInfo
                {
                    Key = "expiring_products",
                    Title = "Productos por Vencer",
                    Description = "Ver productos próximos a vencer",
                    Category = "Inventario",
                    IconName = "warning",
                    RequiredPermission = "inventario_consultar"
                },
                ["transfers"] = new MenuItemInfo
                {
                    Key = "transfers",
                    Title = "Transferencias",
                    Description = "Transferir productos entre almacenes",
                    Category = "Inventario",
                    IconName = "transfer",
                    RequiredPermission = "inventario_transferencia"
                },

                // Configuración
                ["warehouses"] = new MenuItemInfo
                {
                    Key = "warehouses",
                    Title = "Almacenes",
                    Description = "Gestionar almacenes",
                    Category = "Configuración",
                    IconName = "warehouse",
                    RequiredPermission = "config_almacenes"
                },
                ["locations"] = new MenuItemInfo
                {
                    Key = "locations",
                    Title = "Ubicaciones",
                    Description = "Gestionar ubicaciones en almacenes",
                    Category = "Configuración",
                    IconName = "location",
                    RequiredPermission = "config_almacenes"
                },
                ["users"] = new MenuItemInfo
                {
                    Key = "users",
                    Title = "Usuarios",
                    Description = "Gestionar usuarios del sistema",
                    Category = "Configuración",
                    IconName = "users",
                    RequiredPermission = "usuarios_consultar"
                },
                ["suppliers"] = new MenuItemInfo
                {
                    Key = "suppliers",
                    Title = "Proveedores",
                    Description = "Gestionar proveedores",
                    Category = "Configuración",
                    IconName = "supplier",
                    RequiredPermission = "config_proveedores"
                },

                // Reportes
                ["inventory_report"] = new MenuItemInfo
                {
                    Key = "inventory_report",
                    Title = "Reporte de Inventario",
                    Description = "Generar reporte de inventario",
                    Category = "Reportes",
                    IconName = "report",
                    RequiredPermission = "reportes_inventario"
                },
                ["movement_report"] = new MenuItemInfo
                {
                    Key = "movement_report",
                    Title = "Reporte de Movimientos",
                    Description = "Generar reporte de movimientos",
                    Category = "Reportes",
                    IconName = "report",
                    RequiredPermission = "reportes_inventario"
                },
                ["expiry_report"] = new MenuItemInfo
                {
                    Key = "expiry_report",
                    Title = "Reporte de Vencimientos",
                    Description = "Generar reporte de vencimientos",
                    Category = "Reportes",
                    IconName = "report",
                    RequiredPermission = "reportes_vencimientos"
                }
            };
        }

        private static void InitializePermissionMappings()
        {
            _menuPermissions = new Dictionary<string, string>();

            foreach (var item in _menuItems.Values)
            {
                if (!string.IsNullOrEmpty(item.RequiredPermission))
                {
                    _menuPermissions[item.Key] = item.RequiredPermission;
                }
            }
        }

        private static void ApplyPermissionsToMenuItem(ToolStripMenuItem menuItem)
        {
            if (menuItem.Tag != null)
            {
                var hasPermission = HasPermissionForAction(menuItem.Tag.ToString());
                menuItem.Enabled = hasPermission;
                menuItem.Visible = hasPermission;
            }

            // Aplicar recursivamente a sub-menús
            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem subMenuItem)
                {
                    ApplyPermissionsToMenuItem(subMenuItem);
                }
            }

            // Ocultar separadores si todos los elementos están ocultos
            HideUnnecessarySeparators(menuItem);
        }

        private static void ApplyPermissionsToTreeNode(TreeNode node)
        {
            if (node.Tag != null)
            {
                var hasPermission = HasPermissionForAction(node.Tag.ToString());

                if (!hasPermission)
                {
                    node.Remove();
                    return;
                }
            }

            // Aplicar recursivamente a nodos hijos
            var nodesToRemove = new List<TreeNode>();
            foreach (TreeNode childNode in node.Nodes)
            {
                if (childNode.Tag != null && !HasPermissionForAction(childNode.Tag.ToString()))
                {
                    nodesToRemove.Add(childNode);
                }
                else
                {
                    ApplyPermissionsToTreeNode(childNode);
                }
            }

            // Remover nodos sin permisos
            foreach (var nodeToRemove in nodesToRemove)
            {
                nodeToRemove.Remove();
            }
        }

        private static void HideUnnecessarySeparators(ToolStripMenuItem menuItem)
        {
            var visibleItems = menuItem.DropDownItems.Cast<ToolStripItem>()
                .Where(item => item.Visible).ToList();

            for (int i = 0; i < visibleItems.Count; i++)
            {
                if (visibleItems[i] is ToolStripSeparator)
                {
                    // Ocultar separador si es el primero, último, o si hay otro separador adyacente
                    if (i == 0 || i == visibleItems.Count - 1 ||
                        (i > 0 && visibleItems[i - 1] is ToolStripSeparator))
                    {
                        visibleItems[i].Visible = false;
                    }
                }
            }
        }

        public static List<string> GetAvailableCategories()
        {
            return _menuItems.Values
                .Where(m => string.IsNullOrEmpty(m.RequiredPermission) ||
                           PermissionManager.HasPermission(m.RequiredPermission))
                .Select(m => m.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        public static List<MenuItemInfo> GetAvailableMenuItems()
        {
            return _menuItems.Values
                .Where(m => string.IsNullOrEmpty(m.RequiredPermission) ||
                           PermissionManager.HasPermission(m.RequiredPermission))
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Title)
                .ToList();
        }
    }

    public class MenuItemInfo
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string IconName { get; set; }
        public string RequiredPermission { get; set; }
        public bool IsVisible => string.IsNullOrEmpty(RequiredPermission) ||
                                PermissionManager.HasPermission(RequiredPermission);
    }

    public static class NavigationHelper
    {
        public static TreeNode CreateNavigationTree()
        {
            var rootNode = new TreeNode("Sistema Farmacia");
            var categories = MenuManager.GetAvailableCategories();

            foreach (var category in categories)
            {
                var categoryNode = new TreeNode(GetCategoryIcon(category) + " " + category)
                {
                    Tag = category.ToLower().Replace(" ", "_")
                };

                var menuItems = MenuManager.GetMenuItemsByCategory(category)
                    .Where(m => m.IsVisible)
                    .OrderBy(m => m.Title);

                foreach (var menuItem in menuItems)
                {
                    var itemNode = new TreeNode(menuItem.Title)
                    {
                        Tag = menuItem.Key,
                        ToolTipText = menuItem.Description
                    };
                    categoryNode.Nodes.Add(itemNode);
                }

                if (categoryNode.Nodes.Count > 0)
                {
                    rootNode.Nodes.Add(categoryNode);
                }
            }

            return rootNode;
        }

        private static string GetCategoryIcon(string category)
        {
            switch (category)
            {
                case "Principal": return "📊";
                case "Productos": return "📦";
                case "Inventario": return "📋";
                case "Configuración": return "⚙️";
                case "Reportes": return "📈";
                default: return "📁";
            }
        }

        public static ToolStripMenuItem CreateMenuFromStructure()
        {
            var rootMenu = new ToolStripMenuItem("Sistema");
            var categories = MenuManager.GetAvailableCategories();

            foreach (var category in categories)
            {
                var categoryMenu = new ToolStripMenuItem(category);
                var menuItems = MenuManager.GetMenuItemsByCategory(category)
                    .Where(m => m.IsVisible)
                    .OrderBy(m => m.Title);

                foreach (var menuItem in menuItems)
                {
                    var item = new ToolStripMenuItem(menuItem.Title)
                    {
                        Tag = menuItem.Key,
                        ToolTipText = menuItem.Description
                    };
                    categoryMenu.DropDownItems.Add(item);
                }

                if (categoryMenu.DropDownItems.Count > 0)
                {
                    rootMenu.DropDownItems.Add(categoryMenu);
                }
            }

            return rootMenu;
        }
    }
}
