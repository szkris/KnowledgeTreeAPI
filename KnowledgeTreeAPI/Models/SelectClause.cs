using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KnowledgeTreeAPI.Models
{
    public class SelectClause
    {
        public string[] Id { get; set; }

        public string[] Title { get; set; }

        public AuthorData[] Author { get; set; }

        public string[] Venue { get; set; }

        public int[] Year { get; set; }

        public string[] Keyword { get; set; }

        public string[] Fos { get; set; }

        public int NCitation { get; set; }

        public string[] Reference { get; set; }

        public string[] PageStart { get; set; }

        public string[] PageEnd { get; set; }

        /// <summary>
        /// Paper type: journal, book title etc.
        /// </summary>
        public string[] DocType { get; set; }

        public string[] Lang { get; set; }

        public string[] Publisher { get; set; }

        public string[] Volume { get; set; }

        public string[] Issue { get; set; }

        public string[] Issn { get; set; }

        public string[] Isbn { get; set; }

        public string[] Doi { get; set; }

        public string[] Pdf { get; set; }

        public string[] Url { get; set; }

        public string[] Abstract { get; set; }
    }
}