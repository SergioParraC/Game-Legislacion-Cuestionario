using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game.Forms
{
    public partial class MainMenuForm : Form
    {
        private TextBox txtPlayerName;
        private Button btnStart;
        private Button btnLeaderboard;
        private Button btnExit;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Trivia Challenge - Menú Principal";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            Label titleLabel = new Label
            {
                Text = "TRIVIA CHALLENGE",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
            };

            PictureBox picTitle = new PictureBox
            {
                Image = Properties.Resources.gameControl,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(80, 80),
                Margin = new Padding(0, 0, 6, 0),
            };

            FlowLayoutPanel flowTitle = new FlowLayoutPanel
            {
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Location = new Point(80, 50),
            };
            flowTitle.Controls.Add(picTitle);
            flowTitle.Controls.Add(titleLabel);

            Label subtitleLabel = new Label
            {
                Text = "¿Estás listo para el desafío?",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(180, 110)
            };

            txtPlayerName = new TextBox
            {
                Name = "txtPlayerName",
                Font = new Font("Segoe UI", 14),
                Size = new Size(300, 35),
                Location = new Point(150, 160),
                Text = "Jugador 1"
            };
            btnStart = new Button
            {
                Image = new Bitmap(Properties.Resources.playGame, new Size(35, 35)),
                Text = "Jugar",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(300, 50),
                Location = new Point(150, 220),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;

            btnLeaderboard = new Button
            {
                Image = new Bitmap(Properties.Resources.positionList, new Size(35, 35)),
                Text = "Tabla de Líderes",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 45),
                Location = new Point(150, 285),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnLeaderboard.FlatAppearance.BorderSize = 0;
            btnLeaderboard.Click += BtnLeaderboard_Click;

            btnExit = new Button
            {
                Image = new Bitmap(Properties.Resources.exit, new Size(35, 35)),
                Text = "Salir",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 45),
                Location = new Point(150, 345),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += BtnExit_Click;

            this.Controls.AddRange(new Control[] {
                flowTitle, subtitleLabel, txtPlayerName,
                btnStart, btnLeaderboard, btnExit
            });
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                MessageBox.Show("Por favor, ingresa tu nombre", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Mostrar primero el formulario de instrucciones
            var instructionsForm = new InstructionsForm();
            var result = instructionsForm.ShowDialog();

            // Si el usuario completa el tutorial, iniciar el juego
            if (result == DialogResult.OK)
            {
                var gameForm = new GameForm(txtPlayerName.Text);
                gameForm.FormClosed += (s, args) => this.Show();
                this.Hide();
                gameForm.Show();
            }
        }

        private void BtnLeaderboard_Click(object sender, EventArgs e)
        {
            var leaderboardForm = new LeaderboardForm();
            leaderboardForm.ShowDialog();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
