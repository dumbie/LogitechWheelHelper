using System;
using System.Windows;
using System.Windows.Controls;

namespace LogiWheelSettings
{
    public partial class WindowMain : Window
    {
        void combobox_SettingsLoadProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox senderCombobox = (ComboBox)sender;
                string profileName = senderCombobox.SelectedItem.ToString();
                vLoadProfile = @"Profiles\" + profileName + ".xml";
                SettingsLoadProfile();
            }
            catch { }
        }

        void checkbox_ForceEnabled_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateRegistryValueIntDword("ForceEnabled", 1);
            }
            catch { }
        }

        void checkbox_ForceEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateRegistryValueIntDword("ForceEnabled", 0);
            }
            catch { }
        }

        void checkbox_PersistentCenteringSpring_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateRegistryValueIntDword("PersistentCenteringSpring", 1);
            }
            catch { }
        }

        void checkbox_PersistentCenteringSpring_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateRegistryValueIntDword("PersistentCenteringSpring", 0);
            }
            catch { }
        }

        void slider_OverallStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Slider senderSlider = (Slider)sender;
                int sliderValue = Convert.ToInt32(senderSlider.Value);
                int registryValue = sliderValue * 100;
                textblock_OverallStrength.Text = "Overall effects strength: " + sliderValue + "%";
                UpdateRegistryValueIntDword("OverallStrength", registryValue);
            }
            catch { }
        }

        void slider_SpringStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Slider senderSlider = (Slider)sender;
                int sliderValue = Convert.ToInt32(senderSlider.Value);
                int registryValue = sliderValue * 100;
                textblock_SpringStrength.Text = "Spring effects strength: " + sliderValue + "%";
                UpdateRegistryValueIntDword("SpringStrength", registryValue);
            }
            catch { }
        }

        void slider_DamperStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Slider senderSlider = (Slider)sender;
                int sliderValue = Convert.ToInt32(senderSlider.Value);
                int registryValue = sliderValue * 100;
                textblock_DamperStrength.Text = "Damper effect strength: " + sliderValue + "%";
                UpdateRegistryValueIntDword("DamperStrength", registryValue);
            }
            catch { }
        }

        void slider_CenteringSpring_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Slider senderSlider = (Slider)sender;
                int sliderValue = Convert.ToInt32(senderSlider.Value);
                int registryValue = sliderValue * 100;
                textblock_CenteringSpring.Text = "Centering spring strength: " + sliderValue + "%";
                UpdateRegistryValueIntDword("CenteringSpring", registryValue);
            }
            catch { }
        }

        void button_SettingsReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsReset();
            }
            catch { }
        }
    }
}