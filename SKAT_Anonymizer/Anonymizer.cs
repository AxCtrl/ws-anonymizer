using System;
using System.Collections.Generic;
using System.Text;
using PatientDataGenerator;
using System.Linq;

namespace SKAT_Anonymizer
{
    class Anonymizer
    {
        private List<string> _groups = new List<string>();
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
            get { return _groups; }
        }

        public Dictionary<int, List<object>> Anonymize(PatientData[] patientDataset, bool withSAGeneralization, bool withSuppressedSmallGroups)
        {
            // Alte Werte löschen.
            _groups.Clear();
            _tClosenessPerGroup.Clear();
            _kAnonymityPerGroup.Clear();

            PatientData[] tmpPatientDataSet = (PatientData[])patientDataset.Clone();

            Dictionary<int, List<object>> anonymousDataSet = new Dictionary<int, List<object>>();

            Dictionary<string, int> numOfDiagValuesPerTable = new Dictionary<string, int>();
            Dictionary<double, int> numOfNumericValKTVPerTable = new Dictionary<double, int>();
            Dictionary<double, int> numOfNumericValPCRPerTable = new Dictionary<double, int>();
            Dictionary<double, int> numOfNumericValTACUreaPerTable = new Dictionary<double, int>();
            Dictionary<int, int> numOfNumericValTimeOfDialysisPerTable = new Dictionary<int, int>();
            Dictionary<int, int> numOfNumericValBloodflowPerTable = new Dictionary<int, int>();
            

            Dictionary<string, Dictionary<string,int>> numOfDiagValuesPerDiagPerGroup = new Dictionary<string, Dictionary<string,int>>();
            Dictionary<string, Dictionary<double, int>> numOfNumericValuePerKTVPerGroup = new Dictionary<string, Dictionary<double, int>>();
            Dictionary<string, Dictionary<double, int>> numOfNumericValuePerPCRPerGroup = new Dictionary<string, Dictionary<double, int>>();
            Dictionary<string, Dictionary<double, int>> numOfNumericValuePerTACUreaPerGroup = new Dictionary<string, Dictionary<double, int>>();
            Dictionary<string, Dictionary<int, int>> numOfNumericValuePerTimeOfDialysisPerGroup = new Dictionary<string, Dictionary<int, int>>();
            Dictionary<string, Dictionary<int, int>> numOfNumericValuePerBloodflowPerGroup = new Dictionary<string, Dictionary<int, int>>();

            string generalizedAge = "";
            string group = "";
            List<object> patientAttributes;
            int dataSize = tmpPatientDataSet.Length;            
            for (int i = 0; i < dataSize; i++)
            {
                // Auf Grenzwerte prüfen, bei Unter- oder Überschreitung Tupelunterdrückung durchführen.
                if ((tmpPatientDataSet[i].KtV < CAnonymizer.MinKtV || tmpPatientDataSet[i].KtV > CAnonymizer.MaxKtV ||
                    tmpPatientDataSet[i].PCR < CAnonymizer.MinPCR || tmpPatientDataSet[i].PCR > CAnonymizer.MaxPCR ||
                    tmpPatientDataSet[i].TACUrea < CAnonymizer.MinTACUrea || tmpPatientDataSet[i].TACUrea > CAnonymizer.MaxTACUrea ||
                    tmpPatientDataSet[i].TimeOfDialysis < CAnonymizer.MinTimeOfDialysis || tmpPatientDataSet[i].TimeOfDialysis > CAnonymizer.MaxTimeOfDialysis ||
                    tmpPatientDataSet[i].Bloodflow < CAnonymizer.MinBloodflow || tmpPatientDataSet[i].Bloodflow > CAnonymizer.MaxBloodflow))
                {
                    patientAttributes = new List<object>();
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                    patientAttributes.Add(CAnonymizer.Suppressed);
                }
                else
                {
                    //**Optional**
                    //Generalisieren der sensitiven Attribute auf den nächst größeren Wert nach vorgegebenem Muster.
                    if (withSAGeneralization)
                    {
                        GeneralizeSAWithPattern(ref tmpPatientDataSet[i]);
                    }

                    /*Keine Überschreitung Patienten Daten ohne Name und Vorname kopieren, Alter durch Pattern generalisieren.*/
                    patientAttributes = new List<object>();
                    generalizedAge = GeneralizeAge(CalcAge(tmpPatientDataSet[i].Birth));
                    patientAttributes.Add(generalizedAge);
                    patientAttributes.Add(tmpPatientDataSet[i].Sex);
                    patientAttributes.Add(tmpPatientDataSet[i].Diagnosis);
                    patientAttributes.Add(tmpPatientDataSet[i].KtV);
                    patientAttributes.Add(tmpPatientDataSet[i].PCR);
                    patientAttributes.Add(tmpPatientDataSet[i].TACUrea);
                    patientAttributes.Add(tmpPatientDataSet[i].TimeOfDialysis);
                    patientAttributes.Add(tmpPatientDataSet[i].Bloodflow);

                    // Vorbereitung für t-closeness: Vorkommen der einzelnen Elemente pro Attribut und Gesamtdatensatz.
                    // Häufigkeit des Attributs im gesamt Datensatz.
                    CountOccurenceOfAttributePerTable(ref numOfDiagValuesPerTable, tmpPatientDataSet[i]);
                    CountOccurenceOfAttributePerTable(ref numOfNumericValKTVPerTable, tmpPatientDataSet[i], CAnonymizer.Attribute.KtV);
                    CountOccurenceOfAttributePerTable(ref numOfNumericValPCRPerTable, tmpPatientDataSet[i], CAnonymizer.Attribute.PCR);
                    CountOccurenceOfAttributePerTable(ref numOfNumericValTACUreaPerTable, tmpPatientDataSet[i], CAnonymizer.Attribute.TACUrea);
                    CountOccurenceOfAttributePerTable(ref numOfNumericValTimeOfDialysisPerTable, tmpPatientDataSet[i], CAnonymizer.Attribute.TimeOfDialysis);
                    CountOccurenceOfAttributePerTable(ref numOfNumericValBloodflowPerTable, tmpPatientDataSet[i], CAnonymizer.Attribute.Bloodflow);

                    // Quasi-identifizierende Attribute die maßgeblich für die Äquivalenzklasse sind.
                    // In Liste speichern, damit auf die gruppen später zugegriffen werden kann.
                    group = string.Format("{0}, {1}", generalizedAge, tmpPatientDataSet[i].Sex);
                    if (!_groups.Contains(group))
                    {
                        _groups.Add(group);
                    }

                    // Häufigkeiten des Attributs pro gruppe.
                    CountOccurenceOfAttributePerGroup(group, ref numOfDiagValuesPerDiagPerGroup, tmpPatientDataSet[i]);
                    CountOccurenceOfAttributePerGroup(group, ref numOfNumericValuePerKTVPerGroup, tmpPatientDataSet[i], CAnonymizer.Attribute.KtV);
                    CountOccurenceOfAttributePerGroup(group, ref numOfNumericValuePerPCRPerGroup, tmpPatientDataSet[i], CAnonymizer.Attribute.PCR);
                    CountOccurenceOfAttributePerGroup(group, ref numOfNumericValuePerTACUreaPerGroup, tmpPatientDataSet[i], CAnonymizer.Attribute.TACUrea);
                    CountOccurenceOfAttributePerGroup(group, ref numOfNumericValuePerTimeOfDialysisPerGroup, tmpPatientDataSet[i], CAnonymizer.Attribute.TimeOfDialysis);
                    CountOccurenceOfAttributePerGroup(group, ref numOfNumericValuePerBloodflowPerGroup, tmpPatientDataSet[i], CAnonymizer.Attribute.Bloodflow);
                }

                // Pro patient ID und attribute zusammen speichern.
                anonymousDataSet.Add(i + 1, patientAttributes);
            }// End of for

            int kAnonymity = 0;
            double tCloseness = 0.0;
            List<double> probabilitiesDiagPerGroup = new List<double>();
            List<double> probabilitiesKTVPerGroup = new List<double>();
            List<double> probabilitiesPCRPerGroup = new List<double>();
            List<double> probabilitiesTACUreaPerGroup = new List<double>();
            List<double> probabilitiesTimeOfDialysisPerGroup = new List<double>();
            List<double> probabilitiesBloodflowPerGroup = new List<double>();
            List<double> tClosenessPerGroupPerSA = new List<double>();

            // Wahrscheinlichkeiten Pro Element für die Gesamttabelle berechnen -> Entspricht der Verteilung Q.
            List<double> probabilitiesDiagPerTable = CalcProbabilitiesOfAttributePerTable(numOfDiagValuesPerTable, dataSize);
            List<double> probabilitiesKTVPerTable = CalcProbabilitiesOfAttributePerTable(numOfNumericValKTVPerTable, dataSize);
            List<double> probabilitiesPCRPerTable = CalcProbabilitiesOfAttributePerTable(numOfNumericValPCRPerTable, dataSize);
            List<double> probabilitiesTACUreaPerTable = CalcProbabilitiesOfAttributePerTable(numOfNumericValTACUreaPerTable, dataSize);
            List<double> probabilitiesTimeOfDialysisPerTable = CalcProbabilitiesOfAttributePerTable(numOfNumericValTimeOfDialysisPerTable, dataSize);
            List<double> probabilitiesBloodflowPerTable = CalcProbabilitiesOfAttributePerTable(numOfNumericValBloodflowPerTable, dataSize);

            foreach (var iGroup in _groups)
            {
               
                // K-Anonymität für jede Gruppe berechnen.
                kAnonymity = CalcKAnonymityOfGroup(iGroup, anonymousDataSet);
                _kAnonymityPerGroup.Add(iGroup, kAnonymity);

                // Make P diagnosis and calc t-closeness.

                probabilitiesDiagPerGroup = CalcProbabilitiesOfAttributePerGroup(probabilitiesDiagPerTable.Count(), 
                                                                                 numOfDiagValuesPerDiagPerGroup[iGroup]);
                tCloseness = CalcTClosenessCatVal(probabilitiesDiagPerGroup, probabilitiesDiagPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesDiagPerGroup.Clear();
                
                // Make P ktv and calc t-closeness.
                probabilitiesKTVPerGroup = CalcProbabilitiesOfAttributePerGroup(probabilitiesKTVPerTable.Count(),
                                                                                numOfNumericValuePerKTVPerGroup[iGroup]);
                tCloseness = CalcTClosenessNumVal(probabilitiesKTVPerGroup, probabilitiesKTVPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesKTVPerGroup.Clear();

                // Make P pcr and calc t-closeness.
                probabilitiesPCRPerGroup = CalcProbabilitiesOfAttributePerGroup(probabilitiesPCRPerTable.Count(),
                                                                                numOfNumericValuePerPCRPerGroup[iGroup]);
                tCloseness = CalcTClosenessNumVal(probabilitiesPCRPerGroup, probabilitiesPCRPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesPCRPerGroup.Clear();

                // Make P tac urea and calc t-closeness.
                probabilitiesTACUreaPerGroup = CalcProbabilitiesOfAttributePerGroup(probabilitiesTACUreaPerTable.Count(),
                                                                                numOfNumericValuePerTACUreaPerGroup[iGroup]);
                tCloseness = CalcTClosenessNumVal(probabilitiesTACUreaPerGroup, probabilitiesTACUreaPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesTACUreaPerGroup.Clear();

                // Make P time of dialysis and calc t-closeness.
                probabilitiesTimeOfDialysisPerGroup = CalcProbabilitiesOfAttributePerGroup(probabilitiesTimeOfDialysisPerTable.Count(),
                                                                numOfNumericValuePerTimeOfDialysisPerGroup[iGroup]);
                tCloseness = CalcTClosenessNumVal(probabilitiesTimeOfDialysisPerGroup, probabilitiesTimeOfDialysisPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesTimeOfDialysisPerGroup.Clear();

                // Make P time of bloodflow and calc t-closeness.
                probabilitiesBloodflowPerGroup = CalcProbabilitiesOfAttributePerGroup(probabilitiesBloodflowPerTable.Count(),
                                                                numOfNumericValuePerBloodflowPerGroup[iGroup]);
                tCloseness = CalcTClosenessNumVal(probabilitiesBloodflowPerGroup, probabilitiesBloodflowPerTable);
                tClosenessPerGroupPerSA.Add(tCloseness);
                probabilitiesBloodflowPerGroup.Clear();

                _tClosenessPerGroup.Add(iGroup, new List<double>(tClosenessPerGroupPerSA));
                tClosenessPerGroupPerSA.Clear();
            }

            return anonymousDataSet;
        }

        /// <summary>
        /// Generalisiert die sensitiven Attribute nach vorgegeben Muster bzw. Schwellwerten.
        /// </summary>
        /// <param name="anonymousDataset">Die spezifischeren Werte werden durch die Generalisierungen.</param>
        private void GeneralizeSAWithPattern( ref PatientData patient)
        {
            IEnumerator<int> generalizedValue = null;
                // Generalisierung für TacUrea.
                generalizedValue = CAnonymizer.TACUreaGeneralization.GetEnumerator();
                do
                {
                    generalizedValue.MoveNext();
                } while (patient.TACUrea > generalizedValue.Current);
                {
                }
                patient.TACUrea = (double)generalizedValue.Current;

                // Generalisierung für Dialysezeit.
                generalizedValue = CAnonymizer.TimeOfDialysisGeneralization.GetEnumerator();
                do
                {
                    generalizedValue.MoveNext();
                } while (patient.TimeOfDialysis > generalizedValue.Current);
                {
                }
                patient.TimeOfDialysis = generalizedValue.Current;

                // Generalisierung für Blutfluss.
                generalizedValue = CAnonymizer.BloodflowGeneralization.GetEnumerator();
                do
                {
                    generalizedValue.MoveNext();
                } while (patient.Bloodflow > generalizedValue.Current);
                {
                }
                patient.Bloodflow = generalizedValue.Current;
        }

        private List<double> CalcProbabilitiesOfAttributePerGroup(int lenQ, Dictionary<string, int> numOfAttribute)
        {
            var probabilitiesOfAttributePerGroup = new List<double>();

             // Make P diagnosis and calc t-closeness.
                List<string> diagnosis = numOfAttribute.Keys.ToList<string>();
            double pValue = 0;
            int lenP = diagnosis.Count();
            int sumOfOccurence = numOfAttribute.Values.Sum();

            for (int j = 0; j < lenQ; j++)
            {
                // Häufigkeit des jeweiligen Attributwertes/ Gesamtanzahl Attributwerte bezogen auf qia.
                if (j < lenP)
                {
                    pValue = Convert.ToDouble(numOfAttribute[diagnosis[j]]) / sumOfOccurence;
                    probabilitiesOfAttributePerGroup.Add(pValue);
                }
                else
                {
                    probabilitiesOfAttributePerGroup.Add(0);
                }
            }
            return probabilitiesOfAttributePerGroup;
        }

        private List<double> CalcProbabilitiesOfAttributePerGroup(int lenQ, Dictionary<double, int> numOfAttribute)
        {
            var probabilitiesOfAttributePerGroup = new List<double>();

            // Make P diagnosis and calc t-closeness.
            List<double> value = numOfAttribute.Keys.ToList<double>();
            double pValue = 0;
            int lenP = value.Count();
            int sumOfOccurence = numOfAttribute.Values.Sum();

            for (int j = 0; j < lenQ; j++)
            {
                // Häufigkeit des jeweiligen Attributwertes/ Gesamtanzahl Attributwerte bezogen auf qia.
                if (j < lenP)
                {
                    pValue = Convert.ToDouble(numOfAttribute[value[j]]) / sumOfOccurence;
                    probabilitiesOfAttributePerGroup.Add(pValue);
                }
                else
                {
                    probabilitiesOfAttributePerGroup.Add(0);
                }
            }
            return probabilitiesOfAttributePerGroup;
        }

        private List<double> CalcProbabilitiesOfAttributePerGroup(int lenQ, Dictionary<int, int> numOfAttribute)
        {
            var probabilitiesOfAttributePerGroup = new List<double>();

            // Make P diagnosis and calc t-closeness.
            List<int> value = numOfAttribute.Keys.ToList<int>();
            double pValue = 0;
            int lenP = value.Count();
            int sumOfOccurence = numOfAttribute.Values.Sum();

            for (int j = 0; j < lenQ; j++)
            {
                // Häufigkeit des jeweiligen Attributwertes/ Gesamtanzahl Attributwerte bezogen auf qia.
                if (j < lenP)
                {
                    pValue = Convert.ToDouble(numOfAttribute[value[j]]) / sumOfOccurence;
                    probabilitiesOfAttributePerGroup.Add(pValue);
                }
                else
                {
                    probabilitiesOfAttributePerGroup.Add(0);
                }
            }
            return probabilitiesOfAttributePerGroup;
        }

        private List<double> CalcProbabilitiesOfAttributePerTable(Dictionary<string, int> numOfAttribut, int datasize)
        {
            var probabilitiesOfAttributesPerTable = new List<double>();
            foreach (var occurence in numOfAttribut.Values)
            {
                probabilitiesOfAttributesPerTable.Add(Convert.ToDouble(occurence) / datasize);
            }
            return probabilitiesOfAttributesPerTable;
        }

        private List<double> CalcProbabilitiesOfAttributePerTable(Dictionary<double, int> numOfAttribut, int datasize)
        {
            var probabilitiesOfAttributesPerTable = new List<double>();
            foreach (var occurence in numOfAttribut.Values)
            {
                probabilitiesOfAttributesPerTable.Add(Convert.ToDouble(occurence) / datasize);
            }
            return probabilitiesOfAttributesPerTable;
        }

        private List<double> CalcProbabilitiesOfAttributePerTable(Dictionary<int, int> numOfAttribut, int datasize)
        {
            var probabilitiesOfAttributesPerTable = new List<double>();
            foreach (var occurence in numOfAttribut.Values)
            {
                probabilitiesOfAttributesPerTable.Add(Convert.ToDouble(occurence) / datasize);
            }
            return probabilitiesOfAttributesPerTable;
        }

        private void CountOccurenceOfAttributePerTable (ref Dictionary<string, int> numOfAttribute, PatientData patient)
        {
            if (numOfAttribute.ContainsKey(patient.Diagnosis))
            {
                numOfAttribute[patient.Diagnosis]++;
            }
            else
            {
                numOfAttribute.Add(patient.Diagnosis, 1);
            }
        }

        private void CountOccurenceOfAttributePerTable(ref Dictionary<double, int> numOfAttribute, PatientData patient, CAnonymizer.Attribute attribute)
        {
            switch (attribute)
            {
                case CAnonymizer.Attribute.KtV:
                    if (numOfAttribute.ContainsKey(patient.KtV))
                    {
                        numOfAttribute[patient.KtV]++;
                    }
                    else
                    {
                        numOfAttribute.Add(patient.KtV, 1);
                    }
                    break;
                case CAnonymizer.Attribute.PCR:
                    if (numOfAttribute.ContainsKey(patient.PCR))
                    {
                        numOfAttribute[patient.PCR]++;
                    }
                    else
                    {
                        numOfAttribute.Add(patient.PCR, 1);
                    }
                    break;
                case CAnonymizer.Attribute.TACUrea:
                    if (numOfAttribute.ContainsKey(patient.TACUrea))
                    {
                        numOfAttribute[patient.TACUrea]++;
                    }
                    else
                    {
                        numOfAttribute.Add(patient.TACUrea, 1);
                    }
                    break;
            }
        }

        private void CountOccurenceOfAttributePerTable(ref Dictionary<int, int> numOfAttribute, PatientData patient, CAnonymizer.Attribute attribute)
        {
            switch (attribute)
            {
                case CAnonymizer.Attribute.TimeOfDialysis:
                    if (numOfAttribute.ContainsKey(patient.TimeOfDialysis))
                    {
                        numOfAttribute[patient.TimeOfDialysis]++;
                    }
                    else
                    {
                        numOfAttribute.Add(patient.TimeOfDialysis, 1);
                    }
                    break;
                case CAnonymizer.Attribute.Bloodflow:
                    if (numOfAttribute.ContainsKey(patient.Bloodflow))
                    {
                        numOfAttribute[patient.Bloodflow]++;
                    }
                    else
                    {
                        numOfAttribute.Add(patient.Bloodflow, 1);
                    }
                    break;
            }
        }

        private void CountOccurenceOfAttributePerGroup(string group, ref Dictionary<string, Dictionary<string, int>> numOfAttribute,
                                                       PatientData patient)
        {
            if (numOfAttribute.ContainsKey(group))
            {
                // Diagnosen.
                if (numOfAttribute[group].ContainsKey(patient.Diagnosis))
                {
                    numOfAttribute[group][patient.Diagnosis]++;
                }
                else
                {
                    numOfAttribute[group].Add(patient.Diagnosis, 1);
                }
            }
            else
            {
                // Qia nicht vorhanden. Neue Werte registrieren.
                var diag = new Dictionary<string, int>();

                diag.Add(patient.Diagnosis, 1);
                numOfAttribute.Add(group, diag);
            }
        }

        private void CountOccurenceOfAttributePerGroup(string group, ref Dictionary<string, Dictionary<double, int>> numOfAttribute, 
                                                       PatientData patient, CAnonymizer.Attribute attribute)
        {
            if (numOfAttribute.ContainsKey(group))
            {
                switch (attribute)
                {
                    case CAnonymizer.Attribute.KtV:
                        if (numOfAttribute[group].ContainsKey(patient.KtV))
                        {
                            numOfAttribute[group][patient.KtV]++;
                        }
                        else
                        {
                            numOfAttribute[group].Add(patient.KtV, 1);
                        }
                        break;
                    case CAnonymizer.Attribute.PCR:
                        if (numOfAttribute[group].ContainsKey(patient.PCR))
                        {
                            numOfAttribute[group][patient.PCR]++;
                        }
                        else
                        {
                            numOfAttribute[group].Add(patient.PCR, 1);
                        }
                        break;
                    case CAnonymizer.Attribute.TACUrea:
                        if (numOfAttribute[group].ContainsKey(patient.TACUrea))
                        {
                            numOfAttribute[group][patient.TACUrea]++;
                        }
                        else
                        {
                            numOfAttribute[group].Add(patient.TACUrea, 1);
                        }
                        break;
                }
            }
            else
            {
                var val = new Dictionary<double, int>();
                // Group nicht vorhanden, registrieren. 
                switch (attribute)
                {
                    case CAnonymizer.Attribute.KtV:
                        val.Add(patient.KtV, 1);
                        break;
                    case CAnonymizer.Attribute.PCR:
                        val.Add(patient.PCR, 1);
                        break;
                    case CAnonymizer.Attribute.TACUrea:
                        val.Add(patient.TACUrea, 1);
                        break;
                }
                numOfAttribute.Add(group, val);
            }
        }

        private void CountOccurenceOfAttributePerGroup(string group, ref Dictionary<string, Dictionary<int, int>> numOfAttribute, 
                                                       PatientData patient, CAnonymizer.Attribute attribute)
        {
            if (numOfAttribute.ContainsKey(group))
            {
                switch (attribute)
                {
                    case CAnonymizer.Attribute.TimeOfDialysis:
                        if (numOfAttribute[group].ContainsKey(patient.TimeOfDialysis))
                        {
                            numOfAttribute[group][patient.TimeOfDialysis]++;
                        }
                        else
                        {
                            numOfAttribute[group].Add(patient.TimeOfDialysis, 1);
                        }
                        break;
                    case CAnonymizer.Attribute.Bloodflow:
                        if (numOfAttribute[group].ContainsKey(patient.Bloodflow))
                        {
                            numOfAttribute[group][patient.Bloodflow]++;
                        }
                        else
                        {
                            numOfAttribute[group].Add(patient.Bloodflow, 1);
                        }
                        break;
                }
            }
            else
            {
                var val = new Dictionary<int, int>();
                switch (attribute)
                {
                    case CAnonymizer.Attribute.TimeOfDialysis:
                        val.Add(patient.TimeOfDialysis, 1);
                        break;
                    case CAnonymizer.Attribute.Bloodflow:
                        val.Add(patient.Bloodflow, 1);
                        break;
                }
                numOfAttribute.Add(group, val);
            }
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
