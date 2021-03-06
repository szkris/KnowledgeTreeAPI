﻿using Orient.Client;
using System;
using System.Collections.Generic;
using System.Web.Http;
using KnowledgeTreeAPI.Models.a_b_type;
using System.Net.Http;
using System.Net;

// With this controller the API can select records from the database by certain arguments (e.g. by keyword or by author name).

namespace KnowledgeTreeAPI.Controllers
{
    public class SelectController : ApiController
    {
        private static String DBName = "tos_citation_network";

        private OServer Server = Models.Server.Connect();
        private ODatabase Database = new ODatabase("localhost", 2424, DBName, ODatabaseType.Graph, "admin", "admin");

        // GET - TEST
        [HttpGet]
        [Route("api/Select/gettest")]
        public long Get()
        {
            return Database.CountRecords;
        }

        // GET: api/Select/keyword/Mycoplasma hominis
        [HttpGet]
        [Route("api/Select/keyword/{keyword}")]
        public List<ODocument> GetByKeyword(String keyword)
        {
            List<ODocument> keyword_results = TraverseByName("keyword", keyword);
            keyword_results.RemoveAt(0);
            List<String> documentRefs = AddIds(keyword_results);
            List<ODocument> results = Database.Select().From("document_data").Where("id").In<String>(documentRefs).ToList();
            Database.Close();
            return results;
        }


        // GET: api/Select/author/Smith
        [HttpGet]
        [Route("api/Select/author/{authorName}")]
        public List<ODocument> GetByAuthor(String authorName)
        {
            List<ODocument> authorResults = TraverseByName("author", authorName);
            authorResults.RemoveAt(0);
            List<String> documentRefs = AddIds(authorResults);
            List<ODocument> results = Database.Select().From("document_data").Where("id").In<String>(documentRefs).ToList();
            Database.Close();
            return results;
        }

        // GET : api/Select/fos/...
        [HttpGet]
        [Route("api/Select/fos/{fos}")]
        public List<ODocument> GetByFos(String fos)
        {
            List<ODocument> fosResult = TraverseByName("fos", fos);
            fosResult.RemoveAt(0);
            List<String> documentRefs = AddIds(fosResult);
            List<ODocument> results = Database.Select().From("document_data").Where("id").In<String>(documentRefs).ToList();
            Database.Close();
            return results;
        }

        // GET: api/Select/citation/...
        [HttpGet]
        [Route("api/Select/citation/{citation}")]
        public List<ODocument> GetByCitation(String citation)
        {
            List<ODocument> citationResult = TraverseByName("citation", citation);
            citationResult.RemoveAt(0);
            List<String> documentRefs = AddIds(citationResult);
            List<ODocument> results = Database.Select().From("document_data").Where("id").In<String>(documentRefs).ToList();
            Database.Close();
            return results;
        }

        // Szelekció úgy, hogy nem kell tudnunk, hogy amire keresünk az keyword, vagy author, vagy más...
        // GET: api/Select/any/Smith ||  api/Select/any/security
        [HttpGet]
        [Route("api/Select/any/{anything}")]
        public List<ODocument> GetByAnything(String anything)
        {
            List<ODocument> result = TraverseByName("b_type_node", anything);
            result.RemoveAt(0);
            List<String> documentRefs = AddIds(result);
            List<ODocument> results = Database.Select().From("document_data").Where("id").In<String>(documentRefs).ToList();
            Database.Close();
            return results;
        }

        // GET: api/Select/id/53e99784b7602d9701f3e482
        [HttpGet]
        [Route("api/Select/id/{id}")]
        public HttpResponseMessage GetById(String id)
        {
            try
            {
                List<ODocument> results = Database.Select().From("document_data").Where("id").Equals<String>(id).ToList();
                Database.Close();
                if(results.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK, results);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // GET: api/Select/documentreference/fromcomponent/5
        [HttpGet]
        [Route("api/Select/documentreference/fromcomponent/{componentNumber}")]
        public List<ODocument> GetDocumentReferenceFromComponent(int componentNumber)
        {
            List<ODocument> results = Database.Select().From("document_reference").Where("component").Equals<int>(componentNumber).ToList();
            Database.Close();
            return results;
        }

        // GET: api/Select/documentdata/fromcomponent/5
        [HttpGet]
        [Route("api/Select/documentdata/fromcomponent/{componentNumber}")]
        public List<ODocument> GetDocumentDataFromComponent(int componentNumber)
        {
            List<ODocument> componentResult = Database.Select().From("document_reference").Where("component").Equals<int>(componentNumber).ToList();
            //componentResult.RemoveAt(0);
            List<String> documentRefs = AddIds(componentResult);
            List<ODocument> results = Database.Select().From("document_data").Where("id").In<String>(documentRefs).ToList();
            Database.Close();
            return results;
        }



        public List<ODocument> TraverseByName(String name, String attributeName)
        {
            return Database.Command(String.Format("traverse out() from (select from {0} where name = '{1}') maxdepth 1", name, attributeName)).ToList();
        }

        public List<String> AddIds(List<ODocument> results) {
            List<String> documentRefs = new List<String>();
            foreach (var result in results)
            {
                Document_reference actual = new Document_reference();
                documentRefs.Add(result.GetField<string>("id"));
            }
            return documentRefs;
        }


        /*
        // GET: api/Select/5before/1000
        [HttpGet]
        [Route("api/Select/5before/{before}")]
        public List<ODocument> GetBeforeNum(int before)
        {
            List<ODocument> entries = Database.Select().From("Document").Skip(before).Limit(5).ToList();
            return entries;
        }
        */

        // POST: api/Select
        //public QueryResult Post([FromBody]QueryData data)
        //{
        //    var alreadyWhere = false;
        //    var sel = Database.Select().From("...");

        //    foreach (var prop in typeof(SelectClause).GetProperties())
        //    {
        //        // FIXME
        //        sel = sel.WhereAndEquals(prop.Name, prop.GetValue(data.SelectClause).ToString(), ref alreadyWhere);
        //    }

        //    /*
        //    var select = db.Select().From("")
        //        .WhereAndEquals("id", data.SelectClause.Id.ToString(), ref alreadyWhere)
        //        .WhereAndEquals("", "", ref alreadyWhere);
        //    */

        //    return new QueryResult
        //    {

        //    };
        //}
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