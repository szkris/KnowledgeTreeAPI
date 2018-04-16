using KnowledgeTreeAPI.Models;
using Orient.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

// With this controller the API can handle select clauses then return the result of a query.
// TODO: The API also can handle search by ID and individual searches by other attributes.

namespace KnowledgeTreeAPI.Controllers
{
    public class SelectController : ApiController
    {
        private OServer Server = Models.Server.Connect();
        private ODatabase Database = new ODatabase("localhost", 2424, "TreeOfScience", ODatabaseType.Graph, "admin", "admin");

        // GET: api/Select
        //public IEnumerable<string> Get()
        //{
        //    return new string[] {"Elérhető vagyok kívülről! :)"};
        //}

         // GET: api/Select/year/2010
        [HttpGet]
        [Route("api/Select/year")]
        public OSqlSelect Get(int year)
        {
            var select = Database.Select().From("Document").Where("year").Equals(year);
            return select;
        }
        
        // GET - TEST
        [HttpGet]
        [Route("api/Select/gettest")]
        public List<ODocument> Get()
        {
            List<ODocument> entries = Database.Select().From("Document").Limit(50).Where("year").Like(1965).ToList();
            return entries;
        }

        // POST: api/Select
        public QueryResult Post([FromBody]QueryData data)
        {
            var alreadyWhere = false;
            var sel = Database.Select().From("...");

            foreach (var prop in typeof(SelectClause).GetProperties())
            {
                // FIXME
                sel = sel.WhereAndEquals(prop.Name, prop.GetValue(data.SelectClause).ToString(), ref alreadyWhere);
            }

            /*
            var select = db.Select().From("")
                .WhereAndEquals("id", data.SelectClause.Id.ToString(), ref alreadyWhere)
                .WhereAndEquals("", "", ref alreadyWhere);
            */
            
            return new QueryResult
            {
                
            };
        }
    }

    public static class OSqlSelectExtensions
    {
        public static OSqlSelect WhereAndEquals(this OSqlSelect select, string field, string item, ref bool alreadyWhere)
        {
            if (item == null)
            {
                return select;
            }

            if (alreadyWhere)
            {
                return select.And(field).Equals(item);
            }
            else
            {
                alreadyWhere = true;
                return select.Where(field).Equals(item);
            }
        }
    }
}
