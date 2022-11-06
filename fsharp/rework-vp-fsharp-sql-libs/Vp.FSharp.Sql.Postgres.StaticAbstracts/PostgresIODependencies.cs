using System.Data;

using Npgsql;

using Vp.FSharp.Sql.StaticAbstracts;

namespace Vp.FSharp.Sql.Postgres.StaticAbstracts;


public class PostgresIODependencies : IIODependencies<NpgsqlConnection, NpgsqlCommand, NpgsqlDataReader, NpgsqlTransaction>
{
    public static NpgsqlCommand CreateCommand(
        NpgsqlConnection connection) =>
        connection.CreateCommand();

    public static void SetCommandTransaction(
        NpgsqlCommand command, NpgsqlTransaction transaction) =>
        command.Transaction = transaction;

    public static NpgsqlTransaction BeginTransaction(
        NpgsqlConnection connection, IsolationLevel isolationLevel) =>
        connection.BeginTransaction(isolationLevel);

    public static ValueTask<NpgsqlTransaction> BeginTransactionTask(
        NpgsqlConnection connection, IsolationLevel isolationLevel, CancellationToken cancellationToken) =>
        connection.BeginTransactionAsync(isolationLevel, cancellationToken);

    public static NpgsqlDataReader ExecuteReader(
        NpgsqlCommand command) =>
        command.ExecuteReader();

    public static Task<NpgsqlDataReader> ExecuteReaderTask(
        NpgsqlCommand command, CancellationToken cancellationToken) =>
        command.ExecuteReaderAsync(cancellationToken);
}
