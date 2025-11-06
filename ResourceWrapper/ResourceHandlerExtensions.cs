using System;
using UnityEngine;

namespace Utils.ResourcesWrapper
{
    public static class ResourceHandlerExtensions
    {
        public static GameObject Instantiate(this ResourceHandler<GameObject> handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            GameObject prefab = handle.Load();
            if (prefab == null)
            {
                Debug.LogError($"[ResourceHandler] Instantiate failed: '{handle.Path}' is not a valid GameObject prefab.");
                return null;
            }

            return UnityEngine.Object.Instantiate(prefab);
        }

        public static GameObject Instantiate(
            this ResourceHandler<GameObject> handle,
            Vector3 position = default,
            Quaternion rotation = default,
            Transform parent = null,
            bool worldPositionStays = false)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            GameObject prefab = handle.Load();
            if (prefab == null)
            {
                Debug.LogError($"[ResourceHandler] Instantiate failed: '{handle.Path}' is not a valid GameObject prefab.");
                return null;
            }

            if (parent == null)
            {
                return UnityEngine.Object.Instantiate(prefab, position, rotation);
            }

            if (position == default && rotation == default)
            {
                return UnityEngine.Object.Instantiate(prefab, parent, worldPositionStays);
            }

            return UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
        }

        public static bool TryLoad<T>(this ResourceHandler<T> handle, out T asset)
            where T : UnityEngine.Object
        {
            asset = null;

            if (handle == null || handle.IsDisposed)
                return false;

            asset = handle.Load();
            return asset != null;
        }

        public static bool IsValid<T>(this ResourceHandler<T> handle)
            where T : UnityEngine.Object
        {
            return handle != null && !handle.IsDisposed;
        }

        public static void SafeRelease<T>(this ResourceHandler<T> handle)
            where T : UnityEngine.Object
        {
            if (handle != null && !handle.IsDisposed)
                handle.Release();
        }

        public static void ApplyTo(this ResourceHandler<Mesh> meshHandle, MeshFilter filter)
        {
            if (filter == null) return;
            if (!meshHandle.IsValid()) return;

            filter.mesh = meshHandle;
        }
    }
}
