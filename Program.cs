using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using System.Drawing;
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
            #region timeformat config
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

            #endregion


            Application.Run(new Form1());
      
        }
    }

    public  static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 类似于几种单例模式；抢占式；
        /// https://www.cnblogs.com/michaelxu/archive/2007/03/29/693401.html
        /// https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/static-constructors
        //1、静态构造函数既没有访问修饰符，也没有参数。因为是.NET调用的，所以像public和private等修饰符就没有意义了。

        //2、是在创建第一个类实例或任何静态成员被引用时，.NET将自动调用静态构造函数来初始化类，也就是说我们无法直接调用静态构造函数，也就无法控制什么时候执行静态构造函数了。

        //3、一个类只能有一个静态构造函数。

        //4、无参数的构造函数可以与静态构造函数共存。尽管参数列表相同，但一个属于类，一个属于实例，所以不会冲突。

        //5、最多只运行一次。

        //6、静态构造函数不可以被继承。

        //7、如果没有写静态构造函数，而类中包含带有初始值设定的静态成员，那么编译器会自动生成默认的静态构造函数。
        /// </summary>
        /// 在此示例中, TypeInitializationException引发了异常, 因为未能加载程序集。 如果静态构造函数尝试打开数据文件 (如配置文件、XML 文件或包含序列化数据的文件), 则也可能会引发异常。
         static ServiceProviderFactory()
        {
           // 饿汉式

            // 1. crete a service collection for DI
            var serviceCollection = new ServiceCollection();
            // 2. Build a configuration 
            IConfiguration configuration;
            //Generally speaking, AppContext.BaseDirectory should point to the folder containing the managed entry point assembly, not the CLR host.It is used here to locate asset which are expected to be next to that file. What host are you using that causes it to point somewhere else?
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName?? Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build(); //
            //3. Add the configuration to the service collection 
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton<MyService>();

            // start service
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
        #region “双重锁定”
        //// 定义一个标识确保线程同步
        //private static readonly object locker = new object();

        //// 定义私有构造函数，使外界不能创建该类实例
        //private Singleton()
        //{
        //}

        ///// <summary>
        ///// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        ///// </summary>
        ///// <returns></returns>
        //public static Singleton GetInstance()
        //{
        //    // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
        //    // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
        //    // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
        //    // 双重锁定只需要一句判断就可以了
        //    if (uniqueInstance == null)
        //    {
        //        lock (locker)
        //        {
        //            // 如果类的实例不存在则创建，否则直接返回
        //            if (uniqueInstance == null)
        //            {
        //                uniqueInstance = new Singleton();
        //            }
        //        }
        //    }
        //    return uniqueInstance;
        //} 
        #endregion
    }

}