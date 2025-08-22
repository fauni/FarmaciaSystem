using FarmaciaSystem.Models;
using FarmaciaSystem.Utils.Security;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FarmaciaSystem.Forms.Base
{
    public partial class BaseForm : Form
    {
        protected User CurrentUser => SessionManager.CurrentUser;
        protected bool IsUserLoggedIn => SessionManager.IsLoggedIn;

        public BaseForm()
        {
            InitializeComponent();
            ConfigureForm();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Configuración básica del formulario
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private void ConfigureForm()
        {
            // Configuración para hardware limitado
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected virtual void CheckPermissions()
        {
            // Override en formularios que requieran permisos específicos
        }

        protected virtual void ShowMessage(string message, MessageType type = MessageType.Information)
        {
            MessageBoxIcon icon;
            switch (type)
            {
                case MessageType.Information:
                    icon = MessageBoxIcon.Information;
                    break;
                case MessageType.Warning:
                    icon = MessageBoxIcon.Warning;
                    break;
                case MessageType.Error:
                    icon = MessageBoxIcon.Error;
                    break;
                case MessageType.Success:
                    icon = MessageBoxIcon.Information;
                    break;
                default:
                    icon = MessageBoxIcon.Information;
                    break;
            }

            MessageBox.Show(message, GetMessageTitle(type), MessageBoxButtons.OK, icon);
        }

        protected virtual bool ShowConfirmation(string message, string title = "Confirmación")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private string GetMessageTitle(MessageType type)
        {
            switch (type)
            {
                case MessageType.Information:
                    return "Información";
                case MessageType.Warning:
                    return "Advertencia";
                case MessageType.Error:
                    return "Error";
                case MessageType.Success:
                    return "Éxito";
                default:
                    return "Sistema Farmacia";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CheckPermissions();
        }
    }

    public enum MessageType
    {
        Information,
        Warning,
        Error,
        Success
    }
}
