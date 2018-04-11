using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KnowledgeTreeAPI.Models
{
    public class QueryData
    {
        public SelectClause SelectClause { get; set; }
        public int NumberOfNeighbors { get; set; }
        public SelectClause NeighborsSelect { get; set; }
    }
}