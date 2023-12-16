using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.AccessControl;

//new Files(100, 10000);

//Console.WriteLine(Files.MergeFiles("FFF"));

//Files.SendFileToDB(1);

//Files.SendAllFileToDB();

while (Menu() != 1); // вызов меню бесконечным циклом пока клиент не решит выйти

static int Choiсe()
{
    Console.Write("Choiсe: ");
    return int.Parse(Console.ReadLine());
}  // Ввод выбора
static int Menu()
{
    Console.Clear();
    Console.WriteLine("**************************");
    Console.WriteLine("* 1) merge Files         *");
    Console.WriteLine("* 2) import in DB        *");
    Console.WriteLine("* 3) calc sum and median *");
    Console.WriteLine("* 0) exit                *");
    Console.WriteLine("**************************");

    switch (Choiсe())
    {
        case 1:
            Console.WriteLine();
            Console.Write("Enter str for delete: ");
            Console.WriteLine("amount of deleted strings: " + Files.MergeFiles(Console.ReadLine()));
            Console.WriteLine("Press enter");
            Console.ReadLine();
            return 0;

        case 2:
            Console.Clear();
            Files.SendAllFileToDB();
            return 0;

        case 3:
            double sum = 0, median = 0;
            DBcontext.SummAndMedian(ref sum,ref median);
            return 0;
        case 0:
            return 1;
        default:
            Console.WriteLine("ERR");
            return 0;
    }
} // Вывод вариантов выбора и обработка выбоа

/// <summary>
/// Класс для работы со строкой
/// </summary>
class Stroka
{
    private DateTime date;
    private String eng;
    private String rus;
    private int numb;
    private double numb2;

    /// <summary>
    /// Геттеры 
    /// </summary>
    /// <returns> Возвращает значение приватного поля </returns>
    #region Get
    public DateTime GetDate()
    {
        return date;
    }

    public String GetEng()
    { return eng; }

    public String GetRus()
    { return rus; }

    public int GetNumb()
    { return numb; }

    public double GetNumb2()
    { return numb2; }

    #endregion 

    /// <summary>
    /// Функция генерация рандомной строки
    /// </summary>
    /// <param name="str"> Строка из символов которые могут составлять случайную строку </param>
    /// <returns> Случайная строка состоящаяя из символов строки str </returns>
    private static char[] RandChar(string str)
    {
        Random random = new();
        char[] randomChars = new char[10];

        for (int i = 0; i < 10; i++)
        {
            randomChars[i] = str[random.Next(str.Length)];
        }

        return randomChars;
    }

    /// <summary>
    /// Конструктор который инициализирует класс нашей строки случайными значениями
    /// </summary>
    public Stroka()
    {
        Random random = new();
        date = DateTime.Now.AddDays(-1*random.Next(1, 1826));

        eng = new String(RandChar("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));
        rus = new string(RandChar("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя"));

        do
        {
            numb = random.Next(2, 100000000);
        } while (numb % 2 != 0);

        numb2 = (double)random.Next(1, 20) + random.Next(10000000, 99999999)/(double)100000000;
    }

    /// <summary>
    /// Создаём объект строки файла на основе считанной строки из файла
    /// </summary>
    /// <param name="Str"> Строка определённого формата </param>
    public Stroka(string Str)
    {
        string[] parts = Str.Split("||", StringSplitOptions.None);

        if (parts.Length == 6)
        {
            if (DateTime.TryParse(parts[0], out DateTime parsedDate))
            {
                date = parsedDate;
            }

            eng = parts[1];
            rus = parts[2];

            if (int.TryParse(parts[3], out int parsedInt))
            {
                numb = parsedInt;
            }

            if (double.TryParse(parts[4], out double parsedDouble))
            {
                numb2 = parsedDouble;
            }
        }
        else
        {
            Console.WriteLine(Str + parts.Length); 
        }
    }


    /// <summary>
    /// перегрузка метода для конвертации нашего класса в строку
    /// </summary>
    /// <returns></returns>
    public override string? ToString()
    {
        return date.ToShortDateString() + "||" + eng + "||" + rus + "||" + numb + "||" + Math.Round(numb2,8) + "||";
    }
}

/// <summary>
/// Работа с файлами
/// </summary>
class Files
{
    /// <summary>
    /// Количество файлов в выбранной папке
    /// </summary>
    public static int count;

    /// <summary>
    /// Путь к папке где будут храниться все файлы
    /// </summary>
    private static readonly string folderPath = "D:\\test\\";

    /// <summary>
    /// Список файлов (Каждый элемент хранит все элементы файла)
    /// </summary>
    private static List<List<Stroka>> stroks = ImportAllFiles();

    /// <summary>
    /// Генерация файлов
    /// </summary>
    /// <param name="amountFiles"> Количество генерируемых файлов </param>
    /// <param name="amountStrings"> Количество случайных строк в каждом файле </param>
    public Files(int amountFiles, int amountStrings)
    {
        int a;
        if ((a = AmountOfFiles()) > 0)
            count = a;
        else
            count = 0;

        for (int i = 0; i < amountFiles; i++)
        {
            count++;

            using StreamWriter writer = new(folderPath + count + ".txt");

            for (int j = 0; j < amountStrings; j++)
            {
                writer.WriteLine(new Stroka());
            }
        }
    }


    /// <summary>
    /// Узнать количество файлов в папке
    /// </summary>
    /// <returns> Количество файлов в папке </returns>
    public static int AmountOfFiles()
    {
        string extension = ".txt";

        if (Directory.Exists(folderPath))
        {
            int fileCount = Directory.GetFiles(folderPath, "*" + extension).Length;
            return --fileCount;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// Достать из файла все строки
    /// </summary>
    /// <param name="fileNumber"> Номер файла </param>
    /// <returns> Список записей файла </returns>
    static public List<Stroka>? GetStroks(int fileNumber)
    {
        List<Stroka> stroki = new();

        string filePath = folderPath + fileNumber + ".txt";

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    stroki.Add(new Stroka(line));
                }
            }

            return stroki;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Выгрузить все записи из всех файлов
    /// </summary>
    /// <returns> Список файлов (списков записей) </returns>
    static public List<List<Stroka>> ImportAllFiles()
    {
        int a;
        if ((a = AmountOfFiles()) > 0)
            count = a;
        else
            count = 0;

        List<List<Stroka>> stroki = new();

        for (int i = 1; i <= count; i++) 
        {
            stroki.Add(GetStroks(i));

        }
        
        return stroki;
    }

    /// <summary>
    /// очистить папку
    /// </summary>
    static public void DeleteAllFiles()
    {
        int a;
        if ((a = AmountOfFiles()) > 0)
            count = a;
        else
            count = 0;
        for (int i = 1; i <= a;i++)
        {
            File.Delete(folderPath + i + ".txt");
        }
    }

    /// <summary>
    /// Создать файл и записать в него все записи из списка
    /// </summary>
    /// <param name="stroki"> список записей </param>
    /// <param name="FileNumber"> номер файла (название) </param>
    static public void WriteOnFile(List<Stroka> stroki, int FileNumber)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(folderPath + FileNumber + ".txt", true))
            {
                foreach (var item in stroki)
                {
                    writer.WriteLine(item.ToString());
                }

            }
        }
        catch (IOException e)
        {
            Console.WriteLine($"Err: {e.Message}");
        }
    }

    /// <summary>
    /// Перегрузка для случая когда файл называн не цифрой
    /// </summary>
    /// <param name="stroki"> список записей </param>
    /// <param name="FileName"> название нового файла </param>
    static public void WriteOnFile(List<Stroka> stroki, String FileName)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(folderPath + FileName + ".txt", true))
            {
                foreach (var item in stroki)
                {
                    writer.WriteLine(item.ToString());
                }

            }
        }
        catch (IOException e)
        {
            Console.WriteLine($"Err: {e.Message}");
        }
    }

    /// <summary>
    /// Удаление записей содержащих подстроку из всех файлов
    /// </summary>
    /// <param name="str"> искомая подстрока </param>
    /// <returns> Количество удалённых записей </returns>
    static public int RemooveFromFiles(String str)
    {
        int value = 0, f;

        foreach (var s in stroks)
        {
            f = s.Count;
            s.RemoveAll(s => (s.ToString().Contains(str)));
            value += f - s.Count;
        }

        DeleteAllFiles();

        int i = 1;

        foreach(var s in stroks)
        {
            WriteOnFile(s, i);
            i++;
        }

        return value;
    }

    /// <summary>
    /// Объединить файлы в один, удалив зиписи с подстрокой
    /// </summary>
    /// <param name="deleteStr"> Подстрока по которой выбираются записи для удаления </param>
    /// <returns> Количество удалённых записей </returns>
    static public int MergeFiles(String deleteStr)
    {
        int a = RemooveFromFiles(deleteStr);

        File.WriteAllText(folderPath + "Merge.txt", string.Empty);

        foreach (var s in stroks)
        {
            WriteOnFile(s, "Merge");
        }


        return a;
    }

    /// <summary>
    /// Отправка определённого файла в дб
    /// </summary>
    /// <param name="fileNumber"></param>
    static public void SendFileToDB(int fileNumber) 
    {
        var stroki = GetStroks(fileNumber);

        int i = 0;
        int a = stroki.Count();

        foreach (var item in stroki)
        {
            DBcontext.SendStrokToBD(item);
            i++;
            if (i%3 == 0)
                Console.Write($"\r Импортировано {i} строк из {a}. (файл {fileNumber}) ");
        }
    }

    /// <summary>
    /// Отправка всех файлов в бд
    /// </summary>
    static public void SendAllFileToDB()
    {
        int a;
        if ((a = AmountOfFiles()) > 0)
            count = a;
        else
            count = 0;

        for(int i = 1; i <= count; i++)
        {
            Console.WriteLine($"\rИмпортировано {i} файлов из {count}. ");
            SendFileToDB(i);
            Console.Clear();
        }    
    }
}

/// <summary>
/// Класс для работы с БД (с использованием библиотеки ADO.NET 
/// (ADO.NET тк он позволяет отслеживать процесс на более низком уровне))
/// </summary>
class DBcontext
{
    /// <summary>
    /// Строка подключения к базе данных
    /// </summary>
    private readonly static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True";

    /// <summary>
    /// Отправить запись в базу данных
    /// </summary>
    /// <param name="stroka"> отправляемая запись </param>
    public static void SendStrokToBD(Stroka stroka)
    {
        string insertQuery = "INSERT INTO Stroki (RandomDate,LatinString, CyrillicString, IntegerNumber,DoubleNumber ) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Value1", stroka.GetDate());
                command.Parameters.AddWithValue("@Value2", stroka.GetEng());
                command.Parameters.AddWithValue("@Value3", stroka.GetRus());
                command.Parameters.AddWithValue("@Value4", stroka.GetNumb());
                command.Parameters.AddWithValue("@Value5", stroka.GetNumb2());


                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Вызов хранимой в бд процедуры для подсчёта суммы и медианы чисел
    /// </summary>
    /// <param name="sum"> значение передаётся по ссылке, после отработки функции в переменной будет лежать сумма элементов </param>
    /// <param name="median"> аналогично, как и для суммы, только медиана </param>
    public static void SummAndMedian(ref double sum, ref double median)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand("CalculateSumAndMedian", connection);
            command.CommandType = CommandType.StoredProcedure;

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sum = (double)reader["SumOfIntegers"];
                        median = (double)reader["MedianOfDoubles"];
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Ошибка SQL: " + ex.Message);
            }
        }
    }
}


