#define __DEBUG__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SQLite;

namespace GroupNotificationRemoverForWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private string DatabaseURL = "%USERPROFILE%\\AppData\\Local\\Microsoft\\Windows\\Notifications\\wpndatabase.db";
        private SQLiteConnection conn;

        public MainWindow()
        {
#if __DEBUG__
            ConsoleAllocator.ShowConsoleWindow();
            Console.WriteLine("Console mode on.");
            test();
#endif
            InitializeComponent();
        }

        private void test()
        {
            // get database path
            var DatabaseURL = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                                            "Microsoft\\Windows\\Notifications\\wpndatabase.db");
            Console.WriteLine(DatabaseURL);

            // connect
            this.conn = new SQLiteConnection("Data Source="+DatabaseURL);
            try
            {
                conn.Open();

                // query
                // var command = new SQLiteCommand("select * from NotificationHandler", this.conn);
                var command = new SQLiteCommand("select * from HandlerAssets", this.conn);
                var result = command.ExecuteReader();
                var schema = result.GetValues();
                foreach(var v in schema)
                {
                    // Console.WriteLine(v);
                    Console.Write(v+"\t");
                }
                Console.Write("\n");

#region get info of groupnotifications
                var groupNotifications = new List<(int HandlerId, string AssetKey, string AssetValue)>();
                
                while(result.Read()) {
                    foreach(string v in schema)
                    {
                        Console.Write(result[v]+"\t");
                        if(v == "AssetKey" && result[v].ToString() == "DisplayName")
                        {
                            groupNotifications.Add((int.Parse(result["HandlerId"].ToString()), result["AssetKey"].ToString(), result["AssetValue"].ToString()));
                        }

                    }
                    Console.WriteLine("");
                    // Console.WriteLine(result);
                }
                result.Close();
#endregion

#region list group notification
                Console.WriteLine("[Group Notification]");
                foreach(var noti in groupNotifications)
                    Console.WriteLine($"{noti.HandlerId}\t{noti.AssetKey}\t{noti.AssetValue}");
#endregion

#region remove group notification
                Console.WriteLine("[Remove Group Notification]");
                foreach(var noti in groupNotifications)
                {
                    // 7. Enter and execute entry: "delete from HandlerSettings where HandlerID = X".
                    // 8. Enter and execute entry: "delete from Notification where HandlerID = X".
                    // 9. Enter and execute entry: "delete from WNSPushChannel where HandlerID = X".
                    // 10. Enter and execute entry: "delete from NotificationHandler where HandlerID = X".
                }
                // 11. Save file: "wpndatabase.db".
#endregion

                // disconnect
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
