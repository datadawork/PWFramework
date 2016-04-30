using System;
using System.Collections.Generic;
using System.Text;

namespace PWFrameWork
{
    public class PWGameWindow
    {
        //функции для работы с памятью
        public MemoryWork memory;


        public PWGameWindow(ClientWindow client_window)
        {
            this.memory = new MemoryWork(client_window);
        }

        public int HostPlayerStruct
        {
            get
            {
                return memory.ChainReadInt32(PWOffssAndAddrss.base_address, PWOffssAndAddrss.game_struct_offset, PWOffssAndAddrss.host_player_struct_offset);
            }
        }

        /// <summary>
        /// Получает класс host-игрока для данной игры
        /// </summary>
        public PWHostPlayer HostPlayer
        {
            get
            {
                return new PWHostPlayer(this);
            }
        }


        //public int OtherPlayerStruct
        //{
        //    get
        //    {
        //        return memory.ChainReadInt32(
        //    }
        //}
    }

    public class PWHostPlayer
    {
        //функции для работы с памятью
        public MemoryWork memory;


        /// <summary>
        /// Конструктор формирующий класс Personage 
        /// </summary>
        /// <param name="window"></param>
        public PWHostPlayer(PWGameWindow game_class)
        {
            this.memory = game_class.memory;
            //вычисляем структуру для host-игрока 
            this.Structure = game_class.HostPlayerStruct;
        }
        #region Свойства
        /// <summary>
        /// Структура персонажа
        /// </summary>
        public int Structure { get; set; }

        /// <summary>
        /// ID Профессии персонажа(0-воин 1-маг 2-шаман 3-друид 4-оборотень 5-убийца 6-лучник 7-жрец 8-страж 9-мистик)
        /// </summary>
        public byte ProfClass
        {
            get
            {
                return memory.ReadByte(this.Structure + PWOffssAndAddrss.class_id);
            }
        }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get
            {
                return memory.ChainReadString_Unicode(this.Structure + PWOffssAndAddrss.host_player_name_offset, 32, 0);
            }
        }

        /// <summary>
        /// Здоровье в % от максимального
        /// </summary>
        public float HpPersent
        {
            get
            {
                int hp = memory.ChainReadInt32(this.Structure + PWOffssAndAddrss.host_player_hp_offset);
                int max_hp = memory.ChainReadInt32(this.Structure + PWOffssAndAddrss.host_player_max_hp_offset);
                return ((float)hp / (float)max_hp) * 100;
            }
        }

        /// <summary>
        /// Количество манны в % от максимального
        /// </summary>
        public float MpPersent
        {
            get
            {
                int mp = memory.ChainReadInt32(this.Structure + PWOffssAndAddrss.host_player_mp_offset);
                int max_mp = memory.ChainReadInt32(this.Structure + PWOffssAndAddrss.host_player_max_mp_offset);
                return ((float)mp / (float)max_mp) * 100;
            }
        }

        public int TargetWID
        {
            get
            {
                return memory.ReadInt32(this.Structure + PWOffssAndAddrss.host_player_target_wid_offset);
            }
        }

        #endregion

        #region Методы
        /// <summary>
        /// Использовать умение
        /// </summary>
        /// <param name="skill_id">int: ID умения</param>
        public void UseSkill(int skill_id)
        {
            ASM asm = new ASM(this.memory);
            asm.Pushad();
            asm.Mov_ECX(PWOffssAndAddrss.base_address); //Базовый адрес
            asm.Mov_ECX_DWORD_Ptr_ECX();
            asm.Mov_ECX_DWORD_Ptr_ECX_Add(0x1C); //Переходим в структуру игры
            asm.Mov_ECX_DWORD_Ptr_ECX_Add(0x34);// // Переходим в структуру игрока
            asm.Push6A(-1);
            asm.Push6A(0);
            asm.Push6A(0);
            asm.Mov_EAX(skill_id);
            asm.Push_EAX();
            asm.Mov_EDX(PWOffssAndAddrss.use_skill_function_address);
            asm.Call_EDX();
            asm.Popad();
            asm.Ret();
            asm.RunAsm();

        }

        /// <summary>
        /// Сесть в медитацию
        /// </summary>
        public void MeditationStart()
        {
            PacketSender ps = new PacketSender(this.memory);
            ps.Send(Packet.MeditationStart());
        }

        public void MeditationStop()
        {
            PacketSender ps = new PacketSender(this.memory);
            ps.Send(Packet.MeditationStop());
        }

        #endregion

        #region Дочерние структуры

        /// <summary>
        /// Получает структуру Команды для данного персонажа
        /// </summary>
        public PWParty Party
        {
            get
            {
                return new PWParty(this);
            }
        }
        #endregion


    }

    /// <summary>
    /// Класс описывающий команду игрока
    /// </summary>
    public class PWParty
    {
        /// <summary>
        /// Класс host-игрока, к которому относится эта команда
        /// </summary>
        public PWHostPlayer HostPlayer;

        /// <summary>
        /// Получает класс PWParty для заданного PWPersonage
        /// </summary>
        /// <param name="host_player_class"></param>
        public PWParty(PWHostPlayer host_player_class)
        {
            this.HostPlayer = host_player_class;

        }

        /// <summary>
        /// int: Получает структуру команды
        /// </summary>
        public int Struct
        {
            get
            {
                return HostPlayer.memory.ReadInt32(HostPlayer.Structure + PWOffssAndAddrss.party_struct_offset);
            }
        }

        /// <summary>
        /// int: Получает количество персонажей в команде
        /// </summary>
        public int MembersCount
        {
            get
            {
                return HostPlayer.memory.ReadInt32(this.Struct + PWOffssAndAddrss.party_members_count_offset);
            }
        }

        /// <summary>
        /// Возвращает список членов пати
        /// </summary>
        public List<PartyMember> MembersList
        {
            get
            {
                List<PartyMember> party_members = new List<PartyMember>();
                if (this.MembersCount > 0)
                {
                    for (int i = 0; i < this.MembersCount; i++)
                    {
                        party_members.Add(new PartyMember(i, this));
                    }
                }
                return party_members;

            }
        }

        /// <summary>
        /// PartyMember: Возвращает лидера команды
        /// </summary>
        public PartyMember Leader
        {
            get
            {
                return new PartyMember(0, this);
            }
        }

        /// <summary>
        /// Возвращает члена команды по индексу - номеру в списке от 0
        /// </summary>
        /// <param name="Index">int: Индекс члена команды</param>
        /// <returns>PartyMember: </returns>
        public PartyMember Member(int Index)
        {
            return new PartyMember(Index, this);
        }
    }

    /// <summary>
    /// Класс описывающий члена в пати
    /// </summary>
    public class PartyMember
    {

        public int Index { get; set; }

        private PWParty party;

        public PartyMember(int index, PWParty party_class)
        {
            this.Index = index;
            this.party = party_class;
        }

        private int PartyMemersArrayStruct
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(party.Struct + PWOffssAndAddrss.party_member_array_struct_offset);
            }
        }

        private int Structure
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(this.PartyMemersArrayStruct + PWOffssAndAddrss.party_member_array_step * Index);
            }
        }

        /// <summary>
        /// Получает WorldID члена команды
        /// </summary>
        public int WID
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_wid_offset);
            }
        }

        public int Level
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_lvl_offset);
            }
        }

        public int HP
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_hp_offset);
            }
        }

        public int MP
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_mp_offset);
            }
        }

        public int MaxHP
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_max_hp_offset);
            }
        }

        public int MaxMP
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_max_mp_offset);
            }
        }

        public float LocX
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_loc_x);
            }
        }

        public float LocZ
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_loc_z);
            }
        }

        public float LocY
        {
            get
            {
                return party.HostPlayer.memory.ReadInt32(Structure + PWOffssAndAddrss.party_member_loc_y);
            }
        }
    }


}
