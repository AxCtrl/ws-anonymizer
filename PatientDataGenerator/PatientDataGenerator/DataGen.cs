using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder;
using Excel = Microsoft.Office.Interop.Excel;

namespace PatientDataGenerator
{
    public class DataGen
    {
        int _datasize;
        const int attrcount = 10;
        string[,] strPatienDataSet;
        int decimalpoint = 2;

        private Random randomizer = new Random();
            // Declare Arrays for all atttributes of dataset

        string[] firstname = new string[] {"Hannsgeorg", "Heidegret", "Isedore", "Mira", "Hanspeter",
                                           "Folkhart", "Linhilde", "Rebecca", "Silvia", "Walther", "Joscha",
                                           "Adam", "Daniela", "Chantalle", "Birk", "Zoe", "Aylin", "Franziskus",
                                           "Dankhard", "Hildegardt", "Helmbrecht", "Tristan", "Filippina",
                                           "Rose", "Annika", "Denny", "Wilbrand", "Amelie", "Xenia", "Manuel",
                                           "Nathalie", "Kläre", "Joelle", "Wolfhelm", "Wernhild", "Siegbert",
                                           "Arthur", "Melisande", "Arnd", "Michaele", "Gottholde", "Irenus",
                                           "Theodeline", "Louis", "Romanus", "Erik", "Theohold", "Curt", "Christiane",
                                           "Heyko", "Helma", "Englbert", "Uve", "Alexandra", "Gerhard", "Vroni",
                                           "Walburg", "Wulfhardt", "Josefa", "Gretl", "Maya", "Kajetan", "Frank",
                                           "Berndt", "Friedeborn", "Siegmund", "Anni", "Siegulf", "Nelli", "Rezzo",
                                           "Friedlinde", "Krista", "Friedemann", "Natascha", "Isidor", "Aron",
                                           "Ferhard", "Wingolf", "Zoe", "Arnhild", "Felix", "Marliese", "Mariechen",
                                           "Dorena", "Sidonie", "Hansgeorg", "Gritt", "Robert", "Altrud", "Jolina",
                                           "Traudi", "Michelle", "Maya", "Veit", "Florentine", "Bernd", "Elrike",
                                           "Willehad", "Karen", "Nicole"};
        string[] lastname = new string[]{"Nowotny", "Haase", "Wahlers", "Lisson",
                                        "Murawski", "Aretz", "Schumann", "Buhlke", "Klemke", "Kahn", "Hartel",
                                         "Arnhold", "Klappert", "Barthel", "Limberger", "Jansen", "Turan",
                                         "Ertel", "Schramm", "Viereck", "Rohe", "Gehr", "Kluger", "Senioren",
                                         "Rottmann", "Knaup", "Denz", "Paris", "Beyer", "Magnus", "Weiss",
                                         "Bastian", "Behrens", "Donaubauer", "Stephan", "Herbold", "Kaden",
                                         "Dean", "Elfers", "Krinke", "Ebbinghaus", "Sticht", "Diekmann", "Ude",
                                         "Sikora", "Nolde", "Seiler", "Frische", "Feist", "Hühner", "Kanone",
                                         "Bieker", "Apfel", "Hoer", "Buttjer", "Conner", "Schneid", "Rohner",
                                         "Merten", "Schwarzfischer", "Domann", "Klinge", "Streubel", "Krey",
                                         "Finkler", "Gerstner", "Krause", "Schiffer", "Knaus", "Soika",
                                         "Lommatzsch", "Lehmann", "Knothe", "Wieneke", "Meuer", "Tober",
                                         "Bettermann", "Carmen", "Link", "Rotter", "Buchwald", "Käner", "Eisenhardt",
                                         "Gorges", "Kostka", "Gonsior", "Langenbach", "Demmer", "Korell",
                                         "Piotrowski", "Raue", "Ling", "Middendorf", "Rohland", "Angermeier",
                                         "Reise", "Cole", "Bastian", "Wein", "Woods"};

        // Range of attributes.
        string[] Sex = new string[] { "M", "W" };

        /// <summary>
        /// Berechnet die Standardabweichung für eine gegebene Datenreihe.
        /// </summary>
        /// <param name="values">Datenreihe</param>
        /// <returns>Standardabweichung der Datenreihe</returns>
        private double CalcStandardVariation(List<double> values)
        {
            int n = values.Count();
            double average = values.Average();

            for (int i = 0; i < n; i++)
            {
                values[i] = Math.Pow(values[i] - average, 2);
            }

            double standardVariation = Math.Sqrt((1 / (n - 1)) * values.Sum());

            return standardVariation;
        }

        private int CalcStandardVariation(List<int> values)
        {
            int n = values.Count();
            double average = values.Average();
            List<double> valSubAvg = new List<double>();

            for (int i = 0; i < n; i++)
            {
                valSubAvg.Add(Math.Pow(values[i] - average, 2));
            }

            double standardVariation = Math.Sqrt((double)1 / (n - 1) * valSubAvg.Sum());

            return (int)standardVariation;
        }

        public List<DateTime> GenerateBirthDates(int beginYear, int endYear, int range)
        {
            const int minDay = 1;
            const int maxDayFeb = 28;
            const int maxDayAprJunSepNov = 30;
            const int maxDayJaMaJulAugOctDec = 31;
            const int minMonth = 1;
            const int maxMonth = 12;

            int day = 0;
            int k = 0;
            List<DateTime> rangeBirthday = new List<DateTime>();

            while (beginYear <= endYear + 1)
            {
                for (int j = 0; j < range; j++)
                {
                    var month = randomizer.Next(minMonth, maxMonth);

                    switch (month)
                    {
                        case (int)Month.January:
                        case (int)Month.March:
                        case (int)Month.May:
                        case (int)Month.July:
                        case (int)Month.August:
                        case (int)Month.October:
                        case (int)Month.December:
                            day = randomizer.Next(minDay, maxDayJaMaJulAugOctDec);
                            break;
                        case (int)Month.April:
                        case (int)Month.June:
                        case (int)Month.September:
                        case (int)Month.November:
                            day = randomizer.Next(minDay, maxDayAprJunSepNov);
                            break;
                        case (int)Month.February:
                            day = randomizer.Next(minDay, maxDayFeb);
                            break;
                    }
                    rangeBirthday.Add(new DateTime(beginYear, month, day));
                }
                // Bereinigung des Datums.
                rangeBirthday.RemoveAll(invalid => rangeBirthday[k+4] == DateTime.MinValue);
                k++;
                beginYear++;
            }
            return rangeBirthday;
        }

        /// <summary>
        /// Generiert ein zufälliges Alter basierend auf mittelwert +- Standardabweichung.
        /// </summary>
        /// <param name="baseAge">Basis Alter auf dem dem das zufällige Alter genriert wird.</param>
        /// <returns></returns>
        public int GenerateAge(int minAge, int maxAge)
        {
           return randomizer.Next(minAge, maxAge);
        }


        /// <summary>
        /// Generiert einen Laborwert basierend auf dem Mittelwert +- Standardabweichung.
        /// </summary>
        /// <param name="baseLabVal">Basis für die Laborwertgenerierung</param>
        /// <returns>Basis Laborwert zufällig Mittelwert +- Standardabweichung</returns>
        public double GenerateLabVal(List<double> labValues)
        {
            double average = labValues.Average();
            double standardVariation = CalcStandardVariation(labValues);
            double[] labVal = new double[] { average + standardVariation, average - standardVariation };


            return labVal[randomizer.Next(labVal.Length - 1)];
        }


        /// <summary>
        /// Generiert einen Wer für Dialysezeit oder Blutfluss basierend auf dem Mittelwert +- Standardabweichung.
        /// </summary>
        /// <param name="baseLabVal">Basis für die Laborwertgenerierung</param>
        /// <returns>Basis Laborwert zufällig Mittelwert +- Standardabweichung</returns>
        public int GenerateTimeOfDialOrBloodflow(List<int> baseValues)
        {
            double average = baseValues.Average();
            int standardVariation = CalcStandardVariation(baseValues);
            double[] labVal = new double[] { average + standardVariation, average - standardVariation };

            return (int)Math.Round(labVal[randomizer.Next(labVal.Length - 1)]);
        }

        List<string> rangeDiagnosis = new List<string> {"Akutes Nierenversagen", "Chronische Nierenkrankheit", "Niereninsuffizienz",
                                                    "Alport-Syndrom", "Diabetes Mellitus"};
        public double[] GenerateKtV(double beginVal, double endVal, double precision)
        {
            // Fill wit value between 0.8 and 2.5 18 und 0.1
            int range = (int)((endVal - beginVal) / precision) + 1;
            double[] rangeKtV = new double[range];
            for (int i = 0; i < range; i++)
            {
                rangeKtV[i] = Math.Round(beginVal + (i * precision),decimalpoint);
            }
            return rangeKtV;
        } 

        public double[] GeneratePCR(double beginVal, double endVal, double precision)
        {
                // 0.5 2.5 0.1 21
            int range = (int)((endVal - beginVal) / precision) + 1;
            double[] rangePCR = new double[range];

            for (int i = 0; i < range; i++)
            {
                rangePCR[i] = Math.Round(beginVal + (i * precision), decimalpoint);
            }
            return rangePCR;
        }
       
        public double[] GenerateTACUrea(double beginVal, double endVal, double precision)
        {
            // Fill wit value between 15. and 75. mg/dl 
            int range = (int)((endVal - beginVal) / precision) + 1;
            double[] rangeTACUrea = new double[range];

            for (int i = 0; i < range; i++)
            {
                rangeTACUrea[i] = Math.Round(beginVal + (i * precision), decimalpoint);
            }

            return rangeTACUrea;
        }

        public int[] GenerateBloodflow(int beginVal, int endVal, int precision)
        {
            // Fill wit value between 100. and 500. ml/min
            int range = (int)((endVal - beginVal) / precision) + 1;
            int[] rangeBloodFlow = new int[range];

            for (int i = 0; i < range; i++)
            {
                rangeBloodFlow[i] = beginVal + i;
            }

            return rangeBloodFlow;
        }
        
        public int[] GenerateTimeOfDialysis(int beginVal, int endVal, int precision)
        {
            // Fill wit value between 120. and 500. min
            int range = (int)((endVal - beginVal) / precision) + 1;
            int[] rangeDialisysTime = new int[range];

            for (int i = 0; i < range; i++)
            {
                rangeDialisysTime[i] = beginVal + i;
            }
            return rangeDialisysTime;
        }

        public PatientData[] GeneratePatientData(int datasize)
        {
            _datasize = datasize;   
            List<DateTime> birthDates = GenerateBirthDates(1921, 2016, 5);
            double[] ktv = GenerateKtV(0.800, 2.500, 0.100);
            double[] pcr = GeneratePCR(0.500, 2.500, 0.100);
            double[] tacUrea = GenerateTACUrea(15.0, 100.0, 0.1);
            int[] timeOfDialysis = GenerateTimeOfDialysis(120, 350, 1);
            int[] bloodflow = GenerateBloodflow(100, 500, 1);

            PatientData[] patientDataSet = new PatientData[datasize];
            strPatienDataSet = new string[datasize, attrcount];

            for (int patient = 0; patient < datasize; patient++)
            {
               
                patientDataSet[patient] = new PatientData(lastname[randomizer.Next(lastname.Length)],
                                firstname[randomizer.Next(firstname.Length)],
                                birthDates[randomizer.Next(birthDates.Count)],
                                Sex[randomizer.Next(Sex.Length)],
                                rangeDiagnosis[randomizer.Next(rangeDiagnosis.Count)],
                                ktv[randomizer.Next(ktv.Length)],
                                pcr[randomizer.Next(pcr.Length)],
                                tacUrea[randomizer.Next(tacUrea.Length)],
                                timeOfDialysis[randomizer.Next(timeOfDialysis.Length)],
                                bloodflow[randomizer.Next(bloodflow.Length)]);

                // Copy to string because interops.
                strPatienDataSet[patient, (int)PatientData.Attribute.Lastname] = patientDataSet[patient].Lastname;
                strPatienDataSet[patient, (int)PatientData.Attribute.Firstname] = patientDataSet[patient].Firstname;
                strPatienDataSet[patient, (int)PatientData.Attribute.Birth] = patientDataSet[patient].Birth.ToShortDateString();
                strPatienDataSet[patient, (int)PatientData.Attribute.Sex] = patientDataSet[patient].Sex;
                strPatienDataSet[patient, (int)PatientData.Attribute.Diagnosis] = patientDataSet[patient].Diagnosis;
                strPatienDataSet[patient, (int)PatientData.Attribute.KtV] = patientDataSet[patient].KtV.ToString();
                strPatienDataSet[patient, (int)PatientData.Attribute.PCR] = patientDataSet[patient].PCR.ToString();
                strPatienDataSet[patient, (int)PatientData.Attribute.TacUrea] = patientDataSet[patient].TACUrea.ToString();
                strPatienDataSet[patient, (int)PatientData.Attribute.TimeOfDialysis] = patientDataSet[patient].TimeOfDialysis.ToString();
                strPatienDataSet[patient, (int)PatientData.Attribute.BloodFlow] = patientDataSet[patient].Bloodflow.ToString();
            }
            return patientDataSet;
        }            

        public Boolean SaveGeneratedDataAsExcel(string path)
        {
            if (strPatienDataSet != null)
            {
                var appExcel = new Excel.Application();
                var wbExcel = (Excel._Workbook)(appExcel.Workbooks.Add());
                var sheetExcel = (Excel._Worksheet)wbExcel.ActiveSheet;

                try
                {

                    // Create an array for the headers and add it to cells A1:j1
                    object[] datasetHeader = { "Name", "Vorname", "Geburtsdatum", "Geschlecht", "Diagnose",
                                        "KtV", "PCR g pro kg pro Tag", "TAC Urea mg pro dl", "Dialysezeit min", "Blutfluss ml pro min" };

                    var sheetRange = sheetExcel.get_Range("A1", "J1");
                    sheetRange.Value = datasetHeader;
                    var headerFont = sheetRange.Font;
                    headerFont.Bold = true;

                    sheetRange = sheetExcel.get_Range("A2");
                    sheetRange = sheetRange.get_Resize(_datasize, attrcount);
                    sheetRange.Value = strPatienDataSet;

                    // Save the Workbook and quit Excel.
                    wbExcel.SaveAs(@path);
                    return true;
                }
                finally
                {
                    wbExcel.Close(true);
                    appExcel.Quit();
                }
            }
            else
            {
                return false;
            }
        }
        enum Month : ushort
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12,
        }
        static void Main() { }
    }
}
