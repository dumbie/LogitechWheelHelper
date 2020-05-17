using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace LogitechWheelHelper
{
    public partial class WindowMain : Window
    {
        //Application Variables
        public static string vLoadProfile = string.Empty;
        public static string vLoadDeviceName = "G27/G29/G920";
        public static string vLoadDeviceId = "VID_046D&PID_C29B"; //G27/G29/G920
        //public static string vLoadDeviceId = "VID_046D&PID_C299"; //G25

        //Application Timers
        public static DispatcherTimer vDispatcherTimer = new DispatcherTimer();

        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Load the current settings
                SettingsLoad();

                //Load the available profiles
                ProfilesLoad();

                //Update version tooltip
                string stringVersion = "Application made by Arnold Vink\nVersion: v" + Assembly.GetEntryAssembly().FullName.Split('=')[1].Split(',')[0];
                ToolTip tooltipVersion = new ToolTip() { Content = stringVersion };
                image_LogitechWheelHelper.ToolTip = tooltipVersion;
            }
            catch { }
        }

        //Show message status
        public void ShowMessageStatus(string message)
        {
            try
            {
                vDispatcherTimer.Stop();

                textblock_MessageStatus.Text = message;
                grid_MessageStatus.Visibility = Visibility.Visible;

                vDispatcherTimer.Interval = TimeSpan.FromSeconds(3);
                vDispatcherTimer.Tick += delegate
                {
                    grid_MessageStatus.Visibility = Visibility.Collapsed;
                    vDispatcherTimer.Stop();
                };
                vDispatcherTimer.Start();
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
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Read the Logitech settings
                    using (RegistryKey openSubKey = registryKeyCurrentUser.OpenSubKey(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId))
                    {
                        if (openSubKey != null)
                        {
                            int ForceEnabled = Convert.ToInt32(openSubKey.GetValue("ForceEnabled"));
                            checkbox_ForceEnabled.IsChecked = ForceEnabled == 1;

                            int OverallStrength = Convert.ToInt32(openSubKey.GetValue("OverallStrength")) / 100;
                            slider_OverallStrength.Value = OverallStrength;

                            int SpringStrength = Convert.ToInt32(openSubKey.GetValue("SpringStrength")) / 100;
                            slider_SpringStrength.Value = SpringStrength;

                            int DamperStrength = Convert.ToInt32(openSubKey.GetValue("DamperStrength")) / 100;
                            slider_DamperStrength.Value = DamperStrength;
                        }
                        else
                        {
                            throw new ArgumentNullException();
                        }
                    }
                }
            }
            catch
            {
                SettingsReset();
            }
        }

        //Load the available profiles
        void ProfilesLoad()
        {
            try
            {
                combobox_SettingsLoadProfile.Items.Clear();
                DirectoryInfo directoryInfo = new DirectoryInfo("Profiles");
                foreach (FileInfo file in directoryInfo.GetFiles("*.xml"))
                {
                    combobox_SettingsLoadProfile.Items.Add(Path.GetFileNameWithoutExtension(file.Name));
                }
            }
            catch { }
        }

        //Reset the settings to default
        void SettingsReset()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Remove Logitech settings key
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
                }

                //Set default Logitech settings
                UpdateRegistryValueIntDword("ForceEnabled", 1);
                UpdateRegistryValueIntDword("OverallStrength", 10000);
                UpdateRegistryValueIntDword("SpringStrength", 10000);
                UpdateRegistryValueIntDword("DamperStrength", 10000);

                //Reset profile selection
                combobox_SettingsLoadProfile.SelectedIndex = -1;
                vLoadProfile = string.Empty;

                //Load the current settings
                SettingsLoad();

                //Show the message status
                ShowMessageStatus("Settings reset to defaults");
            }
            catch
            {
                ShowMessageStatus("Failed resetting settings");
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
                ShowMessageStatus("Could not update registry");
            }
        }

        void button_ShowLogitech_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowMessageStatus("Showing Logitech software");
                string logitechHubPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\LGHub\LGHub.exe";
                string logitechGamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Logitech Gaming Software\LCore.exe";

                if (File.Exists(logitechHubPath))
                {
                    Process launchProcess = new Process();
                    launchProcess.StartInfo.FileName = logitechHubPath;
                    launchProcess.Start();
                }
                else if (File.Exists(logitechGamingPath))
                {
                    Process launchProcess = new Process();
                    launchProcess.StartInfo.FileName = logitechGamingPath;
                    launchProcess.Start();
                }
            }
            catch { }
        }

        void button_SettingsSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vLoadProfile == string.Empty)
                {
                    ShowMessageStatus("No profile selected");
                    Debug.WriteLine("No profile selected");
                    return;
                }

                XDocument xmlProfile = XDocument.Load(vLoadProfile);

                int ForceEnabled = (bool)checkbox_ForceEnabled.IsChecked ? 1 : 0;
                xmlProfile.Descendants("ForceEnabled").First().Value = ForceEnabled.ToString();

                int OverallStrength = Convert.ToInt32(slider_OverallStrength.Value) * 100;
                xmlProfile.Descendants("OverallStrength").First().Value = OverallStrength.ToString();

                int SpringStrength = Convert.ToInt32(slider_SpringStrength.Value) * 100;
                xmlProfile.Descendants("SpringStrength").First().Value = SpringStrength.ToString();

                int DamperStrength = Convert.ToInt32(slider_DamperStrength.Value) * 100;
                xmlProfile.Descendants("DamperStrength").First().Value = DamperStrength.ToString();

                xmlProfile.Save(vLoadProfile);

                ShowMessageStatus("Saved profile: " + Path.GetFileNameWithoutExtension(vLoadProfile));
                Debug.WriteLine("Saved profile: " + vLoadProfile);
            }
            catch
            {
                ShowMessageStatus("Failed saving profile: " + Path.GetFileNameWithoutExtension(vLoadProfile));
            }
        }

        void SettingsLoadProfile()
        {
            try
            {
                XDocument xmlProfile = XDocument.Load(vLoadProfile);

                int ForceEnabled = Convert.ToInt32(xmlProfile.Descendants("ForceEnabled").First().Value);
                checkbox_ForceEnabled.IsChecked = ForceEnabled == 1;

                int OverallStrength = Convert.ToInt32(xmlProfile.Descendants("OverallStrength").First().Value) / 100;
                slider_OverallStrength.Value = OverallStrength;

                int SpringStrength = Convert.ToInt32(xmlProfile.Descendants("SpringStrength").First().Value) / 100;
                slider_SpringStrength.Value = SpringStrength;

                int DamperStrength = Convert.ToInt32(xmlProfile.Descendants("DamperStrength").First().Value) / 100;
                slider_DamperStrength.Value = DamperStrength;

                ShowMessageStatus("Loaded profile: " + Path.GetFileNameWithoutExtension(vLoadProfile));
                Debug.WriteLine("Loaded profile: " + vLoadProfile);
            }
            catch
            {
                ShowMessageStatus("Failed loading profile: " + Path.GetFileNameWithoutExtension(vLoadProfile));
            }
        }

        void button_SettingsRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Remove Logitech settings key
                    registryKeyCurrentUser.DeleteSubKeyTree(@"Software\Logitech\Gaming Software\DriverSettings\" + vLoadDeviceId);
                }

                ShowMessageStatus("Settings removed from registry");
                MessageBox.Show("Settings removed from registry, the application will now be closed.\n\nThe next time you run this application the settings will be recreated.", "Logitech Wheel Settings");
                Environment.Exit(0);
            }
            catch
            {
                ShowMessageStatus("Failed removing settings");
            }
        }
    }
}