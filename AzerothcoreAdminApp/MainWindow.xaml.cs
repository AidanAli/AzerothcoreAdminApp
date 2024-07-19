using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AzerothcoreAdminApp
{
    public partial class MainWindow : Window
    {
        private const string ConfigFilePath = "config.json";

        private string _worldServerPath;
        private string _authServerPath;

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonConvert.DeserializeObject<dynamic>(json);
                    _worldServerPath = config.WorldServerPath;
                    _authServerPath = config.AuthServerPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveConfig()
        {
            try
            {
                dynamic config = new
                {
                    WorldServerPath = _worldServerPath,
                    AuthServerPath = _authServerPath
                };

                string json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartWorldServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_worldServerPath))
            {
                _worldServerPath = BrowseForExecutable();
                if (string.IsNullOrEmpty(_worldServerPath))
                    return;
                SaveConfig();
            }

            StartProcess(_worldServerPath);
        }

        private void StartAuthServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_authServerPath))
            {
                _authServerPath = BrowseForExecutable();
                if (string.IsNullOrEmpty(_authServerPath))
                    return;
                SaveConfig();
            }

            StartProcess(_authServerPath);
        }

        private string BrowseForExecutable()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe",
                Title = "Select Executable File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        private void StartProcess(string processName)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // Use cmd.exe to launch the executable
                    Arguments = $"/K \"{processName}\"", // /K keeps cmd window open after execution
                    WorkingDirectory = System.IO.Path.GetDirectoryName(processName) // Set working directory if needed
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting {processName}: {ex.Message}", "Server Control", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
