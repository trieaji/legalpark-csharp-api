using LegalPark.Data;
using LegalPark.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LegalPark.Repositories.ParkingTransaction
{
    // Implementasi spesifik untuk ParkingTransactionRepository
    public class ParkingTransactionRepository : GenericRepository<LegalPark.Models.Entities.ParkingTransaction>, IParkingTransactionRepository
    {
        public ParkingTransactionRepository(LegalParkDbContext context) : base(context) { }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> GetAllWithDetailsAsync()
        {
            return await _context.ParkingTransactions
                .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .OrderByDescending(pt => pt.EntryTime)
                .ToListAsync();
        }

        public async Task<LegalPark.Models.Entities.ParkingTransaction?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ParkingTransactions
                .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .Where(pt => pt.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<LegalPark.Models.Entities.ParkingTransaction?> UpdatePaymentStatusAsync(
    Guid transactionId,
    LegalPark.Models.Entities.PaymentStatus newPaymentStatus)
        {
            // Ambil data lengkap beserta relasi-relasi
            var transaction = await _context.ParkingTransactions
                .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .FirstOrDefaultAsync(pt => pt.Id == transactionId);

            if (transaction == null)
            {
                return null; // tidak ditemukan
            }

            // Update PaymentStatus
            transaction.PaymentStatus = newPaymentStatus;

            // Tandai entity untuk update
            _context.ParkingTransactions.Update(transaction);

            // Simpan ke database
            await _context.SaveChangesAsync();

            return transaction; // kembalikan entity lengkap yang sudah diupdate
        }



        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByStatus(LegalPark.Models.Entities.ParkingStatus status)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .OrderByDescending(pt => pt.EntryTime)
                                 .Where(pt => pt.Status == status)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByPaymentStatus(LegalPark.Models.Entities.PaymentStatus paymentStatus)
        {
            return await _context.ParkingTransactions
                .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .OrderByDescending(pt => pt.EntryTime)
                                 .Where(pt => pt.PaymentStatus == paymentStatus)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicle(LegalPark.Models.Entities.Vehicle vehicle)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .OrderByDescending(pt => pt.EntryTime)
                                 .Where(pt => pt.VehicleId == vehicle.Id)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByParkingSpot(LegalPark.Models.Entities.ParkingSpot parkingSpot)
        {
            return await _context.ParkingTransactions
                .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .OrderByDescending(pt => pt.EntryTime)
                                 .Where(pt => pt.ParkingSpotId == parkingSpot.Id)
                                 .ToListAsync();
        }

        public async Task<LegalPark.Models.Entities.ParkingTransaction?> findByVehicleAndStatus(LegalPark.Models.Entities.Vehicle vehicle, LegalPark.Models.Entities.ParkingStatus status)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.ParkingSpot)
                            .ThenInclude(ps => ps.Merchant) // penting
                         .Include(pt => pt.Vehicle)
                            .ThenInclude(v => v.Owner)      // supaya vehicle.Owner tidak null lagi
                         .Where(pt => pt.VehicleId == vehicle.Id && pt.Status == status)
                         .FirstOrDefaultAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicle_Id(Guid vehicleId)
        {
            return await _context.ParkingTransactions
                                 .Where(pt => pt.VehicleId == vehicleId)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByParkingSpot_Merchant(LegalPark.Models.Entities.Merchant merchant)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Include(pt => pt.ParkingSpot)
                    .ThenInclude(ps => ps.Merchant)
                .OrderByDescending(pt => pt.EntryTime)
                                 .Where(pt => pt.ParkingSpot.MerchantId == merchant.Id)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicleOwnerIdAndEntryTimeBetween(Guid userId, DateTime startDateTime, DateTime endDateTime)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.Vehicle)
                                 .ThenInclude(v => v.Owner)
                                 .Where(pt => pt.Vehicle.OwnerId == userId && pt.EntryTime >= startDateTime && pt.EntryTime <= endDateTime)
                                 .OrderByDescending(pt => pt.EntryTime)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findByVehicleOwnerId(Guid userId)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.Vehicle)
                                 .ThenInclude(v => v.Owner)
                                 .Where(pt => pt.Vehicle.OwnerId == userId)
                                 .OrderByDescending(pt => pt.EntryTime)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findPaidTransactionsByExitTimeBetween(DateTime startOfDay, DateTime endOfDay)
        {
            return await _context.ParkingTransactions
                                 .Where(pt => pt.PaymentStatus == LegalPark.Models.Entities.PaymentStatus.PAID && pt.ExitTime >= startOfDay && pt.ExitTime <= endOfDay)
                                 .OrderByDescending(pt => pt.ExitTime)
                                 .ToListAsync();
        }

        public async Task<List<LegalPark.Models.Entities.ParkingTransaction>> findPaidTransactionsByExitTimeBetweenAndMerchantCode(DateTime startOfDay, DateTime endOfDay, string merchantCode)
        {
            return await _context.ParkingTransactions
                                 .Include(pt => pt.ParkingSpot)
                                 .ThenInclude(ps => ps.Merchant)
                                 .Where(pt => pt.PaymentStatus == LegalPark.Models.Entities.PaymentStatus.PAID &&
                                              pt.ExitTime >= startOfDay && pt.ExitTime <= endOfDay &&
                                              pt.ParkingSpot.Merchant.MerchantCode == merchantCode)
                                 .OrderByDescending(pt => pt.ExitTime)
                                 .ToListAsync();
        }

        public async Task<LegalPark.Models.Entities.ParkingTransaction?> findByParkingSpotAndStatus(LegalPark.Models.Entities.ParkingSpot parkingSpot, LegalPark.Models.Entities.ParkingStatus status)
        {
            return await _context.ParkingTransactions
                                 .Where(pt => pt.ParkingSpotId == parkingSpot.Id && pt.Status == status)
                                 .FirstOrDefaultAsync();
        }
    }
}
