using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KnowledgeTreeAPI.Models
{
    public class AuthorData
    {
        public string @type { get; set; }
        public string @class { get; set; }
        public string name { get; set; }
        public string org { get; set; }
    }
}