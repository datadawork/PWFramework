using System;
using System.Windows.Forms;

using PWFrameWork;

namespace SendPacketTest
{
    public partial class Main : Form
    {
        private static class HostPlayerOffsets
        {
            public const Int32 Name = 0x608;
        }

        private const Int32 BaseAddress         = 0x9C0E6C,
                            GameRun             = 0x9C1514,
                            PacketSendFunction  = 0x5D7C30;

        private ClientFinder ClientFinder { get; set; }

        // Код инжекта на отправку пакетов
        private readonly byte[] _sendPacketOpcode = new byte[] 
        { 
            0x60,                                   //PUSHAD
            0xB8, 0x00, 0x00, 0x00, 0x00,           //MOV EAX, SendPacketAddress
            0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00,     //MOV ECX, DWORD PTR [realBaseAddress]
            0x8B, 0x49, 0x20,                       //MOV ECX, DWORD PTR [ECX+20]
            0xBF, 0x00, 0x00, 0x00, 0x00,           //MOV EDI, packetAddress
            0x6A, 0x00,                             //PUSH packetSize
            0x57,                                   //PUSH EDI
            0xFF, 0xD0,                             //CALL EAX
            0x61,                                   //POPAD
            0xC3                                    //RET
        };

        public Main()
        {
            InitializeComponent();

            ClientFinder = new ClientFinder(BaseAddress, GameRun, HostPlayerOffsets.Name);
        }

        private int _packetAddressLocation;
        private int _packetSizeAddress;
        private int _sendPacketOpcodeAddress;

        private void LoadSendPacketOpcode(IntPtr processHandle)
        {
            // Выделяем память под код отправки пакета
            _sendPacketOpcodeAddress = InjectHelper.AllocateMemory(processHandle, _sendPacketOpcode.Length);

            // Записываем код отправки пакета
            MemoryManager.WriteBytes(_sendPacketOpcodeAddress, _sendPacketOpcode);

            // Переводим адрес функции отправки пакетов в массив байт
            var functionAddress = BitConverter.GetBytes(PacketSendFunction);
            // Переводим базовый адрес в массив байт
            var realBaseAddress = BitConverter.GetBytes(BaseAddress);

            // Записываем адрес функции отправки пакетов в тело нашего инжекта
            MemoryManager.WriteBytes(_sendPacketOpcodeAddress + 2, functionAddress);
            // Записываем базовый адрес в тело нашего инжекта
            MemoryManager.WriteBytes(_sendPacketOpcodeAddress + 8, realBaseAddress);

            // Указываем адрес, куда будет записан адрес загруженного пакета 
            _packetAddressLocation = _sendPacketOpcodeAddress + 16;
            // Указываем адрес, куда будет записан размер загруженного пакета
            _packetSizeAddress = _sendPacketOpcodeAddress + 21;
        }

        public void SendPacket(IntPtr processHandle, byte[] packetData)
        {
            // Выделяем место под пакет, который мы будем посылать
            var packetAddress = InjectHelper.AllocateMemory(processHandle, packetData.Length);
            // Записываем пакет
            MemoryManager.WriteBytes(packetAddress, packetData);

            // Переводим адрес, куда мы записали пакет в массив байт
            var packetLocation = BitConverter.GetBytes(packetAddress);

            // Если код не загружен ранее, загружаем его в память
            if (_sendPacketOpcodeAddress == 0) LoadSendPacketOpcode(processHandle);

            // Записываем адрес, где лежит пакет в тело нашего инжекта
            MemoryManager.WriteBytes(_packetAddressLocation, packetLocation);
            // Записываем длину пакета
            MemoryManager.WriteBytes(_packetSizeAddress, new[] { (byte)packetData.Length });

            // Запускаем инжект
            var threadHandle = InjectHelper.CreateRemoteThread(processHandle, _sendPacketOpcodeAddress);

            // Ждем окончания выполнения потока
            WinApi.WaitForSingleObject(threadHandle, 0xFFFFFFFF);
            // Уничтожаем поток
            WinApi.CloseHandle(threadHandle);

            // Освобождаем память, выделенную под пакет
            InjectHelper.FreeMemory(processHandle, packetAddress, packetData.Length);
            // Освобождаем память, выделенную под код отправки пакета
            InjectHelper.FreeMemory(processHandle, _sendPacketOpcodeAddress, _sendPacketOpcode.Length);
        }

        private void BSendPacketClick(object sender, EventArgs e)
        {
            var window = cClients.SelectedItem as ClientWindow;

            // Получаем дескриптор процесса, выбранного клиента PW и открываем память для чтения / записи
            if (window != null) MemoryManager.OpenProcess(window.ProcessId);

            // Отправляем пакет на медитацию
            SendPacket(MemoryManager.OpenProcessHandle, new byte[] { 0x2e, 0x00 });

            // Закрываем дескриптор процесса
            MemoryManager.CloseProcess();
        }

        private void CClientsDropDown(object sender, EventArgs e)
        {
            cClients.Items.Clear();
            cClients.Items.AddRange(ClientFinder.GetWindows());
        }
    }
}
