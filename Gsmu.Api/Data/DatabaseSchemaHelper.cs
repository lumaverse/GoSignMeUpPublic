using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data
{
    public class DatabaseSchemaHelper
    {
        public static List<DatabaseSchemaColumnModel> GetSchoolDatabaseTableColumnNames(string tableName)
        {
            var columns = new List<DatabaseSchemaColumnModel>();
            using (var connection = Connections.GetSchoolConnection())
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = string.Format(
                    "select c.name, t.name as type_name, c.max_length from sys.all_columns as c left join sys.types as t on c.system_type_id = t.system_type_id and c.user_type_id = t.user_type_id where c.object_id = OBJECT_ID('{0}');",
                    tableName
                );
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(
                        new DatabaseSchemaColumnModel
                        {
                            Name = reader.GetString(0),
                            Type = reader.GetString(1),
                            Length = reader.GetInt16(2)
                        });
                }

                connection.Close();
                return columns;
            }
        }
    }
}
