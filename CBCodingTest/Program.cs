using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string dataFilePath = "Data.txt";
               
                ReadWriteData rWData = new ReadWriteData();
                List<string> readData = rWData.ReadData(dataFilePath).ToList();
               
                if (readData != null)
                {
                    rWData.GetResults(readData);
                }
                else
                {
                    Console.WriteLine("No data in source file");
                }
            }
            catch (FileNotFoundException ex) {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }    
    }
    public class ReadWriteData {
       
       private const string basePath= "../../Content/";
       public IEnumerable<string> ReadData(string fileName)
        {
            string filePath = Path.Combine(basePath, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadLines(filePath);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public void GetResults(List<String> readData)
        {
            string ruleFilePath = "RulesData.txt";
            List<string> rules = ReadData(ruleFilePath).ToList();
            foreach (var rule in rules)
            {
                string[] ruleData = rule.Split(',');
                int result = 0;
               List<String> filterData = readData.Where(line => ruleData[1].Any(s => line.StartsWith(s.ToString()))).ToList();
                if (ruleData[0].Split('_').Count() == 1) {
                    if (ruleData[0] == "AverageLength")
                    {
                        result = Convert.ToInt32(filterData.Select(x => x).Average(x => x.Length));
                    }
                    else if (ruleData[0].ToString() == "LongestLength")
                    {
                        result = filterData.Max(res => res.Length);
                    }
                }
                else if (ruleData[0].Split('_').Count() > 1)
                {
                    string[] filterRule = ruleData[0].Split('_');
                    if (filterRule[0] == "CountTotal")
                    {
                        foreach (var fd in filterData)
                        {
                            if (filterRule[1].Any(c => fd.Contains(c)))
                            {
                                foreach (var c in filterRule[1])
                                {
                                    result += fd.Count(x => x == c);
                                }
                            }
                        }
                    }
                    else if (filterRule[0] == "CountSequence")
                    {
                        filterData = new List<string>();
                        int i = 0;
                        foreach (var rd in readData)
                        {
                            if (ruleData[1].ToString().Any(s => rd.StartsWith(s.ToString())))
                            {
                                if (filterRule[1].Any(n => readData[i + 1].StartsWith(n.ToString())))
                                {
                                    filterData.Add(readData[i + 1]);
                                }
                            }
                            i++;
                        }
                        result = filterData.Count();
                    }
                }
               WriteToFile(ruleData[2].ToString(), result.ToString());
            }    
        }

        public void WriteToFile(string fileName,string result)
        {
            try
            {
                string filePath = Path.Combine(basePath, fileName);
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();

                }
               File.WriteAllText(filePath, result, Encoding.UTF8);
            }
            catch (FileNotFoundException ex) {
                throw ex;
            }
        }

        public string[] GetDataFromUser()
        {

            Console.WriteLine("Please enter the filer criteria:");
            string[] startWith = { Console.ReadLine(), Console.ReadLine() };
            return startWith;
            
        }
      
    }
    public class Rules {
        public string RuleName { get; set; }
        public string FilePath { get; set; }
    }
  
}
