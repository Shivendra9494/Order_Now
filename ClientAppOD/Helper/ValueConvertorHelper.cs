using System;
using System.Globalization;
using System.Linq;
using ClientAppOD.Helper;
using Xamarin.Forms;

namespace ClientAppOD
{
    
    public class PriceConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string ret = "";
                var id = (int)value;
                var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.Name != "Want to repeat?" && x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
                var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
                try
                {
                    if (menuItem != null)
                    {
                        if (menuItem.MenuItemProperties.Count() == 0)
                        {
                            ret = "£" + menuItem.Price;

                        if (menuItem.MenuItemModels.Count() > 0)
                        {
                            foreach (var optionItem in menuItem.MenuItemModels)
                            {
                                if (optionItem.OptionItem != null)
                                {
                                    var options = optionItem.OptionItem.MenuDishProperties.Where(y => y.DishPropertyPrice > 0);
                                    if (options.Count() > 0)
                                    {
                                        ret = "from " + ret;
                                        return ret;
                                    }
                                }
                                if (optionItem.ExtraItem != null)
                                {
                                    var extras = optionItem.ExtraItem.MenuToppings.Where(y => y.ToppingPrice > 0);
                                    if (extras.Count() > 0)
                                    {
                                        ret = "from " + ret;
                                        return ret;
                                    }
                                }
                            }
                        }
                            return ret;
                        }
                        else
                        {
                            var proMinPrice = menuItem.MenuItemProperties.OrderBy(x => x.Price).First().Price;
                            ret = ret = "£" + proMinPrice;
                            var proDiffPrice = menuItem.MenuItemProperties.Where(x => x.Price > proMinPrice);
                            if (proDiffPrice.Count() > 0)
                            {
                                ret = ret = "from £" + proMinPrice;
                                return ret;
                            }
                            foreach (var menuPro in menuItem.MenuItemProperties)
                            {
                                if (menuPro.MenuItemPropertyModels.Count() > 0)
                                {
                                    var options = menuPro.MenuItemPropertyModels.Where(x => x.OptionItem.MenuDishProperties.Where(y => y.DishPropertyPrice > 0).Count() > 0);
                                    if (options.Count() > 0)
                                    {
                                        ret = "from " + ret;
                                        return ret;
                                    }
                                    var extras = menuPro.MenuItemPropertyModels.Where(x => x.ExtraItem.MenuToppings.Where(y => y.ToppingPrice > 0).Count() > 0);
                                    if (extras.Count() > 0)
                                    {
                                        ret = "from " + ret;
                                        return ret;
                                    }
                                }
                            }
                            return ret;
                        }
                    }
                }
                catch(Exception ex)
                {
                    var tt = ex;
                }
                return ret;
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? 1 : 0;
            }
        }
    public class OrderItemProVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.Name != "Want to repeat?" && x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
            var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
            if(menuItem.MenuItemProperties.Count>0)
            {
                return true;
            }
            else if (menuItem.MenuItemModels.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }
    
    public class OrderItemVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
            var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
            if (menuItem.MenuItemProperties.Count > 0)
            {
                return false;
            }
            else if (menuItem.MenuItemModels.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }
    public class ListHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
            var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
            int ExtraSpace = 0;
            string orderText="";
            if (menuItem.MenuItemProperties.Count > 0 || menuItem.MenuItemModels.Count > 0)
            {
                foreach(var item in menuItem.OrderItems)
                {
                    ExtraSpace += 50;
                    orderText += item.OrderText + " ";
                }
                var space =((orderText.Length / 20) * 14) + ExtraSpace;
                if(space<80 && space>0)
                {
                    space = 80;
                }
                return space;
            }
            else
            {
                return 0;
            }
            
            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((int)value!=0)
            {
                return (int)value / 60;
            }
            else
            {
                return 0;
            }
            
        }
    }
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value.ToString()=="0")
            {
                return "";
            }
            var id = (decimal)value;
            if(id>0)
            {
                return "£" + id.ToString();
            }
            else
            {
                return "";
            }
            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value != 0)
            {
                return (int)value / 60;
            }
            else
            {
                return 0;
            }

        }
    }
    public class IntToQtyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "0")
            {
                return " ";
            }
            var id = (int)value;
            if (id > 0)
            {
                return id.ToString();
            }
            else
            {
                return " ";
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value != 0)
            {
                return (int)value / 60;
            }
            else
            {
                return 0;
            }

        }
    }

    public class TopImageHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = Double.Parse(value.ToString());
            if(id>200)
            {
                return 0;
            }
            else
            {
                return 200 - id;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }
    public class TopBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = Double.Parse(value.ToString());
            if (id < 60)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }
    public class OrderItemQtyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
            var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
            if (menuItem.MenuItemProperties.Count > 0)
            {
                return false;
            }
            else if (menuItem.MenuItemModels.Count > 0)
            {
                return false;
            }
            else
            {
                if (menuItem.OrderItems.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }
    }
    public class OrderItemToQtyConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (int)value;
            var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
            var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
            return menuItem.OrderItems.Sum(x => x.Qta);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value != 0)
            {
                return (int)value / 60;
            }
            else
            {
                return 0;
            }

        }
    }
}
