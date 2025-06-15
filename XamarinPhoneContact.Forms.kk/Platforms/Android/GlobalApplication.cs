/*using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace XamarinPhoneContact.Droid
{
    [Application]
    public class GlobalApplication :Android.App.Application, Application.IActivityLifecycleCallbacks
    {

        internal static Context CurrentContext { get; private set; }

        public GlobalApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {

        }
        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CurrentContext = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
           // throw new NotImplementedException();
        }

        public void OnActivityPaused(Activity activity)
        {
           // throw new NotImplementedException();
        }

        public void OnActivityResumed(Activity activity)
        {
            CurrentContext = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            
        }

        public void OnActivityStarted(Activity activity)
        {
            CurrentContext = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
           
        }
    }
}*/
