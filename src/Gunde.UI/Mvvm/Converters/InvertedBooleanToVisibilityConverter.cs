using System.Windows;

namespace Gunde.UI.Mvvm.Converters
{
    public class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvertedBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Hidden) { }
    }
}