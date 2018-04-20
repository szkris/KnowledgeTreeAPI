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

        // GET: api/Select/id/53e99784b7602d9701f3e130
        [HttpGet]
        [Route("api/Select/id/{id}")]
        public List<ODocument> Get(string id)
        {
            return Database.Select().From("Document").Where("aminer_id").Like(id).ToList();
        }
        
        // GET - TEST
        [HttpGet]
        [Route("api/Select/gettest")]
        public List<ODocument> Get()
        {
            List<ODocument> entries = Database.Select().From("Document").Limit(50).Where("year").Like(1965).ToList();
            return entries;
        }

        // GET: api/Select/5before/1000
        [HttpGet]
        [Route("api/Select/5before/{before}")]
        public List<ODocument> GetBeforeNum(int before)
        {
            List<ODocument> entries = Database.Select().From("Document").Skip(before).Limit(5).ToList();
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
