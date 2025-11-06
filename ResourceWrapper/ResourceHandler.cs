using System;
using UnityEngine;

namespace Utils.ResourcesWrapper
{
    public sealed class ResourceHandler<T> : IDisposable, IResourceHandler
        where T : UnityEngine.Object
    {
        private T _asset;
        private bool _isDisposed;
        private readonly bool _autoUnload;

        public Type ResourceType => typeof(T);
        public string Path { get; }
        public bool IsLoaded => _asset != null;
        public bool IsDisposed => _isDisposed;

        public ResourceHandler(string path, bool autoUnload = true)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Resource path cannot be null or empty", nameof(path));

            Path = path;
            _autoUnload = autoUnload;
#if RESOURCE_TRACKING
            ResourceLeakTracker.RegisterHandle(this);
#endif
        }

        public T Load()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(
                    $"ResourceHandler<{typeof(T).Name}> for '{Path}' has been disposed.");

            if (_asset == null)
            {
                _asset = Resources.Load<T>(Path);
                if (_asset == null)
                {
                    Debug.LogError($"[ResourceHandler] Failed to load '{Path}' as {typeof(T).Name}.");
                }
            }

            return _asset;
        }

        public T Value => Load();

        public static implicit operator T(ResourceHandler<T> handle)
        {
            return handle != null ? handle.Load() : null;
        }

        public void Release()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            if (_asset != null && _autoUnload)
            {
                if (!(_asset is GameObject or Component))
                {
                    Resources.UnloadAsset(_asset);
                }

                _asset = null;
            }
        }

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        ~ResourceHandler()
        {
            if (!_isDisposed)
            {
                Debug.LogWarning(
                    $"[ResourceHandler] Potential leak: ResourceHandler<{typeof(T).Name}> " +
                    $"for '{Path}' was not released/disposed."
                );
            }
        }
    }

    /// <summary>
    /// Non-generic interface for resource handlers.
    /// </summary>
    internal interface IResourceHandler
    {
        Type ResourceType { get; }
        string Path { get; }
        bool IsLoaded { get; }
        bool IsDisposed { get; }
    }
}