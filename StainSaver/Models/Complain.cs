using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StainSaver.Models
{
    public class Complain
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Complain ID")]
        public int ComplainId { get; set; }

        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        [Display(Name = "Customer")]
        public ApplicationUser Customer { get; set; }

        public int BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }

        [Display(Name = "Complain Type")]
        public ComplainType ComplainType { get; set; }

        [Display(Name = "Bank Name")]
        public Bank? Bank { get; set; }

        [Display(Name = "Bank Account Type")]
        public BankAccountType? BankAccountType { get; set; }

        [Display(Name = "Bank Account Number")]
        public string? BankAccountNumber { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }



        [Display(Name = "Proof of Payment")]
        public string? ProofOfPayment { get; set; }

        [Display(Name = "Is Lost")]
        public bool IsLost { get; set; }

        [Display(Name = "Is Found")]
        public bool IsFound { get; set; }

        [Display(Name = "Lost or Found Date")]
        public DateTime? LostOrFoundDate { get; set; }

        [Display(Name = "Complain Status")]
        public ComplainStatus Status { get; set; }

        [Display(Name = "Comment")]
        public List<string>? Comments { get; set; } = new();

        [Display(Name = "Reason For Refund")]
        public RefundTo? ReasonForRefund { get; set; }

        public string ReferenceNumber { get; set; }

        public ICollection<LostOrFoundItem> LostOrFoundItems { get; set; } = new List<LostOrFoundItem>();

        public ICollection<RefundItem> RefundItems { get; set; } = new List<RefundItem>();
    }

    public enum ComplainType
    {
        [Display(Name = "Refund")]
        Refund,

        [Display(Name = "Lost and Found")]
        Lost_and_found
    }

    public enum BankAccountType
    {
        [Display(Name = "Current Account")]
        Current,

        [Display(Name = "Savings Account")]
        Savings,

        [Display(Name = "Fixed Deposit Account")]
        FixedDeposit,

        [Display(Name = "Money Market Account")]
        MoneyMarket
    }

    public enum RefundTo
    {
        [Display(Name = "Lost Item")]
        LostItem,

        [Display(Name = "Damaged Item")]
        DamagedItem,

        [Display(Name = "Wrong Item Returned")]
        WrongItemReturned,

        [Display(Name = "Missing Item")]
        MissingItem,

        [Display(Name = "Late Delivery")]
        LateDelivery,

        [Display(Name = "Poor Cleaning")]
        PoorCleaning,

        [Display(Name = "Stain Not Removed")]
        StainNotRemoved,

        [Display(Name = "Wrong Service Performed")]
        WrongServicePerformed,

        [Display(Name = "Overcharge")]
        Overcharge,

        [Display(Name = "Duplicate Charge")]
        DuplicateCharge,

        [Display(Name = "Cancelled Order Refund")]
        CancelledOrderRefund,

        [Display(Name = "Payment Error")]
        PaymentError,

        [Display(Name = "Customer Dissatisfaction")]
        CustomerDissatisfaction
    }

    public enum ComplainStatus
    { 
      Review,
      Approved,
      Rejected,
      DriverAssigned,
      Returned,
      Collected,
      Refunded,
      AwaitingCustomer,
      Completed
    }

    public enum Bank
    {
        [Display(Name = "Absa Bank")]
        AbsaBank,

        [Display(Name = "African Bank")]
        AfricanBank,

        [Display(Name = "Bidvest Bank")]
        BidvestBank,

        [Display(Name = "Capitec Bank")]
        CapitecBank,

        [Display(Name = "Discovery Bank")]
        DiscoveryBank,

        [Display(Name = "First National Bank (FNB)")]
        FirstNationalBank,

        [Display(Name = "Investec Bank")]
        InvestecBank,

        [Display(Name = "Nedbank")]
        Nedbank,

        [Display(Name = "Standard Bank")]
        StandardBank,

        [Display(Name = "TymeBank")]
        TymeBank,

        [Display(Name = "Sasfin Bank")]
        SasfinBank,

        [Display(Name = "OM Bank")]
        OmBank,

        [Display(Name = "Ubank")]
        Ubank
    }

}
