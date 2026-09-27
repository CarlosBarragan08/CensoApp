using SQLite;
using CensoApp.Models;

namespace CensoApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _db;

        private async Task Init()
        {
            if (_db != null)
                return;

            SQLitePCL.Batteries_V2.Init();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "censo_ivc.db3");
            _db = new SQLiteAsyncConnection(dbPath);

            await _db.CreateTableAsync<Establecimiento>();
            await _db.CreateTableAsync<Visita>();
        }

        // --- ESTABLECIMIENTOS
        public async Task<List<Establecimiento>> ObtenerEstablecimientosAsync()
        {
            await Init();
            return await _db!.Table<Establecimiento>().ToListAsync();
        }

        public async Task<int> GuardarEstablecimientoAsync(Establecimiento establecimiento)
        {
            await Init();
            if (establecimiento.Id != 0)
                return await _db!.UpdateAsync(establecimiento);
            else
                return await _db!.InsertAsync(establecimiento);
        }

        public async Task<int> EliminarEstablecimientoAsync(Establecimiento establecimiento)
        {
            await Init();
            var visitas = await ObtenerVisitasPorEstablecimientoAsync(establecimiento.Id);
            foreach (var v in visitas)
            {
                await _db!.DeleteAsync(v);
            }
            return await _db!.DeleteAsync(establecimiento);
        }

        // --- VISITAS
        public async Task<List<Visita>> ObtenerTodasLasVisitasAsync()
        {
            await Init();
            return await _db!.Table<Visita>().ToListAsync();
        }

        public async Task<List<Visita>> ObtenerVisitasPorEstablecimientoAsync(int establecimientoId)
        {
            await Init();
            return await _db!.Table<Visita>()
                             .Where(v => v.EstablecimientoId == establecimientoId)
                             .OrderByDescending(v => v.NumeroVisita)
                             .ToListAsync();
        }

        public async Task GuardarVisitaAsync(Visita visita)
        {
            await Init();
            if (visita.Id != 0)
                await _db!.UpdateAsync(visita);
            else
                await _db!.InsertAsync(visita);

            var est = await _db!.Table<Establecimiento>()
                                .FirstOrDefaultAsync(e => e.Id == visita.EstablecimientoId);
            if (est != null)
            {
                est.ConceptoSanitario = visita.ConceptoSanitario;
                est.PorcentajeCumplimiento = visita.PorcentajeCumplimiento;
                await _db!.UpdateAsync(est);
            }
        }

        public async Task GuardarVisitaAsync(Visita visita, Establecimiento establecimiento)
        {
            if (establecimiento != null)
            {
                visita.EstablecimientoId = establecimiento.Id;
                establecimiento.ConceptoSanitario = visita.ConceptoSanitario;
                establecimiento.PorcentajeCumplimiento = visita.PorcentajeCumplimiento;
                await GuardarEstablecimientoAsync(establecimiento);
            }
            await GuardarVisitaAsync(visita);
        }
    }
}