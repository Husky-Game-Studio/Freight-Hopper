using System;
using System.Collections;
using Ore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableAssets
{
    static HashMap<AssetReference, AsyncOperationHandleCounter> _assetHandles 
      = new HashMap<AssetReference, AsyncOperationHandleCounter>();
    public static void RequestAsset<T>(Action<T, AssetReference> onComplete, AssetReference assetReference) where T : UnityEngine.Object
    {
      ActiveScene.Coroutines.Run(RequestAssetInternal<T>(onComplete, assetReference));
    }
    public static IEnumerator RequestAssetInternal<T>(Action<T, AssetReference> onComplete, AssetReference assetReference) where T : UnityEngine.Object
    {
        if (!assetReference.RuntimeKeyIsValid())
        {
            Debug.LogError($"key invalid {assetReference}");
            yield break;
        }
        while (!assetReference.IsDone) yield return null;
        
        if (!_assetHandles.TryGetValue(assetReference, out var assetReferenceList))
        {
            //Debug.Log("adding ref " + assetReference.editorAsset.name);
            if (assetReference.IsValid()) // oh no :(
            {
                yield break;
            }

            var handle = assetReference.LoadAssetAsync<T>();
            yield return handle;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                Debug.LogError($"Failed to load {assetReference.Asset.name}");
                yield break;
            }

            assetReferenceList = new AsyncOperationHandleCounter { Handle = handle, Count = 0 };
        }

        T asset = assetReferenceList.Handle.Result as T;
        assetReferenceList.Count++;
        //Debug.Log($"adding count {assetReference.editorAsset.name} is " + assetReferenceList.Count);
        _assetHandles[assetReference] = assetReferenceList;
        onComplete.Invoke(asset, assetReference);
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
            int val = _assetHandles[reference].Count--;
            while (!reference.IsDone) yield return null;
            if (val != 0) yield break;

            while (condition())
            {
              yield return null;
              if (assetReference.Count > 0) yield break;
            }

            Addressables.Release(assetReference.Handle);
            //Debug.Log("Clearing ref " + reference.editorAsset.name);
            _assetHandles.Remove(reference);
        }
    }
    public static IEnumerator ReleaseAssetInternal(AssetReference reference, float secondsDelay)
    {
        //Debug.Log($"Release attempt for {reference.editorAsset.name} when at {GetCount(reference)}");
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            if (assetReference.Count == 0)
            {
                if (secondsDelay == 0)
                {
                    Addressables.Release(assetReference.Handle);
                    //Debug.Log("Clearing ref " + reference.editorAsset.name);
                    _assetHandles.Remove(reference);
                }
                yield break;
            }
            int val = _assetHandles[reference].Count--;
            while (!reference.IsDone) yield return null;
            if (val != 0) yield break;

            float duration = 0;
            while (duration < secondsDelay)
            {
              duration += Time.unscaledDeltaTime;
              yield return null;
              if (assetReference.Count > 0) yield break;
            }

            Addressables.Release(assetReference.Handle);
            Debug.Log("Clearing ref " + reference.editorAsset.name);
            _assetHandles.Remove(reference);
        }
    }
    public static void ReleaseAllOfAsset(AssetReference reference)
    {
        if (_assetHandles.TryGetValue(reference, out var assetReference))
        {
            Addressables.Release(assetReference.Handle);
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

    public static bool AssetIsLoading (AssetReference reference) => reference.IsDone;
    public static IEnumerator WaitUntilAssetIsLoaded(AssetReference reference)
    {
        if (reference.IsDone) yield break;
        while (!reference.IsDone) yield return null;
        yield return null; // if we don't wait an extra frame, the loading/releasing functions will make a duplicate due to using similar logic and needing to be "faster"
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
    }
}

