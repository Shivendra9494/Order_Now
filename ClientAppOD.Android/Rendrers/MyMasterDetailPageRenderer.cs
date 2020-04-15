using System;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Views;
using ClientAppOD.Droid.Rendrers;
using ClientAppOD.MenuPages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(MenuCategoryPages), typeof(MyMasterDetailPageRenderer))]
namespace ClientAppOD.Droid.Rendrers
{
    public class MyMasterDetailPageRenderer : MasterDetailPageRenderer
    {
        public MyMasterDetailPageRenderer(Context context) : base(context) { }

        public static void Init()
        {
            //No init needed
        }

        protected override void OnElementChanged(VisualElement oldElement, VisualElement newElement)
        {
            base.OnElementChanged(oldElement, newElement);

            var width = Resources.DisplayMetrics.WidthPixels;

            var fieldInfo = GetType().BaseType.GetField("_masterLayout", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var _masterLayout = (ViewGroup)fieldInfo.GetValue(this);
            var lp = new DrawerLayout.LayoutParams(_masterLayout.LayoutParameters);

            MenuCategoryPages page = (MenuCategoryPages)newElement;
            lp.Width = (int)(page.WidthRatio * width);

            lp.Gravity = (int)GravityFlags.Left;
            _masterLayout.LayoutParameters = lp;
        }

    }
}