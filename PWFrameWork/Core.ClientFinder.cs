using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PWFrameWork
{
    public class ClientWindow
    {
        public String Name { get; set; }
        public IntPtr Handle { get; set; }
        public Int32 ProcessId { get; set; }

        public ClientWindow(string name, IntPtr handle, int id)
        {
            Name = name; Handle = handle; ProcessId = id;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ClientFinder
    {
        private Int32 BaseAddress { get; set; }
        private Int32 GameRun { get; set; }
        private Int32 HostPlayerStruct { get; set; }
        private Int32 HostPlayerName { get; set; }

        /// <summary>
        /// Инициализирует новый объект ClientFinder класса.
        /// </summary>
        /// <param name="baseAddress">Базовый адрес клиента PW</param>
        /// <param name="gameRun">Адрес структуры GameRun</param>
        /// <param name="hostPlayerNameOffset">Оффсет имени персонажа</param>
        public ClientFinder(Int32 baseAddress, Int32 gameRun, Int32 hostPlayerNameOffset)
        {
            HostPlayerStruct = 0x20;
            BaseAddress = baseAddress; GameRun = gameRun; HostPlayerName = hostPlayerNameOffset;
        }

        /// <summary>
        /// Проверяет версию клиента PW, путем сравнения значения GA из памяти клиента и указанным.
        /// </summary>
        /// <returns></returns>
        private bool CheckClientVersion()
        {
            return MemoryManager.ReadInt32(BaseAddress) + 0x1C == GameRun ? true : false;
        }

        /// <summary>
        /// Создает объект ClientWindow для каждого клиента PW, который найден.
        /// </summary>
        /// <returns></returns>
        public ClientWindow[] GetWindows()
        {
            var rtnList = new List<ClientWindow>();

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (WinApi.GetWindowClass(process.MainWindowHandle).Equals("ElementClient Window"))
                    {
                        MemoryManager.OpenProcess(process.Id);
                        if (CheckClientVersion())
                        {
                            var charName = MemoryManager.ChainReadString(GameRun, 32, HostPlayerStruct, HostPlayerName,
                                                                         0x0);

                            rtnList.Add(new ClientWindow(
                                            String.IsNullOrEmpty(charName) ? process.MainWindowTitle : charName,
                                            process.MainWindowHandle, 
                                            process.Id));
                        }
                        MemoryManager.CloseProcess();
                    }
                }
                catch(Exception)
                {
                    continue;
                }
            }

            return rtnList.ToArray();
        }
    }
}
