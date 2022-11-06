using System.Data;
using System.Data.Common;

namespace Vp.FSharp.Sql.StaticAbstracts;

public interface IIODependencies<TConnection, TCommand, TDataReader, TTransaction>
    where TConnection : DbConnection
    where TCommand : DbCommand
    where TDataReader : DbDataReader
    where TTransaction : DbTransaction
{
    static abstract TCommand CreateCommand(
        TConnection connection);

    static abstract void SetCommandTransaction(
        TCommand command, TTransaction transaction);

    static abstract TTransaction BeginTransaction(
        TConnection connection, IsolationLevel isolationLevel);

    static abstract ValueTask<TTransaction> BeginTransactionTask(
        TConnection connection, IsolationLevel isolationLevel, CancellationToken cancellationToken);

    static abstract TDataReader ExecuteReader(
        TCommand command);

    static abstract Task<TDataReader> ExecuteReaderTask(
        TCommand command, CancellationToken cancellationToken);
}
