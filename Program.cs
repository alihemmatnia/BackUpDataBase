using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BackUpDatabase
{
    public class Program
    {
        private const string ConnectionString = "Data Source=.;Initial Catalog=Employee; Integrated Security=true";
        private const string Path = @"C:\BackUpEmployee\";
        static void Main(string[] args)
        {
            var sqlConStrBuilder = new SqlConnectionStringBuilder(ConnectionString);
            var date = DateTime.Now;
            var backupFileName = String.Format("{0}{1}-{2}-{3}-{4}.bak",
                Path, sqlConStrBuilder.InitialCatalog,
                date.ToString("yyyy-MM-dd"), date.ToString("HH"), date.ToString("mm"));

            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("BACKUP DATABASE {0} TO DISK='{1}'",
                    sqlConStrBuilder.InitialCatalog, backupFileName);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    DirectoryInfo directoryInfo = new DirectoryInfo(Path);
                    var result = directoryInfo.GetFiles().OrderByDescending(t => t.LastWriteTime).Skip(3).ToList();
                    if (result != null)
                    {
                        result.ForEach(e =>
                        {
                            File.Delete(e.FullName);
                        });
                    }
                   
                }
            }
        }
    }
}
