using System;

namespace PatientDataGenerator
{
    public class PatientData
    {
       private string _lastame;
       private string _firstname;
       private DateTime _birth;
       private string _sex;
       private string _diagnosis;
       private double _ktv;
       private double _pcr;
       private double _tacUrea;
       private int _timeOfdialysis;
       private int _bloodflow;

        public string Lastname
        {
            get{ return _lastame; }
            set { _lastame = value; }
        }
        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }
        public DateTime Birth
        {
            get { return _birth; }
            set { _birth = value; }
        }

        public string Sex
        {
            get { return _sex.ToString(); }
            set { _sex = value; }
        }
        public string Diagnosis
        {
            get { return _diagnosis; }
            set { _diagnosis = value; }
        }
        public Double KtV
        {
            get{ return _ktv; }
            set { _ktv = value; }
           
        }
        public Double PCR
        {
            get { return _pcr; }
            set { _pcr = value; }
        }
        public Double TACUrea
        {
            get { return _tacUrea; }
            set { _tacUrea = value; }
        }
        public int TimeOfDialysis
        {
            get { return _timeOfdialysis; }
            set { _timeOfdialysis = value; }
        }
        public int Bloodflow
        {
            get { return _bloodflow; }
            set { _bloodflow = value; }
        }
        public PatientData(string lastname, string firstname, DateTime birth, string sex, string diagnosis, 
                           double ktv, double pcr, double tacUrea, int timeOfDialysis, int bloodflow)
        {
            _lastame = lastname;
            _firstname = firstname;
            _birth = birth;
            _sex = sex;
            _diagnosis = diagnosis;
            _ktv = ktv;
            _pcr = pcr;
            _tacUrea = tacUrea;
            _timeOfdialysis = timeOfDialysis;
            _bloodflow = bloodflow;
        }
        public PatientData()
        {
        }

        public enum Attribute: ushort
        {
            Lastname = 0,
            Firstname = 1,
            Birth = 2,
            Sex = 3,
            Diagnosis = 4,
            KtV = 5,
            PCR = 6,
            TacUrea = 7,
            TimeOfDialysis = 8,
            BloodFlow = 9
        }
    }
}
