using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Gunde.UI.Mvvm;

namespace Gunde.UI.ViewModels
{
    internal class TaskGroupViewModel : ViewModelBase
    {
        public TaskGroupViewModel(Type taskType, TaskListViewModel taskListViewModel)
        {
            Name = taskType.Name;
            Tasks = new ObservableCollection<TaskViewModel>();

            var allowedParameterTypes = new[]
            {
                typeof(int),
                typeof(int?),
                typeof(decimal),
                typeof(decimal?),
                typeof(short),
                typeof(short?),
                typeof(long),
                typeof(long?),
                typeof(float),
                typeof(float?),
                typeof(bool),
                typeof(bool?),
                typeof(string),
                typeof(char),
                typeof(char?),
                typeof(DateTime),
                typeof(DateTime?)
            };

            var orderedTasks = taskType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(x => x.Name)
                .ToList();
                //.OrderBy(x => x.GetCustomAttribute<UIExecutableAttribute>().OptionalDependencyGroupOrder ?? -1)
                //.ThenBy(TaskViewModel.GetDisplayName);

            foreach (var task in orderedTasks)
            {
                var parameters = task.GetParameters();

                if (parameters.All(x => allowedParameterTypes.Contains(x.ParameterType)))
                {
                    var parameterViewModels = parameters.Select(x => new TaskParameterViewModel(x));
                    var taskViewModel = new TaskViewModel(taskType, task, parameterViewModels, taskListViewModel);

                    Tasks.Add(taskViewModel);
                }
            }
        }

        public string Name { get; }
        public bool HasName => !string.IsNullOrWhiteSpace(Name);
        public ObservableCollection<TaskViewModel> Tasks { get; } 
    }
}