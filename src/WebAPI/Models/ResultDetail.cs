namespace DesignPatternSamples.WebAPI.Models
{
    public class ResultDetail
    {
        public string Message { get; }

        public ResultDetail(string message)
        {
            Message = message;
        }
    }
}
