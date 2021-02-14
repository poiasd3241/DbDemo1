using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DbDemo1
{
	public class SqlServerOperations
	{
		#region Private Members

		private readonly string _conStr;

		#endregion

		#region Constructor

		public SqlServerOperations(string conStr)
		{
			_conStr = conStr;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Runs the SQL script that creates a table.
		/// </summary>
		public void CreateTable()
		{
			var sql = @"
				CREATE TABLE [dbo].[Person] (
					[ID] INT IDENTITY (1, 1) NOT NULL,
					[FullName] NVARCHAR (100) NOT NULL,
					[DateOfBirth] DATETIME2 (0) NOT NULL,
					[Sex] CHAR (1) NOT NULL,
					PRIMARY KEY CLUSTERED ([ID] ASC)
				)
				";

			using IDbConnection connection = new SqlConnection(_conStr);
			connection.ExecuteScalar(sql);
		}

		/// <summary>
		/// Runs the SQL script that creates an entry.
		/// </summary>
		/// <param name="person">Entry details to use for creation.</param>
		public void CreateEntry(Person person)
		{
			var sql = @"
				INSERT INTO dbo.Person (FullName, DateOfBirth, Sex) 
					Values (@FullName, @DateOfBirth, @Sex)
				";

			using IDbConnection connection = new SqlConnection(_conStr);
			connection.Execute(sql, person);
		}

		/// <summary>
		/// Runs the SQL script that gets entries with unique FullName and DateOfBirth.
		/// </summary>
		public Person[] GetUniqueFullNameAndDateOfBirthEntries()
		{
			var sql = @"
				WITH cte AS
				(
					SELECT MIN(ID) as minID
						FROM dbo.Person
						GROUP BY FullName, DateOfBirth
				)
				SELECT p.FullName, p.DateOfBirth, p.Sex
					FROM  dbo.Person p
						INNER JOIN cte
						on p.ID = cte.minID
				";

			using IDbConnection connection = new SqlConnection(_conStr);
			var entries = connection.Query<Person>(sql).ToArray();
			return entries;
		}

		/// <summary>
		/// Runs the SQL script that creates 1000000 random entries and 
		/// then 100 random entries with Sex = 'M' and FullName[0] = 'F'.
		/// </summary>
		/// <param name="people">Random data to use for new entries.</param>
		public void CreateRandomEntries(Person[] people)
		{
			var sql = @"
				INSERT INTO dbo.Person (FullName, DateOfBirth, Sex) 
					Values (@FullName, @DateOfBirth, @Sex)
				";

			using IDbConnection connection = new SqlConnection(_conStr);
			connection.Execute(sql, people);
		}

		/// <summary>
		/// Runs the SQL script that gets all entries with Sex = 'M' and FullName[0] = 'F'.
		/// </summary>
		public Person[] GetMaleAndFullNameSpecificEntries()
		{
			var sql = @"
				SELECT FullName, DateOfBirth, Sex
					FROM dbo.Person
					WHERE FullName LIKE 'F%'
					AND Sex = 'M'
				";

			using IDbConnection connection = new SqlConnection(_conStr);
			var entries = connection.Query<Person>(sql).ToArray();
			return entries;
		}

		/// <summary>
		/// Runs the SQL script that creates nonclustered index
		/// on FullName and Sex columns and includes DateOfBirth column
		/// for faster <see cref="GetMaleAndFullNameSpecificEntries"/> execution.
		/// </summary>
		public void IndexFullNameAndSexColumns()
		{
			// From MS docs: A SQL Server index is an on-disk or in-memory structure
			// associated with a table or view that speeds retrieval of rows from the table or view.

			// Personal test: 1000 consequent GetMaleAndFullNameSpecificEntries calls
			// (removed individual call execution measurement and display of success/entries,
			// only query execution was left).
			//
			// (1000100 entries in the table, 19544 of which are matched by GetMaleAndFullNameSpecificEntries)
			//
			// Without index: 85.3 s 
			// With index   : 34.4 s   

			var sql = @"
				CREATE NONCLUSTERED INDEX IX_FullName_Sex_INCLUDE_DateOfBirth
					ON dbo.Person(FullName, Sex)
					INCLUDE (DateOfBirth)
				";

			using IDbConnection connection = new SqlConnection(_conStr);
			connection.ExecuteScalar(sql);
		}

		#endregion
	}
}
