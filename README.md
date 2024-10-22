<h1 align="center" height="128">Code for lab on TiOPO</h1>

        static int GenNum()
        {
            Random random = new Random();
            int number = 0;
            while (true)
            {
                number = random.Next(1000, 10000);
                char[] digits = number.ToString().ToCharArray();
                if (digits.Length == 4 && digits.Distinct().Count() == 4)
                {
                    break;
                }
            }
            return number;
        }


Here my menu: 
