using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Orient.Client;
using KnowledgeTreeAPI.Models;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using KnowledgeTreeAPI.Models.a_b_type;
using DBUpload;

// With this controller the API enables the user to insert records into the database.

namespace KnowledgeTreeAPI.Controllers
{
    public class InsertController : ApiController
    {

        private static string DBName = "tos_citation_network";
        // PUT: api/Insert
        public HttpResponseMessage PutRecord(HttpRequestMessage request)
        {
            //connecting to the db
            OServer Server = Models.Server.Connect();
            ODatabase Database = new ODatabase("localhost", 2424, DBName, ODatabaseType.Graph, "admin", "admin");
            //creating auxiliary variables
            Query query = new Query();

            // Get JSON from body.
            var value = request.Content.ReadAsStringAsync().Result;

            string source = "tos";     
            //doesn't - mag
            //initializing string builder for batch
            string batch;
            StringBuilder builder = new StringBuilder("BEGIN;\n");

            //data objects for transaction
            List<string> aux_authors = new List<string>();
            List<string> aux_fos = new List<string>();
            List<string> aux_keyword = new List<string>();
            List<string> aux_references = new List<string>();

            //checking exception
            try
            {
                //deserialization
                Vertex v = JsonConvert.DeserializeObject<Vertex>(value);
                //parsing it:
                //document_reference
                Document_reference document_reference = new Document_reference();
                document_reference.id = v.id;
                document_reference.source = source;
                document_reference.status = "0";
                document_reference.component = 0;
                //document_data
                Document_data document_data = new Document_data();
                document_data.doc_type = v.doc_type;
                document_data.id = v.id;
                document_data.pdf = v.pdf;
                document_data.title = v.title;
                document_data.url = v.url;
                document_data.venue = v.venue;
                document_data.year = v.year;
                //checking if data already exists
                Document_reference result = query.Select_id(document_reference.id);
                //if exists -> update with data (was uploaded from reference, doesn't contain data)
                if (result != null)
                {
                    builder.Append("INSERT INTO document_data CONTENT ");
                    builder.Append(JsonConvert.SerializeObject(document_data));
                    builder.Append(";\n");
                }
                //if not -> create it with the data given
                else
                {
                    //creating command
                    //inserting document reference:
                    if (!aux_references.Contains(document_reference.id))
                    {
                        aux_references.Add(document_reference.id);
                        builder.Append("INSERT INTO document_reference CONTENT ");
                        builder.Append(JsonConvert.SerializeObject(document_reference));
                        builder.Append(";\n");
                    }
                    //inserting document data:
                    builder.Append("INSERT INTO document_data CONTENT ");
                    builder.Append(JsonConvert.SerializeObject(document_data));
                    builder.Append(";\n");
                }
                //uploading references
                if (v.references != null) foreach (string refer in v.references)
                {
                    if (refer != null)
                    {
                        if (!aux_references.Contains(refer))
                        {
                            Document_reference refered_document = query.Select_id(refer);
                            //if missing -> create it
                            if (refered_document == null)
                            {
                                aux_references.Add(refer);
                                //document_reference
                                Document_reference document_reference_ref = new Document_reference();
                                document_reference_ref.id = refer;
                                document_reference_ref.source = source;
                                document_reference_ref.status = "0";
                                document_reference_ref.component = 0;
                                //creating command
                                builder.Append("INSERT INTO document_reference CONTENT ");
                                builder.Append(JsonConvert.SerializeObject(document_reference_ref));
                                builder.Append(";\n");
                            }
                        }
                        //link it
                        builder.Append(String.Format("CREATE EDGE reference FROM (SELECT FROM document_reference WHERE id = {0}) TO (SELECT FROM document_reference WHERE id = {1});\n"
                                                    , query.Serializer(refer), query.Serializer(document_reference.id)));
                    }
                }
                //processing keywords
                if (v.keywords != null) foreach (string keyword in v.keywords)
                {
                    if (keyword != null)
                    {
                        //check if it's already part of the transaction
                        if (!aux_keyword.Contains(keyword))
                        {
                            //if missing -> create it
                            Keyword actual_keyword = query.Select_keyword_name(keyword);
                            if (actual_keyword == null)
                            {
                                aux_keyword.Add(keyword);
                                Keyword k = new Keyword();
                                k.name = keyword;
                                builder.Append("INSERT INTO keyword CONTENT {\"name\": ");
                                builder.Append(JsonConvert.SerializeObject(keyword));
                                builder.Append("};\n");
                            }
                        }
                        //link it
                        builder.Append(String.Format("CREATE EDGE using_keyword FROM (SELECT FROM keyword WHERE name = {0}) TO (SELECT FROM document_reference WHERE id = {1});\n"
                                        , query.Serializer(keyword), query.Serializer(document_reference.id)));
                    }
                }
                //processing fos
                if (v.fos != null) foreach (string fos in v.fos)
                {
                    if (fos != null)
                    {
                        if (!aux_fos.Contains(fos))
                        {
                            //if missing -> create it
                            Fos actual_fos = query.Select_fos_name(fos);
                            if (actual_fos == null)
                            {
                                aux_fos.Add(fos);
                                Fos f = new Fos();
                                f.name = fos;
                                builder.Append("INSERT INTO fos CONTENT {\"name\": ");
                                builder.Append(JsonConvert.SerializeObject(f));
                                builder.Append(";\n");
                            }
                            //link it
                            builder.Append(String.Format("CREATE EDGE using_fos FROM (SELECT FROM fos WHERE name = {0}) TO (SELECT FROM document_reference WHERE id = {1});\n"
                                            , query.Serializer(fos), query.Serializer(document_reference.id)));
                        }
                    }
                }
                //processing authors
                if (v.authors != null) foreach (AuthorData author in v.authors)
                {
                    if (author.name != null)
                    {
                        if (!aux_authors.Contains(author.name))
                        {
                            //if missing -> create it
                            AuthorData actual_author = query.Select_author_name(author.name);
                            if (actual_author == null)
                            {
                                aux_authors.Add(author.name);
                                AuthorData a = new AuthorData();
                                a.name = author.name;
                                a.org = author.org;
                                builder.Append("INSERT INTO author CONTENT ");
                                builder.Append(JsonConvert.SerializeObject(a));
                                builder.Append(";\n");
                            }
                            //link it
                            builder.Append(String.Format("CREATE EDGE written_by_author FROM (SELECT FROM author WHERE name = {0}) TO (SELECT FROM document_reference WHERE id = {1});\n"
                                            , query.Serializer(author.name), query.Serializer(document_reference.id)));
                        }
                    }
                }
                //adding a commit if limit reached
                builder.Append("COMMIT RETRY 100;");
                batch = builder.ToString();
                Database.SqlBatch(batch).Run();
                builder.Append("BEGIN;\n");
            }
            catch (System.IO.IOException e)
            {
                return new HttpResponseMessage(HttpStatusCode.GatewayTimeout);
            }
            Database.Close();
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
