namespace WebDataEntry.Web.Models
{
    public class UpdateEntryRequest
    {
        public DateEntry OriginalDate { get; set; }
        public Entry Entry { get; set; }
    }
}