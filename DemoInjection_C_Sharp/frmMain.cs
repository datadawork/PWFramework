using System;
using System.Windows.Forms;

using PWFrameWork;
using System.Diagnostics;

namespace DemoInjection_C_Sharp
{
    public partial class frmMain : Form
    {
        //объявляем новый экземпляр класса ASM
        private ASM asm = new ASM();

        private static class HostPlayerOffsets
        {
            public const Int32 Name = 0x66c;
        }

        private const Int32 BaseAddress = 0xa571e0,
                            GameRun = 0xa57acc,
                            PacketSendFunction = 0x63db70;

        //ID процесса -
        private Int32 ProcessID;
       
        private ClientFinder ClientFinder { get; set; }

        public frmMain()
        {
            InitializeComponent();

            ClientFinder = new ClientFinder(BaseAddress, GameRun, HostPlayerOffsets.Name,0x34);
            this.FormClosed += new FormClosedEventHandler(Main_FormClosed);
        }

        void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mem != null)
                mem.Process.Dispose();
        }

        ProcessMemory mem = null;
        
        //Процедура WalkCall на Delphi - для примера перевода на C#. 
        //     procedure WalkCall(aPParams:PParams);Stdcall;
        //var CallAddress1,CallAddress2,CallAddress3:Pointer;
        //    x,y,z:single;
        //    flying:DWORD;
        //begin
        //CallAddress1:=Pointer($00494620);
        //CallAddress2:=Pointer($00498290);
        //CallAddress3:=Pointer($00494EC0);
        //x:=aPParams^.x;
        //y:=aPParams^.y;
        //z:=aPParams^.z;
        //flying:=aPParams^.Param1;
        // asm
        //  pushad

        //  mov eax, dword ptr [GA]
        //  mov esi, dword ptr [eax+$34]
        //  mov ecx, dword ptr [esi+$10BC]
        //  push 1
        //  call CallAddress1

        //  mov edi, eax
        //  lea eax, dword ptr [esp+$18]
        //  push eax
        //  push flying
        //  mov ecx, edi
        //  call CallAddress2

        //  mov ecx, dword ptr [esi+$10BC]
        //  push 0
        //  push 1
        //  push edi
        //  push 1
        //  call CallAddress3

        //  mov eax, dword ptr [GA]
        //  mov eax, dword ptr [eax+$34]
        //  mov eax, dword ptr [eax+$10BC]
        //  mov eax, dword ptr [eax+$30]
        //  mov ecx, dword ptr [eax+$4]
        //  mov eax, x
        //  mov dword ptr[ecx+$20], eax
        //  mov eax, z
        //  mov dword ptr[ecx+$24], eax
        //  mov eax, y
        //  mov dword ptr[ecx+$28], eax
        //  popad
        // end;
        //end;

        /// <summary> Движение по заданным координатам. Переделано krukovis. ru_off 1.4.5 Descent actual on 20.06.2012 </summary>
        /// <param name="sngx">координата X</param>
        /// <param name="sngy">координата Y</param>
        /// <param name="sngz">координата Z</param>
        /// <param name="mode">полет (1 полет, 0 пешком)</param>
        private void WalkTo(Single coord_X, Single coord_Y, Single coord_Z, int go_mode)
        {
            int intProcID = this.ProcessID;
            int Address1 = 0x0494620;
            int Address2 = 0x0498290;
            int Address3 = 0x0494EC0;

            asm.Pushad();
            asm.Mov_EAX(BaseAddress);
            asm.Mov_EAX_DWORD_Ptr_EAX();
            asm.Mov_EAX_DWORD_Ptr_EAX_Add(0x1c);
            asm.Mov_ESI_DWORD_Ptr_EAX_Add(0x34);
            asm.Mov_ECX_DWORD_Ptr_ESI_Add(0x10bc);
            asm.Push68(1);
            asm.Mov_EDX(Address1);
            asm.Call_EDX();

            asm.Mov_EDI_EAX();
            asm.Lea_EAX_DWORD_Ptr_ESP_Add(0x18);
            asm.Push_EAX();
            asm.Mov_EDX(go_mode);
            asm.Push_EDX();
            asm.Mov_ECX_EDI();
            asm.Mov_EDX(Address2);
            asm.Call_EDX();

            asm.Mov_ECX_DWORD_Ptr_ESI_Add(0x10bc);
            asm.Push68(0);
            asm.Push68(1);
            asm.Push_EDI();
            asm.Push68(1);
            asm.Mov_EDX(Address3);
            asm.Call_EDX();

            asm.Mov_EAX(BaseAddress);
            asm.Mov_EAX_DWORD_Ptr_EAX();
            asm.Mov_EAX_DWORD_Ptr_EAX_Add(0x1c);
            asm.Mov_EAX_DWORD_Ptr_EAX_Add(0x34);
            asm.Mov_EAX_DWORD_Ptr_EAX_Add(0x10bc);
            asm.Mov_EAX_DWORD_Ptr_EAX_Add(0x30);
            asm.Mov_ECX_DWORD_Ptr_EAX_Add(0x4);
            asm.Mov_EAX(coord_X);
            asm.Mov_EDX_ECX();
            asm.Add_EDX(0x20);
            asm.Mov_DWORD_Ptr_EDX_EAX();
            asm.Mov_EAX(coord_Z);
            asm.Mov_EDX_ECX();
            asm.Add_EDX(0x24);
            asm.Mov_DWORD_Ptr_EDX_EAX();
            asm.Mov_EAX(coord_Y);
            asm.Mov_EDX_ECX();
            asm.Add_EDX(0x28);
            asm.Mov_DWORD_Ptr_EDX_EAX();
            asm.Mov_EDX(0);
            asm.Popad();
            asm.Ret();
            asm.RunAsm(intProcID, 0);
        }

       //Подключение к процессу
        private void button1_Click(object sender, EventArgs e)
        {
            //запоминаем какое окно выбрано
            var window = cClients.SelectedItem as ClientWindow;

             //Получаем дескриптор процесса, выбранного клиента PW и открываем память для чтения / записи
            
            if (window == null)
            {
                return;
            }

            //Если выбрано - подключаемся к процессу игры запоминая ID процесса

            if (mem == null || mem.Process.Id != window.ProcessId) 
            {
                if (mem != null)
                mem.Process.Dispose();
                mem = new ProcessMemory(window.Process);
                ProcessID = mem.Process.Id;

                this.Text = "Подключено к " + window.Name;
                btnConnect.Text = "Connected to" + window.Name;
                btnConnect.Enabled = false;
                cClients.Enabled = false;
            }
        }

        //При клике по выпадающему списку в него грузятся все найденные окна игры
        private void cClients_DropDown(object sender, EventArgs e)
        {
                      cClients.Items.Clear();
            //находятся все окна и если возможно - грузится имя персонажа, если нет - название окна. 
            foreach (ClientWindow cw in ClientFinder.GetWindowsNamedPrior())
            {
                cClients.Items.Add(cw);
            }
        }

        
        private void cClients_DropDownClosed(object sender, EventArgs e)
        {
            //запоминаем какое окно выбрано
            var window = cClients.SelectedItem as ClientWindow;

            //Получаем дескриптор процесса, выбранного клиента PW и открываем память для чтения / записи
            //Если окно не выбрано:
            if (window == null)
            {
                //блокируем кнопку подключения
                btnConnect.Enabled = false;
            }
            else
            {
                //разблокируем
                btnConnect.Enabled = true;
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
        
        Single X = Convert.ToSingle(txtX.Text);
        X = (X-400)*10; //переводим в игровые координаты
        Single Y = Convert.ToSingle(txtY.Text);
        Y = (Y-550)*10; //переводим в игровые координаты
        Single Z = Convert.ToSingle(txtZ.Text);
        Z = Z*10; //переводим в игровые координаты

        //Идем по координатам:
        WalkTo(X, Y, Z, 0);
        }

         }
}
