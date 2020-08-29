using System;
using System.Collections.Generic;
using UnityEngine;

/*

 Modifications have been made to the original:
 Added an out parameter to indicate if the object has already been recycled
 This is useful when you want to add logic on first creation of an object.

 Renmaed namespace to prevent overlap issues for users that potentially might use this pooling solution already.

 Source : https://github.com/thefuntastic/unity-object-pool
 Licence : MIT

*/

namespace MonsterLove.HealthPro.Collections
{
    public class ObjectPool<T> where T : class
    {
        private List<ObjectPoolContainer<T>> list;
        private Dictionary<T, ObjectPoolContainer<T>> lookup;
        private Func<T> factoryFunc;
        private int lastIndex = 0;

        public ObjectPool(Func<T> factoryFunc, int initialSize)
        {
            this.factoryFunc = factoryFunc;

            list = new List<ObjectPoolContainer<T>>(initialSize);
            lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

            Warm(initialSize);
        }

        private void Warm(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                CreateConatiner();
            }
        }

        private ObjectPoolContainer<T> CreateConatiner()
        {
            var container = new ObjectPoolContainer<T>();
            container.Item = factoryFunc();
            list.Add(container);
            return container;
        }

        public T GetItem(out bool isRecycled)
        {
            isRecycled = false;
            ObjectPoolContainer<T> container = null;

            for (int i = 0; i < list.Count; i++)
            {
                lastIndex++;
                if (lastIndex > list.Count - 1) lastIndex = 0;

                if (list[lastIndex].Used)
                {
                    continue;
                }
                else
                {
                    if (list[lastIndex].Item as GameObject != null)
                    {
                        isRecycled = true;
                        container = list[lastIndex];
                        break;
                    }
                    else
                    {
                        list.RemoveAt(lastIndex);
                        continue;
                    }
                }
            }

            if (container == null)
            {
                container = CreateConatiner();
            }

            container.Consume();
            lookup.Add(container.Item, container);
            return container.Item;
        }

        public void ReleaseItem(object item)
        {
            ReleaseItem((T)item);
        }

        public void ReleaseItem(T item)
        {
            if (lookup.ContainsKey(item))
            {
                var container = lookup[item];
                container.Release();
                lookup.Remove(item);
            }
            else
            {
                Debug.LogWarning("This object pool does not contain the item provided: " + item);
            }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public int CountUsedItems
        {
            get { return lookup.Count; }
        }
    }
}
