using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game.Forms
{
    public partial class LeaderboardForm : Form
    {
        private ListView lvLeaderboard;

        public LeaderboardForm()
        {
            InitializeComponent();
            LoadLeaderboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Tabla de Líderes";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            Label titleLabel = new Label
            {
                Text = "?? TOP 10 JUGADORES",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                AutoSize = true,
                Location = new Point(150, 20)
            };

            lvLeaderboard = new ListView
            {
                Name = "lvLeaderboard",
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10),
                Size = new Size(550, 350),
                Location = new Point(20, 70),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };

            lvLeaderboard.Columns.Add("Posición", 80);
            lvLeaderboard.Columns.Add("Jugador", 200);
            lvLeaderboard.Columns.Add("Nivel", 80);
            lvLeaderboard.Columns.Add("Puntos", 90);
            lvLeaderboard.Columns.Add("Fecha", 100);

            Button btnClose = new Button
            {
                Text = "Cerrar",
                Font = new Font("Segoe UI", 12),
                Size = new Size(200, 40),
                Location = new Point(200, 430),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { titleLabel, lvLeaderboard, btnClose });
        }

        private void LoadLeaderboard()
        {
            var facade = new Facade.GameFacade();
            var scores = facade.GetLeaderboard(10);

            int position = 1;
            foreach (var score in scores)
            {
                var item = new ListViewItem(position.ToString());
                item.SubItems.Add(score.PlayerName);
                item.SubItems.Add(score.CurrentLevel.ToString());
                item.SubItems.Add(score.MaxScore.ToString());
                item.SubItems.Add(score.DatePlayed.ToString("dd/MM/yyyy"));
                lvLeaderboard.Items.Add(item);
                position++;
            }
        }
    }
}
