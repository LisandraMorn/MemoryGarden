using Memory.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Memory
{
    public class Level2 : PlatformerLevel
    {
        public Level2(Profile prof, int profIdx, Panel panel, Form main, int lvlId = 1)
            : base(prof, profIdx, panel, main, lvlId) { }

        protected override void BuildWorld()
        {
            this.Text = "Уровень 2 - Ребёнок";
            worldWidth = 8000;
            int clientHeight = this.ClientSize.Height;
            int floorY = clientHeight - 50;

            // ПОЛ
            platforms.Add(new RectangleF(0, floorY, 8000, 50));

            // Декорации
            AddDecoration("level2_1.png", 0, floorY - 750, 1000, 750);
            AddDecoration("level2_2.png", 1000, floorY - 750, 1000, 750);

            // Платформы
            platforms.Add(new RectangleF(600, 550, 100, 20));
            platforms.Add(new RectangleF(900, 480, 100, 20));
            platforms.Add(new RectangleF(1200, 420, 100, 20));
            platforms.Add(new RectangleF(2000, 500, 100, 20));
            platforms.Add(new RectangleF(2300, 440, 100, 20));
            platforms.Add(new RectangleF(2600, 550, 100, 20));

            // Проходимые объекты
            AddWalkThroughObject(bookshelfImg, new RectangleF(3000, floorY - 200, 100, 200));
            AddWalkThroughObject(booksStackImg, new RectangleF(3400, floorY - 50, 80, 40));
            AddWalkThroughObject(dresserImg, new RectangleF(3800, floorY - 250, 150, 250));
            AddWalkThroughObject(hourglassImg, new RectangleF(4200, floorY - 150, 80, 150));

            // Алиса 1
            aliceTriggers.Add(new RectangleF(700, floorY - 70, 40, 70));
            aliceVisible.Add(true);
            aliceDialogueDone.Add(false);
            aliceDialogues.Add(new string[] {
                "*Этот уровень в разработке*",
                "..."
            });

            // Алиса 2
            aliceTriggers.Add(new RectangleF(5000, floorY - 70, 40, 70));
            aliceVisible.Add(true);
            aliceDialogueDone.Add(false);
            aliceDialogues.Add(new string[] {
                "...",
                "..."
            });

            // Воспоминания
            memoryItems[0] = new RectangleF(3050, floorY - 200 - 25, 25, 25);
            memoryItems[1] = new RectangleF(3850, floorY - 250 - 25, 25, 25);
            memoryItems[2] = new RectangleF(5000, floorY - 25, 25, 25);
            for (int i = 0; i < 3; i++) memoryActive[i] = true;

            
            dialogueTriggers.Add(new RectangleF(3050, floorY - 230, 100, 80));
            dialogueTexts.Add(new string[] {
                "...",
                "..."
            });
            dialogueDone.Add(false);

            dialogueTriggers.Add(new RectangleF(3850, floorY - 280, 100, 80));
            dialogueTexts.Add(new string[] {
                "...",
                "..."
            });
            dialogueDone.Add(false);

            dialogueTriggers.Add(new RectangleF(5000, floorY - 80, 100, 80));
            dialogueTexts.Add(new string[] {
                "...",
                "..."
            });
            dialogueDone.Add(false);

            // Дверь
            doorTrigger = new RectangleF(7700, floorY - 200, 100, 200);
        }
    }
}