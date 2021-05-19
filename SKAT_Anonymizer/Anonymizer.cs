using System;
using System.Collections.Generic;
using System.Text;
using PatientDataGenerator;
using System.Linq;

namespace SKAT_Anonymizer
{
    class Anonymizer
    {
        private List<string> _qIA = new List<string>();
        private Dictionary<string, List<double>> _tClosenessPerGroup = new Dictionary<string, List<double>>();
        private Dictionary<string, int> _kAnonymityPerGroup = new Dictionary<string, int>();

        public Dictionary<string, int> KAnonymity
        {
            get { return _kAnonymityPerGroup; }
        }

        public Dictionary<string, List<double>> TClosenessPerClass
        {
            get 
            {return _tClosenessPerGroup; }
        }

        public List<string> QIA 
        {
            get { return _qIA; }
        }

        public Dictionary<int, List<object>> Anonymize(PatientData[] patientDataset)
        {
            // Alte Werte löschen.
            _qIA.Clear();
            _tClosenessPerGroup.Clear();
            _kAnonymityPerGroup.Clear();

            Dictionary<int, List<object>> anonymousDataSet = new Dictionary<int, List<object>>();

            Dictionary<string, int> numOfDiagValuesPerTable = new Dictionary<string, int>();
            Dictionary<double, int> numOfNumericValKTVPerTable = new Dictionary<double, int>();
            Dictionary<double, int> numOfNumericValPCRPerTable = new Dictionary<double, int>();
            Dictionary<double, int> numOfNumericValTACUreaPerTable = new Dictionary<double, int>();
            Dictionary<int, int> numOfNumericValTimeOfDialysisPerTable = new Dictionary<int, int>();
            Dictionary<int, int> numOfNumericValBloodflowPerTable = new Dictionary<int, int>();
            

            Dictionary<string, Dictionary<string,int>> numOfDiagValuesPerDiagPerClass = new Dictionary<string, Dictionary<string,int>>();
            Dictionary<string, Dictionary<double, int>> numOfNumericValuePerKTVPerClass = new Dictionary<string, Dictionary<double, int>>();
            Dictionary<string, Dictionary<double, int>> numOfNumericValuePerPCRPerClass = new Dictionary<string, Dictionary<double, int>>();
            Dictionary<string, Dictionary<double, int>> numOfNumericValuePerTACUreaPerClass = new Dictionary<string, Dictionary<double, int>>();
            Dictionary<string, Dictionary<int, int>> numOfNumericValuePerTimeOfDialysisPerClass = new Dictionary<string, Dictionary<int, int>>();
            Dictionary<string, Dictionary<int, int>> numOfNumericValuePerBloodflowPerClass = new Dictionary<string, Dictionary<int, int>>();

            List<double> probabilitiesPerDiagPerClass = new List<double>();
            List<double> probabilitiesPerKTVPerClass = new List<double>();
            List<double> probabilitiesPerPCRPerClass = new List<double>();
            List<double> probabilitiesPerTACUreaPerClass = new List<double>();
            List<double> probabilitiesPerTimeOfDialysisPerClass = new List<double>();
            List<double> probabilitiesPerBloodflowPerClass = new List<double>();

            List<double> probabilitiesPerDiagPerTable = new List<double>();
            List<double> probabilitiesPerKTVPerTable = new List<double>();
            List<double> probabilitiesPerPCRPerTable = new List<double>();
            List<double> probabilitiesPerTACUreaPerTable = new List<double>();
            List<double> probabilitiesPerTimeOfDialysisPerTable = new List<double>();
            List<double> probabilitiesPerBloodflowPerTable = new List<double>();

            string generalizedAge = "";
            string qIA = "";

            // Auf Grenzwerte prüfen und ggf. Zeile unterdrücken.
            List<PatientData> supressPatientData = SuppressColContainsLimitValues(patientDataset);

            int dataSize = supressPatientData.Count;            
            for (int i = 0; i < dataSize; i++)
            {              
                List<object> attributes = new List<object>();                              

                // Alter generalisieren.
                generalizedAge = GeneralizeAge(CalcAge(supressPatientData[i].Birth));

                // Patientendaten kopieren, Name und Vorname entfernen und Alter durch Generalisierung ersetzen.
                attributes.Add(generalizedAge);
                attributes.Add(supressPatientData[i].Sex);
                attributes.Add(supressPatientData[i].Diagnosis);
                attributes.Add(supressPatientData[i].KtV);
                attributes.Add(supressPatientData[i].PCR);
                attributes.Add(supressPatientData[i].TACUrea);
                attributes.Add(supressPatientData[i].TimeOfDialysis);
                attributes.Add(supressPatientData[i].Bloodflow);
                
                // Anonyme Patientendaten mit id versehen.
                anonymousDataSet.Add(i + 1, attributes);
                
                // Häufigkeit des Attributs im gesamt Datensatz.
                if (numOfDiagValuesPerTable.ContainsKey(supressPatientData[i].Diagnosis))
                {
                    numOfDiagValuesPerTable[supressPatientData[i].Diagnosis]++;
                }
                else
                {
                    numOfDiagValuesPerTable.Add(supressPatientData[i].Diagnosis, 1);
                }

                if (numOfNumericValKTVPerTable.ContainsKey(supressPatientData[i].KtV))
                {
                    numOfNumericValKTVPerTable[supressPatientData[i].KtV]++;
                }
                else
                {
                    numOfNumericValKTVPerTable.Add(supressPatientData[i].KtV, 1);
                }

                if (numOfNumericValPCRPerTable.ContainsKey(supressPatientData[i].PCR))
                {
                    numOfNumericValPCRPerTable[supressPatientData[i].PCR]++;
                }
                else
                {
                    numOfNumericValPCRPerTable.Add(supressPatientData[i].PCR, 1);
                }

                if (numOfNumericValTACUreaPerTable.ContainsKey(supressPatientData[i].TACUrea))
                {
                    numOfNumericValTACUreaPerTable[supressPatientData[i].TACUrea]++;
                }
                else
                {
                    numOfNumericValTACUreaPerTable.Add(supressPatientData[i].TACUrea, 1);
                }

                if (numOfNumericValTimeOfDialysisPerTable.ContainsKey(supressPatientData[i].TimeOfDialysis))
                {
                    numOfNumericValTimeOfDialysisPerTable[supressPatientData[i].TimeOfDialysis]++;
                }
                else
                {
                    numOfNumericValTimeOfDialysisPerTable.Add(supressPatientData[i].TimeOfDialysis, 1);
                }

                if (numOfNumericValBloodflowPerTable.ContainsKey(supressPatientData[i].Bloodflow))
                {
                    numOfNumericValBloodflowPerTable[supressPatientData[i].Bloodflow]++;
                }
                else
                {
                    numOfNumericValBloodflowPerTable.Add(supressPatientData[i].Bloodflow, 1);
                }

                // Quasi-identifizierende Attribute die maßgeblich für die Äquivalenzklasse sind.
                qIA = string.Format("{0}, {1}", generalizedAge, supressPatientData[i].Sex);
                                
                // Qia vorhanden, alle bereits registrierten werte um 1 erhöhen.
                if (numOfDiagValuesPerDiagPerClass.ContainsKey(qIA))
                {
                    // Diagnosen.
                    if(numOfDiagValuesPerDiagPerClass[qIA].ContainsKey(supressPatientData[i].Diagnosis))
                    {
                        numOfDiagValuesPerDiagPerClass[qIA][supressPatientData[i].Diagnosis]++;
                    }
                    else
                    {
                        numOfDiagValuesPerDiagPerClass[qIA].Add(supressPatientData[i].Diagnosis, 1);
                    }

                    // KtV.
                    if (numOfNumericValuePerKTVPerClass[qIA].ContainsKey(supressPatientData[i].KtV))
                    {
                        numOfNumericValuePerKTVPerClass[qIA][supressPatientData[i].KtV]++;
                    }
                    else
                    {
                        numOfNumericValuePerKTVPerClass[qIA].Add(supressPatientData[i].KtV, 1);
                    }

                    // PCR.
                    if (numOfNumericValuePerPCRPerClass[qIA].ContainsKey(supressPatientData[i].PCR))
                    {
                        numOfNumericValuePerPCRPerClass[qIA][supressPatientData[i].PCR]++;
                    }
                    else
                    {
                        numOfNumericValuePerPCRPerClass[qIA].Add(supressPatientData[i].PCR, 1);
                    }

                    // TAC Urea.
                    if (numOfNumericValuePerTACUreaPerClass[qIA].ContainsKey(supressPatientData[i].TACUrea))
                    {
                        numOfNumericValuePerTACUreaPerClass[qIA][supressPatientData[i].TACUrea]++;
                    }
                    else
                    {
                        numOfNumericValuePerTACUreaPerClass[qIA].Add(supressPatientData[i].TACUrea, 1);
                    }

                    // Time of dialysis.
                    if (numOfNumericValuePerTimeOfDialysisPerClass[qIA].ContainsKey(supressPatientData[i].TimeOfDialysis))
                    {
                        numOfNumericValuePerTimeOfDialysisPerClass[qIA][supressPatientData[i].TimeOfDialysis]++;
                    }
                    else
                    {
                        numOfNumericValuePerTimeOfDialysisPerClass[qIA].Add(supressPatientData[i].TimeOfDialysis, 1);
                    }

                    // bloodflow.
                    if (numOfNumericValuePerBloodflowPerClass[qIA].ContainsKey(supressPatientData[i].Bloodflow))
                    {
                        numOfNumericValuePerBloodflowPerClass[qIA][supressPatientData[i].Bloodflow]++;
                    }
                    else
                    {
                        numOfNumericValuePerBloodflowPerClass[qIA].Add(supressPatientData[i].Bloodflow, 1);
                    }
                }
                else
                {
                    // Qia nicht vorhanden. Neue Werte registrieren.
                    var diag = new Dictionary<string, int>();
                    var ktv = new Dictionary<double, int>();
                    var pcr = new Dictionary<double, int>();
                    var tacUrea = new Dictionary<double, int>();
                    var timeOfDialysis = new Dictionary<int, int>();
                    var bloodflow = new Dictionary<int, int>();

                    diag.Add(supressPatientData[i].Diagnosis, 1 );
                    numOfDiagValuesPerDiagPerClass.Add(qIA, diag);

                    ktv.Add(supressPatientData[i].KtV, 1);
                    numOfNumericValuePerKTVPerClass.Add(qIA, ktv);

                    pcr.Add(supressPatientData[i].PCR, 1);
                    numOfNumericValuePerPCRPerClass.Add(qIA, pcr);

                    tacUrea.Add(supressPatientData[i].TACUrea, 1);
                    numOfNumericValuePerTACUreaPerClass.Add(qIA, tacUrea);

                    timeOfDialysis.Add(supressPatientData[i].TimeOfDialysis, 1);
                    numOfNumericValuePerTimeOfDialysisPerClass.Add(qIA, timeOfDialysis);

                    bloodflow.Add(supressPatientData[i].Bloodflow, 1);
                    numOfNumericValuePerBloodflowPerClass.Add(qIA, bloodflow);
                }
            }// End of for.


            List<string> setOfQIA = numOfDiagValuesPerDiagPerClass.Keys.ToList<string>();

            int kAnonymity = 0;
            double p = 0.0;
            double tCloseness = 0.0;
            List<double> tClosenessPerGroupPerSA = new List<double>();

            // Make Q's.
            foreach (var value in  numOfDiagValuesPerTable.Values)
            {
                probabilitiesPerDiagPerTable.Add(Convert.ToDouble(value) / dataSize);
            }

            foreach(var value in numOfNumericValKTVPerTable.Values)
            {
                probabilitiesPerKTVPerTable.Add(Convert.ToDouble(value) / dataSize);
            }

            foreach(var value in numOfNumericValPCRPerTable.Values)
            {
                probabilitiesPerPCRPerTable.Add(Convert.ToDouble(value) / dataSize);
            }

            foreach (var value in numOfNumericValTACUreaPerTable.Values)
            {
                probabilitiesPerTACUreaPerTable.Add(Convert.ToDouble(value) / dataSize);
            }

            foreach (var value in numOfNumericValTimeOfDialysisPerTable.Values)
            {
                probabilitiesPerTimeOfDialysisPerTable.Add(Convert.ToDouble(value) / dataSize);
            }

            foreach (var value in numOfNumericValBloodflowPerTable.Values)
            {
                probabilitiesPerBloodflowPerTable.Add(Convert.ToDouble(value) / dataSize);
            }

            int lenP = 0;
            int lenQ = 0;
            int sumOfOccurence = 0;
            int minAge = 0;
            int maxAge = 0;
            int idGeneratedData = 1000;

            foreach (var qia in setOfQIA)
            {
                // Save Groups.
                _qIA.Add(qia);

                // K-Anonymität
                kAnonymity = CalcKAnonymityOfGroup(qia, anonymousDataSet);
                _kAnonymityPerGroup.Add(qia, kAnonymity);

                // Make P diagnosis and calc t-closeness.
                List<string> diagnosis = numOfDiagValuesPerDiagPerClass[qia].Keys.ToList<string>();
                lenP = diagnosis.Count();
                lenQ = probabilitiesPerDiagPerTable.Count();
                sumOfOccurence = numOfDiagValuesPerDiagPerClass[qia].Values.Sum();

                for (int j = 0; j < lenQ; j++)
                {
                    // Häufigkeit des jeweiligen Attributwertes/ Gesamtanzahl Attributwerte bezogen auf qia.
                    if (j < lenP)
                    {
                        p = Convert.ToDouble(numOfDiagValuesPerDiagPerClass[qia][diagnosis[j]]) / sumOfOccurence;
                        probabilitiesPerDiagPerClass.Add(p);
                    }
                    else
                    {
                        probabilitiesPerDiagPerClass.Add(0);
                    }
                }
                tCloseness = CalcTClosenessCatVal(probabilitiesPerDiagPerClass, probabilitiesPerDiagPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPerDiagPerClass.Clear();

                // Make P ktv and calc t-closeness.
                List<double> ktv = numOfNumericValuePerKTVPerClass[qia].Keys.ToList<double>();
                lenP = ktv.Count();
                lenQ = probabilitiesPerKTVPerTable.Count();
                sumOfOccurence = numOfNumericValuePerKTVPerClass[qia].Values.Sum();
                for (int j = 0; j < lenQ; j++)
                {
                    if (j < lenP)
                    {
                        p = Convert.ToDouble(numOfNumericValuePerKTVPerClass[qia][ktv[j]]) / sumOfOccurence;
                        probabilitiesPerKTVPerClass.Add(p);
                    }
                    else
                    {
                        probabilitiesPerKTVPerClass.Add(0);
                    }
                }
                tCloseness = CalcTClosenessNumVal(probabilitiesPerKTVPerClass, probabilitiesPerKTVPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPerKTVPerClass.Clear();

                // Make P pcr and calc t-closeness.
                List<double> pcr = numOfNumericValuePerPCRPerClass[qia].Keys.ToList<double>();
                lenP = pcr.Count();
                lenQ = probabilitiesPerPCRPerTable.Count();
                sumOfOccurence = numOfNumericValuePerPCRPerClass[qia].Values.Sum();
                for (int j = 0; j < lenQ; j++)
                {
                    if (j < lenP)
                    {
                        numOfNumericValuePerPCRPerClass[qia].Values.Sum();
                        p = Convert.ToDouble(numOfNumericValuePerPCRPerClass[qia][pcr[j]]) / sumOfOccurence ;
                        probabilitiesPerPCRPerClass.Add(p);
                    }
                    else
                    {
                        probabilitiesPerPCRPerClass.Add(0);
                    }
                }
                tCloseness = CalcTClosenessNumVal(probabilitiesPerPCRPerClass, probabilitiesPerPCRPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPerPCRPerClass.Clear();

                // Make P tac urea and calc t-closeness.
                List<double> tacUrea = numOfNumericValuePerTACUreaPerClass[qia].Keys.ToList<double>();
                lenP = tacUrea.Count();
                lenQ = probabilitiesPerTACUreaPerTable.Count();
                sumOfOccurence = numOfNumericValuePerTACUreaPerClass[qia].Values.Sum();

                for (int j = 0; j < lenQ; j++)
                {
                    if (j < lenP)
                    {
                        p = Convert.ToDouble(numOfNumericValuePerTACUreaPerClass[qia][tacUrea[j]]) / sumOfOccurence;
                        probabilitiesPerTACUreaPerClass.Add(p);
                    }
                    else
                    {
                        probabilitiesPerTACUreaPerClass.Add(0);
                    }
                }
                tCloseness = CalcTClosenessNumVal(probabilitiesPerTACUreaPerClass, probabilitiesPerTACUreaPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPerTACUreaPerClass.Clear();

                // Make P time of dialysis and calc t-closeness.
                List<int> timeOfDialysis = numOfNumericValuePerTimeOfDialysisPerClass[qia].Keys.ToList<int>();
                lenP = timeOfDialysis.Count();
                lenQ = probabilitiesPerTimeOfDialysisPerTable.Count();
                sumOfOccurence = numOfNumericValuePerTimeOfDialysisPerClass[qia].Values.Sum();

                for (int j = 0; j < lenQ; j++)
                {
                    if (j < lenP)
                    {
                        p = Convert.ToDouble(numOfNumericValuePerTimeOfDialysisPerClass[qia][timeOfDialysis[j]]) / sumOfOccurence;
                        probabilitiesPerTimeOfDialysisPerClass.Add(p);
                    }
                    else
                    {
                        probabilitiesPerTimeOfDialysisPerClass.Add(0);
                    }
                }
                tCloseness = CalcTClosenessNumVal(probabilitiesPerTimeOfDialysisPerClass, probabilitiesPerTimeOfDialysisPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPerTimeOfDialysisPerClass.Clear();

                // Make P time of bloodflow and calc t-closeness.
                List<int> bloodflow = numOfNumericValuePerBloodflowPerClass[qia].Keys.ToList<int>();
                lenP = bloodflow.Count();
                lenQ = probabilitiesPerBloodflowPerTable.Count();
                sumOfOccurence = numOfNumericValuePerBloodflowPerClass[qia].Values.Sum();
                for (int j = 0; j < lenQ; j++)
                {
                    if (j < lenP)
                    {
                        p = Convert.ToDouble(numOfNumericValuePerBloodflowPerClass[qia][bloodflow[j]]) / sumOfOccurence;
                        probabilitiesPerBloodflowPerClass.Add(p);
                    }
                    else
                    {
                        probabilitiesPerBloodflowPerClass.Add(0);
                    }
                }
                tCloseness = CalcTClosenessNumVal(probabilitiesPerBloodflowPerClass, probabilitiesPerBloodflowPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPerBloodflowPerClass.Clear();

                _tClosenessPerGroup.Add(qia, new List<double>(tClosenessPerGroupPerSA));
                tClosenessPerGroupPerSA.Clear();

                /*  Mittlere Gruppengröße bestimmten. zufällige Daten generieren. Wie ist der Einfluss auf die Daten 
                int averageGroupSize = (dataSize / setOfQIA.Count);
                int minGroupSize = (int)Math.Round((CAnonymizer.thresholdGroup * averageGroupSize), 0);

                if (_kAnonymityPerGroup[qia] >= minGroupSize && _kAnonymityPerGroup[qia] < averageGroupSize)
                {                   
                    if(qia.Contains(String.Format("{0} {1}", CAnonymizer.AgeCriteriaYounger, CAnonymizer.Eighteen)))
                    {
                        minAge = 5;
                        maxAge = CAnonymizer.Eighteen - 1;
                    }
                    else if((qia.Contains(String.Format("{0} {1} {2}", CAnonymizer.Eighteen, CAnonymizer.AgeCriteriaBetween, CAnonymizer.Thirty))))
                    {
                        minAge = CAnonymizer.Eighteen;
                        maxAge = CAnonymizer.Thirty;
                    }
                    else if ((qia.Contains(String.Format("{0} {1} {2}", CAnonymizer.ThirtyOne, CAnonymizer.AgeCriteriaBetween, CAnonymizer.Fifty))))
                    {
                        minAge = CAnonymizer.ThirtyOne;
                        maxAge = CAnonymizer.Fifty;
                    }
                    else if ((qia.Contains(String.Format("{0} {1} {2}", CAnonymizer.FiftyOne, CAnonymizer.AgeCriteriaBetween, CAnonymizer.Seventy))))
                    {
                        minAge = CAnonymizer.FiftyOne;
                        maxAge = CAnonymizer.Seventy;
                    }
                    else if (qia.Contains(String.Format("{0} {1}", CAnonymizer.AgeCriteriaOlder, CAnonymizer.Seventy)))
                    {
                        minAge = CAnonymizer.Seventy + 1;
                        maxAge = 100;
                    }


                    DataGen dataGen = new DataGen();
                    anonymousDataSet.Add(idGeneratedData++, new List<object>() { qia.Substring(0, qia.Length-3), qia.Substring(qia.Length -1),
                        dataGen.GenerateAge(minAge, maxAge), 
                        dataGen.GenerateLabVal(numOfNumericValuePerKTVPerClass[qia].Keys.ToList<double>()),
                        dataGen.GenerateLabVal(numOfNumericValuePerPCRPerClass[qia].Keys.ToList<double>()),
                        dataGen.GenerateLabVal(numOfNumericValuePerTACUreaPerClass[qia].Keys.ToList<double>()),
                        dataGen.GenerateTimeOfDialOrBloodflow(numOfNumericValuePerTimeOfDialysisPerClass[qia].Keys.ToList<int>()),
                        dataGen.GenerateTimeOfDialOrBloodflow(numOfNumericValuePerBloodflowPerClass[qia].Keys.ToList<int>())});
                }*/
            }

            // Testweise Generalisieren der SA.
            GeneralizeSAWithPattern(ref anonymousDataSet);

            return anonymousDataSet;
        }

        /// <summary>
        /// Generalisiert die sensitiven Attribute nach vorgegeben Muster bzw. Schwellwerten.
        /// </summary>
        /// <param name="anonymousDataset">Die spezifischeren Werte werden durch die Generalisierungen.</param>
        private void GeneralizeSAWithPattern( ref Dictionary<int, List<object>> anonymousDataset)
        {
            IEnumerator<int> generalizedValue = null;

            foreach (var anonoymousPatient in anonymousDataset.Values)
            {
                // Generalisierung für TacUrea.
                generalizedValue = CAnonymizer.TACUreaGeneralization.GetEnumerator();
                do
                {
                    generalizedValue.MoveNext();
                } while ((double)anonoymousPatient[(int)CAnonymizer.Attribute.TACUrea] > generalizedValue.Current);
                {
                }
                anonoymousPatient[(int)CAnonymizer.Attribute.TACUrea] = (double)generalizedValue.Current;

                // Generalisierung für Dialysezeit.
                generalizedValue = CAnonymizer.TimeOfDialysisGeneralization.GetEnumerator();
                do
                {
                    generalizedValue.MoveNext();
                } while ((int)anonoymousPatient[(int)CAnonymizer.Attribute.TimeOfDialysis] > generalizedValue.Current);
                {
                }
                anonoymousPatient[(int)CAnonymizer.Attribute.TimeOfDialysis] = generalizedValue.Current;

                // Generalisierung für Blutfluss.
                generalizedValue = CAnonymizer.BloodflowGeneralization.GetEnumerator();
                do
                {
                    generalizedValue.MoveNext();
                } while ((int)anonoymousPatient[(int)CAnonymizer.Attribute.Bloodflow] > generalizedValue.Current);
                {
                }
                anonoymousPatient[(int)CAnonymizer.Attribute.Bloodflow] = generalizedValue.Current;
            }
        }

        private List<PatientData> SuppressColContainsLimitValues(PatientData[] patientDataset)
        {
           List<PatientData> suppressedPatiendData = new List<PatientData>();

            foreach(PatientData patient in patientDataset)
            {
                // Auf Grenzwerte prüfen und ggf Tupel-unterdrückung durchführen.
                if (!(patient.KtV < CAnonymizer.MinKtV || patient.KtV > CAnonymizer.MaxKtV ||
                     patient.PCR < CAnonymizer.MinPCR || patient.PCR > CAnonymizer.MaxPCR ||
                     patient.TACUrea < CAnonymizer.MinTACUrea || patient.TACUrea > CAnonymizer.MaxTACUrea ||
                     patient.TimeOfDialysis < CAnonymizer.MinTimeOfDialysis || patient.TimeOfDialysis > CAnonymizer.MaxTimeOfDialysis ||
                     patient.Bloodflow < CAnonymizer.MinBloodflow || patient.Bloodflow > CAnonymizer.MaxBloodflow))
                {
                    suppressedPatiendData.Add(patient);
                }
            }
            return suppressedPatiendData;
        }

        private int CalcAge(DateTime birth)
        {
            int age = 0;
            if (DateTime.Now.Month > birth.Month)
            {
                age = (int)(DateTime.Now.Year - birth.Year);
            }
            else
            {
               age = ((int)(DateTime.Now.Year - birth.Year) - 1);
            }

            return age;
        }

        /// <summary>
        /// Prüft in welcher Altergruppe das alter gehört und ersetzt es mit ihr.
        /// </summary>
        /// <param name="age">Alter einer Person.</param>
        /// <returns>Gibt das alter als eine der vorgegebenen Altersgruppen wider.</returns>
        private string GeneralizeAge(int age)
        {
            string generalizedAge = "";

            if (age < CAnonymizer.Eighteen)
            {
                generalizedAge = string.Format(CAnonymizer.AgeCriteriaYounger + "{0}", CAnonymizer.Eighteen);
            }
            else if (age >= CAnonymizer.Eighteen && age <=CAnonymizer.Thirty)
            {
                generalizedAge = string.Format("{0} " + CAnonymizer.AgeCriteriaBetween + "{1}", CAnonymizer.Eighteen, CAnonymizer.Thirty);
            }
            else if (age > CAnonymizer.Thirty && age <= CAnonymizer.Fifty)
            {
                generalizedAge = string.Format("{0} " + CAnonymizer.AgeCriteriaBetween + "{1}", CAnonymizer.ThirtyOne, CAnonymizer.Fifty);
            }
            else if (age > CAnonymizer.Fifty && age <= CAnonymizer.Seventy)
            {
                generalizedAge = string.Format("{0} " + CAnonymizer.AgeCriteriaBetween + "{1}", CAnonymizer.FiftyOne, CAnonymizer.Seventy);
            }
            else if (age > CAnonymizer.Seventy)
            {
                generalizedAge = string.Format(CAnonymizer.AgeCriteriaOlder + "{0}", CAnonymizer.Seventy);
            }
            return generalizedAge;
        }

        /// <summary>
        /// Bestimmt der Groupengröße der Äquivalenzklassen. Also den Faktor k in k-Anonymität.
        /// </summary>
        /// <param name="ageCriteria">Für den Vergleich der Altergruppe.</param>
        /// <param name="sex">Für den Vergleich des Geschlechts</param>
        public int CalcKAnonymityOfGroup(string group, Dictionary<int, List<object>> anonymousData)
        {
            int k = 0;
            string qIA = "";
            object generalizedAge;
            object sex;
            foreach (var element in anonymousData)
            {
                generalizedAge = element.Value[(int)CAnonymizer.Attribute.Age];
                sex = element.Value[(int)CAnonymizer.Attribute.Sex];
                qIA = String.Format("{0}, {1}", generalizedAge, sex);

                if (qIA.ToString() == group)
                {
                    k++;
                }
            }
            return k;
        }
        private double CalcTClosenessNumVal(List<double> p,  List<double> q)
        {
            double innerSum = 0.0;
            double tCloseness = 0.0;

            for (int i = 0; i < q.Count; i++)
            {
                innerSum = innerSum + (p[i] - q[i]);
                tCloseness = tCloseness + Math.Abs(innerSum);
            }
            if (q.Count - 1 > 0)
            {
                tCloseness = tCloseness / (q.Count - 1);
            }
            return tCloseness;
        }

        private double CalcTClosenessCatVal(List<double> p, List<double> q)
        {
            
            double tCloseness = 0.0;

            for (int i = 0; i < q.Count; i++)
            {
                tCloseness = (tCloseness + Math.Abs(p[i] - q[i]));
            }

            tCloseness = tCloseness / 2;
            return tCloseness;
        }

        public Dictionary<string, List<object>> MicroAggregation(List<string> baseGroups, Dictionary<int, List<object>> anonymousData)
        {

            Dictionary<string, List<object>> microAggregatedData = new Dictionary<string, List<object>>();

            for (int i = 0; i < baseGroups.Count; i++)
            {
                if (!(baseGroups is null))
                {
                    List<object> averageAttributes = new List<object>();
                    averageAttributes.Add(anonymousData.Values.Where(anonymousPatient => String.Format("{0}, {1}",anonymousPatient[(int)CAnonymizer.Attribute.Age]
                                                                                                                 ,anonymousPatient[(int)CAnonymizer.Attribute.Sex])
                                                                     == baseGroups[i]).Average(attributes => (double)attributes[(int)CAnonymizer.Attribute.KtV]));
                    averageAttributes.Add(anonymousData.Values.Where(anonymousPatient => String.Format("{0}, {1}", anonymousPatient[(int)CAnonymizer.Attribute.Age]
                                                                                                                 , anonymousPatient[(int)CAnonymizer.Attribute.Sex])
                                                                     == baseGroups[i]).Average(attributes => (double)attributes[(int)CAnonymizer.Attribute.PCR]));
                    averageAttributes.Add(anonymousData.Values.Where(anonymousPatient => String.Format("{0}, {1}", anonymousPatient[(int)CAnonymizer.Attribute.Age]
                                                                                                                 , anonymousPatient[(int)CAnonymizer.Attribute.Sex])
                                                                     == baseGroups[i]).Average(attributes => (double)attributes[(int)CAnonymizer.Attribute.TACUrea]));
                    averageAttributes.Add(anonymousData.Values.Where(anonymousPatient => String.Format("{0}, {1}", anonymousPatient[(int)CAnonymizer.Attribute.Age]
                                                                                                                 , anonymousPatient[(int)CAnonymizer.Attribute.Sex])
                                                                     == baseGroups[i]).Average(attributes => (int)attributes[(int)CAnonymizer.Attribute.TimeOfDialysis]));
                    averageAttributes.Add(anonymousData.Values.Where(anonymousPatient => String.Format("{0}, {1}", anonymousPatient[(int)CAnonymizer.Attribute.Age]
                                                                                                                 , anonymousPatient[(int)CAnonymizer.Attribute.Sex])
                                                                     == baseGroups[i]).Average(attributes => (int)attributes[(int)CAnonymizer.Attribute.Bloodflow]));
                    microAggregatedData.Add(baseGroups[i], averageAttributes);
                }
            }
            return microAggregatedData;
        }
    }
}
