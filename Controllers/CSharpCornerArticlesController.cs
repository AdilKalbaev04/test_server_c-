using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSharpCornerApi.Data;
using CSharpCornerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[Route("API/[controller]")]
[ApiController]
public class CSharpCornerArticlesController: ControllerBase
{
    private readonly AppDbContext _context;

    public CSharpCornerArticlesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CSharpCornerArticle>>> GetArticles()
    {
        return await _context.Articles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CSharpCornerArticle>> GetArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        return article;
    }

    [HttpPost]
    public async Task<ActionResult<CSharpCornerArticle>> PostArticle(CSharpCornerArticle article)
    {
        article.CreatedAt = DateTime.UtcNow; 
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetArticle", new { id = article.Id }, article);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutArticle(int id, CSharpCornerArticle article)
    {
        if (id != article.Id)
        {
            return BadRequest();
        }

        _context.Entry(article).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ArticleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ArticleExists(int id)
    {
        return _context.Articles.Any(e => e.Id == id);
    }
}