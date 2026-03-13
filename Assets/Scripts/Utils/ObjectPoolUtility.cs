using UnityEngine;
using UnityEngine.Pool;
namespace Utils
{
    public class ObjectPoolUtility
    {
        public static ObjectPool<T> CreatePoolFast<T>(GameObject prefab, Transform parent = null,
            bool setsActive = true) where T : Component
        {
            return new ObjectPool<T>(() =>
                {
                    var go = Object.Instantiate(prefab);
                    if (setsActive)
                        go.SetActive(false);

                    if (parent != null)
                    {
                        go.transform.SetParent(parent);
                        go.transform.localScale = Vector3.one;
                    }

                    return go.GetComponent<T>();
                }, setsActive ? component => component.gameObject.SetActive(true) : null,
                setsActive ? component => component.gameObject.SetActive(false) : null,
                component =>
                {
                    if (component != null)
                        Object.Destroy(component.gameObject);
                });
        }
    }
}
