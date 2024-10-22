using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Security;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Unicode;

namespace ForGreatGit
{
    class Program
    {
        static void Main(string[] args)
        {
            firstMenu(); 
        }

        static void firstMenu()
        {
            Console.WriteLine("Добро пожаловать в игру, аналогичную worldly - numbly! \n" +
            "1. Авторизироваться?\n" +
            "2. Зарегистрироваться?\n" + 
            "3. Выход"
            );
            int input = Convert.ToInt32(Console.ReadLine()); 

            switch(input)
            {
                case 1: 
                    Auth(); 
                    break; 
                case 2: 
                    Registration(); 
                    break; 
                case 3: 
                    Environment.Exit(0); 
                    break; 
            }


        }

        static void Auth()
        {
            string login; 
            string password;
            string pathToUserData = "users.txt"; 

            if (!File.Exists(pathToUserData))
            {
                Console.WriteLine("Такого пользователя не существует, зарегистрируйтесь!");
                Registration(); 
            }

            while (true)
            {
                Console.Write("Введите ваш логин: "); 
                login = Console.ReadLine(); 

                var users = File.ReadAllLines(pathToUserData);
                bool userExists = false;
                string userPassword = "";

                foreach (var user in users)
                {
                    string userLogin = user.Split(':')[0];
                    if (userLogin == login)
                    {
                        userPassword = user.Split(':')[1];
                        userExists = true;
                        break;
                    }
                }

                if (!userExists)
                {
                    Console.WriteLine("Такого пользователя не существует, зарегистрируйтесь!");
                    firstMenu();
                }

                while (true)
                {
                    Console.Write("Ввведите ваш пароль: "); 
                    password = Console.ReadLine(); 

                    if (password == userPassword)
                    {
                        Console.WriteLine("Авторизация успешна, вход в программу");
                        GameMenu(login, password); 
                    }
                    else
                    {
                        Console.WriteLine("Неправильный пароль, попробуйте еще раз.");
                    }
                }
            }
        }
        static void Registration()
        {
            string login; 
            string password;
            string pathToUserData = "users.txt"; 

            if (!File.Exists(pathToUserData))
            {
                File.Create(pathToUserData).Dispose();
            }

            while (true)
            {
                Console.Write("Введите ваш логин: "); 
                login = Console.ReadLine(); 

                var users = File.ReadAllLines(pathToUserData);
                bool userExists = false;

                foreach (var user in users)
                {
                    string userLogin = user.Split(':')[0];
                    if (userLogin == login)
                    {
                        userExists = true;
                        break;
                    }
                }

                if (userExists)
                {
                    Console.WriteLine("Такой пользователь уже существует, попробуйте другой логин.");
                }
                else
                {
                    Console.Write("Введите ваш пароль: "); 
                    password = Console.ReadLine(); 

                    File.AppendAllText(pathToUserData, $"{login}:{password}" + Environment.NewLine);
                    Console.WriteLine("Регистрация успешна, теперь вы можете авторизоваться.");
                    firstMenu();
                    
                }
            }
        }

        static void GameMenu(string login, string password)
        {
            Console.WriteLine($"Добро пожаловать, {login}\n" + 
            "1. Начать игру\n" + 
            "2. Ваши игры\n" + 
            "3. Таблица лидеров\n" + 
            "4. Выход"
            ); 

            int input = Convert.ToInt32(Console.ReadLine()); 

            switch(input)
            {
                case 1: 
                    Game(login, password); 
                    break; 
                case 2: 
                    PrintPlayerResults(login, password); 
                    break; 
                case 3: 
                    printLeaderBord(login, password); 
                    break; 
                case 4: 
                    Environment.Exit(0); 
                    break; 
            }

            
        }

        static void Game(string login, string password)
        {

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
                    SaveResult(login, attempts);
                    GetPlace(login);
                    GameMenu(login, password); 
                    
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

        static void SaveResult(string login, int attempts)
        {
            var resultPath = "results.txt";
            var result = $"{login}:{attempts}";

            if (!File.Exists(resultPath))
            {
                File.Create(resultPath);
            }

            string[] existingResults = File.ReadAllLines(resultPath); 
            List<string> updatedResults = new List<string>(); 

            bool existingResultsFound = false; 

            foreach (string existingResult in existingResults)
            {
                if (existingResult.StartsWith(login))
                {
                    string[] existingAttempts = existingResult.Split(':');
                    string newAttempts = $"{existingAttempts[1].Trim()}|{attempts}";
                    updatedResults.Add($"{login}:{newAttempts}");
                    existingResultsFound = true;
                }
                else
                {
                    updatedResults.Add(existingResult);
                }
            }

             if (!existingResultsFound)
            {
                updatedResults.Add(result);
            }


            File.WriteAllText(resultPath, string.Join(Environment.NewLine, updatedResults) + Environment.NewLine);

            Console.WriteLine($"Вы прошли за {attempts} попыток"); 
            
        }

        static List<(string, int)> makeLeaderBord()
        {
            string resultPath = "results.txt"; 

            if (!File.Exists(resultPath))
            {
                Console.WriteLine("Файл результатов не существует.");
                return new List<(string, int)>();
            }

            string[] results = File.ReadAllLines(resultPath); 
            Dictionary<string, int> playerResults = new Dictionary<string, int>(); 

            foreach (string result in results) 
            {
                string[] parts = result.Split(':');
                string player = parts[0];
                string[] attempts = parts[1].Split('|');

                int bestResult = 1000000;

                foreach (string attempt in attempts)
                {
                    int attemptValue = int.Parse(attempt.Trim());
                    if (attemptValue < bestResult)
                    {
                        bestResult = attemptValue;
                    }
                }
                playerResults[player] = bestResult;

                
            }

            List<(string, int)> sortedPlayerResults = playerResults.OrderBy(x => x.Value).Select(x => (x.Key, x.Value)).ToList();
            return sortedPlayerResults; 
                
            
        }

        static void printLeaderBord(string login, string password)
        {
            List<(string, int)> sortedPlayerResults = makeLeaderBord();
            if (sortedPlayerResults.Count == 0) 
            {
                Console.WriteLine("Лидерборд пуст");
            }
            else 
            {
                Console.WriteLine("Лидерборд:");
                int place = 1; 
                foreach ((string, int) playerResult in sortedPlayerResults)
                {
                    Console.WriteLine($"{place}.{playerResult.Item1} победил за {playerResult.Item2} ходов!");
                    place++; 
                }
            }
            
            GameMenu(login, password);
        }

        static void GetPlace(string login)
        {
            List<(string, int)> sortedPlayerResults = makeLeaderBord(); 
            Console.WriteLine("Лидерборд:");
            int place = 1; 
            foreach ((string, int) playerResult in sortedPlayerResults)
            {
               if (playerResult.Item1 == login) 
               {
                Console.WriteLine($"Ваше место в лидерборде: {place}"); 
                return; 
               }
               place++; 
            }
            Console.WriteLine("Вы не найдены в лидерборде.");
        }

        static void PrintPlayerResults(string login, string password)
        {
            string resultPath = "results.txt";

            if (!File.Exists(resultPath))
            {
                Console.WriteLine("Файл результатов не существует.");
                return;
            }

            string[] results = File.ReadAllLines(resultPath);

            foreach (string result in results)
            {
                string[] parts = result.Split(':');
                string player = parts[0];

                if (player == login)
                {
                    string[] attempts = parts[1].Split('|');
                    Console.WriteLine("Результаты игрока:");
                    for (int i = 0; i < attempts.Length; i++)
                    {
                        Console.WriteLine($"Игра {i + 1}: {attempts[i]} попыток");
                        Console.WriteLine("\n"); 
                    }
                    GameMenu(login, password);
                }
            }

            Console.WriteLine("Игрок не найден в файлах результатов.");
        }
    }
}