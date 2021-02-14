using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using static System.Console;

namespace DbDemo1
{
	public class Operations
	{
		#region Private Members

		private readonly SqlServerOperations _sqlServerOperations;
		private readonly Random _rnd = new();
		private readonly List<char> _alphabetLowercase = new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', };
		private readonly DateTime _oldestDateOfBirth = new(1921, 1, 1);
		private readonly DateTime _newestDateOfBirth = new(2011, 1, 1);
		private readonly int _daysBetweenOldestAndNewestDate;

		#endregion

		#region Constructor

		public Operations(string conStr)
		{
			_sqlServerOperations = new SqlServerOperations(conStr);
			_daysBetweenOldestAndNewestDate = (_newestDateOfBirth - _oldestDateOfBirth).Days;
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
					   dateOfBirth >= _oldestDateOfBirth && dateOfBirth <= _newestDateOfBirth;
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
				_sqlServerOperations.CreateRandomEntries(GetRandomEntries(1000000));
				_sqlServerOperations.CreateRandomEntries(GetRandomEntries(100, fullNameFirstLetter: 'F', sex: 'M'));

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
				WriteLine($"Age          : {people[i].Age}");
			}
		}

		#endregion

		#region Private Helpers

		// No documentation due to lack of time );

		private Person[] GetRandomEntries(int amount)
		{
			var result = new Person[amount];

			for (int i = 0; i < amount; i++)
			{
				result[i] = GetRandomPerson();
			}

			return result;
		}

		private Person[] GetRandomEntries(int amount, char fullNameFirstLetter, char sex)
		{
			var result = new Person[amount];

			for (int i = 0; i < amount; i++)
			{
				result[i] = GetRandomPerson(fullNameFirstLetter, sex);
			}

			return result;
		}

		private Person GetRandomPerson()
		{
			return new Person()
			{
				FullName = GetRandomFullName(100),
				DateOfBirth = GetRandomDateOfBirth(),
				Sex = _rnd.Next(0, 2) == 0 ? 'M' : 'F'
			};
		}

		private Person GetRandomPerson(char fullNameFirstLetter, char sex)
		{
			return new Person()
			{
				FullName = GetRandomFullName(100, fullNameFirstLetter),
				DateOfBirth = GetRandomDateOfBirth(),
				Sex = sex
			};
		}

		private string GetRandomFullName(int maxLength)
		{
			StringBuilder sb = new();

			// Generate a full name consisting of 3 parts.

			// First part leaves 2 spaces and 5+5 minimum for 2nd and 3rd parts.
			var firstPartLength = _rnd.Next(5, maxLength - 12 + 1);
			// Second part leaves 2 spaces and 5 minimum for 3rd part.
			var secondPartLength = _rnd.Next(5, maxLength - firstPartLength - 7 + 1);
			var thirdPartLength = _rnd.Next(5, maxLength - firstPartLength - secondPartLength - 2);

			AddNamePart(ref sb, firstPartLength);
			sb.Append(' ');
			AddNamePart(ref sb, secondPartLength);
			sb.Append(' ');
			AddNamePart(ref sb, thirdPartLength);

			return sb.ToString();
		}
		private string GetRandomFullName(int maxLength, char fullNameFirstLetter)
		{
			StringBuilder sb = new();

			// Generate a full name consisting of 3 parts.

			// First part leaves 2 spaces and 5+5 minimum for 2nd and 3rd parts.
			var firstPartLength = _rnd.Next(5, maxLength - 12 + 1);
			// Second part leaves 2 spaces and 5 minimum for 3rd part.
			var secondPartLength = _rnd.Next(5, maxLength - firstPartLength - 7 + 1);
			var thirdPartLength = _rnd.Next(5, maxLength - firstPartLength - secondPartLength - 2);

			AddNamePart(ref sb, firstPartLength, fullNameFirstLetter);
			sb.Append(' ');
			AddNamePart(ref sb, secondPartLength);
			sb.Append(' ');
			AddNamePart(ref sb, thirdPartLength);

			return sb.ToString();
		}

		private void AddNamePart(ref StringBuilder sb, int namePartLength)
		{
			char current;

			for (int i = 0; i < namePartLength; i++)
			{
				current = _alphabetLowercase[_rnd.Next(0, _alphabetLowercase.Count)];

				if (i == 0)
				{
					current = char.ToUpper(current);
				}

				sb.Append(current);
			}
		}

		private void AddNamePart(ref StringBuilder sb, int namePartLength, char firstLetter)
		{
			char current;

			for (int i = 0; i < namePartLength; i++)
			{

				if (i == 0)
				{
					current = char.ToUpper(firstLetter);
				}
				else
				{
					current = _alphabetLowercase[_rnd.Next(0, _alphabetLowercase.Count)];
				}

				sb.Append(current);
			}
		}

		private DateTime GetRandomDateOfBirth()
		{
			return _oldestDateOfBirth.AddDays(_rnd.Next(_daysBetweenOldestAndNewestDate + 1));
		}

		#endregion
	}
}
