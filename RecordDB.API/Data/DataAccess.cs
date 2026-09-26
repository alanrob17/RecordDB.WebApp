using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace RecordDB.API.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;

        public DataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString() =>
            _configuration.GetConnectionString("RecordDb")
                ?? throw new InvalidOperationException("Connection string 'RecordDb' is not configured.");

        // -----------------------------------------------------------------------
        // Query methods — return IEnumerable<T>
        // -----------------------------------------------------------------------

        /// <summary>
        /// Executes a stored procedure and returns a collection of <typeparamref name="T"/>.
        /// Pass <see cref="CommandType.Text"/> to execute a raw SQL query instead.
        /// </summary>
        public async Task<IEnumerable<T>> GetData<T, P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            return await connection.QueryAsync<T>(sql, parameters, commandType: commandType);
        }

        /// <summary>
        /// Executes a stored procedure using Dapper multi-mapping to combine two entity types into one.
        /// </summary>
        public async Task<IEnumerable<TReturn>> GetData<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object parameters,
            string splitOn = "Id",
            CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            return await connection.QueryAsync<TFirst, TSecond, TReturn>(
                sql,
                map,
                parameters,
                splitOn: splitOn,
                commandType: commandType);
        }

        // -----------------------------------------------------------------------
        // Single-row methods — return T (or default)
        // -----------------------------------------------------------------------

        /// <summary>
        /// Executes a stored procedure and returns the first result, or default if none.
        /// Pass <see cref="CommandType.Text"/> to execute a raw SQL query instead.
        /// </summary>
        public async Task<T?> GetFirstOrDefault<T, P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: commandType);
        }

        // -----------------------------------------------------------------------
        // Scalar methods — return a single value
        // -----------------------------------------------------------------------

        /// <summary>
        /// Executes a stored procedure or query and returns a single scalar value of <typeparamref name="TResult"/>.
        /// Pass <see cref="CommandType.Text"/> to execute a raw SQL query instead.
        /// </summary>
        public async Task<TResult?> GetScalar<TResult, P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            return await connection.ExecuteScalarAsync<TResult>(sql, parameters, commandType: commandType);
        }

        // -----------------------------------------------------------------------
        // Execute methods — no return value
        // -----------------------------------------------------------------------

        /// <summary>
        /// Executes a stored procedure or query with no return value.
        /// Pass <see cref="CommandType.Text"/> to execute a raw SQL query instead.
        /// </summary>
        public async Task SaveData<P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            await connection.ExecuteAsync(sql, parameters, commandType: commandType);
        }

        /// <summary>
        /// Executes a stored procedure and reads back an int OUTPUT parameter.
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure to execute.</param>
        /// <param name="parameters">The <see cref="DynamicParameters"/> containing input and output params.</param>
        /// <param name="outputParameterName">The name of the OUTPUT parameter to read (default: <c>@Result</c>).</param>
        public async Task<int> SaveDataReturnId(string storedProcedure, DynamicParameters parameters, string outputParameterName = "@Result")
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>(outputParameterName);
        }
    }
}
