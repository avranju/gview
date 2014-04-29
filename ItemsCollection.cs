using System.Collections;

namespace RanjuSoft.Utilities
{
	/// <summary>
    /// A collection of elements of type Item
    /// </summary>
    public class ItemsCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the ItemsCollection class.
        /// </summary>
        public ItemsCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the ItemsCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new ItemsCollection.
        /// </param>
        public ItemsCollection(Item[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the ItemsCollection class, containing elements
        /// copied from another instance of ItemsCollection
        /// </summary>
        /// <param name="items">
        /// The ItemsCollection whose elements are to be added to the new ItemsCollection.
        /// </param>
        public ItemsCollection(ItemsCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this ItemsCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this ItemsCollection.
        /// </param>
        public virtual void AddRange(Item[] items)
        {
            foreach (Item item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another ItemsCollection to the end of this ItemsCollection.
        /// </summary>
        /// <param name="items">
        /// The ItemsCollection whose elements are to be added to the end of this ItemsCollection.
        /// </param>
        public virtual void AddRange(ItemsCollection items)
        {
            foreach (Item item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type Item to the end of this ItemsCollection.
        /// </summary>
        /// <param name="value">
        /// The Item to be added to the end of this ItemsCollection.
        /// </param>
        public virtual void Add(Item value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic Item value is in this ItemsCollection.
        /// </summary>
        /// <param name="value">
        /// The Item value to locate in this ItemsCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this ItemsCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(Item value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this ItemsCollection
        /// </summary>
        /// <param name="value">
        /// The Item value to locate in the ItemsCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(Item value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the ItemsCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the Item is to be inserted.
        /// </param>
        /// <param name="value">
        /// The Item to insert.
        /// </param>
        public virtual void Insert(int index, Item value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the Item at the given index in this ItemsCollection.
        /// </summary>
        public virtual Item this[int index]
        {
            get
            {
                return (Item) this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific Item from this ItemsCollection.
        /// </summary>
        /// <param name="value">
        /// The Item value to remove from this ItemsCollection.
        /// </param>
        public virtual void Remove(Item value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by ItemsCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(ItemsCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public Item Current
            {
                get
                {
                    return (Item) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (Item) (this.wrapped.Current);
                }
            }

            public bool MoveNext()
            {
                return this.wrapped.MoveNext();
            }

            public void Reset()
            {
                this.wrapped.Reset();
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the elements of this ItemsCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual ItemsCollection.Enumerator GetEnumerator()
        {
            return new ItemsCollection.Enumerator(this);
        }
    }
}