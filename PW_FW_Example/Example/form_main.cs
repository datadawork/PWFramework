using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PWFrameWork;
namespace GreecePriest
{
    public partial class form_main : Form
    {
        //объявляем класс для поиска окон
        private ClientFinder cf;


        //Точка входа
        public form_main()
        {

            //Инициализируем компоненты
            InitializeComponent();
            //Определяем поисковик окон - создаем новый экземпляр класса поисковика
            cf = new ClientFinder(PWOffssAndAddrss.base_address, PWOffssAndAddrss.game_struct_offset, PWOffssAndAddrss.host_player_struct_offset, PWOffssAndAddrss.host_player_name_offset);
        }

        //объявляем экземлпряр класса ClientWindow для хранения выбранного окна
        private ClientWindow SelectedWindow;
        //Менять выбранное окно будетм при смене индекса комбо
        private void clients_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ClientWindow win in ClientWindows)
            {
                if (win.Name == combo_clients.SelectedItem.ToString())
                {
                    SelectedWindow = win;
                }
            }
        }

        //объявляем хранилище для клиентских окон - здесь будет храниться актуальный список окон
        private List<ClientWindow> ClientWindows = new List<ClientWindow>();
        //Определяем процедуру для обновления комбо
        private void RefreshClientsCombo()
        {
            //очищаем хранилище ококн
            ClientWindows.Clear();
            //для каждого найденого окна
            foreach (ClientWindow win in cf.ClientWindows)
            {
                //добавляем запись в хранилище
                ClientWindows.Add(win);
            }

            //Очищаем список список окон в комбо
            combo_clients.Items.Clear();
            //добавляем список окон в комбо
            combo_clients.Items.AddRange(ClientWindows.ToArray());
            //если окон нет - очищаем комбо
            if (ClientWindows.Count == 0)
                combo_clients.Text = "";
        }

        //определяем поток для работы программы
        private Thread bot_thread;

        //метод для вывода данных на форму Можно переделать по аналогии для любого контрола. Для вывода данных на форму. 
        public void SetTextBoxText( string text)
        {

            try
            {

                if (textBox1.InvokeRequired)
                    textBox1.BeginInvoke((Action<string>)SetTextBoxText, text);
                else
                    textBox1.Text = text;
            }
            catch
            { }
        }


        //делегат для SetControlValue
        public delegate void ControlDelegate(Control ctrl, int value);
        /// <summary>
        /// Метод для назначения свойсва Value объекту Control. Передает данные из второстепенного потока в главный поток. 
        /// </summary>
        /// <param name="ctrl">Control: Контрол, которому нужно изменить свойство Value</param>
        /// <param name="value">int: Значение свойства Value</param>
        public void SetControlValue(Control ctrl, int value)
        {
            try
            {

                Object[] param_array = new Object[2];
                param_array[0] = ctrl;
                param_array[1] = value;

                if (ctrl.InvokeRequired)
                {
                    ctrl.BeginInvoke(new ControlDelegate(SetControlValue), param_array);
                }
                else
                {
                    PropertyInfo value_property = ctrl.GetType().GetProperty("Value");
                    if (value_property != null)
                    {
                        value_property.SetValue(ctrl, value, null);
                    }
                    else
                        throw new Exception("Нет свойства Value");
                }
            }
            catch
            { }
        }

        //определяем процедуру, которую будем выполнять в отдельном потоке (чтобы не подвешивать основную форму)
        private void bot_work()
        {

            int i = 0;
            //получаем класс игры для запущенного окна
            PWGameWindow game_window = new PWGameWindow(SelectedWindow);
            //объявляем host-игрока для данного окна игры
            PWHostPlayer MyPersonage = game_window.HostPlayer;


            //задаем бесконечный цикл в котором будем проверять какие то данные и на на их основании что то делать
            while (true)
            {
                i += 1;
                SetTextBoxText(MyPersonage.TargetWID.ToString());
                SetControlValue(progressBar1, (int)MyPersonage.HpPersent);
                SetControlValue(progressBar2, (int)MyPersonage.MpPersent);

                if (MyPersonage.TargetWID != 0)
                {
                    //Смерчь у приста
                    MyPersonage.UseSkill(127);
                    Thread.Sleep(500);
                    //Оперенная стрела
                    MyPersonage.UseSkill(125);

                }
                else
                {
                    if (checkBox1.Checked)
                    {
                        //Сесть в медитацию
                        MyPersonage.MeditationStart();
                    }
                    else
                    {
                        MyPersonage.MeditationStop();
                    }
                }

                Thread.Sleep(1000);
            }
        }

        //загрузка формы
        private void form_main_Load(object sender, EventArgs e)
        {
            //Обновляем список окон
            RefreshClientsCombo();
            //Если список окон больше 0
            if (combo_clients.Items.Count > 0)
            {
                //выбираем первого в списке
                combo_clients.SelectedIndex = 0;
            }

            lbl_messages.Text = "Выберите в выпадающем списке персонажа и нажмите Подключить";

        }


        //при клике по комбо - обновляем список окон
        private void clients_Click(object sender, EventArgs e)
        {
            RefreshClientsCombo();
        }

        //Кнопка Подключить/Отключить
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Если не выбрано окно - выходим
            if (SelectedWindow == null) return;
            //Если окно выбрано, но уже успело пропасть в списке окон (клиент закрылся)
            if (!cf.IsProcessExist(SelectedWindow.ProcessId))
            {
                combo_clients.Text = "";
                return;
            }

            //подключаемся или отключаемся
            if (btnConnect.Text == "Подключить")
            {
                lbl_messages.Text = "Подключено к " + combo_clients.Text;
                btnConnect.Text = "Отключить";
                combo_clients.Enabled = false;
                bot_thread = new Thread(bot_work);
                bot_thread.Start();

            }
            else if (btnConnect.Text == "Отключить")
            {
                btnConnect.Text = "Подключить";
                combo_clients.Enabled = true;
                RefreshClientsCombo();
                bot_thread.Abort();
                lbl_messages.Text = "Выберите в выпадающем списке приста";
            }
        }


        //При закрытии формы закрываем запущенный поток
        private void form_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bot_thread != null)
            {

                if (bot_thread.IsAlive)
                {
                    bot_thread.Abort();
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Считывает данные из процесса в буфер
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        private byte[] ProcessToByte(Process proc)
        {
            //начало поиска опкода
            Int32 StartAddress = 0x00401000;
            //длинна страницы
            Int32 SheetLenght = 0x01000000;//Абстрактная большая величина                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               

            int read = 0;
            byte[] buffer = new byte[SheetLenght];
            //считываем в буфер процесс
            WinApi.ReadProcessMemory(proc.Handle, StartAddress, buffer, SheetLenght, out read);
            return buffer;
        }


        //находим и отображаем смещения
        private void button1_Click(object sender, EventArgs e)
        {
            //Если не выбрано окно - выходим
            if (SelectedWindow == null) return;
            //Если окно выбрано, но уже успело пропасть в списке окон (клиент закрылся)
            if (!cf.IsProcessExist(SelectedWindow.ProcessId))
            {
                combo_clients.Text = "";
                return;
            }
            //получаем опкод в котором будем искать смещения
            byte[] opcode = ProcessToByte(SelectedWindow.Process);

            //объявляем класс для поиска смещений
            OffsetRetriever ret = new OffsetRetriever();
            //добавляем шаблоны для PWI и RUOFF
            ret.AddOffsetPattern("BA_PWI", "A1?578B482081C1EC");
            ret.AddOffsetPattern("BA_RUOFF", "A1?8B48205781C1EC");
            ret.AddOffsetPatterns(@"UNFREEZE: 0F95C084C08885?, GA: 8B0D?898D40F1FFFF6A01, CHAR_LVL: 8986?8A4F03, CHAR_CULT: 898E?8B5714, CHAR_EXP: 8996?8B4718, CHAR_SPIRIT: 8986?8B4F04,
                              CHAR_HP: 898E?8B570C, CHAR_MP: 8996?8B471C, CHAR_CHI:8986?8B4F08, CHAR_MaxHP: 898E?8B5710, CHAR_MaxMP: 8996?8B4720,
                              CHAR_MaxCHI: 8986?8A4702, CHAR_GOLD: 8B108996?8B4004, CHAR_STATE: 6A0A8986?, CHAR_CastID: 8986?8B40048BC8,
                              BA: A1?578B482081C1EC, CHAR_TARGET: 568BF18B86?85C0, CHAR_PET: 8B8E?3BCB740655, CHAR_STR: 8B8E?8B47503BC8,
                              CHAR_DEX: 8B96?8B47543BD0, CHAR_CON: 8B86?8B4F5C3BC1, CHAR_INT: 8B8E?8B47603BC8, CHAR_REP: 8B96?8B47643BD0,
                              CHAR_CLASS: EB5D8B8E?B801, CHAR_JUMP: 8B491C33C08B91?85D2, CHAR_ID: 32C0C38B89?568BB0, CHAR_NAME: 8B83?8D4C243C");
            
            //находим и загружаем значения в словарь
            Dictionary<string, int> finded_offsets = ret.FindOffsets(opcode);

            //добавляем маски адресов гуи функции для RUOFF и PWI
            ret.AddAddressPattern("GUI_RuOff", "55568B74240C8BC68BE98D50018D49008A0883C00184C9");
            ret.AddAddressPattern("GUI_PWI", "53568B74240C8BD9578BFE83C9FF33C0F2AE");
            //находим все адреса и загружаем в словарь
            Dictionary<string, int> finded_addresses = ret.FindAddress(opcode);

            //очищаем лист
            listView1.Items.Clear();
            listView1.View = View.Details;

            //заполняем найденными данными (что смогли найти)
            foreach (var offset in finded_offsets)
            {
                //создаем массив из подстрок
                string[] items = {offset.Key, offset.Value.ToString("X")};
                //заполняем им listview
                listView1.Items.Add(new ListViewItem(items));
            }
            //все аналогично
            foreach (var address in finded_addresses)
            {
                string[] items = { address.Key, address.Value.ToString("X") };
                listView1.Items.Add(new ListViewItem(items));
            }
            
            //выравниваем стобцы по содержимому
            offsetName.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            offsetValue.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

        }

        




    }
}
