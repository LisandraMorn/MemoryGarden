using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class TutorialForm : Form
    {
        private int tutorialType;

        public TutorialForm(int type)
        {
            tutorialType = type;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Обучение";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(255, 248, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitle = new Label
            {
                Font = new Font("Georgia", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 105, 180),
                AutoSize = true,
                Location = new Point(30, 30)
            };

            Label lblContent = new Label
            {
                Font = new Font("Georgia", 13),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = false,
                Size = new Size(600, 350),
                Location = new Point(30, 80),
                TextAlign = ContentAlignment.TopLeft
            };

            if (tutorialType == 1)
            {
                lblTitle.Text = "🌸 Что такое Японский кроссворд?";
                lblContent.Text = @"Японский кроссворд — это головоломка, где нужно восстановить картинку по числам.

📐 Числа слева и сверху показывают, сколько клеток подряд нужно закрасить.

🎨 В цветных кроссвордах каждая цифра имеет свой цвет.

✅ Между группами одного цвета должна быть минимум одна пустая клетка.

❓ Твоя задача — определить, где начинается и заканчивается каждая группа.

💡 Логика и внимание — твои главные инструменты!";
            }
            else
            {
                lblTitle.Text = "🎮 Управление";
                lblContent.Text = @"🖱️ ЛКМ — закрасить клетку выбранным цветом

❌ ПКМ — поставить крестик

🎨 Палитра цветов находится слева от поля

✔️ Кнопка 'Проверка' покажет ошибки

💡 Не спеши! Думай логически, не угадывай.

🏆 Когда все клетки будут правильными — уровень пройден!

🌟 Удачи в разгадывании!";
            }

            Button btnOk = new Button
            {
                Text = "Понятно 🌸",
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(180, 45),
                Location = new Point(260, 420),
                BackColor = Color.FromArgb(255, 182, 193),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 2;
            btnOk.FlatAppearance.BorderColor = Color.FromArgb(255, 105, 180);
            btnOk.Cursor = Cursors.Hand;
            btnOk.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblContent);
            this.Controls.Add(btnOk);
        }
    }
}