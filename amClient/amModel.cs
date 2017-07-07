using System;

namespace amClient
{
    public class amModel
    {
        public string amModelId { get; set; }
        //public string appProcessName { get; set; }
        public string appExePath { get; set; }
        public string appExeName { get; set; }
        public string userID { get; set; }
        public string InputType { get; set; }
        //public string KeyLogCatch { get; set; }
        public long InputClickedCounter { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
