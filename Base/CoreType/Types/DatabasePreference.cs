namespace CoreType.Types
{
    public enum DatabasePreference
    {
        Default, //Read from config
        GenesisDB,
        WorkflowDB,
        PostgreSQL,
        MySQL,
        MSSQL,
        Oracle
    }
}