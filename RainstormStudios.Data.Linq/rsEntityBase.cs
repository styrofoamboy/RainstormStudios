//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Data.Linq
{
    /********************************************************************************
     * READ ME FIRST...
     * 
     * All of the classes below have a region called 'Implentation Instructions' with
     * a detailed explaination of how you use them.  Please read these instructions
     * before deciding which of these classes you want to implement.
     * 
     */


    /// <summary>
    /// This class is used to more easily identify your Linq2Sql class in code, and
    /// provides some additional functionality for those entities.  To make your
    /// Linq2Sql entities inherit from this class, please look at the inline
    /// documentation in the code file.
    /// </summary>
    [Serializable]
    public abstract class rsEntityBase
    {
        #region Implementation Instructions
        /*
         *  --=< How to Make Your Linq2Sql Entities Inherit from this Class >=--
         * 
         * Linq2Sql is awesome, but the auto-generated entity classes all inherit directly
         * from System.Object, so it's imposible to determine (in an generic method, for example)
         * if you're actually dealing with a Linq2Sql entity instance.
         * 
         * This will "fix" that problem.
         * 
         * Microsoft has not provided an "easy" way to do this yet (and might never do so, since they seem
         * to be pushing the EntityFramework over Linq2Sql), so it will require you to "tinker" with
         * the DBML in Notepad.  It's just an XML doc, so this is really super easy.
         * 
         * NOTE:  You will have to have this class library referenced in your application, or you will get
         * compile errors after making this change.  That *should* be obvious, but better to get that out
         * of the way.
         * 
         * NOTE:  If you implement the 'rsEntityDetachable' as a base class in your DBML, you *will* be
         * required to implement the abstract 'Detach' method for each generated Linq2Sql class.  It might
         * work better if you use the 'rsEntityBase' and then alter the code to inherit from the detachable
         * class as needed.  You have been warned!
         * 
         * 
         * Now, on to the "fun"...
         * 
         * 
         * 1) Create your DBML, as normal.
         * 
         * 2) Close your project/solution in VisualStudio.
         * 
         * 3) Navigate to where the web site/application files are located on your computer with Windows
         *      Explorer
         * 
         * 4) If you want to make a copy of the DBML (and related .cs, .designer.cs, and dbml.layout files) now is your chance.
         * 
         * 5) Open the DBML file in Notepad (it's just an XML document).
         *      
         * 6) At the very top of the file, there should be a node called "Database".  This will contain
         *      attributes for the context "Name" and "Serialization" method.  Add the following attribute:
         *      
         *      EntityBase="RainstormStudios.Data.Linq.rsEntityBase"
         *      
         * 7) Save the file in Notepad and close it
         * 
         * 8) Re-open the project in VisualStudio, and compile.
         * 
         * 9) All your Linq2Sql entities will now inherit from this class, so you can easily identify them
         *      in your code.
         * 
         */
        #endregion

        #region Declarations
        //***************************************************************************
        // Constants
        // 
        private const System.Reflection.BindingFlags
            flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.DeclaredOnly;
        //***************************************************************************
        // Public Fields
        // 
        public bool
            AllowCreateNewContext = true;
        //***************************************************************************
        // Public Events
        // 
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public object GetMyTable<TContext>()
            where TContext : System.Data.Linq.DataContext, new()
        {
            var context = this.GetMyContext<TContext>();
            return GetMyTable<TContext>(context);
        }
        public object GetMyTable<TContext>(TContext context)
            where TContext : System.Data.Linq.DataContext, new()
        {
            return GetMyTable(this, context);
        }

        public System.Data.Linq.Mapping.MetaDataMember[] GetPrimaryKey<TContext>()
            where TContext : System.Data.Linq.DataContext, new()
        {
            var context = this.GetMyContext<TContext>();
            return GetPrimaryKey(context);
        }
        public System.Data.Linq.Mapping.MetaDataMember[] GetPrimaryKey<TContext>(TContext context)
            where TContext : System.Data.Linq.DataContext, new()
        {
            return (from m in context.Mapping.MappingSource.GetModel(typeof(TContext)).GetMetaType(this.GetType()).DataMembers
                    where m.IsPrimaryKey == true
                    select m).ToArray();
        }

        public object GetPrimaryKeyValue<TContext>()
            where TContext : System.Data.Linq.DataContext, new()
        {
            var context = this.GetMyContext<TContext>();
            return GetPrimaryKeyValue<TContext>(context);
        }
        public object GetPrimaryKeyValue<TContext>(TContext context)
            where TContext : System.Data.Linq.DataContext, new()
        {
            System.Data.Linq.Mapping.MetaDataMember[] pkMembers = this.GetPrimaryKey<TContext>(context);
            if (pkMembers.Length > 1)
                throw new Exception("Unable to deal with multiple primary key fields at this time.");

            string pk = pkMembers[0].Name;

            var table = GetMyTable(this, context);
            return this.GetType().GetProperty(pk, flags).GetValue(this, null);
        }

        public T GetMyContext<T>()
            where T : System.Data.Linq.DataContext, new()
        {
            Type objType = this.GetType();

            Delegate[] onChangingHandlers = null;
            System.Reflection.FieldInfo fEvent = objType.GetField("PropertyChanging", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            MulticastDelegate dEvent = (MulticastDelegate)fEvent.GetValue(this);
            if (dEvent != null)
                onChangingHandlers = dEvent.GetInvocationList();

            if (onChangingHandlers == null || onChangingHandlers.Length < 1)
                return this.AllowCreateNewContext ? new T() : null;

            for (int i = 0; i < onChangingHandlers.Length; i++)
            {
                Type tgtType = onChangingHandlers[i].Target.GetType();
                if (tgtType.Name == "StandardChangeTracker")
                {
                    System.Reflection.FieldInfo fiSvcs = tgtType.GetField("services", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (fiSvcs == null)
                        return this.AllowCreateNewContext ? new T() : null; ;

                    object fiSvcsVal = fiSvcs.GetValue(onChangingHandlers[i].Target);
                    if (fiSvcsVal == null)
                        return this.AllowCreateNewContext ? new T() : null; ;

                    Type tSvcs = fiSvcsVal.GetType();
                    System.Reflection.PropertyInfo pi = tSvcs.GetProperty("Context", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty);
                    if (pi == null)
                        return this.AllowCreateNewContext ? new T() : null; ;

                    object objCtxVal = pi.GetValue(fiSvcsVal, null);
                    if (objCtxVal == null || !(objCtxVal is T))
                        return this.AllowCreateNewContext ? new T() : null;

                    else
                        return (T)objCtxVal;
                }
            }
            return this.AllowCreateNewContext ? new T() : null;
        }

        /// <summary>
        /// Determines if this Linq2Sql entity is attached to the specified context.
        /// </summary>
        /// <typeparam name="T">A Linq2Sql entity that inherits from the <see cref="T:RainstormStudios.Data.Linq.rsEntityBase"/> class.</typeparam>
        /// <param name="entity">The Linq2Sql entity to test.</param>
        /// <param name="dbContext">An <see cref="T:System.Data.Linq.DataContext"/> Linq2Sql data context.</param>
        /// <returns>A <see cref="T:System.Boolean"/> value indicating true, if the entity is attached. Otherwise, false.</returns>
        public bool IsAttached<TContext>(TContext dbContext)
            where TContext : System.Data.Linq.DataContext
        {
            return CheckEntityIsAttached(dbContext, this);
        }

        public byte[] GetSerialized()
        {
            return rsEntityBase.GetSerialized(this);
        }

        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// NOTE:  This may not work the way you expect.  The cloned entity will have no relation
        /// to the original context.  If you use the "InsertOnSubmit", the existing record will
        /// be updated.  If you "Add" to a queried entity's foreign key collection, then a new
        /// record will be created.
        /// </summary>
        public static T Clone<T>(T source)
            where T : rsEntityBase
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            using (var ms = new System.IO.MemoryStream())
            {
                dcs.WriteObject(ms, source);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }
        public static byte[] GetSerialized<T>(T source)
            where T : rsEntityBase
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            using (var ms = new System.IO.MemoryStream())
            {
                dcs.WriteObject(ms, source);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return ms.ToArray();
            }
        }
        public static T GetDeserialized<T>(byte[] serialized)
            where T : rsEntityBase
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(serialized, 0, serialized.Length);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected virtual System.Data.Linq.Table<TEntity> GetMyTable<TEntity, TContext>(TEntity entity, TContext context)
            where TEntity : rsEntityBase
            where TContext : System.Data.Linq.DataContext
        {
            var table = context.GetTable<TEntity>();
            return table;
        }
        private static bool CheckEntityIsAttached<TContext, TEntity>(TContext dbContext, TEntity entity)
            where TContext : System.Data.Linq.DataContext
            where TEntity : rsEntityBase
        {
            System.Data.Linq.Table<TEntity> entityTbl = dbContext.GetTable<TEntity>();
            return (entityTbl.GetOriginalEntityState(entity) != null);
        }
        #endregion
    }
    /// <summary>
    /// This class further extends the functionality of the <see cref="T:ITCWebToolkit.Linq.ITCEntityBase"/> class by
    /// enabling on-the-fly attach/detach methods for your Linq2Sql entities.
    /// </summary>
    [Serializable]
    public abstract class rsEntityDetachable : rsEntityBase, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
    {
        #region Implementation Instructions
        /*
         * This class will require a bit more work than the rsEntityBase class.
         * 
         * There are actually two ways to do this:
         * 
         *      1) Implement it the same way as the rsEntity base so that all of your Linq2Sql entities
         *          inherit from it, and then implement the abstract "Detach" method.  This method will
         *          require the most work, since you will have to define the "Detach" method on every
         *          generated Linq2Sql entity.  Depending on how many entities there are, this could
         *          become time comsuming.
         *          
         *      2) Have all of your Linq2Sql entities inherit from rsEntity base, and then implment this
         *          via partial class definitions.  This method requires less work, but will mean that
         *          only the entities you specify will be able to be used in a detachable maner and
         *          requires a basic understanding of C# 'Partial Classes'.
         *          
         * I will show how to accomplish both below.
         * 
         * -=< To implement across the board >=-
         * 
         * 
         * 
         * -=< To implement for specific entities >=-
         *      
         */
        #endregion

        #region Declarations
        //***************************************************************************
        // Events
        // 
        public virtual event System.ComponentModel.PropertyChangingEventHandler
            PropertyChanging;
        public virtual event System.ComponentModel.PropertyChangedEventHandler
            PropertyChanged;
        #endregion

        #region Public Methods
        //***************************************************************************
        // Abstract Methods
        // 
        public abstract void Detach();
        #endregion

        #region Private Methods
        //***************************************************************************
        // Protected Methods
        // 
        protected static System.Data.Linq.EntityRef<TEntity> Detach<TEntity>(System.Data.Linq.EntityRef<TEntity> entity)
            where TEntity : rsEntityDetachable
        {
            // HasLoadedOrAssignedValue checks to see if this entity has even been
            //   'populated'.  This method prevents the entity from being lazily
            //   populated by the Linq2Sql context.
            if (!entity.HasLoadedOrAssignedValue || entity.Entity == null)
                // Not populated yet, so just return a new instance.
                return new System.Data.Linq.EntityRef<TEntity>();

            // Calls that entity's 'Detach' method (defined by epEntity base).
            entity.Entity.Detach();

            // Return a new instance of the original entity.
            return new System.Data.Linq.EntityRef<TEntity>(entity.Entity);
        }
        protected static System.Data.Linq.EntitySet<TEntity> Detach<TEntity>(System.Data.Linq.EntitySet<TEntity> set, Action<TEntity> onAdd, Action<TEntity> onRemove)
            where TEntity : rsEntityDetachable
        {
            if (set == null || !set.HasLoadedOrAssignedValues)
                return new System.Data.Linq.EntitySet<TEntity>(onAdd, onRemove);

            // copy list and detach all entities
            var list = set.ToList();
            list.ForEach(t => t.Detach());

            // Create the new EntitySet and return it.
            var newSet = new System.Data.Linq.EntitySet<TEntity>(onAdd, onRemove);
            newSet.Assign(list);
            return newSet;
        }
        protected static System.Data.Linq.Link<TEntity> Detach<TEntity>(System.Data.Linq.Link<TEntity> value)
        {
            if (!value.HasLoadedOrAssignedValue)
                return default(System.Data.Linq.Link<TEntity>);

            return new System.Data.Linq.Link<TEntity>(value.Value);
        }
        #endregion
    }
    /// <summary>
    /// This class does not yet function.  Eventually, I hope to complete this generic 'context-less' class
    /// that can be use a 'Repository pattern' for Linq2Sql objects.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class rsEntityRepository<TEntity> : rsEntityDetachable, IRepository<TEntity>
        where TEntity : rsEntityDetachable
    {
        #region Implementation Instructions
        /*
         * This class requires the most work to implement, so here's the instructions
         * on how to make this happen.
         * 
         * First, you will have to implement the rsEntityDetachable class above for
         * your Linq2Sql entities.
         * 
         * NOTE:  If you want to use this class, you *must* choose option #1 from the
         * rsEntityDetachable implementation methods.
         * 
         * 
         * 
         */
        #endregion

        #region Public Methods
        //***************************************************************************
        // Abstract Methods
        // 
        public abstract TEntity Save();
        //***************************************************************************
        // Public Methods
        // 
        public virtual TEntity GetByPrimaryKey(object pkValue)
        {
            throw new NotImplementedException();
        }
        public virtual IList<TEntity> GetAllRecords()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected static TEntity Save<TContext>(TEntity entity)
            where TContext : System.Data.Linq.DataContext, new()
        {
            using (var ctx = new TContext())
            {

                object pkVal = entity.GetPrimaryKeyValue(ctx);

                if (pkVal is int)
                    if ((int)pkVal > 0)
                        ctx.GetTable<TEntity>().Attach(entity, true);
                    else
                        ctx.GetTable<TEntity>().InsertOnSubmit(entity);
                else
                    if (pkVal != null)
                        ctx.GetTable<TEntity>().Attach(entity, true);
                    else
                        ctx.GetTable<TEntity>().InsertOnSubmit(entity);

                ctx.SubmitChanges();
                entity.Detach();
            }
            return entity;
        }
        private static bool EntityContainsTimestamp<TContext>(TContext context)
            where TContext : System.Data.Linq.DataContext, new()
        {
            return (context.Mapping.MappingSource.GetModel(context.GetType()).GetMetaType(typeof(TEntity)).DataMembers.Where(dm => dm.IsVersion == true).FirstOrDefault() != null);
        }
        #endregion
    }
    public abstract class rsContextFactory<TContext> : IRepositoryFactory<TContext>
        where TContext : System.Data.Linq.DataContext
    {
    }
    /// <summary>
    /// This interface is specifically for 'Repository pattern' entities and gives a simple way to cast
    /// an 'unknown' entity type for access to these methods.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        TEntity GetByPrimaryKey(object pkValue);
        IList<TEntity> GetAllRecords();
        TEntity Save();
    }
    public interface IRepositoryFactory<TContext>
        where TContext : System.Data.Linq.DataContext
    {
    }
}
