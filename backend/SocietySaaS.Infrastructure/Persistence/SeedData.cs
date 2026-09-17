using Microsoft.EntityFrameworkCore;
using SocietySaaS.Domain.Entities;
using SocietySaaS.Infrastructure.Services;

namespace SocietySaaS.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var superAdmin = new User
        {
            Id = Guid.NewGuid(),
            Email = "superadmin@societypro.com",
            FirstName = "Super",
            LastName = "Admin",
            PasswordHash = JwtTokenService.HashPassword("SuperAdmin@123"),
            IsActive = true,
            IsSuperAdmin = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(superAdmin);

        var demoTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Sunshine Residency",
            Address = "123 MG Road",
            City = "Mumbai",
            State = "Maharashtra",
            PinCode = "400001",
            Phone = "9876543210",
            Email = "admin@sunshineresidency.com",
            Slug = "sunshineresidency",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Tenants.Add(demoTenant);

        var societyAdmin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@sunshineresidency.com",
            FirstName = "Rajesh",
            LastName = "Kumar",
            PasswordHash = JwtTokenService.HashPassword("Admin@123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(societyAdmin);

        db.UserTenants.Add(new UserTenant
        {
            Id = Guid.NewGuid(),
            UserId = societyAdmin.Id,
            TenantId = demoTenant.Id,
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var charges = new[]
        {
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Maintenance", CalculationType = "Fixed", Amount = 3000, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Sinking Fund", CalculationType = "Fixed", Amount = 500, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Water Charges", CalculationType = "Fixed", Amount = 400, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Parking", CalculationType = "Fixed", Amount = 1000, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Charge { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Electricity Common Area", CalculationType = "Fixed", Amount = 600, IsRecurring = true, IsActive = true, CreatedAt = DateTime.UtcNow },
        };
        db.Charges.AddRange(charges);

        var wingA = new Wing { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Wing A", TotalFloors = 10, FlatsPerFloor = 4, IsActive = true, CreatedAt = DateTime.UtcNow };
        var wingB = new Wing { Id = Guid.NewGuid(), TenantId = demoTenant.Id, Name = "Wing B", TotalFloors = 10, FlatsPerFloor = 4, IsActive = true, CreatedAt = DateTime.UtcNow };
        db.Wings.AddRange(wingA, wingB);

        var flatNumbers = new[] { "101", "102", "103", "104", "201", "202", "203", "204", "301", "302", "303", "304", "401", "402", "403", "404", "501", "502", "503", "504" };
        var names = new[] { "Amit Sharma", "Priya Patel", "Vikram Singh", "Neha Gupta", "Rahul Verma", "Anjali Desai", "Sanjay Mehta", "Pooja Reddy", "Arun Nair", "Deepa Iyer", "Suresh Pillai", "Kavita Joshi", "Manoj Tiwari", "Sunita Rao", "Vivek Choudhary", "Meena Bhat", "Ravi Shankar", "Lakshmi Menon", "Kiran Bhatt", "Geeta Pandey" };
        var mobiles = new[] { "9876543210", "9876543211", "9876543212", "9876543213", "9876543214", "9876543215", "9876543216", "9876543217", "9876543218", "9876543219", "9876543220", "9876543221", "9876543222", "9876543223", "9876543224", "9876543225", "9876543226", "9876543227", "9876543228", "9876543229" };
        var paymentModes = new[] { "UPI", "BankTransfer", "Cash", "Cheque", "Online" };

        var allBillIds = new List<Guid>();

        for (int i = 0; i < flatNumbers.Length; i++)
        {
            var wing = i < 10 ? wingA : wingB;
            var floor = int.Parse(flatNumbers[i][0].ToString());
            var flatId = Guid.NewGuid();

            var flat = new Flat
            {
                Id = flatId,
                TenantId = demoTenant.Id,
                FlatNumber = flatNumbers[i],
                Floor = floor,
                CarpetArea = 800 + (i % 5) * 100,
                BuiltUpArea = 1000 + (i % 5) * 120,
                FlatType = i % 3 == 0 ? "2BHK" : "3BHK",
                OccupancyStatus = i == 18 || i == 19 ? "Vacant" : "Owner",
                WingId = wing.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Flats.Add(flat);

            var member = new Member
            {
                Id = Guid.NewGuid(),
                TenantId = demoTenant.Id,
                FlatId = flatId,
                FirstName = names[i].Split(' ')[0],
                LastName = names[i].Split(' ')[1],
                Mobile = mobiles[i],
                Email = $"{names[i].Split(' ')[0].ToLower()}@email.com",
                MemberType = "Owner",
                IsPrimary = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Members.Add(member);

            db.OpeningBalances.Add(new OpeningBalance
            {
                Id = Guid.NewGuid(),
                TenantId = demoTenant.Id,
                FlatId = flatId,
                Amount = i < 5 ? 0 : -(500 + i * 200),
                BalanceType = "Opening",
                AsOfDate = DateTime.UtcNow.AddMonths(-6),
                CreatedAt = DateTime.UtcNow
            });

            for (int m = 0; m < 3; m++)
            {
                var billingPeriod = DateTime.UtcNow.AddMonths(-m - 1).ToString("yyyy-MM");
                var billId = Guid.NewGuid();
                var billAmount = charges.Sum(c => c.Amount);
                var isPaid = m == 1 || m == 2;
                var paidAmount = isPaid ? billAmount : (i % 3 == 0 ? billAmount * 0.5m : 0);

                db.Bills.Add(new Bill
                {
                    Id = billId,
                    TenantId = demoTenant.Id,
                    FlatId = flatId,
                    BillNumber = $"BILL-{billingPeriod}-{flatNumbers[i]}",
                    BillingPeriod = billingPeriod,
                    BillDate = new DateTime(int.Parse(billingPeriod.Split('-')[0]), int.Parse(billingPeriod.Split('-')[1]), 1),
                    DueDate = new DateTime(int.Parse(billingPeriod.Split('-')[0]), int.Parse(billingPeriod.Split('-')[1]), 15).AddMonths(1),
                    PreviousOutstanding = 0,
                    CurrentCharges = billAmount,
                    GrandTotal = billAmount,
                    BalanceOutstanding = billAmount - paidAmount,
                    AmountPaid = paidAmount,
                    Status = paidAmount >= billAmount ? "Paid" : paidAmount > 0 ? "Partial" : "Pending",
                    CreatedAt = DateTime.UtcNow
                });

                if (isPaid || paidAmount > 0)
                {
                    allBillIds.Add(billId);
                    var paymentId = Guid.NewGuid();
                    var paymentDate = new DateTime(int.Parse(billingPeriod.Split('-')[0]), int.Parse(billingPeriod.Split('-')[1]), 10 + i % 15);

                    db.Payments.Add(new Payment
                    {
                        Id = paymentId,
                        TenantId = demoTenant.Id,
                        FlatId = flatId,
                        PaymentNumber = $"PAY-{billingPeriod}-{flatNumbers[i]}",
                        Amount = paidAmount,
                        PaymentDate = paymentDate,
                        PaymentMode = paymentModes[i % paymentModes.Length],
                        TransactionReference = $"REF-{i}-{m}",
                        Status = "Completed",
                        CreatedAt = DateTime.UtcNow
                    });

                    db.Receipts.Add(new Receipt
                    {
                        Id = Guid.NewGuid(),
                        TenantId = demoTenant.Id,
                        FlatId = flatId,
                        PaymentId = paymentId,
                        ReceiptNumber = $"RCT-{billingPeriod}-{flatNumbers[i]}",
                        Amount = paidAmount,
                        ReceiptDate = paymentDate,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        db.LatePaymentRules.Add(new LatePaymentRule
        {
            Id = Guid.NewGuid(),
            TenantId = demoTenant.Id,
            GracePeriodDays = 15,
            Percentage = 2,
            MaximumFine = 500,
            IsEnabled = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
}
