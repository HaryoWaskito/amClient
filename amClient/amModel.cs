using System;

namespace amClient
{
    public class amModel
    {
        public string amModelId { get; set; }
        public int SessionID { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public string InputKey { get; set; }
        public long KeyStrokeCount { get; set; }
        public long MouseClickCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsSuccessSendToServer { get; set; }
    }
}
