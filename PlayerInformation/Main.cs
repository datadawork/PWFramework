using System;
using System.Text;
using System.Windows.Forms;

using PWFrameWork;

namespace PlayerInformation
{
    public partial class Main : Form
    {
        private static class HostPlayerOffsets
        {
            public const Int32  Struct          = 0x020,
                                Name            = 0x608,
                                Level           = 0x464,
                                Class           = 0x610,
                                Gender          = 0x614,
                                CurrentExp      = 0x474,
                                Hp              = 0x46C,
                                Mp              = 0x470,
                                MaxHp           = 0x4A4,
                                MaxMp           = 0x4A8,
                                LocX            = 0x03C,
                                LocZ            = 0x040,
                                LocY            = 0x044;
        }

        private const Int32 BaseAddress         = 0x9C0E6C,
                            GameRun             = 0x9C1514,
                            LevelRanks          = 0x9C2078;

        private ClientFinder ClientFinder { get; set; }

        public Main()
        {
            InitializeComponent();

            ClientFinder = new ClientFinder(BaseAddress, GameRun, HostPlayerOffsets.Name);
        }

        private void CClientsDropDown(object sender, EventArgs e)
        {
            cClients.Items.Clear();
            cClients.Items.AddRange(ClientFinder.GetWindows());
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

        private static string GetGenderById(int id)
        {
            switch (id)
            {
                case 0: return "Male";
                case 1: return "Female";
                default: return string.Empty;
            }
        }

        private static string GetProcent(int exp, int maxexp)
        {
            return String.Format("{0}%", Math.Round(((double)exp / maxexp) * 100, 1));
        }

        private void BGetClick(object sender, EventArgs e)
        {
            if (cClients.SelectedIndex != -1)
            {
                var window = cClients.SelectedItem as ClientWindow;

                // Получаем дескриптор процесса, выбранного клиента PW и открываем память для чтения / записи
                if (window != null) MemoryManager.OpenProcess(window.ProcessId);

                var resultBuilder = new StringBuilder();

                // Получаем адрес начала структуры данных о нашем персонаже
                var hostPlayerStructAddress = MemoryManager.ChainReadInt32(GameRun, HostPlayerOffsets.Struct);

                // Читаем ник персонажа и записываем их в строку
                var playerName = MemoryManager.ChainReadString(hostPlayerStructAddress + HostPlayerOffsets.Name, 32, 0x0);
                resultBuilder.AppendFormat("Name: {0}\r\n", playerName);

                // Читаем уровень персонажа и записываем их в строку
                var playerLevel = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.Level);
                resultBuilder.AppendFormat("Level: {0}\r\n", playerLevel);

                // Читаем ID класса персонажа, затем получаем название по ID и записываем их в строку
                var playerClassId = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.Class);
                resultBuilder.AppendFormat("Class: {0}\r\n", GetClassById(playerClassId));

                // Читаем пол персонажа, затем получаем название и записываем их в строку
                var playerGenderId = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.Gender);
                resultBuilder.AppendFormat("Gender: {0}\r\n", GetGenderById(playerGenderId));

                // Читаем текущий опыт, опыт требуемый до апа и записываем их в строку
                int playerExp   = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.CurrentExp),
                    playerTotal = MemoryManager.ReadInt32(LevelRanks + (playerLevel * 4));
                resultBuilder.AppendFormat("Exp: {0} / {1} ({2})\r\n", playerExp, playerTotal, GetProcent(playerExp, playerTotal));

                // Читаем значения HP и MaxHP и записываем их в строку
                int playerHp    = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.Hp),
                    playerMaxHp = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.MaxHp);
                resultBuilder.AppendFormat("HP: {0} / {1}\r\n", playerHp, playerMaxHp);

                // Читаем значения MP и MaxMP и записываем их в строку
                int playerMp    = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.Mp),
                    playerMaxMp = MemoryManager.ReadInt32(hostPlayerStructAddress + HostPlayerOffsets.MaxMp);
                resultBuilder.AppendFormat("MP: {0} / {1}\r\n", playerMp, playerMaxMp);

                float locX = MemoryManager.ReadFloat(hostPlayerStructAddress + HostPlayerOffsets.LocX),
                      locZ = MemoryManager.ReadFloat(hostPlayerStructAddress + HostPlayerOffsets.LocZ),
                      locY = MemoryManager.ReadFloat(hostPlayerStructAddress + HostPlayerOffsets.LocY);

                resultBuilder.AppendFormat("Location: {0}, {1} ↑{2}",
                    (int)(locX + 4000) / 10,
                    (int)(locY + 5500) / 10,
                    (int)(locZ) / 10);

                // Выводим текст
                resultBox.Text = resultBuilder.ToString();

                // Закрываем дескриптор процесса
                MemoryManager.CloseProcess();
            }
        }

        private void bShowPlayersList_Click(object sender, EventArgs e)
        {
            var window = cClients.SelectedItem as ClientWindow;
            if (window != null)
                new frmPlayersList(window).ShowDialog();
        }
    }
}
