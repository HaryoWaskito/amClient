using System;

namespace amClient
{
    public class amModel
    {
        public string amModelId { get; set; }
        public int sessionID { get; set; }
        public string ApplicationName { get; set; }
        public string InputType { get; set; }
        public long KeyStrokeCount { get; set; }
        public long MouseClickCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSuccessSendToServer { get; set; }
    }
}
