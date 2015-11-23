using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Gunde.Core.Output;
using Gunde.UI.Infrastructure;
using Gunde.UI.Mvvm;

namespace Gunde.UI.ViewModels
{
    internal class TaskViewModel : ViewModelBase
    {
        private readonly MethodInfo _task;
        private readonly IResultListener _resultListener;

        public TaskViewModel(
            Type taskType,
            MethodInfo task,
            IEnumerable<TaskParameterViewModel> parameterViewModels,
            IResultListener resultListener)
        {
            _task = task;
            _resultListener = resultListener;

            //var uiExecutableAttribute = taskType.GetCustomAttribute<UIExecutableAttribute>();

            TaskType = taskType;
            Name = GetDisplayName(task);
            Description = "";//uiExecutableAttribute.Description;
            DependencyGroupOrder = null;//uiExecutableAttribute.OptionalDependencyGroupOrder;
            Parameters = new ObservableCollection<TaskParameterViewModel>(parameterViewModels);
            ExecuteCommand = new DelegateCommand(Execute);
        }

        public Type TaskType { get; }

        public bool? WasSuccessful
        {
            get { return Get(() => WasSuccessful); }
            set { Set(() => WasSuccessful, value); }
        }

        public bool? IsTaskExecuting
        {
            get { return Get(() => IsTaskExecuting); }
            set { Set(() => IsTaskExecuting, value); }
        }

        public double TaskProgressPercentageValue
        {
            get { return Get(() => TaskProgressPercentageValue); }
            set { Set(() => TaskProgressPercentageValue, value); }
        }

        public bool? IsTaskProgressIndeterminate
        {
            get { return Get(() => IsTaskProgressIndeterminate); }
            set { Set(() => IsTaskProgressIndeterminate, value); }
        }

        public string Name { get; }
        public string Description { get; }
        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

        public int? DependencyGroupOrder { get; }
        public bool HasDependencyGroupOrder => DependencyGroupOrder.HasValue;
        
        public ObservableCollection<TaskParameterViewModel> Parameters { get; }
        public DelegateCommand ExecuteCommand { get; }

        public bool HasValidParameterValues
        {
            get { return Parameters.All(x => x.HasValidValue); }
        }

        public bool? WasIgnored
        {
            get { return Get(() => WasIgnored); }
            set { Set(() => WasIgnored, value); }
        }

        public bool? CouldBeExecuted
        {
            get { return Get(() => CouldBeExecuted); }
            set { Set(() => CouldBeExecuted, value); }
        }

        public async void Execute()
        {
            IsTaskExecuting = true;
            IsTaskProgressIndeterminate = true;

            WasSuccessful = await ExecuteAsync();

            IsTaskProgressIndeterminate = false;
            IsTaskExecuting = false;
        }

        private async Task<bool> ExecuteAsync()
        {
            return await Task.Factory.StartNew(() =>
            {
                _resultListener.OutputLine($"[{DateTime.Now}] Starting task '{Name}'...");

                try
                {
                    var instance = Activator.CreateInstance(TaskType);
                    _task.Invoke(instance, Parameters.Select(x => x.TypedValue).ToArray());

                    _resultListener.OutputLine($"[{DateTime.Now}] Task finished");

                    return true;
                }
                catch (Exception ex)
                {
                    EventAggregator.FireTaskExecutionFailed(ex.Message);
                    _resultListener.OutputLine("Exception occurred:");
                    _resultListener.OutputLine(ex.ToString());
                }
                finally
                {
                    InvokeUIAction(() => { IsTaskProgressIndeterminate = true; });
                }

                return false;
            });
        }

        public static string GetDisplayName(MethodInfo taskType)
        {
            //var attribute = taskType.GetCustomAttribute<UIExecutableAttribute>();
            //return attribute.DisplayName ?? taskType.Name;

            return taskType.Name;
        }
    }
}