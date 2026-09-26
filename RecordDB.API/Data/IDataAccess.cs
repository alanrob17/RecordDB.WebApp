using Dapper;
using System.Data;

namespace RecordDB.API.Data
{
    public interface IDataAccess
    {
        /// <summary>
        /// Executes a stored procedure (or raw SQL query) and returns a collection of <typeparamref name="T"/>.
        /// </summary>
        Task<IEnumerable<T>> GetData<T, P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure);

        /// <summary>
        /// Executes a stored procedure (or raw SQL query) using Dapper multi-mapping to combine two
        /// entity types (<typeparamref name="TFirst"/> and <typeparamref name="TSecond"/>) into a
        /// single return type (<typeparamref name="TReturn"/>).
        /// </summary>
        Task<IEnumerable<TReturn>> GetData<TFirst, TSecond, TReturn>(
            string sql,
            Func<TFirst, TSecond, TReturn> map,
            object parameters,
            string splitOn = "Id",
            CommandType commandType = CommandType.StoredProcedure);

        /// <summary>
        /// Executes a stored procedure (or raw SQL query) and returns the first result, or default if none.
        /// </summary>
        Task<T?> GetFirstOrDefault<T, P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure);

        /// <summary>
        /// Executes a stored procedure (or raw SQL query) and returns a single scalar value of <typeparamref name="TResult"/>.
        /// </summary>
        Task<TResult?> GetScalar<TResult, P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure);

        /// <summary>
        /// Executes a stored procedure (or raw SQL query) with no return value.
        /// </summary>
        Task SaveData<P>(string sql, P parameters, CommandType commandType = CommandType.StoredProcedure);

        /// <summary>
        /// Executes a stored procedure and reads back an int OUTPUT parameter.
        /// </summary>
        /// <param name="storedProcedure">The name of the stored procedure to execute.</param>
        /// <param name="parameters">The <see cref="DynamicParameters"/> containing input and output params.</param>
        /// <param name="outputParameterName">The name of the OUTPUT parameter to read (default: <c>@Result</c>).</param>
        Task<int> SaveDataReturnId(string storedProcedure, DynamicParameters parameters, string outputParameterName = "@Result");
    }
}
