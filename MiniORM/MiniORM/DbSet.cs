﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiniORM
{
    public class DbSet<TEntity> : ICollection<TEntity>
        where TEntity : class, new()
    {
        public DbSet(IEnumerable<TEntity> entities)
        {
            this.Entities = entities.ToList();
            this.ChangeTracker = new ChangeTraker<TEntity>(entities);
        }

        internal ChangeTraker<TEntity> ChangeTracker {get; set;}

        internal IList<TEntity> Entities { get; set; }

        public int Count => this.Entities.Count();

        public bool IsReadOnly => this.Entities.IsReadOnly;

        public void Add(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "item can`t be null");
            }
            this.Entities.Add(item);
            this.ChangeTracker.Add(item);
        }

        public bool Remove(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item),
                    "item can`t be null");
            }

            bool removedSuccesfully = this.Entities
                .Remove(item);

            if (removedSuccesfully)
            {
                this.ChangeTracker.Remove(item);
            }

            return removedSuccesfully;
        }

        public void RemoveRage(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities.ToArray())
            {
                this.Remove(entity);
            }
        }

        public void Clear()
        {
            while(this.Entities.Any())
            {
                TEntity entity = this.Entities
                   .First();
                this.Remove(entity);
                    
            }
        }


        public bool Contains(TEntity item) => this.Entities.Contains(item);

        public void CopyTo(TEntity[] array, int startIndex)
        {
            this.Entities.CopyTo(array, startIndex);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}