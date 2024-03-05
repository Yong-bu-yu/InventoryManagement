using System.Globalization;

namespace InventoryManagement.Converter
{
    class IndexConverter<T> : IValueConverter where T : class
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Label label = (parameter as Binding).Source as Label;
            List<T> list = value as List<T>;
            T item = label.BindingContext as T;
            return list?.IndexOf(item) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
