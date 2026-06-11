using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryService.WebAPI.Data;
using LibraryService.WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.WebAPI.Services
{
    public interface IFraudService
    {
        /// <summary>Obtiene todos los reportes de fraude, del mas reciente al mas antiguo.</summary>
        Task<IEnumerable<Fraud>> GetAll();

        /// <summary>Inserta un nuevo reporte de fraude.</summary>
        Task<Fraud> Add(Fraud fraud);
    }

    public class FraudService : IFraudService
    {
        private readonly LibraryContext _context;

        public FraudService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fraud>> GetAll()
        {
            return await _context.Frauds
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Fraud> Add(Fraud fraud)
        {
            fraud.CreatedAt = DateTime.UtcNow;

            await _context.Frauds.AddAsync(fraud);
            await _context.SaveChangesAsync();

            return fraud;
        }
    }
}
