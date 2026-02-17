using Game.Enums;
using Game.Models;
using Game.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game.Forms
{
    public partial class InstructionsForm : Form
    {
        private Panel contentPanel;
        private Button btnNext, btnPrevious, btnStart;
        private int currentPage = 0;
        private const int TOTAL_PAGES = 3;

        public InstructionsForm()
        {
            InitializeComponent();
            ShowPage(0);
        }

        private void InitializeComponent()
        {
            this.Text = "Tutorial - Cómo Jugar";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Panel de contenido
            contentPanel = new Panel
            {
                Location = new Point(50, 50),
                Size = new Size(800, 520),
                BackColor = Color.FromArgb(60, 60, 60),
                AutoScroll = true
            };

            // Botón Anterior
            btnPrevious = new Button
            {
                Text = "  Anterior",
                Image = new Bitmap(Resources.leftIcon, new Size(25, 25)),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(150, 50),
                Location = new Point(50, 600),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Visible = false,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft
            };
            btnPrevious.FlatAppearance.BorderSize = 0;
            btnPrevious.Click += BtnPrevious_Click;

            // Botón Siguiente
            btnNext = new Button
            {
                Text = "Siguiente  ",
                Image = new Bitmap(Resources.rigthIcon, new Size(25, 25)),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(150, 50),
                Location = new Point(700, 600),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextImageRelation = TextImageRelation.TextBeforeImage,
                ImageAlign = ContentAlignment.MiddleRight
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;

            // Botón Comenzar (oculto inicialmente)
            btnStart = new Button
            {
                Text = "  ¡Comenzar!",
                Image = new Bitmap(Resources.gameControl, new Size(30, 30)),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(350, 600),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Visible = false,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;

            this.Controls.AddRange(new Control[] { contentPanel, btnPrevious, btnNext, btnStart });
        }

        private void ShowPage(int pageIndex)
        {
            currentPage = pageIndex;
            contentPanel.Controls.Clear();

            // Actualizar visibilidad de botones
            btnPrevious.Visible = currentPage > 0;
            btnNext.Visible = currentPage < TOTAL_PAGES - 1;
            btnStart.Visible = currentPage == TOTAL_PAGES - 1;

            switch (pageIndex)
            {
                case 0:
                    ShowInstructionsPage();
                    break;
                case 1:
                    ShowPowerUpsPage();
                    break;
                case 2:
                    ShowPenaltiesPage();
                    break;
            }
        }

        private void ShowInstructionsPage()
        {
            var title = CreateTitleWithIcon(Resources.gameControl, "Instrucciones del Juego", 20);
            contentPanel.Controls.Add(title.icon);
            contentPanel.Controls.Add(title.label);

            int yPos = 80;
            
            // Objetivo
            CreateInstructionWithIcon(Resources.DianaIcon, "Objetivo: Responde correctamente las preguntas en cada nivel", 30, yPos, true);
            yPos += 40;
            
            // Niveles
            CreateInstructionWithIcon(Resources.estadisctIcon, "Niveles:", 30, yPos, true);
            yPos += 35;
            CreateInstructionText("  • Nivel 1: 5 preguntas, 20 segundos de tiempo", 60, yPos);
            yPos += 25;
            CreateInstructionText("  • Nivel 2: 5 preguntas, 17 segundos de tiempo", 60, yPos);
            yPos += 25;
            CreateInstructionText("  • Nivel 3: 5 preguntas, 15 segundos de tiempo", 60, yPos);
            yPos += 25;
            CreateInstructionText("  • Nivel 4: 5 preguntas, 12 segundos de tiempo y un jefe", 60, yPos);
            yPos += 40;
            
            // Tiempo
            CreateInstructionWithIcon(Resources.TimeOut, "El jefe final tiene todos los Power-Ups desactivados", 30, yPos, true);
            yPos += 40;
            
            // Power-ups iniciales
            CreateInstructionWithIcon(Resources.regaloIcon, "Al inicio de cada nivel, se restauran los Power-Ups, pero solo puedes usar dos en todo el juego", 30, yPos, true);
            yPos += 40;
            
            // Power-ups ayuda
            CreateInstructionWithIcon(Resources.rayoIcon, "Los power-ups te ayudan a responder las preguntas", 30, yPos, true);
            yPos += 40;
            
            // Penalizaciones
            CreateInstructionWithIcon(Resources.alertIcon, "Las penalizaciones dificultan el juego en niveles avanzados o cuando te equivocas PRECAUCIÓN: Son acumulables", 30, yPos, true);
            yPos += 40;
            
            // Acumular puntos
            CreateInstructionWithIcon(Resources.premioIcon, "Acumula puntos y mantén tu racha de respuestas correctas", 30, yPos, true);
            yPos += 40;
            
            // Consejo
            CreateInstructionWithIcon(Resources.locationIcon, "¡Usa tus power-ups estratégicamente!", 30, yPos, true);
        }

        private void ShowPowerUpsPage()
        {
            var title = CreateTitleWithIcon(Resources.rayoIcon, "Power-Ups Disponibles", 20);
            contentPanel.Controls.Add(title.icon);
            contentPanel.Controls.Add(title.label);

            var subtitle = new Label
            {
                Text = "Usa estos poderes para facilitar las preguntas:",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(30, 70),
                AutoSize = true
            };
            contentPanel.Controls.Add(subtitle);

            int yPos = 110;
            var powerUpTypes = new[] { 
                PowerUpType.EliminateOption, 
                PowerUpType.ExtraTime, 
                PowerUpType.Retry, 
                PowerUpType.DoublePoints 
            };

            foreach (var type in powerUpTypes)
            {
                var powerUp = new PowerUp(type);
                CreateItemRow(powerUp.Image, powerUp.Name, powerUp.Description, yPos);
                yPos += 90;
            }
        }

        private void ShowPenaltiesPage()
        {
            var title = CreateTitleWithIcon(Resources.alertIcon, "Penalizaciones", 20);
            contentPanel.Controls.Add(title.icon);
            contentPanel.Controls.Add(title.label);

            var subtitle = new Label
            {
                Text = "Estas penalizaciones aparecen aleatoriamente o cuando te equivocas de respuesta como penalización:",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(255, 193, 7),
                Location = new Point(30, 70),
                AutoSize = true,
                MaximumSize = new Size(740, 0)
            };
            contentPanel.Controls.Add(subtitle);

            int yPos = 120;
            var penaltyTypes = new[] { 
                PenaltyType.ReducedTime, 
                PenaltyType.ShuffleOptions, 
                PenaltyType.GhostAnswer, 
                PenaltyType.BlockPowerUp 
            };

            foreach (var type in penaltyTypes)
            {
                if (type != PenaltyType.Nothing)
                {
                    var penalty = new Penalty(type);
                    CreateItemRow(penalty.Image, penalty.Name, penalty.Description, yPos);
                    yPos += 90;
                }
            }
        }

        private (PictureBox icon, Label label) CreateTitleWithIcon(Bitmap iconImage, string text, int yPos)
        {
            var icon = new PictureBox
            {
                Image = iconImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(30, yPos),
                BackColor = Color.Transparent
            };

            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 193, 7),
                Location = new Point(80, yPos + 5),
                AutoSize = true
            };

            return (icon, label);
        }

        private void CreateInstructionWithIcon(Bitmap iconImage, string text, int xPos, int yPos, bool bold = false, Color? textColor = null)
        {
            var icon = new PictureBox
            {
                Image = iconImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(30, 30),
                Location = new Point(xPos, yPos),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(icon);

            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = textColor ?? Color.White,
                Location = new Point(xPos + 40, yPos + 3),
                AutoSize = true,
                MaximumSize = new Size(700, 0)
            };
            contentPanel.Controls.Add(label);
        }

        private void CreateInstructionText(string text, int xPos, int yPos)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new Point(xPos, yPos),
                AutoSize = true,
                MaximumSize = new Size(710, 0)
            };
            contentPanel.Controls.Add(label);
        }

        private void CreateItemRow(Bitmap image, string name, string description, int yPos)
        {
            // Imagen
            var pictureBox = new PictureBox
            {
                Image = image,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(50, 50),
                Location = new Point(30, yPos),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(pictureBox);

            // Nombre
            var nameLabel = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(100, yPos + 5),
                AutoSize = true
            };
            contentPanel.Controls.Add(nameLabel);

            // Descripción
            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(100, yPos + 30),
                AutoSize = true,
                MaximumSize = new Size(650, 0)
            };
            contentPanel.Controls.Add(descLabel);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < TOTAL_PAGES - 1)
            {
                ShowPage(currentPage + 1);
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                ShowPage(currentPage - 1);
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
