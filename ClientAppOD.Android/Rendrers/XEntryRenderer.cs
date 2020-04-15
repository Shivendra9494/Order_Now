using System;
using Android.Graphics.Drawables;
using ClientAppOD.Droid.Rendrers;
using ClientAppOD.Helper;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(XEntry), typeof(XEntryRenderer))]
namespace ClientAppOD.Droid.Rendrers
{
    public class XEntryRenderer: EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
            }
        }
    }
}
