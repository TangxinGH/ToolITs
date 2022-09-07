using System.Globalization;
namespace ToolITs
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            CultureInfo en = new CultureInfo("en-US");
            // string[] mon =   { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",  ""};// "MMM";
            // en.DateTimeFormat.MonthDayPattern = "dd-MMM"  ; 
            // en.DateTimeFormat.FullDateTimePattern = "dd-MMM-yy HH:mm:ss";
            en.DateTimeFormat.ShortDatePattern = "dd-MMM-yy";
            en.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            Thread.CurrentThread.CurrentCulture = en;
            // string test =  DateTime.Now.ToString("dd-MMM-yy HH:mm:ss") ;
            // System.Console.WriteLine( DateTime.Now.ToString());
            // en.DateTimeFormat.DateSeparator = "-";;// .RFC1123Pattern  = "R";
            Application.Run(new Form1());
      
        }
    }
}