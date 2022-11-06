using System.Data;
using System.Data.SQLite;

using Vp.FSharp.Sql.StaticAbstracts;

namespace Vp.FSharp.Sql.Sqlite.StaticAbstracts;


public class SqliteIODependencies : IIODependencies<SQLiteConnection, SQLiteCommand, SQLiteDataReader, SQLiteTransaction>
{
    public static SQLiteCommand CreateCommand(
        SQLiteConnection connection) =>
        connection.CreateCommand();

    public static void SetCommandTransaction(
        SQLiteCommand command, SQLiteTransaction transaction) =>
        command.Transaction = transaction;

    public static SQLiteTransaction BeginTransaction(
        SQLiteConnection connection, IsolationLevel isolationLevel) =>
        connection.BeginTransaction(isolationLevel);

    public static ValueTask<SQLiteTransaction> BeginTransactionTask(
        SQLiteConnection connection, IsolationLevel isolationLevel, CancellationToken cancellationToken) =>
        ValueTask.FromResult(connection.BeginTransaction(isolationLevel));

    public static SQLiteDataReader ExecuteReader(
        SQLiteCommand command) =>
        command.ExecuteReader();

    public static Task<SQLiteDataReader> ExecuteReaderTask(
        SQLiteCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(command.ExecuteReader());
}
