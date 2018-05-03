using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Orient.Client;
using KnowledgeTreeAPI.Models;
using Newtonsoft.Json;

// With this controller the API enables the user to insert records into the database.

namespace KnowledgeTreeAPI.Controllers
{
    public class InsertController : ApiController
    {
        //public IEnumerable<string> Get()
        //{
        //    OServer Server = Models.Server.Connect();
        //    ODatabase db = new ODatabase("localhost", 2424, "TOSDB", ODatabaseType.Graph, "admin", "admin");

        //    string s = db.CountRecords.ToString();

        //    return new string[] { s };
        //}

        // PUT: api/Insert
        //public void Put (HttpRequestMessage request)
        //{
        //    var value = request.Content.ReadAsStringAsync().Result;

        //    OServer Server = Models.Server.Connect();
        //    ODatabase Database = new ODatabase("localhost", 2424, "TreeOfScience", ODatabaseType.Graph, "admin", "admin");

        //    Vertex Vertex;
        //    AminerVertex a = new AminerVertex { };

        //    Vertex = JsonConvert.DeserializeObject<Vertex>(value);

        //    if (Vertex.authors != null) for (int currentAuthor = 0; currentAuthor < Vertex.authors.Length; currentAuthor++)
        //    {
        //        Vertex.authors[currentAuthor].@class = "Author";
        //        Vertex.authors[currentAuthor].@type = "d";
        //    }
           

        //    a.aminer_id = Vertex.id;
        //    a.title = Vertex.title;
        //    a.authors = Vertex.authors;
        //    a.year = Vertex.year;
        //    a.venue = Vertex.venue;
        //    a.keywords = Vertex.keywords;
        //    a.fos = Vertex.fos;
        //    a.n_citation = Vertex.n_citation;
        //    a.references = Vertex.references;
        //    a.page_stat = Vertex.page_stat;
        //    a.page_end = Vertex.page_end;
        //    a.doc_type = Vertex.doc_type;
        //    a.lang = Vertex.lang;
        //    a.publisher = Vertex.publisher;
        //    a.volume = Vertex.volume;
        //    a.issue = Vertex.issue;
        //    a.issn = Vertex.issn;
        //    a.isbn = Vertex.isbn;
        //    a.doi = Vertex.doi;
        //    a.pdf = Vertex.pdf;
        //    a.url = Vertex.url;
        //    a.Abstract = Vertex.Abstract;
        //    value = JsonConvert.SerializeObject(a).Replace("Abstract", "abstract").Replace("type", "@type").Replace("class", "@class").Replace("doc_@type", "doc_type");
        //    Database.Command("INSERT INTO Document CONTENT " + value);
        //}
    }
}
