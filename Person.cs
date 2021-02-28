using System;

namespace DbDemo1
{
	/// <summary>
	/// Person model.
	/// </summary>
	public class Person
	{
		#region Public Properties

		public string FullName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public char Sex { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Calculates the age of this person.
		/// </summary>
		public int GetAge()
		{
			DateTime now = DateTime.Today;
			int age = now.Year - DateOfBirth.Year;

			if (now < DateOfBirth.AddYears(age))
			{
				age--;
			}

			return age;
		}

		#endregion
	}
}
