using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PWFrameWork;

namespace PlayerInformation
{
    public partial class frmPlayersList : Form
    {
        private ClientWindow selectedWindow;

        private const Int32 BaseAddress         = 0x9C0E6C,
                            GameRun             = 0x9C1514,
                            HostPlayerStruct    = 0x20;

        public frmPlayersList(ClientWindow window)
        {
            InitializeComponent();

            selectedWindow = window;
        }

        private void frmPlayersList_Load(object sender, EventArgs e)
        {
            RetrievePlayerList();
        }

        private static string GetClassById(int id)
        {
            switch (id)
            {
                case 0: return "Воин";
                case 1: return "Маг";
                case 2: return "Шаман";
                case 3: return "Друид";
                case 4: return "Оборотень";
                case 5: return "Убийца";
                case 6: return "Лучник";
                case 7: return "Жрец";
                default: return string.Empty;
            }
        }

        private void RetrievePlayerList()
        {
            // Доступ к элементам списка осуществляется так:
            // GA +20 +380 +088 +I*4 (I In [0..N])
            // где N = кол-во игроков рядов

            // Открываем память процесса для чтения / записи
            MemoryManager.OpenProcess(selectedWindow.ProcessId);

            var resultBuilder = new StringBuilder();

            // Получаем кол-во людей, которое рядом с нами
            // GA +20 +380 +14
            var nearPlayersCount = MemoryManager.ChainReadInt32(GameRun, HostPlayerStruct, 0x380, 0x14);
            // Записываем результат
            resultBuilder.AppendLine(String.Format("Players count: {0}", nearPlayersCount));

            // Получаем указатель, на начало списка
            // GA +20 + 380 +88
            var pointer = MemoryManager.ChainReadInt32(GameRun, HostPlayerStruct, 0x380, 0x88);

            // Начинаем пробегать по списку игроков
            for (var i = 0; i < nearPlayersCount; i++)
            {
                // Получаем указатель, на начало структуры из списка игроков
                var playerBase = MemoryManager.ReadInt32(pointer + i * 0x4);
                // Проверяем существует ли запись
                if (playerBase != 0)
                {
                    // Получаем данные из структуры, которые нам нужны
                    var playerId        = MemoryManager.ReadInt32(playerBase + 0x458);
                    var playerName      = MemoryManager.ChainReadString(playerBase + 0x608, 32, 0x0);
                    var playerLevel     = MemoryManager.ReadInt32(playerBase + 0x464);
                    var playerClassId   = MemoryManager.ReadInt32(playerBase + 0x610);
                    var playerHp        = MemoryManager.ReadInt32(playerBase + 0x46C);
                    var playerMp        = MemoryManager.ReadInt32(playerBase + 0x470);
                    var playerMaxHp     = MemoryManager.ReadInt32(playerBase + 0x4A4);
                    var playerMaxMp     = MemoryManager.ReadInt32(playerBase + 0x4A8);
                    var playerClanId    = MemoryManager.ReadInt32(playerBase + 0x5D4);
                    var playerGender    = MemoryManager.ReadInt32(playerBase + 0x614);
                    var playerWalkMode  = MemoryManager.ReadInt32(playerBase + 0x61C);
                    var playerX         = MemoryManager.ReadFloat(playerBase + 0x3C);
                    var playerZ         = MemoryManager.ReadFloat(playerBase + 0x40);
                    var playerY         = MemoryManager.ReadFloat(playerBase + 0x44);

                    // Записываем полученные данные
                    resultBuilder.AppendLine(String.Format("Player: {0}", playerName));
                    // Выводим координаты, преобразуя их в "игровые" -те, что видят игроки
                    resultBuilder.AppendLine(String.Format("  Location: {0}, {1} ↑{2}",
                                            (int)(playerX + 4000) / 10,
                                            (int)(playerY + 5500) / 10,
                                            (int)(playerZ) / 10));
                    resultBuilder.AppendLine(String.Format("  ID: {0:X8}", playerId));
                    resultBuilder.AppendLine(String.Format("  Class: {0}", GetClassById(playerClassId)));
                    resultBuilder.AppendLine(String.Format("  Level: {0}", playerLevel));
                    resultBuilder.AppendLine(String.Format("  HP: {0} / {1}", playerHp, playerMaxHp));
                    resultBuilder.AppendLine(String.Format("  MP: {0} / {1}", playerMp, playerMaxMp));
                    resultBuilder.AppendLine();
                }
            }

            // Выводим результаты в текстовое поле
            tResult.Text = resultBuilder.ToString();

            // Закрываем дескриптор процесса
            MemoryManager.CloseProcess();
        }

        private void bRefresh_Click(object sender, EventArgs e)
        {
            RetrievePlayerList();
        }
    }
}
