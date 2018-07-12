using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AndroidMultiThreading.Screens
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainScreen : AppCompatActivity, IHaveAProgressBar
    {
        static readonly string TAG = "ATM:MainScreen";
        TaskHelperFragment frag;
        Button updateUIButton, noupdateUIButton;
        ProgressBar progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            updateUIButton = FindViewById<Button>(Resource.Id.StartBackgroundTaskUpdateUI);
            noupdateUIButton = FindViewById<Button>(Resource.Id.StartBackgroundTaskNoUpdate);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            frag = FragmentManager.FindFragmentByTag<TaskHelperFragment>(TaskHelperFragment.FRAGMENT_TAG);
            if (frag == null)
            {
                frag = TaskHelperFragment.NewInstance(this);
                FragmentManager.BeginTransaction()
                               .Add(frag, TaskHelperFragment.FRAGMENT_TAG)
                               .Commit();
                Log.Debug(TAG, "Instantiated a new TaskHelperFragment.");
            }
            else
            {
                Log.Debug(TAG, "Using the pre-existing TaskHelperFragment.");
            }
        }

        public void DisplayProgressbar()
        {
            progressBar.Visibility = ViewStates.Visible;
            Log.Debug(TAG, "Should be showing the progress bar.");
        }

        protected override void OnResume()
        {
            base.OnResume();
            frag.DisplayProgressBarIfNecesary();
            noupdateUIButton.Click += frag.NoUpdateUIButtonClick;
            updateUIButton.Click += frag.UpdateUIButtonClick;
        }

        protected override void OnPause()
        {
            noupdateUIButton.Click -= frag.NoUpdateUIButtonClick;
            updateUIButton.Click -= frag.UpdateUIButtonClick;
            base.OnPause();
        }

        public void HideProgessbar()
        {
            progressBar.Visibility = ViewStates.Gone;
            Log.Debug(TAG, "Should NOT be showing the progress bar.");
        }
    }
}
