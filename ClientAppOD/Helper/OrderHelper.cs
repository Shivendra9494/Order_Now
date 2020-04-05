using System;
using System.Linq;
using OD.Data;

namespace ClientAppOD.Helper
{
    public class OrderHelper
    {
        public int GetNewOrderItemId(MenuItem menuItem)
        {
            if(menuItem.OrderItems.Count>0)
            {
                return menuItem.OrderItems.Max(x => x.ID) + 1;
            }
            else
            {
                return 1;
            }
        }
        public string GetItemPrice(int id)
        {
            var menuItemCat = MenuCatHelper.MenuCategories.FirstOrDefault(x => x.Name != "Want to repeat?" && x.MenuItems.FirstOrDefault(y => y.Id == id) != null);
            var menuItem = menuItemCat.MenuItems.FirstOrDefault(x => x.Id == id);
            if (menuItem.Price > 0)
            {
                return "£" + menuItem.Price;
            }
            else
            {
                return "£" + menuItem.MenuItemProperties.OrderBy(x => x.Price).First().Price;
            }
        }
        public string GetPriceForMenuItemPage(int id)
        {
            string ret = "";
            
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
            catch (Exception ex)
            {
                var tt = ex;
            }
            return ret;
        }
    }
}
