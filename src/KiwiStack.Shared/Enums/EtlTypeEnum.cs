namespace KiwiStack.Shared.Enums;

public enum EtlTypeEnum
{
    /// <summary>
    /// Full data migration
    /// </summary>
    Full = 0,
    /// <summary>
    /// Incremental data migration
    /// </summary>
    Incremental = 1,

    /// <summary>
    /// sql script based migration
    /// </summary>
    Sql = 2
}
