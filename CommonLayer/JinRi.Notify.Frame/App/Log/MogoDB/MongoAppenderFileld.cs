using log4net.Layout;

namespace JinRi.Notify.Frame
{
	public class MongoAppenderFileld
	{
		public string Name { get; set; }
		public IRawLayout Layout { get; set; }
	}
}