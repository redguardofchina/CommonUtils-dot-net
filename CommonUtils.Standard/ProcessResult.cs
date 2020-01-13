namespace CommonUtils
{
    public class ProcessResult
    {
        public string Command { get; set; }
        public string Result { get; set; } = "";
        public bool HasError { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            var message = Command + "\r\n\r\n" + Result;
            if (HasError)
                message += "↓↓↓↓↓↓↓↓↓error↓↓↓↓↓↓↓↓↓\r\n" + Error;
            return message;
        }
    }
}
