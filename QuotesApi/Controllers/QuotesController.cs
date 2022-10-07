using QuotesApi.Data;
using QuotesApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace QuotesApi.Controllers
{
    public class QuotesController : ApiController
    {

        QuotesDbContext context= new QuotesDbContext();
        // GET: api/Quotes
        [HttpGet]
        public IHttpActionResult Quotes(string sort)
        {
            sort = sort.ToLower();
            IQueryable<Quote> quotes;
            switch (sort)
            {
                case "asc":
                    quotes = context.Quotes.OrderBy(q => q.CreatedAt);
                        break;
                case "desc":
                    quotes = context.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;
                case null:
                    quotes = context.Quotes;
                    break;
                default: 
                    quotes= context.Quotes;
                    break;
            }
           
            if (quotes == null)
            {
                return NotFound();
            }
            return Ok(quotes);
        }

        [HttpGet]
        [Route("api/Quotes/PagingQuotes/{pageNumber=1}/{pageSize=5}")]
        public IHttpActionResult PagingQuote(int pageNumber, int pageSize)
        {
            IQueryable<Quote> pages;
            var quotes = context.Quotes.OrderBy(q=>q.Id);
           
            pages = quotes.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return Ok(pages);
        }

        // GET: api/Quotes/5
        [HttpGet]
        public IHttpActionResult Quote(int id)
        {
            var quote= context.Quotes.Find(id);
            if(quote == null)
            {
                return NotFound();
            }
            return Ok(quote);
        }

        //public async Task Quote(Quote quote)
        //{
        //     context.Quotes.Add(quote);

        //    await context.SaveChangesAsync();

        //}

        // POST: api/Quotes
        [HttpPost]
        public IHttpActionResult Post([FromBody]Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.Quotes.Add(quote);  
            context.SaveChanges();
            return StatusCode(HttpStatusCode.Created);
        }

        // PUT: api/Quotes/5
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody]Quote quote)
        {
            if((id<=0 || quote == null))
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }
            var quoteInDb = context.Quotes.Find(id);
            if (quoteInDb == null)
            {
                return BadRequest($"Can't perform update for {id}. {id} not found");
            }
            quoteInDb.Description = quote.Description;
            quoteInDb.Author=quote.Author;
            quoteInDb.Title=quote.Title;
            context.SaveChanges();

            return Ok("Update successful");

        }

        // DELETE: api/Quotes/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var quoteInDb=context.Quotes.Find(id);
            if(quoteInDb == null)
            {
                return BadRequest($"Quote with {id} not found in Db");
            }
            context.Quotes.Remove(quoteInDb);
            context.SaveChanges();

            return Ok();
        }
    }
}
