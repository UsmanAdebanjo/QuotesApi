using Microsoft.AspNet.Identity;
using QuoteWebApiAuth.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace QuoteWebApiAuth.Controllers
{
    [Authorize]
    public class QuotesController : ApiController
    {

        ApplicationDbContext context= new ApplicationDbContext();


        // GET: api/Quotes
        [CacheOutput(ClientTimeSpan = 160)]
        [AllowAnonymous]
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


        [HttpGet]
        [Route("api/Quotes/SearchQuote/{search=}")]
        public IHttpActionResult SearchQuote(string search)
        {
            search = search.ToLower();
            var searchedQuote = context.Quotes.Where(q => q.Type.Contains(search));
            return Ok(searchedQuote);
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

        [HttpGet]
        public IHttpActionResult MyQuotes()
        {
            string userId=User.Identity.GetUserId();

            var myQuotes = context.Quotes.Where(q=>q.UserId==userId);
            return Ok(myQuotes);
        }

        // POST: api/Quotes
        [HttpPost]
        public IHttpActionResult Post([FromBody]Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.Identity.GetUserId();
            quote.UserId = userId;

                context.Quotes.Add(quote);
                context.SaveChanges();
                
            
            return StatusCode(HttpStatusCode.Created);
          
        }

        // PUT: api/Quotes/5
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody]Quote quote)
        {
            string userId = User.Identity.GetUserId();  

            var quoteInDb = context.Quotes.Find(id);
            if (quoteInDb == null)
            {
                return BadRequest($"Can't perform update, Id {id} not found");
            }
            if (quoteInDb.UserId == userId)
            {
                quoteInDb.Description = quote.Description;
                quoteInDb.Author = quote.Author;
                quoteInDb.Title = quote.Title;
                context.SaveChanges();

                return Ok("Update successful");
            }
            else
            {
                return BadRequest("You are not authorised to modify this data");
            }

        }

        // DELETE: api/Quotes/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            string userId=User.Identity.GetUserId();
            var quoteInDb=context.Quotes.Find(id);
            if(quoteInDb == null)
            {
                return BadRequest($"Quote with {id} not found in Db");
            }
            if(quoteInDb.UserId == userId)
            {
                context.Quotes.Remove(quoteInDb);
                context.SaveChanges();
                return Ok();
            }

            else
            {
                return BadRequest("You don't have acces to delete this data");
            }

        }
    }
}
