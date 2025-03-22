using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductMVC.Models
{
    public class PagedList<T> : List<T>
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRecords { get; private set; }
        public int TotalPages { get; private set; }

        public List<T> Items => this; // Explicitly expose items

        public PagedList(IEnumerable<T> items, int pageNumber, int pageSize, int totalRecords) : base(items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var totalRecords = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, pageNumber, pageSize, totalRecords);
        }
    }
}
