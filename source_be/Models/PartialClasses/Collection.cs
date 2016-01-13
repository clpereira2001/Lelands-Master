using System;

namespace Vauction.Models
{
    [Serializable]
    public partial class Collection : ICollection
    {
        public void UpdateFields(string title, string webTitle, string description)
        {
            Title = title;
            Description = description;
            WebTitle = webTitle;
        }
    }
}