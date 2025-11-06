using System;
using System.Collections.Generic;
using System.Diagnostics; // for [Conditional]
using UnityEngine;

namespace Utils.ResourcesWrapper
{
    /// <summary>
    /// Global leak tracker for ResourceHandlers.
    /// Used when RESOURCE_TRACKING is defined.
    /// </summary>
    internal static class ResourceLeakTracker
    {
        private static readonly List<WeakReference<IResourceHandler>> _handles =
            new List<WeakReference<IResourceHandler>>();

        [Conditional("RESOURCE_TRACKING")]
        internal static void RegisterHandle(IResourceHandler handle)
        {
            if (handle == null) return;
            _handles.Add(new WeakReference<IResourceHandler>(handle));
        }

        [Conditional("RESOURCE_TRACKING")]
        public static void DumpLiveHandles()
        {
            int total = 0;
            int alive = 0;

            for (int i = _handles.Count - 1; i >= 0; i--)
            {
                if (!_handles[i].TryGetTarget(out IResourceHandler tracked) || tracked == null)
                {
                    _handles.RemoveAt(i);
                    continue;
                }

                total++;
                if (!tracked.IsDisposed)
                {
                    alive++;
                    UnityEngine.Debug.LogWarning(
                        $"[ResourceLeakTracker] Live handle: {tracked.ResourceType.Name} at '{tracked.Path}' not released."
                    );
                }
            }

            UnityEngine.Debug.Log($"[ResourceLeakTracker] Checked {total} tracked handles, {alive} still alive.");
        }
    }
}
