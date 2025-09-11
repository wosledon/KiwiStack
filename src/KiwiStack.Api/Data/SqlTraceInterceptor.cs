using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KiwiStack.Api.Services.Data;

public class SqlTraceInterceptor : DbCommandInterceptor
{
    private readonly Action<string> _logger;
    private readonly int _slowThresholdMs;

    public SqlTraceInterceptor(Action<string> logger, int slowThresholdMs = 500)
    {
        _logger = logger;
        _slowThresholdMs = slowThresholdMs;
    }

    private void LogCommand(DbCommand command, long elapsedMs = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("----- SQL Command -----");
        sb.AppendLine("SQL: " + command.CommandText);
        if (command.Parameters.Count > 0)
        {
            sb.AppendLine("Parameters:");
            foreach (DbParameter param in command.Parameters)
            {
                sb.AppendLine($"  {param.ParameterName} = {param.Value} ({param.DbType})");
            }
        }
        if (elapsedMs > 0)
        {
            sb.AppendLine($"Elapsed: {elapsedMs}ms");
            if (elapsedMs >= _slowThresholdMs)
                sb.AppendLine("!!! SLOW QUERY !!!");
        }
        sb.AppendLine("-----------------------");
        _logger(sb.ToString());
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        var sw = Stopwatch.StartNew();
        var res = base.ReaderExecuting(command, eventData, result);
        sw.Stop();
        LogCommand(command, sw.ElapsedMilliseconds);
        return res;
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        var sw = Stopwatch.StartNew();
        var res = base.NonQueryExecuting(command, eventData, result);
        sw.Stop();
        LogCommand(command, sw.ElapsedMilliseconds);
        return res;
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        var sw = Stopwatch.StartNew();
        var res = base.ScalarExecuting(command, eventData, result);
        sw.Stop();
        LogCommand(command, sw.ElapsedMilliseconds);
        return res;
    }
}
