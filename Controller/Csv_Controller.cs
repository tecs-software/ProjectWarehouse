using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;
using WWarehouseManagement.Database;

namespace WarehouseManagement.Controller
{
    public class Csv_Controller
    {
        static sql_control sql = new sql_control();
        public static DataTable dataTableAddress { get; set; }
        public static Boolean ConfirmedToImport { get; set; }

        public static DataTable GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {

                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {

                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn dataColumn = new DataColumn(column);
                        dataColumn.AllowDBNull = true;
                        csvData.Columns.Add(dataColumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return csvData;
        }
        public static void ImportAddress(TextBlock txtCount, ProgressBar pbLoad)
        {
            sql.Query("EXEC SpAddress_Truncate");
            int totalImported = 0;
            foreach (DataRow dr in dataTableAddress.Rows)
            {
                sql.AddParam("@province", dr[0].ToString());
                sql.AddParam("@city", dr[1].ToString());
                sql.AddParam("@areaName", dr[2].ToString());
                sql.AddParam("@canDeliver", dr[3].ToString());

                sql.Query($"EXEC SpAddress_Import @province, @city, @areaName, @canDeliver ");
                if (sql.HasException(true)) return;

                totalImported++;
                //txtCount.Text = totalImported.ToString();
                txtCount.Dispatcher.Invoke(DispatcherPriority.Normal,
                new System.Action(() => { txtCount.Text = totalImported.ToString(); pbLoad.Value = totalImported; }));
            }
        }
    }
}
