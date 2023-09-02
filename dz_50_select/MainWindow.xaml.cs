using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Data.SqlClient;
namespace dz_50_select
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxConcurrentQueries = 5; 
        private SemaphoreSlim semaphore = new SemaphoreSlim(MaxConcurrentQueries);

      
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < 50; i++) 
            {
                tasks.Add(ExecuteQueryAsync(i, "SELECT name FROM table_1"));
            }

            await Task.WhenAll(tasks);

            MessageBox.Show("Все запросы завершены.");
        }

        private async Task ExecuteQueryAsync(int queryNumber, string querie)
        {
            await semaphore.WaitAsync(); 
            try
            {
                string connectionString = "Server=DESKTOP-HGDM54L;Database=person;Trusted_Connection=True;"; 
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(querie, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows) 
                            {
                                while (reader.Read())
                                {
                                    
                                   TbNamePerson.Text += reader.GetString(0) + Environment.NewLine;
                                    
                                   
                                    
                                }
                            }
                           
                        }
                    }
                }

                UpdateTb($"Запрос {queryNumber + 1} завершен.\n");
            }
            finally
            {
                semaphore.Release(); 
            }
        }

        private void UpdateTb(string message)
        {
            TbTask.Text += message;
        }
    }
}
