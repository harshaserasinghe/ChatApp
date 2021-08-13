namespace Chat.Common.Models
{
    public class SupportRequestQueue
    {
        public SupportRequestQueue(int count, int capacity)
        {
            Count = count;
            Capacity = capacity;
        }

        public int Count { get; set; }
        public int Capacity { get; set; }
    }
}
