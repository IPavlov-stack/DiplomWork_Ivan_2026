using System;
using System.Windows;
using DiplomWork_Ivan_2026.Services;

namespace DiplomWork_Ivan_2026
{
    public partial class MainWindow
    {
        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            LocalizationService.ToggleLanguage();
        }

        private void LocalizationService_LanguageChanged(object? sender, EventArgs e)
        {
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            LocalizationService.ApplyStaticText(this);
            StatusLabelRun.Text = LocalizationService.Text("Status: ", "Статус: ");
            LanguageButton.Content = LocalizationService.IsBulgarian
                ? "English"
                : "Български";
            LanguageButton.ToolTip = LocalizationService.Text(
                "Switch interface language to Bulgarian",
                "Смяна на езика на интерфейса на английски");

            MaterialComboBox.Items.Refresh();
            if (!_processStarted && MaterialComboBox.SelectedItem != null)
                ApplyDryingMode(GetSelectedDryingMode());
            UpdateUi();
        }
    }
}
