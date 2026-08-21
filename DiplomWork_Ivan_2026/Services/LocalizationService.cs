using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DiplomWork_Ivan_2026.Services
{
    public enum UiLanguage
    {
        English,
        Bulgarian
    }

    public static class LocalizationService
    {
        private static readonly Dictionary<string, string> EnglishToBulgarian =
            new(StringComparer.Ordinal)
            {
                ["SCADA - Vacuum Dryer"] = "SCADA - Вакуумна сушилня",
                ["Vacuum Dryer Control"] = "Управление на вакуумна сушилня",
                ["Material:"] = "Материал:",
                ["Temperature Setpoint [°C]:"] = "Задание за температура [°C]:",
                ["Pressure Setpoint [kPa]:"] = "Задание за налягане [kPa]:",
                ["Operation Mode:"] = "Режим на работа:",
                ["Auto"] = "Автоматичен",
                ["Manual"] = "Ръчен",
                ["Temperature Control:"] = "Управление на температурата:",
                ["Drying Mode:"] = "Режим на сушене:",
                ["Soft"] = "Щадящ",
                ["Normal"] = "Нормален",
                ["Hard"] = "Интензивен",
                ["Select a material and drying mode."] = "Изберете материал и режим на сушене.",
                ["Process control"] = "Управление на процеса",
                ["Start"] = "Старт",
                ["Pause"] = "Пауза",
                ["Reset Process"] = "Нулиране на процеса",
                ["Safety"] = "Безопасност",
                ["Safety state: Normal"] = "Състояние на защитата: Нормално",
                ["EMERGENCY STOP"] = "АВАРИЙНО СПИРАНЕ",
                ["Reset Safety"] = "Нулиране на защитата",
                ["Status: "] = "Статус: ",
                ["Ready"] = "Готовност",
                ["Stage: Idle"] = "Етап: Готовност",
                ["Simulation speed:"] = "Скорост на симулацията:",
                ["VACUUM"] = "ВАКУУМНА",
                ["DRYER"] = "СУШИЛНЯ",
                ["VACUUM DRYER"] = "ВАКУУМНА СУШИЛНЯ",
                ["CHAMBER"] = "КАМЕРА",
                ["HEATER"] = "НАГРЕВАТЕЛ",
                ["FAN"] = "ВЕНТ.",
                ["PUMP"] = "ПОМПА",
                ["VENT"] = "КЛАПАН",
                ["Vacuum line"] = "Вакуумна линия",
                ["Process Progress"] = "Ход на процеса",
                ["Estimated Remaining: calculating..."] = "Оставащо време: изчислява се...",
                ["Sensors: OK"] = "Датчици: OK",
                ["Batch Performance"] = "Показатели на партидата",
                ["Moisture Ratio: 1.000"] = "Отношение на влагата: 1.000",
                ["Vacuum Level: 0.0 %"] = "Ниво на вакуум: 0.0 %",
                ["Total Energy: 0.000 kWh"] = "Обща енергия: 0.000 kWh",
                ["Efficiency: 0.000 kg/kWh"] = "Ефективност: 0.000 kg/kWh",
                ["Active Alarm"] = "Активна аларма",
                ["No active alarms"] = "Няма активни аларми",
                ["Navigation"] = "Навигация",
                ["DETAILS"] = "ДЕТАЙЛИ",
                ["TRENDS"] = "ГРАФИКИ",
                ["ALARMS"] = "АЛАРМИ",
                ["Context / Controls"] = "Контекст / Управление",
                ["Vacuum Chamber"] = "Вакуумна камера",
                ["State: Ready"] = "Състояние: Готовност",
                ["Actuator outputs"] = "Изходи към изп. механизми",
                ["Auto mode: controller outputs"] = "Автоматичен режим: изходи от регулаторите",
                ["Heater Power"] = "Мощност на нагревателя",
                ["Vacuum Pump"] = "Вакуумна помпа",
                ["Vent Valve"] = "Вент. клапан",
                ["Fan Speed"] = "Скорост на вентилатора",
                ["Heater Power [%]"] = "Мощност на нагревателя [%]",
                ["Vacuum Pump [%]"] = "Вакуумна помпа [%]",
                ["Vent Valve [%]"] = "Вентилационен клапан [%]",
                ["Fan Speed [%]"] = "Скорост на вентилатора [%]",
                ["EXIT"] = "ИЗХОД",

                ["Process Trends"] = "Графики на процеса",
                ["Vacuum Dryer Process Trends"] = "Графики на вакуумната сушилня",
                ["CLOSE"] = "ЗАТВОРИ",
                ["Selected trend:"] = "Избрана графика:",
                ["Temperature"] = "Температура",
                ["Pressure"] = "Налягане",
                ["Moisture / Humidity"] = "Влага / Влажност",
                ["Drying Rate"] = "Скорост на сушене",
                ["Actuators"] = "Изпълнителни механизми",
                ["Energy"] = "Енергия",
                ["Time range:"] = "Времеви диапазон:",
                ["All"] = "Всички",
                ["Display:"] = "Изглед:",
                ["Raw"] = "Необработена",
                ["Smooth"] = "Плавна",
                ["Changes only the chart appearance; stored process data is not modified."] = "Променя само изгледа на графиката; записаните процесни данни не се променят.",
                ["Current: 0.0"] = "Текуща: 0.0",
                ["Temperature [°C]"] = "Температура [°C]",

                ["Process Details"] = "Детайли за процеса",
                ["Vacuum Dryer Process Details"] = "Детайли за вакуумната сушилня",
                ["Process Variables"] = "Процесни величини",
                ["Material Data"] = "Данни за материала",
                ["Energy / Efficiency"] = "Енергия / Ефективност",

                ["Alarm History"] = "История на алармите",
                ["Vacuum Dryer Alarm History"] = "История на алармите на вакуумната сушилня",
                ["Active alarms: 0"] = "Активни аларми: 0",
                ["Total alarms: 0"] = "Общо аларми: 0",
                ["REFRESH"] = "ОБНОВИ",
                ["Status"] = "Статус",
                ["Priority"] = "Приоритет",
                ["Date"] = "Дата",
                ["Time"] = "Час",
                ["Type"] = "Тип",
                ["Description"] = "Описание",
                ["Recommended action"] = "Препоръчано действие"
                ,[
                    "Pause the simulation and set all actuator outputs to zero. The batch can be resumed."
                ] = "Поставя симулацията на пауза и нулира изходите. Партидата може да бъде продължена.",
                ["Clear the current batch progress and trend history."] = "Изчиства хода на текущата партида и историята на графиките.",
                ["Available after a safety trip and only when safe reset conditions are met."] = "Достъпно след задействане на защита и само при изпълнени условия за безопасно нулиране.",
                ["Show chamber measurements and state"] = "Показва измерванията и състоянието на камерата",
                ["Show heater information"] = "Показва информация за нагревателя",
                ["Show circulation fan information"] = "Показва информация за циркулационния вентилатор",
                ["Show vacuum pump information"] = "Показва информация за вакуумната помпа",
                ["Show vent valve information"] = "Показва информация за вентилационния клапан"
            };

        public static UiLanguage CurrentLanguage { get; private set; } = UiLanguage.English;
        public static bool IsBulgarian => CurrentLanguage == UiLanguage.Bulgarian;
        public static event EventHandler? LanguageChanged;

        public static string Text(string english, string bulgarian) =>
            IsBulgarian ? bulgarian : english;

        public static void ToggleLanguage()
        {
            CurrentLanguage = IsBulgarian ? UiLanguage.English : UiLanguage.Bulgarian;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void ApplyStaticText(Window window)
        {
            window.Title = TranslateLiteral(window.Title);
            ApplyToObject(window.Content as DependencyObject);
        }

        private static void ApplyToObject(DependencyObject? element)
        {
            if (element == null)
                return;

            // TextBlocks with multiple inline Runs contain independently updated
            // values (for example the Status label and value). Assigning Text here
            // would remove those Runs from the visual tree.
            if (element is TextBlock textBlock && textBlock.Inlines.Count <= 1)
                textBlock.Text = TranslateLiteral(textBlock.Text);

            if (element is FrameworkElement frameworkElement && frameworkElement.ToolTip is string toolTip)
                frameworkElement.ToolTip = TranslateLiteral(toolTip);

            if (element is ContentControl contentControl && contentControl.Content is string content)
                contentControl.Content = TranslateLiteral(content);

            if (element is HeaderedContentControl headered && headered.Header is string header)
                headered.Header = TranslateLiteral(header);

            if (element is DataGrid dataGrid)
            {
                foreach (DataGridColumn column in dataGrid.Columns)
                {
                    if (column.Header is string columnHeader)
                        column.Header = TranslateLiteral(columnHeader);
                }
            }

            foreach (object child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is DependencyObject dependencyChild)
                    ApplyToObject(dependencyChild);
            }
        }

        private static string TranslateLiteral(string value)
        {
            if (IsBulgarian)
                return EnglishToBulgarian.TryGetValue(value, out string? bg) ? bg : value;

            foreach (KeyValuePair<string, string> pair in EnglishToBulgarian)
            {
                if (string.Equals(pair.Value, value, StringComparison.Ordinal))
                    return pair.Key;
            }

            return value;
        }
    }
}
