using System;
using System.Collections.Generic;
using System.Text;

using io = System.IO;

using json = Newtonsoft.Json;
using csv = Kent.Boogaart.KBCsv;
using lang = Gsmu.Api.Language;

namespace Gsmu.Api.Export
{
    public static class ExtJsCsvExport
    {
        public static string GenerateCvsFile(Data.DataStoreResult queryResult, string exportColumns)
        {
            var columns = json.JsonConvert.DeserializeObject<ExtJsCvsExportFieldInfo[]>(exportColumns);

            using(var output = new io.MemoryStream())
            using(var writer = new csv.CsvWriter(output, Encoding.UTF8))
            {
                csv.HeaderRecord headerRecord = new csv.HeaderRecord();
                foreach (var column in columns)
                {
                    headerRecord.Add(column.Text);
                }
                writer.WriteRecord(headerRecord);

                var data = queryResult.Data;
                foreach (var record in queryResult.Data)
                {
                    var csvData = new csv.DataRecord(headerRecord);
                    foreach (var column in columns)
                    {
                        var value = lang.ReflectionHelper.GetPropertyValue(record, column.Column);
                        if (value != null)
                        {
                            csvData.Add(value.ToString());
                        }
                        else
                        {
                            csvData.Add(null);
                        }
                    }
                    writer.WriteRecord(csvData);
                }
                writer.Flush();
                return Encoding.UTF8.GetString(output.ToArray());

            }
        }
    }
}
