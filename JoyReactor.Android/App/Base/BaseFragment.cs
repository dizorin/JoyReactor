﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V4.App;
using Autofac;
using Android.OS;
using JoyReactor.Core.Model.Inject;

namespace JoyReactor.Android.App.Base
{
	public class BaseFragment : Fragment
	{
		public const string Arg1 = "arg1";
		public const string Arg2 = "arg2";
		public const string Arg3 = "arg3";
		public const string Arg4 = "arg4";

		private List<Action> onResumeEvents = new List<Action>();
		private List<Action> onPauseEvents = new List<Action>();

		protected static T NewFragment<T>( params object[] args) where T : Fragment {
			var bundle = new Bundle ();
			for (int i = 0; i < args.Length; i++) {
				var a = args [i];
				var key = "arg" + (i + 1);

				if (a is string) bundle.PutString (key, (string)a);
				else if (a is int) bundle.PutInt (key, (int)a);
				else if (a is long) bundle.PutLong(key, (long)a);
			}

			var f = Activator.CreateInstance<T> ();
			f.Arguments = bundle;
			return f;
		}

		protected void AddLifeTimeEvent(Action onResume, Action onPause) {
			onResumeEvents.Add (onResume);
			onPauseEvents.Add (onPause);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			onResumeEvents.ForEach (s => s ());
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			onResumeEvents.Clear ();
			onPauseEvents.Clear ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			onPauseEvents.ForEach (s => s ());
		}
	}
}