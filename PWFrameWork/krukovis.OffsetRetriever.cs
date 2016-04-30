using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace PWFrameWork
{

    /// <summary>
    /// Класс для поиска смещений в массиве byte. Массив может получаться как из файла, так и из динамической памяти.
    /// </summary>
    public class OffsetRetriever
    {

        /// <summary>
        /// Cтруктура для хранения шаблонов смещений
        /// </summary>
        public struct OffsetPattern
        {
            public string name;
            public byte[] prefix, suffix;
        }

        /// <summary>
        /// Структура для хранения шаблона адреса
        /// </summary>
        public struct AddressPattern
        {
            public string name;
            //public byte[] pattern;
            //список частей шаблонов
            public List<byte[]> pattern_as_list ;
        }

        //Лист для хранения шаблонов смещений
        private List<OffsetPattern> offset_patterns = new List<OffsetPattern>();
        //Лист для хранения названий шаблонов смещений
        private List<string> names = new List<string>();

        List<AddressPattern> address_patterns = new List<AddressPattern>();
        List<string> address_names = new List<string>();

        /// <summary>
        /// Переводит hex-строку в массив байт
        /// </summary>
        /// <param name="str">строка представляющая запись массива байт в hex</param>
        /// <returns></returns>
        static byte[] FromHexString(string str)
        {
            //Проверяем четно ли количество символов в строке, т.к. на каждый байт приходится 2 символа
            if (str.Length % 2 != 0)
                throw new ArgumentException("Not a hex string");
            //Переводим строку в верхний регистр. 
            str = str.ToUpper();
            //Создаем новый массив равный половине длинны строки
            byte[] result = new byte[str.Length / 2];
            //Запонялем массив значениями
            for (int i = 0; i < result.Length; ++i)
                result[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            //Возвращаем массив
            return result;
        }

        /// <summary>
        /// Добавляет шаблон смещения по имени и шаблону
        /// </summary>
        /// <param name="name">Имя смещения</param>
        /// <param name="pattern">Шаблон</param>
        /// <returns></returns>
        public OffsetRetriever AddOffsetPattern(string name, string pattern)
        {
            //Исключение - пустое имя
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException();
            //Исключение - Такое имя уже есть
            if (names.Contains(name))
                throw new ArgumentException("Duplicate name");
            //Разделяем шаблон на части по знаку ?
            string[] parts = pattern.Split('?');
            //Если длинна частей не равна 2 или нет префикса или суффикса
            if (parts.Length != 2 || parts[0].Length == 0 && parts[1].Length == 0)
                throw new ArgumentException("Pattern format:  \"HexPrefix?HexSuffix\"");

            OffsetPattern p = new OffsetPattern();
            p.name = name;
            p.prefix = FromHexString(parts[0]);
            p.suffix = FromHexString(parts[1]);
            //добавляем стуктуру шаблона в список
            offset_patterns.Add(p);
            //добавляем имя в список имен
            names.Add(name);
            return this;
        }


        /// <summary>
        /// Добавляет набор смещений в виде строки 
        /// </summary>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public OffsetRetriever AddOffsetPatterns(string patterns)
        {
            foreach (var pat in patterns.Split(','))
            {
                string[] v = pat.Split(':');
                if (v.Length != 2)
                    throw new ArgumentException("Pattern syntax: name:pattern");
                AddOffsetPattern(v[0].Trim(), v[1].Trim());
            }

            return this;
        }

        /// <summary>
        /// Возращает словарь смещений анализируя массив типа byte
        /// </summary>
        /// <param name="buffer">byte: Анализируемый массив типа byte, в котором нужно найти определенные значения</param>
        /// <returns></returns>
        public Dictionary<string, int> FindOffsets(byte[] buffer)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            //string fileName = file_name;

            //byte[] buffer;

            ////загружаем файл в массив байт
            //using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    buffer = new byte[fs.Length];
            //    fs.Read(buffer, 0, buffer.Length);
            //}

            //Преобразуем файл в массив байт
            //buffer = File.ReadAllBytes(file_name);


            //Ищем для каждого шаблона в списке шаблонов
            foreach (OffsetPattern p in offset_patterns)
            {
                //начинаем поиск с начала файла
                int pos = 0;

                //Если длинна префикса больше нуля
                if (p.prefix.Length > 0)
                {

                    //перебираем пока не дойдем до конца файла
                    while (pos < buffer.Length)
                    {
                        //находим ближайшее вхождение первого байта префикса, ищем от значения pos, а pos определяется как последнее найденное соответствие или 0
                        pos = Array.IndexOf(buffer, p.prefix[0], pos);
                        //Если не нашли вхождения или до конца файла уже не вместятся префикс, 4 байта значения и суффикс
                        if (pos == -1 || pos + p.prefix.Length + 4 + p.suffix.Length >= buffer.Length)
                        {
                            //устанавливаем позицию на конец файла - что по моему безсмысленно если вышли из перебора
                            //pos = buffer.Length;
                            //выходим из while
                            break;
                        }

                        //задаем условие нахождения - маркер f (find) 
                        bool f = true;
                        //Если дошлю до этого блока, значит нашли вхождение первого элемента префикса
                        //Начинаем перебирать байты префикса с первого (следующего за найденным в предыдущем блоке)
                        for (int i = 1; i < p.prefix.Length; ++i)
                        {
                            //Если следующий элемент в буфере не совпадает со следующим элементом префикса
                            if (buffer[pos + i] != p.prefix[i])
                            {
                                //говорим что не нашли
                                f = false;
                                //выходим из for
                                break;
                            }
                        }
                        //Если совпали все элементы (f не изменило своего значения)
                        if (f)
                        {
                            //перебираем элементы суффикса от первго
                            for (int i = 0; i < p.suffix.Length; ++i)
                            {
                                //начинаем сравнение с найденной позиции префикса, смещенной на длинну префикса и увеличенную на 4 байта самого значения
                                //Есил в какой то момент значения не совпали
                                if (buffer[pos + p.prefix.Length + 4 + i] != p.suffix[i])
                                {
                                    //говорим что не нашли
                                    f = false;
                                    //выходим из for
                                    break;
                                }
                            }
                        }

                        //Если совпали все элементы (f не изменило значение)
                        if (f)
                        {
                            //добавляем в словарь result значение 
                            result.Add(p.name, BitConverter.ToInt32(buffer, pos + p.prefix.Length));
                            //увеличиваем позицию на длинну префикса, значения и суффикса.
                            //pos += p.prefix.Length + 4 + p.suffix.Length;

                            //выходим из while
                            break;
                        }

                        //увеличиаем текущую позицию
                        ++pos;
                    }
                }
                else//если префикса нет
                {
                    //значит ? c начала шаблона и нужно отступить 4 байта на значение
                    pos += 4;
                    //делаем пока не дойдем до конца файла
                    while (pos < buffer.Length)
                    {
                        //находим вхождение первого элемента суффикса в массиве файла
                        pos = Array.IndexOf(buffer, p.suffix[0], pos);
                        //Если не нашли или суффикс уже не вмещается в оставшееся место в файле
                        if (pos == -1 || pos + p.suffix.Length >= buffer.Length)
                        {
                            //pos = buffer.Length;
                            //выходим из while
                            break;
                        }
                        //задаем маркер поиска f - find
                        bool f = true;
                        //сравниваем поэлементно буфер и суффикс
                        for (int i = 1; i < p.suffix.Length; ++i)
                            if (buffer[pos + i] != p.suffix[i])
                            {
                                //если в какой то момент не буфер с суффикстом не совпали
                                //говорим что не нашли
                                f = false;
                                //выходим из for
                                break;
                            }
                        //Если дошли до сюда и f - осталось в true значит нашли полное совпадение
                        if (f)
                        {
                            //сохраняем результат 
                            result.Add(p.name, BitConverter.ToInt32(buffer, pos - 4));
                            //pos += p.suffix.Length;
                            //выходим из while
                            break;
                        }
                        //если не вышли из while - увеличиваем позицию на один - берем следующий элемент в буфере
                        ++pos;
                    }
                }
            }
            //возвращаем результаты
            return result;
        }

        /// <summary>
        /// Добавляет шаблон поиска функции в список
        /// </summary>
        /// <param name="name">string: Имя шаблона</param>
        /// <param name="pattern">string: Шаблон функции (пропускаемые байты нужно заменять на FF)</param>
        /// <returns></returns>
        public OffsetRetriever AddAddressPattern(string name, string pattern)
        {

            //Исключение - пустое имя - не задано имя
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException();
            //Исключение - Такое имя уже есть
            if (names.Contains(name))
                throw new ArgumentException("Duplicate name");

            //объявляем структуру 
            AddressPattern ap = new AddressPattern();
            //заполняем значения
            ap.name = name;
            //ap.pattern = FromHexString(pattern);

            //Разделяем шаблон на части по знаку ?
            string[] parts = pattern.Split('?');

            //для каждой из частей шаблона
            foreach (var part in parts)
            {
                //переводим часть шаблона в массив байт и добавляем в лист шаблона
                if (part.Length % 2 != 0)
                    throw new ArgumentException("Длинна части опкода не кратна 2");
                if (part.Length == 0)
                    throw new ArgumentException("Шаблон не должен начинаться или заканчиваться на знак разделителя ?, и знаки разделителя не должны идти друг за другом");
                ap.pattern_as_list = new List<byte[]>();
                ap.pattern_as_list.Add(FromHexString(part));
            }

            //добавляем стуктуру шаблона в список
            address_patterns.Add(ap);
            //добавляем имя в список имен
            address_names.Add(name);
            return this;
        }

        /// <summary>
        /// Добавляет шаблоны поиска функций в список. 
        /// </summary>
        /// <param name="patterns">Шаблон представляет собой строку, которая начинается и заканчивается на опкод и разделена знаками ? - которыми заменяют части опкода длинной 4 байта (адреса)</param>
        /// <returns></returns>
        public OffsetRetriever AddAddressPatterns(string patterns)
        {

            foreach (var pat in patterns.Split(','))
            {
                string[] v = pat.Split(':');
                if (v.Length != 2)
                    throw new ArgumentException("Pattern syntax: name:pattern");
                AddOffsetPattern(v[0].Trim(), v[1].Trim());
            }

            return this;

        }


        /// <summary>
        /// Возращает словарь имен функций и адреса их опкодов анализируя массив типа byte
        /// </summary>
        /// <param name="file_name">string: Имя файла</param>
        /// <returns></returns>
        public Dictionary<string, int> FindAddress(byte[] buffer)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            //Преобразуем файл в массив байт
            //byte[] buffer = File.ReadAllBytes(file_name);

            foreach (AddressPattern ap in address_patterns)
            {

                //Задаем точку отсчета
                int pos = 0;
                //определяем общую длинну шаблона
                int full_pattern_lenght = 0;

                //для каждой части шаблона в общем шаблоне
                foreach (byte[] part_of_pattern in ap.pattern_as_list)
                {
                    //вычисляем длинну и добавляем ее к полной длинне шаблона
                    full_pattern_lenght = full_pattern_lenght + part_of_pattern.Length;
                }
                //а так же добавляем длинну пропущенных адресов - длинна каждого из адресов  = 4 и их на 1 меньше чем частей, т.к. между 2мя частями может быть только 1 разделитель и т.д.
                full_pattern_lenght = full_pattern_lenght + (ap.pattern_as_list.Count - 1) * 4;

                //пока не дошли до конца файла
                while (pos < buffer.Length)
                {
                    //находим ближайшее вхождение первого байта первого паттерна, 
                    //продолжаем поиск от значения pos, а pos определяется как последнее найденное соответствие увеличенное на 1 или 0
                    pos = Array.IndexOf(buffer, ap.pattern_as_list[0][0], pos);

                    //Если не нашли вхождения или до конца файла уже не вместится паттерн
                    if (pos == -1 || pos + full_pattern_lenght >= buffer.Length)
                    {
                        //устанавливаем позицию на конец файла 
                        pos = buffer.Length;
                        //выходим из перебора
                        break;
                    }
                    //пока не убедились в обратном думаем что это искомый адрес
                    int address = pos;

                    //задаем условие нахождения - маркер f (find) 
                    bool f = true;

                    //для каждой части шаблона в шаблоне
                    foreach (byte[] part_of_pattern in ap.pattern_as_list)
                    {

                        //Начинаем перебирать байты очередного шаблона
                        for (int i = 0; i < part_of_pattern.Length; ++i)
                        {
                            //Пропускаем FF-ы при сравнении. 
                            //if (ap.pattern[i] == 0xff)
                            //    continue;

                            //Если следующий элемент в буфере не совпадает со следующим элементом части
                            if (buffer[pos + i] != part_of_pattern[i])
                            {
                                //говорим что не нашли
                                f = false;
                                //выходим из for - прекращаем дальнейший перебор
                                break;
                            }
                        }
                        //если f не изменило значение - значит эта часть шаблона совпала
                        if (f)
                        {
                            //значит - смещаем проверяемую позицию на длинну проверенной части шаблона и увеличиваем на длинну разделителя (адреса)
                            pos = pos + part_of_pattern.Length + 4;
                        }
                    }

                    //Если совпали все элементы (f не изменило значение)
                    if (f)
                    {
                        //добавляем в словарь result значение 
                        result.Add(ap.name, address + 0x400000);
                        //уходим в конец файла
                        pos += buffer.Length;
                        break;
                    }
                    else//если на каком то этапе перестало сопадать - смещаемся на один байт от последнего найденного вхождения и анализируем дальше
                    {
                        //увеличиаем текущую позицию и продолжаем анализ буфера
                        pos = address + 1;
                    }
                }
            }

            //спискок результатов
            return result;
        }

    }
}
