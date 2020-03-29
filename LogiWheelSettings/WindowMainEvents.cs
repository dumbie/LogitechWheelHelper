using System;
using System.Windows;
using System.Windows.Controls;

namespace LogiWheelSettings
{
    public partial class WindowMain : Window
    {
        void checkbox_ForceEnabled_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)checkbox_ForceEnabled.IsChecked)
                {
                    UpdateRegistryValueIntDword("ForceEnabled", 1);
                }
                else
                {
                    UpdateRegistryValueIntDword("ForceEnabled", 0);
                }
            }
            catch { }
        }

        void checkbox_PersistentCenteringSpring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)checkbox_PersistentCenteringSpring.IsChecked)
                {
                    UpdateRegistryValueIntDword("PersistentCenteringSpring", 1);
                }
                else
                {
                    UpdateRegistryValueIntDword("PersistentCenteringSpring", 0);
                }
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