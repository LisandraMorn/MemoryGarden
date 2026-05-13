using Memory.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class Level1 : PlatformerLevel
    {
        public Level1(Profile prof, int profIdx, Panel panel, Form main, int lvlId = 0)
            : base(prof, profIdx, panel, main, lvlId) { }

        protected override void BuildWorld()
        {
            this.Text = "Уровень 1 - Ребёнок";
            worldWidth = 8000;
            int clientHeight = this.ClientSize.Height;
            int floorY = clientHeight - 50;

            // ПОЛ
            platforms.Add(new RectangleF(0, floorY, 8000, 50));
            

            // Первая Алиса
            aliceTriggers.Add(new RectangleF(600, floorY - 70, 40, 70));
            aliceVisible.Add(true);
            aliceDialogueDone.Add(false);
            aliceDialogues.Add(new string[] { "Я помню, что меня зовут Алиса..", "Где я?" });

            // Вторая Алиса 
            aliceTriggers.Add(new RectangleF(7500, floorY - 70, 40, 70));
            aliceVisible.Add(true);
            aliceDialogueDone.Add(false);
            aliceDialogues.Add(new string[] { "Я умерла?..", "Мои воспоминания.." });


            AddDecoration("lantern.png", 750, floorY - 512, 100, 512);
            platforms.Add(new RectangleF(1000, 600, 100, 20));
            platforms.Add(new RectangleF(1200, 500, 100, 20));
            AddWalkThroughObject(bookshelfImg, new RectangleF(1400, floorY - 200, 100, 200)); // Воспоминание 1
            platforms.Add(new RectangleF(1600, 500, 100, 20));
            platforms.Add(new RectangleF(1800, 600, 100, 20));
            AddDecoration("large_cabinet.png", 2000, floorY - 800, 800, 800);
            AddDecoration("lantern.png", 3000, floorY - 512, 100, 512);
            AddWalkThroughObject(booksStackImg, new RectangleF(3200, floorY - 62, 125, 62));
            AddWalkThroughObject(hourglassImg, new RectangleF(3400, floorY - 128, 64, 128));
            AddWalkThroughObject(booksStackImg, new RectangleF(3600, floorY - 212, 125, 62));
            
            AddWalkThroughObject(booksStackImg, new RectangleF(3900, floorY - 300, 125, 62));
            
            AddWalkThroughObject(dresserImg, new RectangleF(4200, floorY - 336, 192, 336));
            AddWalkThroughObject(booksStackImg, new RectangleF(4550, floorY - 400, 125, 62));

            AddDecoration("Zhnets.png", 4500, floorY - 200, 400, 200);

            AddWalkThroughObject(booksStackImg, new RectangleF(4800, floorY - 400, 125, 62));
            AddDecoration("lantern.png", 4950, floorY - 307, 60, 307);
            AddWalkThroughObject(booksStackImg, new RectangleF(5050, floorY - 400, 125, 62));
            AddWalkThroughObject(hourglassImg, new RectangleF(5220, floorY - 128, 64, 128)); // Воспоминание 3

            AddWalkThroughObject(booksStackImg, new RectangleF(5300, floorY - 400, 125, 62));
            AddDecoration("lantern.png", 5450, floorY - 410, 80, 410);
            AddWalkThroughObject(booksStackImg, new RectangleF(5550, floorY - 500, 125, 62));
            AddWalkThroughObject(booksStackImg, new RectangleF(5800, floorY - 400, 125, 62)); // Воспоминание 2
            AddDecoration("large_cabinet.png", 6000, floorY - 800, 800, 800);
            AddDecoration("lantern.png", 6900, floorY - 512, 100, 512);

            // Воспоминания
            memoryItems[0] = new RectangleF(1450, floorY - 200 - 25, 25, 25); // над шкафом
            memoryItems[1] = new RectangleF(5850, floorY - 26 - 400, 25, 25); // на книжной стопке
            memoryItems[2] = new RectangleF(5220, floorY - 26 - 128, 25, 25); // на песочных часах
            for (int i = 0; i < 3; i++) memoryActive[i] = true;

            // Диалоговые триггеры
            dialogueTriggers.Add(new RectangleF(1400, floorY - 280, 100, 80));
            dialogueTexts.Add(new string[] { "Этот цветок мне подарила мама на день рождение.", "Мне было 8?" });
            dialogueDone.Add(false);

            dialogueTriggers.Add(new RectangleF(5800, floorY - 80 - 400, 100, 80));
            dialogueTexts.Add(new string[] { "Я обожала какао с молоком!", "Могла выпить 5 кружек за раз." });
            dialogueDone.Add(false);

            dialogueTriggers.Add(new RectangleF(5220, floorY - 128 - 80, 100, 80));
            dialogueTexts.Add(new string[] { "Всегда мечтала построить свой дом.", "Это мой детский рисунок.", "Мама с ним не расставалась до самой смерти.." });
            dialogueDone.Add(false);

            // Дверь
            doorTrigger = new RectangleF(7700, floorY - 200, 100, 200);
        }
    }
}