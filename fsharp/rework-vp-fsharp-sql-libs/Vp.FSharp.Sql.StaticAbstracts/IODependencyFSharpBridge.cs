using System.Data;
using System.Data.Common;

namespace Vp.FSharp.Sql.StaticAbstracts;

public class SqlDependencyFSharpBridge<TConnection, TCommand, TDataReader, TTransaction,
    TSqlDependencies>
    where TConnection : DbConnection
    where TCommand : DbCommand
    where TDataReader : DbDataReader
    where TTransaction : DbTransaction
    where TSqlDependencies : IIODependencies<TConnection, TCommand, TDataReader, TTransaction>
{
    public static TCommand CreateCommand(TConnection connection) =>
        TSqlDependencies.CreateCommand(connection);
    public static void SetCommandTransaction(TCommand command, TTransaction transaction) =>
        TSqlDependencies.SetCommandTransaction(command, transaction);
    public static void BeginTransaction(TConnection connection, IsolationLevel isolationLevel) =>
        TSqlDependencies.BeginTransaction(connection, isolationLevel);
    public static ValueTask<TTransaction> BeginTransactionTask(TConnection connection, IsolationLevel isolationLevel, CancellationToken cancellationToken) =>
        TSqlDependencies.BeginTransactionTask(connection, isolationLevel, cancellationToken);
    public static TDataReader ExecuteReader(TCommand command) =>
        TSqlDependencies.ExecuteReader(command);
    public static Task<TDataReader> ExecuteReaderTask(TCommand command, CancellationToken cancellationToken) =>
        TSqlDependencies.ExecuteReaderTask(command, cancellationToken);
}
