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

        public const double minKtV = 0.8;
        public const double maxKtV = 2.5;
        public const double minPCR = 0.5;
        public const double maxPCR = 3.0;
        public const double minTACUrea = 15.0;
        public const double maxTACUrea = 200.0;
        public const int minTimeOfDialysis = 100;
        public const int maxTimeOfDialysis = 350;
        public const int minBloodflow = 100;
        public const int maxBloodflow = 500;

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

    }
}
