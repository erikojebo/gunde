using System;
using System.Windows;
using System.Windows.Controls;
using Gunde.UI.Infrastructure;
using Gunde.UI.ViewModels;
using Microsoft.Win32;

namespace Gunde.UIRunner
{
    public partial class MainWindow
    {
        private bool _autoScroll = true;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new TaskListViewModel();

            EventAggregator.TaskExecutionFailed += OnTaskExecutionFailed;

            LogScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        private void OnTaskExecutionFailed(string message)
        {
            MessageBox.Show($"The task could not be executed: {message}", "Task execution failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal TaskListViewModel ViewModel => DataContext as TaskListViewModel;

        private void OnBrowseTaskAssemblyClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "DLL (*.dll)|*.dll",
                Title = "Choose assembly containing automation tasks"
            };

            var result = fileDialog.ShowDialog();

            if (result == true)
            {
                ViewModel.TaskAssemblyPath = fileDialog.FileName;
                ViewModel.InitializeTaskList();
            }
        }

        private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (LogScrollViewer.VerticalOffset == LogScrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    _autoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    _autoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (_autoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                LogScrollViewer.ScrollToVerticalOffset(LogScrollViewer.ExtentHeight);
            }
        }
    }
}
