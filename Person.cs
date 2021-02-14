using System;

namespace DbDemo1
{
	/// <summary>
	/// Person model.
	/// </summary>
	public class Person
	{
		public string FullName { get; set; }
		public DateTime DateOfBirth { get; set; }
		public char Sex { get; set; }
		public int Age
		{
			get
			{
				DateTime now = DateTime.Today;
				int age = now.Year - DateOfBirth.Year;

				if (now < DateOfBirth.AddYears(age))
				{
					age--;
				}

				return age;
			}
		}

	}
}
