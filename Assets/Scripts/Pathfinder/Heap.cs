using System;


public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;

        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T itemToRemove = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return itemToRemove;
    }

    public void UpdateItem(T item) => SortUp(item);

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }
    public bool Contains(T item) => Equals(items[item.HeapIndex], item);

    private void SortDown(T item)
    {
        while (true)
        {
            int firstChildIndex = item.HeapIndex * 2 + 1;
            int secondChildIndex = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (firstChildIndex < currentItemCount)
            {
                swapIndex = firstChildIndex;

                if (secondChildIndex < currentItemCount)
                {
                    if (items[firstChildIndex].CompareTo(items[secondChildIndex]) < 0)
                    {
                        swapIndex = secondChildIndex;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

    }

    private void SortUp(T item)
    {
        while (item.HeapIndex > 0)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            if (item.CompareTo(items[parentIndex]) > 0)
            {
                Swap(item, items[parentIndex]);
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int temp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = temp;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}
