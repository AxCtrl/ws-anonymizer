using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using PatientDataGenerator;

namespace SKAT_Anonymizer
{
    class DataReader
    {
        private const string validFileExt = ".xlsx";
        private const string ExceptionUnvalidFile = "Ungültiger Dateityp. Bitte wählen Sie eine .xlsx Datei";

        PatientData[] _patientDataSet;
        public PatientData[] ReadDataFromExcel(string filepath)
        {
            if (File.Exists(filepath))
            {
                // Init Excelsheet.
                var appExcel = new Excel.Application();
                var wbExcel = appExcel.Workbooks.Open(filepath, 0, true);
                var sheetExcel = (Excel.Worksheet)wbExcel.Worksheets.get_Item(1);
                var range = sheetExcel.UsedRange;

                int rows = range.Rows.Count;
                int cols = range.Columns.Count;
                List<object> patient = null;
                _patientDataSet = new PatientData[rows - 1];
                
                try
                {
                    for (int rowCount = 1; rowCount < rows; rowCount++)
                    {

                        //_dataSet[rowCount] = new string[cols];
                        if (rowCount <= rows - 1)
                        {
                            patient = new List<object>();
                            for (int colCount = 1; colCount <= cols; colCount++)
                            {
                                patient.Add((range.Cells[rowCount + 1, colCount] as Excel.Range).Value2);
                            }
                            _patientDataSet[rowCount - 1] = new PatientData(patient[(int)PatientData.Attribute.Lastname].ToString(),
                                                                        patient[(int)PatientData.Attribute.Firstname].ToString(),
                                                                        Convert.ToDateTime(patient[(int)PatientData.Attribute.Birth]),
                                                                        patient[(int)PatientData.Attribute.Sex].ToString(),  
                                                                        patient[(int)PatientData.Attribute.Diagnosis].ToString(),
                                                                        Convert.ToDouble(patient[(int)PatientData.Attribute.KtV]),
                                                                        Convert.ToDouble(patient[(int)PatientData.Attribute.PCR]),
                                                                        Convert.ToDouble(patient[(int)PatientData.Attribute.TacUrea]),
                                                                        Convert.ToInt32(patient[(int)PatientData.Attribute.TimeOfDialysis]),
                                                                        Convert.ToInt32(patient[(int)PatientData.Attribute.BloodFlow]));
                        } 
                    }
                }
                finally
                {
                    if (wbExcel != null)
                    {
                        wbExcel.Close(true, null, null);
                        appExcel.Quit();
                    }
                }

                return _patientDataSet;
            }
            else
            {
                return null;
                throw new FormatException(ExceptionUnvalidFile);
            }
        }

        public List<List<object>> ReadConfigFile(string filename)
        {
           
            XmlTextReader xmlReader = null;
            try
            {
                List<List<object>> setOfageCriteria = new List<List<object>>();
                List<object> ageCriteria = null;
                bool nextCriteria = true;

                xmlReader = new XmlTextReader(filename);
                xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                if (File.Exists(filename))
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xmlReader.Name == CAnonymizer.ConfigCriteriaGroup)
                                {
                                  if (nextCriteria)
                                    {
                                        ageCriteria = new List<object>();
                                        nextCriteria = false;
                                    }
                                }
                                break;
                            case XmlNodeType.Text:
                                ageCriteria.Add(xmlReader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                if (xmlReader.Name == CAnonymizer.ConfigCriteriaGroup)
                                {
                                    setOfageCriteria.Add(ageCriteria);
                                    nextCriteria = true;
                                }
                            break;
                        }
                    }
                }
                return setOfageCriteria;
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            } 
        }
    }
}
