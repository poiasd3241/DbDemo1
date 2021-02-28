using System;
using System.Data.SqlClient;
using System.Diagnostics;
using static System.Console;

namespace DbDemo1
{
	public class Operations
	{
		#region Private Members

		private readonly SqlServerOperations _sqlServerOperations;
		private readonly OperationsHelpers _helpers;

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="conStr">The connecting string to use.</param>
		public Operations(string conStr)
		{
			_sqlServerOperations = new(conStr);
			_helpers = new();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a table.
		/// </summary>
		public void CreateTable() => TryBase(() =>
		{
			_sqlServerOperations.CreateTable();
			WriteLine($"Create table: SUCCESS.");
		}, "Create Table");

		/// <summary>
		/// Takes in and validates user input for a new entry, then creates the entry.
		/// </summary>
		public void CreateEntry()
		{
			string fullName = GetValidEntryPart("Enter the full name (5-100 characters long) and press Enter.",
				(input) => input.Length >= 5 && input.Length <= 100,
				(input) => input);

			DateTime dateOfBirth = GetValidEntryPart("Enter the date of birth (between 1921-01-01 and 2011-01-01) and press Enter.",
				(input) =>
				{
					return DateTime.TryParse(input, out DateTime dateOfBirth) &&
					   dateOfBirth >= _helpers.OldestDateOfBirth &&
					   dateOfBirth <= _helpers.NewestDateOfBirth;
				},
				(input) => DateTime.Parse(input));

			char sex = GetValidEntryPart("Enter the sex (one letter, M or F) and press Enter.",
				(input) =>
				{
					return char.TryParse(input, out char sexInput) &&
					sexInput == 'F' || sexInput == 'M';
				},
				(input) => char.Parse(input));

			// Input valid.

			TryBase(() =>
			{
				_sqlServerOperations.CreateEntry(new Person { FullName = fullName, DateOfBirth = dateOfBirth, Sex = sex });
				WriteLine($"Create entry: SUCCESS.");
			}, "Create entry");
		}

		/// <summary>
		/// Gets entries with unique FullName and DateOfBirth.<br/>
		/// Displays up to 1000 entries.
		/// </summary>
		public void GetUniqueFullNameAndDateOfBirthEntries()
		{
			TryBase(() =>
			{
				var result = _sqlServerOperations.GetUniqueFullNameAndDateOfBirthEntries();

				WriteLine($"Get unique entries: SUCCESS.");
				WriteLine();

				if (result.Length == 0)
				{
					WriteLine($"No unique entries, the table is empty.");
				}
				else
				{
					WriteLine($"{result.Length} unique entries.");

					DisplayEntries(result);
				}
			}, "Get unique entries");
		}

		/// <summary>
		/// Creates 1000000 random entries.
		/// Then creates 100 random entries with Sex = 'M' and FullName[0] = 'F'.
		/// </summary>
		public void CreateRandomEntries()
		{
			TryBase(() =>
			{
				_sqlServerOperations.CreateRandomEntries(_helpers.GetRandomEntries(1000000));
				_sqlServerOperations.CreateRandomEntries(_helpers.GetRandomEntries(100, fullNameFirstLetter: 'F', sex: 'M'));

				WriteLine($"Create random entries: SUCCESS.");
			}, "Get unique entries");
		}

		/// <summary>
		/// Gets all entries with Sex = 'M' and FullName[0] = 'F'.
		/// Measures and displays the execution time.
		/// Displays up to 1000 entries.
		/// </summary>
		public void GetMaleAndFullNameSpecificEntries()
		{
			TryBase(() =>
			{
				Stopwatch sw = new();
				sw.Start();
				var result = _sqlServerOperations.GetMaleAndFullNameSpecificEntries();
				sw.Stop();

				WriteLine($"Get unique entries: SUCCESS.");
				WriteLine($"Completed in {sw.ElapsedMilliseconds} ms.");

				WriteLine();

				if (result.Length == 0)
				{
					WriteLine($"No male entries with full name starting with 'F'.");
				}
				else
				{
					WriteLine($"{result.Length} male entries with full name starting with 'F'.");

					DisplayEntries(result);
				}
			}, "Get male entries with full name starting with 'F'");
		}

		/// <summary>
		/// Creates nonclustered index on FullName and Sex columns and includes DateOfBirth column.
		/// </summary>
		public void IndexFullNameAndSexColumns()
		{
			TryBase(() =>
			{
				_sqlServerOperations.IndexFullNameAndSexColumns();

				WriteLine($"Index FullName and Sex columns: SUCCESS.");
			}, "Index FullName and Sex columns (include DateOfBirth)");
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Simple base method for handling errors for other methods.
		/// </summary>
		/// <param name="executeInsideTryBlock"></param>
		/// <param name="operationName"></param>
		private static void TryBase(Action executeInsideTryBlock, string operationName)
		{
			try
			{
				executeInsideTryBlock();
			}
			catch (SqlException ex)
			{
				WriteLine($"{operationName}: SQL ERROR: {ex.Message}");
			}
			catch (Exception ex)
			{
				WriteLine($"{operationName}: Non-SQL ERROR: {ex.Message}");
			}
		}

		/// <summary>
		/// Used for convenient validation of user input in the console.
		/// </summary>
		/// <typeparam name="T">The type of the needed input entry part.</typeparam>
		/// <param name="entryPartInputTip">Tip to display to the user regarding current entry part and its validation requirements.</param>
		/// <param name="validCondition">Returns <see langword="true"/> when the input string is valid for conversion to the needed entry part;
		/// otherwise, <see langword="false"/>.</param>
		/// <param name="conversion">Defines the logic to convert from string input to the needed entry part type.</param>
		/// <returns></returns>
		private static T GetValidEntryPart<T>(string entryPartInputTip, Func<string, bool> validCondition,
			Func<string, T> conversion)
		{
			WriteLine(entryPartInputTip);
			int inputLength;

			while (true)
			{
				var cursorTop = CursorTop;

				var entryPartInputString = ReadLine();
				inputLength = entryPartInputString.Length;

				entryPartInputString = entryPartInputString.Trim();

				if (validCondition(entryPartInputString))
				{
					return conversion(entryPartInputString);
				}
				else
				{
					// Undo any user input.
					SetCursorPosition(0, cursorTop);
					Write(new string(' ', inputLength));
					SetCursorPosition(0, cursorTop);
				}
			}
		}

		/// <summary>
		/// Displays up to 1000 entries. 
		/// Implemented for better performance and to avoid the console buffer size limitations.
		/// </summary>
		/// <param name="people">Entries array to display.</param>
		private static void DisplayEntries(Person[] people)
		{
			if (people.Length > 1000)
			{
				WriteLine($"Only the top 1000 will be displayed.");
			}
			WriteLine();

			WriteLine("Press enter to display entries.");
			ReadLine();

			var iterations = Math.Min(people.Length, 1000);

			for (int i = 0; i < iterations; i++)
			{
				WriteLine($"##################");
				WriteLine($"Full name    : {people[i].FullName}");
				WriteLine($"Date of birth: {people[i].DateOfBirth:yyyy'-'MM'-'dd}");
				WriteLine($"Sex          : {people[i].Sex}");
				WriteLine($"Age          : {people[i].GetAge()}");
			}
		}

		#endregion
	}
}
