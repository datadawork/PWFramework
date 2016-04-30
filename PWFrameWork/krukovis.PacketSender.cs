using System;
using System.Collections.Generic;
using System.Text;

namespace PWFrameWork
{
    class PacketSender
    {
        private MemoryWork memory;

        /// <summary>
        /// Конструирует класс для работы с пакетами для конкретного класса MemoryWork
        /// </summary>
        /// <param name="memory_work_class"></param>
        public PacketSender(MemoryWork memory_work_class)
        {
            this.memory = memory_work_class;
        }

        /// <summary>
        /// Отправляет пакет
        /// </summary>
        /// <param name="packet">byte[]: Байт-код пакета </param>
        public void Send(byte[] packet)
        {
            //временная переменная
            int tmpInt;
            //Записываем в открытую память пакет в выделенное место
            WinApi.WriteProcessMemory(memory.OpenedProcessHandle, memory.PacketAllocMemory, packet, packet.Length, out tmpInt);

            //выполняем функцию отправки пакета
            ASM asm = new ASM(this.memory);
            asm.Pushad();
            asm.Mov_EAX(PWOffssAndAddrss.packet_function_address);
            asm.Mov_ECX_DWORD_Ptr(PWOffssAndAddrss.base_address);
            asm.Mov_ECX_DWORD_Ptr_ECX_Add(0x20);
            asm.Mov_EDI(memory.PacketAllocMemory);
            asm.Push6A(packet.Length);
            asm.Push_EDI();
            asm.Call_EAX();
            asm.Popad();
            asm.Ret();
            asm.RunAsm();

        }
    }

    //Набор конструктовров Пакетов
    public static class Packet
    {

        #region "Инвентарь"
        //Использовать предмет, нужны (cellNum-номер ячейки, itemID - ID предмета)
        public static byte[] ItemUsePacket(Int32 cellNum, Int32 itemID)
        {
            //Использовать предмет в инвентаре
            // (28 00 00 01 '02 00' 'A5 21 00 00') , где 28 00 00 01 - основа, 02 00 - номеря чейки, A5 21 00 00 - ID предмета
            byte[] packet = { 0x28, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            byte[] cell_num = BitConverter.GetBytes(cellNum);
            byte[] item_id = BitConverter.GetBytes(itemID);
            Array.Copy(cell_num, 0, packet, 4, 2);
            Array.Copy(item_id, 0, packet, 6, 4);
            return packet;
        }
        //Перемещение предмета из одной ячейки в другую
        public static byte[] ItemChangeCell(Int32 cellSourceCellNum, Int32 cellDestinationCellNum)
        {
            //0C 00 '01' '02' - Перемешение итема в инвентаре.
            //(2) - номер ячейки из которой требуется переместить предмет.
            //(3) - номер целевой ячейки.
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = { 0xc, 0x0, 0x1, 0x2 };
            byte[] cell_SourceCellNum = BitConverter.GetBytes(cellSourceCellNum);
            byte[] cell_DestinationCellNum = BitConverter.GetBytes(cellDestinationCellNum);
            Array.Copy(cell_SourceCellNum, 0, packet, 2, 1);
            Array.Copy(cell_DestinationCellNum, 0, packet, 3, 1);
            return packet;
        }
        //Разбить стек предметов в инвентаре
        public static byte[] SplitStackItems(Int32 cellSourceCellNum, Int32 cellDestinationCellNum, Int32 itemsAmount)
        {
            //{&HD, &H0, &H0, &H0, &H0, &H0} - разбить стек вещей на 2
            //(2) - номер ячейки из которой требуется переместить предмет.
            //(3) - номер целевой ячейки.
            //(4-5) - кол-во предметов для перемещения
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = { 0xd, 0x0, 0x0, 0x0, 0x0, 0x0 };
            byte[] cell_SourceCellNum = BitConverter.GetBytes(cellSourceCellNum);
            byte[] cell_DestinationCellNum = BitConverter.GetBytes(cellDestinationCellNum);
            byte[] items_Amount = BitConverter.GetBytes(itemsAmount);
            Array.Copy(cell_SourceCellNum, 0, packet, 2, 1);
            Array.Copy(cell_DestinationCellNum, 0, packet, 3, 1);
            Array.Copy(items_Amount, 0, packet, 4, 2);
            return packet;
        }
        //Выкинуть предметы на землю
        public static byte[] DropItems(Int32 cellSourceCellNum, Int32 itemsAmount)
        {
            //{&HE, &H0, &H0, &H0, &H0} - выкинуть часть вещей
            //(2) - номер ячейки из которой требуется переместить предмет.
            //(3) - кол-во предметов для перемещения
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = { 0xe, 0x0, 0x0, 0x0, 0x0 };
            byte[] cell_SourceCellNum = BitConverter.GetBytes(cellSourceCellNum);
            byte[] items_Amount = BitConverter.GetBytes(itemsAmount);
            Array.Copy(cell_SourceCellNum, 0, packet, 2, 1);
            Array.Copy(items_Amount, 0, packet, 3, 2);
            return packet;
        }
        //Выкинуть деньги на землю
        public static byte[] DropGold(Int32 itemsAmount)
        {
            //{&H14, &H0, &H0, &H0, &H0, &H0} - выкинуть часть денег
            //(2) - количество монет
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = { 0x14, 0x0, 0x0, 0x0, 0x0, 0x0 };
            byte[] items_Amount = BitConverter.GetBytes(itemsAmount);
            Array.Copy(items_Amount, 0, packet, 2, 4);
            return packet;
        }
        //Обновить позиции инвентаря
        public static byte[] UpdateInventaryPositions()
        {
            byte[] packet = { 0x9, 0x0, 0x0 };
            return packet;
            //todo: Проверить работоспособность
        }
        //Переместить предмет в инвентаре
        public static byte[] ItemInBankChangeCell(Int32 cellSourceCellNum, Int32 cellDestinationCellNum)
        {
            byte[] packet = { 0x38, 0x0, 0x3, 0x0, 0x0 };
            byte[] cell_SourceCellNum = BitConverter.GetBytes(cellSourceCellNum);
            byte[] cell_DestinationCellNum = BitConverter.GetBytes(cellDestinationCellNum);
            Array.Copy(cell_SourceCellNum, 0, packet, 3, 1);
            Array.Copy(cell_DestinationCellNum, 0, packet, 4, 1);
            return packet;
        }
        //Переместить часть предметов в другую ячейку в инвентаре
        public static byte[] SplitStackItemsInBank(Int32 cellSourceCellNum, Int32 cellDestinationCellNum, Int32 itemsAmount)
        {
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = { 0x39, 0x0, 0x3, 0x0, 0x0, 0x0, 0x0 };
            byte[] cell_SourceCellNum = BitConverter.GetBytes(cellSourceCellNum);
            byte[] cell_DestinationCellNum = BitConverter.GetBytes(cellDestinationCellNum);
            byte[] items_Amount = BitConverter.GetBytes(itemsAmount);
            Array.Copy(cell_SourceCellNum, 0, packet, 3, 1);
            Array.Copy(cell_DestinationCellNum, 0, packet, 4, 1);
            Array.Copy(items_Amount, 0, packet, 5, 2);
            return packet;
        }
        //Сменить Стильный/Боевой стиль
        public static byte[] ToggleFashionDisplay()
        {
            byte[] packet = { 0x55, 0x0 };
            return packet;
        }

        #endregion
        #region "Аммуниция"
        //Выкинуть на землю амулет на ХП
        public static byte[] ItemDropAmuletHP()
        {
            byte[] packet = { 0xf, 0x0, 0x14 };
            return packet;
        }
        //Выкинуть на землю идол на МП
        public static byte[] ItemDropIdolMP()
        {
            byte[] packet = { 0xf, 0x0, 0x15 };
            return packet;
        }
        //Выкинуть на землю любой другой предмет амуниции
        public static byte[] ItemDrop(int cellAmmunitionNum)
        {
            byte[] packet = { 0xf, 0x0, 0x0 };
            byte[] cell_AmmunitionNum = BitConverter.GetBytes(cellAmmunitionNum);
            Array.Copy(cell_AmmunitionNum, 0, packet, 2, 1);
            return packet;
        }
        //Одевать или снимать амуницию в инвентарь
        public static byte[] ItemChangeAmmunition(Int32 cellInventaryNum, Int32 cellAmmunitionNum)
        {
            //11 00 '01' 05' - Одеть предмет.
            //{&H10, &H0, &H0, &H0}  - поменять местами кольца в икиперовке 
            // (2) - номер ячейки инвентаря, в которой лежит предмет.
            // (3) - номер целевой ячейки на "кукле".
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = {
			0x11,
			0x0,
			0x1,
			0x5
		};
            byte[] cell_InventaryNum = BitConverter.GetBytes(cellInventaryNum);
            byte[] cell_AmmunitionNum = BitConverter.GetBytes(cellAmmunitionNum);
            Array.Copy(cell_InventaryNum, 0, packet, 2, 1);
            Array.Copy(cell_AmmunitionNum, 0, packet, 3, 1);
            return packet;
            //todo - Проверить как работает одевание предмета и смена у чувака пакет 
        }
        #endregion
        #region "Банк"
        //Обменять вещь между инвентарем и банком
        public static byte[] SwapItemsInventoryAndBank(Int32 cellInventaryNum, Int32 cellBankNum)
        {
            //Если в целевой ячейке есть предмет - предметы поменяются местами.
            byte[] packet = {
			0x3a,
			0x0,
			0x3,
			0x0,
			0x0
		};
            byte[] cell_InventaryNum = BitConverter.GetBytes(cellInventaryNum);
            byte[] cell_BankNum = BitConverter.GetBytes(cellBankNum);
            Array.Copy(cell_InventaryNum, 0, packet, 3, 1);
            Array.Copy(cell_BankNum, 0, packet, 4, 1);
            return packet;
            //todo - Проверить как работает одевание предмета и смена у чувака пакет 
        }
        //Переложить часть вещей из Банка в Инвентарь
        public static byte[] SplitStackItemFromBankToInventory(Int32 cellBankNum, Int32 cellInventaryNum, int itemAmount)
        {
            byte[] packet = {
			0x3b,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] cell_BankNum = BitConverter.GetBytes(cellBankNum);
            byte[] cell_InventaryNum = BitConverter.GetBytes(cellInventaryNum);
            byte[] item_Amount = BitConverter.GetBytes(itemAmount);
            Array.Copy(cell_BankNum, 0, packet, 3, 1);
            Array.Copy(cell_InventaryNum, 0, packet, 4, 1);
            Array.Copy(item_Amount, 0, packet, 5, 1);
            return packet;
        }
        //Переложить часть вещей из Инвентаря в Банк
        public static byte[] SplitStackItemFromInventoryToBank(Int32 cellInventaryNum, Int32 cellBankNum, int itemAmount)
        {
            byte[] packet = {
			0x3c,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] cell_InventaryNum = BitConverter.GetBytes(cellInventaryNum);
            byte[] cell_BankNum = BitConverter.GetBytes(cellBankNum);
            byte[] item_Amount = BitConverter.GetBytes(itemAmount);
            Array.Copy(cell_InventaryNum, 0, packet, 3, 1);
            Array.Copy(cell_BankNum, 0, packet, 4, 1);
            Array.Copy(item_Amount, 0, packet, 5, 1);
            return packet;

        }
        #endregion
        #region "Магазин Кот"
        //Запустить встать в кота
        public static byte[] InitiateSettingUpCatShop()
        {
            byte[] packet = {
			0x54,
			0x0
		};
            return packet;
        }
        #endregion
        #region "Взаимодействие с игроками"
        //Посмотреть экипировку чужого игрока
        public static byte[] ViewPlayerEquip(int playerID)
        {
            byte[] packet = {
			0x63,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Попросить мужчину взять на руки
        public static byte[] AskMaleToBeCarried(int playerID)
        {
            byte[] packet = {
			0x5e,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Предложить женщине взять ее на руки
        public static byte[] AskFemaleToBeCarried(int playerID)
        {
            byte[] packet = {
			0x5f,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Согласиться на предложение женщины взять на руки
        public static byte[] AcceptRequestByFemaleToBeCarried(int playerID)
        {
            byte[] packet = {
			0x60,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};

            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Согласиться на предложение мужчины взять на руки
        public static byte[] AcceptRequestByMaleToCarryYou(int playerID)
        {
            byte[] packet = {
			0x61,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Перестать обниматься на руках
        public static byte[] ReleaseCarryMode()
        {
            byte[] packet = {
			0x62,
			0x0
		};
            return packet;
        }
        //Взять Асист с персонажа по его ID
        public static byte[] Asist(int playerID)
        {
            byte[] packet = {
			0x38,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;

        }

        #endregion
        #region "Дуэль"
        //Пригласить на дуэль
        public static byte[] InviteToDuel(int playerID)
        {
            //23 00 D1 3C 10 80 - открыть диалог с NPC. (c 2 по 5 = 4 байта - ID NPC)
            byte[] packet = {
			0x5c,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Согласиться на дуэль
        public static byte[] AcceptDuel(int playerID)
        {
            //23 00 D1 3C 10 80 - открыть диалог с NPC. (c 2 по 5 = 4 байта - ID NPC)
            byte[] packet = {
			0x5d,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        #endregion
        #region "Работа с командой"
        //Пригласить в пати, нужен WID игрока
        public static byte[] InvitePaty(int playerID)
        {
            //1B 00 'E1 0E 56 00' - пригласить в пати. Песочный(c 2 по 5 = 4 байта) - id персонажа
            byte[] packet = {
			0x1b,
			0x0,
			0xe1,
			0xe,
			0x56,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Согласиться войти в пати (возможно partyCounter = 0 или 1, если нам кинули только одно приглашение)
        public static byte[] AcceptPartyIvite(int playerID, int partyCounter)
        {
            //{&H1C, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}
            byte[] packet = {
			0x1c,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            byte[] party_Counter = BitConverter.GetBytes(partyCounter);
            Array.Copy(player_ID, 0, packet, 2, 4);
            Array.Copy(party_Counter, 0, packet, 6, 4);
            return packet;
        }
        //Отказаться войти в пати
        public static byte[] RefusePartyIvite(int playerID)
        {
            //{&H1C, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}
            byte[] packet = {
			0x1d,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //Покинуть пати
        public static byte[] LeaveParty()
        {
            //выйти из пати 1E 00
            byte[] packet = {
			0x1e,
			0x0
		};
            return packet;
        }
        //Выкинуть из пати
        public static byte[] KickFromParty(int playerID)
        {
            //{&H1C, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0}
            byte[] packet = {
			0x1f,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] player_ID = BitConverter.GetBytes(playerID);
            Array.Copy(player_ID, 0, packet, 2, 4);
            return packet;
        }
        //установить Поиск членов для группы
        public static byte[] SetFindPartyMessage(int jobId, int lvl, int recruit, int slogan)
        {
            byte[] packet = {
			0x3f,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] _jobId = BitConverter.GetBytes(jobId);
            byte[] _lvl = BitConverter.GetBytes(lvl);
            byte[] _recruit = BitConverter.GetBytes(recruit);
            byte[] _slogan = BitConverter.GetBytes(slogan);
            Array.Copy(_jobId, 0, packet, 2, 4);
            Array.Copy(_lvl, 0, packet, 6, 4);
            Array.Copy(_recruit, 0, packet, 10, 4);
            Array.Copy(_slogan, 0, packet, 14, 4);
            return packet;
        }
        //Передать ПЛ
        public static byte[] ShiftPartyCaptain(int playerID)
        {
            byte[] packet = {
			0x48,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] npc_ID = BitConverter.GetBytes(playerID);
            Array.Copy(npc_ID, 0, packet, 2, 4);
            return packet;
        }

        #endregion
        #region "Действия Персонажа"
        //Сесть в медитацию
        public static byte[] MeditationStart()
        {

            //Включить медитацию
            byte[] packet = {
			0x2e,
			0x0
		};

            return packet;

        }
        //Выйти из медитации
        public static byte[] MeditationStop()
        {

            //Включить медитацию
            byte[] packet = {
			0x2f,
			0x0
		};

            return packet;

        }
        //Использовать Основную атаку
        public static byte[] HostPlayerAtack()
        {
            byte[] packet = {
			0x3,
			0x0,
			0x0
		};
            return packet;
        }
        //Взлет и посадка на Полете, flyID - ID предмета Полета
        public static byte[] UseFly(int flyID)
        {
            //Использовать полет 28 00 01 01 0C 00 '30 08 00 00' - взлет и посадка на флайте. Песочный(c 6 по 9 = 4 байта) - id флайта.
            byte[] packet = {
			0x28,
			0x0,
			0x1,
			0x1,
			0xc,
			0x0,
			0xf7,
			0x31,
			0x0,
			0x0
		};
            byte[] fly_ID = BitConverter.GetBytes(flyID);
            Array.Copy(fly_ID, 0, packet, 6, 4);
            return packet;
        }
        //Включить ускоренный полет
        public static byte[] FastFlyOn()
        {
            //включить ускоренный полет
            byte[] packet = {
			0x5a,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Выключить ускоренный полет
        public static byte[] FastFlyOFF()
        {
            //выключить ускоренный полет
            byte[] packet = {
			0x5a,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Набор кривляний по названию 
        //public static Function StartEmotion(actionName As String) As Byte()
        //    Dim packet As Byte()
        //    Select Case actionName
        //        Case "Помахать рукой"
        //            packet = {&H30, &H0, &H0, &H0}
        //        Case "Кивать головой"
        //            packet = {&H30, &H0, &H1, &H0}
        //        Case "Качать головой"
        //            packet = {&H30, &H0, &H2, &H0}
        //        Case "Пожать плечами"
        //            packet = {&H30, &H0, &H3, &H0}
        //        Case "Рассмеяться"
        //            packet = {&H30, &H0, &H4, &H0}
        //        Case "Рассердиться"
        //            packet = {&H30, &H0, &H5, &H0}
        //        Case "Упасть"
        //            packet = {&H30, &H0, &H6, &H0}
        //        Case "Грустить"
        //            packet = {&H30, &H0, &H7, &H0}
        //        Case "Воздушный поцелуй"
        //            packet = {&H30, &H0, &H8, &H0}
        //        Case "Стесняться"
        //            packet = {&H30, &H0, &H9, &H0}
        //        Case "Благодарить"
        //            packet = {&H30, &H0, &HA, &H0}
        //        Case "Сесть"
        //            packet = {&H30, &H0, &HB, &H0}
        //        Case "Загадка"
        //            packet = {&H30, &H0, &HC, &H0}
        //        Case "Думать"
        //            packet = {&H30, &H0, &HD, &H0}
        //        Case "Насмехаться"
        //            packet = {&H30, &H0, &HE, &H0}
        //        Case "Победа"
        //            packet = {&H30, &H0, &HF, &H0}
        //        Case "Потянуться"
        //            packet = {&H30, &H0, &H10, &H0}
        //        Case "Бой"
        //            packet = {&H30, &H0, &H11, &H0}
        //        Case "Атака1"
        //            packet = {&H30, &H0, &H12, &H0}
        //        Case "Атака2"
        //            packet = {&H30, &H0, &H13, &H0}
        //        Case "Атака3"
        //            packet = {&H30, &H0, &H14, &H0}
        //        Case "Атака4"
        //            packet = {&H30, &H0, &H15, &H0}
        //        Case "Защита"
        //            packet = {&H30, &H0, &H16, &H0}
        //        Case "Упасть"
        //            packet = {&H30, &H0, &H17, &H0}
        //        Case "Притворная смерть"
        //            packet = {&H30, &H0, &H18, &H0}
        //        Case "Оглядеться"
        //            packet = {&H30, &H0, &H19, &H0}
        //        Case "Танец"
        //            packet = {&H30, &H0, &H1A, &H0}
        //    End Select

        //    Return packet
        //End Function
        //Быть интимным хз что
        public static byte[] BeIntimate()
        {
            byte[] packet = {
			0x30,
			0x0,
			0x1d,
			0x0
		};
            return packet;
        }
        //Воскреситься в город
        public static byte[] HostPlayerResurectToTown()
        {
            byte[] packet = {
			0x4,
			0x0
		};
            return packet;
        }
        //Воскреситься свитком
        public static byte[] HostPlayerResurectWithScroll()
        {
            byte[] packet = {
			0x5,
			0x0
		};
            return packet;
        }
        //Воскреситься от Приста
        public static byte[] HostPlayerResurectWithPrist()
        {
            byte[] packet = {
			0x57,
			0x0
		};
            return packet;
        }
        //применить изменение статов
        public static byte[] UpdateStats()
        {
            byte[] packet = {
			0x15,
			0x0
		};
            return packet;
        }
        //увеличить значение статов
        public static byte[] IncreaseStats(int constitution, int intelligence, int strength, int agility)
        {
            byte[] packet = {
			0x16,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] _constitution = BitConverter.GetBytes(constitution);
            byte[] _intelligence = BitConverter.GetBytes(intelligence);
            byte[] _strength = BitConverter.GetBytes(strength);
            byte[] _agility = BitConverter.GetBytes(agility);
            Array.Copy(_constitution, 0, packet, 2, 4);
            Array.Copy(_intelligence, 0, packet, 6, 4);
            Array.Copy(_strength, 0, packet, 10, 4);
            Array.Copy(_agility, 0, packet, 14, 4);
            return packet;
        }

        //Выделить моба/НПС/Игрока, нужен WID моба/НПС или ID игрока
        public static byte[] SelectTarget(int targetWID)
        {
            //02 00 EC 3D 10 80 - Выделить моба/НПС/Игрока. Песочный - WID моба/НПС или ID игрока. (c 2 по 5 = 4 байта - ID цели)
            byte[] packet = {
			0x2,
			0x0,
			0xec,
			0x3d,
			0x10,
			0x80
		};
            byte[] target_WID = BitConverter.GetBytes(targetWID);
            Array.Copy(target_WID, 0, packet, 2, 4);
            return packet;
        }
        //Сбросить цель
        public static byte[] DeselectTarget()
        {
            byte[] packet = {
			0x8,
			0x0
		};
            return packet;
        }


        #endregion
        #region "Скилы"
        //Использовать скилл (Нужно контролировать расстояние до цели в этом случае)
        public static byte[] HostPlayerUseSkill(int skillID, int targetID)
        {
            // - использовать скилл. (c 2 по 5 = 4 байта - skillID, c 8 по 11 = 4 байта - targetID)
            byte[] packet = { 0x50, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0 };
            byte[] skill_ID = BitConverter.GetBytes(skillID);
            byte[] target_ID = BitConverter.GetBytes(targetID);
            Array.Copy(skill_ID, 0, packet, 2, 4);
            Array.Copy(target_ID, 0, packet, 8, 4);
            return packet;
        }
        //Прекратить использовать скилл
        public static byte[] HostPlayerStopUseSkill()
        {
            byte[] packet = {
			0x2a,
			0x0
		};
            return packet;
        }

        //Использовать скилл без времени каста
        public static byte[] HostPlayerUseSkillWithoutCastTime(int skillID, int targetID)
        {
            //29 00 '49 01 00 00' 00 01 '70 92 41 00' - использовать скилл. (c 2 по 5 = 4 байта - skillID, c 8 по 11 = 4 байта - targetID)
            byte[] packet = {
			0x29,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] skill_ID = BitConverter.GetBytes(skillID);
            byte[] target_ID = BitConverter.GetBytes(targetID);
            Array.Copy(skill_ID, 0, packet, 2, 4);
            Array.Copy(target_ID, 0, packet, 8, 4);
            return packet;
        }

        #endregion
        #region "Питомец"
        //Перевести пета в состояние охраны
        public static byte[] PetSecurityOn()
        {
            byte[] packet = {
			0x67,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x3,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Перевести пета в состояние атаки
        public static byte[] PetAgressiveOn()
        {
            byte[] packet = {
			0x67,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x3,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Перевести пета в состояние По команде
        public static byte[] PetPassiveOn()
        {
            byte[] packet = {
			0x67,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x3,
			0x0,
			0x0,
			0x0,
			0x2,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Вызвать пета(cellNum-номер ячейки)
        public static byte[] PetCall(Int32 hutchNum)
        {
            //вызывать пета
            //64 00 00 00 00 00 , где последнии 4 байта 0N 00 00 0 - клетка от нуля
            byte[] packet = {
			0x64,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] hutch_Num = BitConverter.GetBytes(hutchNum);
            Array.Copy(hutch_Num, 0, packet, 2, 4);
            return packet;
        }
        //Убрать пета
        public static byte[] PetHide()
        {
            //вызывать пета
            //65 00 
            byte[] packet = {
			0x65,
			0x0
		};
            return packet;
        }
        //Физическая атака петом, нужен mobWID -Мировой ID моба
        public static byte[] PetAtack(Int32 mobWID)
        {
            //атака петом
            //67 00 '00 00 00 80' 01 00 00 00 00 - атаковать петом, где '00 00 00 80' - WID моба
            byte[] packet = {
			0x67,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] hutch_WID = BitConverter.GetBytes(mobWID);
            Array.Copy(hutch_WID, 0, packet, 2, 4);
            return packet;
        }
        //Следовать за персонажем
        public static byte[] PetFollow()
        {
            byte[] packet = {
			0x67,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x2,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Остановиться пету
        public static byte[] PetStop()
        {
            byte[] packet = {
			0x67,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x2,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }
        //Использовать скиллы пета
        public static byte[] PetUseSkill(int skillID, int targetWID)
        {
            byte[] packet = {
			0x67,
			0x0,
			0x9c,
			0x49,
			0x10,
			0x80,
			0x4,
			0x0,
			0x0,
			0x0,
			0xaf,
			0x2,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] skill_ID = BitConverter.GetBytes(skillID);
            byte[] target_WID = BitConverter.GetBytes(targetWID);
            Array.Copy(target_WID, 0, packet, 2, 4);
            Array.Copy(skill_ID, 0, packet, 10, 4);
            return packet;
            //todo: Если не будет работать - попробовать укоротить основной массив на 1 байт
        }

        #endregion
        #region "Лут и Ресы"
        //Поднять лут
        public static byte[] PickUpLoot(int itemWID, int itemID)
        {
            byte[] packet = {
			0x6,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] item_WID = BitConverter.GetBytes(itemWID);
            byte[] item_ID = BitConverter.GetBytes(itemID);
            Array.Copy(item_WID, 0, packet, 2, 4);
            Array.Copy(item_ID, 0, packet, 6, 4);
            return packet;
        }
        //Добыть ресурс
        public static byte[] HurvestResource(int itemWID)
        {
            byte[] packet = {
			0x36,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x1e,
			0x0,
			0x1,
			0xc,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] item_WID = BitConverter.GetBytes(itemWID);
            Array.Copy(item_WID, 0, packet, 2, 4);
            return packet;
        }
        #endregion
        #region "Общие действия"
        //Выйти из игры
        public static byte[] LogOutServer()
        {
            byte[] packet = {
			0x1,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            return packet;
        }

        #endregion
        #region "Джины"
        //Использовать скиллы джина, 
        public static byte[] GenieUseSkill(int skillID, int targetWID)
        {
            byte[] packet = {
			0x74,
			0x0,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] skill_ID = BitConverter.GetBytes(skillID);
            byte[] target_WID = BitConverter.GetBytes(targetWID);
            Array.Copy(target_WID, 0, packet, 2, 4);
            Array.Copy(skill_ID, 0, packet, 6, 4);
            return packet;
        }
        //Положить что то джину (видимо камни, чтобы джин мог использовать свои скилы)
        public static byte[] FeedEquippedGenie(int cellIndex, int itemAmount)
        {
            byte[] packet = {
			0x75,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] cell_Index = BitConverter.GetBytes(cellIndex);
            byte[] item_Amount = BitConverter.GetBytes(itemAmount);
            Array.Copy(cell_Index, 0, packet, 2, 1);
            Array.Copy(item_Amount, 0, packet, 3, 4);
            return packet;
        }
        //Передать опыт джину '71 00 4c 00 00 00 00 - слить опыт джину со 2 = 4  - кол-во очков опыта Персонажа
        public static byte[] TransmitExpToGenie(int ExpPointAmount)
        {
            byte[] packet = {
			0x71,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] ExpPoint_Amount = BitConverter.GetBytes(ExpPointAmount);
            Array.Copy(ExpPoint_Amount, 0, packet, 2, 4);
            return packet;
        }

        #endregion
        #region "Квесты"
        //Согласиться на квест
        public static byte[] AcceptQuest(int questID)
        {
            byte[] packet = {
			0x25,
			0x0,
			0x7,
			0x0,
			0x0,
			0x0,
			0x4,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] quest_ID = BitConverter.GetBytes(questID);
            Array.Copy(quest_ID, 0, packet, 10, 4);
            return packet;
        }
        //Поле в квестах
        public static byte[] HandInQuest(int questID, int optionIndex)
        {
            byte[] packet = {
			0x25,
			0x0,
			0x6,
			0x0,
			0x0,
			0x0,
			0x8,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] quest_ID = BitConverter.GetBytes(questID);
            byte[] option_Index = BitConverter.GetBytes(optionIndex);
            Array.Copy(quest_ID, 0, packet, 10, 4);
            Array.Copy(option_Index, 0, packet, 14, 4);
            return packet;
        }
        #endregion
        #region "NPC"
        //открыть диалог с NPC
        public static byte[] StartDialogNPC(int npcID)
        {
            //23 00 D1 3C 10 80 - открыть диалог с NPC. (c 2 по 5 = 4 байта - ID NPC)
            byte[] packet = {
			0x23,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] npc_ID = BitConverter.GetBytes(npcID);
            Array.Copy(npc_ID, 0, packet, 2, 4);
            return packet;
        }
        //Продать вещь
        public static byte[] SellSingleItem(int itemID, int cellIndex, int itemAmount)
        {
            byte[] packet = {
			0x25,
			0x0,
			0x2,
			0x0,
			0x0,
			0x0,
			0x10,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            //gurin утвреждает что опкод будет 25 00 02 00 00 00 10 00 00 00 01 00 00 00 [00 00 00 00] (00 00 00 00) {00 00 00 00}
            byte[] item_ID = BitConverter.GetBytes(itemID);
            byte[] cell_Index = BitConverter.GetBytes(cellIndex);
            byte[] item_Amount = BitConverter.GetBytes(itemAmount);
            Array.Copy(item_ID, 0, packet, 14, 4);
            Array.Copy(cell_Index, 0, packet, 18, 4);
            Array.Copy(item_Amount, 0, packet, 12, 4);
            return packet;
        }
        //Купить  вещь
        public static byte[] BuySingleItem(int itemID, int shopIndex, int itemAmount)
        {
            //gurin утвреждает что опкод будет http://zhyk.ru/forum/showpost.php?p=2440306&postcount=86 25 00 01 00 00 00 1C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 [00 00 00 00](00 00 00 00){00 00 00 00}
            byte[] packet = {
			0x25,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0,
			0x14,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x1,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] item_ID = BitConverter.GetBytes(itemID);
            byte[] shop_Index = BitConverter.GetBytes(shopIndex);
            byte[] item_Amount = BitConverter.GetBytes(itemAmount);
            Array.Copy(item_ID, 0, packet, 18, 4);
            Array.Copy(shop_Index, 0, packet, 22, 4);
            Array.Copy(item_Amount, 0, packet, 26, 4);
            return packet;
        }
        //Чинить все
        public static byte[] RepairAll()
        {
            byte[] packet = {
			0x25,
			0x0,
			0x3,
			0x0,
			0x0,
			0x0,
			0x6,
			0x0,
			0x0,
			0x0,
			0xff,
			0xff,
			0xff,
			0xff,
			0x0,
			0x0
		};
            return packet;
        }
        //Чинить конкретный предмет
        public static byte[] RepairSingleItem(int itemID, int isEquipped, int locationIndex)
        {
            byte[] packet = {
			0x25,
			0x0,
			0x3,
			0x0,
			0x0,
			0x0,
			0x6,
			0x0,
			0x0,
			0x0,
			0xff,
			0xff,
			0xff,
			0xff,
			0x0,
			0x0
		};
            byte[] item_ID = BitConverter.GetBytes(itemID);
            byte[] is_Equipped = BitConverter.GetBytes(isEquipped);
            byte[] location_Index = BitConverter.GetBytes(locationIndex);
            Array.Copy(item_ID, 0, packet, 10, 4);
            Array.Copy(is_Equipped, 0, packet, 14, 1);
            Array.Copy(location_Index, 0, packet, 15, 1);
            return packet;
        }
        //Улучшить скилл
        public static byte[] UpgradeSkill(int skillID)
        {
            byte[] packet = {
			0x25,
			0x0,
			0x9,
			0x0,
			0x0,
			0x0,
			0x4,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0,
			0x0
		};
            byte[] skill_ID = BitConverter.GetBytes(skillID);
            Array.Copy(skill_ID, 0, packet, 10, 4);
            return packet;
        }

        #endregion

    }
}
