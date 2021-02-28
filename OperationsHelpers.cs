using System;
using System.Collections.Generic;
using System.Text;

namespace DbDemo1
{
	public class OperationsHelpers
	{
		#region Private Members

		private readonly Random _rnd = new();
		private readonly List<char> _alphabetLowercase = new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', };
		private readonly DateTime _oldestDateOfBirth = new(1921, 1, 1);
		private readonly DateTime _newestDateOfBirth = new(2011, 1, 1);
		private readonly int _daysBetweenOldestAndNewestDate;

		#endregion

		#region Public Properties

		public DateTime OldestDateOfBirth => _oldestDateOfBirth;
		public DateTime NewestDateOfBirth => _newestDateOfBirth;

		#endregion

		#region Constructor

		public OperationsHelpers()
		{
			_daysBetweenOldestAndNewestDate = (_newestDateOfBirth - _oldestDateOfBirth).Days;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates a random <see cref="Person"/> objects (entries).
		/// </summary>
		/// <param name="amount">The amount of entries to generate.</param>
		public Person[] GetRandomEntries(int amount)
		{
			var result = new Person[amount];

			for (int i = 0; i < amount; i++)
			{
				result[i] = GetRandomPerson();
			}

			return result;
		}

		/// <summary>
		/// Generates a random <see cref="Person"/> objects (entries).
		/// </summary>
		/// <param name="amount">The amount of entries to generate.</param>
		/// <param name="fullNameFirstLetter">The character that the full name of the generated entries has to begin with.</param>
		/// <param name="sex">The sex that the generated entries have to have.</param>
		/// <returns></returns>
		public Person[] GetRandomEntries(int amount, char fullNameFirstLetter, char sex)
		{
			var result = new Person[amount];

			for (int i = 0; i < amount; i++)
			{
				result[i] = GetRandomPerson(fullNameFirstLetter, sex);
			}

			return result;
		}

		/// <summary>
		/// Generates a random <see cref="Person"/> object (entry).
		/// </summary>
		public Person GetRandomPerson()
		{
			return new Person()
			{
				FullName = GetRandomFullName(100),
				DateOfBirth = GetRandomDateOfBirth(),
				Sex = _rnd.Next(0, 2) == 0 ? 'M' : 'F'
			};
		}

		/// <summary>
		/// Generates a random <see cref="Person"/> object (entry).
		/// </summary>
		/// <param name="fullNameFirstLetter">The character that the full name of the generated entry has to begin with.</param>
		/// <param name="sex">The sex that the generated entry has to have.</param>
		public Person GetRandomPerson(char fullNameFirstLetter, char sex)
		{
			return new Person()
			{
				FullName = GetRandomFullName(100, fullNameFirstLetter),
				DateOfBirth = GetRandomDateOfBirth(),
				Sex = sex
			};
		}

		/// <summary>
		/// Generates a random full name consisting of 3 parts.
		/// </summary>
		/// <param name="maxLength">The maximum length of the generated full name.</param>
		public string GetRandomFullName(int maxLength)
		{
			StringBuilder sb = new();

			// Each part of the full name should be at least 5 characters long.

			// First part leaves 2 spaces between parts and 5+5 minimum for 2nd and 3rd parts.
			var firstPartLength = _rnd.Next(5, maxLength - 12 + 1);
			// Second part leaves 2 spaces between parts and 5 minimum for 3rd part.
			var secondPartLength = _rnd.Next(5, maxLength - (firstPartLength + 7) + 1);
			var thirdPartLength = _rnd.Next(5, maxLength - (firstPartLength + secondPartLength + 2) + 1);

			AddNamePart(sb, firstPartLength);
			sb.Append(' ');
			AddNamePart(sb, secondPartLength);
			sb.Append(' ');
			AddNamePart(sb, thirdPartLength);

			return sb.ToString();
		}

		/// <summary>
		/// Generates random full name consisting of 3 parts.
		/// </summary>
		/// <param name="maxLength">The maximum length of the generated full name.</param>
		/// <param name="fullNameFirstLetter">The character that the full name has to begin with.</param>
		public string GetRandomFullName(int maxLength, char fullNameFirstLetter)
		{
			StringBuilder sb = new();

			// Generate a full name consisting of 3 parts.

			// First part leaves 2 spaces between parts and 5+5 minimum for 2nd and 3rd parts.
			var firstPartLength = _rnd.Next(5, maxLength - 12 + 1);
			// Second part leaves 2 spaces between parts and 5 minimum for 3rd part.
			var secondPartLength = _rnd.Next(5, maxLength - (firstPartLength + 7) + 1);
			var thirdPartLength = _rnd.Next(5, maxLength - (firstPartLength + secondPartLength + 2) + 1);

			AddNamePart(sb, firstPartLength, fullNameFirstLetter);
			sb.Append(' ');
			AddNamePart(sb, secondPartLength);
			sb.Append(' ');
			AddNamePart(sb, thirdPartLength);

			return sb.ToString();
		}

		/// <summary>
		/// Generates a random name part to the provided <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> to append the generated name part to.</param>
		/// <param name="namePartLength">The length of the generated name part.</param>
		public void AddNamePart(StringBuilder sb, int namePartLength)
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

		/// <summary>
		/// Generates a random name part to the provided <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="sb">The <see cref="StringBuilder"/> to append the generated name part to.</param>
		/// <param name="namePartLength">The length of the generated name part.</param>
		/// <param name="firstCharacter">The character that the name part has to begin with.</param>
		public void AddNamePart(StringBuilder sb, int namePartLength, char firstCharacter)
		{
			char current = char.ToUpper(firstCharacter);
			sb.Append(current);

			for (int i = 1; i < namePartLength; i++)
			{
				current = _alphabetLowercase[_rnd.Next(0, _alphabetLowercase.Count)];
				sb.Append(current);
			}
		}

		/// <summary>
		/// Generates a random date of birth in this app's specific valid date of birth range.
		/// </summary>
		public DateTime GetRandomDateOfBirth()
		{
			return _oldestDateOfBirth.AddDays(_rnd.Next(_daysBetweenOldestAndNewestDate + 1));
		}

		#endregion
	}
}
