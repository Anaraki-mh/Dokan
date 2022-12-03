using Dokan.Core.DataAccess;
using Dokan.Domain.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokan.Services
{
    public class FileService : IFileService
    {
        #region Fields and Properties

        private IRepository<File> _repository { get; }

        #endregion


        #region Constructor

        public FileService(IRepository<File> repository)
        {
            _repository = repository;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates a new File as a new record in the File table
        /// </summary>
        /// <param name="entity">The File that gets added to the table in the database</param>
        /// <returns>A taks of File</returns>
        public async Task<File> CreateAsync(File entity)
        {
            string prefix;
            DateTime date = new DateTime();

            switch (entity.FileType)
            {
                case Domain.Enums.FileType.Image:
                    prefix = "IMG";
                    break;
                case Domain.Enums.FileType.Video:
                    prefix = "VID";
                    break;
                case Domain.Enums.FileType.Audio:
                    prefix = "AUD";
                    break;
                case Domain.Enums.FileType.PDF:
                    prefix = "PDF";
                    break;
                case Domain.Enums.FileType.TextFile:
                    prefix = "TXT";
                    break;
                default:
                    prefix = "FILE";
                    break;
            }

            entity.Title = $"{prefix}_{date.Year}{date.Month:00}{date.Day:00}{date.Hour:00}{date.Minute:00}{date.Second:00}_{entity.Title}";

            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Deletes a File from the File table
        /// </summary>
        /// <param name="id">The id of the File that gets deleted</param>
        /// <returns>A task</returns>
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(await FindByIdAsync(id));
        }

        /// <summary>
        /// Deletes a list of entities from the File table
        /// </summary>
        /// <param name="entities">The list of Files that get deleted from the database</param>
        /// <returns>A task</returns>
        public async Task DeleteRangeAsync(List<File> entities)
        {
            await _repository.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// Finds a record of the table File by its id
        /// </summary>
        /// <param name="id">The id of the File that gets returned</param>
        /// <returns>The (task of) File that has a matching id with the given id</returns>
        public async Task<File> FindByIdAsync(int id)
        {
            try
            {
                var entityList = await _repository.ListAsync();
                return entityList.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return new File();
            }
        }

        /// <summary>
        /// Returns a list containing all records of File table where the IsRemoved property is false
        /// </summary>
        /// <returns>A (task of) list of all the entities in File table with IsRemoved of false</returns>
        public async Task<List<File>> ListAsync()
        {
            var entityList = await _repository.ListAsync();

            if (entityList.Count < 1)
                entityList = new List<File>();

            return entityList;
        }

        /// <summary>
        /// Updates a File
        /// </summary>
        /// <param name="entity">The modified File</param>
        /// <returns>a Task</returns>
        public async Task UpdateAsync(File entity)
        {
            await _repository.UpdateAsync(entity);
        }


        /// <summary>
        /// Searches for the searchString in the File's Title and Content
        /// </summary>
        /// <param name="searchString">A string that gets searched for in the File table</param>
        /// <returns>A list of entities that contain the searchString in their Title or Content</returns>
        public async Task<List<File>> SearchAsync(string searchString)
        {
            var list = await ListAsync();
            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString) && list != null)
            {
                list = list
                    .Where(x => x.Title.ToLower().Contains(searchString))
                    .ToList();
            }
            else
                list = new List<File>();

            return list;
        }

        #endregion
    }
}
