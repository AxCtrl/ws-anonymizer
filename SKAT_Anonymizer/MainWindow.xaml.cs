﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PatientDataGenerator;

namespace SKAT_Anonymizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DefFilename = "\t Bitte Datenquelle auswählen...";
        const string DefFileExt = ".xlsx";
        const string DlgFilter = "Excel (.xlsx)|*.xlsx";
        const string ExportPath = @"D:\ws-anonymizer\Beispieldaten\PatientData.xlsx";

        PatientData[] _patientDataSet = null;
        DataTable _data = new DataTable();
        DataTable _anonymousData = new DataTable();
        DataTable _kTCriteriaData = new DataTable();
        DataTable _MicroAggregatedData = new DataTable();
        int _numOfSuppressedData = 0;
        int _numOfAnonymousData = 0;

        Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
        public MainWindow()
        {
            InitializeComponent();
             // Configure open file dialog.
            openFileDlg.FileName = DefFilename;
            openFileDlg.DefaultExt = DefFileExt;
            openFileDlg.Filter = DlgFilter;

            _anonymousData.Columns.Add(CAnonymizer.ColID);
            _anonymousData.Columns.Add(CAnonymizer.ColAge);
            _anonymousData.Columns.Add(CAnonymizer.ColSex);
            _anonymousData.Columns.Add(CAnonymizer.ColDiag);
            _anonymousData.Columns.Add(CAnonymizer.ColKtV);
            _anonymousData.Columns.Add(CAnonymizer.ColPCR);
            _anonymousData.Columns.Add(CAnonymizer.ColTACUrea);
            _anonymousData.Columns.Add(CAnonymizer.ColTimeOfDialysis);
            _anonymousData.Columns.Add(CAnonymizer.ColBloodFlow);

            _data.Columns.Add(CAnonymizer.ColID);
            _data.Columns.Add(CAnonymizer.ColLastname);
            _data.Columns.Add(CAnonymizer.ColFirstname);
            _data.Columns.Add(CAnonymizer.ColBirth);
            _data.Columns.Add(CAnonymizer.ColSex);
            _data.Columns.Add(CAnonymizer.ColDiag);
            _data.Columns.Add(CAnonymizer.ColKtV);
            _data.Columns.Add(CAnonymizer.ColPCR);
            _data.Columns.Add(CAnonymizer.ColTACUrea);
            _data.Columns.Add(CAnonymizer.ColTimeOfDialysis);
            _data.Columns.Add(CAnonymizer.ColBloodFlow);

            _kTCriteriaData.Columns.Add(CAnonymizer.ColGroupDescription);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColGroupSizeK);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColDiag);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColKtV);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColPCR);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColTACUrea);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColTimeOfDialysis);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColBloodFlow);

            _MicroAggregatedData.Columns.Add(CAnonymizer.ColGroupDescription);
            _MicroAggregatedData.Columns.Add(CAnonymizer.ColGroupSizeK);
            _MicroAggregatedData.Columns.Add(CAnonymizer.ColKtV);
            _MicroAggregatedData.Columns.Add(CAnonymizer.ColPCR);
            _MicroAggregatedData.Columns.Add(CAnonymizer.ColTACUrea);
            _MicroAggregatedData.Columns.Add(CAnonymizer.ColTimeOfDialysis);
            _MicroAggregatedData.Columns.Add(CAnonymizer.ColBloodFlow);

            lblPatientData.Content = CAnonymizer.lblPatientData;
            lblAnonymizedData.Content = CAnonymizer.lblAnonymousData;
        }

        private bool ReadInPatientData(PatientData[] patientDataSet)
        {
            // convert data array in a datatable to show in datagrid.
            _data.Clear();
            // Initialisieren Datatable.

            if (patientDataSet != null)
            {
                int id = 1;
                foreach (PatientData patient in patientDataSet)
                {
                    _data.Rows.Add(id, patient.Lastname, patient.Firstname, patient.Birth.ToShortDateString(),
                                  patient.Sex, patient.Diagnosis, patient.KtV, patient.PCR, patient.TACUrea,
                                  patient.TimeOfDialysis, patient.Bloodflow);
                    id++;

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ReadInAnonymizedData(Dictionary<int, List<object>> anonymousDataSet)
        {
            _anonymousData.Clear();

            if (anonymousDataSet != null)
            {
                foreach (var anonymousPatient in anonymousDataSet)
                {
                    if (anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.Age].ToString() == "-")
                    {
                        _numOfSuppressedData++;
                    }
                    else
                    {
                        _numOfAnonymousData++;
                    }
                    _anonymousData.Rows.Add(anonymousPatient.Key, anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.Age],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.Sex],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.Diagnosis],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.KtV],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.PCR],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.TACUrea],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.TimeOfDialysis],
                                                                 anonymousPatient.Value.ToArray()[(int)CAnonymizer.Attribute.Bloodflow]);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ReadInKTCriteriaData(Anonymizer anonymizer)
        {
            const int numOfDigits = 3;
            bool result = false;

            _kTCriteriaData.Clear();

            if (!(anonymizer is null))
            {


                List<string> qIA = anonymizer.QIA;
                Dictionary<string, int> kAnonymität = anonymizer.KAnonymity;
                Dictionary<string, List<double>> tCloseness = anonymizer.TClosenessPerClass;

                double tClosenessDiagnosis = 0.0;
                double tClosenessKtV = 0.0;
                double tClosenessPCR = 0.0;
                double tClosenessTACUrea = 0.0;
                double tClosenessTimeOfDialysis = 0.0;
                double tClosenessBloodflow = 0.0;

                for (int i = 0; i < qIA.Count; i++)
                {
                    tClosenessDiagnosis = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.Diagnosis], numOfDigits);
                    tClosenessKtV = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.KtV], numOfDigits);
                    tClosenessPCR = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.PCR], numOfDigits);
                    tClosenessTACUrea = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.TACUrea], numOfDigits);
                    tClosenessTimeOfDialysis = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.TimeOfDialysis], numOfDigits);
                    tClosenessBloodflow = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.Bloodflow], numOfDigits);

                    _kTCriteriaData.Rows.Add(qIA[i], kAnonymität[qIA[i]], tClosenessDiagnosis,
                                                                          tClosenessKtV,
                                                                          tClosenessPCR,
                                                                          tClosenessTACUrea,
                                                                          tClosenessTimeOfDialysis,
                                                                          tClosenessBloodflow);
                }
                result = true;
            }

            return result;
        }

        private bool ReadInAggregatedData(Anonymizer anonymizer, Dictionary<string, List<object>> microAggregatedData)
        {
            _MicroAggregatedData.Clear();

            const int numOfDigits = 2;
            bool result = false;

            if (!(anonymizer is null))
            {
                List<string> groups = anonymizer.QIA;
                Dictionary<string, int> groupSizes = anonymizer.KAnonymity;

                foreach (var group in groups)
                {
                    _MicroAggregatedData.Rows.Add(group, groupSizes[group], 
                                                  Math.Round(Convert.ToDouble(microAggregatedData[group][(int)CAnonymizer.Aggregated.KtV]), numOfDigits),
                                                  Math.Round(Convert.ToDouble(microAggregatedData[group][(int)CAnonymizer.Aggregated.PCR]), numOfDigits),
                                                  Math.Round(Convert.ToDouble(microAggregatedData[group][(int)CAnonymizer.Aggregated.TACUrea]), numOfDigits),
                                                  Convert.ToInt32((microAggregatedData[group][(int)CAnonymizer.Aggregated.TimeOfDialysis])),
                                                  Convert.ToInt32((microAggregatedData[group][(int)CAnonymizer.Aggregated.Bloodflow])));
                }
                result = true;
            }
            return result;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {

            // delete old Data.
            dgData.ItemsSource = null;
            dgAnonymousData.ItemsSource = null;
            dgAnonymityMeasure.ItemsSource = null;
            dgMicroAggregatedData.ItemsSource = null;
            lblPatientData.Content = CAnonymizer.lblPatientData;
            lblAnonymizedData.Content = CAnonymizer.lblAnonymousData;

            // Reset filename
            openFileDlg.FileName = DefFilename;

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                string filename = openFileDlg.FileName;
                txtbxFilename.Text = filename;

                DataReader dr = new DataReader();
                try
                {
                    _patientDataSet = dr.ReadDataFromExcel(filename);
                }
                catch
                {
                    MessageBox.Show("Ungültige Datei, bitte prüfen Sie die Formatierung");
                }
                finally
                {
                    dr = null;
                }

                // convert data array in a datatable to show in datagrid.
                if (ReadInPatientData(_patientDataSet))
                {
                    dgData.ItemsSource = _data.DefaultView;
                    lblPatientData.Content = String.Format("{0} {1}", CAnonymizer.lblPatientData, _patientDataSet.Length);
                }
            }
        }

        private void btnGenerateData_Click(object sender, RoutedEventArgs e)
        {
            int datasize = 0;
            DataGen dataGen = new DataGen();
            if (Int32.TryParse(inDataSize.Text, out datasize)) 
            {
                dataGen.GeneratePatientData(datasize);
                dataGen.SaveGeneratedDataAsExcel(ExportPath);
            }
            else
            {
                MessageBox.Show(CAnonymizer.MsgUnvailDatasize);
            }
        }

        private void btnAnonymize_Click(object sender, RoutedEventArgs e)
        {
            _anonymousData.Clear();
            _kTCriteriaData.Clear();
            _MicroAggregatedData.Clear();
            dgAnonymousData.ItemsSource = null;
            dgAnonymityMeasure.ItemsSource = null;
            dgMicroAggregatedData.ItemsSource = null;

            _numOfAnonymousData = 0;
            _numOfSuppressedData = 0;

            if (_patientDataSet != null)
            {
                Anonymizer anonymizer = new Anonymizer();

                Dictionary<int, List<object>> anonymousDataSet = anonymizer.Anonymize(_patientDataSet, (bool)cbSAGeneralization.IsChecked, (bool)cbLimitValueSuppression.IsChecked);

                
                if (ReadInAnonymizedData(anonymousDataSet))
                {
                    dgAnonymousData.ItemsSource = _anonymousData.DefaultView;
                    lblAnonymizedData.Content = String.Format("{0}{1}{2}, {3}{4}", CAnonymizer.lblAnonymousData, CAnonymizer.lblAnonymousDataAnonym, _numOfAnonymousData, 
                                                                                  CAnonymizer.lbAnonymousDataSuppressed, _numOfSuppressedData);
                }


                if (ReadInKTCriteriaData(anonymizer))
                {
                    dgAnonymityMeasure.ItemsSource = _kTCriteriaData.DefaultView;
                }

                Dictionary<string, List<object>> mAggregatedDAta = anonymizer.MicroAggregation(anonymizer.QIA, anonymousDataSet);

                if (ReadInAggregatedData(anonymizer, mAggregatedDAta))
                {
                    dgMicroAggregatedData.ItemsSource = _MicroAggregatedData.DefaultView;
                }
            }
            else
            {
                MessageBox.Show("Bitte erst Daten einlesen.");
            }
        }
    }
}
