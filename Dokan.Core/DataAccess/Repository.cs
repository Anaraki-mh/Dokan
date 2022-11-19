using Dokan.Core.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Core.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Fields and Properties

        private DokanContext _context { get; }

        #endregion


        #region Constructor

        public Repository()
        {
            _context = new DokanContext();
        }

        #endregion


        #region Methods

        /// <summary>
        /// Adds a new row to the table that matches the type of the entity.
        /// </summary>
        /// <param name="entity">The object that gets saved in the database as a row in the corresponding table.</param>
        /// <returns>The added entity</returns>
        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Deletes the entity from the correspinding table in the database
        /// </summary>
        /// <param name="entity">The entity that gets removed from the entity</param>
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }


        /// <summary>
        /// Gets a list of all the entities in a table in the database
        /// </summary>
        /// <returns>A list of all the saved entities in a table of the database</returns>
        public List<T> List()
        {
            return _context.Set<T>().ToList();
        }

        /// <summary>
        /// Modifies an existing record 
        /// </summary>
        /// <param name="entity">The modified version of an exisiting record in a table in the database</param>
        public void Update(T entity)
        {
            _context.Set<T>().AddOrUpdate(entity);
            _context.SaveChanges();
        }

        #endregion
    }
}
