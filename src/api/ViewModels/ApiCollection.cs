using System.Collections.Generic;


namespace MawApi.ViewModels
{
    public class ApiCollection<T>
    {
        public List<T> Items { get; }
        public long Count => Items.Count;


        public ApiCollection(List<T> items)
        {
            Items = items;
        }
    }
}
