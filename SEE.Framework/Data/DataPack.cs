using SEE.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework.Data
{
    public abstract class DataPack<TEntity, TResult>
    {
        #region ctor
        public DataPack()
        {
            PageSize = DefaultPageSize;
            SortDirection = DefaultSortDirection;
            SortColumn = DefaultSortColumn;
        }

        #endregion

        #region props
        /// <summary>
        /// Total number of records found
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Number of records on single page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Current page number (zero based)
        /// </summary>
        public int PageIndex { get; set; }

        // Number of pages
        public int NumOfPages
        {
            get
            {
                if (PageSize > 0)
                {
                    return (int)Math.Ceiling((decimal)TotalRecords / PageSize);
                }
                return 1;
            }
        }

        /// <summary>
        /// Current offset
        /// </summary>
        public int Offset
        {
            get
            {
                if (PageSize > 0)
                {
                    return PageIndex * PageSize;
                }
                return 0;
            }
        }

        /// <summary>
        /// Number of records on current page
        /// </summary>
        public int RecordsOnPage
        {
            get
            {
                if (PageSize > 0)
                {
                    return PageIndex * PageSize + PageSize > TotalRecords ? TotalRecords : PageIndex * PageSize + PageSize;
                }
                return TotalRecords;
            }
        }

        /// <summary>
        /// Determine if paging is enabled
        /// </summary>
        public bool IsPaged
        {
            get
            {
                return PageSize > 0 && TotalRecords > PageSize;
            }
        }

        /// <summary>
        /// Current direction of sort (1 = Ascending, 2 = Descending)
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// Name of current direction of sort
        /// </summary>
        public string SortDirectionString
        {
            get
            {
                return SortDirection.ToString();
            }
        }

        /// <summary>
        /// Current sorted by column name
        /// </summary>
        public string SortColumn { get; set; }

        /// <summary>
        /// Collection of found records
        /// </summary>
        public IEnumerable<TResult> Data { get; set; }
        #endregion

        #region abstract
        /// <summary>
        /// Override this to return default page size. Set return value to 0 to disable paging.
        /// </summary>
        abstract protected int DefaultPageSize { get; }

        /// <summary>
        /// Override this to return initial sorted by column name
        /// </summary>
        abstract protected string DefaultSortColumn { get; }

        /// <summary>
        /// Override this to return initial sort direction
        /// </summary>
        abstract protected SortDirection DefaultSortDirection { get; }

        /// <summary>
        /// Override this method to filter query
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <returns>Filtered query</returns>
        abstract protected IQueryable<TEntity> SetFilter(IQueryable<TEntity> query);

        /// <summary>
        /// Override this method to set current order and order direction
        /// </summary>
        /// <param name="query">Query to set order and direction</param>
        /// <returns>Ordered query</returns>
        abstract protected IQueryable<TEntity> SetSort(IQueryable<TEntity> query);

        #endregion

        #region public
        /// <summary>
        /// Returns filtered by <see cref="SetFilter(IQueryable{TEntity})"/> query.
        /// </summary>
        /// <param name="query">Query to filter</param>
        /// <returns>Filtered query</returns>
        public IQueryable<TEntity> GetFilteredQuery(IQueryable<TEntity> query)
        {
            query = SetFilter(query);
            TotalRecords = query.Count();
            return query;
        }

        /// <summary>
        /// Returns filtered by <see cref="SetFilter(IQueryable{TEntity})"/> and sorted by <see cref="SetSort(IQueryable{TEntity})"/> query.
        /// </summary>
        /// <param name="query">Query to filter and sort</param>
        /// <returns>Filtered and ordered query</returns>
        public IQueryable<TEntity> GetFilteredAndSortedQuery(IQueryable<TEntity> query)
        {
            return SetSort(GetFilteredQuery(query));
        }

        /// <summary>
        /// Returns filtered by <see cref="SetFilter(IQueryable{TEntity})"/>, sorted by <see cref="SetSort(IQueryable{TEntity})"/> and paged by <see cref="PageSize"/>, <see cref="PageIndex"/> pair query.
        /// </summary>
        /// <param name="query">Query to filter, sort and paging</param>
        /// <returns>Filtered, ordered and paged query</returns>
        public IQueryable<TEntity> GetFilteredSortedAndPagedQuery(IQueryable<TEntity> query)
        {
            if (Offset > TotalRecords)
            {
                PageIndex = 0;
            }
            if (PageSize > 0)
            {
                return GetFilteredAndSortedQuery(query).Skip(Offset).Take(PageSize);
            }
            return GetFilteredAndSortedQuery(query);
        }

        /// <summary>
        /// Executing prepared query and fill <see cref="Data"/> property with the result.
        /// </summary>
        /// <param name="entity">Data source for query.</param>
        /// <param name="select">Function to assign data from TSource to TResult</param>
        public void ExecuteQuery(IEnumerable<TEntity> entity, Func<IQueryable<TEntity>, IEnumerable<TResult>> select)
        {
            if (select == null)
            {
                throw new Exception("You must set your select function");
            }
            Data = select(GetFilteredSortedAndPagedQuery(entity.AsQueryable())).ToList();
        }

        #endregion
    }
}
