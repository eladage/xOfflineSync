package android.support.customtabs;


public class CustomTabsClient_CustomTabsCallbackImpl
	extends android.support.customtabs.CustomTabsCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Android.Support.CustomTabs.CustomTabsClient+CustomTabsCallbackImpl, Xamarin.Android.Support.CustomTabs", CustomTabsClient_CustomTabsCallbackImpl.class, __md_methods);
	}


	public CustomTabsClient_CustomTabsCallbackImpl ()
	{
		super ();
		if (getClass () == CustomTabsClient_CustomTabsCallbackImpl.class)
			mono.android.TypeManager.Activate ("Android.Support.CustomTabs.CustomTabsClient+CustomTabsCallbackImpl, Xamarin.Android.Support.CustomTabs", "", this, new java.lang.Object[] {  });
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
