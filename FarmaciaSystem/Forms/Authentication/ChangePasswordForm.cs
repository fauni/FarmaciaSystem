using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Forms.Base;
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

namespace FarmaciaSystem.Forms.Authentication
{
    public partial class ChangePasswordForm : BaseForm
    {
        private Panel pnlMain;
        private Label lblTitle;
        private Label lblCurrentPassword;
        private TextBox txtCurrentPassword;
        private Label lblNewPassword;
        private TextBox txtNewPassword;
        private Label lblConfirmPassword;
        private TextBox txtConfirmPassword;
        private Button btnSave;
        private Button btnCancel;
        private Label lblPasswordStrength;

        private readonly AuthenticationService _authService;

        public ChangePasswordForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureForm();
        }

        private void InitializeComponent()
        {
            this.pnlMain = new Panel();
            this.lblTitle = new Label();
            this.lblCurrentPassword = new Label();
            this.txtCurrentPassword = new TextBox();
            this.lblNewPassword = new Label();
            this.txtNewPassword = new TextBox();
            this.lblConfirmPassword = new Label();
            this.txtConfirmPassword = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.lblPasswordStrength = new Label();

            this.pnlMain.SuspendLayout();
            this.SuspendLayout();

            // 
            // ChangePasswordForm
            // 
            this.ClientSize = new Size(400, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Cambiar Contraseña";
            this.StartPosition = FormStartPosition.CenterParent;

            // 
            // pnlMain
            // 
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Padding = new Padding(30);
            this.pnlMain.BackColor = Color.White;

            // 
            // lblTitle
            // 
            this.lblTitle.Text = "Cambiar Contraseña";
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            this.lblTitle.Location = new Point(30, 30);
            this.lblTitle.Size = new Size(340, 30);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // lblCurrentPassword
            // 
            this.lblCurrentPassword.Text = "Contraseña Actual:";
            this.lblCurrentPassword.Font = new Font("Segoe UI", 10F);
            this.lblCurrentPassword.Location = new Point(30, 80);
            this.lblCurrentPassword.Size = new Size(340, 20);

            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Font = new Font("Segoe UI", 10F);
            this.txtCurrentPassword.Location = new Point(30, 105);
            this.txtCurrentPassword.Size = new Size(340, 25);
            this.txtCurrentPassword.UseSystemPasswordChar = true;

            // 
            // lblNewPassword
            // 
            this.lblNewPassword.Text = "Nueva Contraseña:";
            this.lblNewPassword.Font = new Font("Segoe UI", 10F);
            this.lblNewPassword.Location = new Point(30, 145);
            this.lblNewPassword.Size = new Size(340, 20);

            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Font = new Font("Segoe UI", 10F);
            this.txtNewPassword.Location = new Point(30, 170);
            this.txtNewPassword.Size = new Size(340, 25);
            this.txtNewPassword.UseSystemPasswordChar = true;
            this.txtNewPassword.TextChanged += TxtNewPassword_TextChanged;

            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.Text = "Confirmar Contraseña:";
            this.lblConfirmPassword.Font = new Font("Segoe UI", 10F);
            this.lblConfirmPassword.Location = new Point(30, 210);
            this.lblConfirmPassword.Size = new Size(340, 20);

            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Font = new Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new Point(30, 235);
            this.txtConfirmPassword.Size = new Size(340, 25);
            this.txtConfirmPassword.UseSystemPasswordChar = true;

            // 
            // lblPasswordStrength
            // 
            this.lblPasswordStrength.Font = new Font("Segoe UI", 8F);
            this.lblPasswordStrength.Location = new Point(30, 195);
            this.lblPasswordStrength.Size = new Size(340, 15);
            this.lblPasswordStrength.ForeColor = Color.Gray;

            // 
            // btnSave
            // 
            this.btnSave.Text = "Cambiar";
            this.btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.Location = new Point(180, 280);
            this.btnSave.BackColor = Color.FromArgb(46, 204, 113);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Click += BtnSave_Click;

            // 
            // btnCancel
            // 
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Font = new Font("Segoe UI", 10F);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Location = new Point(290, 280);
            this.btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Click += BtnCancel_Click;

            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Controls.Add(this.lblCurrentPassword);
            this.pnlMain.Controls.Add(this.txtCurrentPassword);
            this.pnlMain.Controls.Add(this.lblNewPassword);
            this.pnlMain.Controls.Add(this.txtNewPassword);
            this.pnlMain.Controls.Add(this.lblPasswordStrength);
            this.pnlMain.Controls.Add(this.lblConfirmPassword);
            this.pnlMain.Controls.Add(this.txtConfirmPassword);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.btnCancel);

            this.Controls.Add(this.pnlMain);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);
        }

        private void InitializeServices()
        {
            var userRepository = new UserRepository();
            _authService = new AuthenticationService(userRepository);
        }

        private void ConfigureForm()
        {
            this.ActiveControl = txtCurrentPassword;
        }

        private void TxtNewPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordStrength();
        }

        private void UpdatePasswordStrength()
        {
            var password = txtNewPassword.Text;

            if (string.IsNullOrEmpty(password))
            {
                lblPasswordStrength.Text = "";
                lblPasswordStrength.ForeColor = Color.Gray;
                return;
            }

            if (PasswordHelper.IsPasswordStrong(password))
            {
                lblPasswordStrength.Text = "Contraseña segura ✓";
                lblPasswordStrength.ForeColor = Color.FromArgb(46, 204, 113);
            }
            else
            {
                lblPasswordStrength.Text = "Debe contener mayúsculas, minúsculas y números (mín. 6 caracteres)";
                lblPasswordStrength.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                btnSave.Enabled = false;
                btnSave.Text = "Cambiando...";

                var success = await _authService.ChangePasswordAsync(
                    CurrentUser.Id,
                    txtCurrentPassword.Text,
                    txtNewPassword.Text);

                if (success)
                {
                    ShowMessage("Contraseña cambiada exitosamente", MessageType.Success);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowMessage("La contraseña actual es incorrecta", MessageType.Error);
                    txtCurrentPassword.Clear();
                    txtCurrentPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al cambiar contraseña: {ex.Message}", MessageType.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Cambiar";
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                ShowMessage("Debe ingresar su contraseña actual", MessageType.Warning);
                txtCurrentPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                ShowMessage("Debe ingresar la nueva contraseña", MessageType.Warning);
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                ShowMessage("La nueva contraseña debe tener al menos 6 caracteres", MessageType.Warning);
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                ShowMessage("Las contraseñas no coinciden", MessageType.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            if (txtCurrentPassword.Text == txtNewPassword.Text)
            {
                ShowMessage("La nueva contraseña debe ser diferente a la actual", MessageType.Warning);
                txtNewPassword.Focus();
                return false;
            }

            return true;
        }
    }
}
