using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DocStore.API.Shared
{
    public class ReaderExtension
    {
        public static string GetString(SqlDataReader reader, string Key)
        {
            if (reader.IsDBNull(reader.GetOrdinal(Key)))
            {
                return String.Empty;
            }
            else
            {
                return reader[Key].ToString();
            }
        }

        public static int GetInt(SqlDataReader reader, string Key)
        {
            return Int32.Parse(reader[Key].ToString());
        }

        public static bool GetBool(SqlDataReader reader, string Key)
        {
            return (bool)reader[Key];
        }

        public static DateTime GetDate(SqlDataReader reader, string Key)
        {
            return reader.GetDateTime(reader.GetOrdinal(Key));
        }

        public static DateTime? GetNullableDate(SqlDataReader reader, string Key)
        {
            var col = reader.GetOrdinal(Key);
            return reader.IsDBNull(col) ?
                        (DateTime?)null :
                        (DateTime?)reader.GetDateTime(col);
        }

        public static bool IsNull(SqlDataReader reader, string Key)
        {
            return reader.IsDBNull(reader.GetOrdinal(Key));
        }
        public static bool IsNotNull(SqlDataReader reader, string Key)
        {
            return !reader.IsDBNull(reader.GetOrdinal(Key));
        }
    }
}
