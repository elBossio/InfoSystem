using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace InfoSystemApp
{
    public partial class MainWindow : Window
    {
        private HttpClient _httpClient;
        private string _currentLanguage = "en";
        private readonly Dictionary<string, string> _deviceUrls = new Dictionary<string, string>
        {
            { "pumps", "https://2392bb8b-2589-4515-a05d-bff3882c6c75.mock.pstmn.io/pumps" },
            { "cylinders", "https://2392bb8b-2589-4515-a05d-bff3882c6c75.mock.pstmn.io/cylinders" },
            { "valves", "https://2392bb8b-2589-4515-a05d-bff3882c6c75.mock.pstmn.io/valves" }
        };

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            LoadDeviceTypes();
            UpdateUI();
        }

        private async void LoadDeviceTypes()
        {
            var devicesUrl = "https://2392bb8b-2589-4515-a05d-bff3882c6c75.mock.pstmn.io/devices";
            try
            {
                var response = await _httpClient.GetStringAsync(devicesUrl);
                var deviceTypes = JsonConvert.DeserializeObject<List<DeviceType>>(response);
                DeviceTypeComboBox.ItemsSource = deviceTypes;
                DeviceTypeComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке типов устройств: " + ex.Message);
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceTypeComboBox.SelectedItem == null) return;

            var selectedDeviceType = (DeviceType)DeviceTypeComboBox.SelectedItem;
            if (_deviceUrls.TryGetValue(selectedDeviceType.Name.ToLower(), out var url))
            {
                try
                {
                    var response = await _httpClient.GetStringAsync(url);
                    List<Device> devices = new List<Device>();

                    if (selectedDeviceType.Name.ToLower() == "pumps")
                    {
                        var pumps = JsonConvert.DeserializeObject<List<Pump>>(response);
                        devices.AddRange(pumps);
                    }
                    else if (selectedDeviceType.Name.ToLower() == "cylinders")
                    {
                        var cylinders = JsonConvert.DeserializeObject<List<Cylinder>>(response);
                        devices.AddRange(cylinders);
                    }
                    else if (selectedDeviceType.Name.ToLower() == "valves")
                    {
                        var valves = JsonConvert.DeserializeObject<List<Valve>>(response);
                        devices.AddRange(valves);
                    }

                    DeviceDataGrid.ItemsSource = devices;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных устройств: " + ex.Message);
                }
            }
        }

        private void DeviceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceDataGrid.SelectedItem is Device selectedDevice)
            {
                var stringBuilder = new StringBuilder();

                if (selectedDevice is Pump pump)
                {
                    stringBuilder.AppendLine($"Производительность: {pump.Capacity}");
                    stringBuilder.AppendLine($"Напор: {pump.Head}");
                    stringBuilder.AppendLine($"Скорость: {pump.Speed}");
                    stringBuilder.AppendLine($"Эффективность: {pump.Efficiency}");
                    stringBuilder.AppendLine($"Мощность двигателя: {pump.MotorPower}");
                }
                else if (selectedDevice is Cylinder cylinder)
                {
                    stringBuilder.AppendLine($"Ход: {cylinder.Stroke}");
                    stringBuilder.AppendLine($"Диаметр: {cylinder.Bore}");
                    stringBuilder.AppendLine($"Внешний диаметр: {cylinder.OuterDiameter}");
                    stringBuilder.AppendLine($"Длина сжатия: {cylinder.ShrinkLength}");
                    stringBuilder.AppendLine($"Длина вытягивания: {cylinder.ExtendLength}");
                    stringBuilder.AppendLine($"Расстояние до масляного порта: {cylinder.OilPortDistance}");
                }
                else if (selectedDevice is Valve valve)
                {
                    stringBuilder.AppendLine($"Номинальный размер трубы: {valve.NominalPipeSize}");
                    stringBuilder.AppendLine($"Глобус: {valve.Globe}");
                    stringBuilder.AppendLine($"Шаровый клапан: {valve.BallCheck}");
                    stringBuilder.AppendLine($"Угол: {valve.Angle}");
                    stringBuilder.AppendLine($"Клапан с колеблющимся диском: {valve.SwingCheck}");
                    stringBuilder.AppendLine($"Задвижка: {valve.PlugCock}");
                    stringBuilder.AppendLine($"Задвижка или шаровой клапан: {valve.GateOrBallValve}");
                }

                PropertyTextBlock.Text = stringBuilder.ToString();
            }
        }

        private void SwitchLanguage_Click(object sender, RoutedEventArgs e)
        {
            _currentLanguage = _currentLanguage == "en" ? "ru" : "en";
            UpdateUI();
        }

        private void UpdateUI()
        {
            LoadButton.Content = _currentLanguage == "en" ? "Load" : "Загрузить";

            SwitchLanguageButton.Content = _currentLanguage == "en" ? "English" : "Русский";

            DeviceTypeComboBox.ToolTip = _currentLanguage == "en" ? "Select Device Type" : "Выберите тип устройства";
            PropertiesPanelHeader.Text = _currentLanguage == "en" ? "Selected Device Properties:" : "Свойства выбранного устройства";

            if (DeviceDataGrid.Columns.Count >= 4)
            {
                DeviceDataGrid.Columns[0].Header = _currentLanguage == "en" ? "Id" : "Идентификатор";
                DeviceDataGrid.Columns[1].Header = _currentLanguage == "en" ? "UID" : "UID";
                DeviceDataGrid.Columns[2].Header = _currentLanguage == "en" ? "Code" : "Обозначение";
                DeviceDataGrid.Columns[3].Header = _currentLanguage == "en" ? "Name" : "Наименование";
            }
        }
    }

    public class DeviceType
    {
        public string Name { get; set; }
    }

    public class Device
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class Pump : Device
    {
        public double Capacity { get; set; }
        public double Head { get; set; }
        public double Speed { get; set; }
        public double Efficiency { get; set; }
        public double MotorPower { get; set; }
    }

    public class Cylinder : Device
    {
        public double Stroke { get; set; }
        public double Bore { get; set; }
        public double OuterDiameter { get; set; }
        public double ShrinkLength { get; set; }
        public double ExtendLength { get; set; }
        public double OilPortDistance { get; set; }
    }

    public class Valve : Device
    {
        public double NominalPipeSize { get; set; }
        public double Globe { get; set; }
        public double BallCheck { get; set; }
        public double Angle { get; set; }
        public double SwingCheck { get; set; }
        public double PlugCock { get; set; }
        public double GateOrBallValve { get; set; }
    }
}
