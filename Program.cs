using System.Text;
using System.Text.Unicode;

namespace ForGreatGit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите ваше имя?: ");
            var userName = Console.ReadLine();
            Console.WriteLine(userName);
            PrintLeader();

            var num = GenNum();

            var attempts = 0;

            while (true)
            {
                Console.Write("Введите ваше предположение: ");
                var guess = Console.ReadLine();

                if (!int.TryParse(guess, out var guessNum))
                {
                    Console.WriteLine("Некорректный ввод, используйте только цифры.");
                    continue;
                }

                if (guessNum.ToString().Length != 4)
                {
                    Console.WriteLine("Число должно состоять из 4 цифр");
                    continue;
                }

                var guessDig = guessNum.ToString().ToCharArray();
                if (guessDig.Distinct().Count() != 4)
                {
                    Console.WriteLine("Число не должно содержать повторяющиеся цифры.");
                    continue;
                }

                var result = CompareNumbers(num, guessNum);

                Console.WriteLine($"Вы угадали {result.GuessedDigits} цифр, из них {result.GuessedInPlace} на своем месте.");

                if (result.GuessedInPlace == 4)
                {
                    Console.WriteLine("Поздравляем Вы угадали число!");
                    attempts++;
                    SaveResult(userName, attempts);
                    var place = GetPlace(userName, attempts);
                    Console.WriteLine($"Поздравляем, ваше место - {place}!");
                    return; 
                    
                }

                attempts++;
            }
        }

        static int GenNum()
        {
            var random = new Random();
            var number = 0;

            while (true)
            {
                number = random.Next(1000, 10000);
                var digits = number.ToString().ToCharArray();
                if (digits.Length == 4 && digits.Distinct().Count() == 4)
                {
                    break;
                }
            }

            return number;
        }

        static (int GuessedDigits, int GuessedInPlace) CompareNumbers(int number, int guess)
        {
            var numberDig = number.ToString().ToCharArray();
            var guessDig = guess.ToString().ToCharArray();

            var guessedDig = 0;
            var guessedInPlace = 0;

            for (var i = 0; i < 4; i++)
            {
                if (guessDig[i] == numberDig[i])
                {
                    guessedInPlace++;
                    guessedDig++;
                }
                else if (numberDig.Contains(guessDig[i]))
                {
                    guessedDig++;
                }
            }

            return (guessedDig, guessedInPlace);
        }

        static void SaveResult(string userName, int attempts)
        {
            var filePath = "results.txt";
            var result = $"{userName} - {attempts} попыток";

            if (!File.Exists(filePath))
            {
                File.Create(filePath);//.Close();
            }

            File.AppendAllText(filePath, result + Environment.NewLine, Encoding.GetEncoding("Windows-1251"));

            Console.WriteLine($"Вы прошли за {attempts} попыток"); 
            Console.WriteLine("Результат сохранен в файле results.txt");
        }

        static void PrintLeader()
        {
            var filePath = "results.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Лидерборд пуст.");
                return;
            }

            var results = File.ReadAllLines(filePath);
            var sortedResults = results.OrderBy(x => int.Parse(x.Split('-')[1].Trim().Split(' ')[0])).ToArray();

            Console.WriteLine("Доска почета:");
            for (var i = 0; i < sortedResults.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {sortedResults[i]}");
            }
        }

        static int GetPlace(string userName, int attempts)
        {
            var filePath = "results.txt";
            if (!File.Exists(filePath))
            {
                return 1;
            }

            var results = File.ReadAllLines(filePath);
            var sortedResults = results.OrderBy(x => int.Parse(x.Split('-')[1].Trim().Split(' ')[0])).ToArray();

            for (var i = 0; i < sortedResults.Length; i++)
            {
                if (sortedResults[i].StartsWith(userName))
                {
                    return i + 1;
                }
            }

            return sortedResults.Length + 1;
        }

    }
}