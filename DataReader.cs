namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>() { new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 0; i <= importedLines.Count - 1; i++)
            {

                if (string.IsNullOrEmpty(importedLines[i]))
                {
                    continue;
                }

                var importedLine = $"{importedLines[i]};";

                var values = importedLine.Split(';', '\r', '\n');
                var importedObject = new ImportedObject();
                importedObject.Type = values[0].Trim().ToUpper().Replace(" ", "");
                importedObject.Name = values[1].Trim().Replace(" ", "");
                importedObject.Schema = values[2].Trim().Replace(" ", "");
                importedObject.ParentName = values[3].Trim().Replace(" ", "");
                importedObject.ParentType = values[4].Trim().ToUpper().Replace(" ", "");
                importedObject.DataType = values[5].Trim();
                importedObject.IsNullable = values[6].Trim();
                ((List<ImportedObject>)ImportedObjects).Add(importedObject);
            }

            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.NumberOfChildren = ImportedObjects.Count(x => x.ParentType == importedObject.Type && x.ParentName == importedObject.Name);
            }


            if (printData)
            {
                foreach (var database in ImportedObjects.Where(x => x.Type == "DATABASE"))
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects.Where(x => x.ParentType == database.Type && x.ParentName == database.Name))
                    {
                        Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                        // print all table's columns
                        foreach (var column in ImportedObjects.Where(x => x.ParentType == table.Type && x.ParentName == table.Name))
                        {
                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                        }
                    }
                }
            }

            Console.ReadLine();
        }
    }




}
