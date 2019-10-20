// https://www.tenforums.com/general-support/62662-how-clear-applications-get-notifications-these-senders.html

using System;
using System.Collections.Generic;
using System.Windows;

using System.Data.SQLite;

namespace GroupNotificationRemoverForWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string DatabasePath = "Microsoft\\Windows\\Notifications\\wpndatabase.db"; // "%USERPROFILE%\\AppData\\Local\\Microsoft\\Windows\\Notifications\\wpndatabase.db";
        private List<string> groupList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
            Update();

            removeButton.Click += RemoveProcess;
        }

        private void RemoveProcess(object sender, RoutedEventArgs args)
        {
            var items = GroupListView.SelectedItems;
            var itemId = new List<int>();

            try
            {
                var DatabaseURL = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.DatabasePath);
                var conn = new SQLiteConnection("Data Source="+DatabaseURL);
                conn.Open();

                var command = new SQLiteCommand("select * from HandlerAssets", conn);
                var result = command.ExecuteReader();
                var schema = result.GetValues();

                while(result.Read()) {
                    foreach(string v in schema)
                        if(v == "AssetKey" && result[v].ToString() == "DisplayName")
                        {
                            var assetValue = result["AssetValue"];
                            if(items.Contains(assetValue))
                                itemId.Add(int.Parse(result["HandlerId"].ToString()));
                        }
                }

                foreach(var id in itemId)
                {
                    command = new SQLiteCommand($"delete from HandlerSettings where HandlerID = {id}", conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand($"delete from Notification where HandlerID = {id}", conn);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand($"delete from WNSPushChannel where HandlerID = {id}", conn);
                    command.ExecuteNonQuery();
                    // command = new SQLiteCommand($"delete from NotificationHandler where HandlerID = {id}", conn); // no HandlerID
                    // command.ExecuteNonQuery();
                    command = new SQLiteCommand($"delete from HandlerAssets where HandlerID = {id}", conn);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            foreach(string item in items)
                groupList.Remove(item);
            GroupListView.Items.Refresh();
        }

        private void Update()
        {
            var DatabaseURL = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.DatabasePath);
            var conn = new SQLiteConnection("Data Source="+DatabaseURL);

            try
            {
                conn.Open();

                var command = new SQLiteCommand("select * from HandlerAssets", conn);
                var result = command.ExecuteReader();
                var schema = result.GetValues();

                while(result.Read()) {
                    foreach(string v in schema)
                        if(v == "AssetKey" && result[v].ToString() == "DisplayName")
                            groupList.Add(result["AssetValue"].ToString());
                }
                conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            GroupListView.Items.Refresh();
        }

        private void Initialize()
        {
            this.GroupListView.ItemsSource = groupList;
        }
    }
}
