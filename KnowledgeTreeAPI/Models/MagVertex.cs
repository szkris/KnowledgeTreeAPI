using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KnowledgeTreeAPI.Models
{
    public class MagVertex
    {
        public string mag_id { get; set; }
        public string title { get; set; }
        public AuthorData[] authors { get; set; }
        public string venue { get; set; }
        public int year { get; set; }
        public string[] keywords { get; set; }
        public string[] fos { get; set; }
        public int n_citation { get; set; }
        public string[] references { get; set; }
        public string page_stat { get; set; }
        public string page_end { get; set; }
        public string doc_type { get; set; }
        public string lang { get; set; }
        public string publisher { get; set; }
        public string volume { get; set; }
        public string issue { get; set; }
        public string issn { get; set; }
        public string isbn { get; set; }
        public string doi { get; set; }
        public string pdf { get; set; }
        public string[] url { get; set; }
        public string Abstract { get; set; }
    }
}