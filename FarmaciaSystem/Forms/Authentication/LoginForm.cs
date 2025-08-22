using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Forms.Base;
using FarmaciaSystem.Forms.Main;
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
    public partial class LoginForm : BaseForm
    {
        private Panel pnlMain;
        private Panel pnlLogin;
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private PictureBox picLogo;
        private Label lblVersion;
        private ProgressBar progressBar;

        private AuthenticationService _authService;
        private int _loginAttempts = 0;

        public LoginForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureLoginForm();
        }

        private void InitializeServices()
        {
            var userRepository = new UserRepository();
            _authService = new AuthenticationService(userRepository);
        }

        private void ConfigureLoginForm()
        {
            this.Text = "Sistema Farmacia - Iniciar Sesión";
            this.Size = new Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(52, 73, 94);

            CreateLoginInterface();
            SetDefaultCredentials();
        }

        private void CreateLoginInterface()
        {
            // Panel principal
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            // Panel de login
            pnlLogin.Location = new Point((this.Width - pnlLogin.Width) / 2, (this.Height - pnlLogin.Height) / 2);

            // Logo/Ícono
            picLogo = new PictureBox
            {
                Size = new Size(64, 64),
                Location = new Point((pnlLogin.Width - 64) / 2, 20),
                BackColor = Color.FromArgb(46, 204, 113),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Título
            lblTitle = new Label
            {
                Text = "SISTEMA FARMACIA",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Size = new Size(300, 25),
                Location = new Point(25, 100),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Usuario
            lblUsername = new Label
            {
                Text = "Usuario:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(25, 140),
                Size = new Size(80, 20)
            };

            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(25, 165),
                Size = new Size(300, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Contraseña
            lblPassword = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(25, 200),
                Size = new Size(80, 20)
            };

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(25, 225),
                Size = new Size(300, 25),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Botones
            btnLogin = new Button
            {
                Text = "INGRESAR",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(140, 35),
                Location = new Point(25, 260),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            btnExit = new Button
            {
                Text = "SALIR",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(140, 35),
                Location = new Point(185, 260),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;

            // Progress bar (oculto inicialmente)
            progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Location = new Point(25, 265),
                Size = new Size(300, 25),
                Visible = false
            };

            // Versión
            lblVersion = new Label
            {
                Text = "Versión 1.0.0",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(10, this.Height - 40),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Agregar controles
            pnlLogin.Controls.Add(picLogo);
            pnlLogin.Controls.Add(lblTitle);
            pnlLogin.Controls.Add(lblUsername);
            pnlLogin.Controls.Add(txtUsername);
            pnlLogin.Controls.Add(lblPassword);
            pnlLogin.Controls.Add(txtPassword);
            pnlLogin.Controls.Add(btnLogin);
            pnlLogin.Controls.Add(btnExit);
            pnlLogin.Controls.Add(progressBar);

            pnlMain.Controls.Add(pnlLogin);
            pnlMain.Controls.Add(lblVersion);

            this.Controls.Add(pnlMain);

            // Eventos
            btnLogin.Click += BtnLogin_Click;
            btnExit.Click += BtnExit_Click;
            txtPassword.KeyPress += TxtPassword_KeyPress;
            txtUsername.KeyPress += TxtUsername_KeyPress;

            // Focus inicial
            this.ActiveControl = txtUsername;
        }

        private void SetDefaultCredentials()
        {
            // Para desarrollo, usar credenciales por defecto
            txtUsername.Text = "admin";
            txtPassword.Text = "admin123";
        }

        private void TxtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                txtPassword.Focus();
                e.Handled = true;
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
                e.Handled = true;
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            await PerformLoginAsync();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowMessage("Debe ingresar un nombre de usuario", MessageType.Warning);
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowMessage("Debe ingresar una contraseña", MessageType.Warning);
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        private async Task PerformLoginAsync()
        {
            try
            {
                SetLoginControlsEnabled(false);
                ShowProgress(true);

                var user = await _authService.AuthenticateAsync(txtUsername.Text.Trim(), txtPassword.Text);

                if (user != null)
                {
                    SessionManager.StartSession(user);

                    // Ocultar formulario de login
                    this.Hide();

                    // Mostrar formulario principal
                    var mainForm = new MainForm();
                    var result = mainForm.ShowDialog();

                    // Cuando se cierre el formulario principal, cerrar la aplicación
                    Application.Exit();
                }
                else
                {
                    _loginAttempts++;
                    if (_loginAttempts >= 3)
                    {
                        ShowMessage("Demasiados intentos fallidos. La aplicación se cerrará.", MessageType.Error);
                        Application.Exit();
                        return;
                    }

                    ShowMessage($"Usuario o contraseña incorrectos. Intento {_loginAttempts} de 3.", MessageType.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error al intentar iniciar sesión: {ex.Message}", MessageType.Error);
            }
            finally
            {
                SetLoginControlsEnabled(true);
                ShowProgress(false);
            }
        }

        private void SetLoginControlsEnabled(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnLogin.Text = enabled ? "INGRESAR" : "INGRESANDO...";
        }

        private void ShowProgress(bool show)
        {
            progressBar.Visible = show;
            btnLogin.Visible = !show;
            btnExit.Visible = !show;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (ShowConfirmation("¿Está seguro que desea salir de la aplicación?", "Confirmar Salida"))
            {
                Application.Exit();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!ShowConfirmation("¿Está seguro que desea salir de la aplicación?", "Confirmar Salida"))
                {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}
