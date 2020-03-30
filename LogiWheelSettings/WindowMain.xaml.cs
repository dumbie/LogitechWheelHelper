using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace LogiWheelSettings
{
    public partial class WindowMain : Window
    {
        //Application Variables
        public static string vLoadDeviceName = "G27/G29/G920";
        public static string vLoadDeviceId = "VID_046D&PID_C29B"; //G27/G29/G920
        //public static string vLoadDeviceId = "VID_046D&PID_C299"; //G25
        //public static string vLoadDeviceId = "VID_046D&PID_C29A"; //DFGT

        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Load the current settings
                SettingsLoad();
            }
            catch { }
        }

        //Load the current settings
        void SettingsLoad()
        {
            try
            {
                //Update current device text
                textblock_DeviceCurrent.Text = "Current device: " + vLoadDeviceId + " (" + vLoadDeviceName + ")";

                //Open the Windows registry
                RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

                //Read the Logitech settings
                using (RegistryKey registryKey = registryKeyCurrentUser.OpenSubKey(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId))
                {
                    if (registryKey != null)
                    {
                        //AmplitudeBasedForce
                        //AmplitudeBasedForceThreshold
                        //DeltaSpringAdjustDown
                        //DeltaSpringXAdjust
                        //DeltaSpringYAdjust
                        //FlipDeltaSpringToY

                        int ForceEnabled = Convert.ToInt32(registryKey.GetValue("ForceEnabled"));
                        checkbox_ForceEnabled.IsChecked = ForceEnabled == 1;

                        int PersistentCenteringSpring = Convert.ToInt32(registryKey.GetValue("PersistentCenteringSpring"));
                        checkbox_PersistentCenteringSpring.IsChecked = PersistentCenteringSpring == 1;

                        int OverallStrength = Convert.ToInt32(registryKey.GetValue("OverallStrength")) / 100;
                        slider_OverallStrength.Value = OverallStrength;

                        int SpringStrength = Convert.ToInt32(registryKey.GetValue("SpringStrength")) / 100;
                        slider_SpringStrength.Value = SpringStrength;

                        int DamperStrength = Convert.ToInt32(registryKey.GetValue("DamperStrength")) / 100;
                        slider_DamperStrength.Value = DamperStrength;

                        int CenteringSpring = Convert.ToInt32(registryKey.GetValue("CenteringSpring")) / 100;
                        slider_CenteringSpring.Value = CenteringSpring;
                    }
                    else
                    {
                        throw new ArgumentNullException();
                    }
                }

                //Close and dispose the registry
                registryKeyCurrentUser.Dispose();
            }
            catch
            {
                MessageBox.Show("Registry settings not available, resetting to default settings.", "LogiWheelSettings");
                SettingsReset();
            }
        }

        //Reset the settings to default
        void SettingsReset()
        {
            try
            {
                //Open the Windows registry
                RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

                //Remove Logitech settings
                try
                {
                    registryKeyCurrentUser.DeleteSubKeyTree(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId);
                }
                catch
                {
                    Debug.WriteLine("Failed removing settings key.");
                }

                //Create Logitech settings key
                try
                {
                    registryKeyCurrentUser.CreateSubKey(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId);
                }
                catch
                {
                    Debug.WriteLine("Failed creating settings key.");
                }

                //Close and dispose the registry
                registryKeyCurrentUser.Dispose();

                //Set default Logitech settings
                UpdateRegistryValueIntDword("CenteringSpring", 2500);
                UpdateRegistryValueIntDword("DamperStrength", 10000);
                UpdateRegistryValueIntDword("ForceEnabled", 1);
                UpdateRegistryValueIntDword("OverallStrength", 10000);
                UpdateRegistryValueIntDword("PersistentCenteringSpring", 0);
                UpdateRegistryValueIntDword("SpringStrength", 10000);

                //Load the current settings
                SettingsLoad();
            }
            catch
            {
                MessageBox.Show("Failed resetting to default values.", "LogiWheelSettings");
            }
        }

        void UpdateRegistryValueIntDword(string name, int value)
        {
            try
            {
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyCurrentUser.OpenSubKey(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId, true))
                    {
                        openSubKey.SetValue(name, value);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Could not change registry, please run as administrator.", "LogiWheelSettings");
            }
        }

        void UpdateRegistryValueStringSz(string name, string value)
        {
            try
            {
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    using (RegistryKey openSubKey = registryKeyCurrentUser.OpenSubKey(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId, true))
                    {
                        openSubKey.SetValue(name, value);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Could not change registry, please run as administrator.", "LogiWheelSettings");
            }
        }

        void button_ShowLogitech_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string logitechHubPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\LGHub\LGHub.exe";
                string logitechGamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Logitech Gaming Software\LCore.exe";

                if (File.Exists(logitechHubPath))
                {
                    Process launchProcess = new Process();
                    launchProcess.StartInfo.FileName = logitechHubPath;
                    launchProcess.Start();
                }

                if (File.Exists(logitechGamingPath))
                {
                    Process launchProcess = new Process();
                    launchProcess.StartInfo.FileName = logitechGamingPath;
                    launchProcess.Start();
                }
            }
            catch { }
        }
    }
}