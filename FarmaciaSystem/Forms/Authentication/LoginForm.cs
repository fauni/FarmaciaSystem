using FarmaciaSystem.Business.Services;
using FarmaciaSystem.Forms.Base;
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

        private readonly AuthenticationService _authService;

        public LoginForm()
        {
            InitializeComponent();
            InitializeServices();
            ConfigureForm();
        }


    }
}
