using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class HowToPlayForm : Form
    {
        public HowToPlayForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Обучение";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                ForeColor = Color.White
            };

            TabPage page1 = new TabPage("🐱 Котик");
            page1.BackColor = Color.Black;
            Label lblCat = CreateStyledLabel(
@"🖱️ Управление котиком:

A / ← — идти влево
D / → — идти вправо
W / ↑ — прыжок

Котик умеет бегать, прыгать и взбираться на платформы.
Используй это, чтобы добраться до воспоминаний и двери.

На пути тебе встретится Алиса — просто подойди к ней."
            );
            page1.Controls.Add(lblCat);

            TabPage page2 = new TabPage("🎨 Нонограммы");
            page2.BackColor = Color.Black;
            Label lblPuzzle = CreateStyledLabel(
@"Цветные нонограммы — головоломки, в которых нужно закрашивать клетки согласно подсказкам.

🔢 Числа слева и сверху показывают, сколько клеток подряд одного цвета нужно закрасить.
Цвет числа соответствует цвету закрашиваемых клеток.

🎨 Выбери цвет на палитре и закрашивай клетки левой кнопкой мыши.
❌ Правой кнопкой ставь крестик на пустых клетках.

✔️ Кнопка «Проверка» скажет, всё ли верно.

Между группами одного цвета должна быть хотя бы одна пустая клетка."
            );
            page2.Controls.Add(lblPuzzle);

            tabs.TabPages.Add(page1);
            tabs.TabPages.Add(page2);

            this.Controls.Add(tabs);

            
            Button btnOk = new Button
            {
                Text = "Понятно 🌸",
                Font = new Font("Georgia", 14, FontStyle.Bold),
                Size = new Size(180, 45),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 2;
            btnOk.FlatAppearance.BorderColor = Color.White;
            btnOk.Cursor = Cursors.Hand;
            btnOk.Click += (s, e) => this.Close();
            btnOk.Location = new Point((this.ClientSize.Width - 180) / 2, this.ClientSize.Height - 80);
            this.Controls.Add(btnOk);
        }

        private Label CreateStyledLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Georgia", 12),
                ForeColor = Color.White,
                BackColor = Color.Black,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                TextAlign = ContentAlignment.TopLeft
            };
        }
    }
}