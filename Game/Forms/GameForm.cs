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
        private System.Windows.Forms.Timer _shuffleTimer;
        private Label lblLevel, lblScore, lblStreak, lblQuestion, lblTimer, lblPenalty;
        private Button[] optionButtons;
        private Button btnEliminate, btnTime, btnRetry, btnDouble;
        private Panel powerUpPanel;
        private List<PowerUpType> powersUsed = new List<PowerUpType>();
        private int _shuffleCountdown = 0;

        private Button btnPowerUpActivated = new Button
        {
            Text = "",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Size = new Size(400, 45),
            Location = new Point(250, 430), // Movido debajo de las opciones
            BackColor = System.Drawing.Color.Transparent, // Verde sólido
            ForeColor = Color.FromArgb(255, 21, 255, 0),
            FlatStyle = FlatStyle.Flat,
            Visible = false,
            TextAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft
        };

        public GameForm(string playerName)
        {
            _gameManager = new GameManager(playerName);
            InitializeComponent();
            _gameManager.StartLevel(1);
            UpdateUI("Start");
            StartTimer();
            
            // Suscribirse al evento de cierre del formulario
            this.FormClosing += GameForm_FormClosing;
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
            lblTimer = CreateLabel("⏱ 15s", new Point(30, 70), new Font("Segoe UI", 24, FontStyle.Bold), Color.FromArgb(30, 67, 54));
            // lblPenalty a la derecha del timer para mejor visibilidad
            lblPenalty = CreateLabel("", new Point(150, 78), new Font("Segoe UI", 18, FontStyle.Bold | FontStyle.Italic), Color.FromArgb(255, 152, 0));
            lblPenalty.MaximumSize = new Size(600, 0); // Ancho máximo para que no se salga del formulario
            lblPenalty.AutoSize = true;
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

            // Configurar apariencia del botón de power-up activado
            btnPowerUpActivated.FlatAppearance.BorderSize = 0;
            btnPowerUpActivated.FlatAppearance.MouseOverBackColor = btnPowerUpActivated.BackColor;
            this.Controls.AddRange(new Control[] {
                lblLevel, lblScore, lblStreak, lblTimer, lblPenalty, lblQuestion, btnPowerUpActivated
            });
            this.Controls.AddRange(optionButtons);
            this.Controls.Add(powerUpPanel);
            
            // Asegurar que el botón de power-up esté al frente
            btnPowerUpActivated.BringToFront();

            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;

            // Timer de shuffle se ejecuta cada segundo para poder mostrar alertas
            _shuffleTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _shuffleTimer.Tick += ShuffleTimer_Tick;
            _gameManager.IsShufflePenaltyActive = false;
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
            PowerUp eliminate = new PowerUp(PowerUpType.EliminateOption);
            btnEliminate = CreatePowerUpButton(eliminate.ShortName + " (2)", new Point(10, 50), PowerUpType.EliminateOption, eliminate.Image);
            PowerUp time = new PowerUp(PowerUpType.ExtraTime);
            btnTime = CreatePowerUpButton(time.ShortName + " (2)", new Point(210, 50), PowerUpType.ExtraTime, time.Image);
            PowerUp retry = new PowerUp(PowerUpType.Retry);
            btnRetry = CreatePowerUpButton(retry.ShortName + " (2)", new Point(410, 50), PowerUpType.Retry, retry.Image);
            PowerUp dPoints = new PowerUp(PowerUpType.DoublePoints);
            btnDouble = CreatePowerUpButton(dPoints.ShortName + " (2)", new Point(610, 50), PowerUpType.DoublePoints, dPoints.Image);

            powerUpPanel.Controls.AddRange(new Control[] { btnEliminate, btnTime, btnRetry, btnDouble });
        }

        private Button CreatePowerUpButton(string text, Point location, PowerUpType type, Bitmap image)
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
                Tag = type,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                Image = image
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
                
                // Crear instancia de PowerUp para obtener la descripción
                var powerUp = new PowerUp(powerUpType);
                
                // Mostrar indicador visual del power-up activado
                btnPowerUpActivated.Image = powerUp.Image;
                btnPowerUpActivated.Text = $"   ✅ {powerUp.Name}: {powerUp.Description}";
                btnPowerUpActivated.TextImageRelation = TextImageRelation.ImageBeforeText;
                btnPowerUpActivated.ImageAlign = ContentAlignment.MiddleLeft;
                btnPowerUpActivated.TextAlign = ContentAlignment.MiddleLeft;
                btnPowerUpActivated.Visible = true;
                btnPowerUpActivated.BringToFront();
                
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
            _shuffleTimer.Stop();
            _shuffleCountdown = 0; // Resetear contador

            bool isCorrect = _gameManager.CheckAnswer(selectedAnswer);

            if (isCorrect)
            {
                button.BackColor = Color.FromArgb(76, 175, 80);
                MessageBox.Show("✅ ¡Correcto!", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _gameManager.IsShufflePenaltyActive = false;
                _gameManager.LastPenaltyMessage = "";
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
                _shuffleTimer.Stop();
                _shuffleCountdown = 0; // Resetear contador
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
            
            // Actualizar mensaje de penalización y traerlo al frente
            lblPenalty.Text = _gameManager.LastPenaltyMessage;
            if (!string.IsNullOrEmpty(_gameManager.LastPenaltyMessage))
            {
                lblPenalty.BringToFront();
            }
            
            btnPowerUpActivated.Visible = false;

            UpdateOptionsDisplay();
            UpdatePowerUpButtons(gameState);
        }

        private void UpdateOptionsDisplay()
        {
            // Método separado para actualizar solo las opciones
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
        }

        private void UpdatePowerUpButtons(String state)
        {
            if (_gameManager.IsGhostAnswerActive)
            {
                var rnd = new Random();
                int numberAnswer = rnd.Next(0, 4);
                optionButtons[numberAnswer].BackColor = Color.FromArgb(50, 205, 243);
                _gameManager.Penality = PenaltyType.Nothing;
            }
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
            
            // Solo iniciar el timer de mezcla si la penalización está activa
            if (_gameManager.IsShufflePenaltyActive)
            {
                _shuffleCountdown = 0; // Resetear contador
                _shuffleTimer.Start();
            }

        }

        private void ShuffleTimer_Tick(object sender, EventArgs e)
        {            
            // Solo mezclar si la penalización sigue activa
            if (_gameManager.IsShufflePenaltyActive)
            {
                _shuffleCountdown++;
                
                if (_shuffleCountdown == 5)
                {
                    // ⚠️ ALERTA: 2 segundos antes del shuffle
                    ShowShuffleWarning();
                }
                else if (_shuffleCountdown >= 7)
                {
                    // 🔀 SHUFFLE: Mezclar opciones
                    _gameManager.ShuffleCurrentOptions();
                    UpdateOptionsDisplay();
                    
                    // Actualizar mensaje y resetear contador
                    lblPenalty.Text = "🔀 ¡Opciones mezcladas! (Penalización activa)";
                    lblPenalty.ForeColor = Color.FromArgb(255, 87, 34);
                    lblPenalty.BringToFront();
                    
                    _shuffleCountdown = 0; // Resetear para el próximo ciclo
                }
            }
            else
            {
                _shuffleTimer.Stop();
                _shuffleCountdown = 0;
            }
        }

        private void ShowShuffleWarning()
        {
            // Cambiar color de fondo de los botones a amarillo/naranja como advertencia
            foreach (var btn in optionButtons)
            {
                if (btn.Visible)
                {
                    btn.BackColor = Color.FromArgb(255, 193, 7);
                    btn.ForeColor = Color.Black;
                }
            }
            
            // Actualizar mensaje con cuenta regresiva
            lblPenalty.Text = "⚠️ ¡ALERTA! Las opciones se mezclarán";
            lblPenalty.ForeColor = Color.FromArgb(255, 193, 7);
            
            // Hacer que el mensaje parpadee y traerlo al frente
            lblPenalty.Font = new Font(lblPenalty.Font, FontStyle.Bold | FontStyle.Italic);
            lblPenalty.BringToFront();
        }
        
        private void LevelComplete()
        {
            _timer.Stop();
            _shuffleTimer.Stop();
            _shuffleCountdown = 0;
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
            _shuffleTimer.Stop();
            _shuffleCountdown = 0;
            _gameManager.SaveProgress();
            MessageBox.Show(
                $"🏆 ¡JUEGO TERMINADO!\n\nJugador: {_gameManager.Player.Name}\nNivel alcanzado: {_gameManager.CurrentLevel.Number}\nPuntuación final: {_gameManager.Player.Score}",
                "Fin del Juego",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            this.Close();
        }
        
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Detener todos los timers
            _timer?.Stop();
            _shuffleTimer?.Stop();
            
            // Guardar el progreso del jugador antes de cerrar
            try
            {
                _gameManager.SaveProgress();
            }
            catch (Exception ex)
            {                
                // Preguntar al usuario si desea cerrar sin guardar
                var result = MessageBox.Show(
                    "Hubo un error al guardar tu progreso. ¿Deseas cerrar de todos modos?",
                    "Error al guardar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                
                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Cancelar el cierre
                }
            }
        }
    }
}
