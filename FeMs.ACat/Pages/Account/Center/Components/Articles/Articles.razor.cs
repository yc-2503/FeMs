using FeMs.ACat.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FeMs.ACat.Pages.Account.Center
{
    public partial class Articles
    {
        [Parameter] public IList<ListItemDataType> List { get; set; }
    }
}