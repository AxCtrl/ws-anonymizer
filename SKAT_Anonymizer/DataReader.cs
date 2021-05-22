﻿using System;
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

        string[][] _dataSet;
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
                _dataSet = new string[rows][];
                _patientDataSet = new PatientData[rows - 1];
                
                try
                {
                    for (int rowCount = 0; rowCount < rows; rowCount++)
                    {

                        //_dataSet[rowCount] = new string[cols];
                        if (rowCount <= rows - 2)
                        {
                            _patientDataSet[rowCount] = new PatientData((string)(range.Cells[rowCount + 2, (int)PatientData.Attribute.Lastname + 1] as Excel.Range).Value2,
                                                                            (string)(range.Cells[rowCount + 2, (int)PatientData.Attribute.Firstname + 1] as Excel.Range).Value2,
                                                                            DateTime.Parse((string)(range.Cells[rowCount + 2, (int)PatientData.Attribute.Birth + 1] as Excel.Range).Value2),
                                                                            (string)(range.Cells[rowCount + 2, (int)PatientData.Attribute.Sex + 1] as Excel.Range).Value2,
                                                                            (string)(range.Cells[rowCount + 2, (int)PatientData.Attribute.Diagnosis + 1] as Excel.Range).Value2,
                                                                            Convert.ToDouble((range.Cells[rowCount + 2, (int)PatientData.Attribute.KtV + 1] as Excel.Range).Value2),
                                                                            Convert.ToDouble((range.Cells[rowCount + 2, (int)PatientData.Attribute.PCR + 1] as Excel.Range).Value2),
                                                                            Convert.ToDouble((range.Cells[rowCount + 2, (int)PatientData.Attribute.TacUrea + 1] as Excel.Range).Value2),
                                                                            Convert.ToInt32((range.Cells[rowCount + 2, (int)PatientData.Attribute.TimeOfDialysis + 1] as Excel.Range).Value2),
                                                                            Convert.ToInt32((range.Cells[rowCount + 2, (int)PatientData.Attribute.BloodFlow + 1] as Excel.Range).Value2));
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
