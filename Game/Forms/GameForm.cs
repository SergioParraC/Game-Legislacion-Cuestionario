using Game.Enums;
using Game.Managers;
using Game.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game.Forms
{
    public partial class GameForm : Form
    {
        private readonly GameManager _gameManager;
        private System.Windows.Forms.Timer _timer;
        private Label lblLevel, lblScore, lblStreak, lblQuestion, lblTimer, lblPenalty;
        private Button[] optionButtons;
        private Button btnEliminate, btnTime, btnRetry, btnDouble;
        private Panel powerUpPanel;
        private List<PowerUpType> powersUsed = new List<PowerUpType>();

        public GameForm(string playerName)
        {
            _gameManager = new GameManager(playerName);
            InitializeComponent();
            _gameManager.StartLevel(1);
            UpdateUI("Start");
            StartTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Trivia Challenge - ¡A Jugar!";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            lblLevel = CreateLabel("NIVEL 1", new Point(20, 20), new Font("Segoe UI", 16, FontStyle.Bold), Color.FromArgb(255, 193, 7));
            lblScore = CreateLabel("Puntos: 0", new Point(350, 20), new Font("Segoe UI", 16, FontStyle.Bold), Color.FromArgb(76, 175, 80));
            lblStreak = CreateLabel("Racha: 0", new Point(650, 20), new Font("Segoe UI", 16, FontStyle.Bold), Color.FromArgb(156, 39, 176));
            lblTimer = CreateLabel("⏱ 15s", new Point(400, 70), new Font("Segoe UI", 24, FontStyle.Bold), Color.FromArgb(244, 67, 54));
            lblPenalty = CreateLabel("", new Point(200, 120), new Font("Segoe UI", 12, FontStyle.Italic), Color.FromArgb(255, 152, 0));

            lblQuestion = new Label
            {
                Text = "Pregunta aquí",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(50, 170),
                Size = new Size(800, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            optionButtons = new Button[4];
            for (int i = 0; i < 4; i++)
            {
                optionButtons[i] = new Button
                {
                    Font = new Font("Segoe UI", 12),
                    Size = new Size(380, 60),
                    Location = new Point(50 + (i % 2) * 420, 280 + (i / 2) * 80),
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = i
                };
                optionButtons[i].FlatAppearance.BorderSize = 0;
                optionButtons[i].Click += OptionButton_Click;
            }

            CreatePowerUpPanel();

            this.Controls.AddRange(new Control[] {
                lblLevel, lblScore, lblStreak, lblTimer, lblPenalty, lblQuestion
            });
            this.Controls.AddRange(optionButtons);
            this.Controls.Add(powerUpPanel);

            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
        }

        private void CreatePowerUpPanel()
        {
            powerUpPanel = new Panel
            {
                Location = new Point(50, 480),
                Size = new Size(800, 150),
                BackColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label powerUpTitle = CreateLabel("⚡ POWER-UPS", new Point(10, 10), new Font("Segoe UI", 12, FontStyle.Bold), Color.FromArgb(255, 193, 7));
            powerUpPanel.Controls.Add(powerUpTitle);

            btnEliminate = CreatePowerUpButton("🚫 Eliminar (3)", new Point(10, 50), PowerUpType.EliminateOption);
            btnTime = CreatePowerUpButton("⏰ +5s (3)", new Point(210, 50), PowerUpType.ExtraTime);
            btnRetry = CreatePowerUpButton("🔄 Reintento (2)", new Point(410, 50), PowerUpType.Retry);
            btnDouble = CreatePowerUpButton("⭐ x2 (2)", new Point(610, 50), PowerUpType.DoublePoints);

            powerUpPanel.Controls.AddRange(new Control[] { btnEliminate, btnTime, btnRetry, btnDouble });
        }

        private Button CreatePowerUpButton(string text, Point location, PowerUpType type)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(180, 70),
                Location = location,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = type
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += PowerUpButton_Click;
            return btn;
        }

        private Label CreateLabel(string text, Point location, Font font, Color color)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                Location = location,
                AutoSize = true
            };
        }

        private void PowerUpButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var powerUpType = (PowerUpType)button.Tag;
            powersUsed.Add(powerUpType);
            if (_gameManager.UsePlayerPowerUp(powerUpType))
            {
                UpdatePowerUpButtons("");
                UpdateUI();
                
                string message = powerUpType == PowerUpType.Retry 
                    ? "¡CAMBIO DE PREGUNTA activado!\n\nSi fallas, se cargará una pregunta diferente sin penalización." 
                    : $"¡Power-up {powerUpType} activado!";
                
                MessageBox.Show(message, "Power-Up", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No tienes este power-up disponible o está bloqueado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OptionButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string selectedAnswer = button.Text;

            _timer.Stop();

            bool isCorrect = _gameManager.CheckAnswer(selectedAnswer);

            if (isCorrect)
            {
                button.BackColor = Color.FromArgb(76, 175, 80);
                MessageBox.Show("✅ ¡Correcto!", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                System.Threading.Thread.Sleep(500);
                
                // Avanzar a la siguiente pregunta
                if (_gameManager.IsLevelComplete())
                {
                    LevelComplete();
                }
                else
                {
                    _gameManager.LoadNextQuestion();
                    UpdateUI();
                    StartTimer();
                }
            }
            else
            {
                button.BackColor = Color.FromArgb(244, 67, 54);
                
                // Verificar si tiene reintento disponible (cambio de pregunta)
                if (_gameManager.IsRetryAvailable)
                {
                    MessageBox.Show($"❌ Incorrecto. ¡CAMBIO DE PREGUNTA activado!\n\nSe cargará una nueva pregunta.", 
                        "Cambio de Pregunta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Cambiar a la pregunta de reserva
                    _gameManager.ChangeToReserveQuestion();
                    
                    // Restaurar UI con la nueva pregunta
                    UpdateUI();
                    StartTimer();
                }
                else
                {
                    MessageBox.Show($"❌ Incorrecto. La respuesta correcta era: {_gameManager.CurrentQuestion.GetCorrectAnswer()}", 
                        "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    System.Threading.Thread.Sleep(500);
                    
                    // Avanzar a la siguiente pregunta
                    if (_gameManager.IsLevelComplete())
                    {
                        LevelComplete();
                    }
                    else
                    {
                        _gameManager.LoadNextQuestion();
                        UpdateUI();
                        StartTimer();
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _gameManager.TimeRemaining--;
            lblTimer.Text = $"⏱ {_gameManager.TimeRemaining}s";

            if (_gameManager.TimeRemaining <= 5)
                lblTimer.ForeColor = Color.Red;

            if (_gameManager.TimeRemaining <= 0)
            {
                _timer.Stop();
                MessageBox.Show("⏰ ¡Se acabó el tiempo!", "Tiempo agotado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _gameManager.CheckAnswer("");
                
                if (_gameManager.IsLevelComplete())
                    LevelComplete();
                else
                {
                    _gameManager.LoadNextQuestion();
                    UpdateUI();
                    StartTimer();
                }
            }
        }

        private void UpdateUI(String gameState = "")
        {
            lblLevel.Text = $"NIVEL {_gameManager.CurrentLevel.Number}";
            lblScore.Text = $"Puntos: {_gameManager.Player.Score}";
            lblStreak.Text = $"Racha: {_gameManager.Player.CorrectStreak}";
            lblTimer.Text = $"⏱ {_gameManager.TimeRemaining}s";
            lblTimer.ForeColor = Color.FromArgb(244, 67, 54);
            lblQuestion.Text = _gameManager.CurrentQuestion.Text;
            lblPenalty.Text = _gameManager.LastPenaltyMessage;

            for (int i = 0; i < _gameManager.DisplayedOptions.Count && i < 4; i++)
            {
                optionButtons[i].Text = _gameManager.DisplayedOptions[i];
                optionButtons[i].BackColor = Color.FromArgb(33, 150, 243);
                optionButtons[i].Visible = true;
            }

            for (int i = _gameManager.DisplayedOptions.Count; i < 4; i++)
            {
                optionButtons[i].Visible = false;
            }

            UpdatePowerUpButtons(gameState);
        }

        private void UpdatePowerUpButtons(String state)
        {
            // Texto de los power-ups
            btnEliminate.Text = $"🚫 Eliminar ({_gameManager.Player.PowerUps[PowerUpType.EliminateOption]})";
            btnTime.Text = $"⏰ +5s ({_gameManager.Player.PowerUps[PowerUpType.ExtraTime]})";
            btnRetry.Text = $"🔄 Reintento ({_gameManager.Player.PowerUps[PowerUpType.Retry]})";
            btnDouble.Text = $"⭐ x2 ({_gameManager.Player.PowerUps[PowerUpType.DoublePoints]})";
            // Habilitar o deshabilitar botones según disponibilidad
            if (_gameManager.Penality == PenaltyType.BlockPowerUp)
            {
                // Si se ha aplicado una penalización, bloquear todos los power-ups durante esa pregunta
                btnEliminate.Enabled = false;
                btnEliminate.BackColor = Color.FromArgb(64, 64, 64);
                btnTime.Enabled = false;
                btnTime.BackColor = Color.FromArgb(64, 64, 64);
                btnRetry.Enabled = false;
                btnRetry.BackColor = Color.FromArgb(64, 64, 64);
                btnDouble.Enabled = false;
                btnDouble.BackColor = Color.FromArgb(64, 64, 64);
                _gameManager.Penality = PenaltyType.Nothing;
            }
            else
            {
                btnEliminate.Enabled = true;
                btnEliminate.BackColor = Color.FromArgb(255, 193, 7);
                btnTime.Enabled = true;
                btnTime.BackColor = Color.FromArgb(255, 193, 7);
                btnRetry.Enabled = true;
                btnRetry.BackColor = Color.FromArgb(255, 193, 7);
                btnDouble.Enabled = true;
                btnDouble.BackColor = Color.FromArgb(255, 193, 74);
                _gameManager.Penality = PenaltyType.Nothing;
            }
            foreach(PowerUpType power in powersUsed)
            {
                if (power == PowerUpType.EliminateOption)
                {
                    // Si se usó el power-up de eliminar, deshabilitarlo inmediatamente para evitar múltiples usos
                    btnEliminate.BackColor = Color.FromArgb(64, 64, 64);
                    btnEliminate.Enabled = false;
                }
    
                if (power == PowerUpType.ExtraTime)
                {
                    btnTime.BackColor = Color.FromArgb(64, 64, 64);
                    btnTime.Enabled = false;
                }
    
                if (power == PowerUpType.Retry)
                {
                    btnRetry.BackColor = Color.FromArgb(64, 64, 64);
                    btnRetry.Enabled = false;
                }
                    
                if (power == PowerUpType.DoublePoints)
                {
                    btnDouble.BackColor = Color.FromArgb(64, 64, 64);
                    btnDouble.Enabled = false;
                }

            }
            if (state == "Start")
            {
                btnEliminate.Enabled = !_gameManager.IsPowerUpBlocked && _gameManager.Player.PowerUps[PowerUpType.EliminateOption] > 0;
                btnTime.Enabled = !_gameManager.IsPowerUpBlocked && _gameManager.Player.PowerUps[PowerUpType.ExtraTime] > 0;
                btnRetry.Enabled = !_gameManager.IsPowerUpBlocked && _gameManager.Player.PowerUps[PowerUpType.Retry] > 0;
                btnDouble.Enabled = !_gameManager.IsPowerUpBlocked && _gameManager.Player.PowerUps[PowerUpType.DoublePoints] > 0;
            }

        }

        private void StartTimer()
        {
            _timer.Start();
        }

        private void LevelComplete()
        {
            _timer.Stop();
            powersUsed.Clear();
            if (_gameManager.CurrentLevel.Number < 4)
            {
                var result = MessageBox.Show(
                    $"🎉 ¡Nivel {_gameManager.CurrentLevel.Number} completado!\n\nPuntos: {_gameManager.Player.Score}\n¿Continuar al siguiente nivel?",
                    "Nivel Completado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _gameManager.StartLevel(_gameManager.CurrentLevel.Number + 1);
                    UpdateUI("Start");
                    StartTimer();
                }
                else
                {
                    GameOver();
                }
            }
            else
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            _timer.Stop();
            _gameManager.SaveProgress();
            MessageBox.Show(
                $"🏆 ¡JUEGO TERMINADO!\n\nJugador: {_gameManager.Player.Name}\nNivel alcanzado: {_gameManager.CurrentLevel.Number}\nPuntuación final: {_gameManager.Player.Score}",
                "Fin del Juego",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            this.Close();
        }
    }
}
