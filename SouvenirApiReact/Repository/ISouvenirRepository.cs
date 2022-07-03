using SouvenirApiReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SouvenirApiReact.Repository
{
  public  interface ISouvenirRepository
    {
        Task<IEnumerable<Souvenir>> Gets();
        Task<Souvenir> Get(int id);
        Task<Souvenir> Create(Souvenir souvenir);
        Task Update(Souvenir souvenir);
        Task Delete(int id);
        Task<IEnumerable<Souvenir>> findAll(int min, int max);
    }
}
