using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

namespace PWFrameWork
{
    /// <summary>
    /// Класс описывающий окно клиента игры
    /// </summary>
    public class ClientWindow
    {
        /// <summary>
        /// Имя клиента
        /// </summary>
        public String Name { get; set; }
        
        /// <summary>
        /// ID связанного с клиентом процесса
        /// </summary>
        public Int32 ProcessId { get; set; }
        /// <summary>
        /// Связанный с клиентом процесс
        /// </summary>
        public Process Process
        {

            get
            {
                return Process.GetProcessById(ProcessId);
            }
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="name">string: Имя процесса</param>
        /// <param name="process_id">int: ID связанного с клиентом процесса</param>
        public ClientWindow(string name, int process_id)
        {
            Name = name;
            ProcessId = process_id;
        }

        /// <summary>
        /// Переопределенный метод ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }

    public class ClientFinder
    {
        private Int32 BaseAddress { get; set; }
        private Int32 GameStructOffset { get; set; }
        private Int32 HostPlayerStructOffset { get; set; }
        private Int32 HostPlayerNameOffset { get; set; }

        /// <summary>
        /// Конструктор класса ClientFinder
        /// </summary>
        /// <param name="base_address">Базовый адрес клиента</param>
        /// <param name="game_struct_offset">Адрес структуры игры</param>
        /// <param name="host_player_struct_offset">Смещение к структуре персонажа в структуре игры</param>
        /// <param name="host_player_name_offset">Смещение к адресу имени персонажа в структуре персонажа</param>
        public ClientFinder(Int32 base_address, Int32 game_struct_offset, Int32 host_player_struct_offset, Int32 host_player_name_offset)
        {

            BaseAddress = base_address;
            GameStructOffset = game_struct_offset;
            HostPlayerStructOffset = host_player_struct_offset;
            HostPlayerNameOffset = host_player_name_offset;
        }

        /// <summary>
        /// Получает список объектов ClientWindow для каждого найденного клиента
        /// </summary>
        public IEnumerable<ClientWindow> ClientWindows
        {
            get
            {
                //Задаем начало отсчета
                IntPtr hwnd = IntPtr.Zero;
                //В бесконечном цикле перебираем все запущенные окна с классом ElementClient Window
                while (true)
                {
                    //получаем следующее окно с классом ElementClient Window. 
                    hwnd = WinApi.FindWindowEx(IntPtr.Zero, hwnd, "ElementClient Window", null);

                    //Если наткнулись на ноль - значит выходим 
                    if (hwnd == IntPtr.Zero) break;
                    //объявляем переменную для хранения id процесса
                    int process_id;
                    //получаем id процесса по хендлу
                    WinApi.GetWindowThreadProcessId(hwnd, out  process_id);

                    //Открываем память на чтение 
                    MemoryWork memory = new MemoryWork(process_id);

                    //Считываем имя персонажа
                    string personage_name = memory.ChainReadString_Unicode(this.BaseAddress, 32, this.GameStructOffset, this.HostPlayerStructOffset, this.HostPlayerNameOffset, 0);

                    //Если удалось считать имя
                    if (personage_name != "")
                    {
                        //добавляем в список окон окно с именем персонажа
                        yield return new ClientWindow(personage_name, process_id);
                    }
                    else
                    {
                        //если считать не удалось - добавляем в список окон окно с названием окна
                        yield return new ClientWindow(WinApi.GetWindowText(hwnd), process_id);
                    }
                }
            }
        }


        /// <summary>
        /// Проверяет существует ли процесс с заданным id. 
        /// </summary>
        /// <returns></returns>
        public bool IsProcessExist(int process_id)
        {
            //задаем точку отсчета
            IntPtr hwnd = IntPtr.Zero;

            //перебираем в бесконечном цикле все существующие процессы
            while (true)
            {
                //получаем следующий hwnd с заданным классом окна
                hwnd = WinApi.FindWindowEx(IntPtr.Zero, hwnd, "ElementClient Window", null);
                //если перебрали все процессы (следующий хэндл = 0) - выходим
                if (hwnd == IntPtr.Zero) break;

                //объявляем переменную для хранения id процесса
                int pid;
                //получаем id процесса по hwnd окна
                WinApi.GetWindowThreadProcessId(hwnd, out  pid);

                //Если процесс с таким id существует
                if (pid == process_id)
                {
                    return true;
                }
            }
            //Если вышли так и не получив процесса:
            return false;
        }

    }
}
