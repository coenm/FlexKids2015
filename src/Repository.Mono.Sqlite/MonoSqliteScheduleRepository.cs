using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Repository.Model;

namespace Repository.Mono.Sqlite
{
    public class MonoSqliteScheduleRepository : IScheduleRepository
    {
        private readonly SqLiteDatabase sqlitedb;
        private const string SqliteDateFormat = "yyyy-MM-dd HH:mm:ss";

        public MonoSqliteScheduleRepository(SqLiteDatabase sqlitedb)
        {
            this.sqlitedb = sqlitedb;
            Init();
        }

        public Week Update(Week originalWeek, Week updatedWeek)
        {
            const string sql = "UPDATE week set year = {1}, week = {2}, hash = '{3}' where id = {0}";
            String sql2 = String.Format(sql, updatedWeek.Id, updatedWeek.Year, updatedWeek.WeekNr, updatedWeek.Hash);

            var updatedRows = sqlitedb.ExecuteNonQuery(sql2);
            if(updatedRows == 1)
                return GetWeek(updatedWeek.Year, updatedWeek.WeekNr);

            return null;
        }

        public Week GetWeek(int year, int weekNr)
        {
            string sql = String.Format("SELECT * FROM week WHERE year = {0} and week = {1}", year, weekNr);
            return GetWeek(sql);
        }

        public Week GetWeek(int weekId)
        {
            string sql = String.Format("SELECT * FROM week WHERE id = {0}", weekId );
            return GetWeek(sql);
        }



        public IList<Schedule> GetSchedules(int year, int weekNr)
        {
           // string sql = "SELECT * FROM week WHERE year = {0} AND week = {1}";
            //string sql = "SELECT * FROM schedule WHERE year = '{0}' and week = '{1}'";
            //string sql = "SELECT w.* FROM schedule s INNER JOIN week w ON s.week_id = w.id WHERE w.year = {0} and w.week = {1}";
            string sql = "SELECT s.start, s.end, s.location FROM schedule s INNER JOIN week w ON s.week_id = w.id WHERE w.year = {0} and w.week = {1}";
            sql = "SELECT s.* FROM schedule s INNER JOIN week w ON s.week_id = w.id WHERE w.year = {0} and w.week = {1}";
            var sql2 = String.Format(sql, year, weekNr);

            var week = GetWeek(year, weekNr);
            if (week == null)
                return null;

            return GetSchedule(sql2, week);
        }

        
        public IList<Schedule> GetSchedules(DateTime @from, DateTime until)
        {
            throw new NotImplementedException();
        }

        public Schedule GetSchedule(int id)
        {
            string weekSql = String.Format("SELECT w.* FROM week w INNER JOIN schedule s ON s.week_id = w.id WHERE s.id = {0}", id);
            var week = GetWeek(weekSql);
            if (week == null)
                return null;

            const string sqlFixed = "SELECT s.* FROM schedule s INNER JOIN week w ON s.week_id = w.id WHERE s.id = {0}";
            var sql = String.Format(sqlFixed, id);
            var schedule = GetSchedule(sql, week);
            return schedule.First();
        }

        public Schedule Insert(Schedule schedule)
        {
            const string sqlFixed = "insert into schedule (week_id, location, start, end, last_update) values({0}, '{1}', '{2}', '{3}', CURRENT_TIMESTAMP)";
            var result = sqlitedb.ExecuteNonQuery(String.Format(sqlFixed, schedule.WeekId, schedule.Location.Replace("'", "''"), schedule.StartDateTime.ToString(SqliteDateFormat), schedule.EndDateTime.ToString(SqliteDateFormat)));

//            SQLiteCommand cmd = _connection.CreateCommand();
//            cmd.CommandType = CommandType.Text;
//            cmd.CommandText = "insert into schedule (week_id, location, start, end, last_update) values(@parameter1, @parameter2, @parameter3, @parameter4, CURRENT_TIMESTAMP)";
//            cmd.Parameters.Add(new SQLiteParameter("@parameter", textfield));
//            SQLiteDataReader reader = cmd.ExecuteReader();


            var sqlSelect = String.Format("SELECT * FROM schedule WHERE week_id = {0} AND location = '{1}' AND start = '{2}' AND end = '{3}'",
                schedule.WeekId, schedule.Location.Replace("'", "''"), schedule.StartDateTime.ToString(SqliteDateFormat), schedule.EndDateTime.ToString(SqliteDateFormat));

            if(result == 1)
            {
                Week week = GetWeek(schedule.WeekId);
                if(week == null)
                    return null;

                var resultSchedule = GetSchedule(sqlSelect, week);
                return resultSchedule.First();
            }
            return null;
        }

        public Schedule Update(Schedule originalSchedule, Schedule updatedSchedule)
        {
            throw new NotImplementedException();
        }

        public int Delete(IEnumerable<Schedule> schedules)
        {
            if(schedules == null)
                return 0;
            var scheduleList = schedules.ToList();
            if(!scheduleList.Any())
                return 0;

            const string sqlFixed = "DELETE FROM schedule WHERE id IN ({0})";
            var s = String.Join(", ", scheduleList.Select(x => x.Id));
            string sql = String.Format(sqlFixed, s);
            return sqlitedb.ExecuteNonQuery(sql);
        }

        public Week Insert(Week week)
        {
            const string sqlFixed = "insert into week (year, week, hash) values({0}, {1}, '{2}')";
            var result = sqlitedb.ExecuteNonQuery(String.Format(sqlFixed, week.Year, week.WeekNr, week.Hash));

            if(result == 1)
            {
                Week resultWeek = GetWeek(week.Year, week.WeekNr);
                return resultWeek;
            }
            return null;
        }


        private void Init()
        {
            //-- DROP TABLE IF EXISTS week;
            //DROP TABLE IF EXISTS schedule;
            const string sql = @"

	        CREATE TABLE IF NOT EXISTS week ( 
		           id                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 		
		           year               SMALLINT NOT NULL, 
		           week               SMALLINT NOT NULL, 
		           hash				  VARCHAR(40)
	        );

	        CREATE TABLE IF NOT EXISTS schedule ( 
		           id                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
		           week_id            INTEGER NOT NULL, 
		           start			  TIMESTAMP NOT NULL,
		           end				  TIMESTAMP NOT NULL,
		           location			  VARCHAR(1024) NOT NULL,
		           last_update		  TIMESTAMP NOT NULL,
		           FOREIGN KEY(week_id) REFERENCES week(id)
	        );";

            sqlitedb.ExecuteNonQuery(sql);
        }

        private Week GetWeek(string sql)
        {
            var result = sqlitedb.GetDataTable(sql);
            int i;
            if (result.Rows.Count == 1)
            {
                int indexYear = result.Columns.IndexOf("year");
                int indexWeek = result.Columns.IndexOf("week");
                int indexId = result.Columns.IndexOf("id");
                int indexHash = result.Columns.IndexOf("hash");

                if (indexYear == -1 || indexWeek == -1 || indexId == -1 || indexHash == -1)
                    return null;

                DataRow row = result.Rows[0];

                var week = new Week();

                if (!TryParseObjectToInteger((row[indexId]).ToString(), out i))
                    return null;
                week.Id = i;

                if (!TryParseObjectToInteger(row[indexYear], out i))
                    return null;
                week.Year = i;

                if (!TryParseObjectToInteger(row[indexWeek], out i))
                    return null;
                week.WeekNr = i;

                week.Hash = (string)row[indexHash];


                //                var index = result.Columns.IndexOf("id");
                //                if (index < 0)
                //                    return null;
                //                if (!Int32.TryParse((row[index]).ToString(), out i))
                //                    return null;
                //                week.Id = i;
                //
                //                index = result.Columns.IndexOf("hash");
                //                if (index < 0)
                //                    return null;
                //                week.Hash = (string)row[index];

                return week;
            }
            return null;
        }

        private IList<Schedule> GetSchedule(string sql, Week week)
        {
            int i;
            DateTime tmpDateTime;
            try
            {
                var result = sqlitedb.GetDataTable(sql);
                var scheduleList = new List<Schedule>();

                int indexId = result.Columns.IndexOf("id");
                int indexStartDateTime = result.Columns.IndexOf("start");
                int indexEndDateTime = result.Columns.IndexOf("end");
                int indexLocation = result.Columns.IndexOf("location");

                if (indexId == -1 || indexStartDateTime == -1 || indexEndDateTime == -1 || indexLocation == -1)
                    return null;

                foreach (DataRow r in result.Rows)
                {
                    var s = new Schedule
                    {
                        WeekId = week.Id,
                        Week = week
                    };

                    if (!Int32.TryParse((r[indexId]).ToString(), out i))
                        return null;
                    s.Id = i;

                    if (!TryParseObjectToDateTime(r[indexStartDateTime], out tmpDateTime))
                        return null;
                    s.StartDateTime = tmpDateTime;

                    if (!TryParseObjectToDateTime(r[indexEndDateTime], out tmpDateTime))
                        return null;
                    s.EndDateTime = tmpDateTime;

                    s.Location = (string)(r[indexLocation]);

                    scheduleList.Add(s);
                }
                return scheduleList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool TryParseObjectToInteger(object obj, out int result)
        {
            if (obj is Int32)
            {
                result = (int)obj;
                return true;
            }

            return Int32.TryParse(obj.ToString(), out result);
        }

        private bool TryParseObjectToDateTime(object obj, out DateTime result)
        {
            if (obj is DateTime)
            {
                result = (DateTime)obj;
                return true;
            }

            return DateTime.TryParse(obj.ToString(), out result);
        }
    }
}