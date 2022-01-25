package crc644ac82cd32e557928;


public class FileProviderClass
	extends android.support.v4.content.FileProvider
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("EloComandas.Provider.FileProviderClass, EloComandas", FileProviderClass.class, __md_methods);
	}


	public FileProviderClass ()
	{
		super ();
		if (getClass () == FileProviderClass.class)
			mono.android.TypeManager.Activate ("EloComandas.Provider.FileProviderClass, EloComandas", "", this, new java.lang.Object[] {  });
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
