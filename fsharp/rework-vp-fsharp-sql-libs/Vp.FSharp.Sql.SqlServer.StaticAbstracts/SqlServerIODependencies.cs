using System.Data;
using Microsoft.Data.SqlClient;
using Vp.FSharp.Sql.StaticAbstracts;

namespace Vp.FSharp.Sql.SqlServer.StaticAbstracts;


public class SqlServerIODependencies : IIODependencies<SqlConnection, SqlCommand, SqlDataReader, SqlTransaction>
{
    public static SqlCommand CreateCommand(
        SqlConnection connection) =>
        connection.CreateCommand();

    public static void SetCommandTransaction(
        SqlCommand command, SqlTransaction transaction) =>
        command.Transaction = transaction;

    public static SqlTransaction BeginTransaction(
        SqlConnection connection, IsolationLevel isolationLevel) =>
        connection.BeginTransaction(isolationLevel);

    public static ValueTask<SqlTransaction> BeginTransactionTask(
        SqlConnection connection, IsolationLevel isolationLevel, CancellationToken cancellationToken) =>
        ValueTask.FromResult(connection.BeginTransaction(isolationLevel));

    public static SqlDataReader ExecuteReader(
        SqlCommand command) =>
        command.ExecuteReader();

    public static Task<SqlDataReader> ExecuteReaderTask(
        SqlCommand command, CancellationToken cancellationToken) =>
        command.ExecuteReaderAsync(cancellationToken);
}
