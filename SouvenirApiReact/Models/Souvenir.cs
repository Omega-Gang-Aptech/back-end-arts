using System;
using System.Collections.Generic;

#nullable disable

namespace SouvenirApiReact.Models
{
    public partial class Souvenir
    {
        public int SouvenirId { get; set; }
        public string SouvenirName { get; set; }
        public int? SouvenirPrice { get; set; }
        public string SouvenirImage { get; set; }
    }
}
