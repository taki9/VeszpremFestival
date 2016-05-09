
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace server
{
    class Database
    {
        private SQLiteConnection database;

        public Database()
        {
            database = new SQLiteConnection(@"Data Source=D:\Programming\VisualStudioProjects\VeszpremFest\database\database; Version=3; UseUTF16Encoding=True;");
        }


        public void executeQuery(string query)
        {

            try {
                database.Open();

                SQLiteCommand cmd = new SQLiteCommand(query, database);
                cmd.ExecuteNonQuery();
            } catch(SQLiteException ex)
            {


            }

            database.Close();
        }

        public DataTable selectQuery(string query)
        {
            SQLiteDataAdapter ad;

            DataTable dt = new DataTable();

            try
            {
                SQLiteCommand cmd;

                database.Open();

                cmd = database.CreateCommand();
                cmd.CommandText = query;

                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt);

            }
            catch(SQLiteException ex)
            {

            }

            database.Close();

            return dt;
        }
    }
}
