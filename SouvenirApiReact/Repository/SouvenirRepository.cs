using Microsoft.EntityFrameworkCore;
using SouvenirApiReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouvenirApiReact.Repository
{
    public class SouvenirRepository : ISouvenirRepository
    {
        private readonly souvenirDbContext _context;
        public SouvenirRepository(souvenirDbContext context)
        {
            _context = context;
        }
        public async Task<Souvenir> Create(Souvenir souvenir)
        {
            _context.Souvenirs.Add(souvenir);
            await _context.SaveChangesAsync();
            return souvenir;
        }

        public async Task Delete(int id)
        {
            var data = await _context.Souvenirs.FindAsync(id);
            _context.Souvenirs.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Souvenir>> findAll(int min, int max)
        {
            return await _context.Souvenirs.Where(b => b.SouvenirPrice >= min && b.SouvenirPrice <= max).ToListAsync();
        }

        public async Task<Souvenir> Get(int id) 
        {
            return await _context.Souvenirs.FindAsync(id);
        }

        public async Task<IEnumerable<Souvenir>> Gets()
        {
            return await _context.Souvenirs.ToListAsync();
        }

        public async Task Update(Souvenir souvenir)
        {
            _context.Entry(souvenir).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
