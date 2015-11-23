using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Gunde.Core.Loader;
using Gunde.Core.Output;
using Gunde.UI.Infrastructure;
using Gunde.UI.Mvvm;

namespace Gunde.UI.ViewModels
{
    internal class TaskListViewModel : ViewModelBase, IResultListener
    {
        private readonly UserSettingsService _settingsService;
        private readonly TaskAssemblyLoader _taskLoader;

        public TaskListViewModel()
        {
            InitializeTaskListCommand = new DelegateCommand(InitializeTaskList);
            ExecuteAllTasksCommand = new DelegateCommand(ExecuteAllTasks);
            TaskGroups = new ObservableCollection<TaskGroupViewModel>();

            _taskLoader = new TaskAssemblyLoader();
            _settingsService = new UserSettingsService();

            TaskAssemblyPath = _settingsService.LastTaskAssemblyPath;

            if (!string.IsNullOrWhiteSpace(TaskAssemblyPath))
            {
                InitializeTaskList();
            }
        }

        public string Log
        {
            get { return Get(() => Log); }
            set { Set(() => Log, value); }
        }

        public string TaskAssemblyPath
        {
            get { return Get(() => TaskAssemblyPath); }
            set
            {
                Set(() => TaskAssemblyPath, value);

                TaskAssemblyFileName = Path.GetFileName(value);

                _settingsService.LastTaskAssemblyPath = value;
            }
        }

        public string TaskAssemblyFileName
        {
            get { return Get(() => TaskAssemblyFileName); }
            set { Set(() => TaskAssemblyFileName, value); }
        }

        public ObservableCollection<TaskGroupViewModel> TaskGroups { get; }
        public DelegateCommand InitializeTaskListCommand { get; }
        public DelegateCommand ExecuteAllTasksCommand { get; }

        public void OutputLine(string message, params object[] formatParams)
        {
            Application.Current.Dispatcher.Invoke(
                (Action)(() => { Log += Environment.NewLine + string.Format(message, formatParams); }));
        }

        public void InitializeTaskList()
        {
            TaskGroups.Clear();

            _taskLoader.LoadTaskTypes(TaskAssemblyPath)
                .ContinueWith(t => InvokeUIAction(() =>
                {
                    var taskTypes = t.Result;
                    var taskTypeList = taskTypes.ToList();

                    foreach (var taskType in taskTypeList)
                    {
                        var groupViewModel = new TaskGroupViewModel(taskType, this);

                        TaskGroups.Add(groupViewModel);
                    }
                }));
        }

        private void ExecuteAllTasks()
        {
            var tasksToExecute = new List<TaskViewModel>();

            var allTaskViewModels = TaskGroups.SelectMany(x => x.Tasks).ToList();

            foreach (var taskViewModel in allTaskViewModels)
            {
                taskViewModel.CouldBeExecuted = null;
                taskViewModel.WasIgnored = null;
            }

            var viewModelsByTaskType = allTaskViewModels.GroupBy(x => x.TaskType).ToList();

            foreach (var g in viewModelsByTaskType)
            {
                var executableViewModel =
                    g.OrderByDescending(x => x.Parameters.Count).FirstOrDefault(x => x.HasValidParameterValues);

                if (executableViewModel != null)
                    tasksToExecute.Add(executableViewModel);
            }

            var taskViewModelsWhichCouldNotBeExecuted =
                allTaskViewModels.Where(x => !tasksToExecute.Any(y => y.TaskType == x.TaskType));

            foreach (var taskViewModel in taskViewModelsWhichCouldNotBeExecuted)
            {
                taskViewModel.CouldBeExecuted = false;
            }

            var taskViewModelsWhichWereIgnored =
                allTaskViewModels.Where(
                    x => tasksToExecute.Any(y => y.TaskType == x.TaskType) && !tasksToExecute.Contains(x));

            foreach (var taskViewModel in taskViewModelsWhichWereIgnored)
            {
                taskViewModel.WasIgnored = true;
            }

            foreach (var taskListViewModel in tasksToExecute)
            {
                taskListViewModel.Execute();
            }
        }
    }
}