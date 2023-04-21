using System;
using System.Collections;
using Ore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public static class AddressableAssets
{
    static HashMap<AssetReference, AsyncOperationHandleCounter> _assetHandles 
      = new HashMap<AssetReference, AsyncOperationHandleCounter>();
    public static void RequestAsset<T>(Action<Object, AssetReference> onComplete, AssetReference assetReference)
    {
      ActiveScene.Coroutines.Run(RequestAssetInternal<T>(onComplete, assetReference));
    }
    public static IEnumerator RequestAssetInternal<T>(Action<Object, AssetReference> onComplete, AssetReference assetReference)
    {
        if (!assetReference.RuntimeKeyIsValid())
        {
            Debug.LogError($"key invalid {assetReference}");
            onComplete.Invoke(default,assetReference);
            yield break;
        }
        bool valueExists = _assetHandles.TryGetValue(assetReference, out var handleCounter);
        if (!valueExists)
        {
            handleCounter = new AsyncOperationHandleCounter
            {
                Handle = default,
                Count = 1,
                OnComplete = onComplete
            };
            _assetHandles[assetReference] = handleCounter;
            handleCounter.Handle = Addressables.LoadAssetAsync<T>(assetReference);
            yield return handleCounter.Handle;
            if (handleCounter.Handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handleCounter.Handle);
                Debug.LogError($"Failed to load {assetReference.Asset.name}");
                yield break;
            }
            handleCounter.OnComplete.Invoke(handleCounter.Handle.Result as Object, assetReference);
            handleCounter.OnComplete = null;
        }
        else
        {
            handleCounter.Count++;
            if (handleCounter.IsLoading)
            {
                handleCounter.OnComplete += onComplete;
                _assetHandles[assetReference] = handleCounter;
                while (handleCounter.IsLoading) yield return null;
            }
            else
            {
                _assetHandles[assetReference] = handleCounter;
                Object asset = handleCounter.Handle.Result as Object;
                onComplete.Invoke(asset, assetReference);
            }
        }
    }

    /// <summary>
    /// Releases the asset with the following reference. Only releases if count < 0
    /// </summary>
    /// <param name="secondsDelay">If reference count == 0, wait seconds before releasing just in-case we increment counter again so we don't release/load rapidly</param>
    public static void ReleaseAsset(AssetReference reference, float secondsDelay = 2)
    {
        ActiveScene.Coroutines.Run(ReleaseAssetInternal(reference, secondsDelay));
    }

    // Similar to ReleaseAsset with seconds overload, except if count == 0, we wait until condition is false before releasing. If count > 0 during that time then we don't release
    public static void ReleaseAsset(AssetReference reference, Func<bool> condition)
    {
        ActiveScene.Coroutines.Run(ReleaseAssetInternal(reference, condition));
    }
    public static IEnumerator ReleaseAssetInternal(AssetReference reference, Func<bool> condition)
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            while (assetReference.IsLoading) yield return null;
            int val = --_assetHandles[reference].Count;
            if (val > 0) yield break;

            while (condition())
            {
              yield return null;
              if (assetReference.Count > 0) yield break;
            }

            Addressables.Release(assetReference.Handle);
            _assetHandles.Remove(reference);
        }
    }
    public static IEnumerator ReleaseAssetInternal(AssetReference reference, float secondsDelay)
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            while (assetReference.IsLoading) yield return null;
            if (assetReference.Count == 0)
            {
                if (secondsDelay == 0)
                {
                    Addressables.Release(assetReference.Handle);
                    _assetHandles.Remove(reference);
                }
                yield break;
            }
            
            int val = --_assetHandles[reference].Count;
            if (val > 0) yield break;

            float duration = 0;
            while (duration < secondsDelay)
            {
              duration += Time.unscaledDeltaTime;
              yield return null;
              if (assetReference.Count > 0 || !assetReference.Handle.IsValid()) yield break;
            }
      
            Addressables.Release(assetReference.Handle);
            _assetHandles.Remove(reference);
        }
    }
    public static void ReleaseAllOfAsset(AssetReference reference)
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            if (assetReference.Handle.IsValid())
            {
                Addressables.Release(assetReference.Handle);
            }
            _assetHandles.Remove(reference);
        }
    }
    public static void ReleaseAllAssets()
    {
        foreach (var assetReference in _assetHandles)
        {
            Addressables.Release(assetReference.val.Handle);
        }
        _assetHandles.Clear();
    }

    public static bool AssetIsLoading (AssetReference reference)
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            return assetReference.IsLoading;
        }
        return false;
    }
    public static IEnumerator WaitUntilAssetIsLoaded(AssetReference reference)
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            while (assetReference.IsLoading) yield return null;
        }
    }
    public static bool InMemory(AssetReference reference)
    {
        return _assetHandles.ContainsKey(reference);
    }
    public static int GetCount(AssetReference reference) // should be internal and only visible to tests
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            return assetReference.Count;
        }
        return 0;
    }

    class AsyncOperationHandleCounter
    {
        public int Count;
        public AsyncOperationHandle Handle;
        public Action<Object, AssetReference> OnComplete;
        public bool IsLoading => OnComplete != null;
    }
}

