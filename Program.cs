using System.Collections.Generic;
using static System.Console;

namespace DbDemo1
{
	class Program
	{
		static void Main()
		{
			var conStr = "Server=DESKTOP-95EH9IE;Database=Db1;Trusted_Connection=True";

			Operations operations = new(conStr);

			List<string> operationTypes = new(6) { "1", "2", "3", "4", "5", "6" };

			while (true)
			{
				WriteLine("1. Create table.");
				WriteLine("2. Create entry.");
				WriteLine("3. Get entries with unique [full name and date of birth].");
				WriteLine("4. Create 1000000 [random] + 100 [random, male and full name starts with 'F'] entries.");
				WriteLine("5. Get [male and full name starts with 'F'] entries.");
				WriteLine("6. Improve execution time of operation #5.");
				WriteLine();
				WriteLine("Enter the number of the required operation and press Enter.");

				var cursorTop = CursorTop;

				string input;

				while (true)
				{
					input = ReadLine();
					if (operationTypes.Contains(input))
					{
						// Valid operation number.
						break;
					}
					else
					{
						// Invalid operation number.
						SetCursorPosition(0, cursorTop);
						Write(new string(' ', input.Length));
						SetCursorPosition(0, cursorTop);
					}
				}

				switch (input)
				{
					case "1":
						operations.CreateTable();
						break;
					case "2":
						operations.CreateEntry();
						break;
					case "3":
						operations.GetUniqueFullNameAndDateOfBirthEntries();
						break;
					case "4":
						operations.CreateRandomEntries();
						break;
					case "5":
						operations.GetMaleAndFullNameSpecificEntries();
						break;
					case "6":
						operations.IndexFullNameAndSexColumns();
						break;
				}

				// Hold operation result...
				WriteLine();
				WriteLine("Press Enter to go back to operations selection.");
				ReadLine();
				Clear();
			}
		}
	}
}
