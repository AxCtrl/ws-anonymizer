using System;
using System.Collections.Generic;
using System.Text;

namespace SKAT_Anonymizer
{
    public static class CAnonymizer
    {
        public const string AgeCriteriaYounger = " < ";
        public const string AgeCriteriaBetween = " - ";
        public const string AgeCriteriaOlder = " > ";
        public const string AgeCriteriaEqual = "=";

        public const string SexM = "m";
        public const string SexW = "w";

        
        public const string ColID = "ID";
        public const string ColLastname = "Nachname";
        public const string ColFirstname = "Vorname";
        public const string ColBirth = "Geburtsdatum";
        public const string ColAge = "Alter";
        public const string ColSex = "Geschlecht";
        public const string ColDiag = "Diagnose";
        public const string ColKtV = "Kt pro V";
        public const string ColPCR = "PCR g pro kg pro Tag";
        public const string ColTACUrea = "TAC Urea mg pro dl";
        public const string ColTimeOfDialysis = "Dialysezeit min";
        public const string ColBloodFlow = "Blutfluss ml pro min";

        public const string ColGroupDescription = "Group";
        public const string ColGroupSizeK = "K";
        public const string ColTCloseness = "t ";
        public const string ColTClosenessGroup = ColTCloseness + "AEK";

        public const string SortOrderASC = "ASC";
        public const string SortOrderDESC = "DESC";

        public const string MsgUnvailDatasize = "Bitte gültigen Wert eingeben 0 - 10.000.";

        public const int Eighteen = 18;
        public const int Thirty = 30;
        public const int ThirtyOne = 31;
        public const int Fifty = 50;
        public const int FiftyOne = 51;
        public const int Seventy = 70;

        public const double MinKtV = 0.8;
        public const double MaxKtV = 2.2;
        public const double MinPCR = 0.5;
        public const double MaxPCR = 2.5;
        public const double MinTACUrea = 15.0;
        public const double MaxTACUrea = 100.0;
        public const int MinTimeOfDialysis = 100;
        public const int MaxTimeOfDialysis = 360;
        public const int MinBloodflow = 100;
        public const int MaxBloodflow = 500;
        public const double ThresholdGroup = 0.6;

        public const string Suppressed = "-";

        public static readonly List<int> TACUreaGeneralization = new List<int>{ 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 85, 90, 100 };
 
        public static readonly List<int> TimeOfDialysisGeneralization = new List<int>{ 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 
                                                                                       210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 
                                                                                       310, 320, 330, 340, 350, 360 };
        public static readonly List<int> BloodflowGeneralization = new List<int>{ 150, 200, 250, 300, 350, 400, 450, 500 };

        public enum Attribute : ushort
        {
            Age = 0,
            Sex = 1,
            Diagnosis = 2,
            KtV = 3,
            PCR = 4,
            TACUrea = 5,
            TimeOfDialysis = 6,
            Bloodflow = 7
        }

        public enum SA: ushort
        {
            Diagnosis = 0,
            KtV = 1,
            PCR = 2,
            TACUrea = 3,
            TimeOfDialysis = 4,
            Bloodflow = 5
        }
        public enum Aggregated : ushort
        {
            KtV = 0,
            PCR = 1,
            TACUrea = 2,
            TimeOfDialysis = 3,
            Bloodflow = 4
        }
    }
}
