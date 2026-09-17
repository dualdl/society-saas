namespace SocietySaaS.Shared;

public static class Constants
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string SocietyAdmin = "SocietyAdmin";
        public const string Accountant = "Accountant";
        public const string CommitteeMember = "CommitteeMember";
        public const string Auditor = "Auditor";
        public const string Resident = "Resident";
    }

    public static class BillStatus
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Partial = "Partial";
        public const string Cancelled = "Cancelled";
        public const string Overdue = "Overdue";
    }

    public static class PaymentModes
    {
        public const string UPI = "UPI";
        public const string NEFT = "NEFT";
        public const string RTGS = "RTGS";
        public const string IMPS = "IMPS";
        public const string Cheque = "Cheque";
        public const string Cash = "Cash";
        public const string BankTransfer = "BankTransfer";
        public const string Other = "Other";
    }

    public static class CalculationType
    {
        public const string Fixed = "Fixed";
        public const string PerFlat = "PerFlat";
        public const string PerArea = "PerArea";
        public const string Percentage = "Percentage";
        public const string Manual = "Manual";
    }

    public static class OccupancyStatus
    {
        public const string Owner = "Owner";
        public const string Rented = "Rented";
        public const string Vacant = "Vacant";
    }

    public static class BalanceType
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";
    }

    public static class ImportStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }

    public static class EmailStatus
    {
        public const string Pending = "Pending";
        public const string Sent = "Sent";
        public const string Failed = "Failed";
    }
}
