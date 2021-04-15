using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.SPHelpers
{
    static class Utility
    {
        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dtDataTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName);
            sb.AppendLine(string.Join(";", columnNames));
            foreach (DataRow row in dtDataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                  string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(";", fields));
            }
            File.WriteAllText(strFilePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
