using System;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Util;

namespace AndroidMultiThreading.Screens
{
    public class TaskHelperFragment : Fragment
    {
        public const string FRAGMENT_TAG = "state_frag";
        public const string TAG = "ATM:TaskHelperFragment";

        public WeakReference<IHaveAProgressBar> activityWrapper;
        bool isTaskARunning;
        bool isTaskBRunning;

        public static TaskHelperFragment NewInstance(IHaveAProgressBar activity)
        {
            var f = new TaskHelperFragment
                    {
                        activityWrapper = new WeakReference<IHaveAProgressBar>(activity)
                    };
            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            activityWrapper.SetTarget(null);
            activityWrapper = null;
        }

        public async void NoUpdateUIButtonClick(object sender, EventArgs e)
        {
            Log.Info(TAG, "Trying to start Task B is starting.");
            if (isTaskBRunning)
            {
                Log.Info(TAG, "Task B is already running.");
                return;
            }

            var taskB1 = new Task(() => { LongRunningProcess(10, "Task B-1"); });
            var taskB2 = new Task(() => { LongRunningProcess(5, "Task B-2"); });

            isTaskBRunning = true;
            taskB1.Start();
            taskB2.Start();

            await Task.WhenAll(taskB1, taskB2);

            Log.Info(TAG, "Task B has finished running");
            isTaskBRunning = false;
        }

        public async void UpdateUIButtonClick(object sender, EventArgs e)
        {
            Log.Info(TAG, "Try to start Task A.");
            if (activityWrapper.TryGetTarget(out var activity))
            {
                if (isTaskARunning)
                {
                    Log.Info(TAG, "Task A is already running.");
                }
                else
                {
                    isTaskARunning = true;
                    activity.DisplayProgressbar();

                    await Task.Run(() =>
                                   {
                                       Log.Info(TAG, "Begin task");
                                       LongRunningProcess(15, "Task A");
                                       Log.Info(TAG, "Done task");
                                   });

                    isTaskARunning = false;
                    activity.HideProgessbar();
                }
            }
            else
            {
                isTaskARunning = false;
                Log.Info(TAG, "Lost the reference to the UI element. Nothing to do with Task A.");
            }
        }

        /// <summary>
        ///     Simulation method to sit for a number of seconds.
        /// </summary>
        protected void LongRunningProcess(int seconds, string name)
        {
            Log.Info(TAG, $"Beginning long running process {name} for {seconds} seconds.");
            Thread.Sleep(seconds * 1000);
            Log.Info(TAG, $"Finished long running process {name} for {seconds} seconds.");
        }

        public void DisplayProgressBarIfNecesary()
        {
            if (activityWrapper.TryGetTarget(out var activity))
            {
                if (isTaskARunning)
                {
                    activity.DisplayProgressbar();
                }
                else
                {
                    activity.HideProgessbar();
                }
            }
            else
            {
                Log.Info(TAG, "No reference to MainScreen, nothing to do!");
            }
        }
    }
}
