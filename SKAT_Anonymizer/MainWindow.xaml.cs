using System;
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

        Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
        public MainWindow()
        {
            InitializeComponent();
             // Configure open file dialog.
            openFileDlg.FileName = DefFilename;
            openFileDlg.DefaultExt = DefFileExt;
            openFileDlg.Filter = DlgFilter;

           
            dgData.Width =  SystemParameters.PrimaryScreenWidth / 2;
            dgData.Height = SystemParameters.PrimaryScreenHeight / 3;
            dgAnonymousData.Width =  SystemParameters.PrimaryScreenWidth / 2;
            dgAnonymousData.Height = SystemParameters.PrimaryScreenHeight / 3;
            dgAnonymityMeasure.Width = SystemParameters.PrimaryScreenWidth / 3;
            dgAnonymityMeasure.Height = SystemParameters.PrimaryScreenHeight / 3;

            // Initialisieren Datatable.
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

            _anonymousData.Columns.Add(CAnonymizer.ColID);
            _anonymousData.Columns.Add(CAnonymizer.ColAge);
            _anonymousData.Columns.Add(CAnonymizer.ColSex);
            _anonymousData.Columns.Add(CAnonymizer.ColDiag);
            _anonymousData.Columns.Add(CAnonymizer.ColKtV);
            _anonymousData.Columns.Add(CAnonymizer.ColPCR);
            _anonymousData.Columns.Add(CAnonymizer.ColTACUrea);
            _anonymousData.Columns.Add(CAnonymizer.ColTimeOfDialysis);
            _anonymousData.Columns.Add(CAnonymizer.ColBloodFlow);

            _kTCriteriaData.Columns.Add("Group");
            _kTCriteriaData.Columns.Add(CAnonymizer.ColGroupSizeK);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColDiag);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColKtV);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColPCR);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColTACUrea);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColTimeOfDialysis);
            _kTCriteriaData.Columns.Add(CAnonymizer.ColTCloseness + CAnonymizer.ColBloodFlow);
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {

            // delete old Data.
            dgData.ItemsSource = null;
            dgAnonymousData.ItemsSource = null;
            dgAnonymityMeasure.ItemsSource = null;

            // Reset filename
            openFileDlg.FileName = DefFilename;

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                string filename = openFileDlg.FileName;
                txtbxFilename.Text = filename;

                DataReader dr = new DataReader();
                _patientDataSet = dr.ReadDataFromExcel(filename);

                // convert data array in a datatable to show in datagrid.
                if (ReadInPatientData(_patientDataSet))
                {
                    dgData.ItemsSource = _data.DefaultView;
                }
            }
        }
        private bool ReadInPatientData(PatientData[] patientDataSet)
        {
            // convert data array in a datatable to show in datagrid.
            _data.Clear();
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

        private bool ReadInAnonymizedData(Dictionary<int, List<object>> anonymousDataSet)
        {
            _anonymousData.Clear();
            foreach (KeyValuePair<int, List<object>> anonymousPatient in anonymousDataSet)
            {
                // anonymousData.LoadDataRow(anonymousPatient.Value.ToArray(), true);
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

        private bool ReadInKTCriteriaData(Anonymizer anonymizer)
        {
            _kTCriteriaData.Clear();
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
                tClosenessDiagnosis = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.Diagnosis], 3);
                tClosenessKtV = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.KtV], 3);
                tClosenessPCR = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.PCR], 3);
                tClosenessTACUrea = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.TACUrea], 3);
                tClosenessTimeOfDialysis = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.TimeOfDialysis], 3);
                tClosenessBloodflow = Math.Round(tCloseness.Values.ToArray()[i][(int)CAnonymizer.SA.Bloodflow], 3);

                _kTCriteriaData.Rows.Add(qIA[i], kAnonymität[qIA[i]], tClosenessDiagnosis,
                                                                      tClosenessKtV,
                                                                      tClosenessPCR,
                                                                      tClosenessTACUrea,
                                                                      tClosenessTimeOfDialysis,
                                                                      tClosenessBloodflow);
            }

            return true;
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
            if (_patientDataSet != null)
            {
                Anonymizer anonymizer = new Anonymizer();
                Dictionary<int, List<object>> anonymousDataSet = anonymizer.Anonymize(_patientDataSet);

                anonymizer.MicroAggregation(anonymizer.QIA, anonymousDataSet);
                if (ReadInAnonymizedData(anonymousDataSet))
                {
                    // Sort by age
                    //_anonymousData.DefaultView.Sort = string.Format("{0} {1} , {2}",CAnonymizer.ColAge, CAnonymizer.SortOrderASC, CAnonymizer.ColSex);
                    //_anonymousData = _anonymousData.DefaultView.ToTable();

                    dgAnonymousData.ItemsSource = _anonymousData.DefaultView;
                }


                if (ReadInKTCriteriaData(anonymizer))
                {
                    dgAnonymityMeasure.ItemsSource = _kTCriteriaData.DefaultView;
                }
            }
            else
            {
                MessageBox.Show("Bitte erst Daten einlesen.");
            }
        }
    }
}
