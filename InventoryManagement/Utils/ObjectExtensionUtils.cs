using System.Reflection;

public static class ObjectExtensionUtils
{
    public static object GetObjectPropertyValue(this object _this, string propertyName)
    {
        Type type = _this.GetType();
        PropertyInfo propertyInfo = type.GetProperty(propertyName);
        return propertyInfo.GetValue(_this);
    }
    public static bool TryGetObjectPropertyValue(this object _this, string propertyName, out object value)
    {
        try
        {
            Type type = _this.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            value = propertyInfo.GetValue(_this);
            return true;
        }
        catch (Exception)
        {
            value = null;
            return false;
        }
    }
    public static void SetObjectPropertyValue(this object _this, string propertyName, object value)
    {
        Type type = _this.GetType();
        PropertyInfo propertyInfo = type.GetProperty(propertyName);
        propertyInfo.SetValue(_this, value);
    }
    public static void TrySetObjectPropertyValue(this object _this, string propertyName, object value)
    {
        try
        {
            Type type = _this.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            propertyInfo.SetValue(_this, value);
        }
        catch (Exception)
        {
            return;
        }
    }
}
