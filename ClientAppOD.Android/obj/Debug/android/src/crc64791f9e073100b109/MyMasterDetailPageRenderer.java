package crc64791f9e073100b109;


public class MyMasterDetailPageRenderer
	extends crc64720bb2db43a66fe9.MasterDetailPageRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("ClientAppOD.Droid.Rendrers.MyMasterDetailPageRenderer, ClientAppOD.Android", MyMasterDetailPageRenderer.class, __md_methods);
	}


	public MyMasterDetailPageRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == MyMasterDetailPageRenderer.class)
			mono.android.TypeManager.Activate ("ClientAppOD.Droid.Rendrers.MyMasterDetailPageRenderer, ClientAppOD.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public MyMasterDetailPageRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == MyMasterDetailPageRenderer.class)
			mono.android.TypeManager.Activate ("ClientAppOD.Droid.Rendrers.MyMasterDetailPageRenderer, ClientAppOD.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public MyMasterDetailPageRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == MyMasterDetailPageRenderer.class)
			mono.android.TypeManager.Activate ("ClientAppOD.Droid.Rendrers.MyMasterDetailPageRenderer, ClientAppOD.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
