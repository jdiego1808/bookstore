using System;
using Utility;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // string pwd = "admin";
            // string hash = PasswordHashOMatic.Hash(pwd);
            // Console.WriteLine(hash);
            // DataTable dt = new DataTable("Test");
            // DataColumn column;
            // DataRow row;
            // column = new DataColumn();
            // column.DataType = System.Type.GetType("System.String");
            // column.ColumnName = "id";
            // column.AutoIncrement = false;
            // column.Unique = false;
            // dt.Columns.Add(column);

            // column = new DataColumn();
            // column.DataType = System.Type.GetType("System.String");
            // column.ColumnName = "author";
            // column.AutoIncrement = false;
            // column.ReadOnly = false;
            // column.Unique = false;
            // dt.Columns.Add(column);

            // row = dt.NewRow();
            // row["id"] = "20001111";
            // row["author"] = "Conan";
            // dt.Rows.Add(row);

            // row = dt.NewRow();
            // row["id"] = "20001111";
            // row["author"] = "Sherlock";
            // dt.Rows.Add(row);

            // row = dt.NewRow();
            // row["id"] = "20001211";
            // row["author"] = "Authur";
            // dt.Rows.Add(row);

            // row = dt.NewRow();
            // row["id"] = "20001211";
            // row["author"] = "Diego";
            // dt.Rows.Add(row);

            // row = dt.NewRow();
            // row["id"] = "20002211";
            // row["author"] = "Niklaus";
            // dt.Rows.Add(row);
            
            // Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            // dict = dt.AsEnumerable().GroupBy(r => r.Field<string>("id")).ToDictionary(r => r.Key, r => r.Select(x => x.Field<string>("author")).ToList());

            // // Print the dictionary.
            // foreach (KeyValuePair<string, List<string>> kvp in dict)
            // {
            //     Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, string.Join(", ", kvp.Value));
            // }

            // // Find values that match the specified key.
            // List<string> values;
            // if (dict.TryGetValue("20001111", out values))
            // {
            //     Console.WriteLine("Values for key '20001111': {0}", string.Join(", ", values));
            // }
            // else
            // {
            //     Console.WriteLine("No values found for key '20001111'.");
            // }
            var id = Guid.NewGuid().ToString();
            Console.WriteLine(id);
            var tokens = id.Split("-");
            tokens[tokens.Length - 1] = tokens[tokens.Length - 1].Substring(0, 4);
            var final = string.Join("", tokens);
            Console.WriteLine(final);

        }
    }
}
