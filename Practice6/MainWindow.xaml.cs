using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Practice6
{
    public partial class MainWindow
    {
        private readonly ArduinoInterlayer _arduino;

        public MainWindow()
        {
            _arduino = new ArduinoInterlayer();
            InitializeComponent();
        }

        #region UserInterfaceMethods

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _arduino.DisconnectFromPort();
        }

        private void FindPortsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var portNames = ArduinoInterlayer.GetPortNames().ToArray();
            if (portNames.Length != 0)
            {
                PortNamesComboBox.Items.Clear();
                foreach (var name in portNames)
                {
                    PortNamesComboBox.Items.Add(name);
                }

                PortNamesComboBox.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("COM-порт Arduino не найден",
                    "Ой...", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                ConnectPortButton.IsEnabled = false;
            }
        }

        private void ConnectPortButton_Click(object sender, RoutedEventArgs e)
        {
            if (_arduino.IsArduinoConnected == false) ConnectArduino();
            else DisconnectArduino();
        }

        private void ConnectArduino()
        {
            FindPortsButton.IsEnabled = false;

            var port = (string)PortNamesComboBox.SelectedItem;
            PortNamesComboBox.IsEnabled = false;

            ConnectPortButton.Style = (Style)Resources["ConnectedArduino"];
            ConnectPortButton.Content = "Arduino подключен";

            FillGrid(_arduino.SubstationCount);

            _arduino.InvasionHappened += StartDialogWindow;
            _arduino.ConnectToPort(port);
        }

        private void DisconnectArduino()
        {
            FindPortsButton.IsEnabled = true;

            PortNamesComboBox.IsEnabled = true;
            PortNamesComboBox.SelectedItem = null;

            ConnectPortButton.Style = (Style)Resources["DisconnectedArduino"];
            ConnectPortButton.Content = "Arduino отключен";

            ClearGrid();

            _arduino.InvasionHappened -= StartDialogWindow;
            _arduino.DisconnectFromPort();
        }

        private void StopSignalButton_OnClick(object sender, RoutedEventArgs e)
        {
            _arduino.DisableSignalling();
        }

        private void PortNamesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConnectPortButton.Style = (Style)Resources["SampleButton"];
            ConnectPortButton.Content = "Подключиться";
            ConnectPortButton.IsEnabled = ((ComboBox)sender).SelectedItem != null;
        }

        private void EnableStation(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var c = button.Name[button.Name.Length - 1];
            var index = c - '0';
            _arduino.LightLed(index);
        }

        private void DisableStation(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var c = button.Name[button.Name.Length - 1];
            var index = c - '0';
            _arduino.TurnOffLed(index);
        }

        private void StartDialogWindow()
        {
            var message = "Кажется у нас гости! Включить сигнализацию?";
            var caption = "Тревога!";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _arduino.EnableSignalling();
            }
        }

        #endregion

        #region InnerMethods

        private void FillGrid(int count)
        {
            for (var i = 0; i < count; i++)
            {
                AddGridRow(1);

                var buttons = GetSubstationButtons(i);
                foreach (var button in buttons)
                {
                    ConnectedPortGrid.Children.Add(button);
                }
            }

            AddGridRow(2);
            var stopSignallingButton = GetStopSignallingButton(count);
            ConnectedPortGrid.Children.Add(stopSignallingButton);

            ConnectedPortGrid.Visibility = Visibility.Visible;
        }

        private Button GetStopSignallingButton(int count)
        {
            var stopSignallingButton = new Button
            {
                Name = "StopSignalButton",
                Content = "Отключить сигнализацию",
                Style = (Style)Resources["SampleButton"],
                Width = 300,
                Height = 75
            };
            stopSignallingButton.Click += StopSignalButton_OnClick;
            Grid.SetRow(stopSignallingButton, count);
            Grid.SetColumn(stopSignallingButton, 0);
            Grid.SetColumnSpan(stopSignallingButton, 2);
            return stopSignallingButton;
        }

        private void AddGridRow(int length)
        {
            var row = new RowDefinition
            {
                Height = new GridLength(length, GridUnitType.Star)
            };
            ConnectedPortGrid.RowDefinitions.Add(row);
        }

        private IEnumerable<Button> GetSubstationButtons(int number)
        {
            var enableButton = new Button
            {
                Name = $"EnableSubstation{number + 1}",
                Content = $"Включить подстанцию №{number + 1}",
                Style = (Style)Resources["SampleButton"]
            };
            enableButton.Click += EnableStation;
            Grid.SetColumn(enableButton, 0);
            Grid.SetRow(enableButton, number);

            var disableButton = new Button
            {
                Name = $"DisableSubstation{number + 1}",
                Content = $"Отключить подстанцию №{number + 1}",
                Style = (Style)Resources["SampleButton"]
            };
            disableButton.Click += DisableStation;
            Grid.SetColumn(disableButton, 1);
            Grid.SetRow(disableButton, number);

            return new[] { enableButton, disableButton };
        }

        private void ClearGrid()
        {
            ConnectedPortGrid.Children.Clear();
            ConnectedPortGrid.RowDefinitions.Clear();
            ConnectedPortGrid.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}