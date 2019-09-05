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
using System.IO;

namespace Sched
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FilePath = ".\\input.txt";
        List<string> mer = new List<string> {"am", "pm"};
        Dictionary<string, Work> Dict_Work = new Dictionary<string, Work>();
        Dictionary<string, Recreation> Dict_Recr = new Dictionary<string, Recreation>();
        int Starting_Energy = 100;
        int Quanta = 15;
        int SH, SM;
        int EH, EM;
        string SMer, EMer;
        string STime, ETime;

        public MainWindow()
        {
            InitializeComponent();
            Combo_S.ItemsSource = mer;
            Combo_S.SelectedIndex = 0;
            Combo_E.ItemsSource = mer;
            Combo_E.SelectedIndex = 1;
            UpdateListBoxes();
        }

        private void UpdateListBoxes()
        {
            ListBox_Work.ItemsSource = Dict_Work.Keys.ToList();
            ListBox_Recr.ItemsSource = Dict_Recr.Keys.ToList();
            UpdateMinimumDuration();
        }

        private bool UpdateSettings()
        {
            try
            {
                Starting_Energy = Convert.ToInt32(TextBox_StartingEnergy.Text);
                Quanta = Convert.ToInt32(TextBox_Quanta.Text);
                SH = Convert.ToInt32(TextBox_SHr.Text);
                SM = Convert.ToInt32(TextBox_SMin.Text);
                EH = Convert.ToInt32(TextBox_EHr.Text);
                EM = Convert.ToInt32(TextBox_EMin.Text);
                SMer = Combo_S.SelectedValue.ToString();
                EMer = Combo_E.SelectedValue.ToString();
                RectifyTime();
                STime = SH.ToString().PadLeft(2, '0') + ":" + SM.ToString().PadLeft(2, '0') + SMer;
                ETime = EH.ToString().PadLeft(2, '0') + ":" + EM.ToString().PadLeft(2, '0') + EMer;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpdateSettingsView()
        {
            TextBox_StartingEnergy.Text = Starting_Energy.ToString();
            TextBox_Quanta.Text = Quanta.ToString();
            TextBox_SHr.Text = SH.ToString().PadLeft(2, '0');
            TextBox_SMin.Text = SM.ToString().PadLeft(2, '0');
            TextBox_EHr.Text = EH.ToString().PadLeft(2, '0');
            TextBox_EMin.Text = EM.ToString().PadLeft(2, '0');
        }

        private void RectifyTime()
        {
            if (SH > 12) SH = 12;
            else if (SH < 1) SH = 1;
            if (EH > 12) EH = 12;
            else if (EH < 1) EH = 1;
            if (SM > 59) SM = 59;
            else if (SM < 0) SM = 0;
            if (EM > 59) EM = 59;
            else if (EM < 0) EM = 0;
        }

        private void RectifyInputs()
        {
            UpdateSettings();
            UpdateSettingsView();
            RectifyWorks();
            UpdateListBoxes();
        }

        private void RectifyWorks()
        {
            foreach (var key in Dict_Work.Keys)
            {
                if (Dict_Work[key].Duration < Quanta) Dict_Work[key].Duration = Quanta;
                int slotcount = Convert.ToInt32(Math.Round(Convert.ToDouble(Dict_Work[key].Duration) / Convert.ToDouble(Quanta)));
                Dict_Work[key].Duration = slotcount * Quanta;
                int energy = Convert.ToInt32(Math.Round(Convert.ToDouble(Dict_Work[key].Energy) / Convert.ToDouble(slotcount))) * slotcount;
                Dict_Work[key].Energy = energy;
            }
            ListBox_Work.SelectedItem = null;
            ListBox_Recr.SelectedItem = null;
            ClearWorkBoxes();
            ClearRecrBoxes();
        }

        private int GetMinimumDuration()
        {
            int totalduration = 0;
            foreach(var key in Dict_Work.Keys)
            {
                totalduration += Dict_Work[key].Duration;
            }
            totalduration += Dict_Recr.Keys.Count * Quanta;
            return totalduration;
        }

        private void UpdateMinimumDuration()
        {
            int mindur = GetMinimumDuration();
            Label_MinH.Content = (mindur / 60).ToString() + "HR";
            Label_MinM.Content = (mindur % 60).ToString() + "MIN";
        }

        private bool AddWork()
        {
            int energy;
            int duration;
            int priority;
            string name = TextBox_WorkName.Text;
            if(name == null || name == "") return false;
            try
            {
                energy = Convert.ToInt32(TextBox_WorkEnergy.Text);
                duration = Convert.ToInt32(TextBox_WorkDuration.Text);
                priority = Convert.ToInt32(Slider_WorkPriority.Value);
                Dict_Work.Add(name, new Work(name, energy, duration, priority));
                UpdateListBoxes();
                ListBox_Work.SelectedItem = name;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool AddRecr()
        {
            int energy;
            string name = TextBox_RecrName.Text;
            if (name == null || name == "") return false;
            try
            {
                energy = Convert.ToInt32(TextBox_RecrEnergy.Text);
                Dict_Recr.Add(name, new Recreation(name, energy));
                UpdateListBoxes();
                ListBox_Recr.SelectedItem = name;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool RemoveWork()
        {
            try
            {
                string name = TextBox_WorkName.Text;
                bool rtrn = Dict_Work.Remove(name);
                UpdateListBoxes();
                ClearWorkBoxes();
                return rtrn;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        private bool RemoveRecr()
        {
            try
            {
                string name = TextBox_RecrName.Text;
                bool rtrn = Dict_Recr.Remove(name);
                UpdateListBoxes();
                ClearRecrBoxes();
                return rtrn;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ClearWorkBoxes()
        {
            TextBox_WorkName.Text = "";
            TextBox_WorkEnergy.Text = "";
            TextBox_WorkDuration.Text = "";
            Slider_WorkPriority.Value = 0;
        }

        private void ClearRecrBoxes()
        {
            TextBox_RecrName.Text = "";
            TextBox_RecrEnergy.Text = "";
        }

        private void OverWriteLog(string message)
        {
            TextBlock_Log.Text = message;
        }

        private void ClearLog()
        {
            TextBlock_Log.Text = "";
        }

        private void AppendInLog(string line)
        {
            TextBlock_Log.Text = TextBlock_Log.Text + line + "\n";
        }

        private void TESTWriteInLog()
        {
            ClearLog();
            AppendInLog("energy " + Starting_Energy.ToString());
            AppendInLog("stime " + STime);
            AppendInLog("etime " + ETime);
            AppendInLog("quanta " + Quanta.ToString());
            foreach (var work in Dict_Work.Values)
            {
                AppendInLog(work.ToString());
            }
            foreach (var recr in Dict_Recr.Values)
            {
                AppendInLog(recr.ToString());
            }
            AppendInLog("run");
        }

        private bool WriteInFile()
        {
            try
            {
                StreamWriter SW = new StreamWriter(FilePath);
                SW.WriteLine("energy " + Starting_Energy.ToString());
                SW.WriteLine("stime " + STime);
                SW.WriteLine("etime " + ETime);
                SW.WriteLine("quanta " + Quanta.ToString());
                foreach (var work in Dict_Work.Values)
                {
                    SW.WriteLine(work.ToString());
                }
                foreach (var recr in Dict_Recr.Values)
                {
                    SW.WriteLine(recr.ToString());
                }
                SW.WriteLine("run");
                SW.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool RunPythonBridge()
        {
            try
            {
                System.Diagnostics.Process.Start(".\\bridge.exe", FilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Button_AddWork_Click(object sender, RoutedEventArgs e)
        {
            bool successful = AddWork();
            if (successful)
            {
                OverWriteLog("Successfully added work " + TextBox_WorkName.Text);
            }
            else
            {
                OverWriteLog("Failed to add work " + TextBox_WorkName.Text);
            }
        }

        private void Button_AddRecr_Click(object sender, RoutedEventArgs e)
        {
            bool successful = AddRecr();
            if (successful)
            {
                OverWriteLog("Successfully added recreation " + TextBox_RecrName.Text);
            }
            else
            {
                OverWriteLog("Failed to add recreation " + TextBox_RecrName.Text);
            }
        }

        private void Button_Set_Click(object sender, RoutedEventArgs e)
        {
            bool successful = UpdateSettings();
            UpdateSettingsView();
            if (successful)
            {
                OverWriteLog("Succesfully updated settings and works will be rectified");
                RectifyWorks();
                UpdateListBoxes();
            }
            else OverWriteLog("Failed to update settings");
        }

        private void Button_Run_Click(object sender, RoutedEventArgs e)
        {
            if(Dict_Work.Keys.Count == 0 || Dict_Recr.Keys.Count == 0)
            {
                OverWriteLog("Cannot make a routine without works and atleast one recreation");
                return;
            }
            bool successful = UpdateSettings();
            UpdateSettingsView();
            if (successful)
            {
                OverWriteLog("Succesfully updated settings");
                RectifyWorks();
                UpdateListBoxes();
                bool writesuccess = WriteInFile();
                if (writesuccess)
                {
                    OverWriteLog("File ready");
                    bool bridgingsucess = RunPythonBridge();
                    if (bridgingsucess) OverWriteLog("Executing Python now");
                    else OverWriteLog("Bridging to Python failed");
                }
                else OverWriteLog("Failed to prepare file");
            }
            else OverWriteLog("Failed to update settings");
        }

        private void Button_Help_Click(object sender, RoutedEventArgs e)
        {
            string message = "Developed by Mohammad Ishrak Abedin. Works should have durations that are multiple of quanta. " +
                "And energy should be divisible into each slot determined by total amout of quantas availble. " +
                "Rectification will automatically format duration and energy of of works to closest valid value. " + 
                "Minimum duration is just an indicator to set the starting and ending time and should generally be set much higher than minimum duration. " +
                "Keep the quanta as large as possible to decrease computaion time.";
            MessageBox.Show(message, "About", MessageBoxButton.OK ,MessageBoxImage.Question);
        }

        private void Button_Rectify_Click(object sender, RoutedEventArgs e)
        {
            RectifyInputs();
            OverWriteLog("Work durations and required energies have been updated according to quanta");
        }

        private void Button_RemoveWork_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBox_WorkName.Text;
            bool successful = RemoveWork();
            if (successful)
            {
                OverWriteLog("Succesfully removed work " + name);
            }
            else
            {
                OverWriteLog("Failed to remove work " + name);
            }
        }

        private void Button_RemoveRecr_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBox_RecrName.Text;
            bool successful = RemoveRecr();
            if (successful)
            {
                OverWriteLog("Succesfully removed recreation " + name);
            }
            else
            {
                OverWriteLog("Failed to remove recreation " + name);
            }
        }

        private void ListBox_Work_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string name = Convert.ToString(ListBox_Work.SelectedItem);
                Work work = Dict_Work[name];
                TextBox_WorkName.Text = name;
                TextBox_WorkEnergy.Text = work.Energy.ToString();
                TextBox_WorkDuration.Text = work.Duration.ToString();
                Slider_WorkPriority.Value = work.Priority;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void ListBox_Recr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string name = Convert.ToString(ListBox_Recr.SelectedItem);
                Recreation recr = Dict_Recr[name];
                TextBox_RecrName.Text = name;
                TextBox_RecrEnergy.Text = recr.Energy.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
