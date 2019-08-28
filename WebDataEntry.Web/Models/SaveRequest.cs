namespace WebDataEntry.Web.Models
{
	public class SaveRequest
	{
		public bool OldEntriesJson { get; set; }
		public bool LatestEntriesJson { get; set; }
		public bool WhoopsJson { get; set; }
		public bool AllJson { get; set; }
	}
}