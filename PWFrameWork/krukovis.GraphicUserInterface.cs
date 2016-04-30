using System;
using System.Collections.Generic;
using System.Text;

namespace PWFrameWork
{

    /// <summary>
    /// Структура PWGUI
    /// </summary>
    public class PWGUI
    {
        //Объявляем класс для работы с памятью
        private MemoryWork memory;

        //Конструируем PWGUI в зависимости ClientWindow с памятью которого нужно работать
        public PWGUI(ClientWindow client_window)
        {

            memory = new MemoryWork(client_window);

        }


        //Получаем структуру PWForm по имени
        public PWForm Form(string form_name)
        {
            //возвращаем форму по имени
            return new PWForm(this.memory, form_name);

        }

        /// <summary>
        /// Получаем структуру текущего в данный момент окна
        /// </summary>
        public PWForm ActiveForm
        {
            get
            {
                //возвращаем форму активную в данный момент
                return new PWForm(this.memory);
            }
        }

    }

    /// <summary>
    /// Структура формы
    /// </summary>
    public class PWForm
    {
        //класс для работы с памятью
        private MemoryWork memory;
        //структура окна
        private int form_ptr;

        //Конструктор возвращающий форму по имени формы
        public PWForm(MemoryWork memory_work, string form_name)
        {
            this.memory = memory_work;
            this.form_ptr = GetFormPtrOnName(form_name);
        }

        //Конструктор возвращающий форму по умолчанию
        public PWForm(MemoryWork memory_work)
        {
            this.memory = memory_work;
            this.form_ptr = GetActiveFormPtr();
        }

        /// <summary>
        /// Получает структуру формы по имени формы
        /// </summary>
        /// <param name="form_name">string: имя формы</param>
        /// <returns>int: указатель на структуру формы</returns>
        private int GetFormPtrOnName(string form_name)
        {
            //Структура гуи
            int gui_struct = memory.ChainReadInt32(PWOffssAndAddrss.base_address, PWOffssAndAddrss.game_struct_offset, PWOffssAndAddrss.grafic_offset, PWOffssAndAddrss.gui_offset);
            //Обяъвляем адрес начала структуры окон высокого уровня
            int HightWinStruct = memory.ChainReadInt32(gui_struct + PWOffssAndAddrss.hight_level_forms_list_offset);
            //Обяъвляем адрес начала структуры окон низкого уровня
            int LowWinStruct = memory.ChainReadInt32(gui_struct + PWOffssAndAddrss.low_level_forms_list_offset); ;


            //Указатель на текущую структуру окна
            int current_form_ptr;// 
            //Указатель на структуру следующего окна или 0, если это последнее окно
            int nextWinPtr;//

            //Рассмотрим окна высокого уровня
            current_form_ptr = memory.ReadInt32(HightWinStruct + PWOffssAndAddrss.current_form_struct_offset_in_forms_list);
            if (current_form_ptr != 0)
            {
                //Если нашли нужное имя - возвращем указатель и выходим из функции. winName = current_form_ptr + &H4C +&h0, string ASCII 
                if (memory.ChainReadString_ASCII(current_form_ptr + PWOffssAndAddrss.form_name_offset, 32, 0) == form_name)
                {
                    return current_form_ptr;
                }
            }

            nextWinPtr = memory.ReadInt32(HightWinStruct + PWOffssAndAddrss.next_form_ptr_offset_in_forms_list);
            //Перебираем если следующее не ноль
            while (nextWinPtr != 0)
            {
                //получаем стуруктуру нового текущего окна
                current_form_ptr = memory.ReadInt32(nextWinPtr + PWOffssAndAddrss.current_form_struct_offset_in_forms_list);

                //Если нашли нужное имя - возвращем указатель и выходим из функции. winName = current_form_ptr + &H4C +&h0, string ASCII 
                if (memory.ChainReadString_ASCII(current_form_ptr + PWOffssAndAddrss.form_name_offset, 32, 0) == form_name)
                {
                    return current_form_ptr;
                }
                //берем следующее окно 
                nextWinPtr = memory.ReadInt32(nextWinPtr + PWOffssAndAddrss.next_form_ptr_offset_in_forms_list);

            }

            //Рассмотрим окна низкого уровня
            current_form_ptr = memory.ReadInt32(LowWinStruct + PWOffssAndAddrss.current_form_struct_offset_in_forms_list);
            //если текущее (первое) окно не ноль 
            if (current_form_ptr != 0)
            {
                //Если нашли нужное имя - возвращем указатель и выходим из функции. winName = current_form_ptr + &H4C +&h0, string ASCII 
                if (memory.ChainReadString_ASCII(current_form_ptr + PWOffssAndAddrss.form_name_offset, 32, 0) == form_name)
                {
                    return current_form_ptr;
                }
            }
            //смотрим есть ли следующее
            nextWinPtr = memory.ReadInt32(LowWinStruct + PWOffssAndAddrss.next_form_ptr_offset_in_forms_list);
            //Перебираем пока следующее не станет нулем
            while (nextWinPtr != 0)
            {
                //т.к. следующее было не ноль - получаем  указатель на его структуру
                current_form_ptr = memory.ReadInt32(nextWinPtr + PWOffssAndAddrss.current_form_struct_offset_in_forms_list);
                //Если нашли нужное имя - возвращем указатель и выходим из функции. winName = current_form_ptr + &H4C +&h0, string ASCII 
                if (memory.ChainReadString_ASCII(current_form_ptr + PWOffssAndAddrss.form_name_offset, 32, 0) == form_name)
                {
                    return current_form_ptr;
                }
                //берем следующее окно 
                nextWinPtr = memory.ReadInt32(nextWinPtr + PWOffssAndAddrss.next_form_ptr_offset_in_forms_list);
            }

            //Если ничего не нашли - возвращаем ноль
            return 0;
        }

        /// <summary>
        /// Получает структуру активной в данный момент формы
        /// </summary>
        /// <returns></returns>
        private int GetActiveFormPtr()
        {
            int forms_struct = memory.ChainReadInt32(PWOffssAndAddrss.base_address, PWOffssAndAddrss.game_struct_offset, PWOffssAndAddrss.grafic_offset, PWOffssAndAddrss.gui_offset);
            int active_form = memory.ReadInt32(forms_struct + PWOffssAndAddrss.active_form_struct);
            return active_form;
        }

        /// <summary>
        /// Получает или задает видимость формы
        /// </summary>
        public bool Visible
        {
            get
            {
                //Если форма существует
                if (this.form_ptr != 0)
                {
                    //Смотрим видима ли она
                    return Convert.ToBoolean(memory.ReadByte(this.form_ptr + PWOffssAndAddrss.form_visible_offset));
                }
                else
                {
                    return false;
                }

            }
            set
            {
                //Если форма существует 
                if (this.form_ptr != 0)
                {
                    //устанавливаем ей параметр видимости
                    memory.WriteByte(this.form_ptr + PWOffssAndAddrss.form_visible_offset, Convert.ToByte(value));
                }

            }
        }

        /// <summary>
        /// Получает имя формы
        /// </summary>
        public string Name
        {
            get
            {
                if (form_ptr != 0)
                {
                    //
                    return memory.ChainReadString_ASCII(form_ptr + PWOffssAndAddrss.form_name_offset, 32, 0);
                }
                else
                {
                    return @"noname";
                }

            }
        }

        /// <summary>
        /// Получает первый найденный контрол с таким именем(или с частью имени)
        /// </summary>
        /// <param name="control_name">имя контрола</param>
        /// <returns></returns>
        public PWControl Control(string control_name)
        {
            PWControl control = new PWControl(this.memory, control_name, this.form_ptr);
            //возвращает контрол с уже установленными свойствами
            return control;
        }

        /// <summary>
        /// Получает первый найденный CheckBox с таким именем
        /// </summary>
        /// <param name="control_name"></param>
        /// <returns></returns>
        public PWCheckBox CheckBox(string control_name)
        {
            return new PWCheckBox(this.memory, control_name, this.form_ptr);
        }

        /// <summary>
        /// Получает первый найденный Label с таким именем
        /// </summary>
        /// <param name="control_name"></param>
        /// <returns></returns>
        public PWLabel Label(string control_name)
        {
            return new PWLabel(this.memory, control_name, this.form_ptr);
        }

        /// <summary>
        /// Возвращаем первый найденный Label с таким именем
        /// </summary>
        /// <param name="control_name"></param>
        /// <returns></returns>
        public PWListBox ListBox(string control_name)
        {
            return new PWListBox(this.memory, control_name, this.form_ptr);
        }

        /// <summary>
        /// Возвращает первый найденный RadioButton с таким именем
        /// </summary>
        /// <param name="control_name"></param>
        /// <returns></returns>
        public PWRadioButton RadioButton(string control_name)
        {
            return new PWRadioButton(this.memory, control_name, this.form_ptr);
        }

        /// <summary>
        /// Возвращает первый найденный TextBox с таким именем
        /// </summary>
        /// <param name="control_name"></param>
        /// <returns></returns>
        public PWTextBox TextBox(string control_name)
        {
            return new PWTextBox(this.memory, control_name, this.form_ptr);
        }
    }

    /// <summary>
    /// Класс ТекстБокса в ПВ. Наследует у PWControl
    /// </summary>
    public class PWTextBox : PWControl
    {
        //private MemoryWork memory;
        //private string control_name;
        //private int form_ptr;
        //private int control_ptr;

        protected PWTextBox()
        {
            this.memory = null;
            this.control_name = "";
            this.control_ptr = 0;
            this.form_ptr = 0;
        }

        public PWTextBox(MemoryWork memory_work, string control_name, int form_ptr)
        {
            this.memory = memory_work;
            this.control_name = control_name;
            this.control_ptr = GetControlPtr(control_name, form_ptr);
            this.form_ptr = form_ptr;
        }


        /// <summary>
        /// Получает и задает текст в контролле
        /// </summary>
        public string Text
        {
            get
            {
                return memory.ChainReadString_Unicode(this.control_ptr + PWOffssAndAddrss.control_text_address_offset, 32, 0);
            }
            set
            {
                //тут будем хранить адрес
                int text_addr = 0;
                //= WinApi.VirtualAllocEx(memory.OpenedProcessHandle, 0, value.Length * 2 + 2, WinApi.AllocationType.Commit, WinApi.MemoryProtection.ExecuteReadWrite);
                if (this.control_name == "DEFAULT_Txt_Account")
                {
                    text_addr = memory.LoginAllocMemory;
                }
                else if (this.control_name == "Txt_PassWord")
                {
                    text_addr = memory.PassAllocMemory;
                }
                else
                {
                    text_addr = memory.TextAllocMemory;
                }


                //memory.WriteString_Unicode(text_addr, "Будем вводить текст");
                //text_addr = text_addr + 40;
                memory.WriteString_Unicode(text_addr, value);
                //заменяем в поле текущий текст на полученный
                memory.WriteInt32(this.control_ptr + PWOffssAndAddrss.control_text_address_offset, text_addr);
            }
        }


    }

    /// <summary>
    /// Класс для ListBox в PW. Наследует у PWControl
    /// </summary>
    public class PWListBox : PWControl
    {

        public PWListBox(MemoryWork memory_work, string control_name, int form_ptr)
        {
            this.memory = memory_work;
            this.control_name = control_name;
            this.form_ptr = form_ptr;
            this.control_ptr = GetControlPtr(control_name, form_ptr);
        }

        /// <summary>
        /// Возвращает или задает индекс строчки у листа
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return memory.ReadInt32(this.control_ptr + PWOffssAndAddrss.list_selected_index);
            }
            set
            {
                memory.WriteInt32(this.control_ptr + PWOffssAndAddrss.list_selected_index, value);
            }
        }


    }

    /// <summary>
    /// Класс для PW Label. 
    /// </summary>
    public class PWLabel : PWControl
    {

        public PWLabel(MemoryWork memory_work, string control_name, int form_ptr)
        {
            this.memory = memory_work;
            this.control_name = control_name;
            this.control_ptr = GetControlPtr(control_name, form_ptr);
            this.form_ptr = form_ptr;
        }

        /// <summary>
        /// Получает надпись у контролла
        /// </summary>
        public string Caption
        {
            get
            {
                return memory.ChainReadString_Unicode(this.control_ptr + PWOffssAndAddrss.control_caption_address_offset, 64, 0x0);
            }

        }

    }

    /// <summary>
    /// Класс PWCheckBox
    /// </summary>
    public class PWCheckBox : PWControl
    {
        public PWCheckBox(MemoryWork memory_work, string control_name, int form_ptr)
        {
            this.memory = memory_work;
            this.control_name = control_name;
            this.form_ptr = form_ptr;
            this.control_ptr = GetControlPtr(control_name, form_ptr);

        }

        /// <summary>
        /// Получает и задает чекнутость контролла
        /// </summary>
        /// <param name="value">bool: значение чекнутости контрола</param>
        public bool Checked
        {

            get
            {
                //Возвращает значение чекнутости контрола
                return Convert.ToBoolean(memory.ReadByte(this.control_ptr + PWOffssAndAddrss.control_checked_offset));
            }
            set
            {
                bool checked_value = value;
                //Устанавливает значение 
                memory.WriteByte(this.control_ptr + PWOffssAndAddrss.control_checked_offset, Convert.ToByte(checked_value));
            }
        }


    }

    /// <summary>
    /// Класс PWRadioButton
    /// </summary>
    public class PWRadioButton : PWControl
    {
        public PWRadioButton(MemoryWork memory_work, string control_name, int form_ptr)
        {
            this.memory = memory_work;
            this.control_name = control_name;
            this.form_ptr = form_ptr;
            this.control_ptr = GetControlPtr(control_name, form_ptr);

        }

        /// <summary>
        /// Получает и задает чекнутость контролла
        /// </summary>
        /// <param name="value">bool: значение чекнутости контрола</param>
        public bool Checked
        {

            get
            {
                //Возвращает значение чекнутости контрола
                return Convert.ToBoolean(memory.ReadByte(this.control_ptr + PWOffssAndAddrss.control_checked_offset));
            }
            set
            {
                bool checked_value = value;
                //Устанавливает значение 
                memory.WriteByte(this.control_ptr + PWOffssAndAddrss.control_checked_offset, Convert.ToByte(checked_value));
            }
        }
    }

    /// <summary>
    /// Структура контрола
    /// </summary>
    public class PWControl
    {
        protected MemoryWork memory;
        protected string control_name;
        protected int form_ptr;
        protected int control_ptr;

        public PWControl(MemoryWork memory_work, string control_name, int form_ptr)
        {
            this.memory = memory_work;
            this.control_name = control_name;
            this.form_ptr = form_ptr;
            this.control_ptr = GetControlPtr(control_name, form_ptr);

        }

        //Базовый конструктор - все в 0
        protected PWControl()
        {
            memory = null;
            control_name = "";
            form_ptr = 0;
            control_ptr = 0;
        }

        /// <summary>
        /// Получает структуру контрола
        /// </summary>
        /// <param name="control_name">Имя или начальная часть имени контрола</param>
        /// <param name="form_ptr">Указатель на структуру формы</param>
        protected int GetControlPtr(string control_name, int form_ptr)
        {

            //получаем структуру контролов заданного окна
            int form_controls_struct = memory.ReadInt32(form_ptr + PWOffssAndAddrss.form_controls_struct_offset);
            //первый контрол 
            int current_cotrol_struct = memory.ReadInt32(form_controls_struct + PWOffssAndAddrss.current_control_struct_offset);
            //если первый контрол не ноль
            if (current_cotrol_struct != 0)
            {
                string full_control_name = memory.ChainReadString_ASCII(current_cotrol_struct + PWOffssAndAddrss.control_name_offset, 32, 0);
                //задаем указатель на него
                //Если длинна имени текущего контролла >= длинне имени нужного контролла
                if (full_control_name.Length >= control_name.Length)
                {
                    if (full_control_name.Substring(0, control_name.Length) == control_name)
                    {
                        return current_cotrol_struct;

                    }
                }

            }

            //получаем указатель на следующий контрол
            int next_control_struct_ptr = memory.ReadInt32(form_controls_struct + PWOffssAndAddrss.next_control_struct_offset);
            //Если следующий контрол не ноль - перебираем  
            while (next_control_struct_ptr != 0)
            {
                //получаем указатель на структуру нового текущего контрола
                current_cotrol_struct = memory.ReadInt32(next_control_struct_ptr + PWOffssAndAddrss.current_control_struct_offset);
                //Если имя контрола соответствует нужному имени
                string full_control_name = memory.ChainReadString_ASCII(current_cotrol_struct + PWOffssAndAddrss.control_name_offset, 32, 0);
                if (full_control_name.Length >= control_name.Length)
                {
                    if (full_control_name.Substring(0, control_name.Length) == control_name)
                    {
                        //возвращаем указатель на структуру этого контрола
                        return current_cotrol_struct;
                    }
                }
                //смотрим на следующий
                next_control_struct_ptr = memory.ReadInt32(next_control_struct_ptr + PWOffssAndAddrss.next_control_struct_offset);
            }

            //Если не нашли - возвращаем 0
            return 0;

        }

        /// <summary>
        /// Активирует команду контрола
        /// </summary>
        /// <returns>bool: Возвращет true в случае успешного завершения инжекта</returns>
        public bool CommandStart()
        {
            //Проверяем нашелся ли контрол, указана ли форма, и есть ли имя команды
            if (this.control_ptr == 0 || this.form_ptr == 0)
            {
                return false;
            }

            //Смотрим что по адресу где должна быть команда
            int command_text_adr = memory.ReadInt32(control_ptr + PWOffssAndAddrss.control_command_name_offset);
            string cmd_txt = memory.ChainReadString_ASCII(command_text_adr, 32).Trim();
            if (command_text_adr == 0 || cmd_txt == "")
            {
                return false;
            }

            ASM asm = new ASM(this.memory);

            asm.Pushad();
            asm.Mov_ECX(this.form_ptr);
            asm.Push68(command_text_adr);
            asm.Mov_EAX(PWOffssAndAddrss.gui_function_address);
            asm.Call_EAX();
            asm.Popad();
            asm.Ret();

            asm.RunAsm();

            return true;
        }

    }


}
