namespace Repository.Mono.Sqlite
{
    public class MonoSqliteScheduleRepositoryFactory : IScheduleRepositoryFactory
    {
        private SqLiteDatabase sqldb;

        public IScheduleRepository CreateScheduleRepository()
        {
            sqldb = new SqLiteDatabase("schedule.db"); //TODO fix + check if file exists or make file.
            return new MonoSqliteScheduleRepository(sqldb);
        }
    }
}