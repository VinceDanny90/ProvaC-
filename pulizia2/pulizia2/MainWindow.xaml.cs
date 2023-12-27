using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

namespace pulizia2
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string version = "1.0.0";
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;

        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            CheckActu();
            GetDate();
        }
        //calcolare peso file
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di)); 
        }
        //Cancellazione
        public void ClearTempData(DirectoryInfo dir)
        {
            foreach(FileInfo file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                    
                }
                catch(Exception ex)  
                {
                    continue;
                }
            }

            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                try
                {
                    di.Delete(true);
                    Console.WriteLine(di.FullName);     
                }
                catch(Exception ex)
                {
                    continue;
                }
            }
            
        }

        private void Button_Max_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();
        }

        private void Button_Misto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("to do cronologia", "Messaggio", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        /// <summary>
        /// Sito WEB 
        /// </summary>
        /// <param name="sender"> Mettere un sito</param>
        /// <param name="e"></param>
        private void Button_Web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("www.google.it")
                {
                    UseShellExecute = true
                });
            } catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
           
        }

        private void Button_Analisi_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        //calcolo peso
        public void AnalyseFolders()
        {
            Console.WriteLine("Inizio analisi...");
            long totalSize = 0;

            try
            {
                totalSize += DirSize(winTemp) / 1000000;
                totalSize += DirSize(appTemp) / 1000000;
            }catch(Exception ex)
            {
                Console.WriteLine("Impossibile accedere : " + ex.Message);
            }

            spazio.Content = totalSize + "mb";
            titolo.Content = "Analisi Effettuata !";
            data.Content = DateTime.Today;
            SaveDate(); 

        }
        // PULIZIA
        private void Button_Pulisci_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Pulizia in corso...");
            pulizia.Content = "Pulizia in corso";

            Clipboard.Clear();

            try
            {
                ClearTempData(winTemp);
            }catch(Exception ex)
            {
                Console.WriteLine("Errore : " +ex.Message);
            }

            try
            {
                ClearTempData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore : " + ex.Message);
            }

            pulizia.Content = "Pulizia Terminata";
            titolo.Content = "Pulizia Effettuata";
            spazio.Content = "0 mb";

        }
        /// <summary>
        /// check attualità
        /// </summary>
        public void CheckActu()
        {
            string url = "sito locale";
            using(WebClient client = new WebClient())
            {
                string actu = client.DownloadString(url);
                if(actu != String.Empty)
                {
                    actuTxt.Content = actu;
                    actuTxt.Visibility = Visibility.Visible;
                    banner.Visibility = Visibility.Visible;
                }
            }
        }
        /// <summary>
        /// Verifica versione
        /// </summary>
        public void CheckVersion()
        {
            string url = "sito locale/version";
            using (WebClient client = new WebClient())
            {
                string v = client.DownloadString(url);
                if (version != v)
                {
                    MessageBox.Show("Un aggiornamento è disponbiile", "Messaggio", MessageBoxButton.OK, MessageBoxImage.Information);

                } else
                {
                    MessageBox.Show("Hai lultima versione", "Messaggio", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
        }
        //salvataggio data
        public void SaveDate()
        {
            string data = DateTime.Today.ToString();
            File.WriteAllText("data.txt",data);
        }
        public void GetDate()
        {
            string dateFile = File.ReadAllText("data.txt");
            if(dateFile != String.Empty)
            {
                data.Content = dateFile;
            }
        }
    }
}
